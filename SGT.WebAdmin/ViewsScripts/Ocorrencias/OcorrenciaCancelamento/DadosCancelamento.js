/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _dadosCancelamento;

var DadosCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Ocorrencia.getRequiredFieldDescription(), issue: 412, idBtnSearch: guid(), enable: ko.observable(true), required: true });
    this.DataCancelamento = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable(""), def: "", text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DataCancelamento.getRequiredFieldDescription(), enable: ko.observable(false), required: true });
    this.UsuarioSolicitou = PropertyEntity({ type: types.map, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.UsuarioQueSolicitou.getRequiredFieldDescription(), val: ko.observable(_CONFIGURACAO_TMS.UsuarioLogado), def: _CONFIGURACAO_TMS.UsuarioLogado, issue: 632, idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Motivo.getRequiredFieldDescription(), issue: 632, maxlength: 255, enable: ko.observable(true), required: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCancelamentoOcorrencia.Cancelamento), def: EnumTipoCancelamentoOcorrencia.Cancelamento, getType: typesKnockout.int });
    this.InfoCancelamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Anexos = PropertyEntity({ eventClick: AbrirTelaAnexoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Anexos, visible: ko.observable(true), icon: "fa fa-file-zip-o" });
    this.Adicionar = PropertyEntity({ eventClick: gerarCancelamentoClick, type: types.event, text: ko.observable(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.GerarCancelamento), visible: ko.observable(true) });
    this.AdicionarComoCancelamento = PropertyEntity({ eventClick: AdicionarCancelamentoOcorrenciaComoCancelamentoClick, type: types.event, text:Localization.Resources.Ocorrencias.OcorrenciaCancelamento.AdicionarCancelamento, visible: ko.observable(false) });
};

//*******EVENTOS*******
function loadDadosCancelamento() {
    _dadosCancelamento = new DadosCancelamento();
    KoBindings(_dadosCancelamento, "knockoutDadosCancelamento");

    new BuscarOcorrencias(_dadosCancelamento.Ocorrencia, RetornoConsultaOcorrencia);

    LoadAnexo();
}

function gerarCancelamentoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Atencao, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DesejaRealmenteCancelarEstaOcorrencia, function () {
        Salvar(_dadosCancelamento, "OcorrenciaCancelamento/Adicionar", function (arg) {
            if (arg.Success) {

                if (arg.Data) {
                    EnviarArquivosAnexados(arg.Data.Codigo).then(function () {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso, (_dadosCancelamento.Tipo.val() == EnumTipoCancelamentoOcorrencia.Cancelamento ? Localization.Resources.Ocorrencias.OcorrenciaCancelamento.CancelamentoSolicitado : Localization.Resources.Ocorrencias.OcorrenciaCancelamento.AnulacaoSolicitada) + "com sucesso!");
                        _gridCancelamentoOcorrencia.CarregarGrid();
                        BuscarCancelamentoPorCodigo(arg.Data.Codigo);
                    });
                } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

function AdicionarCancelamentoOcorrenciaComoCancelamentoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Atencao, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DesejaRealmenteAdicionarEstaOcorrenciaParaOperacaoCancelamento, function () {
        Salvar(_dadosCancelamento, "OcorrenciaCancelamento/AdicionarComoCancelamento", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso,  Localization.Resources.Ocorrencias.OcorrenciaCancelamento.CancelamentoSolicitadoComSucesso);
                    _gridCancelamentoOcorrencia.CarregarGrid();
                    BuscarCancelamentoPorCodigo(arg.Data.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

//*******MÉTODOS*******
function RetornoConsultaOcorrencia(data) {
    _dadosCancelamento.InfoCancelamento.val("");
    _dadosCancelamento.Ocorrencia.codEntity(data.Codigo);
    _dadosCancelamento.Ocorrencia.val(data.Descricao);
    ObterDetalhesOcorrencia(data.Codigo);
}

function LimparCampoOcorrencia() {
    _dadosCancelamento.Ocorrencia.codEntity(_dadosCancelamento.Ocorrencia.defCodEntity);
    _dadosCancelamento.Ocorrencia.val(_dadosCancelamento.Ocorrencia.def);
}

function ObterDetalhesOcorrencia(codigoOcorrencia) {
    executarReST("OcorrenciaCancelamento/ValidarOcorrencia", { Ocorrencia: codigoOcorrencia }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _dadosCancelamento.Tipo.val(r.Data.TipoCancelamento);

                if (r.Data.TipoCancelamento == EnumTipoCancelamentoOcorrencia.Anulacao) {
                    _dadosCancelamento.InfoCancelamento.val(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OsDocumentosNaoPodemMaisSerCancelados);

                    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.OcorrenciaCancelamento_Anular, _PermissoesPersonalizadasOcorrenciaCancelamento))
                        _dadosCancelamento.Adicionar.text(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.GerarAnulacao);
                    else
                        _dadosCancelamento.Adicionar.visible(false);

                    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.OcorrenciaCancelamento_AdicionarComoCancelamento, _PermissoesPersonalizadasOcorrenciaCancelamento))
                        _dadosCancelamento.AdicionarComoCancelamento.visible(true);

                } else {
                    _dadosCancelamento.Adicionar.text(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.GerarCancelamento);
                    _dadosCancelamento.Adicionar.visible(true);
                    _dadosCancelamento.AdicionarComoCancelamento.visible(false);
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Geral.Gerais.OcorrenciaCancelamento.Atencao, r.Msg);
                LimparCampoOcorrencia();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            LimparCampoOcorrencia();    
        }
    });
}

function EditarDadosCancelamento(data) {
    _dadosCancelamento.Codigo.val(data.Codigo);
    if (data.DadosCancelamento != null) {
        PreencherObjetoKnout(_dadosCancelamento, { Data: data.DadosCancelamento });
    }

    ControleCamposDadosCancelamento(false);

    _anexo.Anexos.val(data.Anexos);
    _anexo.Anexos.visible(false);
    _anexo.Finalizar.visible(false);
}

function ControleCamposDadosCancelamento(status) {
    _dadosCancelamento.Ocorrencia.enable(status);
    _dadosCancelamento.Motivo.enable(status);

    _dadosCancelamento.Adicionar.visible(status);
    _dadosCancelamento.AdicionarComoCancelamento.visible(false);
}

function SetarDadosCancelamento() {
    _dadosCancelamento.DataCancelamento.val(Global.DataAtual());
}

function LimparCamposDadosCancelamento() {
    LimparCampos(_dadosCancelamento);
    ControleCamposDadosCancelamento(true);

    SetarDadosCancelamento();

    LimparCamposAnexo();
}