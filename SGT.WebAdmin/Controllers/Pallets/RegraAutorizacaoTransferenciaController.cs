using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { }, "Pallets/RegraAutorizacaoTransferencia")]
    public class RegraAutorizacaoTransferenciaController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet>
    {
		#region Construtores

		public RegraAutorizacaoTransferenciaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet regra)
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
                UsarRegraPorQuantidade = regra.RegraPorQuantidade,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasQuantidade = (from alcada in regra.AlcadasQuantidade select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaQuantidade, int>(alcada)).ToList(),
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

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var regraAutorizacaoTransferenciaPallet = new Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet();

                PreencherRegra(regraAutorizacaoTransferenciaPallet, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorQuantidade = Request.GetBoolParam("UsarRegraPorQuantidade");
                }));

                repositorioRegra.Inserir(regraAutorizacaoTransferenciaPallet, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoTransferenciaPallet, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaQuantidade, int>(unitOfWork, regraAutorizacaoTransferenciaPallet, "AlcadasQuantidade", ((valorPropriedade, alcada) =>
                {
                    int quantidade = Convert.ToInt32(valorPropriedade);

                    alcada.PropriedadeAlcada = quantidade > 0 ? quantidade : throw new ControllerException("A quantidade deve ser maior do que zero.");
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
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var regraAutorizacaoTransferenciaPallet = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoTransferenciaPallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoTransferenciaPallet.Initialize();

                PreencherRegra(regraAutorizacaoTransferenciaPallet, unitOfWork, ((regra) =>
                {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorQuantidade = Request.GetBoolParam("UsarRegraPorQuantidade");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoTransferenciaPallet, regraAutorizacaoTransferenciaPallet.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaQuantidade, int>(unitOfWork, regraAutorizacaoTransferenciaPallet, regraAutorizacaoTransferenciaPallet.AlcadasQuantidade, "AlcadasQuantidade", ((valorPropriedade, alcada) =>
                {
                    int quantidade = Convert.ToInt32(valorPropriedade);

                    alcada.PropriedadeAlcada = quantidade > 0 ? quantidade : throw new ControllerException("A quantidade deve ser maior do que zero.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoTransferenciaPallet, Auditado);

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