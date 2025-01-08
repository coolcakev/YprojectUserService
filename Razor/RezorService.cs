using RazorLight;

namespace YprojectUserService.Razor
{
    public class RazorRenderer
    {
        private readonly RazorLightEngine _engine;

        public RazorRenderer()
        {
            var projectRoot = Directory.GetCurrentDirectory();
            var templatesFolder = Path.Combine(projectRoot, "Razor", "Templates");

            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(templatesFolder) 
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderAsync<TModel>(string fileName, TModel model)
        {
            var result = await _engine.CompileRenderAsync(fileName, model);
            return result;
        }
    }
}