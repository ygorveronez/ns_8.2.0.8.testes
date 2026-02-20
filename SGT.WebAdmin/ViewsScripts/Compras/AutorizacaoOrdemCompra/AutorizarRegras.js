/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="AutorizacaoOrdemCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasAutorizacaoOrdemCompra;
var _regraAutorizacaoOrdemCompra;
var _autorizacaoAutorizacaoOrdemCompra;

var RegraOrdemCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var AutorizacaoAutorizacaoOrdemCompra = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarOrdemClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasAutorizacaoOrdemCompraClick, text: "Aprovar Regras" });
};

//*******EVENTOS*******

function loadRegrasAutorizacaoOrdemCompra() {
    _regraAutorizacaoOrdemCompra = new RegraOrdemCompra();

    _autorizacaoAutorizacaoOrdemCompra = new AutorizacaoAutorizacaoOrdemCompra();
    KoBindings(_autorizacaoAutorizacaoOrdemCompra, "knockoutAutorizacaoAutorizacaoOrdemCompra");

    GridRegrasAutorizacaoOrdemCompra();
}

function aprovarMultiplasRegrasAutorizacaoOrdemCompraClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        var dados = {
            Codigo: _autorizacaoOrdemCompra.Codigo.val()
        };

        executarReST("AutorizacaoOrdemCompra/AprovarMultiplasRegras", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    RecarregarGridOrdem();
                    AtualizarGridRegrasAutorizacaoOrdemCompra();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function cancelarRejeicaoClick() {
    limparRejeicaoAutorizacaoOrdemCompra();
}

function rejeitarOrdemClick(dataRow) {
    if (_autorizacaoAutorizacaoOrdemCompra.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar a ordem de compra é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a ordem de compra?", function () {
        var dados = {
            Codigo: _regraAutorizacaoOrdemCompra.Codigo.val(),
            Motivo: _autorizacaoAutorizacaoOrdemCompra.Motivo.val(),
        };

        executarReST("AutorizacaoOrdemCompra/Rejeitar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    AtualizarGridRegrasAutorizacaoOrdemCompra();
                    RecarregarGridOrdem();
                    limparRejeicaoAutorizacaoOrdemCompra();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function AtualizarGridRegrasAutorizacaoOrdemCompra() {
    
    _gridRegrasAutorizacaoOrdemCompra.CarregarGrid(function (arg) {
        var exibeBtn = false;
        arg.data.forEach(function (row) {
            if (row.PodeAprovar)
                exibeBtn = true;
        });

        _autorizacaoAutorizacaoOrdemCompra.AprovarTodas.visible(exibeBtn);
    });
}

function RejeitarOrdemCompra(dataRow) {
    _regraAutorizacaoOrdemCompra.Codigo.val(dataRow.Codigo);
    _autorizacaoAutorizacaoOrdemCompra.Motivo.visible(true);
    _autorizacaoAutorizacaoOrdemCompra.Rejeitar.visible(true);
}

function AprovarOrdemCompra(dataRow) {
    executarReST("AutorizacaoOrdemCompra/Aprovar", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                AtualizarGridRegrasAutorizacaoOrdemCompra();
                RecarregarGridOrdem();
                Global.fecharModal("divModalOrdemCompra");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparRegrasAutorizacaoOrdemCompra() {
    limparRejeicaoAutorizacaoOrdemCompra();
}

function limparRejeicaoAutorizacaoOrdemCompra() {
    _autorizacaoAutorizacaoOrdemCompra.Motivo.visible(false);
    _autorizacaoAutorizacaoOrdemCompra.Rejeitar.visible(false);

    LimparCampos(_autorizacaoAutorizacaoOrdemCompra);
}

function GridRegrasAutorizacaoOrdemCompra() {
    var aprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarOrdemCompra
    };
    var rejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarOrdemCompra
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 10,
        opcoes: [aprovar, rejeitar]
    };

    var ko_autorizacaoOrdemCompra = {
        Codigo: _autorizacaoOrdemCompra.Codigo,
        Usuario: _autorizacaoOrdemCompra.Usuario,
    };

    _gridRegrasAutorizacaoOrdemCompra = new GridView(_autorizacaoAutorizacaoOrdemCompra.Regras.idGrid, "AutorizacaoOrdemCompra/RegrasAprovacao", ko_autorizacaoOrdemCompra, menuOpcoes);
}