/**
 * Objeto com o resultado de cada calculo que compoe o valor total
 * Sempre que for precisa recalcular um componente, baste atualizar no objeto
 * e posteriormente invocar a função de calculo total
 * Com isso, não é necessário recalcular todo o total por modificar um componente apenas
 */
var ResultantesCalculo = {
    Diaria: 0,
    Acrescimos: 0,
    Descontos: 0,
    CTes: 0,
    KM: 0,
    Coletas: 0
}

function LimparResultantes() {
    ResultantesCalculo.Diaria = 0;
    ResultantesCalculo.Acrescimos = 0;
    ResultantesCalculo.CTes = 0;
    ResultantesCalculo.KM = 0;
    ResultantesCalculo.Coletas = 0;
    ResultantesCalculo.Descontos = 0;
}

function CalculaTotal() {
    var totalSoma =
        ResultantesCalculo.Diaria +
        ResultantesCalculo.Acrescimos +
        ResultantesCalculo.CTes +
        ResultantesCalculo.KM +
        ResultantesCalculo.Coletas;

    var totalSubtracao = ResultantesCalculo.Descontos;

    var total = totalSoma - totalSubtracao;

    if (CalculoRelacaoCTesEntregues != null)
        $("#txtValorTotal").val(Globalize.format(total, "n2"));
}

/**
 * Se existir um valor para Diaria/Meia Diaria, retorna o valor configurado
 * Se o valor na configuracao estiver em branco, retorna o valor digitado
 */
function ValorDiaria() {
    var valorDiaria = 0;
    switch (parseInt($("#selTipoDiaria").val())) {
        case EnumTipoDiariaRelacaoCTesEntregues.Diaria:
            if (CalculoRelacaoCTesEntregues != null && CalculoRelacaoCTesEntregues.ValorDiaria > 0)
                valorDiaria = CalculoRelacaoCTesEntregues.ValorDiaria;
            else
                valorDiaria = Globalize.parseFloat($("#txtDiaria").val());
            break;

        case EnumTipoDiariaRelacaoCTesEntregues.MeiaDiaria:
            if (CalculoRelacaoCTesEntregues != null && CalculoRelacaoCTesEntregues.ValorMeiaDiaria > 0)
                valorDiaria = CalculoRelacaoCTesEntregues.ValorMeiaDiaria;
            else
                valorDiaria = Globalize.parseFloat($("#txtDiaria").val());
            break;
    }

    ResultantesCalculo.Diaria = valorDiaria;
    CalculaTotal();
}

/**
 * Retornar o valor digitado
 */
function ValorAcrescimos() {
    ResultantesCalculo.Acrescimos = Globalize.parseFloat($("#txtValorAcrescimos").val());
    CalculaTotal();
}
function ValorDescontos() {
    ResultantesCalculo.Descontos = Globalize.parseFloat($("#txtValorDescontos").val());
    CalculaTotal();
}

/**
 * Obtem o valor calculado e atualiza o total
 *
 * Regra 1:
 * Quando existe valor na config "Valor mínimo para CTes com mesmo destino"
 * calcula-se o valor agrupando por destinatario
 *
 * Regra 2:
 * É obtido o percentual por CT-e em cima do Valor a Receber de cada CT-e
 * Caso o valor individual for menor que o valor mínimo, usa-se o valor mínimo
 * Se houver uma configuração específica de percentula por cidade, usa o % específico
 *
 * Regra 3:
 * Divide-se o peso do CT-e pelo valor configurado (se tiver), sempre arredondando para baixo
 * Multiplica-se pelo valor por fracao
 */
