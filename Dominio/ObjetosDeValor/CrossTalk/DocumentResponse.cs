namespace Dominio.ObjetosDeValor.CrossTalk
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class DocumentResponse
    {
        private string innerCodeField;

        private string indexField;

        private string descriptionField;

        private string commentField;

        private string[] errorMessagesField;

        public string InnerCode
        {
            get
            {
                return this.innerCodeField;
            }
            set
            {
                this.innerCodeField = value;
            }
        }

        public string Index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public string Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Error", typeof(string), IsNullable = false)]
        public string[] ErrorMessages
        {
            get
            {
                return this.errorMessagesField;
            }
            set
            {
                this.errorMessagesField = value;
            }
        }
    }
}
