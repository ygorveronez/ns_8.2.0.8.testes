using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Servicos.Embarcador.Transportadores
{
    public class SolicitacaoToken
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public SolicitacaoToken(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados
        private static string GenerarToken()
        {
            DateTime now = DateTime.Now;

            string timestamp = now.ToString("yyyyMMddHHmmssfff");

            string token = CalcularMD5Hash(timestamp);
            return token;
        }

        private static string CalcularMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void EnviarEmailsAsyncAguardandoAprovacao(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacaoToken, string emailCliente)
        {
            List<Dominio.ObjetosDeValor.Email.Mensagem> emails = new List<Dominio.ObjetosDeValor.Email.Mensagem>();

            foreach (var transportador in solicitacaoToken.Transportadores)
            {
                emails.Add(new Dominio.ObjetosDeValor.Email.Mensagem()
                {
                    Assunto = "Solicitação de Token para Integração",
                    Corpo = $@"<h4>{transportador.NomeCNPJ}</h4>
                           <p>Informamos que por meio do Protocolo: {solicitacaoToken.NumeroProtocolo}, fora solicitado a criação de Token para Integração com vigência de {solicitacaoToken.DataInicioVigencia.ToDateString()} à {solicitacaoToken.DataFimVigencia.ToDateString()}.</p>
                           <p>Solicitação encaminhada para aprovação.</p>",
                    Destinatarios = transportador.Email.Split(';').ToList()
                });
            }

            if (!string.IsNullOrEmpty(emailCliente))
            {
                emails.Add(new Dominio.ObjetosDeValor.Email.Mensagem()
                {
                    Assunto = $"Solicitação de Token para Integração: {solicitacaoToken.NumeroProtocolo}, aguardando Aprovação",
                    Corpo = $@"<p>Informamos que por meio do Protocolo: {solicitacaoToken.NumeroProtocolo}, foi solicitado a criação de Token para Integração para: {string.Join(", ", solicitacaoToken.Transportadores.Select(x => x.NomeCNPJ))}. Com vigência de {solicitacaoToken.DataInicioVigencia.ToDateString()} à {solicitacaoToken.DataFimVigencia.ToDateString()}.</p>
                           <p>Solicitação encaminhada para aprovação.</p>",
                    Destinatarios = emailCliente.Split(';').ToList()
                });
            }

            Servicos.Email.EnviarMensagensAsync(emails, _unitOfWork);
        }

        public void EnviarEmailsAsyncTokenGerado(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacaoToken, string emailCliente)
        {
            List<Dominio.ObjetosDeValor.Email.Mensagem> emails = new List<Dominio.ObjetosDeValor.Email.Mensagem>();

            List<string> metodos = solicitacaoToken.PermissoesWS.Count > 0 ? solicitacaoToken.PermissoesWS.Select(x => $"METODO {x.NomeMetodo}").ToList() : new List<string>();

            foreach (var transportador in solicitacaoToken.Transportadores)
            {
                emails.Add(new Dominio.ObjetosDeValor.Email.Mensagem()
                {
                    Assunto = "Credenciais de Token para Integração",
                    Corpo = $@"<h4> TRANSPORTADOR {transportador.NomeCNPJ}</h4>
                           <p>Informamos que por meio do Protocolo: {solicitacaoToken.NumeroProtocolo}, fora solicitado a criação de Token para Integração com vigência de {solicitacaoToken.DataInicioVigencia.ToDateString()} à {solicitacaoToken.DataFimVigencia.ToDateString()}.</p>
                           <p>para os seguintes métodos:</p>
                            {string.Join("<br/>", metodos)}
                           <p>Token cadastrado com sucesso. O registro está disponível no Portal do Transportador.</p>",
                    Destinatarios = transportador.Email.Split(';').ToList()
                });
            }

            emails.Add(new Dominio.ObjetosDeValor.Email.Mensagem()
            {
                Assunto = $"Solicitação de Token para Integração: {solicitacaoToken.NumeroProtocolo}, aguardando Aprovação",
                Corpo = $@"<p>Informamos que por meio do Protocolo: {solicitacaoToken.NumeroProtocolo}, foi solicitado a criação de Token para Integração para a(s) entidade(s): {string.Join(", ", solicitacaoToken.Transportadores.Select(x => x.NomeCNPJ))}. Com vigência de {solicitacaoToken.DataInicioVigencia.ToDateString()} à {solicitacaoToken.DataFimVigencia.ToDateString()}.</p>
                           <p>para os seguintes métodos:</p>
                            {string.Join("<br/>", metodos)}
                           <p>Token cadastrado com sucesso</p>",
                Destinatarios = emailCliente.Split(';').ToList()
            });

            Servicos.Email.EnviarMensagensAsync(emails, _unitOfWork);
        }

        public void ProcessarSocilitacoesTokenPendentesDeGeracao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso cliente)
        {
            Repositorio.Embarcador.Transportadores.SolicitacaoToken repotiroSolicitacaoToken = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken> solicitacoesTokenEmLiberacao = repotiroSolicitacaoToken.BuscarSolicitacoesEmLiberacao();
            Repositorio.Embarcador.Transportadores.SolicitacaoTokenTransportador repositorioSolicitacaoTokenTransportador = new Repositorio.Embarcador.Transportadores.SolicitacaoTokenTransportador(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacao in solicitacoesTokenEmLiberacao)
            {
                try
                {
                    List<Dominio.Entidades.Empresa> transportadores = solicitacao.Transportadores.ToList();

                    foreach (Dominio.Entidades.Empresa transportador in transportadores)
                    {
                        Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador solicitacaoTokenTransportador = new Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador();

                        solicitacaoTokenTransportador.Situacao = true;
                        solicitacaoTokenTransportador.Transportador = transportador;
                        solicitacaoTokenTransportador.SolicitacaoToken = solicitacao;

                        if (solicitacao.TipoAutenticacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutenticacao.UsuarioESenha)
                            solicitacaoTokenTransportador.Usuario = CriarUsuario(transportador, _unitOfWork);

                        if(solicitacao.TipoAutenticacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutenticacao.Token)
                            solicitacaoTokenTransportador.Token = GenerarToken();

                        repositorioSolicitacaoTokenTransportador.Inserir(solicitacaoTokenTransportador);
                    }
                    solicitacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoToken.Finalizada;
                    repotiroSolicitacaoToken.Atualizar(solicitacao);

                    EnviarEmailsAsyncTokenGerado(solicitacao, cliente.Cliente.Email);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    solicitacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoToken.EmLiberacaoSistematica;
                    repotiroSolicitacaoToken.Atualizar(solicitacao);
                }
            }

        }
        #endregion Métodos Públicos

        #region Métodos Privados
        private Dominio.Entidades.Usuario CriarUsuario(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Servicos.Senha svcSenha = new Servicos.Senha();

            string login = MontarLogin(empresa.RazaoSocial, empresa.CNPJ);
            string senha = svcSenha.GerarSenha(12);

            Dominio.Entidades.Usuario usuarioAux = repUsuario.BuscarPorLogin(login);

            if (usuarioAux == null)
            {
                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario();

                usuario.Setor = repSetor.BuscarPorCodigo(1);
                usuario.CPF = empresa.CNPJ;
                usuario.Email = "";
                usuario.Empresa = empresa;
                usuario.Endereco = empresa.Endereco;
                usuario.Localidade = empresa.Localidade;
                usuario.Login = login;
                usuario.Senha = senha;
                usuario.Status = "A";
                usuario.Tipo = "U";
                usuario.Telefone = empresa.Telefone;
                usuario.Nome = empresa.RazaoSocial + " - integração";
                usuario.Email = string.IsNullOrWhiteSpace(empresa.Email) ? string.Empty : empresa.Email.Split(';')[0];
                usuario.UsuarioAdministrador = false;
                usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Terceiro;

                repUsuario.Inserir(usuario);

                return usuario;
            }

            return usuarioAux;
        }

        private string MontarLogin(string razaoSocial, string cnpj)
        {
            string[] razaoSocialSplit = razaoSocial.Split(' ').Take(2).ToArray();

            string razaoSocialAux = string.Join("", razaoSocialSplit) + cnpj.Substring(0,5);

            return razaoSocialAux;
        }

        #endregion
    }
}
