using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class DadosMonitoramento
    {
        public int ProtocoloCarga { get; set; }
        public string NumeroCarga { get; set; }
        public IList<Monitoramento> Monitoramentos { get; set; }
    }
}