namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, IncludeInSchema = false, Namespace = "")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Entrega
    {
        private int seqEntrega;
        public int E_SeqEntrega
        {
            get
            {
                return this.seqEntrega;
            }
            set
            {
                this.seqEntrega = value;
            }
        }


        private string clienteField;
        public string E_Cliente
        {
            get
            {
                return this.clienteField;
            }
            set
            {
                this.clienteField = value;
            }
        }

        private string prevInicioField;
        public string E_PrevInicio
        {
            get
            {
                return this.prevInicioField;
            }
            set
            {
                this.prevInicioField = value;
            }
        }

        private string prevFimField;
        public string E_PrevFim
        {
            get
            {
                return this.prevFimField;
            }
            set
            {
                this.prevFimField = value;
            }
        }

        private decimal? pesoField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? E_Peso
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
        public bool E_PesoSpecified
        {
            get
            {
                return E_Peso.HasValue;
            }
        }

        private decimal? volumeField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? E_Volume
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
        public bool E_VolumeSpecified
        {
            get
            {
                return E_Volume.HasValue;
            }
        }

        private string observacaoField;
        public string E_Observacao
        {
            get
            {
                return this.observacaoField;
            }
            set
            {
                this.observacaoField = value;
            }
        }


        private string informacao1Field;
        public string E_Informacao1
        {
            get
            {
                return this.informacao1Field;
            }
            set
            {
                this.informacao1Field = value;
            }
        }

        private string informacao2Field;
        public string E_Informacao2
        {
            get
            {
                return this.informacao2Field;
            }
            set
            {
                this.informacao2Field = value;
            }
        }

        private Docto[] doctosField;

        [System.Xml.Serialization.XmlElement("Docto")]
        public Docto[] Doctos
        {
            get
            {
                return this.doctosField;
            }
            set
            {
                this.doctosField = value;
            }
        }
    }
}
