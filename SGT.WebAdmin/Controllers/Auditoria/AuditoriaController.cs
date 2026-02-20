using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Auditoria
{
    [CustomAuthorize("Auditoria/Auditoria")]
    public class AuditoriaController : BaseController
    {
		#region Construtores

		public AuditoriaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!this.PermiteAuditar())
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.SemPermissaoParaAuditar);

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
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAuditoria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
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
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        
        public async Task<IActionResult> PesquisaComposAlterados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!this.PermiteAuditar())
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.SemPermissaoParaAuditar);

                Models.Grid.Grid grid = GridDetalhes();

                int totalRegistros = 0;
                var lista = ExecutaPesquisaDetalhes(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaBuscarDetalhes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Exportar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridAuditoria();

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridDetalhes();

                int totalRegistros = 0;
                var lista = ExecutaPesquisaDetalhes(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private bool PermiteAuditar()
        {
            bool permite = this.Usuario.PermiteAuditar;
            string entidade = Request.Params("Entidade");
            string[] entidadesExcecao = new string[] { "ContratoFreteTransportador", "PreCarga" };

            if (!permite && entidadesExcecao.Contains(entidade))
                permite = true;

            return permite;
        }

        private string ReplaceDicionario(string prop, dynamic dicionario)
        {
            string propCorreta = "";

            try
            {
                propCorreta = (string)dicionario[prop];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao acessar propriedade do dicionário de auditoria - WebAdmin: {ex.ToString()}", "CatchNoAction");
            }
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
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Auditoria.Propriedade, "Propriedade", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Auditoria.De, "De", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Auditoria.Para, "Para", 35, Models.Grid.Align.left, false);

            return grid;
        }

        private Models.Grid.Grid GridAuditoria()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Auditoria.Data, "Data", 12, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Auditoria.Auditado, "Auditado", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 25, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Auditoria.Acao, "Acao", 40, Models.Grid.Align.left, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Auditoria.HistoricoObjeto repHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

            string entidade = Request.Params("Entidade");
            long.TryParse(Request.Params("Codigo"), out long codigo);
            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && codigo == 0)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            List<Dominio.Entidades.Auditoria.HistoricoObjeto> listaGrid = repHistoricoObjeto.Consultar(codigoEmpresa, codigo, entidade, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repHistoricoObjeto.ContarConsulta(codigoEmpresa, codigo, entidade);

            var lista = from p in listaGrid
                        select new
                        {
                            p.Codigo,
                            Auditado = (this.Usuario.UsuarioDaMultisoftware && !string.IsNullOrWhiteSpace(p.UsuarioMultisoftware)) ? p.UsuarioMultisoftware : p.Auditado,
                            p.Descricao,
                            Data = p.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                            Acao = p.DescricaoAcao
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaDetalhes(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Auditoria.HistoricoObjeto repHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

            long.TryParse(Request.Params("Codigo"), out long codigo);
            dynamic dicionario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Dicionario"));

            Dominio.Entidades.Auditoria.HistoricoObjeto historico = repHistoricoObjeto.BuscarPorCodigo(codigo);
            totalRegistros = historico.Propriedades.Count;

            var lista = (from p in historico.Propriedades.OrderBy(obj => obj.Propriedade).Skip(inicio).Take(limite).ToList()
                         select new
                         {
                             p.Codigo,
                             Propriedade = ReplaceDicionario(p.Propriedade, dicionario),
                             p.De,
                             p.Para
                         });

            return lista.ToList();
        }
    }
}

