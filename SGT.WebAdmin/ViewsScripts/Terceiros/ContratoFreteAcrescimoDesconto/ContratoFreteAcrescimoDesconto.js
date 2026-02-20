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
/// <reference path="../../Consultas/ContratoFrete.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Enumeradores/EnumTipoFinalidadeJustificativa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFreteAcrescimoDesconto.js" />
/// <reference path="Pesquisa.js" />
/// <reference path="Etapa.js" />
/// <reference path="Aprovacao.js" />
/// <reference path="Integracao.js" />
/// <reference path="ContratoFreteAcrescimoDescontoAnexos.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoFreteAcrescimoDesconto;
var _CRUDContratoFreteAcrescimoDesconto;
var _PermissoesPersonalizadas;

var ContratoFreteAcrescimoDesconto = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoContratoFreteAcrescimoDesconto.Todos), def: EnumSituacaoContratoFreteAcrescimoDesconto.Todos });

    this.ContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Contrato de Frete:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, required: true, enable: ko.observable(true) });
    this.DisponibilizarFechamentoDeAgregado = PropertyEntity({ text: "Disponibilizar no fechamento de agregado", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 1000, enable: ko.observable(true) });
    this.TacAgregado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.TacAgregado.val.subscribe(function (novoValor) {
        HabilitarDesabilitarTacAgregado(novoValor);
    });
};

