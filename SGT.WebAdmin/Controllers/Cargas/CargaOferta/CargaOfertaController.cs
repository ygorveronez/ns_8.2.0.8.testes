using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.CargaOferta
{
    [CustomAuthorize("Cargas/CargaOferta")]
    public class CargaOfertaController : BaseController
    {
        #region Construtores

        public CargaOfertaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridPesquisa(cancellationToken));
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
                Grid grid = await ObterGridPesquisa(cancellationToken);
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

        public async Task<IActionResult> ConsultarIntegracoes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigoCargaOferta = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaOfertaIntegracao repCargaOfertaIntegracao = new(unitOfWork, cancellationToken);

                Grid grid = new Grid(Request);

                grid.header = new List<Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 20, Align.left, false);
                grid.AdicionarCabecalho("Integração", "Integracao", 20, Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 20, Align.left, false);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 40, Align.left, false);
                grid.AdicionarCabecalho("Número de Tentativas", "NumeroTentativas", 20, Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "ProblemaIntegracao", 40, Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao> integracoes = await repCargaOfertaIntegracao.BuscarIntegracaoPorCargaOfertaAsync(codigoCargaOferta, cancellationToken);
                grid.setarQuantidadeTotal(integracoes.Count);

                var retorno = (from obj in integracoes.OrderByDescending(o => o.DataIntegracao).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   DataIntegracao = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                   Integracao = obj.TipoIntegracao.DescricaoTipo,
                                   Tipo = obj.Tipo.ObterDescricao(),
                                   SituacaoIntegracao = obj.SituacaoIntegracao.ObterDescricao(),
                                   obj.NumeroTentativas,
                                   obj.ProblemaIntegracao,
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaIntegracao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaOfertaIntegracaoArquivos repCargaOfertaIntegracaoArquivos = new(unitOfWork, cancellationToken);

                Grid grid = new Grid(Request);

                grid.header = new List<Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracaoArquivos> arquivos = await repCargaOfertaIntegracaoArquivos.BuscarArquivosPorCodigoIntegracaoAsync(codigoCargaIntegracao);
                grid.setarQuantidadeTotal(arquivos.Count);

                var retorno = (from obj in arquivos.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DownloadArquivosIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoArquivo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaOfertaIntegracaoArquivos repCargaOfertaIntegracaoArquivos = new(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracaoArquivos arquivoIntegracao = await repCargaOfertaIntegracaoArquivos.BuscarPorCodigoAsync(codigoArquivo, false);
                Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao integracao = arquivoIntegracao.CargaOfertaIntegracao;

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado);

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NaoHaArquivosDisponiveisParaDownload);

                List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> arquivos = new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>()
                {
                    arquivoIntegracao.ArquivoRequisicao,
                    arquivoIntegracao.ArquivoResposta
                };

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(arquivos);

                return Arquivo(arquivo, "application/zip", $"Arquivos Integração - {integracao.TipoIntegracao.DescricaoTipo}.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Ofertar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Cargas.CargaOferta servicoCargaOferta = new(unitOfWork, TipoServicoMultisoftware);
            List<long> CodigosCargasOfertas = Request.GetListParam<long>("Codigos");

            int quantidadeCriada = 0;
            List<string> mensagensErro = new List<string>();
            try
            {
                (quantidadeCriada, mensagensErro) = await servicoCargaOferta.AtualizarSituacaoAsync(CodigosCargasOfertas, SituacaoCargaOferta.EmOferta, cancellationToken, Auditado);

                if (mensagensErro.Count == 0)
                    return new JsonpResult(true, true, $"Foram criados {quantidadeCriada} registros.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagensErro.Add("Ocorreu uma falha ao ofertar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

            return new JsonpResult(false, true, $"Foram criados {quantidadeCriada} registros. Erros: {string.Join(" | ", mensagensErro)}");
        }

        public async Task<IActionResult> Cancelar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Cargas.CargaOferta servicoCargaOferta = new(unitOfWork, TipoServicoMultisoftware);
            List<long> CodigosCargasOfertas = Request.GetListParam<long>("Codigos");

            int quantidadeCriada = 0;
            List<string> mensagensErro = new List<string>();

            try
            {
                (quantidadeCriada, mensagensErro) = await servicoCargaOferta.AtualizarSituacaoAsync(CodigosCargasOfertas, SituacaoCargaOferta.Cancelada, cancellationToken, Auditado);

                if (mensagensErro.Count == 0)
                    return new JsonpResult(true, true, $"Foram criados {quantidadeCriada} registros.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

            return new JsonpResult(false, true, $"Foram criados {quantidadeCriada} registros. Erros: {string.Join(" | ", mensagensErro)}");
        }

        #endregion Métodos Globais

        #region Métodos privados

        private async Task<Grid> ObterGridPesquisa(CancellationToken cancellationToken)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.CargaOferta repositorioCargaOferta = new Repositorio.Embarcador.Cargas.CargaOferta(unitOfWork, cancellationToken);

            Grid grid = ObterGridPesquisaCargaOferta();

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaOferta.FiltroPesquisaCargaOferta filtrosPesquisa = ObterFiltrosPesquisa();
            int totalLinhas = await repositorioCargaOferta.ContarConsulta(filtrosPesquisa);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "CargaOferta/Pesquisa", "grid-carga-oferta");
            Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciasGrid = gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo);
            grid.AplicarPreferenciasGrid(preferenciasGrid);

            if (totalLinhas == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoCargaOferta.CargaOferta> ofertas = await repositorioCargaOferta.ConsultarAsync(filtrosPesquisa, parametrosConsulta);

            grid.AdicionaRows(ofertas);
            grid.setarQuantidadeTotal(totalLinhas);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.CargaOferta.FiltroPesquisaCargaOferta ObterFiltrosPesquisa()
        {

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaOferta.FiltroPesquisaCargaOferta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaOferta.FiltroPesquisaCargaOferta()
            {
                CodigosFiliais = Request.GetListParam<int>("Filiais"),
                CodigosTiposCarga = Request.GetListParam<int>("TiposCarga"),
                CodigosTransportadores = Request.GetListParam<int>("Transportadores"),
                CodigosCarga = Request.GetListParam<int>("CodigosCarga"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataFim = Request.GetNullableDateTimeParam("DataFim"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao"),
                SituacaoOferta = Request.GetNullableEnumParam<SituacaoCargaOferta>("SituacaoOferta"),
            };
            return filtrosPesquisa;
        }

        private Grid ObterGridPesquisaCargaOferta()
        {

            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoCarga", false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", 10, Align.center, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 10, Align.left, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 10, Align.left, false);
            grid.AdicionarCabecalho("Transportadores", "Transportadores", 10, Align.left, false);
            grid.AdicionarCabecalho("Destino", "Destino", 10, Align.left, false);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Align.left, false);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFreteFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Quilometragem", "QuilometragemFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Data Carregamento", "DataCarregamentoFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Data Entrega", "DataPrevisaoEntregaFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Placa", "Placa", 10, Align.left, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Align.left, true);
            grid.AdicionarCabecalho("Data da Oferta", "DataOfertaFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Data de Aceite", "DataOfertaAceiteFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Data fim Oferta", "DataFimOferta", 10, Align.left, true);
            grid.AdicionarCabecalho("Situação da Carga", "SituacaoCargaFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Situação da Oferta", "SituacaoCargaOfertaFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("Situação da Integração", "SituacaoIntegracaoFormatada", 10, Align.left, true);
            grid.AdicionarCabecalho("SituacaoCarga", false);
            grid.AdicionarCabecalho("SituacaoCargaOferta", false);
            grid.AdicionarCabecalho("SituacaoIntegracao", false);

            return grid;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion Métodos privados
    }
}
