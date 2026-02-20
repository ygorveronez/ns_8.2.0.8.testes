using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Cargas
{
	[Area("BusinessIntelligence")]
	[CustomAuthorize("BusinessIntelligence/Cargas/IndiceAtraso")]
    public class IndiceAtrasoController : BaseController
    {
		#region Construtores

		public IndiceAtrasoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterValoresGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, codigoModeloVeiculo, codigoTipoCarga, codigoRota;
                int.TryParse(Request.Params("Operador"), out codigoOperador);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("ModeloVeiculo"), out codigoModeloVeiculo);
                int.TryParse(Request.Params("TipoCarga"), out codigoTipoCarga);
                int.TryParse(Request.Params("Rota"), out codigoRota);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                int countDentroPrazo = repCarga.BuscarIndiceAtraso(true, dataInicial, dataFinal, codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, cpfCnpjDestinatario, codigoModeloVeiculo, codigoTipoCarga, codigoRota);
                int countForaPrazo = repCarga.BuscarIndiceAtraso(false, dataInicial, dataFinal, codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, cpfCnpjDestinatario, codigoModeloVeiculo, codigoTipoCarga, codigoRota);

                List<dynamic> retorno = new List<dynamic>();

                decimal percentualCarregadoDentroPrazo = 0m, percentualCarregadoForaPrazo = 0m;

                if (countDentroPrazo > 0m)
                    percentualCarregadoDentroPrazo = ((decimal)(countDentroPrazo * 100)) / ((decimal)(countDentroPrazo + countForaPrazo));

                if (countForaPrazo > 0m)
                    percentualCarregadoForaPrazo = ((decimal)(countForaPrazo * 100)) / ((decimal)(countDentroPrazo + countForaPrazo));

                retorno.Add(new { Descricao = "Dentro do Prazo", Valor = countDentroPrazo, DescricaoValor = countDentroPrazo.ToString("n0") + " (" + percentualCarregadoDentroPrazo.ToString("n2") + "%)", ForaPrazo = false, Cor = "#44925e" });
                retorno.Add(new { Descricao = "Fora do Prazo", Valor = countForaPrazo, DescricaoValor = countForaPrazo.ToString("n0") + " (" + percentualCarregadoForaPrazo.ToString("n2") + "%)", ForaPrazo = true, Cor = "#b94747" });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao obter os valores.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterValoresForaPrazo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, codigoModeloVeiculo, codigoTipoCarga, codigoRota;
                int.TryParse(Request.Params("Operador"), out codigoOperador);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("ModeloVeiculo"), out codigoModeloVeiculo);
                int.TryParse(Request.Params("TipoCarga"), out codigoTipoCarga);
                int.TryParse(Request.Params("Rota"), out codigoRota);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                int countAteUmDiaAtraso = repCarga.BuscarIndiceAtrasoForaPrazo(false, dataInicial, dataFinal, codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, cpfCnpjDestinatario, codigoModeloVeiculo, codigoTipoCarga, codigoRota);
                int countMaisQueUmDiaAtraso = repCarga.BuscarIndiceAtrasoForaPrazo(true, dataInicial, dataFinal, codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, cpfCnpjDestinatario, codigoModeloVeiculo, codigoTipoCarga, codigoRota);

                List<dynamic> retorno = new List<dynamic>();

                decimal percentualAteUmDiaAtraso = 0m, percentualMaisQueUmDiaAtraso = 0m;

                if (countAteUmDiaAtraso > 0m)
                    percentualAteUmDiaAtraso = ((decimal)(countAteUmDiaAtraso * 100)) / ((decimal)(countAteUmDiaAtraso + countMaisQueUmDiaAtraso));

                if (countMaisQueUmDiaAtraso > 0m)
                    percentualMaisQueUmDiaAtraso = ((decimal)(countMaisQueUmDiaAtraso * 100)) / ((decimal)(countAteUmDiaAtraso + countMaisQueUmDiaAtraso));

                retorno.Add(new { Descricao = "Até um dia de atraso", Valor = countAteUmDiaAtraso, DescricaoValor = countAteUmDiaAtraso.ToString("n0") + " (" + percentualAteUmDiaAtraso.ToString("n2") + "%)", Cor = "#d6a48a" });
                retorno.Add(new { Descricao = "Mais que um dia de atraso", Valor = countMaisQueUmDiaAtraso, DescricaoValor = countMaisQueUmDiaAtraso.ToString("n0") + " (" + percentualMaisQueUmDiaAtraso.ToString("n2") + "%)", Cor = "#b94747" });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao obter os valores.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
