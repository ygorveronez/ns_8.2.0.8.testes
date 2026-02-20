using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotificacaoMobile;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using System;
using System.Threading.Tasks;

namespace Servicos.Embarcador.SuperApp.Eventos
{
    public class OccurrenceCreate : IntegracaoSuperApp
    {
        private string _cpfMotorista;

        public OccurrenceCreate(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, unitOfWorkAdmin, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;
            _clienteMultisoftware = clienteMultisoftware;
        }

        public void ProcessarEvento(Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp, out RetornoIntegracaoSuperApp retornoIntegracaoSuperApp)
        {
            retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
            try
            {
                Servicos.Log.TratarErro("Inicio Ocorrence Create", "IntegracaoSuperAPPOutrosTipos");

                string jsonRequisicao = integracaoSuperApp.ArquivoRequisicao != null ? obterJsonRequisicao(integracaoSuperApp.ArquivoRequisicao) : integracaoSuperApp.StringJsonRequest;

                if (string.IsNullOrEmpty(jsonRequisicao))
                    throw new ServicoException($"Arquivo de integração/Request não encontrado.");

                EventoOccurrenceCreate eventoOccurrenceCreate = Newtonsoft.Json.JsonConvert.DeserializeObject<EventoOccurrenceCreate>(jsonRequisicao);
                if (eventoOccurrenceCreate == null)
                    throw new ServicoException("Falha na conversão da requisição para objeto.");

                _cpfMotorista = eventoOccurrenceCreate.Data.Driver.Document.Value;

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);

                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(_unitOfWork);

                // Parse datas
                DateTime dataRejeicao = DateTime.Now;
                DateTime dataConfirmacaoChegada = DateTime.MinValue;
                DateTime dataInicio = DateTime.MinValue;

                string IDTrizyCarga = eventoOccurrenceCreate.Data.Travel._id; //Carga.
                int codigoCarga = eventoOccurrenceCreate.Data.Travel.ExternalID.ToInt(); //Codigo Carga.

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorIDIdentificacaoTrizy(IDTrizyCarga) ?? repositorioCarga.BuscarPorCodigo(codigoCarga);

                //IDTrizy
                string IDTrizy = eventoOccurrenceCreate.Data.StoppingPoint?._id ?? eventoOccurrenceCreate.Data.Travel._id;

                string[] codigoExterno = eventoOccurrenceCreate.Data.StoppingPoint?.ExternalId?.Split(';');

                int codigoCargaEntrega = codigoExterno != null && codigoExterno.Length > 0 ? codigoExterno[0].ToInt() : 0;

                double identificacaoCliente = codigoExterno != null && codigoExterno.Length > 1 ? codigoExterno[1].ToDouble() : 0;

                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaPorCPF(_cpfMotorista);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorIdTrizy(IDTrizy) ?? repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega) ?? repCargaEntrega.BuscarPorCargaECliente(carga.Codigo, identificacaoCliente);

                if (cargaEntrega == null)
                    throw new ServicoException($"Coleta/Entrega não localizada. ID:{IDTrizy} | Código:{codigoCargaEntrega}", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);

