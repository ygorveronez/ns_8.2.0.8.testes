using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class LocalidadeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consulta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoIBGE, inicioRegistros;
                int.TryParse(Request.Params["IBGE"], out codigoIBGE);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string descricao = Request.Params["Descricao"];
                string UF = Request.Params["UF"];

                bool.TryParse(Request.Params["SomenteEmpresa"], out bool somenteEmpresa);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                var listaLocalidades = repLocalidade.Consulta(descricao, codigoIBGE, inicioRegistros, 50, "", "", UF, somenteEmpresa, this.EmpresaUsuario.Codigo);
                var countLocalidades = repLocalidade.ContarConsulta(descricao, codigoIBGE, UF, somenteEmpresa, this.EmpresaUsuario.Codigo);

                var retorno = from obj in listaLocalidades select new { obj.Codigo, obj.Descricao, UF = obj.Estado.Sigla, obj.CodigoIBGE };

                return Json(retorno, true, null, new string[] { "Codigo", "Cidade|70", "UF|10", "IBGE|10" }, countLocalidades);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as cidades. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorUF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string uf = Request.Params["UF"];
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                var listaLocalidades = repLocalidade.BuscarPorUF(uf, this.EmpresaUsuario.Codigo);
                return Json(from obj in listaLocalidades select new { obj.Codigo, obj.Descricao }, true, null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as cidades. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                var localidade = repLocalidade.BuscarPorCodigo(codigo);
                var retorno = new
                {
                    localidade.Codigo,
                    localidade.Descricao,
                    Estado = localidade.Estado.Nome,
                    UF = localidade.Estado.Sigla
                };
                return Json(retorno, true, null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as cidades. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Controllers.Conexao.StringConexao);
            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                string descricao = Request.Params["Descricao"] ?? string.Empty;

                Dominio.Entidades.Localidade valida = repLocalidade.BuscarPorEmpresaEDescricao(this.EmpresaUsuario.Codigo, descricao);

                if (codigo == 0 && valida != null)
                    return Json<bool>(false, false, "Já existe uma cidade com esse nome");

                Dominio.Entidades.Localidade cidade = repLocalidade.BuscarPorCodigoEmpresa(this.EmpresaUsuario.Codigo, codigo);

                if (codigo == 0 && cidade != null)
                    return Json<bool>(false, false, "Não foi possível encontrar o registro");

                if(cidade == null)
                {
                    Dominio.Entidades.Localidade cidadeBase = repLocalidade.BuscarPorUF("EX", 0).FirstOrDefault();
                    int ultimoCodigo = repLocalidade.BuscarPorMaiorCodigo() + 1;
                    cidade = new Dominio.Entidades.Localidade()
                    {
                        Codigo = ultimoCodigo,
                        Descricao = descricao,
                        Empresa = this.EmpresaUsuario,
                        Estado = cidadeBase.Estado,
                        CEP = cidadeBase.CEP,
                        CodigoIBGE = cidadeBase.CodigoIBGE,
                        Pais = cidadeBase.Pais,
                        Regiao = cidadeBase.Regiao,
                        TipoEmissaoIntramunicipal = cidadeBase.TipoEmissaoIntramunicipal
                    };
                    repLocalidade.Inserir(cidade);
                } 
                else
                {
                    cidade.Descricao = descricao;
                    repLocalidade.Atualizar(cidade);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as cidades. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
