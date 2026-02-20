namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteOS
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TRespTec
    {

        private string cNPJField;

        private string xContatoField;

        private string emailField;

        private string foneField;

        private string idCSRTField;

        private byte[] hashCSRTField;

        /// <remarks/>
        public string CNPJ
        {
            get
            {
                return this.cNPJField;
            }
            set
            {
                this.cNPJField = value;
            }
        }

        /// <remarks/>
        public string xContato
        {
            get
            {
                return this.xContatoField;
            }
            set
            {
                this.xContatoField = value;
            }
        }

        /// <remarks/>
        public string email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public string fone
        {
            get
            {
                return this.foneField;
            }
            set
            {
                this.foneField = value;
            }
        }

        /// <remarks/>
        public string idCSRT
        {
            get
            {
                return this.idCSRTField;
            }
            set
            {
                this.idCSRTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] hashCSRT
        {
            get
            {
                return this.hashCSRTField;
            }
            set
            {
                this.hashCSRTField = value;
            }
        }
    }
}
