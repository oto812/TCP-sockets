using System;

public class HttpResponse
    {
        public int StatusCode { get; set; }
        public string StatusText { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
}

