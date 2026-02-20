using System;
using Dominio.Excecoes.Embarcador;
using System.Collections.Generic;

namespace SGT.Mobile
{
    public class Notificacao : WebServiceBase, INotificacao
    {
        #region Métodos Públicos

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao> ObterNotificacao(string token, int usuario, int empresaMultisoftware, int codigoNotificacao)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.Notificacao.NotificacaoMobile servicoNotificacaoMobile = new Servicos.Embarcador.Notificacao.NotificacaoMobile(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao notificacao = servicoNotificacaoMobile.ObterNotificacao(codigoNotificacao);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao>.CriarRetornoSucesso(notificacao);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao>.CriarRetornoExcecao("Ocorreu uma falha ao obter a notificação");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida>> ObterNotificacoes(string token, int usuario, int empresaMultisoftware, bool somenteNaoLidas)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario usuarioNotificacao = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Notificacao.NotificacaoMobile servicoNotificacaoMobile = new Servicos.Embarcador.Notificacao.NotificacaoMobile(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida> notificacoes = servicoNotificacaoMobile.ObterNotificacoes(usuarioNotificacao.Codigo, somenteNaoLidas);

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida>>.CriarRetornoSucesso(notificacoes);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida>>.CriarRetornoExcecao("Ocorreu uma falha ao obter as notificações");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion
    }
}
