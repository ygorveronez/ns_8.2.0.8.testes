using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { }, "Pallets/RegraAutorizacaoDevolucaoValePallet")]
    public class RegraAutorizacaoDevolucaoValePalletController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet>
    {
		#region Construtores

		public RegraAutorizacaoDevolucaoValePalletController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet regra)
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
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorDiasDevolucao = regra.RegraPorDiasDevolucao,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasDiasDevolucao = (from alcada in regra.AlcadasDiasDevolucao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaDiasDevolucao, int>(alcada)).ToList()
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

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var regraAutorizacaoDevolucaoValePallet = new Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet();

                PreencherRegra(regraAutorizacaoDevolucaoValePallet, unitOfWork, ((regra) => {
                    regra.RegraPorDiasDevolucao = Request.GetBoolParam("UsarRegraPorDiasDevolucao");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                }));

                repositorioRegra.Inserir(regraAutorizacaoDevolucaoValePallet, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaDiasDevolucao, int>(unitOfWork, regraAutorizacaoDevolucaoValePallet, "AlcadasDiasDevolucao", ((valorPropriedade, alcada) =>
                {
                    int diasDevolucao = Convert.ToInt32(valorPropriedade);

                    alcada.PropriedadeAlcada = diasDevolucao > 0 ? diasDevolucao : throw new ControllerException("Os dias de devolução deve ser maior do que zero.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoDevolucaoValePallet, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
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
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var regraAutorizacaoDevolucaoValePallet = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoDevolucaoValePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoDevolucaoValePallet.Initialize();

                PreencherRegra(regraAutorizacaoDevolucaoValePallet, unitOfWork, ((regra) => {
                    regra.RegraPorDiasDevolucao = Request.GetBoolParam("UsarRegraPorDiasDevolucao");
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaDiasDevolucao, int>(unitOfWork, regraAutorizacaoDevolucaoValePallet, regraAutorizacaoDevolucaoValePallet.AlcadasDiasDevolucao, "AlcadasDiasDevolucao", ((valorPropriedade, alcada) =>
                {
                    int diasDevolucao = Convert.ToInt32(valorPropriedade);

                    alcada.PropriedadeAlcada = diasDevolucao > 0 ? diasDevolucao : throw new ControllerException("Os dias de devolução deve ser maior do que zero.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoDevolucaoValePallet, regraAutorizacaoDevolucaoValePallet.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoDevolucaoValePallet, Auditado);

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