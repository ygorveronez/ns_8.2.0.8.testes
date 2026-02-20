using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class Condicao
    {
        [DataMember(Name = "number")]
        public string number { get; set; }

        [DataMember(Name = "itemNumber")]
        public int itemNumber { get; set; }

        [DataMember(Name = "stepNumber")]
        public int stepNumber { get; set; }

        [DataMember(Name = "counter")]
        public int counter { get; set; }

        [DataMember(Name = "type")]
        public string type { get; set; }

        [DataMember(Name = "typeName")]
        public string typeName { get; set; }

        [DataMember(Name = "calculationType")]
        public string TipoCalculoCondicao { get; set; }

        [DataMember(Name = "baseValue")]
        public decimal baseValue { get; set; }

        [DataMember(Name = "rate")]
        public decimal rate { get; set; }

        [DataMember(Name = "currencyKey")]
        public string Moeda { get; set; }

        [DataMember(Name = "exchangeRate")]
        public int TaxaCambioCondicao { get; set; }

        [DataMember(Name = "category")]
        public string CategoriaCondicao { get; set; }

        [DataMember(Name = "scaleType")]
        public string TipoEscala { get; set; }

        [DataMember(Name = "relevantForAccrual")]
        public string RelevanteAcrescimo { get; set; }

        [DataMember(Name = "origin")]
        public string OrigemCondicao { get; set; }

        [DataMember(Name = "accountNumber")]
        public string NumeroContaFornecedor { get; set; }

        [DataMember(Name = "value")]
        public decimal value { get; set; }

        [DataMember(Name = "class")]
        public string ClarsseCondicao { get; set; }

        [DataMember(Name = "changedManually")]
        public string changedManually { get; set; }
    }
}
