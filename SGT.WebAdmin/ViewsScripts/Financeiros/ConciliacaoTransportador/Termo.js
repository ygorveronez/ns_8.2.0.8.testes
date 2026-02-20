
async function popularTermo(codigoConciliacaoTransportador) {
    let html = await obterHtmlTermo(codigoConciliacaoTransportador);
    $('#termos').html(html);
}

async function obterHtmlTermo(codigoConciliacaoTransportador) {
    const dadosTermo = await obterDadosTermo(codigoConciliacaoTransportador);

    const transportador = dadosTermo.TransportadorParaAssinar;
    return `
        <b>TERMO DE DECLARAÇÃO DE QUITAÇÃO E RENÚNCIA</b>

        <p>            
            Através do presente instrumento e na melhor forma de direito, a empresa ${transportador.NomeEmpresa}, pessoa jurídica de direito privado, registrada no Cadastro Nacional de Pessoas Jurídicas CNPJ sob o nº ${transportador.CNPJ},
            com seu endereço na Rua ${transportador.Endereco}, nº ${transportador.Numero}, Bairro ${transportador.Bairro}, na cidade de ${transportador.Cidade}, estado de ${transportador.Estado}, vem, por meio deste,
            na forma dos artigos 319 e 320 do Código Civil, DECLARAR para os devidos fins de direito ter recebido da Marfrig Global Foods SA, pessoa jurídica de direito privado, registrada no Cadastro Nacional de Pessoas Jurídicas CNPJ sob o
            nº 03.853.896/0001-40 ou de qualquer uma de suas filiais (Grupo Marfrig) e suas subsidiárias, a importância de R$ ${dadosTermo.ValorConciliacao}, dando, assim, plena, geral, irrestrita e irrevogável quitação quanto ao
            recebimento integral do(s) serviço(s) de transporte(s) prestado(s) ao Grupo Marfrig e suas subsidiárias no período compreendido entre ${dadosTermo.DataInicial} e ${dadosTermo.DataFinal}.
        </p>

        <p>
            O presente Termo de Quitação serve ainda como irrevogável declaração de inexistência de outras pendências (conhecidas ou não) decorrentes de serviços de transportes que tenham sido originados no período acima descrito,
            não restando, portanto, débitos pendentes de apuração ou cobranças ou a serem faturadas que tenham origem no período.
        </p>

        <p>
            O aqui atestado implica na declaração da livre vontade de renunciar completamente a eventuais direitos de qualquer natureza oponíveis em relação ao Grupo Marfrig e suas subsidiárias que sejam decorrentes dos
            serviços de transportes ocorridos no período entre ${dadosTermo.DataInicial} e ${dadosTermo.DataFinal}.
        </p>
    `;
}

function obterDadosTermo(codigoConciliacaoTransportador) {
    return new Promise(resolve => {
        executarReST("ConciliacaoTransportador/ObterDadosTermo", { Codigo: codigoConciliacaoTransportador }, function (arg) {
            if (arg.Success) {
                resolve(arg.Data);
            } else {
                resolve(null);
            }
        });
    });
}
