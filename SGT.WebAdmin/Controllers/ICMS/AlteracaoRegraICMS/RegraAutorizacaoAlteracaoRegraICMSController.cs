using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ICMS.AlteracaoRegraICMS
{
    [CustomAuthorize(new string[] { }, "ICMS/RegraAutorizacaoAlteracaoRegraICMS")]
    public class RegraAutorizacaoAlteracaoRegraICMSController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS>
    {
		#region Construtores

		public RegraAutorizacaoAlteracaoRegraICMSController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS regra)
        {
            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList()
            });
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS>(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS regraAutorizacao = new Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => { }));

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public override IActionResult Atualizar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS>(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => { }));

                repositorioRegra.Atualizar(regraAutorizacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}