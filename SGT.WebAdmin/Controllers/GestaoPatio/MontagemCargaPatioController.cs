using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/MontagemCargaPatio", "GestaoPatio/FluxoPatio")]
    public class MontagemCargaPatioController : BaseController
    {
		#region Construtores

		public MontagemCargaPatioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.MontagemCargaPatio servicoMontagemCargaPatio = new Servicos.Embarcador.GestaoPatio.MontagemCargaPatio(unitOfWork, Auditado, Cliente);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.MontagemCargaPatioAvancar montagemCargaPatioAvancar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.MontagemCargaPatioAvancar()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    QuantidadeCaixas = Request.GetIntParam("QuantidadeCaixas"),
                    QuantidadeItens = Request.GetIntParam("QuantidadeItens"),
                    QuantidadePalletsFracionados = Request.GetIntParam("QuantidadePalletsFracionados"),
                    QuantidadePalletsInteiros = Request.GetIntParam("QuantidadePalletsInteiros")
                };

                servicoMontagemCargaPatio.Avancar(montagemCargaPatioAvancar);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar os dados da montagem de carga do pátio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMontagemCargaPatio = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = null;

                if (codigoMontagemCargaPatio > 0)
                    montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorCodigo(codigoMontagemCargaPatio);
                else if (codigoFluxoGestaoPatio > 0)
                    montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (montagemCargaPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (montagemCargaPatio.Carga != null)
                    return new JsonpResult(ObterMontagemCargaPatioPorCarga(unitOfWork, montagemCargaPatio));

                return new JsonpResult(ObterMontagemCargaPatioPorPreCarga(unitOfWork, montagemCargaPatio));
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
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa(false));
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
                var grid = ObterGridPesquisa(true);
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

        public async Task<IActionResult> ReabrirFluxo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorCodigo(codigo);

                if (montagemCargaPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (montagemCargaPatio.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(montagemCargaPatio.FluxoGestaoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir o fluxo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorCodigo(codigo);

                if (montagemCargaPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(montagemCargaPatio.FluxoGestaoPatio, EtapaFluxoGestaoPatio.MontagemCarga);
                servicoFluxoGestaoPatio.Auditar(montagemCargaPatio.FluxoGestaoPatio, "Rejeitou o fluxo.");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorCodigo(codigo);

                if (montagemCargaPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(montagemCargaPatio.FluxoGestaoPatio, EtapaFluxoGestaoPatio.MontagemCarga, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao voltar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ComprovanteMontagemCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio =  new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio = repositorioMontagemCargaPatio.BuscarPorCodigo(codigo);

                if (montagemCargaPatio == null)
                    throw new ServicoException("Não foi possível encontrar o registro.");

                byte[] pdf = ReportRequest.WithType(ReportType.ComprovanteMontagemCarga)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo", codigo.ToString())
                    .CallReport()
                    .GetContentFile();
                
                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o comprovante da montagem de carga.");

                return Arquivo(pdf, "application/pdf", "Comprovante da Montagem de Carga " + montagemCargaPatio.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o comprovante.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaMontagemCargaPatio ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaMontagemCargaPatio()
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                NumeroCarga = Request.GetStringParam("Carga"),
                Situacao = Request.GetNullableEnumParam<SituacaoMontagemCargaPatio>("Situacao")
            };
        }

        private dynamic ObterMontagemCargaPatio(Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            int codigoCargaFiltrarJanelaCarregamento = montagemCargaPatio.Carga?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == codigoCargaFiltrarJanelaCarregamento select o).FirstOrDefault();

            return new
            {
                Codigo = montagemCargaPatio.Codigo,
                Carga = servicoCarga.ObterNumeroCarga(montagemCargaPatio.Carga, configuracaoEmbarcador),
                CodigosAgrupadosCarga = montagemCargaPatio.Carga == null ? "" : string.Join(", ", montagemCargaPatio.Carga.CodigosAgrupados),
                DataMontagemCargaIniciada = montagemCargaPatio.DataMontagemCargaIniciada?.ToString("dd/MM/yyyy") ?? "",
                Situacao = montagemCargaPatio.Situacao.ObterDescricao(),
                Destino = montagemCargaPatio.Carga?.DadosSumarizados?.Destinos ?? "",
                Doca = !string.IsNullOrWhiteSpace(montagemCargaPatio.Carga?.NumeroDocaEncosta) ? montagemCargaPatio.Carga?.NumeroDocaEncosta : montagemCargaPatio.Carga?.NumeroDoca ?? string.Empty,
                TempoJanela = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Veiculo = montagemCargaPatio.Carga?.RetornarPlacas,
                Transportador = montagemCargaPatio.Carga?.Empresa?.Descricao ?? string.Empty,
                ModeloVeiculo = montagemCargaPatio.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                TipoOperacao = montagemCargaPatio.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? string.Empty,
                QuantidadeItens = montagemCargaPatio.QuantidadeItens,
                QuantidadeCaixas = montagemCargaPatio.QuantidadeCaixas,
                QuantidadePalletsInteiros = montagemCargaPatio.QuantidadePalletsInteiros,
                QuantidadePalletsFracionados = montagemCargaPatio.QuantidadePalletsFracionados
            };
        }

        private dynamic ObterMontagemCargaPatioPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(montagemCargaPatio.Carga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(montagemCargaPatio.FluxoGestaoPatio);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();

            bool permitirEditarEtapa = IsPermitirEditarEtapa(montagemCargaPatio.FluxoGestaoPatio, configuracaoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                montagemCargaPatio.Codigo,
                montagemCargaPatio.Situacao,
                Carga = montagemCargaPatio.Carga.Codigo,
                PreCarga = montagemCargaPatio.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(montagemCargaPatio.Carga, unitOfWork),
                NumeroPreCarga = montagemCargaPatio.PreCarga?.NumeroPreCarga ?? "",
                CargaData = montagemCargaPatio.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = montagemCargaPatio.Carga.DataCarregamentoCarga?.ToString($"HH:mm") ?? "",
                Transportador = montagemCargaPatio.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = montagemCargaPatio.Carga.RetornarPlacas,
                Remetente = montagemCargaPatio.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = montagemCargaPatio.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = montagemCargaPatio.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = montagemCargaPatio.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = montagemCargaPatio.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                QuantidadeCaixas = montagemCargaPatio.QuantidadeCaixas > 0 ? montagemCargaPatio.QuantidadeCaixas.ToString("n0") : string.Empty,
                QuantidadeItens = montagemCargaPatio.QuantidadeItens > 0 ? montagemCargaPatio.QuantidadeItens.ToString("n0") : string.Empty,
                QuantidadePalletsFracionados = montagemCargaPatio.QuantidadePalletsFracionados > 0 ? montagemCargaPatio.QuantidadePalletsFracionados.ToString("n0") : string.Empty,
                QuantidadePalletsInteiros = montagemCargaPatio.QuantidadePalletsInteiros > 0 ? montagemCargaPatio.QuantidadePalletsInteiros.ToString("n0") : string.Empty,
                PermiteInformarQuantidadeCaixas = sequenciaGestaoPatio?.MontagemCargaPermiteInformarQuantidadeCaixas ?? false,
                PermiteInformarQuantidadeItens = sequenciaGestaoPatio?.MontagemCargaPermiteInformarQuantidadeItens ?? false,
                PermiteInformarQuantidadePallets = sequenciaGestaoPatio?.MontagemCargaPermiteInformarQuantidadePallets ?? false,
                MontagemCargaPermiteGerarAtendimento = configuracaoGestaoPatio?.MontagemCargaPermiteGerarAtendimento ?? false
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterMontagemCargaPatioPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(montagemCargaPatio.PreCarga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(montagemCargaPatio.FluxoGestaoPatio);

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();

            DateTime? dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento;
            bool permitirEditarEtapa = IsPermitirEditarEtapa(montagemCargaPatio.FluxoGestaoPatio, configuracaoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                montagemCargaPatio.Codigo,
                montagemCargaPatio.Situacao,
                Carga = 0,
                PreCarga = montagemCargaPatio.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = montagemCargaPatio.PreCarga.NumeroPreCarga ?? "",
                CargaData = dataCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = dataCarregamento?.ToString($"HH:mm") ?? "",
                Transportador = montagemCargaPatio.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = montagemCargaPatio.PreCarga.RetornarPlacas,
                Remetente = montagemCargaPatio.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = montagemCargaPatio.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = montagemCargaPatio.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = montagemCargaPatio.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = montagemCargaPatio.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                QuantidadeCaixas = montagemCargaPatio.QuantidadeCaixas > 0 ? montagemCargaPatio.QuantidadeCaixas.ToString("n0") : string.Empty,
                QuantidadeItens = montagemCargaPatio.QuantidadeItens > 0 ? montagemCargaPatio.QuantidadeItens.ToString("n0") : string.Empty,
                QuantidadePalletsFracionados = montagemCargaPatio.QuantidadePalletsFracionados > 0 ? montagemCargaPatio.QuantidadePalletsFracionados.ToString("n0") : string.Empty,
                QuantidadePalletsInteiros = montagemCargaPatio.QuantidadePalletsInteiros > 0 ? montagemCargaPatio.QuantidadePalletsInteiros.ToString("n0") : string.Empty,
                PermiteInformarQuantidadeCaixas = sequenciaGestaoPatio?.MontagemCargaPermiteInformarQuantidadeCaixas ?? false,
                PermiteInformarQuantidadeItens = sequenciaGestaoPatio?.MontagemCargaPermiteInformarQuantidadeItens ?? false,
                PermiteInformarQuantidadePallets = sequenciaGestaoPatio?.MontagemCargaPermiteInformarQuantidadePallets ?? false,
                MontagemCargaPermiteGerarAtendimento = configuracaoGestaoPatio?.MontagemCargaPermiteGerarAtendimento ?? false
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.MontagemCarga) || (configuracaoGestaoPatio?.MontagemCargaPermiteAntecipar ?? false);
        }

        private Models.Grid.Grid ObterGridPesquisa(bool exportarPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cargas Agrupadas", "CodigosAgrupadosCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data da Montagem", "DataMontagemCargaIniciada", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Doca", "Doca", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tempo Janela", "TempoJanela", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo", "ModeloVeiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação Janela", "ObservacaoFluxoPatio", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Quantidade de Itens", "QuantidadeItens", 8, Models.Grid.Align.right, false, exportarPesquisa);
                grid.AdicionarCabecalho("Quantidade de Caixas", "QuantidadeCaixas", 8, Models.Grid.Align.right, false, exportarPesquisa);
                grid.AdicionarCabecalho("Quantidade de Pallets Inteiros", "QuantidadePalletsInteiros", 8, Models.Grid.Align.right, false, exportarPesquisa);
                grid.AdicionarCabecalho("Quantidade de Pallets Fracionados", "QuantidadePalletsFracionados", 8, Models.Grid.Align.right, false, exportarPesquisa);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaMontagemCargaPatio filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio = new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(unitOfWork);
                int totalRegistros = repositorioMontagemCargaPatio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio> listaMontagemCargaPatio = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = null;

                if (totalRegistros > 0)
                {
                    listaMontagemCargaPatio = repositorioMontagemCargaPatio.Consultar(filtrosPesquisa, parametrosConsulta);
                    List<int> codigosCargas = (from o in listaMontagemCargaPatio where o.Carga != null select o.Carga.Codigo).Distinct().ToList();
                    listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
                }
                else
                {
                    listaMontagemCargaPatio = new List<Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio>();
                    listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                }

                var listaMontagemCargaPatioRetornar = (
                    from montagemCargaPatio in listaMontagemCargaPatio
                    select ObterMontagemCargaPatio(montagemCargaPatio, listaCargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork)
                ).ToList();

                grid.AdicionaRows(listaMontagemCargaPatioRetornar);
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

        #endregion
    }
}
