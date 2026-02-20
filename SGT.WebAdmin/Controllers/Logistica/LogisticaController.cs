using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    public class LogisticaController : BaseController
    {
        #region Construtores

        public LogisticaController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Logistica/PercursosEntreEstados")]
        public async Task<IActionResult> PercursosEntreEstados()
        {
            return View();
        }

        [CustomAuthorize("Logistica/Rota")]
        public async Task<IActionResult> Rota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            bool semParar = false;
            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar) != null)
                    semParar = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            ViewBag.PossuiSemParar = Newtonsoft.Json.JsonConvert.SerializeObject(semParar);
            return View();
        }

        [CustomAuthorize("Logistica/Fronteira")]
        public async Task<IActionResult> Fronteira()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ProcedimentoEmbarque")]
        public async Task<IActionResult> ProcedimentoEmbarque()
        {
            return View();
        }

        [CustomAuthorize("Logistica/DatasPreferenciaisDescarga")]
        public async Task<IActionResult> DatasPreferenciaisDescarga()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AgendamentoEntregaPedido")]
        public async Task<IActionResult> AgendamentoEntregaPedido()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AgendamentoEntregaPedidoConsulta")]
        public async Task<IActionResult> AgendamentoEntregaPedidoConsulta()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AbastecimentoGas")]
        public async Task<IActionResult> AbastecimentoGas()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Logistica/AbastecimentoGas");

            ViewBag.PossuiPermissaoLancarAposHorarioLimite = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.AbastecimentoGas_PermiteLancarAbastecimentoAposHorarioLimite) ? "true" : "false";

            return View();
        }

        [CustomAuthorize("Logistica/AprovacaoSolicitacaoGas")]
        public async Task<IActionResult> AprovacaoSolicitacaoGas()
        {
            return View();
        }

        [CustomAuthorize("Logistica/RegraAprovacaoSolicitacaoGas")]
        public async Task<IActionResult> RegraAprovacaoSolicitacaoGas()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ConsolidacaoSolicitacaoGas")]
        public async Task<IActionResult> ConsolidacaoSolicitacaoGas()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AgendamentoColeta")]
        public async Task<IActionResult> AgendamentoColeta()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repositorioModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = Usuario.ClienteFornecedor != null ? repositorioModalidadeFornecedorPessoas.BuscarPorCliente(Usuario.ClienteFornecedor.CPF_CNPJ) : null;
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool compartilharAcessoEntreGrupoPessoas = ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? ((Usuario.ClienteFornecedor?.GrupoPessoas != null) && IsCompartilharAcessoEntreGrupoPessoas()) : IsCompartilharAcessoEntreGrupoPessoas();

                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.ConfiguracoesAgendamentoColeta = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    CompartilharAcessoEntreGrupoPessoas = compartilharAcessoEntreGrupoPessoas,
                    PermiteDownloadDocumentos = (modalidadeFornecedorPessoas?.PermiteDownloadDocumentos ?? true),
                    OrigemPadrao = !compartilharAcessoEntreGrupoPessoas && Usuario.ClienteFornecedor != null ? Usuario.ClienteFornecedor.Localidade.Codigo : 0,
                    UtilizarParametrizacaoDeHorarios = (Usuario?.ClienteFornecedor?.GrupoPessoas?.UtilizarParametrizacaoDeHorariosNoAgendamento ?? false),
                    ExibirOpcaoMultiModalAgendamentoColeta = configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta,
                    SugerirDataEntregaAgendamentoColeta = configuracaoJanelaCarregamento.SugerirDataEntregaAgendamentoColeta,
                    HabilitarCancelamentoAgendamentoColeta = !(Usuario.ClienteFornecedor?.DesabilitarCancelamentoAgendamentoColeta ?? false),
                    PermitirTransportadorCadastrarAgendamentoColeta = (configuracaoAgendamentoColeta?.PermitirTransportadorCadastrarAgendamentoColeta ?? false),
                    GerarAgendamentoPedidosExistentes = modalidadeFornecedorPessoas?.GerarAgendamentoSomentePedidosExistentes ?? false,
                    ConsultarSomenteTransportadoresPermitidosCadastro = configuracaoAgendamentoColeta?.ConsultarSomenteTransportadoresPermitidosCadastro ?? false,
                    CalcularDataDeEntregaPorTempoDeDescargaDaRota = configuracaoAgendamentoColeta?.CalcularDataDeEntregaPorTempoDeDescargaDaRota ?? false,
                    TempoPadraoDeDescargaMinutos = configuracaoAgendamentoColeta?.TempoPadraoDeDescargaMinutos ?? 0,
                });

                return View();
            }
        }

        [CustomAuthorize("Logistica/CentroCarregamento")]
        public async Task<IActionResult> CentroCarregamento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                var configuracoes = new
                {
                    configuracaoGestaoPatio.InformarDocaCarregamentoUtilizarLocalCarregamento,
                    configuracaoMontagemCarga.UtilizarDataPrevisaoSaidaVeiculo,
                    configuracaoJanelaCarregamento.BloquearGeracaoJanelaParaCargaRedespacho,
                    PossuiIntegracaoKlios = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Klios),
                };

                ViewBag.Configuracoes = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoes);

                return View();
            }
        }

        [CustomAuthorize("Logistica/CentroDescarregamento")]
        public async Task<IActionResult> CentroDescarregamento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                ViewBag.ConfiguracoesCentroDescarregamento = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    SugerirDataEntregaAgendamentoColeta = configuracaoJanelaCarregamento.SugerirDataEntregaAgendamentoColeta,
                    configuracaoJanelaCarregamento.BloquearGeracaoJanelaParaCargaRedespacho,
                    PossuiIntegracaoSAD = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAD),
                    configuracaoJanelaCarregamento.UtilizarCentroDescarregamentoPorTipoCarga
                });

                return View();
            }
        }

        [CustomAuthorize("Logistica/MotivoParadaCentro")]
        public async Task<IActionResult> MotivoParadaCentro()
        {
            return View();
        }

        [CustomAuthorize("Logistica/RotaFrete")]
        public async Task<IActionResult> RotaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            bool semParar = false;

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                ViewBag.ConfiguracaoControleEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    ObrigatorioInformarFreetime = configuracaoControleEntrega?.ObrigatorioInformarFreetime ?? false
                });

                ViewBag.ConfiguracaoRotaFrete = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    OrdenarLocalidades = configuracaoRoteirizacao.OrdenarLocalidades,
                    configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga,
                    configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade,
                    ExigirRotaRoteirizadaNaCarga = configuracao?.ExigirRotaRoteirizadaNaCarga ?? false
                });

                semParar = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar) != null;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            ViewBag.PossuiSemParar = semParar ? "true" : "false";

            return View();
        }

        [CustomAuthorize("Logistica/PrazoSituacaoCarga")]
        public async Task<IActionResult> PrazoSituacaoCarga()
        {
            return View();
        }

        [CustomAuthorize("Logistica/JanelaCarregamento")]
        public async Task<IActionResult> JanelaCarregamento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.ConfiguracoesJanelaCarregamento = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoJanelaCarregamento.ExibirOpcaoLiberarParaTransportador,
                    configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores,
                    configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade,
                    PermiteSelecionarHorarioEncaixe = operadorLogistica?.PermiteSelecionarHorarioEncaixe ?? false,
                    PossuiIntegracaoKlios = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Klios),
                    PossuiIntegracaoHUBOfertas = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.HUB),
                });

                return View();
            }
        }

        [CustomAuthorize("Logistica/JanelaDescarga")]
        public async Task<IActionResult> JanelaDescarga()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento", "Cargas/MontagemCarga");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork).BuscarPrimeiroRegistro();

                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

                ViewBag.ConfiguracoesJanelaDescarga = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    PermiteExcluirAgendamentoDaCargaJanelaDescarga = configuracaoGeralCarga?.PermiteExcluirAgendamentoDaCargaJanelaDescarga ?? false,
                    PossuiIntegracaoBoticario = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario),
                    NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento = configuracaoJanelaCarregamento.NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento
                });

                return View();
            }
        }

        [CustomAuthorize("Logistica/ConsultaDisponibilidadeCarregamento")]
        public async Task<IActionResult> ConsultaDisponibilidadeCarregamento()
        {
            return View();
        }

        //[CustomAuthorize("Logistica/JanelaCarregamentoTransportador")]
        [AllowAnonymous]
        public async Task<IActionResult> JanelaCarregamentoTransportador()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoConfiguracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                bool possuiSituacaoAguardandoAceite = repositorioCargaJanelaCarregamentoTransportador.PossuiSituacaoAguardandoAceite(Usuario.Empresa.Codigo);

                var configuracoes = new
                {
                    configuracaoEmbarcador.ExibirValorDetalhadoJanelaCarregamentoTransportador,
                    PossuiSituacaoAguardandoAceite = possuiSituacaoAguardandoAceite,
                    configuracaoConfiguracaoJanelaCarregamento.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga
                };

                ViewBag.Configuracoes = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoes);
            }

            return View();
        }

        [CustomAuthorize("Logistica/Guarita")]
        public async Task<IActionResult> Guarita()
        {
            //Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_conexao.StringConexao);
            //Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            ViewBag.InformarDadosChegadaVeiculoNoFluxoPatio = ConfiguracaoEmbarcador.InformarDadosChegadaVeiculoNoFluxoPatio ? "true" : "false";

            return View();
        }

        [CustomAuthorize("Logistica/Expedicao")]
        public async Task<IActionResult> Expedicao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ReservaCargaGrupoPessoa")]
        public async Task<IActionResult> ReservaCargaGrupoPessoa()
        {
            return View();
        }

        [CustomAuthorize("Logistica/JanelaDescarregamento")]
        public async Task<IActionResult> JanelaDescarregamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/JanelaDescarregamentoSituacao")]
        public async Task<IActionResult> JanelaDescarregamentoSituacao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/GuaritaTMS")]
        public async Task<IActionResult> GuaritaTMS()
        {
            return View();
        }

        [CustomAuthorize("Logistica/HistoricoParadas")]
        public async Task<IActionResult> HistoricoParadas()
        {
            return View();
        }

        [CustomAuthorize("Logistica/FilaCarregamento")]
        public async Task<IActionResult> FilaCarregamento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Logistica/FilaCarregamento");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
                ViewBag.ConfiguracoesFilaCarregamento = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoEmbarcador.UtilizarFilaCarregamentoReversa,
                    configuracaoEmbarcador.ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga,
                    configuracaoGeralCarga.UtilizarProgramacaoCarga,
                    configuracaoFilaCarregamento.DiasFiltrarDataProgramada,
                    configuracaoFilaCarregamento.InformarAreaCDAdicionarVeiculo
                });

                return View();
            }
        }

        [CustomAuthorize("Logistica/MotivoPunicaoVeiculo")]
        public async Task<IActionResult> MotivoPunicaoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Logistica/PunicaoVeiculo")]
        public async Task<IActionResult> PunicaoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MotivoRetiradaFilaCarregamento")]
        public async Task<IActionResult> MotivoRetiradaFilaCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/RestricaoRodagem")]
        public async Task<IActionResult> RestricaoRodagem()
        {
            return View();
        }

        [CustomAuthorize("Logistica/PracaPedagio")]
        public async Task<IActionResult> PracaPedagio()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MarcacaoFilaCarregamento")]
        public async Task<IActionResult> MarcacaoFilaCarregamento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                ViewBag.ConfiguracoesMarcacaoFilaCarregamento = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoEmbarcador.MarcacaoFilaCarregamentoSomentePorVeiculo
                });

                return View();
            }
        }

        [CustomAuthorize("Logistica/MotivoSelecaoMotoristaForaOrdem")]
        public async Task<IActionResult> MotivoSelecaoMotoristaForaOrdem()
        {
            return View();
        }

        [CustomAuthorize("Logistica/FilaCarregamentoNotificacao")]
        public async Task<IActionResult> FilaCarregamentoNotificacao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ImportacaoCentroCarregamentoVeiculo")]
        public async Task<IActionResult> ImportacaoCentroCarregamentoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AcompanhamentoFilaCarregamentoReversa")]
        public async Task<IActionResult> AcompanhamentoFilaCarregamentoReversa()
        {
            return View();
        }

        [CustomAuthorize("Logistica/CategoriaResponsavel")]
        public async Task<IActionResult> CategoriaResponsavel()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MonitoramentoEvento")]
        public async Task<IActionResult> MonitoramentoEvento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/Monitoramento")]
        public async Task<IActionResult> Monitoramento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasMonitoramento = ObterPermissoesPersonalizadas("Logistica/Monitoramento");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasControleEntrega = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");

                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
                ViewBag.PermissoesPersonalizadasControleEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasControleEntrega);
                ViewBag.PermissoesPersonalizadasMonitoramento = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasMonitoramento);

                bool portalCliente = IsLayoutClienteAtivo(unitOfWork);
                string caminhoBaseViews = "~/Views/Logistica";
                string caminhosViewMasterLayout = portalCliente ? $"{caminhoBaseViews}/MonitoramentoCliente.cshtml" : $"{caminhoBaseViews}/Monitoramento.cshtml";

                return View(caminhosViewMasterLayout);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [CustomAuthorize("Logistica/MonitoramentoNovo")]
        public async Task<IActionResult> MonitoramentoNovo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasMonitoramento = ObterPermissoesPersonalizadas("Logistica/Monitoramento");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasMonitoramentoNovo = ObterPermissoesPersonalizadas("Logistica/MonitoramentoNovo");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasControleEntrega = ObterPermissoesPersonalizadas("Cargas/ControleEntrega");

                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
                ViewBag.PermissoesPersonalizadasControleEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasControleEntrega);
                ViewBag.PermissoesPersonalizadasMonitoramento = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasMonitoramento);
                ViewBag.PermissoesPersonalizadasMonitoramentoNovo = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasMonitoramentoNovo);

                bool portalCliente = IsLayoutClienteAtivo(unitOfWork);
                string caminhoBaseViews = "~/Views/Logistica";
                string caminhosViewMasterLayout = portalCliente ? $"{caminhoBaseViews}/MonitoramentoCliente.cshtml" : $"{caminhoBaseViews}/MonitoramentoNovo.cshtml";

                return View(caminhosViewMasterLayout);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [CustomAuthorize("Logistica/Locais")]
        public async Task<IActionResult> Locais()
        {
            return View();
        }

        [CustomAuthorize("Logistica/PosicaoDaFrotaMapa")]
        public async Task<IActionResult> PosicaoDaFrotaMapa()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AlertaTratativaAcao")]
        public async Task<IActionResult> AlertaTratativaAcao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AreaVeiculo")]
        public async Task<IActionResult> AreaVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ManobraAcao")]
        public async Task<IActionResult> ManobraAcao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/Manobra")]
        public async Task<IActionResult> Manobra()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ManobraTracao")]
        public async Task<IActionResult> ManobraTracao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AreaVeiculoPosicao")]
        public async Task<IActionResult> AreaVeiculoPosicao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/FilaCarregamentoReversa")]
        public async Task<IActionResult> FilaCarregamentoReversa()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ControleCarregamento")]
        public async Task<IActionResult> ControleCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MonitoramentoPosicao")]
        public async Task<IActionResult> MonitoramentoPosicao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MotivoAlteracaoPosicaoFilaCarregamento")]
        public async Task<IActionResult> MotivoAlteracaoPosicaoFilaCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/PosicaoAtual")]
        public async Task<IActionResult> PosicaoAtual()
        {
            return View();
        }

        [CustomAuthorize("Logistica/JanelaColeta")]
        public async Task<IActionResult> JanelaColeta()
        {
            return View();
        }

        [CustomAuthorize("Logistica/PosicaoFrota")]
        public async Task<IActionResult> PosicaoFrota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                bool portalCliente = IsLayoutClienteAtivo(unitOfWork);
                string caminhoBaseViews = "~/Views/Logistica";
                string caminhosViewMasterLayout = portalCliente ? $"{caminhoBaseViews}/PosicaoFrotaCliente.cshtml" : $"{caminhoBaseViews}/PosicaoFrota.cshtml";

                return View(caminhosViewMasterLayout);

                //return View();
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [CustomAuthorize("Logistica/PosicaoFrota2")]
        public async Task<IActionResult> PosicaoFrota2()
        {
            return View();
        }

        [CustomAuthorize("Logistica/TermoQuitacao")]
        public async Task<IActionResult> TermoQuitacao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/RegraAutorizacaoTermoQuitacao")]
        public async Task<IActionResult> RegraAutorizacaoTermoQuitacao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AutorizacaoTermoQuitacao")]
        public async Task<IActionResult> AutorizacaoTermoQuitacao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/RotaFreteClassificacao")]
        public async Task<IActionResult> RotaFreteClassificacao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/TipoSubareaCliente")]
        public async Task<IActionResult> TipoSubareaCliente()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MonitoramentoStatusViagem")]
        public async Task<IActionResult> MonitoramentoStatusViagem()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MonitoramentoGrupoStatusViagem")]
        public async Task<IActionResult> MonitoramentoGrupoStatusViagem()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ExcecaoCapacidadeCarregamento")]
        public async Task<IActionResult> ExcecaoCapacidadeCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/CapacidadeCarregamentoAdicional")]
        public async Task<IActionResult> CapacidadeCarregamentoAdicional()
        {
            return View();
        }

        [CustomAuthorize("Logistica/CapacidadeDescarregamentoAdicional")]
        public async Task<IActionResult> CapacidadeDescarregamentoAdicional()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ConfiguracaoRotaFrete")]
        public async Task<IActionResult> ConfiguracaoRotaFrete()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                ViewBag.ConfiguracaoRotaFrete = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga,
                    configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade,
                    PossuiIntegracaoHUBOfertas = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.HUB)

                });

                return View();
            }
        }

        [CustomAuthorize("Logistica/ExcecaoCapacidadeDescarregamento")]
        public async Task<IActionResult> ExcecaoCapacidadeDescarregamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ControleViagem")]
        public async Task<IActionResult> ControleViagem()
        {
            return View();
        }

        [CustomAuthorize("Logistica/Pesagem")]
        public async Task<IActionResult> Pesagem()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ExclusividadeCarregamento")]
        public async Task<IActionResult> ExclusividadeCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/InformacaoDescarga")]
        public async Task<IActionResult> InformacaoDescarga()
        {
            return View();
        }

        [CustomAuthorize("Logistica/DiariaAutomatica")]
        public async Task<IActionResult> DiariaAutomatica()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AlertaMonitoramento")]
        public async Task<IActionResult> AlertaMonitoramento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/TorreMonitoramento")]
        public async Task<IActionResult> TorreMonitoramento()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasMonitoramento = ObterPermissoesPersonalizadas("Logistica/Monitoramento");
            string permissoesPersonalizadasMonitoramentoJson = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasMonitoramento);
            ViewBag.PermissoesPersonalizadasControleEntrega = permissoesPersonalizadasMonitoramentoJson;
            ViewBag.PermissoesPersonalizadasMonitoramento = permissoesPersonalizadasMonitoramentoJson;

            return View();
        }

        [CustomAuthorize("Logistica/JanelaCarregamentoSituacao")]
        public async Task<IActionResult> JanelaCarregamentoSituacao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/JanelaDescargaSituacao")]
        public async Task<IActionResult> JanelaDescargaSituacao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MonitoramentoTecnologia")]
        public async Task<IActionResult> MonitoramentoTecnologia()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ImportacaoProgramacaoColeta")]
        public async Task<IActionResult> ImportacaoProgramacaoColeta()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MotivoReagendamento")]
        public async Task<IActionResult> MotivoReagendamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/MotivoAtrasoCarregamento")]
        public async Task<IActionResult> MotivoAtrasoCarregamento()
        {
            return View();
        }
        [CustomAuthorize("Logistica/VistoriaCheckList")]
        public async Task<IActionResult> VistoriaCheckList()
        {
            return View();
        }
        [CustomAuthorize("Logistica/CentroDistribuicao")]
        public async Task<IActionResult> CentroDistribuicao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/TrechoBalsa")]
        public async Task<IActionResult> TrechoBalsa()
        {
            return View();
        }

        [CustomAuthorize("Logistica/JanelaCarregamentoIntegracao")]
        public async Task<IActionResult> JanelaCarregamentoIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Logistica/ConfigFiliaisFilaCarregamento")]
        public async Task<IActionResult> ConfigFiliaisFilaCarregamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/JustificativaCancelamentoAgendamento")]
        public async Task<IActionResult> JustificativaCancelamentoAgendamento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/RotaFreteAbastecimento")]
        public async Task<IActionResult> RotaFreteAbastecimento()
        {
            return View();
        }

        [CustomAuthorize("Logistica/CentroCustoViagem")]
        public async Task<IActionResult> CentroCustoViagem()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                bool possuiIntegracaoOpenTech = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech) != null;
                bool possuiIntegracaoRepomFrete = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.RepomFrete) != null;

                ViewBag.CentroCustoViagem = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    possuiIntegracaoOpenTech,
                    possuiIntegracaoRepomFrete
                });

                return View();
            }

        }

        [CustomAuthorize("Logistica/AssociacaoBalsa")]
        public ActionResult AssociacaoBalsa()
        {
            return View();
        }

        [CustomAuthorize("Logistica/AcompanhamentoChecklist")]
        public async Task<IActionResult> AcompanhamentoChecklist()
        {
            return View();
        }

        [CustomAuthorize("Logistica/GrupoMotoristas")]
        public async Task<IActionResult> GrupoMotoristas()
        {
            return View();
        }
    }
}
