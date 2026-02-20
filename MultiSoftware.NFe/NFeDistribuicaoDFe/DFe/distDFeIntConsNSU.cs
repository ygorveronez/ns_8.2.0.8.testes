namespace MultiSoftware.NFe.NFeDistribuicaoDFe.DFe
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class distDFeIntConsNSU
    {

        private string nSUField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
        public string NSU
        {
            get
            {
                return this.nSUField;
            }
            set
            {
                this.nSUField = value;
            }
        }
    }
}
