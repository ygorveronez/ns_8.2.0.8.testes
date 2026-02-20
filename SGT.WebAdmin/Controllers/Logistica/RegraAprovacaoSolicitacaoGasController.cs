using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { }, "Logistica/RegraAprovacaoSolicitacaoGas")]
    public class RegraAprovacaoSolicitacaoGasController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas>
    {
		#region Construtores

		public RegraAprovacaoSolicitacaoGasController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas regra)
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
                UsarRegraPorSolicitacaoGasData = regra.RegraPorTempoExcedido,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasSolicitacaoGasData = (from alcada in regra.AlcadasSolicitacaoGasData select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AlcadaSolicitacaoGasData, int>(alcada))
                                            .ToList()
            });
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas>(unitOfWork);
                var regraAutorizacaoSolicitacaoGas = new Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas();

                PreencherRegra(regraAutorizacaoSolicitacaoGas, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTempoExcedido = Request.GetBoolParam("UsarRegraPorSolicitacaoGasData");
                }));

                repositorioRegra.Inserir(regraAutorizacaoSolicitacaoGas, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AlcadaSolicitacaoGasData, int>(unitOfWork, regraAutorizacaoSolicitacaoGas, "AlcadasSolicitacaoGasData", ((valorPropriedade, alcada) =>
                {
                    int minutos = Convert.ToInt32(valorPropriedade);
                    
                    alcada.PropriedadeAlcada = minutos;
                }));

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

                var codigo = Request.GetIntParam("Codigo");
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas>(unitOfWork);
                var regraAutorizacaoSolicitacaoGas = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoSolicitacaoGas == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoSolicitacaoGas.Initialize();

                PreencherRegra(regraAutorizacaoSolicitacaoGas, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTempoExcedido = Request.GetBoolParam("UsarRegraPorSolicitacaoGasData");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AlcadaSolicitacaoGasData, int>(unitOfWork, regraAutorizacaoSolicitacaoGas, regraAutorizacaoSolicitacaoGas.AlcadasSolicitacaoGasData, "AlcadasSolicitacaoGasData", ((valorPropriedade, alcada) =>
                {
                    int minutos = Convert.ToInt32(valorPropriedade);
                    
                    alcada.PropriedadeAlcada = minutos;
                }));

                repositorioRegra.Atualizar(regraAutorizacaoSolicitacaoGas, Auditado);

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