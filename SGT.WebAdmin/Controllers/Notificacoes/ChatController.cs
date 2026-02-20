using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/Chat")]
    public class ChatController : BaseController
    {
		#region Construtores

		public ChatController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarUsuariosChat()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Servicos.Embarcador.Hubs.Chat hubChat = new Servicos.Embarcador.Hubs.Chat();

                int codigoFuncionarioLogado = this.Usuario.Codigo;
                int codigoEmpresa = 0, codigoEmpresaPai = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ||
                    TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                if (this.Usuario.Empresa.EmpresaPai != null)
                    codigoEmpresaPai = this.Usuario.Empresa.EmpresaPai.Codigo;

                IList<Dominio.ObjetosDeValor.Embarcador.Notificacao.ChatUsuario> listaUsuarios = repUsuario.BuscarUsuariosChat(codigoFuncionarioLogado, codigoEmpresa, codigoEmpresaPai);

                List<object> retorno = new List<object>();
                foreach (Dominio.ObjetosDeValor.Embarcador.Notificacao.ChatUsuario usuario in listaUsuarios)
                {
                    retorno.Add(new
                    {
                        CodigoUsuario = usuario.Codigo,
                        idLink = "chat" + usuario.Codigo.ToString(),
                        idChat = "box_" + usuario.Codigo.ToString(),
                        first_name = usuario.Nome,
                        last_name = string.Empty,
                        status = hubChat.VerificarUsuarioOnline(usuario.Codigo) ? "online" : "incognito",
                        alertmsg = string.Empty,
                        alertshow = false,
                        naoLidas = usuario.TotalMensagensNaoLidas,
                        total = usuario.TotalMensagens
                    });
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os usuários da empresa pro Chat.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SaveMessage()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("cod_user"), out int codigoFuncionario);
                string mensagem = Request.Params("msg");

                Repositorio.Embarcador.Notificacoes.Chat repChat = new Repositorio.Embarcador.Notificacoes.Chat(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario usuarioEnvio = repUsuario.BuscarPorCodigo(this.Usuario.Codigo);
                Dominio.Entidades.Usuario usuarioRecebedor = repUsuario.BuscarPorCodigo(codigoFuncionario);
                Dominio.Entidades.Embarcador.Notificacoes.Chat chat = new Dominio.Entidades.Embarcador.Notificacoes.Chat();

                chat.DataEnvio = DateTime.Now;
                chat.Mensagem = mensagem;
                chat.UsuarioEnvio = usuarioEnvio;
                chat.UsuarioRecebedor = usuarioRecebedor;
                chat.Lida = false;
                chat.DataLida = null;
                chat.EnviadoAvisoEmail = false;

                repChat.Inserir(chat);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Hubs.Chat hubChat = new Servicos.Embarcador.Hubs.Chat();
                hubChat.NotificarMensagemUsuario(usuarioEnvio.Codigo, usuarioRecebedor.Codigo, mensagem, chat.DataEnvio.ToString("dd/MM/yyyy HH:mm:ss"), "Eu");//Usuário de envio também recebe, para as outras seções também receber o que é enviado
                hubChat.NotificarMensagemUsuario(usuarioRecebedor.Codigo, usuarioEnvio.Codigo, mensagem, chat.DataEnvio.ToString("dd/MM/yyyy HH:mm:ss"), "Ele");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ||
                    TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    hubChat.CriarNotificaoChatOutroAmbiente(chat.Codigo, ObterURLBaseOutroAmbiente());

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Salvar a mensagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> LoadMessages()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int inicio = int.Parse(Request.Params("inicio"));
                int limite = int.Parse(Request.Params("limite"));
                int.TryParse(Request.Params("cod_user"), out int codigoFuncionario);
                int codigoFuncionarioLogado = this.Usuario.Codigo;

                Repositorio.Embarcador.Notificacoes.Chat repChat = new Repositorio.Embarcador.Notificacoes.Chat(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                List<Dominio.Entidades.Embarcador.Notificacoes.Chat> listaMensagens = repChat.BuscarPorConversa(codigoFuncionarioLogado, codigoFuncionario, "Codigo", "desc", inicio, limite);

                var retorno = new
                {
                    ListaMensagens = from obj in listaMensagens
                                     orderby //obj.Codigo ascending
                                     inicio == 0 ? 0 : obj.Codigo descending,
                                     inicio > 0 ? 0 : obj.Codigo ascending
                                     select new
                                     {
                                         obj.Codigo,
                                         DataEnvio = obj.DataEnvio.ToString("dd/MM/yyyy HH:mm:ss"),
                                         obj.Mensagem,
                                         QuemEnviou = obj.UsuarioEnvio.Codigo == codigoFuncionarioLogado ? "Eu" : "Ele"
                                     }
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao carregar as mensagens.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> MarcarMensagensComoLida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("cod_user"), out int codigoFuncionario);

                Repositorio.Embarcador.Notificacoes.Chat repChat = new Repositorio.Embarcador.Notificacoes.Chat(unitOfWork);
                List<Dominio.Entidades.Embarcador.Notificacoes.Chat> listaMensagens = repChat.BuscarMensagensNaoLidaPorConversa(this.Usuario.Codigo, codigoFuncionario);

                foreach (var message in listaMensagens)
                {
                    message.Lida = true;
                    message.DataLida = DateTime.Now;
                    repChat.Atualizar(message);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao marcar as mensagens como lidas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        public async Task<IActionResult> SendMessageOutroAmbiente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoChat = Request.GetIntParam("CodigoChat");

                Repositorio.Embarcador.Notificacoes.Chat repChat = new Repositorio.Embarcador.Notificacoes.Chat(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.Chat chat = repChat.BuscarPorCodigo(codigoChat, false);

                if (chat != null)
                {
                    Servicos.Embarcador.Hubs.Chat hubChat = new Servicos.Embarcador.Hubs.Chat();
                    hubChat.NotificarMensagemUsuario(chat.UsuarioEnvio.Codigo, chat.UsuarioRecebedor.Codigo, chat.Mensagem, chat.DataEnvio.ToString("dd/MM/yyyy HH:mm:ss"), "Eu");//Usuário de envio também recebe, para as outras seções também receber o que é enviado
                    hubChat.NotificarMensagemUsuario(chat.UsuarioRecebedor.Codigo, chat.UsuarioEnvio.Codigo, chat.Mensagem, chat.DataEnvio.ToString("dd/MM/yyyy HH:mm:ss"), "Ele");
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao disparar a notificação do chat em outro ambiente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterURLBaseOutroAmbiente()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

            var url = "";
            if (clienteAcesso != null)
            {
                var dominioAcesso = clienteAcesso.URLAcesso;
                if (dominioAcesso.Contains("."))
                    dominioAcesso = dominioAcesso.Substring(dominioAcesso.IndexOf("."));
                else dominioAcesso = string.Empty;

                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoParaOutroAmbiente = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoParaOutroAmbiente = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin;

                foreach (var urlAcesso in clienteAcesso.Cliente.ClienteURLsAcesso)
                {
                    if (urlAcesso.TipoServicoMultisoftware == tipoParaOutroAmbiente && !urlAcesso.URLAcesso.Contains("localhost") && urlAcesso.Ativo
                        && (!string.IsNullOrWhiteSpace(dominioAcesso) ? urlAcesso.URLAcesso.Contains(dominioAcesso) : urlAcesso.Ativo))
                    {
                        url = urlAcesso.URLAcesso;
                        break;
                    }
                }
            }
            unitOfWork.Dispose();

            return string.IsNullOrWhiteSpace(url) ? "" : $"http://{url}";
        }

        #endregion
    }
}
