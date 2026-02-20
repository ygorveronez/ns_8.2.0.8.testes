using Hangfire.Common;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGT.Hangfire.Threads
{
    public class JobFinalizadoFilterAttribute : JobFilterAttribute, IServerFilter
    {
        public void OnPerforming(PerformingContext filterContext)
        {
            // Este método é executado ANTES do job iniciar
            // Se precisar fazer algo pré-execução, faça aqui.
        }

        public void OnPerformed(PerformedContext filterContext)
        {
           //Exemplo aqui pode fazer algo assim que o job finalizar
        }
    }
}
