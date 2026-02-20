namespace Servicos.Embarcador.Produto
{
    public class Custo
    {
        public static string ObterFormulaPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

            if (configuracaoDocumentoEntrada != null && !string.IsNullOrWhiteSpace(configuracaoDocumentoEntrada.FormulaCustoPadrao))
                return configuracaoDocumentoEntrada.FormulaCustoPadrao;
            else
                return "#+ #ValorDiferencial #+ #ValorICMSST #+ #ValorIPI #+ #ValorFrete #+ #ValorSeguro #+ #ValorOutras #- #ValorDesconto #- #ValorDescontoFora #+ #ValorImpostoFora #+ #ValorOutrasFora #+ #ValorFreteFora #- #ValorICMSFreteFora #+ #ValorDiferencialFreteFora #- #ValorCreditoPresumido";

            //return "#- #ValorICMS #+ #ValorDiferencial #+ #ValorICMSST #+ #ValorIPI #+ #ValorFrete #+ #ValorSeguro #+ #ValorOutras #- #ValorDesconto #- #ValorDescontoFora #+ #ValorImpostoFora #+ #ValorOutrasFora #+ #ValorFreteFora #- #ValorICMSFreteFora #+ #ValorDiferencialFreteFora";
        }

        //public static decimal CalcularCusto(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item, string formulaCustoProduto)
        //{
        //    decimal custoUnitario = 0m, custoTotal = 0m;

        //    formulaCustoProduto = formulaCustoProduto.Replace(" ", "").ToLower();

        //    custoUnitario = item.ValorTotal;

        //    if (formulaCustoProduto.Contains("#+#valordesconto"))
        //        custoUnitario += item.Desconto;

        //    if (formulaCustoProduto.Contains("#-#valordesconto"))
        //        custoUnitario -= item.Desconto;

        //    if (formulaCustoProduto.Contains("#+#valoroutras"))
        //        custoUnitario += item.OutrasDespesas;

        //    if (formulaCustoProduto.Contains("#-#valoroutras"))
        //        custoUnitario -= item.OutrasDespesas;

        //    if (formulaCustoProduto.Contains("#+#valordiferencial"))
        //        custoUnitario += item.ValorDiferencial;

        //    if (formulaCustoProduto.Contains("#-#valordiferencial"))
        //        custoUnitario -= item.ValorDiferencial;

        //            if (formulaCustoProduto.Contains("#+#valoricmsst"))

        //                if (formulaCustoProduto.Contains("#-#valoricmsst"))


        //                    if (formulaCustoProduto.Contains("#+"))

        //                        if (formulaCustoProduto.Contains("#-"))


        //                            if (formulaCustoProduto.Contains("#+"))


        //                                if (formulaCustoProduto.Contains("#-"))


        //                                    if (formulaCustoProduto.Contains("#+"))


        //                                        if (formulaCustoProduto.Contains("#-"))


        //                                            if (formulaCustoProduto.Contains("#+"))



        //                                                if (formulaCustoProduto.Contains("#-"))


        //                                                    if (formulaCustoProduto.Contains("#+"))


        //                                                        if (formulaCustoProduto.Contains("#-"))


        //                                                            if (formulaCustoProduto.Contains("#+"))



        //                                                                if (formulaCustoProduto.Contains("#-"))


        //                                                                    if (formulaCustoProduto.Contains("#+"))


        //                                                                        if (formulaCustoProduto.Contains("#-"))


        //                                                                            if (formulaCustoProduto.Contains("#+"))



        //                                                                                if (formulaCustoProduto.Contains("#-"))





        //                                                                                    if (formulaCustoProduto.Contains("#-"))


        //                                                                                        if (formulaCustoProduto.Contains("#+"))



        //                                                                                            if (formulaCustoProduto.Contains("#-"))


        //                                                                                                if (formulaCustoProduto.Contains("#+"))




        //                                                                                                    if (formulaCustoProduto.Contains("#-"))


        //                                                                                                        if (formulaCustoProduto.Contains("#+"))




        //                                                                                                            if (formulaCustoProduto.Contains("#-"))


        //                                                                                                                if (formulaCustoProduto.Contains("#+"))




        //                                                                                                                    if (formulaCustoProduto.Contains("#-"))


        //                                                                                                                        if (formulaCustoProduto.Contains("#+"))



        //                                                                                                                            if (formulaCustoProduto.Contains("#-"))


        //                                                                                                                                if (formulaCustoProduto.Contains("#+"))


        //                                                                                                                                    if (formulaCustoProduto.Contains("#-"))


        //                                                                                                                                        if (formulaCustoProduto.Contains("#+"))



        //                                                                                                                                            if (formulaCustoProduto.Contains("#+"))



        //                                                                                                                                            //    custoUnitario = (valorTotal);

        //                                                                                                                                            //    if (valorDesconto > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_Desconto")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorDesconto;
        //                                                                                                                                            //else if ($("#btnValores_Desconto")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorDesconto;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorOutras > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_OutrasDespesas")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorOutras;
        //                                                                                                                                            //else if ($("#btnValores_OutrasDespesas")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorOutras;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorFrete > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_Frete")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorFrete;
        //                                                                                                                                            //else if ($("#btnValores_Frete")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorFrete;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorSeguro > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_Seguro")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorSeguro;
        //                                                                                                                                            //else if ($("#btnValores_Seguro")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorSeguro;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorICMS > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_ICMS")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorICMS;
        //                                                                                                                                            //else if ($("#btnValores_ICMS")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorICMS;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorIPI > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_IPI")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorIPI;
        //                                                                                                                                            //else if ($("#btnValores_IPI")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorIPI;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorICMSST > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_ICMSST")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorICMSST;
        //                                                                                                                                            //else if ($("#btnValores_ICMSST")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorICMSST;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorDiferencial > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_Diferencial")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorDiferencial;
        //                                                                                                                                            //else if ($("#btnValores_Diferencial")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorDiferencial;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorFreteFora > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_FreteFora")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorFreteFora;
        //                                                                                                                                            //else if ($("#btnValores_FreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorFreteFora;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorOutrasFora > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_OutrasDespesasFora")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorOutrasFora;
        //                                                                                                                                            //else if ($("#btnValores_OutrasDespesasFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorOutrasFora;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorImpostoFora > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_ImpostosFora")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorImpostoFora;
        //                                                                                                                                            //else if ($("#btnValores_ImpostosFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorImpostoFora;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorDiferencialFreteFora > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_DiferencialFreteFora")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorDiferencialFreteFora;
        //                                                                                                                                            //else if ($("#btnValores_DiferencialFreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorDiferencialFreteFora;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorICMSFreteFora > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_ICMSFreteFora")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorICMSFreteFora;
        //                                                                                                                                            //else if ($("#btnValores_ICMSFreteFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorICMSFreteFora;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    if (valorDescontoFora > 0)
        //                                                                                                                                            //    {
        //                                                                                                                                            //        if ($("#btnValores_DescontoFora")[0].title.trim().toUpperCase() == "SOMAR")
        //                                                                                                                                            //    custoUnitario = custoUnitario + valorDescontoFora;
        //                                                                                                                                            //else if ($("#btnValores_DescontoFora")[0].title.trim().toUpperCase() == "SUBTRAIR")
        //                                                                                                                                            //    custoUnitario = custoUnitario - valorDescontoFora;
        //                                                                                                                                            //    }

        //                                                                                                                                            //    custoTotal = custoUnitario;
        //                                                                                                                                            //    custoUnitario = custoUnitario / quantidade;

        //                                                                                                                                            //    if (custoUnitario > 0 && custoTotal > 0)
        //                                                                                                                                            {
        //                                                                                                                                                _item.ValorCustoUnitario.val(Globalize.format(custoUnitario, "n2"));
        //                                                                                                                                                _item.ValorCustoTotal.val(Globalize.format(custoTotal, "n2"));
        //                                                                                                                                            }
        //                                                                                                                                            else
        //                                                                                                                                            {
        //                                                                                                                                                _item.ValorCustoUnitario.val("0,00");
        //                                                                                                                                                _item.ValorCustoTotal.val("0,00");
        //                                                                                                                                            }


        //}
    }
}
