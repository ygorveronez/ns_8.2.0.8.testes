/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridFechamentoColetaProdutor;
var _fechamentoColetaProdutor;
var _CRUDFechamentoColetaProdutor;
var _pesquisaFechamentoColetaProdutor;

var _situacaoFechamentoColetaProdutor = [
    { text: "Todas", value: EnumSituacaoFechamentoColetaProdutor.Todas },
    { text: "Em Criação", value: EnumSituacaoFechamentoColetaProdutor.EmCriacao },
    { text: "Ag. Geração Carga", value: EnumSituacaoFechamentoColetaProdutor.AgGeracaoCarga },
    { text: "Finalizado", value: EnumSituacaoFechamentoColetaProdutor.Finalizado }
];

var FechamentoColetaProdutor = function () {
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoColetaProdutor.Todos), def: EnumSituacaoFechamentoColetaProdutor.Todos, text: "Situação: " });
    this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoColetaProdutor.Todos), options: _situacaoFechamentoColetaProdutor, def: EnumSituacaoFechamentoColetaProdutor.Todos, text: "Situação: " });
};

var CRUDFechamentoColetaProdutor = function () {
    this.Limpar = PropertyEntity({ eventClick: limparFechamentoColetaProdutorClick, type: types.event, text: "Limpar (Gerar Novo Fechamento)", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaFechamentoColetaProdutor = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Numero = PropertyEntity({ text: "Número:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.PreCarga = PropertyEntity({ text: "Pré Carga:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pedido:", idBtnSearch: guid(), visible: ko.observable(true) });
    
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoColetaProdutor.Todas), options: _situacaoFechamentoColetaProdutor, def: EnumSituacaoFechamentoColetaProdutor.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFechamentoColetaProdutor.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******

function loadFechamentoColetaProdutor() {
    _fechamentoColetaProdutor = new FechamentoColetaProdutor();
    HeaderAuditoria("FechamentoColetaProdutor", _fechamentoColetaProdutor);

    _CRUDFechamentoColetaProdutor = new CRUDFechamentoColetaProdutor();
    KoBindings(_CRUDFechamentoColetaProdutor, "knockoutCRUD");

    _pesquisaFechamentoColetaProdutor = new PesquisaFechamentoColetaProdutor();
    KoBindings(_pesquisaFechamentoColetaProdutor, "knockoutPesquisaFechamentoColetaProdutor", false, _pesquisaFechamentoColetaProdutor.Pesquisar.id);

    loadEtapasFechamentoColetaProdutor();
    loadSelecaoPedidos();

    //BuscarHTMLINtegracaoFechamentoColetaProdutor();
    // Inicia as buscas
    new BuscarTransportadores(_pesquisaFechamentoColetaProdutor.Empresa);
    new BuscarPedidos(_pesquisaFechamentoColetaProdutor.Pedido);
    new BuscarClientes(_pesquisaFechamentoColetaProdutor.Remetente);
    new BuscarLocalidades(_pesquisaFechamentoColetaProdutor.Origem);
    new BuscarFilial(_pesquisaFechamentoColetaProdutor.Filial);

    LoadCargasFechamento(function () {
        loadResumoFechamento();
        LoadCancelamentoFechamento();
        BuscarFechamentoColetaProdutor();
    }); 
}

function limparFechamentoColetaProdutorClick(e, sender) {
    LimparCamposFechamentoColetaProdutor();
    GridSelecaoPedidos();
}

//*******MÉTODOS*******
function BuscarFechamentoColetaProdutor() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFechamentoColetaProdutor, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridFechamentoColetaProdutor = new GridView(_pesquisaFechamentoColetaProdutor.Pesquisar.idGrid, "FechamentoColetaProdutor/Pesquisa", _pesquisaFechamentoColetaProdutor, menuOpcoes);
    _gridFechamentoColetaProdutor.CarregarGrid();
}

function editarFechamentoColetaProdutor(itemGrid) {
    // Limpa os campos
    LimparCamposFechamentoColetaProdutor();

    // Esconde filtros
    _pesquisaFechamentoColetaProdutor.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarFechamentoColetaProdutorPorCodigo(itemGrid.Codigo);
}

function BuscarFechamentoColetaProdutorPorCodigo(codigo, cb) {
    executarReST("FechamentoColetaProdutor/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            // -- FechamentoColetaProdutor Manula
            EditarFechamentoColetaProdutor(arg.Data);
            PreecherResumoFechamento(arg);
            // -- Selecao de Pedidos
            EditarSelecaoPedidos(arg.Data);
            SetarEtapasFechamentoColetaProdutor();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarFechamentoColetaProdutor(data) {
    _fechamentoColetaProdutor.Situacao.val(data.Situacao);
    _fechamentoColetaProdutor.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _CRUDFechamentoColetaProdutor.Limpar.visible(true);
}

function LimparCamposFechamentoColetaProdutor() {
    LimparCampos(_fechamentoColetaProdutor);
    _CRUDFechamentoColetaProdutor.Limpar.visible(false);
    _fechamentoColetaProdutor.Situacao.val(EnumSituacaoFechamentoColetaProdutor.Todos);
    SetarEtapasFechamentoColetaProdutor();
    SetarEtapaInicio();
    LimparCamposSelecaoPedidos();
    limparCamposCancelamentoFechamento();
    limparResumo();
}