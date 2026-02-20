using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/PerfilAcesso")]
    public class PerfilAcessoController : BaseController
    {
		#region Construtores

		public PerfilAcessoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Usado na tela de usuário para buscar o perfil de permissões
        /// </summary>
        /// <returns></returns>
        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorPerfil()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilFormulario repPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = repPerfilAcesso.BuscarPorCodigo(codigo);

                // Valida
                if (perfilAcesso == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.PerfilAcesso.NaoFoiPossivelEncontrarPerfil);

                // Formata retorno
                var retorno = new
                {
                    UsuarioAdministrador = perfilAcesso.PerfilAdministrador,
                    FormulariosUsuario = (from obj in repPerfilFormulario.BuscarPorPerfil(perfilAcesso.Codigo)
                                          select new
                                          {
                                              obj.CodigoFormulario,
                                              obj.SomenteLeitura,
                                              PermissoesPersonalizadas = (from pp in obj.FormularioPermissaoPersonalizada
                                                                          select new
                                                                          {
                                                                              pp.CodigoPermissao
                                                                          }).ToList(),
                                          }).ToList(),
                    ModulosUsuario = (from codigoModulo in perfilAcesso.ModulosLiberados
                                      select new
                                      {
                                          CodigoModulo = codigoModulo
                                      }).ToList(),
                    TiposPropostasMultimodal = perfilAcesso.TiposPropostasMultimodal.Select(o => o).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.PerfilAcesso.OcorreuFalhaConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = new Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso();

                // Preenche entidade com dados
                PreencheEntidade(ref perfilAcesso, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(perfilAcesso, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repPerfilAcesso.Inserir(perfilAcesso);//, Auditado);

                // Preenche Formulário
                PreencheFormularios(perfilAcesso, null, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.PerfilAcesso.OcorreuFalhaAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilFormulario repPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioFormulario repFuncionarioFormulario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada repFuncionarioFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = repPerfilAcesso.BuscarPorCodigo(codigo, true);

                // Valida
                if (perfilAcesso == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.PerfilAcesso.NaoFoiPossivelEncontrarRegistro);

                // Preenche entidade com dados
                PreencheEntidade(ref perfilAcesso, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(perfilAcesso, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repPerfilAcesso.Atualizar(perfilAcesso);//, Auditado);

                // Remove formulários
                List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> formulariosCadastrados = repPerfilFormulario.BuscarPorPerfil(perfilAcesso.Codigo);
                foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario formularioCadastrado in formulariosCadastrados)
                    repPerfilFormulario.Deletar(formularioCadastrado);

                // Preenche Formulário
                PreencheFormularios(perfilAcesso, formulariosCadastrados, unitOfWork);

                // Persiste dados
                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();

                // Inicia transacao
                unitOfWork.Start();

                //-- Atualiza informações dos usuários

                // Busca formulários da configuracao
                List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> formulariosPerfil = repPerfilFormulario.BuscarPorPerfil(perfilAcesso.Codigo);

                // Atualiza todos os usuários com esse perfil
                List<Dominio.Entidades.Usuario> usuarios = repUsuario.BuscarPorPerfilEmbarcador(perfilAcesso.Codigo);
                foreach (Dominio.Entidades.Usuario usuario in usuarios)
                {
                    usuario.UsuarioAdministrador = perfilAcesso.PerfilAdministrador;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, string.Format(Localization.Resources.Pessoas.PerfilAcesso.AtualizouPerfilAcesso, perfilAcesso.Descricao), unitOfWork);
                    // Atualiza modulos
                    usuario.ModulosLiberados.Clear();
                    foreach (int codigoModulo in perfilAcesso.ModulosLiberados)
                        usuario.ModulosLiberados.Add(codigoModulo);

                    // Apaga Deleta
                    List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario> formulariosCadastradosUsuarios = repFuncionarioFormulario.buscarPorUsuario(usuario.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario formulario in formulariosCadastradosUsuarios)
                        repFuncionarioFormulario.Deletar(formulario);

                    // Atualiza Formulários
                    foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario formulario in formulariosPerfil)
                    {
                        Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario();
                        funcionarioFormulario.Usuario = usuario;
                        funcionarioFormulario.SomenteLeitura = formulario.SomenteLeitura;
                        funcionarioFormulario.CodigoFormulario = formulario.CodigoFormulario;

                        repFuncionarioFormulario.Inserir(funcionarioFormulario);

                        // Atualiza Permissões personalizadas
                        if (formulario.FormularioPermissaoPersonalizada != null)
                        {
                            foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada permissao in formulario.FormularioPermissaoPersonalizada)
                            {
                                Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada funcionarioFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada();
                                funcionarioFormularioPermissaoPersonalizada.CodigoPermissao = permissao.CodigoPermissao;
                                funcionarioFormularioPermissaoPersonalizada.FuncionarioFormulario = funcionarioFormulario;

                                repFuncionarioFormularioPermissaoPersonalizada.Inserir(funcionarioFormularioPermissaoPersonalizada);
                            }
                        }
                    }

                    repUsuario.Atualizar(usuario);
                }


                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.PerfilAcesso.OcorreuFalhaAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilFormulario repPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = repPerfilAcesso.BuscarPorCodigo(codigo);

                if (perfilAcesso == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.PerfilAcesso.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> formularios = repPerfilFormulario.BuscarPorPerfil(perfilAcesso.Codigo);

                var retorno = new
                {
                    perfilAcesso.Codigo,
                    perfilAcesso.Descricao,
                    perfilAcesso.PerfilAdministrador,
                    perfilAcesso.PermiteFaturamentoPermissaoExclusiva,
                    Status = perfilAcesso.Ativo,
                    perfilAcesso.CodigoIntegracao,
                    perfilAcesso.PermiteSalvarNovoRelatorio,
                    perfilAcesso.PermiteTornarRelatorioPadrao,
                    perfilAcesso.PermiteSalvarConfiguracoesRelatoriosParaTodos,
                    perfilAcesso.PermitirAbrirOcorrenciaAposPrazoSolicitacao,
                    perfilAcesso.VisualizarTitulosPagamentoSalario,
                    HoraInicialAcesso = perfilAcesso.HoraInicialAcesso.HasValue ? perfilAcesso.HoraInicialAcesso.Value.ToString(@"hh\:mm") : string.Empty,
                    HoraFinalAcesso = perfilAcesso.HoraFinalAcesso.HasValue ? perfilAcesso.HoraFinalAcesso.Value.ToString(@"hh\:mm") : string.Empty,
                    FormulariosPerfil = (from obj in formularios
                                         select new
                                         {
                                             obj.CodigoFormulario,
                                             obj.SomenteLeitura,
                                             PermissoesPersonalizadas = (from pp in obj.FormularioPermissaoPersonalizada
                                                                         select new
                                                                         {
                                                                             pp.CodigoPermissao
                                                                         }).ToList(),
                                         }).ToList(),
                    ModulosPerfil = (from codigoModulo in perfilAcesso.ModulosLiberados
                                     select new
                                     {
                                         CodigoModulo = codigoModulo
                                     }).ToList(),
                    TiposPropostasMultimodal = perfilAcesso.TiposPropostasMultimodal.Select(o => o).ToList(),
                    Turno = perfilAcesso.Turno != null ? new { perfilAcesso.Turno.Codigo, perfilAcesso.Turno.Descricao } : null,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.PerfilAcesso.OcorreuFalhaConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilFormulario repPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada repPerfilFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = repPerfilAcesso.BuscarPorCodigo(codigo);

                // Valida
                if (perfilAcesso == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.PerfilAcesso.NaoFoiPossivelEncontrarRegistro);

                // Remove Formularios
                List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> formularios = repPerfilFormulario.BuscarPorPerfil(perfilAcesso.Codigo);
                for (var i = 0; i < formularios.Count; i++)
                    repPerfilFormulario.Deletar(formularios[i]);

                // Persiste dados
                repPerfilAcesso.Deletar(perfilAcesso);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.PerfilAcesso.NaoFoiPossivelExcluirPossuiVinculo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Pessoas.PerfilAcesso.OcorreuFalhaExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            Repositorio.Embarcador.Filiais.Turno repositorioTurno = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);

            // Converte valores
            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            bool ativo = false;
            bool.TryParse(Request.Params("Status"), out ativo);

            bool perfilAdministrador = false;
            bool.TryParse(Request.Params("PerfilAdministrador"), out perfilAdministrador);

            TimeSpan horaInicialAcesso = TimeSpan.MinValue, horaFinalAcesso = TimeSpan.MinValue;
            if (!string.IsNullOrWhiteSpace(Request.Params("HoraInicialAcesso")))
                TimeSpan.TryParse(Request.Params("HoraInicialAcesso"), out horaInicialAcesso);
            if (!string.IsNullOrWhiteSpace(Request.Params("HoraFinalAcesso")))
                TimeSpan.TryParse(Request.Params("HoraFinalAcesso"), out horaFinalAcesso);

            string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");

            // Vincula dados
            perfilAcesso.Descricao = descricao;
            perfilAcesso.CodigoIntegracao = codigoIntegracao;
            perfilAcesso.Ativo = ativo;
            perfilAcesso.PerfilAdministrador = perfilAdministrador;
            perfilAcesso.PermiteFaturamentoPermissaoExclusiva = Request.GetBoolParam("PermiteFaturamentoPermissaoExclusiva");

            if (horaInicialAcesso > TimeSpan.MinValue)
                perfilAcesso.HoraInicialAcesso = horaInicialAcesso;
            else
                perfilAcesso.HoraInicialAcesso = null;
            if (horaFinalAcesso > TimeSpan.MinValue)
                perfilAcesso.HoraFinalAcesso = horaFinalAcesso;
            else
                perfilAcesso.HoraFinalAcesso = null;

            perfilAcesso.PermiteSalvarNovoRelatorio = Request.GetBoolParam("PermiteSalvarNovoRelatorio");
            perfilAcesso.PermitirAbrirOcorrenciaAposPrazoSolicitacao = Request.GetBoolParam("PermitirAbrirOcorrenciaAposPrazoSolicitacao");
            perfilAcesso.PermiteTornarRelatorioPadrao = Request.GetBoolParam("PermiteTornarRelatorioPadrao");
            perfilAcesso.PermiteSalvarConfiguracoesRelatoriosParaTodos = Request.GetBoolParam("PermiteSalvarConfiguracoesRelatoriosParaTodos");
            perfilAcesso.VisualizarTitulosPagamentoSalario = Request.GetBoolParam("VisualizarTitulosPagamentoSalario");
            perfilAcesso.Turno = repositorioTurno.BuscarPorCodigo(Request.GetIntParam("Turno"));

            //if (perfilAcesso.TiposPropostasMultimodal == null)
            perfilAcesso.TiposPropostasMultimodal = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TiposPropostasMultimodal");
            foreach (var tipoPropostaMultimodal in tiposPropostasMultimodal)
                perfilAcesso.TiposPropostasMultimodal.Add(tipoPropostaMultimodal);
        }

        private void PreencheFormularios(Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso, List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> formularios, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheFormularios
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Repositorios
            Repositorio.Embarcador.Usuarios.PerfilFormulario repPerfilFormulario = new Repositorio.Embarcador.Usuarios.PerfilFormulario(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada repPerfilFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada(unitOfWork);

            // Vincula Módulos
            var jModulosPerfil = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosPerfil"));
            var jModulosNSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosNSelecionados")); //Módulos que não foram selecionados

            List<int> modulosAnteriores = perfilAcesso?.ModulosLiberados?.ToList() ?? new List<int>(); //Modulos anteriores

            perfilAcesso.ModulosLiberados = new List<int>();
            foreach (var jModulo in jModulosPerfil)
            {
                perfilAcesso.ModulosLiberados.Add((int)jModulo.CodigoModulo);
                if (!modulosAnteriores.Contains((int)jModulo.CodigoModulo))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcesso, null, $"Módulo {(string)jModulo.Descricao}({(string)jModulo.CodigoModulo}) adicionado ao perfil.", unitOfWork);
            }

            var modulosRemovidos = new List<int>();
            if (modulosAnteriores.Count > 0)
                modulosRemovidos = (from obj in modulosAnteriores where !perfilAcesso.ModulosLiberados.Contains(obj) select obj).ToList();

            foreach (int modulo in modulosRemovidos)
            {
                foreach (var jModuloNSelecionado in jModulosNSelecionados)
                {
                    if ((int)jModuloNSelecionado.CodigoModulo == modulo)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcesso, null, $"Módulo {(string)jModuloNSelecionado.Descricao}({(int)jModuloNSelecionado.CodigoModulo}) removido do perfil.", unitOfWork);
                        break;
                    }
                }
            }


            // Vincula Formulário
            var jFormulariosPerfil = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FormulariosPerfil"));
            var jFormulariosNSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FormulariosNSelecionados")); //Formulários não selecionados

            List<int> novosFormularios = new List<int>();
            List<int> novasPermissoes = new List<int>();
            foreach (var jFormulario in jFormulariosPerfil)
            {
                Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario perfilFormulario = new Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario();
                perfilFormulario.SomenteLeitura = (bool)jFormulario.SomenteLeitura;
                perfilFormulario.CodigoFormulario = (int)jFormulario.CodigoFormulario;
                perfilFormulario.PerfilAcesso = perfilAcesso;

                repPerfilFormulario.Inserir(perfilFormulario);
                novosFormularios.Add(perfilFormulario.CodigoFormulario);

                if (formularios == null || formularios.Where(o => o.CodigoFormulario == perfilFormulario.CodigoFormulario).ToList().Count <= 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcesso, null, $"Formulário {(string)jFormulario.Descricao}({perfilFormulario.CodigoFormulario}) adicionado ao perfil{(perfilFormulario.SomenteLeitura ? " com permissão somente leitura" : "")}.", unitOfWork);
                else if (perfilFormulario.SomenteLeitura != (from obj in formularios where obj.CodigoFormulario == perfilFormulario.CodigoFormulario select obj.SomenteLeitura).FirstOrDefault())
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcesso, null, $"Alterado formulário {(string)jFormulario.Descricao}({perfilFormulario.CodigoFormulario}) para {(perfilFormulario.SomenteLeitura ? "somente leitura" : "acesso liberado")}.", unitOfWork);

                // Vincula Permissões personalizadas (caso tenha)
                var formularioPermissao = formularios?.Where(o => o.CodigoFormulario == perfilFormulario.CodigoFormulario).FirstOrDefault();
                foreach (var dynPermissao in jFormulario.PermissoesPersonalizadas)
                {
                    Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada perfilFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada();

                    perfilFormularioPermissaoPersonalizada.CodigoPermissao = (int)dynPermissao.CodigoPermissaoPersonalizada;
                    perfilFormularioPermissaoPersonalizada.PerfilFormulario = perfilFormulario;

                    repPerfilFormularioPermissaoPersonalizada.Inserir(perfilFormularioPermissaoPersonalizada);
                    novasPermissoes.Add(perfilFormularioPermissaoPersonalizada.CodigoPermissao);

                    if (formularioPermissao == null || !formularioPermissao.FormularioPermissaoPersonalizada.Any(fpp => fpp?.CodigoPermissao == perfilFormularioPermissaoPersonalizada.CodigoPermissao))
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcesso, null, $"Permissão {(string)dynPermissao.Descricao}({perfilFormularioPermissaoPersonalizada.CodigoPermissao}) adicionada ao perfil.", unitOfWork);
                }

                if (formularioPermissao != null)
                {
                    var permissoesRemovidas = (from obj in formularioPermissao.FormularioPermissaoPersonalizada where !novasPermissoes.Contains(obj.CodigoPermissao) select obj).ToList();

                    foreach (var permissao in permissoesRemovidas)
                    {
                        foreach (var permissaoNSelecionada in jFormulario.PermissoesPersonalizadasNSelecionadas)
                        {
                            if ((int)permissaoNSelecionada.CodigoPermissaoPersonalizada == permissao.CodigoPermissao)
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcesso, null, $"Permissão {(string)permissaoNSelecionada.Descricao}({(int)permissaoNSelecionada.CodigoPermissaoPersonalizada}) removida do perfil.", unitOfWork);
                        }
                    }

                }
            }

            var formulariosRemovidos = new List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario>();
            if (formularios != null)
                formulariosRemovidos = (from obj in formularios where !novosFormularios.Contains(obj.CodigoFormulario) select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario formulario in formulariosRemovidos)
            {
                foreach (var jFormularioNSelecionado in jFormulariosNSelecionados)
                {
                    if ((int)jFormularioNSelecionado.CodigoFormulario == formulario.CodigoFormulario)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcesso, null, $"Formulário {(string)jFormularioNSelecionado.Descricao}({(int)jFormularioNSelecionado.CodigoFormulario}) removido do perfil.", unitOfWork);
                        break;
                    }
                }

            }

        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (string.IsNullOrWhiteSpace(perfilAcesso.Descricao))
            {
                msgErro = Localization.Resources.Pessoas.PerfilAcesso.DescricaoObrigatoria;
                return false;
            }
            else if ((perfilAcesso.HoraInicialAcesso != null && perfilAcesso.HoraFinalAcesso == null) ||
                (perfilAcesso.HoraInicialAcesso == null && perfilAcesso.HoraFinalAcesso != null))
            {
                msgErro = Localization.Resources.Pessoas.PerfilAcesso.FavorInformarHorarioAcessoInicialFinal;
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilPermissao = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Ativo")))
                Enum.TryParse(Request.Params("Ativo"), out status);
            else
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> listaGrid = repPerfilPermissao.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPerfilPermissao.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            DescricaoAtivo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Ativo, "DescricaoAtivo", 20, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}
