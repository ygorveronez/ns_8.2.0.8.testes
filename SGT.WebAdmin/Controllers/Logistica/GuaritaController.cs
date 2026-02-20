using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicos.Extensions;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { "BuscarPorCodigo", "ObterDadosGuarita", "ComprovanteSaida", "DownloadDetalhesCarga" }, "Logistica/Guarita", "GestaoPatio/FluxoPatio")]
    public class GuaritaController : BaseController
    {
        #region Construtores

        public GuaritaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCarregamentoGuarita()
                {
                    DataInicialCarregamento = Request.GetNullableDateTimeParam("DataInicialCarregamento"),
                    DataFinalCarregamento = Request.GetNullableDateTimeParam("DataFinalCarregamento"),
                    Situacao = Request.GetNullableEnumParam<SituacaoCargaGuarita>("Situacao"),
                    CodigosTransportadores = Request.GetListParam<int>("Transportadores"),
                    CodigosMotoristas = Request.GetListParam<int>("Motoristas"),
                    CodigosVeiculos = Request.GetListParam<int>("Veiculos"),
                    DataAgendada = Request.GetNullableDateTimeParam("DataAgendada"),
                    CodigosTipoOperacoes = Request.GetListParam<int>("TipoOperacao"),
                    CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                    ListaCodigoCarga = Request.GetListParam<int>("ListaCodigoCarga"),
                    NumeroCarga = Request.GetStringParam("NumeroCarga"),
                    CodigosFiliais = Request.GetListParam<int>("Filial"),
                    CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                    DataInicialChegada = Request.GetNullableDateTimeParam("DataInicialChegada"),
                    DataFinalChegada = Request.GetNullableDateTimeParam("DataFinalChegada")
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Carregamento", "DataCarregamento", 10, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Carga", "NumeroCarga", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data de chegada", "DataChegadaVeiculo", 8, Models.Grid.Align.center, true, false);

                if (!configuracaoTMS.NaoExibirInfosAdicionaisGridPatio)
                    grid.AdicionarCabecalho("Cargas Agrupadas", "CodigosAgrupadosCarga", 10, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Tipo", "DescricaoTipoChegadaGuarita", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Ajudante", "Ajudante", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CPF Ajudante", "AjudanteCpf", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo Veículo", "ModeloVeiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("LiberadaSaida", false);
                grid.AdicionarCabecalho("TipoChegadaGuarita", false);
                grid.AdicionarCabecalho("SomentePreCarga", false);
                grid.AdicionarCabecalho("InformarChegadaVeiculo", false);
                grid.AdicionarCabecalho("PermiteInformarMotoristaEVeiculo", false);
                grid.AdicionarCabecalho("ChegadaDenegada", false);
                grid.AdicionarCabecalho("GuaritaEntradaPermiteDenegarChegada", false);
                grid.AdicionarCabecalho("PermitirAlterarDataChegadaVeiculo", false);

                if (!configuracaoTMS.NaoExibirInfosAdicionaisGridPatio)
                    grid.AdicionarCabecalho("Doca", "Doca", 10, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Tempo Janela", "TempoJanela", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);

                if (!configuracaoTMS.NaoExibirInfosAdicionaisGridPatio)
                    grid.AdicionarCabecalho("Observação Janela", "ObservacaoFluxoPatio", 10, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Observação Guarita", "ObservacaoGuaritaChegadaDenegada", 10, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Carga", false);
                grid.AdicionarCabecalho("PreCarga", false);
                grid.AdicionarCabecalho("ObservacaoGuarita", false);
                grid.AdicionarCabecalho("Data Agendada", "DataAgendada", 10, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Provedores OS", "ProvedoresOS", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("CPF Motorista", "CPFMotorista", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("PermiteVisualizarAnexos", false);
                grid.AdicionarCabecalho("PossuiPesagemInicial", false);
                grid.AdicionarCabecalho("PossuiPesagemFinal", false);
                grid.AdicionarCabecalho("PermiteImprimirOrdemColetaNaGuarita", false);
                grid.AdicionarCabecalho("Remetente", "RemetentePedido", 15, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("CNPJ Remetente", "CNPJRemetentePedido", 10, Models.Grid.Align.left, false, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unidadeDeTrabalho, "Guarita/Pesquisa", "grid-guarita");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                List<int> centrosCarregamento = ObterListaCodigoCentroCarregamentoPermitidosOperadorLogistica(unidadeDeTrabalho);

                filtrosPesquisa.CodigosCentrosCarregamento = centrosCarregamento;

                int rowCount = repCargaGuarita.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> listaCargaGuarita = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>();

                if (rowCount > 0)
                    listaCargaGuarita = repCargaGuarita.Consultar(filtrosPesquisa, parametrosConsulta);

                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                grid.setarQuantidadeTotal(rowCount);

                var retorno = ObterDadosGuaritas(listaCargaGuarita, unidadeDeTrabalho, cancellationToken);
                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [Obsolete("Método utilizado somente no fluxo de entrega (DESCONTINUADO)")]
        public async Task<IActionResult> BuscarPorCarga()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCarga(codigoCarga);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                return new JsonpResult(ObterDadosGuarita(cargaGuarita, configuracaoEmbarcador, unidadeDeTrabalho));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar por carga.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = null;

                if (codigoGuarita > 0)
                    cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);
                else if (codigoFluxoGestaoPatio > 0)
                    cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                return new JsonpResult(ObterDadosGuarita(cargaGuarita, configuracaoEmbarcador, unidadeDeTrabalho));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar por Carga.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCodigoGuarita()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (guarita == null)
                    throw new Exception("Guarita não encontrada");

                return new JsonpResult(guarita.Codigo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a pesagem final.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarCarga()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();

                int codigo = Request.GetIntParam("Codigo");
                bool etapaAntecipada = Request.GetBoolParam("EtapaAntecipada");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao fluxoGestaoPatioConfiguracao = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = fluxoGestaoPatioConfiguracao.ObterConfiguracao();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigo);

                if (cargaGuarita == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (cargaGuarita.Situacao != SituacaoCargaGuarita.AguardandoLiberacao)
                    throw new ControllerException("A situação atual não permite a liberação.");

                if (cargaGuarita.FluxoGestaoPatio.Etapas.Any(etapa => etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.Guarita) && (cargaGuarita.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != EtapaFluxoGestaoPatio.Guarita) && !etapaAntecipada)
                    throw new ControllerException("Ainda não foi autorizada a entrada do veículo.");

                if (cargaGuarita.FluxoGestaoPatio.CargaBase.Veiculo != null)
                    cargaGuarita.EtapaGuaritaLiberada = true;

                if (!cargaGuarita.EtapaGuaritaLiberada)
                    throw new ControllerException("Ainda não foi autorizada a entrada do veículo.");

                if (cargaGuarita.FluxoGestaoPatio.Filial.InformarEquipamentoFluxoPatio && cargaGuarita.FluxoGestaoPatio.Equipamento == null)
                    throw new ControllerException("Obrigatório preencher o equipamento para avançar.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaGuarita.FluxoGestaoPatio);

                if (sequenciaGestaoPatio == null)
                    throw new ControllerException("Não foi encontrada uma configuração para está carga.");

                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                bool existeIntegracaoBalancaQbit = repTipoIntegracao.ExistePorTipo(TipoIntegracao.Qbit);//Balança Qbit irá retornar o peso inicial, assim irá preencher e obrigar na pesagem final

                if (sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesPesagem)
                {
                    if (cargaGuarita.PesagemInicial <= 0 && !existeIntegracaoBalancaQbit)
                        throw new ControllerException("Informe a pesagem inicial antes de avançar a etapa.");

                    //if (sequenciaGestaoPatio.GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem && (cargaGuarita.PesagemQuantidadeCaixas <= 0))
                    //    throw new ControllerException("Informe a quantidade de caixas na pesagem antes de avançar a etapa.");

                    if (sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesProdutor && !cargaGuarita.PesagemProdutorRural.HasValue)
                        throw new ControllerException("Informe o produtor rural na pesagem antes de avançar a etapa.");
                }

                cargaGuarita.Situacao = SituacaoCargaGuarita.Liberada;
                cargaGuarita.DataEntregaGuarita = DateTime.Now;
                cargaGuarita.ChegadaDenegada = false;
                cargaGuarita.PossuiDevolucao = Request.GetBoolParam("PossuiDevolucao");
                cargaGuarita.ObservacaoDevolucao = cargaGuarita.PossuiDevolucao ? Request.GetStringParam("ObservacaoDevolucao") : string.Empty;

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unidadeDeTrabalho, Auditado, Cliente);

                servicoFluxoGestaoPatio.LiberarProximaEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.Guarita);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaGuarita, null, $"Liberou a {cargaGuarita.CargaBase.DescricaoEntidade}.", unidadeDeTrabalho);

                repositorioCargaGuarita.Atualizar(cargaGuarita);

                if (sequenciaGestaoPatio.GuaritaEntradaPermiteInformarDadosDevolucao && !cargaGuarita.PossuiDevolucao)
                {
                    if (cargaGuarita.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InicioDescarregamento)
                        servicoFluxoGestaoPatio.AvancarEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioDescarregamento);

                    if (cargaGuarita.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.FimDescarregamento)
                        servicoFluxoGestaoPatio.AvancarEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.FimDescarregamento);
                }

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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = null;

                if (carga == null)
                {
                    preCarga = repositorioPreCarga.BuscarPorCodigo(codigoCarga);
                    if (preCarga == null)
                        return new JsonpResult(false, "Carga/Pré-carga não encontrada");
                }

                Dominio.Entidades.Usuario motorista = carga == null ? preCarga.Motoristas?.FirstOrDefault() : carga.Motoristas?.FirstOrDefault();
                Dominio.Entidades.Veiculo veiculo = carga == null ? preCarga.Veiculo : carga.Veiculo;
                Dominio.Entidades.Empresa transportador = carga == null ? preCarga.Empresa : carga.Empresa;

                return new JsonpResult(new
                {
                    Motorista = new { Codigo = motorista?.Codigo ?? 0, Descricao = motorista?.Descricao ?? "" },
                    Veiculo = new { Codigo = veiculo?.Codigo ?? 0, Descricao = veiculo?.Descricao ?? "" },
                    Transportador = new { Codigo = transportador?.Codigo ?? 0, Descricao = transportador?.Descricao ?? "" }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados de transporte da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SaidaVeiculo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Repositorio.Embarcador.GestaoPatio.GuaritaAnexo repositorioGuaritaAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaAnexo(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigo);

                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaGuarita.FluxoGestaoPatio);

                unidadeDeTrabalho.Start();

                if (sequenciaGestaoPatio?.GuaritaSaidaPermiteInformacoesPesagem ?? false)
                {
                    if (cargaGuarita.PesagemFinal <= 0)
                        throw new ControllerException("Informe a pesagem final antes de avançar a etapa.");

                    if (sequenciaGestaoPatio.GuaritaSaidaPermiteInformarPercentualRefugoPesagem && (cargaGuarita.PorcentagemPerda <= 0))
                        throw new ControllerException("Informe o percentual de refugo na pesagem antes de avançar a etapa.");

                    if (sequenciaGestaoPatio.GuaritaSaidaPermiteInformarLacrePesagem && string.IsNullOrWhiteSpace(cargaGuarita.NumeroLacre))
                        throw new ControllerException("Informe o lacre na pesagem antes de avançar a etapa.");

                    if (sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesPesagem && (cargaGuarita.PesagemInicial <= 0))
                        throw new ControllerException("Pesagem Inicial não foi informada, a mesma é obrigatória para avançar");
                }

                Servicos.Embarcador.GestaoPatio.InicioViagem servicoInicioViagem = new Servicos.Embarcador.GestaoPatio.InicioViagem(unidadeDeTrabalho, Auditado, Cliente);

                if (!servicoInicioViagem.IsSituacaoCargaPermiteInicioViagem(cargaGuarita))
                    throw new ControllerException("Não é possível informar a saída do veículo na atual situação da carga.");

                cargaGuarita.DataSaidaGuarita = DateTime.Now;
                cargaGuarita.Situacao = SituacaoCargaGuarita.SaidaLiberada;

                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaAnexo> guaritaAnexos = repositorioGuaritaAnexo.BuscarPorGuarita(codigo);

                if ((sequenciaGestaoPatio?.GuaritaSaidaExigirAnexo ?? false) && guaritaAnexos?.Count == 0)
                    throw new ControllerException("Configuração exige anexos para finalizar a etapa.");

                repositorioCargaGuarita.Atualizar(cargaGuarita);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaGuarita.Carga.Codigo);
                DateTime dataInicioViagem = DateTime.Now;

                if (ValidarInicioViagemPelaSaidaVeiculo(carga, dataInicioViagem))
                {
                    if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga.Codigo, dataInicioViagem, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Cliente, Auditado, unidadeDeTrabalho))
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Início de viagem informado manualmente juntamente com a Saida Veiculo pela guarita", unidadeDeTrabalho);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaGuarita, null, "Informou Saída do Veículo.", unidadeDeTrabalho);

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unidadeDeTrabalho, Auditado, Cliente);
                servicoFluxoGestaoPatio.LiberarProximaEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioViagem);

                if (carga?.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.UtilizarDataSaidaGuaritaComoTerminoCarregamento ?? false)
                {
                    Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unidadeDeTrabalho, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);
                    Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        pedido.DataTerminoCarregamento = cargaGuarita.DataSaidaGuarita;

                        try
                        {
                            servicoAgendamentoEntregaPedido.SetarRotaPedido(pedido);
                        }
                        catch (ServicoException excecao) 
                        {
                            Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao setar rota do pedido na guarita: {excecao.ToString()}", "CatchNoAction");
                        }

                        repositorioPedido.Atualizar(pedido);
                    }
                }

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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ComprovanteSaida()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio repositorioConfiguracaoFluxoPatio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio(unidadeDeTrabalho);
            try
            {
                int codigoGuarita = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = null;

                if (codigoGuarita > 0)
                    cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);
                else if (codigoFluxoGestaoPatio > 0)
                    cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                if (cargaGuarita.FluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");


                var pdf = ReportRequest.WithType(ReportType.ComprovanteSaidaGuarita)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoGuarita", codigoGuarita.ToString())
                    .AddExtraData("codigoFluxoGestaoPatio", codigoFluxoGestaoPatio.ToString())
                    .CallReport()
                    .GetContentFile();

                return Arquivo(pdf, "application/pdf", "Comprovante de Saída " + cargaGuarita.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadTicket()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = null;

                if (codigoGuarita > 0)
                    cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);
                else if (codigoFluxoGestaoPatio > 0)
                    cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                if (cargaGuarita.FluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");


                byte[] pdf = ReportRequest.WithType(ReportType.TicketBalanca)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoGuarita", codigoGuarita)
                    .AddExtraData("codigoFluxoGestaoPatio", codigoFluxoGestaoPatio)
                    .AddExtraData("CodigoUsuario", Usuario.Codigo)
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o comprovante de saída.");

                return Arquivo(pdf, "application/pdf", "Ticket Balança " + cargaGuarita.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarInformacoesPesagem()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaGuarita.FluxoGestaoPatio);
                Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados cargaDadosSumarizados = cargaGuarita.CargaBase.DadosSumarizados;
                decimal pesoLiquido = (cargaGuarita.PesagemFinal - cargaGuarita.PesagemInicial);

                return new JsonpResult(new
                {
                    Destino = cargaDadosSumarizados?.Destinos ?? "",
                    ExibirLacre = sequenciaGestaoPatio.GuaritaSaidaPermiteInformarLacrePesagem,
                    ExibirLoteInterno = sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesPesagem && sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesLoteInterno,
                    Lacre = cargaGuarita.NumeroLacre,
                    cargaGuarita.LoteInterno,
                    Origem = cargaDadosSumarizados?.Origens ?? "",
                    PesagemInicial = cargaGuarita.PesagemInicial > 0m ? cargaGuarita.PesagemInicial.ToString("n2") : string.Empty,
                    PesagemFinal = cargaGuarita.PesagemFinal > 0m ? cargaGuarita.PesagemFinal.ToString("n2") : string.Empty,
                    PesoLiquido = pesoLiquido > 0m ? pesoLiquido.ToString("n2") : string.Empty,
                    NumeroPedido = ObterNumeroPedidosCarga(cargaGuarita.CargaJanelaCarregamento?.Carga ?? null, unidadeDeTrabalho)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarInformacoesPesagemInicial()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Repositorio.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo repositorioGuaritaPesagemAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unidadeDeTrabalho);
                Repositorio.Embarcador.Filiais.FilialBalanca repositorioFilialBalanca = new Repositorio.Embarcador.Filiais.FilialBalanca(unidadeDeTrabalho);

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigo);
                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaGuarita.FluxoGestaoPatio);

                bool existeIntegracaoBalancaToledo = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Toledo);
                bool existeIntegracaoBalancaQbit = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Qbit);
                bool existeIntegracaoBalancaDeca = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Deca);
                bool existeIntegracaoBalanca = existeIntegracaoBalancaToledo || existeIntegracaoBalancaQbit || existeIntegracaoBalancaDeca;

                bool pesagemIniciada = existeIntegracaoBalanca ? repositorioPesagem.ExistePorGuarita(codigo) : false;
                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = existeIntegracaoBalanca ? repositorioPesagem.BuscarPorGuarita(codigo) : null;
                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo> anexosGuarita = repositorioGuaritaPesagemAnexo.BuscarPorGuarita(codigo);
                List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> balancas = existeIntegracaoBalancaDeca && cargaGuarita.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.Guarita ? repositorioFilialBalanca.BuscarPorFilial(cargaGuarita.FluxoGestaoPatio.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca>();

                string retornoProdutorRural;
                if (!cargaGuarita.PesagemProdutorRural.HasValue)
                    retornoProdutorRural = "";
                else if (cargaGuarita.PesagemProdutorRural.Value)
                    retornoProdutorRural = "1";
                else
                    retornoProdutorRural = "0";

                return new JsonpResult(new
                {
                    Pedido = cargaGuarita.PesagemPedido,
                    PesagemInicial = cargaGuarita.PesagemInicial > 0 ? cargaGuarita.PesagemInicial.ToString("n2") : string.Empty,
                    ProdutorRural = retornoProdutorRural,
                    QuantidadeCaixas = cargaGuarita.PesagemQuantidadeCaixas > 0 ? cargaGuarita.PesagemQuantidadeCaixas.ToString() : "0",
                    IntegracaoToledo = existeIntegracaoBalancaToledo,
                    IntegracaoQbit = existeIntegracaoBalancaQbit,
                    PesagemBalancaIniciada = pesagemIniciada,
                    CodigoPesagem = pesagem?.Codigo ?? 0,
                    Pressao = cargaGuarita.PesagemPressao > 0m ? cargaGuarita.PesagemPressao.ToString("n2") : "",
                    PodeEditar = cargaGuarita.FluxoGestaoPatio.DataEntregaGuarita == null,
                    ExibirInformacoesProdutor = sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesProdutor,
                    ExibirAnexos = sequenciaGestaoPatio.GuaritaEntradaPermiteInformarAnexoPesagem,
                    ExibirPressao = sequenciaGestaoPatio.GuaritaEntradaPermiteInformarPressaoPesagem,
                    ExibirQuantidadeCaixas = sequenciaGestaoPatio.GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem,
                    Anexos = (
                        from o in anexosGuarita
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.NomeArquivo
                        }
                    ).ToList(),
                    Balanca = existeIntegracaoBalancaDeca ? new { Codigo = sequenciaGestaoPatio.BalancaGuaritaEntrada?.Codigo ?? 0, Descricao = sequenciaGestaoPatio.BalancaGuaritaEntrada?.Descricao ?? string.Empty } : null,
                    Balancas = (
                        from o in balancas
                        select new
                        {
                            o.Codigo,
                            o.Descricao
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarInformacoesPesagemFinal()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PesagemIntegracao repositorioPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.GestaoPatio.GuaritaEntradaPesagemFinalAnexo repositorioGuaritaPesagemFinalAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaEntradaPesagemFinalAnexo(unidadeDeTrabalho);
                Repositorio.Embarcador.Filiais.FilialBalanca repositorioFilialBalanca = new Repositorio.Embarcador.Filiais.FilialBalanca(unidadeDeTrabalho);

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigo);
                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaGuarita.FluxoGestaoPatio);

                bool existeIntegracaoBalancaToledo = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Toledo);
                bool existeIntegracaoBalancaQbit = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Qbit);
                bool existeIntegracaoBalancaDeca = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Deca);
                bool existeIntegracaoBalanca = existeIntegracaoBalancaToledo || existeIntegracaoBalancaQbit || existeIntegracaoBalancaDeca;

                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = existeIntegracaoBalanca ? repositorioPesagem.BuscarPorGuarita(codigo) : null;
                Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagemFinal = pesagem != null ? repositorioPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.PesagemFinal) : null;
                Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoRefazerPesagem = pesagem != null ? repositorioPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.RefazerPesagem) : null;
                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemFinalAnexo> anexosGuarita = repositorioGuaritaPesagemFinalAnexo.BuscarPorGuarita(codigo);
                List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> balancas = existeIntegracaoBalancaDeca && cargaGuarita.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InicioViagem ? repositorioFilialBalanca.BuscarPorFilial(cargaGuarita.FluxoGestaoPatio.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca>();

                return new JsonpResult(new
                {
                    PesagemFinal = cargaGuarita.PesagemFinal > 0 ? cargaGuarita.PesagemFinal.ToString("n2") : string.Empty,
                    PorcentagemPerda = cargaGuarita.PorcentagemPerda > 0 ? cargaGuarita.PorcentagemPerda.ToString("n2") : string.Empty,
                    CodigoPesagem = pesagem?.Codigo ?? 0,
                    StatusBalanca = pesagem?.StatusBalanca ?? StatusBalanca.Todos,
                    PermiteRefazerPesagem = (
                        (integracaoPesagemFinal?.SituacaoIntegracao ?? SituacaoIntegracao.ProblemaIntegracao) == SituacaoIntegracao.Integrado &&
                        (integracaoRefazerPesagem?.SituacaoIntegracao ?? SituacaoIntegracao.ProblemaIntegracao) != SituacaoIntegracao.Integrado &&
                        (existeIntegracaoBalancaToledo && integracaoPesagemFinal?.TipoIntegracao.Tipo == TipoIntegracao.Toledo)
                    ),
                    PermiteReenviarConsultaRejeitada = existeIntegracaoBalancaQbit && (integracaoPesagemFinal?.SituacaoIntegracao ?? SituacaoIntegracao.AgIntegracao) == SituacaoIntegracao.ProblemaIntegracao,
                    CodigoPesagemIntegracao = integracaoPesagemFinal?.Codigo ?? 0,
                    ProblemaIntegracao = integracaoPesagemFinal?.ProblemaIntegracao ?? string.Empty,
                    cargaGuarita.LoteInterno,
                    Lacre = cargaGuarita.NumeroLacre,
                    PodeEditar = cargaGuarita.FluxoGestaoPatio.DataInicioViagem == null,
                    ExibirLacre = sequenciaGestaoPatio.GuaritaSaidaPermiteInformarLacrePesagem,
                    ExibirPercentualRefugo = sequenciaGestaoPatio.GuaritaSaidaPermiteInformarPercentualRefugoPesagem,
                    ExibirInformacoesLoteInterno = sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesPesagem && sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesLoteInterno,
                    ExibirAnexos = sequenciaGestaoPatio.GuaritaSaidaPermiteAnexosPesagem,
                    Anexos = (
                        from o in anexosGuarita
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.NomeArquivo
                        }
                    ).ToList(),
                    Balanca = existeIntegracaoBalancaDeca ? new { Codigo = sequenciaGestaoPatio.BalancaGuaritaSaida?.Codigo ?? 0, Descricao = sequenciaGestaoPatio.BalancaGuaritaSaida?.Descricao ?? string.Empty } : null,
                    Balancas = (
                        from o in balancas
                        select new
                        {
                            o.Codigo,
                            o.Descricao
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarInformacoesPesagemLoteInterno()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    cargaGuarita.LoteInterno,
                    cargaGuarita.LoteInternoDois
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarInformacoesPesagemFinal()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoPatio.Guarita servicoGuarita = new Servicos.Embarcador.GestaoPatio.Guarita(unidadeDeTrabalho, Auditado);

                unidadeDeTrabalho.Start();

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoGuaritaDadosPesagem dadosPesagem = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoGuaritaDadosPesagem
                {
                    CodigoGuarita = Request.GetIntParam("CodigoGuarita"),
                    PesagemFinal = Request.GetDecimalParam("PesagemFinal"),
                    PorcentagemPerda = Request.GetDecimalParam("PorcentagemPerda"),
                    NumeroLacre = Request.GetStringParam("Lacre"),
                    Usuario = Usuario
                };

                servicoGuarita.SalvarInformacoesPesagemFinal(dadosPesagem);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as informações da pesagem final.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarInformacoesPesagemInicial()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoPatio.Guarita servicoGuarita = new Servicos.Embarcador.GestaoPatio.Guarita(unidadeDeTrabalho, Auditado);

                unidadeDeTrabalho.Start();

                string produtorRural = Request.GetStringParam("ProdutorRural");

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoGuaritaDadosPesagem dadosPesagem = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoGuaritaDadosPesagem
                {
                    PesagemInicial = Request.GetDecimalParam("PesagemInicial"),
                    QuantidadeCaixas = Request.GetIntParam("QuantidadeCaixas"),
                    CodigoGuarita = Request.GetIntParam("CodigoGuarita"),
                    Pressao = Request.GetDecimalParam("Pressao"),
                    Usuario = Usuario
                };

                if (produtorRural == "1")
                    dadosPesagem.ProdutorRural = true;
                else if (produtorRural == "0")
                    dadosPesagem.ProdutorRural = false;
                else
                    dadosPesagem.ProdutorRural = null;

                servicoGuarita.SalvarInformacoesPesagemInicial(dadosPesagem);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as informações da pesagem.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarInformacoesPesagemLoteInterno()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (cargaGuarita == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                unidadeDeTrabalho.Start();

                cargaGuarita.LoteInterno = Request.GetStringParam("LoteInterno");
                cargaGuarita.LoteInternoDois = Request.GetStringParam("LoteInternoDois");

                if (string.IsNullOrWhiteSpace(cargaGuarita.LoteInterno))
                    return new JsonpResult(false, "É obrigatório o preenchimento do lote interno um.");

                repositorioCargaGuarita.Atualizar(cargaGuarita);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as informações de lote.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ValidarRegrasPesagemInicial()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("CodigoGuarita");
                decimal pesagemInicial = Request.GetDecimalParam("PesagemInicial");

                if (pesagemInicial <= 0)
                    return new JsonpResult(false, true, "A pesagem inicial não foi informada.");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCodigo(codigoGuarita);
                if (guarita == null)
                    return new JsonpResult(false, true, "Guarita não foi encontrada.");

                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repositorioPesagem.BuscarPorGuarita(codigoGuarita);
                if (pesagem == null)
                    return new JsonpResult(true);

                Servicos.Embarcador.GestaoPatio.Guarita servicoGuarita = new Servicos.Embarcador.GestaoPatio.Guarita(unidadeDeTrabalho, Auditado);

                servicoGuarita.ValidarRegrasPesagemInicial(pesagem, pesagemInicial);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(true, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao validar as regras da pesagem.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ValidarRegrasPesagemFinal()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("CodigoGuarita");
                decimal pesagemFinal = Request.GetDecimalParam("PesagemFinal");

                if (pesagemFinal <= 0)
                    return new JsonpResult(false, true, "A pesagem final não foi informada.");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCodigo(codigoGuarita);
                if (guarita == null)
                    return new JsonpResult(false, true, "Guarita não foi encontrada.");

                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repositorioPesagem.BuscarPorGuarita(codigoGuarita);
                if (pesagem == null)
                    return new JsonpResult(true);

                Servicos.Embarcador.GestaoPatio.Guarita servicoGuarita = new Servicos.Embarcador.GestaoPatio.Guarita(unidadeDeTrabalho, Auditado);

                servicoGuarita.ValidarRegrasPesagemFinal(pesagem, pesagemFinal);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(true, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao validar as regras da pesagem.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapaEntradaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaGuarita.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível voltar etapa nessa situação.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.Guarita, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao retornar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapaSaidaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoGuarita = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaGuarita.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível voltar etapa nessa situação.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.VoltarEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioViagem, this.Usuario, permissoesPersonalizadas);

                Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorFluxoGestaoPatio(cargaGuarita.FluxoGestaoPatio.Codigo);

                if (cargaControleExpedicao != null)
                {
                    cargaControleExpedicao.SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.AguardandoLiberacao;
                    repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao retornar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarEtapaSaidaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoGuarita = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaGuarita.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível rejeitar fluxo nessa situação.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.RejeitarEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioViagem);
                servicoFluxoGestaoPatio.Auditar(cargaGuarita.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar a saída de veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarEtapaEntradaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaGuarita.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível rejeitar fluxo nessa situação.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.RejeitarEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.Guarita);
                servicoFluxoGestaoPatio.Auditar(cargaGuarita.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar a entrada de veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarEtapaChegadaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaGuarita.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível rejeitar fluxo nessa situação.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.RejeitarEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.ChegadaVeiculo);
                servicoFluxoGestaoPatio.Auditar(cargaGuarita.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar a chegada de veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReabrirFluxo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);

                if (cargaGuarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaGuarita.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");

                if (cargaGuarita.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InicioViagem && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.ReabrirFluxo(cargaGuarita.FluxoGestaoPatio);

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

        public async Task<IActionResult> InformarChegadaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigo);

                if (cargaGuarita == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (cargaGuarita.Situacao != SituacaoCargaGuarita.AgChegadaVeiculo)
                    throw new ControllerException("A situação atual da carga não permite informar a chegada do veículo.");

                if (!ValidarInformacoesObrigatorias(cargaGuarita, unitOfWork))
                    throw new ControllerException("É necessário que as informações de veículo, motorista, transportador e número NF do produtor estejam informadas para prosseguir.");

                bool chegadaInformadaNaGuarita = Request.GetBoolParam("ChegadaInformadaNaGuarita");
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();
                Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repFluxoPatioIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao tipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                bool permitirInformarComEtapaBloqueada = chegadaInformadaNaGuarita && configuracaoGestaoPatio.ChegadaVeiculoPermiteInformarComEtapaBloqueada;

                if (cargaGuarita.FluxoGestaoPatio.CargaBase.Veiculo != null)
                    cargaGuarita.EtapaGuaritaLiberada = true;

                if (!permitirInformarComEtapaBloqueada && !cargaGuarita.EtapaGuaritaLiberada)
                    throw new ControllerException("Ainda não foi autorizado informar a chegada do veículo para esta carga.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (configuracaoEmbarcador.InformarDadosChegadaVeiculoNoFluxoPatio && !chegadaInformadaNaGuarita)
                    SalvarInformacaoChegadaVeiculo(cargaGuarita, unitOfWork);

                cargaGuarita.DocaChegadaGuarita = Request.GetStringParam("DocaChegada");
                cargaGuarita.Carga.NumeroDoca = Request.GetStringParam("DocaChegada");
                cargaGuarita.SenhaChegadaGuarita = Request.GetStringParam("SenhaChegada");

                cargaGuarita.Situacao = SituacaoCargaGuarita.AguardandoLiberacao;
                cargaGuarita.DataChegadaVeiculo = DateTime.Now;
                cargaGuarita.ChegadaDenegada = false;

                repositorioCargaGuarita.Atualizar(cargaGuarita);

                Servicos.Embarcador.Integracao.Eship.IntegracaoEship serEShip = new Servicos.Embarcador.Integracao.Eship.IntegracaoEship(unitOfWork);
                serEShip.VerificarIntegracaoEShip(cargaGuarita.Carga);

                AdicionarAdvertenciaTransportadorPorAtrasoNaChegada(cargaGuarita, unitOfWork);

                if (cargaGuarita.EtapaGuaritaLiberada)
                    servicoFluxoGestaoPatio.LiberarProximaEtapa(cargaGuarita.FluxoGestaoPatio, EtapaFluxoGestaoPatio.ChegadaVeiculo);
                else
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaGuarita.FluxoGestaoPatio, null, $"Informou a chegada do veículo {(chegadaInformadaNaGuarita ? "na guarita" : "pelo fluxo de pátio")} sem avançar a etapa.", unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaGuarita, null, $"Informou a chegada do veículo {(chegadaInformadaNaGuarita ? "na guarita" : "pelo fluxo de pátio")}{(cargaGuarita.EtapaGuaritaLiberada ? "" : " sem avançar a etapa")}.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDetalhesCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita = repCargaGuarita.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamentoGuarita?.Carga == null)
                    return new JsonpResult(true, false, "Não foi possível encontrar a carga para gerar o relatório de detalhes.");

                byte[] pdf = Servicos.Embarcador.Carga.Carga.GerarRelatorioDetalhesCarga(cargaJanelaCarregamentoGuarita.Carga.Codigo, unidadeTrabalho);

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o relatório de detalhes da carga. Tente novamente.");

                return Arquivo(pdf, "application/pdf", "Carga " + cargaJanelaCarregamentoGuarita.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do contrato de frete.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadOrdemColetaGuarita()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita = repCargaGuarita.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamentoGuarita?.Carga == null)
                    return new JsonpResult(true, false, "Não foi possível encontrar a carga para gerar a ordem de coleta.");

                byte[] pdf = Servicos.Embarcador.Carga.Carga.GerarOrdemColetaGuarita(cargaJanelaCarregamentoGuarita.Carga.Codigo, unidadeTrabalho);

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o relatório de ordem de coleta. Tente novamente.");

                return Arquivo(pdf, "application/pdf", "OrdemColeta" + cargaJanelaCarregamentoGuarita.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da ordem de coleta.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarDetalhesCargaPorEmail()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamentoGuarita?.Carga == null)
                    return new JsonpResult(true, false, "Não foi possível encontrar a carga para gerar o relatório de detalhes.");

                string observacao = Request.GetStringParam("Observacao");
                dynamic emails = JsonConvert.DeserializeObject<dynamic>(Request.Params("Emails"));
                List<string> listaEmails = new List<string>();

                foreach (dynamic email in emails)
                    listaEmails.Add((string)email.Email);

                string mensagemErro;

                if (!Servicos.Embarcador.Carga.Carga.EnviarRelatorioDetalhesCargaPorEmail(cargaJanelaCarregamentoGuarita.Carga.Codigo, listaEmails, observacao, unidadeTrabalho, Usuario, out mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamentoGuarita, null, "Enviou detalhes da Carga por E-mail.", unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar os detalhes da carga por e-mail.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DenegarChegada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("CodigoGuarita");

                if (codigo <= 0)
                    throw new ControllerException("O código do registro não foi recebido.");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorCodigo(codigo);

                if (guarita == null)
                    throw new ControllerException("O registro não foi encontrado.");

                if (guarita.Carga?.SituacaoCarga == SituacaoCarga.Cancelada || guarita.Carga?.SituacaoCarga == SituacaoCarga.Anulada || guarita.Carga?.SituacaoCarga == SituacaoCarga.Encerrada)
                    throw new ControllerException("A carga já foi cancelada.");

                if (guarita.Carga?.SituacaoCarga.IsSituacaoCargaFaturada() ?? false)
                    throw new ControllerException("A carga já foi faturada.");

                unitOfWork.Start();

                guarita.ObservacaoChegadaDenegada = Request.GetStringParam("Observacao");
                guarita.ChegadaDenegada = true;

                repositorioGuarita.Atualizar(guarita);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao denegar chegada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDataChegadaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Carga");
                DateTime dataChegada = Request.GetDateTimeParam("DataChegada");


                if (dataChegada == DateTime.MinValue)
                    throw new ControllerException("Informe uma data valida");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioGuarita.BuscarPorCodigo(codigo);


                if (cargaGuarita == null)
                    return new JsonpResult(true, false, "Carga não encontrada");

                unitOfWork.Start();

                cargaGuarita.DataChegadaVeiculo = dataChegada;
                repositorioGuarita.Atualizar(cargaGuarita);

                unitOfWork.CommitChanges();

                return new JsonpResult(null, true, "Data atualizada com sucesso");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacaoPesagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem repositorioAprovacao = new Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    Data = autorizacao.Data?.ToString("dd/MM/yyyy") ?? string.Empty,
                    Motivo = autorizacao.Motivo
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

        public async Task<IActionResult> PesquisaAutorizacaoPesagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGuarita = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorCodigo(codigoGuarita);

                if (guarita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if ((guarita.SituacaoPesagemCarga ?? SituacaoPesagemCarga.NaoInformada) == SituacaoPesagemCarga.NaoInformada)
                    return new JsonpResult(null);

                if (guarita.SituacaoPesagemCarga == SituacaoPesagemCarga.SemRegraAprovacao)
                {
                    return new JsonpResult(new
                    {
                        AprovacoesNecessarias = "",
                        Aprovacoes = "",
                        CorIconeAba = guarita.SituacaoPesagemCarga.Value.ObterCorIcone(),
                        PossuiRegras = false,
                        Reprovacoes = "",
                        Autorizacoes = new List<dynamic>()
                    });
                }
                else
                {
                    Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem repositorioAprovacao = new Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem(unitOfWork);
                    List<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem> listaAutorizacao = repositorioAprovacao.ConsultaAutorizacoes(codigoGuarita, parametroConsulta: null);
                    int aprovacoes = repositorioAprovacao.ContarAprovacoes(codigoGuarita);
                    int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(codigoGuarita);
                    int reprovacoes = repositorioAprovacao.ContarReprovacoes(codigoGuarita);

                    return new JsonpResult(new
                    {
                        AprovacoesNecessarias = aprovacoesNecessarias,
                        Aprovacoes = aprovacoes,
                        CorIconeAba = guarita.SituacaoPesagemCarga.Value.ObterCorIcone(),
                        PossuiRegras = true,
                        Reprovacoes = reprovacoes,
                        Autorizacoes = (
                            from autorizacao in listaAutorizacao
                            select new
                            {
                                autorizacao.Codigo,
                                PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                                Situacao = autorizacao.Situacao.ObterDescricao(),
                                Usuario = autorizacao.Usuario?.Nome,
                                Regra = autorizacao.Descricao,
                                Data = autorizacao.Data?.ToString() ?? string.Empty,
                                Motivo = autorizacao.Motivo,
                                DT_RowColor = autorizacao.ObterCorGrid()
                            }
                        ).ToList()
                    });
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as autorizações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AdicionarAdvertenciaTransportadorPorAtrasoNaChegada(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita, Repositorio.UnitOfWork unitOfWork)
        {
            if ((cargaGuarita.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Destino) || (cargaGuarita.CargaJanelaCarregamento?.CentroCarregamento?.MotivoAdvertenciaChegadaEmAtraso == null))
                return;

            DateTime dataLimiteChegadaVeiculo = cargaGuarita.DataProgramadaParaChegada.AddMinutes(cargaGuarita.CargaJanelaCarregamento.CentroCarregamento.TempoToleranciaChegadaAtraso);

            if (cargaGuarita.DataChegadaVeiculo <= dataLimiteChegadaVeiculo)
                return;

            Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador repositorioAdvertenciaTransportador = new Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador advertenciaTransportador = new Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador();

            advertenciaTransportador.Data = DateTime.Now;
            advertenciaTransportador.Motivo = cargaGuarita.CargaJanelaCarregamento.CentroCarregamento.MotivoAdvertenciaChegadaEmAtraso;
            advertenciaTransportador.Observacao = $"Chegada atrasada para a carga {(cargaGuarita.Carga?.CodigoCargaEmbarcador ?? "")}. Horario Limite: {dataLimiteChegadaVeiculo.ToString("dd/MM/yyyy HH:mm:ss")}; Horario Chegada: {cargaGuarita.DataChegadaVeiculo.Value.ToString("dd/MM/yyyy HH:mm:ss")}";
            advertenciaTransportador.Motivo = cargaGuarita.CargaJanelaCarregamento.CentroCarregamento.MotivoAdvertenciaChegadaEmAtraso;
            advertenciaTransportador.Pontuacao = advertenciaTransportador.Motivo.Pontuacao;
            advertenciaTransportador.Transportador = cargaGuarita.Carga?.Empresa;
            advertenciaTransportador.Usuario = this.Usuario;

            repositorioAdvertenciaTransportador.Inserir(advertenciaTransportador);
        }

        private void AtualizarInformacaoChegadaVeiculoNfe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo).FirstOrDefault();

            if (cargaPedido != null)
            {
                dynamic listaNfe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaNfeChegadaVeiculo"));

                ExcluirInformacaoChegadaVeiculoNfeRemovidas(cargaPedido, listaNfe, unidadeDeTrabalho);
                SalvarInformacaoChegadaVeiculoNfeAdicionadas(cargaPedido, listaNfe, unidadeDeTrabalho);
            }
        }

        private void ExcluirInformacaoChegadaVeiculoNfeRemovidas(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, dynamic listaNfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (cargaPedido.CargaPedidoXMLNotasFiscaisParcial?.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var nfe in listaNfe)
                {
                    int? codigo = ((string)nfe.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> listaNfeRemover = (from nfeParcial in cargaPedido.CargaPedidoXMLNotasFiscaisParcial where !listaCodigosAtualizados.Contains(nfeParcial.Codigo) select nfeParcial).ToList();
                Servicos.Log.TratarErro($"3 Carga Pedido Parcial deletados {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")} {string.Join(", ", listaNfeRemover.Select(x => x.Chave).ToList())} ");
                foreach (var nfe in listaNfeRemover)
                    repositorioNotaFiscalParcial.Deletar(nfe);


            }
        }

        private void SalvarInformacaoChegadaVeiculoNfeAdicionadas(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, dynamic listaNfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> listaNfeAdicionadas = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            foreach (var nfe in listaNfe)
            {
                int? codigo = ((string)nfe.Codigo).ToNullableInt();

                if (codigo.HasValue)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial notaFiscalParcial = (
                        from notasFiscaisParcial in cargaPedido.CargaPedidoXMLNotasFiscaisParcial
                        where notasFiscaisParcial.Codigo == codigo.Value
                        select notasFiscaisParcial
                    ).FirstOrDefault();

                    if (notaFiscalParcial != null)
                        listaNfeAdicionadas.Add(notaFiscalParcial);
                }
                else
                {
                    if (!Utilidades.Validate.ValidarChave((string)nfe.Chave))
                        throw new ControllerException("A chave NF-e informada é inválida");

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial notaFiscalParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial()
                    {
                        CargaPedido = cargaPedido,
                        Chave = (string)nfe.Chave
                    };

                    ValidarnformacaoChegadaVeiculoNfeDuplicada(listaNfeAdicionadas, notaFiscalParcial);

                    repositorioNotaFiscalParcial.Inserir(notaFiscalParcial);

                    listaNfeAdicionadas.Add(notaFiscalParcial);
                }
            }
        }

        private void SalvarInformacaoChegadaVeiculo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaGuarita.CargaBase.IsCarga())
                SalvarInformacaoChegadaVeiculoPorCarga(cargaGuarita.FluxoGestaoPatio, unitOfWork);
            else
                SalvarInformacaoChegadaVeiculoPorPreCarga(cargaGuarita.FluxoGestaoPatio, unitOfWork);
        }

        private void SalvarNumeroLacreNaCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            if (fluxoGestaoPatio.Carga == null)
                return;

            string numeroLacre = Request.GetStringParam("NumeroLacre");

            if (string.IsNullOrWhiteSpace(numeroLacre))
                return;

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            servicoCarga.ValidarPermissaoAlterarDadosEtapaFrete(fluxoGestaoPatio.Carga, unitOfWork);

            Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre = new Dominio.Entidades.Embarcador.Cargas.CargaLacre
            {
                Carga = fluxoGestaoPatio.Carga,
                Numero = numeroLacre
            };

            repositorioCargaLacre.Inserir(cargaLacre, Auditado);
        }

        private void SalvarInformacaoChegadaVeiculoPorCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            if (fluxoGestaoPatio == null)
                throw new ControllerException("Não foi possível encontrar o fluxo de pátio");

            int codigoMotorista = Request.GetIntParam("CodigoMotoristaChegada");
            string cpfMotorista = Request.GetStringParam("CpfMotoristaChegada").ObterSomenteNumeros();
            string nomeMotorista = Request.GetStringParam("MotoristaChegada");
            string placaReboque = Request.GetStringParam("ReboqueChegada").ToUpper();
            string placaVeiculo = Request.GetStringParam("VeiculoChegada").ToUpper();
            string rgMotorista = Request.GetStringParam("RgMotoristaChegada");
            string telefoneMotorista = Request.GetStringParam("TelefoneChegada");

            if (string.IsNullOrWhiteSpace(cpfMotorista))
                throw new ControllerException("É obrigatório informar o CPF do motorista.");
            else if (!Utilidades.Validate.ValidarCPF(cpfMotorista))
                throw new ControllerException("O CPF do motorista informado é inválido.");

            if (string.IsNullOrWhiteSpace(nomeMotorista))
                throw new ControllerException("É obrigatório informar o nome do motorista.");

            if (string.IsNullOrWhiteSpace(placaVeiculo) || placaVeiculo.Length != 7)
                throw new ControllerException("É obrigatório informar o veículo.");

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresaPai = repositorioEmpresa.BuscarEmpresaPai();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = fluxoGestaoPatio.Carga;

            if (carga.Empresa == null || carga.Empresa == empresaPai)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(empresaPai.Codigo, placaVeiculo);
                Dominio.Entidades.Veiculo reboque = repositorioVeiculo.BuscarPorPlaca(empresaPai.Codigo, placaReboque);

                if (veiculo == null)
                    veiculo = Servicos.Veiculo.PreencherVeiculoGenerico(placaVeiculo, empresaPai, unitOfWork);

                if (reboque == null && !string.IsNullOrWhiteSpace(placaReboque) && placaVeiculo != placaReboque)
                {
                    reboque = Servicos.Veiculo.PreencherVeiculoGenerico(placaReboque, empresaPai, unitOfWork);
                    reboque.TipoVeiculo = "1";
                }

                Dominio.Entidades.Usuario motorista = null;

                if (codigoMotorista > 0)
                    motorista = repositorioUsuario.BuscarMotoristaPorCodigoEEmpresa(empresaPai.Codigo, codigoMotorista);
                else
                    motorista = repositorioUsuario.BuscarMotoristaPorCPFEEmpresa(cpfMotorista, empresaPai.Codigo);

                if (motorista == null)
                {
                    motorista = Servicos.Usuario.PreencherMotoristaGenerico(nomeMotorista, empresaPai, unitOfWork);

                    motorista.CPF = cpfMotorista;
                    motorista.Status = "A";
                }
                else
                    motorista.Initialize();

                motorista.Nome = nomeMotorista;
                motorista.RG = rgMotorista;
                motorista.Telefone = telefoneMotorista;

                if (reboque != null)
                    repositorioVeiculo.Atualizar(reboque);

                repositorioVeiculo.Atualizar(veiculo);
                repositorioUsuario.Atualizar(motorista, (motorista.IsInitialized() ? Auditado : null));

                if ((carga.Veiculo?.Codigo ?? 0) != veiculo.Codigo)
                    Servicos.Embarcador.GestaoPatio.DisponibilidadeVeiculo.SetaVeiculoDisponivel(veiculo.Codigo, unitOfWork);

                carga.VeiculosVinculados.Clear();

                if (reboque != null)
                    carga.VeiculosVinculados.Add(reboque);

                carga.Empresa = empresaPai;
                carga.Veiculo = veiculo;
                carga.Motoristas.Clear();
                carga.Motoristas.Add(motorista);
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada;
                carga.DataEncerramentoCarga = DateTime.Now;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada.ObterDescricao()}", unitOfWork);

                if (fluxoGestaoPatio != null)
                {
                    Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

                    fluxoGestaoPatio.Veiculo = veiculo;

                    repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
                }

                repositorioCarga.Atualizar(carga);

                AtualizarInformacaoChegadaVeiculoNfe(carga, unitOfWork);
            }

            SalvarNumeroLacreNaCarga(fluxoGestaoPatio, unitOfWork);
            Servicos.Embarcador.GestaoPatio.DisponibilidadeVeiculo.GeraControleDisponibilidadeVeiculo(fluxoGestaoPatio, unitOfWork);
        }

        private void SalvarInformacaoChegadaVeiculoPorPreCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            if (fluxoGestaoPatio == null)
                throw new ControllerException("Não foi possível encontrar o fluxo de pátio");

            int codigoMotorista = Request.GetIntParam("CodigoMotoristaChegada");
            string cpfMotorista = Request.GetStringParam("CpfMotoristaChegada").ObterSomenteNumeros();
            string nomeMotorista = Request.GetStringParam("MotoristaChegada");
            string placaReboque = Request.GetStringParam("ReboqueChegada").ToUpper();
            string placaVeiculo = Request.GetStringParam("VeiculoChegada").ToUpper();
            string rgMotorista = Request.GetStringParam("RgMotoristaChegada");
            string telefoneMotorista = Request.GetStringParam("TelefoneChegada");

            if (string.IsNullOrWhiteSpace(cpfMotorista))
                throw new ControllerException("É obrigatório informar o CPF do motorista.");
            else if (!Utilidades.Validate.ValidarCPF(cpfMotorista))
                throw new ControllerException("O CPF do motorista informado é inválido.");

            if (string.IsNullOrWhiteSpace(nomeMotorista))
                throw new ControllerException("É obrigatório informar o nome do motorista.");

            if (string.IsNullOrWhiteSpace(placaVeiculo) || placaVeiculo.Length != 7)
                throw new ControllerException("É obrigatório informar o veículo.");

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresaPai = repositorioEmpresa.BuscarEmpresaPai();
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = fluxoGestaoPatio.PreCarga;

            if (preCarga.Empresa == null || preCarga.Empresa == empresaPai)
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(empresaPai.Codigo, placaVeiculo);
                Dominio.Entidades.Veiculo reboque = repositorioVeiculo.BuscarPorPlaca(empresaPai.Codigo, placaReboque);

                if (veiculo == null)
                    veiculo = Servicos.Veiculo.PreencherVeiculoGenerico(placaVeiculo, empresaPai, unitOfWork);

                if (reboque == null && !string.IsNullOrWhiteSpace(placaReboque) && placaVeiculo != placaReboque)
                {
                    reboque = Servicos.Veiculo.PreencherVeiculoGenerico(placaReboque, empresaPai, unitOfWork);
                    reboque.TipoVeiculo = "1";
                }

                Dominio.Entidades.Usuario motorista = null;

                if (codigoMotorista > 0)
                    motorista = repositorioUsuario.BuscarMotoristaPorCodigoEEmpresa(empresaPai.Codigo, codigoMotorista);
                else
                    motorista = repositorioUsuario.BuscarMotoristaPorCPFEEmpresa(cpfMotorista, empresaPai.Codigo);

                if (motorista == null)
                {
                    motorista = Servicos.Usuario.PreencherMotoristaGenerico(nomeMotorista, empresaPai, unitOfWork);

                    motorista.CPF = cpfMotorista;
                    motorista.Status = "A";
                }
                else
                    motorista.Initialize();

                motorista.Nome = nomeMotorista;
                motorista.RG = rgMotorista;
                motorista.Telefone = telefoneMotorista;

                if (reboque != null)
                    repositorioVeiculo.Atualizar(reboque);

                repositorioVeiculo.Atualizar(veiculo);
                repositorioUsuario.Atualizar(motorista, (motorista.IsInitialized() ? Auditado : null));

                if ((preCarga.Veiculo?.Codigo ?? 0) != veiculo.Codigo)
                    Servicos.Embarcador.GestaoPatio.DisponibilidadeVeiculo.SetaVeiculoDisponivel(veiculo.Codigo, unitOfWork);

                preCarga.VeiculosVinculados.Clear();

                if (reboque != null)
                    preCarga.VeiculosVinculados.Add(reboque);

                preCarga.Empresa = empresaPai;
                preCarga.Veiculo = veiculo;
                preCarga.Motoristas.Clear();
                preCarga.Motoristas.Add(motorista);

                if (fluxoGestaoPatio != null)
                {
                    Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

                    fluxoGestaoPatio.Veiculo = veiculo;

                    repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
                }

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

                repositorioPreCarga.Atualizar(preCarga);
            }

            SalvarNumeroLacreNaCarga(fluxoGestaoPatio, unitOfWork);
            Servicos.Embarcador.GestaoPatio.DisponibilidadeVeiculo.GeraControleDisponibilidadeVeiculo(fluxoGestaoPatio, unitOfWork);
        }

        private List<dynamic> ObterDadosGuaritas(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> guaritas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork, cancellationToken);


            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();


            List<dynamic> listaDadosGuaritas = new List<dynamic>();

            if (guaritas.IsNullOrEmpty())
                return listaDadosGuaritas;

            List<int> guaritasGestao = guaritas.Select(x => x.Codigo).Distinct().ToList();
            List<int> cargasGuaritas = guaritas.Select(x => x.Carga.Codigo).Distinct().ToList();
            IList<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SequenciaGestaoPatioGuarita> sequenciaGestaoPatios = repSequenciaGestaoPatio.ObterSequenciaGestaoPatioGuaritaAsync(guaritasGestao).Result;
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga> cargasJanelasCarregamento = repositorioCargaJanelaCarregamento.BuscarCargasJanelaCarregamentoGuaritaAsync(cargasGuaritas).Result;
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AgendamentoColeta> agendamentosColetas = repositorioAgendamentoColeta.BuscarPorAgendamentoAsync(cargasGuaritas).Result;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita in guaritas)
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SequenciaGestaoPatioGuarita sequenciaGestaoPatio = sequenciaGestaoPatios.FirstOrDefault(x => x.CodigoGuarita == guarita.Codigo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = guarita.Carga;
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = guarita.PreCarga;
                Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault();
                Dominio.Entidades.Usuario ajudante = carga.Ajudantes?.FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga cargaJanelaCarregamento = cargasJanelasCarregamento.FirstOrDefault(x => x.CodigoCarga == carga.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = agendamentosColetas.FirstOrDefault(x => x.CodigoCarga == carga.Codigo);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCarga(carga.Codigo);

                List<(string Nome, double CNPJ)> clientesOrigem = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork).BuscarNomeECNPJsClienteOrigem(carga.Codigo);
                bool exibirDataCarregamentoExato = sequenciaGestaoPatio?.GuaritaEntradaExibirHorarioExato ?? false;
                bool exibirImprimirTicketBalanca = repSequenciaGestaoPatio.ExibirImprimirTicketBalanca(carga.Filial?.Codigo ?? 0);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> listaNotaFiscalParcial = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres = repositorioCargaLacre.BuscarPorCarga(guarita.Carga.Codigo);

                if (cargaPedido != null)
                    listaNotaFiscalParcial = cargaPedido.CargaPedidoXMLNotasFiscaisParcial?.ToList();

                dynamic dadosGuarita = null;
                if (guarita.Carga != null)
                    dadosGuarita = new
                    {
                        guarita.Codigo,
                        Carga = carga.Codigo,
                        PreCarga = preCarga?.Codigo ?? 0,
                        DataInicioViagem = string.Empty,
                        DataAgendada = agendamentoColeta?.DataEntrega?.ToString("dd/MM/yyyy HH:mm") ?? carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataCarregamento = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:" + (exibirDataCarregamentoExato ? "mm" : "00")) ?? string.Empty,
                        NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty,
                        CodigosAgrupadosCarga = string.Join(", ", carga.CodigosAgrupados),
                        ObservacaoGuarita = guarita.Observacao,
                        ObservacaoGuaritaChegadaDenegada = guarita.Observacao,
                        NumeroPreCarga = preCarga?.NumeroPreCarga ?? "",
                        Transportador = !string.IsNullOrWhiteSpace(agendamentoColeta?.TransportadorManual) ? agendamentoColeta.TransportadorManual : (carga.Empresa?.Descricao ?? string.Empty),
                        Motorista = motorista?.Nome ?? string.Empty,
                        Ajudante = ajudante?.Nome ?? string.Empty,
                        AjudanteCpf = ajudante?.CPF_CNPJ_Formatado ?? string.Empty,
                        MotoristaTelefone = motorista?.Telefone ?? string.Empty,
                        MotoristaCelular = motorista?.Celular ?? string.Empty,
                        Veiculo = carga.RetornarPlacas,
                        ModeloVeiculo = carga.ModeloVeicularCarga?.Descricao ?? "",
                        DescricaoSituacao = guarita.Situacao.ObterDescricao(),
                        guarita.Situacao,
                        guarita.NumeroNfProdutor,
                        DescricaoTipoChegadaGuarita = guarita.TipoChegadaGuarita.ObterDescricao(),
                        guarita.TipoChegadaGuarita,
                        guarita.PossuiDevolucao,
                        guarita.ObservacaoDevolucao,
                        InformarChegadaVeiculo = sequenciaGestaoPatio?.ChegadaVeiculo ?? true,
                        LiberadaSaida = guarita.DataSaidaGuarita.HasValue,
                        SomentePreCarga = false,
                        Remetente = carga.DadosSumarizados?.Remetentes ?? string.Empty,
                        TipoCarga = carga.TipoDeCarga?.Descricao ?? string.Empty,
                        TipoOperacao = carga.TipoOperacao?.Descricao ?? string.Empty,
                        VeiculoChegada = carga.Veiculo?.Placa ?? string.Empty,
                        ReboqueChegada = carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                        MotoristaChegada = motorista?.Nome ?? string.Empty,
                        TelefoneChegada = motorista?.Telefone ?? string.Empty,
                        CodigoMotoristaChegada = motorista?.Codigo ?? 0,
                        CpfMotoristaChegada = motorista?.CPF_Formatado ?? string.Empty,
                        RgMotoristaChegada = motorista?.RG ?? string.Empty,
                        ListaNfeChegadaVeiculo = (
                             from nfe in listaNotaFiscalParcial
                             select new
                             {
                                 nfe.Codigo,
                                 nfe.Chave
                             }
                         ).ToList(),
                        Destinatario = carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                        CodigoIntegracaoDestinatario = carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                        ObservacaoFluxoPatio = guarita.CargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                        Doca = !string.IsNullOrWhiteSpace(guarita.Carga?.NumeroDocaEncosta) ? guarita.Carga?.NumeroDocaEncosta : guarita.Carga?.NumeroDoca ?? string.Empty,
                        TempoJanela = cargaJanelaCarregamento?.DataInicioCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        PermiteInformarMotoristaEVeiculo = carga.Veiculo == null || (carga.Motoristas == null || carga.Motoristas?.Count == 0),
                        guarita.ChegadaDenegada,
                        GuaritaEntradaPermiteDenegarChegada = (sequenciaGestaoPatio?.GuaritaEntradaPermiteDenegarChegada ?? false) && carga.SituacaoCarga.IsSituacaoCargaNaoFaturada(),
                        PermiteInformarDadosDevolucao = sequenciaGestaoPatio?.GuaritaEntradaPermiteInformarDadosDevolucao ?? false,
                        DT_RowColor = ObterRowColor(guarita),
                        DT_FontColor = ObterFontColor(guarita),
                        CodigoFluxoGestaoPatio = guarita.FluxoGestaoPatio?.Codigo ?? 0,
                        ProvedoresOS = carga.DadosSumarizados?.ProvedoresOS ?? string.Empty,
                        DataChegadaVeiculo = guarita.DataChegadaVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        PermiteVisualizarAnexos = carga.TipoOperacao?.ConfiguracaoCarga?.PermiteAdicionarAnexosGuarita ?? false,
                        PermitirAlterarDataChegadaVeiculo = carga?.TipoOperacao?.PermitirAlterarDataChegadaVeiculo ?? false,
                        PossuiPesagemInicial = guarita.PesagemInicial > 0,
                        PossuiPesagemFinal = guarita.PesagemFinal > 0,
                        NumeroLacreChegada = string.Join(", ", cargaLacres.Select(o => o.Numero).ToList()),
                        PermiteImprimirOrdemColetaNaGuarita = carga?.TipoOperacao?.PermiteImprimirOrdemColetaNaGuarita ?? false,
                        PermiteGerarAtendimento = configuracaoGestaoPatio?.PermiteGerarAtendimento ?? false,
                        Atendimento = chamado?.Numero ?? 0,
                        CPFMotorista = motorista?.CPF_CNPJ_Formatado ?? string.Empty,
                        DocaChegada = guarita.DocaChegadaGuarita ?? string.Empty,
                        SenhaChegada = guarita.SenhaChegadaGuarita ?? string.Empty,
                        ExibirImprimirTicketBalanca = exibirImprimirTicketBalanca,
                        RemetentePedido = string.Join(", ", clientesOrigem.Select(o => o.Nome)),
                        CNPJRemetentePedido = string.Join(", ", clientesOrigem.Select(o => o.CNPJ.ToString().ObterCpfOuCnpjFormatado())),
                    };
                else
                    dadosGuarita = ObterDadosGuaritaPorPreCarga(guarita, unitOfWork);

                listaDadosGuaritas.Add(dadosGuarita);
            }
            return listaDadosGuaritas;
        }

        private dynamic ObterDadosGuarita(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            if (guarita.Carga != null)
                return ObterDadosGuaritaPorCarga(guarita, configuracaoEmbarcador, unitOfWork);

            return ObterDadosGuaritaPorPreCarga(guarita, unitOfWork);
        }

        private dynamic ObterDadosGuaritaPorCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(guarita.FluxoGestaoPatio);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = guarita.Carga;
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = guarita.PreCarga;
            Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault();
            Dominio.Entidades.Usuario ajudante = carga.Ajudantes?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCarga(carga.Codigo);

            List<(string Nome, double CNPJ)> clientesOrigem = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork).BuscarNomeECNPJsClienteOrigem(carga.Codigo);

            bool exibirDataCarregamentoExato = sequenciaGestaoPatio?.GuaritaEntradaExibirHorarioExato ?? false;
            bool exibirImprimirTicketBalanca = repSequenciaGestaoPatio.ExibirImprimirTicketBalanca(carga.Filial?.Codigo ?? 0);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> listaNotaFiscalParcial = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres = repositorioCargaLacre.BuscarPorCarga(guarita.Carga.Codigo);

            if (cargaPedido != null)
                listaNotaFiscalParcial = cargaPedido.CargaPedidoXMLNotasFiscaisParcial?.ToList();

            var retorno = new
            {
                guarita.Codigo,
                Carga = carga.Codigo,
                PreCarga = preCarga?.Codigo ?? 0,
                DataInicioViagem = string.Empty,
                DataAgendada = agendamentoColeta?.DataEntrega?.ToString("dd/MM/yyyy HH:mm") ?? carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataCarregamento = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:" + (exibirDataCarregamentoExato ? "mm" : "00")) ?? string.Empty,
                NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty,
                CodigosAgrupadosCarga = string.Join(", ", carga.CodigosAgrupados),
                ObservacaoGuarita = guarita.Observacao,
                ObservacaoGuaritaChegadaDenegada = guarita.Observacao,
                NumeroPreCarga = preCarga?.NumeroPreCarga ?? "",
                Transportador = !string.IsNullOrWhiteSpace(agendamentoColeta?.TransportadorManual) ? agendamentoColeta.TransportadorManual : (carga.Empresa?.Descricao ?? string.Empty),
                Motorista = motorista?.Nome ?? string.Empty,
                Ajudante = ajudante?.Nome ?? string.Empty,
                AjudanteCpf = ajudante?.CPF_CNPJ_Formatado ?? string.Empty,
                MotoristaTelefone = motorista?.Telefone ?? string.Empty,
                MotoristaCelular = motorista?.Celular ?? string.Empty,
                Veiculo = carga.RetornarPlacas,
                ModeloVeiculo = carga.ModeloVeicularCarga?.Descricao ?? "",
                DescricaoSituacao = guarita.Situacao.ObterDescricao(),
                guarita.Situacao,
                guarita.NumeroNfProdutor,
                DescricaoTipoChegadaGuarita = guarita.TipoChegadaGuarita.ObterDescricao(),
                guarita.TipoChegadaGuarita,
                guarita.PossuiDevolucao,
                guarita.ObservacaoDevolucao,
                InformarChegadaVeiculo = sequenciaGestaoPatio?.ChegadaVeiculo ?? true,
                LiberadaSaida = guarita.DataSaidaGuarita.HasValue,
                SomentePreCarga = false,
                Remetente = carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = carga.TipoOperacao?.Descricao ?? string.Empty,
                VeiculoChegada = carga.Veiculo?.Placa ?? string.Empty,
                ReboqueChegada = carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                MotoristaChegada = motorista?.Nome ?? string.Empty,
                TelefoneChegada = motorista?.Telefone ?? string.Empty,
                CodigoMotoristaChegada = motorista?.Codigo ?? 0,
                CpfMotoristaChegada = motorista?.CPF_Formatado ?? string.Empty,
                RgMotoristaChegada = motorista?.RG ?? string.Empty,
                ListaNfeChegadaVeiculo = (
                    from nfe in listaNotaFiscalParcial
                    select new
                    {
                        nfe.Codigo,
                        nfe.Chave
                    }
                ).ToList(),
                Destinatario = carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                ObservacaoFluxoPatio = guarita.CargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                Doca = !string.IsNullOrWhiteSpace(guarita.Carga?.NumeroDocaEncosta) ? guarita.Carga?.NumeroDocaEncosta : guarita.Carga?.NumeroDoca ?? string.Empty,
                TempoJanela = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                PermiteInformarMotoristaEVeiculo = carga.Veiculo == null || (carga.Motoristas == null || carga.Motoristas?.Count == 0),
                guarita.ChegadaDenegada,
                GuaritaEntradaPermiteDenegarChegada = (sequenciaGestaoPatio?.GuaritaEntradaPermiteDenegarChegada ?? false) && carga.SituacaoCarga.IsSituacaoCargaNaoFaturada(),
                PermiteInformarDadosDevolucao = sequenciaGestaoPatio?.GuaritaEntradaPermiteInformarDadosDevolucao ?? false,
                DT_RowColor = ObterRowColor(guarita),
                DT_FontColor = ObterFontColor(guarita),
                CodigoFluxoGestaoPatio = guarita.FluxoGestaoPatio?.Codigo ?? 0,
                ProvedoresOS = carga.DadosSumarizados?.ProvedoresOS ?? string.Empty,
                DataChegadaVeiculo = guarita.DataChegadaVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                PermiteVisualizarAnexos = carga.TipoOperacao?.ConfiguracaoCarga?.PermiteAdicionarAnexosGuarita ?? false,
                PermitirAlterarDataChegadaVeiculo = carga?.TipoOperacao?.PermitirAlterarDataChegadaVeiculo ?? false,
                PossuiPesagemInicial = guarita.PesagemInicial > 0,
                PossuiPesagemFinal = guarita.PesagemFinal > 0,
                NumeroLacreChegada = string.Join(", ", cargaLacres.Select(o => o.Numero).ToList()),
                PermiteImprimirOrdemColetaNaGuarita = carga?.TipoOperacao?.PermiteImprimirOrdemColetaNaGuarita ?? false,
                PermiteGerarAtendimento = configuracaoGestaoPatio?.PermiteGerarAtendimento ?? false,
                Atendimento = chamado?.Numero ?? 0,
                CPFMotorista = motorista?.CPF_CNPJ_Formatado ?? string.Empty,
                DocaChegada = guarita.DocaChegadaGuarita ?? string.Empty,
                SenhaChegada = guarita.SenhaChegadaGuarita ?? string.Empty,
                ExibirImprimirTicketBalanca = exibirImprimirTicketBalanca,
                RemetentePedido = string.Join(", ", clientesOrigem.Select(o => o.Nome)),
                CNPJRemetentePedido = string.Join(", ", clientesOrigem.Select(o => o.CNPJ.ToString().ObterCpfOuCnpjFormatado())),
            };

            return retorno;
        }

        private dynamic ObterDadosGuaritaPorPreCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(guarita.FluxoGestaoPatio);
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = guarita.PreCarga;
            Dominio.Entidades.Usuario motorista = preCarga.Motoristas?.FirstOrDefault();
            bool exibirDataCarregamentoExato = sequenciaGestaoPatio?.GuaritaEntradaExibirHorarioExato ?? false;
            string destinatario = "";
            string codintegdestinatario = "";
            string dataInicioViagemGestaoEntrega = "";
            string dataCarregamento = "";
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> listaNotaFiscalParcial = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres = repositorioCargaLacre.BuscarPorCarga(guarita.Carga.Codigo);

            if (unitOfWork != null)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(preCarga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = preCarga.Pedidos?.FirstOrDefault();

                dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento.ToString($"dd/MM/yyyy HH:{(exibirDataCarregamentoExato ? "mm" : "00")}") ?? "";
                destinatario = pedido?.Destinatario.Nome ?? string.Empty;
                codintegdestinatario = pedido?.Destinatario.CodigoIntegracao ?? string.Empty;
            }

            var retorno = new
            {
                guarita.Codigo,
                Carga = 0,
                PreCarga = preCarga.Codigo,
                DataInicioViagem = dataInicioViagemGestaoEntrega,
                DataCarregamento = dataCarregamento,
                NumeroCarga = "",
                CodigosAgrupadosCarga = "",
                DataAgendada = "",
                ObservacaoGuarita = guarita.Observacao,
                ObservacaoGuaritaChegadaDenegada = guarita.ObservacaoChegadaDenegada,
                NumeroPreCarga = preCarga.NumeroPreCarga ?? "",
                Transportador = preCarga.Empresa?.Descricao ?? string.Empty,
                Motorista = motorista?.Nome ?? string.Empty,
                MotoristaTelefone = motorista?.Telefone ?? string.Empty,
                MotoristaCelular = motorista?.Celular ?? string.Empty,
                Veiculo = preCarga.RetornarPlacas,
                ModeloVeiculo = preCarga.ModeloVeicularCarga?.Descricao,
                DescricaoSituacao = guarita.Situacao.ObterDescricao(),
                guarita.Situacao,
                DescricaoTipoChegadaGuarita = guarita.TipoChegadaGuarita.ObterDescricao(),
                guarita.TipoChegadaGuarita,
                guarita.PossuiDevolucao,
                guarita.ObservacaoDevolucao,
                InformarChegadaVeiculo = sequenciaGestaoPatio?.ChegadaVeiculo ?? true,
                LiberadaSaida = guarita.DataSaidaGuarita.HasValue ? true : false,
                SomentePreCarga = true,
                Remetente = preCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = preCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = preCarga.TipoOperacao?.Descricao ?? string.Empty,
                VeiculoChegada = preCarga.Veiculo?.Placa ?? string.Empty,
                ReboqueChegada = preCarga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                MotoristaChegada = motorista?.Nome ?? string.Empty,
                TelefoneChegada = motorista?.Telefone ?? string.Empty,
                CodigoMotoristaChegada = motorista?.Codigo ?? 0,
                CpfMotoristaChegada = motorista?.CPF_Formatado ?? string.Empty,
                RgMotoristaChegada = motorista?.RG ?? string.Empty,
                ListaNfeChegadaVeiculo = (
                    from nfe in listaNotaFiscalParcial
                    select new
                    {
                        nfe.Codigo,
                        nfe.Chave
                    }
                ).ToList(),
                Destinatario = destinatario,
                CodigoIntegracaoDestinatario = codintegdestinatario,
                ObservacaoFluxoPatio = string.Empty,
                Doca = string.Empty,
                TempoJanela = string.Empty,
                PermiteInformarMotoristaEVeiculo = preCarga.Veiculo == null || (preCarga.Motoristas == null || preCarga.Motoristas?.Count == 0),
                guarita.ChegadaDenegada,
                GuaritaEntradaPermiteDenegarChegada = (sequenciaGestaoPatio?.GuaritaEntradaPermiteDenegarChegada ?? false),
                PermiteInformarDadosDevolucao = sequenciaGestaoPatio?.GuaritaEntradaPermiteInformarDadosDevolucao ?? false,
                DT_RowColor = ObterRowColor(guarita),
                DT_FontColor = ObterFontColor(guarita),
                DataChegadaVeiculo = guarita.DataChegadaVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                PermiteVisualizarAnexos = preCarga.TipoOperacao?.ConfiguracaoCarga?.PermiteAdicionarAnexosGuarita ?? false,
                NumeroLacreChegada = string.Join(", ", cargaLacres.Select(o => o.Numero).ToList()),
                DocaChegada = guarita.DocaChegadaGuarita ?? string.Empty,
                SenhaChegada = guarita.SenhaChegadaGuarita ?? string.Empty,
                RemetentePedido = string.Join(", ", preCarga.Pedidos?.Select(o => o.Remetente?.CPF_CNPJ_Formatado).Distinct().ToList() ?? new List<string>()),
            };

            return retorno;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataCarregamento")
                return "CargaJanelaCarregamento.InicioCarregamento";

            if (propriedadeOrdenar == "NumeroCarga")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Transportador")
                return "Carga.Empresa.RazaoSocial";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "Situacao";

            if (propriedadeOrdenar == "TipoOperacao")
                return "Carga.TipoOperacao";

            return propriedadeOrdenar;
        }

        private string ObterRowColor(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita)
        {
            if (guarita.Carga != null && guarita.Carga.SituacaoCarga == SituacaoCarga.Cancelada)
                return CorGrid.Cinza;

            if (guarita.Situacao == SituacaoCargaGuarita.AguardandoLiberacao)
                return CorGrid.Laranja;

            if (guarita.Situacao == SituacaoCargaGuarita.Liberada || guarita.Situacao == SituacaoCargaGuarita.SaidaLiberada)
                return CorGrid.Verde;

            if (guarita.Situacao == SituacaoCargaGuarita.AgChegadaVeiculo)
                return CorGrid.Amarelo;

            if (guarita.CargaJanelaCarregamento != null && guarita.CargaJanelaCarregamento.InicioCarregamento < DateTime.Now && (guarita.Situacao == SituacaoCargaGuarita.AgChegadaVeiculo || guarita.Situacao == SituacaoCargaGuarita.AguardandoLiberacao))
                return CorGrid.Vermelho;

            return "";
        }

        private string ObterFontColor(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita)
        {
            if (
                guarita.Carga != null && guarita.Carga.SituacaoCarga == SituacaoCarga.Cancelada ||
                (guarita.Situacao == SituacaoCargaGuarita.AguardandoLiberacao && guarita.CargaJanelaCarregamento != null && guarita.CargaJanelaCarregamento.InicioCarregamento < DateTime.Now)
            )
                return CorGrid.Branco;

            return string.Empty;
        }

        private void ValidarnformacaoChegadaVeiculoNfeDuplicada(List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> listaNotaFiscalParcial, Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial notaFiscalParcialAdicionar)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial notaFiscalParcialDuplicada = (from notaFiscalParcial in listaNotaFiscalParcial where notaFiscalParcial.Chave == notaFiscalParcialAdicionar.Chave select notaFiscalParcial).FirstOrDefault();

            if (notaFiscalParcialDuplicada != null)
                throw new ControllerException($"NF-e {notaFiscalParcialDuplicada.Chave} duplicada");
        }

        private bool ValidarInformacoesObrigatorias(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaGuarita.FluxoGestaoPatio);

            if ((sequenciaGestaoPatio != null) && sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesPesagem)
            {
                if (cargaGuarita.Carga?.Motoristas?.FirstOrDefault() == null || cargaGuarita.Carga?.Empresa == null || cargaGuarita.Carga?.Veiculo == null)
                    return false;

                cargaGuarita.NumeroNfProdutor = Request.GetIntParam("NumeroNfProdutor");

                if (sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesProdutor && (cargaGuarita.NumeroNfProdutor <= 0))
                    return false;
            }

            return true;
        }

        private bool ValidarInicioViagemPelaSaidaVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga Carga, DateTime dataInicioViagem)
        {
            // Valida
            bool permiteDataAnteriorCarregamento = Carga.TipoOperacao != null ? Carga.TipoOperacao.PermitirDataInicioViagemAnteriorDataCarregamento : false;
            if (!Carga.TipoOperacao?.PossibilitarInicioViagemViaGuarita ?? false)
                return false;
            if (!(Carga.TipoOperacao?.PermitirTransportadorConfirmarRejeitarEntrega ?? false))
                return false;
            if (Carga == null)
                return false;
            if (Carga.DataInicioViagem.HasValue)
                return false;
            if (dataInicioViagem < Carga.DataCarregamentoCarga && !permiteDataAnteriorCarregamento)
                return false;

            return true;
        }

        private string ObterNumeroPedidosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork UnitOfWork)
        {
            if (carga != null)
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXml = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(UnitOfWork);
                List<string> listaPedidos = repPedidoXml.BuscarNumeroPedidoEmbarcadorPorCarga(carga.Codigo);
                return string.Join(", ", listaPedidos);
            }
            else
                return "";
        }

        #endregion
    }
}
