using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaRelatorioDocumentoFaturamento
    {
        public int CodigoTransportador { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public int CodigoVeiculo { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int CodigoFilial { get; set; }
        public int ModeloDocumento { get; set; }
        public int NumeroDocumentoOriginario { get; set; }
        public int NumeroOcorrencia { get; set; }
        public int NumeroFatura { get; set; }

        public double CpfCnpjRemetente { get; set; }
        public double CpfCnpjDestinatario { get; set; }
        public List<double> CpfCnpjTomador { get; set; }

        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public DateTime DataAutorizacaoInicial { get; set; }
        public DateTime DataAutorizacaoFinal { get; set; }
        public DateTime DataCancelamentoInicial { get; set; }
        public DateTime DataCancelamentoFinal { get; set; }
        public DateTime DataAnulacaoInicial { get; set; }
        public DateTime DataAnulacaoFinal { get; set; }

        public decimal ValorInicial { get; set; }
        public decimal ValorFinal { get; set; }

        public string TipoPropriedadeVeiculo { get; set; }
        public string EstadoOrigem { get; set; }
        public string EstadoDestino { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public string NumeroOcorrenciaCliente { get; set; }
        public string TipoOperacao { get; set; }

        public List<int> GruposPessoas { get; set; }
        public List<int> TipoOcorrencia{ get; set; }
        public List<int> GruposPessoasDiferente { get; set; }
        public List<TipoFaturamentoRelatorioDocumentoFaturamento> TipoFaturamento { get; set; }
        public SituacaoDocumentoFaturamento? Situacao { get; set; }
        public TipoLiquidacaoRelatorioDocumentoFaturamento? TipoLiquidacao { get; set; }

        public bool? DocumentoComCanhotosRecebidos { get; set; }
        public bool? DocumentoComCanhotosDigitalizados { get; set; }
        public List<int> TipoCTe { get; set; }
        public DateTime DataLiquidacaoInicial { get; set; }
        public DateTime DataLiquidacaoFinal { get; set; }
        public DateTime DataBaseLiquidacaoInicial { get; set; }
        public DateTime DataBaseLiquidacaoFinal { get; set; }
        public List<int> TipoServico { get; set; }
    }
}
