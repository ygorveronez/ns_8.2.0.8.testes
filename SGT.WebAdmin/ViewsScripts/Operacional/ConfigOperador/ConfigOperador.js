/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="OperadorFilial.js" />
/// <reference path="OperadorTipoOperacao.js" />
/// <reference path="OperadorTipoCarga.js" />
/// <reference path="TabelaFrete.js" />
/// <reference path="GrupoPessoa.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="CentroCarregamento.js" />
/// <reference path="CentroDescarregamento.js" />
/// <reference path="OperadorCliente.js" />
/// <reference path="OperadorFilialVenda.js" />
/// <reference path="OperadorTransportador.js" />
/// <reference path="OperadorRecebedor.js" />
/// <reference path="OperadorExpedidor.js" />
/// <reference path="OperadorVendedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOperador;
var _operador;
var _pesquisaOperador;

var PesquisaOperador = function () {
    this.Nome = PropertyEntity({ text: Localization.Resources.Operacional.ConfigOperador.Nome.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1 });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOperador.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Operacional.ConfigOperador.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Operacional.ConfigOperador.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

var Operador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoOperadorLogistica = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.OperadorFiliais = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.OperadorTiposCarga = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.OperadorClientes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.OperadorTomadores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.TipoTransportador = PropertyEntity({ type: types.dynamic, val: ko.observable(new Array()), required: false });

    this.PossuiFiltroGrupoPessoas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VisualizaCargasSemGrupoPessoas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiFiltroTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TelaPedidosResumido = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteSelecionarHorarioEncaixe = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.VisualizaCargasSemTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegraRecebedorSeraSobrepostaNasDemais = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegraExpedidorSeraSobrepostaNasDemais = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.GrupoPessoas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CentrosCarregamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CentrosDescarregamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.FiliaisVenda = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Transportadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Recebedores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Expedidores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Vendedores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Operacional.ConfigOperador.Cancelar, visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Operacional.ConfigOperador.Salvar, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loaConfigOperador() {

    _operador = new Operador();
    KoBindings(_operador, "knockoutConfigOperador");

    _pesquisaOperador = new PesquisaOperador();
    KoBindings(_pesquisaOperador, "knockoutPesquisaConfigOperador", false, _pesquisaOperador.Pesquisar.id);

    HeaderAuditoria("OperadorLogistica", _operador, "CodigoOperadorLogistica");
    buscarOperadores();

    loadOperadorFiliais();
    loadTipoOperacao();
    loadOperadorTipoCarga();
    LoadCentroCarregamento();
    LoadCentroDescarregamento();
    loadGrupoPessoa();
    loadOperadorClientes();
    LoadFilialVenda();
    LoadTomadoresGestaoDocumentos();
    LoadTransportador();
    LoadRecebedor();
    LoadExpedidor();
    LoadVendedor();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liFiliais").show();
        $("#liFiliais").attr("class", "active");
        $("#knockoutOperadorFiliais").attr("class", "tab-pane fade active show");
        $("#liTipoCarga").show();
        $("#liCentroCarregamento").show();
        $("#liCentroDescarregamento").show();
        $("#liGrupoPessoa").show();
        $("#liTipoOperacao").show();
        $("#liCliente").show();
        $("#liFilialVenda").show();
        $("#liTomadoresGestaoDocumentos").show();
        $("#liTransportador").show();
        $("#liRecebedor").show();
        $("#liExpedidor").show();
        $("#liVendedor").show();
    } else {
        $("#liGrupoPessoa").show();
        $("#liGrupoPessoa").attr("class", "active");
        $("#knockoutGrupoPessoa").attr("class", "tab-pane fade active show");
    }
}

