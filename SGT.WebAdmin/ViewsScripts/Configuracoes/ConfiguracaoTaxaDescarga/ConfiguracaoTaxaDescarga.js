/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoAlerta.js" />
/// <reference path="../../Enumeradores/EnumConfiguracaoTaxaDescargaTipo.js" />
/// <reference path="../../Consultas/Usuario.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDConfiguracaoTaxaDescarga;
var _configuracaoTaxaDescarga;
var _configInteiro = { precision: 0, allowZero: false, thousands: "" }
var _pesquisaConfiguracaoTaxaDescarga;
var _gridConfiguracaoTaxaDescarga;
var _gridBasicConfiguracaoTaxaDescargaAjudantes;

/*
 * Declaração das Classes
 */

var CRUDConfiguracaoTaxaDescarga = function () {
	this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
	this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
	this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo" });
	this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var ConfiguracaoTaxaDescarga = function () {
	this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
	this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable(""), required: true, visible: true });
	this.Situacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.select, options: _status, val: ko.observable(""), def: "" });

	this.Tipo = PropertyEntity({ text: "Tipo", val: ko.observable(new Array()), getType: typesKnockout.select, visible: ko.observable(true), options: EnumConfiguracaoTaxaDescargaTipo.obterOpcoes(), required: true })
	this.QuantidadeInicial = PropertyEntity({ text: "Quantidade inicial", val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: "" } });
	this.QuantidadeFinal = PropertyEntity({ text: "Quantidade final", val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: "" } });
	this.QuantidadeAjudantes = PropertyEntity({ text: "Quantidade ajudantes", val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: "" } });

	this.ConfiguracaoTaxaDescargaAjudantes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
	this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoTaxaDescargaAjudantesClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });


	this.Codigo.val.subscribe(function (novoValor) {
		if (novoValor > 0) {
			_CRUDConfiguracaoTaxaDescarga.Atualizar.visible(true);
			_CRUDConfiguracaoTaxaDescarga.Excluir.visible(true);
			_CRUDConfiguracaoTaxaDescarga.Adicionar.visible(false);
		} else {
			_CRUDConfiguracaoTaxaDescarga.Atualizar.visible(false);
			_CRUDConfiguracaoTaxaDescarga.Excluir.visible(false);
			_CRUDConfiguracaoTaxaDescarga.Adicionar.visible(true);
		}
	});
}

