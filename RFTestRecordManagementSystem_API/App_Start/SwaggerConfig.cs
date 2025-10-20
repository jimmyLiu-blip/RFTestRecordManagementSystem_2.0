using System.Web.Http;
using WebActivatorEx;
using RFTestRecordManagementSystem_API;
using Swashbuckle.Application;

// 在應用程式啟動時，會自動呼叫 SwaggerConfig.Register()
// 當專案啟動時，ASP.NET 會在 Global.asax.cs之前自動執行這個方法 = SwaggerConfig.Register(); 自動執行
[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace RFTestRecordManagementSystem_API
{
    public class SwaggerConfig
    {
        // 在應用程式啟動時被呼叫，用來設定 Swagger 行為
        public static void Register()
        {
            // 取得全域 Web API 設定物件 (相當於 WebApiConfig.Register 中的 config )
            var config = GlobalConfiguration.Configuration;

            // 取得目前專案組件資訊(Assembly)，Swagger 需要這個來掃描 Controller
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            // 啟用 Swagger 功能
            config
                // 註冊Swagger的核心服務，會自動建立
                // /swagger/docs/v1 → Swagger JSON 文件
                // /swagger/ui      → Swagger UI   頁面
                .EnableSwagger(c =>
                    {
                        // 宣告 API 文件版本"v1"與 API 名稱（顯示在 UI 頂部標題列）
                        // 會顯示在 Swagger UI 頁面頂部
                        c.SingleApiVersion("v1", "RFTestRecordManagementSystem_API");
                    })
                // 啟用 Swagger UI 視覺化界面，，能透過瀏覽器互動操作
                .EnableSwaggerUi(c =>
                    {
                        // 停用 swagger.io 線上驗證器(避免 UI 無限轉圈)
                        c.DisableValidator();

                        // UI 預設展開控制器列表 ("List"會顯示各 Controller名稱)
                        c.DocExpansion(DocExpansion.List);
                    });
        }
    }
}
