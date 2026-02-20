using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.ICMS
{
    [DataContract]
    public class ICMS
    {
        public ICMS()
        {
            this.ObservacaoCTe = "";
            this.SimplesNacional = false;
            this.Aliquota = 0;
            this.ValorICMS = 0;
            this.ValorBaseCalculoICMS = 0;
            this.PercentualReducaoBC = 0;
            this.PercentualInclusaoBC = 0;
            this.IncluirICMSBC = false;
            this.CST = "";
            this.ValorCreditoPresumido = 0;
            this.ExibirNaDacte = true;
            this.ValorTotalTributos = 0;
        }

        [DataMember]
        public decimal Aliquota { get; set; }
        [DataMember]
        public decimal ValorICMS { get; set; }
        [DataMember]
        public decimal ValorCreditoPresumido { get; set; }
        [DataMember]
        public decimal ValorBaseCalculoICMS { get; set; }
        [DataMember]
        public decimal PercentualReducaoBC { get; set; }
        [DataMember]
        public bool IncluirICMSBC { get; set; }
        [DataMember]
        public bool? IncluirICMSBCFreteProprio { get; set; }
        [DataMember]
        public decimal PercentualInclusaoBC { get; set; }
        [DataMember]
        public string CST { get; set; }
        [DataMember]
        public string ObservacaoCTe { get; set; }
        [DataMember]
        public bool SimplesNacional { get; set; }
        [DataMember]
        public bool ExibirNaDacte { get; set; }
        [DataMember]
        public decimal ValorTotalTributos { get; set; }
    }
}
