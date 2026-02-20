/// <reference path="LicitacaoParticipacaoAnexo.js" />
/// <reference path="LicitacaoParticipacaoEtapa.js" />
/// <reference path="LicitacaoParticipacaoOferta.js" />
/// <reference path="LicitacaoParticipacaoResumo.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumAbaLicitacaoParticipacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLicitacaoParticipacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDLicitacaoParticipacaoCadastro;
var _licitacaoParticipacaoCadastro;

/*
 * Declaração das Classes
 */

var CRUDLicitacaoParticipacaoCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: ko.observable("Participar"), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Deixar Licitação", visible: ko.observable(false) });
}

var LicitacaoParticipacaoCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoLicitacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable(""), def: "", getType: typesKnockout.int, enable: false });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 2000, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLicitacaoParticipacao.Todas), def: EnumSituacaoLicitacaoParticipacao.Todas, text: "*Situãção: ", getType: typesKnockout.int, visible: false });
    this.AceitarTermosContrato = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Aceitar termos de contrato", def: false, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadLicitacaoParticipacaoCadastro() {
    _licitacaoParticipacaoCadastro = new LicitacaoParticipacaoCadastro();
    KoBindings(_licitacaoParticipacaoCadastro, "knockoutLicitacaoParticipacaoInscricao");

    _CRUDLicitacaoParticipacaoCadastro = new CRUDLicitacaoParticipacaoCadastro();
    KoBindings(_CRUDLicitacaoParticipacaoCadastro, "knockoutCRUDCadastroLicitacaoParticipacao");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    switch (_licitacaoParticipacaoCadastro.Situacao.val()) {
        case EnumSituacaoLicitacaoParticipacao.Todas:
            participar();
            break;

        case EnumSituacaoLicitacaoParticipacao.AguardandoOferta:
            if (_abaAtiva === EnumAbaLicitacaoParticipacao.Oferta) {
                enviarOferta();
            }
            break;

        case EnumSituacaoLicitacaoParticipacao.OfertaRecusada:
            if (_abaAtiva === EnumAbaLicitacaoParticipacao.RetornoOferta) {
                refazerOferta();
            }
            break;
    }
}

function cancelarClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja deixar a licitação?", function () {
        executarReST("LicitacaoParticipacao/Cancelar", { Codigo: _licitacaoParticipacaoCadastro.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Participação cancelada com sucesso.");
                    buscarPorCodigoLicitacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções
 */

function buscarPorCodigoLicitacao(callback) {
    executarReST("LicitacaoParticipacao/BuscarPorLicitacao", { CodigoLicitacao: _licitacaoParticipacaoCadastro.CodigoLicitacao.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.Inscricao)
                    PreencherObjetoKnout(_licitacaoParticipacaoCadastro, { Data: retorno.Data.Inscricao });

                preencherResumo(retorno.Data.Resumo);
                preencherOferta(retorno.Data.Oferta);
                if (retorno.Data.Oferta != null)
                    preencherOpcoesOferta(retorno.Data.Oferta.OpcaoOferta);
                preencherRetornoOferta(retorno.Data.RetornoOferta);
                recarregarGridAnexo(retorno.Data.Anexos);
                controlarComponentesHabilitados();
                setarEtapas();

                if (callback instanceof Function)
                    callback();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function controlarBotoesHabilitados() {
    var botaoAdicionarTexto = "Adicionar";
    var botaoAdicionarVisivel = false;
    var botaoCancelarVisivel = false;

    switch (_licitacaoParticipacaoCadastro.Situacao.val()) {
        case EnumSituacaoLicitacaoParticipacao.Todas:
            botaoAdicionarTexto = "Participar";
            botaoAdicionarVisivel = true;
            break;

        case EnumSituacaoLicitacaoParticipacao.AguardandoOferta:
            if (_abaAtiva === EnumAbaLicitacaoParticipacao.Oferta) {
                botaoAdicionarTexto = "Enviar Oferta";
                botaoAdicionarVisivel = true;
            }
            break;

        case EnumSituacaoLicitacaoParticipacao.AguardandoRetornoOferta:
            if (_abaAtiva === EnumAbaLicitacaoParticipacao.RetornoOferta) {
                botaoCancelarVisivel = true;
            }
            break;

        case EnumSituacaoLicitacaoParticipacao.OfertaRecusada:
            if (_abaAtiva === EnumAbaLicitacaoParticipacao.RetornoOferta) {
                botaoAdicionarTexto = "Refazer Oferta"
                botaoAdicionarVisivel = true;
                botaoCancelarVisivel = true;
            }
            break;
    }

    _CRUDLicitacaoParticipacaoCadastro.Adicionar.text(botaoAdicionarTexto);
    _CRUDLicitacaoParticipacaoCadastro.Adicionar.visible(botaoAdicionarVisivel);
    _CRUDLicitacaoParticipacaoCadastro.Cancelar.visible(botaoCancelarVisivel);
    _licitacaoParticipacaoOferta.OpcoesOferta.enable(botaoAdicionarVisivel);
}

function controlarCamposHabilitados() {
    _licitacaoParticipacaoCadastro.AceitarTermosContrato.visible(false);
    _licitacaoParticipacaoCadastro.Observacao.enable(false);

    switch (_licitacaoParticipacaoCadastro.Situacao.val()) {
        case EnumSituacaoLicitacaoParticipacao.Todas:
            _licitacaoParticipacaoCadastro.Observacao.enable(true);
            _licitacaoParticipacaoCadastro.AceitarTermosContrato.visible(true);
            break;

    }
}

function controlarComponentesHabilitados() {
    controlarCamposHabilitados();
    controlarBotoesHabilitados();
}

function enviarOferta() {
    if (_tabelaFreteCliente.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Selecione uma tabela de frete.");
        return;
    }

    obterValoresTabelaValores();
    _tabelaFreteCliente.CodigoLicitacaoParticipacao.val(_licitacaoParticipacaoCadastro.Codigo.val());

    exibirConfirmacao("Confirmação", "Você realmente deseja enviar a oferta para a licitação?", function () {
        Salvar(_tabelaFreteCliente, "LicitacaoParticipacao/EnviarOferta", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Oferta enviada com sucesso.");
                    buscarPorCodigoLicitacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function exibirLicitacaoParticipacaoCadastro(registroSelecionado) {
    _licitacaoParticipacaoCadastro.CodigoLicitacao.val(registroSelecionado.Codigo);

    buscarPorCodigoLicitacao(exibirModalLicitacaoParticipacao);
}

function exibirModalLicitacaoParticipacao() {
    Global.abrirModal('divModalLicitacaoParticipacao');
    $("#divModalLicitacaoParticipacao").one('hidden.bs.modal', function () {
        limparLicitacaoParticipacaoCadastro();
    });
}

function limparLicitacaoParticipacaoCadastro() {
    LimparCampos(_licitacaoParticipacaoCadastro);
    limparResumo();
    limparOferta();
    limparRetornoOferta();
    setarEtapaInicial();
}

function participar() {
    if (_licitacaoParticipacaoCadastro.AceitarTermosContrato.val()) {
        exibirConfirmacao("Confirmação", "Você realmente deseja participar da licitação?", function () {
            Salvar(_licitacaoParticipacaoCadastro, "LicitacaoParticipacao/Participar", function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Participação realizada com sucesso.");
                        buscarPorCodigoLicitacao();
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    }
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Para participar da licitação é necessário aceitar os termos de contrato");
}

function refazerOferta() {
    exibirConfirmacao("Confirmação", "Você realmente deseja refazer a oferta para a licitação?", function () {
        executarReST("LicitacaoParticipacao/RefazerOferta", { Codigo: _licitacaoParticipacaoCadastro.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação para refazer oferta realizada com sucesso.");
                    buscarPorCodigoLicitacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}