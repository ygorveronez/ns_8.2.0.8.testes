/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCotacao.js" />
/// <reference path="../../Consultas/RequisicaoCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCotacao;
var _pesquisaCotacao;
var _gridRequisicoes;
var codigosRequisicoes = new Array();
var _modalCotacaoCompra;

var _situacaoCotacao = [
    { text: "Todas", value: EnumSituacaoCotacao.Todas },
    { text: "Aberto", value: EnumSituacaoCotacao.Aberto },
    { text: "Aguardando Retorno", value: EnumSituacaoCotacao.AguardandoRetorno },
    { text: "Finalizado", value: EnumSituacaoCotacao.Finalizado },
    { text: "Cancelado", value: EnumSituacaoCotacao.Cancelado }
]

var PesquisaCotacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataEmissaoDe = PropertyEntity({ text: "Emissão De: ", getType: typesKnockout.date });
    this.DataEmissaoAte = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor: ", idBtnSearch: guid(), visible: true });

    this.DataRetornoDe = PropertyEntity({ text: "Prev. Retorno De: ", getType: typesKnockout.date });
    this.DataRetornoAte = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(EnumSituacaoCotacao.Todas), options: _situacaoCotacao, def: EnumSituacaoCotacao.Todas, text: "Situação: " });

    this.NovaCotacao = PropertyEntity({ eventClick: NovaCotacaoClick, type: types.event, text: "Nova Cotação", icon: ko.observable("fal fa-plus"), idGrid: guid(), visible: ko.observable(true) });
    this.ImportarRequisicao = PropertyEntity({ type: types.map, required: false, text: "Importar de Requisição", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCotacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

//*******EVENTOS*******

function loadCotacaoCompra() {
    carregarConteudosCotacaoHTML(carregarCoracaoCompra);
}

function carregarCoracaoCompra() {
    _pesquisaCotacao = new PesquisaCotacao();
    KoBindings(_pesquisaCotacao, "knockoutPesquisaCotacao", false, _pesquisaCotacao.Pesquisar.id);

    CarregarGridRequisicoes();

    new BuscarClientes(_pesquisaCotacao.Fornecedor);
    new BuscarProdutoTMS(_pesquisaCotacao.Produto);
    new BuscarRequisicaoCompra(_pesquisaCotacao.ImportarRequisicao, retornoRequisicoesCompras, null, _gridRequisicoes);

    buscarCotacaos();

    loadMercadoriaCotacaoCompra();
    loadFornecedorCotacaoCompra();
    loadRetornoCotacaoCompra();
    loadHistoricoCotacaoCompra();

    _modalCotacaoCompra = new bootstrap.Modal(document.getElementById("divModalCotacao"), { backdrop: 'static', keyboard: true });
}

function retornoRequisicoesCompras(requisicoes) {
    codigosRequisicoes = new Array();
    $.each(requisicoes, function (i, req) {
        codigosRequisicoes.push({ Codigo: req.Codigo });
    });

    LimparCamposCotacaoCompra();
    _cotacaoCompra.Codigo.val(JSON.stringify(codigosRequisicoes));

    BuscarPorCodigo(_cotacaoCompra, "CotacaoCompra/ImportarDeRequisicoes", function (arg) {
        _cotacaoCompra.Codigo.val(guid());

        RegarregarGridMercadorias();
        RegarregarGridRetornos();
        RegarregarGridFornecedores();
        RegarregarGridRetornoProdutoFornecedor();
        _cotacaoCompra.CodigoRequisicaoCompra.val(JSON.stringify(codigosRequisicoes));

        _cotacaoCompra.Numero.enable = false;
        _cotacaoCompra.ValorTotal.enable(false);
        _modalCotacaoCompra.show();
    }, null);
}

function NovaCotacaoClick(e, sender) {
    LimparCamposCotacaoCompra();
    _cotacaoCompra.Codigo.val(guid());
    _modalCotacaoCompra.show();
    _cotacaoCompra.Numero.enable = false;
    _cotacaoCompra.ValorTotal.enable(false);
}

//*******MÉTODOS*******

function CarregarGridRequisicoes() {
    var pedidos = new Array();

    var header = [{ data: "Codigo", visible: false },
    { data: "Numero", visible: false },
    { data: "Filial", visible: false },
    { data: "MotivoCompra", visible: false },
    { data: "Data", visible: false },
    { data: "Situacao", visible: false }
    ];

    _gridRequisicoes = new BasicDataTable(_pesquisaCotacao.ImportarRequisicao.idGrid, header);
    _gridRequisicoes.CarregarGrid(pedidos);
}

function buscarCotacaos() {

    var editarCotacao = { descricao: "Editar", id: guid(), metodo: editarCotacaoClick, icone: "" };
    var duplicarCotacao = { descricao: "Duplicar", id: guid(), metodo: duplicarCotacaoClick, icone: "" };
    var enviarEmailCotacao = { descricao: "Enviar E-mail", id: guid(), metodo: enviarEmailCotacaoClick, icone: "" };
    var visualizarCotacao = { descricao: "Visualizar", id: guid(), metodo: visualizarCotacaoClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editarCotacao, duplicarCotacao, enviarEmailCotacao, visualizarCotacao] };

    _gridCotacao = new GridView(_pesquisaCotacao.Pesquisar.idGrid, "CotacaoCompra/Pesquisa", _pesquisaCotacao, menuOpcoes, null);
    _gridCotacao.CarregarGrid();
}

function editarCotacaoClick(cotacaoGrid) {
    if (cotacaoGrid.Situacao == EnumSituacaoCotacao.AguardandoRetorno || cotacaoGrid.Situacao == EnumSituacaoCotacao.Aberto) {
        LimparCamposCotacaoCompra();
        _cotacaoCompra.Codigo.val(cotacaoGrid.Codigo);

        BuscarPorCodigo(_cotacaoCompra, "CotacaoCompra/BuscarPorCodigo", function (arg) {

            RegarregarGridMercadorias();
            RegarregarGridRetornos();
            RegarregarGridFornecedores();
            RegarregarGridRetornoProdutoFornecedor();

            _cotacaoCompra.Numero.enable = false;
            _cotacaoCompra.ValorTotal.enable(false);
            _modalCotacaoCompra.show();
        }, null);
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "O status da Cotação não permite a alteração.");
    }
}

