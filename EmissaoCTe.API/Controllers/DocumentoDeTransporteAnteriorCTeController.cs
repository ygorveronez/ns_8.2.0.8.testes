using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DocumentoDeTransporteAnteriorCTeController : ApiController
    {
        public ActionResult ObterDocumentosEletronicosPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                    List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentos = repDocumentoAnterior.BuscarDocumentosEletronicosPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    var retorno = from obj in documentos
                                  select new Dominio.ObjetosDeValor.DocumentoDeTransporteAnterior
                                  {
                                      Chave = obj.Chave,
                                      Codigo = obj.Codigo,
                                      Emissor = obj.Emissor.Tipo.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.Emissor.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.Emissor.CPF_CNPJ),
                                      NomeEmissor = obj.Emissor.Nome,
                                      Excluir = false
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
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os documentos de transporte anteriores eletrônicos do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult ObterDocumentosPapelPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                    List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentos = repDocumentoAnterior.BuscarDocumentosPapelPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    var retorno = from obj in documentos
                                  select new Dominio.ObjetosDeValor.DocumentoDeTransporteAnterior
                                  {
                                      Codigo = obj.Codigo,
                                      Emissor = obj.Emissor.Tipo.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.Emissor.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.Emissor.CPF_CNPJ),
                                      NomeEmissor = obj.Emissor.Nome,
                                      DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                      DescricaoTipo = obj.DescricaoTipo,
                                      Tipo = obj.Tipo,
                                      Numero = obj.Numero,
                                      Serie = obj.Serie,
                                      Excluir = false
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
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os documentos de transporte anteriores eletrônicos do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
