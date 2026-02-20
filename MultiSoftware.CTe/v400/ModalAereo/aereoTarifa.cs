namespace MultiSoftware.CTe.v400.ModalAereo
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class aereoTarifa
    {

        private string clField;

        private string cTarField;

        private string vTarField;

        /// <remarks/>
        public string CL
        {
            get
            {
                return this.clField;
            }
            set
            {
                this.clField = value;
            }
        }

        /// <remarks/>
        public string cTar
        {
            get
            {
                return this.cTarField;
            }
            set
            {
                this.cTarField = value;
            }
        }

        /// <remarks/>
        public string vTar
        {
            get
            {
                return this.vTarField;
            }
            set
            {
                this.vTarField = value;
            }
        }
    }
}
