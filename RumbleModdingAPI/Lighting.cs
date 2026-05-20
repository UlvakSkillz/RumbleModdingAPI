using System.Diagnostics;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;

namespace RumbleModdingAPI;

public class Lighting
{
    private static bool _locked = false;
    private static int _lockPriority = 0;
    private static string _lockedBy = "Not Locked, lol? You shouldn't see this.";
    
    private static AmbientMode _ambientMode = AmbientMode.Skybox;
    private static Color _skyColor = new Color(0.350f, 0.428f, 0.584f);
    private static Color _horizonColor = new Color(0.114f, 0.125f, 0.133f);
    private static Color _groundColor = new Color(0.047f, 0.043f, 0.035f);

    /// <summary>
    /// Locks all lighting settings so they cannot be changed by other mods.
    /// If a mod attempts to unlock lighting with a lower lock priority, it will be ignored.
    /// </summary>
    /// <param name="lockPriority">Use reasonable judgement when setting lock priority, your mod isn't the main character, and you likely don't need it.</param>
    public static void Lock(int lockPriority = 1)
    {
        _locked = true;
        _lockPriority = lockPriority;
        _lockedBy = GetCallingModName();
    }
    
    /// <summary>
    /// Attempts to unlock lighting settings so they can be changed again.
    /// If a mod attempts to unlock lighting with a lower lock priority, it will be ignored.
    /// </summary>
    /// <param name="lockPriority">Use reasonable judgement when unlocking settings. your mod isn't the main character, and if someone's locked with high priority, it should probably stay that way.</param>
    public static void Unlock(int lockPriority = 1)
    {
        if (_lockPriority > lockPriority && _lockedBy != GetCallingModName()) return;
        _locked = false;
        _lockPriority = 0;
    }

    /// <summary>
    /// Reloads lighting settings on scene load, since RenderSettings is a per-scene object.
    /// </summary>
    internal static void OnSceneWasLoaded()
    {
        RenderSettings.ambientMode = _ambientMode;
        RenderSettings.ambientLight = _skyColor;
        RenderSettings.ambientEquatorColor = _horizonColor;
        RenderSettings.ambientGroundColor = _groundColor;
    }
    
    /// <summary>
    /// Sets the current scene's lighting mode. Node: RUMBLE's Loader scene uses trilight by default.
    /// </summary>
    /// <param name="useBounce">If true, sets lighting mode to Trilight. If false, sets it back to skybox.</param>
    public static void UseBounceLighting(bool useBounce)
    {
        if (_locked) return;
        RenderSettings.ambientMode = useBounce ? AmbientMode.Trilight : AmbientMode.Skybox;
        _ambientMode = useBounce ? AmbientMode.Trilight : AmbientMode.Skybox;
    }

    /// <summary>
    /// Sets the color of the light applied to the top of all dynamically lit surfaces in the scene.
    /// </summary>
    /// <param name="color">The color to set. Leave null for default settings.</param>
    public static void SetSkyLightColor(Color? color = null)
    {
        if (_locked) return;
        RenderSettings.ambientSkyColor = color ?? new Color(0.350f, 0.428f, 0.584f);
        _skyColor = color ?? _skyColor;
    }

    /// <summary>
    /// Sets the color of the light applied to the sides of all dynamically lit surfaces in the scene.
    /// </summary>
    /// <param name="color">The color to set. Leave null for default settings.</param>
    public static void SetHorizonLightColor(Color? color = null)
    {
        if (_locked) return;
        RenderSettings.ambientEquatorColor = color ?? new Color(0.114f, 0.125f, 0.133f);
        _horizonColor = color ?? _horizonColor;
    }

    /// <summary>
    /// Sets the color of the light applied to the bottom of all dynamically lit surfaces in the scene.
    /// </summary>
    /// <param name="color">The color to set. Leave null for default settings.</param>
    public static void SetGroundLightColor(Color? color = null)
    {
        if (_locked) return;
        RenderSettings.ambientGroundColor = color ?? new Color(0.047f, 0.043f, 0.035f);
        _groundColor = color ?? _groundColor;
    }

    private static string GetCallingModName()
    {
        Assembly apiAssembly = Assembly.GetExecutingAssembly();
        var stack = new StackTrace();
        for (int i = 0; i < stack.FrameCount; i++)
        {
            MethodBase method = stack.GetFrame(i).GetMethod();
            if (method?.DeclaringType == null)
                continue;
            
            Assembly assembly = method.DeclaringType.Assembly;

            if (assembly == apiAssembly)
                continue;

            foreach (var melon in MelonMod.RegisteredMelons)
            {
                if (melon.MelonAssembly.Assembly == assembly)
                    return melon.Info.Name;
            }
        }

        return "Unknown Caller";
    }
}