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
/// <reference path="../../Consultas/CIOT.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCIOT.js" />
/// <reference path="EtapaFechamentoAgregado.js" />
/// <reference path="Etapa1SelecaoCIOT.js" />
/// <reference path="Etapa2Consolidacao.js" />
/// <reference path="Etapa3Integracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _fechamentoAgregado;
var _pesquisaFechamentoAgregado;
var _gridFechamentoAgregado;
var _CRUDAFechamentoAgregado;

var PesquisaFechamentoAgregado = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.CIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CIOT:", idBtnSearch: guid(), visible: true });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarFechamentoAgregado();
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

var FechamentoAgregado = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Numero = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
}

var CRUDFechamentoAgregado = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparFechamentoAgregadoClick, type: types.event, text: "Limpar/Cancelar", idGrid: guid(), visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadFechamentoAgregado() {
    _fechamentoAgregado = new FechamentoAgregado();

    _pesquisaFechamentoAgregado = new PesquisaFechamentoAgregado();
    KoBindings(_pesquisaFechamentoAgregado, "knockoutPesquisaFechamentoAgregado", false, _pesquisaFechamentoAgregado.Pesquisar.id);

    _CRUDAFechamentoAgregado = new CRUDFechamentoAgregado();
    KoBindings(_CRUDAFechamentoAgregado, "knockoutCRUD");

    LoadEtapasFechamentoAgregado();
    LoadEtapaSelecaoCIOT();
    LoadEtapaConsolidacao();
    LoadEtapaIntegracao();
    LoadFechamentoAgregadoDetalhes();
    LoadFechamentoContratoFreteAcrescimoDesconto();
    LoadAdicionarFechamentoContratoFreteAcrescimoDesconto();

    // Busca componentes pesquisa
    BuscarFuncionario(_pesquisaFechamentoAgregado.Usuario);
    BuscarCIOT(_pesquisaFechamentoAgregado.CIOT, null, null, [EnumSituacaoCIOT.Aberto, EnumSituacaoCIOT.Encerrado], [EnumTipoProprietarioVeiculo.TACAgregado]);

    // Busca
    BuscarFechamentoAgregado();
}

function LimparFechamentoAgregadoClick(e, sender) {
    LimparCamposFechamentoAgregado();
    SetarEtapasFechamentoAgregado();
    GridSelecaoCIOT();
    GridIntegracao();
}

//*******MÉTODOS*******

function BuscarFechamentoAgregado() {
    //-- Cabecalho
    let editar = {
        descricao: "Editar",
        id: guid(),
        evento: "onclick",
        metodo: EditarFechamentoAgregado,
        tamanho: "10",
        icone: ""
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    let configExportacao = {
        url: "FechamentoAgregado/ExportarPesquisa",
        titulo: "Fechamento de Agregado"
    };

    _gridFechamentoAgregado = new GridView(_pesquisaFechamentoAgregado.Pesquisar.idGrid, "FechamentoAgregado/Pesquisa", _pesquisaFechamentoAgregado, menuOpcoes, null, 25, null, null, null, null, null, null, configExportacao);
    _gridFechamentoAgregado.CarregarGrid();
}

function EditarFechamentoAgregado(itemGrid) {
    // Limpa os campos
    LimparCamposFechamentoAgregado();

    // Esconde filtros
    _pesquisaFechamentoAgregado.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarFechamentoAgregadoPorCodigo(itemGrid.Codigo, false);
}

function BuscarFechamentoAgregadoPorCodigo(codigo, recarregarCampos) {
    executarReST("FechamentoAgregado/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            if (recarregarCampos) {
                CarregarDadosCampos(arg.Data)
            }
            else {
                CarregarDadosFechamentoAgregado(arg.Data);
                SetarEtapasFechamentoAgregado();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function LimparCamposFechamentoAgregado() {
    LimparCampos(_fechamentoAgregado);
    LimparCampos(_etapa1SelecaoCIOT);
    _fechamentoAgregado.Codigo.val(0);
    _CRUDAFechamentoAgregado.Limpar.visible(false);
    _etapa1SelecaoCIOT.Adicionar.visible(true);
    _etapa1SelecaoCIOT.Transportador.visible(true);
    _etapa2Consolidacao.EncerrarCIOTAgregado.visible(false);
}