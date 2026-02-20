/// <reference path="FluxoCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _recebimentoProduto;
var _CRUDRecebimentoProduto;
var _gridRecebimentoProduto;

var RecebimentoProduto = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Mercadorias = PropertyEntity({ idGrid: guid() });
};

var CRUDRecebimentoProduto = function () {
    this.Finalizar = PropertyEntity({ eventClick: FinalizarFluxoClick, type: types.event, text: "Finalizar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadRecebimentoProduto() {
    _recebimentoProduto = new RecebimentoProduto();
    KoBindings(_recebimentoProduto, "knockoutRecebimentoProduto");

    _CRUDRecebimentoProduto = new CRUDRecebimentoProduto();
    KoBindings(_CRUDRecebimentoProduto, "knockoutCRUDRecebimentoProduto");

    LoadGridRecebimentoProduto();
}

function FinalizarFluxoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar o Fluxo de Compra?", function () {
        executarReST("FluxoCompraOrdemCompra/FinalizarFluxoCompra", { Codigo: _fluxoCompra.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _fluxoCompra.Situacao.val(EnumSituacaoFluxoCompra.Finalizado);
                    controleCamposRecebimentoProduto();

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo de compra finalizado com sucesso!");

                    RecarregarGridPesquisa();
                    SetarEtapaFluxoCompra();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

////*******MÉTODOS*******

function CarregarRecebimentoProduto() {
    _gridRecebimentoProduto.CarregarGrid();
}

function LoadGridRecebimentoProduto() {
    var editarColuna = { permite: true, callback: callbackEditarColunaRecebimentoProduto, atualizarRow: true };

    _gridRecebimentoProduto = new GridView(_recebimentoProduto.Mercadorias.idGrid, "FluxoCompraOrdemCompra/PesquisaRecebimentoProduto", _recebimentoProduto, null, null, null, null, null, null, null, null, editarColuna);
}

function callbackEditarColunaRecebimentoProduto(recebimento) {
    var dados = {
        Codigo: recebimento.Codigo,
        QuantidadeRecebida: recebimento.QuantidadeRecebida
    };
    executarReST("FluxoCompraOrdemCompra/AtualizarRecebimentoProduto", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro alterado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function controleCamposRecebimentoProduto() {
    _recebimentoProduto.Codigo.val(_fluxoCompra.Codigo.val());

    _CRUDRecebimentoProduto.Finalizar.visible(false);
    if (_fluxoCompra.EtapaAtual.val() === EnumEtapaFluxoCompra.RecebimentoProduto && _fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto) {
        _CRUDRecebimentoProduto.Finalizar.visible(true);

        var editarColuna = { permite: true, callback: callbackEditarColunaRecebimentoProduto, atualizarRow: true };
        _gridRecebimentoProduto.SetarEditarColunas(editarColuna);
    } else {
        var editarColuna = { permite: false, callback: null, atualizarRow: false };
        _gridRecebimentoProduto.SetarEditarColunas(editarColuna);
    }
}

function LimparCamposRecebimentoProduto() {
    _CRUDRecebimentoProduto.Finalizar.visible(true);

    LimparCampos(_recebimentoProduto);
}