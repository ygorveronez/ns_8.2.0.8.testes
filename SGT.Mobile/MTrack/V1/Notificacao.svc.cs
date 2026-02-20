using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.NovoApp.Comum;
using Dominio.ObjetosDeValor.NovoApp.Notificacao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Notificacao : BaseControllerNovoApp, INotificacao
    {
        public List<ResponseObterNotificacoes> ObterNotificacoes(int ultimoCodigoRecebido)
        {
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao();

            try
            {
                AdminMultisoftware.Repositorio.Mobile.NotificacaoOneSignal repNotificacoes = new AdminMultisoftware.Repositorio.Mobile.NotificacaoOneSignal(adminUnitOfWork);
                var notificacoes = repNotificacoes.BuscarPorUsuarioMobile(usuarioMobile.Codigo, ultimoCodigoRecebido);

                return (from o in notificacoes
                        select new ResponseObterNotificacoes
                        {
                            Codigo = o.Codigo,
                            Headings = o.Headings,
                            Contents = o.Contents,
                            Data = o.Data,
                            DataCriacao = o.DataCriacao.ToUnixSeconds(),
                            Tipo = o.Tipo,
                            Lida = o.Lida,
                            notificationId = o.IdOneSignal,
                        }).ToList();
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao obter as notificações", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }

            return null;
        }

        public ResponseBool ConfirmarLeituraNotificacao(RequestConfirmarLeituraNotificacao request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando ConfirmarLeituraNotificacao - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao();

            try
            {
                AdminMultisoftware.Repositorio.Mobile.NotificacaoOneSignal repNotificacoes = new AdminMultisoftware.Repositorio.Mobile.NotificacaoOneSignal(adminUnitOfWork);
                var notificacoes = repNotificacoes.BuscarPorCodigos(request.codigosNotificacaoes);

                bool lida = true; //request.lida ?? true; // Se algum dia precisar "Marcar notificação como não-lida", pode só mandar lida = false

                foreach (var notificacao in notificacoes)
                {
                    notificacao.Lida = lida;
                    repNotificacoes.Atualizar(notificacao);
                }

                Servicos.Log.TratarErro($"NovoApp - Finalizando ConfirmarLeituraNotificacao");

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
                RetornarErro("Ocorreu uma falha ao confirmar a leitura das notificações", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }
    }
}
