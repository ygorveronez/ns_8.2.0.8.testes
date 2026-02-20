using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto
{
    public class FaturaDetalhe
    {
        [JsonProperty("codigoBanco")]
        public int CodigoBanco { get; set; }

        [JsonProperty("codigoCobranca")]
        public int CodigoCobranca { get; set; }

        [JsonProperty("codigoTipoOrdemCompra")]
        public int CodigoTipoOrdemCompra { get; set; }

        [JsonProperty("codigoOperacao")]
        public int CodigoOperacao { get; set; }
        
        [JsonProperty("codigoFilial")]
        
        public int CodigoFilial { get; set; }
        
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("codigoCompra")]
        public int CodigoCompra { get; set; }

        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        [JsonProperty("encargos")]
        public int Encargos { get; set; }
        
        [JsonProperty("cep")]
        public string Cep { get; set; }
        
        [JsonProperty("Bairro")]
        public string Bairro { get; set; }
        
        [JsonProperty("Complemento")]
        public string Complemento { get; set; }

        [JsonProperty("contato")]
        public string Contato { get; set; }

        [JsonProperty("codigoAlternativo")]
        public string CodigoAlternativo { get; set; }

        [JsonProperty("dataChegada")]
        public string DataChegada { get; set; }

        [JsonProperty("valorDescontoTotal")]
        public decimal ValorDescontoTotal { get; set; }
        
        [JsonProperty("saldoFinanceiro")]
        public decimal SaldoFinanceiro { get; set; }
        
        [JsonProperty("codigoMoeda")]
        public int CodigoMoeda { get; set; }
        
        [JsonProperty("daf")]
        public string Daf { get; set; }
        
        [JsonProperty("codigoMovCtb")]
        public int CodigoMovCtb { get; set; }
        
        [JsonProperty("valorBruto")]
        public decimal ValorBruto { get; set; }

        [JsonProperty("entidadeCliente")]
        public EntidadeCliente EntidadeCliente { get; set; }

        [JsonProperty("entidadeFornecedor")]
        public EntidadeFornecedor EntidadeFornecedor { get; set; }

        [JsonProperty("ordemCompraItens")]
        public List<OrdemCompraItem> OrdemCompraItens { get; set; }
    }
}
