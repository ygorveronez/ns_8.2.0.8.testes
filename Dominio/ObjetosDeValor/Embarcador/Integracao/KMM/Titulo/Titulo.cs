using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Titulo
{
    /// <summary>
    /// Classe que representa uma requisição de fatura.
    /// </summary>

    public class Titulo
    {
        /// <summary>
        /// Operação a ser realizada. Valores possíveis: INSERT ou DELETE.
        /// </summary>
        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        /// <summary>
        /// Tipo do documento fiscal. Valores possíveis: CTE, NFES, CRT.
        /// </summary>
        [JsonProperty(PropertyName = "tipo_documento_fiscal", Required = Required.Default)]
        public string TipoDocumentoFiscal { get; set; }

        /// <summary>
        /// Tipo do documento financeiro.
        /// </summary>
        [JsonProperty(PropertyName = "tipo_documento_financeiro", Required = Required.Default)]
        public string TipoDocumentoFinanceiro { get; set; }

        /// <summary>
        /// Número da fatura.
        /// </summary>
        [JsonProperty(PropertyName = "numero_fatura", Required = Required.Default)]
        public string NumeroFatura { get; set; }

        /// <summary>
        /// ID externo associado à fatura.
        /// </summary>
        [JsonProperty(PropertyName = "id_externo", Required = Required.Default)]
        public int IdExterno { get; set; }

        /// <summary>
        /// Código do organograma da fatura no KMM.
        /// </summary>
        [JsonProperty(PropertyName = "organograma", Required = Required.Default)]
        public string Organograma { get; set; }

        /// <summary>
        /// Conta corrente do favorecido.
        /// </summary>
        [JsonProperty(PropertyName = "cc_favorecido", Required = Required.Default)]
        public string CCFavorecido { get; set; }

        /// <summary>
        /// Lista de documentos fiscais associados à fatura.
        /// </summary>
        [JsonProperty(PropertyName = "docs_fiscais", Required = Required.Default)]
        public List<DocumentoFiscal> DocumentosFiscais { get; set; }

        /// <summary>
        /// Lista de parcelas associadas à fatura.
        /// </summary>
        [JsonProperty(PropertyName = "parcelas", Required = Required.Default)]
        public List<Parcela> Parcelas { get; set; }
    }

    /// <summary>
    /// Classe que representa um documento fiscal.
    /// </summary>
    public class DocumentoFiscal
    {
        /// <summary>
        /// ID do documento no KMM.
        /// </summary>
        [JsonProperty(PropertyName = "id_documento", Required = Required.Default)]
        public string IdDocumento { get; set; }

        /// <summary>
        /// Número do documento.
        /// </summary>
        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        /// <summary>
        /// Série do documento.
        /// </summary>
        [JsonProperty(PropertyName = "serie", Required = Required.Default)]
        public string Serie { get; set; }
    }

    /// <summary>
    /// Classe que representa uma parcela.
    /// </summary>
    public class Parcela
    {
        /// <summary>
        /// Número sequencial da parcela.
        /// </summary>
        [JsonProperty(PropertyName = "parcela", Required = Required.Default)]
        public int NumeroParcela { get; set; }

        /// <summary>
        /// Data de emissão do contas a receber.
        /// </summary>
        [JsonProperty(PropertyName = "data_emissao", Required = Required.Default)]
        public string DataEmissao { get; set; }

        /// <summary>
        /// Data de vencimento do contas a receber.
        /// </summary>
        [JsonProperty(PropertyName = "data_vencimento", Required = Required.Default)]
        public string DataVencimento { get; set; }

        /// <summary>
        /// Valor na moeda especificada.
        /// </summary>
        [JsonProperty(PropertyName = "valor_moeda", Required = Required.Default)]
        public decimal ValorMoeda { get; set; }

        /// <summary>
        /// Valor em reais.
        /// </summary>
        [JsonProperty(PropertyName = "valor_real", Required = Required.Default)]
        public decimal ValorReal { get; set; }

        /// <summary>
        /// Comentário associado à parcela.
        /// </summary>
        [JsonProperty(PropertyName = "comentario", Required = Required.Default)]
        public string Comentario { get; set; }

        /// <summary>
        /// Lista de rateios associados à parcela.
        /// </summary>
        [JsonProperty(PropertyName = "rateio", Required = Required.Default)]
        public List<Rateio> Rateio { get; set; }

        /// <summary>
        /// Informações do boleto, se houver.
        /// </summary>
        [JsonProperty(PropertyName = "boleto", Required = Required.Default)]
        public Boleto Boleto { get; set; }
    }

    /// <summary>
    /// Classe que representa o rateio de uma parcela.
    /// </summary>
    public class Rateio
    {
        /// <summary>
        /// Código do organograma.
        /// </summary>
        [JsonProperty(PropertyName = "organograma", Required = Required.Default)]
        public int Organograma { get; set; }

        /// <summary>
        /// Centro de custo gerencial (CCG).
        /// </summary>
        [JsonProperty(PropertyName = "centro_custo_gerencial", Required = Required.Default)]
        public string CentroCustoGerencial { get; set; }

        /// <summary>
        /// Percentual de rateio.
        /// </summary>
        [JsonProperty(PropertyName = "percentual", Required = Required.Default)]
        public decimal Percentual { get; set; }
    }

    /// <summary>
    /// Classe que representa as informações de um boleto.
    /// </summary>
    public class Boleto
    {
        /// <summary>
        /// Código de barras do boleto.
        /// </summary>
        [JsonProperty(PropertyName = "codigo_barras", Required = Required.Default)]
        public string CodigoBarras { get; set; }

        /// <summary>
        /// Nosso número do boleto.
        /// </summary>
        [JsonProperty(PropertyName = "nosso_numero", Required = Required.Default)]
        public string NossoNumero { get; set; }
    }
}

