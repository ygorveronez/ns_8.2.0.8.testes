using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/CalculoPisCofins")]
    public class CalculoPisCofinsController : BaseController
    {
		#region Construtores

		public CalculoPisCofinsController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCalculoPisCofins()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Contabeis.CalculoPisCofins repCalculoPisCofins = new Repositorio.Embarcador.Contabeis.CalculoPisCofins(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins calculoPisCofins = repCalculoPisCofins.BuscarConfiguracaoCalculoPisCofins();

                if (calculoPisCofins == null)
                    calculoPisCofins = new Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins();

                var dynConfiguracao = new
                {
                    calculoPisCofins.CSTPISTributavel,
                    calculoPisCofins.CSTPISNaoTributavel,
                    calculoPisCofins.CSTCOFINSTributavel,
                    calculoPisCofins.CSTCOFINSNaoTributavel,
                    AliquotaPIS = calculoPisCofins.AliquotaPIS > 0 ? calculoPisCofins.AliquotaPIS.ToString("n2") : string.Empty,
                    AliquotaCOFINS = calculoPisCofins.AliquotaCOFINS > 0 ? calculoPisCofins.AliquotaCOFINS.ToString("n2") : string.Empty
                };

                return new JsonpResult(dynConfiguracao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Contabeis.CalculoPisCofins repCalculoPisCofins = new Repositorio.Embarcador.Contabeis.CalculoPisCofins(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins calculoPisCofins = repCalculoPisCofins.BuscarConfiguracaoCalculoPisCofins();

                bool inserir = false;
                if (calculoPisCofins == null)
                {
                    calculoPisCofins = new Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins();
                    inserir = true;
                }
                else
                    calculoPisCofins.Initialize();

                calculoPisCofins.CSTPISTributavel = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS>("CSTPISTributavel");
                calculoPisCofins.CSTPISNaoTributavel = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS>("CSTPISNaoTributavel");
                calculoPisCofins.CSTCOFINSTributavel = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS>("CSTCOFINSTributavel");
                calculoPisCofins.CSTCOFINSNaoTributavel = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS>("CSTCOFINSNaoTributavel");

                calculoPisCofins.AliquotaPIS = Request.GetDecimalParam("AliquotaPIS");
                calculoPisCofins.AliquotaCOFINS = Request.GetDecimalParam("AliquotaCOFINS");

                if (inserir)
                    repCalculoPisCofins.Inserir(calculoPisCofins, Auditado);
                else
                    repCalculoPisCofins.Atualizar(calculoPisCofins, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Salvar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
