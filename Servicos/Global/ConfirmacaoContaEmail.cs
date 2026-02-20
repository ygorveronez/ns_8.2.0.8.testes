using System;

namespace Servicos.Global
{
    public class ConfirmacaoContaEmail
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ConfirmacaoContaEmail(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void GerarConfirmacaoContaEEnviarEmail(Dominio.Entidades.Usuario usuario, int codigoCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, string adminConexao)
        {
            if(usuario.Status != "A")
            {
                GerarTokenConfirmacaoConta(usuario);
                EnviarEmailConfirmacaoConta(usuario, codigoCliente, tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, adminConexao);
            }
        }

        public bool ConfirmarEmail(string tokenConfirmacaoEmail)
        {
            var repUsuario = new Repositorio.Usuario(_unitOfWork);
            var usuario = repUsuario.BuscarPorTokenConfirmacaoEmail(tokenConfirmacaoEmail);

            if(usuario != null && usuario.Status != "A")
            {
                usuario.Status = "A";
                usuario.TokenConfirmacaoConta = null;
                repUsuario.Atualizar(usuario);
                return true;
            }

            return false;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void GerarTokenConfirmacaoConta(Dominio.Entidades.Usuario usuario)
        {
            var repUsuario = new Repositorio.Usuario(_unitOfWork);

            Guid g = Guid.NewGuid();
            string token = Convert.ToBase64String(g.ToByteArray());
            token = token.Replace("=", "");
            token = token.Replace("+", "");
            token = token.Replace("/", "");
            token = token.Replace("*", "");

            usuario.TokenConfirmacaoConta = token;
            repUsuario.Atualizar(usuario);
        }

        private void EnviarEmailConfirmacaoConta(Dominio.Entidades.Usuario usuario, int codigoCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, string adminConexao)
        {
            var token = usuario.TokenConfirmacaoConta;
            var urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(codigoCliente, tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, adminConexao);
            var urlAtivacao = $"{urlBase}/Registro/ConfirmarConta?token={token}";
            var email = usuario.Cliente.Email;
            var assunto = "Confirmação de e-mail";
            var corpo = $@"<p>Bem-vindo {usuario.Nome}! Segue seus dados para entrar no sistema:
                Usuário: Seu CPF/CNPJ cadastrado no registro <b>(apenas números)</b>
                Senha: Senha cadastrada no registro</p>
                <p>Para seguir, basta confirmar seu endereço de e-mail clicando no link abaixo:</p><a href='{urlAtivacao}'>Confirmar meu e-mail</a>
                <br/>
                <small>
                Caso tenha alguma dificuldade para fazer a confirmação no botão acima, copie e cole a URL a seguir no seu navegador: <a href='{urlAtivacao}'>{urlAtivacao}</a>
                </small>";

            Servicos.Email.EnviarEmailAutenticado(email, assunto, corpo, _unitOfWork, out string msgErro, null);
        }

        #endregion Métodos Privados
    }
}
