using System.Collections.Generic;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Dominio.Ferroviario.EventoFerroviario
{

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/eventosFerro")]
    //public partial class MEventoFerroviario
    //{

    //    private string protocoloEnvioField;

    //    private System.DateTime dataHoraEnvioField;

    //    private string cNPJEmissorField;

    //    private string cNPJDestinatarioField;

    //    private string nomeProcessoEnvioField;

    //    private string codigoUsuarioSistemaField;

    //    private System.DateTime dataHoraField;

    //    private string observacaoField;

    //    private TEvento[] eventosField;

    //    private string versaoField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    //    public string protocoloEnvio
    //    {
    //        get
    //        {
    //            return this.protocoloEnvioField;
    //        }
    //        set
    //        {
    //            this.protocoloEnvioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public System.DateTime dataHoraEnvio
    //    {
    //        get
    //        {
    //            return this.dataHoraEnvioField;
    //        }
    //        set
    //        {
    //            this.dataHoraEnvioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string CNPJEmissor
    //    {
    //        get
    //        {
    //            return this.cNPJEmissorField;
    //        }
    //        set
    //        {
    //            this.cNPJEmissorField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string CNPJDestinatario
    //    {
    //        get
    //        {
    //            return this.cNPJDestinatarioField;
    //        }
    //        set
    //        {
    //            this.cNPJDestinatarioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string nomeProcessoEnvio
    //    {
    //        get
    //        {
    //            return this.nomeProcessoEnvioField;
    //        }
    //        set
    //        {
    //            this.nomeProcessoEnvioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    //    public string codigoUsuarioSistema
    //    {
    //        get
    //        {
    //            return this.codigoUsuarioSistemaField;
    //        }
    //        set
    //        {
    //            this.codigoUsuarioSistemaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public System.DateTime dataHora
    //    {
    //        get
    //        {
    //            return this.dataHoraField;
    //        }
    //        set
    //        {
    //            this.dataHoraField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string observacao
    //    {
    //        get
    //        {
    //            return this.observacaoField;
    //        }
    //        set
    //        {
    //            this.observacaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("Evento", IsNullable = false)]
    //    public TEvento[] eventos
    //    {
    //        get
    //        {
    //            return this.eventosField;
    //        }
    //        set
    //        {
    //            this.eventosField = value;
    //        }
    //    }

    //}

    //[System.ServiceModel.MessageContract(IsWrapped = false, WrapperNamespace = "http://xmlns.mrs.com.br/iti/tipos/eventosFerro")]
    //public partial class TEventoFerroviario
    //{
    //    [MessageBodyMember(Name = "EventoFerroviario", Namespace = "http://xmlns.mrs.com.br/iti/tipos/eventosFerro")]
    //    public MEventoFerroviario data { get; set; }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEvento
    //{

    //    private TTipoEvento tipoEventoField;

    //    private string codigoEventoField;

    //    private string siglaPatioRefField;

    //    private string siglaTerminalRefField;

    //    private System.DateTime dataHoraInicioField;

    //    private bool dataHoraInicioFieldSpecified;

    //    private System.DateTime dataHoraFimField;

    //    private bool dataHoraFimFieldSpecified;

    //    private System.DateTime dataHoraInicioPrevistaField;

    //    private System.DateTime dataHoraFimPrevistaField;

    //    private TEventoVeiculosMensagem veiculosMensagemField;

    //    private object itemField;

    //    private TNotaFiscalCliente[] notasFiscaisField;

    //    private TDespacho[] despachosField;

    //    private string observacaoField;

    //    /// <remarks/>
    //    public TTipoEvento tipoEvento
    //    {
    //        get
    //        {
    //            return this.tipoEventoField;
    //        }
    //        set
    //        {
    //            this.tipoEventoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string codigoEvento
    //    {
    //        get
    //        {
    //            return this.codigoEventoField;
    //        }
    //        set
    //        {
    //            this.codigoEventoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string siglaPatioRef
    //    {
    //        get
    //        {
    //            return this.siglaPatioRefField;
    //        }
    //        set
    //        {
    //            this.siglaPatioRefField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string siglaTerminalRef
    //    {
    //        get
    //        {
    //            return this.siglaTerminalRefField;
    //        }
    //        set
    //        {
    //            this.siglaTerminalRefField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public System.DateTime dataHoraInicio
    //    {
    //        get
    //        {
    //            return this.dataHoraInicioField;
    //        }
    //        set
    //        {
    //            this.dataHoraInicioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public bool dataHoraInicioSpecified
    //    {
    //        get
    //        {
    //            return this.dataHoraInicioFieldSpecified;
    //        }
    //        set
    //        {
    //            this.dataHoraInicioFieldSpecified = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public System.DateTime dataHoraFim
    //    {
    //        get
    //        {
    //            return this.dataHoraFimField;
    //        }
    //        set
    //        {
    //            this.dataHoraFimField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public bool dataHoraFimSpecified
    //    {
    //        get
    //        {
    //            return this.dataHoraFimFieldSpecified;
    //        }
    //        set
    //        {
    //            this.dataHoraFimFieldSpecified = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public System.DateTime dataHoraInicioPrevista
    //    {
    //        get
    //        {
    //            return this.dataHoraInicioPrevistaField;
    //        }
    //        set
    //        {
    //            this.dataHoraInicioPrevistaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public System.DateTime dataHoraFimPrevista
    //    {
    //        get
    //        {
    //            return this.dataHoraFimPrevistaField;
    //        }
    //        set
    //        {
    //            this.dataHoraFimPrevistaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TEventoVeiculosMensagem veiculosMensagem
    //    {
    //        get
    //        {
    //            return this.veiculosMensagemField;
    //        }
    //        set
    //        {
    //            this.veiculosMensagemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("EventoTerminal", typeof(TEventoEventoTerminal))]
    //    [System.Xml.Serialization.XmlElementAttribute("EventoTrem", typeof(TEventoEventoTrem))]
    //    [System.Xml.Serialization.XmlElementAttribute("EventoVeiculo", typeof(TEventoEventoVeiculo))]
    //    public object Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("NotaFiscalCliente", IsNullable = false)]
    //    public TNotaFiscalCliente[] notasFiscais
    //    {
    //        get
    //        {
    //            return this.notasFiscaisField;
    //        }
    //        set
    //        {
    //            this.notasFiscaisField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("Despacho", IsNullable = false)]
    //    public TDespacho[] despachos
    //    {
    //        get
    //        {
    //            return this.despachosField;
    //        }
    //        set
    //        {
    //            this.despachosField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string observacao
    //    {
    //        get
    //        {
    //            return this.observacaoField;
    //        }
    //        set
    //        {
    //            this.observacaoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public enum TTipoEvento
    //{

    //    /// <remarks/>
    //    ATIVIDADE_INTERNA,

    //    /// <remarks/>
    //    EVENTO_VEICULO,

    //    /// <remarks/>
    //    TERMINAL,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoVeiculosMensagem
    //{

    //    private TVagao[] vagoesField;

    //    private TLocomotiva[] locomotivasField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("Vagao", IsNullable = false)]
    //    public TVagao[] vagoes
    //    {
    //        get
    //        {
    //            return this.vagoesField;
    //        }
    //        set
    //        {
    //            this.vagoesField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("Locomotiva", IsNullable = false)]
    //    public TLocomotiva[] locomotivas
    //    {
    //        get
    //        {
    //            return this.locomotivasField;
    //        }
    //        set
    //        {
    //            this.locomotivasField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
    //public partial class TVagao
    //{

    //    private string tipoVagaoField;

    //    private string identificadorField;

    //    private TLotacao lotacaoField;

    //    private string taraField;

    //    private string capacidadeNominalField;

    //    private string cNPJProprietarioField;

    //    private string idVagaoField;

    //    /// <remarks/>
    //    public string tipoVagao
    //    {
    //        get
    //        {
    //            return this.tipoVagaoField;
    //        }
    //        set
    //        {
    //            this.tipoVagaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string identificador
    //    {
    //        get
    //        {
    //            return this.identificadorField;
    //        }
    //        set
    //        {
    //            this.identificadorField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TLotacao lotacao
    //    {
    //        get
    //        {
    //            return this.lotacaoField;
    //        }
    //        set
    //        {
    //            this.lotacaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string tara
    //    {
    //        get
    //        {
    //            return this.taraField;
    //        }
    //        set
    //        {
    //            this.taraField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string capacidadeNominal
    //    {
    //        get
    //        {
    //            return this.capacidadeNominalField;
    //        }
    //        set
    //        {
    //            this.capacidadeNominalField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string CNPJProprietario
    //    {
    //        get
    //        {
    //            return this.cNPJProprietarioField;
    //        }
    //        set
    //        {
    //            this.cNPJProprietarioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string IdVagao
    //    {
    //        get
    //        {
    //            return this.idVagaoField;
    //        }
    //        set
    //        {
    //            this.idVagaoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
    //public enum TLotacao
    //{

    //    /// <remarks/>
    //    VAZIO,

    //    /// <remarks/>
    //    CARREGADO,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    //public partial class MRetornoEnvio
    //{

    //    private string cNPJFerroviaField;

    //    private TStatus statusEnvioField;

    //    private string protocoloEnvioField;

    //    private object itemField;

    //    private string versaoField;

    //    /// <remarks/>
    //    public string CNPJFerrovia
    //    {
    //        get
    //        {
    //            return this.cNPJFerroviaField;
    //        }
    //        set
    //        {
    //            this.cNPJFerroviaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TStatus statusEnvio
    //    {
    //        get
    //        {
    //            return this.statusEnvioField;
    //        }
    //        set
    //        {
    //            this.statusEnvioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    //    public string protocoloEnvio
    //    {
    //        get
    //        {
    //            return this.protocoloEnvioField;
    //        }
    //        set
    //        {
    //            this.protocoloEnvioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("erros", typeof(MRetornoEnvioErros))]
    //    [System.Xml.Serialization.XmlElementAttribute("protocoloRecebimento", typeof(string), DataType = "positiveInteger")]
    //    public object Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string versao
    //    {
    //        get
    //        {
    //            return this.versaoField;
    //        }
    //        set
    //        {
    //            this.versaoField = value;
    //        }
    //    }
    //}

    //[System.ServiceModel.MessageContract(IsWrapped = false, WrapperNamespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    //public partial class TRetornoEnvio
    //{
    //    [MessageBodyMember(Name = "RetornoEnvio", Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    //    public MRetornoEnvio data { get; set; }

    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    //public enum TStatus
    //{

    //    /// <remarks/>
    //    OK,

    //    /// <remarks/>
    //    NOK,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    //public partial class MRetornoEnvioErros
    //{

    //    private MRetornoEnvioErrosErro[] erroField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("Erro")]
    //    public MRetornoEnvioErrosErro[] Erro
    //    {
    //        get
    //        {
    //            return this.erroField;
    //        }
    //        set
    //        {
    //            this.erroField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    //public partial class MRetornoEnvioErrosErro
    //{

    //    private string codigoErroField;

    //    private TTipoErro tipoErroField;

    //    private string mensagemErroField;

    //    /// <remarks/>
    //    public string codigoErro
    //    {
    //        get
    //        {
    //            return this.codigoErroField;
    //        }
    //        set
    //        {
    //            this.codigoErroField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TTipoErro tipoErro
    //    {
    //        get
    //        {
    //            return this.tipoErroField;
    //        }
    //        set
    //        {
    //            this.tipoErroField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string mensagemErro
    //    {
    //        get
    //        {
    //            return this.mensagemErroField;
    //        }
    //        set
    //        {
    //            this.mensagemErroField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/retorno")]
    //public enum TTipoErro
    //{

    //    /// <remarks/>
    //    I,

    //    /// <remarks/>
    //    N,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TCarga
    //{

    //    private string itemField;

    //    private ItemChoiceType5 itemElementNameField;

    //    private TTamanhoCNT tamanhoContainerField;

    //    private bool tamanhoContainerFieldSpecified;

    //    private ArrayOfTCargaRefNotaFiscalClienteRefNotaFiscalCliente[] notasFiscaisField;

    //    private string[] lacresField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("identificadorCarga", typeof(string), DataType = "token")]
    //    [System.Xml.Serialization.XmlElementAttribute("numeroContainer", typeof(string))]
    //    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
    //    public string Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public ItemChoiceType5 ItemElementName
    //    {
    //        get
    //        {
    //            return this.itemElementNameField;
    //        }
    //        set
    //        {
    //            this.itemElementNameField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TTamanhoCNT tamanhoContainer
    //    {
    //        get
    //        {
    //            return this.tamanhoContainerField;
    //        }
    //        set
    //        {
    //            this.tamanhoContainerField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public bool tamanhoContainerSpecified
    //    {
    //        get
    //        {
    //            return this.tamanhoContainerFieldSpecified;
    //        }
    //        set
    //        {
    //            this.tamanhoContainerFieldSpecified = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("refNotaFiscalCliente", IsNullable = false)]
    //    public ArrayOfTCargaRefNotaFiscalClienteRefNotaFiscalCliente[] notasFiscais
    //    {
    //        get
    //        {
    //            return this.notasFiscaisField;
    //        }
    //        set
    //        {
    //            this.notasFiscaisField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("nLacre", DataType = "token", IsNullable = false)]
    //    public string[] lacres
    //    {
    //        get
    //        {
    //            return this.lacresField;
    //        }
    //        set
    //        {
    //            this.lacresField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho", IncludeInSchema = false)]
    //public enum ItemChoiceType5
    //{

    //    /// <remarks/>
    //    identificadorCarga,

    //    /// <remarks/>
    //    numeroContainer,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public enum TTamanhoCNT
    //{

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlEnumAttribute("20")]
    //    Item20,

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlEnumAttribute("40")]
    //    Item40,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class ArrayOfTCargaRefNotaFiscalClienteRefNotaFiscalCliente
    //{

    //    private string idNotaFiscalField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idNotaFiscal
    //    {
    //        get
    //        {
    //            return this.idNotaFiscalField;
    //        }
    //        set
    //        {
    //            this.idNotaFiscalField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TMercadoria
    //{

    //    private string descricaoResumidaField;

    //    private string descricaoDetalhadaField;

    //    private string codigoField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string descricaoResumida
    //    {
    //        get
    //        {
    //            return this.descricaoResumidaField;
    //        }
    //        set
    //        {
    //            this.descricaoResumidaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string descricaoDetalhada
    //    {
    //        get
    //        {
    //            return this.descricaoDetalhadaField;
    //        }
    //        set
    //        {
    //            this.descricaoDetalhadaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string codigo
    //    {
    //        get
    //        {
    //            return this.codigoField;
    //        }
    //        set
    //        {
    //            this.codigoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TFluxoFerroviario
    //{

    //    private string numeroFluxoField;

    //    private string cNPJFerroviaField;

    //    private TFluxoFerroviarioCorrentista correntistaField;

    //    private TFluxoFerroviarioRemetente remetenteField;

    //    private TFluxoFerroviarioDestinatario destinatarioField;

    //    private TMercadoria mercadoriaField;

    //    private TFluxoFerroviarioRota rotaField;

    //    private string unidadeCobrancaField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string numeroFluxo
    //    {
    //        get
    //        {
    //            return this.numeroFluxoField;
    //        }
    //        set
    //        {
    //            this.numeroFluxoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string CNPJFerrovia
    //    {
    //        get
    //        {
    //            return this.cNPJFerroviaField;
    //        }
    //        set
    //        {
    //            this.cNPJFerroviaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TFluxoFerroviarioCorrentista Correntista
    //    {
    //        get
    //        {
    //            return this.correntistaField;
    //        }
    //        set
    //        {
    //            this.correntistaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TFluxoFerroviarioRemetente Remetente
    //    {
    //        get
    //        {
    //            return this.remetenteField;
    //        }
    //        set
    //        {
    //            this.remetenteField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TFluxoFerroviarioDestinatario Destinatario
    //    {
    //        get
    //        {
    //            return this.destinatarioField;
    //        }
    //        set
    //        {
    //            this.destinatarioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TMercadoria Mercadoria
    //    {
    //        get
    //        {
    //            return this.mercadoriaField;
    //        }
    //        set
    //        {
    //            this.mercadoriaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TFluxoFerroviarioRota rota
    //    {
    //        get
    //        {
    //            return this.rotaField;
    //        }
    //        set
    //        {
    //            this.rotaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string unidadeCobranca
    //    {
    //        get
    //        {
    //            return this.unidadeCobrancaField;
    //        }
    //        set
    //        {
    //            this.unidadeCobrancaField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TFluxoFerroviarioCorrentista
    //{

    //    private string itemField;

    //    private ItemChoiceType2 itemElementNameField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("CNPJ", typeof(string))]
    //    [System.Xml.Serialization.XmlElementAttribute("CPF", typeof(string))]
    //    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
    //    public string Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public ItemChoiceType2 ItemElementName
    //    {
    //        get
    //        {
    //            return this.itemElementNameField;
    //        }
    //        set
    //        {
    //            this.itemElementNameField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho", IncludeInSchema = false)]
    //public enum ItemChoiceType2
    //{

    //    /// <remarks/>
    //    CNPJ,

    //    /// <remarks/>
    //    CPF,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TFluxoFerroviarioRemetente
    //{

    //    private string itemField;

    //    private ItemChoiceType3 itemElementNameField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("CNPJ", typeof(string))]
    //    [System.Xml.Serialization.XmlElementAttribute("CPF", typeof(string))]
    //    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
    //    public string Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public ItemChoiceType3 ItemElementName
    //    {
    //        get
    //        {
    //            return this.itemElementNameField;
    //        }
    //        set
    //        {
    //            this.itemElementNameField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho", IncludeInSchema = false)]
    //public enum ItemChoiceType3
    //{

    //    /// <remarks/>
    //    CNPJ,

    //    /// <remarks/>
    //    CPF,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TFluxoFerroviarioDestinatario
    //{

    //    private string itemField;

    //    private ItemChoiceType4 itemElementNameField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("CNPJ", typeof(string))]
    //    [System.Xml.Serialization.XmlElementAttribute("CPF", typeof(string))]
    //    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
    //    public string Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public ItemChoiceType4 ItemElementName
    //    {
    //        get
    //        {
    //            return this.itemElementNameField;
    //        }
    //        set
    //        {
    //            this.itemElementNameField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho", IncludeInSchema = false)]
    //public enum ItemChoiceType4
    //{

    //    /// <remarks/>
    //    CNPJ,

    //    /// <remarks/>
    //    CPF,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TFluxoFerroviarioRota
    //{

    //    private string siglaPatioOrigemField;

    //    private string siglaTerminalOrigemField;

    //    private string siglaPatioDestinoField;

    //    private string siglaTerminalDestinoField;

    //    private string siglaPatioIntercambioOrigemField;

    //    private string siglaPatioIntercambioDestinoField;

    //    /// <remarks/>
    //    public string siglaPatioOrigem
    //    {
    //        get
    //        {
    //            return this.siglaPatioOrigemField;
    //        }
    //        set
    //        {
    //            this.siglaPatioOrigemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string siglaTerminalOrigem
    //    {
    //        get
    //        {
    //            return this.siglaTerminalOrigemField;
    //        }
    //        set
    //        {
    //            this.siglaTerminalOrigemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string siglaPatioDestino
    //    {
    //        get
    //        {
    //            return this.siglaPatioDestinoField;
    //        }
    //        set
    //        {
    //            this.siglaPatioDestinoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string siglaTerminalDestino
    //    {
    //        get
    //        {
    //            return this.siglaTerminalDestinoField;
    //        }
    //        set
    //        {
    //            this.siglaTerminalDestinoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string siglaPatioIntercambioOrigem
    //    {
    //        get
    //        {
    //            return this.siglaPatioIntercambioOrigemField;
    //        }
    //        set
    //        {
    //            this.siglaPatioIntercambioOrigemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string siglaPatioIntercambioDestino
    //    {
    //        get
    //        {
    //            return this.siglaPatioIntercambioDestinoField;
    //        }
    //        set
    //        {
    //            this.siglaPatioIntercambioDestinoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TIdentificadorDespacho
    //{

    //    private string serieField;

    //    private string serieFiscalField;

    //    private string numeroField;

    //    private string numeroFiscalField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string serie
    //    {
    //        get
    //        {
    //            return this.serieField;
    //        }
    //        set
    //        {
    //            this.serieField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string serieFiscal
    //    {
    //        get
    //        {
    //            return this.serieFiscalField;
    //        }
    //        set
    //        {
    //            this.serieFiscalField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string numero
    //    {
    //        get
    //        {
    //            return this.numeroField;
    //        }
    //        set
    //        {
    //            this.numeroField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string numeroFiscal
    //    {
    //        get
    //        {
    //            return this.numeroFiscalField;
    //        }
    //        set
    //        {
    //            this.numeroFiscalField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TDespacho
    //{

    //    private TDespachoIdentificadorDespacho identificadorDespachoField;

    //    private TDespachoFluxoTransporte fluxoTransporteField;

    //    private ArrayOfTDespachoVagaoDespachadoVagaoDespachado[] vagoesDespachadosField;

    //    private ArrayOfTDespachoRefNotaFiscalClienteRefNotaFiscalCliente[] notasFiscaisDespachoField;

    //    private string dataGeracaoDespachoField;

    //    private TIndicadorProprietarioLona indicadorProprietarioLonaField;

    //    private TPrefixoTrem prefixoTremPartidaField;

    //    private TDespachoNotaExpedicao notaExpedicaoField;

    //    /// <remarks/>
    //    public TDespachoIdentificadorDespacho identificadorDespacho
    //    {
    //        get
    //        {
    //            return this.identificadorDespachoField;
    //        }
    //        set
    //        {
    //            this.identificadorDespachoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TDespachoFluxoTransporte fluxoTransporte
    //    {
    //        get
    //        {
    //            return this.fluxoTransporteField;
    //        }
    //        set
    //        {
    //            this.fluxoTransporteField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("VagaoDespachado", IsNullable = false)]
    //    public ArrayOfTDespachoVagaoDespachadoVagaoDespachado[] vagoesDespachados
    //    {
    //        get
    //        {
    //            return this.vagoesDespachadosField;
    //        }
    //        set
    //        {
    //            this.vagoesDespachadosField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("refNotaFiscalCliente", IsNullable = false)]
    //    public ArrayOfTDespachoRefNotaFiscalClienteRefNotaFiscalCliente[] notasFiscaisDespacho
    //    {
    //        get
    //        {
    //            return this.notasFiscaisDespachoField;
    //        }
    //        set
    //        {
    //            this.notasFiscaisDespachoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string dataGeracaoDespacho
    //    {
    //        get
    //        {
    //            return this.dataGeracaoDespachoField;
    //        }
    //        set
    //        {
    //            this.dataGeracaoDespachoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TIndicadorProprietarioLona indicadorProprietarioLona
    //    {
    //        get
    //        {
    //            return this.indicadorProprietarioLonaField;
    //        }
    //        set
    //        {
    //            this.indicadorProprietarioLonaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TPrefixoTrem prefixoTremPartida
    //    {
    //        get
    //        {
    //            return this.prefixoTremPartidaField;
    //        }
    //        set
    //        {
    //            this.prefixoTremPartidaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TDespachoNotaExpedicao NotaExpedicao
    //    {
    //        get
    //        {
    //            return this.notaExpedicaoField;
    //        }
    //        set
    //        {
    //            this.notaExpedicaoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TDespachoIdentificadorDespacho
    //{

    //    private TIdentificadorDespacho identificadorDespachoAtualField;

    //    private TIdentificadorDespacho identificadorDespachoOriginalField;

    //    private TIdentificadorDespacho identificadorPrimeiroDespachoOriginalField;

    //    /// <remarks/>
    //    public TIdentificadorDespacho identificadorDespachoAtual
    //    {
    //        get
    //        {
    //            return this.identificadorDespachoAtualField;
    //        }
    //        set
    //        {
    //            this.identificadorDespachoAtualField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TIdentificadorDespacho IdentificadorDespachoOriginal
    //    {
    //        get
    //        {
    //            return this.identificadorDespachoOriginalField;
    //        }
    //        set
    //        {
    //            this.identificadorDespachoOriginalField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TIdentificadorDespacho IdentificadorPrimeiroDespachoOriginal
    //    {
    //        get
    //        {
    //            return this.identificadorPrimeiroDespachoOriginalField;
    //        }
    //        set
    //        {
    //            this.identificadorPrimeiroDespachoOriginalField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TDespachoFluxoTransporte
    //{

    //    private TFluxoFerroviario fluxoFerroviarioField;

    //    private TFluxoFerroviario[] fluxosOutraFerroviaField;

    //    /// <remarks/>
    //    public TFluxoFerroviario FluxoFerroviario
    //    {
    //        get
    //        {
    //            return this.fluxoFerroviarioField;
    //        }
    //        set
    //        {
    //            this.fluxoFerroviarioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("FluxoFerroviario", IsNullable = false)]
    //    public TFluxoFerroviario[] fluxosOutraFerrovia
    //    {
    //        get
    //        {
    //            return this.fluxosOutraFerroviaField;
    //        }
    //        set
    //        {
    //            this.fluxosOutraFerroviaField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class ArrayOfTDespachoVagaoDespachadoVagaoDespachado
    //{

    //    private TCarga[] itensCargaField;

    //    private string quantidadeCargaField;

    //    private ArrayOfTDespachoVagaoDespachadoRefNotaFiscalClienteRefNotaFiscalCliente[] notasFiscaisVagaoDespachadoField;

    //    private string pesoVagaoToneladaBrutaField;

    //    private string pesoCargaToneladaField;

    //    private ArrayOfTDespachoVagaoDespachadoVagaoDespachadoRefVagao refVagaoField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("ItemCarga", IsNullable = false)]
    //    public TCarga[] itensCarga
    //    {
    //        get
    //        {
    //            return this.itensCargaField;
    //        }
    //        set
    //        {
    //            this.itensCargaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    //    public string quantidadeCarga
    //    {
    //        get
    //        {
    //            return this.quantidadeCargaField;
    //        }
    //        set
    //        {
    //            this.quantidadeCargaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("refNotaFiscalCliente", IsNullable = false)]
    //    public ArrayOfTDespachoVagaoDespachadoRefNotaFiscalClienteRefNotaFiscalCliente[] notasFiscaisVagaoDespachado
    //    {
    //        get
    //        {
    //            return this.notasFiscaisVagaoDespachadoField;
    //        }
    //        set
    //        {
    //            this.notasFiscaisVagaoDespachadoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string pesoVagaoToneladaBruta
    //    {
    //        get
    //        {
    //            return this.pesoVagaoToneladaBrutaField;
    //        }
    //        set
    //        {
    //            this.pesoVagaoToneladaBrutaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string pesoCargaTonelada
    //    {
    //        get
    //        {
    //            return this.pesoCargaToneladaField;
    //        }
    //        set
    //        {
    //            this.pesoCargaToneladaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public ArrayOfTDespachoVagaoDespachadoVagaoDespachadoRefVagao refVagao
    //    {
    //        get
    //        {
    //            return this.refVagaoField;
    //        }
    //        set
    //        {
    //            this.refVagaoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class ArrayOfTDespachoVagaoDespachadoRefNotaFiscalClienteRefNotaFiscalCliente
    //{

    //    private string idNotaFiscalField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idNotaFiscal
    //    {
    //        get
    //        {
    //            return this.idNotaFiscalField;
    //        }
    //        set
    //        {
    //            this.idNotaFiscalField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class ArrayOfTDespachoVagaoDespachadoVagaoDespachadoRefVagao
    //{

    //    private string idVagaoField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idVagao
    //    {
    //        get
    //        {
    //            return this.idVagaoField;
    //        }
    //        set
    //        {
    //            this.idVagaoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class ArrayOfTDespachoRefNotaFiscalClienteRefNotaFiscalCliente
    //{

    //    private string idNotaFiscalField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idNotaFiscal
    //    {
    //        get
    //        {
    //            return this.idNotaFiscalField;
    //        }
    //        set
    //        {
    //            this.idNotaFiscalField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public enum TIndicadorProprietarioLona
    //{

    //    /// <remarks/>
    //    SEM_LONA,

    //    /// <remarks/>
    //    LONA_TRANSPORTADORA,

    //    /// <remarks/>
    //    LONA_CLIENTE,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
    //public partial class TPrefixoTrem
    //{

    //    private string codigoPrefixoTremField;

    //    private System.DateTime dataHoraInicioValidadePrefixoField;

    //    /// <remarks/>
    //    public string codigoPrefixoTrem
    //    {
    //        get
    //        {
    //            return this.codigoPrefixoTremField;
    //        }
    //        set
    //        {
    //            this.codigoPrefixoTremField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public System.DateTime dataHoraInicioValidadePrefixo
    //    {
    //        get
    //        {
    //            return this.dataHoraInicioValidadePrefixoField;
    //        }
    //        set
    //        {
    //            this.dataHoraInicioValidadePrefixoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
    //public partial class TDespachoNotaExpedicao
    //{

    //    private string codigoComposicaoClienteField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string codigoComposicaoCliente
    //    {
    //        get
    //        {
    //            return this.codigoComposicaoClienteField;
    //        }
    //        set
    //        {
    //            this.codigoComposicaoClienteField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
    //public partial class TItemNotaFiscal
    //{

    //    private string valorField;

    //    private string descricaoDetalhadaField;

    //    private string codigoDescricaoDetalhadaField;

    //    private string codigoMercadoriaField;

    //    private string pesoItemToneladasField;

    //    private string quantidadeUnidadeMedidaField;

    //    private string descricaoUnidadeMedidaField;

    //    /// <remarks/>
    //    public string valor
    //    {
    //        get
    //        {
    //            return this.valorField;
    //        }
    //        set
    //        {
    //            this.valorField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string descricaoDetalhada
    //    {
    //        get
    //        {
    //            return this.descricaoDetalhadaField;
    //        }
    //        set
    //        {
    //            this.descricaoDetalhadaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string codigoDescricaoDetalhada
    //    {
    //        get
    //        {
    //            return this.codigoDescricaoDetalhadaField;
    //        }
    //        set
    //        {
    //            this.codigoDescricaoDetalhadaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string codigoMercadoria
    //    {
    //        get
    //        {
    //            return this.codigoMercadoriaField;
    //        }
    //        set
    //        {
    //            this.codigoMercadoriaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string pesoItemToneladas
    //    {
    //        get
    //        {
    //            return this.pesoItemToneladasField;
    //        }
    //        set
    //        {
    //            this.pesoItemToneladasField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    //    public string quantidadeUnidadeMedida
    //    {
    //        get
    //        {
    //            return this.quantidadeUnidadeMedidaField;
    //        }
    //        set
    //        {
    //            this.quantidadeUnidadeMedidaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string descricaoUnidadeMedida
    //    {
    //        get
    //        {
    //            return this.descricaoUnidadeMedidaField;
    //        }
    //        set
    //        {
    //            this.descricaoUnidadeMedidaField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
    //public partial class TEndereco
    //{

    //    private string xLgrField;

    //    private string nroField;

    //    private string xCplField;

    //    private string xBairroField;

    //    private string cMunField;

    //    private string xMunField;

    //    private string cEPField;

    //    private TUf ufField;

    //    private string cPaisField;

    //    private string xPaisField;

    //    /// <remarks/>
    //    public string xLgr
    //    {
    //        get
    //        {
    //            return this.xLgrField;
    //        }
    //        set
    //        {
    //            this.xLgrField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string nro
    //    {
    //        get
    //        {
    //            return this.nroField;
    //        }
    //        set
    //        {
    //            this.nroField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string xCpl
    //    {
    //        get
    //        {
    //            return this.xCplField;
    //        }
    //        set
    //        {
    //            this.xCplField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string xBairro
    //    {
    //        get
    //        {
    //            return this.xBairroField;
    //        }
    //        set
    //        {
    //            this.xBairroField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string cMun
    //    {
    //        get
    //        {
    //            return this.cMunField;
    //        }
    //        set
    //        {
    //            this.cMunField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string xMun
    //    {
    //        get
    //        {
    //            return this.xMunField;
    //        }
    //        set
    //        {
    //            this.xMunField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string CEP
    //    {
    //        get
    //        {
    //            return this.cEPField;
    //        }
    //        set
    //        {
    //            this.cEPField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TUf UF
    //    {
    //        get
    //        {
    //            return this.ufField;
    //        }
    //        set
    //        {
    //            this.ufField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string cPais
    //    {
    //        get
    //        {
    //            return this.cPaisField;
    //        }
    //        set
    //        {
    //            this.cPaisField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string xPais
    //    {
    //        get
    //        {
    //            return this.xPaisField;
    //        }
    //        set
    //        {
    //            this.xPaisField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/basicos")]
    //public enum TUf
    //{

    //    /// <remarks/>
    //    AC,

    //    /// <remarks/>
    //    AL,

    //    /// <remarks/>
    //    AM,

    //    /// <remarks/>
    //    AP,

    //    /// <remarks/>
    //    BA,

    //    /// <remarks/>
    //    CE,

    //    /// <remarks/>
    //    DF,

    //    /// <remarks/>
    //    ES,

    //    /// <remarks/>
    //    GO,

    //    /// <remarks/>
    //    MA,

    //    /// <remarks/>
    //    MG,

    //    /// <remarks/>
    //    MS,

    //    /// <remarks/>
    //    MT,

    //    /// <remarks/>
    //    PA,

    //    /// <remarks/>
    //    PB,

    //    /// <remarks/>
    //    PE,

    //    /// <remarks/>
    //    PI,

    //    /// <remarks/>
    //    PR,

    //    /// <remarks/>
    //    RJ,

    //    /// <remarks/>
    //    RN,

    //    /// <remarks/>
    //    RO,

    //    /// <remarks/>
    //    RR,

    //    /// <remarks/>
    //    RS,

    //    /// <remarks/>
    //    SC,

    //    /// <remarks/>
    //    SE,

    //    /// <remarks/>
    //    SP,

    //    /// <remarks/>
    //    TO,

    //    /// <remarks/>
    //    EX,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
    //public partial class TNotaFiscalCliente
    //{

    //    private string nDocField;

    //    private string dEmiField;

    //    private string valorField;

    //    private string codigoDocumentoReferenciaField;

    //    private TNotaFiscalClienteEspelhoNotaFiscal espelhoNotaFiscalField;

    //    private TItemNotaFiscal[] itensNotaFiscalField;

    //    private string idNotaFiscalField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string nDoc
    //    {
    //        get
    //        {
    //            return this.nDocField;
    //        }
    //        set
    //        {
    //            this.nDocField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string dEmi
    //    {
    //        get
    //        {
    //            return this.dEmiField;
    //        }
    //        set
    //        {
    //            this.dEmiField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string valor
    //    {
    //        get
    //        {
    //            return this.valorField;
    //        }
    //        set
    //        {
    //            this.valorField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string codigoDocumentoReferencia
    //    {
    //        get
    //        {
    //            return this.codigoDocumentoReferenciaField;
    //        }
    //        set
    //        {
    //            this.codigoDocumentoReferenciaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TNotaFiscalClienteEspelhoNotaFiscal EspelhoNotaFiscal
    //    {
    //        get
    //        {
    //            return this.espelhoNotaFiscalField;
    //        }
    //        set
    //        {
    //            this.espelhoNotaFiscalField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("ItemNotaFiscal", IsNullable = false)]
    //    public TItemNotaFiscal[] itensNotaFiscal
    //    {
    //        get
    //        {
    //            return this.itensNotaFiscalField;
    //        }
    //        set
    //        {
    //            this.itensNotaFiscalField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idNotaFiscal
    //    {
    //        get
    //        {
    //            return this.idNotaFiscalField;
    //        }
    //        set
    //        {
    //            this.idNotaFiscalField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
    //public partial class TNotaFiscalClienteEspelhoNotaFiscal
    //{

    //    private TNotaFiscalClienteEspelhoNotaFiscalEmitente emitenteField;

    //    private TNotaFiscalClienteEspelhoNotaFiscalDestinatario destinatarioField;

    //    private string chaveField;

    //    private string cFOPField;

    //    private string vBCField;

    //    private string vICMSField;

    //    private string vBCSTField;

    //    private string vSTField;

    //    private string vProdField;

    //    private string serieField;

    //    private string nPesoField;

    //    /// <remarks/>
    //    public TNotaFiscalClienteEspelhoNotaFiscalEmitente emitente
    //    {
    //        get
    //        {
    //            return this.emitenteField;
    //        }
    //        set
    //        {
    //            this.emitenteField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TNotaFiscalClienteEspelhoNotaFiscalDestinatario destinatario
    //    {
    //        get
    //        {
    //            return this.destinatarioField;
    //        }
    //        set
    //        {
    //            this.destinatarioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string chave
    //    {
    //        get
    //        {
    //            return this.chaveField;
    //        }
    //        set
    //        {
    //            this.chaveField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string CFOP
    //    {
    //        get
    //        {
    //            return this.cFOPField;
    //        }
    //        set
    //        {
    //            this.cFOPField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string vBC
    //    {
    //        get
    //        {
    //            return this.vBCField;
    //        }
    //        set
    //        {
    //            this.vBCField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string vICMS
    //    {
    //        get
    //        {
    //            return this.vICMSField;
    //        }
    //        set
    //        {
    //            this.vICMSField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string vBCST
    //    {
    //        get
    //        {
    //            return this.vBCSTField;
    //        }
    //        set
    //        {
    //            this.vBCSTField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string vST
    //    {
    //        get
    //        {
    //            return this.vSTField;
    //        }
    //        set
    //        {
    //            this.vSTField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string vProd
    //    {
    //        get
    //        {
    //            return this.vProdField;
    //        }
    //        set
    //        {
    //            this.vProdField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
    //    public string serie
    //    {
    //        get
    //        {
    //            return this.serieField;
    //        }
    //        set
    //        {
    //            this.serieField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string nPeso
    //    {
    //        get
    //        {
    //            return this.nPesoField;
    //        }
    //        set
    //        {
    //            this.nPesoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
    //public partial class TNotaFiscalClienteEspelhoNotaFiscalEmitente
    //{

    //    private string itemField;

    //    private ItemChoiceType itemElementNameField;

    //    private string ieField;

    //    private string xNomeField;

    //    private TEndereco enderEmitField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("CNPJ", typeof(string))]
    //    [System.Xml.Serialization.XmlElementAttribute("CPF", typeof(string))]
    //    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
    //    public string Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public ItemChoiceType ItemElementName
    //    {
    //        get
    //        {
    //            return this.itemElementNameField;
    //        }
    //        set
    //        {
    //            this.itemElementNameField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string IE
    //    {
    //        get
    //        {
    //            return this.ieField;
    //        }
    //        set
    //        {
    //            this.ieField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string xNome
    //    {
    //        get
    //        {
    //            return this.xNomeField;
    //        }
    //        set
    //        {
    //            this.xNomeField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TEndereco enderEmit
    //    {
    //        get
    //        {
    //            return this.enderEmitField;
    //        }
    //        set
    //        {
    //            this.enderEmitField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal", IncludeInSchema = false)]
    //public enum ItemChoiceType
    //{

    //    /// <remarks/>
    //    CNPJ,

    //    /// <remarks/>
    //    CPF,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
    //public partial class TNotaFiscalClienteEspelhoNotaFiscalDestinatario
    //{

    //    private string itemField;

    //    private ItemChoiceType1 itemElementNameField;

    //    private string ieField;

    //    private string xNomeField;

    //    private TEndereco enderDestField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("CNPJ", typeof(string))]
    //    [System.Xml.Serialization.XmlElementAttribute("CPF", typeof(string))]
    //    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
    //    public string Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlIgnoreAttribute()]
    //    public ItemChoiceType1 ItemElementName
    //    {
    //        get
    //        {
    //            return this.itemElementNameField;
    //        }
    //        set
    //        {
    //            this.itemElementNameField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string IE
    //    {
    //        get
    //        {
    //            return this.ieField;
    //        }
    //        set
    //        {
    //            this.ieField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string xNome
    //    {
    //        get
    //        {
    //            return this.xNomeField;
    //        }
    //        set
    //        {
    //            this.xNomeField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TEndereco enderDest
    //    {
    //        get
    //        {
    //            return this.enderDestField;
    //        }
    //        set
    //        {
    //            this.enderDestField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal", IncludeInSchema = false)]
    //public enum ItemChoiceType1
    //{

    //    /// <remarks/>
    //    CNPJ,

    //    /// <remarks/>
    //    CPF,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
    //public partial class TLocomotiva
    //{

    //    private string identificadorField;

    //    private string pesoBrutoField;

    //    private string cNPJProprietarioField;

    //    private string idLocomotivaField;

    //    /// <remarks/>
    //    public string identificador
    //    {
    //        get
    //        {
    //            return this.identificadorField;
    //        }
    //        set
    //        {
    //            this.identificadorField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string pesoBruto
    //    {
    //        get
    //        {
    //            return this.pesoBrutoField;
    //        }
    //        set
    //        {
    //            this.pesoBrutoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string CNPJProprietario
    //    {
    //        get
    //        {
    //            return this.cNPJProprietarioField;
    //        }
    //        set
    //        {
    //            this.cNPJProprietarioField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string IdLocomotiva
    //    {
    //        get
    //        {
    //            return this.idLocomotivaField;
    //        }
    //        set
    //        {
    //            this.idLocomotivaField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoTerminal
    //{

    //    private TClassificacaoParalisacao paralisacaoField;

    //    /// <remarks/>
    //    public TClassificacaoParalisacao paralisacao
    //    {
    //        get
    //        {
    //            return this.paralisacaoField;
    //        }
    //        set
    //        {
    //            this.paralisacaoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public enum TClassificacaoParalisacao
    //{

    //    /// <remarks/>
    //    PREVENTIVA,

    //    /// <remarks/>
    //    CORRETIVA,

    //    /// <remarks/>
    //    SOLICITACAO,

    //    /// <remarks/>
    //    FUNCIONAMENTO,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoTrem
    //{

    //    private TEventoEventoTremTrem tremField;

    //    private TEventoEventoTremVeiculos veiculosField;

    //    /// <remarks/>
    //    public TEventoEventoTremTrem Trem
    //    {
    //        get
    //        {
    //            return this.tremField;
    //        }
    //        set
    //        {
    //            this.tremField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TEventoEventoTremVeiculos veiculos
    //    {
    //        get
    //        {
    //            return this.veiculosField;
    //        }
    //        set
    //        {
    //            this.veiculosField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoTremTrem
    //{

    //    private TPrefixoTrem prefixoTremField;

    //    private string tabelaField;

    //    private TEstadoTrem estadoTremField;

    //    /// <remarks/>
    //    public TPrefixoTrem prefixoTrem
    //    {
    //        get
    //        {
    //            return this.prefixoTremField;
    //        }
    //        set
    //        {
    //            this.prefixoTremField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string Tabela
    //    {
    //        get
    //        {
    //            return this.tabelaField;
    //        }
    //        set
    //        {
    //            this.tabelaField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TEstadoTrem estadoTrem
    //    {
    //        get
    //        {
    //            return this.estadoTremField;
    //        }
    //        set
    //        {
    //            this.estadoTremField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public enum TEstadoTrem
    //{

    //    /// <remarks/>
    //    EM_CIRCULACAO,

    //    /// <remarks/>
    //    ENCERRADO,

    //    /// <remarks/>
    //    SUPRIMIDO,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoTremVeiculos
    //{

    //    private object itemField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("locomotivas", typeof(TEventoEventoTremVeiculosLocomotivas))]
    //    [System.Xml.Serialization.XmlElementAttribute("vagoes", typeof(TEventoEventoTremVeiculosVagoes))]
    //    public object Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoTremVeiculosLocomotivas
    //{

    //    private TEventoEventoTremVeiculosLocomotivasRefLocomotiva[] refLocomotivaField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("refLocomotiva")]
    //    public TEventoEventoTremVeiculosLocomotivasRefLocomotiva[] refLocomotiva
    //    {
    //        get
    //        {
    //            return this.refLocomotivaField;
    //        }
    //        set
    //        {
    //            this.refLocomotivaField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoTremVeiculosLocomotivasRefLocomotiva
    //{

    //    private TFuncaoLocomotiva funcaoField;

    //    private TSentidoLocomotiva sentidoField;

    //    private string posicaoField;

    //    private string idLocomotivaField;

    //    /// <remarks/>
    //    public TFuncaoLocomotiva funcao
    //    {
    //        get
    //        {
    //            return this.funcaoField;
    //        }
    //        set
    //        {
    //            this.funcaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TSentidoLocomotiva sentido
    //    {
    //        get
    //        {
    //            return this.sentidoField;
    //        }
    //        set
    //        {
    //            this.sentidoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "positiveInteger")]
    //    public string posicao
    //    {
    //        get
    //        {
    //            return this.posicaoField;
    //        }
    //        set
    //        {
    //            this.posicaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idLocomotiva
    //    {
    //        get
    //        {
    //            return this.idLocomotivaField;
    //        }
    //        set
    //        {
    //            this.idLocomotivaField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
    //public enum TFuncaoLocomotiva
    //{

    //    /// <remarks/>
    //    TRACIONANDO,

    //    /// <remarks/>
    //    REBOCADA,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
    //public enum TSentidoLocomotiva
    //{

    //    /// <remarks/>
    //    FRENTE,

    //    /// <remarks/>
    //    RE,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoTremVeiculosVagoes
    //{

    //    private TEventoEventoTremVeiculosVagoesRefVagao[] refVagaoField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("refVagao")]
    //    public TEventoEventoTremVeiculosVagoesRefVagao[] refVagao
    //    {
    //        get
    //        {
    //            return this.refVagaoField;
    //        }
    //        set
    //        {
    //            this.refVagaoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoTremVeiculosVagoesRefVagao
    //{

    //    private string posicaoField;

    //    private string idVagaoField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "positiveInteger")]
    //    public string posicao
    //    {
    //        get
    //        {
    //            return this.posicaoField;
    //        }
    //        set
    //        {
    //            this.posicaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idVagao
    //    {
    //        get
    //        {
    //            return this.idVagaoField;
    //        }
    //        set
    //        {
    //            this.idVagaoField = value;
    //        }
    //    }
    //}



    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoVeiculoVeiculos
    //{

    //    private object itemField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("locomotivas", typeof(TEventoEventoVeiculoVeiculosLocomotivas))]
    //    [System.Xml.Serialization.XmlElementAttribute("vagoes", typeof(TEventoEventoVeiculoVeiculosVagoes))]
    //    public object Item
    //    {
    //        get
    //        {
    //            return this.itemField;
    //        }
    //        set
    //        {
    //            this.itemField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoVeiculoVeiculosLocomotivas
    //{

    //    private TEventoEventoVeiculoVeiculosLocomotivasRefLocomotiva[] refLocomotivaField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("refLocomotiva")]
    //    public TEventoEventoVeiculoVeiculosLocomotivasRefLocomotiva[] refLocomotiva
    //    {
    //        get
    //        {
    //            return this.refLocomotivaField;
    //        }
    //        set
    //        {
    //            this.refLocomotivaField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoVeiculoVeiculosLocomotivasRefLocomotiva
    //{

    //    private TFuncaoLocomotiva funcaoField;

    //    private TSentidoLocomotiva sentidoField;

    //    private string posicaoField;

    //    private string idLocomotivaField;

    //    /// <remarks/>
    //    public TFuncaoLocomotiva funcao
    //    {
    //        get
    //        {
    //            return this.funcaoField;
    //        }
    //        set
    //        {
    //            this.funcaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public TSentidoLocomotiva sentido
    //    {
    //        get
    //        {
    //            return this.sentidoField;
    //        }
    //        set
    //        {
    //            this.sentidoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "positiveInteger")]
    //    public string posicao
    //    {
    //        get
    //        {
    //            return this.posicaoField;
    //        }
    //        set
    //        {
    //            this.posicaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idLocomotiva
    //    {
    //        get
    //        {
    //            return this.idLocomotivaField;
    //        }
    //        set
    //        {
    //            this.idLocomotivaField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoVeiculoVeiculosVagoes
    //{

    //    private TEventoEventoVeiculoVeiculosVagoesRefVagao[] refVagaoField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute("refVagao")]
    //    public TEventoEventoVeiculoVeiculosVagoesRefVagao[] refVagao
    //    {
    //        get
    //        {
    //            return this.refVagaoField;
    //        }
    //        set
    //        {
    //            this.refVagaoField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.8.3928.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    //public partial class TEventoEventoVeiculoVeiculosVagoesRefVagao
    //{

    //    private string posicaoField;

    //    private string idVagaoField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "positiveInteger")]
    //    public string posicao
    //    {
    //        get
    //        {
    //            return this.posicaoField;
    //        }
    //        set
    //        {
    //            this.posicaoField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string idVagao
    //    {
    //        get
    //        {
    //            return this.idVagaoField;
    //        }
    //        set
    //        {
    //            this.idVagaoField = value;
    //        }
    //    }
    //}


    [System.ServiceModel.MessageContract(IsWrapped = false, WrapperNamespace = "http://xmlns.mrs.com.br/iti/tipos/eventosFerro")]
    public partial class TEventoFerroviario
    {
        [MessageBodyMember(Name = "EventoFerroviario", Namespace = "http://xmlns.mrs.com.br/iti/tipos/eventosFerro")]
        public EventoFerroviario data { get; set; }
    }


    [XmlRoot(ElementName = "Vagao")]
    public class Vagao
    {

        [XmlElement(ElementName = "tipoVagao", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string tipoVagao { get; set; }

        [XmlElement(ElementName = "identificador", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string identificador { get; set; }

        [XmlElement(ElementName = "lotacao", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string lotacao { get; set; }

        [XmlElement(ElementName = "tara", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string tara { get; set; }

        [XmlElement(ElementName = "capacidadeNominal", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string capacidadeNominal { get; set; }

        [XmlElement(ElementName = "CNPJProprietario", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string CNPJProprietario { get; set; }

    }


    [XmlRoot(ElementName = "Locomotiva")]
    public class Locomotiva
    {

        [XmlElement(ElementName = "identificador", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string Identificador { get; set; }

        [XmlElement(ElementName = "pesoBruto", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string PesoBruto { get; set; }

        [XmlElement(ElementName = "CNPJProprietario", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string CNPJProprietario { get; set; }

        [XmlAttribute(AttributeName = "IdLocomotiva")]
        public string IdLocomotiva { get; set; }

    }


    [XmlRoot(ElementName = "veiculosMensagem", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    public class VeiculosMensagem
    {

        [XmlElement(ElementName = "vagoes")]
        public Vagao[] vagoes { get; set; }

        [XmlElement(ElementName = "locomotivas")]
        public Locomotiva[] locomotivas { get; set; }

    }

    [XmlRoot(ElementName = "prefixoTrem")]
    public class PrefixoTrem
    {

        [XmlElement(ElementName = "codigoPrefixoTrem", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string codigoPrefixoTrem { get; set; }

        [XmlElement(ElementName = "dataHoraInicioValidadePrefixo", Namespace = "http://xmlns.mrs.com.br/iti/tipos/comuns")]
        public string dataHoraInicioValidadePrefixo { get; set; }
    }

    [XmlRoot(ElementName = "Trem")]
    public class Trem
    {

        [XmlElement(ElementName = "prefixoTrem")]
        public PrefixoTrem prefixoTrem { get; set; }

        [XmlElement(ElementName = "Tabela")]
        public string Tabela { get; set; }

        [XmlElement(ElementName = "estadoTrem")]
        public string estadoTrem { get; set; }
    }

    [XmlRoot(ElementName = "refLocomotiva")]
    public class RefLocomotiva
    {

        [XmlElement(ElementName = "funcao")]
        public string funcao { get; set; }

        [XmlElement(ElementName = "sentido")]
        public string sentido { get; set; }

        [XmlAttribute(AttributeName = "posicao")]
        public string posicao { get; set; }

        [XmlAttribute(AttributeName = "idLocomotiva")]
        public string idLocomotiva { get; set; }
    }

    [XmlRoot(ElementName = "veiculos")]
    public class Veiculos
    {

        [XmlElement(ElementName = "locomotivas")]
        public RefLocomotiva[] locomotivas { get; set; }

        [XmlElement(ElementName = "vagoes")]
        public RefVagao[] vagoes { get; set; }

    }

    [XmlRoot(ElementName = "EventoTrem")]
    public class EventoTrem
    {

        [XmlElement(ElementName = "Trem")]
        public Trem Trem { get; set; }

        [XmlElement(ElementName = "veiculos")]
        public Veiculos Veiculos { get; set; }

    }

    [XmlRoot(ElementName = "enderEmit")]
    public class EnderEmit
    {

        [XmlElement(ElementName = "xLgr")]
        public string xLgr { get; set; }

        [XmlElement(ElementName = "nro")]
        public string nro { get; set; }

        [XmlElement(ElementName = "xCpl")]
        public string xCpl { get; set; }

        [XmlElement(ElementName = "xBairro")]
        public string xBairro { get; set; }

        [XmlElement(ElementName = "cMun")]
        public string cMun { get; set; }

        [XmlElement(ElementName = "xMun")]
        public string xMun { get; set; }

        [XmlElement(ElementName = "CEP")]
        public string CEP { get; set; }

        [XmlElement(ElementName = "UF")]
        public string UF { get; set; }

        [XmlElement(ElementName = "cPais")]
        public string cPais { get; set; }

        [XmlElement(ElementName = "xPais")]
        public string xPais { get; set; }
    }

    [XmlRoot(ElementName = "emitente")]
    public class Emitente
    {

        [XmlElement(ElementName = "CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement(ElementName = "IE")]
        public string IE { get; set; }

        [XmlElement(ElementName = "xNome")]
        public string XNome { get; set; }

        [XmlElement(ElementName = "enderEmit")]
        public EnderEmit EnderEmit { get; set; }
    }

    [XmlRoot(ElementName = "enderDest")]
    public class EnderDest
    {

        [XmlElement(ElementName = "xLgr")]
        public string XLgr { get; set; }

        [XmlElement(ElementName = "nro")]
        public string Nro { get; set; }

        [XmlElement(ElementName = "xCpl")]
        public string XCpl { get; set; }

        [XmlElement(ElementName = "xBairro")]
        public string XBairro { get; set; }

        [XmlElement(ElementName = "cMun")]
        public string CMun { get; set; }

        [XmlElement(ElementName = "xMun")]
        public string XMun { get; set; }

        [XmlElement(ElementName = "CEP")]
        public string CEP { get; set; }

        [XmlElement(ElementName = "UF")]
        public string UF { get; set; }

        [XmlElement(ElementName = "cPais")]
        public string CPais { get; set; }

        [XmlElement(ElementName = "xPais")]
        public string XPais { get; set; }
    }

    [XmlRoot(ElementName = "destinatario")]
    public class Destinatario
    {

        [XmlElement(ElementName = "CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement(ElementName = "IE")]
        public string IE { get; set; }

        [XmlElement(ElementName = "xNome")]
        public string XNome { get; set; }

        [XmlElement(ElementName = "enderDest")]
        public EnderDest EnderDest { get; set; }
    }

    [XmlRoot(ElementName = "EspelhoNotaFiscal")]
    public class EspelhoNotaFiscal
    {

        [XmlElement(ElementName = "emitente")]
        public Emitente emitente { get; set; }

        [XmlElement(ElementName = "destinatario")]
        public Destinatario destinatario { get; set; }

        [XmlElement(ElementName = "chave")]
        public string chave { get; set; }

        [XmlElement(ElementName = "CFOP")]
        public string CFOP { get; set; }

        [XmlElement(ElementName = "vBC")]
        public string VBC { get; set; }

        [XmlElement(ElementName = "vICMS")]
        public string vICMS { get; set; }

        [XmlElement(ElementName = "vBCST")]
        public string vBCST { get; set; }

        [XmlElement(ElementName = "vST")]
        public string vST { get; set; }

        [XmlElement(ElementName = "vProd")]
        public string vProd { get; set; }

        [XmlElement(ElementName = "serie")]
        public string serie { get; set; }

        [XmlElement(ElementName = "nPeso")]
        public string nPeso { get; set; }

    }

    [XmlRoot(ElementName = "ItemNotaFiscal")]
    public class ItemNotaFiscal
    {

        [XmlElement(ElementName = "valor")]
        public string valor { get; set; }

        [XmlElement(ElementName = "descricaoDetalhada")]
        public string descricaoDetalhada { get; set; }

        [XmlElement(ElementName = "codigoDescricaoDetalhada")]
        public string codigoDescricaoDetalhada { get; set; }

        [XmlElement(ElementName = "codigoMercadoria")]
        public string codigoMercadoria { get; set; }

        [XmlElement(ElementName = "pesoItemToneladas")]
        public string pesoItemToneladas { get; set; }

        [XmlElement(ElementName = "quantidadeUnidadeMedida")]
        public string quantidadeUnidadeMedida { get; set; }

        [XmlElement(ElementName = "descricaoUnidadeMedida")]
        public string descricaoUnidadeMedida { get; set; }
    }

    [XmlRoot(ElementName = "itensNotaFiscal")]
    public class ItensNotaFiscal
    {

        [XmlElement(ElementName = "ItemNotaFiscal")]
        public List<ItemNotaFiscal> ItemNotaFiscal { get; set; }

    }

    [XmlRoot(ElementName = "NotaFiscalCliente")]
    public class NotaFiscalCliente
    {

        [XmlElement(ElementName = "nDoc", Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
        public string nDoc { get; set; }

        [XmlElement(ElementName = "dEmi", Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
        public string dEmi { get; set; }

        [XmlElement(ElementName = "valor", Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
        public string valor { get; set; }

        [XmlElement(ElementName = "codigoDocumentoReferencia", Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
        public string codigoDocumentoReferencia { get; set; }

        [XmlElement(ElementName = "EspelhoNotaFiscal", Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
        public EspelhoNotaFiscal EspelhoNotaFiscal { get; set; }

        [XmlElement(ElementName = "itensNotaFiscal", Namespace = "http://xmlns.mrs.com.br/iti/tipos/notaFiscal")]
        public ItensNotaFiscal itensNotaFiscal { get; set; }

        [XmlAttribute(AttributeName = "idNotaFiscal")]
        public string idNotaFiscal { get; set; }
    }

    [XmlRoot(ElementName = "notasFiscais", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
    public class NotasFiscais
    {

        [XmlElement(ElementName = "NotaFiscalCliente")]
        public NotaFiscalCliente[] NotaFiscalCliente { get; set; }

        [XmlElement(ElementName = "refNotaFiscalCliente")]
        public refNotaFiscalCliente[] refNotaFiscalCliente { get; set; }
    }

    [XmlRoot(ElementName = "identificadorDespachoAtual")]
    public class IdentificadorDespachoAtual
    {

        [XmlElement(ElementName = "serie")]
        public string serie { get; set; }

        [XmlElement(ElementName = "serieFiscal")]
        public string serieFiscal { get; set; }

        [XmlElement(ElementName = "numero")]
        public string numero { get; set; }

        [XmlElement(ElementName = "numeroFiscal")]
        public string numeroFiscal { get; set; }
    }

    [XmlRoot(ElementName = "IdentificadorDespachoOriginal")]
    public class IdentificadorDespachoOriginal
    {

        [XmlElement(ElementName = "serie")]
        public string serie { get; set; }

        [XmlElement(ElementName = "serieFiscal")]
        public string serieFiscal { get; set; }

        [XmlElement(ElementName = "numero")]
        public string numero { get; set; }

        [XmlElement(ElementName = "numeroFiscal")]
        public string numeroFiscal { get; set; }
    }

    [XmlRoot(ElementName = "IdentificadorPrimeiroDespachoOriginal")]
    public class IdentificadorPrimeiroDespachoOriginal
    {

        [XmlElement(ElementName = "serie")]
        public string serie { get; set; }

        [XmlElement(ElementName = "serieFiscal")]
        public string serieFiscal { get; set; }

        [XmlElement(ElementName = "numero")]
        public string numero { get; set; }

        [XmlElement(ElementName = "numeroFiscal")]
        public string numeroFiscal { get; set; }
    }

    [XmlRoot(ElementName = "identificadorDespacho")]
    public class IdentificadorDespacho
    {

        [XmlElement(ElementName = "identificadorDespachoAtual")]
        public IdentificadorDespachoAtual identificadorDespachoAtual { get; set; }

        [XmlElement(ElementName = "IdentificadorDespachoOriginal")]
        public IdentificadorDespachoOriginal IdentificadorDespachoOriginal { get; set; }

        [XmlElement(ElementName = "IdentificadorPrimeiroDespachoOriginal")]
        public IdentificadorPrimeiroDespachoOriginal IdentificadorPrimeiroDespachoOriginal { get; set; }

    }

    [XmlRoot(ElementName = "Correntista")]
    public class Correntista
    {

        [XmlElement(ElementName = "CNPJ")]
        public string CNPJ { get; set; }
    }

    [XmlRoot(ElementName = "Remetente")]
    public class Remetente
    {

        [XmlElement(ElementName = "CNPJ")]
        public string CNPJ { get; set; }
    }


    [XmlRoot(ElementName = "Mercadoria")]
    public class Mercadoria
    {

        [XmlElement(ElementName = "descricaoResumida")]
        public string descricaoResumida { get; set; }

        [XmlElement(ElementName = "descricaoDetalhada")]
        public string descricaoDetalhada { get; set; }

        [XmlElement(ElementName = "codigo")]
        public string codigo { get; set; }
    }

    [XmlRoot(ElementName = "rota")]
    public class Rota
    {

        [XmlElement(ElementName = "siglaPatioOrigem")]
        public string siglaPatioOrigem { get; set; }

        [XmlElement(ElementName = "siglaTerminalOrigem")]
        public string siglaTerminalOrigem { get; set; }

        [XmlElement(ElementName = "siglaPatioDestino")]
        public string siglaPatioDestino { get; set; }

        [XmlElement(ElementName = "siglaTerminalDestino")]
        public string siglaTerminalDestino { get; set; }

        [XmlElement(ElementName = "siglaPatioIntercambioOrigem")]
        public string siglaPatioIntercambioOrigem { get; set; }

        [XmlElement(ElementName = "siglaPatioIntercambioDestino")]
        public string siglaPatioIntercambioDestino { get; set; }
    }

    [XmlRoot(ElementName = "FluxoFerroviario")]
    public class FluxoFerroviario
    {

        [XmlElement(ElementName = "numeroFluxo")]
        public string numeroFluxo { get; set; }

        [XmlElement(ElementName = "CNPJFerrovia")]
        public string CNPJFerrovia { get; set; }

        [XmlElement(ElementName = "Correntista")]
        public Correntista Correntista { get; set; }

        [XmlElement(ElementName = "Remetente")]
        public Remetente Remetente { get; set; }

        [XmlElement(ElementName = "Destinatario")]
        public Destinatario Destinatario { get; set; }

        [XmlElement(ElementName = "Mercadoria")]
        public Mercadoria Mercadoria { get; set; }

        [XmlElement(ElementName = "rota")]
        public Rota rota { get; set; }

        [XmlElement(ElementName = "unidadeCobranca")]
        public string unidadeCobranca { get; set; }
    }

    //[XmlRoot(ElementName = "fluxosOutraFerrovia")]
    //public class FluxosOutraFerrovia
    //{

    //    [XmlElement(ElementName = "FluxoFerroviario")]
    //    public List<FluxoFerroviario> FluxoFerroviario { get; set; }
    //}

    [XmlRoot(ElementName = "fluxoTransporte")]
    public class FluxoTransporte
    {

        [XmlElement(ElementName = "FluxoFerroviario")]
        public FluxoFerroviario FluxoFerroviario { get; set; }

        [XmlElement(ElementName = "fluxosOutraFerrovia")]
        public FluxoFerroviario[] FluxosOutraFerrovia { get; set; }
    }

    [XmlRoot(ElementName = "refNotaFiscalCliente")]
    public class refNotaFiscalCliente
    {
        [XmlAttribute(AttributeName = "idNotaFiscal")]
        public string idNotaFiscal { get; set; }
    }

    [XmlRoot(ElementName = "lacres")]
    public class Lacres
    {

        [XmlElement(ElementName = "nLacre")]
        public List<string> nLacre { get; set; }
    }

    [XmlRoot(ElementName = "ItemCarga")]
    public class ItemCarga
    {

        [XmlElement(ElementName = "numeroContainer")]
        public string numeroContainer { get; set; }

        [XmlElement(ElementName = "tamanhoContainer")]
        public string tamanhoContainer { get; set; }

        [XmlElement(ElementName = "notasFiscais")]
        public NotasFiscais notasFiscais { get; set; }

        [XmlElement(ElementName = "lacres")]
        public Lacres lacres { get; set; }
    }

    [XmlRoot(ElementName = "refVagao")]
    public class RefVagao
    {

        [XmlAttribute(AttributeName = "idVagao")]
        public string idVagao { get; set; }
    }

    [XmlRoot(ElementName = "VagaoDespachado")]
    public class VagaoDespachado
    {

        [XmlElement(ElementName = "itensCarga")]
        public ItemCarga[] itensCarga { get; set; }

        [XmlElement(ElementName = "quantidadeCarga")]
        public string quantidadeCarga { get; set; }

        [XmlElement(ElementName = "notasFiscaisVagaoDespachado")]
        public refNotaFiscalCliente[] notasFiscaisVagaoDespachado { get; set; }

        [XmlElement(ElementName = "pesoVagaoToneladaBruta")]
        public string pesoVagaoToneladaBruta { get; set; }

        [XmlElement(ElementName = "pesoCargaTonelada")]
        public string pesoCargaTonelada { get; set; }

        [XmlElement(ElementName = "refVagao")]
        public RefVagao refVagao { get; set; }
    }


    [XmlRoot(ElementName = "NotaExpedicao")]
    public class NotaExpedicao
    {

        [XmlElement(ElementName = "codigoComposicaoCliente")]
        public string codigoComposicaoCliente { get; set; }

    }

    [XmlRoot(ElementName = "Despacho")]
    public class Despacho
    {

        [XmlElement(ElementName = "identificadorDespacho", Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
        public IdentificadorDespacho identificadorDespacho { get; set; }

        [XmlElement(ElementName = "fluxoTransporte", Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
        public FluxoTransporte fluxoTransporte { get; set; }

        [XmlElement(ElementName = "vagoesDespachados", Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
        public VagaoDespachado[] vagoesDespachados { get; set; }

        [XmlElement(ElementName = "notasFiscaisDespacho", Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
        public refNotaFiscalCliente[] notasFiscaisDespacho { get; set; }

        [XmlElement(ElementName = "dataGeracaoDespacho", Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
        public string dataGeracaoDespacho { get; set; }

        [XmlElement(ElementName = "indicadorProprietarioLona", Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
        public string indicadorProprietarioLona { get; set; }

        [XmlElement(ElementName = "prefixoTremPartida", Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
        public PrefixoTrem prefixoTremPartida { get; set; }

        [XmlElement(ElementName = "NotaExpedicao", Namespace = "http://xmlns.mrs.com.br/iti/tipos/despacho")]
        public NotaExpedicao NotaExpedicao { get; set; }
    }


    /// <remarks/>
    [XmlRoot(ElementName = "EventoVeiculo")]
    public partial class EventoVeiculo
    {

        [XmlElement(ElementName = "prefixoUltimoTrem", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public PrefixoTrem prefixoUltimoTrem { get; set; }

        [XmlElement(ElementName = "veiculos", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public Veiculos veiculos { get; set; }

    }

    [XmlRoot(ElementName = "Evento")]
    public class Evento
    {

        [XmlElement(ElementName = "tipoEvento", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string tipoEvento { get; set; }

        [XmlElement(ElementName = "codigoEvento", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string codigoEvento { get; set; }

        [XmlElement(ElementName = "siglaPatioRef", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string siglaPatioRef { get; set; }

        [XmlElement(ElementName = "siglaTerminalRef", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string siglaTerminalRef { get; set; }

        [XmlElement(ElementName = "dataHoraInicio", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string dataHoraInicio { get; set; }

        [XmlElement(ElementName = "dataHoraFim", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string dataHoraFim { get; set; }

        [XmlElement(ElementName = "dataHoraInicioPrevista", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string dataHoraInicioPrevista { get; set; }

        [XmlElement(ElementName = "dataHoraFimPrevista", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string dataHoraFimPrevista { get; set; }

        [XmlElement(ElementName = "veiculosMensagem", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public VeiculosMensagem veiculosMensagem { get; set; }

        [XmlElement(ElementName = "EventoTrem", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public EventoTrem EventoTrem { get; set; }

        [XmlElement(ElementName = "EventoVeiculo", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public EventoVeiculo EventoVeiculo { get; set; }

        [XmlElement(ElementName = "notasFiscais", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public NotaFiscalCliente[] notasFiscais { get; set; }

        [XmlElement(ElementName = "despachos", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public Despacho[] despachos { get; set; }

        [XmlElement(ElementName = "observacao", Namespace = "http://xmlns.mrs.com.br/iti/tipos/evento")]
        public string observacao { get; set; }
    }


    [XmlRoot(ElementName = "EventoFerroviario")]
    public class EventoFerroviario
    {

        [XmlElement(ElementName = "protocoloEnvio", Order = 1)]
        public int protocoloEnvio { get; set; }

        [XmlElement(ElementName = "dataHoraEnvio", Order = 2)]
        public string dataHoraEnvio { get; set; }

        [XmlElement(ElementName = "CNPJEmissor", Order = 3)]
        public string CNPJEmissor { get; set; }

        [XmlElement(ElementName = "CNPJDestinatario", Order = 4)]
        public string CNPJDestinatario { get; set; }

        [XmlElement(ElementName = "nomeProcessoEnvio", Order = 5)]
        public string nomeProcessoEnvio { get; set; }

        [XmlElement(ElementName = "codigoUsuarioSistema", Order = 6)]
        public string codigoUsuarioSistema { get; set; }

        [XmlElement(ElementName = "dataHora", Order = 7)]
        public string dataHora { get; set; }

        [XmlElement(ElementName = "observacao", Order = 8)]
        public string observacao { get; set; }

        [XmlElement(ElementName = "eventos", Order = 9)]
        public Evento[] eventos { get; set; }

    }

}
