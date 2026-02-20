namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporte
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TProtCTeInfFisco
    {

        private string cMsgField;

        private string xMsgField;

        /// <remarks/>
        public string cMsg
        {
            get
            {
                return this.cMsgField;
            }
            set
            {
                this.cMsgField = value;
            }
        }

        /// <remarks/>
        public string xMsg
        {
            get
            {
                return this.xMsgField;
            }
            set
            {
                this.xMsgField = value;
            }
        }
    }
}
