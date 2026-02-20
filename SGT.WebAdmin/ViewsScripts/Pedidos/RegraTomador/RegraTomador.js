/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
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
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegraTomador;
var _regraTomador;
var _pesquisaRegraTomador;
var _regraTomadorCRUD;

var _TipoTomador = [{ text: "Remetente", value: EnumTipoTomador.Remetente },
{ text: "Destinatário", value: EnumTipoTomador.Destinatario },
{ text: "Expedidor", value: EnumTipoTomador.Expedidor },
{ text: "Recebedor", value: EnumTipoTomador.Recebedor },
{ text: "Outros", value: EnumTipoTomador.Outros }];

var PesquisaRegraTomador = function () {

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OrigemFilial = PropertyEntity({ val: ko.observable(""), options: _origemDestinoFilialPesquisa, def: "", text: "Origem: ", visible: ko.observable(true) });
    this.DestinoFilial = PropertyEntity({ val: ko.observable(""), options: _origemDestinoFilialPesquisa, def: "", text: "Destino: ", visible: ko.observable(true) });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraTomador.CarregarGrid();
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
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var _origemDestinoFilialPesquisa = [
    { text: "Todos", value: "" },
    { text: "Filial", value: true },
    { text: "Não Filial", value: false }
];

var _origemDestinoFilial = [
    { text: "Filial", value: true },
    { text: "Não Filial", value: false }
];

var RegraTomador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoTomador.Remetente), options: _TipoTomador, def: EnumTipoTomador.Remetente, text: "Tipo Tomador: ", required: false, eventChange: tipoTomadorChange });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true), eventChange: validarRegra });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true), eventChange: validarRegra });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tomador:", idBtnSearch: guid(), visible: ko.observable(false), required : false });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.OrigemFilial = PropertyEntity({ val: ko.observable(false), options: _origemDestinoFilial, def: false, text: "Origem: ", visible: ko.observable(true) });
    this.DestinoFilial = PropertyEntity({ val: ko.observable(false), options: _origemDestinoFilial, def: false, text: "Destino: ", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 400 });

    //this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    //this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    //this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    //this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    //this.Importar = PropertyEntity({
    //    type: types.local,
    //    text: "Importar",
    //    visible: ko.observable(true),
    //    accept: ".xls,.xlsx,.csv,.txt",
    //    cssClass: "btn-default",
    //    visible: ko.observable(true),
    //    UrlImportacao: "RegraTomador/Importar",
    //    UrlConfiguracao: "RegraTomador/ConfiguracaoImportacao",
    //    CodigoControleImportacao: EnumCodigoControleImportacao.O014_RegraTomador,
    //    CallbackImportacao: function () {
    //        _gridRegraTomador.CarregarGrid();
    //    }
    //});
}

var CRUDRegraTomador = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        visible: ko.observable(true),
        UrlImportacao: "RegraTomador/Importar",
        UrlConfiguracao: "RegraTomador/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O014_RegraTomador,
        CallbackImportacao: function () {
            _gridRegraTomador.CarregarGrid();
        }
    });
}


//*******EVENTOS*******

function tipoTomadorChange(e, sender) {
    if (_regraTomador.TipoTomador.val() == EnumTipoTomador.Outros) {
        _regraTomador.Tomador.visible(true);
        _regraTomador.Tomador.required = true;
    }
    else {
        _regraTomador.Tomador.visible(false);
        _regraTomador.Tomador.required = false;
        _regraTomador.Tomador.val("");
        _regraTomador.Tomador.codEntity(0);
    }
}

function loadRegraTomador() {

    _regraTomador = new RegraTomador();
    KoBindings(_regraTomador, "knockoutCadastroRegraTomador");

    _regraTomadorCRUD = new CRUDRegraTomador();
    KoBindings(_regraTomadorCRUD, "knockoutCadastroRegraTomadorCRUD");

    HeaderAuditoria("RegraTomador", _regraTomador);

    _pesquisaRegraTomador = new PesquisaRegraTomador();
    KoBindings(_pesquisaRegraTomador, "knockoutPesquisaRegraTomador", false, _pesquisaRegraTomador.Pesquisar.id);


    
    new BuscarClientes(_pesquisaRegraTomador.Remetente);
    new BuscarClientes(_pesquisaRegraTomador.Destinatario);
    new BuscarClientes(_pesquisaRegraTomador.Tomador);

    new BuscarClientes(_regraTomador.Remetente, callbackRemetente);
    new BuscarClientes(_regraTomador.Destinatario, callbackDestinatario);
    new BuscarClientes(_regraTomador.Tomador);
    
    buscarRegraTomador();

}

function callbackRemetente(dataRow) {
    _regraTomador.Remetente.codEntity(dataRow.Codigo);
    _regraTomador.Remetente.val(dataRow.Descricao);
    validarRegra();
}

function callbackDestinatario(dataRow) {
    _regraTomador.Destinatario.codEntity(dataRow.Codigo);
    _regraTomador.Destinatario.val(dataRow.Descricao);
    validarRegra();
}

function validarRegra() {

    if (_regraTomador.Remetente.codEntity() > 0)
        _regraTomador.OrigemFilial.visible(false);
    else
        _regraTomador.OrigemFilial.visible(true);

    if (_regraTomador.Destinatario.codEntity() > 0)
        _regraTomador.DestinoFilial.visible(false);
    else
        _regraTomador.DestinoFilial.visible(true);
}


function adicionarClick(e, sender) {
    Salvar(_regraTomador, "RegraTomador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridRegraTomador.CarregarGrid();
                limparCamposRegraTomador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_regraTomador, "RegraTomador/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridRegraTomador.CarregarGrid();
                limparCamposRegraTomador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a regra do tomador?", function () {
        ExcluirPorCodigo(_regraTomador, "RegraTomador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridRegraTomador.CarregarGrid();
                limparCamposRegraTomador();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRegraTomador();
}

//*******MÉTODOS*******


function buscarRegraTomador() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraTomador, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegraTomador = new GridView(_pesquisaRegraTomador.Pesquisar.idGrid, "RegraTomador/Pesquisa", _pesquisaRegraTomador, menuOpcoes);
    _gridRegraTomador.CarregarGrid();
}

function editarRegraTomador(regraTomadorGrid) {
    limparCamposRegraTomador();
    _regraTomador.Codigo.val(regraTomadorGrid.Codigo);
    BuscarPorCodigo(_regraTomador, "RegraTomador/BuscarPorCodigo", function (arg) {
        _pesquisaRegraTomador.ExibirFiltros.visibleFade(false);
        _regraTomadorCRUD.Atualizar.visible(true);
        _regraTomadorCRUD.Cancelar.visible(true);
        _regraTomadorCRUD.Excluir.visible(true);
        _regraTomadorCRUD.Adicionar.visible(false);
        validarRegra();
        tipoTomadorChange(_regraTomador);
        
    }, null);
}


function limparCamposRegraTomador() {
    _regraTomador.Tomador.visible(false);
    _regraTomador.Tomador.required = false;
    _regraTomadorCRUD.Atualizar.visible(false);
    _regraTomadorCRUD.Cancelar.visible(false);
    _regraTomadorCRUD.Excluir.visible(false);
    _regraTomadorCRUD.Adicionar.visible(true);
    validarRegra();
    LimparCampos(_regraTomador);
}

function DescricaoEstado(sigla) {
    var descricao = "";
    $.each(_estados, function (i, estado) {
        if (estado.value == sigla) {
            descricao = estado.text;
            return false;
        }
    });
    return descricao;
}