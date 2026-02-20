/// <reference path="../../Enumeradores/EnumTipoEmissorDocumento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamentoCotacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

let _configuracaoEmissorDocumento;
let _CRUDConfiguracaoEmissorDocumento;

/*
 * Declaração das Classes
 */

let ConfiguracaoEmissorDocumento = function () {
    this.TipoEmissorDocumentoCTe = PropertyEntity({ text: "Tipo de Emissor CTe: ", val: ko.observable(EnumTipoEmissorDocumento.Integrador), options: EnumTipoEmissorDocumento.obterOpcoes(), def: EnumTipoEmissorDocumento.Integrador });
    this.TipoEmissorDocumentoMDFe = PropertyEntity({ text: "Tipo de Emissor MDFe: ", val: ko.observable(EnumTipoEmissorDocumento.Integrador), options: EnumTipoEmissorDocumento.obterOpcoes(), def: EnumTipoEmissorDocumento.Integrador, visible: true });
    this.GrupoResponsavelTecnico = PropertyEntity({ visible: ko.observable(false) });
    this.ResponsavelTecnicoCNPJ = PropertyEntity({ text: "CNPJ:", val: ko.observable(""), maxlength: 14 });
    this.ResponsavelTecnicoNomeContato = PropertyEntity({ text: "Nome Contato:", val: ko.observable(""), maxlength: 60 });
    this.ResponsavelTecnicoEmail = PropertyEntity({ text: "E-mail:", val: ko.observable(""), maxlength: 60 });
    this.ResponsavelTecnicoTelefone = PropertyEntity({ text: "Telefone:", val: ko.observable(""), maxlength: 12 });
    this.NSTechUrlAPICte = PropertyEntity({ text: "Url API CT-e:", val: ko.observable(""), maxlength: 300 });
    this.NSTechUrlAPIMDFe = PropertyEntity({ text: "Url API MDF-e:", val: ko.observable(""), maxlength: 300, visible: true });
    this.NSTechUrlAPIWebHook = PropertyEntity({ text: "Url API WebHook:", val: ko.observable(""), maxlength: 300 });
    this.NSTechUrlAPICertificado = PropertyEntity({ text: "Url API Certificado:", val: ko.observable(""), maxlength: 300 });
    this.NSTechUrlAPILogo = PropertyEntity({ text: "Url API Logo:", val: ko.observable(""), maxlength: 300 });
    this.NSTechTokenAPIKey = PropertyEntity({ text: "Token API Key:", val: ko.observable(""), maxlength: 300 });
    this.NSTechUrlWebhook = PropertyEntity({ text: "Url Webhook:", val: ko.observable(""), maxlength: 300 });
    this.NSTechTokenWebhook = PropertyEntity({ text: "Token Webhook:", val: ko.observable(""), enable: ko.observable(false), maxlength: 300 });
    this.NSTechSubscribeId = PropertyEntity({ text: "Subscribe Id:", val: ko.observable(""), enable: ko.observable(false), maxlength: 300 });

    this.TipoEmissorDocumentoCTe.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoEmissorDocumento.NSTech || _configuracaoEmissorDocumento.TipoEmissorDocumentoMDFe.val() == EnumTipoEmissorDocumento.NSTech) {
            $("#liNSTech").show();
            _configuracaoEmissorDocumento.GrupoResponsavelTecnico.visible(true);
        }
        else {
            $("#liNSTech").hide();
            _configuracaoEmissorDocumento.GrupoResponsavelTecnico.visible(false);
        }
    });

    this.TipoEmissorDocumentoMDFe.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoEmissorDocumento.NSTech || _configuracaoEmissorDocumento.TipoEmissorDocumentoCTe.val() == EnumTipoEmissorDocumento.NSTech) {
            $("#liNSTech").show();
            _configuracaoEmissorDocumento.GrupoResponsavelTecnico.visible(true);
        }
        else {
            $("#liNSTech").hide();
            _configuracaoEmissorDocumento.GrupoResponsavelTecnico.visible(false);
        }
    });
  
};

