using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Cargas
{
	[Area("BusinessIntelligence")]
	[CustomAuthorize("BusinessIntelligence/Cargas/ValorMedioFrete")]
    public class ValorMedioFreteController : BaseController
    {
		#region Construtores

		public ValorMedioFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterValoresGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, codigoModeloVeiculo, codigoTipoCarga;
                int.TryParse(Request.Params("Operador"), out codigoOperador);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("ModeloVeiculo"), out codigoModeloVeiculo);
                int.TryParse(Request.Params("TipoCarga"), out codigoTipoCarga);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                Dominio.ObjetosDeValor.Embarcador.Carga.GraficoValorMedioFrete dados = repCarga.BuscarValorMedioFrete(dataInicial, dataFinal, codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, cpfCnpjDestinatario, codigoModeloVeiculo, codigoTipoCarga);

                List<dynamic> retorno = new List<dynamic>();

                retorno.Add(new { Descricao = "Quilômetro", Valor = dados.ValorTotalFrete > 0m && dados.DistanciaTotal > 0 ? Math.Round((dados.ValorTotalFrete / dados.DistanciaTotal), 5, MidpointRounding.ToEven) : 0m });
                retorno.Add(new { Descricao = "Quilograma", Valor = dados.ValorTotalFrete > 0m && dados.PesoTotal > 0m ? Math.Round((dados.ValorTotalFrete / dados.PesoTotal), 5, MidpointRounding.ToEven) : 0m });

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
