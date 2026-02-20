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
/// <reference path="../../Enumeradores/EnumSituacaoEtapaSaidaFornecedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaEtapaSaidaFornecedor;
var _gridEtapaSaidaFornecedor;

var EtapaSaidaFornecedor = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EtapaLiberada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    // Resumo
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.DataInformada = PropertyEntity({ text: "*Data Informada: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: true });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: true, maxlength: 2000, enable: ko.observable(true), required: false });

    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarEtapaSaidaFornecedorClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function ExibirDetalhesSaidaFornecedorFluxoColetaEntrega(e) {
    _fluxoAtual = e;
    var data = { CodigoColetaEntrega: e.Codigo.val() }

    executarReST("EtapaSaidaFornecedor/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_etapaEtapaSaidaFornecedor, arg);                
                Global.abrirModal("divModalDetalhesEtapaSaidaFornecedor");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);


}

function loadEtapaSaidaFornecedor() {
    _etapaEtapaSaidaFornecedor = new EtapaSaidaFornecedor();
    KoBindings(_etapaEtapaSaidaFornecedor, "knockoutEtapaSaidaFornecedor");
}

function atualizarEtapaSaidaFornecedorClick(e) {
    Salvar(_etapaEtapaSaidaFornecedor, "EtapaSaidaFornecedor/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposEtapaSaidaFornecedor();
                atualizarFluxoColetaEntrega();
                Global.fecharModal('divModalDetalhesEtapaSaidaFornecedor');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparCamposEtapaSaidaFornecedor() {
    LimparCampos(_etapaEtapaSaidaFornecedor);
}