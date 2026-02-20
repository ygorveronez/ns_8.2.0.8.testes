using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Notificacao
{
    public sealed class Notificacao
    {
        #region Atributos Privados

        private readonly string _adminStringConexao;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Notificacao(string stringConexao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao)
        {
            _adminStringConexao = adminStringConexao;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = cliente;
        }

        public Notificacao() { }

        #endregion

        #region Metodos Publicos

        public void GerarNotificacao(Dominio.Entidades.Usuario usuario, int codigoObjeto, string URLPagina, string nota, IconesNotificacao icone, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, Repositorio.UnitOfWork unitOfWork, int codigoClienteMultisoftware)
        {
            string stringConexao = unitOfWork.StringConexao;
            int codigoUsuario = usuario?.Codigo ?? 0;
            int codigoUsuarioGerouNotificacao = 0;

            Task.Factory.StartNew(() => gerarNotificacao(codigoUsuario, codigoUsuarioGerouNotificacao, codigoObjeto, URLPagina, "", nota, icone, icone.ObterCorFundoPadrao(), tipoNotificacao, tipoServicoMultisoftwareNotificar, false, stringConexao, codigoClienteMultisoftware));
        }

        public void GerarNotificacao(Dominio.Entidades.Usuario usuario, int codigoObjeto, string URLPagina, string nota, IconesNotificacao icone, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, Repositorio.UnitOfWork unitOfWork)
        {
            string stringConexao = unitOfWork.StringConexao;
            int codigoUsuario = usuario?.Codigo ?? 0;
            int codigoUsuarioGerouNotificacao = 0;

            Task.Factory.StartNew(() => gerarNotificacao(codigoUsuario, codigoUsuarioGerouNotificacao, codigoObjeto, URLPagina, "", nota, icone, icone.ObterCorFundoPadrao(), tipoNotificacao, tipoServicoMultisoftwareNotificar, false, stringConexao));
        }

        public void GerarNotificacao(Dominio.Entidades.Usuario usuario, int codigoObjeto, string URLPagina, string nota, IconesNotificacao icone, SmartAdminBgColor cor, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, Repositorio.UnitOfWork unitOfWork)
        {
            string stringConexao = unitOfWork.StringConexao;
            int codigoUsuario = usuario?.Codigo ?? 0;
            int codigoUsuarioGerouNotificacao = 0;

            Task.Factory.StartNew(() => gerarNotificacao(codigoUsuario, codigoUsuarioGerouNotificacao, codigoObjeto, URLPagina, "", nota, icone, cor, tipoNotificacao, tipoServicoMultisoftwareNotificar, false, stringConexao));
        }

        public void GerarNotificacao(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Usuario usuarioGerouNotificacao, int codigoObjeto, string URLPagina, string nota, IconesNotificacao icone, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, Repositorio.UnitOfWork unitOfWork)
        {
            string stringConexao = unitOfWork.StringConexao;
            int codigoUsuario = usuario?.Codigo ?? 0;
            int codigoUsuarioGerouNotificacao = usuarioGerouNotificacao?.Codigo ?? 0;

            Task.Factory.StartNew(() => gerarNotificacao(codigoUsuario, codigoUsuarioGerouNotificacao, codigoObjeto, URLPagina, "", nota, icone, icone.ObterCorFundoPadrao(), tipoNotificacao, tipoServicoMultisoftwareNotificar, false, stringConexao));
        }

        public void GerarNotificacao(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Usuario usuarioGerouNotificacao, int codigoObjeto, string URLPagina, string nota, IconesNotificacao icone, SmartAdminBgColor cor, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, Repositorio.UnitOfWork unitOfWork)
        {
            string stringConexao = unitOfWork.StringConexao;
            int codigoUsuario = usuario?.Codigo ?? 0;
            int codigoUsuarioGerouNotificacao = usuarioGerouNotificacao?.Codigo ?? 0;

            Task.Factory.StartNew(() => gerarNotificacao(codigoUsuario, codigoUsuarioGerouNotificacao, codigoObjeto, URLPagina, "", nota, icone, cor, tipoNotificacao, tipoServicoMultisoftwareNotificar, false, stringConexao));
        }

        public void GerarNotificacaoEmail(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Usuario usuarioGerouNotificacao, int codigoObjeto, string URLPagina, string titulo, string nota, IconesNotificacao icone, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, Repositorio.UnitOfWork unitOfWork)
        {
            string stringConexao = unitOfWork.StringConexao;
            int codigoUsuario = usuario?.Codigo ?? 0;
            int codigoUsuarioGerouNotificacao = usuarioGerouNotificacao?.Codigo ?? 0;

            Task.Factory.StartNew(() => gerarNotificacao(codigoUsuario, codigoUsuarioGerouNotificacao, codigoObjeto, URLPagina, titulo, nota, icone, icone.ObterCorFundoPadrao(), tipoNotificacao, tipoServicoMultisoftwareNotificar, true, stringConexao));
        }

        public void InfomarPercentualProcessamento(Dominio.Entidades.Usuario usuario, int codigoObjeto, string pagina, decimal percentual, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, Repositorio.UnitOfWork unitOfWork)
        {
            Task.Factory.StartNew(() => gerarPercentualProcessamento(usuario, codigoObjeto, pagina, percentual, tipoNotificacao, tipoServicoMultisoftwareNotificar));
        }

        public static dynamic ObterObjetoRetorno(Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao)
        {
            return new
            {
                notificacao.Codigo,
                notificacao.CodigoObjetoNotificacao,
                DataNotificacao = notificacao.DataNotificacao.ToString("dd/MM/yyyy HH:mm"),
                notificacao.SituacaoNotificacao,
                notificacao.TipoNotificacao,
                notificacao.URLPagina,
                notificacao.Nota,
                Icone = notificacao.Icone.ObterDescricao(),
                IconeCorFundo = "bg-color-" + notificacao.IconeCorFundo.ToString(),
                ReadClass = notificacao.SituacaoNotificacao == SituacaoNotificacao.Nova ? "unread" : ""
            };
        }

        #endregion

        #region Métodos Privados

        private void gerarNotificacao(int codigoUsuario, int codigoUsuarioGerouNotificacao, int codigoObjeto, string URLPagina, string titulo, string nota, IconesNotificacao icone, SmartAdminBgColor cor, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, bool enviarEmailNotificacao, string stringConexao, int codigoClienteMultisoftware = 0)
        {
            if (codigoUsuario <= 0)
                return;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, TipoSessaoBancoDados.Nova);

            try
            {
                Repositorio.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada repNotoficacaoQuantidadeNaoVisualizada = new Repositorio.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada(unitOfWork);
                Repositorio.Embarcador.Notificacoes.Notificacao repNotificacao = new Repositorio.Embarcador.Notificacoes.Notificacao(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario usuarioGerouNotificacao = null;
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                if (codigoUsuarioGerouNotificacao > 0)
                    usuarioGerouNotificacao = repUsuario.BuscarPorCodigo(codigoUsuarioGerouNotificacao);

                Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao = new Dominio.Entidades.Embarcador.Notificacoes.Notificacao
                {
                    CodigoObjetoNotificacao = codigoObjeto,
                    DataNotificacao = DateTime.Now,
                    Nota = Utilidades.String.Left(nota, 1000),
                    URLPagina = URLPagina,
                    UsuarioGerouNotificacao = usuarioGerouNotificacao,
                    Icone = icone,
                    TipoNotificacao = tipoNotificacao,
                    IconeCorFundo = cor,
                    Usuario = usuario,
                    SituacaoNotificacao = SituacaoNotificacao.Nova
                };

                repNotificacao.Inserir(notificacao);

                Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada notoficacaoQuantidadeNaoVisualizada = repNotoficacaoQuantidadeNaoVisualizada.BuscarPorUsuario(usuario.Codigo);

                if (notoficacaoQuantidadeNaoVisualizada == null)
                {
                    notoficacaoQuantidadeNaoVisualizada = new Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada();
                    notoficacaoQuantidadeNaoVisualizada.Usuario = usuario;
                    notoficacaoQuantidadeNaoVisualizada.QuantidadeNaoVisualizada++;
                    repNotoficacaoQuantidadeNaoVisualizada.Inserir(notoficacaoQuantidadeNaoVisualizada);
                }
                else
                {
                    notoficacaoQuantidadeNaoVisualizada.QuantidadeNaoVisualizada++;
                    repNotoficacaoQuantidadeNaoVisualizada.Atualizar(notoficacaoQuantidadeNaoVisualizada);
                }
                //todo: se necessário tornar assincrona a chamada do métodos que envia as notificações;

                NotificarUsuarioOnline(tipoServicoMultisoftwareNotificar, codigoClienteMultisoftware, notificacao, unitOfWork);

                bool notificacaoRelatorio = (int)_tipoServicoMultisoftware == 0;
                bool enviarNotificacaoPorEmail = enviarEmailNotificacao || (!notificacaoRelatorio && Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarTodasNotificacoesPorEmail.Value);
                bool usuarioConfiguradoReceberNotificacaoPorEmail = (usuario?.EnviarNotificacaoPorEmail ?? false) && !string.IsNullOrWhiteSpace(usuario?.Email);

#if DEBUG
                // Desativa envio de e-mail no ambiente de desenvolvimento
                enviarNotificacaoPorEmail = false;
#endif

                if (usuarioConfiguradoReceberNotificacaoPorEmail && enviarNotificacaoPorEmail)
                {
                    Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmail notificacaoEmail = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmail()
                    {
                        Data = notificacao.DataNotificacao,
                        Email = notificacao.Usuario.Email,
                        Nota = notificacao.Nota,
                        QuantidadeNotificacoesNaoVisualizadas = notoficacaoQuantidadeNaoVisualizada.QuantidadeNaoVisualizada,
                        Titulo = titulo
                    };

                    GerarEnviarEmailNotificao(notificacaoEmail, _tipoServicoMultisoftware, unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Notificacao");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void NotificarUsuarioOnline(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar, int codigoClienteMultisoftware, Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente();

            if (notificacao.TipoNotificacao != TipoNotificacao.relatorio && (configuracaoAmbiente.DesabilitarPopUpsNotificacao.HasValue && configuracaoAmbiente.DesabilitarPopUpsNotificacao.Value))
                return;

            if (_tipoServicoMultisoftware == tipoServicoMultisoftwareNotificar)
            {
                if (codigoClienteMultisoftware > 0)
                {
                    MSMQ.MSMQ.SendPrivateMessage(new Dominio.MSMQ.Notification()
                    {
                        ClientMultisoftwareID = codigoClienteMultisoftware,
                        Content = Servicos.Embarcador.Notificacao.Notificacao.ObterObjetoRetorno(notificacao),
                        Hub = Dominio.SignalR.Hubs.Notificacao,
                        MSMQQueue = tipoServicoMultisoftwareNotificar == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Dominio.MSMQ.MSMQQueue.MultiCTe : tipoServicoMultisoftwareNotificar == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros ? Dominio.MSMQ.MSMQQueue.Terceiros : tipoServicoMultisoftwareNotificar == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor ? Dominio.MSMQ.MSMQQueue.Fornecedor : tipoServicoMultisoftwareNotificar == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro ? Dominio.MSMQ.MSMQQueue.TransportadorTerceiro : Dominio.MSMQ.MSMQQueue.SGTWebAdmin,
                        UsersID = new List<int>() { notificacao.Usuario?.Codigo ?? 0 }
                    });
                }
                else
                {
                    Servicos.Embarcador.Hubs.Notificacao hubNotificacao = new Hubs.Notificacao();
                    hubNotificacao.NotificarUsuario(notificacao);
                }
            }
            else
                InformarNotificacaoOutroAmbiente(notificacao.Codigo, tipoServicoMultisoftwareNotificar);
        }

        private void InformarNotificacaoOutroAmbiente(int codigoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar)
        {
            if (string.IsNullOrWhiteSpace(_adminStringConexao))
                return;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_adminStringConexao);

            try
            {
                if (_clienteMultisoftware == null)
                    return;

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repConfig = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repConfig.BuscarPorClienteETipo(_clienteMultisoftware.Codigo, tipoServicoMultisoftwareNotificar);

                if (clienteURLAcesso == null)
                    return;

                string urlJanelaCarregamento = "http://" + clienteURLAcesso.URLAcesso;

                if (clienteURLAcesso.URLAcesso.Contains("192.168.0.125"))
                    urlJanelaCarregamento += "/Embarcador";

                urlJanelaCarregamento += "/Notificacao/DispararNotificacao?Notificacao=" + codigoNotificacao;

                WebRequest wRequest = WebRequest.Create(urlJanelaCarregamento);
                wRequest.Method = "GET";

                WebResponse response = wRequest.GetResponse();

                response.Close();
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

        private void GerarEnviarEmailNotificao(Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmail notificacaoEmail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfiguracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfiguracaoEmail.BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string from = configuracaoEmail.Email;
                string user = configuracaoEmail.Email;
                string password = configuracaoEmail.Senha;
                string recepient = notificacaoEmail.Email;
                //recepient = "dev.multisoftware@outlook.com";
                string[] bcc = new string[] { };
                string[] cc = new string[] { };
                string servidorSMTP = configuracaoEmail.Smtp;
                string signature = "";
                bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                List<System.Net.Mail.Attachment> attachments = null;
                string subject = $"Nova notificação{(string.IsNullOrWhiteSpace(notificacaoEmail.Titulo) ? "" : $" de {notificacaoEmail.Titulo}")} - {tipoServicoMultisoftware.ToString("G")}";
                string notificacoesNaoLidas = "";

                if (notificacaoEmail.QuantidadeNotificacoesNaoVisualizadas == 1)
                    notificacoesNaoLidas = "Existe 1 notificação não visualizada.";
                else if (notificacaoEmail.QuantidadeNotificacoesNaoVisualizadas > 1)
                    notificacoesNaoLidas = "Existem " + notificacaoEmail.QuantidadeNotificacoesNaoVisualizadas + " notificações não visualizadas.";

                string cabecalho = $"Você possui uma nova notificação{(string.IsNullOrWhiteSpace(notificacaoEmail.Titulo) ? "" : $" de {notificacaoEmail.Titulo}")} pendente.";
                string body = BodyEmailNotificacao(notificacaoEmail, cabecalho, notificacoesNaoLidas);

                if (!Servicos.Email.EnviarEmail(from, user, password, recepient, bcc, cc, subject, body, servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, attachments, signature, possuiSSL, string.Empty, 0, unitOfWork))
                    Servicos.Log.TratarErro(erro, "NotificaoPorEmail");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "NotificaoPorEmail");
            }
        }

        private void gerarPercentualProcessamento(Dominio.Entidades.Usuario usuario, int codigoObjeto, string pagina, decimal percentual, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar)
        {
            if (_tipoServicoMultisoftware == tipoServicoMultisoftwareNotificar)
            {
                Servicos.Embarcador.Hubs.Notificacao hubNotificacao = new Hubs.Notificacao();
                hubNotificacao.InformarPercentualProcessado(tipoNotificacao, codigoObjeto, pagina, percentual, usuario);
            }
            else
            {
                InfomarPercentualProcessamentoAmbiente(usuario, codigoObjeto, pagina, percentual, tipoNotificacao, tipoServicoMultisoftwareNotificar);
            }
        }

        private string BodyEmailNotificacao(Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmail notificacaoEmail, string cabecalho, string rodape)
        {
            string[] splitComposto = notificacaoEmail.Nota.Split('|');
            string nota = string.Empty;
            if (splitComposto.Length > 0)
            {
                for (int i = 0; i < splitComposto.Length; i++)
                    nota = string.Concat(nota, @"<p style=""margin:0px"">" + splitComposto[i] + @"</p>");
            }
            else
                nota = @"<p style=""margin:0px"">" + notificacaoEmail.Nota + @"</p>";

            //<p style=""margin:0px"">" + notificacao.Nota + @"</p>
            string body = @"
<div style=""font-family: Arial;"">
    <p><strong>" + cabecalho + @"</strong></p>
    " + nota + @"
    <p style=""font-size: 12px; margin:0px"" >" + notificacaoEmail.Data.ToString("dd/MM/yyyy HH:mm") + @"</p>
    <p style=""font-size: 12px; margin-bottom:0px"">" + rodape + @"</p>
    <p></p>
    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>
</div>";

            return body;
        }

        private void InfomarPercentualProcessamentoAmbiente(Dominio.Entidades.Usuario usuario, int codigoObjeto, string pagina, decimal percentual, TipoNotificacao tipoNotificacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftwareNotificar)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_adminStringConexao);
            try
            {
                if (_clienteMultisoftware == null)
                    return;

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repConfig = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repConfig.BuscarPorClienteETipo(_clienteMultisoftware.Codigo, tipoServicoMultisoftwareNotificar);

                if (clienteURLAcesso == null)
                    return;

                string urlJanelaCarregamento = "http://" + clienteURLAcesso.URLAcesso;

                if (clienteURLAcesso.URLAcesso.Contains("192.168.0.125"))
                    urlJanelaCarregamento += "/Embarcador";

                int codigoUsuario = 0;
                if (usuario != null)
                    codigoUsuario = usuario.Codigo;

                urlJanelaCarregamento += "/Notificacao/DispararProcessamento?Codigo=" + codigoObjeto + "&Pagina=" + pagina + "&Percentual=" + percentual + "&TipoNotificacao=" + tipoNotificacao + "&Usuario=";

                WebRequest wRequest = WebRequest.Create(urlJanelaCarregamento);
                wRequest.Method = "GET";

                WebResponse response = wRequest.GetResponse();

                response.Close();
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

        #endregion
    }
}
