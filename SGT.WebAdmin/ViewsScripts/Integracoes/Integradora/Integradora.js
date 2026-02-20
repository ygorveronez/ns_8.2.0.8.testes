/// <reference path="../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/knockout/knockout-3.3.0.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="PermissoesWebService.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridIntegradora;
var _integradora;
var _CRUDintegradora;
var _pesquisaIntegradora;

var PesquisaIntegradora = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegradora.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Integradora = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true, maxlength: 150 });
    this.TipoAutenticacao = PropertyEntity({ text: Localization.Resources.Integracoes.Integradora.TipoAutenticacao.getFieldDescription(), options: EnumTipoAutenticacao.obterOpcoes(), val: ko.observable(EnumTipoAutenticacao.Token), def: EnumTipoAutenticacao.Token, visible: ko.observable(true) });
    this.TipoAutenticacao.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoAutenticacao.Token) {
            _integradora.Token.visible(true);
            
            if (_integradora.Codigo.val() > 0 && _integradora.Token.val() == "")
                _integradora.Token.enable(true);

            _integradora.Usuario.visible(false);
            _integradora.Usuario.required(false);
            _integradora.Senha.visible(false);
            _integradora.Senha.required(false);
            _integradora.TempoExpiracao.visible(false);
            _integradora.TempoExpiracao.required(false);
        } else {
            _integradora.Token.visible(false);
            _integradora.Usuario.visible(true);
            _integradora.Usuario.required(true);
            _integradora.Senha.visible(true);
            _integradora.Senha.required(true);
            _integradora.TempoExpiracao.visible(true);
            _integradora.TempoExpiracao.required(true);
        }
    });
    this.Token = PropertyEntity({ text: Localization.Resources.Integracoes.Integradora.TokenDeIntegracao.getFieldDescription(), maxlength: 500, enable: ko.observable(true), visible: ko.observable(true) });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Usuario.getRequiredFieldDescription(), val: ko.observable(""), def: "", maxlength: 50, required: ko.observable(false), visible: ko.observable(false) });
    this.Senha = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Senha.getRequiredFieldDescription(), val: ko.observable(""), def: "", maxlength: 50, required: ko.observable(false), visible: ko.observable(false) });
    this.TempoExpiracao = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Integracoes.Integradora.TempoDeExpiracao.getRequiredFieldDescription(), val: ko.observable(0), def: 0, maxlength: 3, configInt: { precision: 0, allowZero: false, thousands: "" }, required: ko.observable(false), visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Integracoes.Integradora.Transportadora.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Gerais.Geral.Cliente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557 });
    this.TodosWebServicesLiberados = PropertyEntity({ text: Localization.Resources.Integracoes.Integradora.TodosWebServicesLiberados.getFieldDescription(), val: ko.observable(true), def: true, getType: typesKnockout.bool, enable: ko.observable(true), visible: true });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557 });
    this.Clientes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.ListaPermissoesWebService = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Integracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
};

var CRUDIntegradora = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadIntegradora() {

    _integradora = new Integradora();
    KoBindings(_integradora, "knockoutCadastroIntegradora");

    _pesquisaIntegradora = new PesquisaIntegradora();
    KoBindings(_pesquisaIntegradora, "knockoutPesquisaIntegradora", false, _pesquisaIntegradora.Pesquisar.id);

    _CRUDintegradora = new CRUDIntegradora();
    KoBindings(_CRUDintegradora, "knockoutCRUDCadastroIntegradora");

    HeaderAuditoria("Integradora", _integradora);

    new BuscarTransportadores(_integradora.Empresa);
    new BuscarGruposPessoas(_integradora.GrupoPessoas);
    new BuscarClientes(_integradora.Cliente);

    loadPermissoesWebService();
    LoadClienteIntegradora();
    buscarIntegradoras();
    carregarIntegracoesIntegradora();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _integradora.Empresa.visible(false);
        _integradora.GrupoPessoas.visible(true);
    }
}

function adicionarClick(e, sender) {
    _integradora.Clientes.val(JSON.stringify(_clienteIntegradora.Cliente.basicTable.BuscarRegistros()));
    _integradora.Integracoes.val(obterConfiguracaoIntegracaoSalvar());

    if (_integradora.TodosWebServicesLiberados.val() && _integradora.Empresa.codEntity() > 0) {
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Não é possível liberar todos os Web Services para o Transportador.");
    }

    Salvar(_integradora, "Integradora/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridIntegradora.CarregarGrid();
                limparCamposIntegradora();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    _integradora.Clientes.val(JSON.stringify(_clienteIntegradora.Cliente.basicTable.BuscarRegistros()));
    _integradora.Integracoes.val(obterConfiguracaoIntegracaoSalvar());

    console.log(obterConfiguracaoIntegracaoSalvar())

    if (_integradora.TodosWebServicesLiberados.val() && _integradora.Empresa.codEntity() > 0) {
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Não é possível liberar todos os Web Services para o Transportador.");
    }
    Salvar(_integradora, "Integradora/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridIntegradora.CarregarGrid();
                limparCamposIntegradora();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Integracoes.Integradora.RealmenteDesejaExcluirIntegradora + _integradora.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_integradora, "Integradora/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                _gridIntegradora.CarregarGrid();
                limparCamposIntegradora();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposIntegradora();
}

//*******MÉTODOS*******


function buscarIntegradoras() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarIntegradora, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridIntegradora = new GridView(_pesquisaIntegradora.Pesquisar.idGrid, "Integradora/Pesquisa", _pesquisaIntegradora, menuOpcoes, null);
    _gridIntegradora.CarregarGrid();
}

function editarIntegradora(integradoraGrid) {
    limparCamposIntegradora();
    _integradora.Codigo.val(integradoraGrid.Codigo);
    BuscarPorCodigo(_integradora, "Integradora/BuscarPorCodigo", function (arg) {
        _pesquisaIntegradora.ExibirFiltros.visibleFade(false);

        RecarregarGridClienteIntegradora();
        recarregarGridPermissoesWebService();
        preencherIntegracao(arg.Data.Integracoes);

        _integradora.Token.enable(false);
        _CRUDintegradora.Atualizar.visible(true);
        _CRUDintegradora.Cancelar.visible(true);
        _CRUDintegradora.Excluir.visible(true);
        _CRUDintegradora.Adicionar.visible(false);
    }, null);
}

function limparCamposIntegradora() {

    _CRUDintegradora.Atualizar.visible(false);
    _CRUDintegradora.Cancelar.visible(false);
    _CRUDintegradora.Excluir.visible(false);
    _CRUDintegradora.Adicionar.visible(true);

    _integradora.Token.enable(true);
    LimparCampos(_integradora);
    RecarregarGridClienteIntegradora();
    recarregarGridPermissoesWebService();
    limparCamposIntegracaoIntegradora();
}



















