namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem
{
    public class Produto
    {
        private string codigoField;
        public string P_Codigo
        {
            get
            {
                return this.codigoField;
            }
            set
            {
                this.codigoField = value;
            }
        }

        private decimal valorField;
        public decimal P_Valor
        {
            get
            {
                return this.valorField;
            }
            set
            {
                this.valorField = value;
            }
        }

        private decimal? pesoField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? P_Peso
        {
            get
            {
                return this.pesoField;
            }
            set
            {
                this.pesoField = value;
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool P_PesoSpecified
        {
            get
            {
                return P_Peso.HasValue;
            }
        }

        private decimal? valumeField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? P_Volume
        {
            get
            {
                return this.valumeField;
            }
            set
            {
                this.valumeField = value;
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool P_VolumeSpecified
        {
            get
            {
                return P_Volume.HasValue;
            }
        }
    }
}
