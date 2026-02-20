using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.Configuracoes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.Mobile
{
    public class Autenticacao : WebServiceBase, IAutenticacao
    {
        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Modulo> ObterPermissoes(Dominio.Entidades.Usuario funcionario, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            AdminMultisoftware.Repositorio.Modulos.Modulo repositorioModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(unitOfWorkAdmin);

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Modulo> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Modulo>();

            try
            {
                foreach (var item in funcionario.ModulosLiberadosMobile)
                {
                    AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo = repositorioModulo.BuscarPorCodigoModulo(item);
                    if (modulo != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Modulo permissaoModulo = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Modulo()
                        {
                            CodigoModulo = item,
                            Sequencia = modulo.Sequencia,
                            Descricao = modulo.Descricao
                        };

                        permissoes.Add(permissaoModulo);
                    }
                }

                if (permissoes.Count == 0)
                    return null;
                else
                    return permissoes;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return null;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        private Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario> ObterRetornoUsuario(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario usuario = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario()
            {
                Codigo = usuarioMobile.Codigo,
                Nome = usuarioMobile.Nome,
                Empresas = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>(),
                PermissaoAdministrador = false,
                Permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Modulo>(),
            };

            foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
            {
                bool utilizarClienteURL = Startup.appSettingsAD["AppSettings:UtilizarURLCliente"]?.ToString() == "SIM";

                if (!utilizarClienteURL || ClienteAcesso.Cliente.Codigo == usuarioMobileCliente.Cliente.Codigo)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    ConfigurationInstance.GetInstance(unitOfWork);

                    Servicos.Embarcador.Transportadores.MotoristaJornada servicoJornadaMotorista = new Servicos.Embarcador.Transportadores.MotoristaJornada(unitOfWork);

                    try
                    {
                        if (usuarioMobileCliente.Cliente == null)
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Empresa não encontrada.");

                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                        Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobile.CPF);
                        Dominio.Entidades.Veiculo veiculo = motorista != null ? ObterVeiculoPorMotorista(unitOfWork, motorista.Codigo) : null;

                        Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa empresaMobile = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa()
                        {
                            Codigo = usuarioMobileCliente.Cliente.Codigo,
                            Descricao = usuarioMobileCliente.Cliente.RazaoSocial,
                            JornadaMotoristaIniciada = servicoJornadaMotorista.IsJornadaMotoristaIniciada(motorista),
                            Placa = veiculo?.Placa ?? string.Empty,
                            Reboques = veiculo != null ? (from reboque in veiculo.VeiculosVinculados select reboque.Placa).ToList() : new List<string>(),
                            Transportadora = motorista?.Empresa != null ? motorista.Empresa.RazaoSocial : usuarioMobileCliente.Cliente.RazaoSocial,
                            UrlEmbarcador = ObterURLCliente(usuarioMobileCliente),
                            UrlMobile = ObterURLClienteMobile(usuarioMobileCliente),
                            VeiculoTipoManobra = veiculo?.TipoManobra ?? false,

                            //Configurações
                            ExigirDataEntregaNotaClienteCanhotos = configuracaoTMS.ExigirDataEntregaNotaClienteCanhotos,
                            LinkVideoMobile = configuracaoTMS.LinkVideoMobile
                        };

                        Dominio.Entidades.Usuario funcionario = ObterUsuarioMotoristaMobile(unitOfWork, usuarioMobile.CPF);

                        usuario.Empresas.Add(empresaMobile);
                        usuario.UtilizaControleEntrega = configuracaoTMS.AppUtilizaControleColetaEntrega;
                        usuario.TipoAcessoMobile = ObterTipoAcessoMobile(funcionario);
                        usuario.PermissaoAdministrador = funcionario != null ? (funcionario.UsuarioAdministradorMobile == null || funcionario.UsuarioAdministradorMobile.Value) : false;
                        usuario.Permissoes = funcionario == null || usuario.PermissaoAdministrador ? null : ObterPermissoes(funcionario, unitOfWorkAdmin);

                        if (funcionario != null && funcionario.Tipo == "M" && empresaMobile.Codigo == 18)
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Por Favor, Faça download da nova versão do aplicativo na Google Play.");
                    }
                    catch (ServicoException excecao)
                    {
                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos(excecao.Message);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                    }
                }
            }

            if (usuario.Empresas.Count == 0)
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Credenciais inválidas - Empresa.");

            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoSucesso(usuario);
        }

        private TipoAcessoMobile ObterTipoAcessoMobile(Dominio.Entidades.Usuario motorista)
        {
            if (motorista == null)
                return TipoAcessoMobile.Transportador;

            if (motorista.Tipo == "M")
                return TipoAcessoMobile.Motorista;

            if (motorista.Cliente != null)
                return TipoAcessoMobile.Cliente;

            if (motorista.CPF == "11111111111")
                return TipoAcessoMobile.Transportador;

            return TipoAcessoMobile.Funcionario;
        }

        private string GerarSessao()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        #endregion

        #region Métodos Públicos

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario> AutenticarUsuario(string cpf, string senha, string token)
        {
            ValidarToken(token);

            var unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {

                var credencial = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Credencial()
                {
                    CPF = cpf,
                    Senha = senha
                };

                var repositorioUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWork);
                var usuarioMobile = repositorioUsuarioMobile.BuscarPorCFPESenha(Utilidades.String.OnlyNumbers(credencial.CPF), credencial.Senha);

                if (usuarioMobile == null)
                {
                    usuarioMobile = repositorioUsuarioMobile.BuscarPorCFP(cpf);

                    if (usuarioMobile != null)
                    {
                        if (!string.IsNullOrWhiteSpace(usuarioMobile.Senha))
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Seu usuário está vinculado a outro aparelho, por gentileza solicite ao embarcador que faça uma nova liberação para seu CPF.");

                        if (usuarioMobile.Clientes.Any(obj => obj.Cliente.BloquearLoginVersaoAntigaAPP))
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Por favor, baixe a nova versão do Multimobile.");

                        usuarioMobile.Senha = senha;

                        repositorioUsuarioMobile.Atualizar(usuarioMobile);
                    }
                }

                if (usuarioMobile == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Credenciais inválidas.");

                return ObterRetornoUsuario(usuarioMobile, unitOfWork);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoExcecao("Ocorreu uma falha ao autenticar");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso>> ObterURL(string cpf, string senha, string token)
        {
            ValidarToken(token);

            var unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                var credencial = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Credencial()
                {
                    CPF = cpf,
                    Senha = senha
                };

                var repositorioUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWork);
                var usuarioMobile = repositorioUsuarioMobile.BuscarPorCFPESenha(Utilidades.String.OnlyNumbers(credencial.CPF), credencial.Senha);

                if (usuarioMobile == null)
                {
                    usuarioMobile = repositorioUsuarioMobile.BuscarPorCFP(cpf);

                    if (usuarioMobile != null)
                    {
                        if (!string.IsNullOrWhiteSpace(usuarioMobile.Senha))
                            return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso>>.CriarRetornoDadosInvalidos("Seu usuário está vinculado a outro aparelho, por gentileza solicite ao embarcador que faça uma nova liberação para seu CPF.");
                    }
                }

                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso>>.CriarRetornoDadosInvalidos("Credenciais inválidas.");



                var ListaUrl = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso>();



                foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                {




                    Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso urlAcesso = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso
                    {
                        URL = ObterURLClienteMobile(usuarioMobileCliente),
                        CodigoEmpresa = usuarioMobileCliente.Cliente.Codigo,
                        Empresa = usuarioMobileCliente.Cliente.NomeFantasia
                    };


                    ListaUrl.Add(urlAcesso);
                };


                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso>>.CriarRetornoSucesso(ListaUrl);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso>>.CriarRetornoExcecao("Ocorreu uma falha ao obter url");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [ObsoleteAttribute("Este método está obsoleto. Utilizar apenas o método AutenticarUsuario.")]
        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario> AutenticarUsuarioPost(Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Credencial credencial, string token)
        {
            ValidarToken(token);

            var unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                var repositorioUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWork);
                var usuarioMobile = repositorioUsuarioMobile.BuscarPorCFPESenha(Utilidades.String.OnlyNumbers(credencial.CPF), credencial.Senha);

                if (usuarioMobile != null)
                {
                    if (usuarioMobile.Clientes.Any(obj => obj.Cliente.BloquearLoginVersaoAntigaAPP))
                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Por favor, baixe a nova versão do Multimobile.");

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoSucesso(
                            new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario()
                            {
                                Codigo = usuarioMobile.Codigo,
                                Nome = usuarioMobile.Nome
                            }
                        );
                }
                else
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Credenciais inválidas.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoExcecao("Ocorreu uma falha ao autenticar");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> DeslogarUsuario(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterUsuarioMotoristaMobile(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    if (motorista == null)
                        throw new WebServiceException("Usuário não encontrado");

                    string retorno = Servicos.Usuario.ConfigurarUsuarioMobile(ref motorista, "", ClienteAcesso, unitOfWorkAdmin);

                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWorkAdmin.Rollback();
                        throw new WebServiceException(retorno);
                    }

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao deslogar");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario> ObterSessao(Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Credencial credencial, string token)
        {
            ValidarToken(token);

            var unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repositorioUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWork);
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repositorioUsuarioMobile.BuscarPorCFPESenha(Utilidades.String.OnlyNumbers(credencial.CPF), credencial.Senha);

                if (usuarioMobile == null)
                {
                    usuarioMobile = repositorioUsuarioMobile.BuscarPorCFP(Utilidades.String.OnlyNumbers(credencial.CPF));

                    if (usuarioMobile != null)
                    {
                        if (usuarioMobile.Clientes.Any(obj => obj.Cliente.BloquearLoginVersaoAntigaAPP))
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Por favor, baixe a nova versão do Multimobile.");

                        if (!string.IsNullOrWhiteSpace(usuarioMobile.Senha) && !usuarioMobile.NaoBloquearAcessoSimultaneo)
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Seu usuário está vinculado a outro aparelho, por gentileza solicite ao embarcador que faça uma nova liberação para seu CPF.");

                        usuarioMobile.Senha = credencial.Senha;
                    }
                }

                if (usuarioMobile == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Credenciais inválidas.");

                List<AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente> usuariosMobileCliente = repUsuarioMobileCliente.BuscarPorUsuario(usuarioMobile.Codigo);

                bool requerContraSenha = repUsuarioMobileCliente.ClienteExigeContraSenha(usuarioMobile.Codigo);
                bool validarSenha = !string.IsNullOrWhiteSpace(credencial.ContraSenha);
                bool senhaValida = usuarioMobile.ContraSenha == credencial.ContraSenha;

                if (requerContraSenha && validarSenha && !senhaValida)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoDadosInvalidos("Senha inválida.");

                string sessao = !requerContraSenha || (validarSenha && senhaValida) ? GerarSessao() : "";

                Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario usuario = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario()
                {
                    Codigo = usuarioMobile.Codigo,
                    Nome = usuarioMobile.Nome,
                    Sessao = sessao,
                    RequerContraSenha = requerContraSenha,
                    ContraSenhaValida = validarSenha && senhaValida,
                    Empresas = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>(),
                    PermissaoAdministrador = true,
                    Permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Modulo>(),
                };

                var distinctUsuariosMobileCliente = usuariosMobileCliente.GroupBy(o => o.Cliente.Codigo).Select(grp => grp.FirstOrDefault()).ToList();

                foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente clienteUsuarioMobile in distinctUsuariosMobileCliente)
                {
                    Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa empresaMobile = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa()
                    {
                        Codigo = clienteUsuarioMobile.Cliente.Codigo,
                        Descricao = clienteUsuarioMobile.Cliente.RazaoSocial,
                        UrlMobile = ObterURLClienteMobile(clienteUsuarioMobile),
                        UrlEmbarcador = ObterURLCliente(clienteUsuarioMobile),
                    };
                    usuario.Empresas.Add(empresaMobile);
                }

                usuarioMobile.VersaoAPP = credencial.VersaoApp;
                usuarioMobile.Sessao = sessao;
                usuarioMobile.DataSessao = DateTime.Now;
                repositorioUsuarioMobile.Atualizar(usuarioMobile);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoSucesso(usuario);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario>.CriarRetornoExcecao("Ocorreu uma falha ao autenticar");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ValidarWebService(string token)
        {
            if (token == "5227baf9-539d-486a-b25a-e279266fc7d1")
                return Retorno<bool>.CriarRetornoSucesso(true);
            else
                return Retorno<bool>.CriarRetornoExcecao("Token inválido");

        }


        #endregion
    }
}
