using System.Collections.Generic;
using System.Net;

namespace downloader
{
    public class Error
    {
        private int AffectedRow { get; set; }
        private string Message { get; set; }
        private HttpStatusCode StatusCode { get; set; }

        public Error(int affectedRow, HttpStatusCode statusCode, string message)
        {
            AffectedRow = affectedRow;
            StatusCode = statusCode;
            Message = message;
        }

        public static List<string> ToListString(List<Error> errors)
        {
            var result = new List<string>();
            foreach(var err in errors)
            {
                result.Add(err.ToString());
            }
            return result;
        }

        override public string ToString()
        {
            return $"URL number {AffectedRow} failed. HTTP status: {StatusCode}, Message: {Message}";
        }
    }
}
