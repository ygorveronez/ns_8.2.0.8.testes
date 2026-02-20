using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/MotivoAvariaPallet")]
    public class MotivoAvariaPalletController : BaseController
    {
		#region Construtores

		public MotivoAvariaPalletController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Privados

        private void AtualizarMotivoAvariaPallet(Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet motivoAvariaPallet)
        {
            string descricao = string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? string.Empty : Request.Params("Descricao");
            string observacao = string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? string.Empty : Request.Params("Observacao");
            bool ativo = false;

            bool.TryParse(Request.Params("Status"), out ativo);

            motivoAvariaPallet.Ativo = ativo;
            motivoAvariaPallet.Descricao = descricao;
            motivoAvariaPallet.Observacao = observacao;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                var propriedadeOrdenar = (grid.header[grid.indiceColunaOrdena].data == "DescricaoAtivo") ? "Ativo" : grid.header[grid.indiceColunaOrdena].data;
                int totalRegistros = 0;
                var lista = Pesquisar(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
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

        private dynamic Pesquisar(ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var descricao = Request.Params("Descricao");
            var status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
           
            if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                Enum.TryParse(Request.Params("Status"), out status);

            var repositorio = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);
            var listaMotivoAvariaPallet = repositorio.Consultar(descricao, status, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(descricao, status);

            return (from motivoAvariaPallet in listaMotivoAvariaPallet select new
                {
                    Codigo = motivoAvariaPallet.Codigo,
                    Descricao = motivoAvariaPallet.Descricao,
                    DescricaoAtivo = motivoAvariaPallet.DescricaoAtivo
                }
            ).ToList();
        }

        private string ValidarMotivoAvariaPallet(Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet motivoAvariaPallet)
        {
            if (string.IsNullOrWhiteSpace(motivoAvariaPallet.Descricao))
                return "Descrição é obrigatória.";

            if (motivoAvariaPallet.Descricao.Length > 200)
                return "Descrição não pode passar de 200 caracteres.";

            if (motivoAvariaPallet.Observacao.Length > 2000)
                return "Observação não pode passar de 2000 caracteres.";

            return string.Empty;
        }

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);
                var motivoAvariaPallet = new Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet();

                AtualizarMotivoAvariaPallet(motivoAvariaPallet);

                string mensagemErro = ValidarMotivoAvariaPallet(motivoAvariaPallet);

                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                repositorio.Inserir(motivoAvariaPallet, Auditado);
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
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);
                int codigo = 0;

                int.TryParse(Request.Params("Codigo"), out codigo);

                var motivoAvariaPallet = repositorio.BuscarPorCodigo(codigo, true);

                if (motivoAvariaPallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                AtualizarMotivoAvariaPallet(motivoAvariaPallet);

                string mensagemErro = ValidarMotivoAvariaPallet(motivoAvariaPallet);

                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                repositorio.Atualizar(motivoAvariaPallet, Auditado);
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
                var repositorio = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);
                int codigo = 0;

                int.TryParse(Request.Params("Codigo"), out codigo);

                var MotivoAvariaPallet = repositorio.BuscarPorCodigo(codigo);

                if (MotivoAvariaPallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    MotivoAvariaPallet.Codigo,
                    MotivoAvariaPallet.Descricao,
                    Status = MotivoAvariaPallet.Ativo,
                    Observacao = MotivoAvariaPallet.Observacao ?? string.Empty
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
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                var motivoAvariaPallet = repositorio.BuscarPorCodigo(codigo);

                if (motivoAvariaPallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorio.Deletar(motivoAvariaPallet, Auditado);

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
    }
}
