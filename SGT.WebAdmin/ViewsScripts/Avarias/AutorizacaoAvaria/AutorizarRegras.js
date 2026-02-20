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
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeMotivoAvaria.js" />
/// <reference path="Autorizacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegras;
var _regraAvaria;

//*******EVENTOS*******

function loadRegras() {
    _regraAvaria = new RegraAvaria();

    // Pesquisa de justificativa
    new BuscarMotivoAvaria(_autorizacao.Justificativa, EnumFinalidadeMotivoAvaria.AutorizacaoAvaria);

    GridRegras();
}

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        var dados = {
            Codigo: _solicitacaoAvaria.Codigo.val()
        };

        executarReST("AutorizacaoAvaria/AprovarMultiplasRegras", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de solicitações foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de solicitação foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    buscarAvarias();
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

function rejeitarAvariaClick(dataRow) {
    // Valide
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar a avaria é necessário informar ao mínimo 20 caracteres no motivo.");

    if (_autorizacao.Justificativa.codEntity() == 0)
        return exibirMensagem(tipoMensagem.aviso, "Justificativa", "O campo justificativa é obrigatório.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a avaria?", function () {
        var dados = {
            Codigo: _regraAvaria.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
            Justificativa: _autorizacao.Justificativa.codEntity()
        };

        executarReST("AutorizacaoAvaria/Rejeitar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    AtualizarGridRegras();
                    _gridAvaria.CarregarGrid();
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
        // Verifica se tem alugma pendente, então mostra o btn de aprovar multiplas
        var exibeBtn = false;
        arg.data.forEach(function (row) {
            if (row.PodeAprovar)
                exibeBtn = true;
        });

        _autorizacao.AprovarTodas.visible(exibeBtn);
    });
}

function RejeitarAvaria(dataRow) {
    _regraAvaria.Codigo.val(dataRow.Codigo);
    _autorizacao.Motivo.visible(true);
    _autorizacao.Justificativa.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function AprovarAvaria(dataRow) {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar a avaria?", function () {
        executarReST("AutorizacaoAvaria/Aprovar", dataRow, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                    AtualizarGridRegras();
                    _gridAvaria.CarregarGrid();

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function limparRegras() {
    limparRejeicao();
}

function limparRejeicao() {
    _autorizacao.Motivo.visible(false);
    _autorizacao.Justificativa.visible(false);
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
        metodo: AprovarAvaria
    };
    var rejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarAvaria
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [aprovar, rejeitar]
    };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        menuOpcoes = null;
    }

    // Grid
    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoAvaria/RegrasAprovacao", _solicitacaoAvaria, menuOpcoes);
}