using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.IBSCBS
{
    [DataContract]
    public class IBSCBS
    {
        [DataMember]
        public string CST { get; set; }

        [DataMember]
        public string NBS { get; set; }


        [DataMember]
        public string CodigoIndicadorOperacao { get; set; }


        [DataMember]
        public string ClassificacaoTributaria { get; set; }

        [DataMember]
        public decimal BaseCalculo { get; set; }

        [DataMember]
        public decimal AliquotaIBSEstadual { get; set; }

        [DataMember]
        public decimal PercentualReducaoIBSEstadual { get; set; }

        [DataMember]
        public decimal ValorIBSEstadual { get; set; }

        [DataMember]
        public decimal AliquotaIBSMunicipal { get; set; }

        [DataMember]
        public decimal PercentualReducaoIBSMunicipal { get; set; }

        [DataMember]
        public decimal ValorIBSMunicipal { get; set; }

        [DataMember]
        public decimal AliquotaCBS { get; set; }

        [DataMember]
        public decimal PercentualReducaoCBS { get; set; }

        [DataMember]
        public decimal ValorCBS { get; set; }
    }
}
