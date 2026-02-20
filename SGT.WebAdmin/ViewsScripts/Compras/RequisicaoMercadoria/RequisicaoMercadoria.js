/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoRequisicaoMercadoria.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/MotivoCompra.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="Aprovacao.js" />
/// <reference path="Etapas.js" />
/// <reference path="Mercadorias.js" />
/// <reference path="Requisicao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRequisicaoMercadoria;
var _requisicaoMercadoria;
var _CRUDRequisicaoMercadoria;
var _pesquisaRequisicaoMercadoria;

var _situacaoRequisicaoMercadoria = [
    { text: "Todos", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoRequisicaoMercadoria.AgAprovacao },
    { text: "Aprovada", value: EnumSituacaoRequisicaoMercadoria.Aprovada },
    { text: "Rejeitada", value: EnumSituacaoRequisicaoMercadoria.Rejeitada },
    { text: "Finalizado", value: EnumSituacaoRequisicaoMercadoria.Finalizado }
];

var RequisicaoMercadoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0 });
};

var CRUDRequisicaoMercadoria = function () {
    this.Limpar = PropertyEntity({ eventClick: limparCamposRequisicaoClick, type: types.event, text: "Limpar (Gerar Nova Requisição)", idGrid: guid(), visible: ko.observable(true) });
    this.GerarRequisicao = PropertyEntity({ eventClick: gerarRequisicaoClick, type: types.event, text: "Gerar Requisição", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRequisicaoClick, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(false) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirRequisicaoClick, type: types.event, text: "Imprimir", visible: ko.observable(false), enable: ko.observable(true) });
    this.AtualizarProdutos = PropertyEntity({ eventClick: atualizarProdutosClick, type: types.event, text: "Atualizar Produtos", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaRequisicaoMercadoria = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.FuncionarioRequisitado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Requisitado:", idBtnSearch: guid() });
	this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });

    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoRequisicaoMercadoria, def: "", text: "Situação: " });
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRequisicaoMercadoria.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};


//*******EVENTOS*******

function loadRequisicaoMercadoria() {
    _requisicaoMercadoria = new RequisicaoMercadoria();
    HeaderAuditoria("RequisicaoMercadoria", _requisicaoMercadoria);

    _CRUDRequisicaoMercadoria = new CRUDRequisicaoMercadoria();
    KoBindings(_CRUDRequisicaoMercadoria, "knockoutCRUD");

    _pesquisaRequisicaoMercadoria = new PesquisaRequisicaoMercadoria();
    KoBindings(_pesquisaRequisicaoMercadoria, "knockoutPesquisaRequisicaoMercadoria", false, _pesquisaRequisicaoMercadoria.Pesquisar.id);

    LoadRequisicao();
    LoadMercadorias();
    LoadAutorizacao();
    LoadEtapasRequisicao();

    BuscarEmpresa(_pesquisaRequisicaoMercadoria.Filial);
    BuscarMotivoCompra(_pesquisaRequisicaoMercadoria.Motivo);
    BuscarFuncionario(_pesquisaRequisicaoMercadoria.FuncionarioRequisitado);
    BuscarVeiculos(_pesquisaRequisicaoMercadoria.Veiculo);
    BuscarRequisicaoMercadoria();
}

function gerarRequisicaoClick(e, sender) {
    Salvar(_requisicao, "RequisicaoMercadoria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Requisição gerada com sucesso");
                _gridRequisicaoMercadoria.CarregarGrid();
                LimparCamposRequisicao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarProdutosClick(e, sender) {
    Salvar(_requisicao, "RequisicaoMercadoria/AtualizarProdutos", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Produtos alterados com sucesso");
                _gridRequisicaoMercadoria.CarregarGrid();
                LimparCamposRequisicao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarRequisicaoClick(e, sender) {
    LimparCamposRequisicao();
}

function limparCamposRequisicaoClick(e, sender) {
    LimparCamposRequisicao();
}

function imprimirRequisicaoClick(e, sender) {
    const data = { Codigo: _requisicaoMercadoria.Codigo.val() };
    executarReST("RequisicaoMercadoria/BaixarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******
function BuscarRequisicaoMercadoria() {
    const editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarRequisicao, tamanho: "15", icone: "" };
    const menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridRequisicaoMercadoria = new GridView(_pesquisaRequisicaoMercadoria.Pesquisar.idGrid, "RequisicaoMercadoria/Pesquisa", _pesquisaRequisicaoMercadoria, menuOpcoes);
    _gridRequisicaoMercadoria.CarregarGrid();
}

function editarRequisicao(itemGrid) {
    // Limpa os campos
    LimparCamposRequisicao();

    // Esconde filtros
    _pesquisaRequisicaoMercadoria.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarRequisicaoPorCodigo(itemGrid.Codigo);
}

function BuscarRequisicaoPorCodigo(codigo, callback) {
    executarReST("RequisicaoMercadoria/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            EditarRequisicao(arg.Data);

            ListarAprovacoes(arg.Data);

            CarregarProdutosDaRequisicao(arg.Data.Produtos);

            SetarEtapasRequisicao();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        if (callback != null)
            callback();
    }, null);
}

function EditarRequisicao(data) {
    _requisicaoMercadoria.Situacao.val(data.Situacao);
    _requisicaoMercadoria.Codigo.val(data.Codigo);

    _requisicao.Veiculo.required(data.Motivo.ExigeInformarVeiculoObrigatoriamente);
    PreencherObjetoKnout(_requisicao, { Data: data });

    _CRUDRequisicaoMercadoria.GerarRequisicao.visible(false);
    _CRUDRequisicaoMercadoria.Cancelar.visible(true);

    ControleCamposRequisicao(false);
    ControleCamposCRUD();
}

function LimparCamposRequisicao() {
    LimparCampos(_mercadorias);
    LimparCampos(_requisicao);
    _requisicao.Veiculo.required(false);

    LimparCampos(_requisicaoMercadoria);
    RecarregarGridProdutos();
    PreencheUsuarioLogado();

    ControleCamposCRUD();
    _CRUDRequisicaoMercadoria.GerarRequisicao.visible(true);

    ControleCamposRequisicao(true);

    SetarEtapaInicioRequisicao();
}

function ControleCamposCRUD() {
    const situacao = _requisicaoMercadoria.Situacao.val();

    _CRUDRequisicaoMercadoria.Limpar.visible(true);
    _CRUDRequisicaoMercadoria.GerarRequisicao.visible(false);
    _CRUDRequisicaoMercadoria.Cancelar.visible(false);

    if (situacao == EnumSituacaoRequisicaoMercadoria.Aprovada)
        _CRUDRequisicaoMercadoria.Imprimir.visible(true);
    else
        _CRUDRequisicaoMercadoria.Imprimir.visible(false);

    if (situacao == EnumSituacaoRequisicaoMercadoria.AgAprovacao)
        _CRUDRequisicaoMercadoria.AtualizarProdutos.visible(true);
    else
        _CRUDRequisicaoMercadoria.AtualizarProdutos.visible(false);
}