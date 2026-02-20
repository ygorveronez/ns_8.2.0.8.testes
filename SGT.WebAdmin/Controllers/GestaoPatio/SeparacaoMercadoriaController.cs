using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/SeparacaoMercadoria", "GestaoPatio/FluxoPatio")]
    public class SeparacaoMercadoriaController : BaseController
    {
		#region Construtores

		public SeparacaoMercadoriaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.SeparacaoMercadoria servicoSeparacaoMercadoria = new Servicos.Embarcador.GestaoPatio.SeparacaoMercadoria(unitOfWork, Auditado, Cliente);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SeparacaoMercadoriaAvancar separacaoMercadoriaAvancar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SeparacaoMercadoriaAvancar()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    NumeroCarregadores = Request.GetIntParam("NumeroCarregadores"),
                    CodigoResponsavelCarregamento = Request.GetIntParam("ResponsavelCarregamento"),
                    ResponsaveisSeparacao = ObterResponsaveisSeparacao()
                };

                servicoSeparacaoMercadoria.Avancar(separacaoMercadoriaAvancar);

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
                return new JsonpResult(false, "Ocorreu uma falha ao informar a separação de mercadoria.");
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
                int codigoSeparacaoMercadoria = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = null;

                if (codigoSeparacaoMercadoria > 0)
                    separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorCodigo(codigoSeparacaoMercadoria);
                else if (codigoFluxoGestaoPatio > 0)
                    separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (separacaoMercadoria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (separacaoMercadoria.Carga != null)
                    return new JsonpResult(ObterSeparacaoMercadoriaPorCarga(unitOfWork, separacaoMercadoria));

                return new JsonpResult(ObterSeparacaoMercadoriaPorPreCarga(unitOfWork, separacaoMercadoria));
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
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }

        }

        public async Task<IActionResult> ReabrirFluxo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorCodigo(codigo);

                if (separacaoMercadoria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (separacaoMercadoria.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(separacaoMercadoria.FluxoGestaoPatio);

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
                Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorCodigo(codigo);

                if (separacaoMercadoria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(separacaoMercadoria.FluxoGestaoPatio, EtapaFluxoGestaoPatio.SeparacaoMercadoria);
                servicoFluxoGestaoPatio.Auditar(separacaoMercadoria.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria = repositorioSeparacaoMercadoria.BuscarPorCodigo(codigo);

                if (separacaoMercadoria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(separacaoMercadoria.FluxoGestaoPatio, EtapaFluxoGestaoPatio.SeparacaoMercadoria, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSeparacaoMercadoria ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSeparacaoMercadoria()
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                NumeroCarga = Request.GetStringParam("Carga"),
                Situacao = Request.GetNullableEnumParam<SituacaoSeparacaoMercadoria>("Situacao")
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavelSeparacao> ObterResponsaveisSeparacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavelSeparacao> listaSeparadores = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavelSeparacao>>(Request.Params("Separadores"));

            return listaSeparadores;
        }

        private dynamic ObterResponsaveisSeparacaoRetornar(Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel repositorioResponsavel = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel(unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel> listaResponsaveis = repositorioResponsavel.BuscarPorSeparacaoMercadoria(separacaoMercadoria.Codigo);

            return (from o in listaResponsaveis
                    select new
                    {
                        o.Codigo,
                        CodigoResponsavel = o.Responsavel.Codigo,
                        o.CapacidadeSeparacao,
                        Descricao = o.Responsavel.Descricao
                    }).ToList();
        }

        private dynamic ObterSeparacaoMercadoria(Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            int codigoCargaFiltrarJanelaCarregamento = separacaoMercadoria.Carga?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == codigoCargaFiltrarJanelaCarregamento select o).FirstOrDefault();

            return new
            {
                Codigo = separacaoMercadoria.Codigo,
                Carga = servicoCarga.ObterNumeroCarga(separacaoMercadoria.Carga, configuracaoEmbarcador),
                CodigosAgrupadosCarga = separacaoMercadoria.Carga == null ? "" : string.Join(", ", separacaoMercadoria.Carga.CodigosAgrupados),
                DataSeparacaoMercadoriaInformada = separacaoMercadoria.DataSeparacaoMercadoriaInformada?.ToString("dd/MM/yyyy") ?? "",
                Situacao = separacaoMercadoria.Situacao.ObterDescricao(),
                Destino = separacaoMercadoria.Carga?.DadosSumarizados?.Destinos ?? "",
                Doca = !string.IsNullOrWhiteSpace(separacaoMercadoria.Carga?.NumeroDocaEncosta) ? separacaoMercadoria.Carga?.NumeroDocaEncosta : separacaoMercadoria.Carga?.NumeroDoca ?? string.Empty,
                TempoJanela = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Veiculo = separacaoMercadoria.Carga?.RetornarPlacas,
                Transportador = separacaoMercadoria.Carga?.Empresa?.Descricao ?? string.Empty,
                ModeloVeiculo = separacaoMercadoria.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                TipoOperacao = separacaoMercadoria.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? string.Empty
            };
        }

        private dynamic ObterSeparacaoMercadoriaPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(separacaoMercadoria.Carga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(separacaoMercadoria.FluxoGestaoPatio);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(separacaoMercadoria.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                separacaoMercadoria.Codigo,
                separacaoMercadoria.Situacao,
                NumeroCarregadores = separacaoMercadoria.NumeroCarregadores > 0 ? separacaoMercadoria.NumeroCarregadores.ToString("n0") : "",
                ResponsavelCarregamento = new { Codigo = separacaoMercadoria.ResponsavelCarregamento?.Codigo ?? 0, Descricao = separacaoMercadoria.ResponsavelCarregamento?.Descricao ?? "" },
                ResponsaveisSeparacao = ObterResponsaveisSeparacaoRetornar(separacaoMercadoria, unitOfWork),
                Carga = separacaoMercadoria.Carga.Codigo,
                PreCarga = separacaoMercadoria.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(separacaoMercadoria.Carga, unitOfWork),
                NumeroPreCarga = separacaoMercadoria.PreCarga?.NumeroPreCarga ?? "",
                CargaData = separacaoMercadoria.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = separacaoMercadoria.Carga.DataCarregamentoCarga?.ToString($"HH:mm") ?? "",
                Transportador = separacaoMercadoria.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = separacaoMercadoria.Carga.RetornarPlacas,
                Remetente = separacaoMercadoria.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = separacaoMercadoria.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = separacaoMercadoria.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = separacaoMercadoria.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = separacaoMercadoria.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                PermiteInformarDadosCarregadores = sequenciaGestaoPatio?.SeparacaoMercadoriaPermiteInformarDadosCarregadores ?? false,
                PermiteInformarDadosSeparadores = sequenciaGestaoPatio?.SeparacaoMercadoriaPermiteInformarDadosSeparadores ?? false
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterSeparacaoMercadoriaPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria separacaoMercadoria)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(separacaoMercadoria.PreCarga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(separacaoMercadoria.FluxoGestaoPatio);
            DateTime? dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento;
            bool permitirEditarEtapa = IsPermitirEditarEtapa(separacaoMercadoria.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                separacaoMercadoria.Codigo,
                separacaoMercadoria.Situacao,
                NumeroCarregadores = separacaoMercadoria.NumeroCarregadores > 0 ? separacaoMercadoria.NumeroCarregadores.ToString("n0") : "",
                ResponsavelCarregamento = new { Codigo = separacaoMercadoria.ResponsavelCarregamento?.Codigo ?? 0, Descricao = separacaoMercadoria.ResponsavelCarregamento?.Descricao ?? "" },
                ResponsaveisSeparacao = ObterResponsaveisSeparacaoRetornar(separacaoMercadoria, unitOfWork),
                Carga = 0,
                PreCarga = separacaoMercadoria.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = separacaoMercadoria.PreCarga.NumeroPreCarga ?? "",
                CargaData = dataCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = dataCarregamento?.ToString($"HH:mm") ?? "",
                Transportador = separacaoMercadoria.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = separacaoMercadoria.PreCarga.RetornarPlacas,
                Remetente = separacaoMercadoria.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = separacaoMercadoria.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = separacaoMercadoria.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = separacaoMercadoria.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = separacaoMercadoria.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                PermiteInformarDadosCarregadores = sequenciaGestaoPatio?.SeparacaoMercadoriaPermiteInformarDadosCarregadores ?? false,
                PermiteInformarDadosSeparadores = sequenciaGestaoPatio?.SeparacaoMercadoriaPermiteInformarDadosSeparadores ?? false
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.SeparacaoMercadoria);
        }

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
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cargas Agrupadas", "CodigosAgrupadosCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data da Separação", "DataSeparacaoMercadoriaInformada", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Doca", "Doca", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tempo Janela", "TempoJanela", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo", "ModeloVeiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação Janela", "ObservacaoFluxoPatio", 10, Models.Grid.Align.left, false);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSeparacaoMercadoria filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria repositorioSeparacaoMercadoria = new Repositorio.Embarcador.GestaoPatio.SeparacaoMercadoria(unitOfWork);
                int totalRegistros = repositorioSeparacaoMercadoria.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria> listaSeparacaoMercadoria = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = null;

                if (totalRegistros > 0)
                {
                    listaSeparacaoMercadoria = repositorioSeparacaoMercadoria.Consultar(filtrosPesquisa, parametrosConsulta);
                    List<int> codigosCargas = (from o in listaSeparacaoMercadoria where o.Carga != null select o.Carga.Codigo).Distinct().ToList();
                    listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
                }
                else
                {
                    listaSeparacaoMercadoria = new List<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria>();
                    listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                }

                var listaSeparacaoMercadoriaRetornar = (
                    from separacaoMercadoria in listaSeparacaoMercadoria
                    select ObterSeparacaoMercadoria(separacaoMercadoria, listaCargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork)
                ).ToList();

                grid.AdicionaRows(listaSeparacaoMercadoriaRetornar);
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
