using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using AdminMultisoftware.Dominio.Enumeradores;
using Microsoft.AspNetCore.SignalR;

namespace Servicos.SignalR
{
    public class Mobile : SignalRBase<Mobile>
    {
        public Mobile()
        {
        }

        public static string GetHub(MobileHubs metodo)
        {
            switch (metodo)
            {
                case MobileHubs.CargaAtualizada:
                    return "CargaAtualizada";
                case MobileHubs.ChamadoAtualizado:
                    return "ChamadoAtualizado";
                case MobileHubs.MensagemChat:
                    return "MensagemChat";
                case MobileHubs.ChamadoEmAnalise:
                    return "ChamadoEmAnalise";
                case MobileHubs.UnificacaoCarga:
                    return "UnificacaoCarga";
                case MobileHubs.EntregaAdicionada:
                    return "EntregaAdicionada";
                case MobileHubs.EntregaAlterada:
                    return "EntregaAlterada";
                case MobileHubs.EntregaExcluida:
                    return "EntregaExcluida";
                case MobileHubs.PushNotificationGenerica:
                    return "PushNotificationGenerica";
                case MobileHubs.ParadaNaoProgramada:
                    return "ParadaNaoProgramada";
                case MobileHubs.CargaAtualizadaPorOutroMotorista:
                    return "CargaAtualizadaPorOutroMotorista";
                case MobileHubs.EntregaConfirmadaNoEmbarcador:
                    return "EntregaConfirmadaNoEmbarcador";
                case MobileHubs.EntregaRejeitadaNoEmbarcador:
                    return "EntregaRejeitadaNoEmbarcador";
                case MobileHubs.EntregaPrevisaoAtualizada:
                    return "EntregaPrevisaoAtualizada";
                case MobileHubs.CargaMotoristaNecessitaConfirmar:
                    return "CargaMotoristaNecessitaConfirmar";
                case MobileHubs.NovaCargaMotorista:
                    return "NovaCargaMotorista";
                case MobileHubs.NaoConformidade:
                    return "NaoConformidade";
                case MobileHubs.NaoConformidadeColetaAutorizada:
                    return "NaoConformidadeColetaAutorizada";
                default:
                    return "";
            }
        }

        public override void ProcessarNotificacao(Dominio.MSMQ.Notification notification)
        {
            switch (notification.Service)
            {
                case "CargaAtualizada":
                    CargaAtualizada(notification);
                    break;
                case "ChamadoAtualizado":
                    ChamadoAtualizado(notification);
                    break;
                case "MensagemChat":
                    MensagemChat(notification);
                    break;
                case "ChamadoEmAnalise":
                    ChamadoEmAnalise(notification);
                    break;
                case "UnificacaoCarga":
                    UnificacaoCarga(notification);
                    break;
                case "CargaMotoristaNecessitaConfirmar":
                    CargaMotoristaNecessitaConfirmar(notification);
                    break;
                case "EntregaAdicionada":
                case "EntregaAlterada":
                case "EntregaExcluida":
                case "EntregaConfirmadaNoEmbarcador":
                case "EntregaRejeitadaNoEmbarcador":
                case "EntregaPrevisaoAtualizada":
                    MovimentacaoCargaEntrega(notification);
                    break;
                case "PushNotificationGenerica":
                    PushNotificationGenerica(notification);
                    break;
                case "ParadaNaoProgramada":
                    EnviarMensagemParadaNaoProgramada(notification);
                    break;
                case "CargaAtualizadaPorOutroMotorista":
                    EnviarNotificacaoCargaAtualizadaPorOutroMotorista(notification);
                    break;
                case "NovaCargaMotorista":
                    NovaCargaMotorista(notification);
                    break;
                case "NaoConformidade":
                    NaoConformidade(notification);
                    break;
                case "NaoConformidadeColetaAutorizada":
                    NaoConformidadeColetaAutorizada(notification);
                    break;
                default:
                    break;
            }
        }

        public override string GetKey()
        {
			return Context.GetHttpContext()?.Request.Headers["Sessao"];
        }

