using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Data_Inspector1.Startup))]
namespace Data_Inspector1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
