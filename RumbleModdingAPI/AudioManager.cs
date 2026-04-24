using Il2CppRUMBLE.Audio;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Pools;
using UnityEngine;
using static Il2CppRUMBLE.Audio.AudioCall;

namespace RumbleModdingAPI.RMAPI
{
    /// <summary>
    /// Audio Manager to Create AudioCalls
    /// </summary>
    public static class AudioManager
    {
        /// <summary>
        /// CreateAudioCalls Triggers this but leaving this visible for others if they want it
        /// </summary>
        public static AudioClip LoadWavFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            byte[] fileBytes = File.ReadAllBytes(filePath);
            return LoadWavFromBytes(fileBytes, Path.GetFileNameWithoutExtension(filePath));
        }


        // WAV files can use "EXTENSIBLE" format, which hides the real encoding in a 16-byte GUID.
        // These are the GUIDs for the two encodings we support:
        // PCM = raw integer samples (most common), IEEE float = decimal samples (pro audio tools)
        private static readonly byte[] PcmSubFormatGuid = {
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00,
            0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71
        };
        private static readonly byte[] FloatSubFormatGuid = {
            0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00,
            0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71
        };

        // μ-law decoding: 1 byte → 16-bit signed sample.
        // Used in North American/Japanese telephony. Each byte encodes a logarithmic sample.
        private static short DecodeULaw(byte ulawByte)
        {
            ulawByte = (byte)~ulawByte;
            int sign = (ulawByte & 0x80) is not 0 ? -1 : 1;
            int exponent = (ulawByte >> 4) & 0x07;
            int mantissa = ulawByte & 0x0F;
            int sample = (mantissa << 3) + 0x84;
            sample <<= exponent;
            sample -= 0x84;
            return (short)(sign * sample);
        }

        // A-law decoding: 1 byte → 16-bit signed sample.
        // Used in European telephony. Similar to μ-law but slightly different curve.
        private static short DecodeALaw(byte alawByte)
        {
            alawByte ^= 0x55;
            int sign = (alawByte & 0x80) is not 0 ? -1 : 1;
            int exponent = (alawByte >> 4) & 0x07;
            int mantissa = alawByte & 0x0F;
            int sample;
            if (exponent is 0)
                sample = (mantissa << 4) + 8;
            else
                sample = ((mantissa << 4) + 0x108) << (exponent - 1);
            return (short)(sign * sample);
        }

        // Check if bytes at a given offset match an ASCII string (used for file format magic bytes)
        private static bool BytesMatch(byte[] bytes, int offset, string magic)
        {
            if (offset + magic.Length > bytes.Length) return false;
            for (int i = 0; i < magic.Length; i++)
                if (bytes[offset + i] != magic[i]) return false;
            return true;
        }

        // Every audio format starts with unique "magic bytes". Check those to identify the file
        private static string DetectAudioFormat(byte[] bytes)
        {
            if (bytes.Length < 12) return null;

            if (BytesMatch(bytes, 0, "RIFF") && BytesMatch(bytes, 8, "WAVE")) return "WAV";
            if (BytesMatch(bytes, 0, "OggS")) return "OGG";
            if (BytesMatch(bytes, 0, "fLaC")) return "FLAC";
            if (BytesMatch(bytes, 0, "FORM") && BytesMatch(bytes, 8, "AIFF")) return "AIFF";
            if (BytesMatch(bytes, 0, "ID3")) return "MP3";
            if (bytes[0] is 0xFF && bytes[1] is 0xFB or 0xF3 or 0xF2) return "MP3";
            if (BytesMatch(bytes, 4, "ftyp")) return "AAC/M4A";
            if (bytes[0] is 0x30 && bytes[1] is 0x26 && bytes[2] is 0xB2 && bytes[3] is 0x75) return "WMA";

            return null;
        }

        private static AudioClip LoadWavFromBytes(byte[] fileBytes, string name)
        {
            if (fileBytes.Length < 12)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' is probably corrupted: file is only {fileBytes.Length} bytes, which is smaller than a valid WAV header (12 bytes minimum)");
                return null;
            }