function duplicarCotacaoClick(cotacaoGrid) {
    LimparCamposCotacaoCompra();
    _cotacaoCompra.Codigo.val(cotacaoGrid.Codigo);

    BuscarPorCodigo(_cotacaoCompra, "CotacaoCompra/DuplicarPorCodigo", function (arg) {
        _cotacaoCompra.Codigo.val(guid());

        RegarregarGridMercadorias();
        RegarregarGridRetornos();
        RegarregarGridFornecedores();
        RegarregarGridRetornoProdutoFornecedor();

        _cotacaoCompra.Numero.enable = false;
        _cotacaoCompra.ValorTotal.enable(false);
        _modalCotacaoCompra.show();
    }, null);
}

function enviarEmailCotacaoClick(cotacaoGrid) {
    if (cotacaoGrid.Situacao == EnumSituacaoCotacao.AguardandoRetorno) {
        var data = { Codigo: cotacaoGrid.Codigo };
        executarReST("CotacaoCompra/EnviarPorEmail", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    BuscarProcessamentosPendentes();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório/e-mail está sendo gerado.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Altere o status para Aguardando Retorno antes de enviar por e-mail.");
    }
}

function visualizarCotacaoClick(cotacaoGrid) {
    LimparCamposCotacaoCompra();
    _cotacaoCompra.Codigo.val(cotacaoGrid.Codigo);

    BuscarPorCodigo(_cotacaoCompra, "CotacaoCompra/BuscarPorCodigo", function (arg) {
        RegarregarGridMercadorias();
        RegarregarGridRetornos();
        RegarregarGridFornecedores();
        RegarregarGridRetornoProdutoFornecedor();
        EnableCamposCotacaoCompra(false);
        _grudCotacaoCompra.Salvar.visible(false);
        _modalCotacaoCompra.show();
    }, null);
}

function carregarConteudosCotacaoHTML(calback) {
    $.get("Content/Static/Compras/Cotacao.html?dyn=" + guid(), function (data) {
        $("#ModaisCotacao").html(data);
        calback();
    });
}