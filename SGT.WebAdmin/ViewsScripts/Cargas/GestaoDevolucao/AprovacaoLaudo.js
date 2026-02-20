//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaAprovacaoLaudo;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaAprovacaoLaudo = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.NumeroLaudo = PropertyEntity({ text: "Número do Laudo", val: ko.observable(""), getType: typesKnockout.string });
    this.DataCriacao = PropertyEntity({ text: "Data Criação", val: ko.observable(""), getType: typesKnockout.string });
    this.Responsavel = PropertyEntity({ text: "Responsavel", val: ko.observable(""), getType: typesKnockout.string });
    this.Transportador = PropertyEntity({ text: "Transportador", val: ko.observable(""), getType: typesKnockout.string });
    this.Veiculo = PropertyEntity({ text: "Veiculo (Placa)", val: ko.observable(""), getType: typesKnockout.string });
    this.VisualizarLaudo = PropertyEntity({ text: "Visualizar Laudo", val: ko.observable(""), enable: ko.observable(true), getType: typesKnockout.string, eventClick: visualizarLaudo });
    this.Motivo = PropertyEntity({ text: "Motivo", val: ko.observable(""), enable: ko.observable(true), getType: typesKnockout.string });
    this.NumeroCompensacao = PropertyEntity({ text: "Número Compensação", val: ko.observable(''), enable: ko.observable(true), getType: typesKnockout.string });
    this.DataCompensacao = PropertyEntity({ text: "Data Compensação", getType: typesKnockout.dateTime, val: ko.observable(null), required: ko.observable(true), enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "Valor", val: ko.observable(0), getType: typesKnockout.decimal, maxlength: 13, enable: ko.observable(true), required: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });

    this.Aprovar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Aprovar, eventClick: aprovarLaudo, visible: ko.observable(true), enable: ko.observable(true) });
    this.Reprovar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Reprovar, eventClick: reprovarLaudo, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, eventClick: cancelarAprovacaoLaudo, visible: ko.observable(true), enable: ko.observable(true) });

    this.auditarLaudoClick = auditarLaudoClick;
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });
}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaAprovacaoLaudo(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/AprovacaoLaudo.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaAprovacaoLaudo = new GestaoDevolucaoEtapaAprovacaoLaudo();
                KoBindings(_gestaoDevolucaoEtapaAprovacaoLaudo, "knockoutAprovacaoLaudo");

                HeaderAuditoria("GestaoDevolucaoLaudo", _gestaoDevolucaoEtapaAprovacaoLaudo);

                PreencherObjetoKnout(_gestaoDevolucaoEtapaAprovacaoLaudo, r);

                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoEtapaAprovacaoLaudo);

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
function auditarLaudoClick() {
    __AbrirModalAuditoria();
}
function aprovarLaudo() {
    salvarAprovacaoLaudo(true);
}
function reprovarLaudo() {
    salvarAprovacaoLaudo(false);
}
function cancelarAprovacaoLaudo() {
    LimparCampos(_gestaoDevolucaoEtapaAprovacaoLaudo);
    mostrarGridDevolucoes();
}
function visualizarLaudo(e) {
    loadVisualizarLaudo(e.NumeroLaudo.val());
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
function salvarAprovacaoLaudo(laudoAprovado) {
    let dados = {
        CodigoGestaoDevolucao: _gestaoDevolucaoEtapaAprovacaoLaudo.CodigoGestaoDevolucao.val(),
        Motivo: _gestaoDevolucaoEtapaAprovacaoLaudo.Motivo.val(),
        LaudoAprovado: laudoAprovado
    }
    executarReST("GestaoDevolucao/SalvarAprovacaoLaudo", dados, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Avaliação do Laudo salva com sucesso.");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções Privadas