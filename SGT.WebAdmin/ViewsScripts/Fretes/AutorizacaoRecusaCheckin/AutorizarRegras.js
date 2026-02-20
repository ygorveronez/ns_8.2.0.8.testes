/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCheckin.js" />
/// <reference path="AutorizacaoRecusaCheckin.js" />

// #region Objetos Globais do Arquivo

var _autorizacao;
var _gridRegras;
var _regra;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarRecusaCheckinClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAutorizacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
    this.Reprocessar = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: reprocessarRecusaCheckinClick, text: "Reprocessar" });
}

var Regra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridRegras() {
    var opcaoAprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: aprovarRecusaCheckin
    };

    var opcaoRejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: rejeitarRecusaCheckin
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [opcaoAprovar, opcaoRejeitar]
    };

    var knoutRecusaCheckin = {
        Codigo: _recusaCheckin.Codigo,
        Usuario: _recusaCheckin.Usuario,
    };

    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoRecusaCheckin/RegrasAprovacao", knoutRecusaCheckin, menuOpcoes);
}

function loadRegras() {
    _regra = new Regra();

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    loadGridRegras();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        executarReST("AutorizacaoRecusaCheckin/AprovarMultiplasRegras", { Codigo: _recusaCheckin.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridRecusaCheckin();
                    atualizarRecusaCheckin()
                    atualizarGridRegras();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function cancelarAutorizacaoClick() {
    limparRegras();
}

function rejeitarRecusaCheckinClick() {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar a recusa de checkin é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a recusa de checkin?", function () {
        var dados = {
            Codigo: _regra.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
        };

        executarReST("AutorizacaoRecusaCheckin/Reprovar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridRecusaCheckin();
                    atualizarRecusaCheckin()
                    atualizarGridRegras();
                    limparRegras();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function reprocessarRecusaCheckinClick() {
    executarReST("AutorizacaoRecusaCheckin/ReprocessarRecusaCheckin", { Codigo: _recusaCheckin.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegraReprocessada) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "A recusa de checkin foi reprocessada com sucesso.");

                    atualizarGridRecusaCheckin();
                    atualizarRecusaCheckin()
                    atualizarGridRegras();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada.");
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function atualizarGridRegras() {
    _gridRegras.CarregarGrid(function (retorno) {
        var exibirBotaoAprovarTodas = false;
        var exibirBotaoReprocessar = true;

        retorno.data.forEach(function (autorizacao) {
            exibirBotaoReprocessar = false;

            if (autorizacao.PodeAprovar)
                exibirBotaoAprovarTodas = true;
        });

        _autorizacao.AprovarTodas.visible(exibirBotaoAprovarTodas);
        _autorizacao.Reprocessar.visible(exibirBotaoReprocessar);
    });
}

function atualizarRecusaCheckin() {
    BuscarPorCodigo(_recusaCheckin, "AutorizacaoRecusaCheckin/BuscarPorCodigo", function (retorno) {
        if (retorno.Success)
            controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoCheckin.AguardandoAprovacao);
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparRegras() {
    _autorizacao.Motivo.visible(false);
    _autorizacao.Rejeitar.visible(false);

    LimparCampos(_autorizacao);
}

// #endregion Funções Públicas

// #region Funções Privadas

function aprovarRecusaCheckin(registroSelecionado) {
    executarReST("AutorizacaoRecusaCheckin/Aprovar", registroSelecionado, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                atualizarGridRecusaCheckin();
                atualizarRecusaCheckin()
                atualizarGridRegras();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else 
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function rejeitarRecusaCheckin(registroSelecionado) {
    _regra.Codigo.val(registroSelecionado.Codigo);

    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

// #endregion Funções
