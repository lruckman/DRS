using StructureMap;
using Web.Engine.Services;
using Web.Engine.Services.Lucene;
using Web.Engine.Services.Lucene.Definitions;
using Web.Models;

namespace Web.Engine
{
    public class WebRegistry : Registry
    {
        public WebRegistry()
        {
            Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();
                scanner.LookForRegistries();
                scanner.AssemblyContainingType<WebRegistry>();

                For<IFileStorage>().Use<EncryptedFileStorage>();
                For<IFileIndexer>().Use<IndexService>();
                For<IIndexDefinition<Revision>>().Use<RevisionDefinition>();
                For<IFileSearcher>().Use<SearchService>();
            });
        }
    }
}