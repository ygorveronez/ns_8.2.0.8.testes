/// <reference path="ContratoFreteTerceiros.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="Pedagio.js" />
/// <reference path="Abastecimento.js" />
//*******MAPEAMENTO KNOUCKOUT*******



//*******EVENTOS*******

function LoadConfiguracaoFinanceira() {

    LoadConfiguracaoContratoFreteTerceiros();
    LoadConfiguracaoAcertoViagem();
    LoadConfiguracaoPedagio();
    LoadConfiguracaoAbastecimento();
    LoadConfiguracaoFatura();
    LoadConfiguracaoBaixaTituloReceber();
    LoadConfiguracaoGNRE();

    BuscarConfiguracaoFinanceira();
}

//*******MÉTODOS*******


function BuscarConfiguracaoFinanceira() {
    executarReST("ConfiguracaoFinanceira/ObterDetalhes", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_configuracaoContratoFreteTerceiros, { Data: r.Data.ConfiguracaoContratoFreteTerceiros });
                PreencherObjetoKnout(_configuracaoAcertoViagem, { Data: r.Data.ConfiguracaoContratoAcertoViagem });
                PreencherObjetoKnout(_configuracaoPedagio, { Data: r.Data.ConfiguracaoPedagio });
                PreencherObjetoKnout(_configuracaoAbastecimento, { Data: r.Data.ConfiguracaoAbastecimento });
                PreencherObjetoKnout(_configuracaoFatura, { Data: r.Data.ConfiguracaoFatura });
                PreencherObjetoKnout(_configuracaoBaixaTituloReceber, { Data: r.Data.ConfiguracaoBaixaTituloReceber });
                PreencherObjetoKnout(_configuracaoGNRE, { Data: r.Data.ConfiguracaoGNRE });

                RecarregarGridConfiguracaoContratoFreteTerceirosTipoOperacao();
                RecarregarGridConfiguracaoBaixaTituloReceberMoeda();
                RecarregarGridConfiguracaoGNRERegistro();
                RecarregarGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento();
                RecarregarGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao();

                $("#divContent").removeClass("hidden");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}