var PesquisaConfiguracaoTaxaDescarga = function () {
	this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable(""), def: "", maxlentgh: 100 });
	this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(""), getType: typesKnockout.select, options: _statusPesquisa, def: "" });

	this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
	this.Pesquisar = PropertyEntity({ eventClick: recarregarGridConfiguracaoTaxaDescarga, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadConfiguracaoTaxaDescarga() {
	_configuracaoTaxaDescarga = new ConfiguracaoTaxaDescarga();
	KoBindings(_configuracaoTaxaDescarga, "knockoutConfiguracaoTaxaDescarga");

	_pesquisaConfiguracaoTaxaDescarga = new PesquisaConfiguracaoTaxaDescarga();
	KoBindings(_pesquisaConfiguracaoTaxaDescarga, "knockoutPesquisaConfiguracaoTaxaDescarga", false, _pesquisaConfiguracaoTaxaDescarga.Pesquisar.id);

	_CRUDConfiguracaoTaxaDescarga = new CRUDConfiguracaoTaxaDescarga();
	KoBindings(_CRUDConfiguracaoTaxaDescarga, "knockoutCRUDConfiguracaoTaxaDescarga");

	loadGridConfiguracaoTaxaDescarga();
	loadGridConfiguracaoTaxaDescargaAjudantes();
}

function loadGridConfiguracaoTaxaDescarga() {
	let opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
	let menuOpcoes = {
		tipo: TypeOptionMenu.link,
		opcoes: [opcaoEditar]
	};

	_gridConfiguracaoTaxaDescarga = new GridView(_pesquisaConfiguracaoTaxaDescarga.Pesquisar.idGrid, "ConfiguracaoTaxaDescarga/Pesquisa", _pesquisaConfiguracaoTaxaDescarga, menuOpcoes);
	_gridConfiguracaoTaxaDescarga.CarregarGrid();

}

function loadGridConfiguracaoTaxaDescargaAjudantes() {
	let linhasPorPaginas = 5;
	let opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerConfiguracaoTaxaDescargaItem, icone: "" };
	let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

	let header = [
		{ data: "Codigo", visible: false },
		{ data: "Tipo", visible: false},
		{ data: "TipoDescricao", title: "Tipo", width: "75%", className: "text-align-left" },
		{ data: "QuantidadeInicial", title: "Quantidade inicial", width: "75%", className: "text-align-left" },
		{ data: "QuantidadeFinal", title: "Quantidade final", width: "75%", className: "text-align-left" },
		{ data: "QuantidadeAjudantes", title: "Quantidade ajudantes", width: "75%", className: "text-align-left" }
	];

	_gridBasicConfiguracaoTaxaDescargaAjudantes = new BasicDataTable(_configuracaoTaxaDescarga.ConfiguracaoTaxaDescargaAjudantes.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

	_gridBasicConfiguracaoTaxaDescargaAjudantes.CarregarGrid([]);
}

function exibirFiltrosClick(e) {
	e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}


function recarregarGridConfiguracaoTaxaDescarga() {
	_gridConfiguracaoTaxaDescarga.CarregarGrid();
}

function editarClick(registroSelecionado) {
	LimparCamposConfiguracaoTaxaDescarga();

	_configuracaoTaxaDescarga.Codigo.val(registroSelecionado.Codigo);

	BuscarPorCodigo(_configuracaoTaxaDescarga, "ConfiguracaoTaxaDescarga/BuscarPorCodigo", function (retorno) {
		if (retorno.Success) {
			if (retorno.Data) {
				_gridBasicConfiguracaoTaxaDescargaAjudantes.CarregarGrid(retorno.Data.ConfiguracaoTaxaDescargaAjudantes)
			}
			else
				exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
		}
		else
			exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
	}, null);
}

function LimparCamposConfiguracaoTaxaDescarga() {
	LimparCampos(_configuracaoTaxaDescarga);
	_gridBasicConfiguracaoTaxaDescargaAjudantes.CarregarGrid([])

}


function removerConfiguracaoTaxaDescargaItem(registroSelecionado) {
	let lista = _gridBasicConfiguracaoTaxaDescargaAjudantes.BuscarRegistros();

	for (let i = 0; i < lista.length; i++) {
		if (registroSelecionado.Codigo == lista[i].Codigo) {
			lista.splice(i, 1)
		}
	}

	_gridBasicConfiguracaoTaxaDescargaAjudantes.CarregarGrid(lista)

}

function adicionarConfiguracaoTaxaDescargaAjudantesClick() {
	let valido = ValidarCamposObrigatorios(_configuracaoTaxaDescarga);
	if (valido) {
		let dadosGrid = {
			Codigo: guid(),
			TipoDescricao: EnumConfiguracaoTaxaDescargaTipo.obterDescricao(_configuracaoTaxaDescarga.Tipo.val()),
			Tipo: _configuracaoTaxaDescarga.Tipo.val(),
			QuantidadeInicial: _configuracaoTaxaDescarga.QuantidadeInicial.val(),
			QuantidadeFinal: _configuracaoTaxaDescarga.QuantidadeFinal.val(),
			QuantidadeAjudantes: _configuracaoTaxaDescarga.QuantidadeAjudantes.val(),
		}

		let registrosGrid = _gridBasicConfiguracaoTaxaDescargaAjudantes.BuscarRegistros();
		registrosGrid.push(dadosGrid)

		_gridBasicConfiguracaoTaxaDescargaAjudantes.CarregarGrid(registrosGrid);

		limparCamposTaxaDescargaAjudantes()
	}
}


function adicionarClick() {
	if (!ValidarCamposObrigatorios(_configuracaoTaxaDescarga))
		return;
	executarReST("ConfiguracaoTaxaDescarga/Adicionar", obterConfiguracaoTaxaDescargaSalvar(), function (retorno) {
		if (retorno.Success) {
			if (retorno.Data) {
				recarregarGridConfiguracaoTaxaDescarga();
				LimparCamposConfiguracaoTaxaDescarga();
				exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
			}
			else
				exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
		}
		else
			exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
	});
}

function excluirClick() {
	exibirConfirmacao("Confirmação", "Realmente deseja excluir esse registro?", function () {
		ExcluirPorCodigo(_configuracaoTaxaDescarga, "ConfiguracaoTaxaDescarga/ExcluirPorCodigo", function (retorno) {
			if (retorno.Success) {
				if (retorno.Data) {
					recarregarGridConfiguracaoTaxaDescarga();
					LimparCamposConfiguracaoTaxaDescarga();
					exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro excluído com sucesso");
				}
				else
					exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
			}
			else
				exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
		}, null);
	});
}

function atualizarClick() {
	if (!ValidarCamposObrigatorios(_configuracaoTaxaDescarga))
		return;
	executarReST("ConfiguracaoTaxaDescarga/Atualizar", obterConfiguracaoTaxaDescargaSalvar(), function (retorno) {
		if (retorno.Success) {
			if (retorno.Data) {
				recarregarGridConfiguracaoTaxaDescarga();
				LimparCamposConfiguracaoTaxaDescarga();
				exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
			}
			else
				exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
		}
		else
			exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
	});
}
function cancelarClick() {
	LimparCampos(_configuracaoTaxaDescarga);
	_gridBasicConfiguracaoTaxaDescargaAjudantes.CarregarGrid([])
}


function obterConfiguracaoTaxaDescargaSalvar() {
	let configuracaoTaxaDescarga = RetornarObjetoPesquisa(_configuracaoTaxaDescarga);

	configuracaoTaxaDescarga["dadosAjudantes"] = JSON.stringify(_gridBasicConfiguracaoTaxaDescargaAjudantes.BuscarRegistros())

	return configuracaoTaxaDescarga
}

function limparCamposTaxaDescargaAjudantes() {
	_configuracaoTaxaDescarga.QuantidadeAjudantes.val(0);
	_configuracaoTaxaDescarga.QuantidadeInicial.val(0);
	_configuracaoTaxaDescarga.QuantidadeFinal.val(0);
}