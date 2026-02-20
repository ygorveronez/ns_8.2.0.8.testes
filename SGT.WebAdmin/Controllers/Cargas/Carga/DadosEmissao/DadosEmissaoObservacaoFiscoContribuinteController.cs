using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class DadosEmissaoObservacaoFiscoContribuinteController : BaseController
    {
		#region Construtores

		public DadosEmissaoObservacaoFiscoContribuinteController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AdicionarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Não foi possível encontrar a carga.");

                if (carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                    throw new ControllerException("Não é possível adicionar observações na atual situação da carga.");

                string descricao = Request.GetStringParam("Descricao");
                string identificador = Request.GetStringParam("Identificador");
                TipoObservacaoCTe tipo = Request.GetEnumParam<TipoObservacaoCTe>("TipoObservacao");

                Repositorio.Embarcador.CTe.ObservacaoContribuinte repositorioObservacao = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);

                Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte observacao = new Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte()
                {
                    Ativo = true,
                    Carga = carga,
                    Texto = descricao,
                    Identificador = identificador,
                    Tipo = tipo
                };

                repositorioObservacao.Inserir(observacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    observacao.Codigo
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a observação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                string descricao = Request.GetStringParam("Descricao");
                string identificador = Request.GetStringParam("Identificador");
                TipoObservacaoCTe tipo = Request.GetEnumParam<TipoObservacaoCTe>("TipoObservacao");

                Repositorio.Embarcador.CTe.ObservacaoContribuinte repositorioObservacao = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte observacao = repositorioObservacao.BuscarPorCodigo(codigo);

                if (observacao == null)
                    throw new ControllerException("Observação não encontrada.");

                if (observacao.Carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                    throw new ControllerException("Não é possível atualizar a observação na atual situação da carga.");

                observacao.Ativo = true;
                observacao.Texto = descricao;
                observacao.Identificador = identificador;
                observacao.Tipo = tipo;

                repositorioObservacao.Atualizar(observacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a observação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
