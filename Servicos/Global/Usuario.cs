using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class Usuario
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Usuario(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public static Dominio.Entidades.Usuario PreencherMotoristaGenerico(string nome, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

            Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario()
            {
                CPF = "11111111111",
                Empresa = empresa,
                Localidade = empresa.Localidade,
                Nome = nome,
                Tipo = "M",
                CodigoIntegracao = "",
                CodigoIntegracaoContabilidade = "",
                Session = "",
                Setor = repSetor.BuscarPorCodigo(1),
                DataNascimento = null,
                DataValidadeLiberacaoSeguradora = null,
                DataAdmissao = null,
                RG = "",
                OrgaoEmissorRG = Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.CNH,
                EstadoRG = null,
                Sexo = null,
                Telefone = "",
                Salario = 0,
                Login = "",
                DataUltimaAlteracaoSenhaObrigatoria = null,
                Senha = "",
                SenhaCriptografada = false,
                UsuarioAcessoBloqueado = false,
                DataHoraBloqueio = null,
                TentativasInvalidas = 0,
                Endereco = "",
                Bairro = "",
                CEP = "",
                Complemento = "",
                TipoSanguineo = "",
                NumeroHabilitacao = "",
                DataHabilitacao = null,
                DataVencimentoHabilitacao = null,
                Categoria = "",
                Email = "",
                Moop = "",
                PIS = "",
                Status = "",
                UsuarioAdministrador = false,
                UsuarioMultisoftware = false,
                TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao,
                PercentualComissao = 0,
                DataRemocaoVinculo = null,
                PlanoAcertoViagem = null,
                NumeroCartao = "",
                NumeroEndereco = "",
                TipoLogradouro = null,
                Latitude = "",
                Longitude = "",
                EnderecoDigitado = false,
                TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Principal,
                TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Outros,
                AlterarSenhaAcesso = false,
                PermiteAuditar = false,
                PerfilPermissao = null,
                PerfilAcesso = null,
                ClienteTerceiro = null,
                ClienteFornecedor = null,
                TipoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio,
                CNPJEmbarcador = "",
                NaoGeraComissaoAcerto = true,
                AtivarFichaMotorista = false,
                Banco = null,
                Agencia = "",
                DigitoAgencia = "",
                NumeroConta = "",
                TipoContaBanco = null,
                NotificadoExpedicao = false,
                EnviarNotificacaoPorEmail = false,
                VisualizarGraficosIniciais = false,
                CodigoMobile = 0,
                DataCadastro = DateTime.Now,
                UltimoAcesso = null,
            };

            repUsuario.Inserir(usuario);

            return repUsuario.BuscarPorCodigo(usuario.Codigo);
        }

        public static string ConfigurarUsuarioMobile(ref Dominio.Entidades.Usuario funcionario, string novaSenhaMobile, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            try
            {
                if (!unitOfWorkAdmin.IsOpenSession())
                    unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(unitOfWorkAdmin.StringConexao);

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCFP(funcionario.CPF);
                if (usuarioMobile == null)
                {
                    usuarioMobile = new AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile
                    {
                        CPF = funcionario.CPF,
                        Celular = "",
                        Sessao = ""
                    };
                }

                usuarioMobile.Nome = funcionario.Nome;
                usuarioMobile.NaoBloquearAcessoSimultaneo = funcionario.NaoBloquearAcessoSimultaneo;
                usuarioMobile.DataSessao = DateTime.Now;
                usuarioMobile.Senha = "";
                usuarioMobile.Ativo = true;
                usuarioMobile.TentativasAcessoInvalido = 0;

                if (!string.IsNullOrWhiteSpace(novaSenhaMobile))
                    usuarioMobile.ContraSenha = novaSenhaMobile;

                if (usuarioMobile.Codigo == 0)
                    repUsuarioMobile.Inserir(usuarioMobile);
                else
                    repUsuarioMobile.Atualizar(usuarioMobile);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = null;
                if (usuarioMobile.Clientes != null)
                    usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == clienteAcesso.Cliente.Codigo select obj).FirstOrDefault();

                if (usuarioMobileCliente == null)
                {
                    usuarioMobileCliente = new AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente
                    {
                        Cliente = clienteAcesso.Cliente,
                        UsuarioMobile = usuarioMobile,
                        BaseHomologacao = clienteAcesso.URLHomologacao
                    };
                }
                else
                {
                    if (usuarioMobileCliente.BaseHomologacao == false && clienteAcesso.URLHomologacao)
                        return "Esse usuário já está apto a utilizar o aplicativo em produção, não sendo possível configurar o mesmo em homologação";

                    usuarioMobileCliente.BaseHomologacao = clienteAcesso.URLHomologacao;
                }

                if (usuarioMobileCliente.Codigo == 0)
                    repUsuarioMobileCliente.Inserir(usuarioMobileCliente);
                else
                    repUsuarioMobileCliente.Atualizar(usuarioMobileCliente);

                funcionario.CodigoMobile = usuarioMobile.Codigo;

                return string.Empty;
            }
            catch (Exception ex)
            {
                unitOfWorkAdmin.Rollback();
                throw;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public string RemoveUsuarioMobile(ref Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            try
            {
                if (!unitOfWorkAdmin.IsOpenSession())
                    unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(unitOfWorkAdmin.StringConexao);

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCFP(motorista.CPF);
                if (usuarioMobile == null)
                    return string.Empty;

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == clienteAcesso.Cliente.Codigo select obj).FirstOrDefault();
                if (usuarioMobileCliente == null)
                    return string.Empty;

                if (usuarioMobileCliente.BaseHomologacao == clienteAcesso.URLHomologacao)
                {
                    if (!usuarioMobile.Clientes.Any(obj => obj.Cliente.Codigo != clienteAcesso.Cliente.Codigo))
                    {
                        usuarioMobile.Senha = "";
                        repUsuarioMobile.Atualizar(usuarioMobile);
                    }
                    repUsuarioMobileCliente.Deletar(usuarioMobileCliente);
                    motorista.CodigoMobile = 0;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                unitOfWorkAdmin.Dispose();
                throw;
            }
        }

        public bool SalvarViculosMatrizFilial(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(motorista.Empresa.Codigo);

            if (empresa != null)
            {
                if (empresa.Filiais.Count > 0)
                {
                    foreach (Dominio.Entidades.Empresa empresaFilial in empresa.Filiais)
                        SalvarUsuario(motorista, empresaFilial, unitOfWork, tipoServicoMultisoftware);
                }

                Dominio.Entidades.Empresa empresaMatriz = repEmpresa.BuscarEmpresaMatriz(empresa);
                if (empresaMatriz != null)
                {
                    SalvarUsuario(motorista, empresaMatriz, unitOfWork, tipoServicoMultisoftware);

                    if (empresaMatriz.Filiais.Count > 0)
                    {
                        foreach (Dominio.Entidades.Empresa empresaFilial in empresaMatriz.Filiais)
                        {
                            if (empresaFilial.Codigo != motorista.Empresa.Codigo)
                                SalvarUsuario(motorista, empresaFilial, unitOfWork, tipoServicoMultisoftware);
                        }
                    }
                }

            }
            return true;
        }

        public static bool ObrigatorioTermos(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            // So é obrigatorio termos quando:
            // Empresa ainda não aceitou
            // Quando há um formulário para termo de uso
            // Quando há algum contrato escrito

            Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
            Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork);

            Dominio.Entidades.PaginaUsuario formulario = repPaginaUsuario.BuscarPorUsuarioEFormulario(usuario.Codigo, "TermosDeUso.aspx");
            Dominio.Entidades.EmpresaContrato empresaContrato = repEmpresaContrato.BuscarPorEmpresa(usuario.Empresa.Codigo);
            if (empresaContrato == null)
                empresaContrato = repEmpresaContrato.BuscarPorEmpresa(usuario.Empresa.EmpresaPai.Codigo);

            return
                !usuario.Empresa.AceitouTermosUso
                && formulario != null
                && (empresaContrato != null && !string.IsNullOrWhiteSpace(empresaContrato.Contrato.Trim()))
            ;
        }

        #endregion

        #region Métodos Privados

        private void SalvarUsuario(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (usuario != null && empresa != null)
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuarioFilial = null;
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    usuarioFilial = repUsuario.BuscarPorCPF(usuario.CPF);
                else
                    usuarioFilial = repUsuario.BuscarMotoristaPorCPF(empresa.Codigo, usuario.CPF);
                if (usuarioFilial == null)
                    usuarioFilial = new Dominio.Entidades.Usuario();

                usuarioFilial.Empresa = empresa;
                usuarioFilial.Tipo = usuario.Tipo;
                usuarioFilial.CPF = usuario.CPF;
                usuarioFilial.Nome = usuario.Nome;
                usuarioFilial.Bairro = usuario.Bairro;
                usuarioFilial.Categoria = usuario.Categoria;
                usuarioFilial.CEP = usuario.CEP;
                usuarioFilial.Complemento = usuario.Complemento;
                usuarioFilial.DataAdmissao = usuario.DataAdmissao;
                usuarioFilial.DataHabilitacao = usuario.DataHabilitacao;
                usuarioFilial.DataNascimento = usuario.DataNascimento;
                usuarioFilial.DataVencimentoHabilitacao = usuario.DataVencimentoHabilitacao;
                usuarioFilial.Email = usuario.Email;
                usuarioFilial.Endereco = usuario.Endereco;
                usuarioFilial.EnderecoDigitado = usuario.EnderecoDigitado;
                usuarioFilial.EstadoCivil = usuario.EstadoCivil;
                usuarioFilial.EstadoRG = usuario.EstadoRG;
                usuarioFilial.Localidade = usuario.Localidade;
                usuarioFilial.Moop = usuario.Moop;
                usuarioFilial.NumeroCartao = usuario.NumeroCartao;
                usuarioFilial.NumeroEndereco = usuario.NumeroEndereco;
                usuarioFilial.NumeroHabilitacao = usuario.NumeroHabilitacao;
                usuarioFilial.OrgaoEmissorRG = usuario.OrgaoEmissorRG;
                usuarioFilial.PercentualComissao = usuario.PercentualComissao;
                usuarioFilial.PIS = usuario.PIS;
                usuarioFilial.RG = usuario.RG;
                usuarioFilial.Salario = usuario.Salario;
                usuarioFilial.Sexo = usuario.Sexo;
                usuarioFilial.Status = usuario.Status;
                usuarioFilial.Setor = usuario.Setor;
                usuarioFilial.Telefone = usuario.Telefone;
                usuarioFilial.TipoAcesso = usuario.TipoAcesso;
                usuarioFilial.TipoEmail = usuario.TipoEmail;
                usuarioFilial.TipoEndereco = usuario.TipoEndereco;
                usuarioFilial.TipoLogradouro = usuario.TipoLogradouro;
                usuarioFilial.TipoSanguineo = usuario.TipoSanguineo;

                if (usuarioFilial.Codigo > 0)
                    repUsuario.Atualizar(usuarioFilial);
                else
                    repUsuario.Inserir(usuarioFilial);
            }
        }

        #endregion

        #region Relatório Perfil de Acesso

        public List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> AgrupaRelatorioPerfilAcesso(List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> dataSet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var queryableDataSet = (from o in dataSet
                                    group o by new
                                    {
                                        Descricao = o.Descricao,
                                        CodigoIntegracao = o.CodigoIntegracao,
                                        Modulo = o.Modulo,
                                        Formulario = o.Formulario,
                                        PermissoesPersonalizadas = o.PermissoesPersonalizadas,
                                        TipoPermissao = o.TipoPermissao
                                    } into g
                                    select new Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso
                                    {
                                        Descricao = g.Key.Descricao,
                                        CodigoIntegracao = g.Key.CodigoIntegracao,
                                        Formulario = g.Key.Formulario,
                                        Modulo = g.Key.Modulo,
                                        PermissoesPersonalizadas = g.Key.PermissoesPersonalizadas,
                                        TipoPermissao = g.Key.TipoPermissao
                                    }).AsQueryable();

            if (parametrosConsulta.DirecaoOrdenar == "desc")
                queryableDataSet = queryableDataSet.OrderByDescending(i => OrderRelatorioPerfilAcesso(i, parametrosConsulta.PropriedadeOrdenar));
            else
                queryableDataSet = queryableDataSet.OrderBy(i => OrderRelatorioPerfilAcesso(i, parametrosConsulta.PropriedadeOrdenar));

            return queryableDataSet.ToList();
        }

        private object OrderRelatorioPerfilAcesso(Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso par, string propOrdena)
        {
            if (propOrdena == "PermissoesPersonalizadas")
                return par.PermissoesPersonalizadas;
            if (propOrdena == "Formulario")
                return par.Formulario;
            if (propOrdena == "Modulo")
                return par.Modulo;
            if (propOrdena == "CodigoIntegracao")
                return par.CodigoIntegracao;
            else
                return par.Descricao;
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> RelatorioPerfilAcesso(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(filtrosPesquisa.AdminStringConexao);

            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilFormulario repPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);
            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminUnitOfWork);
            AdminMultisoftware.Repositorio.Modulos.Modulo repModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(adminUnitOfWork);
            AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(adminUnitOfWork);
            AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada repPermissaoPersonalizada = new AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada(adminUnitOfWork);

            List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> dsRelatorio = new List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso>();
            List<string> propriedades = propriedadesAgrupamento.Select(o => o.Propriedade).ToList();

            List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> perfis = repPerfilAcesso.RelatorioPerfis(filtrosPesquisa.CodigoPerfil, filtrosPesquisa.Ativo);


            List<AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada> permissoesPersonalizadas = repPermissaoPersonalizada.BuscarPorClieteETipoServico(repCliente.BuscarPorCodigo(filtrosPesquisa.Cliente), filtrosPesquisa.TipoServicoMultisoftware, (repClienteURLAcesso.BuscarPorCodigo(filtrosPesquisa.ClienteAcesso)?.URLHomologacao ?? false));
            List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> todosModulos = repModulo.BuscarPorClieteETipoServico(repCliente.BuscarPorCodigo(filtrosPesquisa.Cliente), filtrosPesquisa.TipoServicoMultisoftware, (repClienteURLAcesso.BuscarPorCodigo(filtrosPesquisa.ClienteAcesso)?.URLHomologacao ?? false));

            foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfil in perfis)
            {
                bool perfilSemInformacoes = true;

                if (perfil.PerfilAdministrador)
                {
                    PreencherDataSet(ref dsRelatorio, propriedades, perfil.Descricao, perfil.CodigoIntegracao, "(Perfil Administrador)", "(Perfil Administrador)", "(Perfil Administrador)", "(Perfil Administrador)");
                    continue;
                }

                List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> formulariosPerfil = repPerfilFormulario.BuscarPorPerfil(perfil.Codigo);
                foreach (int codigoModulo in perfil.ModulosLiberados)
                {
                    AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo = (from m in todosModulos where m.CodigoModulo == codigoModulo select m).FirstOrDefault();//repModulo.BuscarPorCodigoModulo(codigoModulo);
                    // Se o modulo for filho de um modulo que tem acesso total, não escreve
                    if (modulo != null && (modulo.ModuloPai == null || (modulo.ModuloPai != null && !perfil.ModulosLiberados.Contains(modulo.ModuloPai.CodigoModulo))))
                    {
                        PreencherDataSet(ref dsRelatorio, propriedades, perfil.Descricao, perfil.CodigoIntegracao, HirarquiaModulo(modulo) + " (Acesso Total)", "", "", "");
                        perfilSemInformacoes = false;
                    }
                }

                // Todo: Criar o método no repFormulario para buscar por vários códigos
                List<int> codigosFormularios = (from o in formulariosPerfil select o.CodigoFormulario).Distinct().ToList();
                List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = new List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario>();
                for (int i = 0; i < codigosFormularios.Count; i++)
                {
                    var form = repFormulario.BuscarPorCodigoFormulario(codigosFormularios[i]);
                    if (form != null) formularios.Add(form);
                }

                // Obtem todos modulos que possuem os formularios
                List<int> codigosModulos = (from f in formularios select f.Modulo.CodigoModulo).ToList();

                foreach (int codigoModulo in codigosModulos)
                {
                    List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> modulosOrfaos = (from m in todosModulos where m.ModuloPai == null && m.CodigoModulo == codigoModulo select m).ToList();

                    AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo = (from m in todosModulos where m.CodigoModulo == codigoModulo select m).FirstOrDefault();

                    FormulariosPorModulo(perfil.Descricao, perfil.CodigoIntegracao, ref dsRelatorio, propriedades, modulo, formularios, formulariosPerfil, todosModulos, permissoesPersonalizadas);

                    foreach (AdminMultisoftware.Dominio.Entidades.Modulos.Modulo moduloOrfao in modulosOrfaos)
                        FormulariosPorModulo(perfil.Descricao, perfil.CodigoIntegracao, ref dsRelatorio, propriedades, moduloOrfao, formularios, formulariosPerfil, todosModulos, permissoesPersonalizadas);

                    perfilSemInformacoes = false;
                }

                if (perfilSemInformacoes)
                    PreencherDataSet(ref dsRelatorio, propriedades, perfil.Descricao, perfil.CodigoIntegracao, "", "", "", "");
            }

            adminUnitOfWork.Dispose();
            return dsRelatorio;
        }

        private void FormulariosPorModulo(
            string perfil,
            string codigoIntegracao,
            ref List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> dsRelatorio,
            List<string> propriedades,
            AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo,
            List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios,
            List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> formulariosPerfil,
            List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> modulosLiberados,
            List<AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada> permissoesPersonalizadas
        )
        {
            if (modulo != null)
            {
                List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formulariosDoModulo = (from o in formularios where o.Modulo.CodigoModulo == modulo.CodigoModulo select o).ToList();
                foreach (AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario in formulariosDoModulo)
                {
                    Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario objFormulario = (from o in formulariosPerfil where o.CodigoFormulario == formulario.CodigoFormulario select o).FirstOrDefault();
                    string somenteLeitura = objFormulario.SomenteLeitura ? "Somente Leitura" : "Acesso Total";
                    PreencherDataSet(ref dsRelatorio, propriedades, perfil, codigoIntegracao, HirarquiaModulo(modulo), formulario.Descricao, "", somenteLeitura);

                    if (objFormulario.FormularioPermissaoPersonalizada.Count() > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada objPermissaoPersonalizada in objFormulario.FormularioPermissaoPersonalizada)
                        {
                            var permissaoPersonalizada = (from opc in permissoesPersonalizadas
                                                          where opc.Formulario.CodigoFormulario == formulario.CodigoFormulario && opc.CodigoPermissao == (AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada)objPermissaoPersonalizada.CodigoPermissao
                                                          select opc).FirstOrDefault();

                            if (permissaoPersonalizada != null)
                            {
                                somenteLeitura = "Personalizada";
                                PreencherDataSet(ref dsRelatorio, propriedades, perfil, codigoIntegracao, HirarquiaModulo(modulo), formulario.Descricao, permissaoPersonalizada.Descricao, somenteLeitura);
                            }
                        }
                    }
                }

                List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> proximosModulos = (from m in modulosLiberados where m.ModuloPai != null && m.ModuloPai.CodigoModulo == modulo.CodigoModulo select m).ToList();
                foreach (AdminMultisoftware.Dominio.Entidades.Modulos.Modulo prox in proximosModulos)
                    FormulariosPorModulo(perfil, codigoIntegracao, ref dsRelatorio, propriedades, prox, formularios, formulariosPerfil, modulosLiberados, permissoesPersonalizadas);
            }
        }

        private string HirarquiaModulo(AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo)
        {
            List<string> descricaoModulo = new List<string>();
            do
            {
                descricaoModulo.Add(modulo.Descricao);
                modulo = modulo.ModuloPai;
            } while (modulo != null);

            descricaoModulo.Reverse();

            return String.Join(" > ", descricaoModulo);
        }

        private void PreencherDataSet(ref List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> dsRelatorio, List<string> propriedades, string perfil, string codigoIntegracao, string modulo, string formulario, string permissao, string tipoPermissao)
        {
            dsRelatorio.Add(new Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso()
            {
                Descricao = propriedades.Contains("Descricao") ? perfil : "",
                CodigoIntegracao = propriedades.Contains("CodigoIntegracao") ? codigoIntegracao : "",
                Modulo = propriedades.Contains("Modulo") ? modulo : "",
                Formulario = propriedades.Contains("Formulario") ? formulario : "",
                PermissoesPersonalizadas = propriedades.Contains("PermissoesPersonalizadas") ? permissao : "",
                TipoPermissao = propriedades.Contains("TipoPermissao") ? tipoPermissao : "",
            });
        }

        #endregion
    }
}
