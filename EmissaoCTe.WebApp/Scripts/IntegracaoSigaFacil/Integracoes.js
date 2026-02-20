function AbrirTelaIntegracoes(ciot) {
    var dados = {
        InicioRegistros: 0,
        CIOT: ciot.data.Codigo
    };

    var opcoes = [];
    opcoes.push({ Descricao: "Requisição", Evento: DownloadRequisicaoXMLIntegracao });
    opcoes.push({ Descricao: "Resposta", Evento: DownloadRespostaXMLIntegracao });

    CriarGridView("/IntegracaoSigaFacil/ConsultarIntegracoes?callback=?", dados, "tbl_integracao_table", "tbl_integracao", "tbl_paginacao_integracao", opcoes, [0], null);
    $("#divIntegracoesCIOT").modal();
}

function DownloadRequisicaoXMLIntegracao(integracao) {
    factoryDownloadRespostaXMLIntegracao(integracao.data.Codigo, "Requisicao");
}

function DownloadRespostaXMLIntegracao(integracao) {
    factoryDownloadRespostaXMLIntegracao(integracao.data.Codigo, "Resposta");
}

function factoryDownloadRespostaXMLIntegracao(codigo, tipo) {
    executarDownload("/IntegracaoSigaFacil/DownloadXMLIntegracao", { Integracao: codigo, Tipo: tipo }, null, null, true, "messages-placeholder-integracaoCIOT");
}