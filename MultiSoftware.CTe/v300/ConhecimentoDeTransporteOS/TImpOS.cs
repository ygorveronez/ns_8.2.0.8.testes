namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TImpOS
    {

        private object itemField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ICMS00", typeof(TImpOSICMS00))]
        [System.Xml.Serialization.XmlElementAttribute("ICMS20", typeof(TImpOSICMS20))]
        [System.Xml.Serialization.XmlElementAttribute("ICMS45", typeof(TImpOSICMS45))]
        [System.Xml.Serialization.XmlElementAttribute("ICMS90", typeof(TImpOSICMS90))]
        [System.Xml.Serialization.XmlElementAttribute("ICMSOutraUF", typeof(TImpOSICMSOutraUF))]
        [System.Xml.Serialization.XmlElementAttribute("ICMSSN", typeof(TImpOSICMSSN))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }
}
