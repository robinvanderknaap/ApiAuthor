using System.Collections.Generic;

namespace ApiAuthor.Specification
{
    public class ResourceMethod
    {
        private readonly IList<HttpStatusCode> _httpStatusCodes = new List<HttpStatusCode>();
        
        public string Uri { get; set; }
        public string Example { get; set; }

        public IList<HttpStatusCode> HttpStatusCodes
        {
            get { return _httpStatusCodes; }
        }
    }
}