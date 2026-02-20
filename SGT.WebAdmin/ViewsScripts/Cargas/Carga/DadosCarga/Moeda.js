function ObterCotacaoMoedaCarga(carga, tipoMoeda) {
    var dados = {
        Carga: carga.Codigo.val(),
        Moeda: tipoMoeda
    };

    if (tipoMoeda === EnumMoedaCotacaoBancoCentral.Real) {
        carga.ValorCotacaoMoeda.val("1,0000000000");
        ConverterValorMoedaCarga(carga);
    } else {
        executarReST("Cotacao/ConverterMoedaEstrangeiraPedido", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    carga.ValorCotacaoMoeda.val(r.Data);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    carga.ValorCotacaoMoeda.val("0,0000000000");
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                carga.ValorCotacaoMoeda.val("0,0000000000");
            }

            ConverterValorMoedaCarga(carga);
        });
    }
}

function ConverterValorMoedaCarga(carga) {
    var valorCotacaoMoeda = Globalize.parseFloat(carga.ValorCotacaoMoeda.val() || "0");
    var valorTotalMoeda = Globalize.parseFloat(carga.ValorTotalMoeda.val() || "0");

    if (isNaN(valorCotacaoMoeda))
        valorCotacaoMoeda = 0;
    if (isNaN(valorTotalMoeda))
        valorTotalMoeda = 0;

    var valorTotalConvertido = valorCotacaoMoeda * valorTotalMoeda;

    if (valorTotalConvertido > 0)
        carga.ValorFreteOperador.val(Globalize.format(valorTotalConvertido, "n2"));
    else
        carga.ValorFreteOperador.val("");
}