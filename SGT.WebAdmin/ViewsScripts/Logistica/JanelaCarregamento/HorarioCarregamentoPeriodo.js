/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/PeriodoCarregamento.js" />

// #region Objetos Globais do Arquivo

var _horarioCarregamentoPeriodo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var HorarioCarregamentoPeriodo = function () {
    this.Codigo = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataCarregamento = PropertyEntity({});
    this.PeriodoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.NovoHorario, idBtnSearch: guid(), required: true });

    this.AlocarHorario = PropertyEntity({ eventClick: alocarHorarioCarregamentoPeriodoClick, type: types.event, text: Localization.Resources.Cargas.Carga.AlterarHorario, visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadHorarioCarregamentoPeriodo() {
    _horarioCarregamentoPeriodo = new HorarioCarregamentoPeriodo();
    KoBindings(_horarioCarregamentoPeriodo, "knockoutHorarioCarregamentoPeriodo");

    new BuscarPeriodoCarregamento(_horarioCarregamentoPeriodo.PeriodoCarregamento, retornoConsultaHorarioCarregamentoPeriodo, _horarioCarregamentoPeriodo.DataCarregamento, _horarioCarregamentoPeriodo.CentroCarregamento, _horarioCarregamentoPeriodo.Filial, _horarioCarregamentoPeriodo.TipoCarga, true);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function alocarHorarioCarregamentoPeriodoClick() {
    if (!ValidarCamposObrigatorios(_horarioCarregamentoPeriodo)) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    alocarHorarioCarregamentoPeriodo();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirModalAlteracaoHorarioCarregamentoPeriodoPorCentroCarregamento(codigoJanelaCarregamento, dataCarregamento) {
    _horarioCarregamentoPeriodo.Codigo.val(codigoJanelaCarregamento);
    _horarioCarregamentoPeriodo.CentroCarregamento.codEntity(_dadosPesquisaCarregamento.CentroCarregamento);
    _horarioCarregamentoPeriodo.CentroCarregamento.val(_dadosPesquisaCarregamento.CentroCarregamento);
    _horarioCarregamentoPeriodo.DataCarregamento.val(dataCarregamento);

    exibirModalHorarioCarregamentoPeriodo();
}

function exibirModalAlteracaoHorarioCarregamentoPeriodoPorJanelaCarregamento(janelaCarregamentoSelecionada, dataCarregamento) {
    _horarioCarregamentoPeriodo.Codigo.val(janelaCarregamentoSelecionada.Codigo);
    _horarioCarregamentoPeriodo.Filial.codEntity(janelaCarregamentoSelecionada.CodigoFilial);
    _horarioCarregamentoPeriodo.Filial.val(janelaCarregamentoSelecionada.CodigoFilial);
    _horarioCarregamentoPeriodo.TipoCarga.codEntity(janelaCarregamentoSelecionada.CodigoTipoCarga);
    _horarioCarregamentoPeriodo.TipoCarga.val(janelaCarregamentoSelecionada.CodigoTipoCarga);
    _horarioCarregamentoPeriodo.DataCarregamento.val(dataCarregamento);

    exibirModalHorarioCarregamentoPeriodo();
}

// #endregion Funções Públicas

// #region Funções Privadas

function alocarHorarioCarregamentoPeriodo(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido) {
    var dados = {
        Codigo: _horarioCarregamentoPeriodo.Codigo.val(),
        DataCarregamento: _horarioCarregamentoPeriodo.DataCarregamento.val(),
        PeriodoCarregamento: _horarioCarregamentoPeriodo.PeriodoCarregamento.codEntity(),
        PermitirHorarioCarregamentoComLimiteAtingido: Boolean(permitirHorarioCarregamentoComLimiteAtingido),
        PermitirHorarioCarregamentoInferiorAoAtual: Boolean(permitirHorarioCarregamentoInferiorAoAtual)
    };

    executarReST("JanelaCarregamento/AlocarHorarioCarregamentoPorPeriodo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.HorarioCarregamentoInferiorAtual) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaSobreporEssaRegra.format(retorno.Msg), function () {
                        permitirHorarioCarregamentoInferiorAoAtual = true;
                        alocarHorarioCarregamentoPeriodo(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido);
                    });
                    return;
                }

                if (retorno.Data.HorarioLimiteCarregamentoAtingido) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaSobreporEssaRegra.format(retorno.Msg), function () {
                        permitirHorarioCarregamentoComLimiteAtingido = true;
                        alocarHorarioCarregamentoPeriodo(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido);
                    });
                    return;
                }

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.GeralSucesso, Localization.Resources.Cargas.Carga.HorarioAlocadoComSucesso);
                fecharModalHorarioCarregamentoPeriodo();

                if (_dadosPesquisaCarregamento.DataCarregamento != dados.DataCarregamento)
                    CarregarDadosPesquisa();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirModalHorarioCarregamentoPeriodo() {
    Global.abrirModal('divModalHorarioCarregamentoPeriodo');
    $("#divModalHorarioCarregamentoPeriodo").one('hidden.bs.modal', function () {
        LimparCampos(_horarioCarregamentoPeriodo);
    });
}

function fecharModalHorarioCarregamentoPeriodo() {
    Global.fecharModal('divModalHorarioCarregamentoPeriodo');
}

function retornoConsultaHorarioCarregamentoPeriodo(registroSelecionado) {
    _horarioCarregamentoPeriodo.DataCarregamento.val(registroSelecionado.DataCarregamento)
    _horarioCarregamentoPeriodo.PeriodoCarregamento.codEntity(registroSelecionado.Codigo)
    _horarioCarregamentoPeriodo.PeriodoCarregamento.entityDescription(registroSelecionado.Descricao);
    _horarioCarregamentoPeriodo.PeriodoCarregamento.val(registroSelecionado.Descricao);
}

// #endregion Funções Privadas
