using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/MotivoCancelamentoPedido")]
    public class MotivoCancelamentoPedidoController : BaseController
    {
        #region Construtores

        public MotivoCancelamentoPedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoCancelamentoPedido(unitOfWork);
                var motivoCancelamentoPedido = new Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido();

                AtualizarMotivoCancelamentoPedido(motivoCancelamentoPedido);

                string mensagemErro = ValidarMotivoCancelamentoPedido(motivoCancelamentoPedido);

                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                repositorio.Inserir(motivoCancelamentoPedido, Auditado);
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

                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoCancelamentoPedido(unitOfWork);
                int codigo = 0;

                int.TryParse(Request.Params("Codigo"), out codigo);

                var motivoCancelamentoPedido = repositorio.BuscarPorCodigo(codigo, true);

                if (motivoCancelamentoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                AtualizarMotivoCancelamentoPedido(motivoCancelamentoPedido);

                string mensagemErro = ValidarMotivoCancelamentoPedido(motivoCancelamentoPedido);

                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                repositorio.Atualizar(motivoCancelamentoPedido, Auditado);
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
                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoCancelamentoPedido(unitOfWork);
                int codigo = 0;

                int.TryParse(Request.Params("Codigo"), out codigo);

                var MotivoCancelamento = repositorio.BuscarPorCodigo(codigo);

                if (MotivoCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    MotivoCancelamento.Codigo,
                    MotivoCancelamento.Descricao,
                    Status = MotivoCancelamento.Ativo,
                    Motivo = MotivoCancelamento.Motivo ?? string.Empty,                    
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

                var repositorio = new Repositorio.Embarcador.Pedidos.MotivoCancelamentoPedido(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                var motivoCancelamentoPedido = repositorio.BuscarPorCodigo(codigo);

                if (motivoCancelamentoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorio.Deletar(motivoCancelamentoPedido, Auditado);

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
                    return Arquivo(arquivoBinario, grid.extensaoCSV, $"{grid.tituloExportacao}.{grid.extensaoCSV}");

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

        private void AtualizarMotivoCancelamentoPedido(Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido motivoCancelamentoPedido)
        {
            string descricao = string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? string.Empty : Request.Params("Descricao");
            string motivo = string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? string.Empty : Request.Params("Motivo");

            bool ativo = false;

            bool.TryParse(Request.Params("Status"), out ativo);

            motivoCancelamentoPedido.Ativo = ativo;
            motivoCancelamentoPedido.Descricao = descricao;
            motivoCancelamentoPedido.Motivo = motivo;            
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

            var repositorio = new Repositorio.Embarcador.Pedidos.MotivoCancelamentoPedido(unitOfWork);
            var listaMotivoCancelamento = repositorio.Consultar(descricao, status, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(descricao, status);

            return (from motivoCancelamentoPedido in listaMotivoCancelamento
                    select new
                    {
                        Codigo = motivoCancelamentoPedido.Codigo,
                        Descricao = motivoCancelamentoPedido.Descricao,
                        DescricaoAtivo = motivoCancelamentoPedido.DescricaoAtivo
                    }
            ).ToList();
        }

        private string ValidarMotivoCancelamentoPedido(Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido motivoCancelamentoPedido)
        {
            if (string.IsNullOrWhiteSpace(motivoCancelamentoPedido.Descricao))
                return "Descrição é obrigatória.";

            if (motivoCancelamentoPedido.Descricao.Length > 200)
                return "Descrição não pode passar de 200 caracteres.";

            if (motivoCancelamentoPedido.Motivo.Length > 2000)
                return "Motivo não pode passar de 2000 caracteres.";

            return string.Empty;
        }
        #endregion
    }
}