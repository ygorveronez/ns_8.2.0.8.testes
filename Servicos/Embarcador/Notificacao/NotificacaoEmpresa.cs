using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Notificacao
{
    public sealed class NotificacaoEmpresa
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte _configuracaoEmail;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public NotificacaoEmpresa(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmail: null) { }

        public NotificacaoEmpresa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            _configuracaoEmail = configuracaoEmail;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void GerarNotificacaoEmail(Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            try
            {
                if (notificacaoEmpresa.Empresa == null)
                    return;

                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(notificacaoEmpresa.Empresa.Codigo);

                List<string> emails = new List<string>();

                if (!string.IsNullOrWhiteSpace(empresa.Email))
                    emails.AddRange(empresa.Email.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

                if (!notificacaoEmpresa.NotificarSomenteEmailPrincipal)
                {
                    if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo))
                        emails.AddRange(empresa.EmailAdministrativo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

                    if (!string.IsNullOrWhiteSpace(empresa.EmailContador))
                        emails.AddRange(empresa.EmailContador.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }

                if (emails.Count == 0)
                    return;

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork).BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string de = configuracaoEmail.Email;
                string login = configuracaoEmail.Email;
                string senha = configuracaoEmail.Senha;
                string[] copiaOcultaPara = new string[] { };
                string[] copiaPara = new string[] { };
                string corpo = ObterBodyEmailNotificacao(notificacaoEmpresa);
                string servidorSMTP = configuracaoEmail.Smtp;
                List<System.Net.Mail.Attachment> anexos = notificacaoEmpresa.Anexos;
                string assinatura = "";
                bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                string responderPara = "";
                int porta = configuracaoEmail.PortaSmtp;

                foreach (string para in emails.Distinct())
                {
                    if (!Servicos.Email.EnviarEmail(de, login, senha, para, copiaOcultaPara, copiaPara, notificacaoEmpresa.AssuntoEmail, corpo, servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, unitOfWork))
                        Log.TratarErro(erro, "notificao_empresa_por_email");
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "notificao_empresa_por_email");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte ObterConfiguracaoEmail()
        {
            if (_configuracaoEmail == null)
                _configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork).BuscarEmailEnviaDocumentoAtivo();

            return _configuracaoEmail;
        }

        private string ObterBodyEmailNotificacao(Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa)
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine(@"<div style=""font-family: Arial;"">");
            body.AppendLine($@"    <p><strong>{notificacaoEmpresa.CabecalhoMensagem}</strong></p>", !string.IsNullOrWhiteSpace(notificacaoEmpresa.CabecalhoMensagem));
            body.AppendLine($@"    <p style=""margin:0px"">{notificacaoEmpresa.Mensagem}</p>");
            body.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
            body.AppendLine("    <p></p>");
            body.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
            body.AppendLine("</div>");

            return body.ToString();
        }

        #endregion

        #region Métodos Públicos

        public void GerarNotificacao(Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa, string url, IconesNotificacao icone, TipoNotificacao tipo)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            List<Dominio.Entidades.Usuario> usuarios = repositorioUsuario.BuscarUsuariosAcessoTransportador(notificacaoEmpresa.Empresa.Codigo);
            Notificacao servicoNotificacao = new Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: notificacaoEmpresa.TipoServicoMultisoftware, adminStringConexao: string.Empty);

            foreach (Dominio.Entidades.Usuario usuario in usuarios)
            {
                servicoNotificacao.GerarNotificacaoEmail(
                    usuario: usuario,
                    usuarioGerouNotificacao: null,
                    codigoObjeto: notificacaoEmpresa.CodigoOrigemNotificacao,
                    URLPagina: url,
                    titulo: notificacaoEmpresa.CabecalhoMensagem,
                    nota: notificacaoEmpresa.Mensagem,
                    icone: icone,
                    tipoNotificacao: tipo,
                    tipoServicoMultisoftwareNotificar: notificacaoEmpresa.TipoServicoMultisoftware,
                    unitOfWork: _unitOfWork
                );
            }
        }

        public void GerarNotificacaoEmail(Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa)
        {
            Task.Factory.StartNew(() => GerarNotificacaoEmail(notificacaoEmpresa, _unitOfWork.StringConexao));
        }

        public void EnviarEmailCotacaoDisponivel(List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa> notificacoesEmpresas)
        {
            try
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacao in notificacoesEmpresas)
                {
                    Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = ObterConfiguracaoEmail();

                    if (configuracaoEmail == null)
                        return;

                    string de = configuracaoEmail.Email;
                    string login = configuracaoEmail.Email;
                    string senha = configuracaoEmail.Senha;
                    string[] copiaOcultaPara = new string[] { };
                    string[] copiaPara = new string[] { };
                    string corpo = ObterBodyEmailNotificacao(notificacao);
                    string servidorSMTP = configuracaoEmail.Smtp;
                    List<System.Net.Mail.Attachment> anexos = null;
                    string assinatura = "";
                    bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                    string responderPara = "";
                    int porta = configuracaoEmail.PortaSmtp;

                    if (notificacao.Empresa?.EmailAdministrativo != null && notificacao.Empresa.EmailAdministrativo.Contains(';'))
                        copiaPara = notificacao.Empresa.EmailAdministrativo.Trim().Split(';');
                    else if (notificacao.Empresa?.EmailAdministrativo != null)
                        copiaPara = copiaPara.Append(notificacao.Empresa.EmailAdministrativo.Trim()).ToArray();

                    if (!Servicos.Email.EnviarEmail(de, login, senha, notificacao.Empresa.Email, copiaOcultaPara, copiaPara, notificacao.AssuntoEmail, corpo, servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, _unitOfWork, 0, true))
                        Log.TratarErro(erro, "notificao_empresa_por_email_cotacao_disponivel");
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "notificao_empresa_por_email_cotacao_disponivel");
            }
        }

        #endregion
    }
}