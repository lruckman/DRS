namespace Web.Engine
{
    public static class Constants
    {
        public const string RelativeDocumentPath = "app_data\\documents";
        public const string ThumbnailExtension = ".thm";

        public const string LuceneFieldOCR = "__contents__";
        public const string LuceneFieldTitle = "__title__";
        public const string LuceneFieldAbstract = "__abstract__";
        public const string LuceneFieldId = "id";

        public const int SearchResultsPageSize = 25;
    }
}