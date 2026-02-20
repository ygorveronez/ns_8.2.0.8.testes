//*******MAPEAMENTO KNOUCKOUT*******

var _resumoBordero;

var ResumoBordero = function () {
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(false) });
    this.Tomador = PropertyEntity({ text: "Tomador: " });
    this.DataEmissao = PropertyEntity({ text: "Data de Emissão: " });
    this.DataVencimento = PropertyEntity({ text: "Data de Vencimento: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.ValorACobrar = PropertyEntity({ text: "Valor a Cobrar: " });
    this.ValorTotalAcrescimo = PropertyEntity({ text: "Valor do Acréscimo: " });
    this.ValorTotalDesconto = PropertyEntity({ text: "Valor do Desconto: " });
    this.ValorTotalACobrar = PropertyEntity({ text: "Valor Total a Cobrar: ", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadResumoBordero() {
    _resumoBordero = new ResumoBordero();
    KoBindings(_resumoBordero, "knockoutResumoBordero");
}

//*******MÉTODOS*******

function PreecherResumoBordero(dados) {
    _resumoBordero.Numero.visible(true);

    _resumoBordero.Numero.val(dados.Numero);

    if (dados.TipoPessoa == 1)
        _resumoBordero.Tomador.val(dados.Pessoa.Descricao);
    else
        _resumoBordero.Tomador.val(dados.GrupoPessoas.Descricao);

    _resumoBordero.DataEmissao.val(dados.DataEmissao);
    _resumoBordero.DataVencimento.val(dados.DataVencimento);
    _resumoBordero.Situacao.val(dados.DescricaoSituacao);
    _resumoBordero.ValorACobrar.val(dados.ValorACobrar);
    _resumoBordero.ValorTotalAcrescimo.val(dados.ValorTotalAcrescimo);
    _resumoBordero.ValorTotalDesconto.val(dados.ValorTotalDesconto);
    _resumoBordero.ValorTotalACobrar.val(dados.ValorTotalACobrar);
}

function LimparResumoBordero() {
    _resumoBordero.Numero.visible(false);
    LimparCampos(_resumoBordero);
}