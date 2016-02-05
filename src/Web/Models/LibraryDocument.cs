namespace Web.Models
{
    public class LibraryDocument
    {
        public int LibraryId { get; set; }
        public int DocumentId { get; set; }
        public Library Library { get; set; }
        public Document Document { get; set; }
    }
}