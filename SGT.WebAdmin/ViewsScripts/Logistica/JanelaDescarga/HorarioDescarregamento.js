/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="JanelaDescarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _horarioDescarregamento;

/*
 * Declaração das Classes
 */

var HorarioDescarregamento = function () {
    this.Codigo = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.SolicitouMotivo = PropertyEntity({ type: types.bool, val: ko.observable(0), def: 0 });
    this.NovoHorario = PropertyEntity({ text: "*Novo Horário", getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: true });
    this.MotivoReagendamento = PropertyEntity({ text: "Motivo: ", maxlength: 1000, required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.SolicitouAgendaExtra = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.SolicitouAgendaAcimaDoLimite = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });

    this.AlocarHorario = PropertyEntity({ eventClick: alocarHorarioDescarregamentoClick, type: types.event, text: "Alterar Horário", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadHorarioDescarregamento() {
    _horarioDescarregamento = new HorarioDescarregamento();
    KoBindings(_horarioDescarregamento, "knockoutHorarioDescarregamento");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function alocarHorarioDescarregamentoClick() {
    if (!ValidarCamposObrigatorios(_horarioDescarregamento)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    executarReST("JanelaDescarga/AlterarHorarioDescarregamento", RetornarObjetoPesquisa(_horarioDescarregamento), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (!retorno.Data.SolicitouAgendaExtra && !retorno.Data.SolicitouAgendaAcimaDoLimite)
                {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Horário alterado com sucesso!");
                    fecharModalHorarioDescarregamento();
                    _tabelaDescarregamento.Load();
                }
                else
                    alterarHorarioDescarregamentoExtra(retorno.Data.SolicitouAgendaAcimaDoLimite);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalAlteracaoHorarioDescarregamento(codigoJanelaDescarregamento, solicitarMotivo) {
    _horarioDescarregamento.Codigo.val(codigoJanelaDescarregamento);
    _horarioDescarregamento.SolicitouMotivo.val(solicitarMotivo);

    Global.abrirModal('divModalHorarioDescarregamento');
    $("#divModalHorarioDescarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_horarioDescarregamento);
    });
}

/*
 * Declaração das Funções Privadas
 */

function alterarHorarioDescarregamentoExtra(solicitouAgendaAcimaDoLimite) {
    _horarioDescarregamento.SolicitouAgendaExtra.val(!solicitouAgendaAcimaDoLimite);
    _horarioDescarregamento.SolicitouAgendaAcimaDoLimite.val(solicitouAgendaAcimaDoLimite);

    var mensagem = solicitouAgendaAcimaDoLimite ? "O centro de descarregamento já atingiu o limite para esse grupo de pessoas, deseja alocar mesmo assim?" : "Não foi encontrado um horário. Deseja alocar essa agenda como extra?";
    
    exibirConfirmacao("Confirmação", mensagem, function () {
        executarReST("JanelaDescarga/AlterarHorarioDescarregamento", RetornarObjetoPesquisa(_horarioDescarregamento) , function (retorno) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Horário alterado com sucesso!");
                fecharModalHorarioDescarregamento();
                _tabelaDescarregamento.Load();
            } else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        })
        _horarioDescarregamento.SolicitouAgendaExtra.val(false);
        _horarioDescarregamento.SolicitouAgendaAcimaDoLimite.val(false);
    }, function () {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "O horário não foi alterado.")
        _horarioDescarregamento.SolicitouAgendaExtra.val(false);
        _horarioDescarregamento.SolicitouAgendaAcimaDoLimite.val(false);
    });
}

function fecharModalHorarioDescarregamento() {
    Global.fecharModal("divModalHorarioDescarregamento");
}
