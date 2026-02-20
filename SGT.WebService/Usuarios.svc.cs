using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Usuarios(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IUsuarios
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>> BuscarUsuarios(string sistema, string situacao, int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
            try
            {
                string empresaUsuarios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EmpresasUsuariosMultiCTe;
                List<int> codigosEmpresaUsuariosMultiCTe = null;
                if (!string.IsNullOrWhiteSpace(empresaUsuarios))
                    codigosEmpresaUsuariosMultiCTe = empresaUsuarios.Split(',').Select(Int32.Parse).ToList();

                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>();

                    Servicos.WebService.Usuarios.Usuario serWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                    List<Dominio.Entidades.Usuario> usuarios = repUsuario.ConsultarPorSistema(situacao, sistema, codigosEmpresaUsuariosMultiCTe, "Codigo", "asc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repUsuario.ContarPorSistema(situacao, sistema, codigosEmpresaUsuariosMultiCTe);
                    retorno.Objeto.Itens = serWSUsuario.RetornarUsuarios(usuarios, unitOfWork, unitOfWorkAdmin);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Usuarios", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar Usuarios";
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil>> BuscarPerfil(string sistema, string situacao)
        {
            ValidarToken();

            int inicio = 0;
            int limite = 10000;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);

            try
            {
                if (limite <= 10000)
                {
                    string empresaUsuarios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EmpresasUsuariosMultiCTe;
                    List<int> codigosEmpresaUsuarios = null;
                    if (!string.IsNullOrWhiteSpace(empresaUsuarios))
                        codigosEmpresaUsuarios = empresaUsuarios.Split(',').Select(Int32.Parse).ToList();

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil>();

                    Servicos.WebService.Usuarios.Usuario serWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

                    Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> perfisEmbarcador = sistema != "MultiEmbarcador" && sistema != "" && sistema != "Todos" ? new List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>() : repPerfilAcesso.Consultar("", situacao == "Ativos" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo : situacao == "Inativos" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos, "Codigo", "asc", inicio, limite);

                    Repositorio.PerfilPermissao perfilPermissao = new Repositorio.PerfilPermissao(unitOfWork);
                    List<Dominio.Entidades.PerfilPermissao> perfisMultiCTe = sistema != "MultiCTe" && sistema != "" && sistema != "Todos" ? new List<Dominio.Entidades.PerfilPermissao>() : perfilPermissao.ConsultaListaEmpresa(codigosEmpresaUsuarios, "", inicio, limite, situacao == "Ativos" ? true : situacao == "Inativos" ? false : true);

                    retorno.Objeto.NumeroTotalDeRegistro = perfisMultiCTe.Count() + perfisEmbarcador.Count();
                    retorno.Objeto.Itens = serWSUsuario.RetornarPerfis(perfisEmbarcador, perfisMultiCTe, unitOfWork, unitOfWorkAdmin);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Usuarios", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 1000";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar Usuarios";
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso>> ConsultarPerfisDeAcesso()
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Usuarios.PerfilAcesso repositorioPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);

                List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> listaPerfisDeAcesso = repositorioPerfilAcesso.BuscarTodos();

                Servicos.WebService.Usuarios.Usuario servicoWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso> objetosPerfilDeAcesso = servicoWSUsuario.ObterPerfisAcesso(listaPerfisDeAcesso);

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou todos perfis de acesso", unitOfWork);

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso> paginacaoPerfilAcesso = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso>
                {
                    Itens = objetosPerfilDeAcesso,
                    NumeroTotalDeRegistro = objetosPerfilDeAcesso.Count
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso>>.CriarRetornoSucesso(paginacaoPerfilAcesso);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os perfis de acesso.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>> ObterFormulariosPorPerfilAcesso(int protocoloPerfil)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Modulos.Formulario repositorioFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWorkAdmin);
                Repositorio.Embarcador.Usuarios.PerfilFormulario repositorioPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilAcesso repositorioPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);

                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = protocoloPerfil > 0 ? repositorioPerfilAcesso.BuscarPorCodigo(protocoloPerfil) : null;

                if (perfilAcesso == null)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>>.CriarRetornoDadosInvalidos("O perfil de acesso não foi encontrado.");

                if (perfilAcesso.PerfilAdministrador)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>>.CriarRetornoSucesso(new Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>());

                List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> perfisFormulario = repositorioPerfilFormulario.BuscarPorPerfil(protocoloPerfil);
                List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = perfisFormulario.Count > 0 ? repositorioFormulario.BuscarPorCodigosFormularios(perfisFormulario.Select(obj => obj.CodigoFormulario).Distinct().ToList()) : new List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario>();

                Servicos.WebService.Usuarios.Usuario servicoWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina> objetosPerfilPagina = servicoWSUsuario.ObterFormularios(formularios);

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou formulários por perfil de acesso", unitOfWork);

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina> paginacaoPerfilPagina = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>
                {
                    Itens = objetosPerfilPagina,
                    NumeroTotalDeRegistro = objetosPerfilPagina.Count
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>>.CriarRetornoSucesso(paginacaoPerfilPagina);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os formulários.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>> ObterUsuarios()
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);

            try
            {
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                Servicos.WebService.Usuarios.Usuario servicoWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

                List<Dominio.Entidades.Usuario> listaUsuarios = repositorioUsuario.BuscarTodosComFetchLocalidadePerfilAcesso();

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou todos os usuários", unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario> objetosUsuario = servicoWSUsuario.RetornarUsuarios(listaUsuarios, unitOfWork, unitOfWorkAdmin);

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario> paginacaoPerfilPagina = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>
                {
                    Itens = objetosUsuario,
                    NumeroTotalDeRegistro = objetosUsuario.Count
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>>.CriarRetornoSucesso(paginacaoPerfilPagina);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os usuários.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario>> ObterRelacaoUsuarioPerfilAcesso()
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);

            try
            {
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                Servicos.WebService.Usuarios.Usuario servicoWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

                List<Dominio.Entidades.Usuario> listaUsuarios = repositorioUsuario.BuscarTodosComFetchLocalidadePerfilAcesso();

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou todos os usuários para realizar a relação", unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario> objetosUsuario = servicoWSUsuario.RetornarUsuarios(listaUsuarios, unitOfWork, unitOfWorkAdmin);
                List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario> objetosPerfilAcessoUsuario = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario>();
                
                foreach (Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario objetoUsuario in objetosUsuario)
                    objetosPerfilAcessoUsuario.Add(new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario
                    {
                        Usuario = objetoUsuario,
                        PerfilAcesso = objetoUsuario.PerfilAcesso
                    });

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario> paginacaoPerfilAcessoUsuario = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario>
                {
                    Itens = objetosPerfilAcessoUsuario,
                    NumeroTotalDeRegistro = objetosPerfilAcessoUsuario.Count
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario>>.CriarRetornoSucesso(paginacaoPerfilAcessoUsuario);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }
        
        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial>> ObterRestricoesUsuarios()
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);

            try
            {
                Repositorio.Embarcador.Operacional.OperadorFilial repositorioOperadorFilial = new Repositorio.Embarcador.Operacional.OperadorFilial(unitOfWork);
                List<Dominio.Entidades.Embarcador.Operacional.OperadorFilial> listaOperadorFilial = repositorioOperadorFilial.BuscarTodos();
                List<Dominio.Entidades.Usuario> listaUsuarios = listaOperadorFilial.Select(obj => obj.OperadorLogistica.Usuario).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFiliais = listaOperadorFilial.Select(obj => obj.Filial).Distinct().ToList();

                Servicos.WebService.Usuarios.Usuario servicoWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);
                Servicos.WebService.Filial.Filial servicoWSFilial = new Servicos.WebService.Filial.Filial(unitOfWork);
                
                List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario> objetosUsuario = servicoWSUsuario.RetornarUsuarios(listaUsuarios, unitOfWork, unitOfWorkAdmin);
                List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial> objetosFilial = servicoWSFilial.RetornarFiliais(listaFiliais);
                List<Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial> objetosUsuarioFilial = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial>();

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou restrições", unitOfWork);

                foreach (Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario objetoUsuario in objetosUsuario)
                {
                    List<string> cnpjsFiliaisLiberadasDoUsuario = (from obj in listaOperadorFilial where obj.OperadorLogistica.Usuario.Codigo == objetoUsuario.Protocolo select obj.Filial.CNPJ_Formatado).Distinct().ToList();

                    Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial objetoUsuarioFilial = new Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial();
                    objetoUsuarioFilial.Usuario = objetoUsuario;
                    objetoUsuarioFilial.Filial = new List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>();
                    objetoUsuarioFilial.Filial.AddRange((from obj in objetosFilial where cnpjsFiliaisLiberadasDoUsuario.Contains(obj.CNPJ) select obj).ToList());
                    
                    objetosUsuarioFilial.Add(objetoUsuarioFilial);
                }

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial> paginacaoPerfilAcessoUsuario = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial>
                {
                    Itens = objetosUsuarioFilial,
                    NumeroTotalDeRegistro = objetosUsuarioFilial.Count
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial>>.CriarRetornoSucesso(paginacaoPerfilAcessoUsuario);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<string> CriarUsuario(Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario usuario)
        {
            Servicos.Log.TratarErro("CriarUsuario: " + Newtonsoft.Json.JsonConvert.SerializeObject(usuario));

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Conexao.createInstance(_serviceProvider).ObterHost);
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = clienteURLAcesso?.Cliente;

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.PerfilPermissao repPerfilMultiCTe = new Repositorio.PerfilPermissao(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilEmbarcador = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            if (usuario == null)
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("CriarUsuario: Usuario invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Usuario invalido.", Objeto = "", Status = false };
            }

            if (usuario.Perfil == null || usuario.Perfil.Codigo <= 0)
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("CriarUsuario: Perfil (" + usuario.Perfil.Codigo.ToString() + ") invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Perfil (" + usuario.Perfil.Codigo.ToString() + ") invalido.", Objeto = "", Status = false };
            }

            if (configuracao.ExigePerfilUsuario)
            {
                if (string.IsNullOrWhiteSpace(usuario.Perfil.Sistema) || (usuario.Perfil.Sistema != "MultiCTe" && usuario.Perfil.Sistema != "MultiEmbarcador"))
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("CriarUsuario: Sistema do perfil (" + usuario.Perfil.Sistema + ") invalido.", "Usuarios");
                    return new Retorno<string>() { Mensagem = "Sistema do perfil (" + usuario.Perfil.Sistema + ") invalido.", Objeto = "", Status = false };
                }

                if (usuario.Perfil.Sistema == "MultiCTe" && repPerfilMultiCTe.BuscarPorCodigo(usuario.Perfil.Codigo) == null)
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("CriarUsuario: Perfil (" + usuario.Perfil.Codigo.ToString() + ") não encontrado para o sistema MultiCTe.", "Usuarios");
                    return new Retorno<string>() { Mensagem = "Perfil (" + usuario.Perfil.Codigo.ToString() + ") não encontrado para o sistema MultiCTe.", Objeto = "", Status = false };
                }

                if (usuario.Perfil.Sistema == "MultiEmbarcador" && repPerfilEmbarcador.BuscarPorCodigo(usuario.Perfil.Codigo) == null)
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("CriarUsuario: Perfil (" + usuario.Perfil.Codigo.ToString() + ") não encontrado para o sistema MultiEmbarcador.", "Usuarios");
                    return new Retorno<string>() { Mensagem = "Perfil (" + usuario.Perfil.Codigo.ToString() + ") não encontrado para o sistema MultiEmbarcador.", Objeto = "", Status = false };
                }
            }

            if (usuario.IBGEMunicipio == 0 || repLocalidade.BuscarPorCodigoIBGE(usuario.IBGEMunicipio) == null)
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("CriarUsuario: IBGE (" + usuario.IBGEMunicipio.ToString() + ") invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "IBGE (" + usuario.IBGEMunicipio.ToString() + ") invalido.", Objeto = "", Status = false };
            }

            if (string.IsNullOrWhiteSpace(usuario.Login))
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("CriarUsuario: Login (" + usuario.Login + ") invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Login (" + usuario.Login + ") invalido.", Objeto = "", Status = false };
            }

            if (string.IsNullOrWhiteSpace(usuario.CPF_CNPJ) || !Utilidades.Validate.ValidarCPF(Utilidades.String.OnlyNumbers(usuario.CPF_CNPJ)))
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("CriarUsuario: CPF (" + usuario.CPF_CNPJ + ") invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "CPF (" + usuario.CPF_CNPJ + ") invalido.", Objeto = "", Status = false };
            }

            Dominio.Entidades.Usuario usuarioLogin = repUsuario.BuscarPorLoginETipo(usuario.Login, usuario.Perfil?.Sistema == "MultiCTe" ? Dominio.Enumeradores.TipoAcesso.Emissao : Dominio.Enumeradores.TipoAcesso.Embarcador);
            if (usuarioLogin != null && usuarioLogin.CPF != Utilidades.String.OnlyNumbers(usuario.CPF_CNPJ))
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("CriarUsuario: Login está vinculado a outro CPF.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Login está vinculado a outro CPF.", Objeto = "", Status = false };
            }

            Servicos.WebService.Usuarios.Usuario serWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Usuario usuarioSistema = serWSUsuario.SalvarUsuario(usuario, Auditado, cliente, unitOfWork, unitOfWorkAdmin);

                if (usuarioSistema != null)
                {
                    unitOfWork.CommitChanges();
                    return new Retorno<string>() { Mensagem = "Usuario criado com sucesso", Objeto = usuarioSistema.Codigo.ToString(), Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("CriarUsuario: Não foi possível salvar usuario");
                    return new Retorno<string>() { Mensagem = "Não foi possível salvar usuario", Objeto = "", Status = false };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("CriarUsuario: " + ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha generica ao realizar a integracao.", Status = false };
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> AlterarUsuario(Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario usuario)
        {
            Servicos.Log.TratarErro("AlterarUsuario: " + Newtonsoft.Json.JsonConvert.SerializeObject(usuario));

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Conexao.createInstance(_serviceProvider).ObterHost);
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = clienteURLAcesso?.Cliente;

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.PerfilPermissao repPerfilMultiCTe = new Repositorio.PerfilPermissao(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilEmbarcador = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            if (usuario == null)
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("AlterarUsuario: Usuario invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Usuario invalido.", Objeto = "", Status = false };
            }

            if (usuario.Perfil != null && usuario.Perfil.Codigo > 0)
            {
                if (string.IsNullOrWhiteSpace(usuario.Perfil.Sistema) || (usuario.Perfil.Sistema != "MultiCTe" && usuario.Perfil.Sistema != "MultiEmbarcador"))
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("AlterarUsuario: Sistema do perfil (" + usuario.Perfil.Sistema + ") invalido.", "Usuarios");
                    return new Retorno<string>() { Mensagem = "Sistema do perfil (" + usuario.Perfil.Sistema + ") invalido.", Objeto = "", Status = false };
                }

                if (usuario.Perfil.Sistema == "MultiCTe" && repPerfilMultiCTe.BuscarPorCodigo(usuario.Perfil.Codigo) == null)
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("AlterarUsuario: Perfil (" + usuario.Perfil.Codigo.ToString() + ") não encontrado para o sistema MultiCTe.", "Usuarios");
                    return new Retorno<string>() { Mensagem = "Perfil (" + usuario.Perfil.Codigo.ToString() + ") não encontrado para o sistema MultiCTe.", Objeto = "", Status = false };
                }

                if (usuario.Perfil.Sistema == "MultiEmbarcador" && repPerfilEmbarcador.BuscarPorCodigo(usuario.Perfil.Codigo) == null)
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("AlterarUsuario: Perfil (" + usuario.Perfil.Codigo.ToString() + ") não encontrado para o sistema MultiEmbarcador.", "Usuarios");
                    return new Retorno<string>() { Mensagem = "Perfil (" + usuario.Perfil.Codigo.ToString() + ") não encontrado para o sistema MultiEmbarcador.", Objeto = "", Status = false };
                }
            }

            if (string.IsNullOrWhiteSpace(usuario.Login))
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("AlterarUsuario: Login (" + usuario.Login + ") invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Login (" + usuario.Login + ") invalido.", Objeto = "", Status = false };
            }

            if (string.IsNullOrWhiteSpace(usuario.CPF_CNPJ) || !Utilidades.Validate.ValidarCPF(Utilidades.String.OnlyNumbers(usuario.CPF_CNPJ)))
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("AlterarUsuario: CPF (" + usuario.CPF_CNPJ + ") invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "CPF (" + usuario.CPF_CNPJ + ") invalido.", Objeto = "", Status = false };
            }

            Dominio.Entidades.Usuario usuarioLogin = repUsuario.BuscarPorLoginETipo(usuario.Login, usuario.Perfil?.Sistema == "MultiCTe" ? Dominio.Enumeradores.TipoAcesso.Emissao : Dominio.Enumeradores.TipoAcesso.Embarcador);
            if (usuarioLogin != null && usuarioLogin.CPF != Utilidades.String.OnlyNumbers(usuario.CPF_CNPJ))
            {
                unitOfWorkAdmin.Dispose();
                Servicos.Log.TratarErro("AlterarUsuario: Login está vinculado a outro CPF.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Login está vinculado a outro CPF.", Objeto = "", Status = false };
            }

            Servicos.WebService.Usuarios.Usuario serWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Usuario usuarioSistema = serWSUsuario.SalvarUsuario(usuario, Auditado, cliente, unitOfWork, unitOfWorkAdmin);

                if (usuarioSistema != null)
                {
                    unitOfWork.CommitChanges();

                    return new Retorno<string>() { Mensagem = "Usuario alterado com sucesso", Objeto = usuarioSistema.Codigo.ToString(), Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("AlterarUsuario: Não foi possível salvar usuário");
                    return new Retorno<string>() { Mensagem = "Não foi possível atualizar usuario", Objeto = "", Status = false };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("AlterarUsuario: " + ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha generica ao realizar a integracao.", Status = false };
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> RemoverPerfilUsuario(string login, string sistema)
        {
            Servicos.Log.TratarErro("RemoverPerfilUsuario: " + login + " " + sistema);

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            if (string.IsNullOrWhiteSpace(login))
            {
                Servicos.Log.TratarErro("RemoverPerfilUsuario: Login (" + login + ") invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Login (" + login + ") invalido.", Objeto = "", Status = false };
            }

            if (sistema != "MultiCTe" && sistema != "MultiEmbarcador")
            {
                Servicos.Log.TratarErro("RemoverPerfilUsuario: Sistema (" + sistema + ") invalido.", "Usuarios");
                return new Retorno<string>() { Mensagem = "Sistema (" + sistema + ") invalido.", Objeto = "", Status = false };
            }

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario usuarioLogin = repUsuario.BuscarPorLoginETipo(login, sistema == "MultiCTe" ? Dominio.Enumeradores.TipoAcesso.Emissao : Dominio.Enumeradores.TipoAcesso.Embarcador);

            if (usuarioLogin == null)
            {
                Servicos.Log.TratarErro("RemoverPerfilUsuario: Usuario com Login " + login + " não encontrado para " + sistema, "Usuarios");
                return new Retorno<string>() { Mensagem = "Usuario com Login " + login + " não encontrado para " + sistema, Objeto = "", Status = false };
            }

            if (sistema == "MultiCTe")
                usuarioLogin.PerfilPermissao = null;
            else
                usuarioLogin.PerfilAcesso = null;

            repUsuario.Atualizar(usuarioLogin);

            return new Retorno<string>() { Mensagem = "Perfil removido com sucesso", Objeto = usuarioLogin.Codigo.ToString(), Status = true };
        }

        #endregion

        #region Métodos Privados

        #endregion

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePessoas;
        }
    }
}
