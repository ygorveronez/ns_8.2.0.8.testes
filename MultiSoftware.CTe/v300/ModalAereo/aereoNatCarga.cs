namespace MultiSoftware.CTe.v300.ModalAereo
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class aereoNatCarga
    {

        private string xDimeField;

        private aereoNatCargaCInfManu[] cInfManuField;

        /// <remarks/>
        public string xDime
        {
            get
            {
                return this.xDimeField;
            }
            set
            {
                this.xDimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cInfManu")]
        public aereoNatCargaCInfManu[] cInfManu
        {
            get
            {
                return this.cInfManuField;
            }
            set
            {
                this.cInfManuField = value;
            }
        }
    }
}
