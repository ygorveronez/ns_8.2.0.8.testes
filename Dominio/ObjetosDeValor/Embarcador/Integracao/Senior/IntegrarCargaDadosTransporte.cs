using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior;

public class IntegrarCargaDadosTransporte
{
    public IntegrarCargaDadosTransporte()
    {
        Items = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporteItemTransporte>();
    }
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

    [JsonProperty("descroper")]
    public string DescricaoOperacao { get; set; }

    [JsonProperty("cfop")]
    public string Cfop { get; set; }

    [JsonProperty("data_emissao")]
    public DateTime DataEmissao { get; set; }

    [JsonProperty("pessoa_dest")]
    public string TipoPessoaDestinatario { get; set; }

    [JsonProperty("codigo_dest")]
    public string CodigoDestinatario { get; set; }

    [JsonProperty("nome_dest")]
    public string NomeDestinatario { get; set; }

    [JsonProperty("cnpj_dest")]
    public string DocumentoDestinatario { get; set; }

    [JsonProperty("endereco_dest")]
    public string EnderecoDestinatario { get; set; }

    [JsonProperty("bairro_dest")]
    public string BairroDestinatario { get; set; }

    [JsonProperty("cep_dest")]
    public string CepDestinatario { get; set; }

    [JsonProperty("cidade_dest")]
    public string CidadeDestinatario { get; set; }

    [JsonProperty("estado_dest")]
    public string EstadoDestinatario { get; set; }

    [JsonProperty("vlrprodutos")]
    public string ValorProdutos { get; set; }

    [JsonProperty("vlrtotal")]
    public string ValorTotal { get; set; }

    [JsonProperty("nome_transp")]
    public string NomeTransportadora { get; set; }

    [JsonProperty("cnpj_transp")]
    public string CnpjTransportadora { get; set; }

    [JsonProperty("ciffob")]
    public string ModalidadeFrete { get; set; }

    [JsonProperty("pesoliquido")]
    public string PesoLiquido { get; set; }

    [JsonProperty("gerafinceiro")]
    public bool GeraFinanceiro { get; set; }

    [JsonProperty("tipodocumento")]
    public string TipoDocumento { get; set; }

    [JsonProperty("tipocarga")]
    public string TipoCarga { get; set; }

    [JsonProperty("num_itens")]
    public int NumeroItens { get; set; }

    [JsonProperty("tiponf")]
    public string TipoNotaFiscal { get; set; }

    [JsonProperty("estado")]
    public string EstadoDocumento { get; set; }

    [JsonProperty("cnpj_unidade")]
    public string CnpjUnidade { get; set; }

    [JsonProperty("codigotipopedido")]
    public string CodigoTipoPedido { get; set; }

    [JsonProperty("idintegracaoerp")]
    public string IdIntegracaoErp { get; set; }

    [JsonProperty("canal_venda")]
    public string CanalVenda { get; set; }

    [JsonProperty("protocolocarga")]
    public string ProtocoloCarga { get; set; }

    [JsonProperty("items")]
    public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporteItemTransporte> Items { get; set; }
}

