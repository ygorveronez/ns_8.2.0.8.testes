namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, IncludeInSchema = false, Namespace = "")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Agendamento
    {
        private string versaoField;
        public string Versao
        {
            get
            {
                return this.versaoField;
            }
            set
            {
                this.versaoField = value;
            }
        }


        private Viagem viagemField;
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Viagem", typeof(Viagem), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Viagem Viagem
        {
            get
            {
                return this.viagemField;
            }
            set
            {
                this.viagemField = value;
            }
        }
    }
}
