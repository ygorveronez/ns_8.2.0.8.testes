var _tendenciaParadas;

var TendenciaParadas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataReferencia = PropertyEntity({ val: ko.observable(EnumDataBaseAlertas.PrevisaoEntrega), options: EnumDataBaseAlertas.obterOpcoes(), def: EnumDataBaseAlertas.PrevisaoEntrega, text: Localization.Resources.Cargas.ControleEntrega.DataReferencia, visible: ko.observable(true) });
    this.ConsiderarDataEntradaComoDestino = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Cargas.ControleEntrega.ConsiderarDataEntradaComoDestino, visible: ko.observable(true), getType: typesKnockout.bool })
    this.TempoAdiantado = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Cargas.ControleEntrega.TempoAdiantado, visible: ko.observable(true), getType: typesKnockout.bool })
    this.TempoAdiantadoColeta = PropertyEntity({ val: ko.observable(''), text: Localization.Resources.Cargas.ControleEntrega.Coleta, visible: ko.observable(true), getType: typesKnockout.time, enable: ko.observable(true) })
    this.TempoAdiantadoEntrega = PropertyEntity({ val: ko.observable(''), text: Localization.Resources.Cargas.ControleEntrega.Entrega, visible: ko.observable(true), getType: typesKnockout.time, enable: ko.observable(true) })
    this.TempoAtrasado = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Cargas.ControleEntrega.TempoAtrasado, visible: ko.observable(true), getType: typesKnockout.bool })
    this.TempoAtrasadoColeta = PropertyEntity({ val: ko.observable(''), text: Localization.Resources.Cargas.ControleEntrega.Coleta, visible: ko.observable(true), getType: typesKnockout.time, enable: ko.observable(true) })
    this.TempoAtrasadoEntrega = PropertyEntity({ val: ko.observable(''), text: Localization.Resources.Cargas.ControleEntrega.Entrega, visible: ko.observable(true), getType: typesKnockout.time, enable: ko.observable(true) })

    this.TempoAdiantado.val.subscribe((value) => {
        this.TempoAdiantadoColeta.enable(value);
        this.TempoAdiantadoEntrega.enable(value);
    });

    this.TempoAtrasado.val.subscribe((value) => {
        this.TempoAtrasadoColeta.enable(value);
        this.TempoAtrasadoEntrega.enable(value);
    });

    this.tempoAdiantadoDesativado = ko.computed(() => !this.TempoAdiantado.val());
    this.tempoAtrasadoDesativado = ko.computed(() => !this.TempoAtrasado.val());

    this.Salvar = PropertyEntity({ eventClick: salvarConfiguracao, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
}

function loadTendenciaParadas() {

    _tendenciaParadas = new TendenciaParadas();
    KoBindings(_tendenciaParadas, "knockoutTendenciaParadas");

    loadConfiguracaoTendenciaParadas();
}
function loadConfiguracaoTendenciaParadas() {
    executarReST("TendenciaParadas/BuscarConfiguracao", null, function (arg) {
        if (arg.Success) {
            PreencherObjetoKnout(_tendenciaParadas, arg);
            HeaderAuditoria("AcompanhamentoEntregaTempoConfiguracao", _tendenciaParadas);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function salvarConfiguracao() {
    let dados = {
        Codigo: _tendenciaParadas.Codigo.val(),
        DataReferencia: _tendenciaParadas.DataReferencia.val(),
        ConsiderarDataEntradaComoDestino: _tendenciaParadas.ConsiderarDataEntradaComoDestino.val(),
        TempoAdiantado: _tendenciaParadas.TempoAdiantado.val(),
        TempoAdiantadoColeta: _tendenciaParadas.TempoAdiantadoColeta.val(),
        TempoAdiantadoEntrega: _tendenciaParadas.TempoAdiantadoEntrega.val(),
        TempoAtrasado: _tendenciaParadas.TempoAtrasado.val(),
        TempoAtrasadoColeta: _tendenciaParadas.TempoAtrasadoColeta.val(),
        TempoAtrasadoEntrega: _tendenciaParadas.TempoAtrasadoEntrega.val()
    };
    executarReST("TendenciaParadas/SalvarConfiguracao", dados, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}