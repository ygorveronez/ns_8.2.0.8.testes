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

var _ajudanteCarga;
var _pesquisaAjudanteCarga;
var _gridAjudanteCarga;
var _gridAjudantes;

var AjudanteCarga = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Carga = PropertyEntity({ text: "*Carga:", required: true, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false) });
    this.Ajudantes = PropertyEntity({ type: types.map, required: false, text: "Informar Ajudante", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.ListaAjudantes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.NomesAjudantes = PropertyEntity();

    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaAjudanteCarga = function () {
    this.NumeroCarga = PropertyEntity({ text: "Carga:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoIntegracao = PropertyEntity({ val: ko.observable(""), def: "", visible: false });
    this.Status = PropertyEntity({ val: ko.observable(""), def: "", visible: false });
    this.StatusDiff = PropertyEntity({ val: ko.observable(""), def: "", visible: false });
    this.PossuiDTNatura = PropertyEntity({ val: ko.observable(""), def: "", visible: false });
    this.PermiteCTeComplementar = PropertyEntity({ val: ko.observable(""), def: "", visible: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAjudanteCarga.CarregarGrid();
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
function loadAjudanteCarga() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaAjudanteCarga = new PesquisaAjudanteCarga();
    KoBindings(_pesquisaAjudanteCarga, "knockoutPesquisaAjudanteCarga", false, _pesquisaAjudanteCarga.Pesquisar.id);

    // Instancia objeto principal
    _ajudanteCarga = new AjudanteCarga();
    KoBindings(_ajudanteCarga, "knockoutAjudanteCarga");

    // Inicia busca
    buscarAjudanteCarga();

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverAjudanteClick(_ajudanteCarga.Ajudantes, data)
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [{ data: "Codigo", visible: false },
    { data: "CPF", title: "CPF", width: "20%", className: "text-align-left" },
    { data: "Nome", title: "Nome", width: "60%", className: "text-align-left" }
    ];

    _gridAjudantes = new BasicDataTable(_ajudanteCarga.Ajudantes.idGrid, header, menuOpcoes);
    _ajudanteCarga.Ajudantes.basicTable = _gridAjudantes;

    new BuscarMotorista(_ajudanteCarga.Ajudantes, RetornoInserirAjudante, _gridAjudantes, null, null, EnumSituacaoColaborador.Trabalhando);
    RecarregarListaAjudantes();
}

function RetornoInserirAjudante(data) {
    if (data != null) {
        var dataGrid = _gridAjudantes.BuscarRegistros();

        for (var i = 0; i < data.length; i++) {
            var obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.CPF = data[i].CPF;
            obj.Nome = data[i].Nome;

            dataGrid.push(obj);
        }
        _gridAjudantes.CarregarGrid(dataGrid);
    }
}

function atualizarClick(e, sender) {
    preencherListaAjudante();

    Salvar(_ajudanteCarga, "AjudanteCarga/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridAjudanteCarga.CarregarGrid();
                limparCamposAjudanteCarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposAjudanteCarga();
}

function editarAjudanteCargaClick(itemGrid) {
    // Limpa os campos
    limparCamposAjudanteCarga();

    // Seta o codigo do objeto
    _ajudanteCarga.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_ajudanteCarga, "AjudanteCarga/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaAjudanteCarga.ExibirFiltros.visibleFade(false);
                RecarregarListaAjudantes();

                // Alternas os campos de CRUD
                _ajudanteCarga.Atualizar.visible(true);
                _ajudanteCarga.Cancelar.visible(true);
                _ajudanteCarga.Carga.enable(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function RemoverAjudanteClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o ajudante " + sender.Nome + "?", function () {
        var ajudanteGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < ajudanteGrid.length; i++) {
            if (sender.Codigo == ajudanteGrid[i].Codigo) {
                ajudanteGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(ajudanteGrid);
    });
}

//*******MÉTODOS*******
function buscarAjudanteCarga() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAjudanteCargaClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridAjudanteCarga = new GridView(_pesquisaAjudanteCarga.Pesquisar.idGrid, "Carga/Pesquisa", _pesquisaAjudanteCarga, menuOpcoes, null);
    _gridAjudanteCarga.CarregarGrid();
}

function RecarregarListaAjudantes() {
    var cont = 0;
    var total = 0;
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_ajudanteCarga.ListaAjudantes.val())) {
        $.each(_ajudanteCarga.ListaAjudantes.val(), function (i, ajudante) {
            var obj = new Object();

            obj.Codigo = ajudante.Codigo;
            obj.CPF = ajudante.CPF;
            obj.Nome = ajudante.Nome;

            data.push(obj);
        });
    }
    _gridAjudantes.CarregarGrid(data);
}

function preencherListaAjudante() {
    _ajudanteCarga.ListaAjudantes.list = new Array();

    var Ajudantes = new Array();

    $.each(_ajudanteCarga.Ajudantes.basicTable.BuscarRegistros(), function (i, ajudante) {
        Ajudantes.push({ Ajudante: ajudante });
    });

    _ajudanteCarga.ListaAjudantes.val(JSON.stringify(Ajudantes))
}

function limparCamposAjudanteCarga() {
    _ajudanteCarga.Atualizar.visible(false);
    _ajudanteCarga.Cancelar.visible(false);
    LimparCampos(_ajudanteCarga);

    _ajudanteCarga.Carga.enable(false);
    RecarregarListaAjudantes();
}