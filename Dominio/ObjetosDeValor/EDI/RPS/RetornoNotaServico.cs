using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.RPS
{
    public class RetornoNotaServico
    {
        public string TipoRegistro { get; set; }
        public string InscricaoContribuinte { get; set; }
        public DateTime DataInicioTransferencia { get; set; }
        public DateTime DataTerminoTransferencia { get; set; }
        public string VersaoLayout { get; set; }
        public string IdentificacaoRemessa { get; set; }
        public List<RetornoDetalhe> Detalhes { get; set; }
        public Rodape Rodape { get; set; }
    }

    public class RetornoDetalhe
    {
        public string TipoRegistro { get; set; }
        public string SerieNFe { get; set; }
        public string NumeroNFe { get; set; }
        public DateTime DataNFe { get; set; }
        public DateTime HoraNFe { get; set; }
        public string CodigoAutencidade { get; set; }
        public string SerieRPS { get; set; }
        public string NumeroRPS { get; set; }
        public string Tributacao { get; set; }
        public string ISSRetido { get; set; }
        public string SituacaoNFe { get; set; }
        public DateTime DataCancelamentoNFe { get; set; }
        public string NumeroGuia { get; set; }
        public DateTime DataPagamentoGuia { get; set; }
        public string CPFCNPJTomador { get; set; }
        public string NomeTomador { get; set; }
        public string EnderecoTomador { get; set; }
        public string NumeroTomador { get; set; }
        public string ComplementoTomador { get; set; }
        public string BairroTomador { get; set; }
        public string CidadeTomador { get; set; }
        public string UFTomador { get; set; }
        public string CEPTomador { get; set; }
        public string PaisTomador { get; set; }
        public string EmailTomador { get; set; }
        public string DiscriminacaoServico { get; set; }
        public List<ItensRetornoNotaServico> ItensRetornoNotaServico { get; set; }
        public List<Valore> Valores { get; set; }
    }

    public class ItensRetornoNotaServico
    {
        public string TipoRegistro { get; set; }
        public string QuantidadeServico { get; set; }
        public string DescricaoServico { get; set; }
        public string CodigoServico { get; set; }
        public decimal ValorServico { get; set; }
        public decimal AliquotaServico { get; set; }
    }
}
