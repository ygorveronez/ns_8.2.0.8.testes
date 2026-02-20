namespace MultiSoftware.NFe.NFeDownloadNF.DownloadNF
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TRetDownloadNFeRetNFeProcNFeGrupoZip
    {

        private byte[] nFeZipField;

        private byte[] protNFeZipField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] NFeZip
        {
            get
            {
                return this.nFeZipField;
            }
            set
            {
                this.nFeZipField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] protNFeZip
        {
            get
            {
                return this.protNFeZipField;
            }
            set
            {
                this.protNFeZipField = value;
            }
        }
    }
}
