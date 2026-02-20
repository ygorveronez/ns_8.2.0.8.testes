using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.RPS
{
    public class NotaServico
    {
        public string TipoRegistro { get; set; }
        public string InscricaoContribuinte { get; set; }
        public string VersaoLayOut { get; set; }
        public string IdentificacaoRemessa { get; set; }
        public List<Detalhe> Detalhes { get; set; }
        public Rodape Rodape { get; set; }        
    }

    public class Detalhe
    {
        public string TipoRegistro { get; set; }
        public string TipoRPS { get; set; }
        public string SerieRPS { get; set; }
        public string SerieNFe { get; set; }
        public string NumeroRPS { get; set; }
        public DateTime DataRPS { get; set; }
        public TimeSpan HoraRPS { get; set; }
        public string SituacaoRPS { get; set; }
        public string CodigoMotivoCancelamento { get; set; }
        public string NumeroNFeSubstituida { get; set; }
        public string SerieNFeSubstituida { get; set; }
        public DateTime DataNFeSubstituida { get; set; }
        public string DescricaoCancelamento { get; set; }
        public string CodigoServicoPrestado { get; set; }
        public string LocalServicoPrestado { get; set; }
        public string ServicoPrestadoViasPublicas { get; set; }
        public string EnderecoServicoPrestado { get; set; }
        public string NumeroServicoPrestado { get; set; }
        public string ComplementoServicoPrestado { get; set; }
        public string BairroServicoPrestado { get; set; }
        public string CidadeServicoPrestado { get; set; }
        public string UFServicoPrestado { get; set; }
        public string CEPServicoPrestado { get; set; }
        public decimal QuantidadeServicoPrestado { get; set; }
        public decimal ValorServicoPrestado { get; set; }
        public string Reservado { get; set; }
        public decimal ValorTotalRetencoes { get; set; }
        public string TomadorEstrangeiro { get; set; }
        public string PaisTomadorEstrangeiro { get; set; }
        public string ServicoExportacao { get; set; }
        public string IndicadorCPFCNPJTomador { get; set; }
        public string CPFCNPJTomador { get; set; }
        public string NomeTomador { get; set; }
        public string EnderecoTomador { get; set; }
        public string NumeroTomador { get; set; }
        public string ComplementoTomador { get; set; }
        public string BairroTomador { get; set; }
        public string CidadeTomador { get; set; }
        public string UFTomador { get; set; }
        public string CEPTomador { get; set; }
        public string EmailTomador { get; set; }
        public string Fatura { get; set; }
        public decimal ValorFatura { get; set; }
        public string FormaPagamento { get; set; }
        public string DiscriminacaoServico { get; set; }
        public List<Valore> Valores { get; set; }
    }

    public class Valore
    {
        public string TipoRegistro { get; set; }
        public string CodigoOutrosValores { get; set; }
        public decimal Valor { get; set; }
    }

    public class Rodape
    {
        public string TipoRegistro { get; set; }
        public int NumeroLinhas { get; set; }
        public decimal ValorTotalServicos { get; set; }
        public decimal ValorTotalValores { get; set; }
    }
}
