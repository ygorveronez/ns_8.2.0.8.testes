using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ValePedagioMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ValePedagioMDFe repValePedagio = new Repositorio.ValePedagioMDFe(unitOfWork);
                List<Dominio.Entidades.ValePedagioMDFe> valesPedagio = repValePedagio.BuscarPorMDFe(codigoMDFe);

                var retorno = from obj in valesPedagio
                              select new Dominio.ObjetosDeValor.ValePedagioMDFe()
                              {
                                  CNPJFornecedor = obj.CNPJFornecedor,
                                  CNPJResponsavel = obj.CNPJResponsavel,
                                  Codigo = obj.Codigo,
                                  CodigoAgendamentoPorto = obj.CodigoAgendamentoPorto,
                                  NumeroComprovante = obj.NumeroComprovante,
                                  ValorValePedagio = obj.ValorValePedagio,
                                  QuantidadeEixos = obj.QuantidadeEixos,
                                  TipoCompra = obj.TipoCompra,
                                  Excluir = false
                              };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os vales pedágios.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
