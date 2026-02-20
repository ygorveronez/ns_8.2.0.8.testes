/// <reference path="LicitacaoParticipacaoAvaliacao.js" />
/// <reference path="Resumo.js" />
/// <reference path="..\..\Enumeradores\EnumSituacaoLicitacaoParticipacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _avaliacao;
var _CRUDAvaliacao;
var _gridAvaliacao;
var _reprovarLicitacaoParticipacao
/*
 * Declaração das Classes
 */

var Avaliacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLicitacaoParticipacao.Todas), def: EnumSituacaoLicitacaoParticipacao.Todas, getType: typesKnockout.int });
}

var CRUDAvaliacao = function () {
    this.Aprovar = PropertyEntity({ eventClick: aprovarOfertaClick, type: types.event, text: ko.observable("Aprovar") });
    this.Reprovar = PropertyEntity({ eventClick: exibirModalReprovarLicitacaoParticipacaoClick, type: types.event, text: "Rejeitar" });
}

var ReprovarLicitacaoParticipacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 2000 });

    this.Reprovar = PropertyEntity({ eventClick: reprovarOfertaClick, type: types.event, text: "Rejeitar" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAvaliacao() {
    _avaliacao = new Avaliacao();
    KoBindings(_avaliacao, "knockoutLicitacaoParticipacaoAvaliacao");

    _reprovarLicitacaoParticipacao = new ReprovarLicitacaoParticipacao();
    KoBindings(_reprovarLicitacaoParticipacao, "knockoutReprovarLicitacaoParticipacao");

    _CRUDAvaliacao = new CRUDAvaliacao();
    KoBindings(_CRUDAvaliacao, "knockoutCRUDLicitacaoParticipacaoAvaliacao");

    loadGridAvaliacao();
}

function loadGridAvaliacao() {
    var quantidadePorPagina = 10;

    _gridAvaliacao = new GridView("grid-licitacao-participacao-oferta", "LicitacaoParticipacaoAvaliacao/PesquisaOferta", _avaliacao, null, null, quantidadePorPagina);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarOfertaClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar a proposta?", function () {
        executarReST("LicitacaoParticipacaoAvaliacao/Aprovar", { Codigo: _avaliacao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aprovação realizada com sucesso.");
                    fecharModalAvaliacao();
                    recarregarGridLicitacaoParticipacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function exibirModalReprovarLicitacaoParticipacaoClick() {
    _reprovarLicitacaoParticipacao.Codigo.val(_avaliacao.Codigo.val());

    exibirModalReprovarLicitacaoParticipacao();
}

function reprovarOfertaClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja rejeitar a proposta?", function () {
        executarReST("LicitacaoParticipacaoAvaliacao/Reprovar", RetornarObjetoPesquisa(_reprovarLicitacaoParticipacao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Rejeição realizada com sucesso.");
                    fecharModalReprovarLicitacaoParticipacao();
                    fecharModalAvaliacao();
                    recarregarGridLicitacaoParticipacao();
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

function buscarPorCodigoLicitacaoParticipacao(callback) {
    executarReST("LicitacaoParticipacaoAvaliacao/BuscarPorLicitacaoParticipacao", { Codigo: _avaliacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_avaliacao, { Data: retorno.Data.LicitacaoParticipacao });
                preencherResumo(retorno.Data.Resumo);
                controlarExibicaoBotoesAprovacao();
                recarregarGridAvaliacao();

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

function controlarExibicaoBotoesAprovacao() {
    if (_avaliacao.Situacao.val() === EnumSituacaoLicitacaoParticipacao.AguardandoRetornoOferta)
        $("#knockoutCRUDLicitacaoParticipacaoAvaliacao").show();
    else
        $("#knockoutCRUDLicitacaoParticipacaoAvaliacao").hide();
}

function exibirAvaliacao(codigoLicitacaoParticipacao) {
    _avaliacao.Codigo.val(codigoLicitacaoParticipacao);

    buscarPorCodigoLicitacaoParticipacao(exibirModalAvaliacao);
}

function exibirModalAvaliacao() {
    Global.abrirModal('divModalLicitacaoParticipacaoAvaliacao');
    $("#divModalLicitacaoParticipacaoAvaliacao").one('hidden.bs.modal', function () {
        limparLicitacaoParticipacaoAvaliacao();
    });
}

function exibirModalReprovarLicitacaoParticipacao() {
    Global.abrirModal('divModalReprovarLicitacaoParticipacao');
    $("#divModalReprovarLicitacaoParticipacao").one('hidden.bs.modal', function () {
        LimparCampos(_reprovarLicitacaoParticipacao);
    });
}

function fecharModalAvaliacao() {
    Global.fecharModal('divModalLicitacaoParticipacaoAvaliacao');
}

function fecharModalReprovarLicitacaoParticipacao() {
    Global.fecharModal('divModalReprovarLicitacaoParticipacao');
}

function limparLicitacaoParticipacaoAvaliacao() {
    LimparCampos(_avaliacao);
    limparResumo();
}

function recarregarGridAvaliacao() {
    _gridAvaliacao.CarregarGrid();
}