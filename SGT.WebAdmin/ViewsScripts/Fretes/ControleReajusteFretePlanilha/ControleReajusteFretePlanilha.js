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


var _gridControleReajusteFretePlanilha;
var _pesquisaControleReajusteFretePlanilha;

var _SituacaoControleReajusteFretePlanilha = [
    { text: "Todas", value: EnumSituacaoControleReajusteFretePlanilha.Todas },
    { text: "Ag. Aprovação", value: EnumSituacaoControleReajusteFretePlanilha.AgAprovacao },
    { text: "Aprovado", value: EnumSituacaoControleReajusteFretePlanilha.Aprovado },
    { text: "Finalizado", value: EnumSituacaoControleReajusteFretePlanilha.Finalizado },
    { text: "Cancelado", value: EnumSituacaoControleReajusteFretePlanilha.Cancelado },
    { text: "Rejeitado", value: EnumSituacaoControleReajusteFretePlanilha.Rejeitado },
    { text: "Sem Regra", value: EnumSituacaoControleReajusteFretePlanilha.SemRegra }
];

var PesquisaControleReajusteFretePlanilha = function () {
    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable(""), def: "", enable: false, getType: typesKnockout.int });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true), required: false });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true), required: false });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true), required: false });
    this.SituacaoControleReajusteFretePlanilha = PropertyEntity({ val: ko.observable(EnumSituacaoControleReajusteFretePlanilha.Todas), options: _SituacaoControleReajusteFretePlanilha, text: "Situação: ", def: EnumSituacaoControleReajusteFretePlanilha.Todas });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleReajusteFretePlanilha.CarregarGrid();
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
}


//*******EVENTOS*******
function loadControleReajusteFretePlanilha() {
    _pesquisaControleReajusteFretePlanilha = new PesquisaControleReajusteFretePlanilha();
    KoBindings(_pesquisaControleReajusteFretePlanilha, "knockoutPesquisaControleReajusteFretePlanilha", false, _pesquisaControleReajusteFretePlanilha.Pesquisar.id);

    loadCRUD();
    loadEtapas();
    loadPlanilha();
    loadAprovacao();
    loadFinalizacao();

    // Inicia as buscas
    new BuscarTransportadores(_pesquisaControleReajusteFretePlanilha.Empresa);
    new BuscarFilial(_pesquisaControleReajusteFretePlanilha.Filial);
    new BuscarTiposOperacao(_pesquisaControleReajusteFretePlanilha.TipoOperacao);




    BuscarControleReajusteFretePlanilha();

    LimparCamposControle();
}


//*******MÉTODOS*******
function BuscarControleReajusteFretePlanilha() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarControleReajusteFretePlanilhaClick, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridControleReajusteFretePlanilha = new GridView(_pesquisaControleReajusteFretePlanilha.Pesquisar.idGrid, "ControleReajusteFretePlanilha/Pesquisa", _pesquisaControleReajusteFretePlanilha, menuOpcoes);
    _gridControleReajusteFretePlanilha.CarregarGrid();
}

function editarControleReajusteFretePlanilhaClick(itemGrid) {
    // Limpa os campos
    LimparCamposControle();

    // Esconde filtros
    _pesquisaControleReajusteFretePlanilha.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarControleReajusteFretePlanilhaPorCodigo(itemGrid.Codigo);
}

function BuscarControleReajusteFretePlanilhaPorCodigo(codigo, cb) {
    executarReST("ControleReajusteFretePlanilha/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            AlterarBootesCRUD(arg);

            PreencheDadosPlanilhaReajuste(arg);

            ListarAprovacoes(arg.Data);

            EditarFinalizacao(arg);

            SetarEtapa();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function LimparCamposControle() {
    LimparCampos(_controleReajusteFretePlanilha);

    _controleReajusteFretePlanilha.Planilha.file.value = null;
    _controleReajusteFretePlanilha.Planilha.val("");
    _controleReajusteFretePlanilha.Planilha.name("");

    ResetarBotoesCRUD();

    SetarEtapaInicio();

    limparCamposPlanilha();
    //LimparCamposDadosControleReajusteFretePlanilha();
}