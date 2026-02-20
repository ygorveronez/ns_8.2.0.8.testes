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


var _gridLoteEscrituracao;
var _loteEscrituracao;
var _CRUDLoteEscrituracao;
var _pesquisaLoteEscrituracao;

var _situacaoLoteEscrituracao = [
    { text: "Todas", value: EnumSituacaoLoteEscrituracao.Todas },
    { text: "Em Criação", value: EnumSituacaoLoteEscrituracao.EmCriacao },
    { text: "Ag. Integração", value: EnumSituacaoLoteEscrituracao.AgIntegracao },
    { text: "Falha na Integração", value: EnumSituacaoLoteEscrituracao.FalhaIntegracao },
    { text: "Finalizado", value: EnumSituacaoLoteEscrituracao.Finalizado }
];

var LoteEscrituracao = function () {
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLoteEscrituracao.Todas), def: EnumSituacaoLoteEscrituracao.Todas, text: "Situação: " });
    this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoLoteEscrituracao.Todas), options: _situacaoLoteEscrituracao, def: EnumSituacaoLoteEscrituracao.Todas, text: "Situação: " });
}

var CRUDLoteEscrituracao = function () {
    this.Limpar = PropertyEntity({ eventClick: limparLoteEscrituracaoClick, type: types.event, text: "Limpar (Gerar Nova LoteEscrituracao)", idGrid: guid(), visible: ko.observable(false) });
}

var PesquisaLoteEscrituracao = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Numero = PropertyEntity({ text: "Número Lote:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroDOC = PropertyEntity({ text: "Número NF-e:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Prestação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLoteEscrituracao.Todas), options: _situacaoLoteEscrituracao, def: EnumSituacaoLoteEscrituracao.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLoteEscrituracao.CarregarGrid();
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

function loadLoteEscrituracao() {
    _loteEscrituracao = new LoteEscrituracao();
    HeaderAuditoria("LoteEscrituracao", _loteEscrituracao);

    _CRUDLoteEscrituracao = new CRUDLoteEscrituracao();
    KoBindings(_CRUDLoteEscrituracao, "knockoutCRUD");

    _pesquisaLoteEscrituracao = new PesquisaLoteEscrituracao();
    KoBindings(_pesquisaLoteEscrituracao, "knockoutPesquisaLoteEscrituracao", false, _pesquisaLoteEscrituracao.Pesquisar.id);

    loadEtapasLoteEscrituracao();
    loadSelecaoDocumentos();

    BuscarHTMLINtegracaoLoteEscrituracao();
    // Inicia as buscas
    new BuscarTransportadores(_pesquisaLoteEscrituracao.Empresa);
    new BuscarCargas(_pesquisaLoteEscrituracao.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_pesquisaLoteEscrituracao.Ocorrencia);
    new BuscarClientes(_pesquisaLoteEscrituracao.Tomador);
    new BuscarLocalidades(_pesquisaLoteEscrituracao.LocalidadePrestacao);
    new BuscarFilial(_pesquisaLoteEscrituracao.Filial);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaLoteEscrituracao.Filial.visible(false);
        _pesquisaLoteEscrituracao.Empresa.text("Empresa/Filial:");
    }

    BuscarLoteEscrituracao();
}


function limparLoteEscrituracaoClick(e, sender) {
    LimparCamposLoteEscrituracao();
    GridSelecaoDocumentos();
}


//*******MÉTODOS*******
function BuscarLoteEscrituracao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLoteEscrituracao, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridLoteEscrituracao = new GridView(_pesquisaLoteEscrituracao.Pesquisar.idGrid, "LoteEscrituracao/Pesquisa", _pesquisaLoteEscrituracao, menuOpcoes);
    _gridLoteEscrituracao.CarregarGrid();
}

function editarLoteEscrituracao(itemGrid) {
    // Limpa os campos
    LimparCamposLoteEscrituracao();

    // Esconde filtros
    _pesquisaLoteEscrituracao.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarLoteEscrituracaoPorCodigo(itemGrid.Codigo);
}

function BuscarLoteEscrituracaoPorCodigo(codigo, cb) {
    executarReST("LoteEscrituracao/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            // -- LoteEscrituracao Manula
            EditarLoteEscrituracao(arg.Data);
            // -- Selecao de Documentos
            EditarSelecaoDocumentos(arg.Data);
            SetarEtapasLoteEscrituracao();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarLoteEscrituracao(data) {
    _loteEscrituracao.Situacao.val(data.Situacao);
    _loteEscrituracao.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _CRUDLoteEscrituracao.Limpar.visible(true);
}

function LimparCamposLoteEscrituracao() {
    LimparCampos(_loteEscrituracao);
    _CRUDLoteEscrituracao.Limpar.visible(false);
    _loteEscrituracao.Situacao.val(EnumSituacaoLoteEscrituracao.Todas);
    SetarEtapasLoteEscrituracao();
    LimparCamposSelecaoDocumentos();
    $("#" + _etapaLoteEscrituracao.Etapa1.idTab).click();
}