/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />

var _containerConfiguracaoAcompanhamentoEntrega;


// ******* ENTIDADES KNOCKOUT ****//
var ConfiguracaoAcompanhamentoEntrega = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SaidaEmTempo = PropertyEntity({ text: "*Está Adiantado ", required: true, getType: typesKnockout.time, type: types.time });
    this.SaidaAtraso1 = PropertyEntity({ text: "*No horário ", required: true, getType: typesKnockout.time, type: types.time });
    this.SaidaAtraso2 = PropertyEntity({ text: "*Um Pouco Atrasado ", required: true, getType: typesKnockout.time, type: types.time });
    this.SaidaAtraso3 = PropertyEntity({ text: "*Atrasado ", required: true, getType: typesKnockout.time, type: types.time });
    this.EmtransitoEmTempo = PropertyEntity({ text: "*Está Adiantado ", required: true, getType: typesKnockout.time, type: types.time });
    this.EmTrasitoAtraso1 = PropertyEntity({ text: "*No horário ", required: true, getType: typesKnockout.time, type: types.time });
    this.EmTrasitoAtraso2 = PropertyEntity({ text: "*Um Pouco Atrasado ", required: true, getType: typesKnockout.time, type: types.time });
    this.EmTrasitoAtraso3 = PropertyEntity({ text: "*Atrasado ", required: true, getType: typesKnockout.time, type: types.time });
    this.DestinoEmTempo = PropertyEntity({ text: "*Está Adiantado ", required: true, getType: typesKnockout.time, type: types.time });
    this.DestinoAtraso1 = PropertyEntity({ text: "*No horário ", required: true, getType: typesKnockout.time, type: types.time });
    this.DestinoAtraso2 = PropertyEntity({ text: "*Um Pouco Atrasado ", required: true, getType: typesKnockout.time, type: types.time });
    this.DestinoAtraso3 = PropertyEntity({ text: "*Atrasado ", required: true, getType: typesKnockout.time, type: types.time });


    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: ko.observable("Salvar"), visible: ko.observable(true), enable: ko.observable(true) });
}


// ***** EVENTOS ***** 

function loadconfiguracaoacompanhamentoentrega() {
    _containerConfiguracaControleEntrega = new ConfiguracaoAcompanhamentoEntrega();
    KoBindings(_containerConfiguracaControleEntrega, "knoutConfiguracaoAcompanhamentoEntrega");

    BuscarDadosPrincipais();
}

function SalvarClick(e, sender) {
    if (!ValidarCamposObrigatorios(_containerConfiguracaControleEntrega)) {
        return;
    }

    Salvar(_containerConfiguracaControleEntrega, "AcompanhamentoEntregaTempoConfiguracao/AtualizarConfiguracao", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com Sucesso");
                BuscarDadosPrincipais();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function BuscarDadosPrincipais() {
    executarReST("AcompanhamentoEntregaTempoConfiguracao/ObterDadosConfiguracao", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data !== null) {
                console.log(arg);
                PreencherObjetoKnout(_containerConfiguracaControleEntrega, arg);
            }
        }
    });
}


