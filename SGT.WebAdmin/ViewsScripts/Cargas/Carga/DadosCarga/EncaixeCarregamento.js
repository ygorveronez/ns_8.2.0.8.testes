/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="DataCarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridHorariosEncaixa;
var _encaixeCarregamento;

var EncaixeCarregamento = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEncaixe = PropertyEntity({ getType: typesKnockout.string });
    this.HoraEncaixe = PropertyEntity({ getType: typesKnockout.string });
    this.DescricaoTipoOperacaoEncaixe = PropertyEntity({ getType: typesKnockout.string });
    this.TipoOperacaoEncaixe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataEncaixe.getFieldDescription(), required: true, getType: typesKnockout.dateTime });
    this.BuscarEncaixe = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Buscar, eventClick: buscarEncaixeClick, type: types.event, idGrid: guid() });

    this.Confirmar = PropertyEntity({ type: types.event, eventClick: confirmarEncaixeCarregamentoClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: voltarParaSelecaoHorariosClick, text: Localization.Resources.Cargas.Carga.Fechar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function abrirModalEncaixeCarregamento(e) {
    _encaixeCarregamento = new EncaixeCarregamento();
    _encaixeCarregamento.DataCarregamento.val.subscribe(function (data) {
        var dataHoraSeparado = data.split(" ");

        _encaixeCarregamento.DataEncaixe.val(dataHoraSeparado[0] || "__/__/____");
        _encaixeCarregamento.HoraEncaixe.val(dataHoraSeparado[1] || "00:00");
    });

    _encaixeCarregamento.Carga.val(e.Carga.val());
    _encaixeCarregamento.DataCarregamento.val(e.DataCarregamentoDisponibilidade.val() + " 00:00");

    KoBindings(_encaixeCarregamento, "knoutEncaixeCarregamento");

    CarregarGridHorariosEncaixe();

    Global.abrirModal("divModalEncaixeCarregamento");
}

function voltarParaSelecaoHorariosClick() {
    Global.fecharModal("divModalEncaixeCarregamento");
    Global.abrirModal("divModalAlterarCarregamento");
}

function buscarEncaixeClick() {
    AtualizarHorariosEncaixe();
}

function confirmarEncaixeCarregamentoClick() {
    var _executarAlteracaoData = function (setarComoNaoComparecimento) {
        var dados = {
            Carga: _encaixeCarregamento.Carga.val(),
            DataCarregamento: _encaixeCarregamento.DataCarregamento.val(),
            EncaixarHorario: true,
            TipoOperacaoEncaixe: _encaixeCarregamento.TipoOperacaoEncaixe.val(),
            NaoComparecimento: setarComoNaoComparecimento
        };
        executarReST("Carga/AlterarDataCarregamento", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DataAlteradaComSucesso);
                    _knoutCargaAlterarData.DataCarregamento.val(_encaixeCarregamento.DataCarregamento.val());
                    Global.fecharModal("divModalEncaixeCarregamento");
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }

    executarReST("JanelaCarregamento/VerificarPossibilidadeModificacaoJanela", { Carga: _encaixeCarregamento.Carga.val(), Data: _encaixeCarregamento.DataCarregamento.val() }, function (retornoVerificacao) {
        if (!retornoVerificacao.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retornoVerificacao.Msg);

        if (retornoVerificacao.Data.PossibilidadeNoShow)
            exibirConfirmacao(Localization.Resources.Cargas.Carga.NoShow, Localization.Resources.Cargas.Carga.DesejaMarcarCargaComoNoShow, function () { _executarAlteracaoData(true) }, function () { _executarAlteracaoData(false) });
        else
            _executarAlteracaoData(false);
    });
}


//*******MÉTODOS*******

function CarregarGridHorariosEncaixe() {
    if (_gridHorariosEncaixa) _gridHorariosEncaixa.Destroy();

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function (obj, data) {
            HorarioEncaixeSelecionado(data);
        },
        callbackNaoSelecionado: function () {
        },
        eventos: function () {
        },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    _gridHorariosEncaixa = new GridView(_encaixeCarregamento.BuscarEncaixe.idGrid, "JanelaCarregamento/ObterEncaixesDisponiveis", _encaixeCarregamento, null, null, 10, null, null, null, multiplaescolha);
    _gridHorariosEncaixa.CarregarGrid();
}

function HorarioEncaixeSelecionado(data) {
    _gridHorariosEncaixa.AtualizarRegistrosSelecionados([data]);
    _gridHorariosEncaixa.DrawTable(true);

    _encaixeCarregamento.TipoOperacaoEncaixe.val(data.Codigo);
    _encaixeCarregamento.DescricaoTipoOperacaoEncaixe.val(data.TipoOperacao);
}

function LimparHorarioEncaixeSelecionado() {
    _gridHorariosEncaixa.AtualizarRegistrosSelecionados([]);
    _gridHorariosEncaixa.DrawTable();
    _encaixeCarregamento.TipoOperacaoEncaixe.val(0);
    _encaixeCarregamento.DescricaoTipoOperacaoEncaixe.val("");
}

function AtualizarHorariosEncaixe() {
    LimparHorarioEncaixeSelecionado();
    if (!_encaixeCarregamento.DataCarregamento.val()) return;
    _gridHorariosEncaixa.CarregarGrid();
}