namespace Servicos.Embarcador.Carga
{
    public class RateioCarregamento
    {
        //public static Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete RatearValorFreteEntreCarregamentos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Cargas.Carga carga, ref List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> dadosCalculosRateiados, bool ulitmaCarga, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
        //    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

        //    decimal pesoLiquidoCarga = repCargaPedido.BuscarPesoLiquidoTotalPorCarga(carga.Codigo);
        //    decimal pesoLiquidoCarregamento = repCargaPedido.BuscarPesoLiquidoTotalPorCarregamento(carregamento.Codigo);

        //    decimal pesoCarga = repCargaPedido.BuscarPesoTotalPorCarga(carga.Codigo);
        //    decimal pesoCarregamento = repCargaPedido.BuscarPesoTotalPorCarregamento(carregamento.Codigo);

        //    decimal valorCarga = repPedidoXMLNotaFiscal.BuscarTotalPorCarga(carga.Codigo);
        //    decimal valorCarregamento = repPedidoXMLNotaFiscal.BuscarTotalPorCarregamento(carregamento.Codigo);

        //    decimal metrosCubicosCarga = repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCarga(carga.Codigo);
        //    decimal metrosCubicosCarregamento = repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCarregamento(carregamento.Codigo);

        //    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosRateiado = dados.Clonar();
        //    dadosRateiado.Componentes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente>();
        //    dadosRateiado.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
        //    RateioFormula serRateioFormula = new RateioFormula(unitOfWork.StringConexao);

        //    decimal valorFrete = serRateioFormula.AplicarFormulaRateio(formulaRateio, dados.ValorFrete, 0, 0, pesoCarregamento, pesoCarga, valorCarga, valorCarregamento, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, 0m, 0m, 0m, metrosCubicosCarga, metrosCubicosCarregamento, 0m, false, pesoLiquidoCarga, pesoLiquidoCarregamento);
        //    dadosRateiado.ValorFrete = Math.Round(valorFrete, 2, MidpointRounding.AwayFromZero);

        //    decimal valorFixo = serRateioFormula.AplicarFormulaRateio(formulaRateio, dados.ValorFixo, 0, 0, pesoCarregamento, pesoCarga, valorCarga, valorCarregamento, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, 0m, 0m, 0m, metrosCubicosCarga, metrosCubicosCarregamento, 0m, false, pesoLiquidoCarga, pesoLiquidoCarregamento);
        //    dadosRateiado.ValorFixo = Math.Round(valorFixo, 2, MidpointRounding.AwayFromZero);

        //    if (ulitmaCarga)
        //    {
        //        dadosRateiado.ValorFrete += dados.ValorFrete - (dadosCalculosRateiados.Sum(obj => obj.ValorFrete) + dadosRateiado.ValorFrete);
        //        dadosRateiado.ValorFixo += dados.ValorFixo - (dadosCalculosRateiados.Sum(obj => obj.ValorFixo) + dadosRateiado.ValorFixo);
        //    }

        //    foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente dadoCalculoFreteComponente in dados.Componentes)
        //    {
        //        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente dadosCalculoFrete = dadoCalculoFreteComponente.Clonar();
        //        decimal valorComponente = serRateioFormula.AplicarFormulaRateio(formulaRateio, dadosCalculoFrete.ValorComponente, 0, 0, pesoCarregamento, pesoCarga, valorCarga, valorCarregamento, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, 0m, 0m, 0m, metrosCubicosCarga, metrosCubicosCarregamento, 0m, false, pesoLiquidoCarga, pesoLiquidoCarregamento);
        //        dadosCalculoFrete.ValorComponente = Math.Round(valorComponente, 2, MidpointRounding.AwayFromZero);

        //        if (ulitmaCarga)
        //        {
        //            decimal valorTotal = dadosCalculosRateiados.Sum(obj => (from com in obj.Componentes where com.ComponenteFrete.Codigo == dadoCalculoFreteComponente.ComponenteFrete.Codigo select com.ValorComponente).Sum());
        //            dadosCalculoFrete.ValorComponente += dadoCalculoFreteComponente.ValorComponente - (valorTotal + dadosCalculoFrete.ValorComponente);
        //        }

        //        dadosRateiado.Componentes.Add(dadosCalculoFrete);
        //    }

        //    dadosCalculosRateiados.Add(dadosRateiado);
        //    return dadosRateiado;
        //}
    }
}
