using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.TorreControle
{
    [CustomAuthorize("TorreControle/QualidadeEntrega")]
    public class QualidadeEntregaController : BaseController
    {
        #region Construtores

        public QualidadeEntregaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork, cancellationToken));
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
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork, cancellationToken);
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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarConfiguracoesQualidadeEntrega(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega repositorioConfiguracaoQualidadeEntrega = new Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega(unitOfWork, cancellationToken);

                bool dataConfirmacaoIntervaloRaio = Request.GetBoolParam("VerificarDataConfirmacaoIntervaloRaio");
                bool considerarHora = Request.GetBoolParam("ConsiderarDataHoraConfirmacaoIntervaloRaio");
                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega = await repositorioConfiguracaoQualidadeEntrega.BuscarConfiguracaoPadraoAsync();

                if (configuracaoQualidadeEntrega == null)
                {
                    configuracaoQualidadeEntrega = new Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega()
                    {
                        VerificarDataConfirmacaoIntervaloRaio = dataConfirmacaoIntervaloRaio,
                        ConsiderarDataHoraConfirmacaoIntervaloRaio = considerarHora
                    };

                    await repositorioConfiguracaoQualidadeEntrega.InserirAsync(configuracaoQualidadeEntrega, Auditado);
                }
                else
                {
                    configuracaoQualidadeEntrega.Initialize();

                    configuracaoQualidadeEntrega.VerificarDataConfirmacaoIntervaloRaio = dataConfirmacaoIntervaloRaio;
                    configuracaoQualidadeEntrega.ConsiderarDataHoraConfirmacaoIntervaloRaio = considerarHora;

                    await repositorioConfiguracaoQualidadeEntrega.AtualizarAsync(configuracaoQualidadeEntrega, Auditado);
                }
                await unitOfWork.CommitChangesAsync();
                return new JsonpResult(true, "Configuração salva com sucesso.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o registro.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> BuscarConfiguracoesQualidadeEntrega(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega repositorioConfiguracaoQualidadeEntrega = new Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega = await repositorioConfiguracaoQualidadeEntrega.BuscarConfiguracaoPadraoAsync();

                return new JsonpResult(new
                {
                    Codigo = configuracaoQualidadeEntrega?.Codigo ?? 0,
                    VerificarDataConfirmacaoIntervaloRaio = configuracaoQualidadeEntrega?.VerificarDataConfirmacaoIntervaloRaio ?? false,
                    ConsiderarDataHoraConfirmacaoIntervaloRaio = configuracaoQualidadeEntrega?.ConsiderarDataHoraConfirmacaoIntervaloRaio ?? false
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o registro.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterResumoQualidadeEntregas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaQualidadeEntrega filtroPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.TorreControle.ResumoConsultaQualidadeEntrega resumoConsultaQualidadeEntregas = repositorioCargaEntrega.ObterResumoQualidadeEntregas(filtroPesquisa, TipoConsultaQualidadeEntrega.Totalizador).FirstOrDefault();

                return new JsonpResult(new
                {
                    resumoConsultaQualidadeEntregas.QtdNaoDisponivelParaConsulta,
                    resumoConsultaQualidadeEntregas.QtdDisponivelParaConsulta,
                    resumoConsultaQualidadeEntregas.QtdTotalParaConsulta
                });
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> LiberarNotaParaConsulta(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCanhoto = Request.GetIntParam("CodigoCanhoto");
                string mensagemAuditoria = "Canhoto/Nota liberada para consulta.";

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = await repositorioCanhoto.BuscarPorCodigoAsync(codigoCanhoto, false);
                if (canhoto == null)
                    throw new ControllerException("Canhoto não encontrado para liberação da nota.");

                await unitOfWork.StartAsync();
                canhoto.Initialize();

                canhoto.DisponivelParaConsulta = true;
                canhoto.DigitalizacaoIntegrada = false;

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, canhoto, mensagemAuditoria, unitOfWork, cancellationToken);
                Servicos.Embarcador.Canhotos.Canhoto.GerarHistoricoCanhotos(new() { canhoto }, this.Usuario, mensagemAuditoria, unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true, mensagemAuditoria);
            }

            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao liberar canhoto.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> LiberarMultiplasNotasParaConsulta(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string mensagemAuditoria = "Canhoto/Nota liberada para consulta.";

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosSelecionados = await ObterCanhotosSelecionados(unitOfWork, cancellationToken);

                if (canhotosSelecionados.Count == 0)
                    return new JsonpResult(false, false, "Não foi possivel encontrar os canhotos");

                await unitOfWork.StartAsync();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosSelecionados)
                {
                    canhoto.Initialize();

                    canhoto.DisponivelParaConsulta = true;
                    canhoto.DigitalizacaoIntegrada = false;

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, canhoto, mensagemAuditoria, unitOfWork, cancellationToken);
                    Servicos.Embarcador.Canhotos.Canhoto.GerarHistoricoCanhotos(new() { canhoto }, this.Usuario, mensagemAuditoria, unitOfWork);

                }
                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true, "Notas liberadas para consulta com sucesso.");
            }

            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao liberar canhotos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterImagemCanhoto()
        {
            int codigoCanhoto = Request.GetIntParam("CodigoCanhoto");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Canhotos.Canhoto repCargaEntrega = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto srvCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaEntrega.BuscarPorCodigo(codigoCanhoto);

            if (canhoto == null)
                return new JsonpResult(false, true, "Não foi possível encontrar o canhoto.");

            dynamic miniatura = null;
            bool isPDF = canhoto.IsPDF();

            if (!isPDF)
                miniatura = srvCanhoto.ObterMiniatura(canhoto, unitOfWork);

            var retorno = new
            {
                CodigoCanhoto = canhoto.Codigo,
                Miniatura = miniatura,
                ArquivoPDF = isPDF
            };

            return new JsonpResult(retorno);
        }

        #endregion

        #region Métodos Privados
        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("CodigoCargaEntrega", false);
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoCanhoto", false);
            grid.AdicionarCabecalho("DisponivelParaConsulta", false);
            grid.AdicionarCabecalho("DT_RowId", false);
            grid.AdicionarCabecalho("Data Entrada no Raio", "DataEntradaRaioFormatada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Saída no raio", "DataSaidaRaioFormatada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Número NF", "NumeroNotaFiscal", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Série", "SerieNotaFiscal", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Emissão NF", "DataEmissaoNotaFiscalFormatada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Chave", "ChaveNotaFiscal", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Transportador", "DescricaoTransportadorFormatada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo de Carga", "DescricaoTipoDeCarga", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Destinatário", "DescricaoDestinatarioFormatada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Protocolo Carga", "ProtocoloCarga", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Filial", "DescricaoFilialFormatada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação Canhoto", "DescricaoSituacaoCanhoto", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação digitalização Canhoto", "DescricaoSituacaoDigitalizacaoCanhoto", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Digitalização Canhoto", "DataDigitalizacaoCanhotoFormatada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Entrega no Cliente", "DataEntregaClienteFormatada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação NF", "DescricaoSituacaoNotaFiscal", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Centro de Resultado", "CentroResultado", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Origem", "OrigemDescricao", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Destino", "DestinoDescricao", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Escritório de Vendas", "EscritorioVendas", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Matriz de Vendas", "MatrizVendas", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo NF Integrada", "DescricaoTipoNotaFiscalIntegrada", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação Pagamento Canhoto", "DescricaoSituacaoPgtoCanhoto", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação Liberação Canhoto", "DescricaoDisponivelParaConsulta", 2, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Entrega Original", "DataEntregaOriginalFormatada", 2, Models.Grid.Align.left, false);

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "QualidadeEntrega/Pesquisa", "grid-qualidade-entrega");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaQualidadeEntrega filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            int totalRegistros = repositorioCargaEntrega.ContarConsultaCargaEntregaQualidadeEntrega(filtroPesquisa);
            IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega> consultaQualidadeEntregas = totalRegistros > 0 ? repositorioCargaEntrega.ConsultarCargaEntregaQualidadeEntrega(filtroPesquisa, parametrosConsulta, TipoConsultaQualidadeEntrega.Listagem) : new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega>();

            var listaRetornar = (
                from entrega in consultaQualidadeEntregas
                select new
                {
                    entrega.CodigoCargaEntrega,
                    Codigo = entrega.CodigoNotaFiscal,
                    entrega.CodigoCanhoto,
                    entrega.DataEntradaRaioFormatada,
                    entrega.DataSaidaRaioFormatada,
                    entrega.SerieNotaFiscal,
                    entrega.NumeroNotaFiscal,
                    entrega.DataEmissaoNotaFiscalFormatada,
                    entrega.ChaveNotaFiscal,
                    entrega.NumeroCarga,
                    entrega.DescricaoTransportadorFormatada,
                    entrega.DescricaoTipoDeCarga,
                    entrega.DescricaoDestinatarioFormatada,
                    entrega.ProtocoloCarga,
                    entrega.DescricaoFilialFormatada,
                    entrega.DescricaoSituacaoCanhoto,
                    entrega.DescricaoSituacaoDigitalizacaoCanhoto,
                    entrega.DataDigitalizacaoCanhotoFormatada,
                    entrega.DataEntregaClienteFormatada,
                    entrega.DescricaoSituacaoNotaFiscal,
                    entrega.CentroResultado,
                    entrega.OrigemDescricao,
                    entrega.DestinoDescricao,
                    entrega.EscritorioVendas,
                    entrega.MatrizVendas,
                    entrega.DescricaoTipoNotaFiscalIntegrada,
                    entrega.DescricaoSituacaoPgtoCanhoto,
                    entrega.DescricaoDisponivelParaConsulta,
                    entrega.DisponivelParaConsulta,
                    DT_RowId = Guid.NewGuid(),
                    entrega.DataEntregaOriginalFormatada
                }
            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }
        private FiltroPesquisaQualidadeEntrega ObterFiltrosPesquisa()
        {
            return new FiltroPesquisaQualidadeEntrega()
            {
                Filiais = Request.GetListParam<int>("Filial"),
                Carga = Request.GetStringParam("NumeroCarga"),
                NumeroNF = Request.GetIntParam("NumeroNF"),
                DataInicioCriacaoCarga = Request.GetDateTimeParam("DataInicioCriacaoCarga"),
                DataFimCriacaoCarga = Request.GetDateTimeParam("DataFimCriacaoCarga"),
                DataInicioEmissaoNF = Request.GetDateTimeParam("DataInicioEmissaoNF"),
                DataFimEmissaoNF = Request.GetDateTimeParam("DataFimEmissaoNF"),
                SituacaoDigitalizacaoCanhoto = Request.GetEnumParam<SituacaoDigitalizacaoCanhoto>("SituacaoDigitalizacaoCanhoto"),
                TipoCanhoto = Request.GetEnumParam<TipoCanhoto>("TipoCanhoto"),
                DisponivelParaConsulta = Request.GetNullableBoolParam("DisponivelParaConsulta")
            };
        }

        private async Task<List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>> ObterCanhotosSelecionados(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new(unitOfWork, cancellationToken);
            var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

            List<int> listaCodigosCanhotos = new List<int>();

            foreach (var item in listaItensSelecionados)
            {
                listaCodigosCanhotos.Add((int)item.CodigoCanhoto);
            }
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = await repositorioCanhoto.BuscarPorCodigosAsync(listaCodigosCanhotos);

            return canhotos.ToList();
        }
        #endregion

    }
}
