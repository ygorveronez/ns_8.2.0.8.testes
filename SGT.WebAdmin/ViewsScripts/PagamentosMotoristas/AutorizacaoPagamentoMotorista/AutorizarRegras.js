/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="AutorizacaoPagamentoMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegras;
var _regraPagamentoMotorista;
var _detalhesRegraPagamentoMotorista;

var DetalhesRegraPagamentoMotorista = function () {
    this.MotivoRejeicao = PropertyEntity({ text: "Motivo Rejeição: " });
    this.Observacao = PropertyEntity({ text: "Observação: " });
};

//*******EVENTOS*******

function loadRegras() {
    _regraPagamentoMotorista = new RegraPagamentoMotorista();

    _detalhesRegraPagamentoMotorista = new DetalhesRegraPagamentoMotorista();
    KoBindings(_detalhesRegraPagamentoMotorista, "knockoutDetalhesRegraPagamentoMotorista");

    //-- Grid Regras
    // Menu
    var aprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarPagamentoMotorista
    };
    var rejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarPagamentoMotorista
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
    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoPagamentoMotorista/RegrasAprovacao", _pagamentoMotorista, menuOpcoes);
}

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        var dados = {
            Codigo: _pagamentoMotorista.Codigo.val(),
            Tomador: _autorizacao.Tomador.val()
        };

        executarReST("AutorizacaoPagamentoMotorista/AprovarMultiplasRegras", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de pagamentos foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de pagamento foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    _gridRegras.CarregarGrid();
                    buscarPagamentoMotoristas();

                    if (!string.IsNullOrWhiteSpace(arg.Data.MensagemRetorno))
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.MensagemRetorno);
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

function rejeitarPagamentoMotoristaClick(dataRow) {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar o pagamento é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar o pagamento?", function () {
        var dados = {
            Codigo: _regraPagamentoMotorista.Codigo.val(),
            Motivo: _autorizacao.Motivo.val()
        };

        executarReST("AutorizacaoPagamentoMotorista/Rejeitar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    AtualizarGridRegras();
                    _gridPagamentoMotorista.CarregarGrid();
                    limparRejeicao();

                    if (!string.IsNullOrWhiteSpace(arg.Data.MensagemRetorno))
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.MensagemRetorno);
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
function RejeitarPagamentoMotorista(dataRow) {
    _regraPagamentoMotorista.Codigo.val(dataRow.Codigo);
    _autorizacao.Tomador.visible(false);
    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function DetalhesRegra(dataRow) {
    _detalhesRegraPagamentoMotorista.MotivoRejeicao.val(dataRow.MotivoRejeicao);
    _detalhesRegraPagamentoMotorista.Observacao.val(dataRow.Observacao);

    Global.abrirModal('divModalDetalhesRegraPagamentoMotorista');
}

function AprovarPagamentoMotorista(dataRow) {
    dataRow.Tomador = _autorizacao.Tomador.val();

    executarReST("AutorizacaoPagamentoMotorista/Aprovar", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                AtualizarGridRegras();
                _gridPagamentoMotorista.CarregarGrid();

                if (!string.IsNullOrWhiteSpace(arg.Data.MensagemRetorno))
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.MensagemRetorno);
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