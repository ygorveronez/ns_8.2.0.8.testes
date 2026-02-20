//#region Objetos Globais do Arquivo
var _aprovacaoDataDescarga;
// #endregion Objetos Globais do Arquivo

//#region Classes
var AprovacaoDataDescarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });

    this.DataDaSolicitacao = PropertyEntity({ text: "Data da Solicitação", getType: typesKnockout.dateTime, val: ko.observable("-"), def: "-" });
    this.DataCarregamento = PropertyEntity({ text: "Data de Carregamento", getType: typesKnockout.dateTime, val: ko.observable("-"), def: "-" });
    this.DataDescarregamento = PropertyEntity({ text: "Data de Descarregamento", getType: typesKnockout.dateTime, val: ko.observable("-"), def: "-" });

    this.Transportador = PropertyEntity({ text: "Transportador", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veiculo", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: "Motorista", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ text: "Tipo de Carga", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: "Destinatário", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });

    this.ObservacaoAgendamento = PropertyEntity({ text: "Observação Transportador", val: ko.observable(""), getType: typesKnockout.string, enable: ko.observable(true) });
    this.ObservacaoAnaliseAgendamento = PropertyEntity({ text: "Observações", val: ko.observable(""), getType: typesKnockout.string, enable: ko.observable(true) });
    this.SituacaoAprovacao = PropertyEntity({ text: "Situação da Aprovação", val: ko.observable("Aprovado"), getType: typesKnockout.string, enable: ko.observable(true) });

    this.Aprovar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Aprovar, eventClick: aprovarAgendamentoDescarga, visible: ko.observable(true), enable: ko.observable(true) });
    this.Reprovar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Reprovar, eventClick: reprovarAgendamentoDescarga, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, eventClick: cancelarAgendamentoDescarga, visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

}
//#endregion Classes

// #region Funções de Inicialização
function loadAprovacaoDataDescarga(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/AprovacaoDataDescarga.html?dyn=" + guid(), function (data) {
                $("#container-principal-content").html(data);

                _aprovacaoDataDescarga = new AprovacaoDataDescarga();
                KoBindings(_aprovacaoDataDescarga, "knockoutGestaoDevolucaoEtapaAprovacaoDataDescarga");

                BuscarTiposOperacao(_aprovacaoDataDescarga.TipoOperacao);

                PreencherObjetoKnout(_aprovacaoDataDescarga, r);
                controlarAcoesContainerPrincipal(etapa, _aprovacaoDataDescarga);

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function aprovarAgendamentoDescarga() {
    if (!ValidarCamposObrigatorios(_aprovacaoDataDescarga))
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.FiltrosObrigatorios, Localization.Resources.Gerais.Geral.InformeOsFiltrosObrigatorios);
    else
        salvarAgendamentoDescarga(true);
}
function reprovarAgendamentoDescarga() {
    salvarAgendamentoDescarga(false);
}
function cancelarAgendamentoDescarga() {
    LimparCampos(_aprovacaoDataDescarga);
    mostrarGridDevolucoes();
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
function salvarAgendamentoDescarga(aprovado) {
    let dados = {
        CodigoGestaoDevolucao: _aprovacaoDataDescarga.Codigo.val(),
        Observacoes: _aprovacaoDataDescarga.ObservacaoAnaliseAgendamento.val(),
        TipoOperacao: _aprovacaoDataDescarga.TipoOperacao.codEntity(),
        Aprovado: aprovado
    }
    executarReST("GestaoDevolucao/SalvarAprovacaoAgendamentoDescarga", dados, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Análise salva com sucesso.");
            if (!aprovado) {
                controlarVisibilidadeGrids();
                _gridGestaoDevolucaoDevolucoes.CarregarGrid();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções Privadas