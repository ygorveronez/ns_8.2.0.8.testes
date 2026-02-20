using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "BuscarPorCarga" }, "GestaoPatio/FluxoPatio")]
    public class RastreamentoCargaController : BaseController
    {
		#region Construtores

		public RastreamentoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [Obsolete("Método utilizado somente no fluxo de entrega (DESCONTINUADO)")]
        public async Task<IActionResult> BuscarPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repositorioRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamento = repositorioRastreamentoCarga.BuscarPorCarga(codigoCarga);

                if (rastreamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    rastreamento.Codigo,
                    Carga = rastreamento.Carga.Codigo,
                    rastreamento.Latitude,
                    rastreamento.Longitude,
                    UltimaAtualizacao = rastreamento.UltimaAtualizacao?.ToString("dd/MM/yyyy HH:mm:ss")
                };
                
                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRastreamentoCarga = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repositorioRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamento = null;

                if (codigoRastreamentoCarga > 0)
                    rastreamento = repositorioRastreamentoCarga.BuscarPorCodigo(codigoRastreamentoCarga, auditavel: false);
                else if (codigoFluxoGestaoPatio > 0)
                    rastreamento = repositorioRastreamentoCarga.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (rastreamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    rastreamento.Codigo,
                    Carga = rastreamento.Carga.Codigo,
                    rastreamento.Latitude,
                    rastreamento.Longitude,
                    UltimaAtualizacao = rastreamento.UltimaAtualizacao?.ToString("dd/MM/yyyy HH:mm:ss")
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
