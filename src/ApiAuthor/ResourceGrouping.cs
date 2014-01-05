using System.Collections.Generic;
using System.Web.Http.Description;

namespace ApiAuthor
{
    public class ResourceGrouping
    {
        public string Name { get; set; }
        public string Remarks { get; set; }
        public IList<ApiDescription> ApiDescriptions { get; set; }
    }
}
