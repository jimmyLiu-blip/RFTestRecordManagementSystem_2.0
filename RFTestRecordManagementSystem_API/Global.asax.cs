using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RFTestRecordManagementSystem_API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            // 先註冊 Web API 路由
            GlobalConfiguration.Configure(WebApiConfig.Register);
            // 不要再手動呼叫 SwaggerConfig.Register()，會導致路由重複
            // 因為 WebActivatorEx 已經會自動執行
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
