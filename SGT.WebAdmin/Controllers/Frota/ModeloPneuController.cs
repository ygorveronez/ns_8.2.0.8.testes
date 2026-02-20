using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/ModeloPneu")]
    public class ModeloPneuController : BaseController
    {
		#region Construtores

		public ModeloPneuController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu = new Dominio.Entidades.Embarcador.Frota.ModeloPneu();

                try
                {
                    PreencherModeloPneu(modeloPneu, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frota.ModeloPneu repositorio = new Repositorio.Embarcador.Frota.ModeloPneu(unitOfWork);

                repositorio.Inserir(modeloPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.ModeloPneu repositorio = new Repositorio.Embarcador.Frota.ModeloPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (modeloPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherModeloPneu(modeloPneu, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(modeloPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                Repositorio.Embarcador.Frota.ModeloPneu repositorio = new Repositorio.Embarcador.Frota.ModeloPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu = repositorio.BuscarPorCodigo(codigo);

                if (modeloPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    modeloPneu.Codigo,
                    modeloPneu.Descricao,
                    Dimensao = new { modeloPneu.Dimensao.Codigo, modeloPneu.Dimensao.Descricao },
                    Marca = new { modeloPneu.Marca.Codigo, modeloPneu.Marca.Descricao },
                    Status = modeloPneu.Ativo
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
                Repositorio.Embarcador.Frota.ModeloPneu repositorio = new Repositorio.Embarcador.Frota.ModeloPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (modeloPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(modeloPneu, Auditado);

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
                var grid = ObterGridPesquisa();

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

        private Dominio.Entidades.Embarcador.Frota.DimensaoPneu ObterDimensaoPneu(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoDimensao = Request.GetIntParam("Dimensao");
            Repositorio.Embarcador.Frota.DimensaoPneu repositorio = new Repositorio.Embarcador.Frota.DimensaoPneu(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoDimensao) ?? throw new ControllerException("Dimensão não encontrada");
        }

        private Dominio.Entidades.Embarcador.Frota.MarcaPneu ObterMarcaPneu(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoMarca = Request.GetIntParam("Marca");
            Repositorio.Embarcador.Frota.MarcaPneu repositorio = new Repositorio.Embarcador.Frota.MarcaPneu(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoMarca) ?? throw new ControllerException("Marca não encontrada");
        }

        private void PreencherModeloPneu(Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.GetStringParam("Descricao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            modeloPneu.Descricao = descricao;
            modeloPneu.Ativo = Request.GetBoolParam("Status");
            modeloPneu.Dimensao = ObterDimensaoPneu(unitOfWork);
            modeloPneu.Marca = ObterMarcaPneu(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                modeloPneu.Empresa = this.Usuario.Empresa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaModeloPneu filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaModeloPneu()
                {
                    CodigoDimensao = Request.GetIntParam("Dimensao"),
                    CodigoMarca = Request.GetIntParam("Marca"),
                    Descricao = Request.GetStringParam("Descricao"),
                    SituacaoAtivo = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Código","Codigo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Marca", "Marca", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Dimensão", "Dimensao", 25, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 12, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.ModeloPneu repositorio = new Repositorio.Embarcador.Frota.ModeloPneu(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.ModeloPneu> listaModeloPneu = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                var listaModeloPneuRetornar = (
                    from modeloPneu in listaModeloPneu
                    select new
                    {
                        modeloPneu.Codigo,
                        modeloPneu.Descricao,
                        modeloPneu.DescricaoAtivo,
                        Dimensao = modeloPneu.Dimensao.Descricao,
                        Marca = modeloPneu.Marca.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaModeloPneuRetornar);
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

            if (propriedadeOrdenar == "Dimensao")
                return "Dimensao.Descricao";

            if (propriedadeOrdenar == "Marca")
                return "Marca.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
