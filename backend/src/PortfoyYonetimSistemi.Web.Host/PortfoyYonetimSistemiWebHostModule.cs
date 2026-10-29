using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PortfoyYonetimSistemi.EntityFrameworkCore;

namespace PortfoyYonetimSistemi.Web.Host
{
    [DependsOn(typeof(PortfoyYonetimSistemiApplicationModule), typeof(PortfoyYonetimSistemiEntityFrameworkCoreModule), typeof(AbpAspNetCoreModule))]
    public class PortfoyYonetimSistemiWebHostModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Expose all AppServices as dynamic API controllers
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(PortfoyYonetimSistemiApplicationModule).GetAssembly(),
                    moduleName: "app",
                    useConventionalHttpVerbs: true
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PortfoyYonetimSistemiWebHostModule).GetAssembly());
        }
    }
}
