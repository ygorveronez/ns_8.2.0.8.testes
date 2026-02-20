using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/MotivoRetificacaoColeta")]
    public class MotivoRetificacaoColetaController : BaseController
    {
		#region Construtores

		public MotivoRetificacaoColetaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

          [AllowAuthenticate]
        public async Task<IActionResult> BuscarMotivosRetificacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega>("TipoAplicacaoColetaEntrega");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> motivosRetificacao = repMotivoRetificacaoColeta.BuscarMotivosRetificacao(tipoAplicacaoColetaEntrega);

                dynamic retorno = (from obj in motivosRetificacao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Descricao
                                   }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os tipos de ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta motivoRetificacao = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta();

                PreencherEntidade(motivoRetificacao, unitOfWork);

                unitOfWork.Start();

                repMotivoRetificacaoColeta.Inserir(motivoRetificacao, Auditado);

                unitOfWork.CommitChanges();


                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta motivoRetificacao = repMotivoRetificacaoColeta.BuscarPorCodigo(codigo, true);

                if (motivoRetificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(motivoRetificacao, unitOfWork);

                unitOfWork.Start();

                repMotivoRetificacaoColeta.Atualizar(motivoRetificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta motivoRetificacao = repMotivoRetificacaoColeta.BuscarPorCodigo(codigo, false);

                if (motivoRetificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoRetificacao.Codigo,
                    motivoRetificacao.Descricao,
                    motivoRetificacao.Observacao,
                    motivoRetificacao.TipoAplicacaoColetaEntrega,
                    ReabrirEntregaZerarData = motivoRetificacao.ReabrirEntregaZerarDataFim,
                    TipoOperacao = new { Codigo = motivoRetificacao?.TipoOperacao?.Codigo ?? 0, Descricao = motivoRetificacao?.TipoOperacao?.Descricao ?? "" },
                    Situacao = motivoRetificacao.Ativo,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta motivoRetificacao = repMotivoRetificacaoColeta.BuscarPorCodigo(codigo, true);

                if (motivoRetificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();


                repMotivoRetificacaoColeta.Deletar(motivoRetificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta motivoRetificacao, Repositorio.UnitOfWork unitOfWork)
        {
            motivoRetificacao.Ativo = Request.GetBoolParam("Situacao");
            motivoRetificacao.Descricao = Request.GetStringParam("Descricao");
            motivoRetificacao.TipoAplicacaoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega>("TipoAplicacaoColetaEntrega");
            motivoRetificacao.Observacao = Request.GetStringParam("Observacao");
            motivoRetificacao.ReabrirEntregaZerarDataFim = Request.GetBoolParam("ReabrirEntregaZerarData");

            int paramTipoOperacao = Request.GetIntParam("TipoOperacao");
            if (paramTipoOperacao > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(paramTipoOperacao);
                motivoRetificacao.TipoOperacao = tipoOperacao;
            }

        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Aplicação", "TipoAplicacaoColetaEntrega", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de operação", "TipoOperacao", 20, Models.Grid.Align.left, false);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> listaMotivoRetificacaoColeta = repMotivoRetificacaoColeta.Consultar(descricao, status, codigoTipoOperacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repMotivoRetificacaoColeta.ContarConsulta(descricao, status, codigoTipoOperacao);

                var retorno = listaMotivoRetificacaoColeta.Select(motivo => new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    TipoAplicacaoColetaEntrega = motivo.TipoAplicacaoColetaEntrega.ObterDescricao(),
                    TipoOperacao = motivo.TipoOperacao?.Descricao ?? "",
                    motivo.DescricaoAtivo
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
