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
/// <reference path="../../Enumeradores/EnumSituacaoEtapaVeiculoAlocado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaEtapaVeiculoAlocado;
var _gridEtapaVeiculoAlocado;

var EtapaVeiculoAlocado = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EtapaLiberada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    // Resumo
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.DataInformada = PropertyEntity({ text: "*Data Informada: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: true });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "CD onde está o veículo: ", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo:"), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-6") });
    this.Tracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cavalo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: true, maxlength: 2000, enable: ko.observable(true), required: false });
    
    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarEtapaVeiculoAlocadoClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function ExibirDetalhesVeiculoAlocadoFluxoColetaEntrega(e) {
    _fluxoAtual = e;
    var data = { CodigoColetaEntrega: e.Codigo.val() }

    executarReST("EtapaVeiculoAlocado/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_etapaEtapaVeiculoAlocado, arg);

                if (arg.Data.ExigePlacaTracao) {
                    _etapaEtapaVeiculoAlocado.Tracao.visible(true);
                    _etapaEtapaVeiculoAlocado.Tracao.required = true;
                    _etapaEtapaVeiculoAlocado.Veiculo.cssClass("col col-xs-12 col-sm-3 col-md-3")
                    _etapaEtapaVeiculoAlocado.Veiculo.text("*Reboque:");
                } else {
                    _etapaEtapaVeiculoAlocado.Tracao.visible(false);
                    _etapaEtapaVeiculoAlocado.Tracao.required = false;
                    _etapaEtapaVeiculoAlocado.Veiculo.cssClass("col col-xs-12 col-sm-6 col-md-6")
                    _etapaEtapaVeiculoAlocado.Veiculo.text("*Veículo:");
                }

                Global.abrirModal("divModalDetalhesEtapaVeiculoAlocado");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function loadEtapaVeiculoAlocado() {
    _etapaEtapaVeiculoAlocado = new EtapaVeiculoAlocado();
    KoBindings(_etapaEtapaVeiculoAlocado, "knockoutEtapaVeiculoAlocado");

    new BuscarFilial(_etapaEtapaVeiculoAlocado.Filial);
    new BuscarMotoristas(_etapaEtapaVeiculoAlocado.Motorista, null, _etapaEtapaVeiculoAlocado.Transportador);
    new BuscarVeiculos(_etapaEtapaVeiculoAlocado.Veiculo, null, _etapaEtapaVeiculoAlocado.Transportador, null, null, true, null, null, true, null, null, "1");
    new BuscarVeiculos(_etapaEtapaVeiculoAlocado.Tracao, null, _etapaEtapaVeiculoAlocado.Transportador, null, null, true, null, null, true, null, null, "0");
    new BuscarTransportadores(_etapaEtapaVeiculoAlocado.Transportador);
}

function atualizarEtapaVeiculoAlocadoClick(e) {
    Salvar(_etapaEtapaVeiculoAlocado, "EtapaVeiculoAlocado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposEtapaVeiculoAlocado();
                atualizarFluxoColetaEntrega();
                Global.fecharModal('divModalDetalhesEtapaVeiculoAlocado');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparCamposEtapaVeiculoAlocado() {
    LimparCampos(_etapaEtapaVeiculoAlocado);
}
