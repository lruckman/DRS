using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using System;
using System.IO;
using System.Reflection;

namespace Web.Engine.Services
{
    public interface IPdfRasterizer
    {
        System.Drawing.Bitmap GetThumbnail(Stream input, int pageNumber);
    }

    public class PdfRasterizer : IPdfRasterizer
    {
        private readonly GhostscriptVersionInfo _versionInfo;
        private readonly GhostscriptRasterizer _rasterizer;

        public PdfRasterizer()
        {
            var ghostDllPath = Path.Combine(GetBinPath(), Environment.Is64BitProcess ? "gsdll64.dll" : "gsdll32.dll");

            _versionInfo = new GhostscriptVersionInfo(new Version(0, 0, 0), ghostDllPath, string.Empty,
                GhostscriptLicense.GPL);

            _rasterizer = new GhostscriptRasterizer();
        }

        private string GetBinPath()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;

            var uri = new UriBuilder(codeBase);

            string path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }

        public System.Drawing.Bitmap GetThumbnail(Stream input, int pageNumber)
        {
            try
            {
                _rasterizer.Open(input, _versionInfo, true);

                return new System.Drawing.Bitmap(_rasterizer.GetPage(200, 200, pageNumber));
            }
            finally
            {
                _rasterizer.Close();
            }
        }
    }
}
