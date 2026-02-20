using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class FiltroPesquisaAvaliacaoEntrega
    {
        public string NumeroCarga { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public List<double> CNPJsDestinatario { get; set; }
        public string NumeroTransporte { get; set; }
        public int? Respondida { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega? SituacaoEntrega { get; set; }
    }
}
