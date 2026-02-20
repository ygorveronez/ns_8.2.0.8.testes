using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Text;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { "ObterDetalhesCarga", "ObterMensagemPadraoInformarDadosTransporte", "ConsultarCargas", "ObterDadosRota", "ObterHorariosDisponiveis", "BuscarProximaDataDisponivel" }, "Logistica/JanelaCarregamentoTransportador", "Logistica/JanelaCarregamento")]
    public class JanelaCarregamentoTransportadorController : BaseController
    {
        #region Construtores

        public JanelaCarregamentoTransportadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AceitarTermoDeAceite()
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoJanelaCarregamentoTransportador = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                string termoAceite = Request.GetStringParam("TermoAceite");

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoJanelaTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, ConfiguracaoEmbarcador);

                if (codigoJanelaCarregamentoTransportador > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioJanelaTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador janelaTransportador = null;

                    janelaTransportador = repositorioJanelaTransportador.BuscarPorCodigo(codigoJanelaCarregamentoTransportador);

                    if (janelaTransportador == null)
                        return new JsonpResult(true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.RegistroNaoFoiEncontrado);

                    servicoJanelaTransportador.AceitarTermoDeAceite(janelaTransportador, Usuario, termoAceite, Auditado);

                    dynamic retorno = ObterDadosCarga(janelaTransportador, unitOfWork);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(retorno, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.AceiteRealizadoComSucesso);
                }
                else if (codigoCarga > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                    carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                    if (carga == null)
                        return new JsonpResult(true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.RegistroNaoFoiEncontrado);

                    servicoJanelaTransportador.AceitarTermoDeAceitePorCarga(carga, Usuario, termoAceite, Auditado);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.AceiteRealizadoComSucesso);
                }

                return new JsonpResult(false, false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoRealizarAceiteDoTermo);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoAceitarTermoDeAceite);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarHorariosDescarregamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                dynamic dadosDescarregamentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DatasDescarregamento"));

                foreach (var dadosDescarregamento in dadosDescarregamentos)
                {
                    int codigoCarga = ((string)dadosDescarregamento.Carga).ToInt();
                    int codigoCentroDescarregamemto = ((string)dadosDescarregamento.CentroDescarregamento).ToInt();
                    DateTime dataDescarregamemto = ((string)dadosDescarregamento.DataDescarregamento).ToNullableDateTime() ?? throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataDeDescarregamentoDeveSerinformada);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarDescarregamentoPorCargaECentro(codigoCarga, codigoCentroDescarregamemto) ?? throw new ControllerException("Não foi possível encontrar a janela de descarregamento.");

                    cargaJanelaCarregamento.Initialize();

                    AlocarHorarioCarregamento(cargaJanelaCarregamento, dataDescarregamemto, configuracaoEmbarcador, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AlteradoHorarioDeDescarregamentoPara, cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"))), unitOfWork);
                }

                unitOfWork.CommitChanges();

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(Request.GetIntParam("Codigo"));

                return new JsonpResult(ObterDadosCarga(cargaJanelaCarregamentoTransportador, unitOfWork));
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
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoAlterarHorarioDeDescarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IndisponibilizarVeiculo()
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ParaDisponibilizarPlacasAcesseMultiCTe);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repositorioVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento veiculoDisponivelCarregamento = repositorioVeiculoDisponivelCarregamento.BuscarPorCodigo(codigo);

                if (veiculoDisponivelCarregamento == null)
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, ConfiguracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro(unitOfWork, ConfiguracaoEmbarcador);

                veiculoDisponivelCarregamento.Disponivel = false;
                veiculoDisponivelCarregamento.DataIndisponibilizacaoVeiculo = DateTime.Now;
                veiculoDisponivelCarregamento.UsuarioIndisponibilizou = this.Usuario;

                repositorioVeiculoDisponivelCarregamento.Atualizar(veiculoDisponivelCarregamento);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    servicoCargaJanelaCarregamentoTransportadorTerceiro.ValidarCargasInteressadas(Usuario?.ClienteTerceiro, veiculoDisponivelCarregamento.Veiculo.ModeloVeicularCarga);
                else
                    servicoCargaJanelaCarregamentoTransportador.ValidarCargasInteressadas(Usuario.Empresa, veiculoDisponivelCarregamento.Veiculo.ModeloVeicularCarga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoDisponivelCarregamento, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.IndisponibilizouVeIculo, unitOfWork);

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
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoDisponibilizarVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarVeiculoDisponivel()
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ParaDisponibilizarPlacasAcesseMultiCTe);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                string placaDisponivel = Request.Params("PlacaDisponivel").Replace(" - ", "");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlacaVarrendoFiliais(Usuario.Empresa.Codigo, placaDisponivel);

                if (veiculo == null)
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OVeiculoInformadoNaoFoiLocalizado);

                if (veiculo.VeiculoBloqueado)
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.VeiculoBloqueado);

                new Servicos.Embarcador.Integracao.SemParar.ConsultaCadastroVeiculo(unitOfWork, TipoServicoMultisoftware).GerarIntegracaoCadastroVeiculo(veiculo);

                if ((veiculo.TipoVeiculo == "1") && (veiculo.VeiculosTracao?.Count > 0))
                {
                    Dominio.Entidades.Veiculo tracao = (from o in veiculo.VeiculosTracao where o.Ativo select o).FirstOrDefault();

                    if (tracao != null)
                        veiculo = tracao;
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (
                    (!configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportador && !configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao) ||
                    (configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao && !Request.GetBoolParam("SelecaoQualquerVeiculoConfirmada"))
                )
                {
                    Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes servicoJanelaCarregamentoTransportadorValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes(unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                    servicoJanelaCarregamentoTransportadorValidacoes.ValidarPlaca(veiculo, Usuario.Empresa);

                    if (veiculo.VeiculosVinculados != null)
                    {
                        foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                            servicoJanelaCarregamentoTransportadorValidacoes.ValidarPlaca(reboque, Usuario.Empresa);
                    }
                }

                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                if (!veiculo.PossuiTagValePedagio && (configuracaoJanelaCarregamento.BloquearVeiculoSemTagValePedagioAtiva ?? false) && !veiculo.NaoComprarValePedagio)
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossuiTagValePedagioAtiva);

                Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repositorioVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento veiculoDisponivelCarregamento = new Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento
                {
                    DataDisponibilizacaoVeiculo = DateTime.Now,
                    DataIndisponibilizacaoVeiculo = DateTime.Now,
                    Disponivel = true,
                    UsuarioDisponibilizou = this.Usuario,
                    Veiculo = veiculo
                };

                if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                    veiculoDisponivelCarregamento.Terceiro = Usuario.ClienteTerceiro;
                else
                    veiculoDisponivelCarregamento.Empresa = Usuario.Empresa;

                repositorioVeiculoDisponivelCarregamento.Inserir(veiculoDisponivelCarregamento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(excecao.ErrorCode == CodigoExcecao.SelecaoQualquerVeiculoNaoConfirmada, true, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(excecao.ErrorCode == CodigoExcecao.SelecaoQualquerVeiculoNaoConfirmada, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoDisponibilizarVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork, configuracaoEmbarcador, null);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: true);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: true);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga select o).FirstOrDefault();

                if (cargaJanelaCarregamentoTransportadorReferencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoVinculadaAoTransportador);

                if (cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgAceite)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.situacaoDaCargaNaoPermiteConfirmacao);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga;

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AAtualSituacaoDaCargaNaoPermiteConfirmacao, carga.DescricaoSituacaoCarga)));

                servicoCargaJanelaCarregamentoTransportador.InformarAceiteCargasTransportador(cargaJanelaCarregamentoTransportadorReferencia, cargasJanelaCarregamentoTransportador, Usuario.Empresa, Usuario, Auditado, configuracaoEmbarcador, TipoServicoMultisoftware, _conexao.AdminStringConexao, Cliente, unitOfWork);

                unitOfWork.CommitChanges();

                cargaJanelaCarregamentoTransportadorReferencia = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(cargaJanelaCarregamentoTransportadorReferencia.Codigo);

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargas(cargasJanelaCarregamentoTransportador, unitOfWork)
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoConfirmarCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaVeiculosDisponiveis()
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                try
                {

                    Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                    grid.header = new List<Models.Grid.Head>();
                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho("Placa", "Placa", 60, Models.Grid.Align.left, false);

                    string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                    if (propOrdenar == "Placa")
                    {
                        propOrdenar += "Veiculo.Placa";
                    }

                    Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento> veiculosDisponibilizados = repVeiculoDisponivelCarregamento.Consultar(Usuario.Empresa.Codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                    grid.setarQuantidadeTotal(repVeiculoDisponivelCarregamento.ContarConsulta(Usuario.Empresa.Codigo));


                    var lista = (from p in veiculosDisponibilizados
                                 select new
                                 {
                                     p.Codigo,
                                     Placa = BuscarReboquesComModeloVeicular(p.Veiculo)
                                 }).ToList();
                    grid.AdicionaRows(lista);

                    return new JsonpResult(grid);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoConsultar);
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            else
            {
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ParaConsultarAsPlacasDisponiveisAcesseMultiCTe);
            }
        }

        public async Task<IActionResult> ObterDetalhesCarga()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(codigo);
                    cargaJanelaCarregamento = cargaJanelaCarregamentoTransportador?.CargaJanelaCarregamento;
                }
                else
                {
                    cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);
                    cargaJanelaCarregamentoTransportador = await repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaJanelaCarregamentoAsync(codigo);
                }

                if (cargaJanelaCarregamentoTransportador == null && cargaJanelaCarregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoEncontrada);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                if (!ValidarVeiculoDisponivel(cargaJanelaCarregamentoTransportador, configuracaoEmbarcador, unidadeDeTrabalho) && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.VoceNaoTemMaisVeiculosDisponiveisCargaFoiRepassadaParaOutroTransportador);

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeDeTrabalho);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaVeiculoContainer repCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaVeiculoContainerAnexo repCargaVeiculoContainerAnexo = new Repositorio.Embarcador.Cargas.CargaVeiculoContainerAnexo(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.AgendamentoColetaAnexo repositorioAgendamentoColetaAnexo = new Repositorio.Embarcador.Logistica.AgendamentoColetaAnexo(unidadeDeTrabalho);
                List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> cargaVeiculosContainer = repCargaVeiculoContainer.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo> cargaVeiculosContainerAnexo = repCargaVeiculoContainerAnexo.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo> agendamentoColetaAnexos = repositorioAgendamentoColetaAnexo.BuscarPorPedidosECarga(cargaJanelaCarregamento.Carga?.Pedidos?.Select(o => o.Pedido.Codigo).ToList(), cargaJanelaCarregamento.Carga?.Codigo ?? 0);
                List<Dominio.Entidades.Usuario> ajudantes = cargaJanelaCarregamento.Carga.Ajudantes?.ToList() ?? new List<Dominio.Entidades.Usuario>();
                Dominio.Entidades.Usuario motorista = cargaJanelaCarregamento.Carga.Motoristas?.FirstOrDefault();
                ICollection<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaJanelaCarregamento.Carga.Pedidos;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();
                DateTime? dataPrevisaoEntrega = (from o in cargaPedidos where o.Pedido.PrevisaoEntrega.HasValue select o.Pedido.PrevisaoEntrega).FirstOrDefault();
                DateTime? dataBaseCalculoLimiteCarregamento = (from o in cargaPedidos where o.Pedido.PrevisaoEntrega.HasValue select o.Pedido.PrevisaoEntrega).OrderBy(o => o).FirstOrDefault();
                bool possuiGenset = cargaPedidos?.Any(o => o.Pedido.PossuiGenset == true) ?? false;
                bool exigirConfirmacaoTracao = cargaJanelaCarregamento.Carga.TipoOperacao?.ExigePlacaTracao ?? false;
                string destino = cargaJanelaCarregamento.Carga.DadosSumarizados?.Destinos ?? string.Empty;
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapaConfiguracao configuracoesGestaoPatioPorCarga = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterConfiguracoesPorCarga(cargaJanelaCarregamento.Carga);
                DateTime? previsaoTerminoCarregamento = cargaPedidos.Where(o => o.Pedido.DataPrevisaoTerminoCarregamento.HasValue).OrderBy(o => o.Pedido.DataPrevisaoTerminoCarregamento).Select(o => o.Pedido.DataPrevisaoTerminoCarregamento).FirstOrDefault();
                string numeroPedido = string.Join(", ", cargaPedidos.Select(o => o.Pedido.NumeroPedidoEmbarcador));

                if (!string.IsNullOrWhiteSpace(cargaJanelaCarregamento.Carga.ObservacaoLocalEntrega))
                    destino += " (" + cargaJanelaCarregamento.Carga.ObservacaoLocalEntrega + ")";

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte dadosTransporte = null;
                Dominio.Entidades.Veiculo veiculo = null;
                Dominio.Entidades.Veiculo reboque = null;
                Dominio.Entidades.Veiculo segundoReboque = null;

                if (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao)
                {
                    Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte repositorioDadosTransporte = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte(unidadeDeTrabalho);
                    dadosTransporte = repositorioDadosTransporte.BuscarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);
                }

                if (dadosTransporte == null)
                {
                    veiculo = cargaJanelaCarregamento.Carga.Veiculo;
                    reboque = cargaJanelaCarregamento.Carga.VeiculosVinculados?.ElementAtOrDefault(0);
                    segundoReboque = cargaJanelaCarregamento.Carga.VeiculosVinculados?.ElementAtOrDefault(1);
                }
                else
                {
                    veiculo = dadosTransporte.Veiculo;
                    reboque = dadosTransporte.VeiculosVinculados?.ElementAtOrDefault(0);
                    segundoReboque = dadosTransporte.VeiculosVinculados?.ElementAtOrDefault(1);

                    if (configuracaoJanelaCarregamento.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga)
                    {
                        motorista = dadosTransporte.Motorista;
                    }
                }

                dynamic dynChecklist = null;
                if (cargaJanelaCarregamento.CentroCarregamento?.ExigirChecklistAoConfirmarDadosTransporteMultiTransportador ?? false)
                    dynChecklist = servicoCargaJanelaCarregamento.ObterChecklist(cargaJanelaCarregamento.Codigo, unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerVeiculo = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == veiculo?.Codigo select cargaVeiculoContainer).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == reboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerSegundoReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == segundoReboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = cargaJanelaCarregamento.CentroCarregamento;
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unidadeDeTrabalho).BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = cargaPedido.Pedido.Destinatario?.ClienteDescargas.FirstOrDefault() ?? new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga();

                List<int> pedidosComNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeDeTrabalho).BuscarListaPedidosComNotaPorPedidos((from o in cargaPedidos select o.Pedido.Codigo).ToList());
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> ceps = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unidadeDeTrabalho).BuscarPorCargaPedidos((from o in cargaPedidos select o.Codigo).ToList());

                DateTime? dataBaseCalculoLimiteCarregamentoSemNF = ceps.Where(o => o.CargaEntrega.DataAgendamento.HasValue
                                                                                                && (!o.CargaPedido.Pedido?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.AgendamentoExigeNotaFiscal ?? false)
                                                                                                && !pedidosComNota.Contains(o.CargaPedido.Pedido.Codigo)).OrderBy(o => o.CargaEntrega.DataAgendamento).Select(o => o.CargaEntrega?.DataAgendamento).FirstOrDefault();

                if (!dataBaseCalculoLimiteCarregamentoSemNF.HasValue)
                {
                    dataBaseCalculoLimiteCarregamentoSemNF = ceps.Where(o => (o.CargaPedido.Pedido?.DataAgendamento.HasValue ?? false)
                                                                                && !(o.CargaPedido.Pedido?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.AgendamentoExigeNotaFiscal ?? false)
                                                                                && !pedidosComNota.Contains(o.CargaPedido.Pedido.Codigo))
                                                                        .OrderBy(o => o.CargaPedido.Pedido.DataAgendamento).Select(o => o.CargaPedido.Pedido.DataAgendamento).FirstOrDefault();
                }

                if (dataBaseCalculoLimiteCarregamentoSemNF.HasValue)
                    dataBaseCalculoLimiteCarregamento = dataBaseCalculoLimiteCarregamentoSemNF;

                var retorno = new
                {
                    Codigo = cargaJanelaCarregamento.Carga.Codigo,
                    CodigoCargaJanelaCarregamento = cargaJanelaCarregamento.Codigo,
                    CodigoJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador.Codigo,
                    Numero = cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador,
                    NumeroPedido = numeroPedido,
                    cargaJanelaCarregamento.Carga.CargaDeComplemento,
                    PrevisaoEntrega = dataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"),
                    DataPrevisaoTerminoCarregamento = previsaoTerminoCarregamento.HasValue && (centroCarregamento?.CamposVisiveisTransportador ?? "").Contains("32") ? previsaoTerminoCarregamento.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    DataLimiteCarregamento = (cargaJanelaCarregamento?.CentroCarregamento?.ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido ?? false) ? dataBaseCalculoLimiteCarregamento?.AddMinutes(-CalcularTempoLimiteCarregamentoEmMinutos(cargaJanelaCarregamento, unidadeDeTrabalho)).ToString("dd/MM/yyyy HH:mm") ?? "" : "",
                    TempoCarregamento = cargaJanelaCarregamento.TempoCarregamento,
                    NumeroEntregas = cargaJanelaCarregamento.Carga.DadosSumarizados?.NumeroEntregas ?? 0,
                    SituacaoCarga = cargaJanelaCarregamento.Carga.SituacaoCarga,
                    ExigirConfirmacaoTracao = exigirConfirmacaoTracao,
                    NumeroReboques = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.NumeroReboques ?? 0,
                    ExigirDefinicaoReboquePedido = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false,
                    PossuiGenset = possuiGenset,
                    Situacao = cargaJanelaCarregamentoTransportador.Situacao,
                    NumeroDoca = (centroCarregamento?.CamposVisiveisTransportador ?? "27").Contains("27") ? cargaJanelaCarregamento.Carga?.NumeroDoca ?? "" : "",
                    CentroCarregamento = new
                    {
                        PermitirAlterarModeloVeicularCargaJanelaCarregamentoTransportador = centroCarregamento?.PermitirAlterarModeloVeicularCargaJanelaCarregamentoTransportador ?? false,
                        PermitirAlterarTransportador = centroCarregamento?.PermitirMatrizSelecionarFilial ?? false,
                        PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador = centroCarregamento?.PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador ?? false,
                        ExigirMotivoRejeicaoCarga = centroCarregamento?.ExigirTransportadorInformarMotivoAoRejeitarCarga ?? false
                    },
                    TipoCarga = new
                    {
                        Codigo = cargaJanelaCarregamento.Carga.TipoDeCarga?.Codigo ?? 0,
                        Descricao = cargaJanelaCarregamento.Carga.TipoDeCarga?.Descricao ?? string.Empty
                    },
                    ModeloVeiculo = new
                    {
                        Codigo = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.Codigo ?? 0,
                        Descricao = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.Descricao ?? string.Empty
                    },
                    Motorista = new
                    {
                        Codigo = motorista?.Codigo ?? 0,
                        Descricao = motorista?.Nome ?? string.Empty
                    },
                    Veiculo = new
                    {
                        Codigo = veiculo?.Codigo ?? 0,
                        Descricao = exigirConfirmacaoTracao ? veiculo?.Descricao ?? "" : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(veiculo),
                        Tipo = veiculo != null ? veiculo.Tipo : "P",
                    },
                    Ajudante = ajudantes.Select(ajudante => new
                    {
                        Codigo = ajudante.Codigo,
                        Descricao = ajudante.Nome
                    }).ToList(),
                    Transportador = new
                    {
                        Codigo = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? cargaJanelaCarregamentoTransportador?.Terceiro.CPF_CNPJ : cargaJanelaCarregamentoTransportador?.Transportador.Codigo,
                        Descricao = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? cargaJanelaCarregamentoTransportador?.Terceiro.Nome : cargaJanelaCarregamentoTransportador?.Transportador.Descricao,
                    },
                    PermiteInformarAjudantes = cargaJanelaCarregamento.Carga?.TipoOperacao?.ConfiguracaoCarga?.PermitirInformarAjudantesNaCarga ?? false,
                    Reboque = new { Codigo = reboque?.Codigo ?? 0, Descricao = reboque?.Descricao ?? "" },
                    SegundoReboque = new { Codigo = segundoReboque?.Codigo ?? 0, Descricao = segundoReboque?.Descricao ?? "" },
                    CIOT = veiculo != null ? veiculo.CIOT : "",
                    ValorValePedagio = veiculo != null ? veiculo.ValorValePedagio > 0 ? veiculo.ValorValePedagio.ToString("n2") : "" : "",
                    NumeroCompraValePedagio = veiculo != null ? veiculo.NumeroCompraValePedagio : "",
                    ResponsavelValePedagio = veiculo != null ? veiculo.ResponsavelValePedagio != null ? new { Codigo = veiculo.ResponsavelValePedagio.CPF_CNPJ, Descricao = veiculo.ResponsavelValePedagio.Nome } : new { Codigo = (double)0, Descricao = "" } : new { Codigo = (double)0, Descricao = "" },
                    FornecedorValePedagio = veiculo != null ? veiculo.FornecedorValePedagio != null ? new { Codigo = veiculo.FornecedorValePedagio.CPF_CNPJ, Descricao = veiculo.FornecedorValePedagio.Nome } : new { Codigo = (double)0, Descricao = "" } : new { Codigo = (double)0, Descricao = "" },
                    Remetente = cargaJanelaCarregamento.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                    CDDestino = cargaJanelaCarregamento.Carga.DadosSumarizados?.CentrosDeDistribuicao ?? string.Empty,
                    Origem = cargaJanelaCarregamento.Carga.DadosSumarizados?.Origens,
                    Destino = destino,
                    Destinatario = cargaJanelaCarregamento.Carga.DadosSumarizados?.DestinatariosReais ?? string.Empty,
                    Componentes = (
                        from obj in cargaJanelaCarregamento.Carga.Componentes
                        where obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS
                        select new
                        {
                            Descricao = obj.ComponenteFrete?.Descricao ?? obj.DescricaoComponente,
                            Valor = obj.ValorComponente.ToString("n2")
                        }
                    ).ToList(),
                    Peso = cargaJanelaCarregamento.Carga.DadosSumarizados?.PesoTotal.ToString("n2") ?? "",
                    ValorICMS = cargaJanelaCarregamento.Carga.ValorICMS.ToString("n2"),
                    ValorFrete = cargaJanelaCarregamento.Carga.ValorFrete.ToString("n2"),
                    ValorTotalFrete = cargaJanelaCarregamento.Carga.ValorFreteAPagar.ToString("n2"),
                    cargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete,
                    Observacao = cargaJanelaCarregamento.ObservacaoTransportador ?? "",
                    ObservacaoCarregamento = cargaJanelaCarregamento.Carga?.Carregamento?.Observacao ?? "",
                    ObservacaoInformadaPeloTransportador = cargaJanelaCarregamentoTransportador.Observacao,
                    CodigoContainerVeiculo = containerVeiculo?.Codigo.ToString() ?? Guid.NewGuid().ToString(),
                    DataRetiradaCtrnVeiculo = containerVeiculo?.DataRetiradaCtrn?.ToString("dd/MM/yyyy"),
                    GensetVeiculo = containerVeiculo?.Genset,
                    MaxGrossVeiculo = containerVeiculo?.MaxGross > 0 ? containerVeiculo.MaxGross.ToString("n0") : string.Empty,
                    NumeroContainerVeiculo = containerVeiculo?.NumeroContainer,
                    TaraContainerVeiculo = containerVeiculo?.TaraContainer > 0 ? containerVeiculo.TaraContainer.ToString("n0") : string.Empty,
                    AnexosVeiculo = (
                        from anexo in cargaVeiculosContainerAnexo
                        where anexo.EntidadeAnexo.Codigo == containerVeiculo?.Codigo
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo
                        }
                    ).ToList(),
                    CodigoContainerReboque = containerReboque?.Codigo.ToString() ?? Guid.NewGuid().ToString(),
                    DataRetiradaCtrnReboque = containerReboque?.DataRetiradaCtrn?.ToString("dd/MM/yyyy"),
                    GensetReboque = containerReboque?.Genset,
                    MaxGrossReboque = containerReboque?.MaxGross > 0 ? containerReboque.MaxGross.ToString("n0") : string.Empty,
                    NumeroContainerReboque = containerReboque?.NumeroContainer,
                    TaraContainerReboque = containerReboque?.TaraContainer > 0 ? containerReboque.TaraContainer.ToString("n0") : string.Empty,
                    AnexosReboque = (
                        from anexo in cargaVeiculosContainerAnexo
                        where anexo.EntidadeAnexo.Codigo == containerReboque?.Codigo
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo
                        }
                    ).ToList(),
                    CodigoContainerSegundoReboque = containerSegundoReboque?.Codigo.ToString() ?? Guid.NewGuid().ToString(),
                    DataRetiradaCtrnSegundoReboque = containerSegundoReboque?.DataRetiradaCtrn?.ToString("dd/MM/yyyy"),
                    GensetSegundoReboque = containerSegundoReboque?.Genset,
                    MaxGrossSegundoReboque = containerSegundoReboque?.MaxGross > 0 ? containerSegundoReboque.MaxGross.ToString("n0") : string.Empty,
                    NumeroContainerSegundoReboque = containerSegundoReboque?.NumeroContainer,
                    TaraContainerSegundoReboque = containerSegundoReboque?.TaraContainer > 0 ? containerSegundoReboque.TaraContainer.ToString("n0") : string.Empty,
                    AnexosSegundoReboque = (
                        from anexo in cargaVeiculosContainerAnexo
                        where anexo.EntidadeAnexo.Codigo == containerSegundoReboque?.Codigo
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo
                        }
                    ).ToList(),
                    ExigirDataRetiradaCtrnJanelaCarregamentoTransportador = cargaJanelaCarregamento.Carga.TipoOperacao?.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador ?? false,
                    ExigirMaxGrossJanelaCarregamentoTransportador = cargaJanelaCarregamento.Carga.TipoOperacao?.ExigirMaxGrossJanelaCarregamentoTransportador ?? false,
                    ExigirNumeroContainerJanelaCarregamentoTransportador = cargaJanelaCarregamento.Carga.TipoOperacao?.ExigirNumeroContainerJanelaCarregamentoTransportador ?? false,
                    ExigirTaraContainerJanelaCarregamentoTransportador = cargaJanelaCarregamento.Carga.TipoOperacao?.ExigirTaraContainerJanelaCarregamentoTransportador ?? false,
                    PermitirInformarAnexoContainerCarga = configuracaoEmbarcador.PermitirInformarAnexoContainerCarga,
                    PermitirEdicaoDadosTransporte = !configuracoesGestaoPatioPorCarga.BloquearEdicaoDadosTransporteJanelaTransportador,
                    PermitirEdicaoVeiculos = !configuracoesGestaoPatioPorCarga.BloquearEdicaoVeiculosCarga,
                    CargaLacres = (
                        from lacre in cargaJanelaCarregamento.Carga.Lacres
                        select new
                        {
                            lacre.Codigo,
                            lacre.Numero
                        }
                    ).ToList(),
                    ListaAnexosAgendamento = (
                        from anexo in agendamentoColetaAnexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo
                        }
                    ).ToList(),
                    PermitirRejeitarCargaJanelaCarregamentoTransportador = cargaJanelaCarregamento.Carga.TipoOperacao?.ConfiguracaoJanelaCarregamento?.PermitirRejeitarCargaJanelaCarregamentoTransportador ?? false,
                    PossuiInformacoesIMO = this.Usuario.Empresa != null ? (!string.IsNullOrEmpty(this.Usuario.Empresa.IMO) && (this.Usuario.Empresa.DataValidadeIMO.HasValue && this.Usuario.Empresa.DataValidadeIMO.Value > DateTime.Now)) : false,
                    AlertarTransportadorNaoIMOCargasPerigosas = cargaJanelaCarregamento.Carga.TipoOperacao?.ConfiguracaoTransportador?.AlertarTransportadorNaoIMOCargasPerigosas ?? false,
                    CargaPerigosa = cargaJanelaCarregamento.Carga != null ? (cargaJanelaCarregamento.Carga.CargaPerigosaIntegracaoLeilao ? "Sim" : "Não") : string.Empty,
                    Agendado = clienteDescarga?.ExigeAgendamento ?? false ? "Sim" : "Não",
                    DataAgendamento = cargaPedido.Pedido.DataAgendamento.HasValue ? cargaPedido.Pedido.DataAgendamento.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    ExigirChecklist = centroCarregamento?.ExigirChecklistAoConfirmarDadosTransporteMultiTransportador ?? false,
                    Checklist = dynChecklist,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoObterOsDetalhesDaCarga);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterMensagemPadraoInformarDadosTransporte()
        {
            try
            {
                return new JsonpResult(new
                {
                    MensagemPadraoInformarDadosTransporte = this.ConfiguracaoEmbarcador?.MensagemPadraoInformarDadosTransporteJanelaCarregamentoTransportador ?? ""
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoObterMensagemPadraoAoInformarOsDadosDeTransporte);
            }
        }

        public async Task<IActionResult> ConsultarCargas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoTransportador filtrosPesquisa = ObterFiltrosPesquisa(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeDeTrabalho).BuscarPrimeiroRegistro();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                };

                var cargas = repositorioCargaJanelaCarregamento.ConsultarPorTransportador(filtrosPesquisa, parametrosConsulta, configuracaoGeralCarga);
                int countCargas = repositorioCargaJanelaCarregamento.ContarConsultaPorTransportador(filtrosPesquisa, configuracaoGeralCarga);

                AtualizarCargarNaoDestinadasAoTransportador(cargas, configuracaoEmbarcador, unidadeDeTrabalho);

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargas(cargas, unidadeDeTrabalho),
                    Total = countCargas
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoConsultarAscargas);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("carga");
                Repositorio.Embarcador.Cargas.CargaRotaFrete repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repositorioCargaRotaFrete.BuscarPorCarga(codigoCarga);

                if (cargaRotaFrete == null)
                    return new JsonpResult(null);

                var pontos = new
                {
                    cargaRotaFrete.PolilinhaRota,
                    PontosDaRota = Servicos.Embarcador.Carga.RotaFrete.ObterPontosPassagemCargaRotaFreteSerializada(cargaRotaFrete, unitOfWork) ?? ""
                };

                return new JsonpResult(pontos);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoObterOsDetalhesParaMontagemDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterHorariosDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "HoraInicio", visivel: false);
                grid.AdicionarCabecalho(propriedade: "HoraTermino", visivel: false);
                grid.AdicionarCabecalho(descricao: "Horário", propriedade: "Horario", tamanho: 30, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
                DateTime? dia = Request.GetNullableDateTimeParam("DataDisponibilidade");

                if (codigoCarga <= 0)
                    return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ACargaNaoFoiEncontrada);

                if (!dia.HasValue)
                    return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ADataNaoFoiInformada);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = null;

                if (codigoCentroDescarregamento > 0)
                {
                    cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCargaECentroDescarregamento(codigoCarga, codigoCentroDescarregamento);

                    if (cargaJanelaCarregamento == null)
                        return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CentroDeDescarregamentoNaoEncontrado);
                }
                else
                {
                    cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                    if (cargaJanelaCarregamento?.CentroCarregamento == null)
                        return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CentroDeCarregamentoNaoEncontrado);
                }

                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento
                {
                    CodigoCarga = cargaJanelaCarregamento.Carga.Codigo,
                    CodigoTipoOperacao = cargaJanelaCarregamento.Carga.TipoOperacao?.Codigo ?? 0,
                    CodigoTransportador = cargaJanelaCarregamento.Carga.Empresa?.Codigo ?? 0,
                    CpfCnpjCliente = cargaJanelaCarregamento.Carga.Pedidos.FirstOrDefault()?.Pedido?.Destinatario?.CPF_CNPJ ?? 0,
                    CodigoModeloVeicularCarga = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.Codigo ?? 0
                };

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, ConfiguracaoEmbarcador, configuracaoDisponibilidadeCarregamento);
                List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento> horariosDisponiveis = servicoDisponibilidadeCarregamento.ObterIntervalosCarregamentosDisponiveis(cargaJanelaCarregamento.CentroCarregamento, dia.Value);

                dynamic horariosDisponiveisRetornar = (
                    from periodo in horariosDisponiveis
                    select new
                    {
                        Codigo = $"{periodo.Periodo}-{periodo.Index}",
                        Horario = $"{periodo.HoraInicio:hh\\:mm} - {periodo.HoraTermino:hh\\:mm}",
                        periodo.HoraInicio,
                        periodo.HoraTermino,
                    }
                ).ToList();

                grid.AdicionaRows(horariosDisponiveisRetornar);
                grid.setarQuantidadeTotal(horariosDisponiveis.Count);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoObterOsHorariosDisponíveis);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProximaDataDisponivel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: false);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: false);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga select o).FirstOrDefault();

                if (cargaJanelaCarregamentoTransportadorReferencia == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoVinculadaAoTransportador);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                List<dynamic> cargas = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo);
                    bool exibirMenorLance = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento?.CentroCarregamento?.PermiteTransportadorVisualizarMenorLanceLeilao ?? false;
                    Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete janelaComMenorLance = null;

                    if (exibirMenorLance)
                    {
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                            janelaComMenorLance = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargaJanelaCarregamentoTransportadorTerceiroComMenorLance(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, considerarCargasVinculadas: false);
                        else
                            janelaComMenorLance = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargaJanelaCarregamentoTransportadorComMenorLance(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, considerarCargasVinculadas: false);
                    }

                    cargas.Add(new
                    {
                        CodigoCarga = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo,
                        cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador,
                        ValorFrete = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.ValorFrete.ToString("n2"),
                        cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.TipoFreteEscolhido,
                        cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Moeda,
                        ValorCotacaoMoeda = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.ValorCotacaoMoeda?.ToString("n10") ?? "",
                        CobrarOutroDocumento = (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.Numero != "57") ? true : false,
                        ModeloDocumentoFiscal = (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.Numero != "57") ? new { cargaPedido.ModeloDocumentoFiscal.Codigo, cargaPedido.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                        MenorLance = exibirMenorLance ? (janelaComMenorLance?.ValorTotalFrete.ToString("n2") ?? "") : ""
                    });
                }

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga;
                DateTime dataCarregamentoProximoDia = RetornarProximoDiaUtilDeFuncionamentoDoCentro(cargaJanelaCarregamentoTransportadorReferencia, unitOfWork);
                bool exigirConfirmacaoTracao = carga.TipoOperacao?.ExigePlacaTracao ?? false;
                bool exigirInformarVeiculo = (cargaJanelaCarregamentoTransportadorReferencia.Tipo != TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo) && cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CargaLiberadaCotacao && !configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores;
                bool permitirSelecaoHorarioCarregamento = !(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.PermiteTransportadorSelecionarHorarioCarregamento ?? false) && !(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDescarga ?? false);

                return new JsonpResult(new
                {
                    DataCarregamentoProximoDia = dataCarregamentoProximoDia.ToString("dd/MM/yyyy"),
                    CodigoCarga = carga.Codigo,
                    PermitirSelecaoHorarioCarregamento = permitirSelecaoHorarioCarregamento,
                    ExigirConfirmacaoTracao = exigirConfirmacaoTracao,
                    ExigirInformarVeiculo = exigirInformarVeiculo,
                    ModeloVeiculo = new { Codigo = carga.ModeloVeicularCarga?.Codigo ?? 0, Descricao = carga.ModeloVeicularCarga?.Descricao ?? string.Empty },
                    NumeroReboques = carga.ModeloVeicularCarga?.NumeroReboques ?? 0,
                    TipoCarga = new { Codigo = carga.TipoDeCarga?.Codigo ?? 0, Descricao = carga.TipoDeCarga?.Descricao ?? string.Empty },
                    TipoVeiculo = (exigirConfirmacaoTracao ? "0" : ""),
                    Transportador = new { Usuario.Empresa.Codigo, Usuario.Empresa.Descricao },
                    Cargas = cargas
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoBuscarProximaDatadisponivel);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> MarcarSemInteresseCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                string observacao = Request.GetStringParam("Observacao");
                int codigoRecusa = Request.GetIntParam("MotivoRecusaCarga");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento repositorioMotivoRetiradaFilaCarregamento = new Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: true);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: true);

                Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento motivo = repositorioMotivoRetiradaFilaCarregamento.BuscarPorCodigo(codigoRecusa);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                {

                    MarcarSemInteresseTransportadorCarga(cargaJanelaCarregamentoTransportador, configuracaoEmbarcador, motivo, observacao, unitOfWork);

                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargas(cargasJanelaCarregamentoTransportador, unitOfWork)
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoSalvarAsInformacoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverInteresseCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: true);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: true);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                {
                    if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Empresa != null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossIvelDesmarcarInteresseDaCargaPoisViagemJaFoiDestinadaParaTransporte, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador)));
                    }

                    RemoverInteresseCarga(cargaJanelaCarregamentoTransportador, configuracaoEmbarcador, unitOfWork);

                    if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaLiberadaCotacao)
                        servicoCargaJanelaCarregamentoCotacao.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRemoveuInteresseNaCarga, cargaJanelaCarregamentoTransportador.Descricao)));
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargas(cargasJanelaCarregamentoTransportador, unitOfWork)
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoSalvarAsInformacoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: false);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: false);

                List<int> codigosCargasJanelaCarregamentoTransportador = cargasJanelaCarregamentoTransportador.Select(o => o.Codigo).ToList();
                List<dynamic> cargasRetornar = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga;
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                    cargasRetornar.Add(new
                    {
                        CodigoCarga = carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        ValorFrete = carga.ValorFrete.ToString("n2"),
                        carga.TipoFreteEscolhido,
                        carga.Moeda,
                        ValorCotacaoMoeda = carga.ValorCotacaoMoeda?.ToString("n10") ?? "",
                        CobrarOutroDocumento = (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.Numero != "57"),
                        ModeloDocumentoFiscal = (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal.Numero != "57") ? new { cargaPedido.ModeloDocumentoFiscal.Codigo, cargaPedido.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                    });
                }

                return new JsonpResult(cargasRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoObterOsDadosDoFrete);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarValorFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.Carga.ComponetesFrete servicoComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
                Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, ConfiguracaoEmbarcador);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelasCarregamentoTransportador = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();
                List<dynamic> valoresFretePorCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("Cargas"));

                foreach (dynamic valorFretePorCarga in valoresFretePorCarga)
                {
                    int codigoCarga = ((string)valorFretePorCarga.Codigo).ToInt();
                    decimal valorFreteTransportador = ((string)valorFretePorCarga.Valor).ToDecimal();
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(codigoCarga, Usuario.Empresa.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento;
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamento.Carga;

                    cargaJanelaCarregamentoTransportador.ValorFreteTransportador = valorFreteTransportador;

                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    SalvarComponentesFrete(valorFretePorCarga.ComponentesFrete, cargaJanelaCarregamentoTransportador, unitOfWork);

                    servicoCargaJanelaCarregamentoTransportador.DefinirTransportadorComValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware);
                    servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorAlterouValorDeFreteDaCarga, Usuario);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamentoTransportador, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.AlterouValorDeFreteDaCarga, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AlterouValorDeFreteDaCarga, cargaJanelaCarregamentoTransportador.Transportador.Descricao)), unitOfWork);

                    servicoCargaJanelaCarregamentoTransportador.InformaCargaAtualizadaEmbarcador(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo, Cliente, _conexao.AdminStringConexao);

                    cargasJanelasCarregamentoTransportador.Add(cargaJanelaCarregamentoTransportador);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargas(cargasJanelasCarregamentoTransportador, unitOfWork)
                });
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
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoInformarValorDeFrete);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarInteresseCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unitOfWork, ConfiguracaoEmbarcador);
                Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes servicoJanelaCarregamentoTransportadorValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes(unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, ConfiguracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro(unitOfWork, ConfiguracaoEmbarcador);
                Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorTerceiroValidacoes servicoJanelaCarregamentoTransportadorTerceiroValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorTerceiroValidacoes(unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                string horarioCarregamento = Request.GetStringParam("HorarioCarregamento");
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelasCarregamentoTransportador = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();
                List<dynamic> cargasInformarInteresse = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("Cargas"));

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = null;

                unitOfWork.Start();

                foreach (dynamic cargaInformarInteresse in cargasInformarInteresse)
                {
                    int codigoCarga = ((string)cargaInformarInteresse.Codigo).ToInt();
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = repositorioCarga.BuscarPorCodigo(codigoCarga);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { cargaReferencia };

                    if (cargaReferencia.CargaAgrupada)
                        cargas.AddRange(repositorioCarga.BuscarCargasOriginais(cargaReferencia.Codigo));

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = null;
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                            cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportadorTerceiro(carga.Codigo, Usuario?.ClienteTerceiro?.Codigo ?? 0);
                        else
                            cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(carga.Codigo, Usuario.Empresa.Codigo);

                        if (cargaJanelaCarregamentoTransportador == null)
                            continue;

                        if (configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores && cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.ProcessoCotacaoFinalizada)
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ACotacaooJaFoiEncerradaENaoPermiteMaisInformarInteresseNaCarga);

                        if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento?.EscolherPrimeiroTransportadorAoMarcarInteresseAutomaticamente ?? false)
                            SalvarInteressadoCarga(unitOfWork, carga);
                        else
                        {
                            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento;

                            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                            {
                                servicoJanelaCarregamentoTransportadorValidacoes.ValidarCertificadoDigital(centroCarregamento, Empresa);
                                servicoJanelaCarregamentoTransportadorValidacoes.ValidarApoliceSeguro(codigoCarga, Empresa, centroCarregamento, configuracaoDadosTransporte);
                            }

                            if (cargaJanelaCarregamentoTransportadorReferencia == null)
                                cargaJanelaCarregamentoTransportadorReferencia = cargaJanelaCarregamentoTransportador;

                            InformarInteresseCarga(cargaJanelaCarregamentoTransportador, horarioCarregamento, unitOfWork);
                            SalvarDadosTransporteInteresseCarga(cargaJanelaCarregamentoTransportador, configuracaoEmbarcador, configuracaoJanelaCarregamento, unitOfWork);

                            if ((carga.Codigo == cargaReferencia.Codigo) && (centroCarregamento?.PermitirTransportadorInformarValorFrete ?? false) || cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaLiberadaCotacao)
                            {
                                decimal valorFreteTransportador = ((string)cargaInformarInteresse.Valor).ToDecimal();

                                cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Rodada += 1;

                                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento);

                                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                                    servicoJanelaCarregamentoTransportadorTerceiroValidacoes.ValidarValorFrete(valorFreteTransportador, centroCarregamento, cargaJanelaCarregamentoTransportador, cargasInformarInteresse.Count > 1);
                                else
                                    servicoJanelaCarregamentoTransportadorValidacoes.ValidarValorFrete(valorFreteTransportador, centroCarregamento, cargaJanelaCarregamentoTransportador, cargasInformarInteresse.Count > 1);

                                cargaJanelaCarregamentoTransportador.ValorFreteTransportador = valorFreteTransportador;
                                repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);

                                SalvarComponentesFrete(cargaInformarInteresse.ComponentesFrete, cargaJanelaCarregamentoTransportador, unitOfWork);
                            }

                            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaLiberadaCotacao)
                            {
                                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(unitOfWork);
                                decimal valorComponentesFrete = repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.BuscarValorPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);
                                StringBuilder mensagem = new StringBuilder();

                                mensagem.Append((string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.InformouInteresseNaCarga, cargaJanelaCarregamentoTransportador.Descricao)));

                                if (cargaJanelaCarregamentoTransportador.HorarioCarregamento.HasValue)
                                    mensagem.Append(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.Data, cargaJanelaCarregamentoTransportador.HorarioCarregamento.Value.ToDateTimeString()));

                                if ((cargaJanelaCarregamentoTransportador.ValorFreteTransportador > 0m) && (valorComponentesFrete > 0m))
                                    mensagem.Append($" Valor: {(cargaJanelaCarregamentoTransportador.ValorFreteTransportador + valorComponentesFrete).ToString("n2")} (Valor Frete R$ {cargaJanelaCarregamentoTransportador.ValorFreteTransportador.ToString("n2")} + Valor Componentes Frete R$ {valorComponentesFrete.ToString("n2")}).");
                                else if (cargaJanelaCarregamentoTransportador.ValorFreteTransportador > 0m)
                                    mensagem.Append(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorValorFrete, cargaJanelaCarregamentoTransportador.ValorFreteTransportador.ToString("n2"), cargaJanelaCarregamentoTransportador.ValorFreteTransportador.ToString("n2")));
                                else if (valorComponentesFrete > 0m)
                                    mensagem.Append((string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorValorComponentesFrete, valorComponentesFrete.ToString("n2"), valorComponentesFrete.ToString("n2"))));

                                servicoCargaJanelaCarregamentoCotacao.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, mensagem.ToString());
                            }
                        }

                        if (carga.Codigo == cargaReferencia.Codigo)
                        {
                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                                servicoCargaJanelaCarregamentoTransportadorTerceiro.InformaCargaAtualizadaEmbarcador(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo, Cliente, _conexao.AdminStringConexao);
                            else
                                servicoCargaJanelaCarregamentoTransportador.InformaCargaAtualizadaEmbarcador(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo, Cliente, _conexao.AdminStringConexao);

                            cargasJanelasCarregamentoTransportador.Add(cargaJanelaCarregamentoTransportador);
                        }
                    }
                }

                EnviarNotificacoesEmailInteresseCarga(cargasJanelasCarregamentoTransportador, cargaJanelaCarregamentoTransportadorReferencia, configuracaoEmbarcador, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargas(cargasJanelasCarregamentoTransportador, unitOfWork)
                });
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
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoInformarInteresse);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> InformarInteresseCargaDadosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte repositorioCargaJanelaCarregamentoTransportadorDadosTransporte = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                List<dynamic> cargasInformarInteresse = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("Cargas"));

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo, true);
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);

                unitOfWork.Start();

                foreach (dynamic cargaInformarInteresse in cargasInformarInteresse)
                {
                    int codigoCarga = ((string)cargaInformarInteresse.Codigo).ToInt();

                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                        cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: true);
                    else
                        cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: true);

                    foreach (var cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte dadosTransporte = null;
                        if (cargaJanelaCarregamentoTransportador.DadosTransporte != null)
                            dadosTransporte = cargaJanelaCarregamentoTransportador.DadosTransporte;
                        else
                            dadosTransporte = new CargaJanelaCarregamentoTransportadorDadosTransporte();

                        dadosTransporte.CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador;
                        dadosTransporte.Motorista = motorista;
                        dadosTransporte.Veiculo = veiculo;

                        if (dadosTransporte.Codigo > 0)
                            repositorioCargaJanelaCarregamentoTransportadorDadosTransporte.Atualizar(dadosTransporte);
                        else
                            repositorioCargaJanelaCarregamentoTransportadorDadosTransporte.Inserir(dadosTransporte);

                        cargaJanelaCarregamentoTransportador.DadosTransporte = dadosTransporte;
                        repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    }

                }
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
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoInformarInteresse);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> SolicitarNotasFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: true);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: true);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga select o).FirstOrDefault();

                if (cargaJanelaCarregamentoTransportadorReferencia == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoFoiPossivelEncontrarCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga;

                unitOfWork.Start();

                servicoCarga.SolicitarNotasFiscais(carga, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, unitOfWork);

                unitOfWork.CommitChanges();

                if (carga.TipoOperacao?.NotificarRemetentePorEmailAoSolicitarNotas ?? false)
                    servicoCarga.NotificarRemetentesPorEmailAoSolicitarNotasFiscais(carga, unitOfWork);

                return new JsonpResult(new
                {
                    PercursoMDFeValido = true
                });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();

                if (excecao.ErrorCode == CodigoExcecao.PercursoNaoConfigurado)
                    return new JsonpResult(new
                    {
                        PercursoMDFeValido = false
                    });

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoBuscarAoSolicitarAsNotasFiscais);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarDadosTransporteCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repositorioCConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDadosSumarizados repositorioCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist repChecklist = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoEmpresa = Request.GetIntParam("Transportador");
                bool dadosInformadosNoInteresse = Request.GetBoolParam("DadosInformadosNoInteresse");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repositorioCConfiguracaoMotorista.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Empresa transportadorAuditoria = Usuario.Empresa;
                Dominio.Entidades.Empresa transportador = ObterEmpresaSelecionada(codigoCarga, codigoEmpresa, unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);
                bool enviarEmailDadosTransporte = !repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: true);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: true);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga select o).FirstOrDefault();

                if (cargaJanelaCarregamentoTransportadorReferencia == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoVinculadaAoTransportador);

                if (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador
                    && cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.Empresa != null
                    && cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.Empresa != transportador)
                    return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelSalvarDadosTransportePoisTransportadorDiferenteInformadoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cPedidos = repCargaPedido.BuscarPorCarga(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.Codigo);
                DateTime? dataBaseCalculoLimiteCarregamento = (from o in cPedidos where o.Pedido.PrevisaoEntrega.HasValue select o.Pedido.PrevisaoEntrega).OrderBy(o => o).FirstOrDefault();

                List<int> pedidosComNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork).BuscarListaPedidosComNotaPorPedidos((from o in cPedidos select o.Pedido.Codigo).ToList());
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> ceps = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork).BuscarPorCargaPedidos((from o in cPedidos select o.Codigo).ToList());

                DateTime? dataBaseCalculoLimiteCarregamentoSemNF = ceps.Where(o => o.CargaEntrega.DataAgendamento.HasValue
                                                                                && (!o.CargaPedido.Pedido?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.AgendamentoExigeNotaFiscal ?? false)
                                                                                && !pedidosComNota.Contains(o.CargaPedido.Pedido.Codigo)).OrderBy(o => o.CargaEntrega.DataAgendamento).Select(o => o.CargaEntrega?.DataAgendamento).FirstOrDefault();

                if (dataBaseCalculoLimiteCarregamentoSemNF.HasValue)
                    dataBaseCalculoLimiteCarregamento = dataBaseCalculoLimiteCarregamentoSemNF;

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork).BuscarPorCarga(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.Codigo);

                if (cargaJanelaCarregamentoTransportadorReferencia?.CargaJanelaCarregamento?.CentroCarregamento?.ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido ?? false)
                    if (dataBaseCalculoLimiteCarregamento != null && dataBaseCalculoLimiteCarregamento?.AddMinutes(-CalcularTempoLimiteCarregamentoEmMinutos(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, unitOfWork)) < cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.InicioCarregamento)
                        return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.HorarioCarregamentoUltrapassaHorarioLimite);

                if (!dadosInformadosNoInteresse)
                {
                    if (
                        (
                            (cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgAceite) &&
                            (cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao) &&
                            (cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Confirmada)
                        ) ||
                        cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Cancelada
                    )
                        return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.AsituacaoDaCargaNaoPermiteAtualizacaoDosDadosDeTransporte);

                }
                bool exibirAvisoVerificarExistenciaMDFeAbertoForaDoSistema = Request.GetBoolParam("VerificarMDFeAbertoForaDoSistema") && (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.ExigirTransportadorConfirmarMDFeNaoEncerradoForaDoSistema ?? false);

                if (exibirAvisoVerificarExistenciaMDFeAbertoForaDoSistema)
                {
                    return new JsonpResult(new
                    {
                        AvisoMDFeEmAberto = true
                    });
                }

                int codigoModeloVeicularCarga = Request.GetIntParam("ModeloVeiculo");
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoReboque = Request.GetIntParam("Reboque");
                int codigoSegundoReboque = Request.GetIntParam("SegundoReboque");
                List<int> codigosAjudantes = Request.GetListParam<int>("Ajudante");


                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = janelaCarregamento.Carga;
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = janelaCarregamento.CentroCarregamento;
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo, true);
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                Dominio.Entidades.Veiculo primeiroReboque = null;
                Dominio.Entidades.Veiculo segundoReboque = null;
                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repPosicaoAtual.BuscarPorVeiculo(veiculo.Codigo);
                Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento = null;
                Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca = repositorioCargaLicenca.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Usuario> ajudantes = (codigosAjudantes.Count > 0) ? repUsuario.BuscarPorCodigos(codigosAjudantes) : new List<Dominio.Entidades.Usuario>();

                string observacoes = (centroCarregamento?.PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador ?? false) ? Request.GetStringParam("ObservacaoInformadaPeloTransportador") : "";

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoEmbarcador, Auditado);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, Auditado);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);
                new Servicos.Embarcador.Integracao.SemParar.ConsultaCadastroVeiculo(unitOfWork, TipoServicoMultisoftware).GerarIntegracaoCadastroVeiculo(veiculo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist> checklistReboque = new List<CargaJanelaCarregamentoTransportadorChecklist>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist> checklistSegundoReboque = new List<CargaJanelaCarregamentoTransportadorChecklist>();

                if (codigoReboque > 0)
                    checklistReboque = repChecklist.BuscarPorCargaJanelaCarregamentoEVeiculo(cargaJanelaCarregamentoTransportadorReferencia.Codigo, codigoReboque);

                if (codigoSegundoReboque > 0)
                    checklistSegundoReboque = repChecklist.BuscarPorCargaJanelaCarregamentoEVeiculo(cargaJanelaCarregamentoTransportadorReferencia.Codigo, codigoSegundoReboque);

                bool possuiChecklist = (codigoReboque > 0 && checklistReboque.Count > 0) && (codigoSegundoReboque == 0 || (codigoSegundoReboque > 0 && checklistSegundoReboque.Count > 0));

                if ((centroCarregamento?.ExigirChecklistAoConfirmarDadosTransporteMultiTransportador ?? false) && !possuiChecklist)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ObrigatorioPreencherChecklist);

                if (servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueAguardandoRetornoIntegracao(carga))
                    return new JsonpResult(true, false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.AIntegracaoDaCargaParaGerarOrdemDeEmbarqueEstaAguardandoRetorno);

                carga.Initialize();
                carga.ModeloVeicularCarga = modeloVeicularCarga;

                bool permitirSelecaoPeriodoCarregamento = !carga.HorarioCarregamentoInformadoNoPedido && (centroCarregamento?.PermitirSelecaoPeriodoCarregamentoJanelaCarregamentoTransportador ?? false);

                servicoCarga.ValidarPermissaoAlterarDadosEtapaTransportadorOuDadosTransporte(carga, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork);

                Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes servicoJanelaCarregamentoTransportadorValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes(unitOfWork, TipoServicoMultisoftware, configuracaoEmbarcador);

                servicoJanelaCarregamentoTransportadorValidacoes.ValidarMDFeEmAberto(veiculo);

                if ((centroCarregamento?.BloquearVeiculoSemEspelhamento ?? false) || (carga.TipoOperacao?.ConfiguracaoTransportador?.BloquearVeiculoSemEspelhamentoJanela ?? false))
                    servicoJanelaCarregamentoTransportadorValidacoes.ValidarEspelhamentoVeiculo(veiculo, posicaoAtual);

                if ((carga.TipoOperacao?.ExigirVeiculoComRastreador ?? false) && (veiculo != null && (veiculo?.TipoVeiculo ?? "1") == "0" && !(veiculo?.PossuiRastreador ?? false)))
                    return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ParaTipoDeOperacaoObrigatorioInformarUmVeiculoComRastreadorCadastrado, carga.TipoOperacao.Descricao)));

                if (string.IsNullOrWhiteSpace(veiculo.Renavam) && config.Pais == TipoPais.Brasil)
                    return new JsonpResult(true, false, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelSelecionarVeiculoPoisMesmoNaoPossuiRENAVAMInformado, veiculo.Placa)));

                if (dadosInformadosNoInteresse)
                {
                    SalvarDadosTransporteInteresse(cargaJanelaCarregamentoTransportadorReferencia, unitOfWork, veiculo, motorista);
                }

                if (
                    (!configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportador && !configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao) ||
                    (configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao && !Request.GetBoolParam("SelecaoQualquerVeiculoConfirmada"))
                )
                {
                    string codigoCargaAberto = repCarga.BuscaNumeroDaCargaEmAbertoPorVeiculo(carga.Codigo, veiculo.Placa);

                    if (!string.IsNullOrWhiteSpace(codigoCargaAberto))
                    {
                        if (configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao)
                            return new JsonpResult(new
                            {
                                ConfirmarSelecaoQualquerVeiculo = true,
                                Mensagem = (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OVeiculoEstaAlocadoEmUmaCargaQueEstaEmProcessoDeAlocacaoCarga, codigoCargaAberto))
                            });

                        return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelSelecionarEsteVeiculoPoisEleEstaAlocadoEmUmaCargaQueEstaEmProcessoDeAlocaçãoCarga, codigoCargaAberto)));
                    }
                }

                if (configuracaoEmbarcador.ValidarDataLiberacaoSeguradora && !motorista.DataValidadeLiberacaoSeguradora.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OMotoristaNaoPossuiUmaDataDeLimiteDaSeguradoraConfiguradaPorIssoNaoPossivelIinformarEsteMotoristaParatransportarEssaCargaVerifiqueTenteNovamente);

                if (configuracaoEmbarcador.ValidarDataLiberacaoSeguradora && (motorista.DataValidadeLiberacaoSeguradora.HasValue && motorista.DataValidadeLiberacaoSeguradora.Value.AddDays(1).AddMinutes(-1) < DateTime.Now))
                    return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ADataDeLimiteMotoristaNaSeguradoraEstaValidoAtePorIssoNaoPossivelInformarEsteMotoristaParaTransportarEssaCargaVerifiqueTenteNovamente, motorista.DataValidadeLiberacaoSeguradora.Value.ToString("dd/MM/yyyy"))));

                if (veiculo.TipoVeiculo == "1" && veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                {
                    Dominio.Entidades.Veiculo tracao = (from obj in veiculo.VeiculosTracao where obj.Ativo select obj).FirstOrDefault();
                    if (tracao != null)
                        veiculo = tracao;
                }

                if (motorista.Bloqueado)
                {
                    if (!string.IsNullOrWhiteSpace(configuracaoMotorista.MensagemPersonalizadaMotoristaBloqueado))
                        return new JsonpResult(false, true, configuracaoMotorista.MensagemPersonalizadaMotoristaBloqueado);

                    return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OMotoristaEstaBloqueadoProcureEmbarcadorParaenntendermotivo, motorista.Nome)));
                }

                if (veiculo.VeiculoBloqueado)
                    return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OVeiculoEstaBloqueadoProcureEmbarcadorParaEntenderMotivo, veiculo.Placa_Formatada)));

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    if (transportador?.BloquearTransportador ?? false)
                        return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OtransportadorEstaBloqueadoProcureEmbarcadorParaEntenderMotivo, transportador.Descricao)));

                    if (carga.CargaPerigosaIntegracaoLeilao && (carga.TipoOperacao?.ConfiguracaoTransportador?.BloquearTransportadorNaoIMOAptoCargasPerigosas ?? false))
                    {
                        bool possuiInformacoesIMO = transportador != null ? (!string.IsNullOrEmpty(transportador.IMO) && (transportador.DataValidadeIMO.HasValue && transportador.DataValidadeIMO.Value > DateTime.Now)) : false;

                        if (!possuiInformacoesIMO)
                            throw new ControllerException("Configuração não permite salvar pois o transportador não contém documento de IMO.");
                    }
                }

                // Validação do veículo e motorista da carga nas Gerenciadoras de Risco e Seguradoras
                Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoMotorista = new Servicos.Embarcador.Transportadores.MotoristaGR(unitOfWork).Validar(carga, motorista, TipoServicoMultisoftware);
                if (retornoMotorista != null)
                {
                    carga.ProtocoloIntegracaoGR = retornoMotorista.Protocolo;
                    if (!retornoMotorista.Sucesso)
                    {
                        if (retornoMotorista.TipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Telerisco)
                        {
                            return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.RetornoConsultaMotoristaTelerisco, retornoMotorista.Mensagem)));
                        }
                        else if (retornoMotorista.TipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Adagio)
                        {
                            return new JsonpResult(false, true, retornoMotorista.Mensagem);
                        }
                        else
                        {
                            carga.MensagemProblemaIntegracaoGrMotoristaVeiculo = retornoMotorista.Mensagem;
                            carga.ProblemaIntegracaoGrMotoristaVeiculo = true;
                            carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo = false;
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoVeiculo = new Servicos.Embarcador.Veiculo.VeiculoGR(unitOfWork).Validar(carga, veiculo);
                if (retornoVeiculo != null && !retornoVeiculo.Sucesso)
                {
                    if (retornoVeiculo.TipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Adagio)
                    {
                        return new JsonpResult(false, true, retornoVeiculo.Mensagem);
                    }
                    carga.ProblemaIntegracaoGrMotoristaVeiculo = true;
                    carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo = false;
                    carga.MensagemProblemaIntegracaoGrMotoristaVeiculo = string.IsNullOrWhiteSpace(carga.MensagemProblemaIntegracaoGrMotoristaVeiculo) ? retornoVeiculo.Mensagem : string.Concat(carga.MensagemProblemaIntegracaoGrMotoristaVeiculo, " | ", retornoVeiculo.Mensagem);
                }

                bool exigirConfirmacaoTracao = carga?.TipoOperacao?.ExigePlacaTracao ?? false;
                bool possuiGenset = carga.Pedidos?.Any(o => o.Pedido.PossuiGenset == true) ?? false;
                List<Dominio.Entidades.Veiculo> reboques = null;

                if (exigirConfirmacaoTracao)
                {
                    reboques = new List<Dominio.Entidades.Veiculo>();

                    if (codigoReboque > 0)
                    {
                        primeiroReboque = repVeiculo.BuscarPorCodigo(codigoReboque);

                        if (primeiroReboque != null)
                            reboques.Add(primeiroReboque);
                    }

                    if (codigoSegundoReboque > 0)
                    {
                        segundoReboque = repVeiculo.BuscarPorCodigo(codigoSegundoReboque);

                        if (segundoReboque != null && !reboques.Contains(segundoReboque))
                            reboques.Add(segundoReboque);
                    }
                }
                else
                    reboques = veiculo.VeiculosVinculados.ToList();

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCargaVeiculo = (reboques.Count > 0) ? reboques.FirstOrDefault().ModeloVeicularCarga : veiculo.ModeloVeicularCarga;

                if (exigirConfirmacaoTracao)
                {
                    if (veiculo == null)
                        return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarPlacaDaTracaoAoConjuntoDaPlacaSelecionadaPorFavorFacaVinculoTracaoboqueNoCadastroDeVeiculosTenteNovamente);

                    if (modeloVeicularCargaVeiculo == null)
                        return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarUmVeiculoComModeloVeicularDefinido);

                    if (modeloVeicularCargaVeiculo.Tipo == TipoModeloVeicularCarga.Tracao && modeloVeicularCargaVeiculo.NumeroReboques > reboques.Count())
                        return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarReboquedatracaoSelecionadaNumeroDeReboquesNecessarioPorFavorFacaVinculoTracaoReboqueNoCadastroDeVeiculostenteNovamente, modeloVeicularCargaVeiculo.NumeroReboques)));
                }

                Servicos.Embarcador.Veiculo.Veiculo.ValidarBloqueioPorCargaNaoFinalizada(veiculo?.Placa, carga.Codigo, configuracaoEmbarcador, unitOfWork);
                Servicos.Embarcador.Veiculo.Veiculo.ValidarDataLiberacaoSeguradora(veiculo, configuracaoEmbarcador);
                Servicos.Embarcador.Veiculo.Veiculo.ValidarDataLiberacaoSeguradora(reboques, configuracaoEmbarcador);

                if (carga.TipoOperacao != null && carga.TipoOperacao.ExigeQueVeiculoIgualModeloVeicularDaCarga && carga.ModeloVeicularCarga != null)
                {
                    if (reboques.Count > 0)
                    {
                        if (reboques.Any(vei => vei.ModeloVeicularCarga?.Codigo != carga.ModeloVeicularCarga.Codigo))
                            return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarReboquesCompativeisComModeloDeVeiculoDaCarga, carga.ModeloVeicularCarga.Descricao)));
                    }
                    else if (veiculo != null)
                    {
                        if (veiculo.ModeloVeicularCarga?.Codigo != carga.ModeloVeicularCarga.Codigo)
                            return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarUmVeiculoCompativelComModeloDoVeiculoDCarga, carga.ModeloVeicularCarga.Descricao)));
                    }
                }

                if (permitirSelecaoPeriodoCarregamento)
                {
                    int codigoPeriodoCarregamento = Request.GetIntParam("PeriodoCarregamento");
                    DateTime dataCarregamento = janelaCarregamento.InicioCarregamento;
                    List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = null;
                    Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecaoDia = (from o in centroCarregamento.ExcecoesCapacidadeCarregamento where o.Data == dataCarregamento.Date select o).FirstOrDefault();

                    if (excecaoDia != null)
                        periodosCarregamento = excecaoDia.PeriodosCarregamento.ToList();
                    else
                    {
                        DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataCarregamento);
                        periodosCarregamento = (from o in centroCarregamento.PeriodosCarregamento where o.Dia == diaSemana select o).ToList();
                    }

                    if (periodosCarregamento.Count == 0)
                        return new JsonpResult(false, true, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoExistePeriodoDeCarregamentoConfiguradoParaDia, dataCarregamento.ToString("dd/MM/yyyy"))));

                    if (codigoPeriodoCarregamento == 0)
                    {
                        var periodosCarregamentoRetornar = (
                            from o in periodosCarregamento
                            select new
                            {
                                o.Codigo,
                                Descricao = o.DescricaoPeriodo
                            }
                        ).ToList();

                        return new JsonpResult(new
                        {
                            SelecionarPeriodoCarregamento = true,
                            PeriodosCarregamento = periodosCarregamentoRetornar
                        });
                    }

                    periodoCarregamento = (from o in periodosCarregamento where o.Codigo == codigoPeriodoCarregamento select o).FirstOrDefault();

                    if (periodoCarregamento == null)
                        return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OPeriodoDeCarregamentoInformadoNaoFoiEncontrado);
                }

                string mensagemRetorno = "";

                List<Dominio.Entidades.Veiculo> veiculosDaCarga = new List<Dominio.Entidades.Veiculo>();

                if (veiculo != null)
                    veiculosDaCarga.Add(veiculo);

                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                    veiculosDaCarga.AddRange(carga.VeiculosVinculados);

                servicoJanelaCarregamentoTransportadorValidacoes.ValidarDadosViaIntegracao(carga, veiculosDaCarga, transportador, out mensagemRetorno);

                if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                    throw new ControllerException(mensagemRetorno);

                servicoJanelaCarregamentoTransportadorValidacoes.ValidarLimiteCargasParaMotorista(janelaCarregamento, motorista);
                servicoJanelaCarregamentoTransportadorValidacoes.ValidarLimiteCargasParaVeiculo(janelaCarregamento, veiculo);

                unitOfWork.Start();

                AtualizarVeiculoContainer(carga, veiculo, primeiroReboque, segundoReboque, possuiGenset, unitOfWork);

                if (veiculo.Tipo == "T")
                {
                    double responsavelValePedagio = Request.GetDoubleParam("ResponsavelValePedagio");
                    double fornecedorValePedagio = Request.GetDoubleParam("FornecedorValePedagio");

                    veiculo.CIOT = Request.GetStringParam("CIOT");
                    veiculo.ValorValePedagio = Request.GetDecimalParam("ValorValePedagio");
                    veiculo.ResponsavelValePedagio = (responsavelValePedagio > 0) ? new Dominio.Entidades.Cliente() { CPF_CNPJ = responsavelValePedagio } : null;
                    veiculo.FornecedorValePedagio = (fornecedorValePedagio > 0) ? new Dominio.Entidades.Cliente() { CPF_CNPJ = fornecedorValePedagio } : null;
                    veiculo.NumeroCompraValePedagio = Request.GetStringParam("NumeroCompraValePedagio");

                    repVeiculo.Atualizar(veiculo, Auditado);

                    foreach (Dominio.Entidades.Veiculo reboque in reboques)
                    {
                        reboque.Initialize();

                        reboque.CIOT = veiculo.CIOT;
                        reboque.ValorValePedagio = veiculo.ValorValePedagio;
                        reboque.NumeroCompraValePedagio = veiculo.NumeroCompraValePedagio;
                        reboque.FornecedorValePedagio = veiculo.FornecedorValePedagio;
                        reboque.ResponsavelValePedagio = veiculo.ResponsavelValePedagio;

                        repVeiculo.Atualizar(reboque, Auditado);
                    }
                    carga.FreteDeTerceiro = true;
                }
                else if (carga.ProvedorOS != null)
                    carga.FreteDeTerceiro = true;
                else
                    carga.FreteDeTerceiro = false;

                repCarga.Atualizar(carga);

                if (carga.DadosSumarizados != null)
                {
                    carga.DadosSumarizados.ObservacaoInformadaPeloTransportador = Utilidades.String.Left(observacoes, 400);

                    repositorioCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
                }

                AtualizarCargaLacres(carga, unitOfWork);

                if (veiculo.ModeloVeicularCarga == null && carga.ModeloVeicularCarga != null)
                {
                    veiculo.ModeloVeicularCarga = carga.ModeloVeicularCarga;
                    repVeiculo.Atualizar(veiculo, Auditado);

                    foreach (Dominio.Entidades.Veiculo reboque in reboques)
                    {
                        if (reboque.ModeloVeicularCarga == null)
                        {
                            reboque.ModeloVeicularCarga = carga.ModeloVeicularCarga;
                            repVeiculo.Atualizar(reboque);
                        }
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repTipoCargaModeloVeicular.ConsultarPorTipoCarga(carga.TipoDeCarga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = (from o in tipoCargaModeloVeicular select o.ModeloVeicularCarga).ToList();


                if (!modelosVeicularesPermitidos.Contains(veiculo.ModeloVeicularCarga))
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelUtilizarEsseVeiculoParaTransportarCargaPoisSeuTipoNaoSuportaEsseTransporte);

                if (configuracaoEmbarcador.TipoRestricaoPalletModeloVeicularCarga != TipoRestricaoPalletModeloVeicularCarga.NaoBloquear)
                {
                    int palletsModelo = veiculo.ModeloVeicularCarga.NumeroPaletes.HasValue ? veiculo.ModeloVeicularCarga.NumeroPaletes.Value : 0;
                    int numeroPallet = carga.ModeloVeicularCarga?.NumeroPaletes ?? carga.Pedidos?.Sum(obj => obj.Pedido?.NumeroPaletes) ?? 0;
                    if (configuracaoEmbarcador.TipoRestricaoPalletModeloVeicularCarga == TipoRestricaoPalletModeloVeicularCarga.BloquearSomenteNumeroSuperior && numeroPallet > palletsModelo)
                        throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelUtilizarEsseVeiculoParaTransportarCargaPoisQuantidadeDePalletsExigidoParaCargaSuperiorAoSuportadoPorEsseVeiculo, numeroPallet.ToString(), palletsModelo.ToString()));

                    if (configuracaoEmbarcador.TipoRestricaoPalletModeloVeicularCarga == TipoRestricaoPalletModeloVeicularCarga.BloquearSomenteNumeroInferior && numeroPallet < palletsModelo)
                        throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelUtilizarEsseVeiculoParaTransportarCargaPoisQuantidadeDePalletsExigidoParaCargaInferiorCapacidadeDesteVeiculo, numeroPallet.ToString(), palletsModelo.ToString()));
                }

                servicoCarga.ValidarDadosFaltantesNoVeiculo(veiculo, transportador, unitOfWork);

                if (!configuracaoEmbarcador.NaoExigeInformarDisponibilidadeDeVeiculo)
                {
                    Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento veiculoDisponivelCarregamento = null;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                        veiculoDisponivelCarregamento = repVeiculoDisponivelCarregamento.BuscarPorVeiculoProprietario(veiculo.Codigo, Usuario.ClienteTerceiro.CPF_CNPJ);
                    else
                        veiculoDisponivelCarregamento = repVeiculoDisponivelCarregamento.BuscarPorVeiculoEmpresa(veiculo.Codigo, transportador.Codigo);

                    if (veiculoDisponivelCarregamento != null)
                    {
                        veiculoDisponivelCarregamento.DataIndisponibilizacaoVeiculo = DateTime.Now;
                        veiculoDisponivelCarregamento.UsuarioIndisponibilizou = Usuario;
                        veiculoDisponivelCarregamento.Disponivel = false;

                        repVeiculoDisponivelCarregamento.Atualizar(veiculoDisponivelCarregamento);
                    }
                    else if ((carga.Veiculo == null || carga.Veiculo.Codigo != veiculo.Codigo))
                        throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoEPossivelUtilizarEsseVeiculoParaTransportarCargaPoisEleNaoEstaDisponivel);

                }

                List<Dominio.Entidades.Usuario> usuariosNotificacao = new List<Dominio.Entidades.Usuario>();
                List<string> emailsNotificacao = new List<string>();
                bool shouldNotificarPorEmail = centroCarregamento?.EnviarNotificacoesPorEmail ?? false;

                if (cargasJanelaCarregamentoTransportador == null || cargasJanelaCarregamentoTransportador.Count() == 0)
                {
                    Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracaoEmbarcador, Auditado, Localization.Resources.Logistica.JanelaCarregamentoTransportador.JanelaDeCarregamentoDoTransportador, unitOfWork);
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                    {
                        cargaJanelaCarregamentoTransportador.Observacao = observacoes;
                        SalvarDadosTransporte(cargaJanelaCarregamentoTransportador, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga, veiculo, reboques, motorista, ajudantes, transportador, modeloVeicularCarga, unitOfWork, ConfiguracaoEmbarcador);
                        List<Dominio.Entidades.Usuario> usuariosNotificacaoCarregamento = centroCarregamento?.UsuariosNotificacao.ToList() ?? new List<Dominio.Entidades.Usuario>();
                        List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail> emailsCentroCarregamento = shouldNotificarPorEmail ? centroCarregamento?.Emails.ToList() : null;

                        if (usuariosNotificacaoCarregamento.Count > 0)
                            usuariosNotificacao.AddRange(usuariosNotificacaoCarregamento);
                        else if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador != null)
                            usuariosNotificacao.Add(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador);

                        if (emailsCentroCarregamento != null && emailsCentroCarregamento.Count > 0)
                            emailsNotificacao.AddRange(emailsCentroCarregamento.Select(o => o.Email));
                    }
                }

                if (periodoCarregamento != null)
                {
                    janelaCarregamento.Initialize();
                    servicoDisponibilidadeCarregamento.DefinirHorarioCarregamentoPorPeriodo(janelaCarregamento, periodoCarregamento);
                    Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, janelaCarregamento, janelaCarregamento.GetChanges(), $"Data de carregamento alterada{(janelaCarregamento.Excedente ? "" : $" para {janelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy")}")}", unitOfWork);
                }

                Servicos.Embarcador.Carga.AlertaCarga.CargaSemVeiculo servicoAlertaCargaSemVeiculo = new Servicos.Embarcador.Carga.AlertaCarga.CargaSemVeiculo(unitOfWork, _conexao.StringConexao);
                if (servicoAlertaCargaSemVeiculo.EstaAtivo() && carga.Veiculo != null)
                    servicoAlertaCargaSemVeiculo.TratarAlertaAposAdicionarVeiculoCarga(carga, carga.Veiculo);

                servicoCargaJanelaDescarregamento.AtualizarSituacao(carga, SituacaoCargaJanelaDescarregamentoCadastrada.Programado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamentoTransportadorReferencia, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.SalvouDadosTransporteCarga, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OTransportadorInformouOsDadosDeTransporteDaCarga, transportadorAuditoria.Descricao)), unitOfWork);

                if (carga.Rota != null && (carga.Rota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Erro || carga.Rota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.SemDefinicao) && !configuracaoEmbarcador.CadastrarRotaAutomaticamente && !configuracaoEmbarcador.ExigirRotaRoteirizadaNaCarga)
                    Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, null, configuracaoEmbarcador, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                    Servicos.Embarcador.Carga.RateioFrete servicoCargaRateio = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

                    servicoCargaRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, false, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                    if (carga.EmpresaFilialEmissora != null)
                        servicoCargaRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, true, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                }

                string numeroCarga = servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador);
                string mensagemNotificacao = (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.InformouOsDadosDeTransporteDaCarga, transportadorAuditoria.RazaoSocial, numeroCarga));

                servicoCargaJanelaCarregamentoTransportador.NotificaUsuarios(cargaJanelaCarregamentoTransportadorReferencia, usuariosNotificacao, mensagemNotificacao, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);
                if (enviarEmailDadosTransporte)
                    servicoCargaJanelaCarregamentoTransportador.NotificaEmails(cargaJanelaCarregamentoTransportadorReferencia, emailsNotificacao, mensagemNotificacao, unitOfWork);

                Servicos.Embarcador.Integracao.OneTrust.IntegracaoOneTrust servicoIntegracaoOneTrust = new Servicos.Embarcador.Integracao.OneTrust.IntegracaoOneTrust(ConfiguracaoEmbarcador, unitOfWork);
                servicoIntegracaoOneTrust.VerificarSituacaoMotorista(motorista);

                unitOfWork.CommitChanges();

                cargaJanelaCarregamentoTransportadorReferencia = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(cargaJanelaCarregamentoTransportadorReferencia.Codigo);

                servicoCargaJanelaCarregamentoTransportador.ValidarCargasInteressadas(cargaJanelaCarregamentoTransportadorReferencia.Transportador, carga);
                servicoCargaJanelaCarregamentoTransportador.InformaCargaAtualizadaEmbarcador(carga.Codigo, Cliente, _conexao.AdminStringConexao);
                servicoCarga.GerarNotificacaoEmailFornecedorDadosTransporte(carga, unitOfWork);
                new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork, TipoServicoMultisoftware).ReenviarIntegracoesCargaDadosTransportePosFreteAsync(carga, unitOfWork).GetAwaiter().GetResult();

                if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                    servicoCarga.NotificarAlteracaoAoOperador(carga, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OTransportadorInformouOsDadosDeTransporteDaCargan, transportadorAuditoria.Descricao, carga.CodigoCargaEmbarcador)), unitOfWork, TipoServicoMultisoftware);

                cargasJanelaCarregamentoTransportador.Remove(cargaJanelaCarregamentoTransportadorReferencia);

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargas(cargasJanelaCarregamentoTransportador, cargaJanelaCarregamentoTransportadorReferencia, unitOfWork),
                    SolicitarNotasFiscaisAoSalvarDadosTransportador = carga?.TipoOperacao?.SolicitarNotasFiscaisAoSalvarDadosTransportador ?? false
                });
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
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoSalvarOsDadosDeTransporteDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecalcularValorFreteTabelaTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(codigoCarga, Usuario.Empresa.Codigo);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite repositorioJanelaCarregamentoTermoAceite = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite(unitOfWork);

                if (cargaJanelaCarregamentoTransportador == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoVinculadaAoTransportador);

                if (!(cargaJanelaCarregamentoTransportador.PendenteCalcularFrete || cargaJanelaCarregamentoTransportador.FreteCalculadoComProblemas))
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OValorDoFreteJaEstaCalculado);

                unitOfWork.Start();

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, ConfiguracaoEmbarcador);

                servicoCargaJanelaCarregamentoTransportador.CalcularFreteParaTransportador(cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamentoTransportador, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CalculadoValorDeFreteDoTransportador, unitOfWork);

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscias = repositorioPedidoXmlNotaFiscal.BuscarPorCarga(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo);

                unitOfWork.CommitChanges();

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoDados> dadosDescarregamentos = repositorioCargaJanelaCarregamento.BuscarDadosDescarregamentosPorCarga(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite> termosAceite = repositorioJanelaCarregamentoTermoAceite.BuscarPorJanelasTransportador(new List<int> { cargaJanelaCarregamentoTransportador.Codigo });

                return new JsonpResult(ObterDadosCarga(cargaJanelaCarregamentoTransportador, notasFiscias, dadosDescarregamentos, termosAceite, unitOfWork));
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoCalcularValorDeFreteDoTransportador);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                string motivoRejeicaoCarga = Request.GetStringParam("MotivoRejeicaoCarga");

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(codigoCarga, Usuario?.ClienteTerceiro, retornarCargasOriginais: true);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, Usuario.Empresa, retornarCargasOriginais: true);

                unitOfWork.Start();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    servicoCargaJanelaCarregamentoTransportadorTerceiro.RejeitarCarga(codigoCarga, motivoRejeicaoCarga, cargasJanelaCarregamentoTransportador, Cliente, Usuario, TipoServicoMultisoftware, Auditado, _conexao.AdminStringConexao);
                else
                    servicoCargaJanelaCarregamentoTransportador.RejeitarCarga(codigoCarga, motivoRejeicaoCarga, cargasJanelaCarregamentoTransportador, Cliente, Usuario, TipoServicoMultisoftware, Auditado, _conexao.AdminStringConexao);

                unitOfWork.Rollback();

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargas(cargasJanelaCarregamentoTransportador, unitOfWork)
                });
            }
            catch (ServicoException servicoExcecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(servicoExcecao);
                return new JsonpResult(false, true, servicoExcecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoRejeitarCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ImprimirOrdemColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioJanelaCarregamento.BuscarPorCargaComFetchCargaCentroDescarregamento(codigoCarga);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.RegistroNaoFoiEncontrado);

                byte[] pdf = ReportRequest.WithType(ReportType.OrdemColeta)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoJanelaCarregamento", cargaJanelaCarregamento.Codigo)
                    .AddExtraData("CodigoCarga", codigoCarga)
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoFoiPossivelGerarOrdemDeColeta);

                return Arquivo(pdf, "application/pdf", "Ordem de Coleta " + cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoGerarOrdemDeColeta);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AuditarCargaTermoChegadaHorario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.RegistroNaoFoiEncontrado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, Localization.Resources.Logistica.JanelaCarregamentoTransportador.DeAcordoComComparecimentoNaDataDeSarregamentoSemAtrasoSujeitoMulta, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoAuditarTermoDeChegadaHorario);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarValidacaoCentroCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaCarregamento = Request.GetIntParam("CodigoJanelaCarregamento");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);

                if (cargaJanelaCarregamento?.CentroCarregamento?.ExigirConfirmacaoParticipacaoLeilao ?? false)
                    return new JsonpResult(new { MensagemConfirmacao = cargaJanelaCarregamento.CentroCarregamento.MensagemConfirmacaoLeilao });
                else
                    return new JsonpResult(false);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoAuditarTermoDeChegadaHorario);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarHorarioCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCargaJanelaCarregamento = Request.GetIntParam("CodigoCargaJanelaCarregamento");
                DateTime dataCarregamento = Request.GetDateTimeParam("DataCarregamento");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCargaJanelaCarregamento);

                if (cargaJanelaCarregamento == null)
                    throw new ControllerException("Carga não localizada.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cPedidos = repCargaPedido.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);
                DateTime? dataBaseCalculoLimiteCarregamento = (from o in cPedidos where o.Pedido.PrevisaoEntrega.HasValue select o.Pedido.PrevisaoEntrega).OrderBy(o => o).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork).BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                List<int> pedidosComNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork).BuscarListaPedidosComNotaPorPedidos((from o in cPedidos select o.Pedido.Codigo).ToList());
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> ceps = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork).BuscarPorCargaPedidos((from o in cPedidos select o.Codigo).ToList());

                DateTime? dataBaseCalculoLimiteCarregamentoSemNF = ceps.Where(o => o.CargaEntrega.DataAgendamento.HasValue
                                                                                && (!o.CargaPedido.Pedido?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.AgendamentoExigeNotaFiscal ?? false)
                                                                                && !pedidosComNota.Contains(o.CargaPedido.Pedido.Codigo))
                                                                        .OrderBy(o => o.CargaEntrega.DataAgendamento).Select(o => o.CargaEntrega?.DataAgendamento).FirstOrDefault();

                if (!dataBaseCalculoLimiteCarregamentoSemNF.HasValue)
                {
                    dataBaseCalculoLimiteCarregamentoSemNF = ceps.Where(o => (o.CargaPedido.Pedido?.DataAgendamento.HasValue ?? false)
                                                                                && !(o.CargaPedido.Pedido?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.AgendamentoExigeNotaFiscal ?? false)
                                                                                && !pedidosComNota.Contains(o.CargaPedido.Pedido.Codigo))
                                                                        .OrderBy(o => o.CargaPedido.Pedido.DataAgendamento).Select(o => o.CargaPedido.Pedido.DataAgendamento).FirstOrDefault();
                }

                if (dataBaseCalculoLimiteCarregamentoSemNF.HasValue)
                    dataBaseCalculoLimiteCarregamento = dataBaseCalculoLimiteCarregamentoSemNF;

                if (cargaJanelaCarregamento.CentroCarregamento?.ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido ?? false)
                    if (dataBaseCalculoLimiteCarregamento?.AddMinutes(-CalcularTempoLimiteCarregamentoEmMinutos(cargaJanelaCarregamento, unitOfWork)) < dataCarregamento)
                        return new JsonpResult(false, false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.HorarioCarregamentoUltrapassaHorarioLimite);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                cargaJanelaCarregamento.Initialize();

                AlocarHorarioCarregamento(cargaJanelaCarregamento, dataCarregamento, configuracaoEmbarcador, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AlteradoHorarioDeDescarregamentoPara, cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"))), unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(false, true, "Data de Agendamento de Entrega de todos os Pedidos e de Entrega alteradas com Sucesso!");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a Data de Carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarConfiguracoesCentroCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaCarregamento = Request.GetIntParam("CodigoJanelaCarregamento");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);

                bool permiteReenviarIntegracaoDocumentosRejeitados = false;
                if (cargaJanelaCarregamento.RecomendacaoGR.HasValue)
                    permiteReenviarIntegracaoDocumentosRejeitados = (cargaJanelaCarregamento.CentroCarregamento?.PermiteTransportadorEnviarIntegracaoGRDocumentosReprovados ?? false) && cargaJanelaCarregamento.RecomendacaoGR.Value == RecomendacaoGR.PendenciaDocumentos;

                var dynamic = new
                {
                    PermitirAlterarDataCarregamento = cargaJanelaCarregamento.CentroCarregamento?.PermitirQueTransportadorAltereHorarioDoCarregamento ?? false,
                    PermiteReenviarIntegracaoDocumentosRejeitados = permiteReenviarIntegracaoDocumentosRejeitados
                };

                return new JsonpResult(dynamic);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Centro de Carregamento não localizado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarLanceLimite()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);

                int codigoCarga = 0;
                decimal valor = 0;
                List<dynamic> cargas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("Cargas"));

                foreach (dynamic carga in cargas)
                {
                    if (((string)carga.Codigo).ToInt() > 0)
                        codigoCarga = ((string)carga.Codigo).ToInt();

                    if (((string)carga.Valor).ToDecimal() > 0)
                        valor = ((string)carga.Valor).ToDecimal();
                }

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                if (cargaJanelaCarregamento == null || cargaJanelaCarregamento.Carga.CustoAtualIntegracaoLeilao == 0)
                    return new JsonpResult(false);

                decimal valorLimiteParaCotacao = cargaJanelaCarregamento.Carga.CustoAtualIntegracaoLeilao;

                if (valor > valorLimiteParaCotacao)
                    return new JsonpResult(true, "Valor de frete informado será contabilizado, porém está acima do valor limite para este leilão. Deseja confirmar?");

                return new JsonpResult(false);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoInformarInteresse);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterInformacaoExigirInformarLacre()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaCarregamento = Request.GetIntParam("CodigoJanelaCarregamento");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);

                var retorno = new
                {
                    ExigirInformacaoLacreJanelaCarregamento = cargaJanelaCarregamento?.Carga?.ModeloVeicularCarga?.ExigirInformacaoLacreJanelaCarregamentoPortalTransportador ?? false
                };

                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarDocumentosReprovados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(unitOfWork);
                Servicos.Embarcador.Integracao.Klios.IntegracaoKlios servicoIntegracaoKlios = new Servicos.Embarcador.Integracao.Klios.IntegracaoKlios(unitOfWork, TipoServicoMultisoftware);
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao = repositorioJanelaCarregamentoIntegracao.BuscarPorCarga(codigoCarga);

                if (janelaCarregamentoIntegracao == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                servicoIntegracaoKlios.EnviarDocumentosReprovadosIntegracao(janelaCarregamentoIntegracao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, janelaCarregamentoIntegracao, Localization.Resources.Logistica.JanelaCarregamentoTransportador.SolicitadoReenvioDaIntegracaoPeloTranspotador, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuFalhaAoReenviarIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarRegistroVisualizacaoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoJanelaCarregamentoTransportador = Request.GetIntParam("CodigoJanelaCarregamentoTransportador");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repositorioCargaJanelaCarregamentoTransportadorHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(codigoJanelaCarregamentoTransportador);

                unitOfWork.Start();

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);

                if (!repositorioCargaJanelaCarregamentoTransportadorHistorico.ExistePorCargaJanelaCarregamentoTransportadorETipo(codigoJanelaCarregamentoTransportador, TipoCargaJanelaCarregamentoTransportadorHistorico.VisualizouCarga))
                    servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, string.Concat("Usuário ", Usuario.Nome, " visualizou dados da carga"), TipoCargaJanelaCarregamentoTransportadorHistorico.VisualizouCarga, null, null, Usuario);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoSalvarAsInformacoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AlocarHorarioCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime novoHorario, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
            {
                BloquearJanelaCarregamentoExcedente = true,
                PermitirHorarioCarregamentoComLimiteAtingido = true
            };
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoEmbarcador, Auditado, configuracaoDisponibilidadeCarregamento);

            servicoDisponibilidadeCarregamento.ValidarPermissaoAlterarHorarioCarregamento(cargaJanelaCarregamento);
            servicoDisponibilidadeCarregamento.AlterarHorarioCarregamento(cargaJanelaCarregamento, novoHorario, TipoServicoMultisoftware);
        }

        private Dominio.Entidades.Empresa ObterEmpresaSelecionada(int codigoCarga, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);
            if (cargaJanelaCarregamento == null)
                throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.RegistroNaoFoiEncontrado);

            if (!(cargaJanelaCarregamento.CentroCarregamento?.PermitirMatrizSelecionarFilial ?? false) || codigoEmpresa == 0 || Usuario.Empresa.Matriz.Any())
                return Usuario.Empresa;

            return Usuario.Empresa.Filiais.Where(o => o.Codigo == codigoEmpresa).FirstOrDefault() ?? Usuario.Empresa;
        }

        private void AtualizarCargarNaoDestinadasAoTransportador(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargas, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
            var cargaJanelaCarregamentoTransportadors = (
                from o in cargas
                where o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.ComInteresse && o.CargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador
                select o
            ).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargaJanelaCarregamentoTransportadors)
            {
                cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga = null;
                cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Rejeitada;
                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamentoTransportador);
                servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaRejeitadaParaTransportador);
            }
        }

        private void AtualizarCargaLacres(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic cargaLacres = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CargaLacres"));

            if ((carga?.ModeloVeicularCarga?.ExigirInformacaoLacreJanelaCarregamentoPortalTransportador ?? false) && (cargaLacres?.Count == 0))
                throw new ControllerException("O Modelo Veicular da Carga exige informação do Lacre.");

            ExcluirCargaLacresRemovidas(carga, cargaLacres, unitOfWork);
            InserirCargaLacresAdicionadas(carga, cargaLacres, unitOfWork);
        }

        private void AtualizarVeiculoContainer(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Veiculo reboque, Dominio.Entidades.Veiculo segundoReboque, bool possuiGenset, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo, Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo, Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaVeiculoContainer repositorioCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaVeiculoContainerAnexo repositorioCargaVeiculoContainerAnexo = new Repositorio.Embarcador.Cargas.CargaVeiculoContainerAnexo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> veiculosContainer = repositorioCargaVeiculoContainer.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> veiculosContainerAdicionadosOuAtualizados = new List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>();

            bool permitirAtualizarContainerVeiculo = (
                configuracaoEmbarcador.PermitirInformarAnexoContainerCarga ||
                configuracaoEmbarcador.PermitirInformarDataRetiradaCtrnCarga ||
                configuracaoEmbarcador.PermitirInformarNumeroContainerCarga ||
                configuracaoEmbarcador.PermitirInformarTaraContainerCarga ||
                configuracaoEmbarcador.PermitirInformarMaxGrossCarga ||
                possuiGenset
            );

            if (permitirAtualizarContainerVeiculo)
            {
                bool exigirConfirmacaoTracao = carga?.TipoOperacao?.ExigePlacaTracao ?? false;
                bool validarDataRetiradaCtrn = configuracaoEmbarcador.PermitirInformarDataRetiradaCtrnCarga && (carga.TipoOperacao?.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador ?? false);
                bool validarMaxGross = configuracaoEmbarcador.PermitirInformarMaxGrossCarga && (carga.TipoOperacao?.ExigirMaxGrossJanelaCarregamentoTransportador ?? false);
                bool validarNumeroContainer = configuracaoEmbarcador.PermitirInformarNumeroContainerCarga && (carga.TipoOperacao?.ExigirNumeroContainerJanelaCarregamentoTransportador ?? false);
                bool validarTaraContainer = configuracaoEmbarcador.PermitirInformarTaraContainerCarga && (carga.TipoOperacao?.ExigirTaraContainerJanelaCarregamentoTransportador ?? false);

                if (veiculo != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerVeiculo = (from o in veiculosContainer where o.Veiculo.Codigo == veiculo.Codigo select o).FirstOrDefault();

                    if (containerVeiculo == null)
                        containerVeiculo = new Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer();

                    containerVeiculo.Carga = carga;
                    containerVeiculo.Veiculo = veiculo;
                    containerVeiculo.DataRetiradaCtrn = Request.GetNullableDateTimeParam("DataRetiradaCtrnVeiculo");
                    containerVeiculo.Genset = Request.GetStringParam("GensetVeiculo");
                    containerVeiculo.MaxGross = Request.GetIntParam("MaxGrossVeiculo");
                    containerVeiculo.NumeroContainer = Request.GetStringParam("NumeroContainerVeiculo");
                    containerVeiculo.NumeroReboque = NumeroReboque.SemReboque;
                    containerVeiculo.TaraContainer = Request.GetIntParam("TaraContainerVeiculo");

                    if (!exigirConfirmacaoTracao)
                    {
                        if (validarDataRetiradaCtrn && !containerVeiculo.DataRetiradaCtrn.HasValue)
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarDataDeRetiradaCTRNDoVeiculo);

                        if (validarMaxGross && (containerVeiculo.MaxGross <= 0))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarMaxGrossDoVeiculo);

                        if (validarNumeroContainer && string.IsNullOrWhiteSpace(containerVeiculo.NumeroContainer))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarNumeroDoContainerDoVeículo);

                        if (validarTaraContainer && (containerVeiculo.TaraContainer <= 0))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarNumeroDoContainerDoVeículo);

                        if (possuiGenset && string.IsNullOrWhiteSpace(containerVeiculo.Genset))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarGensetDoVeículo);
                    }

                    if (containerVeiculo.Codigo > 0)
                        repositorioCargaVeiculoContainer.Atualizar(containerVeiculo);
                    else
                        repositorioCargaVeiculoContainer.Inserir(containerVeiculo);

                    veiculosContainerAdicionadosOuAtualizados.Add(containerVeiculo);
                }

                if (reboque != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerReboque = (from o in veiculosContainer where o.Veiculo.Codigo == reboque.Codigo select o).FirstOrDefault();

                    if (containerReboque == null)
                        containerReboque = new Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer();

                    containerReboque.Carga = carga;
                    containerReboque.Veiculo = reboque;
                    containerReboque.DataRetiradaCtrn = Request.GetNullableDateTimeParam("DataRetiradaCtrnReboque");
                    containerReboque.Genset = Request.GetStringParam("GensetReboque");
                    containerReboque.MaxGross = Request.GetIntParam("MaxGrossReboque");
                    containerReboque.NumeroContainer = Request.GetStringParam("NumeroContainerReboque");
                    containerReboque.NumeroReboque = NumeroReboque.ReboqueUm;
                    containerReboque.TaraContainer = Request.GetIntParam("TaraContainerReboque");

                    if (exigirConfirmacaoTracao)
                    {
                        if (validarDataRetiradaCtrn && !containerReboque.DataRetiradaCtrn.HasValue)
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarDataDeRetiradaCTRNDoReboque);

                        if (validarMaxGross && (containerReboque.MaxGross <= 0))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarMaxGrossDoReboque);

                        if (validarNumeroContainer && string.IsNullOrWhiteSpace(containerReboque.NumeroContainer))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarNumeroDoContainerDoReboque);

                        if (validarTaraContainer && (containerReboque.TaraContainer <= 0))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarTaraDoContainerDoReboque);

                        if (possuiGenset && string.IsNullOrWhiteSpace(containerReboque.Genset))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarGensetDoReboque);
                    }

                    if (containerReboque.Codigo > 0)
                        repositorioCargaVeiculoContainer.Atualizar(containerReboque);
                    else
                        repositorioCargaVeiculoContainer.Inserir(containerReboque);

                    veiculosContainerAdicionadosOuAtualizados.Add(containerReboque);
                }

                if (segundoReboque != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerSegundoReboque = (from o in veiculosContainer where o.Veiculo.Codigo == segundoReboque.Codigo select o).FirstOrDefault();

                    if (containerSegundoReboque == null)
                        containerSegundoReboque = new Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer();

                    containerSegundoReboque.Carga = carga;
                    containerSegundoReboque.Veiculo = segundoReboque;
                    containerSegundoReboque.DataRetiradaCtrn = Request.GetNullableDateTimeParam("DataRetiradaCtrnSegundoReboque");
                    containerSegundoReboque.Genset = Request.GetStringParam("GensetSegundoReboque");
                    containerSegundoReboque.MaxGross = Request.GetIntParam("MaxGrossSegundoReboque");
                    containerSegundoReboque.NumeroContainer = Request.GetStringParam("NumeroContainerSegundoReboque");
                    containerSegundoReboque.NumeroReboque = NumeroReboque.ReboqueDois;
                    containerSegundoReboque.TaraContainer = Request.GetIntParam("TaraContainerSegundoReboque");

                    if (exigirConfirmacaoTracao)
                    {
                        if (validarDataRetiradaCtrn && !containerSegundoReboque.DataRetiradaCtrn.HasValue)
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarDataDeRetiradaCTRNDoSegundoReboque);

                        if (validarMaxGross && (containerSegundoReboque.MaxGross <= 0))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarMaxGrossDoSegundoReboque);

                        if (validarNumeroContainer && string.IsNullOrWhiteSpace(containerSegundoReboque.NumeroContainer))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarNumeroDoContainerDoSegundoReboque);

                        if (validarTaraContainer && (containerSegundoReboque.TaraContainer <= 0))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarTaraDoContainerDoSegundoReboque);

                        if (possuiGenset && string.IsNullOrWhiteSpace(containerSegundoReboque.Genset))
                            throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarGensetDoSegundoReboque);
                    }

                    if (containerSegundoReboque.Codigo > 0)
                        repositorioCargaVeiculoContainer.Atualizar(containerSegundoReboque);
                    else
                        repositorioCargaVeiculoContainer.Inserir(containerSegundoReboque);

                    veiculosContainerAdicionadosOuAtualizados.Add(containerSegundoReboque);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer veiculoContainer in veiculosContainer)
            {
                if ((from o in veiculosContainerAdicionadosOuAtualizados where o.Codigo == veiculoContainer.Codigo select o).Any())
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer veiculoContainerNovo = (from o in veiculosContainerAdicionadosOuAtualizados where o.NumeroReboque == veiculoContainer.NumeroReboque select o).FirstOrDefault();
                if (veiculoContainerNovo != null)
                    servicoAnexo.TrocarAnexos(veiculoContainer, veiculoContainerNovo);
                else
                    servicoAnexo.ExcluirAnexos(veiculoContainer);

                repositorioCargaVeiculoContainer.Deletar(veiculoContainer);
            }
        }

        private string BuscarReboquesComModeloVeicular(Dominio.Entidades.Veiculo veiculo)
        {
            string conjunto = veiculo.Placa;

            if (veiculo.TipoVeiculo == "0")
            {
                foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                {
                    conjunto += ", " + reboque.Placa;
                }
            }
            else
            {
                if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                {
                    Dominio.Entidades.Veiculo tracao = veiculo.VeiculosTracao.FirstOrDefault();
                    foreach (Dominio.Entidades.Veiculo reboque in tracao.VeiculosTracao)
                    {
                        conjunto += ", " + reboque.Placa;
                    }
                }
            }

            return conjunto;
        }

        private void EnviarNotificacoesEmailInteresseCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);

            cargasJanelaCarregamentoTransportador = cargasJanelaCarregamentoTransportador
                .Where(obj => obj.CargaJanelaCarregamento.CentroCarregamento != null)
                .Where(obj => obj.CargaJanelaCarregamento.CentroCarregamento.UsuariosNotificacao != null || obj.CargaJanelaCarregamento.CentroCarregamento.Emails != null)
                .Where(obj => obj.CargaJanelaCarregamento.CentroCarregamento.EnviarNotificacoesPorEmail).Distinct().ToList();

            List<Dominio.Entidades.Usuario> usuariosNotificacao = new List<Dominio.Entidades.Usuario>();
            List<string> emailsNotificacao = new List<string>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
            {
                if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.UsuariosNotificacao?.Count > 0)
                    usuariosNotificacao.AddRange(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.UsuariosNotificacao.ToList());
                else if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador != null)
                    usuariosNotificacao.Add(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador);

                if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.Emails?.Count > 0)
                    emailsNotificacao.AddRange(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.Emails.Select(obj => obj.Email).ToList());
            }

            if (cargaJanelaCarregamentoTransportadorReferencia == null || cargaJanelaCarregamentoTransportadorReferencia.Tipo != TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo)
                return;

            string numeroCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork).ObterNumeroCarga(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga, ConfiguracaoEmbarcador);
            StringBuilder mensagemNotificacao = new StringBuilder(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EstaInteressadoEmFazerCarga, Usuario.Empresa.RazaoSocial, numeroCarga));

            if (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CargaLiberadaCotacao)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> componentesFrete = repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.BuscarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportadorReferencia.Codigo);

                mensagemNotificacao.AppendLine();
                mensagemNotificacao.AppendLine();

                if (cargaJanelaCarregamentoTransportadorReferencia.HorarioCarregamento.HasValue)
                    mensagemNotificacao.AppendLine($"Data: {cargaJanelaCarregamentoTransportadorReferencia.HorarioCarregamento.Value.ToDateTimeString()}");

                if (cargaJanelaCarregamentoTransportadorReferencia.ValorFreteTransportador > 0m)
                    mensagemNotificacao.AppendLine(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorFrete, cargaJanelaCarregamentoTransportadorReferencia.ValorFreteTransportador.ToString("n2")));

                if (componentesFrete.Count > 0)
                {
                    decimal valorComponentesFrete = 0m;

                    foreach (var componenteFrete in componentesFrete)
                    {
                        if (componenteFrete.TipoValor == TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal)
                            mensagemNotificacao.AppendLine($"{componenteFrete.DescricaoComponente}: R$ {componenteFrete.ValorComponente.ToString("n2")}");
                        else
                            mensagemNotificacao.AppendLine($"{componenteFrete.DescricaoComponente}: {componenteFrete.Percentual.ToString("n2")}%");

                        valorComponentesFrete += componenteFrete.ValorComponente;
                    }

                    mensagemNotificacao.AppendLine($"Valor Total: R$ {(cargaJanelaCarregamentoTransportadorReferencia.ValorFreteTransportador + valorComponentesFrete).ToString("n2")}");
                }
            }

            servicoCargaJanelaCarregamentoTransportador.NotificaUsuarios(cargaJanelaCarregamentoTransportadorReferencia, usuariosNotificacao, mensagemNotificacao.ToString(), Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);
            servicoCargaJanelaCarregamentoTransportador.NotificaEmails(cargaJanelaCarregamentoTransportadorReferencia, emailsNotificacao, mensagemNotificacao.ToString(), unitOfWork);
        }

        private void ExcluirCargaLacresRemovidas(Dominio.Entidades.Embarcador.Cargas.Carga carga, dynamic cargaLacres, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Lacres?.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var cargaLacre in cargaLacres)
                {
                    int? codigo = ((string)cargaLacre.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> listaCargaLacresRemover = (from cargaLacre in carga.Lacres where !listaCodigosAtualizados.Contains(cargaLacre.Codigo) select cargaLacre).ToList();

                foreach (var cargaLacre in listaCargaLacresRemover)
                {
                    repositorioCargaLacre.Deletar(cargaLacre);
                }

                if (listaCargaLacresRemover.Count > 0)
                {
                    string descricaoAcao = listaCargaLacresRemover.Count == 1 ? Localization.Resources.Logistica.JanelaCarregamentoTransportador.LacreRemovido : Localization.Resources.Logistica.JanelaCarregamentoTransportador.MultiplosLacresRemovidos;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoTransportador ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            bool dadosCargaVinculada = Request.GetBoolParam("DadosCargaVinculada");
            List<int> codigoCargasVinculadas = null;

            if (dadosCargaVinculada)
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.CargaVinculada repositorioCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(unitOfWork);

                codigoCargasVinculadas = repositorioCargaVinculada.BuscarCodigosCargasPorCarga(Request.GetIntParam("Carga"));
            }

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoTransportador filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoTransportador()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoRota = Request.GetIntParam("Rota"),
                CodigosCargasVinculadas = codigoCargasVinculadas,
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                ExibirCargaSemValorFrete = ConfiguracaoEmbarcador.ExibirCargaSemValorFreteJanelaCarregamentoTransportador,
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                Situacao = Request.GetNullableEnumParam<SituacaoCargaJanelaCarregamentoTransportador>("Situacao"),
                TipoLiberacao = Request.GetNullableEnumParam<TipoLiberacaoCargaJanelaCarregamento>("TipoLiberacao"),
                NumeroExp = Request.GetStringParam("NumeroExp"),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigosFiliais = Request.GetListParam<int>("Filiais"),
            };


            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
            {
                filtrosPesquisa.CodigoClienteTerceiro = Usuario?.ClienteTerceiro?.Codigo ?? 0;
                filtrosPesquisa.NaoRetornarCargasMarcadoSemInteresse = true;
            }
            else
                filtrosPesquisa.CodigoTransportador = Usuario.Empresa.Codigo;

            return filtrosPesquisa;
        }

        private void InformarInteresseCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, string horarioCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unidadeDeTrabalho, ConfiguracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unidadeDeTrabalho, ConfiguracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro(unidadeDeTrabalho, ConfiguracaoEmbarcador);
            Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes servicoJanelaCarregamentoTransportadorValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes(unidadeDeTrabalho, TipoServicoMultisoftware, ConfiguracaoEmbarcador);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga;

            if (carga.CargaAgrupamento == null)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    servicoCargaJanelaCarregamentoTransportadorTerceiro.ValidarPermissaoMarcarInteresseCarga(Usuario?.ClienteTerceiro, carga);
                else
                    servicoCargaJanelaCarregamentoTransportador.ValidarPermissaoMarcarInteresseCarga(Usuario.Empresa, carga);

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelMarcarInteresseNaAtualSituacaoDaCarga, carga.DescricaoSituacaoCarga));

                if (cargaJanelaCarregamentoTransportador.FreteCalculadoComProblemas)
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.SemValorDeFreteVerifiqueComEmbarcador);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    if (cargaJanelaCarregamentoTransportador.PendenteCalcularFrete)
                        servicoCargaJanelaCarregamentoTransportador.CalcularFreteParaTransportador(cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware);

                servicoJanelaCarregamentoTransportadorValidacoes.ValidarVeiculosPorCliente(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, Usuario.Empresa);
            }

            cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.ComInteresse;
            cargaJanelaCarregamentoTransportador.UsuarioResponsavel = this.Usuario;
            cargaJanelaCarregamentoTransportador.DataInteresse = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(horarioCarregamento))
            {
                bool CarregamentoProximoDia = Request.GetBoolParam("CarregamentoProximoDia");
                DateTime data = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.InicioCarregamento;
                if (CarregamentoProximoDia)
                    data = RetornarProximoDiaUtilDeFuncionamentoDoCentro(cargaJanelaCarregamentoTransportador, unidadeDeTrabalho);

                TimeSpan hora = TimeSpan.ParseExact(horarioCarregamento, "g", null, System.Globalization.TimeSpanStyles.None);

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = RetornarPeriodosDisponiveis(cargaJanelaCarregamentoTransportador, data, unidadeDeTrabalho);
                if (periodosCarregamento.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosOrdenados = periodosCarregamento.OrderBy(obj => obj.HoraInicio).ToList();
                    bool podeInformar = false;
                    string strPeriodos = Localization.Resources.Logistica.JanelaCarregamentoTransportador.OHorarioInformadoParaCarregamentoNaoEstaDisponivelPorFavorInformeUmHorarioConformeOrientacao;
                    foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento in periodosOrdenados)
                    {
                        if (hora < periodoCarregamento.HoraTermino && hora >= periodoCarregamento.HoraInicio)
                        {
                            podeInformar = true;
                            break;
                        }
                        else
                        {
                            strPeriodos += ", das " + periodoCarregamento.HoraInicio.ToString(@"hh\:mm") + " até " + periodoCarregamento.HoraTermino.ToString(@"hh\:mm");
                        }
                    }
                    if (podeInformar)
                    {
                        cargaJanelaCarregamentoTransportador.HorarioCarregamento = new DateTime(data.Year, data.Month, data.Day, hora.Hours, hora.Minutes, hora.Seconds);
                    }
                    else
                    {
                        strPeriodos += ".";
                        throw new ControllerException(strPeriodos);
                    }

                }
                else
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoExisteUmPeriodoDeCarregamentoDisponivelParaDiaSelecionado);
            }
            else
                cargaJanelaCarregamentoTransportador.HorarioCarregamento = null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
            {
                if ((carga.CargaAgrupamento == null) && ((carga.Terceiro != null) || (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador) || cargaJanelaCarregamentoTransportador.Bloqueada))
                    throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EstaCargaNaoEstaMaisDisponivelParaMarcarInteresse, carga.CodigoCargaEmbarcador));
            }
            else
            {
                if ((carga.CargaAgrupamento == null) && ((carga.Empresa != null) || (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador) || cargaJanelaCarregamentoTransportador.Bloqueada))
                    throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EstaCargaNaoEstaMaisDisponivelParaMarcarInteresse, carga.CodigoCargaEmbarcador));
            }

            if (cargaJanelaCarregamentoTransportador.Tipo == TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo)
            {
                carga.DataAtualizacaoCarga = DateTime.Now;
                carga.Empresa = Usuario.Empresa;
                carga.RejeitadaPeloTransportador = false;
                cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao;

                repositorioCarga.Atualizar(carga);
                repositorioCargaJanelaCarregamentoTransportador.BloquearTodasPorPrioridadeRotaGrupo(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo, cargaJanelaCarregamentoTransportador.Codigo);
                servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, TipoServicoMultisoftware);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    servicoCargaJanelaCarregamentoTransportadorTerceiro.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorMarcouInteresseNaCargaAguardandoConfirmacaoDosDadosDeTransporte, Usuario);
                else
                    servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorMarcouInteresseNaCargaAguardandoConfirmacaoDosDadosDeTransporte, Usuario);

                if (carga.CargaAgrupamento == null)
                    new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unidadeDeTrabalho).EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);
            }
            else
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                servicoCargaJanelaCarregamentoTransportadorTerceiro.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorMarcouInteresseNaCargaAguardandoConfirmacaoDosDadosDeTransporte, Usuario);
            else
                servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorMarcouInteresseNaCarga, Usuario);

            repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamentoTransportador, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.InformouInteresseNaCarga, unidadeDeTrabalho);
        }

        private void InserirCargaLacresAdicionadas(Dominio.Entidades.Embarcador.Cargas.Carga carga, dynamic cargaLacres, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
            int totalCargaLacresAdicionados = 0;

            foreach (var cargaLacre in cargaLacres)
            {
                int? codigo = ((string)cargaLacre.Codigo).ToNullableInt();

                if (!codigo.HasValue)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacreAdicionar = new Dominio.Entidades.Embarcador.Cargas.CargaLacre()
                    {
                        Carga = carga,
                        Numero = ((string)cargaLacre.Numero).Trim()
                    };

                    repositorioCargaLacre.Inserir(cargaLacreAdicionar);

                    totalCargaLacresAdicionados++;
                }
            }

            if (totalCargaLacresAdicionados > 0)
            {
                string descricaoAcao = totalCargaLacresAdicionados == 1 ? Localization.Resources.Logistica.JanelaCarregamentoTransportador.LacreAdicionado : Localization.Resources.Logistica.JanelaCarregamentoTransportador.MultiploslacresAdicionados;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private dynamic ObterDadosCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento;
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoDados> dadosDescarregamentos = repositorioCargaJanelaCarregamento.BuscarDadosDescarregamentosPorCarga(cargaJanelaCarregamento.Carga.Codigo);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite repositorioJanelaCarregamentoTermoAceite = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite(unitOfWork);

            if (cargaJanelaCarregamento.CentroCarregamento?.ExibirNotasFiscaisJanelaCarregamentoTransportador ?? false)
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                List<int> codigosCargaPedido = cargaJanelaCarregamento.Carga.Pedidos?.Select(o => o.Codigo)?.ToList() ?? new List<int>();
                notasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedidosComFetchCargaPedido(codigosCargaPedido);
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite> termosAceite = repositorioJanelaCarregamentoTermoAceite.BuscarPorJanelasTransportador(new List<int> { cargaJanelaCarregamentoTransportador.Codigo });

            return ObterDadosCarga(cargaJanelaCarregamentoTransportador, (from o in notasFiscais where o.CargaPedido.Carga.Codigo == cargaJanelaCarregamento.Carga.Codigo select o).ToList(), dadosDescarregamentos, termosAceite, unitOfWork);
        }

        private dynamic ObterDadosCargas(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite repositorioJanelaCarregamentoTermoAceite = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite(unitOfWork);

            var retorno = new List<dynamic>();
            var codigosCargas = (from o in cargas select o.CargaJanelaCarregamento.Carga.Codigo).ToList();
            var notasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var dadosDescarregamentos = repositorioCargaJanelaCarregamento.BuscarDadosDescarregamentosPorCargas(codigosCargas);

            if (cargas.Any(obj => obj.CargaJanelaCarregamento.CentroCarregamento?.ExibirNotasFiscaisJanelaCarregamentoTransportador ?? false))
            {
                var repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                var codigosCargaPedido = cargas.Where(obj => obj.CargaJanelaCarregamento.CentroCarregamento?.ExibirNotasFiscaisJanelaCarregamentoTransportador
                    ?? false).SelectMany(obj => obj.CargaJanelaCarregamento.Carga.Pedidos).Select(cp => cp.Codigo).ToList();

                notasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedidosComFetchCargaPedido(codigosCargaPedido);
            }

            var termosAceite = repositorioJanelaCarregamentoTermoAceite.BuscarPorJanelasTransportador(cargas.Select(obj => obj.Codigo).ToList());

            foreach (var cargaJanelaCarregamentoTransportador in cargas)
                retorno.Add(ObterDadosCarga(cargaJanelaCarregamentoTransportador, (from o in notasFiscais where o.CargaPedido.Carga.Codigo == cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo select o).ToList(), dadosDescarregamentos, (from o in termosAceite where o.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador.Codigo select o).ToList(), unitOfWork));

            return retorno;
        }

        private dynamic ObterDadosCargas(List<CargaJanelaCarregamentoTransportador> cargas, CargaJanelaCarregamentoTransportador cargaReferencia, UnitOfWork unitOfWork)
        {
            cargas.Remove(cargaReferencia);
            cargas.Insert(0, cargaReferencia);

            return ObterDadosCargas(cargas, unitOfWork);
        }

        private dynamic ObterDadosCarga(CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais, List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoDados> dadosDescarregamentos, List<CargaJanelaCarregamentoTransportadorTermoAceite> termosAceite, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamento.Carga;
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFreteDinamicos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            decimal valorFreteAPagar = carga.ValorFreteAPagar;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
            bool naoGerarAgendamentoColeta = repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete janelaComMenorLance = null;
            if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                janelaComMenorLance = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargaJanelaCarregamentoTransportadorTerceiroComMenorLance(cargaJanelaCarregamento, considerarCargasVinculadas: false);
            else
                janelaComMenorLance = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargaJanelaCarregamentoTransportadorComMenorLance(cargaJanelaCarregamento, considerarCargasVinculadas: false);

            Repositorio.Embarcador.Pedidos.PedidoStage repositorioPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);

            int posicao = ObterPosicaoLeilao(cargaJanelaCarregamento, unitOfWork);

            if (valorFreteAPagar > 0)
            {
                Servicos.Embarcador.Frete.BonificacaoTransportador servicoBonificacaoTransportador = new Servicos.Embarcador.Frete.BonificacaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacao = (carga.Empresa == null) ? servicoBonificacaoTransportador.ObterBonificacao(carga, Usuario.Empresa) : null;
                Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteBonus = null;

                if (bonificacao != null)
                {
                    componenteBonus = new Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico
                    {
                        ValorComponente = Math.Round(carga.ValorFrete * (bonificacao.PercentualAplicar / 100), 2, MidpointRounding.AwayFromZero),
                        ComponenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete
                        {
                            Descricao = bonificacao.ComponenteFrete?.Descricao ?? string.Empty
                        }
                    };

                    valorFreteAPagar += componenteBonus.ValorComponente;
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponente in carga.Componentes.ToList())
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componente = new Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico
                    {
                        ComponenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete
                        {
                            Descricao = cargaComponente.ComponenteFrete != null ? cargaComponente.ComponenteFrete.Descricao : cargaComponente.DescricaoComponente
                        },
                        ValorComponente = cargaComponente.ValorComponente
                    };

                    if ((cargaComponente.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS) && (componente.ValorComponente > 0))
                    {
                        decimal resto = componente.ValorComponente / carga.ValorFreteAPagar;

                        componente.ValorComponente = Math.Round(resto * valorFreteAPagar, 2, MidpointRounding.AwayFromZero);
                    }

                    componentesFreteDinamicos.Add(componente);
                }

                if (componenteBonus != null)
                    componentesFreteDinamicos.Add(componenteBonus);
            }

            DateTime? dataPrevisaoEntrega = (from o in carga.Pedidos where o.Pedido.PrevisaoEntrega.HasValue select o.Pedido.PrevisaoEntrega).FirstOrDefault();
            string ordem = string.Join(", ", (from o in carga.Pedidos where !string.IsNullOrWhiteSpace(o.Pedido.Ordem) select o.Pedido.Ordem));
            string numeroPedidos = string.Join(", ", (from o in carga.Pedidos select o.Pedido.NumeroPedidoEmbarcador));
            DateTime? previsaoTerminoCarregamento = carga.Pedidos.Where(o => o.Pedido.DataPrevisaoTerminoCarregamento.HasValue).OrderBy(o => o.Pedido.DataPrevisaoTerminoCarregamento).Select(o => o.Pedido.DataPrevisaoTerminoCarregamento).FirstOrDefault();
            string numeroExps = string.Join(", ", (from o in carga.Pedidos where !string.IsNullOrWhiteSpace(o.Pedido.NumeroEXP) select o.Pedido.NumeroEXP));
            string numeroNotasFiscais = string.Join(", ", (from o in notasFiscais select o.XMLNotaFiscal.Numero));
            System.Text.StringBuilder destino = new System.Text.StringBuilder();
            decimal pesoCubado = carga.Pedidos.Sum(obj => obj.Pedido.CubagemTotal);
            int volumesPedidos = carga.Pedidos.Sum(obj => obj.QtVolumes);
            string enderecoDestinatarios = string.Join(", ", (from o in carga.Pedidos select o.Pedido.Destinatario.Endereco).Distinct());
            string canalEntrega = string.Join(", ", repositorioPedidoStage.BuscarPorListaPedidos(carga.Pedidos.Select(p => p.Pedido?.Codigo ?? 0).ToList()).Select(s => s?.CanalEntrega?.Descricao)) ?? string.Empty;
            string canalVenda = string.Join(", ", repositorioPedidoStage.BuscarPorListaPedidos(carga.Pedidos.Select(p => p.Pedido?.Codigo ?? 0).ToList()).Select(s => s?.CanalVenda?.Descricao)) ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(carga.DadosSumarizados?.Destinos))
                destino.Append(carga.DadosSumarizados.Destinos);

            if (!string.IsNullOrWhiteSpace(carga.ObservacaoLocalEntrega))
                destino.Append($" ({carga.ObservacaoLocalEntrega})");

            Repositorio.Embarcador.Cargas.CargaVeiculoContainer repCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaVinculada repCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLicenca repCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = naoGerarAgendamentoColeta ? null : repAgendamentoColeta.BuscarPorCarga(carga.Codigo);
            Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repositorioSimulacaoFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> cargaVeiculosContainer = repCargaVeiculoContainer.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasVinculadas = repCargaVinculada.BuscarCargasPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoDados> dadosDescarregamentosPorCarga = (from o in dadosDescarregamentos where o.CodigoCarga == carga.Codigo select o).ToList();
            Dominio.Entidades.Veiculo veiculo = cargaJanelaCarregamento.Carga.Veiculo;
            Dominio.Entidades.Veiculo reboque = cargaJanelaCarregamento.Carga.VeiculosVinculados?.ElementAtOrDefault(0);
            Dominio.Entidades.Veiculo segundoReboque = cargaJanelaCarregamento.Carga.VeiculosVinculados?.ElementAtOrDefault(1);
            Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerVeiculo = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == veiculo?.Codigo select cargaVeiculoContainer).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == reboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer containerSegundoReboque = (from cargaVeiculoContainer in cargaVeiculosContainer where cargaVeiculoContainer.Veiculo.Codigo == segundoReboque?.Codigo select cargaVeiculoContainer).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = repositorioSimulacaoFrete.BuscarPorCarregamento(carga.Carregamento?.Codigo ?? 0);

            bool lembrarEspelhamento = ConfiguracaoEmbarcador.PossuiMonitoramento && veiculo != null && veiculo.PossuiRastreador;
            bool aguardandoAceiteOuConfirmacao = cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite || cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao;
            bool permitirSelecionarHorariosCarregamentoEDescarregamento = (cargaJanelaCarregamento.CentroCarregamento?.PermiteTransportadorSelecionarHorarioCarregamento ?? false);
            bool exibirFilial = cargaJanelaCarregamento.CentroCarregamento?.ExibirFilialJanelaCarregamentoTransportador ?? false;
            bool exibirMenorLance = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento?.CentroCarregamento?.PermiteTransportadorVisualizarMenorLanceLeilao ?? false;

            TimeSpan? tempoDataColeta = (agendamento != null && agendamento.DataColeta.HasValue && aguardandoAceiteOuConfirmacao) ? agendamento.DataColeta.Value - DateTime.Now : (TimeSpan?)null;

            string volumes = string.Empty;
            if (agendamento != null && agendamento?.Volumes > 0)
                volumes = agendamento.Volumes.ToString();
            else if (volumesPedidos > 0)
                volumes = volumesPedidos.ToString();

            string observacao = (cargaJanelaCarregamento.ObservacaoTransportador == "" ? cargaJanelaCarregamento.Carga.ObservacaoTransportador : cargaJanelaCarregamento.ObservacaoTransportador) ?? string.Empty;
            string observacaoAdicionalPedido = string.Join(", ", (from o in carga.Pedidos where !string.IsNullOrWhiteSpace(o.Pedido?.ObservacaoAdicional) select o.Pedido?.ObservacaoAdicional));

            observacao += observacaoAdicionalPedido ?? "";

            string termoAceite = "";
            bool exigeTermoAceite = false;

            if ((cargaJanelaCarregamento.CargaBase?.TipoOperacao?.ExigirTermoAceite ?? false) || (cargaJanelaCarregamento.CentroCarregamento?.ExigirTermoAceiteTransporte ?? false))
                exigeTermoAceite = true;

            if (!string.IsNullOrWhiteSpace(cargaJanelaCarregamento.CargaBase?.TipoOperacao?.TermoAceite))
                termoAceite = cargaJanelaCarregamento.CargaBase.TipoOperacao.TermoAceite;
            else if (!string.IsNullOrWhiteSpace(cargaJanelaCarregamento.CentroCarregamento?.TermoAceite))
                termoAceite = cargaJanelaCarregamento.CentroCarregamento.TermoAceite;

            string valorTransportadorInteressadoSinalizado = ObterValorTransportadorInteressadoSinalizado(cargaJanelaCarregamentoTransportador, cargaJanelaCarregamento);

            return new
            {
                cargaJanelaCarregamento.Codigo,
                CodigoJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador.Codigo,
                AguardandoAutorizacaoFrete = cargaJanelaCarregamento.Carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.AguardandoAprovacao,
                CamposVisiveis = cargaJanelaCarregamento.CentroCarregamento?.CamposVisiveisTransportador ?? "1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;21;22;23;24;25;26;27;28;29;30",
                Carga = new
                {
                    carga.Codigo,
                    Numero = carga.CodigoCargaEmbarcador,
                    PermiteAvancarEtapaEmissao = PermiteAvancarEtapaEmissao(carga),
                    CargaPerigosa = agendamento != null ? (agendamento.CargaPerigosa ? "Sim" : "Não") : string.Empty,
                    carga.CargaDeComplemento,
                    NumeroDoca = (cargaJanelaCarregamento.CentroCarregamento?.CamposVisiveisTransportador ?? "27").Contains("27") ? carga?.NumeroDoca ?? "" : "",
                },
                Transportador = new
                {
                    Codigo = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? cargaJanelaCarregamentoTransportador?.Terceiro.CPF_CNPJ ?? 0 : cargaJanelaCarregamentoTransportador?.Transportador.Codigo ?? 0,
                    Descricao = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? cargaJanelaCarregamentoTransportador?.Terceiro.Nome ?? string.Empty : cargaJanelaCarregamentoTransportador?.Transportador.Descricao ?? string.Empty
                },
                Rota = new
                {
                    Codigo = carga.Rota?.Codigo ?? 0,
                    Descricao = carga.Rota?.Descricao ?? ""
                },
                Filial = new
                {
                    Codigo = exibirFilial ? carga.Filial?.Codigo ?? 0 : 0,
                    Descricao = exibirFilial ? carga.Filial?.Descricao ?? string.Empty : string.Empty
                },
                PrevisaoEntrega = dataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataCarregamento = cargaJanelaCarregamento.InicioCarregamento != DateTime.MinValue ? cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") : "",
                InicioCarregamento = !cargaJanelaCarregamento.Excedente ? cargaJanelaCarregamento.InicioCarregamento.ToDateTimeString() : string.Empty,
                DataPrevisaoTerminoCarregamento = previsaoTerminoCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                TerminoCarregamento = !cargaJanelaCarregamento.Excedente ? cargaJanelaCarregamento.TerminoCarregamento.ToDateTimeString() : string.Empty,
                HorarioFixoCarregamento = (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento?.CentroCarregamento?.NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDescarga ?? false) ? (carga.DadosSumarizados?.HorarioFixoCarregamento ?? false) : false,
                HorarioLimiteConfirmarCarga = cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga.HasValue ? cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                cargaJanelaCarregamentoTransportador.Situacao,
                ModeloVeiculo = new
                {
                    Codigo = carga.ModeloVeicularCarga?.Codigo ?? 0,
                    Descricao = carga.ModeloVeicularCarga?.Descricao ?? string.Empty
                },
                TipoCarga = new
                {
                    Codigo = carga.TipoDeCarga?.Codigo ?? 0,
                    Descricao = carga.TipoDeCarga?.Descricao ?? string.Empty,
                    ExigeVeiculoRastreado = carga.TipoDeCarga?.ExigeVeiculoRastreado ?? false
                },
                CargasVinculadas = (
                    from obj in cargasVinculadas
                    select new
                    {
                        obj.Codigo,
                        obj.CodigoCargaEmbarcador
                    }
                ).ToList(),
                Motorista = carga.Motoristas.FirstOrDefault()?.Nome ?? string.Empty,
                Placa = carga.Veiculo?.Placa ?? string.Empty,
                Placas = Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(carga.Veiculo),
                Peso = carga.DadosSumarizados?.PesoTotal.ToString("n2") ?? "",
                DataProximaSituacao = cargaJanelaCarregamento.DataProximaSituacao.ToString("dd/MM/yyyy HH:mm"),
                Ordem = string.IsNullOrWhiteSpace(ordem) ? "" : ordem,
                Remetente = carga.DadosSumarizados?.Remetentes ?? "",
                CDDestino = agendamento?.CDDestino?.Descricao ?? string.Empty,
                DataAgendamento = agendamento?.DataAgendamento?.ToDateTimeString() ?? string.Empty,
                DataColeta = tempoDataColeta.HasValue && tempoDataColeta.Value.TotalSeconds > 0 ? $"{(int)tempoDataColeta.Value.TotalHours}h:{tempoDataColeta.Value.Minutes}m" : string.Empty,
                Origem = carga.DadosSumarizados?.Origens ?? string.Empty,
                Destino = destino.ToString(),
                Pedido = numeroPedidos ?? "",
                NumeroEXP = numeroExps ?? "",
                PesoCubado = pesoCubado > 0 ? pesoCubado.ToString("n4") : "",
                Destinatario = carga.DadosSumarizados?.DestinatariosReais ?? string.Empty,
                NumeroEntregas = carga.DadosSumarizados?.NumeroEntregas ?? 0,
                NumeroEntregasFinais = carga.DadosSumarizados?.NumeroEntregasFinais ?? 0,
                Mensagem = ObterMensagemRetornoCargaTransportador(cargaJanelaCarregamentoTransportador.Situacao),
                Observacao = observacao ?? string.Empty,
                ObservacaoCarregamento = cargaJanelaCarregamento.Carga?.Carregamento?.Observacao ?? "",
                ObservacaoCliente = cargaJanelaCarregamento.Carga?.DadosSumarizados?.ClientesRemetentes != null ? string.Join(", ", cargaJanelaCarregamento.Carga.DadosSumarizados.ClientesRemetentes.Where(obj => !string.IsNullOrWhiteSpace(obj.Observacao))?.Select(obj => obj.Observacao)) : "",
                OcultarEdicaoDataHora = cargaJanelaCarregamento.CentroCarregamento?.OcultarEdicaoDataHora ?? false,
                PermitirSelecionarDataCarregamento = permitirSelecionarHorariosCarregamentoEDescarregamento && PermitirSelecionarHorarioCarregamento(cargaJanelaCarregamentoTransportador),
                PermitirSelecionarDatasDescarregamento = permitirSelecionarHorariosCarregamentoEDescarregamento && PermitirSelecionarHorariosDescarregamento(cargaJanelaCarregamentoTransportador, unitOfWork),
                PermitirExibicaoDatasDescarregamento = dadosDescarregamentosPorCarga.Count > 0,
                TotalPallets = ((from cargaPedido in carga.Pedidos select cargaPedido.Pedido.TotalPallets)?.Sum() ?? 0).ToString("n4"),
                ValorMercadorias = ((from cargaPedido in carga.Pedidos select cargaPedido.Pedido.ValorTotalNotasFiscais)?.Sum() ?? 0).ToString("n2"),
                ExibirDetalhesCargaJanelaCarregamentoTransportador = cargaJanelaCarregamento.CentroCarregamento?.ExibirDetalhesCargaJanelaCarregamentoTransportador ?? false,
                BloquearComponentesDeFreteJanelaCarregamentoTransportador = cargaJanelaCarregamento.CentroCarregamento?.BloquearComponentesDeFreteJanelaCarregamentoTransportador ?? false,
                NaoExibirValorFretePortalTransportador = cargaJanelaCarregamento.CentroCarregamento?.NaoExibirValorFretePortalTransportador ?? false,
                PermitirInformarValorFreteCargasAtribuidasAoTransportadorNaJanelaCarregamentoTransportador = cargaJanelaCarregamento.CentroCarregamento?.PermitirInformarValorFreteCargasAtribuidasAoTransportadorNaJanelaCarregamentoTransportador ?? false,
                BloquearTrocaDataListaHorarios = cargaJanelaCarregamento.CentroCarregamento?.BloquearTrocaDataListaHorarios ?? false,
                ExigeTermoAceiteParaTransporte = exigeTermoAceite && termosAceite.Count == 0,
                TermoAceite = termoAceite,
                HabilitarTermoChegadaHorario = cargaJanelaCarregamento.CentroCarregamento?.HabilitarTermoChegadaHorario ?? false,
                TermoChegadaHorario = cargaJanelaCarregamento.CentroCarregamento?.TermoChegadaHorario ?? "",
                cargaJanelaCarregamento.CargaLiberadaCotacao,
                SituacaoJanelaCarregamento = cargaJanelaCarregamento.Situacao,
                CodigoContainerReboque = containerReboque?.Codigo.ToString() ?? Guid.NewGuid().ToString(),
                CodigoContainerSegundoReboque = containerSegundoReboque?.Codigo.ToString() ?? Guid.NewGuid().ToString(),
                CodigoContainerVeiculo = containerVeiculo?.Codigo.ToString() ?? Guid.NewGuid().ToString(),
                Componentes = (
                    from componente in componentesFreteDinamicos
                    select new
                    {
                        Descricao = componente.ComponenteFrete?.Descricao ?? componente.DescricaoComponente,
                        Valor = componente.ValorComponente.ToString("n2")
                    }
                ).ToList(),
                ComponentesFreteTransportador = (
                    from componente in cargaJanelaCarregamentoTransportador.Componentes
                    select new
                    {
                        componente.Descricao,
                        Valor = componente.Valor.ToString("n2")
                    }
                ).ToList(),
                FreteCalculadoComProblemas = cargaJanelaCarregamentoTransportador.FreteCalculadoComProblemas,
                PendenteCalcularFreteTransportador = cargaJanelaCarregamentoTransportador.PendenteCalcularFrete,
                PermitirTransportadorInformarValorFrete = cargaJanelaCarregamento.CentroCarregamento?.PermitirTransportadorInformarValorFrete ?? false,
                PossuiFreteCalculado = cargaJanelaCarregamentoTransportador.PossuiFreteCalculado,
                ValorICMS = carga.ValorICMS.ToString("n2"),
                ValorFrete = valorFreteAPagar.ToString("n2"),
                ValorFreteLiquido = carga.ValorFrete,
                ValorFreteSemICMS = carga.ValorFrete.ToString("n2"),
                ValorFreteTabelaTransportador = cargaJanelaCarregamentoTransportador.ValorFreteTabela + cargaJanelaCarregamentoTransportador.Componentes.Sum(obj => obj.Valor),
                LembrarEspelhamento = lembrarEspelhamento,
                DescricaoRastreador = veiculo?.TecnologiaRastreador?.NomeConta ?? veiculo?.TecnologiaRastreador?.Descricao ?? string.Empty,
                Volumes = volumes,
                MensagemLicenca = carga.LicencaInvalida ? repCargaLicenca.BuscarPorCarga(carga.Codigo)?.Mensagem ?? "Licença inválida" : string.Empty,
                Leilao = new
                {
                    MenorLance = exibirMenorLance ? (janelaComMenorLance?.ValorTotalFrete.ToString("n2") ?? string.Empty) : string.Empty,
                    DataFimLeilao = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.DataTerminoCotacao.HasValue ? cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.DataTerminoCotacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                    CorFonte = janelaComMenorLance?.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador.Codigo ? "#23A723" : "#FF6961",
                    IdTempoRestante = $"tempo-restante-encerramento-cotacao-{carga.Codigo}",
                    PosicaoLeilao = posicao > 0 ? posicao <= 3 ? "Entre os 3 melhores lances" : $"{posicao}º Colocado" : string.Empty,
                    ValorTransportadorInteressadoSinalizado = valorTransportadorInteressadoSinalizado,
                },
                NotasFiscais = numeroNotasFiscais,
                DadosDescarregamentos = (
                    from o in dadosDescarregamentosPorCarga
                    select new
                    {
                        Carga = o.CodigoCarga,
                        CentroDescarregamento = new { Codigo = o.CodigoCentroCarregamento, Descricao = o.DescricaoCentroCarregamento },
                        DataDescarregamentoAgendada = o.DataCarregamentoFormatada
                    }
                ).ToList(),
                Enderecos = enderecoDestinatarios,
                CanalEntrega = canalEntrega,
                CanalVenda = canalVenda,
                DivisoriaIntegracaoLeilao = carga != null ? (carga.DivisoriaIntegracaoLeilao ? "Sim" : "Não") : string.Empty,
                CargaPerigosaIntegracaoLeilao = carga != null ? (carga.CargaPerigosaIntegracaoLeilao ? "Sim" : "Não") : string.Empty,
                NotasEnviadas = carga.DataEnvioUltimaNFe.HasValue ? "Sim" : carga.DataFinalizacaoProcessamentoDocumentosFiscais.HasValue ? "SIM" : "Não",
                ValidacaoConjunto = cargaJanelaCarregamento?.RecomendacaoGR?.ObterDescricao() ?? "",
                FreteSimulado = simulacaoFrete?.ValorFrete.ToString("n4") ?? string.Empty,
                UnidadeDeMedida = agendamento?.UnidadeMedida,
                GerarControleVisualizacaoTransportadorasTerceiros = cargaJanelaCarregamento?.CentroCarregamento?.GerarControleVisualizacaoTransportadorasTerceiros ?? false,
                JaVisualizouCarga = obterJaVisualizouCarga(cargaJanelaCarregamentoTransportador, unitOfWork),
            };
        }

        private string ObterValorTransportadorInteressadoSinalizado(CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            string valor = string.Empty;

            if ((cargaJanelaCarregamento.CentroCarregamento?.CamposVisiveisTransportador ?? "28").Contains("28"))
            {
                decimal valorFreteTransportador = cargaJanelaCarregamentoTransportador.ObterValorFreteTransportador();

                if ((TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || (this.Usuario?.Empresa?.Codigo ?? -1) == (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento?.TransportadorOriginal?.Codigo ?? 0)) && valorFreteTransportador > 0m)
                    valor = valorFreteTransportador.ToString("n2");
            }

            return valor;
        }

        private int ObterPosicaoLeilao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            int posicao = 1;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> lances = repositorioCargaJanelaCarregamentoTransportador.BuscarPorInteressadosComValorInformado(cargaJanelaCarregamento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador lancedoTransportador = lances.Where(o => o.Transportador?.Codigo == Usuario.Empresa.Codigo).FirstOrDefault();

            if (lances.Count == 0 || lancedoTransportador == null)
                return 0;

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> lancesRestantes = lances.Where(o => o.Codigo != lancedoTransportador.Codigo).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador outroLance in lancesRestantes)
            {
                if ((lancedoTransportador.ValorFreteTransportador > outroLance.ValorFreteTransportador) ||
                    (lancedoTransportador.ValorFreteTransportador == outroLance.ValorFreteTransportador && lancedoTransportador.DataInteresse > outroLance.DataInteresse))
                    posicao++;
            }

            return posicao;
        }

        private bool PermitirSelecionarHorarioCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            if (cargaJanelaCarregamentoTransportador.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Confirmada)
                return false;

            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.SemTransportador)
                return false;

            DateTime dataLimiteSelecionarHorarioCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.InicioCarregamento.AddHours(-cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.TempoMaximoModificarHorarioCarregamentoTransportador);

            if ((DateTime.Now > dataLimiteSelecionarHorarioCarregamento) && !cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Excedente)
                return false;

            return true;
        }

        private bool PermitirSelecionarHorariosDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaJanelaCarregamentoTransportador.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Confirmada)
                return false;

            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.SemTransportador)
                return false;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

            if (repositorioFluxoGestaoPatio.ExisteFluxoDeOrigemNaoFinalizadoPorCarga(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo))
                return false;

            return true;
        }

        private string ObterMensagemRetornoCargaTransportador(SituacaoCargaJanelaCarregamentoTransportador? situacao)
        {
            if (situacao.HasValue)
            {
                switch (situacao.Value)
                {
                    case SituacaoCargaJanelaCarregamentoTransportador.ComInteresse:
                        return Localization.Resources.Logistica.JanelaCarregamentoTransportador.AguardandoConfirmacao;
                    case SituacaoCargaJanelaCarregamentoTransportador.Rejeitada:
                        return Localization.Resources.Logistica.JanelaCarregamentoTransportador.InteresseRejeitado;
                }
            }

            return string.Empty;
        }

        private bool PermiteAvancarEtapaEmissao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!(carga.TipoOperacao?.PermiteTransportadorAvancarEtapaEmissao ?? false))
                return false;

            if (carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                return false;

            bool etapasAnterioresConcluidas = carga.DataEnvioUltimaNFe.HasValue && !carga.AguardandoEmissaoDocumentoAnterior && !carga.AgValorRedespacho;
            bool emissaoLiberada = carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.CalculoFrete && etapasAnterioresConcluidas;
            bool emissaoDocumentosAutorizada = carga.DataEnvioUltimaNFe.HasValue && carga.DataEnvioUltimaNFe.Value <= DateTime.Now && emissaoLiberada && !carga.CalculandoFrete && !carga.PendenteGerarCargaDistribuidor && !carga.AguardarIntegracaoEtapaTransportador;

            return !emissaoDocumentosAutorizada;
        }

        private void SalvarComponentesFrete(dynamic componentesFrete, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ComponetesFrete servicoComponenteFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> componentesFreteAdicionados = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete>();

            servicoComponenteFrete.RemoverComponentesCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador, unitOfWork);

            foreach (var interesseCargaComponenteFrete in componentesFrete)
            {
                int codigoComponenteFrete = ((string)interesseCargaComponenteFrete.ComponenteFrete).ToInt();
                int codigoModeloDocumentoFiscal = ((string)interesseCargaComponenteFrete.ModeloDocumentoFiscal).ToInt();
                bool cobrarOutroDocumento = ((string)interesseCargaComponenteFrete.CobrarOutroDocumento).ToBool();
                decimal percentual = ((string)interesseCargaComponenteFrete.Percentual).ToDecimal();
                decimal valorComponente = ((string)interesseCargaComponenteFrete.ValorComponente).ToDecimal();
                decimal valorSugerido = ((string)interesseCargaComponenteFrete.ValorSugerido).ToDecimal();
                decimal valorTotalMoeda = ((string)interesseCargaComponenteFrete.ValorTotalMoeda).ToDecimal();

                componentesFreteAdicionados.Add(servicoComponenteFrete.AdicionarComponenteFreteCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador, codigoComponenteFrete, codigoModeloDocumentoFiscal, cobrarOutroDocumento, percentual, valorComponente, valorSugerido, valorTotalMoeda, unitOfWork));
            }
        }

        private bool ValidarVeiculoDisponivel(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
            Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repositorioVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if ((cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento?.CentroCarregamento?.RepassarCargaCasoNaoExistaVeiculoDisponivel ?? false) && Usuario.Empresa != null && cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento?.CentroCarregamento?.TempoMinutosEscolhaAutomaticaCotacao > 0)
            {
                bool possuiVeiculoDisponivel = false;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    possuiVeiculoDisponivel = repositorioVeiculoDisponivelCarregamento.BuscarSePossuiVeiculoDisponivelTransportadorTerceiro(Usuario?.ClienteTerceiro?.Codigo ?? 0);
                else
                    possuiVeiculoDisponivel = repositorioVeiculoDisponivelCarregamento.BuscarSePossuiVeiculoDisponivel(Usuario.Empresa?.Codigo ?? 0);

                if (!possuiVeiculoDisponivel)
                {
                    Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
                    cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Rejeitada;
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador proximoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaJanelaCarregamentoEOrdem((cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento?.Codigo ?? 0), cargaJanelaCarregamentoTransportador.Ordem + 1);

                    if (proximoTransportador != null)
                    {
                        proximoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao;
                        repositorioCargaJanelaCarregamentoTransportador.Atualizar(proximoTransportador);
                        servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(proximoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaDisponibilizadaParaTransportadorInformarOsDadosDeTransporte);

                        if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaBase.IsCarga())
                        {
                            cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.RejeitadaPeloTransportador = false;
                            repositorioCarga.Atualizar(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga);
                        }

                    }

                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaRejeitadaParaTransportadorPorNaoPossuirVeiculoDisponivel);

                    unitOfWork.CommitChanges();

                    return false;
                }
            }

            return true;
        }

        private void RemoverInteresseCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte repositorioDadosTransporte = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(unitOfWork);

            cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Disponivel;

            repCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte dadosTransporteExistente = repositorioDadosTransporte.BuscarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);

            if (dadosTransporteExistente != null)
                repositorioDadosTransporte.Deletar(dadosTransporteExistente);

            repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.DeletarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamentoTransportador, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.RemoveuInteresseDaCarga, unitOfWork);
            servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRemoveuInteresseNaCarga, Usuario);
            servicoCargaJanelaCarregamentoTransportador.InformaCargaAtualizadaEmbarcador(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo, Cliente, _conexao.AdminStringConexao);
        }

        private void MarcarSemInteresseTransportadorCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento motivo, string observacao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);

            cargaJanelaCarregamentoTransportador.SemInteresseTransportador = true;

            repCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);

            servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, string.Concat("Usuário ", Usuario.Nome, " não demonstrou interesse na carga"), TipoCargaJanelaCarregamentoTransportadorHistorico.DemonstrouNaoInteresse, motivo, observacao, Usuario);


        }
        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> RetornarPeriodosDisponiveis(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, DateTime dataCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = servicoCargaJanelaCarregamentoDisponibilidade.ObterExcecaoCentroCarregamentoPorData(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento?.Codigo ?? 0, dataCarregamento);

            if (excecao != null)
            {

                if (excecao.PeriodosCarregamento.Any())
                {
                    foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento perido in excecao.PeriodosCarregamento)
                    {
                        periodosCarregamento.Add(perido);
                    }
                }
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentoDia = repPeriodoCarregamento.BuscarPorCentroCarregamentoEDia(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento?.Codigo ?? 0, DiaSemanaHelper.ObterDiaSemana(dataCarregamento));
                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento perido in periodosCarregamentoDia)
                {
                    periodosCarregamento.Add(perido);
                }
            }
            return periodosCarregamento;

        }

        private DateTime RetornarProximoDiaUtilDeFuncionamentoDoCentro(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork);
            DateTime data = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.InicioCarregamento;
            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento != null)
            {
                if (data.Date < DateTime.Now.Date)
                {
                    data = DateTime.Now;
                }
                do
                {
                    data = data.AddDays(1);
                    Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = servicoCargaJanelaCarregamentoDisponibilidade.ObterExcecaoCentroCarregamentoPorData(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.Codigo, data);

                    if (excecao != null)
                    {
                        if (excecao.PeriodosCarregamento.Any())
                        {
                            break;
                        }
                        else
                            continue;
                    }

                    DiaSemana Dia = DiaSemanaHelper.ObterDiaSemana(data);

                    if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.PeriodosCarregamento.Any(obj => obj.Dia == Dia))
                    {
                        break;
                    }
                } while (true);
            }

            return data;
        }

        private void SalvarDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboques, Dominio.Entidades.Usuario motorista, List<Dominio.Entidades.Usuario> ajudantes, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);
            Servicos.Embarcador.Carga.CargaIndicador servicoCargaIndicador = new Servicos.Embarcador.Carga.CargaIndicador(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracao);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(TipoServicoMultisoftware));
            Servicos.Embarcador.Veiculo.LicencaVeiculo servicoLicencaVeiculo = new Servicos.Embarcador.Veiculo.LicencaVeiculo(unitOfWork, TipoServicoMultisoftware);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Veiculo veiculoDaCarga = carga.Veiculo;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = carga.Pedidos != null ? carga.Pedidos.ToList() : new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            carga.Initialize();
            carga.DataAtualizacaoCarga = DateTime.Now;
            carga.ModeloVeicularCarga = modeloVeicularCarga;
            carga.Veiculo = veiculo;
            carga.VeiculosVinculados?.Clear();
            carga.Ajudantes?.Clear();

            if (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false)
                carga.CarregamentoIntegradoERP = false;

            Servicos.Embarcador.Carga.CargaDatas servicoCargaDatas = new Servicos.Embarcador.Carga.CargaDatas(ConfiguracaoEmbarcador, unitOfWork);

            servicoCargaDatas.SalvarDataSalvamentoDadosTransporte(carga);

            if (cargaJanelaCarregamentoTransportador.Transportador != transportador)
            {
                cargaJanelaCarregamentoTransportador.Transportador = transportador;
                carga.Empresa = cargaJanelaCarregamentoTransportador.Transportador;
                repCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);

                Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargasPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
            }

            servicoCargaIndicador.DefinirIndicadorDadosTransporte(carga, CargaIndicadorVeiculoMotorista.InformadoTransportador);

            if (reboques?.Count > 0)
            {
                carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                foreach (Dominio.Entidades.Veiculo veiculoVinculado in reboques)
                    carga.VeiculosVinculados.Add(veiculoVinculado);
            }

            if (ajudantes.Count > 0)
            {
                carga.Ajudantes = new List<Dominio.Entidades.Usuario>();

                foreach (Dominio.Entidades.Usuario ajudante in ajudantes)
                    carga.Ajudantes.Add(ajudante);
            }

            bool atualizouMotorista = servicoCargaMotorista.AtualizarMotorista(carga, motorista);
            servicoLicencaVeiculo.GerarCargaLicenca(carga);

            repositorioCarga.Atualizar(carga, Auditado);

            //deve ser chamado sempre que atualiza o veiculo na carga
            Servicos.Embarcador.Carga.Carga.AtualizarIntegracaoConsultaCalculoValePedagio(carga, unitOfWork, TipoServicoMultisoftware);
            servicoCargaJanelaCarregamento.AtualizarSituacao(carga, TipoServicoMultisoftware);
            servicoFilaCarregamentoVeiculo.AtualizarPorCarga(carga, TipoServicoMultisoftware);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

            //Se o transportador pode liberar para faturamente gera o proximo status manualmente
            if (cargaJanelaCarregamento?.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.TransportadoresAutorizadosLiberarFaturamento.Contains(carga.Empresa))
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

                cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.DataProximaSituacao = DateTime.Now.AddMinutes(cargaJanelaCarregamento.CentroCarregamento.TempoEmMinutosLiberacao);
                cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.LiberaradaParaFaturamentePeloTransportador = true;

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento);
            }

            bool recalcularFrete = !(carga.TipoOperacao?.SolicitarNotasFiscaisAoSalvarDadosTransportador ?? false);
            bool freteInformadado = true;

            if (carga.TipoFreteEscolhido != TipoFreteEscolhido.Embarcador && carga.TipoFreteEscolhido != TipoFreteEscolhido.Operador
                && carga.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.AguardandoAprovacao)
            {
                freteInformadado = false;
            }

            if (!carga.ExigeNotaFiscalParaCalcularFrete)
            {
                //Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(_conexao.StringConexao, TipoServicoMultisoftware);
                //serCargaFrete.CalcularFreteSemExtornarComplementos(ref carga, unidadeDeTrabalho, configuracao);
                carga.PossuiPendencia = false;
                carga.MotivoPendencia = "";

                if (repConsultaValorPedagioIntegracao.VerificarExistePorCargaAgIntegracao(carga.Codigo))//se tem consulta tem q voltar para etapa 2
                    carga.SituacaoCarga = SituacaoCarga.CalculoFrete;

                if (recalcularFrete && !freteInformadado)
                {
                    carga.CalculandoFrete = true;
                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                    carga.DataInicioCalculoFrete = DateTime.Now;
                    carga.CalcularFreteSemEstornarComplemento = true;
                }

                repositorioCarga.Atualizar(carga);
            }
            else if ((!carga.ProblemaIntegracaoGrMotoristaVeiculo || carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo)
                && (!carga.LicencaInvalida || carga.LiberadaComLicencaInvalida || (carga.TipoOperacao?.PermitirAvancarEtapaComLicencaInvalida ?? true)))
            {
                if (carga.TipoOperacao?.ConfiguracaoCarga?.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento ?? false)
                {
                    if ((carga.Pedidos?.Count > 0) && (carga.Pedidos.First().Pedido.DataInicialColeta.HasValue) && (carga.TipoOperacao.ConfiguracaoCarga?.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento > 0))
                    {
                        //mercado Livre, as cargas vao avançar por uma thread assim q atinger o tempo limite pela data de coleta do primeiro pedido da carga enquanto nao atende segue nova.
                        List<SituacaoCarga> situacaoesPermitidas = new List<SituacaoCarga>() {
                                    SituacaoCarga.Nova,
                                    SituacaoCarga.AgNFe,
                                    };

                        int horasConfiguradas = carga.TipoOperacao.ConfiguracaoCarga.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento;
                        DateTime horaLimite = carga.Pedidos.First().Pedido.DataInicialColeta.Value.AddHours(-horasConfiguradas);
                        if (DateTime.Now < horaLimite && situacaoesPermitidas.Contains(carga.SituacaoCarga))
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                        else
                        {
                            if (carga.SituacaoCarga == SituacaoCarga.Nova)
                                carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                        }
                    }
                }
                else
                {
                    if ((carga.DataEnvioUltimaNFe.HasValue || carga.SituacaoCarga == SituacaoCarga.CalculoFrete) && !carga.CargaDePreCargaEmFechamento)
                    {

                        carga.DataInicioEmissaoDocumentos = null;
                        carga.DataEnvioUltimaNFe = null;//volta a situacao a data de envio da nota para poder calcular o frete e ver se pode emitir.
                        carga.PossuiPendencia = false;
                        carga.MotivoPendencia = "";

                        if (recalcularFrete && !freteInformadado)
                        {
                            carga.CalculandoFrete = true;
                            carga.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                            carga.DataInicioCalculoFrete = DateTime.Now;
                            carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                            string mensagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).CalcularFreteTodoCarregamento(carga);

                            if (!string.IsNullOrWhiteSpace(mensagem))
                            {
                                carga.PossuiPendencia = true;
                                carga.MotivoPendencia = mensagem;
                                carga.SituacaoCarga = SituacaoCarga.CalculoFrete;

                                if (configuracao.NotificarAlteracaoCargaAoOperador)
                                    servicoCarga.NotificarAlteracaoAoOperador(carga, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoFoiPossivelCalcularFreteDaCargaN, carga.CodigoCargaEmbarcador)), unitOfWork);
                            }
                        }
                    }
                    else
                    {
                        carga.SituacaoCarga = SituacaoCarga.AgNFe;

                        if (!(carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            bool exigeConfirmacaoFreteAntesEmissao = carga.Filial?.ExigeConfirmacaoFreteAntesEmissao ?? false;
                            if (!exigeConfirmacaoFreteAntesEmissao)
                                exigeConfirmacaoFreteAntesEmissao = carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? true;

                            if (!exigeConfirmacaoFreteAntesEmissao)
                            {
                                carga.DataEnvioUltimaNFe = DateTime.Now;
                                carga.DataInicioEmissaoDocumentos = DateTime.Now;
                            }

                            carga.ProcessandoDocumentosFiscais = true;
                            carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                        }
                    }
                }

                repositorioCarga.Atualizar(carga);
            }

            if (!freteInformadado)
                Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracao, base.Auditado, Localization.Resources.Logistica.JanelaCarregamentoTransportador.JanelaDeCarregamentoDoTransportador, unitOfWork);


            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCargaETipo(carga.Codigo, TipoFluxoGestaoPatio.Origem);

            servicoCarga.ControleDisponibilidadeVeiculo(fluxoGestaoPatio, veiculoDaCarga, unitOfWork);

            if (configuracaoGestaoPatio?.ChegadaVeiculoPermiteAvancarAutomaticamenteAposInformarDadosTransporteCarga ?? false)
            {
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.AvancarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.ChegadaVeiculo);
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);

            if (carga.ExigeNotaFiscalParaCalcularFrete)
                Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarIntegracoesCargaDadosTransporte(carga, cargasPedidos, cargaMotoristas, configuracao, TipoServicoMultisoftware, unitOfWork);

            if (cargaJanelaCarregamento != null)
            {
                bool reenviarIntegracaoJanelaCarregamento = atualizouMotorista || (carga.Veiculo.Codigo != veiculoDaCarga.Codigo);
                new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao(unitOfWork, TipoServicoMultisoftware).AdicionarJanelaCarregamentoIntegracao(cargaJanelaCarregamento, reenviarIntegracaoJanelaCarregamento);
            }

            servicoCarga.IntegrarAlteracaoOrdensEmbarque(carga, Usuario, unitOfWork);
        }

        private void SalvarDadosTransporteInteresseCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (
                (cargaJanelaCarregamentoTransportador.Situacao != SituacaoCargaJanelaCarregamentoTransportador.ComInteresse) ||
                (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.CargaAgrupamento != null) ||
                !cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaLiberadaCotacao ||
                configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores
            )
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

            Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes servicoJanelaCarregamentoTransportadorValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes(unitOfWork, TipoServicoMultisoftware, configuracaoEmbarcador);

            int codigoVeiculo = Request.GetIntParam("Veiculo");
            Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo, false);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga;

            servicoJanelaCarregamentoTransportadorValidacoes.ValidacoesVeiculo(Usuario.Empresa, veiculo, carga);

            if ((veiculo?.TipoVeiculo == "1") && (veiculo?.VeiculosTracao?.Count > 0))
            {
                Dominio.Entidades.Veiculo tracao = (from o in veiculo.VeiculosTracao where o.Ativo select o).FirstOrDefault();

                if (tracao != null)
                    veiculo = tracao;
            }

            bool exigirConfirmacaoTracao = carga.TipoOperacao?.ExigePlacaTracao ?? false;
            List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();

            if (exigirConfirmacaoTracao)
            {
                if (veiculo == null)
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarplacaDaTracaoAoConjuntoDaPlacaSelecionadaPorFavorFacaVinculoTracaoReboqueNoCadastroDeVeiculosTenteNovamente);

                int codigoReboque = Request.GetIntParam("Reboque");
                int codigoSegundoReboque = Request.GetIntParam("SegundoReboque");

                Dominio.Entidades.Veiculo primeiroReboque = codigoReboque > 0 ? repositorioVeiculo.BuscarPorCodigo(codigoReboque) : null;
                Dominio.Entidades.Veiculo segundoReboque = codigoSegundoReboque > 0 ? repositorioVeiculo.BuscarPorCodigo(codigoSegundoReboque) : null;

                if (primeiroReboque != null)
                    reboques.Add(primeiroReboque);

                if (segundoReboque != null)
                    reboques.Add(segundoReboque);

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = (reboques.Count > 0) ? reboques.FirstOrDefault().ModeloVeicularCarga : veiculo?.ModeloVeicularCarga;

                if (modeloVeicularCarga == null)
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarUmVeiculoComModeloVeicularDefinido);

                if ((modeloVeicularCarga.Tipo == TipoModeloVeicularCarga.Tracao) && (modeloVeicularCarga.NumeroReboques > reboques.Count()))
                    throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarReboquedatracaoSelecionadaNumeroDeReboquesNecessarioPorFavorFacaVinculoTracaoReboqueNoCadastroDeVeiculostenteNovamente, modeloVeicularCarga.NumeroReboques));
            }
            else if (veiculo?.VeiculosVinculados != null)
                reboques = veiculo.VeiculosVinculados.ToList();

            if ((carga.TipoOperacao?.ExigeQueVeiculoIgualModeloVeicularDaCarga ?? false) && carga.ModeloVeicularCarga != null)
            {
                if (reboques.Count > 0)
                {
                    if (reboques.Any(vei => vei.ModeloVeicularCarga?.Codigo != carga.ModeloVeicularCarga.Codigo))
                        throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarReboquesCompativeisComModeloDeVeiculoDaCarga, carga.ModeloVeicularCarga.Descricao));
                }
                else if (veiculo != null)
                {
                    if (veiculo.ModeloVeicularCarga?.Codigo != carga.ModeloVeicularCarga.Codigo)
                        throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.EObrigatorioInformarReboquesCompativeisComModeloDeVeiculoDaCarga, carga.ModeloVeicularCarga.Descricao));
                }
            }

            if (veiculo != null && veiculo.ModeloVeicularCarga == null && carga.ModeloVeicularCarga != null)
            {
                veiculo.ModeloVeicularCarga = carga.ModeloVeicularCarga;
                repositorioVeiculo.Atualizar(veiculo, Auditado);

                foreach (Dominio.Entidades.Veiculo reboque in reboques)
                {
                    if (reboque.ModeloVeicularCarga == null)
                    {
                        reboque.ModeloVeicularCarga = carga.ModeloVeicularCarga;
                        repositorioVeiculo.Atualizar(reboque);
                    }
                }
            }

            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repositorioTipoCargaModeloVeicular.ConsultarPorTipoCarga(carga.TipoDeCarga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = (from o in tipoCargaModeloVeicular select o.ModeloVeicularCarga).ToList();

            if (veiculo != null)
            {
                if (!modelosVeicularesPermitidos.Contains(veiculo.ModeloVeicularCarga))
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelUilizarEsseVeiculoParaTransportarCargaPoisSeuTipoNaoSuportaEsseTransporte);

                if (configuracaoEmbarcador.TipoRestricaoPalletModeloVeicularCarga != TipoRestricaoPalletModeloVeicularCarga.NaoBloquear)
                {
                    int palletsModelo = veiculo.ModeloVeicularCarga.NumeroPaletes.HasValue ? veiculo.ModeloVeicularCarga.NumeroPaletes.Value : 0;
                    int numeroPallet = carga.ModeloVeicularCarga?.NumeroPaletes ?? carga.Pedidos?.Sum(obj => obj.Pedido?.NumeroPaletes) ?? 0;
                    if (configuracaoEmbarcador.TipoRestricaoPalletModeloVeicularCarga == TipoRestricaoPalletModeloVeicularCarga.BloquearSomenteNumeroSuperior && numeroPallet > palletsModelo)
                        throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelUtilizarEsseVeiculoParaTransportarCargaPoisQuantidadeDePalletsExigidoParaCargaInferiorCapacidadeDesteVeiculo, numeroPallet.ToString(), palletsModelo.ToString()));

                    if (configuracaoEmbarcador.TipoRestricaoPalletModeloVeicularCarga == TipoRestricaoPalletModeloVeicularCarga.BloquearSomenteNumeroInferior && numeroPallet < palletsModelo)
                        throw new ControllerException(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelUtilizarEsseVeiculoParaTransportarCargaPoisQuantidadeDePalletsExigidoParaCargaInferiorCapacidadeDesteVeiculo, numeroPallet.ToString(), palletsModelo.ToString()));
                }

                if (!configuracaoEmbarcador.NaoExigeInformarDisponibilidadeDeVeiculo)
                {
                    Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repositorioVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(unitOfWork);

                    Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento veiculoDisponivelCarregamento = null;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                        veiculoDisponivelCarregamento = repositorioVeiculoDisponivelCarregamento.BuscarPorVeiculoProprietario(veiculo.Codigo, Usuario.ClienteTerceiro.CPF_CNPJ);
                    else
                        veiculoDisponivelCarregamento = repositorioVeiculoDisponivelCarregamento.BuscarPorVeiculoEmpresa(veiculo.Codigo, Usuario.Empresa.Codigo);

                    if ((veiculoDisponivelCarregamento == null) && ((carga.Veiculo == null) || (carga.Veiculo.Codigo != veiculo.Codigo)))
                        throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoPossivelUtilizarEsseVeiculoParaTransportarCargaPoisEleNaoEstaDisponivel);
                }
            }


            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte repositorioDadosTransporte = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte dadosTransporteExistente = repositorioDadosTransporte.BuscarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);

            if (dadosTransporteExistente != null)
                repositorioDadosTransporte.Deletar(dadosTransporteExistente);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte dadosTransporte = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte()
            {
                CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador,
                Veiculo = veiculo,
                VeiculosVinculados = reboques
            };

            repositorioDadosTransporte.Inserir(dadosTransporte);
        }

        private Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento PreencherConfiguracaoDisponibilidadeCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento;
            int codigoCarga = Request.GetIntParam("Carga");

            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamento.Carga;

                configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento
                {
                    CodigoCarga = carga.Codigo,
                    CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                    CodigoTransportador = carga.Empresa?.Codigo ?? 0,
                    CpfCnpjCliente = carga.Pedidos.FirstOrDefault()?.Pedido?.Destinatario?.CPF_CNPJ ?? 0,
                    CodigoModeloVeicularCarga = carga.ModeloVeicularCarga?.Codigo ?? 0
                };
            }
            else
            {
                configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
                {
                    CpfCnpjCliente = Request.GetIntParam("Cliente"),
                    CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                    CodigoTransportador = Request.GetIntParam("Transportador"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                };
            }

            return configuracaoDisponibilidadeCarregamento;
        }

        private void SalvarInteressadoCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (carga == null)
                throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

            if (this.Usuario.Empresa == null)
                throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

            Dominio.Entidades.Empresa transportador = this.Usuario.Empresa;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacaoAprovacao servicoCargaJanelaCarregamentoCotacaoAprovacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacaoAprovacao(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork, configuracaoEmbarcador, null);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro(unitOfWork, configuracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);
            Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(cargaReferencia.Codigo, Usuario?.ClienteTerceiro, retornarCargasOriginais: true);
            else
                cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(cargaReferencia.Codigo, transportador, retornarCargasOriginais: true);

            if (cargasJanelaCarregamentoTransportador.Count == 0)
                throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == cargaReferencia.Codigo select o).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoReferencia = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento;


            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento;

                if (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelConfirmarInteressePoisEssaCargaJaTeveOutroTransportadorComInteresseConfirmado);

                if ((cargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Cancelada) || (cargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Anulada))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelConfirmarInteressePoisEssaCargaPoisMesmaJaFoiCancelada);

                if ((cargaJanelaCarregamento.Carga.CargaAgrupamento == null) && (cargaJanelaCarregamento.Carga.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.NaoInformada) && (cargaJanelaCarregamento.Carga.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.Aprovada))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelConfirmarUmaTransportadoraAntesDaAprovacaoDoValorDoFreteDaCarga);

                if (cargaJanelaCarregamentoTransportador.HorarioCarregamento.HasValue)
                {
                    if (!cargaJanelaCarregamento.Excedente && cargaJanelaCarregamentoTransportador.HorarioCarregamento.Value > cargaJanelaCarregamento.InicioCarregamento)
                        throw new ControllerException(Localization.Resources.Cargas.Carga.ParaSelecionarEsseTransportadorHoraDoCarregamentoDeveSerIgualOuSuperiorHoraQueElePodeAtender);
                }

                if (cargaJanelaCarregamento.DataSituacaoAtual.HasValue && cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.TempoBloqueioEscolhaTransportador > 0)
                {
                    double minutosSituacao = (DateTime.Now - cargaJanelaCarregamento.DataSituacaoAtual.Value).TotalMinutes;

                    if (minutosSituacao < cargaJanelaCarregamento.CentroCarregamento.TempoBloqueioEscolhaTransportador)
                        throw new ControllerException(Localization.Resources.Cargas.Carga.NaopossivelSetarTransportadorDaCargaPoisCargaDeveFicarDisponivelPorMais + (cargaJanelaCarregamento.CentroCarregamento.TempoBloqueioEscolhaTransportador - minutosSituacao).ToString("n0") + Localization.Resources.Cargas.Carga.MinutosParaVisualizacaoDosTransportadoresNaJanelaDeCarregamento);
                }

                if (cargaJanelaCarregamento.Carga.CargaAgrupamento == null)
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    {
                        servicoCargaJanelaCarregamentoTransportadorTerceiro.ValidarPermissaoMarcarInteresseCarga(cargaJanelaCarregamentoTransportador.Terceiro, carga);
                        servicoCargaJanelaCarregamentoTransportadorTerceiro.ValidarCargasInteressadas(cargaJanelaCarregamentoTransportador.Terceiro, carga);
                    }
                    else
                    {
                        servicoCargaJanelaCarregamentoTransportador.ValidarPermissaoMarcarInteresseCarga(cargaJanelaCarregamentoTransportador.Transportador, carga);
                        servicoCargaJanelaCarregamentoTransportador.ValidarCargasInteressadas(cargaJanelaCarregamentoTransportador.Transportador, carga);
                    }
                }

                if (cargaJanelaCarregamento.Codigo == cargaJanelaCarregamentoReferencia.Codigo)
                    servicoCargaJanelaCarregamentoCotacaoAprovacao.CriarAprovacao(cargaJanelaCarregamentoReferencia, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, null, Localization.Resources.Cargas.Carga.AdicionouInteresseNaCarga, unitOfWork);

                if (cargaJanelaCarregamentoReferencia.SituacaoCotacao.IsPendenteAprovacao())
                    continue;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    if (((cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.PermitirTransportadorInformarValorFrete) || cargaJanelaCarregamento.CargaLiberadaCotacao) && cargaJanelaCarregamentoTransportador.ValorFreteTransportador > 0)
                        servicoCargaJanelaCarregamentoTransportadorTerceiro.DefinirTransportadorComValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);
                    else
                        servicoCargaJanelaCarregamentoTransportadorTerceiro.DefinirTransportadorSemValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);
                }
                else
                {
                    if (((cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.PermitirTransportadorInformarValorFrete) || cargaJanelaCarregamento.CargaLiberadaCotacao) && cargaJanelaCarregamentoTransportador.ValorFreteTransportador > 0)
                        servicoCargaJanelaCarregamentoTransportador.DefinirTransportadorComValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);
                    else
                        servicoCargaJanelaCarregamentoTransportador.DefinirTransportadorSemValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);
                }

                servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
            }

            if (!cargaJanelaCarregamentoReferencia.SituacaoCotacao.IsPendenteAprovacao())
            {
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaSemTransportadorEscolhido(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento);
                servicoCargaJanelaCarregamentoNotificacao.NotificarCotacaoComTransportadorEscolhido(cargaJanelaCarregamentoTransportadorReferencia, cargasJanelaCarregamentoTransportador, TipoServicoMultisoftware);
            }
        }

        private double CalcularTempoLimiteCarregamentoEmMinutos(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            TimeSpan? dataLimite = null;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork).BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo).FirstOrDefault();


            if (cargaEntrega != null)
            {
                if (cargaEntrega.Carga.DataInicioViagem.HasValue)
                    dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataInicioViagem;
                else
                {
                    switch (configuracaoTMS.DataBaseCalculoPrevisaoControleEntrega)
                    {
                        case DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataCriacaoCarga;
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataPrevisaoTerminoCarga;
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - (cargaEntrega.Carga.DataInicioViagemPrevista.HasValue ? cargaEntrega.Carga.DataInicioViagemPrevista : cargaEntrega.Carga.DataPrevisaoTerminoCarga ?? cargaEntrega.Carga.DataCriacaoCarga);
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - (cargaEntrega.Carga.DataCarregamentoCarga.HasValue ? cargaEntrega.Carga.DataCarregamentoCarga : cargaEntrega.Carga.DataInicioViagemPrevista);
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaJanelaCarregamento?.InicioCarregamento;
                            break;
                    }
                }
            }

            return dataLimite.HasValue ? dataLimite.Value.TotalMinutes : 0;
        }

        private bool obterJaVisualizouCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            if ((cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento?.CentroCarregamento?.GerarControleVisualizacaoTransportadorasTerceiros) ?? false)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repositorioCargaJanelaCarregamentoTransportadorHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(unitOfWork);

                if (!repositorioCargaJanelaCarregamentoTransportadorHistorico.ExistePorCargaJanelaCarregamentoTransportadorETipo(cargaJanelaCarregamentoTransportador.Codigo, TipoCargaJanelaCarregamentoTransportadorHistorico.VisualizouCarga))
                    return false;
                else
                    return true;
            }
            return false;

        }

        private void SalvarDadosTransporteInteresse(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte repositorioCargaJanelaCarregamentoTransportadorDadosTransporte = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte dadosTransporte = null;
            if (cargaJanelaCarregamentoTransportador.DadosTransporte != null)
                dadosTransporte = cargaJanelaCarregamentoTransportador.DadosTransporte;
            else
                dadosTransporte = new CargaJanelaCarregamentoTransportadorDadosTransporte();

            dadosTransporte.CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador;
            dadosTransporte.Motorista = motorista;
            dadosTransporte.Veiculo = veiculo;

            if (dadosTransporte.Codigo > 0)
                repositorioCargaJanelaCarregamentoTransportadorDadosTransporte.Atualizar(dadosTransporte);
            else
                repositorioCargaJanelaCarregamentoTransportadorDadosTransporte.Inserir(dadosTransporte);

            cargaJanelaCarregamentoTransportador.DadosTransporte = dadosTransporte;
            repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
        }
        #endregion
    }
}
