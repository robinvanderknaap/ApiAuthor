using System.Collections.Generic;

namespace ApiAuthor.Specification
{
    public class ApiSpecification
    {
        private readonly IList<Resource> _resources = new List<Resource>();
        
        public string ApiVersion { get; set; }

        public IList<Resource> Resources
        {
            get { return _resources; }
        }
    }
}
