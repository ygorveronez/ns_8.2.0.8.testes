/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Financeiros/FichaCliente/FichaClienteLancamentos.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDFichaCliente;
var _pesquisaFichaCliente;
var _gridFichaCliente;
var _fichaCliente;

/*
 * Declaração das Classes
 */

var PesquisaFichaCliente = function () {

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente:", idBtnSearch: guid(), required: true });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(false) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridFichaCliente, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var CRUDFichaCliente = function () {

    this.Salvar = PropertyEntity({ type: types.event, eventClick: SalvarClick, text: "Salvar", visible: ko.observable(false), enable: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarEdicaoFichaClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
}

var FichaCliente = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.SaldoAtual = PropertyEntity({ text: "Saldo Atual: R$", val: ko.observable(0), getType: typesKnockout.decimal, maxlength: 18, visible: ko.observable(false) });
    this.ListaLancamentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridFichaCliente() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { EditarFichaClienteClick(item); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridFichaCliente = new GridView(_pesquisaFichaCliente.Pesquisar.idGrid, "FichaCliente/Pesquisa", _pesquisaFichaCliente, menuOpcoes);
    _gridFichaCliente.CarregarGrid();
}

function loadFichaCliente() {

    _fichaCliente = new FichaCliente();
    KoBindings(_fichaCliente, "knockoutFichaCliente");

    _pesquisaFichaCliente = new PesquisaFichaCliente();
    KoBindings(_pesquisaFichaCliente, "knockoutPesquisaFichaCliente", false, _pesquisaFichaCliente.Pesquisar.id);

    _CRUDFichaCliente = new CRUDFichaCliente();
    KoBindings(_CRUDFichaCliente, "knockoutCRUDFichaCliente");

    new BuscarClientes(_pesquisaFichaCliente.Cliente);
    new BuscarClientes(_fichaCliente.Cliente);

    loadGridFichaCliente();

    LoadFichaClienteLancamentos();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function SalvarClick(e, sender) {

    if (!ValidarCamposObrigatorios(_fichaCliente)) {
        return;
    }

    preencherListaLancamentos();

    Salvar(_fichaCliente, "FichaCliente/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");

                LimparTudo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarEdicaoFichaClick() {

    LimparTudo();
}

function AdicionarClick(e, sender) {

    if (!ValidarCamposObrigatorios(_fichaCliente)) {
        return;
    }

    preencherListaLancamentos();

    Salvar(_fichaCliente, "FichaCliente/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");

                recarregarGridFichaCliente();
                LimparCampos(_fichaCliente);
                LimparCamposFichaClienteLancamentos();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function EditarFichaClienteClick(registro) {

    LimparCamposFichaClienteLancamentos();
    _fichaCliente.Codigo.val(registro.Codigo);

    BuscarPorCodigo(_fichaCliente, "FichaCliente/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                var dados = retorno.Data;

                recarregarGridFichaClienteLancamentos(dados.Lancamentos);

                _pesquisaFichaCliente.ExibirFiltros.visibleFade(false);
                _fichaCliente.Visible.visibleFade(true);

                _CRUDFichaCliente.Adicionar.visible(false);
                _CRUDFichaCliente.Adicionar.enable(false);
                _CRUDFichaCliente.Cancelar.visible(true);
                _CRUDFichaCliente.Cancelar.enable(true);
                _CRUDFichaCliente.Salvar.visible(true);
                _CRUDFichaCliente.Salvar.enable(true);

                _fichaCliente.Cliente.enable(false);

                _fichaClienteLancamentos.Visible.visibleFade(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

/*
 * Declaração das Funções
 */

function recarregarGridFichaCliente() {
    _gridFichaCliente.CarregarGrid();
}

function preencherListaLancamentos() {
    _fichaCliente.ListaLancamentos.val(JSON.stringify(_fichaClienteLancamentos.FichaClienteLancamentos.val()));
}

function atualizarSaldo() {
    var saldo = parseFloat(0);

    var dataGrid = _fichaClienteLancamentos.FichaClienteLancamentos.val();

    for (var i = 0; i < dataGrid.length; i++) {
        if (dataGrid[i].Tipo == 'Entrada') {
            saldo += parseFloat(dataGrid[i].Valor.replace('.', '').replace(',', '.'));
        }
        else if (dataGrid[i].Tipo == 'Saída')
        {
            saldo -= parseFloat(dataGrid[i].Valor.replace('.', '').replace(',', '.'));
        }
    }

    _fichaCliente.SaldoAtual.val(saldo.toLocaleString('pt-br', { minimumFractionDigits: 2 }));
}

function LimparTudo()
{
    recarregarGridFichaCliente();
    LimparCampos(_fichaCliente);
    LimparCamposFichaClienteLancamentos();
    _fichaCliente.Cliente.enable(true);

    _CRUDFichaCliente.Salvar.visible(false);
    _CRUDFichaCliente.Salvar.enable(false);
    _CRUDFichaCliente.Cancelar.visible(false);
    _CRUDFichaCliente.Cancelar.enable(false);
    _CRUDFichaCliente.Adicionar.visible(true);
    _CRUDFichaCliente.Adicionar.enable(true);
}

