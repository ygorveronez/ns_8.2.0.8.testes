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
/// <reference path="../../Enumeradores/EnumSituacaoEtapaChegadaCD.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaEtapaChegadaCD;
var _gridEtapaChegadaCD;

var _localVeiculoFluxoColetaEntrega = [
    { text: "Não informado", value: EnumLocalVeiculoFluxoColetaEntrega.NaoInformado },
    { text: "CD-1", value: EnumLocalVeiculoFluxoColetaEntrega.CD1 },
    { text: "CD-2", value: EnumLocalVeiculoFluxoColetaEntrega.CD2 },
    { text: "Serbom", value: EnumLocalVeiculoFluxoColetaEntrega.Serbom },
    { text: "Transportador", value: EnumLocalVeiculoFluxoColetaEntrega.Transportador }
];

var EtapaChegadaCD = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EtapaLiberada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    // Resumo
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.DataInformada = PropertyEntity({ text: "*Data Informada: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: true });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "CD onde está o veículo: ", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: true, maxlength: 2000, enable: ko.observable(true), required: false });

    this.LocalVeiculoFluxoColetaEntrega = PropertyEntity({ val: ko.observable(EnumLocalVeiculoFluxoColetaEntrega.NaoInformado), options: _localVeiculoFluxoColetaEntrega, text: "CD onde está o veículo: ", def: EnumLocalVeiculoFluxoColetaEntrega.NaoInformado, visible: ko.observable(true) });

    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarEtapaChegadaCDClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function ExibirDetalhesChegadaCDFluxoColetaEntrega(e) {
    _fluxoAtual = e;
    var data = { CodigoColetaEntrega: e.Codigo.val() }

    executarReST("EtapaChegadaCD/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_etapaEtapaChegadaCD, arg);                
                Global.abrirModal("divModalDetalhesEtapaChegadaCD");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);


}

function loadEtapaChegadaCD() {
    _etapaEtapaChegadaCD = new EtapaChegadaCD();
    KoBindings(_etapaEtapaChegadaCD, "knockoutEtapaChegadaCD");

    new BuscarFilial(_etapaEtapaChegadaCD.Filial);
}

function atualizarEtapaChegadaCDClick(e) {
    Salvar(_etapaEtapaChegadaCD, "EtapaChegadaCD/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposEtapaChegadaCD();
                atualizarFluxoColetaEntrega();
                Global.fecharModal('divModalDetalhesEtapaChegadaCD');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparCamposEtapaChegadaCD() {
    LimparCampos(_etapaEtapaChegadaCD);
}