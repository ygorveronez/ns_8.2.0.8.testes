using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/TravamentoChave", "GestaoPatio/FluxoPatio")]
    public class TravamentoChaveController : BaseController
    {
		#region Construtores

		public TravamentoChaveController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cargas Agrupadas", "CodigosAgrupadosCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Travamento", "DataTravamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Liberação", "DataLiberacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Doca", "Doca", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tempo Janela", "TempoJanela", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo", "ModeloVeiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação Janela", "ObservacaoFluxoPatio", 10, Models.Grid.Align.left, false);

                string carga = Request.GetStringParam("Carga");
                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");
                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
                SituacaoTravamentoChave? situacao = Request.GetNullableEnumParam<SituacaoTravamentoChave>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(unitOfWork);
                int totalRegistros = repositorioTravamentoChave.ContarConsulta(situacao, dataInicial, dataFinal, carga);
                List<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave> listaTravamentoChave = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = null;

                if (totalRegistros > 0)
                {
                    listaTravamentoChave = repositorioTravamentoChave.Consultar(situacao, dataInicial, dataFinal, carga, parametrosConsulta);
                    List<int> codigosCargas = (from o in listaTravamentoChave where o.Carga != null select o.Carga.Codigo).Distinct().ToList();
                    listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
                }
                else
                {
                    listaTravamentoChave = new List<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave>();
                    listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                }

                var listaTravamentoChaveRetornar = (
                    from travamentoChave in listaTravamentoChave
                    select ObterTravamentoChave(travamentoChave, listaCargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork)
                ).ToList();

                grid.AdicionaRows(listaTravamentoChaveRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTravamentoChave = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                EtapaFluxoGestaoPatio? etapaFluxoGestaoPatio = Request.GetNullableEnumParam<EtapaFluxoGestaoPatio>("Etapa");
                Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = null;

                if (codigoTravamentoChave > 0)
                    travamentoChave = repositorioTravamentoChave.BuscarPorCodigo(codigoTravamentoChave);
                else if (codigoFluxoGestaoPatio > 0)
                    travamentoChave = repositorioTravamentoChave.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (travamentoChave == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (travamentoChave.Carga != null)
                    return new JsonpResult(ObterTravamentoChavePorCarga(travamentoChave, etapaFluxoGestaoPatio, unitOfWork));

                return new JsonpResult(ObterTravamentoChavePorPreCarga(travamentoChave, etapaFluxoGestaoPatio, unitOfWork));
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.TravamentoChaveAnexo repositorioTravamentoChaveAnexo = new Repositorio.Embarcador.GestaoPatio.TravamentoChaveAnexo(unitOfWork);

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(travamentoChave.FluxoGestaoPatio);

                if (travamentoChave == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (travamentoChave.Situacao == SituacaoTravamentoChave.Travada)
                {
                    if (!travamentoChave.EtapaTravaChaveLiberada && !travamentoChave.EtapaLiberacaoChaveLiberada)
                        return new JsonpResult(false, true, "A Trava de chave ainda não foi autorizada.");

                    if (travamentoChave.FluxoGestaoPatio?.Filial != null)
                    {
                        if ((sequenciaGestaoPatio?.LiberaChaveBloquearLiberacaoEtapaAnterior ?? false) && travamentoChave.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != EtapaFluxoGestaoPatio.LiberacaoChave)
                            return new JsonpResult(false, true, "O fluxo de pátio não está na etapa de liberação da chave.");
                    }

                    travamentoChave.Situacao = SituacaoTravamentoChave.Liberada;

                    servicoFluxoGestaoPatio.LiberarProximaEtapa(travamentoChave.FluxoGestaoPatio, EtapaFluxoGestaoPatio.LiberacaoChave);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, travamentoChave, null, "Liberou a Chave", unitOfWork);
                }
                else
                {
                    if (!travamentoChave.EtapaTravaChaveLiberada)
                        return new JsonpResult(false, true, "A Liberação da chave ainda não foi autorizada.");

                    travamentoChave.Situacao = SituacaoTravamentoChave.Travada;

                    servicoFluxoGestaoPatio.LiberarProximaEtapa(travamentoChave.FluxoGestaoPatio, EtapaFluxoGestaoPatio.TravamentoChave);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, travamentoChave, null, "Travou a Chave", unitOfWork);
                }

                travamentoChave.PaletesChep = Request.GetIntParam("PaletesChep");
                travamentoChave.PaletesPBR = Request.GetIntParam("PaletesPBR");

                if (sequenciaGestaoPatio.LiberaChaveInformarNumeroDePaletes && (travamentoChave.Situacao == SituacaoTravamentoChave.Liberada))
                    if (travamentoChave.PaletesPBR == 0 && travamentoChave.PaletesChep == 0)
                        throw new ControllerException("Configuração exige informação de Paletes PBR e Paletes Chep.");

                List<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAnexo> travamentoChaveAnexos = repositorioTravamentoChaveAnexo.BuscarPorTravamentoChave(codigo);

                if ((sequenciaGestaoPatio?.LiberaChaveExigirAnexo ?? false) && (travamentoChave.Situacao == SituacaoTravamentoChave.Liberada) && travamentoChaveAnexos?.Count == 0)
                    throw new ControllerException("Configuração exige anexos para finalizar a etapa.");

                repositorioTravamentoChave.Atualizar(travamentoChave);

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
                return new JsonpResult(false, excecao.Message);
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
                Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorCodigo(codigo);
                EtapaFluxoGestaoPatio? etapaFluxoGestaoPatio = Request.GetNullableEnumParam<EtapaFluxoGestaoPatio>("Etapa");

                if (travamentoChave == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!etapaFluxoGestaoPatio.HasValue)
                    return new JsonpResult(false, true, "Não foi possível identificar a etapa do fluxo de pátio.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(travamentoChave.FluxoGestaoPatio, etapaFluxoGestaoPatio.Value, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        public async Task<IActionResult> GravarAssinaturaMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(unitOfWork);

                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                int codigoTravamentoChave = Request.GetIntParam("TravamentoChave");

                Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorCodigo(codigoTravamentoChave);

                if (travamentoChave == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                string imagemBase64 = Request.GetStringParam("Imagem");
                Stream imagem = new MemoryStream(Convert.FromBase64String(imagemBase64));

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.TravamentoChave servicoTravamentoChave = new Servicos.Embarcador.GestaoPatio.TravamentoChave(unitOfWork, Auditado);
                servicoTravamentoChave.ArmazenarAssinaturaMotorista(imagem, unitOfWork, out string guid);
                if (!servicoTravamentoChave.SalvarAssinaturaMotorista(travamentoChave, guid, DateTime.Now, out string mensagemErro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, mensagemErro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Não foi possível gravar a assinatura do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarAssinaturaMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPatio.TravamentoChave repositorioTravamentoChave = new Repositorio.Embarcador.GestaoPatio.TravamentoChave(unitOfWork);

                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                int codigoTravamentoChave = Request.GetIntParam("TravamentoChave");

                Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave = repositorioTravamentoChave.BuscarPorCodigo(codigoTravamentoChave);

                if (travamentoChave == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                if (travamentoChave.TravamentoChaveAssinaturaMotorista == null)
                    return new JsonpResult(false);

                return new JsonpResult(Base64ImagemAssinaturaMotorista(travamentoChave.TravamentoChaveAssinaturaMotorista, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Não foi possível carregar a assinatura do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxo, EtapaFluxoGestaoPatio? etapaFluxoGestaoPatio)
        {
            if ((fluxo == null) || (fluxo.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            if (!etapaFluxoGestaoPatio.HasValue)
                return false;

            if (fluxo.EtapaFluxoGestaoPatioAtual == etapaFluxoGestaoPatio.Value)
                return true;

            return false;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "CargaGuarita.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador";

            return propriedadeOrdenar;
        }

        private dynamic ObterTravamentoChave(Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            int codigoCargaFiltrarJanelaCarregamento = travamentoChave.Carga?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == codigoCargaFiltrarJanelaCarregamento select o).FirstOrDefault();

            return new
            {
                travamentoChave.Codigo,
                Carga = servicoCarga.ObterNumeroCarga(travamentoChave.Carga, configuracaoEmbarcador),
                CodigosAgrupadosCarga = travamentoChave.Carga == null ? "" : string.Join(", ", travamentoChave.Carga.CodigosAgrupados),
                DataTravamento = travamentoChave.DataTravamento.ToString("dd/MM/yyyy"),
                Situacao = travamentoChave.DescricaoSituacao,
                DataLiberacao = travamentoChave.DataLiberacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                Doca = !string.IsNullOrWhiteSpace(travamentoChave.Carga?.NumeroDocaEncosta) ? travamentoChave.Carga?.NumeroDocaEncosta : travamentoChave.Carga?.NumeroDoca ?? string.Empty,
                TempoJanela = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Veiculo = travamentoChave.Carga?.RetornarPlacas,
                Transportador = travamentoChave.Carga?.Empresa?.Descricao ?? string.Empty,
                ModeloVeiculo = travamentoChave.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                TipoOperacao = travamentoChave.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? string.Empty
            };
        }

        private dynamic ObterTravamentoChavePorCarga(Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave, EtapaFluxoGestaoPatio? etapaFluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(travamentoChave.Carga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(travamentoChave.FluxoGestaoPatio);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();

            return new
            {
                travamentoChave.Codigo,
                Travado = travamentoChave.Situacao == SituacaoTravamentoChave.Travada,
                Carga = servicoCarga.ObterNumeroCarga(travamentoChave.Carga, unitOfWork),
                CodigoCarga = travamentoChave.Carga.Codigo,
                PreCarga = travamentoChave.PreCarga?.NumeroPreCarga ?? "",
                Situacao = travamentoChave.DescricaoSituacao,
                Percurso = "",
                Data = travamentoChave.DataTravamento.ToString("dd/MM/yyyy HH:mm"),
                Transportador = travamentoChave.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = travamentoChave.Carga.RetornarPlacas,
                Motorista = travamentoChave.Carga.NomeMotoristas,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                DataLiberacaoChave = travamentoChave.FluxoGestaoPatio?.DataLiberacaoChave?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataLiberacaoChavePrevista = travamentoChave.FluxoGestaoPatio?.DataLiberacaoChavePrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                PermitirEditarEtapa = IsPermitirEditarEtapa(travamentoChave.FluxoGestaoPatio, etapaFluxoGestaoPatio),
                travamentoChave.PaletesChep,
                travamentoChave.PaletesPBR,
                LiberaChaveInformarNumeroDePaletes = sequenciaGestaoPatio?.LiberaChaveInformarNumeroDePaletes ?? false,
                Recebedores = travamentoChave.Carga?.DadosSumarizados?.Recebedores ?? string.Empty,
                NotasFiscais = travamentoChave.Carga != null ? string.Join(", ", repositorioPedidoXMLNotaFiscal.BuscarNumeroNotasFiscaisPorCarga(travamentoChave.Carga.Codigo)) : string.Empty,
                SolicitarAssinaturaMotorista = sequenciaGestaoPatio?.LiberaChaveSolicitarAssinaturaMotorista ?? false,
                TravaChavePermiteGerarAtendimento = configuracaoGestaoPatio?.TravaChavePermiteGerarAtendimento ?? false
            };
        }

        private dynamic ObterTravamentoChavePorPreCarga(Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave travamentoChave, EtapaFluxoGestaoPatio? etapaFluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(travamentoChave.PreCarga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(travamentoChave.FluxoGestaoPatio);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();

            return new
            {
                travamentoChave.Codigo,
                Travado = travamentoChave.Situacao == SituacaoTravamentoChave.Travada,
                Carga = "",
                CodigoCarga = travamentoChave.Carga.Codigo,
                PreCarga = travamentoChave.PreCarga.NumeroPreCarga ?? "",
                Situacao = travamentoChave.DescricaoSituacao,
                Percurso = "",
                Data = travamentoChave.DataTravamento.ToString("dd/MM/yyyy HH:mm"),
                Transportador = travamentoChave.PreCarga.Empresa?.Descricao ?? "",
                Veiculo = travamentoChave.PreCarga.RetornarPlacas,
                Motorista = (from motorista in travamentoChave.PreCarga.Motoristas select motorista.DescricaoTelefone).ToList(),
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                DataLiberacaoChave = travamentoChave.FluxoGestaoPatio?.DataLiberacaoChave?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataLiberacaoChavePrevista = travamentoChave.FluxoGestaoPatio?.DataLiberacaoChavePrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                PermitirEditarEtapa = IsPermitirEditarEtapa(travamentoChave.FluxoGestaoPatio, etapaFluxoGestaoPatio),
                travamentoChave.PaletesChep,
                travamentoChave.PaletesPBR,
                LiberaChaveInformarNumeroDePaletes = sequenciaGestaoPatio?.LiberaChaveInformarNumeroDePaletes ?? false,
                SolicitarAssinaturaMotorista = sequenciaGestaoPatio?.LiberaChaveSolicitarAssinaturaMotorista ?? false,
                TravaChavePermiteGerarAtendimento = configuracaoGestaoPatio?.TravaChavePermiteGerarAtendimento ?? false
            };
        }

        private string Base64ImagemAssinaturaMotorista(Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAssinaturaMotorista assinaturaMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "GestaoPatio", "TravamentoChave", "AssinaturaMotorista" });
            return Base64Imagem(caminho, assinaturaMotorista.NomeArquivo, assinaturaMotorista.GuidArquivo);
        }

        private string Base64Imagem(string caminho, string nomeArquivo, string guidArquivo)
        {
            string extensao = Path.GetExtension(nomeArquivo);
            string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + "-miniatura" + extensao);

            if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
            {
                byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                return base64ImageRepresentation;
            }
            else
            {
                return "";
            }
        }

        #endregion
    }
}
