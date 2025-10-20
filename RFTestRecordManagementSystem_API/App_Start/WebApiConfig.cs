using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace RFTestRecordManagementSystem_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // 預設回傳 JSON
            // 當瀏覽器(or Swagger)發出請求時，即使它的標頭(Header)指定Accpt： text/html
            // 系統仍然使用JSON格式來回應
            // 瀏覽器預設會要求 HTML(不是JSON)
            // 如果不加這行，Web API會回傳一段 XML(不好看)
            // 加上這行後，就會自動用JSON呈現資料
            //  config.Formatters.JsonFormatter 取得 Web API的「JSON 格式處理器 (Formatter)」，負責把 C# 物件轉換成 JSON 格式。
            // .SupportedMediaTypes 是一個集合（List），裡面存有哪些「MIME 類型 (Media Type)」可用。
            // 建立一個 HTTP 標頭型別物件，表示要支援 text/html。
            // 告訴 Web API：「即使前端要求 HTML 格式，也要使用 JSON 格式回傳」。
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            // 把 XML 輸出功能移除，
            // 讓 API 只輸出 JSON，不再輸出 XML。
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