                if (SituacaoEntregaHelper.ObterSituacaoEntregaFinalizada(cargaEntrega.Situacao))
                    throw new ServicoException(cargaEntrega.Coleta ? "Coleta" : "Entrega" + " já confirmada.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);

                if (cargaEntrega.Carga.DataFimViagem.HasValue)
                    throw new ServicoException($"Não permitido abrir Ocorrência, carga já finalizada.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);

                AtualizarCargaIntegracaoSuperApp(integracaoSuperApp, cargaEntrega.Carga, cargaEntrega);

                // Convertendo as coordenadas para waypoints
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointRejeicaoColetaEntrega = null;
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada = null;

                int codigoMotivoRejeicao = 0;
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = null;
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null;
                Auditado auditado = ObterAuditado(motorista);

                if (integracaoSuperApp.TipoEvento == TipoEventoApp.OccurrenceCreate)
                {
                    ObterDadosEvidencias(eventoOccurrenceCreate.Data.Evidences);
                    string idOcorrencia = eventoOccurrenceCreate.Data.Category._id;
                    if (!string.IsNullOrWhiteSpace(idOcorrencia))
                    {
                        tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigoIntegracaoAuxiliar(idOcorrencia);
                        if (tipoOcorrencia != null)
                            codigoMotivoRejeicao = tipoOcorrencia.Codigo;
                    }
                }
                else
                {
                    ObterDadosChecklistFlow(eventoOccurrenceCreate.Data.Response);
                    string idOcorrencia = eventoOccurrenceCreate.Data.Category.ExternalId;
                    if (!string.IsNullOrWhiteSpace(idOcorrencia))
                    {
                        tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(int.Parse(idOcorrencia));
                        if (tipoOcorrencia != null)
                            codigoMotivoRejeicao = tipoOcorrencia.Codigo;
                    }
                }

                int ocorrenciaQuantidadeImagens = _dadosEvidencias.imagensEntrega.Count;
                string observacaoOcorrencia = _dadosEvidencias.observacoes.Count > 0 ? _dadosEvidencias.observacoes[0] : string.Empty;

              Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros
                {
                    codigoCargaEntrega = cargaEntrega.Codigo,
                    codigoMotivo = codigoMotivoRejeicao,
                    data = dataRejeicao,
                    wayPoint = wayPointRejeicaoColetaEntrega,
                    usuario = null,
                    motivoRetificacao = 0,
                    tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiMobile,
                    observacao = observacaoOcorrencia,
                    configuracao = _configuracaoTMS,
                    devolucaoParcial = false,
                    motivoFalhaGTA = 0,
                    apenasRegistrar = false,
                    permitirEntregarMaisTarde = false,
                    dataConfirmacaoChegada = dataConfirmacaoChegada,
                    wayPointConfirmacaoChegada = wayPointConfirmacaoChegada,
                    atendimentoRegistradoPeloMotorista = true,
                    OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App,
                    dataInicioCarregamento = dataInicio,
                    quantidadeImagens = ocorrenciaQuantidadeImagens,
                    imagens = _dadosEvidencias.imagensEntrega,
                    clienteMultisoftware = _clienteMultisoftware
                };

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, auditado, _unitOfWork, out chamado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiMobile);

                if (chamado != null)
                {
                    if (chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado)
                    {
                        int codigoChamado = chamado.Codigo;
                        string stringConexao = _unitOfWork.StringConexao;
                        Task t = Task.Factory.StartNew(() => { Servicos.Embarcador.Chamado.Chamado.EnviarEmailCargaDevolucao(codigoChamado, stringConexao); });
                    }

                    string IdOcorrenciaTrizy = eventoOccurrenceCreate.Data._id;
                    if (!string.IsNullOrWhiteSpace(IdOcorrenciaTrizy))
                    {
                        chamado.IdOcorrenciaTrizy = IdOcorrenciaTrizy;
                        repChamado.Atualizar(chamado);
                    }

                    Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, _unitOfWork);

                    AdicionarImagensChamado(chamado, _dadosEvidencias.imagensEntrega, _unitOfWork, auditado);

                    new Servicos.Embarcador.Chamado.Chamado(_unitOfWork).EnviarEmailChamadoAberto(chamado, _unitOfWork);
                }

                //forçar mostrar alerta na tela de acompanhamento carga
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargasAcompanamentoMSMQ(cargaEntrega.Carga, _clienteMultisoftware.Codigo);

                Servicos.Log.TratarErro("Fim Ocorrence Create", "IntegracaoSuperAPPOutrosTipos");

                retornoIntegracaoSuperApp.Sucesso = true;
            }
            catch (ServicoException ex) when (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                retornoIntegracaoSuperApp.Sucesso = true;
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPOutrosTipos");
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPOutrosTipos");
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar " + TipoEventoApp.OccurrenceCreate.ObterDescricao();
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPOutrosTipos");
            }
        }
    }
}
