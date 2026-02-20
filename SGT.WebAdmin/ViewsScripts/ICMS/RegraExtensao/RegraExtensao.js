/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumTipoRegraExtensao.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />

// #region Objetos Globais do Arquivo

var _CRUDRegraExtensao;
var _gridRegraExtensao;
var _RegraExtensao;
var _pesquisaRegraExtensao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaRegraExtensao = function () {
	this.Extensao = PropertyEntity({ text: "Extensão:", val: ko.observable(""), def: "" });
	this.TipoPropriedade = PropertyEntity({ getType: typesKnockout.select, text: "Tipo Propriedade:", val: ko.observable(""), def: "", options: EnumTipoVeiculoProprietario.obterOpcoesPesquisa() });
	this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular", idBtnSearch: guid() });

	this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
	this.Pesquisar = PropertyEntity({ eventClick: recarregarGridRegraExtensao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var RegraExtensao = function () {
	this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
	this.Extensao = PropertyEntity({ text: "*Extensão:", val: ko.observable(""), def: "", maxlength: 50, required: ko.observable(true) });
	this.TipoPropriedade = PropertyEntity({ getType: typesKnockout.select, text: "*Tipo Propriedade:", val: ko.observable(EnumTipoVeiculoProprietario.Proprio), def: EnumTipoVeiculoProprietario.Proprio, required: ko.observable(true), options: EnumTipoVeiculoProprietario.obterOpcoes() });
	this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
}

var CRUDRegraExtensao = function () {
	this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
	this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
	this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
	this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadRegraExtensao() {

	_pesquisaRegraExtensao = new PesquisaRegraExtensao();
	KoBindings(_pesquisaRegraExtensao, "knockoutPesquisaRegraExtensao", false, _pesquisaRegraExtensao.Pesquisar.id);

	_regraExtensao = new RegraExtensao();
	KoBindings(_regraExtensao, "knockoutRegraExtensao");

	HeaderAuditoria("RegraExtensao", _regraExtensao);

	_CRUDRegraExtensao = new CRUDRegraExtensao();
	KoBindings(_CRUDRegraExtensao, "knockoutCRUDRegraExtensao");

	new BuscarModelosVeicularesCarga(_pesquisaRegraExtensao.ModeloVeicular);
	new BuscarModelosVeicularesCarga(_regraExtensao.ModeloVeicular);

	loadGridRegraExtensao();
}

function loadGridRegraExtensao() {
	var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
	var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

	_gridRegraExtensao = new GridView(_pesquisaRegraExtensao.Pesquisar.idGrid, "RegraExtensao/Pesquisa", _pesquisaRegraExtensao, menuOpcoes);
	_gridRegraExtensao.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
	Salvar(_regraExtensao, "RegraExtensao/Adicionar", function (retorno) {
			if (retorno.Success) {
				if (retorno.Data) {
					exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
					recarregarGridRegraExtensao();
					limparCamposRegraExtensao();
				}
				else
					exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
			}
			else
				exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
		}, sender);
}

function atualizarClick(e, sender) {
	Salvar(_regraExtensao, "RegraExtensao/Atualizar", function (retorno) {
			if (retorno.Success) {
				if (retorno.Data) {
					exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
					recarregarGridRegraExtensao();
					limparCamposRegraExtensao();
				}
				else
					exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
			}
			else
				exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
		}, sender);
}

function cancelarClick() {
	limparCamposRegraExtensao();
}

function editarClick(registroSelecionado) {
	limparCamposRegraExtensao();

	_regraExtensao.Codigo.val(registroSelecionado.Codigo);

	BuscarPorCodigo(_regraExtensao, "RegraExtensao/BuscarPorCodigo", function (retorno) {
		if (retorno.Success) {
			if (retorno.Data) {
				_pesquisaRegraExtensao.ExibirFiltros.visibleFade(false);

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
		ExcluirPorCodigo(_regraExtensao, "RegraExtensao/ExcluirPorCodigo", function (retorno) {
			if (retorno.Success) {
				if (retorno.Data) {
					exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

					recarregarGridRegraExtensao();
					limparCamposRegraExtensao();
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

// #region Métodos privados

function controlarBotoesHabilitados(isEdicao) {
	_CRUDRegraExtensao.Atualizar.visible(isEdicao);
	_CRUDRegraExtensao.Excluir.visible(isEdicao);
	_CRUDRegraExtensao.Cancelar.visible(isEdicao);
	_CRUDRegraExtensao.Adicionar.visible(!isEdicao);
}

function limparCamposRegraExtensao() {
	var isEdicao = false;

	controlarBotoesHabilitados(isEdicao);
	LimparCampos(_regraExtensao);
	exibirFiltros();
}

function recarregarGridRegraExtensao() {
	_gridRegraExtensao.CarregarGrid();
}

function exibirFiltros() {
	if (!_pesquisaRegraExtensao.ExibirFiltros.visibleFade())
		_pesquisaRegraExtensao.ExibirFiltros.visibleFade(true);
}

// #endregion Métodos privados