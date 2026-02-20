/// <reference path="ResumoCancelamento.js" />
/// <reference path="SignalRCancelamento.js" />
/// <reference path="ResumoCancelamento.js" />
/// <reference path="ResumoCancelamento.js" />
/// <reference path="MDFe.js" />
/// <reference path="EtapaCancelamento.js" />
/// <reference path="CTe.js" />
/// <reference path="EtapaCancelamento.js" />
/// <reference path="Integracao.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CTeComplementar.js" />
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="EtapaCancelamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCancelamentoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCancelamentoPedido;
var _cancelamentoPedido;
var _CRUDCancelamentoPedido;
var _pesquisaCancelamentoPedido;

var PesquisaCancelamento = function () {
    this.NumeroPedido = PropertyEntity({ text: "Nº Pedido:", val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº Pedido Embarcador:", val: ko.observable(""), def: "" });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCancelamentoPedido.CarregarGrid();
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
}

var PedidoCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pedido:", issue: 0, idBtnSearch: guid(), eventChange: PedidoBlur, enable: ko.observable(true) });
    this.DataCancelamento = PropertyEntity({ text: "*Data do Cancelamento:", issue: 0, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), enable: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: "*Usuário que Solicitou:", issue: 0, enable: ko.observable(false) });
    this.MotivoCancelamento = PropertyEntity({ text: "*Motivo do Cancelamento:", issue: 0, maxlength: 255, required: true, enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "Tipo:", val: ko.observable(EnumTipoPedidoCancelamento.Cancelamento), def: EnumTipoPedidoCancelamento.Cancelamento, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoPedidoCancelamento.Cancelado), def: EnumSituacaoPedidoCancelamento.Cancelado, getType: typesKnockout.int });
}

var CRUDPedidoCancelamento = function () {
    this.DesistenciaCarga = PropertyEntity({ eventClick: DesistenciaCargaClick, type: types.event, text: "Desistência da Carga", visible: ko.observable(false) });
    this.DesistenciaCarregamento = PropertyEntity({ eventClick: DesistenciaCarregamentoClick, type: types.event, text: "Desistência no Carregamento", visible: ko.observable(false) });
    this.Cancelamento = PropertyEntity({ eventClick: CancelamentoPedidoClick, type: types.event, text: "Cancelamento do Pedido", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: GerarNovoCancelamentoClick, type: types.event, text: "Gerar Novo Cancelamento", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadPedidoCancelamento() {

    _cancelamentoPedido = new PedidoCancelamento();
    KoBindings(_cancelamentoPedido, "knockoutPedidoCancelamento");

    _CRUDCancelamentoPedido = new CRUDPedidoCancelamento();
    KoBindings(_CRUDCancelamentoPedido, "knockoutCRUDPedidoCancelamento");

    _pesquisaCancelamentoPedido = new PesquisaCancelamento();
    KoBindings(_pesquisaCancelamentoPedido, "knockoutPesquisaPedidoCancelamento", false, _pesquisaCancelamentoPedido.Pesquisar.id);

    HeaderAuditoria("PedidoCancelamento", _cancelamentoPedido);

    new BuscarClientes(_pesquisaCancelamentoPedido.Remetente);
    new BuscarClientes(_pesquisaCancelamentoPedido.Destinatario);
    new BuscarGruposPessoas(_pesquisaCancelamentoPedido.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarPedidos(_cancelamentoPedido.Pedido, RetornoConsultaPedido);

    BuscarCancelamentosPedidos();

    ObterDadosGeraisPedidoCancelamento();
}

function ObterDadosGeraisPedidoCancelamento() {
    executarReST("PedidoCancelamento/ObterDadosGerais", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                _cancelamentoPedido.Usuario.val(r.Data.Usuario);
                _cancelamentoPedido.Usuario.def = r.Data.Usuario;
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function DesistenciaCargaClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente realizar a <b>DESISTÊNCIA DA CARGA</b>? Este processo é irreversível!", function () {
        _cancelamentoPedido.Tipo.val(EnumTipoPedidoCancelamento.DesistenciaCarga);
        Salvar(_cancelamentoPedido, "PedidoCancelamento/Adicionar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Desistência da carga realizado com sucesso!");
                    LimparCamposCancelamentoPedido();
                    _gridCancelamentoPedido.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    });
}

function DesistenciaCarregamentoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente realizar a <b>DESISTÊNCIA NO CARREGAMENTO</b>? Este processo é irreversível!", function () {
        _cancelamentoPedido.Tipo.val(EnumTipoPedidoCancelamento.DesistenciaCarregamento);
        Salvar(_cancelamentoPedido, "PedidoCancelamento/Adicionar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Desistência no carregamento realizado com sucesso!");
                    LimparCamposCancelamentoPedido();
                    _gridCancelamentoPedido.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    });
}

function CancelamentoPedidoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente realizar o <b>CANCELAMENTO DO PEDIDO</b>? Este processo é irreversível!", function () {
        _cancelamentoPedido.Tipo.val(EnumTipoPedidoCancelamento.Cancelamento);
        Salvar(_cancelamentoPedido, "PedidoCancelamento/Adicionar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento do pedido realizado com sucesso!");
                    LimparCamposCancelamentoPedido();
                    _gridCancelamentoPedido.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    });
}

function PedidoBlur() {
    if (_cancelamentoPedido.Pedido.val() == "")
        _cancelamentoPedido.Pedido.codEntity(0);
}

function RetornoConsultaPedido(data) {
    $("#msgInfoCancelamento").addClass("hidden");
    _cancelamentoPedido.Pedido.codEntity(data.Codigo);
    _cancelamentoPedido.Pedido.val(data.Numero);
    ObterDetalhesPedidoCancelamento(data.Codigo);
}

function ObterDetalhesPedidoCancelamento(codigoPedido) {
    executarReST("PedidoCancelamento/ValidarPedido", { Pedido: codigoPedido }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (r.Data.PermiteCancelamento) {

                    if (r.Data.TiposCancelamentosPermitidos.some(function (item) { return item == EnumTipoPedidoCancelamento.Cancelamento }))
                        _CRUDCancelamentoPedido.Cancelamento.visible(true);

                    if (r.Data.TiposCancelamentosPermitidos.some(function (item) { return item == EnumTipoPedidoCancelamento.DesistenciaCarga }))
                        _CRUDCancelamentoPedido.DesistenciaCarga.visible(true);

                    if (r.Data.TiposCancelamentosPermitidos.some(function (item) { return item == EnumTipoPedidoCancelamento.DesistenciaCarregamento }))
                        _CRUDCancelamentoPedido.DesistenciaCarregamento.visible(true);

                } else {

                    _CRUDCancelamentoPedido.Cancelamento.visible(false);
                    _CRUDCancelamentoPedido.DesistenciaCarga.visible(false);
                    _CRUDCancelamentoPedido.DesistenciaCarregamento.visible(false);

                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Data.Mensagem);

                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function GerarNovoCancelamentoClick(e) {
    LimparCamposCancelamentoPedido();
}

//*******MÉTODOS*******

function BuscarCancelamentosPedidos() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarCancelamentoPedido, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCancelamentoPedido = new GridView(_pesquisaCancelamentoPedido.Pesquisar.idGrid, "PedidoCancelamento/Pesquisa", _pesquisaCancelamentoPedido, menuOpcoes, { column: 2, dir: orderDir.desc }, null);

    _gridCancelamentoPedido.CarregarGrid();
}

function EditarCancelamentoPedido(cancelamentoGrid) {
    LimparCamposCancelamentoPedido();

    _cancelamentoPedido.Codigo.val(cancelamentoGrid.Codigo);

    BuscarCancelamentoPorCodigo();
}

function BuscarCancelamentoPorCodigo() {
    BuscarPorCodigo(_cancelamentoPedido, "PedidoCancelamento/BuscarPorCodigo", function (arg) {
        PreecherCamposEdicaoCancelamentoPedido();
    });
}

function PreecherCamposEdicaoCancelamentoPedido() {
    _pesquisaCancelamentoPedido.ExibirFiltros.visibleFade(false);

    _CRUDCancelamentoPedido.Limpar.visible(true);

    _cancelamentoPedido.MotivoCancelamento.enable(false);
    _cancelamentoPedido.Pedido.enable(false);
}

function LimparCamposCancelamentoPedido() {
    _CRUDCancelamentoPedido.Limpar.visible(false);
    _CRUDCancelamentoPedido.Cancelamento.visible(false);
    _CRUDCancelamentoPedido.DesistenciaCarga.visible(false);
    _CRUDCancelamentoPedido.DesistenciaCarregamento.visible(false);

    _cancelamentoPedido.MotivoCancelamento.enable(true);
    _cancelamentoPedido.Pedido.enable(true);

    $("#msgInfoCancelamento").addClass("hidden");

    LimparCampos(_cancelamentoPedido);
}