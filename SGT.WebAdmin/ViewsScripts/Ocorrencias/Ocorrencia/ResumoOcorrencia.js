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
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoOcorrencia;

var ResumoOcorrencia = function () {

    this.NumeroOcorrencia = PropertyEntity({ codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroOcorrencia.getFieldDescription() , visible: ko.observable(false) });
    this.NumeroCarga = PropertyEntity({ codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Carga.getFieldDescription(), visible: ko.observable(true) });
    this.CodigosAgrupadosCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.CargasAgrupadas.getFieldDescription() });
    this.DataOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataOcorrencia.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.TipoOcorrencia.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.ValorOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ValorOcorrencia.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true), visible: ko.observable(false) });
    this.MotivoCancelamento = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.MotivoCancelamento.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true), visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Situacao.getFieldDescription(), visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Placa.getFieldDescription(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Motorista.getFieldDescription(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Cliente.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Transportador.getFieldDescription(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Filial.getFieldDescription(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.TipoOperacao.getFieldDescription(), visible: ko.observable(true) });
    this.DistanciaCTes = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Distancia.getFieldDescription(), visible: ko.observable(false) });
    this.CentroResultado = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.CentroResultado.getFieldDescription(), visible: ko.observable(false) });
}


//*******EVENTOS*******

function loadResumoOcorrencia() {
    _resumoOcorrencia = new ResumoOcorrencia();
    KoBindings(_resumoOcorrencia, "knockoutResumoOcorrencia");

    AlternaCampos();
}


//*******MÉTODOS*******

function AlternaCampos() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
        _resumoOcorrencia.Cliente.visible(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _resumoOcorrencia.Transportador.visible(false);
        _resumoOcorrencia.Filial.visible(false);
    }
}

function preecherResumoOcorrencia(ocorrenciaPorPeriodo) {
    _resumoOcorrencia.NumeroOcorrencia.visible(true);

    if (_ocorrencia.ComponenteFrete.codEntity() > 0)
        _resumoOcorrencia.ValorOcorrencia.visible(true);

    if (_ocorrencia.CentroResultado.visible() == true)
        _resumoOcorrencia.CentroResultado.visible(true);

    _resumoOcorrencia.NumeroOcorrencia.val(_ocorrencia.NumeroOcorrencia.val());
    _resumoOcorrencia.DataOcorrencia.val(_ocorrencia.DataOcorrencia.val());
    _resumoOcorrencia.TipoOcorrencia.val(_ocorrencia.TipoOcorrencia.val());
    _resumoOcorrencia.ValorOcorrencia.val(_ocorrencia.ValorOcorrencia.val());
    _resumoOcorrencia.Situacao.val(_ocorrencia.DescricaoSituacao.val());
    _resumoOcorrencia.Cliente.val(_ocorrencia.Cliente.val());
    _resumoOcorrencia.MotivoCancelamento.val(_ocorrencia.MotivoCancelamento.val());

    if (!string.IsNullOrWhiteSpace(_ocorrencia.DistanciaCTes.val())) {
        _resumoOcorrencia.DistanciaCTes.visible(true);
        _resumoOcorrencia.DistanciaCTes.val(_ocorrencia.DistanciaCTes.val());
    }

    if (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.Cancelada)
        _resumoOcorrencia.MotivoCancelamento.visible(true);
    else
        _resumoOcorrencia.MotivoCancelamento.visible(false);


    if (ocorrenciaPorPeriodo) {
        _resumoOcorrencia.NumeroCarga.visible(false);
        _resumoOcorrencia.Placa.visible(false);
        _resumoOcorrencia.Motorista.visible(false);
        _resumoOcorrencia.Transportador.visible(false);
        _resumoOcorrencia.Filial.visible(false);
        _resumoOcorrencia.TipoOperacao.visible(false);
    }
    else {
        executarReST("Carga/BuscarPorCodigo", { Codigo: _ocorrencia.Carga.codEntity() }, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    _resumoOcorrencia.NumeroCarga.val(_ocorrencia.Carga.val());

                    // Informacoes da carga
                    _resumoOcorrencia.Placa.val(arg.Data.Veiculo.Descricao);
                    _resumoOcorrencia.Motorista.val(arg.Data.Motorista.Descricao);
                    _resumoOcorrencia.Transportador.val(arg.Data.Empresa.Descricao);
                    _resumoOcorrencia.Filial.val(arg.Data.Filial.Descricao);
                    _resumoOcorrencia.TipoOperacao.val(arg.Data.TipoOperacao);
                    _resumoOcorrencia.CentroResultado.val(arg.Data.CentroResultado);
                    _ocorrencia.CentroResultado.val(arg.Data.CentroResultado);

                    if (arg.Data.Filial.Codigo == 0)
                        _resumoOcorrencia.Filial.visible(false);
                } else if (arg.Msg != null) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function limparResumoOcorrencia() {
    _resumoOcorrencia.NumeroOcorrencia.visible(false);
    _resumoOcorrencia.ValorOcorrencia.visible(false);
    LimparCampos(_resumoOcorrencia);
}