using Dominio.Entidades.Embarcador.Usuarios;
using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.WebService.Usuarios
{
    public class Usuario : ServicoBase
    {
        public Usuario(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario ConverterObjetoUsuario(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (usuario == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario dynUsuario = new Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario()
            {
                Protocolo = usuario.Codigo,
                CodigoIntegracao = usuario.CodigoIntegracao,
                DataCriacao = usuario.DataCadastro.HasValue ? usuario.DataCadastro.Value.ToString("dd/MM/yyyy") : "",
                DataUltimaAlteracao = usuario.DataUltimaAlteracaoSenhaObrigatoria.HasValue ? usuario.DataUltimaAlteracaoSenhaObrigatoria.Value.ToString("dd/MM/yyyy") : "",
                Setor = usuario.Setor?.Descricao ?? "",
                DataAdmissao = usuario.DataAdmissao.HasValue ? usuario.DataAdmissao.Value.ToString("dd/MM/yyyy") : "",
                DataDemissao = usuario.DataDemissao.HasValue ? usuario.DataDemissao.Value.ToString("dd/MM/yyyy") : "",
                UsuarioAdministrador = usuario.UsuarioAdministrador,
                CPF_CNPJ = usuario.CPF,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IBGEMunicipio = usuario.Localidade?.CodigoIBGE ?? 0,
                Login = usuario.Login,
                Situacao = usuario.Status == "A" ? "Ativo" : "Inativo"
            };

            return dynUsuario;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario> RetornarUsuarios(List<Dominio.Entidades.Usuario> usuarios, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario> dynUsuarios = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>();
            Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(unitOfWork);
            Servicos.WebService.Carga.ModeloVeicularCarga serWSModeloVeicularCarga = new Carga.ModeloVeicularCarga(unitOfWork);

            foreach (Dominio.Entidades.Usuario usuario in usuarios)
            {
                Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario dynUsuario = new Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario()
                {
                    Protocolo = usuario.Codigo,
                    CodigoIntegracao = usuario.CodigoIntegracao,
                    DataCriacao = usuario.DataCadastro.HasValue ? usuario.DataCadastro.Value.ToString("dd/MM/yyyy") : "",
                    DataUltimaAlteracao = usuario.DataUltimaAlteracaoSenhaObrigatoria.HasValue ? usuario.DataUltimaAlteracaoSenhaObrigatoria.Value.ToString("dd/MM/yyyy") : "",
                    Setor = usuario.Setor?.Descricao ?? "",
                    DataAdmissao = usuario.DataAdmissao.HasValue ? usuario.DataAdmissao.Value.ToString("dd/MM/yyyy") : "",
                    DataDemissao = usuario.DataDemissao.HasValue ? usuario.DataDemissao.Value.ToString("dd/MM/yyyy") : "",
                    UsuarioAdministrador = usuario.UsuarioAdministrador,
                    CPF_CNPJ = usuario.CPF,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    IBGEMunicipio = usuario.Localidade?.CodigoIBGE ?? 0,
                    Login = usuario.Login,
                    Situacao = usuario.Status == "A" ? "Ativo" : "Inativo",
                    Perfil = RetornarPerfilUsuario(usuario, unitOfWork, unitOfWorkAdmin),
                    PerfilAcesso = usuario.PerfilAcesso != null ? ObterPerfisAcesso(new List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> { usuario.PerfilAcesso }).FirstOrDefault() : null
                };

                dynUsuarios.Add(dynUsuario);
            }

            return dynUsuarios;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil> RetornarPerfis(List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> perfisEmbarcador, List<Dominio.Entidades.PerfilPermissao> perfisMultiCTe, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil> retornoPerfil = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil>();

            if (perfisEmbarcador?.Count > 0)
            {
                AdminMultisoftware.Repositorio.Modulos.Formulario repFormularioAdmin = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWorkAdmin);
                Repositorio.Embarcador.Usuarios.PerfilFormulario repPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilEmbarcador in perfisEmbarcador)
                {
                    Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil dynPerfil = new Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil();

                    dynPerfil.Codigo = perfilEmbarcador.Codigo;
                    dynPerfil.Descricao = perfilEmbarcador.Descricao;
                    dynPerfil.Sistema = "MultiEmbarcador";
                    dynPerfil.Paginas = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>();

                    List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> listaPaginas = repPerfilFormulario.BuscarPorPerfil(perfilEmbarcador.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario pagina in listaPaginas)
                    {
                        AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario = repFormularioAdmin.BuscarPorCodigoFormulario(pagina.CodigoFormulario);
                        if (formulario != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina dynPagina = new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina()
                            {
                                Descricao = formulario.Descricao,
                                ApenasLeitura = pagina.SomenteLeitura
                            };
                            dynPerfil.Paginas.Add(dynPagina);
                        }
                    }

                    if (perfilEmbarcador.ModulosLiberados != null && perfilEmbarcador.ModulosLiberados.Count > 0)
                    {
                        foreach (int modulo in perfilEmbarcador.ModulosLiberados)
                        {
                            List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = repFormularioAdmin.BuscarFormularioPorModulo(modulo);
                            foreach (var formulario in formularios)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina dynPagina = new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina()
                                {
                                    Descricao = formulario.Descricao,
                                    ApenasLeitura = false
                                };
                                dynPerfil.Paginas.Add(dynPagina);
                            }
                        }
                    }

                    retornoPerfil.Add(dynPerfil);
                }
            }



            if (perfisMultiCTe != null && perfisMultiCTe.Count > 0)
            {
                Repositorio.PerfilPermissaoPagina repPerfilPermissaoPagina = new Repositorio.PerfilPermissaoPagina(unitOfWork);

                foreach (Dominio.Entidades.PerfilPermissao perfilMultiCTe in perfisMultiCTe)
                {
                    Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil dynPerfil = new Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil();

                    dynPerfil.Codigo = perfilMultiCTe.Codigo;
                    dynPerfil.Descricao = perfilMultiCTe.Descricao + " ( " + perfilMultiCTe.Empresa.Descricao + ")";
                    dynPerfil.Sistema = "MultiCTe";
                    dynPerfil.Paginas = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>();

                    List<Dominio.Entidades.PerfilPermissaoPagina> listaPaginas = repPerfilPermissaoPagina.BuscarPorCodigoPerfil(perfilMultiCTe.Codigo);
                    foreach (Dominio.Entidades.PerfilPermissaoPagina pagina in listaPaginas)
                    {
                        if (pagina.PermissaoDeAcesso == "A")
                        {
                            Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina dynPagina = new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina()
                            {
                                Descricao = pagina.Pagina?.Descricao,
                                ApenasLeitura = pagina.PermissaoDeAlteracao == "I" && pagina.PermissaoDeInclusao == "I"
                            };
                            dynPerfil.Paginas.Add(dynPagina);
                        }
                    }

                    retornoPerfil.Add(dynPerfil);
                }
            }


            return retornoPerfil;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso> ObterPerfisAcesso(List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> perfisAcesso)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso> listaPerfisAcesso = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso>();

            foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso in perfisAcesso)
            {
                listaPerfisAcesso.Add(new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso
                {
                    CodigoIntegracao = perfilAcesso.CodigoIntegracao,
                    Protocolo = perfilAcesso.Codigo,
                    Descricao = perfilAcesso.Descricao,
                    Situacao = perfilAcesso.DescricaoAtivo,
                    HorarioInicioAcesso = perfilAcesso.HoraInicialAcesso.HasValue ? perfilAcesso.HoraInicialAcesso.Value.ToString(@"hh\:mm") : "",
                    HorarioFinalAcesso = perfilAcesso.HoraFinalAcesso.HasValue ? perfilAcesso.HoraFinalAcesso.Value.ToString(@"hh\:mm") : ""
                });
            }

            return listaPerfisAcesso;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina> ObterFormularios(List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina> listaPerfisPagina = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>();

            foreach (AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario in formularios)
            {
                listaPerfisPagina.Add(new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina
                {
                    Descricao = formulario.Descricao,
                    Codigo = formulario.CodigoFormulario,
                    Modulo = new Dominio.ObjetosDeValor.Embarcador.Usuarios.Modulo
                    {
                        Codigo = formulario.Modulo?.Codigo ?? 0,
                        Descricao = formulario.Modulo?.Descricao ?? ""
                    }
                });
            }

            return listaPerfisPagina;
        }

        public Dominio.Entidades.Usuario SalvarUsuario(Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario usuarioWS, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteAdmin, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.PerfilPermissao repPerfilMultiCTe = new Repositorio.PerfilPermissao(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilEmbarcador = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
            Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
            Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior repFuncionarioSenhaAnterior = new Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa repConfiguracaoPessoa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa = repConfiguracaoPessoa.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();


            Dominio.Enumeradores.TipoAcesso tipoAcesso = Dominio.Enumeradores.TipoAcesso.Embarcador;
            if (usuarioWS.Perfil.Sistema == "MultiCTe")
                tipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCPF(usuarioWS.CPF_CNPJ);
            bool inserir = false;
            if (usuario == null)
            {
                usuario = new Dominio.Entidades.Usuario();
                usuario.CPF = Utilidades.String.OnlyNumbers(usuarioWS.CPF_CNPJ);
                usuario.Tipo = "U";
                usuario.DataCadastro = DateTime.Now;
                usuario.Senha = usuario.CPF;
                inserir = true;
            }
            else
                usuario.Initialize();

            if (!string.IsNullOrWhiteSpace(usuarioWS.Senha))
                usuario.Senha = usuarioWS.Senha;

            usuario.TipoAcesso = tipoAcesso;
            if (tipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador)
                usuario.Empresa = repEmpresa.BuscarEmpresaPai();

            if (!string.IsNullOrWhiteSpace(usuarioWS.Nome))
                usuario.Nome = usuarioWS.Nome;
            if (!string.IsNullOrWhiteSpace(usuarioWS.Email))
                usuario.Email = usuarioWS.Email;
            usuario.Login = usuarioWS.Login;
            if (!string.IsNullOrWhiteSpace(usuarioWS.Situacao))
                usuario.Status = usuarioWS.Situacao == "Ativo" ? "A" : "I";
            if (usuarioWS.Perfil != null && usuarioWS.Perfil.Codigo > 0)
            {
                if (tipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao)
                {
                    usuario.PerfilPermissao = repPerfilMultiCTe.BuscarPorCodigo(usuarioWS.Perfil.Codigo);
                    usuario.Empresa = usuario.PerfilPermissao.Empresa;
                }
                else
                    usuario.PerfilAcesso = repPerfilEmbarcador.BuscarPorCodigo(usuarioWS.Perfil.Codigo);
            }
            if (usuarioWS.IBGEMunicipio > 0)
                usuario.Localidade = repLocalidade.BuscarPorCodigoIBGE(usuarioWS.IBGEMunicipio);

            Dominio.Entidades.Usuario usuarioExiste = repUsuario.BuscarPorLogin(usuario.Login, tipoAcesso);

            PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

            string retornoPoliticaSenha = "";
            if (politicaSenha != null)
            {
                usuario.Senha = politicaSenha.CriarNovaSenha();

                if (!configuracaoPessoa.NaoExigirTrocaDeSenhaCasoCadastroPorIntegracao)
                {
                    if (!politicaSenha.ExigirTrocaSenhaPrimeiroAcesso)
                    {
                        retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);
                        usuario.DataUltimaAlteracaoSenhaObrigatoria = DateTime.Now;
                    }
                    else
                        usuario.AlterarSenhaAcesso = true;
                }
                else
                {
                    retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);
                    usuario.DataUltimaAlteracaoSenhaObrigatoria = DateTime.Now;
                }
            }

            if (string.IsNullOrWhiteSpace(retornoPoliticaSenha))
            {
                if (usuarioExiste == null || (usuarioExiste.Codigo == usuario.Codigo))
                {
                    if (inserir)
                        repUsuario.Inserir(usuario, auditado);
                    else
                        repUsuario.Atualizar(usuario, auditado);

                    Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior funcionarioSenhaAnterior = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior();
                    funcionarioSenhaAnterior.Senha = usuario.Senha;
                    funcionarioSenhaAnterior.Usuario = usuario;
                    funcionarioSenhaAnterior.SenhaCriptografada = usuario.SenhaCriptografada;
                    repFuncionarioSenhaAnterior.Inserir(funcionarioSenhaAnterior);
                }
                else
                {
                    throw new Exception("Nome de Usuário já informado");
                }
            }
            else
            {
                throw new Exception(retornoPoliticaSenha);
            }

            if (inserir)
            {
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = new Dominio.Entidades.Embarcador.Operacional.OperadorLogistica();
                operadorLogistica.Ativo = true;
                operadorLogistica.SupervisorLogistica = true;
                operadorLogistica.PermiteAdicionarComplementosDeFrete = false;
                operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = true;
                operadorLogistica.Usuario = usuario;
                repOperadorLogistica.Inserir(operadorLogistica);
            }

            EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario, clienteAdmin, unitOfWork, unitOfWorkAdmin);
            return usuario;
        }

        public Dominio.ObjetosDeValor.WebService.Usuario.DetalheUsuario converterObjetoDetalhesUsuario(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);

            Dominio.ObjetosDeValor.WebService.Usuario.DetalheUsuario userRetorno = new Dominio.ObjetosDeValor.WebService.Usuario.DetalheUsuario()
            {
                CodigoIntegracao = usuario.CodigoIntegracao,
                CPF_CNPJ = usuario.CPF_CNPJ_Formatado,
                Email = usuario.Email,
                Login = usuario.Login,
                Nome = usuario.Nome,
                Situacao = usuario.DescricaoStatus,
                ExibidoAprovacoesTransportador = usuario.ExibirUsuarioAprovacao,
                LiberarAuditoria = usuario.PermiteAuditar,
                NotificacaoEmail = usuario.EnviarNotificacaoPorEmail,
                NotificacaoExpedicao = usuario.NotificadoExpedicao,
                PermissaoAdministrador = usuario.UsuarioAdministrador,

                OperadorLogistica = operadorLogistica != null,
                PermitirInformarComplementoFrete = operadorLogistica != null ? operadorLogistica.PermiteAdicionarComplementosDeFrete : false,
                PermitirVisualizarValorFreteTransportadores = operadorLogistica != null ? operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga : false,
                SupervisorLogistica = operadorLogistica != null ? operadorLogistica.SupervisorLogistica : false,
                FilialUsuario = usuario.Filial != null ? new Dominio.ObjetosDeValor.Embarcador.Filial.Filial { CodigoIntegracao = usuario.Filial.CodigoFilialEmbarcador, Descricao = usuario.Filial.Descricao, Ativo = usuario.Filial.Ativo, CNPJ = usuario.Filial.CNPJ } : null,
                FiliaisConfiguradas = RetornarFiliaisConfiguradasOperador(usuario, operadorLogistica, unitOfWork),
                PerfilAcesso = RetornarDetalhePerfil(usuario, unitOfWork)
            };

            return userRetorno;

        }

        public Dominio.Entidades.Usuario SalvarEAtualizarUsuarioIntegracao(Dominio.ObjetosDeValor.WebService.Usuario.UsuarioIntegracao usuarioWS, Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteAdmin, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.PerfilPermissao repPerfilMultiCTe = new Repositorio.PerfilPermissao(unitOfWork);
            Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
            Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior repFuncionarioSenhaAnterior = new Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Operacional.OperadorFilial repOperadorFilial = new Repositorio.Embarcador.Operacional.OperadorFilial(unitOfWork);
            Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();

            Dominio.Entidades.Usuario usuario = null;

            if (!string.IsNullOrEmpty(usuarioWS.CodigoIntegracao))
                usuario = repUsuario.BuscarPorCodigoIntegracao(usuarioWS.CodigoIntegracao);

            if (usuario == null)
                usuario = repUsuario.BuscarPorCPF(Utilidades.String.OnlyNumbers(usuarioWS.CPF_CNPJ));

            bool inserir = false;
            if (usuario == null)
            {
                usuario = new Dominio.Entidades.Usuario();
                usuario.CPF = Utilidades.String.OnlyNumbers(usuarioWS.CPF_CNPJ);
                usuario.Tipo = "U";
                usuario.DataCadastro = DateTime.Now;
                usuario.Senha = usuario.CPF;
                inserir = true;
            }
            else
                usuario.Initialize();

            usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Embarcador;

            if (usuarioWS.Sistema == "MultiCTe")
                usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;

            if (!string.IsNullOrWhiteSpace(usuarioWS.Senha))
                usuario.Senha = usuarioWS.Senha;

            if (usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador)
                usuario.Empresa = repEmpresa.BuscarEmpresaPai();

            if (!string.IsNullOrWhiteSpace(usuarioWS.Nome))
                usuario.Nome = usuarioWS.Nome;
            if (!string.IsNullOrWhiteSpace(usuarioWS.Email))
                usuario.Email = usuarioWS.Email;
            usuario.Login = usuarioWS.Login;
            if (!string.IsNullOrWhiteSpace(usuarioWS.Situacao))
                usuario.Status = usuarioWS.Situacao;

            if (usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao)
            {
                usuario.PerfilPermissao = repPerfilMultiCTe.BuscarPorCodigo(usuarioWS.CodigoIntegracaoPerfilAcesso.ToInt());
                if (usuario.PerfilPermissao != null)
                    usuario.Empresa = usuario.PerfilPermissao.Empresa;
            }

            if (usuarioWS.IBGEMunicipio > 0)
                usuario.Localidade = repLocalidade.BuscarPorCodigoIBGE(usuarioWS.IBGEMunicipio);
            else
                usuario.Localidade = usuario.Empresa?.Localidade;

            usuario.UsuarioAdministrador = usuarioWS.PermissaoAdministrador;
            usuario.ExibirUsuarioAprovacao = usuarioWS.ExibidoAprovacoesTransportador;
            usuario.PermiteAuditar = usuarioWS.LiberarAuditoria;
            usuario.EnviarNotificacaoPorEmail = usuarioWS.NotificacaoEmail;
            usuario.NotificadoExpedicao = usuarioWS.NotificacaoExpedicao;
            usuario.CodigoIntegracao = usuarioWS.CodigoIntegracao;

            PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            if (politicaSenha == null)
                politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

            string retornoPoliticaSenha = "";
            if (politicaSenha != null)
            {
                usuario.Senha = politicaSenha.CriarNovaSenha();

                if (!politicaSenha.ExigirTrocaSenhaPrimeiroAcesso)
                {
                    retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);
                    usuario.DataUltimaAlteracaoSenhaObrigatoria = DateTime.Now;
                }
                else
                    usuario.AlterarSenhaAcesso = true;
            }

            if (string.IsNullOrWhiteSpace(retornoPoliticaSenha))
            {

                if (inserir)
                    repUsuario.Inserir(usuario, auditado);
                else
                    repUsuario.Atualizar(usuario, auditado);

                if (perfilAcesso != null)
                {
                    usuario.PerfilAcesso = perfilAcesso;
                    PreencherFormulariosUsuario(usuario, perfilAcesso, unitOfWork);
                }

                Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior funcionarioSenhaAnterior = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior();
                funcionarioSenhaAnterior.Senha = usuario.Senha;
                funcionarioSenhaAnterior.Usuario = usuario;
                funcionarioSenhaAnterior.SenhaCriptografada = usuario.SenhaCriptografada;
                repFuncionarioSenhaAnterior.Inserir(funcionarioSenhaAnterior);
            }
            else
            {
                throw new Exception(retornoPoliticaSenha);
            }

            if (usuarioWS.OperadorLogistica)
            {
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                if (inserir)
                {
                    Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = new Dominio.Entidades.Embarcador.Operacional.OperadorLogistica();
                    operadorLogistica.Ativo = true;
                    operadorLogistica.SupervisorLogistica = usuarioWS.SupervisorLogistica;
                    operadorLogistica.PermiteAdicionarComplementosDeFrete = usuarioWS.PermitirInformarComplementoFrete;
                    operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = usuarioWS.PermitirVisualizarValorFreteTransportadores;
                    operadorLogistica.Usuario = usuario;
                    repOperadorLogistica.Inserir(operadorLogistica);

                    if (usuarioWS.CodigosIntegracaoFilialOperadorLogistica != null && usuarioWS.CodigosIntegracaoFilialOperadorLogistica.Count > 0)
                    {
                        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                        foreach (string codigoFilial in usuarioWS.CodigosIntegracaoFilialOperadorLogistica)
                        {
                            if (!string.IsNullOrEmpty(codigoFilial))
                            {
                                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(codigoFilial);
                                if (filial != null)
                                {
                                    Dominio.Entidades.Embarcador.Operacional.OperadorFilial operadorFilial = new Dominio.Entidades.Embarcador.Operacional.OperadorFilial();
                                    operadorFilial.Filial = repFilial.buscarPorCodigoEmbarcador(codigoFilial);
                                    operadorFilial.OperadorLogistica = operadorLogistica;
                                    repOperadorFilial.Inserir(operadorFilial);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);
                    if (operadorLogistica != null)
                    {
                        operadorLogistica.Ativo = true;
                        operadorLogistica.SupervisorLogistica = usuarioWS.SupervisorLogistica;
                        operadorLogistica.PermiteAdicionarComplementosDeFrete = usuarioWS.PermitirInformarComplementoFrete;
                        operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = usuarioWS.PermitirVisualizarValorFreteTransportadores;
                        operadorLogistica.Usuario = usuario;
                        repOperadorLogistica.Atualizar(operadorLogistica);

                        if (usuarioWS.CodigosIntegracaoFilialOperadorLogistica != null && usuarioWS.CodigosIntegracaoFilialOperadorLogistica.Count > 0)
                        {
                            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                            List<Dominio.Entidades.Embarcador.Operacional.OperadorFilial> operadorFiliais = repOperadorFilial.BuscarPorOperador(operadorLogistica.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Operacional.OperadorFilial operadorFilial in operadorFiliais)
                                repOperadorFilial.Deletar(operadorFilial);

                            foreach (string codigoFilial in usuarioWS.CodigosIntegracaoFilialOperadorLogistica)
                            {
                                if (!string.IsNullOrEmpty(codigoFilial))
                                {
                                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(codigoFilial);
                                    if (filial != null)
                                    {
                                        Dominio.Entidades.Embarcador.Operacional.OperadorFilial operadorFilial = new Dominio.Entidades.Embarcador.Operacional.OperadorFilial();
                                        operadorFilial.Filial = repFilial.buscarPorCodigoEmbarcador(codigoFilial);
                                        operadorFilial.OperadorLogistica = operadorLogistica;
                                        repOperadorFilial.Inserir(operadorFilial);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario, clienteAdmin, unitOfWork, unitOfWorkAdmin);

            return usuario;
        }

        public List<Dominio.ObjetosDeValor.WebService.Transportadores.MotoristaLicenca> ObterLicencasMotorista(int codigoUsuarioMobile, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.MotoristaLicenca repositorioMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> motoristaLicencas = repositorioMotoristaLicenca.BuscarLicencasMotoristaPorUsuarioMobile(codigoUsuarioMobile);

            if (motoristaLicencas == null || motoristaLicencas?.Count == 0)
                throw new ServicoException("Não foi encontrado licenças para este usuário.");

            List<Dominio.ObjetosDeValor.WebService.Transportadores.MotoristaLicenca> retornoMotoristaLicencas = new List<Dominio.ObjetosDeValor.WebService.Transportadores.MotoristaLicenca>();

            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca motoristaLicenca in motoristaLicencas)
            {
                Dominio.ObjetosDeValor.WebService.Transportadores.MotoristaLicenca retornoMotoristaLicenca = new Dominio.ObjetosDeValor.WebService.Transportadores.MotoristaLicenca()
                {
                    Protocolo = motoristaLicenca.Codigo,
                    Descricao = motoristaLicenca.Descricao,
                    Numero = motoristaLicenca.Numero,
                    DataCriacao = motoristaLicenca.DataEmissao.HasValue ? motoristaLicenca.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataVencimento = motoristaLicenca.DataVencimento.HasValue ? motoristaLicenca.DataVencimento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicencaHelper.ObterDescricao(motoristaLicenca.Status),
                    TipoLicenca = motoristaLicenca.Licenca != null ? new Dominio.ObjetosDeValor.WebService.Configuracoes.Licenca()
                    {
                        Codigo = motoristaLicenca.Licenca.Codigo.ToString(),
                        Descricao = motoristaLicenca.Licenca.Descricao
                    } : null,
                    ConfirmadaLeituraPendencia = motoristaLicenca.ConfirmadaLeituraPendencia
                };

                retornoMotoristaLicencas.Add(retornoMotoristaLicenca);
            }

            return retornoMotoristaLicencas;
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial> RetornarFiliaisConfiguradasOperador(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial> FiliaisOperadorRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>();

            if (operadorLogistica != null)
            {
                foreach (Dominio.Entidades.Embarcador.Operacional.OperadorFilial filialOperador in operadorLogistica.Filiais.ToList())
                {
                    Dominio.ObjetosDeValor.Embarcador.Filial.Filial filialretorno = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial
                    {
                        CodigoIntegracao = filialOperador.Filial.CodigoFilialEmbarcador,
                        Descricao = filialOperador.Filial.Descricao,
                        Ativo = filialOperador.Filial.Ativo,
                        CNPJ = filialOperador.Filial.CNPJ,
                        Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco(),
                        TipoFilial = filialOperador.Filial.TipoFilial,
                        CodigoAtividade = filialOperador.Filial.Atividade?.Codigo ?? 0
                    };

                    FiliaisOperadorRetorno.Add(filialretorno);
                }
            }

            return FiliaisOperadorRetorno;
        }

        private void EnviarEmailDeNotificacaoDeUsuarioCadastrado(Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteAdmin, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);
            bool naoEnviarSenha = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LoginAD.Value;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador)
            {
                try
                {
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorClienteETipoProducao(clienteAdmin?.Codigo ?? 0, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                    if (clienteURLAcesso != null)
                    {
                        sb.Append("<p>Seus dados para acesso ao sistema MultiEmbarcador são:<br /><br />");
                        sb.Append("Usuário: ").Append(usuario.Login).Append("<br />");

                        if (!naoEnviarSenha)
                        {
                            sb.Append("Senha: ").Append(usuario.Senha).Append("</p><br />");
                        }

                        sb.Append("Para utilizar o sistema acesse ").Append("http://" + clienteURLAcesso?.URLAcesso).Append(".");

                        System.Text.StringBuilder ss = new System.Text.StringBuilder();
                        ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");
                        ss.Append("Embarcador - " + clienteAdmin?.RazaoSocial + "<br />");
                        ss.Append("E-mail automático, favor não responder <br />");

                        svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, usuario.Email.Split(',')[0], "", "", "MultiEmbarcador - Dados para Acesso ao Sistema", sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unitOfWork);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
            else if (usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao)
            {
                try
                {
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorClienteETipoProducao(clienteAdmin?.Codigo ?? 0, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                    if (clienteURLAcesso != null)
                    {
                        sb.Append("<p>Seus dados para acesso ao sistema MultiCTe são:<br /><br />");
                        sb.Append("Usuário: ").Append(usuario.Login).Append("<br />");

                        if (!naoEnviarSenha)
                        {
                            sb.Append("Senha: ").Append(usuario.Senha).Append("</p><br />");
                        }

                        //sb.Append("Para utilizar o sistema MultiCTe acesse ").Append("http://" + clienteURLAcesso?.URLAcesso).Append(".");

                        System.Text.StringBuilder ss = new System.Text.StringBuilder();
                        ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");
                        ss.Append("Embarcador - " + clienteAdmin?.RazaoSocial + "<br />");
                        ss.Append("E-mail automático, favor não responder <br />");

                        svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, usuario.Email.Split(',')[0], "", "", "MultiCTe - Dados para Acesso ao Sistema", sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unitOfWork);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil RetornarPerfilUsuario(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil dynPerfil = new Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil();

            if (usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao && usuario.PerfilPermissao != null)
            {
                Repositorio.PerfilPermissaoPagina repPerfilPermissaoPagina = new Repositorio.PerfilPermissaoPagina(unitOfWork);

                dynPerfil.Codigo = usuario.PerfilPermissao.Codigo;
                dynPerfil.Descricao = usuario.PerfilPermissao.Descricao;
                dynPerfil.Sistema = "MultiCTe";
                dynPerfil.Paginas = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>();

                List<Dominio.Entidades.PerfilPermissaoPagina> listaPaginas = repPerfilPermissaoPagina.BuscarPorCodigoPerfil(usuario.PerfilPermissao.Codigo);
                foreach (Dominio.Entidades.PerfilPermissaoPagina pagina in listaPaginas)
                {
                    if (pagina.PermissaoDeAcesso == "A")
                    {
                        Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina dynPagina = new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina()
                        {
                            Descricao = pagina.Pagina?.Descricao,
                            ApenasLeitura = pagina.PermissaoDeAlteracao == "I" && pagina.PermissaoDeInclusao == "I"
                        };
                        dynPerfil.Paginas.Add(dynPagina);
                    }
                }
            }
            else if (usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador && usuario.PerfilAcesso != null)
            {
                AdminMultisoftware.Repositorio.Modulos.Formulario repFormularioAdmin = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWorkAdmin);
                Repositorio.Embarcador.Usuarios.PerfilFormulario repPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);

                dynPerfil.Codigo = usuario.PerfilAcesso.Codigo;
                dynPerfil.Descricao = usuario.PerfilAcesso.Descricao;
                dynPerfil.CodigoIntegracao = usuario.PerfilAcesso.CodigoIntegracao;

                dynPerfil.Sistema = "MultiEmbarcador";
                dynPerfil.Paginas = new List<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>();

                List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> listaPaginas = repPerfilFormulario.BuscarPorPerfil(usuario.PerfilAcesso.Codigo);
                foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario pagina in listaPaginas)
                {
                    AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario = repFormularioAdmin.BuscarPorCodigoFormulario(pagina.CodigoFormulario);
                    if (formulario != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina dynPagina = new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina()
                        {
                            Descricao = formulario.Descricao,
                            ApenasLeitura = pagina.SomenteLeitura
                        };
                        dynPerfil.Paginas.Add(dynPagina);
                    }
                }

                if (usuario.PerfilAcesso.ModulosLiberados != null && usuario.PerfilAcesso.ModulosLiberados.Count > 0)
                {
                    foreach (int modulo in usuario.PerfilAcesso.ModulosLiberados)
                    {
                        List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = repFormularioAdmin.BuscarFormularioPorModulo(modulo);
                        foreach (var formulario in formularios)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina dynPagina = new Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina()
                            {
                                Descricao = formulario.Descricao,
                                ApenasLeitura = false
                            };
                            dynPerfil.Paginas.Add(dynPagina);
                        }
                    }
                }
            }

            return dynPerfil;
        }

        private Dominio.ObjetosDeValor.WebService.Usuario.DetalhePerfilUsuario RetornarDetalhePerfil(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.WebService.Usuario.DetalhePerfilUsuario dynPerfil = new Dominio.ObjetosDeValor.WebService.Usuario.DetalhePerfilUsuario();

            if (usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao && usuario.PerfilPermissao != null)
            {
                dynPerfil.Descricao = usuario.PerfilPermissao.Descricao;
                dynPerfil.Sistema = "MultiCTe";
                dynPerfil.CodigoIntegracao = "";
            }
            else if (usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador && usuario.PerfilAcesso != null)
            {

                dynPerfil.Descricao = usuario.PerfilAcesso.Descricao;
                dynPerfil.CodigoIntegracao = usuario.PerfilAcesso.CodigoIntegracao;
                dynPerfil.Sistema = "MultiEmbarcador";

            }

            return dynPerfil;
        }

        private void PreencherFormulariosUsuario(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.PerfilFormulario repositorioPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);
            Repositorio.Embarcador.Usuarios.FuncionarioFormulario repositorioFuncionarioFormulario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unitOfWork);
            Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada repositorioFuncionarioFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada(unitOfWork);

            if (usuario.ModulosLiberados != null)
            {
                usuario.ModulosLiberados.Clear();
                foreach (int codigoModulo in perfilAcesso.ModulosLiberados)
                {
                    usuario.ModulosLiberados.Add(codigoModulo);
                }
            }

            List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario> formulariosCadastrados = repositorioFuncionarioFormulario.buscarPorUsuario(usuario.Codigo);
            foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario formularioCadastrado in formulariosCadastrados)
            {
                repositorioFuncionarioFormulario.Deletar(formularioCadastrado);
            }

            List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> perfilFormularios = repositorioPerfilFormulario.BuscarPorPerfil(perfilAcesso.Codigo);
            foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario perfilFormulario in perfilFormularios)
            {
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario();
                funcionarioFormulario.Usuario = usuario;
                funcionarioFormulario.SomenteLeitura = perfilFormulario.SomenteLeitura;
                funcionarioFormulario.CodigoFormulario = perfilFormulario.CodigoFormulario;
                repositorioFuncionarioFormulario.Inserir(funcionarioFormulario);

                foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada permissao in perfilFormulario.FormularioPermissaoPersonalizada)
                {
                    Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada funcionarioFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada();
                    funcionarioFormularioPermissaoPersonalizada.CodigoPermissao = permissao.CodigoPermissao;
                    funcionarioFormularioPermissaoPersonalizada.FuncionarioFormulario = funcionarioFormulario;
                    repositorioFuncionarioFormularioPermissaoPersonalizada.Inserir(funcionarioFormularioPermissaoPersonalizada);
                }
            }
        }

        #endregion
    }
}