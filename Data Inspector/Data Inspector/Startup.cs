using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Data_Inspector.Startup))]
namespace Data_Inspector
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            int test = 0;
            test++;

        }
    }
}
