using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaRelatorioPracaPedagio
    {
        public string Descricao { get; set; }
        public string Rodovia { get; set; }
        public DateTime DataTarifaInicial { get; set; }
        public DateTime DataTarifaFinal { get; set; }
        public Enumeradores.SituacaoAtivoPesquisa Status { get; set; }
    }
}
