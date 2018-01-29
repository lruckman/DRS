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
        private static string _engLanguage = "eng";
        private readonly TesseractEngine _engine;

        public OcrEngine(string tesseractDataPath)
        {
            if (tesseractDataPath == null)
            {
                throw new ArgumentNullException(nameof(tesseractDataPath));
            }

            if (!Directory.Exists(tesseractDataPath))
            {
                throw new ArgumentException("Path does not exist or access is denied.", nameof(tesseractDataPath));
            }

            _engine = new TesseractEngine(tesseractDataPath, _engLanguage, EngineMode.Default);
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
    }
}
