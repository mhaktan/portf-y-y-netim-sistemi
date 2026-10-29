using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace PortfoyYonetimSistemi
{
    [DependsOn(typeof(PortfoyYonetimSistemiCoreModule), typeof(AbpAutoMapperModule))]
    public class PortfoyYonetimSistemiApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                cfg.AddMaps(typeof(PortfoyYonetimSistemiApplicationModule).GetAssembly());
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PortfoyYonetimSistemiApplicationModule).GetAssembly());
        }
    }
}