function atualizarClick(e, sender) {
    preencherListasSelecao();
    ObterRegraRecebedorSeraSobrepostaNasDemais();
    ObterRegraExpedidorSeraSobrepostaNasDemais();
    Salvar(e, "ConfigOperador/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Operacional.ConfigOperador.Sucesso, Localization.Resources.Operacional.ConfigOperador.AtualizadoSucesso);
                limparCamposConfigOperador();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Operacional.ConfigOperador.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Operacional.ConfigOperador.Falha, arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposConfigOperador();
}

function preencherListasSelecao() {
    _operador.GrupoPessoas.val(JSON.stringify(_grupoPessoa.Grupo.basicTable.BuscarRegistros()));
    _operador.TiposOperacao.val(JSON.stringify(_tipoOperacao.Operacao.basicTable.BuscarRegistros()));
    _operador.CentrosCarregamento.val(JSON.stringify(_centroCarregamento.CentroCarregamento.basicTable.BuscarRegistros()));
    _operador.PermiteSelecionarHorarioEncaixe.val(_centroCarregamento.PermiteSelecionarHorarioEncaixe.val());
    _operador.TelaPedidosResumido.val(_centroCarregamento.TelaPedidosResumido.val());
    _operador.CentrosDescarregamento.val(JSON.stringify(_centroDescarregamento.CentroDescarregamento.basicTable.BuscarRegistros()));
    _operador.FiliaisVenda.val(JSON.stringify(_filialVenda.FilialVenda.basicTable.BuscarRegistros()));
    _operador.Transportadores.val(JSON.stringify(_transportador.Transportador.basicTable.BuscarRegistros()));
    _operador.Recebedores.val(JSON.stringify(_recebedor.Recebedor.basicTable.BuscarRegistros()));
    _operador.Expedidores.val(JSON.stringify(_expedidor.Expedidor.basicTable.BuscarRegistros()));
    _operador.Vendedores.val(JSON.stringify(_vendedor.Vendedor.basicTable.BuscarRegistros()));

    _operador.VisualizaCargasSemGrupoPessoas.val(_grupoPessoa.VisualizaCargasSemGrupoPessoas.val());
    _operador.PossuiFiltroGrupoPessoas.val(_grupoPessoa.PossuiFiltroGrupoPessoas.val());
    _operador.PossuiFiltroTipoOperacao.val(_tipoOperacao.PossuiFiltroTipoOperacao.val());
    _operador.VisualizaCargasSemTipoOperacao.val(_tipoOperacao.VisualizaCargasSemTipoOperacao.val());
    _operador.OperadorTomadores.val(JSON.stringify(_gridTomador.BuscarRegistros()));
    _operador.TipoTransportador.val(JSON.stringify(_centroCarregamento.TipoTransportador.val()))
}

function editarOperador(e) {
    $("#wid-id-4").show();
    _operador.Codigo.val(e.Codigo);

    BuscarPorCodigo(_operador, "ConfigOperador/BuscarPorOperador", function (arg) {
        recarregarGridOperadorFilial();
        recarregarGridTipoOperacao();
        recarregarTiposCarga();
        recarregarGridGrupoPessoa();
        RecarregarGridCentroCarregamento();
        RecarregarGridCentroDescarregamento();
        recarregarGridOperadorCliente();
        recarregarGridOperadorTomador();
        RecarregarGridTransportador();
        RecarregarGridRecebedor();
        RecarregarGridExpedidor();
        RecarregarGridFilialVenda();
        RecarregarGridVendedor();
        _recebedor.RegraRecebedorSeraSobrepostaNasDemais.val(arg.Data.RegraRecebedorSeraSobrepostaNasDemais);
        _expedidor.RegraExpedidorSeraSobrepostaNasDemais.val(arg.Data.RegraExpedidorSeraSobrepostaNasDemais);
        var enumTipoTransportadorValues = arg.Data.TipoTransportador.map((x) => { return x.value; });
        _centroCarregamento.TipoTransportador.val(enumTipoTransportadorValues);
        $("#" + _centroCarregamento.TipoTransportador.id).selectpicker("val", enumTipoTransportadorValues);
        _pesquisaOperador.ExibirFiltros.visibleFade(false);
        _operador.Atualizar.visible(true);
        _operador.Cancelar.visible(true);

        $("#" + _centroCarregamento.TipoTransportador.id).trigger("change");
    });
}

//*******MÉTODOS*******

function buscarOperadores() {
    var editar = { descricao: Localization.Resources.Operacional.ConfigOperador.Configurar, id: "clasEditar", evento: "onclick", metodo: editarOperador, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridOperador = new GridView(_pesquisaOperador.Pesquisar.idGrid, "Usuario/PesquisarOperadores", _pesquisaOperador, menuOpcoes, null);
    _gridOperador.CarregarGrid();
}

function limparCamposConfigOperador() {
    _operador.Atualizar.visible(false);
    _operador.Cancelar.visible(false);
    _pesquisaOperador.ExibirFiltros.visibleFade(true);

    LimparCampos(_operador);
    limparCamposTipoOperacao();
    limparCamposGrupoPessoa();
    LimparCamposCentroCarregamento();
    LimparCamposCentroDescarregamento();
    LimparCamposFilialVenda();
    LimparCamposTransportador();
    LimparCamposRecebedor();
    LimparCamposExpedidor();
    LimparCamposVendedor();

    recarregarGridGrupoPessoa();
    recarregarGridTipoOperacao();
    recarregarGridOperadorCliente();
    recarregarGridOperadorTomador();
    RecarregarGridTransportador();
    RecarregarGridRecebedor();
    RecarregarGridExpedidor();
    RecarregarGridVendedor();

    $("#wid-id-4").hide();
    $("#divTiposCargaParent").hide();
    Global.ResetarAbas();
}