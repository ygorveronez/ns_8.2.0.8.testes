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
/// <reference path="../../Enumeradores/EnumSituacaoEtapaAgSenha.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaEtapaAgSenha;
var _gridEtapaAgSenha;

var EtapaAgSenha = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EtapaLiberada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    // Resumo
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.DataColeta = PropertyEntity({ text: "Data Coleta: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.DataInformada = PropertyEntity({ text: "Data Informada: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.Senha = PropertyEntity({ text: "Senha: ", enable: ko.observable(true), required: false });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 2000, enable: ko.observable(true), required: false });

    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarEtapaAgSenhaClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function ExibirDetalhesAgSenhaFluxoColetaEntrega(e) {
    _fluxoAtual = e;
    var data = { CodigoColetaEntrega: e.Codigo.val() }

    executarReST("EtapaAgSenha/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_etapaEtapaAgSenha, arg);

                _etapaEtapaAgSenha.DataColeta.enable(arg.Data.DataColetaLiberada);
                                
                Global.abrirModal("divModalDetalhesEtapaAgSenha");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);


}

function loadEtapaAgSenha() {
    _etapaEtapaAgSenha = new EtapaAgSenha();
    KoBindings(_etapaEtapaAgSenha, "knockoutEtapaAgSenha");
}

function atualizarEtapaAgSenhaClick(e) {
    Salvar(_etapaEtapaAgSenha, "EtapaAgSenha/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposEtapaAgSenha();
                atualizarFluxoColetaEntrega();
                Global.fecharModal('divModalDetalhesEtapaAgSenha');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparCamposEtapaAgSenha() {
    LimparCampos(_etapaEtapaAgSenha);
}