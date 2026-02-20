using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioTakeOrPay
    {
        public DateTime DataInicialFatura { get; set; }
        public DateTime DataFinalFatura { get; set; }
        public int NumeroFatura { get; set; }
        public string NumeroBoleto { get; set; }
        public SituacaoFatura? SituacaoFatura { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public List<TipoPropostaMultimodal> TipoProposta { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public int CodigoPortoDestino { get; set; }
        public int CodigoViagem { get; set; }
        public DateTime DataInicialPrevisaoSaidaNavio { get; set; }
        public DateTime DataFinalPrevisaoSaidaNavio { get; set; }
    }
}
