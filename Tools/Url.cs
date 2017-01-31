namespace Tools
{
    public class Url
    {
        public string Uri { get; set; }
        public int Id { get; set; }
        public string Content { get; set; }

        public Url(string uri, int id)
        {
            Uri = uri;
            Id = id;
        }
    }
}
