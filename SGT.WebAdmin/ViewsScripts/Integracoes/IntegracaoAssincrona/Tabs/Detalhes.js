var _detalhesIntegracaoIntegradora;

var DetalhesIntegracaoIntegradora = function () {
    this.Codigo = PropertyEntity({ visible: ko.observable(false) });
    this.TipoRequisicao = PropertyEntity({ text: "Tipo Requisição: ", val: ko.observable(""), def: "" });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(""), def: "" });
    this.DataCriacao = PropertyEntity({ text: "Data: ", val: ko.observable(""), def: "" });
    this.EtapaAtual = PropertyEntity({ text: "Etapa Atual: ", val: ko.observable(""), def: "" });
    this.Etapas = PropertyEntity({ text: "Etapas: ", val: ko.observable(""), def: "" });
}

function carregarDetalhesIngradoraIntegracao() {
    _detalhesIntegracaoIntegradora = new DetalhesIntegracaoIntegradora();

    KoBindings(_detalhesIntegracaoIntegradora, "knoutDetalhesIntegracaoAssincrona");
}