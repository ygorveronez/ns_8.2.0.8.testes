/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="TabelaDiaria.js" />
/// <reference path="../../Consultas/Justificativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _periodo;
var _gridPeriodo;

var Periodo = function () {
    this.Periodos = PropertyEntity({ idGrid: guid(), type: types.local });

    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), visible: ko.observable(true), required: HandleRequiredComponente(this) });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, val: ko.observable("") });

    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0) });
    
    this.HoraInicial = PropertyEntity({ text: "Hora Inicial:", getType: typesKnockout.time });
    this.HoraFinal = PropertyEntity({ text: "Hora Final:", getType: typesKnockout.time });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, required: true });

    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarPeriodo, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarPeriodo, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirCamposCRUDPeriodo, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposCRUDPeriodo, visible: ko.observable(true) });

    this.Data = PropertyEntity({
        Get: function () { return _tabelaDiaria.Periodos.list.slice(); },
        Set: function (data) { _tabelaDiaria.Periodos.list = data.slice(); },
        Grid: function () { return _gridPeriodo }
    });
}

//*******EVENTOS*******
function LoadPeriodo() {
    _periodo = new Periodo();
    KoBindings(_periodo, "knockoutPeriodo");
    
    new BuscarJustificativas(_periodo.Justificativa);

    CarregarGridPeriodos();
}

function AdicionarPeriodo(ko) {
    var objeto = SalvarObjetoPeriodo(ko, true);

    var dados = ko.Data.Get();
    
    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");    

    dados.push(objeto);
    ko.Data.Set(dados);

    LimparCamposCRUDPeriodo(ko);
    RenderizaGridPeriodo(ko);
}

function AtualizarPeriodo(ko) {
    var objeto = SalvarObjetoPeriodo(ko, false);

    var dados = ko.Data.Get();

    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");    

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val === objeto.Codigo.val) {
            dados[i] = objeto;
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDPeriodo(ko);
    RenderizaGridPeriodo(ko);
}

function ExcluirCamposCRUDPeriodo(ko) {
    var dados = ko.Data.Get();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val === ko.Codigo.val()) {
            dados.splice(i, 1);
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDPeriodo(ko);
    RenderizaGridPeriodo(ko);
}


//*******METODOS*******
function CarregarGridPeriodos() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Editar",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    EditarPeriodo(_periodo, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Periodo", visible: false },
        { data: "Descricao", title: "Descrição", width: "30%", className: "text-align-left" },
        { data: "Valor", title: "Valor", width: "30%", className: "text-align-right" }
    ];

    // Grid
    _gridPeriodo = new BasicDataTable(_periodo.Periodos.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, 10);
    _gridPeriodo.CarregarGrid([]);
}


function LimparCamposPeriodo() {
    LimparCampos(_periodo);

    RenderizaGridPeriodo(_periodo);
}

function LimparCamposCRUDPeriodo(ko) {
    if ("Justificativa" in ko)
        LimparCampoEntity(ko.Justificativa);

    ko.Codigo.val(ko.Codigo.def);
    ko.Descricao.val("");
    ko.HoraInicial.val(ko.HoraInicial.def);
    ko.HoraFinal.val(ko.HoraFinal.def);
    ko.Valor.val(ko.Valor.def);
    LimparCamposCRUDAba(ko);
}

function EditarPeriodo(ko, data) {
    var obj = null;
    ko.Data.Get().forEach(function (item) {
        if (item.Codigo.val === data.Codigo)
            obj = item;
    });

    if (obj !== null) {
        PreencherEditarListEntity(ko, obj);

        ko.Adicionar.visible(false);
        ko.Atualizar.visible(true);
        ko.Excluir.visible(true);
    }
}

function RenderizaGridPeriodo(ko) {
    var data = ko.Data.Get().map(function (item) {
        var obj = {
            Codigo: item.Codigo.val,
            Descricao: item.Descricao.val,
            Periodo: item.HoraInicial.val + " até " + item.HoraFinal.val,
            Valor: item.Valor.val
        };

        if ("Justificativa" in item)
            obj.Justificativa = item.Justificativa.val;

        return obj;
    });

    ko.Data.Grid().CarregarGrid(data);
}

function SalvarObjetoPeriodo(ko, novo) {
    var objeto = SalvarListEntity(ko);

    if (novo) {
        objeto.Codigo.val = guid();
    }

    return objeto;
}

function HandleRequiredComponente(ko) {
    return function () {
        return ko.Data.Grid().BuscarRegistros().length > 0;
    };
}

function LimparCamposCRUDAba(ko) {
    ko.Adicionar.visible(true);
    ko.Atualizar.visible(false);
    ko.Excluir.visible(false);
    ko.Cancelar.visible(true);
}