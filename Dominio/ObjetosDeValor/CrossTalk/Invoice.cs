namespace Dominio.ObjetosDeValor.CrossTalk
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class Invoice
    {
        private string seriesField;

        private string numberField;

        private string issueDateField;

        private string takerCnpjField;

        public string Series
        {
            get
            {
                return this.seriesField;
            }
            set
            {
                this.seriesField = value;
            }
        }

        public string Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        public string IssueDate
        {
            get
            {
                return this.issueDateField;
            }
            set
            {
                this.issueDateField = value;
            }
        }

        public string TakerCnpj
        {
            get
            {
                return this.takerCnpjField;
            }
            set
            {
                this.takerCnpjField = value;
            }
        }
    }
}
