using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior;

public class IntegrarCargaDadosTransporteItemTransporte
{
    [JsonProperty("codigointerno")]
    public string CodigoInterno { get; set; }

    [JsonProperty("numpedido")]
    public string NumeroPedido { get; set; }

    [JsonProperty("cnpj_depositante")]
    public string CnpjDepositante { get; set; }

    [JsonProperty("cnpj_emitente")]
    public string CnpjEmitente { get; set; }

    [JsonProperty("tipo")]
    public string Tipo { get; set; }

    [JsonProperty("idseq")]
    public int Sequencia { get; set; }

    [JsonProperty("descr_prod")]
    public string DescricaoProduto { get; set; }

    [JsonProperty("barra")]
    public string CodigoBarras { get; set; }

    [JsonProperty("qtde")]
    public string Quantidade { get; set; }

    [JsonProperty("vlrunit")]
    public string ValorUnitario { get; set; }

    [JsonProperty("vlrtotal")]
    public string ValorTotal { get; set; }

    [JsonProperty("totalliquido")]
    public string TotalLiquido { get; set; }

    [JsonProperty("tipoproduto")]
    public string TipoProduto { get; set; }

    [JsonProperty("codindustria")]
    public string CodigoIndustria { get; set; }

    [JsonProperty("descrprod")]
    public string DescricaoCompletaProduto { get; set; }
}