            // Identify the file format
            string detectedFormat = DetectAudioFormat(fileBytes);
            if (detectedFormat is not "WAV")
            {
                if (detectedFormat is not null)
                    MelonLoader.MelonLogger.Error($"[AudioManager] File '{name}' is not a WAV file, it's a renamed {detectedFormat} file. Please convert it to WAV first");
                else
                    MelonLoader.MelonLogger.Error($"[AudioManager] File '{name}' is not a valid WAV file. It may be a renamed audio file of an uncommon unsupported type. Please convert it to WAV first");
                return null;
            }

            // A WAV file is a sequence of "chunks" after the 12-byte RIFF header.
            // We need two: "fmt " (format info like sample rate) and "data" (the actual audio).
            // Other chunks (JUNK, bext, LIST, etc.) are metadata we skip over.
            int headerOffset = 12;
            int fmtOffset = -1;
            int fmtSize = 0;
            int dataOffset = -1;
            int dataSize = 0;

            while (headerOffset + 8 <= fileBytes.Length)
            {
                string chunkName = System.Text.Encoding.ASCII.GetString(fileBytes, headerOffset, 4);
                int chunkSize = BitConverter.ToInt32(fileBytes, headerOffset + 4);

                if (chunkSize < 0)
                {
                    MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' is probably corrupted: chunk '{chunkName}' has a negative size ({chunkSize})");
                    return null;
                }

                if (chunkName is "fmt ")
                {
                    fmtOffset = headerOffset + 8;
                    fmtSize = chunkSize;
                }
                else if (chunkName is "data")
                {
                    dataOffset = headerOffset + 8;
                    dataSize = chunkSize;
                }

                // Each chunk is [4-byte ID][4-byte size][payload]. Odd-sized payloads get 1 pad byte.
                headerOffset += 8 + chunkSize + (chunkSize & 1);
            }

