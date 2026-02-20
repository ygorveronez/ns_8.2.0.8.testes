using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PagamentoMotorista
{
    public class PagamentosMotoristasController : BaseController
    {
		#region Construtores

		public PagamentosMotoristasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("PagamentosMotoristas/PagamentoMotoristaTipo")]
        public async Task<IActionResult> PagamentoMotoristaTipo()
        {
            return View();
        }

        [CustomAuthorize("PagamentosMotoristas/RegrasPagamentoMotorista")]
        public async Task<IActionResult> RegrasPagamentoMotorista()
        {
            return View();
        }

        [CustomAuthorize("PagamentosMotoristas/PagamentoMotoristaTMS")]
        public async Task<IActionResult> PagamentoMotoristaTMS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = null;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = null;
            string configuracaoPaginacaoDataLimite = string.Empty;
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces repositorioConfiguracaoPaginacaoInterfaces = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces(unitOfWork);

                configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
                configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
                configuracaoPaginacaoDataLimite = repositorioConfiguracaoPaginacaoInterfaces.BuscarPorDataLimiteInterface(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ConfiguracaoPaginacaoInterfaces.PagamentosMotoristas_PagamentoMotoristaTMS);

                var dynConfiguracaoFinanceiro = new {

                    configuracaoFinanceiro.UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista
                };
                ViewBag.ConfiguracaoFinanceiro = Newtonsoft.Json.JsonConvert.SerializeObject(dynConfiguracaoFinanceiro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            ViewBag.ConfiguracaoGeral = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoGeral);
            ViewBag.ConfiguracaoPaginacaoDataLimite = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoPaginacaoDataLimite);

            return View();
        }

        [CustomAuthorize("PagamentosMotoristas/AutorizacaoPagamentoMotorista")]
        public async Task<IActionResult> AutorizacaoPagamentoMotorista()
        {
            return View();
        }

        [CustomAuthorize("PagamentosMotoristas/PagamentoMotoristaTMSLote")]
        public async Task<IActionResult> PagamentoMotoristaTMSLote()
        {
            return View();
        }

        [CustomAuthorize("PagamentosMotoristas/PendenciaMotorista")]
        public async Task<IActionResult> PendenciaMotorista()
        {
            return View();
        }
    }

}
