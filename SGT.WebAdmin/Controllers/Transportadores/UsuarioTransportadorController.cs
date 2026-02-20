using Dominio.Entidades.Embarcador.Usuarios;
using Dominio.Excecoes.Embarcador;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGT.WebAdmin.Notifications;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/Transportador")]
    public class UsuarioTransportadorController : BaseController
    {
        private readonly IMediator _mediator;

        #region Construtores

        public UsuarioTransportadorController(Conexao conexao, IMediator mediator) : base(conexao)
        {
            _mediator = mediator;
        }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repConfiguracaoTransportador.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario()
                {
                    Nome = Request.GetStringParam("Nome"),
                    CpfCnpj = Request.GetStringParam("CPFCNPJ").ObterSomenteNumeros(),
                    Status = Request.GetStringParam("Status"),
                    CodigoEmpresa = Request.GetIntParam("CodigoTransportador"),
                    IgnorarSituacaoMotorista = true,
                    TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao,
                    Tipo = "U",
                    OcultarUsuarioMultiCTe = configuracaoTransportador.NaoGerarAutomaticamenteUsuarioAcessoPortalTransportador
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Nome, "Nome", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.CPFCNPJ, "CPF", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.RgIe, "RG", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Email, "Email", 20, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);

                List<Dominio.Entidades.Usuario> usuarios = repUsuario.Consultar(filtrosPesquisa, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                int countUsuarios = repUsuario.ContarConsulta(filtrosPesquisa);

                grid.setarQuantidadeTotal(countUsuarios);

                grid.AdicionaRows((from obj in usuarios select new { obj.Codigo, obj.Nome, obj.CPF, obj.RG, obj.Email }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoObterOsUsuarios);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo, codigoTransportador;

                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
                Repositorio.PaginaUsuario repPermissoes = new Repositorio.PaginaUsuario(unidadeTrabalho);

                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                if (politicaSenha == null)
                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorPorCodigoEEmpresa(codigoTransportador, codigo);

                if (usuario == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.UsuarioNaoEncontrado);

                List<Dominio.Entidades.PaginaUsuario> permissoes = repPermissoes.BuscarPorUsuario(usuario.Codigo);

                var retorno = new
                {
                    usuario.Codigo,
                    usuario.Complemento,
                    CPFCNPJ = usuario.CPF,
                    DataAdmissao = usuario.DataAdmissao.HasValue ? usuario.DataAdmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataNascimento = usuario.DataNascimento.HasValue ? usuario.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    usuario.Email,
                    usuario.Endereco,
                    Localidade = new { Descricao = usuario.Localidade.DescricaoCidadeEstado, usuario.Localidade.Codigo },
                    Usuario = usuario.Login,
                    Senha = politicaSenha != null && politicaSenha.HabilitarPoliticaSenha ? "" : usuario?.Senha ?? string.Empty,
                    ConfirmacaoSenha = politicaSenha != null && politicaSenha.HabilitarPoliticaSenha ? "" : usuario?.Senha ?? string.Empty,
                    usuario.Nome,
                    RGIE = usuario.RG,
                    usuario.Salario,
                    usuario.Status,
                    usuario.Telefone,
                    usuario.UsuarioAcessoBloqueado,
                    usuario.PermiteAssinarAnuencia,
                    Permissoes = (from obj in permissoes
                                  select new
                                  {
                                      obj.Pagina.Codigo,
                                      obj.Pagina.Descricao,
                                      Acesso = obj.PermissaoDeAcesso == "A" ? true : false,
                                      Incluir = obj.PermissaoDeInclusao == "A" ? true : false,
                                      Alterar = obj.PermissaoDeAlteracao == "A" ? true : false,
                                      Excluir = obj.PermissaoDeDelecao == "A" ? true : false
                                  }).ToList(),
                    Series = (from obj in usuario.Series
                              select new
                              {
                                  obj.Codigo,
                                  obj.Numero,
                                  obj.Tipo
                              }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
            Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();

            try
            {
                int codigo, codigoTransportador, codigoLocalidade;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);
                int.TryParse(Request.Params("Localidade"), out codigoLocalidade);

                decimal salario;
                decimal.TryParse(Request.Params("Salario"), out salario);

                string confirmacaoSenha = Request.Params("ConfirmacaoSenha");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                Dominio.Entidades.Usuario usuario = null;

                if (codigo > 0)
                {
                    usuario = repUsuario.BuscarPorPorCodigoEEmpresa(codigoTransportador, codigo);
                    usuario.Initialize();
                }
                else
                {
                    usuario = new Dominio.Entidades.Usuario();

                    usuario.Empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);
                    usuario.Tipo = "U";
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                }

                usuario.Complemento = Request.Params("Complemento");
                usuario.CPF = Utilidades.String.OnlyNumbers(Request.Params("CPFCNPJ"));

                DateTime data = new DateTime();

                if (DateTime.TryParseExact(Request.Params("DataAdmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    usuario.DataAdmissao = data;
                else
                    usuario.DataAdmissao = null;

                if (DateTime.TryParseExact(Request.Params("DataNascimento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    usuario.DataNascimento = data;
                else
                    usuario.DataNascimento = null;

                usuario.Email = Request.Params("Email");
                usuario.Endereco = Request.Params("Endereco");
                usuario.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);
                usuario.Login = Request.Params("Usuario");
                usuario.Nome = Request.Params("Nome");
                usuario.RG = Request.Params("RGIE");
                usuario.Salario = salario;
                //usuario.Senha = Request.Params("Senha");
                usuario.Status = Request.Params("Status");
                usuario.Telefone = Request.Params("Telefone");
                usuario.TipoPessoa = usuario.CPF_CNPJ_Formatado.Length > 11 ? "J" : "F";
                usuario.UsuarioAcessoBloqueado = Request.Params("UsuarioAcessoBloqueado").ToBool();
                usuario.PermiteAssinarAnuencia = Request.GetBoolParam("PermiteAssinarAnuencia");

                if (usuario.Setor == null)
                    usuario.Setor = repSetor.BuscarPorCodigo(1);

                if (string.IsNullOrWhiteSpace(usuario.Nome))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.NomeInvalido);

                if (string.IsNullOrWhiteSpace(usuario.CPF) || (usuario.CPF.Length != 11 && usuario.CPF.Length != 14))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.CPFCNPJInvalido);

                if (usuario.Localidade == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.CidadeNaoEncontrada);

                string retornoPoliticaSenha = "";

                string senha = Request.Params("Senha");
                bool enviarEmail = false;

                if (politicaSenha != null && politicaSenha.HabilitarPoliticaSenha)
                {
                    if (string.IsNullOrWhiteSpace(usuario.Senha))
                    {
                        usuario.Senha = politicaSenha.CriarNovaSenha();
                        enviarEmail = true;
                    }

                    if (!politicaSenha.ExigirTrocaSenhaPrimeiroAcesso)
                    {
                        retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);
                        usuario.DataUltimaAlteracaoSenhaObrigatoria = DateTime.Now;
                        usuario.AlterarSenhaAcesso = false;
                    }
                    else
                        usuario.AlterarSenhaAcesso = true;
                }
                else //nao possui politica senha
                {
                    if (string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(confirmacaoSenha) || senha.Length < 5 || (senha != confirmacaoSenha))
                        return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.SenhaDeveConterNoMinimoCincoCaracteresCoincidirComConfirmacaoDeSenha);
                    else
                    {
                        usuario.Senha = senha;
                        usuario.SenhaCriptografada = false;
                    }
                }

                if (string.IsNullOrWhiteSpace(retornoPoliticaSenha))
                {
                    Dominio.Entidades.Usuario usuarioAux = repUsuario.BuscarPorLogin(usuario.Login);

                    if (usuarioAux != null)
                        if (usuarioAux.Codigo != usuario.Codigo)
                            return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.JaExisteUmUsuarioRegistradoComEsteLogin);

                    unitOfWork.Start();

                    if (usuario.Codigo > 0)
                        repUsuario.Atualizar(usuario, Auditado);
                    else
                    {
                        repUsuario.Inserir(usuario, Auditado);
                        AdicionarUsuarioEmAlteracoesTabelasFretePendentesTransportador(unitOfWork, usuario);
                    }

                    SalvarSeries(usuario, unitOfWork);
                    SalvarPermissoes(usuario, unitOfWork);
                    SalvarPermissoesAdminNovo(usuario, unitOfWork);

                    unitOfWork.CommitChanges();

                    if (enviarEmail)
                    {
                        var notifications = new UsuarioImportadoNotification(
                            usuario.Nome,
                            usuario.Email,
                            usuario.Senha,
                            usuario.Login
                        );

                        await EnviarNotificacaoUsuarioAsync(notifications);
                    }

                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, retornoPoliticaSenha);
                }
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoSalvarUsuario);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ResetarSenha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo, codigoTransportador;

                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);

                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
                Usuario repUsuario = new Usuario(unitOfWork);

                PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorPorCodigoEEmpresa(codigoTransportador, codigo);

                if (usuario != null && politicaSenha != null && politicaSenha.HabilitarPoliticaSenha)
                {
                    usuario.Senha = politicaSenha.CriarNovaSenha();
                    usuario.AlterarSenhaAcesso = true;
                    if (!politicaSenha.ExigirTrocaSenhaPrimeiroAcesso)
                        usuario.AlterarSenhaAcesso = false;

                    repUsuario.Atualizar(usuario, Auditado);

                    unitOfWork.CommitChanges();

                    var notifications = new UsuarioImportadoNotification(
                           usuario.Nome,
                           usuario.Email,
                           usuario.Senha,
                           usuario.Login
                       );

                    await EnviarNotificacaoUsuarioAsync(notifications);

                    return new JsonpResult(true);
                }

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ResetarSenhaErro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarSeries(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

            dynamic series = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Series"));

            if (usuario.Series == null)
                usuario.Series = new List<Dominio.Entidades.EmpresaSerie>();
            else
                usuario.Series.Clear();

            foreach (var ser in series)
                usuario.Series.Add(repSerie.BuscarPorCodigo((int)ser.Codigo));
        }

        private void SalvarPermissoes(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Pagina repPagina = new Repositorio.Pagina(unidadeDeTrabalho);
            Repositorio.PaginaUsuario repPermissaoUsuario = new Repositorio.PaginaUsuario(unidadeDeTrabalho);

            dynamic perm = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Permissoes"));

            List<int> codigosPaginas = new List<int>();

            for (var i = 0; i < perm.Count; i++)
                codigosPaginas.Add((int)perm[i].Codigo);

            List<Dominio.Entidades.PaginaUsuario> permissoes = repPermissaoUsuario.BuscarPorUsuario(usuario.Codigo);

            if (permissoes.Count() > 0)
            {
                List<Dominio.Entidades.PaginaUsuario> permissoesDeletar = (from obj in permissoes where !codigosPaginas.Contains(obj.Pagina.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.PaginaUsuario permissaoDeletar in permissoesDeletar)
                    repPermissaoUsuario.Deletar(permissaoDeletar);
            }

            foreach (var p in perm)
            {
                Dominio.Entidades.PaginaUsuario permissao = (from obj in permissoes where obj.Pagina.Codigo == (int)p.Codigo select obj).FirstOrDefault();

                if (permissao == null)
                    permissao = new Dominio.Entidades.PaginaUsuario();

                permissao.Usuario = usuario;
                permissao.Pagina = repPagina.BuscarPorCodigo((int)p.Codigo);
                permissao.PermissaoDeAcesso = (bool)p.Acesso ? "A" : "I";
                permissao.PermissaoDeAlteracao = (bool)p.Alterar ? "A" : "I";
                permissao.PermissaoDeDelecao = (bool)p.Excluir ? "A" : "I";
                permissao.PermissaoDeInclusao = (bool)p.Incluir ? "A" : "I";

                if (permissao.Codigo > 0)
                    repPermissaoUsuario.Atualizar(permissao);
                else
                    repPermissaoUsuario.Inserir(permissao);
            }
        }

        private void SalvarPermissoesAdminNovo(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            usuario.UsuarioAdministrador = true;
            //Repositorio.Embarcador.Usuarios.FuncionarioFormulario repFuncionarioFormulario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unidadeTrabalho);
            //AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            //AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWorkAdmin);

            //List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = repFormulario.BuscarPorTipoServico(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);

            //foreach (AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario in formularios)
            //{
            //    if (formulario.TiposServicosMultisoftware.Contains(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) &&
            //        repFuncionarioFormulario.ContarPorUsuarioEFormulario(usuario.Codigo, formulario.CodigoFormulario) <= 0)
            //    {
            //        Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario();

            //        funcionarioFormulario.Usuario = usuario;
            //        funcionarioFormulario.SomenteLeitura = false;
            //        funcionarioFormulario.CodigoFormulario = formulario.CodigoFormulario;

            //        repFuncionarioFormulario.Inserir(funcionarioFormulario);
            //    }
            //}
        }

        private void AdicionarUsuarioEmAlteracoesTabelasFretePendentesTransportador(UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete repAprovacaoAlcadaTabelaFrete = new Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete(unitOfWork);
            repAprovacaoAlcadaTabelaFrete.AdicionarUsuarioComoNovoAprovadorEmPendentes(usuario.Codigo, usuario.Empresa.Codigo);
        }

        private async Task EnviarNotificacaoUsuarioAsync(params UsuarioImportadoNotification[] notifications)
        {
            var tasks = notifications.Select(notification =>
            {
                System.Diagnostics.Debug.WriteLine($"Publishing notification for user: {notification.NomeUsuario}");
                return _mediator.Publish(notification);
            });

            await Task.WhenAll(tasks);
        }

        #endregion
    }
}
