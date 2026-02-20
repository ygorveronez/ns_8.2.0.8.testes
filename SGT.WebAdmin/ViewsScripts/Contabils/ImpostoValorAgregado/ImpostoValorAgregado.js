/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumUsoMaterial.js" />

// #region Objetos Globais do Arquivo

var _crudIVA;
var _gridIVA;
var _IVA;
var _pesquisaIVA;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaIVA = function () {
    this.CodigoIVA = PropertyEntity({ text: "Código IVA:", val: ko.observable(""), def: "", maxlength: 4 });
    this.ModeloDocumentoFiscal = PropertyEntity({ text: "Modelo de Documento Fiscal:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ImpostoMaiorQueZero = PropertyEntity({ text: "% ICMS/ISS superior a 0?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, required: ko.observable(true) });
    this.DestinatarioExterior = PropertyEntity({ text: "Destinatário Exterior?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, required: ko.observable(true) });
    this.PermitirInformarManualmente = PropertyEntity({ text: "Permitir Informar Manualmente?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, required: ko.observable(true) });
    this.UsoMaterial = PropertyEntity({ text: "Uso Material:", val: ko.observable(EnumUsoMaterial.Todos), options: EnumUsoMaterial.obterOpcoesPesquisa(), def: EnumUsoMaterial.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridIVA, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var IVA = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoIVA = PropertyEntity({ text: "*Código IVA:", val: ko.observable(""), def: "", maxlength: 4, required: true });
    this.ModeloDocumentoFiscal = PropertyEntity({ text: "*Modelo de Documento Fiscal:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.ImpostoMaiorQueZero = PropertyEntity({ text: "*% ICMS/ISS superior a 0?", val: ko.observable(EnumSimNaoPesquisa.Sim), options: EnumSimNaoPesquisa.obterOpcoes(), def: EnumSimNaoPesquisa.Sim, required: true });
    this.DestinatarioExterior = PropertyEntity({ text: "*Destinatário Exterior?", val: ko.observable(EnumSimNaoPesquisa.Sim), options: EnumSimNaoPesquisa.obterOpcoes(), def: EnumSimNaoPesquisa.Sim, required: true });
    this.PermitirInformarManualmente = PropertyEntity({ text: "*Permitir Informar Manualmente?", val: ko.observable(EnumSimNaoPesquisa.Sim), options: EnumSimNaoPesquisa.obterOpcoes(), def: EnumSimNaoPesquisa.Sim, required: true });
    this.UsoMaterial = PropertyEntity({ text: "*Uso Material:", val: ko.observable(EnumUsoMaterial.Revenda), options: EnumUsoMaterial.obterOpcoes(), def: EnumUsoMaterial.Revenda, required: true });
}

var CrudIVA = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadIVA() {
    _pesquisaIVA = new PesquisaIVA();
    KoBindings(_pesquisaIVA, "knockoutPesquisaImpostoValorAgregado", false, _pesquisaIVA.Pesquisar.id);

    _IVA = new IVA();
    KoBindings(_IVA, "knockoutImpostoValorAgregado");

    HeaderAuditoria("ImpostoValorAgregado", _IVA);

    _crudIVA = new CrudIVA();
    KoBindings(_crudIVA, "knockoutCRUD_ImpostoValorAgregado");

    new BuscarModeloDocumentoFiscal(_pesquisaIVA.ModeloDocumentoFiscal);
    new BuscarModeloDocumentoFiscal(_IVA.ModeloDocumentoFiscal);

    loadGridIVA();
}

function loadGridIVA() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridIVA = new GridView(_pesquisaIVA.Pesquisar.idGrid, "ImpostoValorAgregado/Pesquisa", _pesquisaIVA, menuOpcoes);
    _gridIVA.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_IVA, "ImpostoValorAgregado/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridIVA();
                limparCamposIVA();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_IVA, "ImpostoValorAgregado/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridIVA();
                limparCamposIVA();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposIVA();
}

function editarClick(registroSelecionado) {
    limparCamposIVA();
   
    _IVA.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_IVA, "ImpostoValorAgregado/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaIVA.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_IVA, "ImpostoValorAgregado/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridIVA();
                    limparCamposIVA();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function controlarBotoesHabilitados(isEdicao) {
    _crudIVA.Atualizar.visible(isEdicao);
    _crudIVA.Excluir.visible(isEdicao);
    _crudIVA.Cancelar.visible(isEdicao);
    _crudIVA.Adicionar.visible(!isEdicao);
}

function limparCamposIVA() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_IVA);
    exibirFiltros();
}

function recarregarGridIVA() {
    _gridIVA.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaIVA.ExibirFiltros.visibleFade())
        _pesquisaIVA.ExibirFiltros.visibleFade(true);
}

// #endregion Funções Privadas

