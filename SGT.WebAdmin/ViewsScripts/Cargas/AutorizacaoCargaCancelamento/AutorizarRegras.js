/// <reference path="AutorizacaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _autorizacao;
var _gridRegras;
var _regra;

/*
 * Declaração das Classes
 */

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarCargaCancelamentoClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAutorizacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
    this.Reprocessar = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: reprocessarCargaCancelamentoClick, text: "Reprocessar" });
}

var Regra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRegras() {
    var opcaoAprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: AprovarCargaCancelamento
    };

    var opcaoRejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: rejeitarCargaCancelamento
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [opcaoAprovar, opcaoRejeitar]
    };

    var knoutCarga = {
        Codigo: _cargaCancelamento.Codigo,
        Usuario: _cargaCancelamento.Usuario,
    };

    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "AutorizacaoCargaCancelamento/RegrasAprovacao", knoutCarga, menuOpcoes);
}

function loadRegras() {
    _regra = new Regra();

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    loadGridRegras();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        executarReST("AutorizacaoCargaCancelamento/AprovarMultiplasRegras", { Codigo: _cargaCancelamento.Codigo.val() }, function (retorno) {
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

                    atualizarGridCargaCancelamento();
                    atualizarCargaCancelamento()
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

function rejeitarCargaCancelamentoClick() {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar o cancelamento da carga é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar o cancelamento da carga?", function () {
        var dados = {
            Codigo: _regra.Codigo.val(),
            Motivo: _autorizacao.Motivo.val(),
        };

        executarReST("AutorizacaoCargaCancelamento/Reprovar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridCargaCancelamento();
                    atualizarCargaCancelamento()
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

function reprocessarCargaCancelamentoClick() {
    executarReST("AutorizacaoCargaCancelamento/ReprocessarCarga", { Codigo: _cargaCancelamento.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegraReprocessada) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O cancelamento da carga foi reprocessado com sucesso.");

                    atualizarGridCargaCancelamento();
                    atualizarCargaCancelamento()
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

/*
 * Declaração das Funções
 */

function AprovarCargaCancelamento(registroSelecionado) {
    executarReST("AutorizacaoCargaCancelamento/Aprovar", registroSelecionado, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");

                atualizarGridCargaCancelamento();
                atualizarCargaCancelamento()
                atualizarGridRegras();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else 
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

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

function atualizarCargaCancelamento() {
    BuscarPorCodigo(_cargaCancelamento, "AutorizacaoCargaCancelamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success)
            controlarExibicaoAbaDelegar(retorno.Data.SituacaoCargaCancelamentoSolicitacao === EnumSituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao);
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function rejeitarCargaCancelamento(registroSelecionado) {
    _regra.Codigo.val(registroSelecionado.Codigo);

    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

function limparRegras() {
    _autorizacao.Motivo.visible(false);
    _autorizacao.Rejeitar.visible(false);

    LimparCampos(_autorizacao);
}
