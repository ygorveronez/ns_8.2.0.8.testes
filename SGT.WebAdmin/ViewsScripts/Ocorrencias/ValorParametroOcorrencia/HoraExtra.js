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
var _horaExtra;
var _horaExtraVeiculo;
var _horaExtraAjudante;
var _gridHoraExtraVeiculo;
var _gridHoraExtraAjudante;

var HoraExtra = function () {
    this.TipoOcorrencia = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Ocorrência:",  idBtnSearch: guid(), visible: ko.observable(true), required: function () {
            return HandleRequiredTipoOcorrencia(_gridHoraExtraVeiculo, _gridHoraExtraAjudante);
        } });
}

var HoraExtraVeiculo = function () {
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Componente Frete:", idBtnSearch: guid(), visible: ko.observable(true), required: HandleRequiredComponente(this) });
    this.Veiculos = PropertyEntity({ idGrid: guid(), type: types.local });

    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.HoraInicial = PropertyEntity({ text: "Hora Inicial:", getType: typesKnockout.time });
    this.HoraFinal = PropertyEntity({ text: "Hora Final:", getType: typesKnockout.time });
    this.Valor = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarHoraExtra, visible: ko.observable(true)});
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarHoraExtra, visible: ko.observable(false)});
    this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirCamposCRUDHoraExtra, visible: ko.observable(false)});
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposCRUDHoraExtra, visible: ko.observable(true) });

    this.Data = PropertyEntity({
        Get: function () { return _dados.HoraExtraVeiculos.list.slice(); },
        Set: function (data) { _dados.HoraExtraVeiculos.list = data.slice(); },
        Grid: function () { return _gridHoraExtraVeiculo }
    });
}

var HoraExtraAjudante = function () {
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Componente Frete:", idBtnSearch: guid(), visible: ko.observable(true), required: HandleRequiredComponente(this) });
    this.Ajudantes = PropertyEntity({ idGrid: guid(), type: types.local });

    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0) });
    this.HoraInicial = PropertyEntity({ text: "Hora Inicial:", getType: typesKnockout.time });
    this.HoraFinal = PropertyEntity({ text: "Hora Final:", getType: typesKnockout.time });
    this.Valor = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarHoraExtra, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarHoraExtra, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirCamposCRUDHoraExtra, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposCRUDHoraExtra, visible: ko.observable(true) });

    this.Data = PropertyEntity({
        Get: function () { return _dados.HoraExtraAjudantes.list.slice(); },
        Set: function (data) { _dados.HoraExtraAjudantes.list = data.slice(); },
        Grid: function () { return _gridHoraExtraAjudante }
    });
}

//*******EVENTOS*******
function LoadHoraExtra() {
    _horaExtra = new HoraExtra();
    KoBindings(_horaExtra, "knockoutHoraExtra");

    _horaExtraVeiculo = new HoraExtraVeiculo();
    KoBindings(_horaExtraVeiculo, "knockoutHoraExtraVeiculo");

    _horaExtraAjudante = new HoraExtraAjudante();
    KoBindings(_horaExtraAjudante, "knockoutHoraExtraAjudante");

    new BuscarModelosVeicularesCarga(_horaExtraVeiculo.ModeloVeicular);
    new BuscarComponentesDeFrete(_horaExtraVeiculo.ComponenteFrete);
    new BuscarComponentesDeFrete(_horaExtraAjudante.ComponenteFrete);
    new BuscarTipoOcorrencia(_horaExtra.TipoOcorrencia);

    CarregarGridHoraExtraVeiculos();
    CarregarGridHoraExtraAjudante();

    _dados.HoraExtraTipoOcorrencia = _horaExtra.TipoOcorrencia;
    _dados.HoraExtraComponenteFreteVeiculo = _horaExtraVeiculo.ComponenteFrete;
    _dados.HoraExtraComponenteFreteAjudante = _horaExtraAjudante.ComponenteFrete;
}

function AdicionarHoraExtra(ko) {
    var objeto = SalvarObjetoHoraExtra(ko, true);

    var dados = ko.Data.Get();

    ko.ComponenteFrete.required = false;
    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");
    ko.ComponenteFrete.required = HandleRequiredComponente(ko);

    if ("ModeloVeicular" in ko && !ValidarDuplicidadeModeloVeicular(ko, objeto))
        return exibirMensagem(tipoMensagem.atencao, "Duplicidade", "Já existe um cadastro para esse modelo veicular.");

    dados.push(objeto);
    ko.Data.Set(dados);

    LimparCamposCRUDHoraExtra(ko);
    RenderizaGridHoraExtra(ko);
}

function AtualizarHoraExtra(ko) {
    var objeto = SalvarObjetoHoraExtra(ko, false);

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

    LimparCamposCRUDHoraExtra(ko);
    RenderizaGridHoraExtra(ko);
}

function ExcluirCamposCRUDHoraExtra(ko) {
    var dados = ko.Data.Get();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val == ko.Codigo.val()) {
            dados.splice(i, 1);
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDHoraExtra(ko);
    RenderizaGridHoraExtra(ko);
}


//*******METODOS*******
function CarregarGridHoraExtraVeiculos() {
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
                    EditarHoraExtra(_horaExtraVeiculo, data);
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
    _gridHoraExtraVeiculo = new BasicDataTable(_horaExtraVeiculo.Veiculos.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, 10);
    _gridHoraExtraVeiculo.CarregarGrid([]);
}

function CarregarGridHoraExtraAjudante() {
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
                    EditarHoraExtra(_horaExtraAjudante, data);
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
    _gridHoraExtraAjudante = new BasicDataTable(_horaExtraAjudante.Ajudantes.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, 10);
    _gridHoraExtraAjudante.CarregarGrid([]);
}

function LimparCamposHoraExtra() {
    LimparCampos(_horaExtraVeiculo);
    LimparCampos(_horaExtraAjudante);

    RenderizaGridHoraExtra(_horaExtraVeiculo);
    RenderizaGridHoraExtra(_horaExtraAjudante);
}

function LimparCamposCRUDHoraExtra(ko) {
    if ("ModeloVeicular" in ko)
        LimparCampoEntity(ko.ModeloVeicular);

    ko.Codigo.val(ko.Codigo.def);
    ko.HoraInicial.val(ko.HoraInicial.def);
    ko.HoraFinal.val(ko.HoraFinal.def);
    ko.Valor.val(ko.Valor.def);
    LimparCamposCRUDAba(ko);
}

function EditarHoraExtra(ko, data) {
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

function RenderizaGridHoraExtra(ko) {
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

function SalvarObjetoHoraExtra(ko, novo) {
    var objeto = SalvarListEntity(ko);

    if (novo)
        objeto.Codigo.val = guid();

    return objeto;
}