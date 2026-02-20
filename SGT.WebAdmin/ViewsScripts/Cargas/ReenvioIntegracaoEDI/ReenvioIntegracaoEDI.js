/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _reenvioIntegracaoEDI;
var _pesquisaReenvioIntegracaoEDI;
var _gridReenvioIntegracaoEDI;
var _gridCarga;
var _gridEDI;

var ReenvioIntegracaoEDI = function () {
    // Codigo da entidade 
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Resumo = PropertyEntity({ val: ko.observable(false), def: false });
    this.Usuario = PropertyEntity({ val: ko.observable(""), text: "Usuário: " });
    this.DataEnvio = PropertyEntity({ val: ko.observable(""), text: "Data Envio: " });

    this.AdicionarCarga = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.Cargas = PropertyEntity({ getType: typesKnockout.basicTable, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.Cargas.val.subscribe(function () {
        _gridCarga.CarregarGrid(_reenvioIntegracaoEDI.Cargas.val());
    });

    this.AdicionarLayout = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.LayoutsEDI = PropertyEntity({ getType: typesKnockout.basicTable, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.LayoutsEDI.val.subscribe(function () {
        _gridEDI.CarregarGrid(_reenvioIntegracaoEDI.LayoutsEDI.val());
    });

    // CRUD
    this.Enviar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Enviar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.ImportarPlanilha = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-primary botaoDentroSmartAdmimForm",
        UrlImportacao: "ReenvioIntegracaoEDI/ImportarParaProcessar",
        UrlConfiguracao: "ReenvioIntegracaoEDI/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O021_ReenvioIntegracaoEDI,
        CallbackImportacao: function (arg) {
            if (arg.Data.Retorno != null)
                _reenvioIntegracaoEDI.Cargas.val(arg.Data.Retorno);
        },
        FecharModalSeSucesso: true
    });
}

var PesquisaReenvioIntegracaoEDI = function () {
    this.Carga = PropertyEntity({ val: ko.observable(""), def: "", text: "Carga:" });
    this.LayoutEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Layout EDI:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Usuário:", idBtnSearch: guid() });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridReenvioIntegracaoEDI.CarregarGrid();
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
function loadReenvioIntegracaoEDI() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaReenvioIntegracaoEDI = new PesquisaReenvioIntegracaoEDI();
    KoBindings(_pesquisaReenvioIntegracaoEDI, "knockoutPesquisaReenvioIntegracaoEDI", false, _pesquisaReenvioIntegracaoEDI.Pesquisar.id);

    // Instancia objeto principal
    _reenvioIntegracaoEDI = new ReenvioIntegracaoEDI();
    KoBindings(_reenvioIntegracaoEDI, "knockoutReenvioIntegracaoEDI");

    HeaderAuditoria("ReenvioIntegracaoEDI", _reenvioIntegracaoEDI);

    // Inicia busca
    BuscarReenvioIntegracaoEDI();

    LoadGridEDI();
    LoadGridCargas();

    // Instancia buscas
    new BuscarLayoutsEDI(_pesquisaReenvioIntegracaoEDI.LayoutEDI);
    new BuscarFuncionario(_pesquisaReenvioIntegracaoEDI.Usuario);

    new BuscarLayoutsEDI(_reenvioIntegracaoEDI.AdicionarLayout, null, null, null, _gridEDI); 
    new BuscarCargas(_reenvioIntegracaoEDI.AdicionarCarga, null, null, null, null, null, _gridCarga);
}

function LoadGridEDI() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: RemoverEDIClick
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "65%", className: "text-align-left" },
        { data: "DescricaoTipo", title: "Tipo de Layout", width: "25%", className: "text-align-left" }
    ];

    // Grid
    _gridEDI = new BasicDataTable(_reenvioIntegracaoEDI.LayoutsEDI.idGrid, header, menuOpcoes, null, null, 10);
    _gridEDI.CarregarGrid([]);
    _reenvioIntegracaoEDI.LayoutsEDI.basicTable = _gridEDI;
}

function LoadGridCargas() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: RemoverCargaClick
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCargaEmbarcador", title: "Nº da Carga", width: "20%", className: "text-align-left" },
        { data: "Transportador", title: "Transportador", width: "20%", className: "text-align-left" },
        { data: "Filial", title: "Filial", width: "20%", className: "text-align-left" },
        { data: "Veiculo", title: "Veículo", width: "20%", className: "text-align-left" }
    ];

    // Grid
    _gridCarga = new BasicDataTable(_reenvioIntegracaoEDI.Cargas.idGrid, header, menuOpcoes, null, null, 10);
    _gridCarga.CarregarGrid([]);
    _reenvioIntegracaoEDI.Cargas.basicTable = _gridCarga;
}

function adicionarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Tem certeza que deseja processar as cargas selecionadas?", function () {
        Salvar(_reenvioIntegracaoEDI, "ReenvioIntegracaoEDI/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridReenvioIntegracaoEDI.CarregarGrid();
                    LimparCamposReenvioIntegracaoEDI();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function RemoverEDIClick(data) {
    var dataGrid = _reenvioIntegracaoEDI.LayoutsEDI.val();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _reenvioIntegracaoEDI.LayoutsEDI.val(dataGrid);
}

function RemoverCargaClick(data) {
    var dataGrid = _reenvioIntegracaoEDI.Cargas.val();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _reenvioIntegracaoEDI.Cargas.val(dataGrid);
}

function cancelarClick(e) {
    LimparCamposReenvioIntegracaoEDI();
}

function editarReenvioIntegracaoEDIClick(itemGrid) {
    // Limpa os campos
    LimparCamposReenvioIntegracaoEDI();

    // Seta o codigo do objeto
    _reenvioIntegracaoEDI.Codigo.val(itemGrid.Codigo);
    
    // Busca informacoes para edicao
    BuscarPorCodigo(_reenvioIntegracaoEDI, "ReenvioIntegracaoEDI/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaReenvioIntegracaoEDI.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _reenvioIntegracaoEDI.AdicionarLayout.visible(false);
                _reenvioIntegracaoEDI.AdicionarCarga.visible(false);
                _reenvioIntegracaoEDI.Enviar.visible(false);
                _reenvioIntegracaoEDI.ImportarPlanilha.visible(false);

                _reenvioIntegracaoEDI.Resumo.val(true);

                _gridCarga.DesabilitarOpcoes();
                _gridEDI.DesabilitarOpcoes();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarReenvioIntegracaoEDI() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarReenvioIntegracaoEDIClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridReenvioIntegracaoEDI = new GridView(_pesquisaReenvioIntegracaoEDI.Pesquisar.idGrid, "ReenvioIntegracaoEDI/Pesquisa", _pesquisaReenvioIntegracaoEDI, menuOpcoes, null);
    _gridReenvioIntegracaoEDI.CarregarGrid();
}

function LimparCamposReenvioIntegracaoEDI() {
    _reenvioIntegracaoEDI.AdicionarLayout.visible(true);
    _reenvioIntegracaoEDI.AdicionarCarga.visible(true);
    _reenvioIntegracaoEDI.Enviar.visible(true);
    _reenvioIntegracaoEDI.ImportarPlanilha.visible(true);

    _gridCarga.HabilitarOpcoes();
    _gridEDI.HabilitarOpcoes();

    LimparCampos(_reenvioIntegracaoEDI);
}