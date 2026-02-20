using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ResponsavelCarga
{
    [CustomAuthorize("Cargas/ResponsavelCarga")]
    public class ResponsavelCargaController : BaseController
    {
		#region Construtores

		public ResponsavelCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaResponsavel cargaResponsavel = new Dominio.Entidades.Embarcador.Cargas.CargaResponsavel();

                PreencherCargaResponsavel(cargaResponsavel, unitOfWork);

                unitOfWork.Start();

                AdicionarOuAtualizarFiliais(cargaResponsavel, unitOfWork);

                new Repositorio.Embarcador.Cargas.CargaResponsavel(unitOfWork).Inserir(cargaResponsavel, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaResponsavel repositorio = new Repositorio.Embarcador.Cargas.CargaResponsavel(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaResponsavel cargaResponsavel = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (cargaResponsavel == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherCargaResponsavel(cargaResponsavel, unitOfWork);

                unitOfWork.Start();

                AdicionarOuAtualizarFiliais(cargaResponsavel, unitOfWork);

                repositorio.Atualizar(cargaResponsavel, Auditado);

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
                Repositorio.Embarcador.Cargas.CargaResponsavel repositorio = new Repositorio.Embarcador.Cargas.CargaResponsavel(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaResponsavel cargaResponsavel = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (cargaResponsavel == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Dados = new
                    {
                        cargaResponsavel.Codigo,
                        CategoriaResponsavel = new { cargaResponsavel.CategoriaResponsavel.Codigo, cargaResponsavel.CategoriaResponsavel.Descricao },
                        Funcionario = new { cargaResponsavel.Funcionario.Codigo, cargaResponsavel.Funcionario.Descricao },
                        VigenciaFinal = cargaResponsavel.VigenciaFinal?.ToString("dd/MM/yyyy"),
                        VigenciaInicial = cargaResponsavel.VigenciaInicio?.ToString("dd/MM/yyyy")
                    },
                    Filiais = (
                        from Filial in cargaResponsavel.Filiais
                        select new
                        {
                            Filial.Codigo,
                            Filial.Descricao
                        }
                    ).ToList()
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
                Repositorio.Embarcador.Cargas.CargaResponsavel repositorio = new Repositorio.Embarcador.Cargas.CargaResponsavel(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaResponsavel cargaResponsavel = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (cargaResponsavel == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(cargaResponsavel, Auditado);

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

        private void AdicionarOuAtualizarFiliais(Dominio.Entidades.Embarcador.Cargas.CargaResponsavel cargaResponsavel, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic filiais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            ExcluirFiliaisRemovidas(cargaResponsavel, filiais, unitOfWork);
            InserirFiliaisAdicionadas(cargaResponsavel, filiais, unitOfWork);
        }

        private void ExcluirFiliaisRemovidas(Dominio.Entidades.Embarcador.Cargas.CargaResponsavel cargaResponsavel, dynamic filiais, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaResponsavel.Filiais?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var filial in filiais)
                    listaCodigosAtualizados.Add(((string)filial.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFilialRemover = (from filial in cargaResponsavel.Filiais where !listaCodigosAtualizados.Contains(filial.Codigo) select filial).ToList();

                foreach (var filial in listaFilialRemover)
                    cargaResponsavel.Filiais.Remove(filial);

                if (listaFilialRemover.Count > 0)
                {
                    string descricaoAcao = listaFilialRemover.Count == 1 ? "Filial removida" : "Múltiplas filiais removidas";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaResponsavel, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirFiliaisAdicionadas(Dominio.Entidades.Embarcador.Cargas.CargaResponsavel cargaResponsavel, dynamic filiais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            int totalFiliaisAdicionadas = 0;

            if (cargaResponsavel.Filiais == null)
                cargaResponsavel.Filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            foreach (var filial in filiais)
            {
                int codigo = ((string)filial.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Filiais.Filial filialAdicionar = repositorioFilial.BuscarPorCodigo(codigo) ?? throw new ControllerException("Filial não encontrada");

                if (!cargaResponsavel.Filiais.Contains(filialAdicionar))
                {
                    cargaResponsavel.Filiais.Add(filialAdicionar);

                    totalFiliaisAdicionadas++;
                }
            }

            if (cargaResponsavel.IsInitialized() && (totalFiliaisAdicionadas > 0))
            {
                string descricaoAcao = totalFiliaisAdicionadas == 1 ? "Filial adicionada" : "Múltiplas filiais adicionadas";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaResponsavel, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel ObterCategoriaResponsavel(Repositorio.UnitOfWork unitOfWork)
        {
            return new Repositorio.Embarcador.Logistica.CategoriaResponsavel(unitOfWork).BuscarPorCodigo(Request.GetIntParam("CategoriaResponsavel")) ?? throw new ControllerException("Categoria de responsável não encontrada.");
        }

        private Dominio.Entidades.Usuario ObterFuncionario(Repositorio.UnitOfWork unitOfWork)
        {
            return new Repositorio.Usuario(unitOfWork).BuscarPorCodigo(Request.GetIntParam("Funcionario")) ?? throw new ControllerException("Funcionário não encontrado.");
        }

        private void PreencherCargaResponsavel(Dominio.Entidades.Embarcador.Cargas.CargaResponsavel cargaResponsavel, Repositorio.UnitOfWork unitOfWork)
        {
            cargaResponsavel.CategoriaResponsavel = ObterCategoriaResponsavel(unitOfWork);
            cargaResponsavel.Funcionario = ObterFuncionario(unitOfWork);
            cargaResponsavel.VigenciaFinal = Request.GetNullableDateTimeParam("VigenciaFinal");
            cargaResponsavel.VigenciaInicio = Request.GetNullableDateTimeParam("VigenciaInicial");
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaResponsavel ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaResponsavel()
            {
                CodigoCategoriaResponsavel = Request.GetIntParam("CategoriaResponsavel"),
                CodigoFuncionario = Request.GetIntParam("Funcionario"),
                DataVigenciaFinal = Request.GetNullableDateTimeParam("VigenciaFinal"),
                DataVigenciaInicial = Request.GetNullableDateTimeParam("VigenciaInicial")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Funcionario", "Funcionario", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Categoria Responsável", "CategoriaResponsavel", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vigência Inicial", "VigenciaInicio", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Vigência Final", "VigenciaFinal", 15, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaResponsavel filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.CargaResponsavel repositorio = new Repositorio.Embarcador.Cargas.CargaResponsavel(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel> listaCargaResponsavel = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel>();

                var listaCargaResponsavelRetornar = (
                    from cargaResponsavel in listaCargaResponsavel
                    select new
                    {
                        cargaResponsavel.Codigo,
                        Funcionario = cargaResponsavel.Funcionario.Descricao,
                        CategoriaResponsavel = cargaResponsavel.CategoriaResponsavel.Descricao,
                        VigenciaFinal = cargaResponsavel.VigenciaFinal?.ToString("dd/MM/yyyy"),
                        VigenciaInicio = cargaResponsavel.VigenciaInicio?.ToString("dd/MM/yyyy")
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaResponsavelRetornar);
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
            if (propriedadeOrdenar == "Funcionario")
                return "Funcionario.Nome";

            if (propriedadeOrdenar == "CategoriaResponsavel")
                return "CategoriaResponsavel.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
