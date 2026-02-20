/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoControleReajusteFretePlanilha.js" />
/// <reference path="AutorizacaoControleReajusteFretePlanilha.js" />

// #region Objetos Globais do Arquivo

var _autorizacao;
var _gridRegras;
var _regraControleReajusteFretePlanilha;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarControleReajusteFretePlanilhaClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
}

var RegraControleReajusteFretePlanilha = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridRegras() {
    var aprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarControleReajusteFretePlanilha
    };

    var rejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: RejeitarControleReajusteFretePlanilha
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [aprovar, rejeitar]
    };

    var ko_controleReajusteFretePlanilha = {
        Codigo: _controleReajusteFretePlanilha.Codigo,
        Usuario: _controleReajusteFretePlanilha.Usuario,
    };

    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoControleReajusteFretePlanilha/RegrasAprovacao", ko_controleReajusteFretePlanilha, menuOpcoes);
}

function loadRegras() {
    _regraControleReajusteFretePlanilha = new RegraControleReajusteFretePlanilha();

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    loadGridRegras();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        var dados = {
            Codigo: _controleReajusteFretePlanilha.Codigo.val()
        };

        executarReST("AutorizacaoControleReajusteFretePlanilha/AprovarMultiplasRegras", dados, function (arg) {
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

                    atualizarGridControleReajuste();
                    atualizarControleReajusteFretePlanilha();
                    AtualizarGridRegras();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        })
    });
}

function cancelarRejeicaoClick() {
    limparRegras();
}

function rejeitarControleReajusteFretePlanilhaClick() {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar a alçada é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a alçada?", function () {
        var dados = {
            Codigo: _regraControleReajusteFretePlanilha.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
        };

        executarReST("AutorizacaoControleReajusteFretePlanilha/Reprovar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridControleReajuste();
                    atualizarControleReajusteFretePlanilha();
                    AtualizarGridRegras();
                    limparRegras();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function atualizarControleReajusteFretePlanilha() {
    BuscarPorCodigo(_controleReajusteFretePlanilha, "AutorizacaoControleReajusteFretePlanilha/BuscarPorCodigo", function (retorno) {
        if (retorno.Success)
            controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoControleReajusteFretePlanilha.AgAprovacao);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function AtualizarGridRegras() {
    _gridRegras.CarregarGrid(function (retorno) {
        var exibirBotaoAprovarTodas = false;

        retorno.data.forEach(function (row) {
            if (row.PodeAprovar)
                exibirBotaoAprovarTodas = true;
        });

        _autorizacao.AprovarTodas.visible(exibirBotaoAprovarTodas);
    });
}

function limparRegras() {
    _autorizacao.Motivo.visible(false);
    _autorizacao.Rejeitar.visible(false);

    LimparCampos(_autorizacao);
}

// #endregion Funções Públicas

// #region Funções Privadas

function AprovarControleReajusteFretePlanilha(registroSelecionado) {
    executarReST("AutorizacaoControleReajusteFretePlanilha/Aprovar", registroSelecionado, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                atualizarGridControleReajuste();
                atualizarControleReajusteFretePlanilha();
                AtualizarGridRegras();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function RejeitarControleReajusteFretePlanilha(registroSelecionado) {
    _regraControleReajusteFretePlanilha.Codigo.val(registroSelecionado.Codigo);

    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

// #endregion Funções Privadas
