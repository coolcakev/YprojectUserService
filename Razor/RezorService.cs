using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging.Abstractions;

namespace YprojectUserService.Razor
{
    public class RazorRenderer
    {
        private readonly IServiceProvider _serviceProvider;

        public RazorRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderAsync<TComponent>(Dictionary<string, object?> parameters) where TComponent : IComponent
        {
            using var htmlRenderer = new HtmlRenderer(_serviceProvider, NullLoggerFactory.Instance);

            var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var parameterView = ParameterView.FromDictionary(parameters);
                    //TODO чому ти тут не авейтаєш, а знизу евейтаєш?
                var renderFragment = htmlRenderer.RenderComponentAsync<TComponent>(parameterView);
                return (await renderFragment).ToHtmlString();
            });

            return html;
        }
    }
}