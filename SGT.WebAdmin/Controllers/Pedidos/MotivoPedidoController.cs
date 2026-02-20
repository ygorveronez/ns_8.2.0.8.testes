using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/MotivoPedido")]
    public class MotivoPedidoController : BaseController
    {
		#region Construtores

		public MotivoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);
                var motivoPedido = new Dominio.Entidades.Embarcador.Pedidos.MotivoPedido();

                AtualizarMotivoPedido(motivoPedido);

                string mensagemErro = ValidarMotivoPedido(motivoPedido);

                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                repositorio.Inserir(motivoPedido, Auditado);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);
                int codigo = 0;

                int.TryParse(Request.Params("Codigo"), out codigo);

                var motivoPedido = repositorio.BuscarPorCodigo(codigo, true);

                if (motivoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                AtualizarMotivoPedido(motivoPedido);

                string mensagemErro = ValidarMotivoPedido(motivoPedido);

                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                repositorio.Atualizar(motivoPedido, Auditado);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);
                int codigo = 0;

                int.TryParse(Request.Params("Codigo"), out codigo);

                var MotivoPedido = repositorio.BuscarPorCodigo(codigo);

                if (MotivoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    MotivoPedido.Codigo,
                    MotivoPedido.Descricao,
                    Status = MotivoPedido.Ativo,
                    Observacao = MotivoPedido.Observacao ?? string.Empty,
                    MotivoPedido.TipoMotivo
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                var motivoPedido = repositorio.BuscarPorCodigo(codigo);

                if (motivoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorio.Deletar(motivoPedido, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMotivoRejeicao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //pesquisar tipo rejeição no repositorio
                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);
                var motivos = repositorio.BuscarPorTipo(EnumTipoMotivoPedido.RejeicaoPedido, Request.Params("Descricao"));

                //montar grid
                var grid = new Grid(Request)
                {
                    header = new List<Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Align.left, true);

                grid.AdicionaRows(motivos);
                grid.setarQuantidadeTotal(motivos.Count);

                var json = new JsonpResult(grid);
                return json;
            }
            catch (Exception)
            {

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMotivoLancamento()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //pesquisar tipo rejeição no repositorio
                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);
                var motivos = repositorio.BuscarPorTipo(EnumTipoMotivoPedido.LancamentoPedido, Request.Params("Descricao"));

                //montar grid
                var grid = new Grid(Request)
                {
                    header = new List<Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Align.left, true);

                grid.AdicionaRows(motivos);
                grid.setarQuantidadeTotal(motivos.Count);

                var json = new JsonpResult(grid);
                return json;
            }
            catch (Exception)
            {
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMotivoAprovacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //pesquisar tipo rejeição no repositorio
                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);
                var motivos = repositorio.BuscarPorTipo(EnumTipoMotivoPedido.AprovacaoPedido, Request.Params("Descricao"));

                //montar grid
                var grid = new Grid(Request)
                {
                    header = new List<Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Align.left, true);

                grid.AdicionaRows(motivos);
                grid.setarQuantidadeTotal(motivos.Count);

                var json = new JsonpResult(grid);
                return json;
            }
            catch (Exception)
            {
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMotivoPedido()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //pesquisar tipo rejeição no repositorio
                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);
                var motivos = repositorio.BuscarPorTipo(null, Request.Params("Descricao"));

                //montar grid
                var grid = new Grid(Request)
                {
                    header = new List<Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Align.left, true);

                grid.AdicionaRows(motivos);
                grid.setarQuantidadeTotal(motivos.Count);

                var json = new JsonpResult(grid);
                return json;
            }
            catch (Exception)
            {
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }
        #endregion

        #region Métodos Privados

        private void AtualizarMotivoPedido(Dominio.Entidades.Embarcador.Pedidos.MotivoPedido motivoPedido)
        {
            string descricao = string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? string.Empty : Request.Params("Descricao");
            string observacao = string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? string.Empty : Request.Params("Observacao");
            Enum tipomotivo = Request.GetNullableEnumParam<EnumTipoMotivoPedido>("TipoMotivo");

            bool ativo = false;

            bool.TryParse(Request.Params("Status"), out ativo);

            motivoPedido.Ativo = ativo;
            motivoPedido.Descricao = descricao;
            motivoPedido.Observacao = observacao;
            motivoPedido.TipoMotivo = (EnumTipoMotivoPedido)tipomotivo;
        }

        private Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Grid(Request)
                {
                    header = new List<Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Align.left, true);
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 10, Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoMotivo", 10, Align.left, true);

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
            var status = SituacaoAtivoPesquisa.Ativo;

            if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                Enum.TryParse(Request.Params("Status"), out status);

            var repositorio = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);
            var listaMotivoPedido = repositorio.Consultar(descricao, status, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(descricao, status);

            return (from motivoPedido in listaMotivoPedido
                    select new
                    {
                        Codigo = motivoPedido.Codigo,
                        Descricao = motivoPedido.Descricao,
                        DescricaoAtivo = motivoPedido.DescricaoAtivo,
                        DescricaoTipoMotivo = motivoPedido.DescricaoTipoMotivo,
                        TipoMotivo = motivoPedido.TipoMotivo
                    }
            ).ToList();
        }

        private string ValidarMotivoPedido(Dominio.Entidades.Embarcador.Pedidos.MotivoPedido motivoPedido)
        {
            if (string.IsNullOrWhiteSpace(motivoPedido.Descricao))
                return "Descrição é obrigatória.";

            if (motivoPedido.Descricao.Length > 200)
                return "Descrição não pode passar de 200 caracteres.";

            if (motivoPedido.Observacao.Length > 2000)
                return "Observação não pode passar de 2000 caracteres.";

            return string.Empty;
        }

        #endregion
    }
}
