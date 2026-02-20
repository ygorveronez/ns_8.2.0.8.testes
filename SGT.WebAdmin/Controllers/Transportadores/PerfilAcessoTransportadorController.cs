using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/PerfilAcessoTransportador")]
    public class PerfilAcessoTransportadorController : BaseController
    {
		#region Construtores

		public PerfilAcessoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

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
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Usado na tela do transportador para buscar o perfil de permissões
        /// </summary>
        /// <returns></returns>
        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorPerfil()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador repPerfilAcessoTransportador = new Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador(unitOfWork);
                Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario repPerfilTransportadorFormulario = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Perfil"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfilAcesso = repPerfilAcessoTransportador.BuscarPorCodigo(codigo);

                // Valida
                if (perfilAcesso == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                // Formata retorno
                var retorno = new
                {
                    TransportadorAdministrador = perfilAcesso.PerfilAdministrador,
                    FormulariosTransportador = (from obj in repPerfilTransportadorFormulario.BuscarPorPerfil(perfilAcesso.Codigo)
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
                    ModulosTransportador = (from codigoModulo in perfilAcesso.ModulosLiberados
                                            select new
                                            {
                                                CodigoModulo = codigoModulo
                                            }).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
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
                Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador repPerfilAcessoTransportador = new Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfilAcessoTransportador = new Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador();

                // Preenche entidade com dados
                PreencheEntidade(ref perfilAcessoTransportador, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(perfilAcessoTransportador, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repPerfilAcessoTransportador.Inserir(perfilAcessoTransportador, Auditado);

                // Preenche Formulário
                PreencheFormularios(perfilAcessoTransportador, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAdicionarDados);
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
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador repPerfilAcessoTransportador = new Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador(unitOfWork);
                Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario repPerfilTransportadorFormulario = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada repTransportadorFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfilAcessoTransportador = repPerfilAcessoTransportador.BuscarPorCodigo(codigo, true);

                // Valida
                if (perfilAcessoTransportador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                // Preenche entidade com dados
                PreencheEntidade(ref perfilAcessoTransportador, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(perfilAcessoTransportador, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repPerfilAcessoTransportador.Atualizar(perfilAcessoTransportador, Auditado);

                // Remove formulários
                List<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario> formulariosCadastrados = repPerfilTransportadorFormulario.BuscarPorPerfil(perfilAcessoTransportador.Codigo);
                foreach (Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario formularioCadastrado in formulariosCadastrados)
                    repPerfilTransportadorFormulario.Deletar(formularioCadastrado);

                // Preenche Formulário
                PreencheFormularios(perfilAcessoTransportador, unitOfWork);

                // Persiste dados
                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();

                // Inicia transacao
                unitOfWork.Start();

                //-- Atualiza informações dos usuários
                #region Atualiza Informações
                // Busca formulários da configuracao
                List<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario> formulariosPerfil = repPerfilTransportadorFormulario.BuscarPorPerfil(perfilAcessoTransportador.Codigo);

                // Atualiza todos as empresas com esse perfil
                List<Dominio.Entidades.Empresa> empresas = repEmpresa.BuscarPorPerfilEmbarcador(perfilAcessoTransportador.Codigo);
                foreach (Dominio.Entidades.Empresa empresa in empresas)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Atualizou o perfil de acesso " + perfilAcessoTransportador.Descricao + " .", unitOfWork);
                    // Atualiza modulos
                    empresa.ModulosLiberados.Clear();
                    foreach (int codigoModulo in perfilAcessoTransportador.ModulosLiberados)
                        empresa.ModulosLiberados.Add(codigoModulo);

                    // Apaga Deleta
                    List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> formulariosCadastradosTransportador = repTransportadorFormulario.BuscarPorEmpresa(empresa.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario formulario in formulariosCadastradosTransportador)
                        repTransportadorFormulario.Deletar(formulario);

                    // Atualiza Formulários
                    foreach (Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario formulario in formulariosPerfil)
                    {
                        Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario transportadorFormulario = new Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario();
                        transportadorFormulario.Empresa = empresa;
                        transportadorFormulario.SomenteLeitura = formulario.SomenteLeitura;
                        transportadorFormulario.CodigoFormulario = formulario.CodigoFormulario;

                        repTransportadorFormulario.Inserir(transportadorFormulario);

                        // Atualiza Permissões personalizadas
                        if (formulario.FormularioPermissaoPersonalizada != null)
                        {
                            foreach (Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada permissao in formulario.FormularioPermissaoPersonalizada)
                            {
                                Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada transportadorFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada();
                                transportadorFormularioPermissaoPersonalizada.CodigoPermissao = permissao.CodigoPermissao;
                                transportadorFormularioPermissaoPersonalizada.TransportadorFormulario = transportadorFormulario;

                                repTransportadorFormularioPermissaoPersonalizada.Inserir(transportadorFormularioPermissaoPersonalizada);
                            }
                        }
                    }

                    repEmpresa.Atualizar(empresa);
                }
                #endregion

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAtualizarDados);
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
                // Instancia repositorios
                Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador repPerfilAcessoTransportador = new Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador(unitOfWork);
                Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario repPerfilTransportadorFormulario = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfilAcessoTransportador = repPerfilAcessoTransportador.BuscarPorCodigo(codigo);

                // Valida
                if (perfilAcessoTransportador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                // Formata retorno
                var retorno = new
                {
                    perfilAcessoTransportador.Codigo,
                    perfilAcessoTransportador.Descricao,
                    perfilAcessoTransportador.PerfilAdministrador,
                    Status = perfilAcessoTransportador.Ativo,
                    FormulariosPerfil = (from obj in repPerfilTransportadorFormulario.BuscarPorPerfil(perfilAcessoTransportador.Codigo)
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
                    ModulosPerfil = (from codigoModulo in perfilAcessoTransportador.ModulosLiberados
                                     select new
                                     {
                                         CodigoModulo = codigoModulo
                                     }).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
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
                Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador repPerfilAcessoTransportador = new Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador(unitOfWork);
                Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario repPerfilTransportadorFormulario = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario(unitOfWork);
                Repositorio.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada repPerfilTransportadorFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfilAcessoTransportador = repPerfilAcessoTransportador.BuscarPorCodigo(codigo);

                // Valida
                if (perfilAcessoTransportador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                // Remove Formularios
                List<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario> formularios = repPerfilTransportadorFormulario.BuscarPorPerfil(perfilAcessoTransportador.Codigo);
                for (var i = 0; i < formularios.Count; i++)
                    repPerfilTransportadorFormulario.Deletar(formularios[i]);

                // Persiste dados
                repPerfilAcessoTransportador.Deletar(perfilAcessoTransportador, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoEmOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfilAcessoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Converte valores
            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            bool ativo = false;
            bool.TryParse(Request.Params("Status"), out ativo);

            bool perfilAdministrador = false;
            bool.TryParse(Request.Params("PerfilAdministrador"), out perfilAdministrador);

            // Vincula dados
            perfilAcessoTransportador.Descricao = descricao;
            perfilAcessoTransportador.Ativo = ativo;
            perfilAcessoTransportador.PerfilAdministrador = perfilAdministrador;
        }

        private void PreencheFormularios(Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfilAcessoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheFormularios
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Repositorios
            Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario repPerfilFormulario = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario(unitOfWork);
            Repositorio.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada repPerfilTransportadorFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada(unitOfWork);

            // Vincula Módulos
            perfilAcessoTransportador.ModulosLiberados = new List<int>();
            var jModulosPerfil = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosPerfil"));
            foreach (var jModulo in jModulosPerfil)
            {
                perfilAcessoTransportador.ModulosLiberados.Add((int)jModulo.CodigoModulo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcessoTransportador, null, Localization.Resources.Transportadores.Transportador.Modulo + " " + Localization.Resources.Transportadores.Transportador.AdicionadoAoPerfil + (string)jModulo.CodigoModulo + " .", unitOfWork);
            }

            // Vincula Formulário
            var jFormulariosPerfil = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FormulariosPerfil"));
            foreach (var jFormulario in jFormulariosPerfil)
            {
                Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario perfilTransportadorFormulario = new Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario();
                perfilTransportadorFormulario.SomenteLeitura = (bool)jFormulario.SomenteLeitura;
                perfilTransportadorFormulario.CodigoFormulario = (int)jFormulario.CodigoFormulario;
                perfilTransportadorFormulario.PerfilAcessoTransportador = perfilAcessoTransportador;

                repPerfilFormulario.Inserir(perfilTransportadorFormulario, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcessoTransportador, null, Localization.Resources.Transportadores.Transportador.Formulario + " " + perfilTransportadorFormulario.CodigoFormulario + " " + Localization.Resources.Transportadores.Transportador.AdicionadoAoPerfil, unitOfWork);

                // Vincula Permissões personalizadas (caso tenha)
                foreach (var dynPermissao in jFormulario.PermissoesPersonalizadas)
                {
                    Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada perfilTransportadorFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada();

                    perfilTransportadorFormularioPermissaoPersonalizada.CodigoPermissao = (int)dynPermissao.CodigoPermissaoPersonalizada;
                    perfilTransportadorFormularioPermissaoPersonalizada.PerfilTransportadorFormulario = perfilTransportadorFormulario;

                    repPerfilTransportadorFormularioPermissaoPersonalizada.Inserir(perfilTransportadorFormularioPermissaoPersonalizada, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcessoTransportador, null, Localization.Resources.Transportadores.Transportador.Permissao + " " + perfilTransportadorFormularioPermissaoPersonalizada.CodigoPermissao + " " + Localization.Resources.Transportadores.Transportador.AdicionadoAoPerfil, unitOfWork);
                }
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfilAcessoTransportador, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";


            if (string.IsNullOrWhiteSpace(perfilAcessoTransportador.Descricao))
            {
                msgErro = Localization.Resources.Transportadores.Transportador.DescricaoObrigatoria;
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador repPerfilPermissao = new Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Ativo")))
                Enum.TryParse(Request.Params("Ativo"), out status);
            else
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador> listaGrid = repPerfilPermissao.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
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
