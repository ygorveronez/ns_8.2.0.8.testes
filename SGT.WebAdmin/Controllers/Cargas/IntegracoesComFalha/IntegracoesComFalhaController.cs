using System.IO.Compression;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.CargaOferta
{
    [CustomAuthorize(new string[] { "Cargas/IntegracoesComFalha" })]
    public class IntegracoesComFalhaController : BaseController
    {
        #region Construtores

        public IntegracoesComFalhaController(Conexao conexao) : base(conexao) { }

        internal class ItemSelecionado
        {
            public int Codigo { get; set; }
            public string TabelaOrigem { get; set; }
        }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridIntegracoesPesquisa(cancellationToken));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            try
            {
                Grid grid = await ObterGridIntegracoesPesquisa(cancellationToken);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

                return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
        }

        public async Task<IActionResult> DownloadArquivosIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.CargaIntegracaoDadosTransportes repositorioIntegracaoDadosTransporte = new(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioIntegracaoFrete = new(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioIntegracaoCargaCarga = new(unitOfWork);

            try
            {
                List<ItemSelecionado> itensSelecionados = Request.GetListParam<ItemSelecionado>("ListaSelecionados");

                List<int> codigosDadosTransporte = new();
                List<int> codigosFrete = new();
                List<int> codigosCargaCarga = new();

                foreach (var item in itensSelecionados)
                {
                    switch (item.TabelaOrigem)
                    {
                        case "CDI": codigosDadosTransporte.Add(item.Codigo); break;
                        case "CFI": codigosFrete.Add(item.Codigo); break;
                        case "CAI": codigosCargaCarga.Add(item.Codigo); break;
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> integracoesDadosTransporte = repositorioIntegracaoDadosTransporte.BuscarPorCodigos(codigosDadosTransporte, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> integracoesFrete = repositorioIntegracaoFrete.BuscarPorCodigos(codigosFrete, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> integracoesCargaCarga = repositorioIntegracaoCargaCarga.BuscarPorCodigos(codigosCargaCarga, false);
                List<FileContentResult> zips = new List<FileContentResult>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoCDI in integracoesDadosTransporte)
                {
                    int codigo = integracaoCDI.Codigo;

                    Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new(unitOfWork, cancellationToken);

                    zips.Add(ProcessarIntegracao(codigo, codigo => repCargaDadosTransporteIntegracao.BuscarPorCodigo(codigo), integracao => integracao.ArquivosTransacao.SelectMany(item => new[] { item.ArquivoRequisicao, item.ArquivoResposta }).Where(item => item != null).ToList(), integracao => integracao.TipoIntegracao.DescricaoTipo + integracao.Carga.CodigoCargaEmbarcador));
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao integracaoCFI in integracoesFrete)
                {
                    int codigo = integracaoCFI.Codigo;

                    Repositorio.Embarcador.Cargas.CargaFreteIntegracao repCargaFreteIntegracao = new(unitOfWork, cancellationToken);

                    zips.Add(ProcessarIntegracao(codigo, codigo => repCargaFreteIntegracao.BuscarPorCodigo(codigo, false), integracao => integracao.ArquivosTransacao.SelectMany(item => new[] { item.ArquivoRequisicao, item.ArquivoResposta }).Where(item => item != null).ToList(), integracao => integracao.TipoIntegracao.DescricaoTipo + integracao.Carga.CodigoCargaEmbarcador));
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoCAI in integracoesCargaCarga)
                {
                    int codigo = integracaoCAI.Codigo;

                    Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new(unitOfWork, cancellationToken);

                    zips.Add(ProcessarIntegracao(codigo, codigo => repCargaCargaIntegracao.BuscarPorCodigo(codigo, false), integracao => integracao.ArquivosTransacao.SelectMany(item => new[] { item.ArquivoRequisicao, item.ArquivoResposta }).Where(item => item != null).ToList(), integracao => integracao.TipoIntegracao.DescricaoTipo + integracao.Carga.CodigoCargaEmbarcador));
                }

                byte[] arquivoFinal = CriarZipFinal(zips);

                return Arquivo(arquivoFinal, "application/zip", $"Arquivos Integração - {"IntegracoesComFalhaArquivos"}.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Reenviar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoDadosTransportes repositorioIntegracaoDadosTransporte = new(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioIntegracaoFrete = new(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioIntegracaoCargaCarga = new(unitOfWork);

            try
            {
                List<ItemSelecionado> itensSelecionados = Request.GetListParam<ItemSelecionado>("ListaSelecionados");

                if (itensSelecionados.Count == 0 || itensSelecionados.Any(item => item.Codigo == 0))
                    return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.Falha);

                List<int> codigosIntegracoesDadosTransporte = new();
                List<int> codigosIntegracoesFrete = new();
                List<int> codigosIntegracoesCargaCarga = new();

                foreach (ItemSelecionado item in itensSelecionados)
                {
                    switch (item.TabelaOrigem)
                    {
                        case "CDI":
                            codigosIntegracoesDadosTransporte.Add(item.Codigo);
                            break;
                        case "CFI":
                            codigosIntegracoesFrete.Add(item.Codigo);
                            break;
                        case "CAI":
                            codigosIntegracoesCargaCarga.Add(item.Codigo);
                            break;
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> integracoesDadosTransporte = repositorioIntegracaoDadosTransporte.BuscarPorCodigos(codigosIntegracoesDadosTransporte, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> integracoesFrete = repositorioIntegracaoFrete.BuscarPorCodigos(codigosIntegracoesFrete, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> integracoesCargaCarga = repositorioIntegracaoCargaCarga.BuscarPorCodigos(codigosIntegracoesCargaCarga, false);

                await unitOfWork.StartAsync(cancellationToken);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracao in integracoesDadosTransporte)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.ProblemaIntegracao = "";

                    await repositorioIntegracaoDadosTransporte.AtualizarAsync(integracao);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao integracao in integracoesFrete)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.ProblemaIntegracao = "";

                    await repositorioIntegracaoFrete.AtualizarAsync(integracao);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao in integracoesCargaCarga)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.ProblemaIntegracao = "";

                    await repositorioIntegracaoCargaCarga.AtualizarAsync(integracao);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true, true, Localization.Resources.Gerais.Geral.Sucesso);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.Falha);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Métodos Globais

        #region Métodos privados

        private async Task<Grid> ObterGridIntegracoesPesquisa(CancellationToken cancellationToken)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioIntegracaoCarga = new(unitOfWork, cancellationToken);

            Grid grid = ObterGridPesquisaIntegracoes(unitOfWork, cancellationToken);

            Dominio.ObjetosDeValor.Embarcador.Carga.IntegracoesComFalha.FiltroPesquisaIntegracoesComFalha filtrosPesquisa = ObterFiltrosPesquisa();
            int totalLinhas = await repositorioIntegracaoCarga.ContarConsultaGridIntegracoesFalhaAsync(filtrosPesquisa);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "IntegracoesComFalha/Pesquisa", "grid-integracoes_falhas"); //alterar
            Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciasGrid = gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo);
            grid.AplicarPreferenciasGrid(preferenciasGrid);

            if (totalLinhas == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoIntegracoesComFalha.IntegracoesComFalha> integracoesComFalhas = await repositorioIntegracaoCarga.PreencherGridIntegracoesFalhaAsync(filtrosPesquisa, parametrosConsulta);

            grid.AdicionaRows(integracoesComFalhas);
            grid.setarQuantidadeTotal(totalLinhas);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.IntegracoesComFalha.FiltroPesquisaIntegracoesComFalha ObterFiltrosPesquisa()
        {

            Dominio.ObjetosDeValor.Embarcador.Carga.IntegracoesComFalha.FiltroPesquisaIntegracoesComFalha filtrosPesquisa = new()
            {
                CodigosCarga = Request.GetListParam<int>("CodigosCarga"),
                DataInicial = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                EtapaCarga = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>("EtapaCarga"),
                TipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("TipoIntegracao"),
            };
            return filtrosPesquisa;
        }

        private Grid ObterGridPesquisaIntegracoes(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {

            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("TabelaOrigem", false);
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoCarga", false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", 10, Align.center, false);
            grid.AdicionarCabecalho("Etapa", "EtapaCargaFormatada", 10, Align.center, false);
            grid.AdicionarCabecalho("Data", "DataIntegracaoFormatada", 10, Align.center, false);
            grid.AdicionarCabecalho("Integração", "TipoIntegracaoFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Situação da Integração", "SituacaoIntegracaoFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Mensagem", "MensagemRetorno", 10, Align.center, false);
            grid.AdicionarCabecalho("TipoIntegracao", false);
            grid.AdicionarCabecalho("SituacaoIntegracao", false);

            return grid;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        private FileContentResult ProcessarIntegracao<T>(
            int codigo,
            Func<int, T> buscarIntegracao,
            Func<T, List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>> extrairArquivos,
            Func<T, string> obterNomeTipo
             )
        {
            var integracao = buscarIntegracao(codigo);

            if (integracao == null)
                return null;

            List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> arquivos = extrairArquivos(integracao);
            if (arquivos == null || arquivos.Count == 0)
                return null;

            byte[] arquivoZip = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(arquivos);
            return Arquivo(arquivoZip, "application/zip", $"Arquivos Integração - {obterNomeTipo(integracao)}.zip");
        }

        private byte[] CriarZipFinal(List<FileContentResult> zipsIndividuais)
        {
            if (zipsIndividuais == null || zipsIndividuais.Count == 0)
                return null;

            using (var zipFinalStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipFinalStream, ZipArchiveMode.Create, true))
                {
                    foreach (var zipResult in zipsIndividuais)
                    {
                        if (zipResult?.FileContents == null || zipResult.FileContents.Length == 0)
                            continue;

                        string nomeEntrada = !string.IsNullOrWhiteSpace(zipResult.FileDownloadName)
                            ? zipResult.FileDownloadName
                            : $"Arquivo_{Path.GetRandomFileName()}.zip";

                        var zipEntry = zipArchive.CreateEntry(nomeEntrada);

                        using (var entryStream = zipEntry.Open())
                        using (var zipContentStream = new MemoryStream(zipResult.FileContents))
                        {
                            zipContentStream.CopyTo(entryStream);
                        }
                    }
                }

                return zipFinalStream.ToArray();
            }
        }

        #endregion Métodos privados
    }
}
