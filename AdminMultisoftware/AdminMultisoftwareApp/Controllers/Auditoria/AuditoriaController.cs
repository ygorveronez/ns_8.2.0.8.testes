using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace AdminMultisoftwareApp.Controllers.Auditoria
{
    public class AuditoriaController : BaseController
    {
        [HttpPost]
        public ActionResult Pesquisa()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridAuditoria();

                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                return new JsonpResult(false, "Ocorreu falha ao consultar Auditoria.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult ConsultarAuditoria()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridAuditoria();

                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                return new JsonpResult(false, "Ocorreu falha ao consultar Auditoria.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        [HttpPost]
        public ActionResult PesquisaComposAlterados()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridDetalhes();

                int totalRegistros = 0;
                var lista = ExecutaPesquisaDetalhes(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, 0, 0, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                return new JsonpResult(false, "Ocorreu falha ao consultar Auditoria.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ReplaceDicionario(string prop, dynamic dicionario)
        {
            string propCorreta = "";

            try
            {
                propCorreta = (string)dicionario[prop];
            }
            catch (Exception) { }
            if (string.IsNullOrWhiteSpace(propCorreta))
                propCorreta = prop;

            return propCorreta;
        }

        private Models.Grid.Grid GridDetalhes()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Propriedade", "Propriedade", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("De", "De", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Para", "Para", 35, Models.Grid.Align.left, false);

            return grid;
        }

        private Models.Grid.Grid GridAuditoria()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 12, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Auditado", "Auditado", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Ação", "Acao", 40, Models.Grid.Align.left, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.Auditoria.HistoricoObjeto repHistoricoObjeto = new AdminMultisoftware.Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

            string entidade = Request.Params["Entidade"];
            long codigo = long.Parse(Request.Params["Codigo"]);

            List<AdminMultisoftware.Dominio.Entidades.Auditoria.HistoricoObjeto> listaGrid = repHistoricoObjeto.Consultar(codigo, entidade, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repHistoricoObjeto.ContarConsulta(codigo, entidade);

            var lista = from p in listaGrid
                        select new
                        {
                            p.Codigo,
                            Auditado = !string.IsNullOrWhiteSpace(p.UsuarioMultisoftware) ? p.UsuarioMultisoftware : p.Auditado,
                            p.Descricao,
                            Data = p.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                            Acao = p.DescricaoAcao
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaDetalhes(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.Auditoria.HistoricoObjeto repHistoricoObjeto = new AdminMultisoftware.Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

            long codigo = long.Parse(Request.Params["Codigo"]);
            dynamic dicionario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params["Dicionario"]);

            AdminMultisoftware.Dominio.Entidades.Auditoria.HistoricoObjeto historico = repHistoricoObjeto.BuscarPorCodigo(codigo);
            totalRegistros = historico.Propriedades.Count;

            var lista = (from p in historico.Propriedades.OrderBy(obj => obj.Propriedade).ToList()
                         select new
                         {
                             p.Codigo,
                             Propriedade = ReplaceDicionario(p.Propriedade, dicionario),
                             De = p.De,
                             Para = p.Para
                         });

            return lista.ToList();
        }

    }
}