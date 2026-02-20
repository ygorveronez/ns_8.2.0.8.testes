using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace GerenciadorAppService
{
    public class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<Servico>(service =>
                {
                    service.ConstructUsing(s => new Servico());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Configure a Conta que o serviço do Windows usa para rodar
                configure.StartAutomatically();
                configure.RunAsLocalSystem();
                configure.SetServiceName("GerenciadorAppService");
                configure.SetDisplayName("GerenciadorAppService");
                configure.SetDescription("Cria serviços para os processos do GerenciadorApp ");


                configure.EnableServiceRecovery(serviceRecovery =>
                {
                    // first failure, 5 minute delay
                    serviceRecovery.RestartService(1);

                    // second failure, 10 minute delay
                    //serviceRecovery.RunProgram(10, @"C:\Windows\Notepad.exe");

                    // subsequent failures, 15 minute delay
                    //serviceRecovery.RestartComputer(15, "Topshelf demo failure");
                });

            });
        }
    }
}
