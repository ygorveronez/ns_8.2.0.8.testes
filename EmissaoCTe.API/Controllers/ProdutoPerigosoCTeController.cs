using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ProdutoPerigosoCTeController : ApiController
    {
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.ProdutoPerigosoCTE repProdutoPerigosoCTe = new Repositorio.ProdutoPerigosoCTE(unitOfWork);
                    List<Dominio.Entidades.ProdutoPerigosoCTE> produtosPerigosos = repProdutoPerigosoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    var retorno = from obj in produtosPerigosos
                                  select new Dominio.ObjetosDeValor.ProdutoPerigoso
                                  {
                                      ClasseRisco = obj.ClasseRisco,
                                      Codigo = obj.Codigo,
                                      Excluir = false,
                                      GrupoEmbalagem = obj.Grupo,
                                      NomeApropriado = obj.NomeApropriado,
                                      NumeroONU = obj.NumeroONU,
                                      PontoDeFulgor = obj.PontoFulgor,
                                      QuantidadeETipo = obj.Volumes,
                                      QuantidadeTotal = obj.Quantidade
                                  };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Código do CT-e inválido.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os produtos perigosos do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
