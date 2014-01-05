using System.Web.Http;
using ApiAuthor.Specification;

namespace ApiAuthor.Example.Controllers
{
    /// <summary>
    /// Get API documentation
    /// </summary>
    /// <remarks>Use this resource to retrieve the documentation in the Api Author format. This output can be used to generate the user interface.</remarks>
    public class DocumentationController : ApiController
    {
        /// <summary>
        /// Retrieve documentation
        /// </summary>
        /// <returns>API Author specification</returns>
        public ApiSpecification Get()
        {
            return ApiAuthorFactory.GetApiSpecification();
        }
    }
}