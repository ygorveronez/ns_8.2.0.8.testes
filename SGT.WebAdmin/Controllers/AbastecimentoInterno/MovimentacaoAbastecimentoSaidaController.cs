using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Data;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Controllers.AbastecimentoInterno
{
    [CustomAuthorize("AbastecimentoInterno/MovimentacaoAbastecimentoSaida")]
    public class MovimentacaoAbastecimentoSaidaController : BaseController
    {
        #region Construtores

        public MovimentacaoAbastecimentoSaidaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida repMovimentacao = new Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida(unitOfWork);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                int.TryParse(Request.Params("Veiculo"), out int codVeiculo);
                int.TryParse(Request.Params("Empresa"), out int codEmpresa);
                int.TryParse(Request.Params("LocalArmazenamento"), out int codLocalArmazenamento);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Placa", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Local de Armazenamento", "LocalArmazenamentoProduto", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Empresa", "Empresa", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Quantidade", "QuantidadeLitros", 10, Models.Grid.Align.right, false);

                var propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo.Placa";

                var listaMovimentacao = repMovimentacao.Consultar(dataInicial,dataFinal,codVeiculo,codEmpresa,codLocalArmazenamento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMovimentacao.ContarConsulta(dataInicial, dataFinal, codVeiculo, codEmpresa, codLocalArmazenamento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite));

                var lista = (from p in listaMovimentacao
                             select new
                             {
                                 p.Codigo,
                                 Placa = p.Veiculo != null ? p.Veiculo.Placa : string.Empty,
                                 LocalArmazenamentoProduto = p.LocalArmazenamentoProduto.Descricao ?? string.Empty,
                                 Empresa = p.Empresa.Descricao,
                                 Data = p.Data != null ? p.Data.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 QuantidadeLitros = p.QuantidadeLitros.ToString("n4")

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

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida repMovimentacao = new Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida(unitOfWork);
                Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida movimentacaoSaida = new Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida();

                PreencheEntidade(movimentacaoSaida, unitOfWork);

                string erro;
                if (!Servicos.Embarcador.Abastecimento.AbastecimentoInterno.ValidaEntidadeMovimentacaoAbastecimentoSaida(movimentacaoSaida, out erro))
                    return new JsonpResult(false, false , erro);

                repMovimentacao.Inserir(movimentacaoSaida);
                Servicos.Embarcador.Abastecimento.AbastecimentoInterno.GerarHistoricoMovimentoAbastecimento(unitOfWork, movimentacaoSaida.LocalArmazenamentoProduto, TipoMovimentacaoAbastecimento.Saida, movimentacaoSaida, null);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
                    
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();
                Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida repMovimentacao = new Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida(unitOfWork);
                Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida movimentacaoSaida = repMovimentacao.BuscarPorCodigo(codigo);

                if (movimentacaoSaida == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(movimentacaoSaida, unitOfWork);

                string erro;
                if (!Servicos.Embarcador.Abastecimento.AbastecimentoInterno.ValidaEntidadeMovimentacaoAbastecimentoSaida(movimentacaoSaida, out erro))
                    return new JsonpResult(false, true, erro);


                repMovimentacao.Atualizar(movimentacaoSaida);
                Servicos.Embarcador.Abastecimento.AbastecimentoInterno.GerarHistoricoMovimentoAbastecimento(unitOfWork, movimentacaoSaida.LocalArmazenamentoProduto, TipoMovimentacaoAbastecimento.AtualizaSaida, movimentacaoSaida, null);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida repMovimentacao = new Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida(unitOfWork);
                Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida movimentacao = repMovimentacao.BuscarPorCodigo(codigo);

                if (movimentacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynMovimentacao = new
                {
                    movimentacao.Codigo,
                    Data = movimentacao.Data.ToString("dd/MM/yyyy hh:mm:ss"),
                    Veiculo = movimentacao.Veiculo != null ? new { movimentacao.Veiculo.Codigo, Descricao = $"{movimentacao.Veiculo.Placa} ({(movimentacao.Veiculo.Tipo == "P" ? "PRÓPRIO)" : "TERCEIRO)")}) {(movimentacao.Veiculo.ModeloVeicularCarga?.Descricao ?? "")}", Tipo = movimentacao.Veiculo.Tipo } : null,
                    LocalArmazenamento = movimentacao.LocalArmazenamentoProduto != null ? new { movimentacao.LocalArmazenamentoProduto.Codigo, movimentacao.LocalArmazenamentoProduto.Descricao, movimentacao.LocalArmazenamentoProduto.QuantidadeSinalizacaoLitros } : null,
                    Empresa = movimentacao.Empresa != null ? new { movimentacao.Empresa.Codigo, movimentacao.Empresa.Descricao } : null,
                    movimentacao.TipoAbastecimento,
                    BombaAbastecimento = movimentacao.BombaAbastecimento != null ? new { movimentacao.BombaAbastecimento.Codigo, movimentacao.BombaAbastecimento.Descricao } : null,
                    TipoDeOleo = movimentacao.TipoOleo != null ? new { movimentacao.TipoOleo.Codigo, Descricao = movimentacao.TipoOleo.Descricao } : null,
                    Prefixo = movimentacao.Veiculo.NumeroFrota != null ? movimentacao.Veiculo.NumeroFrota : movimentacao.Veiculo.Placa,
                    movimentacao.QuantidadeLitros,
                    movimentacao.Valor,
                    movimentacao.Hodometro,
                    movimentacao.QuantidadeArla32
                };

                return new JsonpResult(dynMovimentacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida repMovimentacao = new Repositorio.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida(unitOfWork);
                Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida movimentacao = repMovimentacao.BuscarPorCodigo(codigo);

                if (movimentacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Abastecimento.AbastecimentoInterno.GerarHistoricoMovimentoAbastecimento(unitOfWork, movimentacao.LocalArmazenamentoProduto, TipoMovimentacaoAbastecimento.CancelaSaida, movimentacao, null);
                repMovimentacao.Deletar(movimentacao);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                {
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                }
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private void PreencheEntidade(Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida movimentacaoSaida, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
            Repositorio.Embarcador.Frotas.BombaAbastecimento repBomba = new Repositorio.Embarcador.Frotas.BombaAbastecimento(unitOfWork);
            Repositorio.Embarcador.Frotas.TipoOleo repTipoOleo = new Repositorio.Embarcador.Frotas.TipoOleo(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
          
            int codigoEmpresa = Request.GetIntParam("Empresa");
            int codigoLocalArmazenamento = Request.GetIntParam("LocalArmazenamento");
            int codigoBomba = Request.GetIntParam("BombaAbastecimento");
            int codigoTipoOleo = Request.GetIntParam("TipoDeOleo");
            int codigoVeiculo = Request.GetIntParam("Veiculo");

            movimentacaoSaida.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            movimentacaoSaida.Data = Request.GetDateTimeParam("Data");
            movimentacaoSaida.TipoAbastecimento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoAbastecimento>("TipoAbastecimento");
            movimentacaoSaida.LocalArmazenamentoProduto = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;
            movimentacaoSaida.BombaAbastecimento = codigoBomba > 0 ? repBomba.BuscarPorCodigo(codigoBomba) : null;
            movimentacaoSaida.TipoOleo = codigoTipoOleo > 0 ? repTipoOleo.BuscarPorCodigo(codigoTipoOleo) : null;
            movimentacaoSaida.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            movimentacaoSaida.QuantidadeLitros = Request.GetDecimalParam("QuantidadeLitros");
            movimentacaoSaida.Valor = Request.GetDecimalParam("Valor");
            movimentacaoSaida.Hodometro = Request.GetIntParam("Hodometro");
            movimentacaoSaida.QuantidadeArla32 = Request.GetDecimalParam("QuantidadeArla32");
            movimentacaoSaida.SaldoInicialAntesMovimentacaoSaida = movimentacaoSaida.SaldoInicialAntesMovimentacaoSaida ?? movimentacaoSaida.LocalArmazenamentoProduto.SaldoDoTanque;
        }

        #endregion Métodos Privados
    }
}