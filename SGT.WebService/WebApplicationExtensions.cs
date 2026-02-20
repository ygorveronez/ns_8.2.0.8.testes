using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.Channels;
using System.Reflection;

namespace SGT.WebService;

public static class WebApplicationExtensions
{

    public static void RegisterWcfServices(this WebApplication app)
    {
        var serviceTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces().Any(i => i.GetCustomAttribute<ServiceContractAttribute>() != null));

        var url = app.Configuration.GetValue<string>("UrlBase");

        var useHttps = url.StartsWith("https", StringComparison.OrdinalIgnoreCase);

        // Define o BasicHttpBinding com segurança para HTTPS
        var binding = new BasicHttpBinding
        {
            MaxReceivedMessageSize = 10485760, // 10 MB
            MaxBufferSize = 10485760,          // Deve ser igual ou maior que MaxReceivedMessageSize
            Security = new BasicHttpSecurity
            {
                Mode = useHttps ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None,
                Transport = new HttpTransportSecurity
                {
                    ClientCredentialType = HttpClientCredentialType.None // Ajuste conforme necessário
                }
            }
        };

        app.UseServiceModel(builder =>
        {
            foreach (var serviceType in serviceTypes)
            {
                var _interface = serviceType.GetInterfaces().FirstOrDefault();
                if (_interface != null)
                {
                    builder.AddService(serviceType, options =>
                    {
                        options.BaseAddresses.Add(new Uri($"{url}/{serviceType.Name}.svc"));
                    });

                    var methods = builder.GetType().GetMethods().Where(x => x.Name == "AddServiceEndpoint").ToList();
                    MethodInfo methodInfo = methods.FirstOrDefault(x => x.GetParameters()[0].ParameterType == typeof(Binding));

                    MethodInfo genericMethod = methodInfo.MakeGenericMethod(serviceType, _interface);
                    genericMethod.Invoke(builder, new object[] { binding, string.Empty });
                }
            }
        });

        var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
        serviceMetadataBehavior.HttpGetEnabled = !useHttps;
        serviceMetadataBehavior.HttpsGetEnabled = useHttps;
    }

}