        public string GetKey(string connectionID)
        {
            return _connections.GetKey(connectionID);
        }

        public void CargaAtualizada(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                        if (usuarioMobile != null)
                        {
                            if (!string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                            {
                                List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                                if (connectionId.Count > 0)
                                {
                                    Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

                                    SendToConnectionIds(connectionId, "cargaAtualizada", JsonConvert.SerializeObject(notification.Content));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        /// <summary>
        /// Método criado para chamada via Mobile não remover
        /// </summary>
        /// <param name="codigoCarga"></param>
        /// <param name="clienteMultisoftware"></param>
        /// <returns></returns>
        public bool ConfirmarRecebimentoCargaAtualizada(string dadosConfirmacao)
        {
            //Servicos.Log.TratarErro("ConfirmarRecebimentoCargaAtualizada " + dadosConfirmacao);
            dynamic dadosDeserializados = JsonConvert.DeserializeObject(dadosConfirmacao);

            int codigoCarga = (int)dadosDeserializados.codigoCarga;
            int clienteMultisoftware = (int)dadosDeserializados.clienteMultisoftware;


            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorSessao(GetKey(Context.ConnectionId), System.DateTime.Now.AddMinutes(-ObterMinutosSessao()));

            bool retorno = true;
            try
            {
                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao(usuarioMobileCliente));
                        try
                        {
                            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(codigoCarga);
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in cargaMotoristas)
                            {
                                if (cargaMotorista.NotificacaoAtualizacaoCargaPendente && cargaMotorista.Motorista.CodigoMobile == usuarioMobile.Codigo)
                                {
                                    cargaMotorista.NotificacaoAtualizacaoCargaPendente = false;
                                    repCargaMotorista.Atualizar(cargaMotorista);
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            retorno = false;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno = false;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
            return retorno;
        }

        public void ChamadoEmAnalise(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            if (notification.UsersID != null)
            {
                foreach (int usuario in notification.UsersID)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                    if (usuarioMobile != null)
                    {
                        if (!string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                        {
                            List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                            if (connectionId.Count > 0)
                            {
                                Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

                                SendToConnectionIds(connectionId, "chamadoEmAnalise", JsonConvert.SerializeObject(notification.Content));
                            }
                        }
                    }
                }
            }
            unitOfWorkAdmin.Dispose();
        }

        public void ChamadoAtualizado(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                        if (usuarioMobile != null)
                        {
                            if (!string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                            {
                                List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                                if (connectionId.Count > 0)
                                {
                                    Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

									SendToConnectionIds(connectionId, "chamadoAtualizado", JsonConvert.SerializeObject(notification.Content));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public void UnificacaoCarga(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                        if (usuarioMobile != null)
                        {
                            if (!string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                            {
                                List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                                if (connectionId.Count > 0)
                                {
                                    Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

									SendToConnectionIds(connectionId, "unificacaoCarga", JsonConvert.SerializeObject(notification.Content));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public void CargaMotoristaNecessitaConfirmar(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                        if (usuarioMobile != null)
                        {
                            if (!string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                            {
                                List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                                if (connectionId.Count > 0)
                                {
                                    Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

									SendToConnectionIds(connectionId, "cargaMotoristaNecessitaConfirmar", JsonConvert.SerializeObject(notification.Content));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public void MovimentacaoCargaEntrega(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID == null)
                    return;

                foreach (int usuario in notification.UsersID)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                    if (usuarioMobile == null || string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                        continue;

                    List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                    if (connectionId.Count == 0)
                        continue;

                    Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

                    SendToConnectionIds(connectionId, "movimentacaoCargaEntrega", JsonConvert.SerializeObject(notification.Content));
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public void PushNotificationGenerica(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID == null)
                    return;

                foreach (int usuario in notification.UsersID)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                    if (usuarioMobile == null || string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                        continue;

                    List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                    if (connectionId.Count == 0)
                        continue;

                    Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

                    SendToConnectionIds(connectionId, "pushNotificationGenerica", JsonConvert.SerializeObject(notification.Content));
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public void EnviarNotificacaoCargaAtualizadaPorOutroMotorista(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID == null)
                    return;

                foreach (int usuario in notification.UsersID)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                    if (usuarioMobile == null || string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                        continue;

                    List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                    if (connectionId.Count == 0)
                        continue;

                    Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

					SendToConnectionIds(connectionId, "cargaAtualizadaPorOutroMotorista", JsonConvert.SerializeObject(notification.Content));
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public bool ConfirmarRecebimentoChamadoAtualizado(string dadosConfirmacao)
        {
            dynamic dadosDeserializados = JsonConvert.DeserializeObject(dadosConfirmacao);

            int codigoChamado = (int)dadosDeserializados.codigoChamado;
            int clienteMultisoftware = (int)dadosDeserializados.clienteMultisoftware;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorSessao(GetKey(Context.ConnectionId), System.DateTime.Now.AddMinutes(-ObterMinutosSessao()));

            bool retorno = true;
            try
            {
                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao(usuarioMobileCliente));
                        try
                        {
                            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoChamado);
                            chamado.NotificacaoMotoristaMobile = false;
                            repChamado.Atualizar(chamado);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            retorno = false;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno = false;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
            return retorno;
        }

        public void NovaCargaMotorista(Dominio.MSMQ.Notification notification)
        {
            Servicos.Log.TratarErro("Entrou SignalR NovaCargaMotorista");
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                        if (!string.IsNullOrWhiteSpace(usuarioMobile?.Sessao))
                        {
                            List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                            if (connectionId.Count > 0)
                            {
                                Servicos.Log.TratarErro("NovaCargaMotorista: " + string.Join(", ", connectionId.Count));

								SendToConnectionIds(connectionId, "novaCargaMotorista", JsonConvert.SerializeObject(notification.Content));
                            }
                        }
                    }
                }
                Servicos.Log.TratarErro("Finalizou SignalR NovaCargaMotorista");
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public void NaoConformidade(Dominio.MSMQ.Notification notification)
        {
            Servicos.Log.TratarErro("Entrou SignalR NaoConformidade");
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                        if (!string.IsNullOrWhiteSpace(usuarioMobile?.Sessao))
                        {
                            List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                            if (connectionId.Count > 0)
                            {
                                Servicos.Log.TratarErro("NaoConformidade: " + string.Join(", ", connectionId.Count));

								SendToConnectionIds(connectionId, "naoConformidade", JsonConvert.SerializeObject(notification.Content));
                            }
                        }
                    }
                }
                Servicos.Log.TratarErro("Finalizou SignalR NaoConformidade");
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public void NaoConformidadeColetaAutorizada(Dominio.MSMQ.Notification notification)
        {
            Servicos.Log.TratarErro("Entrou SignalR NaoConformidadeColetaAutorizada");
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                        if (!string.IsNullOrWhiteSpace(usuarioMobile?.Sessao))
                        {
                            List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                            if (connectionId.Count > 0)
                            {
                                Servicos.Log.TratarErro("NaoConformidadeColetaAutorizada: " + string.Join(", ", connectionId.Count));

								SendToConnectionIds(connectionId, "naoConformidadeColetaAutorizada", JsonConvert.SerializeObject(notification.Content));
                            }
                        }
                    }
                }
                Servicos.Log.TratarErro("Finalizou SignalR NaoConformidadeColetaAutorizada");
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public void MensagemChat(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            try
            {

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                        if (usuarioMobile != null)
                        {
                            if (!string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                            {
                                List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                                if (connectionId.Count > 0)
                                {
									SendToConnectionIds(connectionId, "mensagemChat", JsonConvert.SerializeObject(notification.Content));
                                }
                            }
                        }
                    }

                    Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();
                    servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public bool EnviarMensagemChat(string dadosMensagem)
        {
            dynamic dadosDeserializados = JsonConvert.DeserializeObject(dadosMensagem);
            int carga = (int)dadosDeserializados.carga;
            int clienteMultisoftware = (int)dadosDeserializados.clienteMultisoftware;
            string mensagem = (string)dadosDeserializados.mensagem;
            DateTime data;
            if (!DateTime.TryParseExact((string)dadosDeserializados.dataMensagem, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                data = DateTime.Now;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorSessao(GetKey(Context.ConnectionId), System.DateTime.Now.AddMinutes(-ObterMinutosSessao()));
            bool retorno = true;
            try
            {
                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao(usuarioMobileCliente));
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                        try
                        {
                            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);
                            unitOfWork.Start();
                            Servicos.Embarcador.Chat.ChatMensagem.EnviarMensagemChat(new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = carga }, usuario, data, new List<Dominio.Entidades.Usuario>(), mensagem, clienteMultisoftware, unitOfWork, null);
                            unitOfWork.CommitChanges();

                            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();
                            servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            Servicos.Log.TratarErro(ex);
                            retorno = false;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno = false;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
            return retorno;
        }

        public bool ConfirmarRecebimentoMensagemChat(string dadosConfirmacao)
        {
            //Servicos.Log.TratarErro("ConfirmarRecebimentoChamadoAtualizado " + dadosConfirmacao);
            dynamic dadosDeserializados = JsonConvert.DeserializeObject(dadosConfirmacao);

            int codigo = (int)dadosDeserializados.codigo;
            int clienteMultisoftware = (int)dadosDeserializados.clienteMultisoftware;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorSessao(GetKey(Context.ConnectionId), System.DateTime.Now.AddMinutes(-ObterMinutosSessao()));

            bool retorno = true;
            try
            {
                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao(usuarioMobileCliente));
                        try
                        {
                            Repositorio.Embarcador.Cargas.ChatMensagemDestinatario repChatMensagemDestinatario = new Repositorio.Embarcador.Cargas.ChatMensagemDestinatario(unitOfWork);
                            Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);
                            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

                            Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario chatMensagemDestinatario = repChatMensagemDestinatario.BuscarPorCodigoChatEDestinatario(codigo, usuarioMobile.Codigo);
                            if (chatMensagemDestinatario != null)
                            {
                                unitOfWork.Start();
                                chatMensagemDestinatario.MensagemRecebida = true;
                                chatMensagemDestinatario.DataRecebimento = DateTime.Now;
                                repChatMensagemDestinatario.Atualizar(chatMensagemDestinatario);
                                if (!repChatMensagemDestinatario.VerificarTodasMensagensEntregues(codigo))
                                {
                                    chatMensagemDestinatario.ChatMobileMensagem.MensagemLida = true;
                                    chatMensagemDestinatario.DataRecebimento = DateTime.Now;
                                    repChatMobileMensagem.Atualizar(chatMensagemDestinatario.ChatMobileMensagem);
                                    Servicos.Embarcador.Chat.ChatMensagem.NotificarMensagemRecebida(chatMensagemDestinatario.ChatMobileMensagem, clienteMultisoftware, unitOfWork);
                                }
                                unitOfWork.CommitChanges();

                                servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();
                            }
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            Servicos.Log.TratarErro(ex);
                            retorno = false;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno = false;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
            return retorno;
        }

        public void EnviarMensagemParadaNaoProgramada(Dominio.MSMQ.Notification notification)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                if (notification.UsersID != null)
                {
                    foreach (int usuario in notification.UsersID)
                    {
                        AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);
                        if (usuarioMobile != null)
                        {
                            if (!string.IsNullOrWhiteSpace(usuarioMobile.Sessao))
                            {
                                List<string> connectionId = _connections.GetConnections(usuarioMobile.Sessao).ToList();
                                if (connectionId.Count > 0)
                                {
                                    Servicos.Log.TratarErro(string.Join(",", connectionId.Count));

									SendToConnectionIds(connectionId, "paradaNaoProgramada", JsonConvert.SerializeObject(notification.Content));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        /// <summary>
        /// Método criado para chamada via Mobile não remover
        /// </summary>
        /// <param name="clienteMultisoftware"></param>
        /// <returns></returns>
        public bool ExecutarNotificacoesPendentes(int clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorSessao(GetKey(Context.ConnectionId), System.DateTime.Now.AddMinutes(-ObterMinutosSessao()));
            bool retorno = true;
            try
            {
                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao(usuarioMobileCliente));
                        try
                        {
                            Repositorio.Embarcador.Chamados.Chamado repCahamdo = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                            Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                            Repositorio.Embarcador.Cargas.ChatMensagemDestinatario repChatMensagemDestinatario = new Repositorio.Embarcador.Cargas.ChatMensagemDestinatario(unitOfWork);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCodigoMobilePendentesAtualizacao(usuarioMobile.Codigo);

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in cargaMotoristas)
                            {
                                Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(new { cargaMotorista.Carga.Codigo, CodigoCliente = clienteMultisoftware }, clienteMultisoftware, usuarioMobile.Codigo, Dominio.MSMQ.MSMQQueue.SGTMobile, Dominio.SignalR.Hubs.Mobile, Servicos.SignalR.Mobile.GetHub(MobileHubs.CargaAtualizada));
                                Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
                            }

                            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repCahamdo.BuscarPorPendentesNotificacaoPorUsuarioMobile(usuarioMobile.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                            {
                                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises.FirstOrDefault();
                                foreach (Dominio.Entidades.Usuario motorista in chamado.Carga.Motoristas)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado situacao = chamado.Situacao;
                                    if (chamado.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Aberto)
                                    {
                                        Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(new { chamado.Codigo, Analista = chamado.Responsavel?.Nome ?? "" }, clienteMultisoftware, motorista.CodigoMobile, Dominio.MSMQ.MSMQQueue.SGTMobile, Dominio.SignalR.Hubs.Mobile, Servicos.SignalR.Mobile.GetHub(MobileHubs.ChamadoEmAnalise));
                                        Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
                                    }
                                    else
                                    {
                                        if (chamado.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Finalizado && chamado.CargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue)
                                            situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Cancelada;

                                        Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(new { chamado.Codigo, Observacao = (!string.IsNullOrEmpty(chamado.ObservacaoRetornoMotorista) ? chamado.ObservacaoRetornoMotorista : (chamadoAnalise?.Observacao ?? "")), CargaEntrega = chamado.CargaEntrega.Codigo, ClienteMultisoftware = clienteMultisoftware, SituacaoChamado = chamado.Situacao, SituacaoCargaEntrega = chamado.CargaEntrega.SituacaoParaMobile, DiferencaDevolucao = chamado.CargaEntrega.NotificarDiferencaDevolucao }, clienteMultisoftware, motorista.CodigoMobile, Dominio.MSMQ.MSMQQueue.SGTMobile, Dominio.SignalR.Hubs.Mobile, Servicos.SignalR.Mobile.GetHub(MobileHubs.ChamadoAtualizado));
                                        Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
                                    }

                                }
                            }

                            List<Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario> chatMensagemDestinatarios = repChatMensagemDestinatario.BuscarMensagemPendentesRecebimento(usuarioMobile.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario chatMensagemDestinatario in chatMensagemDestinatarios)
                                Servicos.Embarcador.Chat.ChatMensagem.EnviarNotificacaoMobile(chatMensagemDestinatario.ChatMobileMensagem, clienteMultisoftware, usuarioMobile.Codigo);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            retorno = false;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno = false;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
            return retorno;
        }

        #region String de Conexões

        private int ObterMinutosSessao()
        {   
            return 30;
        }

        private static string StringConexao(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente)
        {
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao = usuarioMobileCliente.Cliente.ClienteConfiguracao;
            if (usuarioMobileCliente.BaseHomologacao && usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao != null)
                configuracao = usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao;

            return "Data Source=" + configuracao.DBServidor + ";Initial Catalog=" + configuracao.DBBase + ";User Id=" + configuracao.DBUsuario + ";Password=" + configuracao.DBSenha + ";Max Pool Size=500;";
        }

        #endregion
    }
}
