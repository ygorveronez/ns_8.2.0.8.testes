//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaAgendamento;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaAgendamento = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });

    this.DataCarregamento = PropertyEntity({ text: "Data de Carregamento", getType: typesKnockout.dateTime, val: ko.observable(null), required: ko.observable(true), enable: ko.observable(true) });
    this.DataDescarregamento = PropertyEntity({ text: "Data de Descarga Prevista", eventClick: exibirDatasAgendamentoClick, getType: typesKnockout.string, val: ko.observable(null), required: ko.observable(true), enable: ko.observable(true), descricao: ko.observable("") });
    this.ObservacaoAgendamento = PropertyEntity({ text: "Observações", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.ObservacaoAnaliseAgendamento = PropertyEntity({ text: "Observação Análise", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    
    this.PeriodoDescarregamento = PropertyEntity({ text: "Período de Descarregamento", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ text: "Motorista", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Placa", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "Veiculo", propCodigo: "Codigo" }, enable: ko.observable(true) });

    this.Destinatario = PropertyEntity({ text: "Destinatário", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true) });
    this.Remetente = PropertyEntity({ text: "Remetente", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ text: "Tipo de Carga", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true) });

    this.Agendar = PropertyEntity({ text: "Solicitar Agendamento", eventClick: agendarClick, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", eventClick: limparCamposAgendamento, visible: ko.observable(true), enable: ko.observable(true) });

    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaAgendamento(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/Agendamento.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaAgendamento = new GestaoDevolucaoEtapaAgendamento();
                KoBindings(_gestaoDevolucaoEtapaAgendamento, "knockoutAgendamento");

                BuscarVeiculos(_gestaoDevolucaoEtapaAgendamento.Veiculo);
                BuscarMotorista(_gestaoDevolucaoEtapaAgendamento.Motorista);
                BuscarClientes(_gestaoDevolucaoEtapaAgendamento.Destinatario, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
                BuscarTiposdeCarga(_gestaoDevolucaoEtapaAgendamento.TipoDeCarga);

                PreencherObjetoKnout(_gestaoDevolucaoEtapaAgendamento, r);

                if (r.Data.DescricaoDataDescarregamento)
                    _gestaoDevolucaoEtapaAgendamento.DataDescarregamento.descricao(r.Data.DescricaoDataDescarregamento);

                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoEtapaAgendamento);

                _gestaoDevolucaoEtapaAgendamento.CodigoGestaoDevolucao.val(_informacoesDevolucao.CodigoDevolucao.val());

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
                $.get("Content/Static/JanelaDescarregamento/HorarioAgendamento.html?dyn=" + guid(), function (data) {
                    $("#modalDataDescarregamento").html(data);
                    loadModalAgendamentoDataDescarga(_gestaoDevolucaoEtapaAgendamento.DataDescarregamento,
                        _gestaoDevolucaoEtapaAgendamento.PeriodoDescarregamento,
                        _gestaoDevolucaoEtapaAgendamento.TipoDeCarga,
                        _gestaoDevolucaoEtapaAgendamento.Remetente,
                        _gestaoDevolucaoEtapaAgendamento.Destinatario);
                });
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function agendarClick() {
    if (!validarDados())
        return;

    var dados = {
        CodigoGestaoDevolucao: _gestaoDevolucaoEtapaAgendamento.CodigoGestaoDevolucao.val(),
        Veiculo: _gestaoDevolucaoEtapaAgendamento.Veiculo.codEntity(),
        Motorista: _gestaoDevolucaoEtapaAgendamento.Motorista.codEntity(),
        Observacao: _gestaoDevolucaoEtapaAgendamento.ObservacaoAgendamento.val(),
        DataCarregamento: _gestaoDevolucaoEtapaAgendamento.DataCarregamento.val(),
        DataDescarregamento: _gestaoDevolucaoEtapaAgendamento.DataDescarregamento.val(),
        PeriodoDescarregamento: _gestaoDevolucaoEtapaAgendamento.PeriodoDescarregamento.codEntity(),
        Destinatario: _gestaoDevolucaoEtapaAgendamento.Destinatario.codEntity(),
        Remetente: _gestaoDevolucaoEtapaAgendamento.Remetente.codEntity(),
        TipoDeCarga: _gestaoDevolucaoEtapaAgendamento.TipoDeCarga.codEntity(),
    }

    executarReST("GestaoDevolucao/SalvarAgendamento", dados, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Agendamento salvo com sucesso");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function limparCamposAgendamento() {
    LimparCampos(_gestaoDevolucaoEtapaAgendamento);
    limparEtapasDevolucao();
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
function validarDados() {
    if (!ValidarCamposObrigatorios(_gestaoDevolucaoEtapaAgendamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    return true;
}
// #endregion Funções Privadas