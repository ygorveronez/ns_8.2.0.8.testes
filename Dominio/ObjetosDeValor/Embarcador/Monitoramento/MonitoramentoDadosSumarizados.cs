using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Monitoramento
{
    public class MonitoramentoDadosSumarizados
    {
        public string Resultado { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> Posicoes { get; set; }
    }
}
