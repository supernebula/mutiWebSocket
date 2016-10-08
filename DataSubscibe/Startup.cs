using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DataSubscibe.Startup))]
namespace DataSubscibe
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
