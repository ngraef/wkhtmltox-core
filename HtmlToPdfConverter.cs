using System;
using System.Collections.Generic;

namespace WkHtmlToX
{
    internal static class TupleListExtensions
    {
        public static void Add<T, U>(this IList<Tuple<T, U>> list, T item1, U item2)
        {
            list.Add(Tuple.Create(item1, item2));
        }
    }


    public sealed class HtmlToPdfConverter : IDisposable
    {
        private IntPtr _globalSettings;

        public HtmlToPdfConverter(PdfGlobalSettings settings = default(PdfGlobalSettings))
        {
            if (Native.Initialize(false))
            {
                _globalSettings = Native.CreateGlobalSettings();
                UpdateSettings(settings);
            }
        }

        private void UpdateSettings(PdfGlobalSettings s)
        {
            var update = new List<Tuple<string, string>> {
                { "size.pageSize", Enum.GetName(typeof(PageSize), s.PageSize) },
                { "orientation", Enum.GetName(typeof(Orientation), s.Orientation) },
                { "colorMode", Enum.GetName(typeof(ColorMode), s.ColorMode) },
                { "useCompression", s.UseCompression.ToString().ToLower() },
                { "margin.top", s.MarginTop },
                { "margin.right", s.MarginRight },
                { "margin.bottom", s.MarginBottom },
                { "margin.left", s.MarginLeft }
            };
            
            foreach (var setting in update)
            {
                Native.SetGlobalSetting(_globalSettings, setting.Item1, setting.Item2);
            }
        }

        public byte[] ConvertToPDF(string html)
        {
            var converter = Native.CreateConverter(_globalSettings);
            var objectSettings = Native.CreateObjectSettings();
            Native.AddObject(converter, objectSettings, html);

            var convertResult = Native.Convert(converter);
            var data = Native.GetOutput(converter);

            // destroy object settings too?
            Native.DestroyConverter(converter);

            return data;
        }


        private bool disposedValue = false; // To detect redundant calls

        private void Cleanup(bool disposing)
        {
            if (!disposedValue)
            {
                // if (disposing)
                // {
                //     // dispose managed state
                // }

                Native.DestroyGlobalSettings(_globalSettings);
                _globalSettings = IntPtr.Zero;

                Native.Destroy();

                disposedValue = true;
            }
        }

        ~HtmlToPdfConverter()
        {
            Cleanup(false);
        }

        public void Dispose()
        {
            Cleanup(true);
            GC.SuppressFinalize(this);
        }
    }

    public class PdfGlobalSettings
    {
        //public Size size;
        public PageSize PageSize = PageSize.Letter;
        // public bool quiet;
        // public bool useGraphics;
        // public bool resolveRelativeLinks;
        public Orientation Orientation = Orientation.Portrait;
        public ColorMode ColorMode = ColorMode.Color;
        // public string resolution;
        // public int dpi;
        // public int pageOffset;
        // public int copies;
        // public bool collate;
        // public bool outline;
        // public int outlineDepth;
        // public string dumpOutline;
        // public string @out;
        // public string documentTitle;
        public bool UseCompression = true;
        //public Margin margin;
        public string MarginTop = "0.5in";
        public string MarginRight = "0.5in";
        public string MarginBottom = "0.5in";
        public string MarginLeft = "0.5in";
        // public string viewportSize;
        // public int imageDPI;
        // public int imageQuality;
    }

    public enum Orientation
    {
        Portrait,
        Landscape
    }

    public enum ColorMode
    {
        Grayscale,
        Color
    }

    public enum PageSize
    {
        Letter,
        Legal
    }
}
