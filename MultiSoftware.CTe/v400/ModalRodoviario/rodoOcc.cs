namespace MultiSoftware.CTe.v400.ModalRodoviario
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class rodoOcc
    {

        private string serieField;

        private string nOccField;

        private string dEmiField;

        private rodoOccEmiOcc emiOccField;

        /// <remarks/>
        public string serie
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

        /// <remarks/>
        public string nOcc
        {
            get
            {
                return this.nOccField;
            }
            set
            {
                this.nOccField = value;
            }
        }

        /// <remarks/>
        public string dEmi
        {
            get
            {
                return this.dEmiField;
            }
            set
            {
                this.dEmiField = value;
            }
        }

        /// <remarks/>
        public rodoOccEmiOcc emiOcc
        {
            get
            {
                return this.emiOccField;
            }
            set
            {
                this.emiOccField = value;
            }
        }
    }
}
