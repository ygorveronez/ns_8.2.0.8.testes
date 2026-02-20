using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Cargas
{
	[Area("BusinessIntelligence")]
	[CustomAuthorize("BusinessIntelligence/Cargas/QuantidadePorRota")]
    public class QuantidadePorRotaController : BaseController
    {
		#region Construtores

		public QuantidadePorRotaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterQuantidadesGeraisPorRota(bool exportacao = false)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRota, codigoCentroCarregamento, codigoTransportador, codigoVeiculo, codigoFilial;
                int.TryParse(Request.Params("Rota"), out codigoRota);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Filial"), out codigoFilial);

                DateTime dataInicialCarregamento, dataFinalCarregamento;
                DateTime.TryParseExact(Request.Params("DataInicialCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCarregamento);
                DateTime.TryParseExact(Request.Params("DataFinalCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCarregamento);


                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoQuantodadeCargaPorRota> quantidades = repCarga.BuscarQuantidadesPorRota(codigoTransportador, codigoVeiculo, codigoFilial, codigoCentroCarregamento, codigoRota, dataInicialCarregamento, dataFinalCarregamento);

                if (exportacao)
                {
                    quantidades = quantidades.Where(o => o.Quantidade > 0).OrderBy(o => o.Quantidade).ToList();

                    byte[] byteArrayObjeto = Utilidades.CSV.GerarCSV(quantidades); ;

                    if (byteArrayObjeto != null)
                        return Arquivo(byteArrayObjeto, "application/csv", "Quantidade por Rota.csv");
                    else
                        return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
                }
                else
                {
                    quantidades = quantidades.Where(o => o.Quantidade > 0).ToList();

                    return new JsonpResult(quantidades);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as quantidades de cargas por rota.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarRelatorioExcel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
				return ObterQuantidadesGeraisPorRota(true).Result;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
