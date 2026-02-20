
namespace Dominio.ObjetosDeValor.CrossTalk
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, IncludeInSchema = false, Namespace = "")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class CrossTalk_Message
    {
        private CrossTalk_Header crossTalk_HeaderField;
        private CrossTalk_Body crossTalk_BodyField;

        [System.Xml.Serialization.XmlElementAttribute("CrossTalk_Header", typeof(CrossTalk_Header), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CrossTalk_Header CrossTalk_Header
        {
            get
            {
                return this.crossTalk_HeaderField;
            }
            set
            {
                this.crossTalk_HeaderField = value;
            }
        }



        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CrossTalk_Body", typeof(CrossTalk_Body), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]

        public CrossTalk_Body CrossTalk_Body
        {
            get
            {
                return this.crossTalk_BodyField;
            }
            set
            {
                this.crossTalk_BodyField = value;
            }
        }
    }
}
