/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegras;
var _regraContratoFreteTerceiro;


var RegraContratoFreteTerceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}


var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarContratoFreteTerceiroClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
}

//*******EVENTOS*******

function loadRegras() {
    _regraContratoFreteTerceiro = new RegraContratoFreteTerceiro();

    GridRegras();
}

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        var dados = {
            Codigo: _contratoFreteTerceiro.Codigo.val()
        };

        executarReST("AutorizacaoContratoFreteTerceiro/AprovarMultiplasRegras", dados, function (arg) {
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

                    BuscarContratoFretes();
                    AtualizarGridRegras();
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
    limparRejeicao();
}

function rejeitarContratoFreteTerceiroClick(dataRow) {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar a alçada é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a alçada?", function () {
        var dados = {
            Codigo: _regraContratoFreteTerceiro.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
        };

        executarReST("AutorizacaoContratoFreteTerceiro/Rejeitar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    AtualizarGridRegras();
                    _gridContratoFreteTerceiro.CarregarGrid();
                    limparRejeicao();
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
function AtualizarGridRegras() {
    _gridRegras.CarregarGrid(function (arg) {
        var exibeBtn = false;
        arg.data.forEach(function (row) {
            if (row.PodeAprovar)
                exibeBtn = true;
        });

        _autorizacao.AprovarTodas.visible(exibeBtn);
    });
}

function RejeitarContratoFreteTerceiro(dataRow) {
    _regraContratoFreteTerceiro.Codigo.val(dataRow.Codigo);
    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function AprovarContratoFreteTerceiro(dataRow) {
    executarReST("AutorizacaoContratoFreteTerceiro/Aprovar", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                AtualizarGridRegras();
                _gridContratoFreteTerceiro.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparRegras() {
    limparRejeicao();
}

function limparRejeicao() {
    _autorizacao.Motivo.visible(false);
    _autorizacao.Rejeitar.visible(false);

    LimparCampos(_autorizacao);
}

function GridRegras() {
    //-- Grid Regras
    // Menu
    var aprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarContratoFreteTerceiro
    };
    var rejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarContratoFreteTerceiro
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [aprovar, rejeitar]
    };

    var ko_contratoFreteTerceiro = {
        Codigo: _contratoFreteTerceiro.Codigo,
        Usuario: _contratoFreteTerceiro.Usuario,
    };

    // Grid
    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoContratoFreteTerceiro/RegrasAprovacao", ko_contratoFreteTerceiro, menuOpcoes);
}