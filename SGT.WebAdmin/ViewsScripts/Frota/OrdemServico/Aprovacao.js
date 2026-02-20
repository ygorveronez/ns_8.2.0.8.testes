/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />
/// <reference path="OrdemServico.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _aprovacao;
var _detalheAutorizacao;
var _gridAutorizacoes;

/*
 * Declaração das Classes
 */

var Aprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });

    this.DetalheAprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(false), val: ko.observable(true), def: true });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });

    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Exibir Aprovadores" : "Ocultar Aprovadores";
        }, this), visible: ko.observable(false), val: ko.observable(true), def: true
    });
};

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadAprovacaoOrdemServico() {
    _aprovacao = new Aprovacao();
    KoBindings(_aprovacao, "knockoutAprovacao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    loadGridAutorizacoes();
}

function CarregarAprovacaoOrdemServico() {
    LimparCamposAprovacaoOrdemServico();

    executarReST("OrdemServicoAprovacao/ObterDetalhesGeraisAprovacao", { Codigo: _ordemServico.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                preencherAprovacaoOrdemServico(r.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function loadGridAutorizacoes() {
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "4", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacao.Regras.idGrid, "OrdemServicoAprovacao/PesquisaAutorizacoes", _aprovacao, menuOpcoes);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacao.ExibirAprovadores.val();

    _aprovacao.ExibirAprovadores.val(!valorAtual);
    _aprovacao.DetalheAprovadores.visible(valorAtual);
}

function reprocessarRegrasClick(e, sender) {
    executarReST("OrdemServicoAprovacao/ReprocessarRegras", { Codigo: _ordemServico.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var dados = retorno.Data;
                if (dados.Situacao === EnumSituacaoOrdemServicoFrota.SemRegraAprovacao)
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a Ordem de Serviço.");
                else if (dados.Situacao === EnumSituacaoOrdemServicoFrota.AguardandoAprovacao)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem de Serviço está aguardando aprovação.");

                _resumoOrdemServico.Situacao.val(dados.DescricaoSituacao);
                preencherAprovacaoOrdemServico(dados);

                if (_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.EmManutencao) {
                    _CRUDOrdemServico.FecharOrdemServico.visible(true);
                    _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(true);
                    _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                    SetarEtapaOrdemServico();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function detalharAutorizacaoClick(registroSelecionado) {
    _detalheAutorizacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "OrdemServicoAprovacao/DetalhesAutorizacao", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != null) {
                Global.abrirModal('divModalDetalhesAutorizacao');
                $("#divModalDetalhesAutorizacao").one('hidden.bs.modal', function () {
                    limparCamposDetalheAutorizacao();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function preencherAprovacaoOrdemServico(dados) {
    if (dados.Situacao === EnumSituacaoOrdemServicoFrota.SemRegraAprovacao)
        _aprovacao.PossuiRegras.val(false);
    else {
        _aprovacao.PossuiRegras.val(true);

        PreencherObjetoKnout(_aprovacao, { Data: dados });

        _gridAutorizacoes.CarregarGrid();
    }
}

function limparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao);
}

function LimparCamposAprovacaoOrdemServico() {
    LimparCampos(_detalheAutorizacao);
    LimparCampos(_aprovacao);
}