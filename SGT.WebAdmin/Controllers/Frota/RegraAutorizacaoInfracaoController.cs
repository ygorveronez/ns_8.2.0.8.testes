using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize(new string[] { }, "Frota/RegraAutorizacaoInfracao")]
    public class RegraAutorizacaoInfracaoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao>
    {
		#region Construtores

		public RegraAutorizacaoInfracaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao regra)
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
                UsarRegraPorTipoInfracao = regra.RegraPorTipoInfracao,
                UsarRegraPorValor = regra.RegraPorValor,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasTipoInfracao = (from alcada in regra.AlcadasTipoInfracao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaTipoInfracao, Dominio.Entidades.Embarcador.Frota.TipoInfracao>(alcada)).ToList(),
                AlcadasValor = (from alcada in regra.AlcadasValor select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaValor, decimal>(alcada)).ToList(),
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

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao>(unitOfWork);
                var repositorioFrota = new Repositorio.Embarcador.Frota.TipoInfracao(unitOfWork);
                var regraAutorizacaoInfracao = new Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao();

                PreencherRegra(regraAutorizacaoInfracao, unitOfWork, ((regra) => {
                    regra.RegraPorTipoInfracao = Request.GetBoolParam("UsarRegraPorTipoInfracao");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                }));

                repositorioRegra.Inserir(regraAutorizacaoInfracao, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaTipoInfracao, Dominio.Entidades.Embarcador.Frota.TipoInfracao>(unitOfWork, regraAutorizacaoInfracao, "AlcadasTipoInfracao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao = repositorioFrota.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoInfracao ?? throw new ControllerException("Tipo da infração não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoInfracao, "AlcadasValor", ((valorPropriedade, alcada) =>
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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao>(unitOfWork);
                var repositorioFrota = new Repositorio.Embarcador.Frota.TipoInfracao(unitOfWork);
                var regraAutorizacaoInfracao = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoInfracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoInfracao, unitOfWork, ((regra) => {
                    regra.RegraPorTipoInfracao = Request.GetBoolParam("UsarRegraPorTipoInfracao");
                    regra.RegraPorValor = Request.GetBoolParam("UsarRegraPorValor");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaTipoInfracao, Dominio.Entidades.Embarcador.Frota.TipoInfracao>(unitOfWork, regraAutorizacaoInfracao, regraAutorizacaoInfracao.AlcadasTipoInfracao, "AlcadasTipoInfracao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao = repositorioFrota.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoInfracao ?? throw new ControllerException("Tipo da infração não encontrado.");
                }));
                

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaValor, decimal>(unitOfWork, regraAutorizacaoInfracao, regraAutorizacaoInfracao.AlcadasValor, "AlcadasValor", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoInfracao, Auditado);

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

        #endregion
    }
}