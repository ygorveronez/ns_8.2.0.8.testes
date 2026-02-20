/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridSolicitacaoVeiculoFluxoPatio;
var _pesquisaSolicitacaoVeiculoFluxoPatio;
var _solicitacaoVeiculoFluxoPatio;

/*
 * Declaração das Classes
 */

var PesquisaSolicitacaoVeiculoFluxoPatio = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoSolicitacaoVeiculo.AguardandoSolicitacaoVeiculo), options: EnumSituacaoSolicitacaoVeiculo.obterOpcoesPesquisa(), def: EnumSituacaoSolicitacaoVeiculo.AguardandoSolicitacaoVeiculo });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridSolicitacaoVeiculoFluxoPatio.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var SolicitacaoVeiculoFluxoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ObservacaoFluxoPatio = PropertyEntity({ visible: false });

    this.NumeroCarga = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "" });
    this.NumeroPreCarga = PropertyEntity({ text: "Pré Carga:", val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: "Data:", val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: "Hora:", val: ko.observable(""), def: "" });

    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: "Destinatário:", val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ text: "Fornecedor:", val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação:", val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: "Transportador:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículo:", val: ko.observable(""), def: "" });

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaSolicitacaoVeiculoClick, type: types.event, text: "Confirmar", visible: ko.observable(false) });
    this.ExibirObservacao = PropertyEntity({ eventClick: function () { exibirObservacaoFluxoPatio(_solicitacaoVeiculoFluxoPatio.ObservacaoFluxoPatio.val()); }, type: types.event, text: "Exibir Observação" });
    this.VerDetalhes = PropertyEntity({ eventClick: exibirDetalhesClick, type: types.event, text: "Ver Detalhes", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridSolicitacaoVeiculoFluxoPatio() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSolicitacaoVeiculoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar], tamanho: "10" };

    _gridSolicitacaoVeiculoFluxoPatio = new GridView(_pesquisaSolicitacaoVeiculoFluxoPatio.Pesquisar.idGrid, "SolicitacaoVeiculo/Pesquisa", _pesquisaSolicitacaoVeiculoFluxoPatio, menuOpcoes, null);
    _gridSolicitacaoVeiculoFluxoPatio.CarregarGrid();
}

function loadSolicitacaoVeiculoFluxoPatio() {
    _pesquisaSolicitacaoVeiculoFluxoPatio = new PesquisaSolicitacaoVeiculoFluxoPatio();
    KoBindings(_pesquisaSolicitacaoVeiculoFluxoPatio, "knockoutPesquisaSolicitacaoVeiculo", false, _pesquisaSolicitacaoVeiculoFluxoPatio.Pesquisar.id);

    _solicitacaoVeiculoFluxoPatio = new SolicitacaoVeiculoFluxoPatio();
    KoBindings(_solicitacaoVeiculoFluxoPatio, "knockoutSolicitacaoVeiculoFluxoPatio");

    loadObservacaoFluxoPatio();
    loadDetalhePedido();
    loadGridSolicitacaoVeiculoFluxoPatio();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function avancarEtapaSolicitacaoVeiculoClick() {
    executarReST("SolicitacaoVeiculo/AvancarEtapa", { Codigo: _solicitacaoVeiculoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de veículo finalizada com sucesso");
                editarSolicitacaoVeiculoFluxoPatio(_solicitacaoVeiculoFluxoPatio.Codigo.val());
                recarregarGridSolicitacaoVeiculoFluxoPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function editarSolicitacaoVeiculoClick(registroSelecionado) {
    editarSolicitacaoVeiculoFluxoPatio(registroSelecionado.Codigo);
}

/*
 * Declaração das Funções Privadas
 */

function editarSolicitacaoVeiculoFluxoPatio(codigo) {
    LimparCampos(_solicitacaoVeiculoFluxoPatio);

    executarReST("SolicitacaoVeiculo/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_solicitacaoVeiculoFluxoPatio, retorno);

                $("#container-solicitacao-veiculo").show();

                _solicitacaoVeiculoFluxoPatio.AvancarEtapa.visible(_solicitacaoVeiculoFluxoPatio.Situacao.val() == EnumSituacaoSolicitacaoVeiculo.AguardandoSolicitacaoVeiculo);
                _pesquisaSolicitacaoVeiculoFluxoPatio.ExibirFiltros.visibleFade(false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function recarregarGridSolicitacaoVeiculoFluxoPatio() {
    _gridSolicitacaoVeiculoFluxoPatio.CarregarGrid();
}

function exibirDetalhesClick() {
    exibirDetalhesPedidos(_solicitacaoVeiculoFluxoPatio.Carga.val());
}