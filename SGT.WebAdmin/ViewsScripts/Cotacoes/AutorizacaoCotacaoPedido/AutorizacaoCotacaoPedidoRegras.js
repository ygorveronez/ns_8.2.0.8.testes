/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="AutorizacaoCotacaoPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegras;
var _regraCotacaoPedido;
var _detalhesRegraCotacaoPedido;
var _modalDetalhesRegraCotacaoPedido;

var DetalhesRegraCotacaoPedido = function () {
    this.MotivoRejeicao = PropertyEntity({ text: "Motivo Rejeição: " });
    this.Observacao = PropertyEntity({ text: "Observação: " });
}

//*******EVENTOS*******

function loadRegras() {
    _regraCotacaoPedido = new RegraCotacaoPedido();

    _detalhesRegraCotacaoPedido = new DetalhesRegraCotacaoPedido();
    KoBindings(_detalhesRegraCotacaoPedido, "knockoutDetalhesRegraCotacaoPedido");

    //-- Grid Regras
    // Menu
    var aprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarCotacaoPedido
    };
    var rejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarCotacaoPedido
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
    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoCotacaoPedido/RegrasAprovacao", _pedido, menuOpcoes);
    _modalDetalhesRegraCotacaoPedido = new bootstrap.Modal(document.getElementById("divModalDetalhesRegraCotacaoPedido"), { backdrop: true, keyboard: true });
}

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        var dados = {
            Codigo: _pedido.Codigo.val(),
            Tomador: _autorizacao.Tomador.val()
        };

        executarReST("AutorizacaoCotacaoPedido/AprovarMultiplasRegras", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de Cotações de Pedidos foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de Cotação Pedido foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    _gridRegras.CarregarGrid();
                    buscarCotacaoPedidos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        })
    });
}

function cancelarRejeicaoClick() {
    limparRejeicao();
}

function rejeitarCotacaoPedidoClick(dataRow) {
    // Valide
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar a cotação pedido é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a cotação pedido?", function () {
        var dados = {
            Codigo: _regraCotacaoPedido.Codigo.val(),
            Motivo: _autorizacao.Motivo.val()
        };

        executarReST("AutorizacaoCotacaoPedido/Rejeitar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    AtualizarGridRegras();
                    _gridCotacaoPedido.CarregarGrid();
                    limparRejeicao();

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
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
function RejeitarCotacaoPedido(dataRow) {
    _regraCotacaoPedido.Codigo.val(dataRow.Codigo);
    _autorizacao.Tomador.visible(false);
    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function DetalhesRegra(dataRow) {
    _detalhesRegraCotacaoPedido.MotivoRejeicao.val(dataRow.MotivoRejeicao);
    _detalhesRegraCotacaoPedido.Observacao.val(dataRow.Observacao);

    _modalDetalhesRegraCotacaoPedido.show();
}

function AprovarCotacaoPedido(dataRow) {
    dataRow.Tomador = _autorizacao.Tomador.val();

    executarReST("AutorizacaoCotacaoPedido/Aprovar", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                AtualizarGridRegras();
                _gridCotacaoPedido.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
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