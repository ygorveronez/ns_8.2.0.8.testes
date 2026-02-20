using System.ServiceModel;

namespace Dominio.Ferroviario
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    public partial class MRetornoEnvioErros
    {

        private MRetornoEnvioErrosErro[] erroField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Erro")]
        public MRetornoEnvioErrosErro[] Erro
        {
            get
            {
                return this.erroField;
            }
            set
            {
                this.erroField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    public enum TTipoErro
    {

        /// <remarks/>
        I,

        /// <remarks/>
        N,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    public partial class MRetornoEnvioErrosErro
    {

        private string codigoErroField;

        private TTipoErro tipoErroField;

        private string mensagemErroField;

        /// <remarks/>
        public string codigoErro
        {
            get
            {
                return this.codigoErroField;
            }
            set
            {
                this.codigoErroField = value;
            }
        }

        /// <remarks/>
        public TTipoErro tipoErro
        {
            get
            {
                return this.tipoErroField;
            }
            set
            {
                this.tipoErroField = value;
            }
        }

        /// <remarks/>
        public string mensagemErro
        {
            get
            {
                return this.mensagemErroField;
            }
            set
            {
                this.mensagemErroField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    public enum TStatus
    {

        /// <remarks/>
        OK,

        /// <remarks/>
        NOK,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    public partial class RetornoEnvio
    {

        private string cNPJFerroviaField;

        private TStatus statusEnvioField;

        private string protocoloEnvioField;

        private MRetornoEnvioErros itemField;

        private string versaoField;

        /// <remarks/>
        public string CNPJFerrovia
        {
            get
            {
                return this.cNPJFerroviaField;
            }
            set
            {
                this.cNPJFerroviaField = value;
            }
        }

        /// <remarks/>
        public TStatus statusEnvio
        {
            get
            {
                return this.statusEnvioField;
            }
            set
            {
                this.statusEnvioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
        public string protocoloEnvio
        {
            get
            {
                return this.protocoloEnvioField;
            }
            set
            {
                this.protocoloEnvioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("erros", typeof(MRetornoEnvioErros))]
        [System.Xml.Serialization.XmlElementAttribute("protocoloRecebimento", typeof(string), DataType = "positiveInteger")]
        public MRetornoEnvioErros Item
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string versao
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
    }

    [System.ServiceModel.MessageContract(IsWrapped = false, WrapperNamespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    public partial class TRetornoEnvio
    {
        [MessageBodyMember(Name = "RetornoEnvio", Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
        public RetornoEnvio data { get; set; }

    }

    [System.ServiceModel.MessageContract(IsWrapped = false, WrapperNamespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    public partial class MRetornoEnvio
    {
        [MessageBodyMember(Name = "RetornoEnvio", Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
        public RetornoEnvio data { get; set; }

    }
}