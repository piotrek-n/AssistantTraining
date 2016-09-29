using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AssistantTraining.Startup))]
namespace AssistantTraining
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}