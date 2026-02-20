using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { }, "CTe/RegraAutorizacaoIntegracaoCTe")]
    public class RegraAutorizacaoIntegracaoCTeController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe>
    {
		#region Construtores

		public RegraAutorizacaoIntegracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe regra)
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
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                UsarRegraPorValorFrete = regra.RegraPorValorFrete,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                AlcadasValorFrete = (from alcada in regra.AlcadasValorFrete select ObterRegraPorTipo<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaValorFrete, decimal>(alcada)).ToList(),
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

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe>(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe regraAutorizacao = new Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorValorFrete = Request.GetBoolParam("UsarRegraPorValorFrete");
                }));

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaValorFrete, decimal>(unitOfWork, regraAutorizacao, "AlcadasValorFrete", ((valorPropriedade, alcada) =>
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

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe>(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorValorFrete = Request.GetBoolParam("UsarRegraPorValorFrete");
                }));

                Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.MotivoSolicitacaoFrete repositorioMotivoSolicitacaoFrete = new Repositorio.Embarcador.Cargas.MotivoSolicitacaoFrete(unitOfWork);

                AtualizarAlcadas<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaValorFrete, decimal>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasValorFrete, "AlcadasValorFrete", ((valorPropriedade, alcada) =>
                {
                    decimal valor = Convert.ToDecimal(valorPropriedade);

                    alcada.PropriedadeAlcada = valor > 0 ? valor : throw new ControllerException("O valor deve ser maior do que zero.");
                }));

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