function CalculaTotalCTes() {
    if (CalculoRelacaoCTesEntregues == null) {
        var cnpjEmitente = "0";
        StateCTes.get().forEach(function (info) {
            if (!info.Excluir) {
                cnpjEmitente = info.CNPJEmitente;
            }
        });

        var dados = {
            codigo: $("#txtCliente").data('codigo'),
            emissor: cnpjEmitente
        };

        executarRest("/CalculoRelacaoCTesEntregues/ObterDetalhesPorCliente?callback=?", dados, function (r) {
            if (r.Sucesso) {
                CalculoRelacaoCTesEntregues = r.Objeto;

                CalculoRelacaoCTesEntreguesChange();
                Calcular();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });
    }
    else
        Calcular();

}


function Calcular() {
    if (CalculoRelacaoCTesEntregues == null)
        return;

    var ctes = StateCTes.get();
    var destinosCalculados = [];
    var totalCTes = 0;

    // Regra 1
    if (CalculoRelacaoCTesEntregues.ValorMinimoCTeMesmoDestino > 0) {
        ctes.forEach(function (cte) {
            if ($.inArray(cte.Destinatario, destinosCalculados) >= 0)
                return; // Destinatario ja calculado

            var ctesPorDestinatario = StateCTes.get({ Destinatario: cte.Destinatario });

            if (ctesPorDestinatario.length == 1)
                return; // Nao possui agrupamento

            // Adiciona nos ja calculado e sumariza agrupamento
            destinosCalculados.push(cte.Destinatario);
            totalCTes += CalculoRelacaoCTesEntregues.ValorMinimoCTeMesmoDestino;
        });
    }

    // Regra 2
    ctes.forEach(function (cte) {
        if ($.inArray(cte.Destinatario, destinosCalculados) >= 0)
            return; // Destinatario ja calculado

        var percentual = CalculoRelacaoCTesEntregues.PercentualPorCTe;

        var percentualPorCidade = BuscaPercentualPorCidade(cte.Cidade);
        if (percentualPorCidade != null)
            percentual = percentualPorCidade;

        var percentualSobreCTe = cte.ValorAReceber * (percentual / 100);
        if (percentualSobreCTe < CalculoRelacaoCTesEntregues.ValorMinimoPorCTe)
            percentualSobreCTe = CalculoRelacaoCTesEntregues.ValorMinimoPorCTe;

        totalCTes += percentualSobreCTe;
    });

    // Regra 3
    if (CalculoRelacaoCTesEntregues.FracaoKG > 0) {
        ctes.forEach(function (cte) {
            var quantidadesFracionadas = Math.floor(cte.Peso / CalculoRelacaoCTesEntregues.FracaoKG);
            var totalPorFracao = quantidadesFracionadas * CalculoRelacaoCTesEntregues.ValorPorFracao;

            totalCTes += totalPorFracao;
        });
    }

    ResultantesCalculo.CTes = totalCTes;
    CalculaTotal();
}



/**
 * Se possuir valor km excedente informada,
 * calcula independente de informar franquia ou nao
 */
function CalculaFranquiaKM() {
    if (CalculoRelacaoCTesEntregues == null || CalculoRelacaoCTesEntregues.ValorKMExcedente == 0)
        return;

    var kmInicial = Globalize.parseInt($("#txtKmInicial").val());
    var kmFinal = Globalize.parseInt($("#txtKmFinal").val());
    var kmTotal = kmFinal - kmInicial;

    var total = (kmTotal - CalculoRelacaoCTesEntregues.FranquiaKM) * CalculoRelacaoCTesEntregues.ValorKMExcedente;

    if (total < 0)
        total = 0;

    ResultantesCalculo.KM = total;
    CalculaTotal();
}




/**
 * Busca todas as coletas e obtem a soma
 */
function CalculaColetas() {
    if (CalculoRelacaoCTesEntregues == null)
        return;

    var coletas = StateColetas.get();
    var totalColetas = 0;

    coletas.forEach(function (coleta) {
        var quantidadesFracionadas = Math.floor(coleta.Peso / CalculoRelacaoCTesEntregues.FracaoKG);
        var totalPorFracao = quantidadesFracionadas * CalculoRelacaoCTesEntregues.ValorPorFracao;

        totalColetas += coleta.ValorEvento + coleta.ValorFracao;
    });

    ResultantesCalculo.Coletas = totalColetas;
    CalculaTotal();
}


/**
 * Recalcula os itens ao modificar ou limpar o campo cliente
 */
function RecalcularTodosItens() {
    ValorAcrescimos();
    ValorDescontos();
    ValorDiaria();
    CalculaTotalCTes();
    CalculaFranquiaKM();
    RecalcularTodasColetas();
}