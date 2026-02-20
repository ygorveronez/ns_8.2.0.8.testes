using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Integracao
{
    [CustomAuthorize("Cargas/Carga", "Cargas/CargaIntegracaoEmbarcador")]
    public class CargaIntegracaoEmbarcadorController : BaseController
    {
        #region Construtores

        public CargaIntegracaoEmbarcadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador filtros = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> registros = new List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador>();

                int countRegistros = repCargaIntegracaoEmbarcador.ContarConsulta(filtros);

                if (countRegistros > 0)
                    registros = repCargaIntegracaoEmbarcador.Consultar(filtros, parametroConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows((from obj in registros
                                   select new
                                   {
                                       obj.Codigo,
                                       DataCriacaoCarga = obj.DataCriacaoCarga?.ToString("dd/MM/yyyy HH:mm"),
                                       NumeroCarga = obj.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                       NumeroCargaEmbarcador = obj.NumeroCarga,
                                       Empresa = obj.Empresa?.Descricao ?? string.Empty,
                                       obj.Origem,
                                       obj.Destino,
                                       Situacao = obj.Situacao.ObterDescricao(),
                                       SituacaoCarga = obj.Carga?.SituacaoCarga.ObterDescricao() ?? string.Empty,
                                       SituacaoCancelamento = obj.CargaCancelamento?.Situacao.Descricao() ?? string.Empty,
                                       obj.Mensagem
                                   }).ToList());

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

        public async Task<IActionResult> BuscarCargasIntegracaoEmbarcador(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!ConfiguracaoEmbarcador.ImportarCargasMultiEmbarcador)
                    return new JsonpResult(false, true, "Recurso não habilitado para o ambiente.");

                if (!Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.ImportarCargasEmbarcador(out string mensagemErro, TipoServicoMultisoftware, unitOfWork))
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, mensagemErro);
                }

                if (!Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.ImportarCTesCargasEmbarcador(out mensagemErro, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador))
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, mensagemErro);
                }

                if (!await new Servicos.Embarcador.Carga.CargaImportacaoEmbarcador(unitOfWork, TipoServicoMultisoftware, cancellationToken).ImportarMDFesCargasEmbarcadorAsync())
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, mensagemErro);
                }

                if (!Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.GerarCargasEmbarcador(out mensagemErro, TipoServicoMultisoftware, unitOfWork))
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, mensagemErro);
                }

                if (!Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.ImportarCancelamentosEmbarcador(out mensagemErro, TipoServicoMultisoftware, unitOfWork))
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, mensagemErro);
                }

                if (!Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.ImportarCTesCanceladosEmbarcador(out mensagemErro, TipoServicoMultisoftware, unitOfWork))
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, mensagemErro);
                }

                if (!Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.GerarCancelamentosEmbarcador(out mensagemErro, TipoServicoMultisoftware, unitOfWork))
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, mensagemErro);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar as cargas de integrações com embarcadores.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridExportacao();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", "CargasImportadasDoEmbarcador." + grid.extensaoCSV);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DefinirAcaoManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                if (codigo == 0)
                    throw new ControllerException("Carga não encontrada");

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracao = repCargaIntegracaoEmbarcador.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                cargaIntegracao.Situacao = SituacaoCargaIntegracaoEmbarcador.AjustadoManualmente;
                repCargaIntegracaoEmbarcador.Atualizar(cargaIntegracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a carga!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador filtros = new Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador()
            {
                CodigoEmpresa = Request.GetListParam<int>("Empresa"),
                CodigoMotorista = Request.GetListParam<int>("Motorista"),
                CodigoVeiculo = Request.GetListParam<int>("Veiculo"),
                CodigoTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                DataFinalCarga = Request.GetNullableDateTimeParam("DataFinalCarga"),
                DataInicialCarga = Request.GetNullableDateTimeParam("DataInicialCarga"),
                NumeroCarga = Request.GetNullableStringParam("NumeroCarga"),
                NumeroCargaEmbarcador = Request.GetNullableStringParam("NumeroCargaEmbarcador"),
                NumeroCTe = Request.GetNullableIntParam("NumeroCTe"),
                NumeroMDFe = Request.GetNullableIntParam("NumeroMDFe"),
                Situacao = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador>("Situacao"),
                SituacaoCancelamento = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga>("SituacaoCancelamento"),
                SituacaoCarga = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>("SituacaoCarga")
            };

            return filtros;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº Carga Embarcador", "NumeroCargaEmbarcador", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Carga", "DataCriacaoCarga", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Empresa/Filial", "Empresa", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", 16, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Destino", "Destino", 16, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Sit. Carga", "SituacaoCarga", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Sit. Cancelamento", "SituacaoCancelamento", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Mensagem Processamento", "Mensagem", 12, Models.Grid.Align.left, false);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string prop)
        {
            if (prop == "SituacaoCarga")
                return "Carga.SituacaoCarga";
            else if (prop == "SituacaoCancelamento")
                return "CargaCancelamento.Situacao";

            return prop;
        }

        private Models.Grid.Grid ObterGridExportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador filtros = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> registros = new List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador>();

                int countRegistros = repCargaIntegracaoEmbarcador.ContarConsulta(filtros);

                if (countRegistros > 0)
                    registros = repCargaIntegracaoEmbarcador.ConsultarSemFetch(filtros, parametroConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows((from obj in registros
                                   select new
                                   {
                                       obj.Codigo,
                                       DataCriacaoCarga = obj.DataCriacaoCarga?.ToString("dd/MM/yyyy HH:mm"),
                                       NumeroCarga = obj.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                       NumeroCargaEmbarcador = obj.NumeroCarga,
                                       Empresa = obj.Empresa?.Descricao ?? string.Empty,
                                       obj.Origem,
                                       obj.Destino,
                                       Situacao = obj.Situacao.ObterDescricao(),
                                       SituacaoCarga = obj.Carga?.SituacaoCarga.ObterDescricao() ?? string.Empty,
                                       SituacaoCancelamento = obj.CargaCancelamento?.Situacao.Descricao() ?? string.Empty,
                                       obj.Mensagem
                                   }).ToList());

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
        #endregion
    }
}
