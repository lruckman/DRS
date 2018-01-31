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
        private readonly string _dataPath;
        private readonly string _lang;
        private TesseractEngine _engine;

        private TesseractEngine Engine
        {
            get
            {
                return _engine ?? (_engine = new TesseractEngine(_dataPath, _lang, EngineMode.Default));
            }
        }

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

            // we cannot initialize tesseract here because if we are doing it out of a normal 
            // context it won't be able to resolve the dlls needed to load.

            _dataPath = args.DataPath ?? throw new ArgumentNullException(nameof(args.DataPath));
            _lang = args.Lang ?? throw new ArgumentNullException(nameof(args.Lang));
        }

        public string GetText(Bitmap bitmap)
        {
            using (var pix = PixConverter.ToPix(bitmap))
            {
                using (var page = Engine.Process(pix))
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
