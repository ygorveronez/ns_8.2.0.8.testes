using System.Linq;

namespace Servicos.WebService.Empresa
{
    public class Funcionario : ServicoBase
    {        
        public Funcionario(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Usuario SalvarFuncionario(Dominio.ObjetosDeValor.Embarcador.Carga.Funcionario funcionarioIntegracao, Dominio.Entidades.Empresa empresaIntegradora, Dominio.Enumeradores.TipoAcesso tipoAcesso, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = null, string adminStringConexao = "")
        {
            bool informouCnpj = false;
            string cpfCnpjMotorista = "";

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);

            if ((string.IsNullOrWhiteSpace(funcionarioIntegracao.CPF) && string.IsNullOrWhiteSpace(funcionarioIntegracao.CNPJ)) ||
                string.IsNullOrWhiteSpace(funcionarioIntegracao.Nome) || string.IsNullOrWhiteSpace(funcionarioIntegracao.Email))
            {
                mensagem = "Informe um CPF/CNPJ/Nome/E-mail válido para cadastrar um novo funcionário. ";
                return null;
            }

            if (!string.IsNullOrEmpty(funcionarioIntegracao.CNPJ))
                informouCnpj = true;

            if (!informouCnpj)
            {
                cpfCnpjMotorista = long.Parse(Utilidades.String.OnlyNumbers(funcionarioIntegracao.CPF)).ToString("d11");
                if (!string.IsNullOrWhiteSpace(funcionarioIntegracao.CPF) && !Utilidades.Validate.ValidarCPF(cpfCnpjMotorista))
                {
                    mensagem = "Informe um CPF/Email válido para cadastrar um novo funcionário. ";
                    return null;
                }
            }

            if (informouCnpj)
            {
                cpfCnpjMotorista = long.Parse(Utilidades.String.OnlyNumbers(funcionarioIntegracao.CNPJ)).ToString("d14");
                if (!Utilidades.Validate.ValidarCNPJ(cpfCnpjMotorista))
                {
                    mensagem = "Informe um CNPJ válido para cadastrar um novo funcionário. ";
                    return null;
                }
            }

            Dominio.Entidades.Empresa empresa = null;
            if (empresaIntegradora != null && empresa == null)
                empresa = repEmpresa.BuscarPorCodigo(empresaIntegradora.Codigo);

            Dominio.Entidades.Usuario funcionario;
            if (empresa != null)
                funcionario = repUsuario.BuscarMotoristaPorCPF(empresa.Codigo, Utilidades.String.OnlyNumbers(cpfCnpjMotorista));
            else
                funcionario = repUsuario.BuscarMotoristaPorCPF(Utilidades.String.OnlyNumbers(cpfCnpjMotorista));

            bool inserir = false;
            if (funcionario == null)
            {
                funcionario = new Dominio.Entidades.Usuario();
                inserir = true;
            }
            else
                funcionario.Initialize();

            if (informouCnpj)
                funcionario.TipoPessoa = "J";

            funcionario.CPF = cpfCnpjMotorista;
            funcionario.Nome = funcionarioIntegracao.Nome;
            funcionario.Email = funcionarioIntegracao.Email;
            funcionario.Telefone = funcionarioIntegracao.Telefone;
            funcionario.EnderecoDigitado = true;
            funcionario.TipoAcesso = tipoAcesso;

            if (!string.IsNullOrWhiteSpace(funcionarioIntegracao.CodigoIntegracao) && string.IsNullOrWhiteSpace(funcionario.CodigoIntegracao))
                funcionario.CodigoIntegracao = funcionarioIntegracao.CodigoIntegracao;

            if (!string.IsNullOrWhiteSpace(funcionarioIntegracao.RG) && funcionarioIntegracao.RG.Length > 20)
                mensagem += "O RG do funcionario não deve conter mais que 20 caracteres. ";

            if (!string.IsNullOrWhiteSpace(funcionarioIntegracao.Nome) && funcionarioIntegracao.Nome.Length > 80)
                mensagem += "O nome do funcionario não deve conter mais que 80 caracteres. ";

            funcionario.RG = funcionarioIntegracao.RG;
            funcionario.Tipo = "U";
            funcionario.Status = "A";

            if (funcionarioIntegracao.TipoComercial != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComercial.NaoComercial)
                funcionario.TipoComercial = funcionarioIntegracao.TipoComercial;

            if (funcionario.Setor == null)
                funcionario.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };

            if (empresa != null)
            {
                if (empresa.Matriz != null && empresa.Matriz.Count > 0)
                {
                    funcionario.Empresa = empresa.Matriz.FirstOrDefault();

                    if (funcionario.Localidade == null)
                        funcionario.Localidade = empresa.Matriz.FirstOrDefault().Localidade;
                }
                else
                {
                    funcionario.Empresa = empresa;
                    if (funcionario.Localidade == null)
                        funcionario.Localidade = empresa.Localidade;
                }
            }

            if (funcionarioIntegracao.PerfilAcesso != null && !string.IsNullOrWhiteSpace(funcionarioIntegracao.PerfilAcesso.CodigoIntegracao))
                funcionario.PerfilAcesso = repPerfilAcesso.BuscarPorCodigoIntegracao(funcionarioIntegracao.PerfilAcesso.CodigoIntegracao);

            if (!string.IsNullOrWhiteSpace(mensagem))
                return null;

            if (inserir)
                repUsuario.Inserir(funcionario, auditado);
            else
                repUsuario.Atualizar(funcionario, auditado);

            return funcionario;
        }

        public Dominio.Entidades.Usuario ConverterObjetoFuncionario(Dominio.ObjetosDeValor.Embarcador.Carga.Funcionario funcionarioIntegracao, Dominio.Entidades.Empresa empresa, out string mensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            mensagem = "";
            Dominio.Entidades.Usuario usuario = null;

            if (!string.IsNullOrWhiteSpace(funcionarioIntegracao.CodigoIntegracao))
                usuario = repUsuario.BuscarPorCodigoIntegracao(funcionarioIntegracao.CodigoIntegracao);

            if (usuario != null)
                return usuario;

            if (string.IsNullOrWhiteSpace(funcionarioIntegracao.CPF) && string.IsNullOrWhiteSpace(funcionarioIntegracao.CNPJ))
                return null;

            string cpfSomenteNumeros = Utilidades.String.OnlyNumbers(funcionarioIntegracao.CNPJ ?? funcionarioIntegracao.CPF);
            bool cpfCnpjValido = Utilidades.Validate.ValidarCPFCNPJ(funcionarioIntegracao.CNPJ ?? funcionarioIntegracao.CPF);

            if (cpfCnpjValido)
            {
                string cpfFuncionario = cpfSomenteNumeros.Length == 11 ? long.Parse(cpfSomenteNumeros).ToString("D11") : long.Parse(cpfSomenteNumeros).ToString("D14");
                usuario = repUsuario.BuscarPorCPF(cpfFuncionario, funcionarioIntegracao.TipoComercial);

                if (usuario == null && !string.IsNullOrWhiteSpace(funcionarioIntegracao.Login))
                    usuario = repUsuario.BuscarPorLogin(funcionarioIntegracao.Login);

                if (usuario == null)
                    usuario = SalvarFuncionario(funcionarioIntegracao, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : empresa, Dominio.Enumeradores.TipoAcesso.Embarcador, ref mensagem, unitOfWork, tipoServicoMultisoftware, auditado);

                return usuario;
            }

            return null;
        }

    }
}
