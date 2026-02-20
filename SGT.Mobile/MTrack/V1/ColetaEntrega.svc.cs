using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotificacaoMobile;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.NovoApp.ColetaEntrega;
using Dominio.ObjetosDeValor.NovoApp.Comum;
using Servicos.Embarcador.Canhotos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ColetaEntrega" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ColetaEntrega.svc or ColetaEntrega.svc.cs at the Solution Explorer and start debugging.
    public class ColetaEntrega : BaseControllerNovoApp, IColetaEntrega
    {
        #region Métodos Públicos

        public ResponseBool Confirmar(RequestConfirmar request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando Confirmar - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                //Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = configuracaoTMS.UtilizaAppTrizy && !string.IsNullOrEmpty(request.IdTrizy) ? repCargaEntrega.BuscarPorIdTrizy(request.IdTrizy) : repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);

                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);

                if (cargaEntrega == null)
                {
                    Servicos.Log.TratarErro($"NovoApp - Confirmar - CargaEntrega não localizada");
                    throw new ServicoException("Entrega não localizada, sincronize e tente novamente.");
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);

                List<SituacaoEntrega> listaSituacaoEmAberto = SituacaoEntregaHelper.ObterListaSituacaoEntregaEmAberto();
                bool permitirRetificarColeta = cargaEntrega.Coleta && request.motivoRetificacao > 0 && (cargaEntrega.Carga.TipoOperacao?.PermiteRetificarMobile ?? false);//Aurora retifica coleta já confirmada, deve permitir confirmar mesmo entregue
                bool entregaAgAtendimentoTrizy = cargaEntrega.Situacao == SituacaoEntrega.AgAtendimento && configuracaoTMS.UtilizaAppTrizy;

                if (!listaSituacaoEmAberto.Contains(cargaEntrega.Situacao) && !permitirRetificarColeta)
                {
                    if (cargaEntrega.Situacao == SituacaoEntrega.Entregue)
                    {
                        //sucesso
                        Servicos.Log.TratarErro($"ConfirmarEntrega - CargaEntrega finalizada");
                        return new ResponseBool
                        {
                            Sucesso = true,
                        };
                    }
                    else if (!entregaAgAtendimentoTrizy)
                    {
                        Servicos.Log.TratarErro($"ConfirmarEntrega - Situação da entrega " + cargaEntrega.Situacao.ObterDescricao() + " não permite confirmar.");
                        throw new ServicoException($"ConfirmarEntrega - Situação da entrega " + cargaEntrega.Situacao.ObterDescricao() + " não permite confirmar.");
                    }
                }

                unitOfWork.Start();

                if (request.dadosRic != null && cargaEntrega?.Carga != null)
                {
                    Dominio.Entidades.Usuario usuario = motorista;
                    if (usuario == null)
                        usuario = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                    if (usuario == null)
                    {
                        Servicos.Log.TratarErro($"NovoApp - Confirmar - Não foi possível identificar o usuário");
                        return new ResponseBool { Sucesso = false };
                    }
                    if (usuario.Empresa == null)
                    {
                        Servicos.Log.TratarErro($"NovoApp - Confirmar - Não foi possível identificar a Empresa do usuário cod.: " + usuario.Codigo.ToString());
                        return new ResponseBool { Sucesso = false };
                    }
                    bool sucessoAoVincularRic = VincularRicNaCarga(unitOfWork, request.dadosRic, cargaEntrega?.Carga, usuario);
                    if (!sucessoAoVincularRic)
                    {
                        Servicos.Log.TratarErro($"NovoApp - Confirmar - Erro ao vincular RIC na Carga.");
                        unitOfWork.Rollback();
                        return new ResponseBool { Sucesso = false };
                    }
                }

                // Parse datas
                DateTime dataInicioColetaEntrega = DateTime.Now;
                DateTime dataConfirmacaoChegada = Utilidades.DateTime.FromUnixSeconds(request.dataConfirmacaoChegada) ?? DateTime.MinValue;
                DateTime dataTerminoColetaEntrega = Utilidades.DateTime.FromUnixSeconds(request.dataTermino) ?? DateTime.Now;

                DateTime dataConfirmacao = Utilidades.DateTime.FromUnixSeconds(request.dataConfirmacao) ?? DateTime.Now;

                if (dataConfirmacaoChegada != DateTime.MinValue)
                    dataInicioColetaEntrega = Utilidades.DateTime.FromUnixSeconds(request.dataInicio) ?? dataConfirmacaoChegada;
                else
                    dataInicioColetaEntrega = Utilidades.DateTime.FromUnixSeconds(request.dataInicio) ?? dataConfirmacao;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS(unitOfWork);

                // Convertendo as coordenadas para waypoints
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoColetaEntrega = null;
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada = null;
                if (!(cargaEntrega?.Carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                {
                    wayPointConfirmacaoColetaEntrega = request.coordenadaConfirmacao.ConverterParaWayPoint();
                    wayPointConfirmacaoChegada = request.coordenadaConfirmacaoChegada?.ConverterParaWayPoint();
                }

                DateTime? dataConfirmacaoChegadaAux = dataConfirmacaoChegada;
                if (configuracaoTMS?.RegistrarChegadaAppEmMetodoDiferenteDoConfirmar ?? false)
                    dataConfirmacaoChegadaAux = null;

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametrosFinalizarEntrega = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
                {
                    cargaEntrega = cargaEntrega,
                    dataInicioEntrega = dataInicioColetaEntrega,
                    dataTerminoEntrega = dataTerminoColetaEntrega,
                    dataConfirmacao = dataConfirmacao,

                    dataSaidaRaio = null,
                    wayPoint = wayPointConfirmacaoColetaEntrega,

                    pedidos = (from o in request.pedidos select o.ConverterParaPedidoMobileAntigo()).ToList(),
                    motivoRetificacao = request.motivoRetificacao,
                    motivoFalhaNotaFiscal = request.motivoFalhaNotaFiscal,
                    justificativaEntregaForaRaio = request.justificativaEntregaForaRaio,
                    motivoFalhaGTA = request.motivoFalhaGTA,
                    configuracaoEmbarcador = configuracao,
                    tipoServicoMultisoftware = TipoServicoMultisoftware,
                    sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                    dadosRecebedor = request.dadosRecebedor?.ConverterParaDadosRecebedorAntigo(),

                    // Confirmação de chegada
                    dataConfirmacaoChegada = dataConfirmacaoChegadaAux,
                    wayPointConfirmacaoChegada = wayPointConfirmacaoChegada,

                    // Documentos
                    handlingUnitIds = request.handlingUnitIds,
                    chavesNFe = request.chavesNFe,

                    // Avaliação da coleta/entrega
                    avaliacaoColetaEntrega = request.avaliacaoColetaEntrega,

                    motorista = motorista,
                    observacao = request.observacao,

                    OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App,
                    auditado = ObterAuditado(motorista),
                    configuracaoControleEntrega = configuracaoControleEntrega,
                    tipoOperacaoParametro = tipoOperacaoParametro,
                    TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                };

                //bool possuiNFAgReentrega = false;
                //if (repositorioXMLNotaFiscal.ExisteNotasFiscaisPorEntrega(request.codigoCargaEntrega, null, SituacaoNotaFiscal.AgReentrega))//Remover finalizando a tarefa #55267
                //{
                //    possuiNFAgReentrega = true;
                //    Servicos.Log.TratarErro($"Alterando situação para Entregue da cargaEntrega {request.codigoCargaEntrega} ws", "FinalizarEntrega");
                //}

                Servicos.Log.TratarErro($"Inicia finalizar pelo Confirmar do ColetaEntrega.svc {request.codigoCargaEntrega}", "FinalizarEntrega");
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametrosFinalizarEntrega, unitOfWork);
                Servicos.Log.TratarErro($"Passou pelo finalizar no Confirmar do ColetaEntrega.svc {request.codigoCargaEntrega}", "FinalizarEntrega");

                if (cargaEntrega?.Carga != null)
                {
                    Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo servicoOcorrenciaAutomaticaPorPeriodo = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking tipoAplicacaoGatilhoTracking = cargaEntrega.Coleta ? TipoAplicacaoGatilhoTracking.Coleta : TipoAplicacaoGatilhoTracking.Entrega;

                    if (cargaEntrega.Fronteira)
                        servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, cargaEntrega.Cliente.CPF_CNPJ, GatilhoFinalTraking.SaidaFronteira, dataConfirmacaoChegada, dataConfirmacao, TipoServicoMultisoftware, Cliente, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                    else if (cargaEntrega.Parqueamento)
                        servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, cargaEntrega.Cliente.CPF_CNPJ, GatilhoFinalTraking.SaidaParqueamento, dataConfirmacaoChegada, dataConfirmacao, TipoServicoMultisoftware, Cliente, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                    else
                        servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, codigoFronteiraParqueamento: 0d, GatilhoFinalTraking.FimEntrega, dataInicioColetaEntrega, dataTerminoColetaEntrega, TipoServicoMultisoftware, Cliente, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                }

                unitOfWork.CommitChanges();

                //if (repositorioXMLNotaFiscal.ExisteNotasFiscaisPorEntrega(request.codigoCargaEntrega, null, SituacaoNotaFiscal.AgReentrega))//Remover finalizando a tarefa #55267
                //    Servicos.Log.TratarErro($"Não atualizou situação das notas fiscais para Entregue da cargaEntrega {request.codigoCargaEntrega} - ws", "FinalizarEntrega");
                //else if (possuiNFAgReentrega)
                //    Servicos.Log.TratarErro($"Atualizou situação das notas fiscais da cargaEntrega {request.codigoCargaEntrega} - ws", "FinalizarEntrega");

                // Notifica para outros motoristas da carga que uma ação foi realizada
                if (motorista != null)
                {
                    Servicos.Embarcador.Chamado.NotificacaoMobile serNotificaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, 0);
                    serNotificaoMobile.NotificarCargaAtualizadaPorOutroMotorista(cargaEntrega?.Carga, cargaEntrega, motorista, TipoEventoAlteracaoCargaPorOutroMotorista.FinalizacaoEntregaColeta);
                }

                Servicos.Log.TratarErro($"NovoApp - Confirmar - Dados salvos com sucesso");
                Servicos.Log.TratarErro($"Finalizou a entrega pelo finalizar no Confirmar do ColetaEntrega.svc {request.codigoCargaEntrega}", "FinalizarEntrega");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (WebFaultException webEx)
            {
                Servicos.Log.TratarErro(webEx);
                throw;
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                RetornarErro("Não foi possível confirmar entrega, sincronize e tente novamente", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseCriarOcorrencia CriarOcorrencia(RequestCriarOcorrencia request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando CriarOcorrencia - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}", "CriarOcorrenciaMultiMobile");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);

                // Parse datas
                DateTime dataRejeicao = Utilidades.DateTime.FromUnixSeconds(request.data) ?? DateTime.Now;
                DateTime dataConfirmacaoChegada = Utilidades.DateTime.FromUnixSeconds(request.dataConfirmacaoChegada) ?? DateTime.MinValue;
                DateTime dataInicio = Utilidades.DateTime.FromUnixSeconds(request.dataInicio) ?? DateTime.MinValue;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = configuracao.UtilizaAppTrizy && !string.IsNullOrEmpty(request.IdTrizy) ? repCargaEntrega.BuscarPorIdTrizy(request.IdTrizy) : repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);

                if (cargaEntrega == null)
                {
                    Servicos.Log.TratarErro($"NovoApp - CriarOcorrencia - Carga entrega não localizada");
                    throw new ServicoException($"Entrega não localizada");
                }

                if (SituacaoEntregaHelper.ObterSituacaoEntregaFinalizada(cargaEntrega.Situacao) && configuracao.UtilizaAppTrizy)
                    throw new ServicoException($"Entrega já confirmada.");

                if (cargaEntrega.Carga.DataFimViagem.HasValue)
                    throw new ServicoException($"Não permitido abrir Ocorrência, carga já finalizada.");

                // Convertendo as coordenadas para waypoints
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointRejeicaoColetaEntrega = null;
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada = null;
                if (!(cargaEntrega.Carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                {
                    wayPointRejeicaoColetaEntrega = request.coordenadaRejeicao.ConverterParaWayPoint();
                    wayPointConfirmacaoChegada = request.coordenadaConfirmacaoChegada?.ConverterParaWayPoint();
                }

                int codigoMotivoRejeicao = request.codigoMotivoRejeicao;

                if (configuracao.UtilizaAppTrizy && !string.IsNullOrWhiteSpace(request.IdMotivoOcorrenciaTrizy))
                {
                    Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigoIntegracaoAuxiliar(request.IdMotivoOcorrenciaTrizy);
                    if (tipoOcorrencia != null)
                        codigoMotivoRejeicao = tipoOcorrencia.Codigo;
                }

                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros
                {
                    codigoCargaEntrega = cargaEntrega?.Codigo ?? 0,
                    codigoMotivo = codigoMotivoRejeicao,
                    data = dataRejeicao,
                    wayPoint = wayPointRejeicaoColetaEntrega,
                    usuario = null,
                    motivoRetificacao = request.motivoRetificacao,
                    tipoServicoMultisoftware = TipoServicoMultisoftware,
                    observacao = request.observacao,
                    configuracao = configuracao,
                    devolucaoParcial = request.devolucaoParcial,
                    notasFiscais = (from o in request.notasFiscais select o.ConverterParaProdutoMobileAntigo()).ToList(),
                    motivoFalhaGTA = request.motivoFalhaGTA,
                    apenasRegistrar = false,
                    dadosRecebedor = request.dadosRecebedor?.ConverterParaDadosRecebedorAntigo(),
                    permitirEntregarMaisTarde = false,
                    dataConfirmacaoChegada = dataConfirmacaoChegada,
                    wayPointConfirmacaoChegada = wayPointConfirmacaoChegada,
                    atendimentoRegistradoPeloMotorista = true,
                    OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App,
                    dataInicioCarregamento = dataInicio, // Isso é meio estranho, mas é como já tava funcionando antes. Acontece.
                    quantidadeImagens = request.quantidadeImagens,
                    imagens = request.imagens,
                    clienteMultisoftware = usuarioMobileCliente.Cliente,
                    valorChamado = request.valorChamado
                };

                Auditado auditado = ObterAuditado(motorista);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, auditado, unitOfWork, out Dominio.Entidades.Embarcador.Chamados.Chamado chamado, TipoServicoMultisoftware);

                if (chamado != null)
                {
                    if (chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado)
                    {
                        int codigoChamado = chamado.Codigo;
                        string stringConexao = unitOfWork.StringConexao;
                        Task t = Task.Factory.StartNew(() => { Servicos.Embarcador.Chamado.Chamado.EnviarEmailCargaDevolucao(codigoChamado, stringConexao); });
                    }

                    if (!string.IsNullOrWhiteSpace(request?.IdOcorrenciaTrizy))
                    {
                        chamado.IdOcorrenciaTrizy = request.IdOcorrenciaTrizy;
                        repChamado.Atualizar(chamado);
                    }

                    Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);

                    AdicionarImagensChamado(chamado, request.imagens, unitOfWork, auditado);

                    new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoAberto(chamado, unitOfWork);
                }

                unitOfWork.CommitChanges();

                try
                {
                    //forçar mostrar alerta na tela de acompanhamento carga
                    servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargasAcompanamentoMSMQ(cargaEntrega?.Carga, Cliente.Codigo);
                }
                catch (Exception ex) //Tratamento para não retornar erro quando ocorrer algum erro de atualização do MSMQ.
                {
                    Servicos.Log.TratarErro(ex, "CriarOcorrenciaMultiMobile");
                }

                try
                {
                    // Notifica para outros motoristas da carga que uma ação foi realizada
                    if (motorista != null)
                    {
                        Servicos.Embarcador.Chamado.NotificacaoMobile serNotificaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, 0);
                        serNotificaoMobile.NotificarCargaAtualizadaPorOutroMotorista(cargaEntrega?.Carga, cargaEntrega, motorista, TipoEventoAlteracaoCargaPorOutroMotorista.RejeicaoEntregaColeta);
                    }
                }
                catch (Exception ex) //Tratamento para não retornar erro quando ocorrer algum erro de notificação dos motoristas.
                {
                    Servicos.Log.TratarErro(ex, "CriarOcorrenciaMultiMobile");
                }

                Servicos.Log.TratarErro($"NovoApp - CriarOcorrencia - Dados salvos com sucesso", "CriarOcorrenciaMultiMobile");

                return new ResponseCriarOcorrencia
                {
                    Codigo = chamado?.Codigo,
                    Numero = chamado?.Numero,
                };
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex, "CriarOcorrenciaMultiMobile");
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao criar a ocorrência", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseCriarOcorrencia
            {
                Codigo = null,
            };
        }

        public ResponseBool AtualizarDadosPosicionamento(RequestAtualizarDadosPosicionamento request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando AtualizarDadosPosicionamento - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork, true, true);

            try
            {
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioconfiguracaoTms = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioconfiguracaoTms.BuscarConfiguracaoPadrao();

                if (motorista == null) RetornarErro("Motorista não encontrado");

                if (!CargaEmMonitoramento(unitOfWork, request.codigoCarga))
                {
                    return new ResponseBool
                    {
                        Sucesso = true,
                    };
                }

                unitOfWork.Start();

                Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCargaNoMonitoramentoOuMotorista(unitOfWork, request.codigoCarga, motorista?.Codigo ?? 0);
                Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao repPosicaoPendenteIntegracao = new Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao(unitOfWork);

                if (veiculo != null)
                {
                    foreach (CoordenadaApp coordenada in request.coordenadas)
                    {
                        if (!DateTime.TryParse(coordenada.timestamp, out DateTime dataPosicao))
                            continue;

                        Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao posicaoPendenteIntegracao = new Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao()
                        {
                            ID = dataPosicao.ToUnixMillseconds(),
                            Data = dataPosicao,
                            DataVeiculo = dataPosicao,
                            DataCadastro = DateTime.Now,
                            IDEquipamento = motorista.Codigo.ToString(),
                            Veiculo = veiculo,
                            Ignicao = 1,
                            Latitude = coordenada.coords.latitude,
                            Longitude = coordenada.coords.longitude,
                            Velocidade = (int)Math.Floor(coordenada.coords.speed * 3.6),
                            Temperatura = 0,
                            Descricao = $"{coordenada.coords.latitude}, {coordenada.coords.longitude} (M)",
                            NivelBateria = coordenada.battery.level,
                            NivelSinalGPS = 0
                        };
                        repPosicaoPendenteIntegracao.Inserir(posicaoPendenteIntegracao);
                    }
                }

                //VAMOS ATUALIZAR A LISTA DE MOTORISTAS QUE ESTAO NESTE CPF na qual estao com versao do APP diferente;
                if (!string.IsNullOrEmpty(usuarioMobileCliente.UsuarioMobile?.VersaoAPP ?? "") && !configuracaoTMS.UtilizaAppTrizy)
                    AtualizarVersaoAppMotoristasPorCPFEVersaoDiferente(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF, usuarioMobileCliente.UsuarioMobile.VersaoAPP);

                unitOfWork.CommitChanges();

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao atualizar os dados do posicionamento", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool EnviarImagem(RequestEnviarImagem request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando EnviarImagem - codigoCargaEntrega: {request.codigoCargaEntrega}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                byte[] buffer = System.Convert.FromBase64String(request.imagem);
                MemoryStream ms = new MemoryStream(buffer);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                    Usuario = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF)
                };

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(ms, unitOfWork, out string tokenImagem);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemEntrega(
                    request.clienteMultisoftware,
                    request.codigoCargaEntrega,
                    tokenImagem,
                    unitOfWork,
                    Utilidades.DateTime.FromUnixSeconds(request.coordenada.dataCoordenada) ?? DateTime.MinValue,
                    request.coordenada?.latitude,
                    request.coordenada?.longitude,
                    OrigemSituacaoEntrega.App,
                    true,
                    auditado
                );

                Servicos.Log.TratarErro($"Novo App - EnviarImagem - Imagem salva com sucesso");
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao enviar a imagem", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool EnviarCanhoto(RequestEnviarCanhoto request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando EnviarCanhoto - codigoCanhoto: {request.codigoCanhoto} | IdTrizy {request.IdTrizy} ", "EnviarCanhoto");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = configuracaoTMS.UtilizaAppTrizy && !string.IsNullOrEmpty(request.IdTrizy) ? repCanhoto.BuscarPorIdTrizy(request.IdTrizy) : repCanhoto.BuscarPorCodigo(request.codigoCanhoto);

                if (canhoto == null) throw new ServicoException("Não foi localizado um canhoto compatível com a entrega para a imagem informada");

                if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado ||
                    canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                {
                    Servicos.Log.TratarErro($"Novo App - EnviarCanhoto - Canhoto já foi digitalizado", "EnviarCanhoto");
                    return new ResponseBool
                    {
                        Sucesso = true,
                    };
                }
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente?.UsuarioMobile.CPF);
                canhoto.DataEnvioCanhoto = DateTime.Now;
                canhoto.Observacao = !string.IsNullOrWhiteSpace(request.observacao) ? request.observacao : "";
                canhoto.DataUltimaModificacao = DateTime.Now;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configuracaoIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unitOfWork).BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork).BuscarConfiguracaoPadrao();

                if (!request.devolvido)
                {
                    byte[] data = System.Convert.FromBase64String(request.imagem);
                    using MemoryStream ms = new MemoryStream(data);

                    unitOfWork.Start();

                    bool possuiIntegracaoComprovei = (configuracaoIntegracaoComprovei?.PossuiIntegracaoIACanhoto ?? false) && (configuracaoCanhoto?.IntegrarCanhotosComValidadorIAComprovei ?? false) && (!canhoto.XMLNotaFiscal?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.PossuiCanhotoDeDuasOuMaisPaginas ?? true);

                    if (!configuracao.ExigeAprovacaoDigitalizacaoCanhoto && (possuiIntegracaoComprovei || !request.requerAprovacao))
                    {
                        canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                        canhoto.DigitalizacaoIntegrada = false;
                        canhoto.ValidacaoViaOCR = true;//Se utiliza o leitor do mobile, define como validada pelo OCR também

                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, TipoServicoMultisoftware, this.Cliente, false);
                        Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracao, TipoServicoMultisoftware, this.Cliente, unitOfWork);
                        Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);
                    }
                    else
                    {
                        canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracao, unitOfWork);
                    }

                    canhoto.DataDigitalizacao = DateTime.Now;

                    if (canhoto.Carga?.TipoOperacao?.ConfiguracaoMobile?.ReplicarDataDigitalizacaoCanhotoDataEntregaCliente ?? false)
                        canhoto.DataEntregaNotaCliente = canhoto.DataDigitalizacao;

                    canhoto.UsuarioDigitalizacao = motorista;

                    string caminhoCanhoto = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);

                    string extensao = ".jpg";
                    canhoto.GuidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    canhoto.NomeArquivo = canhoto.Numero.ToString() + extensao;

                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhoto.GuidNomeArquivo + extensao);
                    using (Image canhotofile = Image.FromStream(ms))
                    using (Bitmap canhotoImagem = new Bitmap(canhotofile))
                    {
                        Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, canhotoImagem, ImageFormat.Jpeg);
                    }

                    canhoto.OrigemDigitalizacao = request.requerAprovacao ? CanhotoOrigemDigitalizacao.MobileSemValidacaoAut : CanhotoOrigemDigitalizacao.Mobile;
                    string tipoDigitalizacao = request.requerAprovacao ? "Mobile sem validação automática" : "Mobile";

                    serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, $"Imagem do Canhoto digitalizada via {tipoDigitalizacao} com o nome {canhoto.GuidNomeArquivo}", unitOfWork);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    {
                        serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, $"Falha ao salvar imagem do Canhoto", unitOfWork);
                        Servicos.Log.TratarErro($"Falha ao salvar imagem do Canhoto de id {canhoto.Codigo} e guid {canhoto.GuidNomeArquivo}. Imagem: {request.imagem} ", "EnviarCanhoto");
                    }
                }
                else
                {
                    canhoto.SituacaoCanhoto = SituacaoCanhoto.Cancelado;
                    serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, "Cancelado via devolução registrada no Mobile.", unitOfWork);
                    Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                }

                canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;
                canhoto.UsuarioDigitalizacao = motorista;
                Servicos.Auditoria.Auditoria.Auditar(ObterAuditado(motorista), canhoto, null, "Enviou imagem do canhoto", unitOfWork);
                repCanhoto.Atualizar(canhoto);
                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro($"Novo App - EnviarCanhoto - Imagem salva com sucesso", "EnviarCanhoto");
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (System.FormatException ex)
            {
                Servicos.Log.TratarErro(ex);
                Servicos.Log.TratarErro($"Número de caracteres do base64: {request.imagem?.Length}. Começo do base64: {request.imagem?.Substring(0, 50)}...", "EnviarCanhoto");
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao enviar o canhoto", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool EnviarAssinatura(RequestEnviarAssinatura request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando EnviarAssinatura - codigoCargaEntrega: {request.codigoCargaEntrega}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                byte[] buffer = System.Convert.FromBase64String(request.imagem);
                MemoryStream ms = new MemoryStream(buffer);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemAssinatura(ms, unitOfWork, out string guid);
                if (
                    !Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarAssinaturaProdutorColetaEntrega(
                        request.clienteMultisoftware,
                        request.codigoCargaEntrega, guid,
                        Utilidades.DateTime.FromUnixSeconds(request.dataEnvio) ?? DateTime.Now,
                        unitOfWork,
                        out string mensagemErro
                    )
                )
                {
                    throw new ServicoException(mensagemErro);
                }

                Servicos.Log.TratarErro($"Novo App - EnviarAssinatura - Imagem salva com sucesso");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao enviar a assinatura", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool MotoristaACaminho(RequestMotoristaACaminho request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando RequestMotoristaACaminho - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            Auditado auditado = new Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMobile,
                Usuario = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF)
            };

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                if (configuracaoTMS.UtilizaAppTrizy)
                {
                    if (string.IsNullOrWhiteSpace(request.IdTrizy))
                        throw new ServicoException($"Id trizzy é obrigatorio");


                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorIDIdentificacaoTrizy(request.IdTrizy);

                    if (carga == null) throw new ServicoException("Carga não foi encontrada");

                    if (request.aCaminho && !carga.DataPreViagemInicio.HasValue && configuracaoTMS.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem)
                    {
                        carga.DataPreViagemInicio = DateTime.Now;
                        Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracaoTMS, auditado, "Motorista sinalizou que está indo (PreTrip)", unitOfWork);
                        repCarga.Atualizar(carga);
                        AdicionarPosicaoProcessar(carga, request.latitude, request.longitude, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Motorista sinalizou que está indo (PreTrip)", unitOfWork);
                    }

                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);

                    if (cargaEntrega == null) RetornarErro("Entrega ou coleta não foi encontrada");

                    if (cargaEntrega.Situacao == SituacaoEntrega.Entregue || cargaEntrega.Situacao == SituacaoEntrega.Rejeitado)
                        throw new ServicoException($"Entrou ou coleta já encerrada.");

                    cargaEntrega.MotoristaACaminho = request.aCaminho;
                    repCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega servicoOcorrenciaEntrega = new Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega(unitOfWork);

                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(cargaEntrega, DateTime.Now, EventoColetaEntrega.EstouIndo, (decimal?)request.latitude, (decimal?)request.longitude, configuracaoTMS, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.App, configuracaoControleEntrega, auditado);

                    if (request.aCaminho)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, "Motorista sinalizou que está a caminho", unitOfWork);
                    else

                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, "Motorista cancelou a confirmação que está a caminho", unitOfWork);
                }


                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro($"NovoApp - RequestMotoristaACaminho - Dados salvos com sucesso");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao confirmar motorista à caminho", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool MotoristaChegouParada(RequestMotoristaChegouParada request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando MotoristaChegouParada - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);

                if (cargaEntrega == null) throw new ServicoException($"Entrega ou coleta não foi encontrada.");

                DateTime? dataChegou = Utilidades.DateTime.FromUnixSeconds(request.data);

                if (!dataChegou.HasValue) throw new ServicoException($"Data inválida.");

                cargaEntrega.DataEntradaRaio = dataChegou.Value;

                if (request.coordenada != null && request.coordenada.latitude != 0)
                    cargaEntrega.LatitudeConfirmacaoChegada = (decimal)request.coordenada.latitude;

                if (request.coordenada != null && request.coordenada.longitude != 0)
                    cargaEntrega.LongitudeConfirmacaoChegada = (decimal)request.coordenada.longitude;

                repCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                // Auditoria
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {

                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMobile,
                    Usuario = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF)

                };

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoTMS(unitOfWork);

                if (configuracaoEmbarcador?.UtilizaAppTrizy ?? false)
                    servMonitoramento.RegistrarPosicaoEventosRelevantesTrizy(cargaEntrega.Carga.Codigo, cargaEntrega.DataEntradaRaio ?? DateTime.Now, request.coordenada.latitude, request.coordenada.longitude, EventoRelevanteMonitoramento.ChegadaRaio, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, "Motorista sinalizou que chegou na parada às " + dataChegou.Value.ToString("dd/MM/yyyy HH:mm:ss"), unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro($"NovoApp - MotoristaChegouParada - Dados salvos com sucesso");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao confirmar que o motorista chegou na parada", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool InformarChegada(RequestInformarChegada request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando InformarChegada - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repositorioConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);

                Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();
                Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega servicoOcorrenciaEntrega = new Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega(unitOfWork);
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = configuracaoTMS.UtilizaAppTrizy && !string.IsNullOrEmpty(request.IdTrizy) ? repCargaEntrega.BuscarPorIdTrizy(request.IdTrizy) : repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);

                if (cargaEntrega == null) throw new ServicoException($"Entrega ou coleta não foi encontrada.");

                cargaEntrega.Initialize();

                DateTime? dataChegada = Utilidades.DateTime.FromUnixSeconds(request.dataConfirmacaoChegada);

                if (!dataChegada.HasValue) throw new ServicoException($"Data invalida.");

                cargaEntrega.DataEntradaRaio = dataChegada.Value;

                if (request.coordenadaConfirmacaoChegada != null && request.coordenadaConfirmacaoChegada.latitude != 0)
                    cargaEntrega.LatitudeConfirmacaoChegada = (decimal)request.coordenadaConfirmacaoChegada.latitude;

                if (request.coordenadaConfirmacaoChegada != null && request.coordenadaConfirmacaoChegada.longitude != 0)
                    cargaEntrega.LongitudeConfirmacaoChegada = (decimal)request.coordenadaConfirmacaoChegada.longitude;


                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMobile;

                bool existeConfiguracaoUltimaChegada = repositorioConfiguracaoOcorrenciaEntrega.ExisteConfiguracaoOcorrenciaPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.UltimaChegadaNoAlvo);

                if (existeConfiguracaoUltimaChegada)
                {
                    bool ultimaChegada = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.UltimaEntregaPendente(cargaEntrega.Carga, unitOfWork);
                    EventoColetaEntrega eventoASerExecutado = ultimaChegada ? EventoColetaEntrega.UltimaChegadaNoAlvo : EventoColetaEntrega.ChegadaNoAlvo;
                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(cargaEntrega, dataChegada.Value, eventoASerExecutado, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, configuracaoTMS, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.WebService, configuracaoControleEntrega, auditado);
                }
                else
                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(cargaEntrega, dataChegada.Value, EventoColetaEntrega.ChegadaNoAlvo, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, configuracaoTMS, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.WebService, configuracaoControleEntrega, auditado);

                repCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

                if (configuracaoTMS?.UtilizaAppTrizy ?? false)
                    servMonitoramento.RegistrarPosicaoEventosRelevantesTrizy(cargaEntrega.Carga.Codigo, cargaEntrega.DataEntradaRaio ?? DateTime.Now, (double?)cargaEntrega.LatitudeConfirmacaoChegada, (double?)cargaEntrega.LongitudeConfirmacaoChegada, EventoRelevanteMonitoramento.ChegadaRaio, unitOfWork);

                // Auditoria
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, "Chegada confirmada às " + dataChegada.Value.ToString("dd/MM/yyyy HH:mm:ss"), unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro($"NovoApp - InformarChegada - Dados salvos com sucesso");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao informar chegada", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool ReceberMensagemChat(RequestReceberMensagemChat request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando ReceberMensagemChat - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ChatMobileMensagem repMensagemChat = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorIDIdentificacaoTrizy(request.IdDaViagem);

                if (carga == null) throw new ServicoException($"Carga não foi encontrada.");
                if (string.IsNullOrWhiteSpace(request.mensagem)) throw new ServicoException($"Chat sem texto.");

                Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem mensagemChat = new Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem();

                mensagemChat.Carga = carga;
                mensagemChat.Mensagem = request.mensagem;
                mensagemChat.DataCriacao = DateTime.Now;
                mensagemChat.DataConfirmacaoLeitura = DateTime.Now;
                mensagemChat.Remetente = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                repMensagemChat.Inserir(mensagemChat);

                // Auditoria
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMobile;
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Mensagem recebida: " + mensagemChat.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"), unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Hubs.Chat hubChat = new Servicos.Embarcador.Hubs.Chat();
                hubChat.NotificarMensagemUsuario(0, mensagemChat.Remetente.Codigo, mensagemChat.Mensagem, mensagemChat.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"), mensagemChat.Remetente.Nome, true);

                Servicos.Log.TratarErro($"NovoApp - ReceberMensagemChat - Dados salvos com sucesso");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao receber mensagem no chat", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool EnviarImagemOcorrencia(RequestEnviarImagemOcorrencia request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando EnviarImagemOcorrencia - codigoOcorrencia: {request.codigoOcorrencia}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(request.codigoOcorrencia);

                if (chamado == null) throw new WebServiceException("Ocorrência não encontrada");

                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);
                Auditado auditado = ObterAuditado(motorista);

                AdicionarImagensChamado(chamado, new List<string>() { request.imagem }, unitOfWork, auditado);

                Servicos.Log.TratarErro($"Novo App - EnviarImagemOcorrencia - Imagem salva com sucesso");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao enviar imagem da ocorrência", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool GuiaTransporteAnimalColetaEntrega(RequestEnviarGuiaTransporteAnimal request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando GuiaTransporteAnimalColetaEntrega - codigoCargaEntrega: {request.codigoCargaEntrega}");

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {

                if (!Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarGTAColetaEntrega(request.codigoCargaEntrega, request.codigoBarras, request.numeroNF, request.serie, request.uf, request.quantidade, unitOfWork, out string mensagemErro))
                {
                    RetornarErro(mensagemErro);
                }
                else
                {
                    Servicos.Log.TratarErro($"Novo App - GuiaTransporteAnimalColetaEntrega - salvo com sucesso");
                    return new ResponseBool { Sucesso = true };
                }

            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao salvar o guia de transporte", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool { Sucesso = false };

        }

        public ResponseBool DeletarGuiaTransporteAnimal(RequestDeletarGuiaTransporteAnimal request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando DeletarGuiaTransporteAnimal - codigoCargaEntrega: {request.codigoCargaEntrega}, codigoBarrasGta: {request.codigoBarrasGta}");

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal repCargaEntregaGuiaTransporteAnimal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);

                if (cargaEntrega == null) RetornarErro("Parada não encontrada");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal gta = repCargaEntregaGuiaTransporteAnimal.BuscarPorCargaEntregaCodigoBarras(cargaEntrega.Codigo, request.codigoBarrasGta);
                if (gta != null)
                {
                    repCargaEntregaGuiaTransporteAnimal.Deletar(gta);

                    // Auditar
                    Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);
                    if (motorista != null)
                        Servicos.Auditoria.Auditoria.Auditar(ObterAuditado(motorista), cargaEntrega, null, $"GTA com código de barras {request.codigoBarrasGta} removida", unitOfWork);

                    Servicos.Log.TratarErro($"Novo App - DeletarGuiaTransporteAnimal - GTA deletada com sucesso");
                }
                else
                {
                    Servicos.Log.TratarErro($"Novo App - DeletarGuiaTransporteAnimal - Erro: GTA não encontrada");
                }

                return new ResponseBool { Sucesso = true };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao deletar o guia de transporte", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool { Sucesso = false };

        }

        public ResponseBool EnvioFotoNotaFiscal(RequestEnvioFotoNotaFiscal request)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            Servicos.Log.TratarErro($"Novo App - Iniciando EnvioFotoNotaFiscal - codigoCargaEntrega: {request.codigoCargaEntrega}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {

                DateTime dataEnvio = Utilidades.DateTime.FromUnixSeconds(request.data) ?? DateTime.Now;

                byte[] buffer = System.Convert.FromBase64String(request.imagem);
                MemoryStream ms = new MemoryStream(buffer);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemNotaFiscal(ms, unitOfWork, out string guid);

                if (!Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemNotaFiscalColetaEntrega(request.clienteMultisoftware, request.codigoCargaEntrega, guid, dataEnvio, unitOfWork, out string mensagemErro))
                {
                    RetornarErro(mensagemErro);
                }
                else
                {
                    Servicos.Log.TratarErro($"Novo App - EnvioFotoNotaFiscal - Imagem salva com sucesso");
                    return new ResponseBool { Sucesso = true };
                }

            }
            catch (BaseException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                RetornarErro(excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao enviar imagem da nota fiscal", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }

            return new ResponseBool { Sucesso = false };
        }

        /// <summary>
        /// Ser documentação da entidade CanhotoEsperandoVinculo para entender essa rota
        /// </summary>
        public ResponseBool EnviarImagemCanhotoSemVinculo(RequestEnviarImagemCanhotoSemVinculo request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando EnviarImagemCanhotoSemVinculo - codigoCargaEntrega: {request.codigoCargaEntrega} idTrizy: {request.IdTrizy}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = (configuracaoTMS?.UtilizaAppTrizy ?? false) ? repCargaEntrega.BuscarPorIdTrizy(request.IdTrizy) : repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);

                if (cargaEntrega == null) throw new WebServiceException("Entrega não encontrada");

                CanhotoEsperandoVinculo serCanhotoEsperandoVinculo = new CanhotoEsperandoVinculo(unitOfWork);
                serCanhotoEsperandoVinculo.Adicionar(request.imagem, cargaEntrega);

                Servicos.Log.TratarErro($"Novo App - EnviarImagemCanhotoSemVinculo - Imagem salva com sucesso");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (WebServiceException we)
            {
                Servicos.Log.TratarErro(we);
                RetornarErro(we.Message);
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao enviar imagem do canhoto", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool AtualizarCheckList(RequestAtualizarCheckList request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando AtualizarCheckList - codigoCargaEntrega: {request.codigoCargaEntrega}, respostas: {Newtonsoft.Json.JsonConvert.SerializeObject(request.respostas)}");

            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);

                TipoCheckList tipoChecklist = cargaEntrega.Coleta ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckList.Coleta : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckList.Entrega;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList checkList = repCargaEntregaCheckList.BuscarPorCargaEntrega(cargaEntrega?.Codigo ?? 0, tipoChecklist);

                if (checkList == null)
                    RetornarErro("Não foi possível encontrar CheckList da Coleta");

                unitOfWork.Start();

                new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork).SalvarRespostasCheckList(checkList, request.respostas);

                unitOfWork.CommitChanges();
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (WebServiceException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                RetornarErro(excecao.Message);
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                RetornarErro("Ocorreu uma falha ao atualizar o checklist", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public List<ResponseObterPontosApoio> ObterPontosApoio(int clienteMultisoftware)
        {
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);

                if (motorista == null)
                    RetornarErro("Motorista não encontrado");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarClientePontoApoio();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS(unitOfWork);

                List<ResponseObterPontosApoio> listaPontosApoio = new List<ResponseObterPontosApoio>();

                foreach (Dominio.Entidades.Cliente cliente in clientes)
                {
                    listaPontosApoio.Add(new ResponseObterPontosApoio
                    {
                        CodigoIntegracao = cliente.CodigoIntegracao ?? "",
                        RaioEmMetros = (cliente.RaioEmMetros.HasValue && cliente.RaioEmMetros.Value > 0) ? cliente.RaioEmMetros.Value : configuracao.RaioPadrao,
                        RazaoSocial = cliente.Descricao,
                        Endereco = new Endereco
                        {
                            Latitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(cliente.Latitude?.Replace(",", "") ?? ""),
                            Longitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(cliente.Longitude?.Replace(",", "") ?? ""),
                            Cidade = cliente.Localidade?.Descricao,
                            Uf = cliente.Localidade?.Estado.Descricao,
                        },
                        NomeFantasia = cliente.NomeFantasia,
                        Telefone = new Telefone
                        {
                            CodigoPais = cliente.Localidade?.Pais?.CodigoTelefonico.ToString() ?? string.Empty,
                            Numero = cliente.Telefone1 ?? string.Empty,
                        },
                    });
                }

                return listaPontosApoio;
            }
            catch (BaseException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                RetornarErro(excecao.Message);
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao obter os pontos de apoio", System.Net.HttpStatusCode.InternalServerError);
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }
        }

        public ResponseLancarDespesaViagem LancarDespesaViagem(RequestLancarDespesaViagem request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando LancarDespesaViagem - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);

                // Parse datas
                DateTime dataRejeicao = Utilidades.DateTime.FromUnixSeconds(request.data) ?? DateTime.Now;
                DateTime dataConfirmacaoChegada = Utilidades.DateTime.FromUnixSeconds(request.dataConfirmacaoChegada) ?? DateTime.MinValue;
                DateTime dataInicio = Utilidades.DateTime.FromUnixSeconds(request.dataInicio) ?? DateTime.MinValue;

                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);
                if (cargaEntrega == null && request.codigoCarga > 0)
                {
                    cargaEntrega = repCargaEntrega.BuscarPrimeiroPorCarga(request.codigoCarga);
                    request.codigoCargaEntrega = cargaEntrega?.Codigo ?? 0;
                }
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarAcertoAberto(motorista?.Codigo ?? 0);

                if (acertoViagem == null)
                {
                    RetornarErro("Motorista não possui nenhum acerto em aberto para lançamento da despesa.");
                    return new ResponseLancarDespesaViagem
                    {
                        CodigoOcorrencia = null,
                        CodigoDespesaViagem = null,
                        NumeroOcorrencia = null
                    };
                }

                if (request.codigoJustificativaDespesaViagem <= 0)
                {
                    RetornarErro("Favor informe o código da justificativa para o lançamento da despesa de viagem.");
                    return new ResponseLancarDespesaViagem
                    {
                        CodigoOcorrencia = null,
                        CodigoDespesaViagem = null,
                        NumeroOcorrencia = null
                    };
                }

                // Convertendo as coordenadas para waypoints
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointRejeicaoColetaEntrega = null;
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada = null;
                if (!(cargaEntrega?.Carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                {
                    wayPointRejeicaoColetaEntrega = request.coordenadaRejeicao?.ConverterParaWayPoint();
                    wayPointConfirmacaoChegada = request.coordenadaConfirmacaoChegada?.ConverterParaWayPoint();
                }

                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros
                {
                    codigoCargaEntrega = request.codigoCargaEntrega,
                    codigoMotivo = request.codigoMotivoRejeicao,
                    data = dataRejeicao,
                    wayPoint = wayPointRejeicaoColetaEntrega,
                    usuario = null,
                    motivoRetificacao = request.motivoRetificacao,
                    tipoServicoMultisoftware = TipoServicoMultisoftware,
                    observacao = request.observacao,
                    configuracao = configuracao,
                    devolucaoParcial = request.devolucaoParcial,
                    notasFiscais = request.notasFiscais != null && request.notasFiscais.Count > 0 ? (from o in request.notasFiscais select o.ConverterParaProdutoMobileAntigo()).ToList() : null,
                    motivoFalhaGTA = request.motivoFalhaGTA,
                    apenasRegistrar = false,
                    dadosRecebedor = request.dadosRecebedor?.ConverterParaDadosRecebedorAntigo(),
                    permitirEntregarMaisTarde = false,
                    dataConfirmacaoChegada = dataConfirmacaoChegada,
                    wayPointConfirmacaoChegada = wayPointConfirmacaoChegada,
                    atendimentoRegistradoPeloMotorista = true,
                    OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App,
                    dataInicioCarregamento = dataInicio, // Isso é meio estranho, mas é como já tava funcionando antes. Acontece.
                    quantidadeImagens = request.quantidadeImagens,
                    clienteMultisoftware = usuarioMobileCliente?.Cliente,
                    valorChamado = request.ValorDespesaViagem,
                    codigoCarga = request.codigoCarga
                };

                Auditado auditado = ObterAuditado(motorista);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, auditado, unitOfWork, out Dominio.Entidades.Embarcador.Chamados.Chamado chamado, TipoServicoMultisoftware);

                if (chamado != null)
                {
                    if (chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado)
                    {
                        int codigoChamado = chamado.Codigo;
                        string stringConexao = unitOfWork.StringConexao;
                        Task t = Task.Factory.StartNew(() => { Servicos.Embarcador.Chamado.Chamado.EnviarEmailCargaDevolucao(codigoChamado, stringConexao); });
                    }
                    Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);

                    AdicionarImagensChamado(chamado, request.imagens, unitOfWork, auditado);

                    new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoAberto(chamado, unitOfWork);
                }
                else
                {
                    unitOfWork.Rollback();
                    RetornarErro("Não foi possível gerar um novo atendimento/chamado, favor verifique as informações e configurações (carga inexistente).");
                    return new ResponseLancarDespesaViagem
                    {
                        CodigoOcorrencia = null,
                        CodigoDespesaViagem = null,
                        NumeroOcorrencia = null
                    };
                }

                Dominio.Entidades.Veiculo veiculo = null;
                if (acertoViagem != null && acertoViagem.Veiculos != null && acertoViagem.Veiculos.Count > 0)
                    veiculo = acertoViagem.Veiculos.FirstOrDefault()?.Veiculo ?? null;
                if (veiculo == null)
                    veiculo = chamado?.Carga?.Veiculo ?? null;

                if (veiculo == null)
                {
                    unitOfWork.Rollback();
                    RetornarErro("Não foi possível localizar nenhum veículo vinculado a carga do chamado ou ao acerto em aberto.");
                    return new ResponseLancarDespesaViagem
                    {
                        CodigoOcorrencia = null,
                        CodigoDespesaViagem = null,
                        NumeroOcorrencia = null
                    };
                }

                if (chamado != null && chamado.Valor <= 0 && request.ValorDespesaViagem > 0)
                {
                    chamado.Valor = request.ValorDespesaViagem;
                    repChamado.Atualizar(chamado);
                }
                if (chamado != null && chamado.Motorista == null && motorista != null)
                {
                    chamado.Motorista = motorista;
                    repChamado.Atualizar(chamado);
                }

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(request.codigoJustificativaDespesaViagem);

                Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa acertoOutraDespesa = new Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa()
                {
                    AcertoViagem = acertoViagem,
                    Chamado = chamado,
                    Data = chamado?.DataCriacao ?? DateTime.Now,
                    DataBaseCRT = null,
                    DespesaPagaPeloAdiantamento = false,
                    Justificativa = justificativa,
                    MoedaCotacaoBancoCentral = MoedaCotacaoBancoCentral.Real,
                    Pessoa = chamado?.MotivoChamado?.FornecedorDespesa ?? chamado?.Cliente ?? null,
                    NumeroDocumento = chamado?.Numero ?? 0,
                    Observacao = $"GERADO A PARTIR DO ATENDIMENTO Nº {(chamado?.Numero ?? 0)} REFERENTE A DESPESA DO MOTORISTA",
                    TipoPagamento = chamado != null && chamado.PagoPeloMotorista ? TipoPagamentoAcertoDespesa.Motorista : TipoPagamentoAcertoDespesa.Empresa,
                    Valor = request.ValorDespesaViagem,
                    Quantidade = 1,
                    ValorMoedaCotacao = 0m,
                    ValorOriginalMoedaEstrangeira = 0m,
                    Veiculo = veiculo,
                    Produto = null
                };

                if (acertoOutraDespesa.Pessoa != null)
                    acertoOutraDespesa.NomeFornecedor = acertoOutraDespesa.Pessoa.Nome.Length > 44 ? acertoOutraDespesa.Pessoa.Nome.Substring(0, 44) : acertoOutraDespesa.Pessoa.Nome;

                if (acertoOutraDespesa.Justificativa == null || acertoOutraDespesa.Pessoa == null)
                {
                    unitOfWork.Rollback();
                    RetornarErro("Favor informe a justificativa e o fornecedor corretamente para o lançamento da despesa do acerto.");
                    return new ResponseLancarDespesaViagem
                    {
                        CodigoOcorrencia = null,
                        CodigoDespesaViagem = null,
                        NumeroOcorrencia = null
                    };
                }

                repAcertoOutraDespesa.Inserir(acertoOutraDespesa, auditado);
                Servicos.Auditoria.Auditoria.Auditar(auditado, acertoOutraDespesa.AcertoViagem, null, "Inserido a despesa " + acertoOutraDespesa.Descricao + ", veículo " + acertoOutraDespesa.Veiculo.Placa + " pelo lançamento de chamado via APP.", unitOfWork);

                unitOfWork.CommitChanges();

                //forçar mostrar alerta na tela de acompanhamento carga
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargasAcompanamentoMSMQ(cargaEntrega?.Carga, Cliente.Codigo);

                // Notifica para outros motoristas da carga que uma ação foi realizada
                if (motorista != null)
                {
                    Servicos.Embarcador.Chamado.NotificacaoMobile serNotificaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, 0);
                    serNotificaoMobile.NotificarCargaAtualizadaPorOutroMotorista(cargaEntrega?.Carga, cargaEntrega, motorista, TipoEventoAlteracaoCargaPorOutroMotorista.RejeicaoEntregaColeta);
                }

                Servicos.Log.TratarErro($"NovoApp - LancarDespesaViagem - Dados salvos com sucesso");

                return new ResponseLancarDespesaViagem
                {
                    CodigoOcorrencia = chamado?.Codigo,
                    NumeroOcorrencia = chamado?.Numero,
                    CodigoDespesaViagem = 0
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao lançar despesa", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseLancarDespesaViagem
            {
                CodigoOcorrencia = null,
                CodigoDespesaViagem = null,
                NumeroOcorrencia = null
            };
        }

        public ResponseBool EnvioFotoGTA(RequestFotoGTA request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando EnvioFotoGTA - codigoCargaEntrega: {request.codigoCargaEntrega}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                byte[] buffer = System.Convert.FromBase64String(request.imagem);
                MemoryStream ms = new MemoryStream(buffer);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                    Usuario = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF)
                };

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(ms, unitOfWork, out string tokenImagem);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarFotoGTA(
                    request.clienteMultisoftware,
                    request.codigoCargaEntrega,
                    tokenImagem,
                    unitOfWork,
                    DateTime.Now,
                    auditado
                );

                Servicos.Log.TratarErro($"Novo App - EnvioFotoGTA - Imagem salva com sucesso");
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao enviar foto.", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool EnviarNFesParaAnaliseNaoConformidade(RequestEnviarNFesParaAnaliseNaoConformidade request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando EnviarNFesParaAnaliseNaoConformidade - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(request.codigoCargaEntrega);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);

                unitOfWork.Start();

                servicoControleEntrega.EnviarNFesParaAnaliseNaoConformidade(cargaEntrega, request.chavesNFe, ObterAuditado(motorista), TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro($"NovoApp - Finalizou - EnviarNFesParaAnaliseNaoConformidade");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao enviar as NFes", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseObterAtendimentoOcorrencia ObterAtendimentoOcorrencia(int clienteMultisoftware, int codigoOcorrencia)
        {
            ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoOcorrencia);

                if (chamado == null)
                    RetornarErro("Chamado não encontrado");

                ResponseObterAtendimentoOcorrencia response = new ResponseObterAtendimentoOcorrencia()
                {
                    Codigo = chamado.Codigo,
                    Numero = chamado.Numero,
                    DevolucaoParcial = chamado.DevolucaoParcial,
                    Observacao = chamado.Observacao,
                    Data = chamado.DataRegistroMotorista?.ToUnixSeconds() ?? chamado.DataCriacao.ToUnixSeconds(),
                    Situacao = chamado.Situacao,
                    TratativaDevolucao = chamado.TratativaDevolucao.ObterDescricaoTratativaDevolucao(),
                    ImagemAnexoAtendimento = ObterImagensAtendimento(chamado, unitOfWork),
                    CodigoCargaEntrega = chamado.CargaEntrega.Codigo
                };

                return response;
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao obter o atendimento", System.Net.HttpStatusCode.InternalServerError);
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool CargaEmMonitoramento(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento config = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            if (config?.ValidarCargaEmMonitoramentoAoReceberPosicao ?? false)
                return repMonitoramento.CargaEstaEmMonitoramento(codigoCarga);
            else
                return true;
        }

        private Dominio.Entidades.Veiculo ObterVeiculoPorCargaNoMonitoramentoOuMotorista(Repositorio.UnitOfWork unitOfWork, int codigoCarga, int codigoMotorista)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCargaNoMonitoramento(unitOfWork, codigoCarga);
            if (veiculo == null)
            {
                veiculo = ObterVeiculoPorMotorista(unitOfWork, codigoMotorista);
            }
            return veiculo;
        }

        private Dominio.Entidades.Veiculo ObterVeiculoPorCargaNoMonitoramento(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarUltimoPorCarga(codigoCarga);
                return monitoramento?.Veiculo ?? null;
            }
            return null;
        }

        private Dominio.Entidades.Veiculo ObterVeiculoPorMotorista(Repositorio.UnitOfWork unitOfWork, int codigoMotorista)
        {
            if (codigoMotorista > 0)
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                return repositorioVeiculo.BuscarPorMotorista(codigoMotorista);
            }
            return null;
        }

        private Dominio.Entidades.Usuario ObterUsuario(Repositorio.UnitOfWork unitOfWork, string cpf)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

            return repositorioUsuario.BuscarPorCPF(cpf);
        }

        private Auditado ObterAuditado(Dominio.Entidades.Usuario motorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMonitoriamento,
                Usuario = motorista,
            };

            return auditado;
        }

        private void AtualizarVersaoAppMotoristasPorCPFEVersaoDiferente(Repositorio.UnitOfWork unitOfWork, string cpf, string versaoApp)
        {
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
            repositorioMotorista.AtualizarVersaoAppPorCPFMotorista(cpf, versaoApp);
        }

        private void AdicionarImagensChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<string> imagens, Repositorio.UnitOfWork unitOfWork, Auditado auditado)
        {
            Repositorio.Embarcador.Chamados.ChamadoAnexo repChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);

            string extensao = ".jpg";

            foreach (string imagem in imagens)
            {
                // Salva imagem no disco
                byte[] buffer = System.Convert.FromBase64String(imagem);
                MemoryStream ms = new MemoryStream(buffer);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ArmazenarArquivoFisico(ms, Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), out string tokenImagem);

                // Salva no banco
                string caminhoChamado = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), tokenImagem + extensao);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoChamado))
                {
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo()
                    {
                        Chamado = chamado,
                        Descricao = string.Empty,
                        GuidArquivo = tokenImagem,
                        NomeArquivo = tokenImagem + extensao,
                    };

                    repChamadoAnexo.Inserir(chamadoAnexo, auditado);
                    if (auditado != null)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, chamado, $"Enviou o anexo {tokenImagem + extensao} na data {DateTime.Now.ToDateTimeString()}", unitOfWork);

                }

            }

        }

        private bool VincularRicNaCarga(Repositorio.UnitOfWork unitOfWork, DadosRIC dadosRIC, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario)
        {
            if ((carga.SituacaoCarga == SituacaoCarga.Cancelada) || (carga.SituacaoCarga == SituacaoCarga.Encerrada) || (carga.SituacaoCarga == SituacaoCarga.Anulada))
                return true;

            bool containerValido = false;
            DateTime dataColetaContainer = Utilidades.DateTime.FromUnixSeconds(dadosRIC.DataDeColeta) ?? DateTime.MinValue;

            if (dataColetaContainer == DateTime.MinValue)
            {
                Servicos.Log.TratarErro($"NovoApp - VincularRicNaCarga - Data de coleta inválida: " + dadosRIC.DataDeColeta.ToString());
                return false;
            }

            if (!string.IsNullOrWhiteSpace(dadosRIC.Container))
            {
                dadosRIC.Container = dadosRIC.Container.Replace("-", string.Empty).ToUpperInvariant().Trim();
                containerValido = dadosRIC.Container.Length == 11;
            }

            if (!containerValido)
            {
                Servicos.Log.TratarErro($"NovoApp - VincularRicNaCarga - Container inválido: " + (dadosRIC.Container ?? string.Empty));
                return false;
            }

            Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Container container = repositorioContainer.BuscarPorNumero(dadosRIC.Container);
            Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario;
            auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.LeituraDeRIC_WebServiceMobile;
            auditado.Usuario = usuario;
            if (container == null)
            {
                Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                try
                {
                    container = servicoColetaContainer.CadastrarContainer(dadosRIC.ConverterEmObjetoDeValor(), auditado);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro($"NovoApp - VincularRicNaCarga - " + e.Message);
                    return false;
                }
                if (container == null)
                    return false;

                VincularContainer(unitOfWork, carga, container, dataColetaContainer, usuario, auditado);
            }
            else
            {
                VincularContainer(unitOfWork, carga, container, dataColetaContainer, usuario, auditado);
            }

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Container.CadastroDeContainer objValor = dadosRIC.ConverterEmObjetoDeValor();
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic entidadeReciboDeContainer = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic()
                {
                    ArmadorBooking = objValor.ArmadorBooking,
                    Container = container,
                    ContainerDescricao = objValor.Container,
                    DataDeColeta = objValor.DataDeColeta,
                    Motorista = objValor.Motorista,
                    Placa = objValor.Placa,
                    TaraContainer = objValor.TaraContainer,
                    TipoContainer = objValor.TipoContainer,
                    Transportadora = objValor.Transportadora
                };
                Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic repositorioRic = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic(unitOfWork);
                repositorioRic.Inserir(entidadeReciboDeContainer, auditado);
            }
            catch (Exception exr)
            {
                Servicos.Log.TratarErro($"NovoApp - VincularRicNaCarga - " + exr.Message);
            }

            return true;
        }

        private void VincularContainer(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga,
            Dominio.Entidades.Embarcador.Pedidos.Container container, DateTime dataColetaContainer, Dominio.Entidades.Usuario usuario, Auditado auditado)
        {
            Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
            Servicos.Embarcador.Pedido.ConferenciaContainer servicoConferenciaContainer = new Servicos.Embarcador.Pedido.ConferenciaContainer(unitOfWork, auditado);
            Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = servicoColetaContainer.VincularContainerAoColetaContainerCargaColeta(carga, container, dataColetaContainer);

            Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
            {
                CargaDaColeta = carga,
                coletaContainer = coletaContainer,
                DataAtualizacao = DateTime.Now,
                LocalAtual = carga.DadosSumarizados?.ClientesRemetentes?.FirstOrDefault() ?? null,
                Status = StatusColetaContainer.EmDeslocamentoVazio,
                Usuario = usuario,
                OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.WebService,
                InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.VincularRICCarga
            };

            servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
            servicoConferenciaContainer.Atualizar(carga, container);

            if (carga.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraColeta = repositorioCargaEntrega.BuscarColetaNaOrigemPorCarga(carga.Codigo);

                if (primeiraColeta?.Situacao == SituacaoEntrega.NaoEntregue)
                {
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(primeiraColeta.Carga.TipoOperacao?.Codigo ?? 0);

                    Servicos.Embarcador.Notificacao.NotificacaoMTrack servicoNotificacaoMTrack = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametrosFinalizarEntrega = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros()
                    {
                        cargaEntrega = primeiraColeta,
                        dataInicioEntrega = dataColetaContainer,
                        dataTerminoEntrega = dataColetaContainer,
                        dataConfirmacao = dataColetaContainer,
                        configuracaoEmbarcador = configuracaoEmbarcador,
                        tipoServicoMultisoftware = TipoServicoMultisoftware,
                        sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                        OrigemSituacaoEntrega = OrigemSituacaoEntrega.UsuarioMultiEmbarcador,
                        Container = container.Codigo,
                        DataColetaContainer = dataColetaContainer,
                        auditado = auditado,
                        configuracaoControleEntrega = configuracaoControleEntrega,
                        tipoOperacaoParametro = tipoOperacaoParametro,
                        TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                    };

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametrosFinalizarEntrega, unitOfWork);
                    servicoNotificacaoMTrack.NotificarMudancaCargaEntrega(primeiraColeta, carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.EntregaConfirmadaNoEmbarcador, notificarSignalR: true, codigoClienteMultisoftware: usuario.Empresa.Codigo);
                }
            }
            Servicos.Auditoria.Auditoria.Auditar(auditado, coletaContainer, null, $"NovoApp - Vinculou Container {container.Numero} a carga {carga.CodigoCargaEmbarcador}", unitOfWork);
        }

        private List<ImagemAnexoAtendimento> ObterImagensAtendimento(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            List<ImagemAnexoAtendimento> imagensAtendimento = new List<ImagemAnexoAtendimento>();

            foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo in chamado.Anexos)
            {
                imagensAtendimento.Add(new ImagemAnexoAtendimento
                {
                    Descricao = chamadoAnexo.Descricao,
                    NomeArquivo = chamadoAnexo.NomeArquivo,
                    ImagemBase64 = ObterBase64ImagemChamadaAnexo(chamadoAnexo, unitOfWork)
                });
            }

            return imagensAtendimento;
        }

        private string ObterBase64ImagemChamadaAnexo(Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" });
            string extensao = Path.GetExtension(chamadoAnexo.NomeArquivo);
            string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, chamadoAnexo.GuidArquivo + "-miniatura" + extensao);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                return string.Empty;

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private void AdicionarPosicaoProcessar(Dominio.Entidades.Embarcador.Cargas.Carga carga, double latitude, double longitude, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento;

            if (carga == null || !Servicos.Embarcador.Logistica.WayPointUtil.ValidarCoordenadas(latitude, longitude))
                return;

            monitoramento = repositorioMonitoramento.BuscarUltimoPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Logistica.Posicao posicaoPendenteIntegracao = new Dominio.Entidades.Embarcador.Logistica.Posicao()
            {
                Data = DateTime.Now,
                DataVeiculo = DateTime.Now,
                DataCadastro = DateTime.Now,
                IDEquipamento = string.IsNullOrEmpty(monitoramento?.UltimaPosicao?.IDEquipamento) ? carga?.Veiculo?.Codigo.ToString() : monitoramento?.UltimaPosicao?.IDEquipamento,
                Descricao = $"{latitude}, {longitude}",
                Veiculo = carga.Veiculo,
                Latitude = latitude,
                Longitude = longitude,
                Processar = ProcessarPosicao.Pendente,
                Rastreador = monitoramento?.UltimaPosicao?.Rastreador ?? EnumTecnologiaRastreador.Mobile
            };

            repositorioPosicao.Inserir(posicaoPendenteIntegracao);
        }

        #endregion Métodos Privados
    }
}
