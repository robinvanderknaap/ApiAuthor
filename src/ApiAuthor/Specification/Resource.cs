using System.Collections.Generic;

namespace ApiAuthor.Specification
{
    public class Resource
    {
        private readonly IList<ResourceMethod> _resourceMethods = new List<ResourceMethod>();
        
        public string Name { get; set; }
        
        public string Remarks { get; set; }

        public IList<ResourceMethod> ResourceMethods
        {
            get { return _resourceMethods; }
        }
    }
}