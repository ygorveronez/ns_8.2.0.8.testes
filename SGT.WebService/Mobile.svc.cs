using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Servicos.Embarcador.Notificacao;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Mobile(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IMobile
    {
        public Retorno<bool> EnviarNotificacao(List<string> cpfs, string mensagem)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();

            retorno.Status = true;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (cpfs.Count == 0)
                {
                    throw new WebServiceException("A lista de CPFs não pode estar vazia");
                }

                if (mensagem.Length > 250)
                {
                    throw new WebServiceException("A mensagem não deve ter mais que 250 caracteres");
                }

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                // Motoristas (filtrando para ter apenas um por cpf)
                var motoristas = repUsuario.BuscarMotoristaPorCPFs(0, cpfs).GroupBy(m => m.CPF).Select(g => g.FirstOrDefault()).ToList();
                if(motoristas.Count > 0)
                {
                    var serNotificacao = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);

                    dynamic conteudo = new
                    {
                        pt = mensagem,
                        es = mensagem,
                        en = mensagem,
                    };

                    serNotificacao.NotificarMotoristasPushGenerica(motoristas, conteudo);

                    // Novo app
                    var serMTrack = new NotificacaoMTrack(unitOfWork);
                    serMTrack.NotificarPushGenerica(motoristas, mensagem);

                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                    retorno.Mensagem = "Mensagem enviada com sucesso";
                } else
                {
                    throw new WebServiceException("Nenhum motorista encontrado");
                }
                
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a notificação";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos;
        }

        #endregion
    }
}
