using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MunicipioCarregamentoMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unitOfWork);
                List<Dominio.Entidades.MunicipioCarregamentoMDFe> municipiosCarregamento = repMunicipioCarregamento.BuscarPorMDFe(codigoMDFe);

                var retorno = from obj in municipiosCarregamento
                              select new Dominio.ObjetosDeValor.MunicipioCarregamentoMDFe()
                              {
                                  Codigo = obj.Codigo,
                                  CodigoMunicipio = obj.Municipio.Codigo,
                                  DescricaoMunicipio = obj.Municipio.Descricao ?? string.Empty,
                                  Excluir = false
                              };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os municípios de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
