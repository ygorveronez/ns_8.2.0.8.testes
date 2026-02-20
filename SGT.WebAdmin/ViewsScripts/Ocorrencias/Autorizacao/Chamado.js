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
/// <reference path="Ocorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamadoCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoChamado;



var ResumoChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Carga.getFieldDescription(), visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Transportador.getFieldDescription()), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Veiculo.getFieldDescription() });
    this.Cliente = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Cliente.getFieldDescription(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Motorista.getFieldDescription() });
    this.Origem = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Origem.getFieldDescription() });
    this.Destino = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Destino.getFieldDescription() });
    this.Valor = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Valor.getFieldDescription() , visible: ko.observable(true) });
    this.DataChamado = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DataChamado.getFieldDescription() });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Situacao.getFieldDescription() });
    this.ObservacoesCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ObervacoesCarga.getFieldDescription(), visible: ko.observable(true) });
    this.DataHoraFaturamento = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DataFaturamento.getFieldDescription(), visible: ko.observable(true) });
    this.Atrazo = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DiasAtraso.getFieldDescription()), visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Numero.getFieldDescription(), visible: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirAnalisesClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Imprimir.getFieldDescription(), visible: ko.observable(true) });
    this.Chamados = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Atendimento.getFieldDescription(), val: ko.observable(0), enable: ko.observable(true), options: ko.observable(new Array()), def: 0, visible: ko.observable(false), eventChange: atendimentoChange });

}

//*******EVENTOS*******

function LoadResumoChamado() {
    _resumoChamado = new ResumoChamado();
    KoBindings(_resumoChamado, "knoutChamado");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _resumoChamado.Empresa.text(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.EmpresaFilial.getFieldDescription())
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _resumoChamado.Empresa.visible(false);
    }
}

function ImprimirAnalisesClick(e, sender) {
    executarDownload("ChamadoAnalise/DownloadRelatorioAnalises", { Codigo: _resumoChamado.Codigo.val() });
}


//*******MÉTODOS*******
var _chamados;
function atendimentoChange() {
    PreecherResumoChamado(_chamados[_resumoChamado.Chamados.val()]);
}

function PreecherResumoChamados(chamados) {
    _chamados = chamados;
    if (chamados.length > 0) {
        PreecherResumoChamado(chamados[0]);
        var listaChamados = new Array();
        for (var i = 0; i < chamados.length; i++) {
            listaChamados.push({ text: chamados[i].Numero, value: i });
        }
        _resumoChamado.Chamados.options(listaChamados);
        if (chamados.length > 1)
            _resumoChamado.Chamados.visible(true);
        else
            _resumoChamado.Chamados.visible(false);
    } else {
        _resumoChamado.Chamados.visible(false);
    }
}

function PreecherResumoChamado(dados) {

    if (dados != null) {
        $("#liChamado").show();
        _resumoChamado.Carga.visible(true);
        PreencherObjetoKnout(_resumoChamado, { Data: dados });

        if (dados.Cliente == "")
            _resumoChamado.Cliente.visible(false);

        if (dados.Valor != "")
            _resumoChamado.Valor.visible(true);
        else
            _resumoChamado.Valor.visible(false);

        if (dados.ObservacoesCarga == "")
            _resumoChamado.ObservacoesCarga.visible(false);

        if (dados.Atrazo == 0)
            _resumoChamado.Atrazo.val(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CargaFaturadaEmDia);
        else if (dados.Atrazo < 0) {
            _resumoChamado.Atrazo.text(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CargaAntecipadaEm.getFieldDescription());
            dados.Atrazo = dados.Atrazo * -1;
            _resumoChamado.Atrazo.val(dados.Atrazo +  Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Dia + (dados.Atrazo > 1 ? "s" : ""));
        }
        else {
            _resumoChamado.Atrazo.text(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DiasEmAtraso.getFieldDescription());
            _resumoChamado.Atrazo.val(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CargaFaturadaCom + dados.Atrazo + Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Dia + (dados.Atrazo > 1 ? "s" : "") + Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DeAtraso);
        }

        //if (dados.Atrazo == 0)
        //    _resumoChamado.Atrazo.val("Carga faturada em dia");
        //else
        //    _resumoChamado.Atrazo.val("Carga faturada com " + dados.Atrazo + " dia" + (dados.Atrazo > 1 ? "s" : "") + " de atraso");

        if (dados.Numero == 0)
            _resumoChamado.Numero.visible(false);
    }
}

function LimparResumoChamado() {
    $("#liChamado").hide();
    LimparCampos(_resumoChamado);
    _resumoChamado.Carga.visible(false);
    _resumoChamado.Numero.visible(true);
}