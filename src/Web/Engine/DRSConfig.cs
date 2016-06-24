using System.IO;

namespace Web.Engine
{
    public class DRSConfig
    {
        private string _documentDirectory;
        private string _tessDataDirectory;

        public string BaseDirectory { get; set; } = "";

        public string DocumentDirectory
        {
            get
            {
                if ((_documentDirectory?.StartsWith("~")).GetValueOrDefault())
                {
                    _documentDirectory = Path.Combine(BaseDirectory, _documentDirectory.TrimStart('~'));
                }
                return _documentDirectory;
            }
            set { _documentDirectory = value; }
        }

        public string TessDataDirectory
        {
            get
            {
                if ((_tessDataDirectory?.StartsWith("~")).GetValueOrDefault())
                {
                    _tessDataDirectory = Path.Combine(BaseDirectory, _tessDataDirectory.TrimStart('~'));
                }
                return _tessDataDirectory;
            }
            set { _tessDataDirectory = value; }
        }
    }
}