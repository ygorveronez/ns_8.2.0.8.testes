/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="DetalhesGestaoDocumentoAprovacao.js" />
/// <reference path="GestaoDocumento.js" />

// #region Objetos Globais do Arquivo

var _autorizacao;
var _gridRegras;
var _regra;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAprovacaoGestaoDocumentoClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAutorizacaoGestaoDocumentoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.DesfazerAprovacao = PropertyEntity({ eventClick: desfazerAprovacaoGestaoDocumentoClick, type: types.event, text: "Defazer Aprovação", idGrid: guid(), visible: ko.observable(false) });
    this.DesfazerRejeicao = PropertyEntity({ eventClick: desfazerRejeicaoGestaoDocumentoClick, type: types.event, text: "Defazer Rejeição", idGrid: guid(), visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
    this.Reprocessar = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: reprocessarGestaoDocumentoClick, text: "Reprocessar" });
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
        metodo: AprovarGestaoDocumento
    };

    var opcaoRejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar ? true : false;
        },
        metodo: rejeitarGestaoDocumento
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [opcaoAprovar, opcaoRejeitar]
    };

    if (!permitirEditarInformacoesGestaoDocumentos())
        menuOpcoes = null;

    var knoutGestaoDocumentoAprovacao = {
        Codigo: _detalheGestaoDocumentoAprovacao.Codigo,
        Usuario: _detalheGestaoDocumentoAprovacao.Usuario,
    };

    _gridRegras = new GridView(_autorizacao.Regras.idGrid, "GestaoDocumento/RegrasAprovacao", knoutGestaoDocumentoAprovacao, menuOpcoes);
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
        executarReST("GestaoDocumento/AprovarMultiplasRegras", { Codigo: _detalheGestaoDocumentoAprovacao.Codigo.val() }, function (retorno) {
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

                    atualizarGridGestaoDocumentos();
                    atualizarGestaoDocumentoAprovacao();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function cancelarAutorizacaoGestaoDocumentoClick() {
    limparRegras();
}

function desfazerAprovacaoGestaoDocumentoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja desfazer a aprovação deste CT-e?", function () {
        executarReST("GestaoDocumento/DesfazerAprovacao", { Codigo: _detalheGestaoDocumentoAprovacao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aprovação do documento revertida com sucesso");
                    atualizarGridGestaoDocumentos();
                    atualizarGestaoDocumentoAprovacao();
                    limparRegras();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function desfazerRejeicaoGestaoDocumentoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja desfazer a rejeição deste CT-e?", function () {
        executarReST("GestaoDocumento/DesfazerRejeicao", { Codigo: _detalheGestaoDocumentoAprovacao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Rejeição do documento revertida com sucesso");
                    atualizarGridGestaoDocumentos();
                    atualizarGestaoDocumentoAprovacao();
                    limparRegras();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function rejeitarAprovacaoGestaoDocumentoClick() {
    if (_autorizacao.Motivo.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "Para rejeitar o documento é necessário informar ao mínimo 20 caracteres no motivo.");

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar o documento?", function () {
        var dados = {
            Codigo: _regra.Codigo.val(),
            Motivo: _autorizacao.Motivo.val()
        };
        executarReST("GestaoDocumento/Reprovar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridGestaoDocumentos();
                    atualizarGestaoDocumentoAprovacao();
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

function reprocessarGestaoDocumentoClick() {
    executarReST("GestaoDocumento/ReprocessarDocumento", { Codigo: _detalheGestaoDocumentoAprovacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegraReprocessada) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O documento foi reprocessado com sucesso.");

                    atualizarGridGestaoDocumentos();
                    atualizarGestaoDocumentoAprovacao();
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

        if (permitirEditarInformacoesGestaoDocumentos()) {
            retorno.data.forEach(function (autorizacao) {
                exibirBotaoReprocessar = false;

                if (autorizacao.PodeAprovar)
                    exibirBotaoAprovarTodas = true;
            });
        }
        else
            exibirBotaoReprocessar = false;

        _autorizacao.AprovarTodas.visible(exibirBotaoAprovarTodas);
        _autorizacao.Reprocessar.visible(exibirBotaoReprocessar);
    });
}

function controlarComponentesDetalheGestaoDocumentoAprovacao() {
    var exibirBotaoDesfazerAprovacao = false;
    var exibirBotaoDesfazerRejeicao = false;

    if (permitirEditarInformacoesGestaoDocumentos()) {
        if (EnumSituacaoGestaoDocumento.isAprovado(_detalheGestaoDocumentoAprovacao.SituacaoGestaoDocumento.val()))
            exibirBotaoDesfazerAprovacao = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GestaoDocumento_PermitirDesfazerAprovacao, _PermissoesPersonalizadas);
        else if (_detalheGestaoDocumentoAprovacao.SituacaoGestaoDocumento.val() == EnumSituacaoGestaoDocumento.Rejeitado)
            exibirBotaoDesfazerRejeicao = true;
    }

    _autorizacao.DesfazerAprovacao.visible(exibirBotaoDesfazerAprovacao);
    _autorizacao.DesfazerRejeicao.visible(exibirBotaoDesfazerRejeicao);
}

function limparRegras() {
    _autorizacao.DesfazerAprovacao.visible(false);
    _autorizacao.Motivo.visible(false);
    _autorizacao.Rejeitar.visible(false);

    LimparCampos(_autorizacao);
}

// #endregion Funções Públicas

// #region Funções Privadas

function AprovarGestaoDocumento(registroSelecionado) {
    executarReST("GestaoDocumento/Aprovar", registroSelecionado, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                atualizarGridGestaoDocumentos();
                atualizarGestaoDocumentoAprovacao();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function rejeitarGestaoDocumento(registroSelecionado) {
    _regra.Codigo.val(registroSelecionado.Codigo);

    _autorizacao.Motivo.visible(true);
    _autorizacao.Rejeitar.visible(true);
}

// #endregion Funções Privadas
