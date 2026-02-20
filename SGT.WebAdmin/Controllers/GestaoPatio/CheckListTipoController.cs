using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/CheckListTipo")]
    public class CheckListTipoController : BaseController
    {
		#region Construtores

		public CheckListTipoController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListTipo = repCheckListTipo.BuscarPorCodigo(codigo);

                // Valida
                if (checkListTipo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    checkListTipo.Codigo,
                    checkListTipo.Descricao,
                    Status = checkListTipo.Ativo,
                    EnviarEmailParaCliente = checkListTipo.EnviarEmailParaCliente,
                    EnviarEmailParaMotorista = checkListTipo.EnviarEmailParaMotorista,
                    PerfisAcesso = (from perfilAcesso in checkListTipo.PerfisAcesso
                                    select new
                                    {
                                        perfilAcesso.Codigo,
                                        perfilAcesso.Descricao
                                    }).ToList(),
                    Clientes = (from cliente in checkListTipo.Clientes
                                select new
                                {
                                    cliente.Codigo,
                                    cliente.Descricao
                                }).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListTipo = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo();

                // Preenche entidade com dados
                PreencheEntidade(ref checkListTipo, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(checkListTipo, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCheckListTipo.Inserir(checkListTipo, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListTipo = repCheckListTipo.BuscarPorCodigo(codigo);

                // Valida
                if (checkListTipo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref checkListTipo, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(checkListTipo, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCheckListTipo.Atualizar(checkListTipo);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListTipo = repCheckListTipo.BuscarPorCodigo(codigo);

                // Valida
                if (checkListTipo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repCheckListTipo.Deletar(checkListTipo, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Descricao").Nome("Descrição").Tamanho(35).Align(Models.Grid.Align.left);
            grid.Prop("Ativo").Nome("Status").Tamanho(25).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);

            // Dados do filtro
            Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);

            string descricao = Request.Params("Descricao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo> listaGrid = repCheckListTipo.Consultar(descricao, status, codigoEmpresa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCheckListTipo.ContarConsulta(descricao, status, codigoEmpresa);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            Ativo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListTipo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;
            dynamic teste = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PerfisAcesso"));

            Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso;
            checkListTipo.PerfisAcesso = new List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>();
            foreach (int codigo in teste)
            {
                perfilAcesso = repPerfilAcesso.BuscarPorCodigo(codigo);
                checkListTipo.PerfisAcesso.Add(perfilAcesso);
            }

            dynamic Clientes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Clientes"));

            Dominio.Entidades.Cliente cliente;
            checkListTipo.Clientes = new List<Dominio.Entidades.Cliente>();
            foreach (double codigo in Clientes)
            {
                cliente = repCliente.BuscarPorCPFCNPJ(codigo);
                checkListTipo.Clientes.Add(cliente);
            }

            bool.TryParse(Request.Params("Status"), out bool ativo);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Vincula dados
            checkListTipo.Descricao = descricao;
            checkListTipo.Ativo = ativo;
            checkListTipo.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            checkListTipo.EnviarEmailParaCliente = Request.GetBoolParam("EnviarEmailParaCliente");
            checkListTipo.EnviarEmailParaMotorista = Request.GetBoolParam("EnviarEmailParaMotorista");
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListTipo, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(checkListTipo.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
        }
        #endregion
    }
}
