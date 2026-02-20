/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Enumeradores/EnumResponsavelAvaria.js" />
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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Filial.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaLiberarPedidos;
var _gridLiberarPedidos;


var _statusPesquisa = [
    { text: "AMBOS", value: -1 },
    { text: "NÃO", value: 0 },
    { text: "SIM", value: 1 }
];

var PesquisaLiberarPedidos = function () {
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), required: true });
    this.Hora = PropertyEntity({ text: "Hora limite pedido: ", getType: typesKnockout.time, type: types.time, required: ko.observable(true), visible: ko.observable(true) });
    this.Liberado = PropertyEntity({ text: "Liberado: ", val: ko.observable(-1), options: _statusPesquisa, def: -1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarClick();
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

    this.LiberarTodosPedidos = PropertyEntity({
        eventClick: function (e) {
            liberarTodosPedidosClick();
        }, type: types.event, text: "Liberar todos", idGrid: guid(), visible: ko.observable(false)
    });
}

//*******EVENTOS*******
function loadLiberarPedidos() {
    // Instancia pesquisa
    _pesquisaLiberarPedidos = new PesquisaLiberarPedidos();
    KoBindings(_pesquisaLiberarPedidos, "knockoutPesquisaLiberarPedidos", false, _pesquisaLiberarPedidos.Pesquisar.id);

    new BuscarFilial(_pesquisaLiberarPedidos.Filial);

    // Inicia busca
    buscarLiberarPedidos();
}

function pesquisarClick() {
    if (ValidarCamposObrigatorios(_pesquisaLiberarPedidos)) {
        executarReST("Pedido/ObterPedidosLiberarAgendamentoPortalRetira", RetornarObjetoPesquisa(_pesquisaLiberarPedidos), function (arg) {
            if (arg.Success) {
                setarRegistrosDataTable(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function liberarPedidoClick(item) {
    var codigos = [];
    codigos.push(item.Codigo);
    liberarPedidos(codigos);
}

function liberarTodosPedidosClick() {
    var codigosPedidos = _gridLiberarPedidos.BuscarRegistros().map(function (item) {
        return item.Codigo;
    });
    setTimeout(function () {
        liberarPedidos(codigosPedidos);;
    }, 200);    
}

function liberarPedidos(codigos) {
    var data = {
        Codigos: JSON.stringify(codigos)
    };
    executarReST("Pedido/LiberarPedidos", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                pesquisarClick();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}


//*******MÉTODOS*******
function buscarLiberarPedidos() {

    // Opcoes
    var editar = { descricao: "Liberar Pedido", id: "clasEditar", evento: "onclick", metodo: liberarPedidoClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 15,
        opcoes: [editar]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DT_RowId", visible: false },
        { data: "Filial", title: "Filial", width: "40%" },
        { data: "NumeroPedidoEmbarcador", title: "Nº Pedido", width: "20%" },
        { data: "DataCriacao", title: "Data", width: "20%" },
        { data: "Liberado", title: "Liberado", width: "10%" },
    ];

    _gridLiberarPedidos = new BasicDataTable(_pesquisaLiberarPedidos.Pesquisar.idGrid, header, menuOpcoes, null, null, 20);
    setarRegistrosDataTable([]);
}

function setarRegistrosDataTable(registros) {
    _gridLiberarPedidos.CarregarGrid(registros);
    var pendentes = registros.filter(function (item) {
        return item.Liberado != 'SIM';
    })
    setTimeout(function () {
        _pesquisaLiberarPedidos.LiberarTodosPedidos.visible(pendentes.length > 0);
    }, 200);    
}
