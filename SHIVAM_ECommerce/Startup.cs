using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SHIVAM_ECommerce.Startup))]
namespace SHIVAM_ECommerce
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
