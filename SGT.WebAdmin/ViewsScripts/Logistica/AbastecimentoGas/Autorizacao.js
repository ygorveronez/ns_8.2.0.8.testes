var _abastecimentoGasAprovacao;
var _abastecimentoGasAprovacaoDetalhe;
var _gridSolicitacaoGasAprovacao;

var AbastecimentoGasAprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.DescricaoSituacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ExibirAprovacao = PropertyEntity({ eventClick: exibirSolicitacaoGasAprovacaoClick, type: types.event, text: ko.observable("Exibir Aprovação"), fadeVisible: ko.observable(false), visible: ko.observable(false) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasSolicitacaoGasClick, type: types.event, text: ko.observable("Reprocessar Regras") });
}

var SolicitacaoAbastecimentoAprovacaoDetalhe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridSolicitacaoGasAprovacao() {
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharSolicitacaoGasAprovacaoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    _gridSolicitacaoGasAprovacao = new GridView("grid-solicitacao-gas-aprovacao", "SolicitacaoAbastecimentoGas/PesquisaAprovacoes", _abastecimentoGasAprovacao, menuOpcoes);
    _gridSolicitacaoGasAprovacao.CarregarGrid();
}

function loadAutorizacaoSolicitacaoGas() {
    _abastecimentoGasAprovacao = new AbastecimentoGasAprovacao();
    KoBindings(_abastecimentoGasAprovacao, "knockoutSolicitacaoGasAprovacao");

    _abastecimentoGasAprovacaoDetalhe = new SolicitacaoAbastecimentoAprovacaoDetalhe();
    KoBindings(_abastecimentoGasAprovacaoDetalhe, "knockoutSolicitacaoAbastecimentoAprovacaoDetalhe");

    loadGridSolicitacaoGasAprovacao();
}

function exibirSolicitacaoGasAprovacaoClick() {
    controlarExibicaoSolicitacaoAbastecimentoAprovacao(_abastecimentoGasAprovacao.ExibirAprovacao.fadeVisible() == false);
}

function reprocessarRegrasSolicitacaoGasClick() {
    executarReST("SolicitacaoAbastecimentoGas/ReprocessarRegras", { Codigo: _abastecimentoGasAprovacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");
                preencherSolicitacaoAbastecimentoGasAprovacao(_abastecimentoGasAprovacao.Codigo.val());
                _gridPesquisaAbastecimentoGas.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", retorno.Msg || "Nenhuma regra de aprovação encontrada.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function controlarExibicaoSolicitacaoAbastecimentoAprovacao(exibir) {
    _abastecimentoGasAprovacao.ExibirAprovacao.fadeVisible(exibir);
    _abastecimentoGasAprovacao.ExibirAprovacao.text(exibir ? "Ocultar Aprovação" : "Exibir Aprovação");
}

function exibirModalSolicitacaoGasAprovacao() {
    Global.abrirModal('divModalSolicitacaoGasAprovacao');
    $("#divModalSolicitacaoGasAprovacao").one('hidden.bs.modal', function () {
        LimparCampos(_abastecimentoGasAprovacaoDetalhe);
    });
}

function preencherSolicitacaoAbastecimentoGasAprovacao(codigoAbastecimento) {
    executarReST("SolicitacaoAbastecimentoGas/BuscarResumoAprovacao", { Codigo: codigoAbastecimento }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_abastecimentoGasAprovacao, retorno);

                if (retorno.Data.PossuiAlcada) {
                    _abastecimentoGasAprovacao.ExibirAprovacao.visible(true);
                    _gridSolicitacaoGasAprovacao.CarregarGrid();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function detalharSolicitacaoGasAprovacaoClick(registroSelecionado) {
    executarReST("SolicitacaoAbastecimentoGas/DetalhesAprovacao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_abastecimentoGasAprovacaoDetalhe, retorno);
                exibirModalSolicitacaoGasAprovacao();
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function limparSolicitacaoGasAprovacao() {
    LimparCampos(_abastecimentoGasAprovacao);
    controlarExibicaoSolicitacaoAbastecimentoAprovacao(false);
    _abastecimentoGasAprovacao.ExibirAprovacao.visible(false);
    _gridSolicitacaoGasAprovacao.CarregarGrid();
}