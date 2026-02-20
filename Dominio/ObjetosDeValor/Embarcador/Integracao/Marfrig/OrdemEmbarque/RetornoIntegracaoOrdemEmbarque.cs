using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public class RetornoIntegracaoOrdemEmbarque
    {
        public string DataHora { get; set; }

        public string NumeroOrdemEmbarque { get; set; }

        public int ProtocoloTMSCarga { get; set; }

        public bool Validado { get; set; }

        public List<RetornoIntegracaoOrdemEmbarqueValidacao> Validacoes { get; set; }

        public string Mensagem
        {
            get { return (Validacoes?.Count > 0) ? string.Join(" | ", Validacoes.Select(o => o.Mensagem)) : string.Empty; }
        }
    }
}
