/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="ValorParametroOcorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _estadia;
var _estadiaVeiculo;
var _estadiaAjudante;
var _gridEstadiaVeiculo;
var _gridEstadiaAjudante;

var Estadia = function () {
    this.TipoOcorrencia = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Ocorrência:",  idBtnSearch: guid(), visible: ko.observable(true), required: function () {
            return HandleRequiredTipoOcorrencia(_gridEstadiaVeiculo, _gridEstadiaAjudante);
        }
    });
}

var EstadiaVeiculo = function () {
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Componente Frete:", idBtnSearch: guid(), visible: ko.observable(true), required: HandleRequiredComponente(this) });
    this.Veiculos = PropertyEntity({ idGrid: guid(), type: types.local });

    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.HoraInicial = PropertyEntity({ text: "Hora Inicial:", getType: typesKnockout.time });
    this.HoraFinal = PropertyEntity({ text: "Hora Final:", getType: typesKnockout.time });
    this.Valor = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarEstadia, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarEstadia, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirCamposCRUDEstadia, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposCRUDEstadia, visible: ko.observable(true) });

    this.Data = PropertyEntity({
        Get: function () { return _dados.EstadiaVeiculos.list.slice(); },
        Set: function (data) { _dados.EstadiaVeiculos.list = data.slice(); },
        Grid: function () { return _gridEstadiaVeiculo }
    });
}

var EstadiaAjudante = function () {
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Componente Frete:", idBtnSearch: guid(), visible: ko.observable(true), required: HandleRequiredComponente(this) });
    this.Ajudantes = PropertyEntity({ idGrid: guid(), type: types.local });

    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0) });
    this.HoraInicial = PropertyEntity({ text: "Hora Inicial:", getType: typesKnockout.time });
    this.HoraFinal = PropertyEntity({ text: "Hora Final:", getType: typesKnockout.time });
    this.Valor = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarEstadia, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarEstadia, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirCamposCRUDEstadia, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposCRUDEstadia, visible: ko.observable(true) });

    this.Data = PropertyEntity({
        Get: function () { return _dados.EstadiaAjudantes.list.slice(); },
        Set: function (data) { _dados.EstadiaAjudantes.list = data.slice(); },
        Grid: function () { return _gridEstadiaAjudante }
    });
}

//*******EVENTOS*******
function LoadEstadia() {
    _estadia = new Estadia();
    KoBindings(_estadia, "knockoutEstadia");

    _estadiaVeiculo = new EstadiaVeiculo();
    KoBindings(_estadiaVeiculo, "knockoutEstadiaVeiculo");

    _estadiaAjudante = new EstadiaAjudante();
    KoBindings(_estadiaAjudante, "knockoutEstadiaAjudante");

    new BuscarModelosVeicularesCarga(_estadiaVeiculo.ModeloVeicular);
    new BuscarComponentesDeFrete(_estadiaVeiculo.ComponenteFrete);
    new BuscarComponentesDeFrete(_estadiaAjudante.ComponenteFrete);
    new BuscarTipoOcorrencia(_estadia.TipoOcorrencia);

    CarregarGridEstadiaVeiculos();
    CarregarGridEstadiaAjudante();

    _dados.EstadiaTipoOcorrencia = _estadia.TipoOcorrencia;
    _dados.EstadiaComponenteFreteVeiculo = _estadiaVeiculo.ComponenteFrete;
    _dados.EstadiaComponenteFreteAjudante = _estadiaAjudante.ComponenteFrete;
}

function AdicionarEstadia(ko) {
    var objeto = SalvarObjetoEstadia(ko, true);

    var dados = ko.Data.Get();

    ko.ComponenteFrete.required = false;
    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");
    ko.ComponenteFrete.required = HandleRequiredComponente(ko);

    if ("ModeloVeicular" in ko && !ValidarDuplicidadeModeloVeicular(ko, objeto))
        return exibirMensagem(tipoMensagem.atencao, "Duplicidade", "Já existe um cadastro para esse modelo veicular.");

    dados.push(objeto);
    ko.Data.Set(dados);

    LimparCamposCRUDEstadia(ko);
    RenderizaGridEstadia(ko);
}

