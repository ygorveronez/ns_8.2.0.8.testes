using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Localidades
{
    [CustomAuthorize("Localidades/Estado")]
    public class EstadoController : BaseController
    {
		#region Construtores

		public EstadoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                int regiao = Request.GetIntParam("Regiao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Estado.Sigla, "Codigo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Descricao")
                    propOrdena = "Nome";
                else if (propOrdena == "Codigo")
                    propOrdena = "Sigla";


                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                List<Dominio.Entidades.Estado> estados = repEstado.Consultar(descricao, regiao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, "");

                grid.setarQuantidadeTotal(repEstado.ContarConsulta(descricao, regiao, ""));

                var lista = (from obj in estados
                             select new
                             {
                                 Codigo = obj.Sigla,
                                 Descricao = obj.Nome
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCadastro()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string sigla = Request.GetStringParam("Codigo");
                string nome = Request.GetStringParam("Nome");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho(Localization.Resources.Consultas.Estado.Sigla, "Sigla", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Codigo", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                List<Dominio.Entidades.Estado> estados = repEstado.Consultar(nome, 0, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, sigla);

                grid.setarQuantidadeTotal(repEstado.ContarConsulta(nome, 0, sigla));

                var lista = (from obj in estados
                             select new
                             {
                                 Sigla = obj.Sigla,
                                 Nome = obj.Nome,
                                 Codigo = obj.Sigla,
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                PreencherEstado(unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorSigla()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string sigla = Request.GetStringParam("Codigo");

                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(sigla);

                var dynEstado = new
                {
                    estado.Sigla,
                    estado.CodigoEstado,
                    estado.Nome,
                    DataAtualizacao = estado.DataAtualizacao.HasValue ? estado.DataAtualizacao.Value.ToString("dd/MM/yyyy") : "",
                    Pais = estado.Pais != null ? new { estado.Pais.Codigo, estado.Pais.Descricao } : null,
                    Status = estado.Status.Equals("A"),
                    estado.CodigoIBGE,
                    estado.TipoEmissao,
                    SefazCTe = estado.SefazCTe != null ? new { estado.SefazCTe.Codigo, estado.SefazCTe.Descricao } : null,
                    SefazCTeHomologacao = estado.SefazCTeHomologacao != null ? new { estado.SefazCTeHomologacao.Codigo, estado.SefazCTeHomologacao.Descricao } : null,
                    SefazMDFe = estado.SefazMDFe != null ? new { estado.SefazMDFe.Codigo, estado.SefazMDFe.Descricao } : null,
                    SefazMDFeHomologacao = estado.SefazMDFeHomologacao != null ? new { estado.SefazMDFeHomologacao.Codigo, estado.SefazMDFeHomologacao.Descricao } : null,
                    estado.Abreviacao,
                    RegiaoBrasil = estado.RegiaoBrasil != null ? new { estado.RegiaoBrasil.Codigo, estado.RegiaoBrasil.Descricao } : null
                };

                return new JsonpResult(dynEstado);
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
        public async Task<IActionResult> PesquisaRegiaoBrasil()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                int regiao = Request.GetIntParam("Regiao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Descricao")
                    propOrdena = "Nome";
                else if (propOrdena == "Codigo")
                    propOrdena = "Sigla";


                Repositorio.Embarcador.Localidades.RegiaoBrasil repEstado = new Repositorio.Embarcador.Localidades.RegiaoBrasil(unitOfWork);

                List<Dominio.Entidades.Embarcador.Localidades.RegiaoBrasil> estados = repEstado.BuscarTodos();

                grid.setarQuantidadeTotal(estados.Count);

                var lista = (from obj in estados
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 Descricao = obj.Descricao
                             }).ToList();

                grid.AdicionaRows(lista);

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

        private void PreencherEstado(Repositorio.UnitOfWork unitOfWork)
        {
            string codigo = Request.GetStringParam("Codigo");

            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(codigo);

            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
            Dominio.Entidades.Pais pais = repPais.BuscarPorCodigo(Request.GetIntParam("Pais"));

            Repositorio.Sefaz repSefaz = new Repositorio.Sefaz(unitOfWork);
            Dominio.Entidades.Sefaz sefazCte = repSefaz.BuscarPorCodigo(Request.GetIntParam("SefazCte"));
            Dominio.Entidades.Sefaz sefazCteHlg = repSefaz.BuscarPorCodigo(Request.GetIntParam("SefazCteHomologacao"));
            Dominio.Entidades.Sefaz sefazMDFe = repSefaz.BuscarPorCodigo(Request.GetIntParam("SefazMdfe"));
            Dominio.Entidades.Sefaz sefazMDFeHlg = repSefaz.BuscarPorCodigo(Request.GetIntParam("SefazMdfeHomologacao"));

            Repositorio.Embarcador.Localidades.RegiaoBrasil repRegiao = new Repositorio.Embarcador.Localidades.RegiaoBrasil(unitOfWork);
            Dominio.Entidades.Embarcador.Localidades.RegiaoBrasil regiaoBrasil = repRegiao.BuscarPorCodigo(Request.GetIntParam("RegiaoBrasil"));

            estado.Nome = Request.GetStringParam("Nome");
            estado.CodigoEstado = Request.GetStringParam("CodigoEstado");
            estado.DataAtualizacao = DateTime.Now;
            estado.CodigoIBGE = Request.GetIntParam("CodigoIBGE");
            estado.Status = Request.GetBoolParam("Status") ? "A" : "I";
            estado.Abreviacao = Request.GetStringParam("Abreviacao");
            estado.Pais = pais;
            estado.TipoEmissao = Request.GetStringParam("TipoEmissao");
            estado.SefazCTe = sefazCte;
            estado.SefazCTeHomologacao = sefazCteHlg;
            estado.SefazMDFe = sefazMDFe;
            estado.SefazMDFeHomologacao = sefazMDFeHlg;
            estado.RegiaoBrasil = regiaoBrasil;

            repEstado.Atualizar(estado, Auditado);
        }
    }
}
