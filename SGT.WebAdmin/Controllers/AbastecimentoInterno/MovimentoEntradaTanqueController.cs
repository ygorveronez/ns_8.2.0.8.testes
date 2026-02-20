using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Globalization;

namespace SGT.WebAdmin.Controllers.AbastecimentoInterno
{
    [CustomAuthorize("AbastecimentoInterno/MovimentoEntradaTanque")]
    public class MovimentoEntradaTanqueController : BaseController
    {
        #region Construtores

        public MovimentoEntradaTanqueController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque repMovimentoEntradaTanque = new Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque(unitOfWork);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicialEntrada"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinalEntrada"), out dataFinal);

                string descricao = Request.Params("Descricao");
                int.TryParse(Request.Params("TipoOleo"), out int codTipoOleo);
                int.TryParse(Request.Params("Empresa"), out int codEmpresa);
                int.TryParse(Request.Params("LocalArmazenamento"), out int codLocalArmazenamento);
                int.TryParse(Request.Params("DocumentoEntrada"), out int codNotaFiscalEntrada);
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Empresa", "Empresa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Oleo", "TipoOleo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Local de Armazenamento", "LocalArmazenamentoProduto", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Hora Entrada", "DataHoraEntrada", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Quantidade", "QuantidadeEntrada", 10, Models.Grid.Align.right, false);

                var propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo.Placa";

                var listaMovimentacao = repMovimentoEntradaTanque.Consultar(descricao,dataInicial, dataFinal, codTipoOleo, codEmpresa, codNotaFiscalEntrada, codLocalArmazenamento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMovimentoEntradaTanque.ContarConsulta(descricao, dataInicial, dataFinal, codTipoOleo, codEmpresa, codNotaFiscalEntrada, codLocalArmazenamento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite));

                var lista = (from p in listaMovimentacao
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 TipoOleo = p.TipoOleo != null ? p.TipoOleo.Descricao : string.Empty,
                                 LocalArmazenamentoProduto = p.LocalArmazenamentoProduto.Descricao ?? string.Empty,
                                 Empresa = p.Empresa.Descricao,
                                 Fornecedor = p.Fornecedor != null ? p.Fornecedor.Descricao : string.Empty,
                                 DataHoraEntrada = p.DataHoraEntrada != null ? p.DataHoraEntrada.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 QuantidadeEntrada = p.QuantidadeLitros.ToString("n4")

                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque repMovimentoEntradaTanque = new Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque movimentoEntradaTanque = new Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque();

                PreencherEntidade(movimentoEntradaTanque, unitOfWork);

                unitOfWork.Start();

                repMovimentoEntradaTanque.Inserir(movimentoEntradaTanque, Auditado);

                Servicos.Embarcador.Abastecimento.AbastecimentoInterno.GerarHistoricoMovimentoAbastecimento(unitOfWork, movimentoEntradaTanque.LocalArmazenamentoProduto, TipoMovimentacaoAbastecimento.Entrada, null, movimentoEntradaTanque);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

                Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque repMovimentoEntradaTanque = new Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque movimentoEntradaTanque = repMovimentoEntradaTanque.BuscarPorCodigo(codigo, true);

                if (movimentoEntradaTanque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(movimentoEntradaTanque, unitOfWork);

                unitOfWork.Start();

                repMovimentoEntradaTanque.Atualizar(movimentoEntradaTanque, Auditado);

                Servicos.Embarcador.Abastecimento.AbastecimentoInterno.GerarHistoricoMovimentoAbastecimento(unitOfWork, movimentoEntradaTanque.LocalArmazenamentoProduto, TipoMovimentacaoAbastecimento.AtualizaEntrada, null, movimentoEntradaTanque);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque repMovimentoEntradaTanque = new Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque movimentoEntradaTanque = repMovimentoEntradaTanque.BuscarPorCodigo(codigo, true);

                if (movimentoEntradaTanque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                Servicos.Embarcador.Abastecimento.AbastecimentoInterno.GerarHistoricoMovimentoAbastecimento(unitOfWork, movimentoEntradaTanque.LocalArmazenamentoProduto, TipoMovimentacaoAbastecimento.CancelaEntrada, null, movimentoEntradaTanque);

                repMovimentoEntradaTanque.Deletar(movimentoEntradaTanque, Auditado);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque repMovimentoEntradaTanque = new Repositorio.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque movimentoEntradaTanque = repMovimentoEntradaTanque.BuscarPorCodigo(codigo, false);

                if (movimentoEntradaTanque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    movimentoEntradaTanque.Codigo,
                    movimentoEntradaTanque.Descricao,
                    Empresa = new { Codigo = movimentoEntradaTanque.Empresa?.Codigo ?? 0, Descricao = movimentoEntradaTanque.Empresa?.Descricao ?? string.Empty },
                    DocumentoEntrada = new { Codigo = movimentoEntradaTanque.DocumentoEntrada?.Codigo ?? 0, Numero = movimentoEntradaTanque.DocumentoEntrada?.Numero ?? 0 ,Descricao = (movimentoEntradaTanque.DocumentoEntrada?.Numero ?? 0).ToString() },
                    DataHoraNotaFiscal = movimentoEntradaTanque.DataHoraNotaFiscal.ToString("dd/MM/yyyy"),
                    Fornecedor = new { Codigo = movimentoEntradaTanque.Fornecedor?.Codigo ?? 0, Descricao = movimentoEntradaTanque.Fornecedor?.Descricao ?? string.Empty },
                    LocalArmazenamento = new { Codigo = movimentoEntradaTanque.LocalArmazenamentoProduto?.Codigo ?? 0, Descricao = movimentoEntradaTanque.LocalArmazenamentoProduto?.Descricao ?? string.Empty },
                    TipoOleo = new { Codigo = movimentoEntradaTanque.TipoOleo?.Codigo ?? 0, Descricao = movimentoEntradaTanque.TipoOleo?.Descricao ?? string.Empty },
                    DataHoraEntrada = movimentoEntradaTanque.DataHoraEntrada.ToString("dd/MM/yyyy HH:mm:ss"),
                    QuantidadeEntrada = movimentoEntradaTanque.QuantidadeLitros.ToString("n2")
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

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque tabelaPrecoCombustivel, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
            Repositorio.Embarcador.Frotas.TipoOleo repTipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

            int codigoEmpresa = Request.GetIntParam("Empresa");
            string descricao = Request.GetStringParam("Descricao");
            int tipoOleo = Request.GetIntParam("TipoOleo");
            int codigoLocalArmazenamentoProduto = Request.GetIntParam("LocalArmazenamento");
            double fornecedor = Request.GetDoubleParam("Fornecedor");
            int codigoDocumentoEntradaTMS = Request.GetIntParam("DocumentoEntrada");

            DateTime dataHoraNotaFiscal;
            DateTime.TryParseExact(Request.Params("DataHoraNotaFiscal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataHoraNotaFiscal);

            DateTime dataHoraEntrada;
            DateTime.TryParseExact(Request.Params("DataHoraEntrada"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataHoraEntrada);

            decimal quantidade;
            decimal.TryParse(Request.Params("QuantidadeEntrada"), out quantidade);

            tabelaPrecoCombustivel.Descricao = !String.IsNullOrEmpty(descricao) ? descricao : throw new ControllerException("A descrição não pode ser nula ou vazia!");
            tabelaPrecoCombustivel.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : throw new ControllerException("Empresa não cadastrado!");
            tabelaPrecoCombustivel.Fornecedor = fornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(fornecedor) : throw new ControllerException("Fornecedor não cadastrado!");
            tabelaPrecoCombustivel.DocumentoEntrada = codigoDocumentoEntradaTMS > 0 ? repDocumentoEntradaTMS.BuscarPorCodigo(codigoDocumentoEntradaTMS) : throw new ControllerException("Docuemento de entrada não cadastrado!");
            tabelaPrecoCombustivel.DataHoraNotaFiscal = dataHoraNotaFiscal;
            tabelaPrecoCombustivel.LocalArmazenamentoProduto = codigoLocalArmazenamentoProduto > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamentoProduto) : throw new ControllerException("Local de armazenamento não cadastrado!");
            tabelaPrecoCombustivel.TipoOleo = tipoOleo > 0 ? repTipoOleo.BuscarPorCodigo(tipoOleo) : throw new ControllerException("Tipo de óleo não cadastrado!");
            tabelaPrecoCombustivel.DataHoraEntrada = dataHoraEntrada;
            tabelaPrecoCombustivel.QuantidadeLitros = quantidade;
            tabelaPrecoCombustivel.SaldoInicialAntesMovimentacaoEntrada = tabelaPrecoCombustivel.SaldoInicialAntesMovimentacaoEntrada ?? tabelaPrecoCombustivel.LocalArmazenamentoProduto.SaldoDoTanque;
        }

        #endregion
    }
}