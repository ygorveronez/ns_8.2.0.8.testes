using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/IdentificacaoMercadoriaKrona")]
    public class IdentificacaoMercadoriaKronaController : BaseController
    {
		#region Construtores

		public IdentificacaoMercadoriaKronaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona repositorio = new Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona identificacaoMercadoriaKrona = new Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona();

                PreencherIdentificacaoMercadoriaKrona(identificacaoMercadoriaKrona);
                repositorio.Inserir(identificacaoMercadoriaKrona, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona repositorio = new Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona identificacaoMercadoriaKrona = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (identificacaoMercadoriaKrona == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherIdentificacaoMercadoriaKrona(identificacaoMercadoriaKrona);
                repositorio.Atualizar(identificacaoMercadoriaKrona, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona repositorio = new Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona identificacaoMercadoriaKrona = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (identificacaoMercadoriaKrona == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    identificacaoMercadoriaKrona.Codigo,
                    identificacaoMercadoriaKrona.IdentificadorDescricao,
                    identificacaoMercadoriaKrona.Identificador,
                    Status = identificacaoMercadoriaKrona.Ativo
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona repositorio = new Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona identificacaoMercadoriaKrona = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (identificacaoMercadoriaKrona == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(identificacaoMercadoriaKrona, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherIdentificacaoMercadoriaKrona(Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona identificacaoMercadoriaKrona)
        {
            identificacaoMercadoriaKrona.Ativo = Request.GetBoolParam("Status");
            identificacaoMercadoriaKrona.Identificador = Request.GetIntParam("Identificador");
            identificacaoMercadoriaKrona.IdentificadorDescricao = Request.GetStringParam("IdentificadorDescricao");

            if (identificacaoMercadoriaKrona.Identificador <= 0)
                throw new ControllerException("O identificador é obrigatória.");

            if (string.IsNullOrWhiteSpace(identificacaoMercadoriaKrona.IdentificadorDescricao))
                throw new ControllerException("A descrição é obrigatória.");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int identificador = Request.GetIntParam("Identificador");
                string identificadorDescricao = Request.GetStringParam("IdentificadorDescricao");
                SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Identificador", "Identificador", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "IdentificadorDescricao", 45, Models.Grid.Align.left, true);

                if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona repositorio = new Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(identificador, identificadorDescricao, situacaoAtivo);
                List<Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona> listaIdentificacaoMercadoriaKrona = (totalRegistros > 0) ? repositorio.Consultar(identificador, identificadorDescricao, situacaoAtivo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Integracao.IdentificacaoMercadoriaKrona>();

                var listaIdentificacaoMercadoriaKronaRetornar = (
                    from identificacaoMercadoriaKrona in listaIdentificacaoMercadoriaKrona
                    select new
                    {
                        identificacaoMercadoriaKrona.Codigo,
                        identificacaoMercadoriaKrona.Identificador,
                        identificacaoMercadoriaKrona.IdentificadorDescricao,
                        identificacaoMercadoriaKrona.Descricao,
                        identificacaoMercadoriaKrona.DescricaoAtivo,
                    }
                ).ToList();

                grid.AdicionaRows(listaIdentificacaoMercadoriaKronaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
