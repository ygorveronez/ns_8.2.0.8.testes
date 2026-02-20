using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ReboqueMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ReboqueMDFe repReboque = new Repositorio.ReboqueMDFe(unitOfWork);
                List<Dominio.Entidades.ReboqueMDFe> reboques = repReboque.BuscarPorMDFe(codigoMDFe);

                var retorno = from obj in reboques
                              select new Dominio.ObjetosDeValor.VeiculoMDFe()
                              {
                                  CapacidadeKG = obj.CapacidadeKG,
                                  CapacidadeM3 = obj.CapacidadeM3,
                                  Codigo = obj.Codigo,
                                  Excluir = false,
                                  Placa = obj.Placa,
                                  RENAVAM = obj.RENAVAM,
                                  RNTRC = obj.RNTRC,
                                  Tara = obj.Tara,
                                  CPFCNPJ = obj.CPFCNPJProprietario,
                                  IE = obj.IEProprietario,
                                  Nome = obj.NomeProprietario,
                                  TipoCarroceria = obj.TipoCarroceria,
                                  TipoProprietario = obj.TipoProprietario,
                                  UF = obj.UF.Sigla,
                                  UFProprietario = obj.UFProprietario != null ? obj.UFProprietario.Sigla : string.Empty,
                              };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os reboques.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
