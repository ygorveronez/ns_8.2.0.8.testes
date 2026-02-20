using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao
{
    [CustomAuthorize("Cargas/ControleGeracaoEDI", "Cargas/Carga")]
    public class ControleGeracaoEDIController : BaseController
    {
        #region Construtores

        public ControleGeracaoEDIController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivoEDI()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (codigo > 0)
                {
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "Processados");
                    Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                    Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI integracao = repControleIntegracao.BuscarPorCodigo(codigo);

                    if (integracao != null)
                    {

                        if (Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminho, integracao.GuidArquivo)))
                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, integracao.GuidArquivo);
                        else
                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, integracao.NomeArquivo);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        {
                            byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                            if (data != null)
                            {
                                return Arquivo(data, "text/txt", integracao.NomeArquivo);
                            }
                        }
                    }
                }
                return new JsonpResult(false, false, Localization.Resources.Cargas.ControleGeracaoEDI.EDINaoEncontradoAtualizePaginaTenteNovamente);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaRealizarDownloadEDI);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string pasta = Utilidades.IO.FileStorageService.Storage.Combine("FTP", "Enviados", "Notfis");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain repositorioConfiguracaoFTPSaintGobain = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFTPAmazon repositorioConfiguracaoFTPAmazon = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFTPAmazon(unitOfWork);

                Servicos.Embarcador.Integracao.SaintGobain.FTPSaintGobain servicoFTPSaintGobain = new Servicos.Embarcador.Integracao.SaintGobain.FTPSaintGobain(unitOfWork, TipoServicoMultisoftware);
                Servicos.Embarcador.Integracao.FTPAmazon.FTPAmazon servicoFTPAmazon = new Servicos.Embarcador.Integracao.FTPAmazon.FTPAmazon(unitOfWork, TipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain configuracaoFTPSaintGobain = repositorioConfiguracaoFTPSaintGobain.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPAmazon configuracaoFTPAmazon = repositorioConfiguracaoFTPAmazon.BuscarPrimeiroRegistro();

                if (configuracaoFTPSaintGobain != null || configuracaoFTPAmazon != null)
                    pasta = "Processados";

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, pasta);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                List<string> erros = new List<string>();
                List<string> extensoesValidas = new List<string>() { ".txt", ".xml" };
                int adicionados = 0;

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleGeracaoEDI.NenhumArquivoSelecionadoParaEnvio);

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (!extensaoArquivo.Contains(extensaoArquivo))
                    {
                        erros.Add(string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.ExtensaoNaoPermitida, extensaoArquivo));
                        continue;
                    }
                    try
                    {
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                        StreamReader readerArquivo = null;
                        if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().CodificacaoEDI == "UTF8")
                            readerArquivo = new StreamReader(file.InputStream, Encoding.UTF8);
                        else
                            readerArquivo = new StreamReader(file.InputStream, Encoding.GetEncoding("iso-8859-1"));

                        Utilidades.IO.FileStorageService.Storage.WriteAllText(Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo), readerArquivo.ReadToEnd());

                        if (configuracaoFTPSaintGobain != null)
                            servicoFTPSaintGobain.GerarRegistroEDI(Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo));

                        if (configuracaoFTPAmazon != null)
                            GerarRegistroEDI(new List<string>() { Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo) }, unitOfWork);

                        readerArquivo.Dispose();

                        adicionados++;
                    }
                    catch (Exception e)
                    {
                        erros.Add(string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.ErroProcessarArquivo, file.FileName));
                        Servicos.Log.TratarErro(e);
                    }
                }

                return new JsonpResult(new
                {
                    Adicionados = adicionados,
                    Erros = erros
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaEnvioArquivo);
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
                var grid = ObterGridPesquisa(isExportarPesquisa: true);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(status: false, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaGerarArquivo);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(status: false, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa(isExportarPesquisa: false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(status: false, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDetalheAlteracao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao(unitOfWork);
                var integracaoAlteracoes = repositorio.BuscarPorIntegracao(codigo);

                if (integracaoAlteracoes.Count == 0)
                    return new JsonpResult(data: false, status: true, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.NaoExistemDetalhesAlteracoesParaEsteRegistro);

                return new JsonpResult(new
                {
                    integracaoAlteracoes.First().ControleIntegracaoCargaEDI.IDOC,
                    DetalhesCarga = (
                        from integracaoAlteracao in integracaoAlteracoes
                        select new
                        {
                            Carga = integracaoAlteracao.ControleIntegracaoCargaEDI.NumeroDT,
                            integracaoAlteracao.MeioTransporteAnterior,
                            integracaoAlteracao.MeioTransporteAtual,
                            integracaoAlteracao.ModeloVeicularAnterior,
                            integracaoAlteracao.ModeloVeicularAtual,
                            integracaoAlteracao.PlacaAnterior,
                            integracaoAlteracao.PlacaAtual,
                            integracaoAlteracao.QuantidadeNfsAnterior,
                            integracaoAlteracao.QuantidadeNfsAtual,
                            integracaoAlteracao.RoteiroAnterior,
                            integracaoAlteracao.RoteiroAtual
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(status: false, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarPrioridade()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var prioritario = Request.GetBoolParam("Prioritario");
                var repositorio = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                var integracao = repositorio.BuscarPorCodigo(codigo);


                if (integracao == null)
                    return new JsonpResult(data: false, status: true, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.NaoFoiPossivelEncontrarRegistro);

                integracao.Prioritario = prioritario;
                repositorio.Atualizar(integracao);

                return new JsonpResult(status: true, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.PrioridadeAlteradaSucesso);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(status: false, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProcessarPrioritario()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var processarPrioritario = Request.GetBoolParam("ProcessarPrioritario");
                var repositorio = new Repositorio.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI configuracao = repositorio.BuscarConfiguracao();

                if (configuracao == null)
                {
                    configuracao = new Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI()
                    {
                        ProcessarSomentePrioritario = processarPrioritario
                    };

                    repositorio.Inserir(configuracao, Auditado);
                }
                else
                {
                    configuracao.Initialize();
                    configuracao.ProcessarSomentePrioritario = processarPrioritario;
                    repositorio.Atualizar(configuracao, Auditado);
                };

                return new JsonpResult(status: true, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.ProcessarSomentePrioritariosAtualizado);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(status: false, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoSomentePrioritario()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var processarPrioritario = Request.GetBoolParam("ProcessarPrioritario");
                var repositorio = new Repositorio.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI configuracao = repositorio.BuscarConfiguracao();

                bool processarSomentePrioritario = false;
                if (configuracao != null)
                    processarSomentePrioritario = configuracao.ProcessarSomentePrioritario;


                return new JsonpResult(new { ProcessarSomentePrioritario = processarSomentePrioritario });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(status: false, mensagem: Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI integracao = repControleIntegracao.BuscarPorCodigo(codigo);

                if (integracao.SituacaoIntegracaoCargaEDI != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha)
                    return new JsonpResult(false, false, Localization.Resources.Cargas.ControleGeracaoEDI.NaoPossivelReenviarIntegracaoSituacaoAtual);

                integracao.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.AgIntegracao;

                repControleIntegracao.Atualizar(integracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaReenviar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarEmLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaControleIntegracaoCargaEDI filtrosPesquisa = ObterFiltrosPesquisa();
                filtrosPesquisa.Situacao = SituacaoIntegracaoCargaEDI.Falha;
                if ((filtrosPesquisa.DataFinal.Value.Date - filtrosPesquisa.DataInicial.Value.Date).Days > 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleGeracaoEDI.OPeriodoNaoPodeExcederUmDia);

                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);

                List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> listaIntegracoes = repControleIntegracao.ConsultarReenvio(filtrosPesquisa);
                if (listaIntegracoes.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.ControleGeracaoEDI.NenhumRegistroComFalhaPeriodo);

                foreach (Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI in listaIntegracoes)
                {
                    controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.AgIntegracao;
                    repControleIntegracao.Atualizar(controleIntegracaoCargaEDI);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaReenviarLote);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repositorioControleIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.AgIntegracao);
                int totalIntegrado = repositorioControleIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado);
                int totalProblemaIntegracao = repositorioControleIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.ControleGeracaoEDI.OcorreuUmaFalhaObterTotaisIntegracoesCTe);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(bool isExportarPesquisa)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaControleIntegracaoCargaEDI filtrosPesquisa = ObterFiltrosPesquisa();

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Prioritario", false);

                if (filtrosPesquisa.CodigoCarga == 0)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.Viagem, "NumeroViagem", 8, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.Placa, "Placa", 5, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.Transportador, "Transportador", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.NomeArquivo, "NomeArquivo", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.IDOC, "IDOC", 8, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.Tentativas, "NumeroTentativas", 4, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.Retorno, "MensagemRetorno", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.Data, "Data", 8, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracaoCargaEDI", 8, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.DataAtualizacao, "DataAtualizacaoSituacaoCarga", 7, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.SituacaoCarga, "SituacaoCarga", 7, Models.Grid.Align.left, true);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.NomeArquivo, "NomeArquivo", 85, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.TipoIntegracao, "TipoIntegracao", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.Tentativas, "NumeroTentativas", 10, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracaoCargaEDI", 8, Models.Grid.Align.left, true);
                }

                if (ConfiguracaoEmbarcador.Pais == TipoPais.Exterior)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.TipoArquivo, "TipoArquivo", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleGeracaoEDI.Cliente, "CodigoIntegracaoCliente", 10, Models.Grid.Align.left, false);
                }

                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                int totalRegistros = repositorioControleIntegracao.ContarConsulta(filtrosPesquisa);

                if ((isExportarPesquisa) && (totalRegistros > 10000))
                    throw new ControllerException(Localization.Resources.Cargas.ControleGeracaoEDI.ONumeroRegistrosUltrapassaLimiteParaExportacooDezMilRegistros);

                List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> listaIntegracoes = repositorioControleIntegracao.Consultar(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        integracao.CodigoIntegracaoCliente,
                        Data = integracao.Data.ToString("dd/MM/yyyy HH:mm"),
                        DataAtualizacaoSituacaoCarga = integracao.DataAtualizacaoSituacaoCarga?.ToString("dd/MM/yyyy HH:mm"),
                        integracao.IDOC,
                        MensagemRetorno = integracao.MensagemRetorno.Replace(";", " | "),
                        TipoArquivo = integracao.TipoArquivo.HasValue ? integracao.TipoArquivo.Value.ObterDescricao() : "",
                        TipoIntegracao = integracao.TipoIntegracao?.Descricao ?? null,
                        integracao.NomeArquivo,
                        integracao.NumeroTentativas,
                        NumeroViagem = !string.IsNullOrWhiteSpace(integracao.NumeroDT) ? integracao.NumeroDT : "",
                        integracao.Placa,
                        SituacaoCarga = integracao.SituacaoCarga?.ObterDescricao(),
                        SituacaoIntegracaoCargaEDI = integracao.SituacaoIntegracaoCargaEDI.ObterDescricao(),
                        Transportador = integracao.Transportador?.Descricao,
                        integracao.Prioritario,
                        DT_RowColor = integracao.Prioritario ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul : "",
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
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

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaControleIntegracaoCargaEDI ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaControleIntegracaoCargaEDI()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                TelaCarga = Request.GetBoolParam("TelaCarga"),
                CodigoCargaEmbarcador = Request.Params("NumeroDT"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                HoraFinal = Request.GetNullableTimeParam("HoraFinal"),
                HoraInicial = Request.GetNullableTimeParam("HoraInicial"),
                IDOC = Request.Params("IDOC"),
                NomeArquivo = Request.Params("NomeArquivo"),
                Placa = Request.Params("Placa"),
                Situacao = Request.GetEnumParam<SituacaoIntegracaoCargaEDI>("Situacao")
            };
        }

        private void GerarRegistroEDI(List<string> arquivos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);

            foreach (string nomeArquivo in arquivos)
            {
                try
                {
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                        continue;

                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI();
                    controleIntegracaoCargaEDI.Data = DateTime.Now;
                    controleIntegracaoCargaEDI.MensagemRetorno = "";
                    controleIntegracaoCargaEDI.NumeroDT = "";
                    controleIntegracaoCargaEDI.NomeArquivo = Path.GetFileName(nomeArquivo);
                    controleIntegracaoCargaEDI.GuidArquivo = Guid.NewGuid().ToString() + Path.GetExtension(nomeArquivo);
                    controleIntegracaoCargaEDI.NumeroTentativas = 0;
                    controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.AgIntegracao;

                    repositorioControleIntegracaoCargaEDI.Inserir(controleIntegracaoCargaEDI);

                    unitOfWork.CommitChanges();

                    try
                    {
                        MoverArquivoPastaProcessados(nomeArquivo, unitOfWork);
                    }
                    catch (Exception ex)
                    {
                        repositorioControleIntegracaoCargaEDI.Deletar(controleIntegracaoCargaEDI);
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    unitOfWork.Rollback();
                    continue;
                }
            }
        }

        private void MoverArquivoPastaProcessados(string arquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "Processados");
            string nomeArquivo = Path.GetFileName(arquivo);

            if (Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo)))
            {
                int numeroArquivosComMesmoNome = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho).Where(x => x.Contains(nomeArquivo)).Count();
                nomeArquivo = $"{Path.GetFileNameWithoutExtension(arquivo)}_{numeroArquivosComMesmoNome}{Path.GetExtension(arquivo)}";
            }

            Utilidades.IO.FileStorageService.Storage.Copy(arquivo, Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo));
        }

        #endregion
    }
}
