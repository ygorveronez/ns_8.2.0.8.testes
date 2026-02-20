using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FaixaEmissaoCTeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult ObterFaixasDoPlano()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoPlano = 0;
                int.TryParse(Request.Params["CodigoPlano"], out codigoPlano);

                Repositorio.FaixaEmissaoCTe repFaixaEmissao = new Repositorio.FaixaEmissaoCTe(unitOfWork);

                List<Dominio.Entidades.FaixaEmissaoCTe> faixas = repFaixaEmissao.BuscarPorPlano(codigoPlano);

                var resultado = from obj in faixas select new Dominio.ObjetosDeValor.FaixaEmissaoCTe() { Codigo = obj.Codigo, Excluir = false, Quantidade = obj.Quantidade, Valor = obj.Valor };

                return Json(resultado, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as faixas de emissão do plano.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
