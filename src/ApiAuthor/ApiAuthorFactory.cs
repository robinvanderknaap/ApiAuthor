using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http.Description;
using System.Xml.Linq;
using ApiAuthor.Specification;

namespace ApiAuthor
{
    public class ApiAuthorFactory
    {
        // http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx
        private static readonly Lazy<ApiAuthorFactory> Instance = new Lazy<ApiAuthorFactory>(() => new ApiAuthorFactory());
        
        private XDocument _xmlDocumentation;
        private IApiExplorer _apiExplorer;
        private ApiSpecification _apiSpecification;
        private Func<IApiExplorer, XDocument, IList<ResourceGrouping>> _resourceGroupings;

        private ApiAuthorFactory()
        {
        }

        public static void Configure(IApiExplorer apiExplorer, string xmlDocumentationPath)
        {
            Configure(apiExplorer, xmlDocumentationPath, null);
        }

        public static void Configure(IApiExplorer apiExplorer, string xmlDocumentationPath, Action<ApiAuthorFactory> configure)
        {
            // Sanity checks
            if (apiExplorer == null) throw new ArgumentNullException("apiExplorer");
            if (xmlDocumentationPath == null) throw new ArgumentNullException("xmlDocumentationPath");
            if (Instance.IsValueCreated) throw new InvalidOperationException("Api Author is already configured. Make sure to not call the configure method twice");

            Instance.Value._apiExplorer = apiExplorer;

            // Try to load xml documentation file
            try
            {
                Instance.Value._xmlDocumentation = XDocument.Load(xmlDocumentationPath);
            }
            catch (Exception exception)
            {
                throw new FileLoadException("Unable to load XML documentation. Make sure to enable XML documentation in the build properties of your API project and specifiy the correct path to the XML file when configuring ApiDiscloser.", xmlDocumentationPath, exception);
            }

            // Execute configuration action specified by consumer of factory if specified.
            if (configure != null)
            {
                configure(Instance.Value);
            }
        }

        public static ApiSpecification GetApiSpecification()
        {
            if (!Instance.IsValueCreated)
            {
                throw new InvalidOperationException("Api discloser has not been configured. Make sure to configure Api Discloser once during application start.");
            }

            return Instance.Value._apiSpecification ?? (Instance.Value._apiSpecification = Instance.Value.CreateApiSpecification());
        }

        public string ApiVersion { get; set; }

        /// <summary>
        /// Function to group API's. 
        /// </summary>
        /// <remarks>
        /// By default groups by controller
        /// </remarks>
        public Func<IApiExplorer, XDocument, IList<ResourceGrouping>> ResourceGroupings
        {
            get { return _resourceGroupings ?? (_resourceGroupings = DefaultResourceGroupings); }
            set { _resourceGroupings = value; }
        }

        private IList<ResourceGrouping> DefaultResourceGroupings(IApiExplorer apiExplorer, XDocument xmlDocumentation)
        {
            var resources = apiExplorer.ApiDescriptions.GroupBy(apiDescription => apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType.FullName);

            var resourceGroupings = new List<ResourceGrouping>();

            foreach (var resource in resources)
            {
                var controllerFullName = resource.Key;
                var apiDescription = apiExplorer.ApiDescriptions.First(x => x.ActionDescriptor.ControllerDescriptor.ControllerType.FullName == controllerFullName);

                var remarks = string.Empty;
                
                var controllerElement = _xmlDocumentation.Descendants("member").FirstOrDefault(x => x.Attribute("name").Value == string.Format("T:{0}", controllerFullName));

                if (controllerElement != null)
                {
                    var remarksElement = controllerElement.Descendants("remarks").FirstOrDefault();

                    if (remarksElement != null)
                    {
                        remarks = remarksElement.Value.Trim();
                    }
                }

                var resourceGrouping = new ResourceGrouping
                {
                    Name = apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName,
                    Remarks = remarks
                };

                resourceGroupings.Add(resourceGrouping);
            }

            return resourceGroupings;
            
            //return (explorer, document) => explorer.ApiDescriptions
            //            .GroupBy(apiDescription => apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName)
            //            .ToDictionary(apiDescriptions => apiDescriptions.Key, x => x.ToList());
        }

        private ApiSpecification CreateApiSpecification()
        {
            var resourceGroupings = ResourceGroupings(_apiExplorer, _xmlDocumentation);

            var apiSpecification = new ApiSpecification
            {
                ApiVersion = string.IsNullOrWhiteSpace(ApiVersion) ? "1.0" : ApiVersion
            };

            foreach (var resourceGrouping in resourceGroupings)
            {
                apiSpecification.Resources.Add(new Resource
                {
                    Name = resourceGrouping.Name,
                    Remarks = resourceGrouping.Remarks
                });
            }

            return apiSpecification;
        }
    }
}
