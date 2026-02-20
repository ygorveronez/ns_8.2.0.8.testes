using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Cargas
{
	[Area("BusinessIntelligence")]
	[CustomAuthorize("BusinessIntelligence/Cargas/FaturamentoTransportador")]
    public class FaturamentoTransportadorController : BaseController
    {
		#region Construtores

		public FaturamentoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterValoresGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroCarregamento, codigoTransportador, codigoFilial, codigoModeloVeiculo, codigoTipoCarga, codigoDestino, codigoRota;
                int.TryParse(Request.Params("Destino"), out codigoDestino);
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

                List<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoFaturamentoTransportador> dados = repCarga.BuscarFaturamentoTransportador(dataInicial, dataFinal, codigoTransportador, codigoFilial, codigoCentroCarregamento, codigoRota, codigoDestino, codigoTipoCarga, codigoModeloVeiculo, cpfCnpjDestinatario);

                dynamic retorno = (from obj in dados
                                   select new
                                   {
                                       Descricao = "(" + obj.CNPJ_Formatado + ") " + Utilidades.String.Left(obj.Transportador, 22),
                                       Valor = obj.ValorFrete,
                                       DescricaoValor = obj.ValorFrete.ToString("n2") + " (" + obj.QuantidadeCargas.ToString() + (obj.QuantidadeCargas > 1 ? " cargas)" : " carga)")
                                   }).ToList();

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
