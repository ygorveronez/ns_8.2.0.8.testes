using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FiltroPesquisaRelatorioContratoFreteTransportador
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public int TipoContratoFrete { get; set; }
        public int DiasParaVencimento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }
        public bool EmVigencia { get; set; }
        public string NumeroEmbarcador { get; set; }
        public DateTime DataConsultaInicial { get; set; }
        public DateTime DataConsultaFinal { get; set; }
    }
}