            if (fmtOffset is -1 || dataOffset is -1)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' is probably corrupted: missing {(fmtOffset == -1 ? "'fmt '" : "'data'")} chunk");
                return null;
            }

            // Validate fmt chunk isn't truncated
            if (fmtOffset + fmtSize > fileBytes.Length || fmtSize < 16)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' is probably corrupted: 'fmt ' chunk is truncated or too small ({fmtSize} bytes)");
                return null;
            }

            // Validate data chunk isn't truncated
            if (dataOffset + dataSize > fileBytes.Length)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' is probably corrupted: 'data' chunk claims {dataSize} bytes but file ends before that");
                return null;
            }

            // Read format info relative to the fmt chunk
            int formatTag = BitConverter.ToInt16(fileBytes, fmtOffset) & 0xFFFF;
            int channels = BitConverter.ToInt16(fileBytes, fmtOffset + 2);
            int sampleRate = BitConverter.ToInt32(fileBytes, fmtOffset + 4);
            int bitsPerSample = BitConverter.ToInt16(fileBytes, fmtOffset + 14);

            if (channels <= 0 || sampleRate <= 0 || bitsPerSample <= 0)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' is probably corrupted: invalid format values (channels={channels}, sampleRate={sampleRate}, bitsPerSample={bitsPerSample})");
                return null;
            }

            // Some audio tools write a 40-byte "EXTENSIBLE" format instead of the simple 16-byte one,
            // even for normal audio. The real format (PCM or float) is in a GUID at the end.
            if (formatTag is 0xFFFE)
            {
                if (fmtSize < 40)
                {
                    MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' is probably corrupted: EXTENSIBLE format but fmt chunk too small ({fmtSize} bytes, need 40)");
                    return null;
                }
                bool isPcm = true, isFloat = true;
                for (int i = 0; i < 16; i++)
                {
                    if (fileBytes[fmtOffset + 24 + i] != PcmSubFormatGuid[i]) isPcm = false;
                    if (fileBytes[fmtOffset + 24 + i] != FloatSubFormatGuid[i]) isFloat = false;
                }
                if (isPcm) formatTag = 0x0001;
                else if (isFloat) formatTag = 0x0003;
                else
                {
                    MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' uses an unsupported EXTENSIBLE SubFormat. Only PCM and IEEE float WAV files are supported");
                    return null;
                }
            }

            // Supported format tags: PCM, IEEE float, A-law, μ-law
            if (formatTag is not 0x0001 and not 0x0003 and not 0x0006 and not 0x0007)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' uses unsupported format tag 0x{formatTag:X4}. Supported: PCM, IEEE float, A-law, \u03BC-law");
                return null;
            }

            if (bitsPerSample % 8 is not 0)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' has unsupported bits per sample ({bitsPerSample}), must be a multiple of 8");
                return null;
            }
            int bytesPerSample = bitsPerSample / 8;
            if (bytesPerSample is not 1 or 2 or 3 or 4 or 8)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' has unsupported bytes per sample ({bytesPerSample}). Supported: 1, 2, 3, 4, 8");
                return null;
            }

            int blockAlign = bytesPerSample * channels;
            int sampleCount = dataSize / blockAlign;
            int totalSamples = sampleCount * channels;
            float[] samples = new float[totalSamples];

            // Convert raw bytes to float samples (-1.0 to 1.0) that Unity expects.
            // PCM stores samples as integers that we divide to normalize.
            // IEEE float samples are already in the right range, just read them directly.
            // μ-law/A-law decode each byte into a 16-bit sample, then normalize.
            if (formatTag is 0x0006 or 0x0007) // μ-law or A-law
            {
                for (int i = 0; i < totalSamples; i++)
                {
                    short decoded = formatTag is 0x0006 ?
                        DecodeALaw(fileBytes[dataOffset + i]) :
                        DecodeULaw(fileBytes[dataOffset + i]);
                    samples[i] = decoded / 32768f;
                }
            }
            else if (formatTag is 0x0003) // IEEE float
            {
                for (int i = 0; i < totalSamples; i++)
                {
                    int offset = dataOffset + i * bytesPerSample;
                    if (bytesPerSample is 4) // 32-bit float
                        samples[i] = BitConverter.ToSingle(fileBytes, offset);
                    else if (bytesPerSample is 8) // 64-bit double, downconvert to float
                        samples[i] = (float)BitConverter.ToDouble(fileBytes, offset);
                }
            }
            else // PCM integer
            {
                for (int i = 0; i < totalSamples; i++)
                {
                    int offset = dataOffset + i * bytesPerSample;
                    switch (bytesPerSample)
                    {
                        case 1: // 8-bit unsigned
                            samples[i] = (fileBytes[offset] - 128) / 128f;
                            break;
                        case 2: // 16-bit signed
                            samples[i] = BitConverter.ToInt16(fileBytes, offset) / 32768f;
                            break;
                        case 3: // 24-bit signed (little-endian, sign-extend from high byte)
                            int s24 = fileBytes[offset] | (fileBytes[offset + 1] << 8) | ((sbyte)fileBytes[offset + 2] << 16);
                            samples[i] = s24 / 8388608f;
                            break;
                        case 4: // 32-bit signed
                            samples[i] = BitConverter.ToInt32(fileBytes, offset) / 2147483648f;
                            break;
                    }
                }
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, channels, sampleRate, false);
            clip.SetData(samples, 0);

            return clip;
        }

        /// <summary>
        /// Plays a specified AudioCall at a Position
        /// </summary>
        public static PooledAudioSource PlaySound(AudioCall audioCall, Vector3 position, bool isLooping = false)
        {
            return Il2CppRUMBLE.Managers.AudioManager.instance.Play(audioCall, position, isLooping);
        }

        /// <summary>
        /// Creates an AudioCall and sets the clip to the file. Must be wav file. Sets the AudioCall/clip HideFlags to HideAndDontSave
        /// </summary>
        public static AudioCall CreateAudioCall(string filePath, float volume)
        {
            string[] filePathSplit = filePath.Replace(".wav", "").Split('\\');
            string name = filePathSplit[filePathSplit.Length - 1];
            bool fileExists = File.Exists(filePath);
            if (fileExists)
            {
                AudioCall audioCall = ScriptableObject.CreateInstance<AudioCall>();
                audioCall.name = name;

                GeneralAudioSettings generalSettings = new GeneralAudioSettings();
                generalSettings.SetVolume(volume);
                generalSettings.Pitch = 1;
                audioCall.generalSettings = generalSettings;

                SpatialAudioSettings spatialSettings = new SpatialAudioSettings();
                spatialSettings.CustomReverbZoneMixCurve = new AnimationCurve();
                spatialSettings.CustomRolloffCurve = new AnimationCurve();
                spatialSettings.CustomSpatialBlendCurve = new AnimationCurve();
                spatialSettings.CustomSpreadCurve = new AnimationCurve();
                audioCall.spatialSettings = spatialSettings;

                AudioCall.WeightedClip weightedClip = new AudioCall.WeightedClip();
                AudioClip clip = AudioManager.LoadWavFile(filePath);

                if (clip == null)
                {
                    fileExists = false;
                    return null;
                }

                clip.name = name;
                clip.hideFlags = HideFlags.HideAndDontSave;
                clip.LoadAudioData();
                weightedClip.Clip = clip;

                AudioCall.WeightedClip[] weightedClips = new AudioCall.WeightedClip[1] { weightedClip };
                weightedClips[0].Weight = 1;
                audioCall.clips = weightedClips;
                audioCall.clips[0] = audioCall.GetRandomClip();
                audioCall.clips[0].Clip = clip;

                audioCall.hideFlags = HideFlags.HideAndDontSave;
                audioCall.clips[0].Clip.hideFlags = HideFlags.HideAndDontSave;
                return audioCall;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a list of AudioCalls and sets the clips to the files. Must be wav files. Sets the AudioCalls/clips HideFlags to HideAndDontSave
        /// </summary>
        public static AudioCall[] CreateAudioCalls(string[] filePaths, float volume)
        {
            AudioCall[] results = new AudioCall[filePaths.Length];
            for (int i = 0; i < filePaths.Length; i++)
            {
                string[] filePathSplit = filePaths[i].Replace(".wav", "").Split('\\');
                string name = filePathSplit[filePathSplit.Length - 1];
                bool fileExists = File.Exists(filePaths[i]);
                if (fileExists)
                {
                    AudioCall audioCall = ScriptableObject.CreateInstance<AudioCall>();
                    audioCall.name = name;

                    GeneralAudioSettings generalSettings = new GeneralAudioSettings();
                    generalSettings.SetVolume(volume);
                    generalSettings.Pitch = 1;
                    audioCall.generalSettings = generalSettings;

                    SpatialAudioSettings spatialSettings = new SpatialAudioSettings();
                    spatialSettings.CustomReverbZoneMixCurve = new AnimationCurve();
                    spatialSettings.CustomRolloffCurve = new AnimationCurve();
                    spatialSettings.CustomSpatialBlendCurve = new AnimationCurve();
                    spatialSettings.CustomSpreadCurve = new AnimationCurve();
                    audioCall.spatialSettings = spatialSettings;

                    AudioCall.WeightedClip weightedClip = new AudioCall.WeightedClip();
                    AudioClip clip = AudioManager.LoadWavFile(filePaths[i]);

                    if (clip == null)
                    {
                        results[i] = null;
                        continue;
                    }

                    clip.name = name;
                    clip.hideFlags = HideFlags.HideAndDontSave;
                    clip.LoadAudioData();
                    weightedClip.Clip = clip;

                    AudioCall.WeightedClip[] weightedClips = new AudioCall.WeightedClip[1] { weightedClip };
                    weightedClips[0].Weight = 1;
                    audioCall.clips = weightedClips;
                    audioCall.clips[0] = audioCall.GetRandomClip();
                    audioCall.clips[0].Clip = clip;

                    audioCall.hideFlags = HideFlags.HideAndDontSave;
                    audioCall.clips[0].Clip.hideFlags = HideFlags.HideAndDontSave;
                    results[i] = audioCall;
                    continue;
                }
                else
                {
                    results[i] = null;
                    continue;
                }
            }
            return results;
        }
    }
}
