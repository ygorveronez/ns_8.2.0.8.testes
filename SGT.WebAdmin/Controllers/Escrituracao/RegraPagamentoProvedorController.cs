using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/RegraPagamentoProvedor")]
    public class RegraPagamentoProvedorController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor>
    {
		#region Construtores

		public RegraPagamentoProvedorController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor regra)
        {
            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.ValidarTodosCamposAuditoriaDocumentoProvedor,
                regra.BloquearPagamentoMultiplosCTe,
                regra.PrioridadeAprovacao,
                UsarRegraPorDiferencaValor = regra.RegraPorDiferencaValor,
                UsarRegraPorDiferencaValorMenor = regra.RegraPorDiferencaValorMenor,
                UsarRegraPorDiferencaValorMaior = regra.RegraPorDiferencaValorMaior,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasDiferencaValor = (from alcada in regra.AlcadasDiferencaValor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValor, decimal>(alcada)).ToList(),
                AlcadasDiferencaValorMenor = (from alcada in regra.AlcadasDiferencaValorMenor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMenor, decimal>(alcada)).ToList(),
                AlcadasDiferencaValorMaior = (from alcada in regra.AlcadasDiferencaValorMaior select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMaior, decimal>(alcada)).ToList(),
            });
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor>(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor regraAutorizacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorDiferencaValor = Request.GetBoolParam("UsarRegraPorDiferencaValor");
                    regra.RegraPorDiferencaValorMenor = Request.GetBoolParam("UsarRegraPorDiferencaValorMenor");
                    regra.RegraPorDiferencaValorMaior = Request.GetBoolParam("UsarRegraPorDiferencaValorMaior");

                    regra.ValidarTodosCamposAuditoriaDocumentoProvedor = Request.GetBoolParam("ValidarTodosCamposAuditoriaDocumentoProvedor");
                    regra.BloquearPagamentoMultiplosCTe = Request.GetBoolParam("BloquearPagamentoMultiplosCTe");
                }));

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValor, decimal>(unitOfWork, regraAutorizacao, "AlcadasDiferencaValor", (valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor;
                });

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMenor, decimal>(unitOfWork, regraAutorizacao, "AlcadasDiferencaValorMenor", (valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor;
                });

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMaior, decimal>(unitOfWork, regraAutorizacao, "AlcadasDiferencaValorMaior", (valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor;
                });

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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor>(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) =>
                {
                    regra.RegraPorDiferencaValor = Request.GetBoolParam("UsarRegraPorDiferencaValor");
                    regra.RegraPorDiferencaValorMenor = Request.GetBoolParam("UsarRegraPorDiferencaValorMenor");
                    regra.RegraPorDiferencaValorMaior = Request.GetBoolParam("UsarRegraPorDiferencaValorMaior");

                    regra.ValidarTodosCamposAuditoriaDocumentoProvedor = Request.GetBoolParam("ValidarTodosCamposAuditoriaDocumentoProvedor");
                    regra.BloquearPagamentoMultiplosCTe = Request.GetBoolParam("BloquearPagamentoMultiplosCTe");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValor, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasDiferencaValor, "AlcadasDiferencaValor", (valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);
                    
                    alcada.PropriedadeAlcada = valor;
                });

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMenor, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasDiferencaValorMenor, "AlcadasDiferencaValorMenor", (valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor;
                });

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMaior, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasDiferencaValorMaior, "AlcadasDiferencaValorMaior", (valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor;
                });

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

        #endregion Métodos Públicos Sobrescritos
    }
}