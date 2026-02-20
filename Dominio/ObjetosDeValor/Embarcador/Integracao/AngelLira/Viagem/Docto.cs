namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, IncludeInSchema = false, Namespace = "")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Docto
    {
        private int numeroField;
        public int D_Numero
        {
            get
            {
                return this.numeroField;
            }
            set
            {
                this.numeroField = value;
            }
        }

        private string serieField;
        public string D_Serie
        {
            get
            {
                return this.serieField;
            }
            set
            {
                this.serieField = value;
            }
        }

        private decimal? pesoField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? D_Peso
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
        public bool D_PesoSpecified
        {
            get
            {
                return D_Peso.HasValue;
            }
        }

        private decimal? volumeField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? D_Volume
        {
            get
            {
                return this.volumeField;
            }
            set
            {
                this.volumeField = value;
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool D_VolumeSpecified
        {
            get
            {
                return D_Volume.HasValue;
            }
        }

        private decimal? valorField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? D_Valor
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
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool D_ValorSpecified
        {
            get
            {
                return D_Valor.HasValue;
            }
        }

        private decimal? freteField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? D_Frete
        {
            get
            {
                return this.freteField;
            }
            set
            {
                this.freteField = value;
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool D_FreteSpecified
        {
            get
            {
                return D_Frete.HasValue;
            }
        }

        private Produto[] produtosField;

        [System.Xml.Serialization.XmlElement("Produto")]
        public Produto[] Produtos
        {
            get
            {
                return this.produtosField;
            }
            set
            {
                this.produtosField = value;
            }
        }
    }
}
