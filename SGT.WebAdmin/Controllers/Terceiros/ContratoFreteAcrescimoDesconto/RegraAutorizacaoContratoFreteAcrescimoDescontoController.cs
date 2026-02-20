using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros.ContratoFreteAcrescimoDesconto
{
    [CustomAuthorize("Terceiros/RegraAutorizacaoContratoFreteAcrescimoDesconto")]
    public class RegraAutorizacaoContratoFreteAcrescimoDescontoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto>
    {
		#region Construtores

		public RegraAutorizacaoContratoFreteAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto regra)
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
                UsarRegraPorJustificativa = regra.RegraPorJustificativa,
                UsarRegraPorValor = regra.RegraPorValor,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasJustificativa = (from alcada in regra.AlcadasJustificativa select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaJustificativa, Dominio.Entidades.Embarcador.Fatura.Justificativa>(alcada)).ToList(),
                AlcadasValor = (from alcada in regra.AlcadasValor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaValor, decimal>(alcada)).ToList()
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

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto>(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repositorioJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto regraAutorizacaoContratoFreteAcrescimoDesconto = new Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto();

                PreencherRegra(regraAutorizacaoContratoFreteAcrescimoDesconto, unitOfWork, ((regra) =>
                {
                    regra.RegraPorJustificativa = Request.GetBoolParam("UsarRegraPorJustificativa");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                }));

                repositorioRegra.Inserir(regraAutorizacaoContratoFreteAcrescimoDesconto, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaJustificativa, Dominio.Entidades.Embarcador.Fatura.Justificativa>(unitOfWork, regraAutorizacaoContratoFreteAcrescimoDesconto, "AlcadasJustificativa", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repositorioJustificativa.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = justificativa ?? throw new ControllerException("Justificativa não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoContratoFreteAcrescimoDesconto, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto>(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repositorioJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto regraAutorizacaoContratoFreteAcrescimoDesconto = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoContratoFreteAcrescimoDesconto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoContratoFreteAcrescimoDesconto, unitOfWork, ((regra) =>
                {
                    regra.RegraPorJustificativa = Request.GetBoolParam("UsarRegraPorJustificativa");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaJustificativa, Dominio.Entidades.Embarcador.Fatura.Justificativa>(unitOfWork, regraAutorizacaoContratoFreteAcrescimoDesconto, regraAutorizacaoContratoFreteAcrescimoDesconto.AlcadasJustificativa, "AlcadasJustificativa", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repositorioJustificativa.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = justificativa ?? throw new ControllerException("Justificativa não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoContratoFreteAcrescimoDesconto, regraAutorizacaoContratoFreteAcrescimoDesconto.AlcadasValor, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoContratoFreteAcrescimoDesconto, Auditado);

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