function AtualizarEstadia(ko) {
    var objeto = SalvarObjetoEstadia(ko, false);

    var dados = ko.Data.Get();

    ko.ComponenteFrete.required = false;
    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");
    ko.ComponenteFrete.required = HandleRequiredComponente(ko);

    if ("ModeloVeicular" in ko && !ValidarDuplicidadeModeloVeicular(ko, objeto))
        return exibirMensagem(tipoMensagem.atencao, "Duplicidade", "Já existe um cadastro para esse modelo veicular.");

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val == objeto.Codigo.val) {
            dados[i] = objeto;
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDEstadia(ko);
    RenderizaGridEstadia(ko);
}

function ExcluirCamposCRUDEstadia(ko) {
    var dados = ko.Data.Get();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val == ko.Codigo.val()) {
            dados.splice(i, 1);
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDEstadia(ko);
    RenderizaGridEstadia(ko);
}


//*******METODOS*******
function CarregarGridEstadiaVeiculos() {
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
                    EditarEstadia(_estadiaVeiculo, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo", width: "30%", className: "text-align-left" },
        { data: "Periodo", title: "Período", width: "30%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "30%", className: "text-align-right" },
    ];

    // Grid
    _gridEstadiaVeiculo = new BasicDataTable(_estadiaVeiculo.Veiculos.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, 10);
    _gridEstadiaVeiculo.CarregarGrid([]);
}

function CarregarGridEstadiaAjudante() {
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
                    EditarEstadia(_estadiaAjudante, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Periodo", title: "Período", width: "30%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "30%", className: "text-align-right" },
    ];

    // Grid
    _gridEstadiaAjudante = new BasicDataTable(_estadiaAjudante.Ajudantes.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, 10);
    _gridEstadiaAjudante.CarregarGrid([]);
}

function LimparCamposEstadia() {
    LimparCampos(_estadia);
    LimparCampos(_estadiaVeiculo);
    LimparCampos(_estadiaAjudante);

    RenderizaGridEstadia(_estadiaVeiculo);
    RenderizaGridEstadia(_estadiaAjudante);
}

function LimparCamposCRUDEstadia(ko) {
    if ("ModeloVeicular" in ko)
        LimparCampoEntity(ko.ModeloVeicular);

    ko.Codigo.val(ko.Codigo.def);
    ko.HoraInicial.val(ko.HoraInicial.def);
    ko.HoraFinal.val(ko.HoraFinal.def);
    ko.Valor.val(ko.Valor.def);
    LimparCamposCRUDAba(ko);
}

function EditarEstadia(ko, data) {
    var obj = null;
    ko.Data.Get().forEach(function (item) {
        if (item.Codigo.val == data.Codigo)
            obj = item;
    });

    if (obj != null) {
        PreencherEditarListEntity(ko, obj);

        ko.Adicionar.visible(false);
        ko.Atualizar.visible(true);
        ko.Excluir.visible(true);
    }
}

function RenderizaGridEstadia(ko) {
    var data = ko.Data.Get().map(function (item) {
        var obj = {
            Codigo: item.Codigo.val,
            Periodo: item.HoraInicial.val + " até " + item.HoraFinal.val,
            Valor: item.Valor.val
        };

        if ("ModeloVeicular" in item)
            obj.ModeloVeicular = item.ModeloVeicular.val;

        return obj;
    });

    ko.Data.Grid().CarregarGrid(data);
}

function SalvarObjetoEstadia(ko, novo) {
    var objeto = SalvarListEntity(ko);

    if (novo)
        objeto.Codigo.val = guid();

    return objeto;
}