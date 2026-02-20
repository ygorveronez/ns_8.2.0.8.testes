namespace MultiSoftware.CTe.v300.ModalRodoviario
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class rodo
    {

        private string rNTRCField;

        private rodoOcc[] occField;

        /// <remarks/>
        public string RNTRC
        {
            get
            {
                return this.rNTRCField;
            }
            set
            {
                this.rNTRCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("occ")]
        public rodoOcc[] occ
        {
            get
            {
                return this.occField;
            }
            set
            {
                this.occField = value;
            }
        }
    }
}
