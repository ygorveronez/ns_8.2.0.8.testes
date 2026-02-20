using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class ConsultaQualidadeEntrega
    {
        public int CodigoCargaEntrega { get; set; }
        public int CodigoNotaFiscal { get; set; }
        public int CodigoCanhoto { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public DateTime DataEntradaRaio { get; set; }
        public DateTime DataSaidaRaio { get; set; }
        public string SerieNotaFiscal { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal TipoNotaFiscal { get; set; }
        public DateTime DataEmissaoNotaFiscal { get; set; }
        public string ChaveNotaFiscal { get; set; }
        public string NumeroCarga { get; set; }
        public string NomeFantasiaTransportador { get; set; }
        public string CNPJTransportador { get; set; }
        public string DescricaoTipoDeCarga { get; set; }
        public string NomeDestinatario { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public int ProtocoloCarga { get; set; }
        public string DescricaoFilial { get; set; }
        public string CNPJFilial { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto SituacaoCanhoto { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }
        public DateTime DataDigitalizacaoCanhoto { get; set; }
        public DateTime DataEntregaCliente { get; set; }
        public DateTime DataEntregaOriginal { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal SituacaoNotaFiscal { get; set; }
        public string CentroResultado { get; set; }
        public string OrigemDescricao { get; set; }
        public string DestinoDescricao { get; set; }
        public string EscritorioVendas { get; set; }
        public string MatrizVendas { get; set; }
        public TipoNotaFiscalIntegrada TipoNotaFiscalIntegrada { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto SituacaoPgtoCanhoto { get; set; }
        public bool DisponivelParaConsulta { get; set; }

        public string DescricaoSituacaoCanhoto
        {
            get
            {
                return this.SituacaoCanhoto.ObterDescricao();
            }
        }
        public string DescricaoDisponivelParaConsulta
        {
            get
            {
                return this.DisponivelParaConsulta ? "Liberado" : "Bloqueado";
            }
        }
        public string DescricaoSituacaoPgtoCanhoto
        {
            get
            {
                return this.SituacaoPgtoCanhoto.ObterDescricao();
            }
        }

        public string DescricaoSituacaoDigitalizacaoCanhoto
        {
            get
            {
                return this.SituacaoDigitalizacaoCanhoto.ObterDescricao();
            }
        }
        public string DescricaoSituacaoNotaFiscal
        {
            get
            {
                return this.SituacaoNotaFiscal.ObterDescricao();
            }
        }

        public string DescricaoTipoNotaFiscalIntegrada
        {
            get
            {
                return this.TipoNotaFiscalIntegrada.ObterDescricao();
            }
        }


        public string DescricaoTransportadorFormatada
        {
            get
            {
                return $"{this.NomeFantasiaTransportador} - {FormatarCPFCNPJ(this.CNPJTransportador)}";
            }
        }

        public string DescricaoFilialFormatada
        {
            get
            {
                return $"{this.DescricaoFilial} - {FormatarCPFCNPJ(this.CNPJFilial)}";
            }
        }

        public string DescricaoDestinatarioFormatada
        {
            get
            {
                return $"{this.NomeDestinatario} - {FormatarCPFCNPJ(this.CPFCNPJDestinatario.ToString())}";
            }
        }

        public string DataEntradaRaioFormatada
        {
            get
            {
                return FormatarData(this.DataEntradaRaio);
            }
        }

        public string DataSaidaRaioFormatada
        {
            get
            {
                return FormatarData(this.DataSaidaRaio);
            }
        }
        public string DataEmissaoNotaFiscalFormatada
        {
            get
            {
                return FormatarData(this.DataEmissaoNotaFiscal);
            }
        }
        public string DataDigitalizacaoCanhotoFormatada
        {
            get
            {
                return FormatarData(this.DataDigitalizacaoCanhoto);
            }
        }

        public string DataEntregaClienteFormatada
        {
            get
            {
                return FormatarData(this.DataEntregaCliente);
            }
        }
        public string DataEntregaOriginalFormatada
        {
            get
            {
                return FormatarData(this.DataEntregaOriginal);
            }
        }
        public string FormatarCPFCNPJ(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return string.Empty;

            if (documento.Length > 11)
                return string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(documento));
            else if (documento.Length == 11)
                return string.Format(@"{0:000\.000\.000\-00}", long.Parse(documento));

            return string.Empty;
        }
        public string FormatarData(DateTime data) { return (data != DateTime.MinValue && data != null) ? data.ToString() : string.Empty; }

    }
}
