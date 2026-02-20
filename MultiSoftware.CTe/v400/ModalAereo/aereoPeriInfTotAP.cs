namespace MultiSoftware.CTe.v400.ModalAereo
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class aereoPeriInfTotAP
    {

        private string qTotProdField;

        private aereoPeriInfTotAPUniAP uniAPField;

        /// <remarks/>
        public string qTotProd
        {
            get
            {
                return this.qTotProdField;
            }
            set
            {
                this.qTotProdField = value;
            }
        }

        /// <remarks/>
        public aereoPeriInfTotAPUniAP uniAP
        {
            get
            {
                return this.uniAPField;
            }
            set
            {
                this.uniAPField = value;
            }
        }
    }
}
