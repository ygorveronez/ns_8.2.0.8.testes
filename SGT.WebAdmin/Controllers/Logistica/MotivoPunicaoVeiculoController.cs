using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MotivoPunicaoVeiculo")]
    public class MotivoPunicaoVeiculoController : BaseController
    {
		#region Construtores

		public MotivoPunicaoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var motivoPunicao = new Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo();

                try
                {
                    PreencherMotivoPunicao(motivoPunicao);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Logistica.MotivoPunicaoVeiculo(unitOfWork);

                repositorio.Inserir(motivoPunicao, Auditado);

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.MotivoPunicaoVeiculo(unitOfWork);
                var motivoPunicao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoPunicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherMotivoPunicao(motivoPunicao);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(motivoPunicao, Auditado);

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.MotivoPunicaoVeiculo(unitOfWork);
                var motivoPunicao = repositorio.BuscarPorCodigo(codigo);

                if (motivoPunicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoPunicao.Codigo,
                    motivoPunicao.Descricao,
                    Status = motivoPunicao.Ativo,
                    motivoPunicao.Observacao
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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.MotivoPunicaoVeiculo(unitOfWork);
                var motivoPunicao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoPunicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(motivoPunicao, Auditado);

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

        private void PreencherMotivoPunicao(Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo motivoPunicao)
        {
            var ativo = Request.GetBoolParam("Status");
            var descricao = Request.Params("Descricao");
            var observacao = Request.Params("Observacao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new Exception("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new Exception("Descrição não pode passar de 200 caracteres.");

            if (observacao.Length > 2000)
                throw new Exception("Observação não pode passar de 2000 caracteres.");

            motivoPunicao.Ativo = ativo;
            motivoPunicao.Descricao = descricao;
            motivoPunicao.Observacao = observacao;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var descricao = Request.Params("Descricao");
                var status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var repositorio = new Repositorio.Embarcador.Logistica.MotivoPunicaoVeiculo(unitOfWork);
                var listaMotivoPunicao = repositorio.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                var totalRegistros = repositorio.ContarConsulta(descricao, status);

                var listaMotivoPunicaoRetornar = (
                    from motivo in listaMotivoPunicao
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoPunicaoRetornar);
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
