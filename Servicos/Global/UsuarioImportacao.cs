using Dominio.Entidades.Embarcador.Usuarios;
using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
namespace Servicos
{
    public class UsuarioImportacao
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Empresa _empresa;
        PoliticaSenha _politicaSenha;
        #endregion

        #region Construtores
        public UsuarioImportacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dictionary<string, dynamic> dados, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _empresa = empresa;
            _politicaSenha = politicaSenha;
        }
        #endregion

        #region Méotodos públicos
        public Dominio.Entidades.Usuario ObterUsuarioImportar()
        {
            Dominio.Entidades.Usuario usuario = ObterUsuario();
            usuario.Nome = ObterNome();
            usuario.CPF = ObterCPF();
            usuario.Email = ObterEmail();
            usuario.Telefone = ObterTelefone();
            usuario.Setor = ObterSetor();
            usuario.CargoSetorTurno = ObterCargo();
            usuario.NivelEscalationList = ObterNivelEscalationList();
            usuario.ClientesSetor = ObterClientes();
            usuario.Login = ObterLogin();

            usuario.Empresa = _empresa;
            usuario.Tipo = "U";
            usuario.DataCadastro = DateTime.Now;
            usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Embarcador;
            usuario.TipoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos;
            usuario.Status = "A";
            usuario.UsuarioMultisoftware = false;
            this.SetPolitica(usuario);
            return usuario;
        }
        #endregion

        #region Métodos privados
        private Dominio.Entidades.Usuario ObterUsuario()
        {
            string cpfUsuario = string.Empty;

            if (_dados.TryGetValue("CPF", out var cpf))
                cpfUsuario = ((string)cpf);

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCPF(cpfUsuario);

            if (usuario == null)
            {
                usuario = new Dominio.Entidades.Usuario();
                usuario.CPF = cpfUsuario;
            }
            else
                usuario.Initialize();

            return usuario;
        }

        private string ObterNome()
        {
            string nome = string.Empty;

            if (_dados.TryGetValue("Nome", out var nomeUsuario))
                nome = ((string)nomeUsuario);

            if (string.IsNullOrWhiteSpace(nome))
                throw new ImportacaoException("Obrigatório informar o nome.");

            return nome;
        }

        private string ObterCPF()
        {
            string cpf = string.Empty;

            if (_dados.TryGetValue("CPF", out var cpfUsuario))
                cpf = ((string)cpfUsuario);

            if (string.IsNullOrWhiteSpace(cpf))
                throw new ImportacaoException("Obrigatório informar o CPF.");


            return cpf;
        }

        private string ObterEmail()
        {
            string email = string.Empty;

            if (_dados.TryGetValue("Email", out var emailRecebido))
                email = ((string)emailRecebido);

            if (string.IsNullOrWhiteSpace(email))
                throw new ImportacaoException("Obrigatório informar o Email.");


            return email;
        }

        private string ObterTelefone()
        {
            string telefone = string.Empty;

            if (_dados.TryGetValue("Telefone", out var telefoneRecebido))
                telefone = ((string)telefoneRecebido);

            return telefone;
        }

        private Dominio.Entidades.Setor ObterSetor()
        {
            string descricaoSetor = string.Empty;

            if (_dados.TryGetValue("Setor", out var descricaoSetorRecebido))
                descricaoSetor = ((string)descricaoSetorRecebido);

            if (string.IsNullOrEmpty(descricaoSetor))
                return null;

            Repositorio.Setor repositorioSetor = new Repositorio.Setor(_unitOfWork);
            Dominio.Entidades.Setor setor = repositorioSetor.BuscarPorDescricao(descricaoSetor.ToLower());

            return setor;
        }

        private Dominio.Entidades.Embarcador.Pessoas.Cargo ObterCargo()
        {
            string descricaoCargo = string.Empty;

            if (_dados.TryGetValue("Cargo", out var descricaoCargoRecebido))
                descricaoCargo = ((string)descricaoCargoRecebido);

            if (string.IsNullOrEmpty(descricaoCargo))
                return null;

            Repositorio.Embarcador.Pessoas.Cargo repositorioCargo = new Repositorio.Embarcador.Pessoas.Cargo(_unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.Cargo cargo = repositorioCargo.BuscarPorDescricao(descricaoCargo?.ToLower());

            return cargo != null ? cargo : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.EscalationList ObterNivelEscalationList()
        {
            string nivel = string.Empty;
            int num = 0;

            if (_dados.TryGetValue("NivelEscalation", out dynamic nivelRecebido))
                nivel = ((string)nivelRecebido);

            if (string.IsNullOrEmpty(nivel))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.EscalationList.SemNivel;

            num = nivel.ToInt();

            return (Dominio.ObjetosDeValor.Embarcador.Enumeradores.EscalationList)num;
        }

        private List<Dominio.Entidades.Cliente> ObterClientes()
        {
            string stringClientes = string.Empty;
            List<Dominio.Entidades.Cliente> clientes = new List<Dominio.Entidades.Cliente>();

            if (_dados.TryGetValue("Clientes", out var clientesRecebidos))
                stringClientes = ((string)clientesRecebidos);

            string[] listaClientes = stringClientes.Split(',');

            foreach (string str in listaClientes)
            {
                if (double.TryParse(str.Trim(), out double clientId))
                {
                    Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                    Dominio.Entidades.Cliente cliente = repositorioCliente.BuscarPorCPFCNPJ(clientId);

                    if (cliente != null)
                        clientes.Add(cliente);

                }
            }

            return clientes;
        }

        private string ObterLogin()
        {
            string usuario = string.Empty;

            if (_dados.TryGetValue("Usuario", out var usuarioRecebido))
                usuario = ((string)usuarioRecebido);

            if (string.IsNullOrEmpty(usuario))
                return null;

            return usuario;
        }

        private void SetPolitica(Dominio.Entidades.Usuario usuario)
        {
            if (_politicaSenha == null)
                return;

            usuario.Senha = _politicaSenha.CriarNovaSenha();
            usuario.AlterarSenhaAcesso = _politicaSenha.ExigirTrocaSenhaPrimeiroAcesso;
        }

        #endregion
    }
}
