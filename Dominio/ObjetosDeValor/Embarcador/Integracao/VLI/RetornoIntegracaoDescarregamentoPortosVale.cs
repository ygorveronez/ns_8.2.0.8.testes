using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VLI
{
    public class RetornoIntegracaoDescarregamentoPortosVale
    {
        public DataDescarregamentoPortosVale[] Data { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class DataDescarregamentoPortosVale
    {
        public string IdVeiculo { get; set; }
        public string CodigoTerminalTransbordo { get; set; }
        public string DescricaoTerminalTransbordo { get; set; }
        public string CnpjTerminalTransbordo { get; set; }
        public string NomeFantasiaTerminalDestino { get; set; }
        public string RazaoSocialTerminalDestino { get; set; }
        public string CnpjTerminalDestino { get; set; }
        public DateTime? DataSaida { get; set; }
        public DateTime? DescargaTermino { get; set; }
        public string NumeroPlaca { get; set; }
        public double PesoOrigem { get; set; }
        public double PesoBruto { get; set; }
        public double PesoTara { get; set; }
        public double PesoLiquido { get; set; }
        public List<NfeDescarregamentoPortosVale> ListaNfes { get; set; }
    }

    public class NfeDescarregamentoPortosVale
    {
        public string ChaveNfe { get; set; }
        public DateTime DataEmissao { get; set; }
        public string IdVeiculo { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string SerieNotaFiscal { get; set; }
        public double PesoDeclarado { get; set; }
        public double PesoDescarregado { get; set; }
    }
}
