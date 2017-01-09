using StructureMap;

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
            });
        }
    }
}