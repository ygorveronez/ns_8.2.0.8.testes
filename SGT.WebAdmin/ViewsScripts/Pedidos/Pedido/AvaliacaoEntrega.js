
var _avaliacaoEntrega;

function AvaliacaoEntrega() {
    this.DataAvaliacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataAvaliacao.getFieldDescription() });
    this.TipoAvaliacaoGeral = PropertyEntity({ val: ko.observable(true) });
    this.Questionario = PropertyEntity({ perguntas: ko.observable([]) });
    this.Avaliacao = PropertyEntity({ estrelas: [5, 4, 3, 2, 1], val: ko.observable(0), def: 0 });
    this.MotivoAvaliacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.MotivoAvaliacao.getFieldDescription() });
    this.ObservacaoAvaliacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ObservacaoAvaliacao.getFieldDescription() });
}

function loadAvaliacaoEntrega() {
    _avaliacaoEntrega = new AvaliacaoEntrega();
    KoBindings(_avaliacaoEntrega, "knockoutAvaliacaoEntrega");
}


function preencherDadosAvaliacao(dados) {
    if (dados == null)
        return $("#tabAvaliacaoEntrega").hide();

    $("#tabAvaliacaoEntrega").show();
    PreencherObjetoKnout(_avaliacaoEntrega, { Data: dados });
    _avaliacaoEntrega.Questionario.perguntas(dados.Questionario);
}

function limparCamposDadosAvaliacao() {
    return $("#tabAvaliacaoEntrega").hide();
    _avaliacaoEntrega.Questionario.perguntas([]);
}