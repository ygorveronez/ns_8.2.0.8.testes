using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Biddings
{
    [CustomAuthorize(new string[] { }, "Bidding/RegrasAutorizacaoBidding")]
    public class RegraAutorizacaoBiddingController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding>
    {
		#region Construtores

		public RegraAutorizacaoBiddingController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding regra)
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
                UsarRegraPorTipoBidding = regra.RegraPorTipoBidding,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasTipoBidding = (from alcada in regra.AlcadasTipoBidding select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AlcadaTipoBidding, Dominio.Entidades.Embarcador.Bidding.TipoBidding>(alcada)).ToList()
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding>(unitOfWork);
                Repositorio.Embarcador.Bidding.TipoBidding repositorioTipoBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork);

                Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding regraAutorizacaoBidding = new Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding();

                PreencherRegra(regraAutorizacaoBidding, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTipoBidding = Request.GetBoolParam("UsarRegraPorTipoBidding");
                }));

                repositorioRegra.Inserir(regraAutorizacaoBidding, Auditado);



                AdicionarAlcadas<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AlcadaTipoBidding, Dominio.Entidades.Embarcador.Bidding.TipoBidding>(unitOfWork, regraAutorizacaoBidding, "AlcadasTipoBidding", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Bidding.TipoBidding tipoBidding = repositorioTipoBidding.BuscarPorCodigo((int)valorPropriedade, false);

                    alcada.PropriedadeAlcada = tipoBidding ?? throw new ControllerException("Tipo de Bidding não encontrado.");
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding>(unitOfWork);

                Repositorio.Embarcador.Bidding.TipoBidding repositorioTipoBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork);


                Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding regraAutorizacaoBidding = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoBidding == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoBidding, unitOfWork, ((regra) =>
                {
                    regra.RegraPorTipoBidding = Request.GetBoolParam("UsarRegraPorTipoBidding");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AlcadaTipoBidding, Dominio.Entidades.Embarcador.Bidding.TipoBidding>(unitOfWork, regraAutorizacaoBidding, regraAutorizacaoBidding.AlcadasTipoBidding, "AlcadasTipoBidding", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Bidding.TipoBidding tipoBidding = repositorioTipoBidding.BuscarPorCodigo((int)valorPropriedade, false);

                    alcada.PropriedadeAlcada = tipoBidding ?? throw new ControllerException("Tipo de Bidding não encontrado.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoBidding, Auditado);

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