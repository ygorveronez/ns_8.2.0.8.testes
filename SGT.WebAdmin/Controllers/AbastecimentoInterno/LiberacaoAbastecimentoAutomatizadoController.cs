using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.AbastecimentoInterno
{
    [CustomAuthorize("AbastecimentoInterno/LiberacaoAbastecimentoAutomatizado")]
    public class LiberacaoAbastecimentoAutomatizadoController : BaseController
    {
        #region Construtores

        public LiberacaoAbastecimentoAutomatizadoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                var propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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
                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(unitOfWork);
                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado = new Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado();
                Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao serAbastecimentoInternoIntegracao = new Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao(unitOfWork);

                PreencherEntidade(liberacaoAbastecimentoAutomatizado, unitOfWork);

                unitOfWork.Start();

                var codigo = repLiberacaoAbastecimentoAutomatizado.Inserir(liberacaoAbastecimentoAutomatizado, Auditado);

                serAbastecimentoInternoIntegracao.VerificarIntegracoesAbastecimentoAutomatizado(liberacaoAbastecimentoAutomatizado, unitOfWork);
                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao = repLiberacaoAbastecimentoAutomatizadoIntegracao.BuscarPorLiberacaoAbetecimentoAutomatizado(liberacaoAbastecimentoAutomatizado.Codigo).FirstOrDefault();
                serAbastecimentoInternoIntegracao.ProcessarIntegracaoPendenteReservaAbastecimento(liberacaoAbastecimentoAutomatizadoIntegracao, unitOfWork);

                unitOfWork.CommitChanges();


                if (liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento != SituacaoIntegracaoAbastecimento.AgRetornoReserva && liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento != SituacaoIntegracaoAbastecimento.Autorizado)
                    return new JsonpResult(false, true, "Reserva de abastecimento não foi realizada com sucesso!");


                return new JsonpResult(new
                {
                    Codigo = codigo
                }, true, "Reserva de abastecimento realizado com sucesso, aguarda o retorno!");

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

                return new JsonpResult(false, excecao.Message ?? "Ocorreu uma falha ao adicionar dados.");
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

                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado = repLiberacaoAbastecimentoAutomatizado.BuscarPorCodigo(codigo, true);

                if (liberacaoAbastecimentoAutomatizado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento != SituacaoIntegracaoAbastecimento.Pendente && liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento != SituacaoIntegracaoAbastecimento.PendenteReserva)
                    return new JsonpResult(false, true, "Não é possível atualizar registro nesta situação.");

                if (!repLiberacaoAbastecimentoAutomatizado.ConsultarDisponibilidadeDaBomba(liberacaoAbastecimentoAutomatizado.BombaAbastecimento.Codigo))
                    throw new ControllerException("A bomba de abastecimento selecionada não está disponível!");

                PreencherEntidade(liberacaoAbastecimentoAutomatizado, unitOfWork);

                unitOfWork.Start();

                repLiberacaoAbastecimentoAutomatizado.Atualizar(liberacaoAbastecimentoAutomatizado, Auditado);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado = repLiberacaoAbastecimentoAutomatizado.BuscarPorCodigo(codigo, false);

                if (liberacaoAbastecimentoAutomatizado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    liberacaoAbastecimentoAutomatizado.Codigo,
                    Veiculo = new { Codigo = liberacaoAbastecimentoAutomatizado.Veiculo?.Codigo ?? 0, Descricao = liberacaoAbastecimentoAutomatizado.Veiculo?.Placa ?? string.Empty },
                    Motorista = new { Codigo = liberacaoAbastecimentoAutomatizado.Motorista?.Codigo ?? 0, Descricao = liberacaoAbastecimentoAutomatizado.Motorista?.Descricao ?? string.Empty },
                    BombaAbastecimento = new { Codigo = liberacaoAbastecimentoAutomatizado.BombaAbastecimento?.Codigo ?? 0, Descricao = liberacaoAbastecimentoAutomatizado.BombaAbastecimento?.Descricao ?? string.Empty },
                    DataAbastecimento = liberacaoAbastecimentoAutomatizado.DataAbastecimento.ToString("dd/MM/yyyy"),
                    UltimaQuilometragem = liberacaoAbastecimentoAutomatizado.UltimaQuilometragem,
                    QuilometragemAtual = liberacaoAbastecimentoAutomatizado.QuilometragemAtual,
                    QuantidadeAbastecida = liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.QuantidadeAbastecida.ToString("n2"),
                    CodigoAutorizacao = liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.AuthID ?? string.Empty,
                    liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento,
                    liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.SituacaoIntegracao
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

                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado = repLiberacaoAbastecimentoAutomatizado.BuscarPorCodigo(codigo, true);

                if (liberacaoAbastecimentoAutomatizado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento != SituacaoIntegracaoAbastecimento.Pendente && liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento != SituacaoIntegracaoAbastecimento.PendenteReserva)
                    return new JsonpResult(false, true, "Não é possível excluir registro nesta situação.");


                unitOfWork.Start();

                repLiberacaoAbastecimentoAutomatizado.Deletar(liberacaoAbastecimentoAutomatizado, Auditado);

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

        public async Task<IActionResult> ExpirarAbastecimentoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(unitOfWork);
                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado = repLiberacaoAbastecimentoAutomatizado.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao = repLiberacaoAbastecimentoAutomatizadoIntegracao.BuscarPorLiberacaoAbetecimentoAutomatizado(codigo).FirstOrDefault();


                unitOfWork.Start();

                if (liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && (liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento != SituacaoIntegracaoAbastecimento.AgRetornoAbastecimento || liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento != SituacaoIntegracaoAbastecimento.Autorizado))
                {
                    liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.ProblemaAbastecimento;
                    liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = "TIMEOUT: Tempo de 2 minutos expirado";
                }

                repLiberacaoAbastecimentoAutomatizado.Atualizar(liberacaoAbastecimentoAutomatizado, Auditado);
                repLiberacaoAbastecimentoAutomatizadoIntegracao.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento
                }, true, "");
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
        public async Task<IActionResult> ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao = repIntegracao.BuscarPorLiberacaoAbetecimentoAutomatizado(codigo).FirstOrDefault();

                if (liberacaoAbastecimentoAutomatizadoIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada");

                unidadeDeTrabalho.Start();

                liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.PendenteReserva;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, liberacaoAbastecimentoAutomatizadoIntegracao, null, "Reenviou Integracao", unidadeDeTrabalho);

                repIntegracao.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu Uma Falha ao Enviar a Integração");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
#endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Frotas.BombaAbastecimento repBombaAbastecimento = new Repositorio.Embarcador.Frotas.BombaAbastecimento(unitOfWork);

            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoBomba = Request.GetIntParam("BombaAbastecimento");
            int ultimaQuilometragem = Request.GetIntParam("UltimaQuilometragem");
            int quilometragemAtual = Request.GetIntParam("QuilometragemAtual");
            int quilometrosRodados = Request.GetIntParam("QuilometrosRodados");

            liberacaoAbastecimentoAutomatizado.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : throw new Exception("Veículo não cadastrado!");
            liberacaoAbastecimentoAutomatizado.Motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : throw new Exception("Motorista não cadastrado!");
            liberacaoAbastecimentoAutomatizado.BombaAbastecimento = codigoBomba > 0 ? repBombaAbastecimento.BuscarPorCodigo(codigoBomba) : throw new Exception("Bomba não cadastrado!");
            liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.Pendente;
            liberacaoAbastecimentoAutomatizado.UltimaQuilometragem = ultimaQuilometragem;
            liberacaoAbastecimentoAutomatizado.QuilometragemAtual = (quilometragemAtual - ultimaQuilometragem) > 0 ? quilometragemAtual : throw new Exception("A quilometragem atual não pode ser menor que a última quilometragem registrada!");
            liberacaoAbastecimentoAutomatizado.QuilometrosRodados = (quilometragemAtual - ultimaQuilometragem) > 0 ? quilometrosRodados : throw new Exception("O valor de quilometros rodados não pode ser 0 ou negativo!");
            liberacaoAbastecimentoAutomatizado.DataAbastecimento = DateTime.Now;

        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data", "DataAbastecimento", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Bomba", "BombaAbastecimento", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "SituacaoAbastecimento", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Integração", "SituacaoIntegracao", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno Integração", "RetornoIntegracao", 50, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(unitOfWork);

            string descricao = Request.Params("Descricao");
            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoBomba = Request.GetIntParam("BombaAbastecimento");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento>("Situacao");

            List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado> listaLiberacaoAbastecimentoAutomatizado = repLiberacaoAbastecimentoAutomatizado.Consultar(descricao, codigoVeiculo, codigoMotorista, codigoBomba, propOrdenar, dirOrdena, inicio, limite, situacao);
            totalRegistros = repLiberacaoAbastecimentoAutomatizado.ContarConsulta(descricao, codigoVeiculo, codigoMotorista, codigoBomba, propOrdenar, dirOrdena, inicio, limite, situacao);

            var dynListaLiberacaoAbastecimentoAutomatizado = from liberacaoAbastecimentoAutomatizado in listaLiberacaoAbastecimentoAutomatizado
                                                             select new
                                                             {
                                                                 liberacaoAbastecimentoAutomatizado.Codigo,
                                                                 Veiculo = liberacaoAbastecimentoAutomatizado.Veiculo != null ? liberacaoAbastecimentoAutomatizado.Veiculo.Placa : string.Empty,
                                                                 Motorista = liberacaoAbastecimentoAutomatizado.Motorista != null ? liberacaoAbastecimentoAutomatizado.Motorista.Descricao : string.Empty,
                                                                 BombaAbastecimento = liberacaoAbastecimentoAutomatizado.BombaAbastecimento != null ? liberacaoAbastecimentoAutomatizado.BombaAbastecimento.Descricao : string.Empty,
                                                                 SituacaoAbastecimento = liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento.ObterDescricao(),
                                                                 SituacaoIntegracao = liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.SituacaoIntegracao.ObterDescricao() ?? string.Empty,
                                                                 RetornoIntegracao = liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.ProblemaIntegracao ?? string.Empty,
                                                                 DataAbastecimento = liberacaoAbastecimentoAutomatizado.DataAbastecimento.ToString("dd/MM/yyyy HH:mm"),
                                                                 DT_RowColor = liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                                         liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                                         liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo :
                                                                         Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                                                                 DT_FontColor = liberacaoAbastecimentoAutomatizado.Integracoes?.FirstOrDefault()?.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",

                                                             };

            return dynListaLiberacaoAbastecimentoAutomatizado.ToList();
        }

        #endregion
    }
}