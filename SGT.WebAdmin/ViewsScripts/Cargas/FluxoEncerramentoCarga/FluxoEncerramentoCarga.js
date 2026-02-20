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
/// <reference path="FluxoEncerramentoCargaCIOT.js" />
/// <reference path="FluxoEncerramentoCargaMDFe.js" />
/// <reference path="FluxoEncerramentoCargaIntegracao.js" />
/// <reference path="FluxoEncerramentoCargaMDFe.js" />
/// <reference path="FluxoEncerramentoCargaResumo.js" />
/// <reference path="EtapaFluxoEncerramentoCarga.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEncerramentoCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFluxoEncerramentoCarga;
var _fluxoEncerramentoCarga;
var _CRUDFluxoEncerramentoCarga;
var _pesquisaFluxoEncerramentoCarga;

var PesquisaFluxoEncerramentoCarga = function () {
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga", val: ko.observable(""), def: "", maxlength: 50 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoEncerramentoCarga.Todas), options: EnumSituacaoEncerramentoCarga.ObterOpcoesPesquisa(), def: EnumSituacaoEncerramentoCarga.Todas });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFluxoEncerramentoCarga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var FluxoEncerramentoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Carga = PropertyEntity({ text: "*Carga:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "Motivo do Encerramento", maxlength: 500, required: false, enable: ko.observable(true) });

    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoEncerramentoCarga.Todas), options: EnumSituacaoEncerramentoCarga.ObterOpcoesPesquisa(), def: EnumSituacaoEncerramentoCarga.Todas });

    this.SituacaoMDFes = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoCIOT = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
    this.SituacaoIntegracoes = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoDocumentoCarga.Cancelando), def: EnumSituacaoCancelamentoDocumentoCarga.Cancelando, getType: typesKnockout.int });
};

var CRUDFluxoEncerramentoCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarEncerramentoCargaClick, type: types.event, text: ko.observable("Gerar Encerramento"), visible: ko.observable(true) });
    this.GerarNovoFluxoEncerramentoCarga = PropertyEntity({ eventClick: GerarNovoCancelamentoClick, type: types.event, text: "Gerar Novo Encerramento", visible: ko.observable(false) });
    this.ReenviarEncerramento = PropertyEntity({ eventClick: ReenviarEncerramentoClick, type: types.event, text: "Reenviar Encerramento", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadFluxoEncerramentoCarga() {
    _fluxoEncerramentoCarga = new FluxoEncerramentoCarga();
    KoBindings(_fluxoEncerramentoCarga, "knockoutDetalhesFluxoEncerramentoCarga");

    _CRUDFluxoEncerramentoCarga = new CRUDFluxoEncerramentoCarga();
    KoBindings(_CRUDFluxoEncerramentoCarga, "knockoutCRUDFluxoEncerramentoCarga");

    _pesquisaFluxoEncerramentoCarga = new PesquisaFluxoEncerramentoCarga();
    KoBindings(_pesquisaFluxoEncerramentoCarga, "knockoutPesquisaFluxoEncerramentoCarga", false, _pesquisaFluxoEncerramentoCarga.Pesquisar.id);

    HeaderAuditoria("CargaRegistroEncerramento", _fluxoEncerramentoCarga);

    new BuscarCargas(_fluxoEncerramentoCarga.Carga, RetornoConsultaCarga, null, null, null, null, null, null, null, null, true, null, null, null, [EnumSituacoesCarga.EmTransporte]);

    LoadEtapaFluxoEncerramentoCarga();
    LoadMDFe();
    LoadCIOT();
    LoadResumoFluxoEncerramentoCarga();
    LoadIntegracoesFluxoEncerramentoCarga();
    LoadSignalREncerramento();

    BuscarFluxoEncerramentoCarga();
}

function AdicionarEncerramentoCargaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, "Deseja mesmo iniciar o encerramento da carga?", function () {
        Salvar(_fluxoEncerramentoCarga, "FluxoEncerramentoCarga/GerarEncerramento", function (r) {
            if (r.Success) {
                if (r.Data) {
                    _fluxoEncerramentoCarga.Codigo.val(r.Data);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Encerramento solicitado com sucesso!");
                    BuscarFluxoEncerramentoCargaPorCodigo();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg, 10000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        }, sender);
    });
}

function RetornoConsultaCarga(data) {
    $("#msgInfoCancelamento").addClass("d-none");
    _fluxoEncerramentoCarga.Carga.codEntity(data.Codigo);
    _fluxoEncerramentoCarga.Carga.val(data.CodigoCargaEmbarcador);
    ObterDetalhesCarga(data.Codigo);
}

function ObterDetalhesCarga(codigoCarga) {
    executarReST("FluxoEncerramentoCarga/ValidarCarga", { Carga: codigoCarga }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!r.Data.EmTransporte) {
                    _CRUDFluxoEncerramentoCarga.Adicionar.visible(false);
                    $("#msgInfoCancelamento").text("Carga precisa estar em transporte para ser encerrada");
                    $("#msgInfoCancelamento").removeClass("d-none");
                } else {
                    _CRUDFluxoEncerramentoCarga.Adicionar.visible(true);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function GerarNovoCancelamentoClick(e) {
    LimparCamposFluxoEncerramentoCarga();
}

//*******MÉTODOS*******

function BuscarFluxoEncerramentoCarga() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: EditarCancelamento, tamanho: "7", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFluxoEncerramentoCarga = new GridViewExportacao(_pesquisaFluxoEncerramentoCarga.Pesquisar.idGrid, "FluxoEncerramentoCarga/Pesquisa", _pesquisaFluxoEncerramentoCarga, menuOpcoes, null, { column: 1, dir: orderDir.desc });
    _gridFluxoEncerramentoCarga.CarregarGrid();
}

function EditarCancelamento(cancelamentoGrid) {
    LimparCamposFluxoEncerramentoCarga();

    _fluxoEncerramentoCarga.Codigo.val(cancelamentoGrid.Codigo);

    BuscarFluxoEncerramentoCargaPorCodigo();
}

function BuscarFluxoEncerramentoCargaPorCodigo(exibirLoading) {
    if (exibirLoading == null)
        exibirLoading = true;

    if (!exibirLoading)
        _ControlarManualmenteProgresse = true;

    BuscarPorCodigo(_fluxoEncerramentoCarga, "FluxoEncerramentoCarga/BuscarPorCodigo", function (arg) {
        _ControlarManualmenteProgresse = false;

        _fluxoEncerramentoCarga.Codigo.val(arg.Data.Codigo);
        _fluxoEncerramentoCarga.Carga.codEntity(arg.Data.Carga.Codigo);
        _fluxoEncerramentoCarga.Carga.val(arg.Data.Carga.CodigoCargaEmbarcador);
        _fluxoEncerramentoCarga.Motivo.val(arg.Data.Motivo);
        _integracaoFluxoEncerramentoCarga.PossuiIntegracao.val(arg.Data.PossuiIntegracao);

        PreecherResumoFluxoEncerramentoCarga(arg.Data);
        ControlarCamposFluxoEncerramentoCarga();
    }, null, exibirLoading);
}

function ReenviarEncerramentoClick(e, sender) {
    executarReST("FluxoEncerramentoCarga/Reenviar", { Codigo: _fluxoEncerramentoCarga.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Encerramento reenviado com sucesso");
                BuscarFluxoEncerramentoCargaPorCodigo(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ControlarCamposFluxoEncerramentoCarga() {
    _pesquisaFluxoEncerramentoCarga.ExibirFiltros.visibleFade(false);

    _CRUDFluxoEncerramentoCarga.GerarNovoFluxoEncerramentoCarga.visible(true);
    _CRUDFluxoEncerramentoCarga.Adicionar.visible(false);

    if (_fluxoEncerramentoCarga.Situacao.val() == EnumSituacaoEncerramentoCarga.RejeicaoEncerramento)
        _CRUDFluxoEncerramentoCarga.ReenviarEncerramento.visible(true);
    else
        _CRUDFluxoEncerramentoCarga.ReenviarEncerramento.visible(false);

    _fluxoEncerramentoCarga.Motivo.enable(false);
    _fluxoEncerramentoCarga.Carga.enable(false);

    ConsultarCIOT();
    ConsultarMDFes();
    SetarEtapaFluxoEncerramentoCarga();
};

function LimparCamposFluxoEncerramentoCarga() {
    _fluxoEncerramentoCarga.Motivo.val("");
    _CRUDFluxoEncerramentoCarga.GerarNovoFluxoEncerramentoCarga.visible(false);
    _CRUDFluxoEncerramentoCarga.Adicionar.visible(true);
    _CRUDFluxoEncerramentoCarga.Adicionar.text("Gerar Encerramento");

    _fluxoEncerramentoCarga.Motivo.enable(true);
    _fluxoEncerramentoCarga.Carga.enable(true);

    $("#msgInfoCancelamento").addClass("d-none");

    LimparCampos(_fluxoEncerramentoCarga);
    LimparCamposCIOT();
    LimparCamposMDFe();
    LimparResumoFluxoEncerramentoCarga();
    SetarEtapaInicioFluxoEncerramentoCarga();
}

function BuscarIntegracoesFluxoEncerramentoCarga() {
    BuscarIntegracoesFluxoEncerramentoCargaIntegracaoCarga();
}

function BuscarDocumentosFluxoEncerramentoCarga() {
    ConsultarCIOT();
    ConsultarMDFes();
}

