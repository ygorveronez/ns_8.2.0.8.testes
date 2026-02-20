namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TImpOSICMSSN
    {

        private TImpOSICMSSNCST cSTField;

        private TImpOSICMSSNIndSN indSNField;

        /// <remarks/>
        public TImpOSICMSSNCST CST
        {
            get
            {
                return this.cSTField;
            }
            set
            {
                this.cSTField = value;
            }
        }

        /// <remarks/>
        public TImpOSICMSSNIndSN indSN
        {
            get
            {
                return this.indSNField;
            }
            set
            {
                this.indSNField = value;
            }
        }
    }
}
