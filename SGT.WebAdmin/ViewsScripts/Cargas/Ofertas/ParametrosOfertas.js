/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Auditoria.js" />
/// <reference path="Constantes.js" />
/// <reference path="Atividades.js" />
/// <reference path="Entidades/TipoCarga.js" />
/// <reference path="Entidades/TipoOperacao.js" />
/// <reference path="Entidades/Filial.js" />
/// <reference path="Entidades/Funcionario.js" />
/// <reference path="Entidades/GrupoMotoristas.js" />

var _pesquisaParametrosOfertas;

var _parametrosOfertas;
var _parametrosOfertasTipoOperacao;
var _parametrosOfertasTipoCarga;
var _parametrosOfertasEmpresa;
var _parametrosOfertasFilial;
var _parametrosOfertasFuncionario;
var _crudGeral;

var _gridParametrosOfertas;
var _gridTipoOperacao;
var _gridTipoCarga;
var _gridEmpresa;
var _gridFilial;
var _gridFuncionario;


function LoadParametrosOfertas() {

    _parametrosOfertas = new ParametrosOfertas();
    KoBindings(_parametrosOfertas, "knockoutDetalhes");

    _pesquisaParametrosOfertas = new PesquisaParametrosOfertas();
    KoBindings(_pesquisaParametrosOfertas, "knockoutPesquisaParametrosOfertas", false, _pesquisaParametrosOfertas.Pesquisar.id);

    _crudGeral = new CRUDGeral();
    KoBindings(_crudGeral, "knockoutCRUDGeral");

    HeaderAuditoria("ParametrosOfertas", _parametrosOfertas);

    BuscarParametrosOfertas();

    LoadParemterosOfertasDadosOferta();
    LoadParemterosOfertasTipoIntegracao();

    LoadTiposOperacao();
    LoadTiposCarga();
    LoadEmpresas();
    LoadFiliais();
    LoadFuncionarios();
    LoadGrupoMotoristasCRUD();
}

var CRUDGeral = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarParametrosOfertasClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarParametrosOfertasClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarEdicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var ParametrosOfertas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição", required: true, maxlength: 100 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração", maxlength: 50 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação", issue: 557 });

    this.ParametrosOfertasDadosOferta = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), val: ko.observableArray([]) });
    this.ParametrosOfertasFuncionario = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), val: ko.observableArray([]) });
    this.ParametrosOfertasTipoOperacao = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), val: ko.observableArray([]) });
    this.ParametrosOfertasEmpresa = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), val: ko.observableArray([]) });
    this.ParametrosOfertasFilial = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), val: ko.observableArray([]) });
    this.ParametrosOfertasTipoCarga = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), val: ko.observableArray([]) });

    this.TiposIntegracao = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), val: ko.observableArray([]) });
    this.TiposCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.Empresas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.Funcionarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.GrupoMotoristas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Motoristas", val: ko.observable(""), idBtnSearch: guid(), def: "", visible: ko.observable(true) });
}

function ObterParametrosOfertasAdicionarCompleto() {

    return {
        Descricao: _parametrosOfertas.Descricao.val(),
        CodigoIntegracao: _parametrosOfertas.CodigoIntegracao.val(),
        Ativo: _parametrosOfertas.Ativo.val(),
        TiposIntegracao: JSON.stringify(_parametrosOfertas.TiposIntegracao.val()),
        TiposOperacao: JSON.stringify(_parametrosOfertas.TiposOperacao.val()),
        TiposCarga: JSON.stringify(_parametrosOfertas.TiposCarga.val()),
        Empresas: JSON.stringify(_parametrosOfertas.Empresas.val()),
        Filiais: JSON.stringify(_parametrosOfertas.Filiais.val()),
        Funcionarios: JSON.stringify(_parametrosOfertas.Funcionarios.val()),
        ParametrosOfertasDadosOfertas: JSON.stringify(_parametrosOfertas.ParametrosOfertasDadosOferta.val()),
        GrupoMotoristas: _parametrosOfertas.GrupoMotoristas.codEntity(),
    }
}

function ObterParametrosOfertasAtualizarCompleto() {

    const ajuste = o => ({ ...o, CodigoRelacionamento: o.CodigoRelacionamento ?? 0 })

    return {
        Codigo: _parametrosOfertas.Codigo.val(),
        Descricao: _parametrosOfertas.Descricao.val(),
        CodigoIntegracao: _parametrosOfertas.CodigoIntegracao.val(),
        Ativo: _parametrosOfertas.Ativo.val(),
        TiposIntegracao: JSON.stringify(_parametrosOfertas.TiposIntegracao.val()),
        TiposOperacao: JSON.stringify(_parametrosOfertas.TiposOperacao.val().map(ajuste)),
        TiposCarga: JSON.stringify(_parametrosOfertas.TiposCarga.val().map(ajuste)),
        Empresas: JSON.stringify(_parametrosOfertas.Empresas.val().map(ajuste)),
        Filiais: JSON.stringify(_parametrosOfertas.Filiais.val().map(ajuste)),
        Funcionarios: JSON.stringify(_parametrosOfertas.Funcionarios.val().map(ajuste)),
        ParametrosOfertasDadosOfertas: JSON.stringify(_parametrosOfertas.ParametrosOfertasDadosOferta.val()),
        GrupoMotoristas: _parametrosOfertas.GrupoMotoristas.codEntity(),
    }
}

function SelecionarParametrosOfertas(parametrosOfertasGrid) {
    LimparGeral();

    RecuperarParametrosOfertas(parametrosOfertasGrid.Codigo);
}


function CancelarEdicaoClick() {
    LimparGeral();
    LigarModoEdicao(false);
}

function LimparCamposParametrosOfertas() {
    LimparCampos(_parametrosOfertas);

    _parametrosOfertas.ParametrosOfertasDadosOferta.val([]);
    _parametrosOfertas.TiposIntegracao.val([]);
}

function BuscarParametrosOfertas() {
    let selecionar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) { SelecionarParametrosOfertas(data); }, tamanho: "10", icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [selecionar], tamanho: 10 };

    _gridParametrosOfertas = new GridView(_pesquisaParametrosOfertas.Pesquisar.idGrid, MontarCaminho(CONTROLLER_PARAMETROS_OFERTAS, ENDPOINT_PESQUISAR_PARAMETROS_OFERTAS), _pesquisaParametrosOfertas, menuOpcoes);
    _gridParametrosOfertas.CarregarGrid();
}

function PreencherParametrosOfertas(data) {
    PreencherObjetoKnout(_parametrosOfertas, { Data: data });
}