using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Servicos.Mobile.Autenticacao
{
    public class Autenticacao
    {
        private readonly AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork;

        public Autenticacao(AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork)
        {
            this.adminUnitOfWork = adminUnitOfWork;
        }

        public string Login(
            string cpf,
            string senha,
            string versaoApp,
            string uniqueId,
            string oneSignalPlayerId,
            bool utilizaAppTrizy,
            bool validarSenhaAutomatica,
            out AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile,
            out List<AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente> empresas
        )
        {
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(adminUnitOfWork);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repositorioUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(adminUnitOfWork);

            usuarioMobile = ObterUsuarioMobile(cpf, uniqueId, utilizaAppTrizy);
            if (usuarioMobile == null) throw new ServicoException("Usuário/Senha inválido");

            // Verifica a senha (se existir)
            bool requerContraSenha = repUsuarioMobileCliente.ClienteExigeContraSenha(usuarioMobile.Codigo);
            bool validarSenha = !string.IsNullOrWhiteSpace(senha);
            bool senhaValida = usuarioMobile.ContraSenha == senha;
            if (requerContraSenha && validarSenha && !senhaValida)
                throw new ServicoException("Usuário/Senha inválido");

            if (!ValidarSenhaAutomatica(usuarioMobile.CPF, senha, validarSenhaAutomatica))
            {
                if (usuarioMobile.TentativasAcessoInvalido < 3)
                {
                    usuarioMobile.TentativasAcessoInvalido += 1;
                    repositorioUsuarioMobile.Atualizar(usuarioMobile);
                }
                else
                    throw new ServicoException("Usuário bloqueado por tentativas inválidas");

                throw new ServicoException("Usuário/Senha inválido");
            }

            string tokenSessao = !requerContraSenha || (validarSenha && senhaValida) ? GerarSessao() : "";

            usuarioMobile.VersaoAPP = versaoApp;
            usuarioMobile.Sessao = tokenSessao;
            usuarioMobile.DataSessao = DateTime.Now;
            usuarioMobile.Senha = uniqueId; // A senha na entidade é, na verdade, o uniqueId do celular do motorista
            usuarioMobile.OneSignalPlayerId = oneSignalPlayerId;
            usuarioMobile.TentativasAcessoInvalido = 0;
            usuarioMobile.Ativo = true;
            repositorioUsuarioMobile.Atualizar(usuarioMobile);

            empresas = ObterEmpresas(usuarioMobile);

            return tokenSessao;
        }


        public List<AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente> ObterEmpresas(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile)
        {
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(adminUnitOfWork);
            List<AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente> usuariosMobileCliente = repUsuarioMobileCliente.BuscarPorUsuario(usuarioMobile.Codigo);
            var distinctUsuariosMobileCliente = usuariosMobileCliente.GroupBy(o => o.Cliente.Codigo).Select(grp => grp.FirstOrDefault()).ToList();
            return distinctUsuariosMobileCliente;
        }

        public AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile ObterUsuarioMobile(string cpf, string uniqueId, bool utilizaAppTrizy)
        {
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repositorioUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(adminUnitOfWork);
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = new AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile();

            string cpfUsuario = Utilidades.String.OnlyNumbers(cpf);

            if (string.IsNullOrWhiteSpace(cpfUsuario))
                return null;

            if (utilizaAppTrizy)
                usuarioMobile = repositorioUsuarioMobile.BuscarPorCFP(cpfUsuario);
            else
                usuarioMobile = repositorioUsuarioMobile.BuscarPorCFPESenha(cpfUsuario, uniqueId);

            if (usuarioMobile == null)
            {
                usuarioMobile = repositorioUsuarioMobile.BuscarPorCFP(Utilidades.String.OnlyNumbers(cpf));

                if (usuarioMobile != null && !string.IsNullOrWhiteSpace(usuarioMobile.Senha) && !usuarioMobile.NaoBloquearAcessoSimultaneo)
                {
                    throw new ServicoException("Seu usuário está vinculado a outro aparelho, por gentileza solicite ao embarcador que faça uma nova liberação para seu CPF.");
                }

                return usuarioMobile;
            }

            return usuarioMobile;
        }

        private string GerarSessao()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private bool ValidarSenhaAutomatica(string cpf, string senhaAPP, bool validarSenhaAutomatica)
        {
            if (!validarSenhaAutomatica)
                return true;            

            string cpfUsuario = Utilidades.String.OnlyNumbers(cpf);
            if (string.IsNullOrWhiteSpace(cpfUsuario) || cpfUsuario.Length < 2)
                return true;

            string cpfAjustado = cpfUsuario.Remove(0, 1);
            cpfAjustado = cpfAjustado.Remove(cpfAjustado.Length - 1, 1);
            string data = DateTime.Today.ToString("yyyyMMdd");

            string senha = string.Concat(cpfAjustado, data);
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(senha));
            var senhaHash = string.Concat(hash.Select(b => b.ToString("x2")));

            bool retornoSenha = senhaAPP == senhaHash;
            if (!retornoSenha)
                Servicos.Log.TratarErro("CPF " + cpf + " | Senha APP = " + senhaAPP + " | Senha WS = " + senhaHash, "ValidacaSenha");

            return retornoSenha;
        }

    }
}
