/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _horarioCarregamento;

/*
 * Declaração das Classes
 */

var HorarioCarregamento = function () {
    this.Codigo = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.DataAtual = PropertyEntity({});
    this.NovoHorario = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NovoHorario, getType: typesKnockout.time, val: ko.observable(""), def: "", required: true });
    this.ConfirmarNaoComparecimento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.AlocarHorario = PropertyEntity({ eventClick: alocarHorarioCarregamentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.AlterarHorario, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadHorarioCarregamento() {
    _horarioCarregamento = new HorarioCarregamento();
    KoBindings(_horarioCarregamento, "knockoutHorarioCarregamento");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function alocarHorarioCarregamentoClick() {
    if (!ValidarCamposObrigatorios(_horarioCarregamento)) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var dados = {
        Codigo: _horarioCarregamento.Codigo.val(),
        NovoHorario: moment(_horarioCarregamento.DataAtual.val() + _horarioCarregamento.NovoHorario.val(), "DD/MM/YYYY HH:mm").format("DD/MM/YYYY HH:mm"),
    };

    var apiAlocarCarregamento = function (setarComoNaoComparecimento) {
        executarReST("JanelaCarregamento/AlocarHorarioCarregamento", { Codigo: dados.Codigo, NovoHorario: dados.NovoHorario, NaoComparecimento: setarComoNaoComparecimento }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.HorarioAlocadoComSucesso);
                    fecharModalHorarioCarregamento();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    };

    executarReST("JanelaCarregamento/VerificarPossibilidadeModificacaoJanela", { JanelaCarregamento: dados.Codigo, Data: dados.NovoHorario }, function (retornoVerificacao) {
        if (!retornoVerificacao.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retornoVerificacao.Msg);

        if (!retornoVerificacao.Data.PermiteModificarHorario)
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retornoVerificacao.Data.MensagemPermiteModificarHorario);

        if (retornoVerificacao.Data.PossibilidadeNoShow)
            exibirConfirmacao(Localization.Resources.Cargas.Carga.NoShow, Localization.Resources.Cargas.Carga.DesejaMarcarCargaComoNoShow, function () { apiAlocarCarregamento(true) }, function () { apiAlocarCarregamento(false) });
        else
            apiAlocarCarregamento(false);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirModalAlteracaoHorarioCarregamento(codigoJanelaCarregamento, dataAtual, confirmarNaoComparecimento) {
    _horarioCarregamento.Codigo.val(codigoJanelaCarregamento);
    _horarioCarregamento.DataAtual.val(dataAtual);
    _horarioCarregamento.ConfirmarNaoComparecimento.val(confirmarNaoComparecimento);

    Global.abrirModal('divModalHorarioCarregamento');
    $("#divModalHorarioCarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_horarioCarregamento);
    });
}

/*
 * Declaração das Funções Privadas
 */

function fecharModalHorarioCarregamento() {
    Global.fecharModal('divModalHorarioCarregamento');
}