let CRUDConfiguracaoEmissorDocumento = function () {
    this.Salvar = PropertyEntity({ eventClick: atualizarConfiguracaoEmissorDocumentoClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });
    this.EnviarCertificadosDigitaisValidos = PropertyEntity({ eventClick: enviarCertificadosDigitaisValidosClick, type: types.event, text: "Enviar Certificados Digitais Válidos", icon: "fa fa-upload", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadConfiguracaoEmissorDocumento() {
    _configuracaoEmissorDocumento = new ConfiguracaoEmissorDocumento();
    KoBindings(_configuracaoEmissorDocumento, "knockoutConfiguracaoEmissorDocumento");

    _CRUDConfiguracaoEmissorDocumento = new CRUDConfiguracaoEmissorDocumento();
    KoBindings(_CRUDConfiguracaoEmissorDocumento, "knockoutCRUDConfiguracaoEmissorDocumento");

    HeaderAuditoria("ConfiguracaoIntegracaoEmissorDocumento", _configuracaoEmissorDocumento);

    buscarConfiguracaoEmissorDocumento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarConfiguracaoEmissorDocumentoClick() {
    if (ValidarCamposObrigatorios(_configuracaoEmissorDocumento)) {
        requestAtualizarConfiguracaoEmissorDocumento(true);
    }
}

function enviarCertificadosDigitaisValidosClick() {
    requestEnviarCertificadosDigitaisValidos();
}

/*
* Declaração das Funções
*/

function buscarConfiguracaoEmissorDocumento() {
    executarReST("ConfiguracaoEmissorDocumento/ObterConfiguracao", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                CarregarDadosCamposEmissorDocumento(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function requestAtualizarConfiguracaoEmissorDocumento(mostrarConfirmacao) {
    let requestObj = obterConfiguracaoSalvarConfiguracaoEmissorDocumento();
    requestObj["ExibirConfirmacao"] = mostrarConfirmacao;
    executarReST("ConfiguracaoEmissorDocumento/Atualizar", requestObj, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.msg) {
                    exibirConfirmacao("Confirmação", retorno.Data.msg, () => requestAtualizar(false));
                }
                else
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}
function requestEnviarCertificadosDigitaisValidos() {
    executarReST("ConfiguracaoEmissorDocumento/EnviarCertificadosDigitaisValido", {}, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function obterConfiguracaoSalvarConfiguracaoEmissorDocumento() {
    var configuracaoEmissorDocumento = RetornarObjetoPesquisa(_configuracaoEmissorDocumento);

    return configuracaoEmissorDocumento;
}

function CarregarDadosCamposEmissorDocumento(data) {
    _configuracaoEmissorDocumento.TipoEmissorDocumentoCTe.val(data.TipoEmissorDocumentoCTe);
    _configuracaoEmissorDocumento.TipoEmissorDocumentoMDFe.val(data.TipoEmissorDocumentoMDFe);
    _configuracaoEmissorDocumento.ResponsavelTecnicoCNPJ.val(data.ResponsavelTecnicoCNPJ);
    _configuracaoEmissorDocumento.ResponsavelTecnicoNomeContato.val(data.ResponsavelTecnicoNomeContato);
    _configuracaoEmissorDocumento.ResponsavelTecnicoEmail.val(data.ResponsavelTecnicoEmail);
    _configuracaoEmissorDocumento.ResponsavelTecnicoTelefone.val(data.ResponsavelTecnicoTelefone);
    _configuracaoEmissorDocumento.NSTechUrlAPICte.val(data.NSTechUrlAPICte);
    _configuracaoEmissorDocumento.NSTechUrlAPIMDFe.val(data.NSTechUrlAPIMDFe);
    _configuracaoEmissorDocumento.NSTechUrlAPIWebHook.val(data.NSTechUrlAPIWebHook);
    _configuracaoEmissorDocumento.NSTechUrlAPICertificado.val(data.NSTechUrlAPICertificado);
    _configuracaoEmissorDocumento.NSTechUrlAPILogo.val(data.NSTechUrlAPILogo);
    _configuracaoEmissorDocumento.NSTechTokenAPIKey.val(data.NSTechTokenAPIKey);
    _configuracaoEmissorDocumento.NSTechUrlWebhook.val(data.NSTechUrlWebhook);
    _configuracaoEmissorDocumento.NSTechTokenWebhook.val(data.NSTechTokenWebhook);
    _configuracaoEmissorDocumento.NSTechSubscribeId.val(data.NSTechSubscribeId);
}