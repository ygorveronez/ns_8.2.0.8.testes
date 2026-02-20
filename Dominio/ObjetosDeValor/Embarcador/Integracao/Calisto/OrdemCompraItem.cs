using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto
{
    public class OrdemCompraItem
    {
        [JsonProperty("codigoDepartamento")]
        public int CodigoDepartamento { get; set; }

        [JsonProperty("dataPrevista")]
        public string DataPrevista { get; set; }

        [JsonProperty("quantidade")]
        public int Quantidade { get; set; }

        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        [JsonProperty("dataEntrega")]
        public string DataEntrega { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("codigoFilial")]
        public int CodigoFilial { get; set; }

        [JsonProperty("espec")]
        public string Espec { get; set; }

        [JsonProperty("cdGrupoCtb")]
        public int CdGrupoCtb { get; set; }

        [JsonProperty("preco")]
        public decimal Preco { get; set; }

        [JsonProperty("fcoberturacambial")]
        public int Fcoberturacambial { get; set; }

        [JsonProperty("produto")]
        public Produto Produto { get; set; }
    }
}
