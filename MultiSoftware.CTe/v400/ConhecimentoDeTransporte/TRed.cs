namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TRed
    {

        private string pRedAliqField;

        private string pAliqEfetField;

        /// <remarks/>
        public string pRedAliq
        {
            get
            {
                return this.pRedAliqField;
            }
            set
            {
                this.pRedAliqField = value;
            }
        }

        /// <remarks/>
        public string pAliqEfet
        {
            get
            {
                return this.pAliqEfetField;
            }
            set
            {
                this.pAliqEfetField = value;
            }
        }
    }
}
