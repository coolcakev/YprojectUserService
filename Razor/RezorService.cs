using RazorLight;

namespace YprojectUserService.Razor
{
    public class RazorRenderer
    {
        private readonly RazorLightEngine _engine;

        public RazorRenderer()
        {
            var projectRoot = Directory.GetCurrentDirectory();
            var templatesFolder = Path.Combine(projectRoot);

            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(templatesFolder) 
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderAsync<TModel>(string filePath, TModel model)
        {
            var result = await _engine.CompileRenderAsync(filePath, model);
            return result;
        }
    }
}