using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaTermoQuitacao
    {
        public int NumeroTermo { get; set; }
        public int Transportador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime DataVigenciaInicial { get; set; }
        public DateTime DataVigenciaFinal { get; set; }
        public bool? ProvisaoPendente { get; set; }
        public SituacaoAlcadaRegra? SitaucaoAprovacaoProvisao { get; set; }
        public SituacaoTermoQuitacaoFinanceiro SitaucaoTermoQuitacao { get; set; }
        public SituacaoAprovacaoTermoQuitacaoTransportador SituacaoAprovacaoTransportador { get; set; }
    }
}
