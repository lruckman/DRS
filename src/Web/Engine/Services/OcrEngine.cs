using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.IO;
using Tesseract;

namespace Web.Engine.Services
{
    public interface IOcrEngine : IDisposable
    {
        string GetText(Bitmap bitmap);
    }

    public class OcrEngine : IOcrEngine
    {
        private readonly TesseractEngine _engine;

        public OcrEngine(IOptions<Config> config)
        {
            var args = config?.Value;

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            
            if (!Directory.Exists(args.DataPath))
            {
                throw new ArgumentException("Path does not exist or access is denied.", nameof(args.DataPath));
            }

            _engine = new TesseractEngine(args.DataPath, args.Lang, EngineMode.Default);
        }

        public string GetText(Bitmap bitmap)
        {
            using (var pix = PixConverter.ToPix(bitmap))
            {
                using (var page = _engine.Process(pix))
                {
                    return page.GetText();
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _engine?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public class Config
        {
            public string DataPath { get; set; }
            public string Lang { get; set; }
        }
    }
}
