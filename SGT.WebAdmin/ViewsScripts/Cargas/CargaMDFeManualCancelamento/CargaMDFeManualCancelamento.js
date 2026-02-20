/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="ResumoCancelamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
/// <reference path="ResumoCancelamento.js" />
/// <reference path="ResumoCancelamento.js" />
/// <reference path="MDFe.js" />
/// <reference path="EtapaCancelamento.js" />
/// <reference path="EtapaCancelamento.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="EtapaCancelamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCancelamento;
var _cancelamento;
var _CRUDCancelamento;
var _pesquisaCancelamento;

var _situacaoCancelamentoMDFeManual = [
    { text: "Todas", value: EnumSituacaoMDFeManualCancelamento.todos },
    { text: "Em Cancelamento", value: EnumSituacaoMDFeManualCancelamento.EmCancelamento },
    { text: "Rejeição no Cancelamento", value: EnumSituacaoMDFeManualCancelamento.CancelamentoRejeitado },
    { text: "Cancelada", value: EnumSituacaoMDFeManualCancelamento.Cancelada },
    { text: "Processando Integração", value: EnumSituacaoMDFeManualCancelamento.ProcessandoIntegracao }
];


var PesquisaCancelamento = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.MDFe = PropertyEntity({ text: "Número MDF-e:", maxlength: 12, enable: ko.observable(true) });
    this.CTe = PropertyEntity({ text: "Número CT-e:", maxlength: 12, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoMDFeManualCancelamento.todos), options: _situacaoCancelamentoMDFeManual, def: EnumSituacaoMDFeManualCancelamento.todos, text: "Situação: " });

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
    this.CargaMDFeManual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*MDF-e Manual:", idBtnSearch: guid(), eventChange: CargaMDFeManualBlur, enable: ko.observable(true) });
    this.DataCancelamento = PropertyEntity({ text: "*Data do Cancelamento:", issue: 630, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(false) });
    this.UsuarioSolicitou = PropertyEntity({ text: "*Usuário que Solicitou:", issue: 631, enable: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: "*Motivo do Cancelamento:", issue: 632, maxlength: 255, required: true, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoMDFeManualCancelamento.todos), def: EnumSituacaoMDFeManualCancelamento.todos, getType: typesKnockout.int });

    this.Motivo.val.subscribe(function (novoValor) {
        const regex = new RegExp(_CONFIGURACAO_TMS.ReplaceMotivoRegexPattern, "gi");

        // Remove os caracteres não permitidos
        _cancelamento.Motivo.val(novoValor.replace(regex, ""));
    });
}

var CRUDCancelamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: ko.observable("Gerar Cancelamento"), visible: ko.observable(true) });
    this.GerarNovoCancelamento = PropertyEntity({ eventClick: GerarNovoCancelamentoClick, type: types.event, text: "Gerar Novo Cancelamento", visible: ko.observable(false) });
    this.ReenviarCancelamento = PropertyEntity({ eventClick: ReenviarCancelamentoClick, type: types.event, text: "Reenviar Cancelamento", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadCancelamento() {
    ConsultarIntegracaoMDFeAquaviarioManual().then(function () {

        _cancelamento = new Cancelamento();
        KoBindings(_cancelamento, "knockoutCadastroCancelamento");

        _CRUDCancelamento = new CRUDCancelamento();
        KoBindings(_CRUDCancelamento, "knockoutCRUDCancelamento");

        _pesquisaCancelamento = new PesquisaCancelamento();
        KoBindings(_pesquisaCancelamento, "knockoutPesquisaCancelamento", false, _pesquisaCancelamento.Pesquisar.id);

        HeaderAuditoria("CargaMDFeManualCancelamento", _cancelamento);

        new BuscarTransportadores(_pesquisaCancelamento.Empresa);
        BuscarLocalidadesBrasil(_pesquisaCancelamento.Origem, "Buscar Origem", "Origens");
        BuscarLocalidadesBrasil(_pesquisaCancelamento.Destino, "Buscar Destino", "Destinos");
        BuscarVeiculos(_pesquisaCancelamento.Veiculo);
        BuscarMotoristas(_pesquisaCancelamento.Motorista);
        BuscarCargas(_pesquisaCancelamento.Carga);

        BuscarCargaMDFeManualParaCancelamento(_cancelamento.CargaMDFeManual, RetornoCargaMDFeManual)

        LoadEtapaCancelamento();
        LoadMDFe();
        LoadResumoCancelamento();
        loadMDFeManualCancelamentoIntegracoes();

        LoadConexaoSignalRCargaMDFeManualCancelamento();

        BuscarCancelamentos();

        ObterDadosGerais();

        ValidarMDFeAquaviarioTelaCarga();
    });
}

function ObterDadosGerais() {
    executarReST("CargaMDFeManualCancelamento/ObterDadosGerais", {}, function (r) {
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
    Salvar(_cancelamento, "CargaMDFeManualCancelamento/Adicionar", function (r) {
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
    executarReST("CargaMDFeManualCancelamento/Reenviar", { Codigo: _cancelamento.Codigo.val() }, function (r) {
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

function CargaMDFeManualBlur() {
    if (_cancelamento.CargaMDFeManual.val() == "")
        _cancelamento.CargaMDFeManual.codEntity(0);
}

function RetornoCargaMDFeManual(data) {
    _cancelamento.CargaMDFeManual.codEntity(data.Codigo);
    _cancelamento.CargaMDFeManual.val(data.Descricao);
    ObterDetalhesCarga(data.Codigo);
}

function ObterDetalhesCarga(codigoCargaMDFeManual) {
    executarReST("CargaMDFeManualCancelamento/ValidarCarga", { CargaMDFeManual: codigoCargaMDFeManual }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!r.Data.MDFePermiteCancelamento) {
                    $("#msgInfoCancelamento").text("Os MDF-es já excederam o prazo para cancelamento.");
                    $("#msgInfoCancelamento").removeClass("d-none");
                    _CRUDCancelamento.Adicionar.visible(false);
                    _CRUDCancelamento.GerarNovoCancelamento.visible(true);
                } else {
                    $("#msgInfoCancelamento").addClass("d-none");
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

    _gridCancelamento = new GridView(_pesquisaCancelamento.Pesquisar.idGrid, "CargaMDFeManualCancelamento/Pesquisa", _pesquisaCancelamento, menuOpcoes, { column: 0, dir: orderDir.desc }, null);

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

    BuscarPorCodigo(_cancelamento, "CargaMDFeManualCancelamento/BuscarPorCodigo", function (arg) {
        _ControlarManualmenteProgresse = false;
        _cancelamento.CargaMDFeManual.codEntity(arg.Data.CargaMDFeManual.Codigo);
        _cancelamento.CargaMDFeManual.val(arg.Data.CargaMDFeManual.Descricao);
        PreecherResumoCancelamento(arg.Data);
        PreecherCamposEdicaoCancelamento();
    }, null, exibirLoading);
}

function PreecherCamposEdicaoCancelamento() {
    _pesquisaCancelamento.ExibirFiltros.visibleFade(false);
    _CRUDCancelamento.GerarNovoCancelamento.visible(true);
    _CRUDCancelamento.Adicionar.visible(false);
    _CRUDCancelamento.ReenviarCancelamento.visible(false);

    if (_cancelamento.Situacao.val() == EnumSituacaoMDFeManualCancelamento.CancelamentoRejeitado)
        _CRUDCancelamento.ReenviarCancelamento.visible(true);

    _cancelamento.Motivo.enable(false);
    _cancelamento.CargaMDFeManual.enable(false);

    ConsultarMDFesCarga();

    SetarEtapaCancelamento();
}

function LimparCamposCancelamento() {
    _CRUDCancelamento.GerarNovoCancelamento.visible(false);
    _CRUDCancelamento.Adicionar.visible(true);

    _CRUDCancelamento.ReenviarCancelamento.visible(false);
    _cancelamento.Motivo.enable(true);
    _cancelamento.CargaMDFeManual.enable(true);

    LimparCampos(_cancelamento);
    LimparCamposMDFe();
    LimparResumoCancelamento();
    SetarEtapaInicioCancelamento();
}

function ValidarMDFeAquaviarioTelaCarga() {
    var codigoMDFE = CODIGO_MDFE_AQUAVIARIO_PARA_CANCELAMENTO_TELA_CARGA.codEntity;

    if (codigoMDFE > 0) {
        _cancelamento.CargaMDFeManual.codEntity(codigoMDFE);
        _cancelamento.CargaMDFeManual.val(CODIGO_MDFE_AQUAVIARIO_PARA_CANCELAMENTO_TELA_CARGA.val());
        _cancelamento.CargaMDFeManual.enable(false);
    }
}