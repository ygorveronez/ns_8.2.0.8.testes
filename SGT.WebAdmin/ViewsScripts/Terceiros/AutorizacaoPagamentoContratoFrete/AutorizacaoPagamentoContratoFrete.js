/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="Etapa.js" />
/// <reference path="EtapaSelecaoContratoFrete.js" />
/// <reference path="EtapaPagamentoCIOT.js" />
/// <reference path="EtapaPagamentoIntegracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _autorizacaoPagamentoContratoFrete;
var _pesquisaAutorizacaoPagamentoContratoFrete;
var _gridAutorizacaoPagamentoContratoFrete;
var _CRUDAutorizacaoPagamentoContratoFrete;

var PesquisaAutorizacaoPagamentoContratoFrete = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.DataCiotInicial = PropertyEntity({ text: "Data CIOT Inicial: ", getType: typesKnockout.date });
    this.DataCiotFinal = PropertyEntity({ text: "Data CIOT Final: ", getType: typesKnockout.date });
    this.Operadora = PropertyEntity({ val: ko.observable(""), options: EnumOperadoraCIOT.ObterOpcoesPesquisa(), def: "", text: "Operadora:" });
    this.TransportadorTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), issue: 56 });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarAutorizacaoPagamentoContratoFrete();
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
}

var AutorizacaoPagamentoContratoFrete = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Numero = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ContratoFretePagamentoCIOTIntegracaoPendente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(0), def: 0 });
    this.ContratoFretePagamentoCONTRATOIntegracaoPendente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(0), def: 0 });
}

var CRUDAutorizacaoPagamentoContratoFrete = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparAutorizacaoPagamentoContratoFreteClick, type: types.event, text: "Limpar/Cancelar", idGrid: guid(), visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAutorizacaoPagamentoContratoFrete() {
    _autorizacaoPagamentoContratoFrete = new AutorizacaoPagamentoContratoFrete();

    _pesquisaAutorizacaoPagamentoContratoFrete = new PesquisaAutorizacaoPagamentoContratoFrete();
    KoBindings(_pesquisaAutorizacaoPagamentoContratoFrete, "knockoutPesquisaAutorizacaoPagamentoContratoFrete", false, _pesquisaAutorizacaoPagamentoContratoFrete.Pesquisar.id);

    _CRUDAutorizacaoPagamentoContratoFrete = new CRUDAutorizacaoPagamentoContratoFrete();
    KoBindings(_CRUDAutorizacaoPagamentoContratoFrete, "knockoutCRUD");

    $modalAutorizacaoPagamentoContratoFrete = $("#divModalAutorizacaoPagamentoContratoFrete");
    LoadEtapasAutorizacaoPagamentoContratoFrete();
    LoadEtapaSelecaoContratoFrete();
    LoadEtapaPagamentoCIOT();
    LoadEtapaPagamentoIntegracao();

    // Busca componentes pesquisa
    BuscarFuncionario(_pesquisaAutorizacaoPagamentoContratoFrete.Usuario);
    BuscarTransportadores(_pesquisaAutorizacaoPagamentoContratoFrete.Empresa);
    BuscarCargas(_pesquisaAutorizacaoPagamentoContratoFrete.Carga);
    BuscarClientes(_pesquisaAutorizacaoPagamentoContratoFrete.TransportadorTerceiro);

    // Busca
    BuscarAutorizacaoPagamentoContratoFrete();
}

function LimparAutorizacaoPagamentoContratoFreteClick(e, sender) {
    LimparCamposAutorizacaoPagamentoContratoFrete();
    SetarEtapasAutorizacaoPagamentoContratoFrete();
    GridSelecaoContratoFrete();
    GridPagamentoCIOT();
    GridPagamentoIntegracao();
}

//*******MÉTODOS*******

function BuscarAutorizacaoPagamentoContratoFrete() {
    //-- Cabecalho
    let editar = {
        descricao: "Editar",
        id: guid(),
        evento: "onclick",
        metodo: EditarAutorizacaoPagamentoContratoFrete,
        tamanho: "10",
        icone: ""
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    let configExportacao = {
        url: "AutorizacaoPagamentoContratoFrete/ExportarPesquisa",
        titulo: "Autorização Pagamento ContratoFrete"
    };

    _gridAutorizacaoPagamentoContratoFrete = new GridView(_pesquisaAutorizacaoPagamentoContratoFrete.Pesquisar.idGrid, "AutorizacaoPagamentoContratoFrete/Pesquisa", _pesquisaAutorizacaoPagamentoContratoFrete, menuOpcoes, null, 25, null, null, null, null, null, null, configExportacao);
    _gridAutorizacaoPagamentoContratoFrete.CarregarGrid();
}

function EditarAutorizacaoPagamentoContratoFrete(itemGrid) {
    // Limpa os campos
    LimparCamposAutorizacaoPagamentoContratoFrete();

    // Esconde filtros
    _pesquisaAutorizacaoPagamentoContratoFrete.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarAutorizacaoPagamentoContratoFretePorCodigo(itemGrid.Codigo);
}

function BuscarAutorizacaoPagamentoContratoFretePorCodigo(codigo) {
    executarReST("AutorizacaoPagamentoContratoFrete/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            _autorizacaoPagamentoContratoFrete.Codigo.val(arg.Data.Codigo);
            _autorizacaoPagamentoContratoFrete.Numero.val(arg.Data.Numero);
            _autorizacaoPagamentoContratoFrete.ContratoFretePagamentoCIOTIntegracaoPendente.val(arg.Data.ContratoFretePagamentoCIOTIntegracaoPendente);
            _autorizacaoPagamentoContratoFrete.ContratoFretePagamentoCONTRATOIntegracaoPendente.val(arg.Data.ContratoFretePagamentoCONTRATOIntegracaoPendente);
            _CRUDAutorizacaoPagamentoContratoFrete.Limpar.visible(true);
            EditarSelecaoContratosFrete(arg.Data);
            SetarEtapasAutorizacaoPagamentoContratoFrete();

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function LimparCamposAutorizacaoPagamentoContratoFrete() {
    LimparCampos(_autorizacaoPagamentoContratoFrete);
    _autorizacaoPagamentoContratoFrete.Codigo.val(0);
    _CRUDAutorizacaoPagamentoContratoFrete.Limpar.visible(false);
    _etapaSelecaoContratoFrete.Filtro.visible(true);
    _etapaSelecaoContratoFrete.Adicionar.visible(true);
}