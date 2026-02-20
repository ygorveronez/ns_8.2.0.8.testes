using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.TipoCarregamento
{
    public class TipoCarregamentoController : BaseController
    {
        #region Construtores

        public TipoCarregamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Cargas.TipoCarregamento repTipoCarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.TipoCarregamento tipoCarregamento = new Dominio.Entidades.Embarcador.Cargas.TipoCarregamento();

                PreencherDados(tipoCarregamento, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoCarregamento tipoCarregamentoCargaAgrupada = await repTipoCarregamento.BuscarTipoPadraoCargaAgrupadaAsync();
                if (tipoCarregamentoCargaAgrupada != null && tipoCarregamento.TipoPadraoAgrupamentoCarga)
                    return new JsonpResult(false, true, $"O Tipo de carregamento {tipoCarregamentoCargaAgrupada.Descricao} já está marcado como Tipo padrão para Agrupamento de Cargas.");


                await repTipoCarregamento.InserirAsync(tipoCarregamento, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.TipoCarregamento repTipoCarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.TipoCarregamento tipoCarregamento = repTipoCarregamento.BuscarPorCodigo(codigo, true);

                PreencherDados(tipoCarregamento, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoCarregamento tipoCarregamentoCargaAgrupada = await repTipoCarregamento.BuscarTipoPadraoCargaAgrupadaAsync();
                if (tipoCarregamentoCargaAgrupada != null && tipoCarregamentoCargaAgrupada.Codigo != tipoCarregamento.Codigo && tipoCarregamento.TipoPadraoAgrupamentoCarga)
                    return new JsonpResult(false, true, $"O Tipo de carregamento {tipoCarregamentoCargaAgrupada.Descricao} já está marcado como Tipo padrão para Agrupamento de Cargas.");

                if (tipoCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                await unitOfWork.StartAsync(cancellationToken);

                await repTipoCarregamento.AtualizarAsync(tipoCarregamento, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.TipoCarregamento repTipoCarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoCarregamento tipoCarregamento = repTipoCarregamento.BuscarPorCodigo(codigo, true);


                if (tipoCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoCarregamento.Codigo,
                    tipoCarregamento.Descricao,
                    CodigoDeIntegracao = tipoCarregamento.CodigoIntegracao,
                    tipoCarregamento.Observacao,
                    tipoCarregamento.Situacao,
                    tipoCarregamento.TipoPadraoAgrupamentoCarga
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
                Repositorio.Embarcador.Cargas.TipoCarregamento repTipoCarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoCarregamento tipoCarregamento = repTipoCarregamento.BuscarPorCodigo(codigo, auditavel: true);

                if (tipoCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repTipoCarregamento.Deletar(tipoCarregamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseje mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
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

        public async Task<IActionResult> ObterConfiguracoesTipoCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.TipoCarregamento repTipoCarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(unitOfWork);

                bool retorno = repTipoCarregamento.BuscarTodos().ToList().Count() > 0;

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.left, true);


                Repositorio.Embarcador.Cargas.TipoCarregamento repTipoCarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoCarregamento filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repTipoCarregamento.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento> listaTipoCarregamento = totalRegistros > 0 ? repTipoCarregamento.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento>();

                var lista = (from p in listaTipoCarregamento
                             select new
                             {
                                 p.Codigo,
                                 Descricao = p.Descricao,
                                 CodigoIntegracao = p.CodigoIntegracao,
                                 Observacao = p.Observacao,
                                 Situacao = p.DescricaoSituacao,
                             }).ToList();


                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoCarregamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoCarregamento()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoDeIntegracao"),
                Observacao = Request.GetStringParam("Observacao"),
                Situacao = Request.GetNullableBoolParam("Situacao"),

            };
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Cargas.TipoCarregamento tipoCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            double codigoPessoa = Request.GetDoubleParam("Pessoa");

            tipoCarregamento.Codigo = Request.GetIntParam("Codigo");
            tipoCarregamento.Descricao = Request.GetStringParam("Descricao");
            tipoCarregamento.CodigoIntegracao = Request.GetStringParam("CodigoDeIntegracao");
            tipoCarregamento.Observacao = Request.GetStringParam("Observacao");
            tipoCarregamento.Situacao = Request.GetBoolParam("Situacao");
            tipoCarregamento.TipoPadraoAgrupamentoCarga = Request.GetBoolParam("TipoPadraoAgrupamentoCarga");

        }


    }


    #endregion
}

