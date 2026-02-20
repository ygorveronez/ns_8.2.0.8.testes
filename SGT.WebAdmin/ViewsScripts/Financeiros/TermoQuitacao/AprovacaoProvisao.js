/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacaoFinanciero.js" />


//#region Propriedades Globais
var _aprovacaoProvisao;
var _detalheAutorizacaoProvacao;
var _gridAutorizacoesProvisao;
//#endregion


//#region Construtores
var AprovacaoProvisao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Solicitante = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Solicitante:", enable: ko.observable(true) });
    this.DataSolicitacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data da Solicitação:", enable: ko.observable(true) });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:", enable: ko.observable(true) });

    this.DetalheAprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(false), val: ko.observable(true), def: true });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false), def: false });
    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });

    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresProvisaoClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Exibir Aprovadores" : "Ocultar Aprovadores";
        }, this), visible: ko.observable(false), val: ko.observable(true), def: true
    });
}

var DetalheAutorizacaoProvisao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}
//#endregion

//#region Funções Carregadoras
function loadAprovacaoProvisao() {
    _aprovacaoProvisao = new AprovacaoProvisao();
    KoBindings(_aprovacaoProvisao, "knockoutAprovacaoProvisao");

    _detalheAutorizacaoProvacao = new DetalheAutorizacaoProvisao();
    KoBindings(_detalheAutorizacaoProvacao, "knockoutDetalheAutorizacaoProvisao");

    loadGridAutorizacoesProvisao();
}

function loadGridAutorizacoesProvisao() {
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoProvisaoClick, tamanho: "4", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoesProvisao = new GridView(_aprovacaoProvisao.Regras.idGrid, "TermoQuitacaoFinanceiro/PesquisaAutorizacoes", _aprovacaoProvisao, menuOpcoes);
}

//#endregion

//#region Funções Auxiliares
function toggleExibirAprovadoresProvisaoClick() {
    var valorAtual = _aprovacaoProvisao.ExibirAprovadores.val();

    _aprovacaoProvisao.ExibirAprovadores.val(!valorAtual);
    _aprovacaoProvisao.DetalheAprovadores.visible(valorAtual);
}

function detalharAutorizacaoProvisaoClick() {
    BuscarPorCodigo(_detalheAutorizacao, "TermoQuitacaoFinanceiro/DetalhesAutorizacao", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != null) {
                Global.abrirModal('divModalDetalhesAutorizacaoProvisao');
                $("#divModalDetalhesAutorizacaoProvisao").one('hidden.bs.modal', function () {
                    limparCamposDetalheAutorizacaoProvisao();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    });
}

function ObterDetalhesGeraisAprovacao() {
    executarReST("TermoQuitacaoFinanceiro/ObterDetalhesGeraisAprovacao", { Codigo: _termoQuitacao.Codigo.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);

        PreencherObjetoKnout(_aprovacaoProvisao,{ Data: arg.Data });
    })
}
function limparCamposDetalheAutorizacaoProvisao() {
    LimparCampos(_detalheAutorizacao);
}

function limparCamposAprovacao() {
    LimparCampos(_detalheAutorizacaoProvacao);
    LimparCampos(_aprovacaoProvisao);

    _gridAutorizacoesProvisao.CarregarGrid();
}


function reprocessarRegrasClick(e, sender) {
    executarReST("TermoQuitacaoFinanceiro/ReprocessarRegras", { Codigo: _aprovacaoProvisao.Codigo.val() }, function (retorno) {
        if (!retorno.Success)
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        if (retorno.Data) {
            var dados = retorno.Data;
            if (dados.Situacao === EnumSituacoesFatura.SemRegraAprovacao)
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a Ordem de Serviço.");
            else if (dados.Situacao === EnumSituacoesFatura.AguardandoAprovacao)
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem de Serviço está aguardando aprovação.");

            preencherAprovacaoAprovacaoTermoQuitacaoFinanceiro(dados);

        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
    });
}
function detalharAutorizacaoProvisaoPendenteClick() {

    _aprovacaoProvisao.Codigo.val(_termoQuitacao.Codigo.val());
    _aprovacaoProvisao.PossuiRegras.val(_termoQuitacao.PossuiRegrasAprovacao.val())

    _gridAutorizacoesProvisao.CarregarGrid((arg) => {
        _aprovacaoProvisao.PossuiRegras.visible(_aprovacaoProvisao.PossuiRegras.val());
    });
    //BuscarPorCodigo(_aprovacaoProvisao, "TermoQuitacaoFinanceiro/PesquisaAutorizacoes", function (retorno) {
    //    if (!retorno.Success)
    //        exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);

    //});
}
function preencherAprovacaoAprovacaoTermoQuitacaoFinanceiro(dados) {

    _aprovacaoProvisao.PossuiRegras.val(!(dados.Situacao === EnumSituacaoTermoQuitacaoFinanceiro.SemRegraProvisao));

    PreencherObjetoKnout(_aprovacaoProvisao, { Data: dados });

    _gridAutorizacoesProvisao.CarregarGrid();
}
//#endregion