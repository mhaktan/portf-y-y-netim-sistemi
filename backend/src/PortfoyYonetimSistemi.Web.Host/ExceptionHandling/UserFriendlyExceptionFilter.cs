using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.UI;
using Abp.Web.Configuration;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PortfoyYonetimSistemi.Web.Host.ExceptionHandling
{
    /// <summary>Kullaniciya gosterilen is kurali hatalari 500 degil 400 doner</summary>
    public class UserFriendlyExceptionFilter : AbpExceptionFilter
    {
        public UserFriendlyExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IAbpAspNetCoreConfiguration configuration, IAbpWebCommonModuleConfiguration abpWebCommonModuleConfiguration)
            : base(errorInfoBuilder, configuration, abpWebCommonModuleConfiguration) { }

        protected override int GetStatusCode(ExceptionContext context, bool wrapOnError)
            => context.Exception is UserFriendlyException ? 400 : base.GetStatusCode(context, wrapOnError);
    }
}
