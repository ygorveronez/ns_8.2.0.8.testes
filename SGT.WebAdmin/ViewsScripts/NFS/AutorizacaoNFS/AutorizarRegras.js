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
/// <reference path="../../Consultas/MotivoRejeicaoLancamentoNFS.js" />
/// <reference path="Autorizacao.js" />

var _gridRegras;
var _regraLancamento;


//*******MAPEAMENTO KNOUCKOUT*******
var RegraLancamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Justificativa = PropertyEntity({ text: "*Justificativa:", type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarLancamentoClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
}

//*******EVENTOS*******

function loadRegras() {
    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _regraLancamento = new RegraLancamento();

    // Pesquisa de justificativa
    new BuscarMotivoRejeicaoLancamentoNFS(_autorizacao.Justificativa);

    GridRegras();
}

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        var dados = {
            Codigo: _lancamentoNFS.Codigo.val()
        };

        executarReST("AutorizacaoNFS/AprovarMultiplasRegras", dados, function (arg) {
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

                    BuscarLancamentosNFS();
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

function rejeitarLancamentoClick(dataRow) {
    // Valide
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar a solicitação é necessário informar ao mínimo 20 caracteres no motivo.");

    if (_autorizacao.Justificativa.codEntity() == 0)
        return exibirMensagem(tipoMensagem.aviso, "Justificativa", "O campo justificativa é obrigatório.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a solicitação?", function () {
        var dados = {
            Codigo: _regraLancamento.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
            Justificativa: _autorizacao.Justificativa.codEntity()
        };

        executarReST("AutorizacaoNFS/Rejeitar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    AtualizarGridRegras();
                    _gridNFS.CarregarGrid();
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

function RejeitarLancamento(dataRow) {
    _regraLancamento.Codigo.val(dataRow.Codigo);
    _autorizacao.Motivo.visible(true);
    _autorizacao.Justificativa.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function AprovarLancamento(dataRow) {
    executarReST("AutorizacaoNFS/Aprovar", dataRow, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                AtualizarGridRegras();
                _gridNFS.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function LimparRegras() {
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
        metodo: AprovarLancamento
    };
    var rejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarLancamento
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
    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoNFS/RegrasAprovacao", _lancamentoNFS, menuOpcoes);
}