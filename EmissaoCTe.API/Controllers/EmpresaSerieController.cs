using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EmpresaSerieController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("series.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoEmpresa, inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                List<Dominio.Entidades.EmpresaSerie> listaSeries = repEmpresaSerie.BuscarTodosAtivosInativos(codigoEmpresa, inicioRegistros, 50);
                int countSeries = repEmpresaSerie.ContarTodosAtivosInativos(codigoEmpresa);

                var retorno = from obj in listaSeries select new { obj.Tipo, Codigo = obj.Codigo.ToString(), Numero = obj.Numero.ToString(), obj.DescricaoTipo, obj.Status };
                return Json(retorno, true, null, new string[] {"Tipo", "Codigo", "Número|10", "Tipo|70", "Status|10" }, countSeries);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as séries.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarSeriesEmpresa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);

                List<Dominio.Entidades.EmpresaSerie> listaSeries = repEmpresaSerie.BuscarTodos(this.EmpresaUsuario.Codigo, 0, 1000);

                var retorno = from obj in listaSeries select new { Codigo = obj.Codigo, Numero = obj.Numero.ToString() };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as séries.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarSeriesDoUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

                List<Dominio.Entidades.EmpresaSerie> listaSeries = null;

                if (this.Usuario.Series != null && this.Usuario.Series.Count() > 0)
                    listaSeries = this.Usuario.Series.ToList();
                else
                    listaSeries = repEmpresaSerie.BuscarTodos(this.EmpresaUsuario.Codigo, 0, 1000);

                var retorno = from obj in listaSeries select new { Numero = obj.Numero.ToString() };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as séries.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int serie, inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["Serie"], out serie);

                string status = Request.Params["Status"];

                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

                List<Dominio.Entidades.EmpresaSerie> listaSeries = repEmpresaSerie.Consultar(this.EmpresaUsuario.Codigo, serie, status, inicioRegistros, 50);

                int countSeries = repEmpresaSerie.ContarConsulta(this.EmpresaUsuario.Codigo, serie, status);

                var retorno = from obj in listaSeries select new { obj.Codigo, Numero = obj.Numero.ToString(), obj.DescricaoTipo };

                return Json(retorno, true, null, new string[] { "Codigo", "Número|20", "Tipo|60" }, countSeries);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as séries.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorTipo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int serie, inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["Serie"], out serie);

                string status = Request.Params["Status"];

                Dominio.Enumeradores.TipoSerie tipo;
                Enum.TryParse(Request.Params["Tipo"], out tipo);

                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

                List<Dominio.Entidades.EmpresaSerie> listaSeries = repEmpresaSerie.ConsultarPorTipo(this.EmpresaUsuario.Codigo, serie, status, tipo, inicioRegistros, 50);

                int countSeries = repEmpresaSerie.ContarConsultaPorTipo(this.EmpresaUsuario.Codigo, serie, status, tipo);

                var retorno = from obj in listaSeries select new { obj.Codigo, Numero = obj.Numero.ToString() };

                return Json(retorno, true, null, new string[] { "Codigo", "Número|80" }, countSeries);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as séries.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int serie, inicioRegistros, codigoEmpresa = 0;

                if (!int.TryParse(Servicos.Criptografia.Descriptografar(Request.Params["CodigoEmpresa"], "CT3##MULT1@#$S0FTW4R3"), out codigoEmpresa))
                    return Json<bool>(false, false, "Código da empresa inválido.");

                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["Serie"], out serie);
                string status = Request.Params["Status"];

                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                List<Dominio.Entidades.EmpresaSerie> listaSeries = repEmpresaSerie.Consultar(codigoEmpresa, serie, status, inicioRegistros, 50);
                int countSeries = repEmpresaSerie.ContarConsulta(codigoEmpresa, serie, status);

                var retorno = from obj in listaSeries select new { obj.Codigo, obj.Tipo, Numero = obj.Numero.ToString(), obj.DescricaoTipo };
                return Json(retorno, true, null, new string[] { "Codigo", "Tipo", "Número|20", "Tipo|60" }, countSeries);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as séries.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão de série negada!");

                int codigoEmpresa, serie = 0;
                Dominio.Enumeradores.TipoSerie tipo;

                string status = Request.Params["Status"];

                if (!int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa) || codigoEmpresa <= 0)
                    return Json<bool>(false, false, "Código da empresa inválido.");

                if (!int.TryParse(Request.Params["Serie"], out serie) || (serie < 0 && serie > 900))
                    return Json<bool>(false, false, "Série inválida.");

                if (!Enum.TryParse<Dominio.Enumeradores.TipoSerie>(Request.Params["Tipo"], out tipo))
                    return Json<bool>(false, false, "Tipo da Série inválido.");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (empresa == null)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Dominio.Entidades.EmpresaSerie serieEmpresa = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serie, tipo);

                if (serieEmpresa == null)
                    serieEmpresa = new Dominio.Entidades.EmpresaSerie();

                serieEmpresa.Empresa = empresa;
                serieEmpresa.Numero = serie;

                if (serieEmpresa.Codigo <= 0)
                    serieEmpresa.Tipo = tipo;

                if (this.Permissao() != null || this.Permissao().PermissaoDeDelecao == "A")
                    serieEmpresa.Status = status;

                if (serieEmpresa.Codigo == 0)
                    repEmpresaSerie.Inserir(serieEmpresa);
                else
                    repEmpresaSerie.Atualizar(serieEmpresa);

                return Json<bool>(true, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a série!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
