/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumTipoIrregularidade.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />

// #region Objetos Globais do Arquivo

var _CRUDIrregularidade;
var _gridIrregularidade;
var _Irregularidade;
var _pesquisaIrregularidade;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaIrregularidade = function () {
	this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlentgh: 100 });
	this.Sequencia = PropertyEntity({ getType: typesKnockout.int, text: "Sequência:", val: ko.observable(""), def: "", maxlentgh: 10 });
	this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", val: ko.observable(""), def: "", maxlentgh: 50 });
	this.PortfolioModuloControle = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Portfólio do Módulo de Controle:", idBtnSearch: guid(), required: true });
	this.SeguirAprovacaoTranspPrimeiro = PropertyEntity({ text: "Seguir para Aprovação da Transportadora primeiro:", getType: typesKnockout.select, def: EnumSimNaoPesquisa.Todos, options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos) });
	this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 0, options: _statusFemPesquisa, val: ko.observable(0), text: "Situação:" });

	this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
	this.Pesquisar = PropertyEntity({ eventClick: recarregarGridIrregularidade, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var Irregularidade = function () {
	this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
	this.Descricao = PropertyEntity({ text: "*Descrição", val: ko.observable(""), def: "", maxlentgh: 100, required: ko.observable(true) });
	this.Sequencia = PropertyEntity({ getType: typesKnockout.int, text: "Sequência", val: ko.observable(""), def: "", maxlentgh: 10 });
	this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração", val: ko.observable(""), def: "", maxlentgh: 50 });
	this.PortfolioModuloControle = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Portfólio do Módulo de Controle:", idBtnSearch: guid() });
	this.SeguirAprovacaoTranspPrimeiro = PropertyEntity({ text: "Seguir para Aprovação da Transportadora primeiro", getType: typesKnockout.bool, def: false, val: ko.observable(false) });
	this.Situacao = PropertyEntity({ getType: typesKnockout.select, def: 1, options: _statusFem, val: ko.observable(1), text: "*Situação:", required: ko.observable(true) });
	this.Tipo = PropertyEntity({ getType: typesKnockout.select, def: 1, options: EnumTipoIrregularidade.obterOpcoes(), val: ko.observable(1), text: "Tipo:", required: ko.observable(false), visible: false });
	this.Gatilho = PropertyEntity({ def: EnumGatilhoIrregularidade.NaoSelecionado, options: EnumGatilhoIrregularidade.obterOpcoes(), val: ko.observable(EnumGatilhoIrregularidade.NaoSelecionado), text: "*Gatilho da Irregularidade:", required: ko.observable(false) });
	this.PercentualTolerancia = PropertyEntity({ getType: typesKnockout.int, maxlength: 2, configInt: { precision: 0, allowZero: false, thousands: "" }, text: "Percentual Tolerância:", val: ko.observable(0), def: "", visible: ko.observable(false) });
	this.ValorTolerancia = PropertyEntity({ text: "Valor Tolerância ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), visible: ko.observable(false), maxlength: 16, enable: ko.observable(true) });

	this.Gatilho.val.subscribe(function (novoValor) {
		_Irregularidade.PercentualTolerancia.visible(false)
		_Irregularidade.ValorTolerancia.visible(false)
		if (novoValor == EnumGatilhoIrregularidade.ValorPrestacaoServico || novoValor == EnumGatilhoIrregularidade.ValorTotalReceber) {
			_Irregularidade.ValorTolerancia.visible(true)
			_Irregularidade.PercentualTolerancia.visible(true)
		}
	})
}

var CRUDIrregularidade = function () {
	this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
	this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
	this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
	this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadIrregularidade() {

	_pesquisaIrregularidade = new PesquisaIrregularidade();
	KoBindings(_pesquisaIrregularidade, "knockoutPesquisaIrregularidade", false, _pesquisaIrregularidade.Pesquisar.id);

	_Irregularidade = new Irregularidade();
	KoBindings(_Irregularidade, "knockoutIrregularidade");

	HeaderAuditoria("Irregularidade", _Irregularidade);

	_CRUDIrregularidade = new CRUDIrregularidade();
	KoBindings(_CRUDIrregularidade, "knockoutCRUDIrregularidade");

	BuscarPortfolioModuloControle(_pesquisaIrregularidade.PortfolioModuloControle);
	BuscarPortfolioModuloControle(_Irregularidade.PortfolioModuloControle);

	loadGridIrregularidade();
}

function loadGridIrregularidade() {
	var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
	var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

	_gridIrregularidade = new GridView(_pesquisaIrregularidade.Pesquisar.idGrid, "Irregularidade/Pesquisa", _pesquisaIrregularidade, menuOpcoes
	);
	_gridIrregularidade.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
	if (!validarCampos()) {
		exibirMensagem(tipoMensagem.falha, "Falha", "Necessário selecionar um gatilho para cadastrar uma irregularidade!");
		return;
	}
		Salvar(_Irregularidade, "Irregularidade/Adicionar", function (retorno) {
			if (retorno.Success) {
				if (retorno.Data) {
					exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
					recarregarGridIrregularidade();
					limparCamposIrregularidade();
				}
				else
					exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
			}
			else
				exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
		}, sender);
}

function atualizarClick(e, sender) {
		if (!validarCampos()) {
			exibirMensagem(tipoMensagem.falha, "Falha", "Necessário selecionar um gatilho para cadastrar uma irregularidade!");
			return;
		}
		Salvar(_Irregularidade, "Irregularidade/Atualizar", function (retorno) {
			if (retorno.Success) {
				if (retorno.Data) {
					exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
					recarregarGridIrregularidade();
					limparCamposIrregularidade();
				}
				else
					exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
			}
			else
				exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
		}, sender);
}

function cancelarClick() {
	limparCamposIrregularidade();
}

function editarClick(registroSelecionado) {
	limparCamposIrregularidade();

	_Irregularidade.Codigo.val(registroSelecionado.Codigo);

	BuscarPorCodigo(_Irregularidade, "Irregularidade/BuscarPorCodigo", function (retorno) {
		if (retorno.Success) {
			if (retorno.Data) {
				_pesquisaIrregularidade.ExibirFiltros.visibleFade(false);

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
		ExcluirPorCodigo(_Irregularidade, "Irregularidade/ExcluirPorCodigo", function (retorno) {
			if (retorno.Success) {
				if (retorno.Data) {
					exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

					recarregarGridIrregularidade();
					limparCamposIrregularidade();
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
	_CRUDIrregularidade.Atualizar.visible(isEdicao);
	_CRUDIrregularidade.Excluir.visible(isEdicao);
	_CRUDIrregularidade.Cancelar.visible(isEdicao);
	_CRUDIrregularidade.Adicionar.visible(!isEdicao);
}

function limparCamposIrregularidade() {
	var isEdicao = false;

	controlarBotoesHabilitados(isEdicao);
	LimparCampos(_Irregularidade);
	exibirFiltros();
}

function recarregarGridIrregularidade() {
	_gridIrregularidade.CarregarGrid();
}

function exibirFiltros() {
	if (!_pesquisaIrregularidade.ExibirFiltros.visibleFade())
		_pesquisaIrregularidade.ExibirFiltros.visibleFade(true);
}

function validarCampos() {
	return _Irregularidade.Gatilho.val() == EnumGatilhoIrregularidade.NaoSelecionado ? false : true
}
// #endregion Métodos privados