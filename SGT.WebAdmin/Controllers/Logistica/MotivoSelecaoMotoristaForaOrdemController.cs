using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MotivoSelecaoMotoristaForaOrdem")]
    public class MotivoSelecaoMotoristaForaOrdemController : BaseController
    {
		#region Construtores

		public MotivoSelecaoMotoristaForaOrdemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivo = new Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem();

                try
                {
                    PreencherMotivoSelecaoMotoristaForaOrdem(motivo);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem repositorio = new Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem(unitOfWork);

                repositorio.Inserir(motivo, Auditado);

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
                Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem repositorio = new Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivo = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherMotivoSelecaoMotoristaForaOrdem(motivo);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(motivo, Auditado);

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
                Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem repositorio = new Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivo = repositorio.BuscarPorCodigo(codigo);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    Status = motivo.Ativo,
                    motivo.Observacao
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
                Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem repositorio = new Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivo = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(motivo, Auditado);

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

        private void PreencherMotivoSelecaoMotoristaForaOrdem(Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivo)
        {
            var descricao = Request.Params("Descricao");
            var observacao = Request.Params("Observacao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new Exception("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new Exception("Descrição não pode passar de 200 caracteres.");

            if (observacao.Length > 2000)
                throw new Exception("Observação não pode passar de 2000 caracteres.");

            motivo.Ativo = Request.GetBoolParam("Status");
            motivo.Descricao = descricao;
            motivo.Observacao = observacao;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);
                
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem repositorio = new Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem> listaMotivo = repositorio.Consultar(descricao, situacaoAtivo, parametrosConsulta);
                int totalRegistros = repositorio.ContarConsulta(descricao, situacaoAtivo);

                var listaMotivoRetornar = (
                    from motivo in listaMotivo
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
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
