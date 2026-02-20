using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.SuperApp
{
    public class IntegracaoSuperAppDemaisEventos
    {
        #region Atributos Privados
        private protected Repositorio.UnitOfWork _unitOfWork;
        private protected AdminMultisoftware.Repositorio.UnitOfWork _unitOfWorkAdmin;
        private protected AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        #endregion

        #region Construtores
        public IntegracaoSuperAppDemaisEventos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;
            _clienteMultisoftware = clienteMultisoftware;
        }
        public IntegracaoSuperAppDemaisEventos(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Métodos Públicos
        public void Iniciar()
        {
            ProcessarDemaisTipos();
        }
        #endregion

        #region Métodos Privados

        private void ProcessarDemaisTipos()
        {
            Servicos.Embarcador.SuperApp.Eventos.ChatSendMessage servicoChatSendMessage = new Servicos.Embarcador.SuperApp.Eventos.ChatSendMessage(_unitOfWork);
            Servicos.Embarcador.SuperApp.Eventos.OccurrenceCreate servicoOccurrenceCreate = new Servicos.Embarcador.SuperApp.Eventos.OccurrenceCreate(_unitOfWork, _unitOfWorkAdmin, _clienteMultisoftware);
            Servicos.Embarcador.SuperApp.Eventos.DeliveryReceiptCreate servicoDeliveryReceiptCreate = new Servicos.Embarcador.SuperApp.Eventos.DeliveryReceiptCreate(_unitOfWork, _unitOfWorkAdmin, _clienteMultisoftware);
            Servicos.Embarcador.SuperApp.Eventos.DriverReceiptCreate servicoDriverReceiptCreate = new Servicos.Embarcador.SuperApp.Eventos.DriverReceiptCreate(_unitOfWork, _unitOfWorkAdmin, _clienteMultisoftware);
            Servicos.Embarcador.SuperApp.Eventos.DriverFreightContactCreate servicoDriverFreightContactCreate = new Servicos.Embarcador.SuperApp.Eventos.DriverFreightContactCreate(_unitOfWork, _unitOfWorkAdmin, _clienteMultisoftware);
            Servicos.Embarcador.SuperApp.Eventos.SalvarDevolucao servicoSalvarDevolucao = new Servicos.Embarcador.SuperApp.Eventos.SalvarDevolucao(_unitOfWork, _unitOfWorkAdmin, _clienteMultisoftware);

            Repositorio.Embarcador.SuperApp.IntegracaoSuperApp repositorioIntegracaoSuperApp = new Repositorio.Embarcador.SuperApp.IntegracaoSuperApp(_unitOfWork);
            List<TipoEventoApp> listaEventos = new() { TipoEventoApp.ChatSendMessage, TipoEventoApp.DeliveryReceiptCreate, TipoEventoApp.DriverReceiptCreate, TipoEventoApp.OccurrenceCreate, TipoEventoApp.DriverOccurrenceCreate, TipoEventoApp.DriverFreightContactCreate, TipoEventoApp.NotDelivered, TipoEventoApp.PartialDelivery };

            List<int> codigosEventosPendentes = repositorioIntegracaoSuperApp.BuscarIntegracoesPendentes(listaEventos, 200);

            Servicos.Log.TratarErro("Iniciando ProcessarDemaisTipos", "IntegracaoSuperAPPOutrosTipos");
            foreach (int codEvento in codigosEventosPendentes)
            {
                RetornoIntegracaoSuperApp retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
                Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp eventoProcessar = repositorioIntegracaoSuperApp.BuscarPorCodigo(codEvento, false);

                eventoProcessar.NumeroTentativas++;
                repositorioIntegracaoSuperApp.Atualizar(eventoProcessar);

                _unitOfWork.Start();
                try
                {
                    switch (eventoProcessar.TipoEvento)
                    {
                        case TipoEventoApp.ChatSendMessage:
                            servicoChatSendMessage.ProcessarEvento(eventoProcessar, out retornoIntegracaoSuperApp);
                            break;

                        case TipoEventoApp.OccurrenceCreate:
                        case TipoEventoApp.DriverOccurrenceCreate:
                            servicoOccurrenceCreate.ProcessarEvento(eventoProcessar, out retornoIntegracaoSuperApp);
                            break;

                        case TipoEventoApp.DeliveryReceiptCreate:
                            servicoDeliveryReceiptCreate.ProcessarEvento(eventoProcessar, out retornoIntegracaoSuperApp);
                            break;

                        case TipoEventoApp.DriverReceiptCreate:
                            servicoDriverReceiptCreate.ProcessarEvento(eventoProcessar, out retornoIntegracaoSuperApp);
                            break;

                        case TipoEventoApp.DriverFreightContactCreate:
                            servicoDriverFreightContactCreate.ProcessarEvento(eventoProcessar, out retornoIntegracaoSuperApp);
                            break;

                        case TipoEventoApp.NotDelivered:
                        case TipoEventoApp.PartialDelivery:
                            servicoSalvarDevolucao.ProcessarEvento(eventoProcessar, eventoProcessar.TipoEvento == TipoEventoApp.PartialDelivery, out retornoIntegracaoSuperApp);
                            break;

                        default:
                            string jsonRequisicao = eventoProcessar.ArquivoRequisicao != null ? obterJsonRequisicao(eventoProcessar.ArquivoRequisicao) : eventoProcessar.StringJsonRequest;

                            if (string.IsNullOrEmpty(jsonRequisicao)) throw new ServicoException($"Arquivo de integração/Request não encontrado.");

                            EventoSuperApp eventoSuperApp = Newtonsoft.Json.JsonConvert.DeserializeObject<EventoSuperApp>(jsonRequisicao);
                            retornoIntegracaoSuperApp.Sucesso = true;
                            retornoIntegracaoSuperApp.Mensagem = "Evento Ignorado - " + $"Evento não tratado: {eventoSuperApp.Event} - Carga: {eventoSuperApp.Data.Travel.ExternalInfo.Id}";

                            Servicos.Log.TratarErro(retornoIntegracaoSuperApp.Mensagem);
                            break;
                    }

                    if (retornoIntegracaoSuperApp.Sucesso)
                        _unitOfWork.CommitChanges();
                    else
                        _unitOfWork.Rollback();

                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp eventonovo = repositorioIntegracaoSuperApp.BuscarPorCodigo(codEvento, false);

                    if (string.IsNullOrEmpty(retornoIntegracaoSuperApp.Mensagem))
                        eventonovo.DetalhesProcessamento = "Processado";
                    else if (retornoIntegracaoSuperApp.Mensagem.Length > 200)
                        eventonovo.DetalhesProcessamento = retornoIntegracaoSuperApp.Mensagem.Substring(0, 200);
                    else
                        eventonovo.DetalhesProcessamento = retornoIntegracaoSuperApp.Mensagem;

                    eventonovo.SituacaoProcessamento = retornoIntegracaoSuperApp.Sucesso ? SituacaoProcessamentoIntegracao.Processado : SituacaoProcessamentoIntegracao.ErroProcessamento;
                    repositorioIntegracaoSuperApp.Atualizar(eventonovo);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPOutrosTipos");
                    _unitOfWork.Rollback();

                    _unitOfWork.Start();
                    Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp eventonovo = repositorioIntegracaoSuperApp.BuscarPorCodigo(codEvento, false);

                    retornoIntegracaoSuperApp.Mensagem = ex.Message;
                    eventonovo.DetalhesProcessamento = "Ocorreu uma falha genérica ao processar o evento.";
                    eventonovo.SituacaoProcessamento = SituacaoProcessamentoIntegracao.ErroProcessamento;
                    repositorioIntegracaoSuperApp.Atualizar(eventonovo);

                    _unitOfWork.CommitChanges();
                }
            }
        }
        #endregion

        #region Métodos Privados Protegidos

        private string obterJsonRequisicao(Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao)
        {
            return Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(arquivoIntegracao);
        }

        #endregion
    }

}