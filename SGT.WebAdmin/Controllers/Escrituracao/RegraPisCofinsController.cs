using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/RegraPisCofins")]
    public class RegraPisCofinsController : BaseController
    {
		#region Construtores

		public RegraPisCofinsController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarRegra()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

                // Valida
                if (empresaPai == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    Pis = empresaPai.Configuracao?.AliquotaPIS?.ToString("n4") ?? "",
                    Cofins = empresaPai.Configuracao?.AliquotaCOFINS?.ToString("n4") ?? ""
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarRegra()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);


                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

                // Valida
                if (empresaPai == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                decimal pis = Request.GetDecimalParam("Pis");
                decimal cofins = Request.GetDecimalParam("Cofins");

                string aliquotaAnterior = (empresaPai.Configuracao.AliquotaPIS?.ToString("n4") ?? "") + "/" + (empresaPai.Configuracao.AliquotaCOFINS?.ToString("n4") ?? "");

                empresaPai.Configuracao.AliquotaPIS = pis;
                empresaPai.Configuracao.AliquotaCOFINS = cofins;

                string novaaliquota = (empresaPai.Configuracao.AliquotaPIS?.ToString("n4") ?? "") + "/" + (empresaPai.Configuracao.AliquotaCOFINS?.ToString("n4") ?? "");
                Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: 0, nomeEntidade: "RegraPisCofins", descricaoEntidade: "", descricaoAcao: $"Atualizou o Pis/Confins de ({aliquotaAnterior}) para {novaaliquota}", unitOfWork);

                repEmpresa.Atualizar(empresaPai);

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
