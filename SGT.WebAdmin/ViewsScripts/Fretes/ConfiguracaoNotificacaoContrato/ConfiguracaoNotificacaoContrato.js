/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _configuracao;
var _pesquisaConfiguracao;
var _gridConfiguracao;

var Configuracao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Dias = PropertyEntity({ text: "Dias de Antecedência para Aviso:", val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Emails = PropertyEntity({ text: "E-mails:", val: ko.observable(""), def: "", getType: typesKnockout.string, maxlength: 400 });

    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar" });
}


//*******EVENTOS*******
function loadConfiguracao() {
    _configuracao = new Configuracao();
    KoBindings(_configuracao, "knockoutConfiguracao");

    ObterConfiguracao();

    //HeaderAuditoria("ConfiguracaoTMS", null, null, {
    //    DiasAvisoVencimentoCotratoFrete: "Dias de Antecedência para Aviso",
    //    EmailsAvisoVencimentoCotratoFrete: "E-mails"
    //});
}

function atualizarClick(e, sender) {
    Salvar(_configuracao, "ConfiguracaoNotificacaoContrato/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ObterConfiguracao() {
    // Busca informacoes para edicao
    BuscarPorCodigo(_configuracao, "ConfiguracaoNotificacaoContrato/ObterConfiguracao", function (arg) {
        if (arg.Success) {
            if (arg.Msg != null && arg.Msg != "") {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}