using System.Web.Http;
using WebActivatorEx;
using RFTestRecordManagementSystem_API;
using Swashbuckle.Application;

// �b���ε{���ҰʮɡA�|�۰ʩI�s SwaggerConfig.Register()
// ��M�ױҰʮɡAASP.NET �|�b Global.asax.cs���e�۰ʰ���o�Ӥ�k = SwaggerConfig.Register(); �۰ʰ���
[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace RFTestRecordManagementSystem_API
{
    public class SwaggerConfig
    {
        // �b���ε{���ҰʮɳQ�I�s�A�Ψӳ]�w Swagger �欰
        public static void Register()
        {
            // ���o���� Web API �]�w���� (�۷�� WebApiConfig.Register ���� config )
            var config = GlobalConfiguration.Configuration;

            // ���o�ثe�M�ײե��T(Assembly)�ASwagger �ݭn�o�Өӱ��y Controller
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            // �ҥ� Swagger �\��
            config
                // ���USwagger���֤ߪA�ȡA�|�۰ʫإ�
                // /swagger/docs/v1 �� Swagger JSON ���
                // /swagger/ui      �� Swagger UI   ����
                .EnableSwagger(c =>
                    {
                        // �ŧi API ��󪩥�"v1"�P API �W�١]��ܦb UI �������D�C�^
                        // �|��ܦb Swagger UI ��������
                        c.SingleApiVersion("v1", "RFTestRecordManagementSystem_API");
                    })
                // �ҥ� Swagger UI ��ı�Ƭɭ��A�A��z�L�s�������ʾާ@
                .EnableSwaggerUi(c =>
                    {
                        // ���� swagger.io �u�W���Ҿ�(�קK UI �L�����)
                        c.DisableValidator();

                        // UI �w�]�i�}����C�� ("List"�|��ܦU Controller�W��)
                        c.DocExpansion(DocExpansion.List);
                    });
        }
    }
}
