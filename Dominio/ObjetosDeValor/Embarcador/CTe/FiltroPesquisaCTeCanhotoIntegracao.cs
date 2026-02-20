using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class FiltroPesquisaCTeCanhotoIntegracao
    {
        public int CodigoCarga { get; set; }
        public int Emitente { get; set; }
        public int NumeroDocumento { get; set; }
        public TipoRegistroIntegracaoCTeCanhoto? TipoRegistro { get; set; }
        public SituacaoIntegracao? Situacao { get; set; }
        public DateTime DataEmissaoNFeInicial { get; set; }
        public DateTime DataEmissaoNFeFinal { get; set; }
        public DateTime DataEntregaInicial { get; set; }
        public DateTime DataEntregaFinal { get; set; }
        public DateTime DataDigitalizacaoInicial { get; set; }
        public DateTime DataDigitalizacaoFinal { get; set; }
        public DateTime DataAprovacaoInicial { get; set; }
        public DateTime DataAprovacaoFinal { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}