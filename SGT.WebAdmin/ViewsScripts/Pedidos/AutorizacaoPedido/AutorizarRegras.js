/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="AutorizacaoPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegras;
var _regraPedido;
var _detalhesRegraPedido;

var DetalhesRegraPedido = function () {
    this.MotivoRejeicao = PropertyEntity({ text: "Motivo Rejeição: " });
    this.Observacao = PropertyEntity({ text: "Observação: " });
}

//*******EVENTOS*******

function loadRegras() {
    _regraPedido = new RegraPedido();

    _detalhesRegraPedido = new DetalhesRegraPedido();
    KoBindings(_detalhesRegraPedido, "knockoutDetalhesRegraPedido");

    //-- Grid Regras
    // Menu
    var aprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarPedido
    };
    var rejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarPedido
    };
    var detalhesAprovacao = {
        descricao: "Detalhes",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.Situacao == "Rejeitada";
        },
        metodo: DetalhesRegra
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [aprovar, rejeitar, detalhesAprovacao]
    };

    // Grid
    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoPedido/RegrasAprovacao", _pedido, menuOpcoes);
}

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        var dados = {
            Codigo: _pedido.Codigo.val(),
            Tomador: _autorizacao.Tomador.val()
        };

        executarReST("AutorizacaoPedido/AprovarMultiplasRegras", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de pedido foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de pedido foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    _gridRegras.CarregarGrid();
                    buscarPedidos();
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

function rejeitarPedidoClick(dataRow) {
    // Valide
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar o pedido é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar o pedido?", function () {
        var dados = {
            Codigo: _regraPedido.Codigo.val(),
            Motivo: _autorizacao.Motivo.val()            
        };

        executarReST("AutorizacaoPedido/Rejeitar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    AtualizarGridRegras();
                    _gridPedido.CarregarGrid();
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
function RejeitarPedido(dataRow) {
    _regraPedido.Codigo.val(dataRow.Codigo);
    _autorizacao.Tomador.visible(false);
    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function DetalhesRegra(dataRow) {
    _detalhesRegraPedido.MotivoRejeicao.val(dataRow.MotivoRejeicao);
    _detalhesRegraPedido.Observacao.val(dataRow.Observacao);

    Global.abrirModal('divModalDetalhesRegraPedido');
}

function AprovarPedido(dataRow) {
    dataRow.Tomador = _autorizacao.Tomador.val();

    executarReST("AutorizacaoPedido/Aprovar", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                AtualizarGridRegras();
                _gridPedido.CarregarGrid();

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
    _autorizacao.Tomador.visible(true);
    _autorizacao.Motivo.visible(false);    
    _autorizacao.Rejeitar.visible(false);

    LimparCampos(_autorizacao);
}