var CRUDContratoFreteAcrescimoDesconto = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: LimparCamposClick, type: types.event, text: "Limpar Campos / Novo", visible: ko.observable(true) });
    this.LiberarPagamento = PropertyEntity({ eventClick: LiberarPagamentoClick, type: types.event, text: "Liberar Pagamento", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadContratoFreteAcrescimoDesconto() {
    _contratoFreteAcrescimoDesconto = new ContratoFreteAcrescimoDesconto();
    KoBindings(_contratoFreteAcrescimoDesconto, "knockoutCadastroContratoFreteAcrescimoDesconto");

    HeaderAuditoria("ContratoFreteAcrescimoDesconto", _contratoFreteAcrescimoDesconto);

    _CRUDContratoFreteAcrescimoDesconto = new CRUDContratoFreteAcrescimoDesconto();
    KoBindings(_CRUDContratoFreteAcrescimoDesconto, "knockoutCRUDContratoFreteAcrescimoDesconto");

    BuscarJustificativas(_contratoFreteAcrescimoDesconto.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete]);
    BuscarContratoFrete(_contratoFreteAcrescimoDesconto.ContratoFrete, CallBackContratoFrete, null, [EnumSituacaoContratoFrete.Aprovado, EnumSituacaoContratoFrete.Finalizada]);

    LoadEtapaContratoFreteAcrescimoDesconto();
    LoadPesquisaContratoFreteAcrescimoDesconto();
    LoadAprovacaoContratoFreteAcrescimoDesconto();
    loadIntegracoes();
    loadAnexo();
    HabilitarDesabilitarTacAgregado(_contratoFreteAcrescimoDesconto.TacAgregado);
}

function AdicionarClick() {
    Salvar(_contratoFreteAcrescimoDesconto, "ContratoFreteAcrescimoDesconto/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridContratoFreteAcrescimoDesconto.CarregarGrid();
                PreencherObjetoKnout(_contratoFreteAcrescimoDesconto, r);
                _CRUDContratoFreteAcrescimoDesconto.Adicionar.visible(false);
                _CRUDContratoFreteAcrescimoDesconto.Cancelar.visible(true);

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Acréscimo/Desconto do Contrato de Frete cadastrado com sucesso!");
                _contratoFreteAcrescimoDesconto.Codigo.val(r.Data.Codigo);
                SetarEtapaContratoFreteAcrescimoDesconto();
                SetarEnableCamposKnockout(_contratoFreteAcrescimoDesconto, false);
                enviarArquivosAnexados(r.Data.Codigo);
                HabilitarDesabilitarTacAgregado(_contratoFreteAcrescimoDesconto.TacAgregado);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CancelarClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar?", function () {
        executarReST("ContratoFreteAcrescimoDesconto/Cancelar", { Codigo: _contratoFreteAcrescimoDesconto.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Acréscimo/Desconto do Contrato de Frete cancelado com sucesso.");
                    LimparCamposContratoFreteAcrescimoDesconto();
                    _gridContratoFreteAcrescimoDesconto.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function LimparCamposClick() {
    LimparCamposContratoFreteAcrescimoDesconto();
}

function LiberarPagamentoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja liberar para pagamento?", function () {
        executarReST("ContratoFreteAcrescimoDesconto/LiberarPagamento", { Codigo: _contratoFreteAcrescimoDesconto.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Acréscimo/Desconto do Contrato de Frete liberado para pagamento com sucesso.");
                    LimparCamposContratoFreteAcrescimoDesconto();
                    _gridContratoFreteAcrescimoDesconto.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

////*******MÉTODOS*******

function EditarContratoFreteAcrescimoDesconto(contratoFreteAcrescimoDescontoGrid) {
    LimparCamposContratoFreteAcrescimoDesconto();

    _contratoFreteAcrescimoDesconto.Codigo.val(contratoFreteAcrescimoDescontoGrid.Codigo);

    BuscarPorCodigo(_contratoFreteAcrescimoDesconto, "ContratoFreteAcrescimoDesconto/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _pesquisaContratoFreteAcrescimoDesconto.ExibirFiltros.visibleFade(false);
                _CRUDContratoFreteAcrescimoDesconto.Adicionar.visible(false);
                _CRUDContratoFreteAcrescimoDesconto.Cancelar.visible(true);

                if (!_contratoFreteAcrescimoDesconto.PagamentoAutorizado && _contratoFreteAcrescimoDesconto.Situacao.val() === EnumSituacaoContratoFreteAcrescimoDesconto.Finalizado &&
                    (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarPagamento, _PermissoesPersonalizadas)) &&
                    !_contratoFreteAcrescimoDesconto.DisponibilizarFechamentoDeAgregado) {
                    _CRUDContratoFreteAcrescimoDesconto.LiberarPagamento.visible(true);
                }

                SetarEnableCamposKnockout(_contratoFreteAcrescimoDesconto, false);

                SetarEtapaContratoFreteAcrescimoDesconto();

                _detalheRejeicao.MotivoRejeicao.val(r.Data.MotivoRejeicao);
                ControleCamposIntegracao();
                preencherAnexos(r.Data.Anexos);
                HabilitarDesabilitarTacAgregado(_contratoFreteAcrescimoDesconto.TacAgregado);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LimparCamposContratoFreteAcrescimoDesconto() {
    _CRUDContratoFreteAcrescimoDesconto.Adicionar.visible(true);
    _CRUDContratoFreteAcrescimoDesconto.Cancelar.visible(false);
    _CRUDContratoFreteAcrescimoDesconto.LiberarPagamento.visible(false);

    SetarEnableCamposKnockout(_contratoFreteAcrescimoDesconto, true);

    LimparCampos(_contratoFreteAcrescimoDesconto);
    LimparCamposIntegracao();

    SetarEtapaInicioContratoFreteAcrescimoDesconto();
    limparAnexos();
    Global.ResetarAba("tabsContratoFrete");
}

function CallBackContratoFrete(data) {
    _contratoFreteAcrescimoDesconto.ContratoFrete.codEntity(data.Codigo);
    _contratoFreteAcrescimoDesconto.ContratoFrete.val(data.Descricao);

    if (_CONFIGURACAO_TMS.UtilizarFechamentoDeAgregado) {
        executarReST("ContratoFreteAcrescimoDesconto/VerificarContratoFreteTacAgregado", { Codigo: data.Codigo }, function (r) {

            if (r.Success) {
                if (r.Data) {
                    _contratoFreteAcrescimoDesconto.TacAgregado.val(true);
                } else {
                    _contratoFreteAcrescimoDesconto.TacAgregado.val(false);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
    else {
        _contratoFreteAcrescimoDesconto.TacAgregado.val(false);
    }
}

function HabilitarDesabilitarTacAgregado(novoValor) {
    if (novoValor) {
        _contratoFreteAcrescimoDesconto.DisponibilizarFechamentoDeAgregado.visible(true);
    }
    else {
        _contratoFreteAcrescimoDesconto.DisponibilizarFechamentoDeAgregado.visible(false);
        _contratoFreteAcrescimoDesconto.DisponibilizarFechamentoDeAgregado.val(false);
    }
}