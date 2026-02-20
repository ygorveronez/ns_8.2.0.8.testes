/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="ResumoCancelamento.js" />
/// <reference path="SignalRCancelamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
/// <reference path="ResumoCancelamento.js" />
/// <reference path="ResumoCancelamento.js" />
/// <reference path="NFS.js" />
/// <reference path="EtapaCancelamento.js" />
/// <reference path="CTe.js" />
/// <reference path="EtapaCancelamento.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoNFSManualCancelamento.js" />
/// <reference path="../../Enumeradores/EnumTipoNFSManualCancelamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCancelamento;
var _cancelamento;
var _CRUDCancelamento;
var _pesquisaCancelamento;

var _situacaoCancelamentoNFSManual = [
    { text: "Todas", value: EnumSituacaoNFSManualCancelamento.todos },
    { text: "Em Cancelamento", value: EnumSituacaoNFSManualCancelamento.EmCancelamento },
    { text: "Rejeição no Cancelamento", value: EnumSituacaoNFSManualCancelamento.CancelamentoRejeitado },
    { text: "Cancelada", value: EnumSituacaoNFSManualCancelamento.Cancelada }
];


var PesquisaCancelamento = function () {

    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Prestação:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.Numero = PropertyEntity({ text: "Número da NFS:", maxlength: 12, enable: ko.observable(true) });
    this.numeroDOC = PropertyEntity({ text: "Número da NF-e:", maxlength: 12, enable: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoNFSManualCancelamento.todos), options: _situacaoCancelamentoNFSManual, def: EnumSituacaoNFSManualCancelamento.todos, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCancelamento.CarregarGrid();
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

var Cancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LancamentoNFSManual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*NFS Manual:", idBtnSearch: guid(), eventChange: LancamentoNFSManualBlur, enable: ko.observable(true) });
    this.DataCancelamento = PropertyEntity({ text: "*Data do Cancelamento:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(false) });
    this.UsuarioSolicitou = PropertyEntity({ text: "*Usuário que Solicitou:", enable: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: "*Motivo do Cancelamento:", issue: 632, maxlength: 255, required: true, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoNFSManualCancelamento.todos), def: EnumSituacaoNFSManualCancelamento.todos, getType: typesKnockout.int });
}

var CRUDCancelamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: ko.observable("Gerar Cancelamento"), visible: ko.observable(true) });
    this.GerarNovoCancelamento = PropertyEntity({ eventClick: GerarNovoCancelamentoClick, type: types.event, text: "Gerar Novo Cancelamento", visible: ko.observable(false) });
    this.ReenviarCancelamento = PropertyEntity({ eventClick: ReenviarCancelamentoClick, type: types.event, text: "Reenviar Cancelamento", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadCancelamento() {

    _cancelamento = new Cancelamento();
    KoBindings(_cancelamento, "knockoutCadastroCancelamento");

    HeaderAuditoria("NFSManualCancelamento", _cancelamento);

    _CRUDCancelamento = new CRUDCancelamento();
    KoBindings(_CRUDCancelamento, "knockoutCRUDCancelamento");
    _pesquisaCancelamento = new PesquisaCancelamento();

    KoBindings(_pesquisaCancelamento, "knockoutPesquisaCancelamento", false, _pesquisaCancelamento.Pesquisar.id);
    new BuscarTransportadores(_pesquisaCancelamento.Empresa);
    BuscarLocalidadesBrasil(_pesquisaCancelamento.LocalidadePrestacao, "Buscar Local da Prestação", "Localidades");
    new BuscarClientes(_pesquisaCancelamento.Tomador);
    BuscarCargas(_pesquisaCancelamento.Carga, null, null, null, null, null, null, null, null, true);

    BuscarLancamentoNFSManualParaCancelamento(_cancelamento.LancamentoNFSManual, RetornoLancamentoNFSManual);

    LoadEtapaCancelamento();
    LoadNFS();
    LoadResumoCancelamento();

    LoadConexaoSignalRNFSManualCancelamento();

    BuscarCancelamentos();

    ObterDadosGerais();
}

function ObterDadosGerais() {
    executarReST("NFSManualCancelamento/ObterDadosGerais", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                _cancelamento.UsuarioSolicitou.val(r.Data.Usuario);
                _cancelamento.UsuarioSolicitou.def = r.Data.Usuario;
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AdicionarClick(e, sender) {
    Salvar(_cancelamento, "NFSManualCancelamento/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "solicitação de cancelamento realizada com sucesso!");
                _cancelamento.Codigo.val(r.Data);
                BuscarCancelamentoPorCodigo();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, sender);
}

function ReenviarCancelamentoClick(e, sender) {
    executarReST("NFSManualCancelamento/Reenviar", { Codigo: _cancelamento.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento reenviado com sucesso!");
                BuscarCancelamentoPorCodigo(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LancamentoNFSManualBlur() {
    if (_cancelamento.LancamentoNFSManual.val() == "")
        _cancelamento.LancamentoNFSManual.codEntity(0);
}

function RetornoLancamentoNFSManual(data) {
    _cancelamento.LancamentoNFSManual.codEntity(data.Codigo);
    _cancelamento.LancamentoNFSManual.val(data.Descricao);
    ObterDetalhesCarga(data.Codigo);
}

function ObterDetalhesCarga(codigoLancamentoNFSManual) {
    executarReST("NFSManualCancelamento/ValidarCarga", { LancamentoNFSManual: codigoLancamentoNFSManual }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!r.Data.NFSPermiteCancelamento) {
                    $("#msgInfoCancelamento").text("As NFS-e já excederam o prazo para cancelamento.");
                    $("#msgInfoCancelamento").removeClass("hidden");
                    _CRUDCancelamento.Adicionar.visible(false);
                    _CRUDCancelamento.GerarNovoCancelamento.visible(true);
                } else {
                    _CRUDCancelamento.Adicionar.visible(true);
                    _CRUDCancelamento.GerarNovoCancelamento.visible(false);
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
    LimparCamposCancelamento();
}

//*******MÉTODOS*******

function BuscarCancelamentos() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarCancelamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCancelamento = new GridView(_pesquisaCancelamento.Pesquisar.idGrid, "NFSManualCancelamento/Pesquisa", _pesquisaCancelamento, menuOpcoes, { column: 0, dir: orderDir.desc }, null);

    _gridCancelamento.CarregarGrid();
}

function EditarCancelamento(cancelamentoGrid) {
    LimparCamposCancelamento();

    _cancelamento.Codigo.val(cancelamentoGrid.Codigo);

    BuscarCancelamentoPorCodigo();
}

function BuscarCancelamentoPorCodigo(exibirLoading) {

    if (exibirLoading == null)
        exibirLoading = true;

    if (!exibirLoading)
        _ControlarManualmenteProgresse = true;

    BuscarPorCodigo(_cancelamento, "NFSManualCancelamento/BuscarPorCodigo", function (arg) {
        _ControlarManualmenteProgresse = false;
        _cancelamento.LancamentoNFSManual.codEntity(arg.Data.LancamentoNFSManual.Codigo);
        _cancelamento.LancamentoNFSManual.val(arg.Data.LancamentoNFSManual.Descricao);
        PreecherResumoCancelamento(arg.Data);
        PreecherCamposEdicaoCancelamento();
    }, null, exibirLoading);
}

function PreecherCamposEdicaoCancelamento() {
    _pesquisaCancelamento.ExibirFiltros.visibleFade(false);
    _CRUDCancelamento.GerarNovoCancelamento.visible(true);
    _CRUDCancelamento.Adicionar.visible(false);
    _CRUDCancelamento.ReenviarCancelamento.visible(false);

    if (_cancelamento.Situacao.val() == EnumSituacaoNFSManualCancelamento.CancelamentoRejeitado)
        _CRUDCancelamento.ReenviarCancelamento.visible(true);


    _cancelamento.Motivo.enable(false);
    _cancelamento.LancamentoNFSManual.enable(false);

    ConsultarNFSsCarga();

    SetarEtapaCancelamento();
}

function LimparCamposCancelamento() {
    _CRUDCancelamento.GerarNovoCancelamento.visible(false);
    _CRUDCancelamento.Adicionar.visible(true);

    _CRUDCancelamento.ReenviarCancelamento.visible(false);
    _cancelamento.Motivo.enable(true);
    _cancelamento.LancamentoNFSManual.enable(true);

    LimparCampos(_cancelamento);
    LimparCamposNFS();
    LimparResumoCancelamento();
    SetarEtapaInicioCancelamento();
}