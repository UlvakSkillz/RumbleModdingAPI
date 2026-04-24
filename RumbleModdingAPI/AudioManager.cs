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

        // IMA ADPCM: a simple compression format (~4:1 ratio). Common in older games and embedded audio.
        // Step size table: maps step index (0–88) to the quantization step size.
        // Each 4-bit nibble in the compressed data is multiplied by this step to reconstruct the sample.
        private static readonly int[] ImaStepTable = {
            7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 19, 21, 23, 25, 28, 31,
            34, 37, 41, 45, 50, 55, 60, 66, 73, 80, 88, 97, 107, 118, 130, 143,
            157, 173, 190, 209, 230, 253, 279, 307, 337, 371, 408, 449, 494, 544, 598, 658,
            724, 796, 876, 963, 1060, 1166, 1282, 1411, 1552, 1707, 1878, 2066, 2272, 2499,
            2749, 3024, 3327, 3660, 4026, 4428, 4871, 5358, 5894, 6484, 7132, 7845, 8630,
            9493, 10442, 11487, 12635, 13899, 15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767
        };

        // IMA ADPCM index adjustment: after decoding a nibble, the step index moves by this amount.
        // Larger nibble values (louder changes) increase the step size for the next sample.
        private static readonly int[] ImaIndexTable = {
            -1, -1, -1, -1, 2, 4, 6, 8,
            -1, -1, -1, -1, 2, 4, 6, 8
        };

        // IMA ADPCM: audio is split into blocks. Each block has a 4-byte header per channel
        // (initial sample + step index), then 4-bit nibbles. Each nibble decodes to one sample.
        private static float[] DecodeImaAdpcm(byte[] data, int dataOffset, int dataSize, int channels, int blockAlign, out string error)
        {
            error = null;
            // Each block: 4 bytes header per channel, then 4-bit nibbles for the rest
            int headerSize = 4 * channels;
            if (blockAlign <= headerSize)
            {
                error = $"block align ({blockAlign}) is too small for {channels} channel(s)";
                return null;
            }

            int nibbleBytes = blockAlign - headerSize;
            // Each byte has 2 nibbles = 2 samples, plus 1 sample from each channel's header
            int samplesPerBlock = nibbleBytes * 2 / channels + 1;

            int numBlocks = dataSize / blockAlign;
            var samples = new System.Collections.Generic.List<float>(numBlocks * samplesPerBlock * channels);

            int[] predictor = new int[channels];
            int[] stepIndex = new int[channels];

            for (int block = 0; block < numBlocks; block++)
            {
                int blockStart = dataOffset + block * blockAlign;

                // Read block header for each channel: 2-byte initial sample, 1-byte step index, 1 reserved
                for (int ch = 0; ch < channels; ch++)
                {
                    int hdrOffset = blockStart + ch * 4;
                    predictor[ch] = BitConverter.ToInt16(data, hdrOffset);
                    stepIndex[ch] = data[hdrOffset + 2];
                    if (stepIndex[ch] > 88) stepIndex[ch] = 88;
                }

                // First sample of each channel comes from the header
                for (int ch = 0; ch < channels; ch++)
                    samples.Add(predictor[ch] / 32768f);

                // Decode nibbles, which are interleaved for stereo: 8 nibbles (4 bytes) per channel, alternating
                int nibbleStart = blockStart + headerSize;
                int nibbleEnd = blockStart + blockAlign;
                int samplesPerChunkPerChannel = channels > 1 ? 8 : nibbleBytes * 2;
                int ch_idx = 0;
                int chunkSampleCount = 0;

                for (int pos = nibbleStart; pos < nibbleEnd; pos++)
                {
                    for (int nibbleIdx = 0; nibbleIdx < 2; nibbleIdx++)
                    {
                        int nibble = nibbleIdx is 0 ? (data[pos] & 0x0F) : ((data[pos] >> 4) & 0x0F);

                        int step = ImaStepTable[stepIndex[ch_idx]];
                        int diff = step >> 3;
                        if ((nibble & 1) is not 0) diff += step >> 2;
                        if ((nibble & 2) is not 0) diff += step >> 1;
                        if ((nibble & 4) is not 0) diff += step;
                        if ((nibble & 8) is not 0) diff = -diff;

                        predictor[ch_idx] = Math.Clamp(predictor[ch_idx] + diff, -32768, 32767);
                        stepIndex[ch_idx] = Math.Clamp(stepIndex[ch_idx] + ImaIndexTable[nibble], 0, 88);

                        samples.Add(predictor[ch_idx] / 32768f);
                        chunkSampleCount++;

                        // For multi-channel: after 8 samples, switch to next channel
                        if (channels > 1 && chunkSampleCount >= samplesPerChunkPerChannel)
                        {
                            chunkSampleCount = 0;
                            ch_idx = (ch_idx + 1) % channels;
                        }
                    }
                }
            }

            return samples.ToArray();
        }

        // MS ADPCM: Microsoft's ADPCM variant (~4:1 ratio). Found in older Windows tools and some game engines.
        // Coefficient pairs: used to predict the next sample from the previous two.
        // Standard WAV defines 7 built-in pairs; files can define more in the fmt chunk.
        private static readonly int[][] MsAdpcmDefaultCoefficients = {
            new[] { 256, 0 },
            new[] { 512, -256 },
            new[] { 0, 0 },
            new[] { 192, 64 },
            new[] { 240, 0 },
            new[] { 460, -208 },
            new[] { 392, -232 }
        };

        // MS ADPCM: each block has a header per channel (predictor index, delta, two initial samples),
        // then pairs of 4-bit nibbles. Each nibble adjusts a prediction based on coefficient pairs.
        private static float[] DecodeMsAdpcm(byte[] data, int dataOffset, int dataSize, int channels, int blockAlign, int fmtOffset, int fmtSize, out string error)
        {
            error = null;

            // Read coefficients from fmt chunk if available (after the standard 18 bytes + 2-byte cbSize)
            int[][] coefficients = MsAdpcmDefaultCoefficients;
            if (fmtSize >= 22)
            {
                int cbSize = BitConverter.ToInt16(data, fmtOffset + 16) & 0xFFFF;
                // cbSize should contain: 2 bytes (numCoefficients) + 4 bytes per coefficient pair
                if (cbSize >= 6 && fmtOffset + 20 + cbSize <= data.Length)
                {
                    int numCoef = BitConverter.ToInt16(data, fmtOffset + 20) & 0xFFFF;
                    if (numCoef > 0 && numCoef <= 32 && fmtOffset + 22 + numCoef * 4 <= data.Length)
                    {
                        coefficients = new int[numCoef][];
                        for (int i = 0; i < numCoef; i++)
                        {
                            int coefOffset = fmtOffset + 22 + i * 4;
                            coefficients[i] = new int[] {
                                BitConverter.ToInt16(data, coefOffset),
                                BitConverter.ToInt16(data, coefOffset + 2)
                            };
                        }
                    }
                }
            }

            int headerSize = 7 * channels; // 1 predictor + 2 delta + 2 sample1 + 2 sample2, per channel
            if (blockAlign <= headerSize)
            {
                error = $"block align ({blockAlign}) is too small for {channels} channel(s)";
                return null;
            }

            int nibbleBytes = blockAlign - headerSize;
            // 2 samples from header per channel + 2 nibbles per byte
            int samplesPerBlock = 2 + nibbleBytes * 2 / channels;

            int numBlocks = dataSize / blockAlign;
            var samples = new System.Collections.Generic.List<float>(numBlocks * samplesPerBlock * channels);

            for (int block = 0; block < numBlocks; block++)
            {
                int blockStart = dataOffset + block * blockAlign;

                int[] predIdx = new int[channels];
                int[] delta = new int[channels];
                int[] sample1 = new int[channels];
                int[] sample2 = new int[channels];

                int pos = blockStart;
                // Read predictor index for each channel
                for (int ch = 0; ch < channels; ch++)
                    predIdx[ch] = Math.Clamp(data[pos++], 0, coefficients.Length - 1);
                // Read delta for each channel
                for (int ch = 0; ch < channels; ch++)
                    { delta[ch] = BitConverter.ToInt16(data, pos); pos += 2; }
                // Read first sample for each channel
                for (int ch = 0; ch < channels; ch++)
                    { sample1[ch] = BitConverter.ToInt16(data, pos); pos += 2; }
                // Read second sample for each channel
                for (int ch = 0; ch < channels; ch++)
                    { sample2[ch] = BitConverter.ToInt16(data, pos); pos += 2; }

                // Output initial samples (sample2 first, then sample1: they're in reverse order)
                for (int ch = 0; ch < channels; ch++)
                    samples.Add(sample2[ch] / 32768f);
                for (int ch = 0; ch < channels; ch++)
                    samples.Add(sample1[ch] / 32768f);

                // Decode nibble pairs
                int ch_idx = 0;
                int blockEnd = blockStart + blockAlign;
                while (pos < blockEnd)
                {
                    byte nibbleByte = data[pos++];
                    // High nibble first, then low nibble
                    for (int nibbleIdx = 0; nibbleIdx < 2 && samples.Count < numBlocks * samplesPerBlock * channels; nibbleIdx++)
                    {
                        int nibble = nibbleIdx is 0 ? ((nibbleByte >> 4) & 0x0F) : (nibbleByte & 0x0F);
                        // Sign-extend the 4-bit nibble
                        if (nibble >= 8) nibble -= 16;

                        int coef1 = coefficients[predIdx[ch_idx]][0];
                        int coef2 = coefficients[predIdx[ch_idx]][1];

                        int predicted = (sample1[ch_idx] * coef1 + sample2[ch_idx] * coef2) / 256;
                        int newSample = Math.Clamp(predicted + nibble * delta[ch_idx], -32768, 32767);

                        sample2[ch_idx] = sample1[ch_idx];
                        sample1[ch_idx] = newSample;

                        // Adaptive delta update
                        int[] adaptTable = { 230, 230, 230, 230, 307, 409, 512, 614 };
                        delta[ch_idx] = Math.Max(16, adaptTable[nibble < 0 ? nibble + 8 : nibble] * delta[ch_idx] / 256);

                        samples.Add(newSample / 32768f);
                        ch_idx = (ch_idx + 1) % channels;
                    }
                }
            }

            return samples.ToArray();
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

            // Supported format tags: PCM, MS ADPCM, IEEE float, A-law, μ-law, IMA ADPCM
            if (formatTag is not 0x0001 and not 0x0002 and not 0x0003 and not 0x0006 and not 0x0007 and not 0x0011)
            {
                MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' uses unsupported format tag 0x{formatTag:X4}. Supported: PCM, ADPCM, IEEE float, A-law, \u03BC-law");
                return null;
            }

            // ADPCM formats are block-based (compressed). They have their own decoding path.
            // All other formats are sample-based (one value per sample, fixed size).
            float[] samples;

            if (formatTag is 0x0002 or 0x0011) // MS ADPCM or IMA ADPCM
            {
                // Block align from the fmt chunk tells us how many bytes per block
                int blockAlign = BitConverter.ToInt16(fileBytes, fmtOffset + 12) & 0xFFFF;
                if (blockAlign <= 0)
                {
                    MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' is probably corrupted: ADPCM block align is {blockAlign}");
                    return null;
                }

                string adpcmError;
                if (formatTag is 0x0011)
                    samples = DecodeImaAdpcm(fileBytes, dataOffset, dataSize, channels, blockAlign, out adpcmError);
                else
                    samples = DecodeMsAdpcm(fileBytes, dataOffset, dataSize, channels, blockAlign, fmtOffset, fmtSize, out adpcmError);

                if (samples is null)
                {
                    MelonLoader.MelonLogger.Error($"[AudioManager] WAV file '{name}' failed to decode ADPCM audio: {adpcmError}");
                    return null;
                }

                int sampleCount = samples.Length / channels;
                AudioClip newClip = AudioClip.Create(name, sampleCount, channels, sampleRate, false);
                newClip.SetData(samples, 0);
                return newClip;
            }

            // Non-ADPCM: validate bits/bytes per sample
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

            int sampleBlockAlign = bytesPerSample * channels;
            int totalFrames = dataSize / sampleBlockAlign;
            int totalSamples = totalFrames * channels;
            samples = new float[totalSamples];

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

            AudioClip clip = AudioClip.Create(name, totalSamples, channels, sampleRate, false);
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

                if (clip is null)
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

                    if (clip is null)
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
