/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />
/// <reference path="../../Consultas/Carga.js" />


// #region Objetos Globais do Arquivo

let _gridContrato;
let _pesquisaContrato;
let _movimento;
let _gridMovimento;

// #endregion Objetos Globais do Arquivo

// #region Classes

let PesquisaContrato = function () {
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.FiltrosPesquisa.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.NumeroContrato = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.NumeroContrato.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Carga.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Transportador.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Cliente.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.DataInicioContrato = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.DataInicioContrato.getFieldDescription() });
    this.DataFimContrato = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.DataFimContrato.getFieldDescription() });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridContrato, type: types.event, text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Pesquisar.getFieldDescription(), idGrid: guid(), visible: ko.observable(true) });
}

let Movimento = function () {
    this.Codigo = PropertyEntity({ type: types.int, val: ko.observable(0) });
    this.Pesquisar = PropertyEntity({ idGrid: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Cliente.getFieldDescription(), visible: ko.observable(true) });
    this.SaldoFinalContrato = PropertyEntity({ text: "Saldo Final Contrato:", val: ko.observable(""), def: "", maxlentgh: 50, visible: ko.observable(false), enable: ko.observable(false) });
    this.FecharContrato = PropertyEntity({ eventClick: FecharContratoClick, type: types.event, text: "Fechar Contrato", visible: ko.observable(false) });
    this.ContratoFechado = PropertyEntity({ type: types.bool, val: ko.observable(0) });
    this.Status = PropertyEntity({ text: Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Status.getFieldDescription(), val: ko.observable("Aberto"), visible: ko.observable(false), enable: false });
}

// #endregion Classes

// #region Funções de Inicialização

function loadMovimentoContrato() {

    $("#divMovimento").hide();

    _pesquisaContrato = new PesquisaContrato();
    KoBindings(_pesquisaContrato, "knockoutPesquisaContrato", false, _pesquisaContrato.Pesquisar.id);

    _movimento = new Movimento();
    KoBindings(_movimento, "knockoutMovimento", false, _movimento.Pesquisar.id);

    HeaderAuditoria("ContratoFreteCliente", _pesquisaContrato);

    loadGridContrato();
    loadGridMovimento();
    loadBuscaModais();

}

function loadGridContrato() {
    let opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridContrato = new GridView(_pesquisaContrato.Pesquisar.idGrid, "SaldoContratoFreteCliente/Pesquisa", _pesquisaContrato, menuOpcoes);
    _gridContrato.CarregarGrid();
}

function loadGridMovimento() {

    _gridMovimento = new GridView(_movimento.Pesquisar.idGrid, "SaldoContratoFreteCliente/PesquisaExtrato", _movimento);
}

function loadBuscaModais() {
    BuscarClientes(_pesquisaContrato.Cliente, null);
    BuscarCargas(_pesquisaContrato.Carga, null);
    BuscarTransportadores(_pesquisaContrato.Transportador, null);
    BuscarContratoFreteCliente(_pesquisaContrato.NumeroContrato, null);
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function editarClick(registroSelecionado) {
    $("#divMovimento").show();
    _movimento.Codigo.val(registroSelecionado.Codigo);
    _gridMovimento.CarregarGrid();
    BuscarPorCodigo(_movimento, "SaldoContratoFreteCliente/BuscarPorCodigo", setarCampos);
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

// #endregion Funções Associadas a Eventos

// #region Métodos privados

function exibirFiltros() {
    if (!_pesquisaContrato.ExibirFiltros.visibleFade())
        _pesquisaContrato.ExibirFiltros.visibleFade(true);
}

function recarregarGridContrato() {
    _gridContrato.CarregarGrid();
}

function FecharContratoClick() {
    executarReST("SaldoContratoFreteCliente/FecharContrato", { Codigo: _movimento.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato de Frete Cliente Fechado.");
                _movimento.Status.val("Fechado");
                _movimento.FecharContrato.visible(false);
                _movimento.SaldoFinalContrato.visible(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function setarCampos(e) {
    _movimento.FecharContrato.visible(!e.Data.ContratoFechado && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Alterar, _PermissoesPersonalizadas)));
    _movimento.SaldoFinalContrato.visible(e.Data.ContratoFechado);

    _movimento.Status.visible(true);
    _movimento.Status.val(e.Data.Status);
}

// #endregion Métodos privados