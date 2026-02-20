using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Globalization;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "AdicionarPorModal", "VerificarSeExisteRegraCotacao" }, "Fretes/TabelaFrete", "Fretes/TabelaFreteAnexo")]
    public class TabelaFreteController : BaseController
    {
        #region Construtores

        public TabelaFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarVigencia()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFrete = int.Parse(Request.Params("TabelaFrete"));
                int codigoContrato = int.Parse(Request.Params("ContratoDoTransportador"));

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, DateTimeStyles.None, out dataFinal);
                int empresa = Request.GetIntParam("Empresa");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.DataInicial, "DataInicial", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.DataFinal, "DataFinal", 25, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Transportador, "Empresa", 30, Models.Grid.Align.left, true);

                DateTime dataInicialContrato = Request.GetDateTimeParam("DataInicialVigencia");
                DateTime dataFinalContrato = Request.GetDateTimeParam("DataFinalVigencia");

                if (dataInicialContrato != DateTime.MinValue)
                    dataInicial = dataInicialContrato;

                Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> vigencias = repVigencia.Consultar(dataInicial, dataFinal, codigoTabelaFrete, empresa, codigoContrato, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repVigencia.ContarConsulta(dataInicial, dataFinal, codigoTabelaFrete, empresa, codigoContrato));

                grid.AdicionaRows((from obj in vigencias
                                   select new
                                   {
                                       obj.Codigo,
                                       DataInicial = obj.DataInicial.ToString("dd/MM/yyyy"),
                                       Empresa = obj.Empresa?.Descricao ?? "",
                                       DataFinal = obj.DataFinal?.ToString("dd/MM/yyyy") ?? ""
                                   }).ToList());


                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarPorModal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");

                DateTime dataInicialContrato = Request.GetDateTimeParam("DataInicialContrato");
                DateTime dataFinalContrato = Request.GetDateTimeParam("DataFinalContrato");

                Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete) ?? throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.NaoFoiPossivelEncontrarATabelaDeFrete);

                Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia = new Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete()
                {
                    TabelaFrete = tabelaFrete,
                    DataInicial = Request.GetDateTimeParam("DataInicial"),
                    DataFinal = Request.GetNullableDateTimeParam("DataFinal")
                };

                ValidarVigenciaContratoTransporteFrete(vigencia, dataInicialContrato, dataFinalContrato);
                ValidarEntidadeVigencia(vigencia, unitOfWork);

                repositorioVigencia.Inserir(vigencia, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    vigencia.Codigo,
                    DataInicial = vigencia.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = vigencia.DataFinal?.ToString("dd/MM/yyyy") ?? ""
                });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoAdicionarAVigencia);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarVigenciaAtual()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFrete = 0;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);

                int empresa = Request.GetIntParam("Empresa");

                Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia = null;
                List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> vigencias = repVigencia.Buscar(DateTime.Now, codigoTabelaFrete, empresa);
                if (vigencias.Count == 1 || empresa > 0)
                    vigencia = vigencias.FirstOrDefault();

                if (vigencia != null)
                {
                    return new JsonpResult(new
                    {
                        DataInicial = vigencia.DataInicial.ToString("dd/MM/yyyy"),
                        DataFinal = vigencia.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                        vigencia.Codigo
                    });
                }
                else
                {
                    return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.VigenciaAtualNaoEncontrada);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoObterAVigenciaAtualParaATabelaDeFrete);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarVigenciaAtualContratoTransportador()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoContratoTransporteFrete = Request.GetIntParam("ContratoTransporteFrete");

                Repositorio.Embarcador.Frete.ContratoTransporteFrete repVigencia = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete = repVigencia.BuscarPorCodigo(codigoContratoTransporteFrete);

                if (contratoTransporteFrete == null)
                    return new JsonpResult(false, "Contrato não encontrado");

                return new JsonpResult(new
                {
                    DataInicial = contratoTransporteFrete.DataInicio.ToString("dd/MM/yyyy"),
                    DataFinal = contratoTransporteFrete.DataFim.ToString("dd/MM/yyyy")
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoObterAVigenciaAtualParaATabelaDeFrete);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTabelasPorTipo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete tipoTabela = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete)int.Parse(Request.Params("TipoTabelaFrete"));

                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> listaTabelaFrete = repTabelaFrete.BuscarPorTipo(tipoTabela);

                var lista = from obj in listaTabelaFrete select new { obj.Codigo, obj.Descricao };

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = new Dominio.Entidades.Embarcador.Frete.TabelaFrete();

                PreencherTabelaFrete(tabelaFrete, unidadeDeTrabalho);

                SalvarFronteiras(tabelaFrete, unidadeDeTrabalho);
                SalvarTipoEmbalagens(tabelaFrete, unidadeDeTrabalho);
                SalvarTiposOperacao(tabelaFrete, unidadeDeTrabalho);
                SalvarTiposOcorrencia(tabelaFrete, unidadeDeTrabalho);
                SalvarTransportadores(tabelaFrete, unidadeDeTrabalho);
                SalvarFiliais(tabelaFrete, unidadeDeTrabalho);
                SalvarTransportadoresTerceiros(tabelaFrete, unidadeDeTrabalho);
                SalvarTiposTerceiros(tabelaFrete, unidadeDeTrabalho);
                SalvarContratos(tabelaFrete, unidadeDeTrabalho);

                ValidarTabelaFreteDuplicada(tabelaFrete, unidadeDeTrabalho);

                repTabelaFrete.Inserir(tabelaFrete, Auditado);

                SalvarTiposCarga(tabelaFrete, unidadeDeTrabalho);
                SalvarModelosTracao(tabelaFrete, unidadeDeTrabalho);
                SalvarModelosReboque(tabelaFrete, unidadeDeTrabalho);
                SalvarVigencias(tabelaFrete, unidadeDeTrabalho);
                SalvarComponentesFrete(tabelaFrete, unidadeDeTrabalho);
                SalvarNumeroEntregas(tabelaFrete, unidadeDeTrabalho);
                SalvarPacotes(tabelaFrete, unidadeDeTrabalho);
                SalvarNumeroPallets(tabelaFrete, unidadeDeTrabalho);
                SalvarPesosTransportados(tabelaFrete, unidadeDeTrabalho);
                SalvarDistancias(tabelaFrete, unidadeDeTrabalho);
                SalvarSubcontratacoes(tabelaFrete, unidadeDeTrabalho);
                SalvarRotas(tabelaFrete, unidadeDeTrabalho);
                SalvarTempos(tabelaFrete, unidadeDeTrabalho);
                SalvarAjudantes(tabelaFrete, unidadeDeTrabalho);
                SalvarHoras(tabelaFrete, unidadeDeTrabalho);
                SalvarVeiculos(tabelaFrete, unidadeDeTrabalho);
                SalvarTabelaFreteRotasBidding(tabelaFrete.Codigo, unidadeDeTrabalho);
                AtualizarAprovacao(tabelaFrete, unidadeDeTrabalho);

                repTabelaFrete.Atualizar(tabelaFrete);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(new { tabelaFrete.Codigo });
            }
            catch (BaseException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                PreencherTabelaFrete(tabelaFrete, unidadeDeTrabalho);

                SalvarTipoEmbalagens(tabelaFrete, unidadeDeTrabalho);
                SalvarTiposOperacao(tabelaFrete, unidadeDeTrabalho);
                SalvarTiposOcorrencia(tabelaFrete, unidadeDeTrabalho);
                SalvarTransportadores(tabelaFrete, unidadeDeTrabalho);
                SalvarFiliais(tabelaFrete, unidadeDeTrabalho);
                SalvarTransportadoresTerceiros(tabelaFrete, unidadeDeTrabalho);
                SalvarTiposTerceiros(tabelaFrete, unidadeDeTrabalho);
                SalvarFronteiras(tabelaFrete, unidadeDeTrabalho);
                SalvarTiposCarga(tabelaFrete, unidadeDeTrabalho);
                SalvarModelosTracao(tabelaFrete, unidadeDeTrabalho);
                SalvarModelosReboque(tabelaFrete, unidadeDeTrabalho);
                SalvarVeiculos(tabelaFrete, unidadeDeTrabalho);
                SalvarContratos(tabelaFrete, unidadeDeTrabalho);

                ValidarTabelaFreteDuplicada(tabelaFrete, unidadeDeTrabalho);

                bool vigenciasAlteradas = SalvarVigencias(tabelaFrete, unidadeDeTrabalho);
                bool componentesAlterados = SalvarComponentesFrete(tabelaFrete, unidadeDeTrabalho);
                bool numeroEntregasAlterados = SalvarNumeroEntregas(tabelaFrete, unidadeDeTrabalho);
                bool numeroPacotesAlterados = SalvarPacotes(tabelaFrete, unidadeDeTrabalho);
                bool numeroPalletsAlterados = SalvarNumeroPallets(tabelaFrete, unidadeDeTrabalho);
                bool pesosTransportadosAlterados = SalvarPesosTransportados(tabelaFrete, unidadeDeTrabalho);
                bool distanciasAlteradas = SalvarDistancias(tabelaFrete, unidadeDeTrabalho);
                bool subcontratacoesAlteradas = SalvarSubcontratacoes(tabelaFrete, unidadeDeTrabalho);
                bool rotasAlteradas = SalvarRotas(tabelaFrete, unidadeDeTrabalho);
                bool temposAlterados = SalvarTempos(tabelaFrete, unidadeDeTrabalho);
                bool ajudantesAlterados = SalvarAjudantes(tabelaFrete, unidadeDeTrabalho);
                bool horasAlteradas = SalvarHoras(tabelaFrete, unidadeDeTrabalho);

                bool necessarioAtualizarAprovacao = (
                    vigenciasAlteradas ||
                    componentesAlterados ||
                    numeroEntregasAlterados ||
                    numeroPacotesAlterados ||
                    numeroPalletsAlterados ||
                    pesosTransportadosAlterados ||
                    distanciasAlteradas ||
                    subcontratacoesAlteradas ||
                    rotasAlteradas ||
                    temposAlterados ||
                    ajudantesAlterados ||
                    horasAlteradas ||
                    ExistemValoresAlteradosParaAprovacao(tabelaFrete) ||
                    tabelaFrete.SituacaoAlteracao == SituacaoAlteracaoTabelaFrete.SemRegraAprovacao
                );

                if (necessarioAtualizarAprovacao)
                    AtualizarAprovacao(tabelaFrete, unidadeDeTrabalho);

                repTabelaFrete.Atualizar(tabelaFrete, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoAtualizar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));

                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigoFetch(codigo);

                return new JsonpResult(ObterDetalhesTabelaFrete(tabelaFrete, false, unidadeDeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarParaDuplicar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));

                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigoFetch(codigo);

                return new JsonpResult(ObterDetalhesTabelaFrete(tabelaFrete, true, unidadeDeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));

                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigo, true);

                unidadeTrabalho.Start();

                repTabelaFrete.Deletar(tabelaFrete, Auditado);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoGerarOArquivoDeExportacaoDaPesquisa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoExportarAPesquisa);
            }
        }

        public async Task<IActionResult> ExportarVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                int codigoTabelaFrete = Request.GetIntParam("Codigo");
                string tipo = Request.GetStringParam("Tipo");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoVeiculo", false);
                grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Modelo Veicular Carga", "ModeloVeicularCarga", 12, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Capacidade KG", "CapacidadeKG", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Capacidade M3", "CapacidadeM3", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Transportador", "Empresa", 10, Models.Grid.Align.left);

                Repositorio.Embarcador.Frete.TabelaFreteVeiculo repositorioTabelaFreteVeiculo = new Repositorio.Embarcador.Frete.TabelaFreteVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo> tabelaFreteVeiculos = repositorioTabelaFreteVeiculo.BuscarPorTabelaETipoVeiculo(codigoTabelaFrete, tipo);

                int count = tabelaFreteVeiculos.Count();
                if (count == 0)
                    return new JsonpResult(false, true, "Nenhum registro encontrado!");

                var retorno = (
                     from o in tabelaFreteVeiculos
                     select new
                     {
                         Codigo = o.Codigo,
                         CodigoVeiculo = o.Veiculo.Codigo,
                         Placa = o.Veiculo.Placa,
                         ModeloVeicularCarga = o.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                         CapacidadeKG = o.Veiculo.CapacidadeKG.ToString("n0"),
                         CapacidadeM3 = o.Veiculo.CapacidadeM3.ToString("n0"),
                         Empresa = o.Veiculo?.Empresa?.RazaoSocial ?? string.Empty,
                     });

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(count);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", "Veículos." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar os Dados!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarSeExisteRegraCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cotacao.RegraCotacao repositorioRegraCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacao(unitOfWork);

                var retorno = new
                {
                    RegraCotacao = repositorioRegraCotacao.VerificarSeExisteRegraCotacao(),
                };

                return new JsonpResult(retorno);
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

        #endregion

        #region Métodos Privados

        private void PreencherTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrenciaCTE = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
            Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);

            int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoa);
            int.TryParse(Request.Params("ContratoFreteTransportador"), out int codigoContratoFreteTransportador);
            int.TryParse(Request.Params("ContratoFreteCliente"), out int codigoContratoFreteCliente);
            int.TryParse(Request.Params("DiasVencimentoAdiantamentoContratoFrete"), out int diasVencimentoAdiantamentoContratoFrete);
            int.TryParse(Request.Params("DiasVencimentoSaldoContratoFrete"), out int diasVencimentoSaldoContratoFrete);

            bool.TryParse(Request.Params("Padrao"), out bool padrao);
            bool.TryParse(Request.Params("TabelaCalculoCliente"), out bool tabelaCalculoCliente);
            bool.TryParse(Request.Params("PermiteValorAdicionalEntregaExcedente"), out bool permiteValorAdicionalEntregaExcedente);
            bool.TryParse(Request.Params("PermiteValorAdicionalPacoteExcedente"), out bool PermiteValorAdicionalPacoteExcedente);
            bool.TryParse(Request.Params("PermiteValorAdicionalQuilometragemExcedente"), out bool permiteValorAdicionalQuilometragemExcedente);
            bool.TryParse(Request.Params("PermiteValorAdicionalPesoExcedente"), out bool permiteValorAdicionalPesoExcedente);
            bool.TryParse(Request.Params("PermiteValorAdicionalPalletExcedente"), out bool permiteValorAdicionalPalletExcedente);
            bool.TryParse(Request.Params("TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas"), out bool tabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas);
            bool.TryParse(Request.Params("MultiplicarValorTempoPorHora"), out bool multiplicarValorTempoPorHora);
            bool.TryParse(Request.Params("PermiteValorAdicionalAjudanteExcedente"), out bool permiteValorAdicionalAjudanteExcedente);
            bool.TryParse(Request.Params("PermiteValorAdicionalHoraExcedente"), out bool permiteValorAdicionalHoraExcedente);
            bool.TryParse(Request.Params("PossuiHorasMinimasCobranca"), out bool possuiHorasMinimasCobranca);
            bool.TryParse(Request.Params("MultiplicarValorDaFaixa"), out bool multiplicarValorDaFaixa);
            bool.TryParse(Request.Params("NaoPermitirLancarValorPorTipoDeCarga"), out bool naoPermitirLancarValorPorTipoDeCarga);
            bool.TryParse(Request.Params("CalcularValorEntregaPorPercentualFrete"), out bool calcularValorEntregaPorPercentualFrete);
            bool.TryParse(Request.Params("PagamentoTerceiro"), out bool pagamentoTerceiro);
            bool utilizarArredondamentoHoras = Request.GetBoolParam("UtilizarArredondamentoHoras");
            bool utilizarMinutosInformadosComoCorteArredondamentoHoraExata = Request.GetBoolParam("UtilizarMinutosInformadosComoCorteArredondamentoHoraExata");
            bool obrigatorioValorFretePeso = Request.GetBoolParam("ObrigatorioValorFretePeso");
            bool utilizarParticipantePedidoParaCalculo = Request.GetBoolParam("UtilizarParticipantePedidoParaCalculo");
            bool utilizarModeloVeicularDaCargaParaCalculo = Request.GetBoolParam("UtilizarModeloVeicularDaCargaParaCalculo");
            bool naoConsiderarExpedidorERecebedor = Request.GetBoolParam("NaoConsiderarExpedidorERecebedor");
            bool usarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga = Request.GetBoolParam("UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga");
            bool reterImpostosContratoFrete = Request.GetBoolParam("ReterImpostosContratoFrete");
            bool tabelaFreteMinima = Request.GetBoolParam("TabelaFreteMinima");
            bool utilizaTabelaFreteMinima = Request.GetBoolParam("UtilizaTabelaFreteMinima");
            bool utilizaModeloVeicularVeiculo = Request.GetBoolParam("UtilizaModeloVeicularVeiculo");
            bool calcularFatorPesoPelaKM = Request.GetBoolParam("CalcularFatorPesoPelaKM");
            bool multiplicarValorPorPallet = Request.GetBoolParam("MultiplicarValorPorPallet");
            bool usarCubagemComoParametroDeDistancia = Request.GetBoolParam("UsarCubagemComoParametroDeDistancia");

            bool UsarCalculoFretePorPedido = Request.GetBoolParam("UsarCalculoFretePorPedido");
            bool utilizarPesoLiquido = Request.GetBoolParam("UtilizarPesoLiquido");
            bool calcularFretePorPesoCubado = Request.GetBoolParam("CalcularFretePorPesoCubado");
            bool aplicarMaiorValorEntrePesoEPesoCubado = Request.GetBoolParam("AplicarMaiorValorEntrePesoEPesoCubado");

            decimal.TryParse(Request.Params("PesoExcedente"), out decimal pesoExcedente);
            decimal.TryParse(Request.Params("FatorCubagem"), out decimal fatorCubagem);
            decimal.TryParse(Request.Params("IsencaoCubagem"), out decimal isencaoCubagem);
            decimal.TryParse(Request.Params("QuilometragemExcedente"), out decimal quilometragemExcedente);

            int.TryParse(Request.Params("ComponenteFreteQuilometragemExcedente"), out int componenteFreteQuilometragemExcedente);
            int.TryParse(Request.Params("ComponenteFreteQuilometragem"), out int componenteFreteQuilometragem);
            int.TryParse(Request.Params("ComponenteFretePeso"), out int componenteFretePeso);
            int.TryParse(Request.Params("ComponenteFreteAjudante"), out int componenteFreteAjudante);
            int.TryParse(Request.Params("componenteFreteDestacar"), out int componenteFreteDestacar);
            int.TryParse(Request.Params("ComponenteFreteHora"), out int componenteFreteHora);
            int.TryParse(Request.Params("ComponenteFreteTempo"), out int componenteFreteTempo);
            int.TryParse(Request.Params("ComponenteFretePallet"), out int componenteFretePallet);
            int.TryParse(Request.Params("ComponenteFreteNumeroEntregas"), out int componenteFreteNumeroEntregas);
            int.TryParse(Request.Params("ComponenteFretePacotes"), out int componenteFretePacotes);
            int minutosArredondamentoHoras = Request.GetIntParam("MinutosArredondamentoHoras");

            Enum.TryParse(Request.Params("TipoCalculo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete tipoCalculo);
            Enum.TryParse(Request.Params("PesoParametroCalculoFrete"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete pesoParametroCalculoFrete);
            LocalFreeTime localFreeTime = Request.GetEnumParam<LocalFreeTime>("LocalFreeTime");

            string observacaoTerceiro = Request.Params("ObservacaoTerceiro");
            string observacaoContratoFrete = Request.Params("ObservacaoContratoFrete");
            string textoAdicionalContratoFrete = Request.Params("TextoAdicionalContratoFrete");
            string horasMinimasCobranca = Request.Params("HorasMinimasCobranca");
            bool emitirOcorrenciaAutomatica = Request.GetBoolParam("EmitirOcorrenciaAutomatica");
            int tipoOcorrenciaTabelaMinima = Request.GetIntParam("TipoOcorrenciaTabelaMinima");

            int canalEntrega = Request.GetIntParam("CanalEntrega");

            //if (codigoContratoFreteTransportador > 0)
            //{
            //    List<string> operacoesInvalidas = (from o in tabelaFrete.TiposOperacao where !o.PermiteUtilizarEmContratoFrete select o.Descricao).ToList();
            //    if (operacoesInvalidas.Count > 0)
            //        throw new ControllerException($"Os tipos de operações {String.Join(", ", operacoesInvalidas)} não são permitidos quando há um Contrato de Frete.");
            //}

            if (tabelaFrete.Codigo == 0 && tabelaFreteMinima && utilizaTabelaFreteMinima)
                throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.TabelaDeFreteMinimaNaoPodeEstarConfiguradaParaUtilizarTabelaMinima);

            tabelaFrete.DiasVencimentoAdiantamentoContratoFrete = diasVencimentoAdiantamentoContratoFrete;
            tabelaFrete.DiasVencimentoSaldoContratoFrete = diasVencimentoSaldoContratoFrete;
            tabelaFrete.ReterImpostosContratoFrete = reterImpostosContratoFrete;
            tabelaFrete.TextoAdicionalContratoFrete = textoAdicionalContratoFrete;
            tabelaFrete.ObservacaoContratoFrete = observacaoContratoFrete;
            tabelaFrete.UtilizarParticipantePedidoParaCalculo = utilizarParticipantePedidoParaCalculo;
            tabelaFrete.UtilizarModeloVeicularDaCargaParaCalculo = utilizarModeloVeicularDaCargaParaCalculo;
            tabelaFrete.NaoConsiderarExpedidorERecebedor = naoConsiderarExpedidorERecebedor;
            tabelaFrete.UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga = usarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga;
            tabelaFrete.PagamentoTerceiro = pagamentoTerceiro;
            tabelaFrete.Padrao = padrao;
            tabelaFrete.TabelaCalculoCliente = tabelaCalculoCliente;
            tabelaFrete.Ativo = bool.Parse(Request.Params("Ativo"));
            tabelaFrete.Descricao = Request.Params("Descricao");
            tabelaFrete.TipoTabelaFrete = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete)int.Parse(Request.Params("TipoTabelaFrete"));
            tabelaFrete.AplicacaoTabela = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela)int.Parse(Request.Params("AplicacaoTabela"));
            tabelaFrete.TipoFreteTabelaFrete = Request.GetEnumParam<TipoFreteTabelaFrete>("TipoFreteTabelaFrete");
            tabelaFrete.CodigoIntegracao = Request.Params("CodigoIntegracao");
            tabelaFrete.GrupoPessoas = codigoGrupoPessoa > 0 ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoPessoa } : null;
            tabelaFrete.ContratoFreteTransportador = codigoContratoFreteTransportador > 0 ? repContratoFreteTransportador.BuscarPorCodigo(codigoContratoFreteTransportador) : null;
            tabelaFrete.ContratoFreteCliente = codigoContratoFreteCliente > 0 ? repContratoFreteCliente.BuscarPorCodigo(codigoContratoFreteCliente, false) : null;
            tabelaFrete.ImprimirObservacaoCTe = bool.Parse(Request.Params("ImprimirObservacaoCTe"));
            tabelaFrete.PossuiMinimoGarantido = bool.Parse(Request.Params("PossuiMinimoGarantido"));
            tabelaFrete.PossuiValorMaximo = bool.Parse(Request.Params("PossuiValorMaximo"));
            tabelaFrete.PossuiValorBase = bool.Parse(Request.Params("PossuiValorBase"));
            tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos = bool.Parse(Request.Params("UtilizarDiferencaDoValorBaseApenasFretePagos"));
            tabelaFrete.ValorMinimoDiferencaFreteNegativo = Request.GetDecimalParam("ValorMinimoDiferencaFreteNegativo");
            tabelaFrete.IncluirICMSValorFrete = bool.Parse(Request.Params("IncluirICMSValorFrete"));
            tabelaFrete.CalcularFreteDestinoPrioritario = Request.GetBoolParam("CalcularFreteDestinoPrioritario");
            tabelaFrete.ValorParametroBaseObrigatorioParaCalculo = Request.GetBoolParam("ValorParametroBaseObrigatorioParaCalculo");
            tabelaFrete.EmissaoAutomaticaCTe = bool.Parse(Request.Params("EmissaoAutomaticaCTe"));
            tabelaFrete.TipoCalculo = tipoCalculo;
            tabelaFrete.Observacao = Request.Params("Observacao");
            tabelaFrete.ObservacaoTerceiro = observacaoTerceiro;
            tabelaFrete.PercentualICMSIncluir = decimal.Parse(Request.Params("PercentualICMSIncluir"));
            tabelaFrete.PermiteAlterarValor = bool.Parse(Request.Params("PermiteAlterarValor"));
            tabelaFrete.ValidarPorDataCarregamento = Request.GetBoolParam("ValidarPorDataCarregamento");
            tabelaFrete.PermitirVigenciasSobrepostas = Request.GetBoolParam("PermitirVigenciasSobrepostas");
            tabelaFrete.UsarComoDataBaseVigenciaDataAtual = Request.GetBoolParam("UsarComoDataBaseVigenciaDataAtual");
            tabelaFrete.TipoAlteracaoValor = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlteracaoValorTabelaFrete)int.Parse(Request.Params("TipoAlteracaoValor"));
            tabelaFrete.PermiteValorAdicionalPorEntregaExcedente = permiteValorAdicionalEntregaExcedente;
            tabelaFrete.PermiteValorAdicionalPorPacoteExcedente = PermiteValorAdicionalPacoteExcedente;
            tabelaFrete.PermiteValorAdicionalPorPalletExcedente = permiteValorAdicionalPalletExcedente;
            tabelaFrete.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas = tabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas;
            tabelaFrete.PermiteValorAdicionalPorPesoExcedente = permiteValorAdicionalPesoExcedente;
            tabelaFrete.PermiteValorAdicionalPorQuilometragemExcedente = permiteValorAdicionalQuilometragemExcedente;
            tabelaFrete.MultiplicarValorTempoPorHora = multiplicarValorTempoPorHora;
            tabelaFrete.PermiteValorAdicionalPorAjudanteExcedente = permiteValorAdicionalAjudanteExcedente;
            tabelaFrete.PermiteValorAdicionalPorHoraExcedente = permiteValorAdicionalHoraExcedente;
            tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida = Request.GetBoolParam("MultiplicarValorFaixaHoraPelaHoraCorrida");
            tabelaFrete.TipoArredondamentoHoras = Request.GetEnumParam<TipoArredondamentoTabelaFrete>("TipoArredondamentoHoras");
            tabelaFrete.CalcularComTodasFaixasHora = Request.GetBoolParam("CalcularComTodasFaixasHora");
            tabelaFrete.PossuiHorasMinimasCobranca = possuiHorasMinimasCobranca;
            tabelaFrete.MultiplicarValorDaFaixa = multiplicarValorDaFaixa;
            tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga = naoPermitirLancarValorPorTipoDeCarga;
            tabelaFrete.ObrigatorioValorFretePeso = obrigatorioValorFretePeso;
            tabelaFrete.TabelaFreteMinima = tabelaFreteMinima;
            tabelaFrete.UtilizaTabelaFreteMinima = utilizaTabelaFreteMinima;
            tabelaFrete.UtilizaModeloVeicularVeiculo = utilizaModeloVeicularVeiculo;
            tabelaFrete.CalcularFatorPesoPelaKM = calcularFatorPesoPelaKM;
            tabelaFrete.MultiplicarValorPorPallet = multiplicarValorPorPallet;
            tabelaFrete.LocalFreeTime = localFreeTime;
            tabelaFrete.UsarCubagemComoParametroDeDistancia = usarCubagemComoParametroDeDistancia;
            tabelaFrete.UsarCalculoFretePorPedido = UsarCalculoFretePorPedido;
            tabelaFrete.UtilizarValorMinimoParaRateio = Request.GetBoolParam("UtilizarValorMinimoParaRateio");
            tabelaFrete.AgrupaPorRecebedorAoCalcularPorPedidoAgrupado = Request.GetBoolParam("AgrupaPorRecebedorAoCalcularPorPedidoAgrupado");
            tabelaFrete.ValorMinimoParaRateio = Request.GetDecimalParam("ValorMinimoParaRateio");
            tabelaFrete.UtilizarPesoLiquido = utilizarPesoLiquido;
            tabelaFrete.CalcularFretePorPesoCubado = calcularFretePorPesoCubado;
            tabelaFrete.PesoParametroCalculoFrete = pesoParametroCalculoFrete;
            tabelaFrete.CalcularValorEntregaPorPercentualFrete = calcularValorEntregaPorPercentualFrete;
            tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes = Request.GetBoolParam("CalcularValorEntregaPorPercentualFreteComComponentes");
            tabelaFrete.CalcularQuantidadeEntregaPorParticipantesPedido = Request.GetBoolParam("CalcularQuantidadeEntregaPorParticipantesPedido");
            tabelaFrete.CalcularQuantidadeEntregaPorNumeroDePedidos = Request.GetBoolParam("CalcularQuantidadeEntregaPorNumeroDePedidos");
            tabelaFrete.PermiteInformarDiasUteisPorFaixaCEP = Request.GetBoolParam("PermiteInformarDiasUteisPorFaixaCEP");
            tabelaFrete.ObrigatorioInformarTerceiro = Request.GetBoolParam("ObrigatorioInformarTerceiro");
            tabelaFrete.ComponenteFreteQuilometragemExcedente = componenteFreteQuilometragemExcedente > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFreteQuilometragemExcedente) : null;
            tabelaFrete.ComponenteFreteQuilometragem = componenteFreteQuilometragem > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFreteQuilometragem) : null;
            tabelaFrete.ComponenteFretePeso = componenteFretePeso > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFretePeso) : null;
            tabelaFrete.ComponenteFreteAjudante = componenteFreteAjudante > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFreteAjudante) : null;
            tabelaFrete.ComponenteFreteHora = componenteFreteHora > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFreteHora) : null;
            tabelaFrete.ComponenteFreteTempo = componenteFreteTempo > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFreteTempo) : null;
            tabelaFrete.ComponenteFretePallet = componenteFretePallet > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFretePallet) : null;
            tabelaFrete.ComponenteFreteNumeroEntregas = componenteFreteNumeroEntregas > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFreteNumeroEntregas) : null;
            tabelaFrete.ComponenteFretePacotes = componenteFretePacotes > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFretePacotes) : null;
            tabelaFrete.EmitirOcorrenciaAutomatica = emitirOcorrenciaAutomatica;
            tabelaFrete.TipoOcorrenciaTabelaMinima = tipoOcorrenciaTabelaMinima > 0 ? repTipoOcorrenciaCTE.BuscarPorCodigo(tipoOcorrenciaTabelaMinima) : null;
            tabelaFrete.CanalEntrega = repCanalEntrega.BuscarPorCodigo(canalEntrega);

            bool destacarComponenteFrete = Request.GetBoolParam("DestacarComponenteFrete");
            tabelaFrete.DestacarComponenteFrete = destacarComponenteFrete;
            tabelaFrete.ComponenteFreteDestacar = destacarComponenteFrete ? repComponenteFrete.BuscarPorCodigo(componenteFreteDestacar) : null;
            tabelaFrete.NaoSomarValorTotalAReceber = destacarComponenteFrete && Request.GetBoolParam("NaoSomarValorTotalAReceber");
            tabelaFrete.NaoSomarValorTotalPrestacao = destacarComponenteFrete && Request.GetBoolParam("NaoSomarValorTotalPrestacao");
            tabelaFrete.DescontarComponenteFreteLiquido = destacarComponenteFrete && Request.GetBoolParam("DescontarComponenteFreteLiquido");
            tabelaFrete.DescontarDoValorAReceberOICMSDoComponente = destacarComponenteFrete && Request.GetBoolParam("DescontarDoValorAReceberOICMSDoComponente");
            tabelaFrete.DescontarDoValorAReceberValorComponente = destacarComponenteFrete && Request.GetBoolParam("DescontarDoValorAReceberValorComponente");
            tabelaFrete.NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS = destacarComponenteFrete && Request.GetBoolParam("NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS");
            tabelaFrete.UtilizarValorDaTabelaMesmoInformandoUmValorDeFreteOperador = Request.GetBoolParam("UtilizarValorDaTabelaMesmoInformandoUmValorDeFreteOperador");

            if (possuiHorasMinimasCobranca)
                tabelaFrete.HorasMinimasCobranca = TimeSpan.ParseExact(horasMinimasCobranca, @"hh\:mm", null, System.Globalization.TimeSpanStyles.None);
            else
                tabelaFrete.HorasMinimasCobranca = null;

            if (permiteValorAdicionalQuilometragemExcedente)
                tabelaFrete.QuilometragemExcedente = quilometragemExcedente;
            else
                tabelaFrete.QuilometragemExcedente = 0m;

            if (permiteValorAdicionalPesoExcedente)
                tabelaFrete.PesoExcecente = pesoExcedente;
            else
                tabelaFrete.PesoExcecente = 0m;

            if (calcularFretePorPesoCubado)
            {
                tabelaFrete.FatorCubagem = fatorCubagem;
                tabelaFrete.IsencaoCubagem = isencaoCubagem;
                tabelaFrete.AplicarMaiorValorEntrePesoEPesoCubado = aplicarMaiorValorEntrePesoEPesoCubado;
            }
            else
            {
                tabelaFrete.FatorCubagem = 0m;
                tabelaFrete.IsencaoCubagem = 0m;
                tabelaFrete.AplicarMaiorValorEntrePesoEPesoCubado = false;
            }

            tabelaFrete.UtilizarArredondamentoHoras = utilizarArredondamentoHoras;
            tabelaFrete.UtilizarMinutosInformadosComoCorteArredondamentoHoraExata = utilizarMinutosInformadosComoCorteArredondamentoHoraExata;
            tabelaFrete.UtilizarTipoOperacaoPedido = Request.GetBoolParam("UtilizarTipoOperacaoPedido");
            tabelaFrete.NaoCalculaSemFreteValor = Request.GetBoolParam("NaoCalculaSemFreteValor");
            tabelaFrete.IncluirICMSValorFreteNaCarga = Request.GetBoolParam("IncluirICMSValorFreteNaCarga");
            tabelaFrete.NaoDestacarResultadoConsultaPedagioComoComponente = Request.GetBoolParam("NaoDestacarResultadoConsultaPedagioComoComponente");
            tabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete = Request.GetBoolParam("PermiteAlterarValorFretePedidoPosCalculoFrete");
            tabelaFrete.NaoUsarCanalEntregaComoFiltroParaCotacao = Request.GetBoolParam("NaoUsarCanalEntregaComoFiltroParaCotacao");
            tabelaFrete.DescontoCubagemCalculoFrete = Request.GetDecimalParam("DescontoCubagemCalculoFrete");

            if (utilizarArredondamentoHoras)
                tabelaFrete.MinutosArredondamentoHoras = minutosArredondamentoHoras;
            else
                tabelaFrete.MinutosArredondamentoHoras = null;

            decimal percentualCobrancaPadrao = 0;
            if (!string.IsNullOrWhiteSpace(Request.Params("PercentualCobrancaPadrao")))
                decimal.TryParse(Request.Params("PercentualCobrancaPadrao"), out percentualCobrancaPadrao);

            if (percentualCobrancaPadrao > 100)
                throw new CustomException("Conferir o valor 'Cobrança Padrão'");

            tabelaFrete.PercentualCobrancaPadraoTerceiros = percentualCobrancaPadrao;


            decimal percentualCobrancaVeiculoFrota = 0;
            if (!string.IsNullOrWhiteSpace(Request.Params("PercentualCobrancaVeiculoFrota")))
                decimal.TryParse(Request.Params("PercentualCobrancaVeiculoFrota"), out percentualCobrancaVeiculoFrota);

            if (percentualCobrancaVeiculoFrota > 100)
                throw new CustomException("Conferir o valor 'Cobrança Veículo Frota'");

            tabelaFrete.PercentualCobrancaVeiculoFrotaTerceiros = percentualCobrancaVeiculoFrota;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete parametroBase;

            if (Enum.TryParse(Request.Params("ParametroBase"), false, out parametroBase))
                tabelaFrete.ParametroBase = parametroBase;
            else
                tabelaFrete.ParametroBase = null;
        }

        private void AtualizarAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho).ExistePorTipo(TipoIntegracao.LBC))
                return;

            Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada servicoAprovacaoAlcada = new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(unidadeDeTrabalho);

            servicoAprovacaoAlcada.AtualizarAprovacao(tabelaFrete, TipoServicoMultisoftware);

            if (tabelaFrete.SituacaoAlteracao.IsAlteracaoTabelaFreteClienteLiberada())
            {
                Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);

                servicoTabelaFreteCliente.AdicionarComponentesFrete(tabelaFrete);
            }
        }

        private bool ExistemValoresAlteradosParaAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = tabelaFrete.GetCurrentChanges();

            valoresAlterados.RemoveAll(o => o.Propriedade == "Transportadores");
            valoresAlterados.RemoveAll(o => o.Propriedade == "Filiais");
            valoresAlterados.RemoveAll(o => o.Propriedade == "TiposOperacao");
            valoresAlterados.RemoveAll(o => o.Propriedade == "TiposCarga");
            valoresAlterados.RemoveAll(o => o.Propriedade == "ModelosTracao");
            valoresAlterados.RemoveAll(o => o.Propriedade == "ModelosReboque");

            return valoresAlterados.Count > 0;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");
                SituacaoAtivoPesquisa ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo);
                bool exibirColunaSituacaoAprovacao = configuracaoEmbarcador.UtilizarAlcadaAprovacaoTabelaFrete && !configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente;
                bool exibirColunaSituacaoAjuste = configuracaoEmbarcador.ExibirSituacaoAjusteTabelaFrete;
                int tamanhoAdicionalColuna = 4;

                if (exibirColunaSituacaoAprovacao)
                    tamanhoAdicionalColuna -= 2;

                if (exibirColunaSituacaoAjuste)
                    tamanhoAdicionalColuna -= 2;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete tipoTabela = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete)int.Parse(Request.Params("TipoTabelaFrete"));
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela aplicacaoTabela = AplicacaoTabela.Todas;
                if (!string.IsNullOrWhiteSpace(Request.Params("AplicacaoTabela")))
                    aplicacaoTabela = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela)int.Parse(Request.Params("AplicacaoTabela"));
                SituacaoAlteracaoTabelaFrete? situacaoAlteracao = Request.GetNullableEnumParam<SituacaoAlteracaoTabelaFrete>("SituacaoAlteracao");

                int codigoGrupoPessoas = 0;
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);

                int empresa = 0;
                int.TryParse(Request.Params("Empresa"), out empresa);

                int tipoCarga = 0;
                int.TryParse(Request.Params("TipoCarga"), out tipoCarga);

                int filial = 0;
                int.TryParse(Request.Params("Filial"), out filial);

                int tipoOperacao = 0;
                int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);

                int tipoOcorrencia = 0;
                int.TryParse(Request.Params("TipoOcorrencia"), out tipoOcorrencia);

                int veiculo = Request.GetIntParam("Veiculo");
                bool calcularFreteDestinoPrioritario = Request.GetBoolParam("CalcularFreteDestinoPrioritario");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoCalculo", false);
                grid.AdicionarCabecalho("PermitirIntegrarAlteracao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TabelaFrete.CodigoDeIntegracao, "CodigoIntegracao", (12 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TabelaFrete.Descricao, "Descricao", (30 + (int)(tamanhoAdicionalColuna * 2.5)), Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TabelaFrete.TipoDeTabela, "DescricaoTipoTabelaFrete", (10 + tamanhoAdicionalColuna), Models.Grid.Align.left, false);

                if (exibirColunaSituacaoAprovacao)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.TabelaFrete.SituacaoDaAprovacao, "DescricaoSituacaoAprovacao", 14, Models.Grid.Align.center, false);

                if (exibirColunaSituacaoAjuste)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.TabelaFrete.SituacaoDoAjuste, "DescricaoSituacaoAjusteTabelaFrete", 12, Models.Grid.Align.center, false);

                if (configuracaoTabelaFrete.UtilizarIntegracaoAlteracaoTabelaFrete)
                    grid.AdicionarCabecalho("Situacao da Integração", "DescricaoSituacaoIntegracaoAlteracao", (10 + tamanhoAdicionalColuna), Models.Grid.Align.center, false);

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", (10 + tamanhoAdicionalColuna), Models.Grid.Align.center, false);

                Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
                int totalRegistros = repositorioTabelaFrete.ContarConsulta(descricao, ativo, tipoTabela, codigoIntegracao, codigoGrupoPessoas, calcularFreteDestinoPrioritario, aplicacaoTabela, empresa, tipoCarga, filial, tipoOperacao, tipoOcorrencia, situacaoAlteracao, veiculo);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> listaTabelaFrete = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ajusteTabelaFretes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();
                IList<(int CodigoTabelaFrete, SituacaoTabelaFreteIntegrarAlteracao Situacao)> situacoesIntegracaoAlteracao = new List<(int CodigoTabelaFrete, SituacaoTabelaFreteIntegrarAlteracao Situacao)>();

                if (totalRegistros > 0)
                {
                    listaTabelaFrete = repositorioTabelaFrete.Consultar(descricao, ativo, tipoTabela, codigoIntegracao, codigoGrupoPessoas, calcularFreteDestinoPrioritario, aplicacaoTabela, empresa, tipoCarga, filial, tipoOperacao, tipoOcorrencia, situacaoAlteracao, veiculo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                    List<int> codigosTabelaFrete = listaTabelaFrete.Select(tabelaFrete => tabelaFrete.Codigo).ToList();

                    if (exibirColunaSituacaoAjuste)
                    {
                        Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unidadeDeTrabalho);
                        ajusteTabelaFretes = repositorioAjusteTabelaFrete.ObterPorTabelasFrete((from obj in listaTabelaFrete select obj.Codigo).ToList());
                    }

                    if (configuracaoTabelaFrete.UtilizarIntegracaoAlteracaoTabelaFrete)
                    {
                        Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao repositorioTabelaFreteIntegrarAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracao(unidadeDeTrabalho);
                        situacoesIntegracaoAlteracao = repositorioTabelaFreteIntegrarAlteracao.BuscarSituacoesIntegracaoAlteracao(codigosTabelaFrete);
                    }
                }

                List<dynamic> listaTabelaFreteRetornar = (
                    from tabelaFrete in listaTabelaFrete
                    select ObterTabelaFreteRetornar(tabelaFrete, ajusteTabelaFretes, situacoesIntegracaoAlteracao)
                ).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaTabelaFreteRetornar);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        private dynamic ObterDetalhesTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool dadosParaDuplicar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigenciaTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.VigenciaTabelaFreteAnexo repVigenciaTabelaFreteAnexo = new Repositorio.Embarcador.Frete.VigenciaTabelaFreteAnexo(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFreteTempo repComponenteFreteTabelaFreteTempo = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFreteTempo(unitOfWork);
            Repositorio.Embarcador.Frete.PesoTabelaFrete repPesoTabelaFrete = new Repositorio.Embarcador.Frete.PesoTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete repRotaEmbarcadorTabelaFrete = new Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.SubcontratacaoTabelaFrete repSubcontratacaoTabelaFrete = new Repositorio.Embarcador.Frete.SubcontratacaoTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteVeiculo repositorioTabelaFreteVeiculo = new Repositorio.Embarcador.Frete.TabelaFreteVeiculo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> vigenciaTabelaFretes = repVigenciaTabelaFrete.BuscarPorTabela(tabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFreteAnexo> vigenciaTabelaFretesAnexo = repVigenciaTabelaFreteAnexo.BuscarPorTabela(tabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> componentesFreteTabelaFrete = repComponenteFreteTabelaFrete.BuscarPorTabelaFrete(tabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFreteTempo> componentesFreteTabelaFretesTempo = repComponenteFreteTabelaFreteTempo.BuscarPorTabelaFrete(tabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete> pesosTabelaFrete = repPesoTabelaFrete.BuscarPorTabelaFrete(tabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete> rotasEmbarcadorTabelaFrete = repRotaEmbarcadorTabelaFrete.BuscarPorTabelaFrete(tabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete> subcontratacaosTabelaFrete = repSubcontratacaoTabelaFrete.BuscarPorTabelaFrete(tabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo> tabelaFreteVeiculos = repositorioTabelaFreteVeiculo.BuscarPorTabela(tabelaFrete.Codigo);

            return new
            {
                tabelaFrete.NaoCalculaSemFreteValor,
                tabelaFrete.IncluirICMSValorFreteNaCarga,
                tabelaFrete.NaoDestacarResultadoConsultaPedagioComoComponente,
                tabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete,
                tabelaFrete.NaoUsarCanalEntregaComoFiltroParaCotacao,
                tabelaFrete.UtilizarTipoOperacaoPedido,
                tabelaFrete.TabelaCalculoCliente,
                tabelaFrete.DiasVencimentoAdiantamentoContratoFrete,
                tabelaFrete.DiasVencimentoSaldoContratoFrete,
                tabelaFrete.TextoAdicionalContratoFrete,
                tabelaFrete.ReterImpostosContratoFrete,
                tabelaFrete.ObservacaoContratoFrete,
                tabelaFrete.UtilizarParticipantePedidoParaCalculo,
                tabelaFrete.UtilizarModeloVeicularDaCargaParaCalculo,
                tabelaFrete.NaoConsiderarExpedidorERecebedor,
                tabelaFrete.UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga,
                tabelaFrete.PagamentoTerceiro,
                tabelaFrete.ObrigatorioInformarTerceiro,
                tabelaFrete.MultiplicarValorDaFaixa,
                tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga,
                Padrao = dadosParaDuplicar ? false : tabelaFrete.Padrao,
                Ativo = dadosParaDuplicar ? true : tabelaFrete.Ativo,
                Codigo = dadosParaDuplicar ? 0 : tabelaFrete.Codigo,
                tabelaFrete.CodigoIntegracao,
                tabelaFrete.Descricao,
                tabelaFrete.DescricaoAtivo,
                tabelaFrete.DescricaoTipoTabelaFrete,
                DescricaoAplicacaoTabela = tabelaFrete.AplicacaoTabela.ObterDescricao(),
                tabelaFrete.EmissaoAutomaticaCTe,
                tabelaFrete.TipoCalculo,
                CanalEntrega = new
                {
                    tabelaFrete.CanalEntrega?.Descricao,
                    tabelaFrete.CanalEntrega?.Codigo,
                },

                ContratoFreteTransportadorPossuiFranquia = ((tabelaFrete.ContratoFreteTransportador?.FranquiaContratoMensal ?? 0) > 0),
                GrupoPessoas = new
                {
                    tabelaFrete.GrupoPessoas?.Descricao,
                    tabelaFrete.GrupoPessoas?.Codigo
                },
                TipoOperacao = new
                {
                    Descricao = "", //tabelaFrete.TipoOperacao?.Descricao,
                    Codigo = 0 //tabelaFrete.TipoOperacao?.Codigo
                },
                ContratoFreteTransportador = new
                {
                    tabelaFrete.ContratoFreteTransportador?.Descricao,
                    tabelaFrete.ContratoFreteTransportador?.Codigo,
                },
                ContratoFreteCliente = new
                {
                    tabelaFrete.ContratoFreteCliente?.Descricao,
                    tabelaFrete.ContratoFreteCliente?.Codigo,
                },
                tabelaFrete.ImprimirObservacaoCTe,
                tabelaFrete.PossuiValorMaximo,
                tabelaFrete.PossuiValorBase,
                tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos,
                ValorMinimoDiferencaFreteNegativo = tabelaFrete.ValorMinimoDiferencaFreteNegativo.ToString("n2"),
                tabelaFrete.PossuiMinimoGarantido,
                tabelaFrete.IncluirICMSValorFrete,
                tabelaFrete.CalcularFreteDestinoPrioritario,
                tabelaFrete.ValorParametroBaseObrigatorioParaCalculo,
                tabelaFrete.Observacao,
                tabelaFrete.ObservacaoTerceiro,
                PercentualICMSIncluir = tabelaFrete.PercentualICMSIncluir.ToString("n2"),
                tabelaFrete.PermiteAlterarValor,
                tabelaFrete.TipoAlteracaoValor,
                tabelaFrete.TipoTabelaFrete,
                tabelaFrete.AplicacaoTabela,
                tabelaFrete.TipoFreteTabelaFrete,
                tabelaFrete.UtilizarArredondamentoHoras,
                tabelaFrete.UtilizarMinutosInformadosComoCorteArredondamentoHoraExata,
                tabelaFrete.UtilizarValorDaTabelaMesmoInformandoUmValorDeFreteOperador,
                MinutosArredondamentoHoras = tabelaFrete.MinutosArredondamentoHoras ?? 0,

                ComponenteFreteQuilometragemExcedente = tabelaFrete.ComponenteFreteQuilometragemExcedente != null ? new { tabelaFrete.ComponenteFreteQuilometragemExcedente.Codigo, tabelaFrete.ComponenteFreteQuilometragemExcedente.Descricao } : null,
                UtilizarComponenteFreteQuilometragemExcedente = tabelaFrete.ComponenteFreteQuilometragemExcedente != null,

                ComponenteFreteQuilometragem = tabelaFrete.ComponenteFreteQuilometragem != null ? new { tabelaFrete.ComponenteFreteQuilometragem.Codigo, tabelaFrete.ComponenteFreteQuilometragem.Descricao } : null,
                UtilizarComponenteFreteQuilometragem = tabelaFrete.ComponenteFreteQuilometragem != null,

                ComponenteFretePeso = tabelaFrete.ComponenteFretePeso != null ? new { tabelaFrete.ComponenteFretePeso.Codigo, tabelaFrete.ComponenteFretePeso.Descricao } : null,
                UtilizarComponenteFretePeso = tabelaFrete.ComponenteFretePeso != null,
                tabelaFrete.ObrigatorioValorFretePeso,
                ComponenteFreteHora = tabelaFrete.ComponenteFreteHora != null ? new { tabelaFrete.ComponenteFreteHora.Codigo, tabelaFrete.ComponenteFreteHora.Descricao } : null,
                UtilizarComponenteFreteHora = tabelaFrete.ComponenteFreteHora != null,
                ComponenteFreteAjudante = tabelaFrete.ComponenteFreteAjudante != null ? new { tabelaFrete.ComponenteFreteAjudante.Codigo, tabelaFrete.ComponenteFreteAjudante.Descricao } : null,
                UtilizarComponenteFreteAjudante = tabelaFrete.ComponenteFreteAjudante != null,
                ComponenteFreteTempo = tabelaFrete.ComponenteFreteTempo != null ? new { tabelaFrete.ComponenteFreteTempo.Codigo, tabelaFrete.ComponenteFreteTempo.Descricao } : null,
                UtilizarComponenteFreteTempo = tabelaFrete.ComponenteFreteTempo != null,
                ComponenteFretePallet = tabelaFrete.ComponenteFretePallet != null ? new { tabelaFrete.ComponenteFretePallet.Codigo, tabelaFrete.ComponenteFretePallet.Descricao } : null,
                UtilizarComponenteFretePallet = tabelaFrete.ComponenteFretePallet != null,
                ComponenteFreteNumeroEntregas = tabelaFrete.ComponenteFreteNumeroEntregas != null ? new { tabelaFrete.ComponenteFreteNumeroEntregas.Codigo, tabelaFrete.ComponenteFreteNumeroEntregas.Descricao } : null,
                UtilizarComponenteFreteNumeroEntregas = tabelaFrete.ComponenteFreteNumeroEntregas != null,
                ComponenteFretePacotes = tabelaFrete.ComponenteFretePacotes != null ? new { tabelaFrete.ComponenteFretePacotes.Codigo, tabelaFrete.ComponenteFretePacotes.Descricao } : null,
                UtilizarComponenteFretePacotes = tabelaFrete.ComponenteFretePacotes != null,

                DestacarComponenteFrete = tabelaFrete.ComponenteFreteDestacar != null,
                ComponenteFreteDestacar = tabelaFrete.ComponenteFreteDestacar != null ? new { tabelaFrete.ComponenteFreteDestacar.Codigo, tabelaFrete.ComponenteFreteDestacar.Descricao } : null,
                tabelaFrete.NaoSomarValorTotalAReceber,
                tabelaFrete.NaoSomarValorTotalPrestacao,
                tabelaFrete.DescontarComponenteFreteLiquido,
                tabelaFrete.DescontarDoValorAReceberOICMSDoComponente,
                tabelaFrete.DescontarDoValorAReceberValorComponente,
                tabelaFrete.NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS,
                tabelaFrete.TabelaFreteMinima,
                tabelaFrete.EmitirOcorrenciaAutomatica,
                TipoOcorrenciaTabelaMinima = new
                {
                    Codigo = tabelaFrete.TipoOcorrenciaTabelaMinima?.Codigo ?? 0,
                    Descricao = tabelaFrete.TipoOcorrenciaTabelaMinima?.Descricao ?? string.Empty,

                },

                tabelaFrete.UtilizaTabelaFreteMinima,
                tabelaFrete.UtilizaModeloVeicularVeiculo,
                tabelaFrete.CalcularFatorPesoPelaKM,
                tabelaFrete.UtilizarPesoLiquido,
                tabelaFrete.PesoParametroCalculoFrete,
                tabelaFrete.MultiplicarValorPorPallet,
                tabelaFrete.LocalFreeTime,
                tabelaFrete.UsarCubagemComoParametroDeDistancia,
                tabelaFrete.UsarCalculoFretePorPedido,
                tabelaFrete.UtilizarValorMinimoParaRateio,
                tabelaFrete.AgrupaPorRecebedorAoCalcularPorPedidoAgrupado,
                tabelaFrete.ValorMinimoParaRateio,
                tabelaFrete.CalcularQuantidadeEntregaPorParticipantesPedido,
                tabelaFrete.CalcularQuantidadeEntregaPorNumeroDePedidos,
                tabelaFrete.PermiteInformarDiasUteisPorFaixaCEP,
                tabelaFrete.CalcularFretePorPesoCubado,
                tabelaFrete.AplicarMaiorValorEntrePesoEPesoCubado,
                FatorCubagem = tabelaFrete.FatorCubagem.ToString("n3"),
                IsencaoCubagem = tabelaFrete.IsencaoCubagem.ToString("n3"),
                DescontoCubagemCalculoFrete = tabelaFrete.DescontoCubagemCalculoFrete > 0 ? tabelaFrete.DescontoCubagemCalculoFrete.ToString("n2") : "",
                PercentualCobrancaPadrao = tabelaFrete.PercentualCobrancaPadraoTerceiros > 0 ? tabelaFrete.PercentualCobrancaPadraoTerceiros.ToString("n2") : "",
                PercentualCobrancaVeiculoFrota = tabelaFrete.PercentualCobrancaVeiculoFrotaTerceiros > 0 ? tabelaFrete.PercentualCobrancaVeiculoFrotaTerceiros.ToString("n2") : "",
                ParametroBase = tabelaFrete.ParametroBase.HasValue ? tabelaFrete.ParametroBase.Value : 0,
                Fronteiras = tabelaFrete.Fronteiras.Select(o => new { Descricao = o.Descricao, Codigo = o.Codigo }).ToList(),
                Contratos = tabelaFrete.ContratosTransporteFrete.Select(o => new { Descricao = o.NomeContrato, Codigo = o.Codigo }).ToList(),
                TiposDeOcorrencia = (
                    from obj in tabelaFrete.TiposDeOcorrencia
                    orderby obj.Descricao
                    select new
                    {
                        Tipo = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                ).ToList(),
                TiposOperacoes = (
                    from obj in tabelaFrete.TiposOperacao
                    orderby obj.Descricao
                    select new
                    {
                        Tipo = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                ).ToList(),
                Transportadores = (
                    from obj in tabelaFrete.Transportadores
                    orderby obj.Descricao
                    select new
                    {
                        Transportador = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                ).ToList(),
                TipoEmbalagens = (
                    from obj in tabelaFrete.TipoEmbalagens
                    orderby obj.Descricao
                    select new
                    {
                        TipoEmbalagem = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                ).ToList(),
                Filiais = (
                    from obj in tabelaFrete.Filiais
                    orderby obj.Descricao
                    select new
                    {
                        Filial = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                ).ToList(),
                TiposCargas = (
                    from obj in tabelaFrete.TiposCarga
                    orderby obj.Descricao
                    select new
                    {
                        Tipo = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                ).ToList(),
                ModelosTracao = (
                    from obj in tabelaFrete.ModelosTracao
                    orderby obj.Descricao
                    select new
                    {
                        Modelo = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                ).ToList(),
                ModelosReboque = (
                    from obj in tabelaFrete.ModelosReboque
                    orderby obj.Descricao
                    select new
                    {
                        Modelo = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                ).ToList(),
                DadosVigencia = new
                {
                    tabelaFrete.ValidarPorDataCarregamento,
                    tabelaFrete.PermitirVigenciasSobrepostas,
                    tabelaFrete.UsarComoDataBaseVigenciaDataAtual,
                    Vigencias = (
                        from obj in vigenciaTabelaFretes
                        orderby obj.DataInicial
                        select new
                        {
                            Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                            DataInicial = obj.DataInicial.ToString("dd/MM/yyyy"),
                            DataFinal = obj.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                            Empresa = new { Codigo = obj.Empresa?.Codigo ?? 0, Descricao = obj.Empresa?.Descricao ?? "" },
                            Anexos = (
                                from anexo in vigenciaTabelaFretesAnexo
                                where anexo.EntidadeAnexo.Codigo == obj.Codigo
                                select new
                                {
                                    anexo.Codigo,
                                    anexo.Descricao,
                                    anexo.NomeArquivo
                                }
                            ).ToList()
                        }
                    ).ToList(),
                    VigenciasFinal = (
                        from obj in vigenciaTabelaFretes
                        orderby obj.DataFinal descending
                        select new
                        {
                            DataFinal = obj.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                        }
                    ).ToList()
                },
                ComponentesFrete = (
                    from obj in componentesFreteTabelaFrete
                    orderby obj.ComponenteFrete.Descricao
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        Componente = new
                        {
                            obj.ComponenteFrete.Codigo,
                            obj.ComponenteFrete.Descricao,
                            Tipo = obj.ComponenteFrete.TipoComponenteFrete
                        },
                        ModeloDocumentoFiscalRestringirQuantidade = new
                        {
                            Codigo = obj.ModeloDocumentoFiscalRestringirQuantidade?.Codigo ?? 0,
                            Descricao = obj.ModeloDocumentoFiscalRestringirQuantidade?.Descricao ?? string.Empty
                        },
                        Justificativa = new
                        {
                            Codigo = obj.Justificativa?.Codigo ?? 0,
                            Descricao = obj.Justificativa?.Descricao ?? string.Empty
                        },
                        obj.IncluirBaseCalculoICMS,
                        obj.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio,
                        obj.ComponenteFrete.TipoComponenteFrete,
                        TipoCalculoPeso = obj.TipoCalculoPeso ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete.PorFracao,
                        TipoCalculoCubagem = obj.TipoCalculoCubagem ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCubagemTabelaFreteComponenteFrete.PorUnidadeIncompleta,
                        Percentual = new { val = obj.Percentual, tipo = "decimal", configDecimal = new { precision = 3 } },
                        Peso = new { val = obj.Peso, tipo = "decimal", configDecimal = new { precision = 6 } },

                        Volume = obj.Volume ?? 0,
                        TipoCalculoVolume = obj.TipoCalculoVolume ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete.PorFracao,
                        ValorExcedentePorVolume = new { val = obj.ValorExcedentePorVolume, tipo = "decimal", configDecimal = new { precision = 6 } },

                        Cubagem = new { val = obj.Cubagem, tipo = "decimal" },
                        ValorExcedentePorKG = new { val = obj.ValorExcedentePorKG, tipo = "decimal", configDecimal = new { precision = 6 } },
                        TipoPercentual = obj.TipoPercentual ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreNotaFiscal,
                        //UtilizarPercentual = obj.UtilizarPercentual ?? false,
                        IncluirBaseCalculo = obj.IncluirBaseCalculo ?? false,
                        //UtilizarPeso = obj.UtilizarPeso ?? false,
                        ValidarValorMercadoria = obj.ValidarValorMercadoria ?? false,
                        ValidarPesoMercadoria = obj.ValidarPesoMercadoria ?? false,
                        ValidarDimensoesMercadoria = obj.ValidarDimensoesMercadoria ?? false,
                        ValorFormula = new { val = obj.ValorFormula, tipo = "decimal", configDecimal = new { precision = 6 } },
                        ValorMaximo = new { val = obj.ValorMaximo, tipo = "decimal" },
                        ValorMinimo = new { val = obj.ValorMinimo, tipo = "decimal" },
                        EntregaMinima = new { val = obj.EntregaMinima, tipo = "decimal" },
                        ValorMercadoriaMaximo = new { val = obj.ValorMercadoriaMaximo, tipo = "decimal" },
                        ValorMercadoriaMinimo = new { val = obj.ValorMercadoriaMinimo, tipo = "decimal" },
                        PesoMercadoriaMaximo = new { val = obj.PesoMercadoriaMaximo, tipo = "decimal" },
                        PesoMercadoriaMinimo = new { val = obj.PesoMercadoriaMinimo, tipo = "decimal" },
                        AlturaMercadoriaMinima = new { val = obj.AlturaMercadoriaMinima, tipo = "decimal" },
                        AlturaMercadoriaMaxima = new { val = obj.AlturaMercadoriaMaxima, tipo = "decimal" },
                        LarguraMercadoriaMinima = new { val = obj.LarguraMercadoriaMinima, tipo = "decimal" },
                        LarguraMercadoriaMaxima = new { val = obj.LarguraMercadoriaMaxima, tipo = "decimal" },
                        ComprimentoMercadoriaMinimo = new { val = obj.ComprimentoMercadoriaMinimo, tipo = "decimal" },
                        ComprimentoMercadoriaMaximo = new { val = obj.ComprimentoMercadoriaMaximo, tipo = "decimal" },
                        VolumeMercadoriaMinimo = new { val = obj.VolumeMercadoriaMinimo, tipo = "decimal" },
                        VolumeMercadoriaMaximo = new { val = obj.VolumeMercadoriaMaximo, tipo = "decimal" },
                        ValorInformadoNaTabela = obj.ValorInformadoNaTabela ?? false,
                        UtilizarCalculoDesseComponenteNaOcorrencia = obj.UtilizarCalculoDesseComponenteNaOcorrencia ?? false,
                        ValorUnicoParaCarga = obj.ValorUnicoParaCarga ?? false,
                        obj.Tipo,
                        TipoCalculo = obj.TipoCalculo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Nenhum,
                        UtilizarFormulaRateioCarga = obj.UtilizarFormulaRateioCarga ?? false,
                        UtilizarDiasEspecificos = obj.UtilizarDiasEspecificos ?? false,
                        SegundaFeira = obj.SegundaFeira ?? false,
                        TercaFeira = obj.TercaFeira ?? false,
                        QuartaFeira = obj.QuartaFeira ?? false,
                        QuintaFeira = obj.QuintaFeira ?? false,
                        SextaFeira = obj.SextaFeira ?? false,
                        Sabado = obj.Sabado ?? false,
                        Domingo = obj.Domingo ?? false,
                        Feriados = obj.Feriados ?? false,
                        UtilizarPeriodoColeta = obj.UtilizarPeriodoColeta ?? false,
                        HoraColetaInicial = obj.HoraColetaInicial?.ToString(@"hh\:mm") ?? string.Empty,
                        HoraColetaFinal = obj.HoraColetaFinal?.ToString(@"hh\:mm") ?? string.Empty,
                        SomenteComDataPrevisaoEntrega = obj.SomenteComDataPrevisaoEntrega ?? false,
                        EscoltaArmada = obj.EscoltaArmada ?? false,
                        GerenciamentoRisco = obj.GerenciamentoRisco ?? false,
                        obj.ComponenteComparado,
                        Rastreado = obj.Rastreado ?? false,
                        Reentrega = obj.Reentrega ?? false,
                        DespachoTransitoAduaneiro = obj.DespachoTransitoAduaneiro ?? false,
                        RestricaoTrafego = obj.RestricaoTrafego ?? false,
                        //UtilizarQuantidadeDocumentos = obj.UtilizarQuantidadeDocumentos ?? false,
                        TipoDocumentoQuantidadeDocumentos = obj.TipoDocumentoQuantidadeDocumentos ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorNotaFiscal,
                        //CalculoPorTempo = obj.CalculoPorTempo ?? false,
                        HorasMinimasCobrancaTempo = obj.HorasMinimasCobrancaTempo?.ToString(@"hh\:mm") ?? string.Empty,
                        MinutosArredondamentoHorasTempo = obj.MinutosArredondamentoHorasTempo ?? 0,
                        obj.MultiplicarPorAjudante,
                        obj.MultiplicarPorDeslocamento,
                        obj.MultiplicarPorDiaria,
                        obj.MultiplicarPorEntrega,
                        obj.MultiplicarPorHoraTempo,
                        obj.PossuiHorasMinimasCobrancaTempo,
                        obj.UtilizarArredondamentoHorasTempo,
                        obj.TipoViagem,
                        Tempos = (
                            from faixa in componentesFreteTabelaFretesTempo
                            where faixa.ComponenteFreteTabelaFrete.Codigo == obj.Codigo
                            select new
                            {
                                Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : faixa.Codigo.ToString(),
                                HoraFinalTempo = faixa.HoraFinal.ToString(@"hh\:mm"),
                                HoraFinalCobrancaMinimaTempo = faixa.HoraFinalCobrancaMinima?.ToString(@"hh\:mm") ?? string.Empty,
                                HoraInicialTempo = faixa.HoraInicial.ToString(@"hh\:mm"),
                                HoraInicialCobrancaMinimaTempo = faixa.HoraInicialCobrancaMinima?.ToString(@"hh\:mm") ?? string.Empty,
                                PeriodoInicialTempo = faixa.PeriodoInicial,
                                ValorTempo = faixa.Valor.ToString("n2")
                            }
                        ).ToList()
                    }
                ).ToList(),
                PermiteValorAdicionalEntregaExcedente = tabelaFrete.PermiteValorAdicionalPorEntregaExcedente,
                PermiteValorAdicionalPacoteExcedente = tabelaFrete.PermiteValorAdicionalPorPacoteExcedente,
                NumeroEntregas = (
                    from obj in tabelaFrete.NumeroEntregas
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        Descricao = obj.Descricao,
                        NumeroFinalEntrega = obj.NumeroFinalEntrega.HasValue ? obj.NumeroFinalEntrega.Value : 0,
                        NumeroInicialEntrega = obj.NumeroInicialEntrega.HasValue ? obj.NumeroInicialEntrega.Value : 0,
                        ComAjudante = obj.ComAjudante,
                        obj.Tipo
                    }
                ).ToList(),
                Pacotes = (
                    from obj in tabelaFrete.Pacotes
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        Descricao = obj.Descricao,
                        NumeroFinalPacote = obj.NumeroFinalPacote.HasValue ? obj.NumeroFinalPacote.Value : 0,
                        NumeroInicialPacote = obj.NumeroInicialPacote.HasValue ? obj.NumeroInicialPacote.Value : 0,
                        ComAjudante = obj.ComAjudante,
                        obj.Tipo
                    }
                ).ToList(),
                PermiteValorAdicionalPalletExcedente = tabelaFrete.PermiteValorAdicionalPorPalletExcedente,
                TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas = tabelaFrete.TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas,
                Pallets = (
                    from obj in tabelaFrete.Pallets
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        Descricao = obj.Descricao,
                        NumeroFinalPallet = obj.NumeroFinalPallet.HasValue ? obj.NumeroFinalPallet.Value : 0,
                        NumeroInicialPallet = obj.NumeroInicialPallet.HasValue ? obj.NumeroInicialPallet.Value : 0,
                        obj.Tipo
                    }
                ).ToList(),
                PermiteValorAdicionalPesoExcedente = tabelaFrete.PermiteValorAdicionalPorPesoExcedente,
                PesoExcedente = tabelaFrete.PesoExcecente.ToString("n3"),
                PesosTransportados = (
                    from obj in pesosTabelaFrete
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        obj.Descricao,
                        Peso = new { val = obj.Peso.HasValue ? obj.Peso.Value : 0m, tipo = "decimal", configDecimal = new { precision = 3 } },
                        PesoFinal = new { val = obj.PesoFinal.HasValue ? obj.PesoFinal.Value : 0m, tipo = "decimal", configDecimal = new { precision = 3 } },
                        PesoInicial = new { val = obj.PesoInicial.HasValue ? obj.PesoInicial.Value : 0m, tipo = "decimal", configDecimal = new { precision = 3 } },
                        obj.Tipo,
                        ModeloVeicularCarga = new { Codigo = obj.ModeloVeicularCarga?.Codigo ?? 0, Descricao = obj.ModeloVeicularCarga?.Descricao ?? "" },
                        obj.CalculoPeso,
                        obj.ComAjudante,
                        obj.ParaCalcularValorBase,
                        CodigoComponenteFrete = obj.ComponenteFrete?.Codigo ?? 0,
                        DescricaoComponente = obj.ComponenteFrete?.Descricao ?? string.Empty,
                        UnidadeMedida = new
                        {
                            obj.UnidadeMedida.Codigo,
                            obj.UnidadeMedida.Descricao,
                            obj.UnidadeMedida.Sigla
                        }
                    }
                ).ToList(),
                PermiteValorAdicionalQuilometragemExcedente = tabelaFrete.PermiteValorAdicionalPorQuilometragemExcedente,
                QuilometragemExcedente = tabelaFrete.QuilometragemExcedente.ToString("n2"),
                Distancias = (
                    from obj in tabelaFrete.Distancias
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        QuilometragemFinal = new { val = obj.QuilometragemFinal.HasValue ? obj.QuilometragemFinal.Value : 0m, tipo = "decimal" },
                        QuilometragemInicial = new { val = obj.QuilometragemInicial.HasValue ? obj.QuilometragemInicial.Value : 0m, tipo = "decimal" },
                        Descricao = obj.Descricao,
                        Quilometros = new { val = obj.Quilometros.HasValue ? obj.Quilometros.Value : 0m, tipo = "decimal" },
                        obj.MultiplicarPeloResultadoDaDistancia,
                        obj.MultiplicarValorDaFaixa,
                        obj.Tipo,
                        obj.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga,
                    }
                ).ToList(),
                Subcontratacoes = (
                    from obj in subcontratacaosTabelaFrete
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        PercentualDesconto = new { val = obj.PercentualDesconto, tipo = "decimal" },
                        PercentualCobranca = new { val = obj.PercentualCobranca, tipo = "decimal" },
                        Pessoa = new { Codigo = obj.Pessoa.CPF_CNPJ, Descricao = obj.Pessoa.Descricao }
                    }
                ).ToList(),
                RotasFreteEmbarcador = (
                    from obj in rotasEmbarcadorTabelaFrete
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        Componente = new
                        {
                            Codigo = obj.ComponenteFrete != null ? obj.ComponenteFrete.Codigo : 0,
                            Descricao = obj.ComponenteFrete != null ? obj.ComponenteFrete.Descricao : ""
                        },
                        RotaFrete = new
                        {
                            Codigo = obj.RotaFrete.Codigo,
                            Descricao = obj.RotaFrete.Descricao
                        },
                        ValorFixoRota = obj.ValorFixoRota.ToString("n2"),
                        obj.ValorAdicionalFixoPorRota
                    }
                ).ToList(),
                tabelaFrete.MultiplicarValorTempoPorHora,
                tabelaFrete.PossuiHorasMinimasCobranca,
                HorasMinimasCobranca = tabelaFrete.HorasMinimasCobranca?.ToString(@"hh\:mm") ?? string.Empty,
                Tempos = (
                    from obj in tabelaFrete.Tempos
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        HoraInicial = string.Format("{0:00}:{1:00}", obj.HoraInicial.Hours, obj.HoraInicial.Minutes),
                        HoraFinal = string.Format("{0:00}:{1:00}", obj.HoraFinal.Hours, obj.HoraFinal.Minutes),
                        obj.PeriodoInicial,
                        HoraInicialCobrancaMinima = obj.HoraInicialCobrancaMinima.HasValue ? string.Format("{0:00}:{1:00}", obj.HoraInicialCobrancaMinima.Value.Hours, obj.HoraInicialCobrancaMinima.Value.Minutes) : string.Empty,
                        HoraFinalCobrancaMinima = obj.HoraFinalCobrancaMinima.HasValue ? string.Format("{0:00}:{1:00}", obj.HoraFinalCobrancaMinima.Value.Hours, obj.HoraFinalCobrancaMinima.Value.Minutes) : string.Empty,
                        obj.Descricao
                    }
                ).ToList(),
                PermiteValorAdicionalAjudanteExcedente = tabelaFrete.PermiteValorAdicionalPorAjudanteExcedente,
                Ajudantes = (
                    from obj in tabelaFrete.Ajudantes
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        obj.Descricao,
                        obj.NumeroInicial,
                        obj.NumeroFinal,
                        obj.Tipo
                    }
                ).ToList(),
                PermiteValorAdicionalHoraExcedente = tabelaFrete.PermiteValorAdicionalPorHoraExcedente,
                tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida,
                tabelaFrete.TipoArredondamentoHoras,
                tabelaFrete.CalcularComTodasFaixasHora,
                Horas = (
                    from obj in tabelaFrete.Horas
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        obj.Descricao,
                        MinutoInicial = obj.MinutoInicial.HasValue ? (obj.MinutoInicial / 60).ToString() : "",
                        MinutoFinal = obj.MinutoFinal.HasValue ? (obj.MinutoFinal / 60).ToString() : "",
                        obj.Tipo
                    }
                ).ToList(),
                tabelaFrete.CalcularValorEntregaPorPercentualFrete,
                tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes,
                TransportadoresTerceiros = (
                    from obj in tabelaFrete.TransportadoresTerceiros
                    select new
                    {
                        obj.Codigo,
                        Descricao = obj.Descricao
                    }
                ).ToList(),
                TiposTerceiros = (
                    from obj in tabelaFrete.TiposTerceiros
                    select new
                    {
                        obj.Codigo,
                        Descricao = obj.Descricao
                    }
                ).ToList(),
                Anexos = (
                    from anexo in tabelaFrete.Anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                    }
                ).ToList(),
                Tracoes = (
                    from o in tabelaFreteVeiculos.Where(o => o.Veiculo.TipoVeiculo == "0").ToList()
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : o.Codigo.ToString(),
                        CodigoVeiculo = o.Veiculo.Codigo,
                        Placa = o.Veiculo.Placa,
                        ModeloVeicularCarga = o.Veiculo.ModeloVeicularCarga?.Descricao ?? "",
                        CapacidadeKG = o.Veiculo.CapacidadeKG.ToString("n0"),
                        CapacidadeM3 = o.Veiculo.CapacidadeM3.ToString("n0"),
                        Empresa = o.Veiculo?.Empresa?.RazaoSocial ?? string.Empty,
                    }
                ).ToList(),
                Reboques = (
                    from o in tabelaFreteVeiculos.Where(o => o.Veiculo.TipoVeiculo == "1").ToList()
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : o.Codigo.ToString(),
                        CodigoVeiculo = o.Veiculo.Codigo,
                        Placa = o.Veiculo.Placa,
                        ModeloVeicularCarga = o.Veiculo.ModeloVeicularCarga?.Descricao ?? "",
                        CapacidadeKG = o.Veiculo.CapacidadeKG.ToString("n0"),
                        CapacidadeM3 = o.Veiculo.CapacidadeM3.ToString("n0"),
                        Empresa = o.Veiculo?.Empresa?.RazaoSocial ?? string.Empty,
                    }
                )
            };
        }

        private dynamic ObterTabelaFreteRetornar(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ajusteTabelaFretes, IList<(int CodigoTabelaFrete, SituacaoTabelaFreteIntegrarAlteracao Situacao)> situacoesIntegracaoAlteracao)
        {
            Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = ajusteTabelaFretes.Where(ajuste => ajuste.TabelaFrete.Codigo == tabelaFrete.Codigo).FirstOrDefault();
            SituacaoTabelaFreteIntegrarAlteracao? situacaoIntegracaoAlteracao = situacoesIntegracaoAlteracao.Where(situacaoIntegracao => situacaoIntegracao.CodigoTabelaFrete == tabelaFrete.Codigo).Select(situacaoIntegracao => (SituacaoTabelaFreteIntegrarAlteracao?)situacaoIntegracao.Situacao).FirstOrDefault();

            return new
            {
                tabelaFrete.Codigo,
                tabelaFrete.TipoTabelaFrete,
                tabelaFrete.TipoCalculo,
                tabelaFrete.Descricao,
                tabelaFrete.DescricaoTipoTabelaFrete,
                tabelaFrete.DescricaoAtivo,
                tabelaFrete.CodigoIntegracao,
                PermitirIntegrarAlteracao = situacaoIntegracaoAlteracao == SituacaoTabelaFreteIntegrarAlteracao.PendenteIntegracao,
                DescricaoSituacaoAprovacao = tabelaFrete.SituacaoAlteracao.ObterDescricaoPorTabelaFrete(),
                DescricaoSituacaoAjusteTabelaFrete = ajusteTabelaFrete?.Situacao?.ObterDescricao() ?? "",
                DescricaoSituacaoIntegracaoAlteracao = situacaoIntegracaoAlteracao?.ObterDescricao() ?? ""
            };
        }

        private void SalvarTiposOcorrencia(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);
            dynamic tiposOcorrencia = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposDeOcorrencia"));

            if (tabelaFrete.TiposDeOcorrencia == null)
                tabelaFrete.TiposDeOcorrencia = new List<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOcorrencia in tiposOcorrencia)
                    codigos.Add((int)tipoOcorrencia.Tipo.Codigo);

                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeletar = tabelaFrete.TiposDeOcorrencia.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrenciaDeletar in tiposDeletar)
                    tabelaFrete.TiposDeOcorrencia.Remove(tipoOcorrenciaDeletar);
            }

            foreach (dynamic tipoOcorrencia in tiposOcorrencia)
            {
                if (tabelaFrete.TiposDeOcorrencia.Any(o => o.Codigo == (int)tipoOcorrencia.Tipo.Codigo))
                    continue;

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrenciaObj = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigo((int)tipoOcorrencia.Tipo.Codigo);
                tabelaFrete.TiposDeOcorrencia.Add(tipoOcorrenciaObj);
            }
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacoes"));

            if (tabelaFrete.TiposOperacao == null)
                tabelaFrete.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOperacao in tiposOperacao)
                    codigos.Add((int)tipoOperacao.Tipo.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposDeletar = tabelaFrete.TiposOperacao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoDeletar in tiposDeletar)
                    tabelaFrete.TiposOperacao.Remove(tipoOperacaoDeletar);
            }

            foreach (dynamic tipoOperacao in tiposOperacao)
            {
                if (tabelaFrete.TiposOperacao.Any(o => o.Codigo == (int)tipoOperacao.Tipo.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoObj = repositorioTipoOperacao.BuscarPorCodigo((int)tipoOperacao.Tipo.Codigo);
                tabelaFrete.TiposOperacao.Add(tipoOperacaoObj);
            }
        }

        private void SalvarTransportadores(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            dynamic transportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Transportadores"));

            if (tabelaFrete.Transportadores == null)
                tabelaFrete.Transportadores = new List<Dominio.Entidades.Empresa>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic transportador in transportadores)
                    codigos.Add((int)transportador.Transportador.Codigo);

                List<Dominio.Entidades.Empresa> transportadoresDeletar = tabelaFrete.Transportadores.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Empresa transportadorDeletar in transportadoresDeletar)
                    tabelaFrete.Transportadores.Remove(transportadorDeletar);
            }

            foreach (dynamic transportador in transportadores)
            {
                if (tabelaFrete.Transportadores.Any(o => o.Codigo == (int)transportador.Transportador.Codigo))
                    continue;

                Dominio.Entidades.Empresa transportadorObj = repositorioEmpresa.BuscarPorCodigo((int)transportador.Transportador.Codigo);
                tabelaFrete.Transportadores.Add(transportadorObj);
            }
        }

        private void SalvarTipoEmbalagens(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Produtos.TipoEmbalagem repositorioTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unidadeDeTrabalho);
            dynamic tipoEmbalagens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoEmbalagens"));

            if (tabelaFrete.TipoEmbalagens == null)
                tabelaFrete.TipoEmbalagens = new List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoEmbalagem in tipoEmbalagens)
                    codigos.Add((int)tipoEmbalagem.TipoEmbalagem.Codigo);

                List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> tipoEmbalagensDeletar = tabelaFrete.TipoEmbalagens.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagemDeletar in tipoEmbalagensDeletar)
                    tabelaFrete.TipoEmbalagens.Remove(tipoEmbalagemDeletar);
            }

            foreach (dynamic tipoEmbalagem in tipoEmbalagens)
            {
                if (tabelaFrete.TipoEmbalagens.Any(o => o.Codigo == (int)tipoEmbalagem.TipoEmbalagem.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagemObj = repositorioTipoEmbalagem.BuscarPorCodigo((int)tipoEmbalagem.TipoEmbalagem.Codigo);
                tabelaFrete.TipoEmbalagens.Add(tipoEmbalagemObj);
            }
        }

        private void SalvarFiliais(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);
            dynamic filiais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            if (tabelaFrete.Filiais == null)
                tabelaFrete.Filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic filial in filiais)
                    codigos.Add((int)filial.Filial.Codigo);

                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisDeletar = tabelaFrete.Filiais.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Filiais.Filial filialDeletar in filiaisDeletar)
                    tabelaFrete.Filiais.Remove(filialDeletar);
            }

            foreach (dynamic filial in filiais)
            {
                if (tabelaFrete.Filiais.Any(o => o.Codigo == (int)filial.Filial.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Filiais.Filial filialObj = repositorioFilial.BuscarPorCodigo((int)filial.Filial.Codigo);
                tabelaFrete.Filiais.Add(filialObj);
            }
        }

        private void SalvarTiposCarga(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
            dynamic tiposCargas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCargas"));

            if (tabelaFrete.TiposCarga == null)
                tabelaFrete.TiposCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoCarga in tiposCargas)
                    codigos.Add((int)tipoCarga.Tipo.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeletar = tabelaFrete.TiposCarga.Where(o => !codigos.Contains(o.Codigo)).ToList();

                if (tiposDeletar.Count > 0)
                    ValidarValoresParametrosTabelasClientes(tabelaFrete, unidadeDeTrabalho);

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaDeletar in tiposDeletar)
                    tabelaFrete.TiposCarga.Remove(tipoCargaDeletar);
            }

            foreach (dynamic tipoCarga in tiposCargas)
            {
                if (tabelaFrete.TiposCarga.Any(o => o.Codigo == (int)tipoCarga.Tipo.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoCarga.BuscarPorCodigo((int)tipoCarga.Tipo.Codigo);
                tabelaFrete.TiposCarga.Add(tipoDeCarga);
            }
        }

        private void SalvarModelosTracao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            dynamic modelosTracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosTracao"));

            if (tabelaFrete.ModelosTracao == null)
                tabelaFrete.ModelosTracao = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic modelo in modelosTracao)
                    codigos.Add((int)modelo.Modelo.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDeletar = tabelaFrete.ModelosTracao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloDeletar in modelosDeletar)
                    tabelaFrete.ModelosTracao.Remove(modeloDeletar);
            }

            foreach (dynamic modeloTracao in modelosTracao)
            {
                if (tabelaFrete.ModelosTracao.Any(o => o.Codigo == (int)modeloTracao.Modelo.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)modeloTracao.Modelo.Codigo);
                tabelaFrete.ModelosTracao.Add(modeloVeicularCarga);
            }
        }

        private void SalvarModelosReboque(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            dynamic modelosReboque = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosReboque"));

            if (tabelaFrete.ModelosReboque == null)
                tabelaFrete.ModelosReboque = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic modelo in modelosReboque)
                    codigos.Add((int)modelo.Modelo.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDeletar = tabelaFrete.ModelosReboque.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloDeletar in modelosDeletar)
                    tabelaFrete.ModelosReboque.Remove(modeloDeletar);
            }

            foreach (dynamic modeloReboque in modelosReboque)
            {
                if (tabelaFrete.ModelosReboque.Any(o => o.Codigo == (int)modeloReboque.Modelo.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo((int)modeloReboque.Modelo.Codigo);
                tabelaFrete.ModelosReboque.Add(modeloVeicularCarga);
            }
        }

        private void SalvarTransportadoresTerceiros(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unidadeTrabalho);
            dynamic transportadoresTerceiros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TransportadoresTerceiros"));

            if (tabelaFrete.TransportadoresTerceiros == null)
                tabelaFrete.TransportadoresTerceiros = new List<Dominio.Entidades.Cliente>();
            else
            {
                List<Dominio.Entidades.Cliente> transportadoresTerceirosDeletar = tabelaFrete.TransportadoresTerceiros.Where(o => !transportadoresTerceiros.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Cliente transportadorTerceiroDeletar in transportadoresTerceirosDeletar)
                    tabelaFrete.TransportadoresTerceiros.Remove(transportadorTerceiroDeletar);
            }

            foreach (double transportadorTerceiro in transportadoresTerceiros)
            {
                if (tabelaFrete.TransportadoresTerceiros.Any(o => o.CPF_CNPJ == transportadorTerceiro))
                    continue;

                Dominio.Entidades.Cliente transportadorTerceiroObj = repositorioCliente.BuscarPorCPFCNPJ(transportadorTerceiro);
                tabelaFrete.TransportadoresTerceiros.Add(transportadorTerceiroObj);
            }
        }

        private void SalvarTiposTerceiros(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.TipoTerceiro repositorioTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unidadeTrabalho);
            dynamic tiposTerceiros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposTerceiros"));

            if (tabelaFrete.TiposTerceiros == null)
                tabelaFrete.TiposTerceiros = new List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>();
            else
            {
                List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> tiposTerceirosDeletar = null;
                if (tabelaFrete.PagamentoTerceiro)
                    tiposTerceirosDeletar = tabelaFrete.TiposTerceiros.Where(o => !tiposTerceiros.Contains(o.Codigo)).ToList();
                else
                    tiposTerceirosDeletar = tabelaFrete.TiposTerceiros.ToList();

                foreach (Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiroDeletar in tiposTerceirosDeletar)
                    tabelaFrete.TiposTerceiros.Remove(tipoTerceiroDeletar);

                if (!tabelaFrete.PagamentoTerceiro)
                    return;
            }

            foreach (int tipoTerceiro in tiposTerceiros)
            {
                if (tabelaFrete.TiposTerceiros.Any(o => o.Codigo == tipoTerceiro))
                    continue;

                Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiroObj = repositorioTipoTerceiro.BuscarPorCodigo(tipoTerceiro);
                tabelaFrete.TiposTerceiros.Add(tipoTerceiroObj);
            }
        }

        private void SalvarFronteiras(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            List<double> fronteiras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Fronteiras"));

            if (tabelaFrete.Fronteiras == null)
                tabelaFrete.Fronteiras = new List<Dominio.Entidades.Cliente>();
            else
            {
                List<Dominio.Entidades.Cliente> fronteirasDeletar = tabelaFrete.Fronteiras.Where(o => !fronteiras.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Cliente fronteiraDeletar in fronteirasDeletar)
                    tabelaFrete.Fronteiras.Remove(fronteiraDeletar);
            }

            foreach (double fronteira in fronteiras)
            {
                if (tabelaFrete.Fronteiras.Any(o => o.CPF_CNPJ == fronteira))
                    continue;

                Dominio.Entidades.Cliente fronteiraObj = repositorioCliente.BuscarPorCPFCNPJ(fronteira);
                tabelaFrete.Fronteiras.Add(fronteiraObj);
            }
        }

        private void SalvarVeiculos(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteVeiculo repositorioTabelaFreteVeiculo = new Repositorio.Embarcador.Frete.TabelaFreteVeiculo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo> tabelaFreteVeiculos = repositorioTabelaFreteVeiculo.BuscarPorTabela(tabelaFrete.Codigo);
            dynamic tabelaFreteVeiculosAdicionarOuAtualizar = Request.GetListParam<dynamic>("Veiculos");
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (tabelaFreteVeiculos.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var tabelaFreteVeiculo in tabelaFreteVeiculosAdicionarOuAtualizar)
                {
                    int? codigo = ((string)tabelaFreteVeiculo.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo> tabelaFreteVeiculosRemover = (from tabelaFreteVeiculo in tabelaFreteVeiculos where !listaCodigosAtualizados.Contains(tabelaFreteVeiculo.Codigo) select tabelaFreteVeiculo).ToList();

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo tabelaFreteVeiculo in tabelaFreteVeiculosRemover)
                {
                    repositorioTabelaFreteVeiculo.Deletar(tabelaFreteVeiculo);
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade() { Propriedade = Localization.Resources.Fretes.TabelaFreteCliente.Veiculos, De = $"{tabelaFreteVeiculo.Veiculo.Codigo} - {tabelaFreteVeiculo.Veiculo.Descricao}", Para = "" });
                }
            }

            foreach (var tabelaFreteVeiculo in tabelaFreteVeiculosAdicionarOuAtualizar)
            {
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(((string)tabelaFreteVeiculo.CodigoVeiculo).ToInt()) ?? throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.VeiculoNaoEncontrado);

                int? codigo = ((string)tabelaFreteVeiculo.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    continue;

                Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo tabelaFreteVeiculoSalvar = new Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo()
                {
                    TabelaFrete = tabelaFrete,
                    Veiculo = veiculo
                };

                repositorioTabelaFreteVeiculo.Inserir(tabelaFreteVeiculoSalvar);
                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade() { Propriedade = Localization.Resources.Fretes.TabelaFreteCliente.Veiculos, De = "", Para = $"{tabelaFreteVeiculoSalvar.Veiculo.Codigo} - {tabelaFreteVeiculoSalvar.Veiculo.Descricao}" });
            }

            tabelaFrete.PossuiVeiculos = tabelaFreteVeiculosAdicionarOuAtualizar.Count > 0;
            tabelaFrete.SetExternalChanges(alteracoes);
        }
        private bool SalvarVigencias(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unidadeDeTrabalho);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            bool vigenciaAlterada = false;

            if (tabelaFrete.ContratoFreteTransportador != null)
            {
                List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> vigencias = repositorioVigencia.BuscarPorTabela(tabelaFrete.Codigo);
                bool auditouReplicacaoVigenciaContrato = false;

                foreach (Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia in vigencias)
                {
                    vigencia.Initialize();
                    vigencia.DataInicial = tabelaFrete.ContratoFreteTransportador.DataInicial;
                    vigencia.DataFinal = tabelaFrete.ContratoFreteTransportador.DataFinal;

                    repositorioVigencia.Atualizar(vigencia);

                    if (!auditouReplicacaoVigenciaContrato)
                    {
                        auditouReplicacaoVigenciaContrato = true;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, "Vigência " + vigencia.Descricao + " replicada do contrato " + tabelaFrete.ContratoFreteTransportador.Descricao + ".", unidadeDeTrabalho);
                    }

                    if (vigencia.IsChanged())
                        vigenciaAlterada = true;
                }

                if (vigencias.Count == 0)
                {
                    Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia = new Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete
                    {
                        TabelaFrete = tabelaFrete,
                        DataInicial = tabelaFrete.ContratoFreteTransportador.DataInicial,
                        DataFinal = tabelaFrete.ContratoFreteTransportador.DataFinal
                    };

                    repositorioVigencia.Inserir(vigencia);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.VigenciaReplicadaContrato, vigencia.Descricao, tabelaFrete.ContratoFreteTransportador.Descricao), unidadeDeTrabalho);
                    vigenciaAlterada = true;
                }
            }
            else
            {
                dynamic vigencias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Vigencias"));

                if (tabelaFrete.Vigencias != null && tabelaFrete.Vigencias.Count > 0)
                {
                    List<int> codigos = new List<int>();

                    foreach (dynamic vigencia in vigencias)
                    {
                        int.TryParse((string)vigencia.Codigo, out int codigo);
                        codigos.Add(codigo);
                    }

                    List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> vigenciasDeletar = (from obj in tabelaFrete.Vigencias where !codigos.Contains(obj.Codigo) select obj).ToList();

                    for (int i = 0; i < vigenciasDeletar.Count; i++)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuVigenciaX, vigenciasDeletar[i].Descricao), unidadeDeTrabalho);
                        repositorioVigencia.Deletar(vigenciasDeletar[i], Auditado);
                        vigenciaAlterada = true;
                    }
                }

                foreach (dynamic dynVigencia in vigencias)
                {
                    Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia = null;

                    int codigo = 0;

                    if (dynVigencia.Codigo != null && int.TryParse((string)dynVigencia.Codigo, out codigo))
                        vigencia = repositorioVigencia.BuscarPorCodigo(codigo);

                    if (vigencia == null)
                        vigencia = new Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete();
                    else
                        vigencia.Initialize();

                    vigencia.TabelaFrete = tabelaFrete;
                    vigencia.DataInicial = ((string)dynVigencia.DataInicial).ToDateTime();
                    vigencia.DataFinal = ((string)dynVigencia.DataFinal).ToNullableDateTime();

                    if ((int)dynVigencia.Empresa.Codigo > 0)
                        vigencia.Empresa = repositorioEmpresa.BuscarPorCodigo((int)dynVigencia.Empresa.Codigo);
                    else
                        vigencia.Empresa = null;

                    if (!tabelaFrete.PermitirVigenciasSobrepostas)
                    {
                        Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigenciaConflito = repositorioVigencia.ValidarVigenciaCompativel(vigencia.Codigo, vigencia.TabelaFrete.Codigo, vigencia.DataInicial, vigencia.DataFinal, vigencia.Empresa?.Codigo ?? 0);

                        if (vigenciaConflito != null)
                            throw new ControllerException(string.Format(Localization.Resources.Fretes.TabelaFrete.OPeriodoDeVigenciaEntrouEmConflitoComAVigencia, vigencia.Descricao.ToLower(), vigenciaConflito.Descricao.ToLower()));
                    }

                    if (vigencia.Codigo > 0)
                    {
                        repositorioVigencia.Atualizar(vigencia, Auditado);
                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = vigencia.GetChanges();

                        if (alteracoes.Count > 0)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouAVigencia, vigencia.Descricao), unidadeDeTrabalho);
                            vigenciaAlterada = true;
                        }
                    }
                    else
                    {
                        repositorioVigencia.Inserir(vigencia, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicinouAVigenciaX, vigencia.Descricao), unidadeDeTrabalho);
                        vigenciaAlterada = true;
                    }
                }
            }

            return vigenciaAlterada;
        }

        private bool SalvarRotas(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete repositorioRotaEmbarcadorTabelaFrete = new Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete(unidadeDeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponente = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);
            dynamic rotas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RotasFreteEmbarcador"));
            bool rotasAlteradas = false;

            if (tabelaFrete.RotasFreteEmbarcador != null && tabelaFrete.RotasFreteEmbarcador.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic rota in rotas)
                    codigos.Add((int)rota.Codigo);

                List<Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete> rotasDeletar = (from obj in tabelaFrete.RotasFreteEmbarcador where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < rotasDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuARota, rotasDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioRotaEmbarcadorTabelaFrete.Deletar(rotasDeletar[i]);
                    rotasAlteradas = true;
                }
            }

            foreach (dynamic rota in rotas)
            {
                Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete rotaFreteEmbarcador = null;

                int codigo = 0;

                if (rota.Codigo != null && int.TryParse((string)rota.Codigo, out codigo))
                    rotaFreteEmbarcador = repositorioRotaEmbarcadorTabelaFrete.BuscarPorCodigo(codigo);

                if (rotaFreteEmbarcador == null)
                    rotaFreteEmbarcador = new Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete();
                else
                    rotaFreteEmbarcador.Initialize();

                rotaFreteEmbarcador.TabelaFrete = tabelaFrete;

                if (rota.Componente != null && (int)rota.Componente.Codigo > 0)
                    rotaFreteEmbarcador.ComponenteFrete = repositorioComponente.BuscarPorCodigo((int)rota.Componente.Codigo);
                else
                    rotaFreteEmbarcador.ComponenteFrete = null;

                rotaFreteEmbarcador.RotaFrete = repositorioRotaFrete.BuscarPorCodigo((int)rota.RotaFrete.Codigo);
                rotaFreteEmbarcador.ValorAdicionalFixoPorRota = (bool)rota.ValorAdicionalFixoPorRota;
                rotaFreteEmbarcador.ValorFixoRota = decimal.Parse(rota.ValorFixoRota.ToString());

                if (rotaFreteEmbarcador.Codigo > 0)
                {
                    repositorioRotaEmbarcadorTabelaFrete.Atualizar(rotaFreteEmbarcador);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = rotaFreteEmbarcador.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouARota, rotaFreteEmbarcador.Descricao), unidadeDeTrabalho);
                        rotasAlteradas = true;
                    }
                }
                else
                {
                    repositorioRotaEmbarcadorTabelaFrete.Inserir(rotaFreteEmbarcador);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouARota, rotaFreteEmbarcador.Descricao), unidadeDeTrabalho);
                    rotasAlteradas = true;
                }
            }

            return rotasAlteradas;
        }

        private bool SalvarComponentesFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repositorioComponenteTabela = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(unidadeDeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponente = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Fatura.Justificativa repositorioJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeDeTrabalho);

            dynamic componentes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ComponentesFrete"));

            List<int> codigos = new List<int>();
            bool componentesAlterados = false;

            if (tabelaFrete.Componentes != null && tabelaFrete.Componentes.Count > 0)
            {
                foreach (dynamic componente in componentes)
                {
                    if (componente.Codigo != null)
                        codigos.Add((int)componente.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> componentesDeletar = (from obj in tabelaFrete.Componentes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < componentesDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOComponente, componentesDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioComponenteTabela.Deletar(componentesDeletar[i]);
                    componentesAlterados = true;
                }
            }

            foreach (dynamic componente in componentes)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete comp = null;
                int codigo = 0;

                if (componente.Codigo != null && int.TryParse((string)componente.Codigo, out codigo))
                    comp = repositorioComponenteTabela.BuscarPorCodigo(codigo);

                if (comp == null)
                    comp = new Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete();
                else
                    comp.Initialize();

                comp.TabelaFrete = tabelaFrete;
                comp.ComponenteFrete = repositorioComponente.BuscarPorCodigo((int)componente.Componente.Codigo);
                comp.IncluirBaseCalculoICMS = (bool)componente.IncluirBaseCalculoICMS;
                comp.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio = (bool)componente.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio;
                comp.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete)componente.Tipo;

                if (comp.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado)
                {
                    comp.TipoCalculo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete)componente.TipoCalculo;
                    comp.UtilizarFormulaRateioCarga = (bool)componente.UtilizarFormulaRateioCarga;
                    comp.Reentrega = (bool)componente.Reentrega;
                    comp.GerenciamentoRisco = (bool)componente.GerenciamentoRisco;
                    comp.ComponenteComparado = (bool)componente.ComponenteComparado;
                    comp.EscoltaArmada = (bool)componente.EscoltaArmada;
                    comp.Rastreado = (bool)componente.Rastreado;
                    comp.DespachoTransitoAduaneiro = (bool)componente.DespachoTransitoAduaneiro;
                    comp.RestricaoTrafego = (bool)componente.RestricaoTrafego;
                    comp.MultiplicarPorAjudante = (bool)componente.MultiplicarPorAjudante;
                    comp.MultiplicarPorDeslocamento = ((string)componente.MultiplicarPorDeslocamento).ToBool();
                    comp.MultiplicarPorDiaria = ((string)componente.MultiplicarPorDiaria).ToBool();
                    comp.MultiplicarPorEntrega = ((string)componente.MultiplicarPorEntrega).ToBool();
                    comp.TipoViagem = ((string)componente.TipoViagem).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoViagemComponenteTabelaFrete>();

                    if (comp.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Tempo)
                    {
                        comp.HorasMinimasCobrancaTempo = TimeSpan.ParseExact((string)componente.HorasMinimasCobrancaTempo, @"hh\:mm", null, System.Globalization.TimeSpanStyles.None);
                        comp.MinutosArredondamentoHorasTempo = (int)componente.MinutosArredondamentoHorasTempo;
                        comp.MultiplicarPorHoraTempo = (bool)componente.MultiplicarPorHoraTempo;
                        comp.PossuiHorasMinimasCobrancaTempo = (bool)componente.PossuiHorasMinimasCobrancaTempo;
                        comp.UtilizarArredondamentoHorasTempo = (bool)componente.UtilizarArredondamentoHorasTempo;
                    }
                    else
                    {
                        comp.Tempos?.Clear();
                        comp.HorasMinimasCobrancaTempo = null;
                        comp.MinutosArredondamentoHorasTempo = null;
                        comp.MultiplicarPorHoraTempo = false;
                        comp.PossuiHorasMinimasCobrancaTempo = false;
                        comp.UtilizarArredondamentoHorasTempo = false;
                    }

                    if (componente.ValorMaximo != null)
                        comp.ValorMaximo = (decimal)componente.ValorMaximo;
                    else
                        comp.ValorMaximo = null;

                    if (componente.ValorMinimo != null)
                        comp.ValorMinimo = (decimal)componente.ValorMinimo;
                    else
                        comp.ValorMinimo = null;

                    if (componente.EntregaMinima != null)
                        comp.EntregaMinima = (int)componente.EntregaMinima;
                    else
                        comp.EntregaMinima = null;

                    int codigoJustificativa = ((string)componente.Justificativa.Codigo).ToInt();
                    comp.Justificativa = codigoJustificativa > 0 && tabelaFrete.PagamentoTerceiro ? repositorioJustificativa.BuscarPorCodigo(codigoJustificativa) : null;

                    if (comp.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Percentual)
                    {
                        comp.TipoPercentual = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete)componente.TipoPercentual;

                        if (componente.Percentual != null)
                            comp.Percentual = (decimal)componente.Percentual;
                        else
                            comp.Percentual = null;

                        if (componente.IncluirBaseCalculo != null)
                            comp.IncluirBaseCalculo = (bool)componente.IncluirBaseCalculo;
                        else
                            comp.IncluirBaseCalculo = null;
                    }
                    else
                    {
                        comp.TipoPercentual = null;
                        comp.Percentual = null;
                        comp.IncluirBaseCalculo = null;
                    }

                    if (comp.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Peso)
                    {
                        comp.TipoCalculoPeso = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete)componente.TipoCalculoPeso;

                        if (componente.Peso != null)
                            comp.Peso = (decimal)componente.Peso;
                        else
                            comp.Peso = null;

                        if (comp.TipoCalculoPeso == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete.PorValorFixoComExcedente && componente.ValorExcedentePorKG != null)
                            comp.ValorExcedentePorKG = (decimal)componente.ValorExcedentePorKG;
                        else
                            comp.ValorExcedentePorKG = null;
                    }
                    else
                    {
                        comp.TipoCalculoPeso = null;
                        comp.Peso = null;
                        comp.ValorExcedentePorKG = null;
                    }
                    if (comp.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Volume)
                    {
                        comp.TipoCalculoVolume = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete)componente.TipoCalculoVolume;

                        if (componente.Volume != null)
                            comp.Volume = (int)componente.Volume;
                        else
                            comp.Volume = null;

                        if (comp.TipoCalculoVolume == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete.PorValorFixoComExcedente && componente.ValorExcedentePorVolume != null)
                            comp.ValorExcedentePorVolume = (decimal)componente.ValorExcedentePorVolume;
                        else
                            comp.ValorExcedentePorVolume = null;
                    }
                    else
                    {
                        comp.TipoCalculoVolume = null;
                        comp.Volume = null;
                        comp.ValorExcedentePorVolume = null;
                    }
                    if (comp.TipoCalculo == TipoCalculoComponenteTabelaFrete.Cubagem)
                    {
                        comp.TipoCalculoCubagem = (TipoCalculoCubagemTabelaFreteComponenteFrete)componente.TipoCalculoCubagem;

                        if (componente.Cubagem != null)
                            comp.Cubagem = (decimal)componente.Cubagem;
                        else
                            comp.Cubagem = null;
                    }
                    else
                    {
                        comp.TipoCalculoCubagem = null;
                        comp.Cubagem = null;
                    }

                    if (comp.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos)
                    {
                        comp.TipoDocumentoQuantidadeDocumentos = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete)componente.TipoDocumentoQuantidadeDocumentos;
                        comp.ModeloDocumentoFiscalRestringirQuantidade = repositorioModeloDocumentoFiscal.BuscarPorCodigo((int)componente.ModeloDocumentoFiscalRestringirQuantidade.Codigo, false);
                    }
                    else
                    {
                        comp.TipoDocumentoQuantidadeDocumentos = null;
                        comp.ModeloDocumentoFiscalRestringirQuantidade = null;
                    }

                    comp.ValidarValorMercadoria = (bool)componente.ValidarValorMercadoria;

                    if (comp.ValidarValorMercadoria.Value)
                    {
                        if (componente.ValorMercadoriaMaximo != null)
                            comp.ValorMercadoriaMaximo = (decimal)componente.ValorMercadoriaMaximo;
                        else
                            comp.ValorMercadoriaMaximo = null;

                        if (componente.ValorMercadoriaMinimo != null)
                            comp.ValorMercadoriaMinimo = (decimal)componente.ValorMercadoriaMinimo;
                        else
                            comp.ValorMercadoriaMinimo = null;
                    }
                    else
                    {
                        comp.ValorMercadoriaMaximo = null;
                        comp.ValorMercadoriaMinimo = null;
                    }

                    comp.ValidarPesoMercadoria = (bool)componente.ValidarPesoMercadoria;

                    if (comp.ValidarPesoMercadoria.Value)
                    {
                        if (componente.PesoMercadoriaMaximo != null)
                            comp.PesoMercadoriaMaximo = (decimal)componente.PesoMercadoriaMaximo;
                        else
                            comp.PesoMercadoriaMaximo = null;

                        if (componente.PesoMercadoriaMinimo != null)
                            comp.PesoMercadoriaMinimo = (decimal)componente.PesoMercadoriaMinimo;
                        else
                            comp.PesoMercadoriaMinimo = null;
                    }
                    else
                    {
                        comp.PesoMercadoriaMaximo = null;
                        comp.PesoMercadoriaMinimo = null;
                    }

                    comp.ValidarDimensoesMercadoria = (bool)componente.ValidarDimensoesMercadoria;

                    if (comp.ValidarDimensoesMercadoria.Value)
                    {
                        if (componente.AlturaMercadoriaMinima != null)
                            comp.AlturaMercadoriaMinima = (decimal)componente.AlturaMercadoriaMinima;
                        else
                            comp.AlturaMercadoriaMinima = null;

                        if (componente.AlturaMercadoriaMaxima != null)
                            comp.AlturaMercadoriaMaxima = (decimal)componente.AlturaMercadoriaMaxima;
                        else
                            comp.AlturaMercadoriaMaxima = null;

                        if (componente.LarguraMercadoriaMinima != null)
                            comp.LarguraMercadoriaMinima = (decimal)componente.LarguraMercadoriaMinima;
                        else
                            comp.LarguraMercadoriaMinima = null;

                        if (componente.LarguraMercadoriaMaxima != null)
                            comp.LarguraMercadoriaMaxima = (decimal)componente.LarguraMercadoriaMaxima;
                        else
                            comp.LarguraMercadoriaMaxima = null;

                        if (componente.ComprimentoMercadoriaMinimo != null)
                            comp.ComprimentoMercadoriaMinimo = (decimal)componente.ComprimentoMercadoriaMinimo;
                        else
                            comp.ComprimentoMercadoriaMinimo = null;

                        if (componente.ComprimentoMercadoriaMaximo != null)
                            comp.ComprimentoMercadoriaMaximo = (decimal)componente.ComprimentoMercadoriaMaximo;
                        else
                            comp.ComprimentoMercadoriaMaximo = null;

                        if (componente.VolumeMercadoriaMinimo != null)
                            comp.VolumeMercadoriaMinimo = (decimal)componente.VolumeMercadoriaMinimo;
                        else
                            comp.VolumeMercadoriaMinimo = null;

                        if (componente.VolumeMercadoriaMaximo != null)
                            comp.VolumeMercadoriaMaximo = (decimal)componente.VolumeMercadoriaMaximo;
                        else
                            comp.VolumeMercadoriaMaximo = null;
                    }
                    else
                    {
                        comp.AlturaMercadoriaMinima = null;
                        comp.AlturaMercadoriaMaxima = null;
                        comp.LarguraMercadoriaMinima = null;
                        comp.LarguraMercadoriaMaxima = null;
                        comp.ComprimentoMercadoriaMinimo = null;
                        comp.ComprimentoMercadoriaMaximo = null;
                        comp.VolumeMercadoriaMinimo = null;
                        comp.VolumeMercadoriaMaximo = null;
                    }

                    comp.ValorUnicoParaCarga = (bool)componente.ValorUnicoParaCarga;

                    comp.ValorInformadoNaTabela = (bool)componente.ValorInformadoNaTabela;
                    comp.UtilizarCalculoDesseComponenteNaOcorrencia = (bool)componente.UtilizarCalculoDesseComponenteNaOcorrencia;

                    if (!comp.ValorInformadoNaTabela.Value && componente.ValorFormula != null)
                        comp.ValorFormula = (decimal)componente.ValorFormula;
                    else
                        comp.ValorFormula = null;

                    comp.UtilizarDiasEspecificos = (bool)componente.UtilizarDiasEspecificos;

                    if (comp.UtilizarDiasEspecificos.Value)
                    {
                        comp.SegundaFeira = (bool)componente.SegundaFeira;
                        comp.TercaFeira = (bool)componente.TercaFeira;
                        comp.QuartaFeira = (bool)componente.QuartaFeira;
                        comp.QuintaFeira = (bool)componente.QuintaFeira;
                        comp.SextaFeira = (bool)componente.SextaFeira;
                        comp.Sabado = (bool)componente.Sabado;
                        comp.Domingo = (bool)componente.Domingo;
                        comp.Feriados = (bool)componente.Feriados;
                    }
                    else
                    {
                        comp.SegundaFeira = null;
                        comp.TercaFeira = null;
                        comp.QuartaFeira = null;
                        comp.QuintaFeira = null;
                        comp.SextaFeira = null;
                        comp.Sabado = null;
                        comp.Domingo = null;
                        comp.Feriados = null;
                    }

                    comp.UtilizarPeriodoColeta = (bool)componente.UtilizarPeriodoColeta;

                    if (comp.UtilizarPeriodoColeta.Value)
                    {
                        comp.HoraColetaInicial = TimeSpan.ParseExact((string)componente.HoraColetaInicial, @"hh\:mm", null);
                        comp.HoraColetaFinal = TimeSpan.ParseExact((string)componente.HoraColetaFinal, @"hh\:mm", null);
                    }
                    else
                    {
                        comp.HoraColetaInicial = null;
                        comp.HoraColetaFinal = null;
                    }

                    comp.SomenteComDataPrevisaoEntrega = (bool)componente.SomenteComDataPrevisaoEntrega;

                }
                else
                {
                    comp.TipoViagem = null;
                    comp.TipoCalculo = null;
                    comp.UtilizarPeriodoColeta = null;
                    comp.HoraColetaInicial = null;
                    comp.HoraColetaFinal = null;
                    comp.TipoPercentual = null;
                    comp.Percentual = null;
                    comp.IncluirBaseCalculo = null;
                    comp.Peso = null;
                    comp.Cubagem = null;
                    comp.TipoCalculoPeso = null;
                    comp.TipoCalculoCubagem = null;
                    comp.UtilizarDiasEspecificos = null;
                    comp.SegundaFeira = null;
                    comp.TercaFeira = null;
                    comp.QuartaFeira = null;
                    comp.QuintaFeira = null;
                    comp.SextaFeira = null;
                    comp.Sabado = null;
                    comp.Domingo = null;
                    comp.Feriados = null;
                    comp.ValorFormula = null;
                    comp.ValorInformadoNaTabela = null;
                    comp.UtilizarCalculoDesseComponenteNaOcorrencia = null;
                    comp.ValorUnicoParaCarga = null;
                    comp.ValorMaximo = null;
                    comp.ValorMinimo = null;
                    comp.EntregaMinima = null;
                    comp.Reentrega = null;
                    comp.GerenciamentoRisco = null;
                    comp.ComponenteComparado = false;
                    comp.EscoltaArmada = null;
                    comp.Rastreado = null;
                    comp.DespachoTransitoAduaneiro = null;
                    comp.RestricaoTrafego = null;
                    comp.ModeloDocumentoFiscalRestringirQuantidade = null;
                    comp.ValidarPesoMercadoria = null;
                    comp.ValidarValorMercadoria = null;
                    comp.ValidarDimensoesMercadoria = null;
                    comp.PesoMercadoriaMinimo = null;
                    comp.PesoMercadoriaMaximo = null;
                    comp.ValorMercadoriaMinimo = null;
                    comp.ValorMercadoriaMaximo = null;
                    comp.AlturaMercadoriaMinima = null;
                    comp.AlturaMercadoriaMaxima = null;
                    comp.LarguraMercadoriaMinima = null;
                    comp.LarguraMercadoriaMaxima = null;
                    comp.ComprimentoMercadoriaMinimo = null;
                    comp.ComprimentoMercadoriaMaximo = null;
                    comp.VolumeMercadoriaMinimo = null;
                    comp.VolumeMercadoriaMaximo = null;
                    comp.Justificativa = null;
                    comp.SomenteComDataPrevisaoEntrega = null;
                }

                if (comp.Codigo > 0)
                {
                    repositorioComponenteTabela.Atualizar(comp);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = comp.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOComponente, comp.Descricao), unidadeDeTrabalho);
                        componentesAlterados = true;
                    }
                }
                else
                {
                    repositorioComponenteTabela.Inserir(comp);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOComponente, comp.Descricao), unidadeDeTrabalho);
                    componentesAlterados = true;
                }

                if (comp.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Tempo)
                {
                    if (SalvarFaixaTempoComponenteFrete(comp, componente, unidadeDeTrabalho))
                        componentesAlterados = true;
                }
            }

            return componentesAlterados;
        }

        private bool SalvarFaixaTempoComponenteFrete(Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteFreteTabelaFrete, dynamic componente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFreteTempo repositorioTempo = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFreteTempo(unitOfWork);
            dynamic tempos = typeof(string) == componente.Tempos.GetType() ? Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)componente.Tempos) : componente.Tempos;
            bool faixasTempoComponenteAlterados = false;

            if (componenteFreteTabelaFrete.Tempos != null && componenteFreteTabelaFrete.Tempos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tempo in tempos)
                {
                    int codigo = 0;
                    if (tempo.Codigo != null && int.TryParse((string)tempo.Codigo, out codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFreteTempo> temposDeletar = (from obj in componenteFreteTabelaFrete.Tempos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < temposDeletar.Count; i++)
                {
                    repositorioTempo.Deletar(temposDeletar[i]);
                    faixasTempoComponenteAlterados = true;
                }
            }

            foreach (dynamic tempo in tempos)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFreteTempo tmp = null;
                int codigo = 0;

                if (tempo.Codigo != null && int.TryParse((string)tempo.Codigo, out codigo))
                    tmp = repositorioTempo.BuscarPorCodigo(codigo, false);

                if (tmp == null)
                    tmp = new Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFreteTempo();
                else
                    tmp.Initialize();

                tmp.ComponenteFreteTabelaFrete = componenteFreteTabelaFrete;
                tmp.HoraInicial = TimeSpan.ParseExact(((string)tempo.HoraInicialTempo) + ":00", "hh\\:mm\\:ss", null, System.Globalization.TimeSpanStyles.None);
                tmp.HoraFinal = TimeSpan.ParseExact(((string)tempo.HoraFinalTempo) + ":59", "hh\\:mm\\:ss", null, System.Globalization.TimeSpanStyles.None);
                tmp.PeriodoInicial = (bool)tempo.PeriodoInicialTempo;
                tmp.Valor = Utilidades.Decimal.Converter((string)tempo.ValorTempo);

                if (componenteFreteTabelaFrete.PossuiHorasMinimasCobrancaTempo)
                {
                    if (TimeSpan.TryParseExact(((string)tempo.HoraInicialCobrancaMinimaTempo) + ":00", "hh\\:mm\\:ss", null, System.Globalization.TimeSpanStyles.None, out TimeSpan horaInicialCobrancaMinima))
                        tmp.HoraInicialCobrancaMinima = horaInicialCobrancaMinima;

                    if (TimeSpan.TryParseExact(((string)tempo.HoraFinalCobrancaMinimaTempo) + ":59", "hh\\:mm\\:ss", null, System.Globalization.TimeSpanStyles.None, out TimeSpan horaFinalCobrancaMinima))
                        tmp.HoraFinalCobrancaMinima = horaFinalCobrancaMinima;
                }
                else
                {
                    tmp.HoraInicialCobrancaMinima = null;
                    tmp.HoraFinalCobrancaMinima = null;
                }

                if (tmp.Codigo > 0)
                {
                    repositorioTempo.Atualizar(tmp);

                    if (tmp.IsChanged())
                        faixasTempoComponenteAlterados = true;
                }
                else
                {
                    repositorioTempo.Inserir(tmp);
                    faixasTempoComponenteAlterados = true;
                }
            }

            return faixasTempoComponenteAlterados;
        }

        private bool SalvarNumeroPallets(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.PalletTabelaFrete repositorioPalletTabelaFrete = new Repositorio.Embarcador.Frete.PalletTabelaFrete(unidadeDeTrabalho);
            dynamic numeroPallets = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Pallets"));
            bool numeroPalletsAlterados = false;

            if (tabelaFrete.Pallets != null && tabelaFrete.Pallets.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic numeroPallet in numeroPallets)
                {
                    if (numeroPallet.Codigo != null)
                        codigos.Add((int)numeroPallet.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete> palletsDeletar = (from obj in tabelaFrete.Pallets where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < palletsDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDePallet, palletsDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioPalletTabelaFrete.Deletar(palletsDeletar[i]);
                    numeroPalletsAlterados = true;
                }
            }

            foreach (dynamic numeroPallet in numeroPallets)
            {
                Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete numPallet = null;
                int codigo = 0;

                if (numeroPallet.Codigo != null && int.TryParse((string)numeroPallet.Codigo, out codigo))
                    numPallet = repositorioPalletTabelaFrete.BuscarPorCodigo(codigo);

                if (numPallet == null)
                    numPallet = new Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete();
                else
                    numPallet.Initialize();

                numPallet.TabelaFrete = tabelaFrete;
                numPallet.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroPalletsTabelaFrete)(int)numeroPallet.Tipo;

                if (numPallet.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroPalletsTabelaFrete.PorFaixaPallets)
                {
                    numPallet.NumeroInicialPallet = (int)numeroPallet.NumeroInicialPallet;
                    numPallet.NumeroFinalPallet = (int)numeroPallet.NumeroFinalPallet;
                }

                if (numPallet.Codigo > 0)
                {
                    repositorioPalletTabelaFrete.Atualizar(numPallet);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = numPallet.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDePallet, numPallet.Descricao), unidadeDeTrabalho);
                        numeroPalletsAlterados = true;
                    }
                }
                else
                {
                    repositorioPalletTabelaFrete.Inserir(numPallet);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDePallet, numPallet.Descricao), unidadeDeTrabalho);
                    numeroPalletsAlterados = true;
                }
            }

            return numeroPalletsAlterados;
        }

        private bool SalvarNumeroEntregas(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.NumeroEntregaTabelaFrete repositorioNumeroEntrega = new Repositorio.Embarcador.Frete.NumeroEntregaTabelaFrete(unidadeDeTrabalho);
            dynamic numeroEntregas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NumeroEntregas"));
            bool numeroEntregasAlterados = false;

            if (tabelaFrete.NumeroEntregas != null && tabelaFrete.NumeroEntregas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic numeroEntrega in numeroEntregas)
                {
                    if (numeroEntrega.Codigo != null)
                        codigos.Add((int)numeroEntrega.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete> numeroEntregasDeletar = (from obj in tabelaFrete.NumeroEntregas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < numeroEntregasDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDeEntregas, numeroEntregasDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioNumeroEntrega.Deletar(numeroEntregasDeletar[i]);
                    numeroEntregasAlterados = true;
                }
            }

            foreach (dynamic numeroEntrega in numeroEntregas)
            {
                Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete numEntrega = null;
                int codigo = 0;

                if (numeroEntrega.Codigo != null && int.TryParse((string)numeroEntrega.Codigo, out codigo))
                    numEntrega = repositorioNumeroEntrega.BuscarPorCodigo(codigo);

                if (numEntrega == null)
                    numEntrega = new Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete();
                else
                    numEntrega.Initialize();

                numEntrega.TabelaFrete = tabelaFrete;
                numEntrega.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroEntregaTabelaFrete)(int)numeroEntrega.Tipo;

                if (numEntrega.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroEntregaTabelaFrete.PorFaixaEntrega)
                {
                    numEntrega.NumeroInicialEntrega = (int)numeroEntrega.NumeroInicialEntrega;
                    numEntrega.NumeroFinalEntrega = (int)numeroEntrega.NumeroFinalEntrega;
                    numEntrega.ComAjudante = (bool)numeroEntrega.ComAjudante;
                }

                if (numEntrega.Codigo > 0)
                {
                    repositorioNumeroEntrega.Atualizar(numEntrega);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = numEntrega.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDeEntregas, numEntrega.Descricao), unidadeDeTrabalho);
                        numeroEntregasAlterados = true;
                    }
                }
                else
                {
                    repositorioNumeroEntrega.Inserir(numEntrega);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDeEntregas, numEntrega.Descricao), unidadeDeTrabalho);
                    numeroEntregasAlterados = true;
                }
            }

            return numeroEntregasAlterados;
        }

        private bool SalvarPacotes(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.PacoteTabelaFrete repositorioPacote = new Repositorio.Embarcador.Frete.PacoteTabelaFrete(unidadeDeTrabalho);
            dynamic numeroPacotes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Pacotes"));
            bool numeroPacotesAlterados = false;

            if (tabelaFrete.Pacotes != null && tabelaFrete.Pacotes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic numeroPacote in numeroPacotes)
                {
                    if (numeroPacote.Codigo != null)
                        codigos.Add((int)numeroPacote.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete> numeroPacotesDeletar = (from obj in tabelaFrete.Pacotes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < numeroPacotesDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDeEntregas, numeroPacotesDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioPacote.Deletar(numeroPacotesDeletar[i]);
                    numeroPacotesAlterados = true;
                }
            }

            foreach (dynamic numeroPacote in numeroPacotes)
            {
                Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete numPacote = null;
                int codigo = 0;

                if (numeroPacote.Codigo != null && int.TryParse((string)numeroPacote.Codigo, out codigo))
                    numPacote = repositorioPacote.BuscarPorCodigo(codigo);

                if (numPacote == null)
                    numPacote = new Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete();
                else
                    numPacote.Initialize();

                numPacote.TabelaFrete = tabelaFrete;
                numPacote.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPacoteTabelaFrete)(int)numeroPacote.Tipo;

                if (numPacote.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPacoteTabelaFrete.PorFaixaPacote)
                {
                    numPacote.NumeroInicialPacote = (int)numeroPacote.NumeroInicialPacote;
                    numPacote.NumeroFinalPacote = (int)numeroPacote.NumeroFinalPacote;
                    numPacote.ComAjudante = (bool)numeroPacote.ComAjudante;
                }

                if (numPacote.Codigo > 0)
                {
                    repositorioPacote.Atualizar(numPacote);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = numPacote.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDeEntregas, numPacote.Descricao), unidadeDeTrabalho);
                        numeroPacotesAlterados = true;
                    }
                }
                else
                {
                    repositorioPacote.Inserir(numPacote);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDeEntregas, numPacote.Descricao), unidadeDeTrabalho);
                    numeroPacotesAlterados = true;
                }
            }

            return numeroPacotesAlterados;
        }

        private bool SalvarPesosTransportados(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.PesoTabelaFrete repositorioPesoTransportado = new Repositorio.Embarcador.Frete.PesoTabelaFrete(unidadeDeTrabalho);
            Repositorio.UnidadeDeMedida repositorioUnidadeMedida = new Repositorio.UnidadeDeMedida(unidadeDeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            dynamic pesosTransportados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PesosTransportados"));
            bool pesosTransportadosAlterados = false;

            if (tabelaFrete.PesosTransportados != null && tabelaFrete.PesosTransportados.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic pesoTransportado in pesosTransportados)
                {
                    if (pesoTransportado.Codigo != null)
                        codigos.Add((int)pesoTransportado.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete> pesosTransportadosDeletar = (from obj in tabelaFrete.PesosTransportados where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < pesosTransportadosDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDeQuantiades, pesosTransportadosDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioPesoTransportado.Deletar(pesosTransportadosDeletar[i]);
                    pesosTransportadosAlterados = true;
                }
            }

            foreach (dynamic pesoTransportado in pesosTransportados)
            {
                Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete peso = null;
                int codigo = 0;

                if (pesoTransportado.Codigo != null && int.TryParse((string)pesoTransportado.Codigo, out codigo))
                    peso = repositorioPesoTransportado.BuscarPorCodigo(codigo);

                if (peso == null)
                    peso = new Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete();
                else
                    peso.Initialize();

                peso.TabelaFrete = tabelaFrete;
                peso.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete)(int)pesoTransportado.Tipo;
                peso.CalculoPeso = ((string)pesoTransportado.CalculoPeso).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorPesoTransportado>();
                peso.UnidadeMedida = repositorioUnidadeMedida.BuscarPorCodigo((int)pesoTransportado.UnidadeMedida.Codigo);
                peso.ComponenteFrete = (int)pesoTransportado.CodigoComponenteFrete > 0 ? repositorioComponenteFrete.BuscarPorCodigo((int)pesoTransportado.CodigoComponenteFrete) : null;

                int codModeloVeicular = (int)pesoTransportado.ModeloVeicularCarga.Codigo;


                if (codModeloVeicular > 0)
                    peso.ModeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codModeloVeicular);
                else
                    peso.ModeloVeicularCarga = null;


                if (pesoTransportado.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete.PorFaixaPesoTransportado)
                {
                    peso.PesoInicial = (decimal)pesoTransportado.PesoInicial;
                    peso.PesoFinal = (decimal)pesoTransportado.PesoFinal;
                    peso.ComAjudante = (bool)pesoTransportado.ComAjudante;
                    peso.ParaCalcularValorBase = (bool)pesoTransportado.ParaCalcularValorBase;
                }
                else if (peso.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete.ValorFixoPorPesoTransportado)
                {
                    peso.Peso = (decimal)pesoTransportado.Peso;
                }

                if (peso.Codigo > 0)
                {
                    repositorioPesoTransportado.Atualizar(peso);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = peso.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDeQuantiades, peso.Descricao), unidadeDeTrabalho);
                        pesosTransportadosAlterados = true;
                    }
                }
                else
                {
                    repositorioPesoTransportado.Inserir(peso);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDeQuantiades, peso.Descricao), unidadeDeTrabalho);
                    pesosTransportadosAlterados = true;
                }
            }

            return pesosTransportadosAlterados;
        }

        private bool SalvarDistancias(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.DistanciaTabelaFrete repositorioDistancia = new Repositorio.Embarcador.Frete.DistanciaTabelaFrete(unidadeDeTrabalho);
            dynamic distancias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Distancias"));
            bool distanciasAlteradas = false;

            if (tabelaFrete.Distancias != null && tabelaFrete.Distancias.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic distancia in distancias)
                {
                    if (distancia.Codigo != null)
                        codigos.Add((int)distancia.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete> distanciasDeletar = (from obj in tabelaFrete.Distancias where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < distanciasDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDeDistancia, distanciasDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioDistancia.Deletar(distanciasDeletar[i]);
                    distanciasAlteradas = true;
                }
            }

            foreach (dynamic distancia in distancias)
            {
                Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete dist = null;
                int codigo = 0;

                if (distancia.Codigo != null && int.TryParse((string)distancia.Codigo, out codigo))
                    dist = repositorioDistancia.BuscarPorCodigo(codigo);

                if (dist == null)
                    dist = new Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete();
                else
                    dist.Initialize();

                dist.TabelaFrete = tabelaFrete;
                dist.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDistanciaTabelaFrete)(int)distancia.Tipo;

                if (dist.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDistanciaTabelaFrete.PorFaixaDistanciaPercorrida)
                {
                    dist.QuilometragemInicial = (decimal)distancia.QuilometragemInicial;
                    dist.QuilometragemFinal = (decimal)distancia.QuilometragemFinal;
                    dist.MultiplicarValorDaFaixa = (bool)distancia.MultiplicarValorDaFaixa;
                    dist.MultiplicarPeloResultadoDaDistancia = (bool)distancia.MultiplicarPeloResultadoDaDistancia;
                    dist.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga = Convert.ToBoolean(distancia.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga);
                }
                else
                {
                    dist.Quilometros = (decimal)distancia.Quilometros;
                }

                if (dist.Codigo > 0)
                {
                    repositorioDistancia.Atualizar(dist);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = dist.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDeDistancia, dist.Descricao), unidadeDeTrabalho);
                        distanciasAlteradas = true;
                    }
                }
                else
                {
                    repositorioDistancia.Inserir(dist);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDeDistancia, dist.Descricao), unidadeDeTrabalho);
                    distanciasAlteradas = true;
                }
            }

            return distanciasAlteradas;
        }

        private bool SalvarTempos(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteTempo repositorioTempo = new Repositorio.Embarcador.Frete.TabelaFreteTempo(unidadeDeTrabalho);
            dynamic tempos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Tempos"));
            bool temposAlterados = false;

            if (tabelaFrete.Tempos != null && tabelaFrete.Tempos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tempo in tempos)
                {
                    if (tempo.Codigo != null)
                        codigos.Add((int)tempo.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteTempo> temposDeletar = (from obj in tabelaFrete.Tempos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < temposDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDeTempo, temposDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioTempo.Deletar(temposDeletar[i]);
                    temposAlterados = true;
                }
            }

            foreach (dynamic tempo in tempos)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteTempo tmp = null;
                int codigo = 0;

                if (tempo.Codigo != null && int.TryParse((string)tempo.Codigo, out codigo))
                    tmp = repositorioTempo.BuscarPorCodigo(codigo, false);

                if (tmp == null)
                    tmp = new Dominio.Entidades.Embarcador.Frete.TabelaFreteTempo();
                else
                    tmp.Initialize();

                tmp.TabelaFrete = tabelaFrete;
                tmp.HoraInicial = TimeSpan.ParseExact(((string)tempo.HoraInicial) + ":00", "hh\\:mm\\:ss", null, System.Globalization.TimeSpanStyles.None);
                tmp.HoraFinal = TimeSpan.ParseExact(((string)tempo.HoraFinal) + ":59", "hh\\:mm\\:ss", null, System.Globalization.TimeSpanStyles.None);
                tmp.PeriodoInicial = (bool)tempo.PeriodoInicial;

                if (tabelaFrete.PossuiHorasMinimasCobranca)
                {
                    if (TimeSpan.TryParseExact(((string)tempo.HoraInicialCobrancaMinima) + ":00", "hh\\:mm\\:ss", null, System.Globalization.TimeSpanStyles.None, out TimeSpan horaInicialCobrancaMinima))
                        tmp.HoraInicialCobrancaMinima = horaInicialCobrancaMinima;
                    if (TimeSpan.TryParseExact(((string)tempo.HoraFinalCobrancaMinima) + ":59", "hh\\:mm\\:ss", null, System.Globalization.TimeSpanStyles.None, out TimeSpan horaFinalCobrancaMinima))
                        tmp.HoraFinalCobrancaMinima = horaFinalCobrancaMinima;
                }
                else
                {
                    tmp.HoraInicialCobrancaMinima = null;
                    tmp.HoraFinalCobrancaMinima = null;
                }

                if (tmp.Codigo > 0)
                {
                    repositorioTempo.Atualizar(tmp);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = tmp.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDeTempo, tmp.Descricao), unidadeDeTrabalho);
                        temposAlterados = true;
                    }
                }
                else
                {
                    repositorioTempo.Inserir(tmp);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDeTempo, tmp.Descricao), unidadeDeTrabalho);
                    temposAlterados = true;
                }
            }

            return temposAlterados;
        }

        private bool SalvarAjudantes(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteAjudante repositorioAjudante = new Repositorio.Embarcador.Frete.TabelaFreteAjudante(unidadeDeTrabalho);
            dynamic ajudantes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Ajudantes"));
            bool ajudantesAlterados = false;

            if (tabelaFrete.Ajudantes != null && tabelaFrete.Ajudantes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic ajudante in ajudantes)
                {
                    if (ajudante.Codigo != null)
                        codigos.Add((int)ajudante.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante> ajudantesDeletar = (from obj in tabelaFrete.Ajudantes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < ajudantesDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDeAjudante, ajudantesDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioAjudante.Deletar(ajudantesDeletar[i]);
                    ajudantesAlterados = true;
                }
            }

            foreach (dynamic ajudante in ajudantes)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante aju = null;
                int codigo = 0;

                if (ajudante.Codigo != null && int.TryParse((string)ajudante.Codigo, out codigo))
                    aju = repositorioAjudante.BuscarPorCodigo(codigo, false);

                if (aju == null)
                    aju = new Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante();
                else
                    aju.Initialize();

                aju.TabelaFrete = tabelaFrete;
                aju.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaAjudanteTabelaFrete)(int)ajudante.Tipo;

                if (aju.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes)
                {
                    aju.NumeroInicial = (int)ajudante.NumeroInicial;
                    aju.NumeroFinal = (int)ajudante.NumeroFinal;
                }

                if (aju.Codigo > 0)
                {
                    repositorioAjudante.Atualizar(aju);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = aju.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDeAjudante, aju.Descricao), unidadeDeTrabalho);
                        ajudantesAlterados = true;
                    }
                }
                else
                {
                    repositorioAjudante.Inserir(aju);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDeAjudante, aju.Descricao), unidadeDeTrabalho);
                    ajudantesAlterados = true;
                }
            }

            return ajudantesAlterados;
        }

        private bool SalvarHoras(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteHora repositorioHora = new Repositorio.Embarcador.Frete.TabelaFreteHora(unidadeDeTrabalho);
            dynamic horas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Horas"));
            bool horasAlteradas = false;

            if (tabelaFrete.Horas != null && tabelaFrete.Horas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic hora in horas)
                {
                    if (hora.Codigo != null)
                        codigos.Add((int)hora.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteHora> horasDeletar = (from obj in tabelaFrete.Horas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < horasDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDeHora, horasDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioHora.Deletar(horasDeletar[i]);
                    horasAlteradas = true;
                }
            }

            foreach (dynamic hora in horas)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteHora hr = null;
                int codigo = 0;

                if (hora.Codigo != null && int.TryParse((string)hora.Codigo, out codigo))
                    hr = repositorioHora.BuscarPorCodigo(codigo, false);

                if (hr == null)
                    hr = new Dominio.Entidades.Embarcador.Frete.TabelaFreteHora();
                else
                    hr.Initialize();

                hr.TabelaFrete = tabelaFrete;
                hr.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaHoraTabelaFrete)(int)hora.Tipo;

                if (hr.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaHoraTabelaFrete.PorFaixaHora)
                {
                    hr.MinutoInicial = (int)hora.MinutoInicial * 60;
                    hr.MinutoFinal = (int)hora.MinutoFinal * 60;
                }

                if (hr.Codigo > 0)
                {
                    repositorioHora.Atualizar(hr);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = hr.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDeHora, hr.Descricao), unidadeDeTrabalho);
                        horasAlteradas = true;
                    }
                }
                else
                {
                    repositorioHora.Inserir(hr);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDeHora, hr.Descricao), unidadeDeTrabalho);
                    horasAlteradas = true;
                }
            }

            return horasAlteradas;
        }

        private bool SalvarSubcontratacoes(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.SubcontratacaoTabelaFrete repositorioSubcontratacao = new Repositorio.Embarcador.Frete.SubcontratacaoTabelaFrete(unidadeDeTrabalho);
            Repositorio.Cliente repositorioPessoa = new Repositorio.Cliente(unidadeDeTrabalho);
            dynamic subcontratacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Subcontratacoes"));
            bool subcontratacoesAlteradas = false;

            if (tabelaFrete.Subcontratacoes != null && tabelaFrete.Subcontratacoes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic subcontratacao in subcontratacoes)
                    codigos.Add((int)subcontratacao.Codigo);

                List<Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete> subcontratacoesDeletar = (from obj in tabelaFrete.Subcontratacoes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < subcontratacoesDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.RemoveuOParametroDeSubcontratacaoDo, subcontratacoesDeletar[i].Descricao), unidadeDeTrabalho);
                    repositorioSubcontratacao.Deletar(subcontratacoesDeletar[i]);
                    subcontratacoesAlteradas = true;
                }
            }

            foreach (dynamic subcontratacao in subcontratacoes)
            {
                Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete sub = null;
                int codigo = 0;

                if (subcontratacao.Codigo != null && int.TryParse((string)subcontratacao.Codigo, out codigo))
                    sub = repositorioSubcontratacao.BuscarPorCodigo(codigo);

                if (sub == null)
                    sub = new Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete();
                else
                    sub.Initialize();

                sub.TabelaFrete = tabelaFrete;
                sub.Pessoa = repositorioPessoa.BuscarPorCPFCNPJ((double)subcontratacao.Pessoa.Codigo);
                sub.PercentualDesconto = (decimal)subcontratacao.PercentualDesconto;
                sub.PercentualCobranca = (decimal)subcontratacao.PercentualCobranca;
                if (sub.Codigo > 0)
                {
                    repositorioSubcontratacao.Atualizar(sub);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = sub.GetChanges();

                    if (alteracoes.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, alteracoes, string.Format(Localization.Resources.Fretes.TabelaFrete.AlterouOParametroDeSubcontratacaoDo, sub.Descricao), unidadeDeTrabalho);
                        subcontratacoesAlteradas = true;
                    }
                }
                else
                {
                    repositorioSubcontratacao.Inserir(sub);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tabelaFrete, null, string.Format(Localization.Resources.Fretes.TabelaFrete.AdicionouOParametroDeSubcontratacaoDo, sub.Descricao), unidadeDeTrabalho);
                    subcontratacoesAlteradas = true;
                }
            }

            return subcontratacoesAlteradas;
        }

        private void SalvarContratos(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoTransporteFrete repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);

            List<int> contratos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Contratos"));

            if (tabelaFrete.ContratosTransporteFrete == null)
                tabelaFrete.ContratosTransporteFrete = new List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();
            else
            {
                List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> contratosDeletar = tabelaFrete.ContratosTransporteFrete.Where(o => !contratos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoDeletar in contratosDeletar)
                    tabelaFrete.ContratosTransporteFrete.Remove(contratoDeletar);
            }

            foreach (int contrato in contratos)
            {
                if (tabelaFrete.ContratosTransporteFrete.Any(o => o.Codigo == contrato))
                    continue;

                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoObj = repositorioContratoTransporteFrete.BuscarPorCodigo(contrato, false);
                tabelaFrete.ContratosTransporteFrete.Add(contratoObj);
            }
        }

        private void SalvarTabelaFreteRotasBidding(int codigoTabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorioBidding = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
            List<int> rotas = Request.GetListParam<int>("RotasBidding");

            foreach (int rota in rotas)
            {
                Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta rotaSelecionada = repositorioBidding.BuscarPorCodigo(rota, false);
                rotaSelecionada.CodigoTabelaFretePai = codigoTabelaFrete;
                repositorioBidding.Atualizar(rotaSelecionada);
            }
        }

        private void ValidarEntidadeVigencia(Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (vigencia.TabelaFrete == null)
                throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.TabelaDeFreteEObrigatoria);

            if (vigencia.DataInicial == DateTime.MinValue)
                throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.DataInicial);

            if (vigencia.DataFinal.HasValue && vigencia.DataFinal <= vigencia.DataInicial)
                throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.DatasDeVigenciaInvalidas);

            if (vigencia.TabelaFrete.PermitirVigenciasSobrepostas)
                return;

            if (!vigencia.TabelaFrete.Ativo)
                throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.TabelaDeFreteDeveEstarAtiva);

            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigenciaConflito = repositorioVigencia.ValidarVigenciaCompativel(vigencia.Codigo, vigencia.TabelaFrete.Codigo, vigencia.DataInicial, vigencia.DataFinal, 0);

            if (vigenciaConflito != null)
                throw new ControllerException(string.Format(Localization.Resources.Fretes.TabelaFrete.PeriodoVigenciaEntrouConflitoVigencia, vigenciaConflito.Descricao.ToLower()));
        }

        private void ValidarVigenciaContratoTransporteFrete(Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia, DateTime dataInicialContrato, DateTime dataFinalContrato)
        {
            if (dataInicialContrato != DateTime.MinValue && vigencia.DataInicial < dataInicialContrato)
                throw new ControllerException(Localization.Resources.Fretes.TabelaFrete.DataInicialVigenciaNaoPodeSerMenorDataInicialContrato);

            if (dataFinalContrato != DateTime.MinValue && vigencia.DataFinal > dataFinalContrato)
                throw new ControllerException(Localization.Resources.Fretes.TabelaFrete.DataFinalVigenciaNaoPodeSerMaiorDataFinalContrato);
        }

        private void ValidarTabelaFreteDuplicada(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaExiste = repTabelaFrete.BuscarPorGrupoPessoaFilial(tabelaFrete.Codigo, tabelaFrete.GrupoPessoas?.Codigo ?? 0, tabelaFrete.Filiais?.Select(o => o.Codigo), tabelaFrete.Transportadores?.Select(o => o.Codigo), tabelaFrete.TiposOperacao?.Select(o => o.Codigo), tabelaFrete.PagamentoTerceiro, tabelaFrete.TabelaFreteMinima, false, tabelaFrete.LocalFreeTime);

            if (tabelaFrete.Padrao)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaPadrao = repTabelaFrete.BuscarPadrao(tabelaFrete.PagamentoTerceiro);

                if (tabelaPadrao != null && tabelaPadrao.Codigo != tabelaFrete.Codigo)
                    throw new ControllerException($"Já existe uma tabela de frete padrão cadastrada ({tabelaPadrao.Descricao}).");
            }

            if (tabelaExiste != null && tabelaExiste.Codigo != tabelaFrete.Codigo)
            {
                string tipoOpera = "";
                if (tabelaExiste.TiposOperacao != null && tabelaExiste.TiposOperacao.Count > 0)
                    tipoOpera = " com o(s) tipo(s) de operação(ões) " + string.Join(", ", tabelaExiste.TiposOperacao.Select(o => o.Descricao));

                throw new ControllerException($"Já existe uma tabela de frete cadastrada para o grupo de pessoa {tabelaExiste.GrupoPessoas.Descricao}{tipoOpera}.");
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (tabelaFrete.GrupoPessoas?.Codigo ?? 0) <= 0)
            {
                string tipoOpera = "";

                tabelaExiste = repTabelaFrete.BuscarSemGrupoPessoaECodigoDiff(tabelaFrete.Codigo, tabelaFrete.TiposOperacao.Select(o => o.Codigo), tabelaFrete.TransportadoresTerceiros.Select(o => o.CPF_CNPJ), tabelaFrete.PagamentoTerceiro, tabelaFrete.TabelaFreteMinima, false, tabelaFrete.TiposTerceiros.Select(o => o.Codigo));

                if (tabelaExiste != null)
                {
                    if (tabelaExiste.TiposOperacao != null && tabelaExiste.TiposOperacao.Count > 0)
                        tipoOpera = " com o(s) tipo(s) de operação(ões) " + string.Join(", ", tabelaExiste.TiposOperacao.Select(o => o.Descricao));

                    if (tabelaFrete.PagamentoTerceiro && tabelaExiste.TransportadoresTerceiros?.Count > 0)
                        tipoOpera += $"{(string.IsNullOrWhiteSpace(tipoOpera) ? "" : " e ")} com o(s) transportador(es) terceiro(s) {string.Join(", ", tabelaExiste.TransportadoresTerceiros.Select(o => o.Descricao))}";

                    throw new ControllerException($"Já existe uma tabela de frete padrão cadastrada/ativa ({tabelaExiste.Descricao}){tipoOpera}.");
                }
            }
        }

        private void ValidarValoresParametrosTabelasClientes(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);

            if (repositorioTabelaFreteCliente.PossuiParametroTipoCargaComValorPorTabelaFrete(tabelaFrete.Codigo))
                throw new ServicoException("Não é possível excluir Tipo de Carga porque há Tabela(s) de Cliente(s) com valores informados.");
        }

        #endregion
    }
}
