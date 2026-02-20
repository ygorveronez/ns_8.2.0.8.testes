/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../Consultas/PeriodoDescarregamento.js" />
/// <reference path="JanelaDescarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _horarioDescarregamentoPeriodo;
var _codigosJanelaDescarregamentoPeriodo = new Array();

/*
 * Declaração das Classes
 */

var HorarioDescarregamentoPeriodo = function () {
    this.Codigo = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.SolicitouMotivo = PropertyEntity({ type: types.bool, val: ko.observable(0), def: 0 });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataDescarregamento = PropertyEntity({});
    this.PeriodoDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Novo Horário", idBtnSearch: guid(), required: true });
    this.MotivoReagendamento = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.AlocarHorario = PropertyEntity({ eventClick: alocarHorarioDescarregamentoPeriodoClick, type: types.event, text: "Alterar Horário", visible: ko.observable(true) });
    this.AlocarHorarios = PropertyEntity({ eventClick: alocarHorariosDescarregamentosPeriodosClick, type: types.event, text: "Alterar Horários", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadHorarioDescarregamentoPeriodo() {
    _horarioDescarregamentoPeriodo = new HorarioDescarregamentoPeriodo();
    KoBindings(_horarioDescarregamentoPeriodo, "knockoutHorarioDescarregamentoPeriodo");

    new BuscarPeriodoDescarregamento(_horarioDescarregamentoPeriodo.PeriodoDescarregamento, retornoConsultaHorarioDescarregamentoPeriodo, _horarioDescarregamentoPeriodo.DataDescarregamento, _horarioDescarregamentoPeriodo.CentroDescarregamento, true);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function alocarHorarioDescarregamentoPeriodoClick() {
    alocarHorarioDescarregamentoPeriodo();
}

function alocarHorariosDescarregamentosPeriodosClick() {
    alocarHorariosDescarregamentosPeriodos();
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalAlteracaoHorarioDescarregamentoPeriodo(codigoJanelaDescarregamento, dataDescarregamento, solicitarMotivo) {
    _horarioDescarregamentoPeriodo.Codigo.val(codigoJanelaDescarregamento);
    _horarioDescarregamentoPeriodo.CentroDescarregamento.codEntity(_dadosPesquisaDescarregamento.CentroDescarregamento);
    _horarioDescarregamentoPeriodo.CentroDescarregamento.val(_dadosPesquisaDescarregamento.CentroDescarregamento);
    _horarioDescarregamentoPeriodo.DataDescarregamento.val(dataDescarregamento);
    _horarioDescarregamentoPeriodo.SolicitouMotivo.val(solicitarMotivo);

    exibirModalHorarioDescarregamentoPeriodo(solicitarMotivo);
}

function exibirModalAlteracaoMultiplosHorariosDescarregamentosPeriodos(codigos, solicitarMotivo) {
    _codigosJanelaDescarregamentoPeriodo = codigos;

    _horarioDescarregamentoPeriodo.AlocarHorario.visible(false);
    _horarioDescarregamentoPeriodo.AlocarHorarios.visible(true);
    _horarioDescarregamentoPeriodo.CentroDescarregamento.codEntity(_dadosPesquisaDescarregamento.CentroDescarregamento);
    _horarioDescarregamentoPeriodo.CentroDescarregamento.val(_dadosPesquisaDescarregamento.CentroDescarregamento);
    _horarioDescarregamentoPeriodo.SolicitouMotivo.val(solicitarMotivo);

    exibirModalHorarioDescarregamentoPeriodo(solicitarMotivo);
}

/*
 * Declaração das Funções Privadas
 */

function alocarHorarioDescarregamentoPeriodo(permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido) {
    if (!ValidarCamposObrigatorios(_horarioDescarregamentoPeriodo)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    if (_horarioDescarregamentoPeriodo.SolicitouMotivo.val() && _horarioDescarregamentoPeriodo.MotivoReagendamento.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, "Caracteres Mínimos", "É preciso informar um motivo com no mínimo 20 caracteres.");

    var dados = {
        Codigo: _horarioDescarregamentoPeriodo.Codigo.val(),
        DataDescarregamento: _horarioDescarregamentoPeriodo.DataDescarregamento.val(),
        PeriodoDescarregamento: _horarioDescarregamentoPeriodo.PeriodoDescarregamento.codEntity(),
        PermitirHorarioDescarregamentoComLimiteAtingido: Boolean(permitirHorarioDescarregamentoComLimiteAtingido),
        PermitirHorarioDescarregamentoInferiorAoAtual: Boolean(permitirHorarioDescarregamentoInferiorAoAtual),
        MotivoReagendamento: _horarioDescarregamentoPeriodo.MotivoReagendamento.val(),
        SolicitouMotivo: _horarioDescarregamentoPeriodo.SolicitouMotivo.val(),
    };

    executarReST("JanelaDescarga/AlterarHorarioDescarregamentoPorPeriodo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.HorarioDescarregamentoInferiorAtual) {
                    exibirConfirmacao("Confirmação", retorno.Msg + " Deseja sobrepor essa regra?", function () {
                        permitirHorarioDescarregamentoInferiorAoAtual = true;
                        alocarHorarioDescarregamentoPeriodo(permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido);
                    });
                    return;
                }

                if (retorno.Data.HorarioLimiteDescarregamentoAtingido) {
                    exibirConfirmacao("Confirmação", retorno.Msg + " Deseja sobrepor essa regra?", function () {
                        permitirHorarioDescarregamentoComLimiteAtingido = true;
                        alocarHorarioDescarregamentoPeriodo(permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido);
                    });
                    return;
                }

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Horário alterado com sucesso!");
                fecharModalHorarioDescarregamentoPeriodo();
                _tabelaDescarregamento.Load();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function alocarHorariosDescarregamentosPeriodos() {
    if (!ValidarCamposObrigatorios(_horarioDescarregamentoPeriodo)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    var dados = {
        Codigos: JSON.stringify(_codigosJanelaDescarregamentoPeriodo),
        DataDescarregamento: _horarioDescarregamentoPeriodo.DataDescarregamento.val(),
        PeriodoDescarregamento: _horarioDescarregamentoPeriodo.PeriodoDescarregamento.codEntity(),
        MotivoReagendamento: _horarioDescarregamentoPeriodo.MotivoReagendamento.val(),
        SolicitouMotivo: _horarioDescarregamentoPeriodo.SolicitouMotivo.val(),
    };

    executarReST("JanelaDescarga/AlterarMultiplosHorariosDescarregamentosPorPeriodo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Horário alterado com sucesso!");
                fecharModalHorarioDescarregamentoPeriodo();
                _tabelaDescarregamento.Load();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });

    _horarioDescarregamentoPeriodo.AlocarHorario.visible(true);
    _horarioDescarregamentoPeriodo.AlocarHorarios.visible(false);
}

function exibirModalHorarioDescarregamentoPeriodo(solicitouMotivo) {
    Global.abrirModal('divModalHorarioDescarregamentoPeriodo');

    _horarioDescarregamentoPeriodo.MotivoReagendamento.visible(solicitouMotivo);

    $("#divModalHorarioDescarregamentoPeriodo").one('hidden.bs.modal', function () {
        LimparCampos(_horarioDescarregamentoPeriodo);
        _codigosJanelaDescarregamentoPeriodo = [];
    })
}

function fecharModalHorarioDescarregamentoPeriodo() {
    Global.fecharModal("divModalHorarioDescarregamentoPeriodo");
}

function retornoConsultaHorarioDescarregamentoPeriodo(registroSelecionado) {
    _horarioDescarregamentoPeriodo.DataDescarregamento.val(registroSelecionado.DataDescarregamento)
    _horarioDescarregamentoPeriodo.PeriodoDescarregamento.codEntity(registroSelecionado.Codigo)
    _horarioDescarregamentoPeriodo.PeriodoDescarregamento.entityDescription(registroSelecionado.Descricao);
    _horarioDescarregamentoPeriodo.PeriodoDescarregamento.val(registroSelecionado.Descricao);
}
