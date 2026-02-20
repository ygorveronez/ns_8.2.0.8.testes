using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Cargas
{
	[Area("BusinessIntelligence")]
	[CustomAuthorize("BusinessIntelligence/Cargas/DirecionamentoOperador")]
    public class DirecionamentoOperadorController : BaseController
    {
		#region Construtores

		public DirecionamentoOperadorController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterQuantidadesGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoVeiculo, codigoFilial;
                int.TryParse(Request.Params("Operador"), out codigoOperador);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoDirecionamentoOperador> quantidades = repCargaJanelaCarregamento.BuscarDirecionamenosPorOperadorGeral(codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoVeiculo, codigoFilial, cpfCnpjDestinatario, dataInicial, dataFinal);

                var retorno = (from obj in quantidades
                               select new
                               {
                                   obj.CNPJ,
                                   obj.CNPJFormatado,
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.Nome,
                                   obj.Quantidade,
                                   obj.QuantidadeDirecionada,
                                   obj.QuantidadeRejeitada
                               }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os direcionamentos gerais.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterQuantidadesPorTransportador()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoFilial, codigoVeiculo;
                int.TryParse(Request.Params("Operador"), out codigoOperador);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoDirecionamentoOperador> quantidades = repCargaJanelaCarregamento.BuscarDirecionamenosPorOperadorTransportador(codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoVeiculo, codigoFilial, cpfCnpjDestinatario, dataInicial, dataFinal);

                return new JsonpResult(quantidades);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os direcionamentos gerais.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
