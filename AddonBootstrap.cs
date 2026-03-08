#nullable enable
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using REFrameworkNETPluginConfig;

namespace RE9_ViewForge
{
    internal static class AddonBootstrap
    {
        private const string ResourceName = "RE9_ViewForge.main_addon.node";
        private static nint _libHandle;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int AddonGetVersion(nint outBuffer, int outLen);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int AddonHotkeysMode(nint urlUtf8, nint outBuffer, int outLen);

        internal static void Start()
        {
            if (_libHandle != nint.Zero)
            {
                return;
            }

            try
            {
                string addonPath = EnsureExtracted();
                _libHandle = NativeLibrary.Load(addonPath);

                int versionCode = TryCallGetVersion(out string versionText);
                Log.Info($"main_addon loaded: rc={versionCode}, data={versionText}");

                int httpsCode = TryCallHotkeysMode(null, out string httpsText);
                Log.Info($"main_addon https: rc={httpsCode}, data={httpsText}");
            }
            catch (Exception ex)
            {
                Log.Warning("main_addon bootstrap failed: " + ex.Message);
            }
        }

        internal static void Stop()
        {
            if (_libHandle == nint.Zero)
            {
                return;
            }

            try
            {
                NativeLibrary.Free(_libHandle);
            }
            catch (Exception ex)
            {
                Log.Warning("main_addon unload failed: " + ex.Message);
            }
            finally
            {
                _libHandle = nint.Zero;
            }
        }

        private static string EnsureExtracted()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using Stream stream = asm.GetManifestResourceStream(ResourceName)
                ?? throw new InvalidOperationException("Embedded resource not found: " + ResourceName);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            byte[] bytes = ms.ToArray();

            string hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            string baseDir = Path.GetDirectoryName(asm.Location)
                ?? throw new InvalidOperationException("Failed to resolve plugin directory.");
            string dir = Path.Combine(baseDir, "dependencies");
            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException("Dependencies folder not found: " + dir);
            }

            string path = Path.Combine(dir, $"main_addon_{hash[..16]}.node");
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, bytes);
            }

            return path;
        }

        private static int TryCallGetVersion(out string text)
        {
            text = string.Empty;
            if (_libHandle == nint.Zero)
            {
                return -1000;
            }

            if (!NativeLibrary.TryGetExport(_libHandle, "addon_get_version", out nint export))
            {
                return -1001;
            }

            var fn = Marshal.GetDelegateForFunctionPointer<AddonGetVersion>(export);
            return CallBuffer(fn, out text);
        }

        private static int TryCallHotkeysMode(string? url, out string text)
        {
            text = string.Empty;
            if (_libHandle == nint.Zero)
            {
                return -1100;
            }

            if (!NativeLibrary.TryGetExport(_libHandle, "hotkeys_mode", out nint export))
            {
                return -1101;
            }

            var fn = Marshal.GetDelegateForFunctionPointer<AddonHotkeysMode>(export);
            return CallBufferHotkeys(fn, url, out text);
        }

        private static int CallBuffer(AddonGetVersion fn, out string text)
        {
            nint buffer = Marshal.AllocHGlobal(4096);
            try
            {
                for (int i = 0; i < 4096; i++)
                {
                    Marshal.WriteByte(buffer, i, 0);
                }

                int rc = fn(buffer, 4096);

                text = ReadUtf8(buffer);
                return rc;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static int CallBufferHotkeys(AddonHotkeysMode fn, string? url, out string text)
        {
            nint buffer = Marshal.AllocHGlobal(4096);
            nint urlPtr = nint.Zero;
            try
            {
                for (int i = 0; i < 4096; i++)
                {
                    Marshal.WriteByte(buffer, i, 0);
                }

                if (!string.IsNullOrWhiteSpace(url))
                {
                    urlPtr = Marshal.StringToHGlobalAnsi(url);
                }

                int rc = fn(urlPtr, buffer, 4096);

                text = ReadUtf8(buffer);
                return rc;
            }
            finally
            {
                if (urlPtr != nint.Zero)
                {
                    Marshal.FreeHGlobal(urlPtr);
                }
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static string ReadUtf8(nint ptr)
        {
            var bytes = new System.Collections.Generic.List<byte>();
            int offset = 0;
            while (true)
            {
                byte b = Marshal.ReadByte(ptr, offset++);
                if (b == 0)
                {
                    break;
                }
                bytes.Add(b);
            }

            return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}
