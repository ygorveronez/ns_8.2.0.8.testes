using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracoes
{
    public sealed class FiltroPesquisaIntegracaoEnvioProgramado
    {
        public string NumeroCarga { get; set; }
        public int NumeroCTE { get; set; }
        public int NumeroOcorrencia { get; set; }
        public SituacaoIntegracao? SituacaoIntegracao { get; set; }
        public TipoEntidadeIntegracao? TipoEntidadeIntegracao { get; set; }
        public DateTime DataIntegracaoInicial { get; set; }
        public DateTime DataIntegracaoFinal { get; set; }
        public DateTime DataProgramadaInicial { get; set; }
        public DateTime DataProgramadaFinal { get; set; }
        public DateTime DataCriacaoCargaInicial { get; set; }
        public DateTime DataCriacaoCargaFinal { get; set; }
        public DateTime DataEmissaoCTEInicial { get; set; }
        public DateTime DataEmissaoCTEFinal { get; set; }

    }
}
