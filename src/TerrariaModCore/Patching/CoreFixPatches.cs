using System;
using System.Reflection;
using HarmonyLib;
using Terraria;
using Terraria.Graphics.Capture;

namespace TerrariaModCore.Patching
{
    /// <summary>
    /// Core engine compatibility patches to protect against vanilla initialization race conditions.
    /// Specifically protects CaptureManager when accessed before XNA GraphicsDevice is instantiated.
    /// </summary>
    public static class CoreFixPatches
    {
        private static readonly Type CameraType = typeof(CaptureManager).Assembly.GetType("Terraria.Graphics.Capture.CaptureCamera");
        private static readonly MethodInfo CaptureMethod = CameraType?.GetMethod("Capture", BindingFlags.Instance | BindingFlags.Public);
        private static readonly MethodInfo DrawTickMethod = CameraType?.GetMethod("DrawTick", BindingFlags.Instance | BindingFlags.Public);
        private static readonly PropertyInfo IsCapturingProp = CameraType?.GetProperty("IsCapturing", BindingFlags.Instance | BindingFlags.Public);

        public static void Apply(Harmony harmony)
        {
            if (harmony == null) return;

            try
            {
                // Patch CaptureManager..ctor
                var ctorMethod = typeof(CaptureManager).GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    Type.EmptyTypes,
                    null
                );
                if (ctorMethod != null)
                {
                    var prefix = typeof(CoreFixPatches).GetMethod(nameof(CaptureManagerCtorPrefix), BindingFlags.Static | BindingFlags.NonPublic);
                    harmony.Patch(ctorMethod, prefix: new HarmonyMethod(prefix));
                }

                // Patch CaptureManager.get_IsCapturing
                var isCapturingProp = typeof(CaptureManager).GetProperty("IsCapturing", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (isCapturingProp?.GetGetMethod() != null)
                {
                    var prefix = typeof(CoreFixPatches).GetMethod(nameof(CaptureManagerIsCapturingPrefix), BindingFlags.Static | BindingFlags.NonPublic);
                    harmony.Patch(isCapturingProp.GetGetMethod(), prefix: new HarmonyMethod(prefix));
                }

                // Patch CaptureManager.Capture
                var captureMethod = typeof(CaptureManager).GetMethod("Capture", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(CaptureSettings) }, null);
                if (captureMethod != null)
                {
                    var prefix = typeof(CoreFixPatches).GetMethod(nameof(CaptureManagerCapturePrefix), BindingFlags.Static | BindingFlags.NonPublic);
                    harmony.Patch(captureMethod, prefix: new HarmonyMethod(prefix));
                }

                // Patch CaptureManager.DrawTick
                var drawTickMethod = typeof(CaptureManager).GetMethod("DrawTick", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (drawTickMethod != null)
                {
                    var prefix = typeof(CoreFixPatches).GetMethod(nameof(CaptureManagerDrawTickPrefix), BindingFlags.Static | BindingFlags.NonPublic);
                    harmony.Patch(drawTickMethod, prefix: new HarmonyMethod(prefix));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TMC WARNING] Could not apply CoreFixPatches: {ex.Message}");
            }
        }

        private static bool CaptureManagerCtorPrefix(CaptureManager __instance, ref CaptureInterface ____interface, ref object ____camera)
        {
            ____interface = new CaptureInterface();
            object gd = GetGraphicsDevice();
            if (!Main.dedServ && gd != null && CameraType != null)
            {
                try
                {
                    ____camera = Activator.CreateInstance(CameraType, gd);
                }
                catch
                {
                    ____camera = null;
                }
            }
            else
            {
                ____camera = null;
            }

            return false; // Skip original constructor
        }

        private static bool CaptureManagerIsCapturingPrefix(CaptureManager __instance, ref object ____camera, ref bool __result)
        {
            if (Main.dedServ)
            {
                __result = false;
                return false;
            }

            EnsureCamera(__instance, ref ____camera);
            if (____camera == null || IsCapturingProp == null)
            {
                __result = false;
                return false;
            }

            try
            {
                __result = (bool)IsCapturingProp.GetValue(____camera, null);
            }
            catch
            {
                __result = false;
            }
            return false;
        }

        private static bool CaptureManagerCapturePrefix(CaptureManager __instance, ref object ____camera, CaptureSettings settings)
        {
            if (Main.dedServ) return false;
            EnsureCamera(__instance, ref ____camera);
            if (____camera != null && CaptureMethod != null)
            {
                try
                {
                    CaptureMethod.Invoke(____camera, new object[] { settings });
                }
                catch { }
            }
            return false;
        }

        private static bool CaptureManagerDrawTickPrefix(CaptureManager __instance, CaptureInterface ____interface, ref object ____camera)
        {
            ____interface?.UpdateCameraCountdown();
            if (!Main.dedServ)
            {
                EnsureCamera(__instance, ref ____camera);
                if (____camera != null && DrawTickMethod != null)
                {
                    try
                    {
                        DrawTickMethod.Invoke(____camera, null);
                    }
                    catch { }
                }
            }
            return false;
        }

        private static void EnsureCamera(CaptureManager instance, ref object camera)
        {
            if (camera == null && !Main.dedServ && CameraType != null)
            {
                object gd = GetGraphicsDevice();
                if (gd != null)
                {
                    try
                    {
                        camera = Activator.CreateInstance(CameraType, gd);
                    }
                    catch
                    {
                        camera = null;
                    }
                }
            }
        }

        private static object GetGraphicsDevice()
        {
            try
            {
                var mainType = typeof(CaptureManager).Assembly.GetType("Terraria.Main");
                var instanceField = mainType?.GetField("instance", BindingFlags.Static | BindingFlags.Public);
                object mainInstance = instanceField?.GetValue(null);
                if (mainInstance == null) return null;

                var gdProp = mainType?.GetProperty("GraphicsDevice", BindingFlags.Instance | BindingFlags.Public);
                return gdProp?.GetValue(mainInstance, null);
            }
            catch
            {
                return null;
            }
        }
    }
}
