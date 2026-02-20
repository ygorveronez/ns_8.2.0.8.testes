using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/Sinistro")]
    public class SinistroHistoricoController : BaseController
    {
		#region Construtores

		public SinistroHistoricoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeTrabalho.Start();

                Repositorio.Embarcador.Frota.SinistroHistorico repositorioSinistroHistorico = new Repositorio.Embarcador.Frota.SinistroHistorico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.SinistroHistorico historico = new Dominio.Entidades.Embarcador.Frota.SinistroHistorico();

                PreencherEntidade(historico, unidadeTrabalho);

                if (historico.Sinistro.Situacao != SituacaoEtapaFluxoSinistro.Aberto)
                    return new JsonpResult(false, true, "Não é possível adicionar históricos em fluxos finalizados.");

                repositorioSinistroHistorico.Inserir(historico);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, historico, "Adicionou histórico", unidadeTrabalho);

                if (historico.Tipo == TipoHistoricoInfracao.Finalizado)
                    FinalizarFluxoSinistro(historico.Sinistro, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new
                {
                    Codigo = historico.Codigo,
                    Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                    historico.Observacao,
                    historico.Tipo,
                    TipoDescricao = historico.Tipo.ObterDescricao(),
                    FluxoFinalizado = historico.Sinistro.Situacao == SituacaoEtapaFluxoSinistro.Finalizado
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar o histórico.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.SinistroHistorico repositorioSinistroHistorico = new Repositorio.Embarcador.Frota.SinistroHistorico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.SinistroHistorico historico = repositorioSinistroHistorico.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (historico == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                unidadeTrabalho.Start();

                PreencherEntidade(historico, unidadeTrabalho);

                repositorioSinistroHistorico.Atualizar(historico);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, historico, "Atualizou histórico", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new
                {
                    Codigo = historico.Codigo,
                    Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                    historico.Observacao,
                    historico.Tipo,
                    TipoDescricao = historico.Tipo.ObterDescricao()
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao atualizar o histórico.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeTrabalho.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.SinistroHistorico repositorioSinistroHistorico = new Repositorio.Embarcador.Frota.SinistroHistorico(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.SinistroHistorico sinitroHistorico = repositorioSinistroHistorico.BuscarPorCodigo(codigo, true);

                if (sinitroHistorico == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo, Dominio.Entidades.Embarcador.Frota.SinistroHistorico> repositorioAnexos = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo, Dominio.Entidades.Embarcador.Frota.SinistroHistorico>(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo> anexos = repositorioAnexos.BuscarPorEntidade(codigo);

                foreach (Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo anexo in anexos)
                    repositorioAnexos.Deletar(anexo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, sinitroHistorico, "Deletou o histórico", unidadeTrabalho);

                repositorioSinistroHistorico.Deletar(sinitroHistorico);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true, true, "Registro excluido com sucesso");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao remover o histórico.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void FinalizarFluxoSinistro(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(unidadeTrabalho);

            sinistro.Situacao = SituacaoEtapaFluxoSinistro.Finalizado;

            repositorioSinistro.Atualizar(sinistro);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, sinistro, "Finalizou o fluxo de sinistro", unidadeTrabalho);
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Frota.SinistroHistorico historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);

            historico.Data = Request.GetDateTimeParam("Data");
            historico.Tipo = Request.GetEnumParam<TipoHistoricoInfracao>("Tipo");
            historico.Observacao = Request.GetStringParam("Observacao");
            historico.Sinistro = repositorioSinistro.BuscarPorCodigo(Request.GetIntParam("Sinistro"));
        }

        #endregion
    }
}
