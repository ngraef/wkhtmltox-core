using System;
using System.Runtime.InteropServices;

namespace WkHtmlToX
{
    internal static class Native
    {
        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_init")]
        public static extern bool Initialize(bool useGraphics);
        
        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_deinit")]
        public static extern bool Destroy();

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_create_global_settings")]
        public static extern IntPtr CreateGlobalSettings();

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_destroy_global_settings")]
        public static extern void DestroyGlobalSettings(IntPtr settings);

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_create_object_settings")]
        public static extern IntPtr CreateObjectSettings();

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_set_global_setting")]
        public static extern bool SetGlobalSetting(IntPtr settings, byte[] name, byte[] value);

        public static bool SetGlobalSetting(IntPtr settings, string name, string value)
        {
            return SetGlobalSetting(settings, StringToUTF8Bytes(name), StringToUTF8Bytes(value));
        }

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_get_global_setting")]
        public static extern bool GetGlobalSetting(IntPtr settings, byte[] name, byte[] value, int len);

        public static string GetGlobalSetting(IntPtr settings, string name)
        {
            var sb = new byte[32];
            var result = GetGlobalSetting(settings, StringToUTF8Bytes(name), sb, sb.Length);
            return result ? StringFromUTF8Bytes(sb) : null;
        }

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_set_object_setting")]
        public static extern bool SetObjectSetting(IntPtr settings, byte[] name, byte[] value);

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_get_object_setting")]
        public static extern bool GetObjectSetting(IntPtr settings, byte[] name, byte[] value, int len);

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_create_converter")]
        public static extern IntPtr CreateConverter(IntPtr globalSettings);

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_destroy_converter")]
        public static extern void DestroyConverter(IntPtr converter);

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_add_object")]
        public static extern void AddObject(IntPtr converter, IntPtr objectSettings, byte[] html);

        public static void AddObject(IntPtr converter, IntPtr objectSettings, string html)
        {
            AddObject(converter, objectSettings, StringToUTF8Bytes(html));
        }

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_convert")]
        public static extern bool Convert(IntPtr converter);

        [DllImport("wkhtmltox", EntryPoint = "wkhtmltopdf_get_output")]
        public static extern Int32 GetOutput(IntPtr converter, out IntPtr data);

        public static byte[] GetOutput(IntPtr converter)
        {
            var dataPtr = IntPtr.Zero;
            var dataLength = GetOutput(converter, out dataPtr);

            var data = new byte[dataLength];
            // This Marshal.Copy overload not yet available in dotnet core?
            //Marshal.Copy(dataPtr, data, 0, dataLength);
            for (int i = 0; i < dataLength; i++)
            {
                data[i] = Marshal.ReadByte(dataPtr, i);
            }
            dataPtr = IntPtr.Zero;
            return data;
        }


        private static byte[] StringToUTF8Bytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        private static string StringFromUTF8Bytes(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
