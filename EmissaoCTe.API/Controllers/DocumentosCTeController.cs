using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DocumentosCTeController : ApiController
    {
        #region Métodos Publicos

        public DocumentosCTeController()
        {
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.DocumentosCTE repositorioDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                    List<Dominio.Entidades.DocumentosCTE> documentos = repositorioDocumentosCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    IEnumerable<object> retorno;
                    var modelo = (from obj in documentos select obj.ModeloDocumentoFiscal).FirstOrDefault();
                    if (modelo != null && (modelo.Numero == "01" || modelo.Numero == "04"))
                    {
                        retorno = from obj in documentos
                                  select new
                                  {
                                      BaseCalculoICMS = obj.BaseCalculoICMS,
                                      BaseCalculoICMSST = obj.BaseCalculoICMSST,
                                      CFOP = obj.CFOP,
                                      Codigo = obj.Codigo,
                                      DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                      Modelo = obj.ModeloDocumentoFiscal.Codigo,
                                      NumeroModelo = obj.ModeloDocumentoFiscal.Numero,
                                      Numero = obj.Numero,
                                      Peso = obj.Peso,
                                      PIN = obj.PINSuframa,
                                      Serie = obj.Serie,
                                      ValorICMS = obj.ValorICMS,
                                      ValorICMSST = obj.ValorICMSST,
                                      ValorProdutos = obj.ValorProdutos,
                                      ValorTotal = obj.Valor,
                                      Excluir = false
                                  };
                    }
                    else if (modelo != null && (modelo.Numero == "00" || modelo.Numero == "99"))
                    {
                        retorno = from obj in documentos
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                      DestinatarioUF = obj.DestinatarioUF,
                                      RemetenteUF = obj.RemetenteUF,
                                      Modelo = obj.ModeloDocumentoFiscal.Codigo,
                                      DescricaoModelo = string.Concat(obj.ModeloDocumentoFiscal.Numero, " - ", obj.ModeloDocumentoFiscal.Descricao),
                                      NumeroModelo = obj.ModeloDocumentoFiscal.Numero,
                                      Numero = obj.Numero,
                                      ValorTotal = obj.Valor,
                                      Descricao = obj.Descricao,
                                      Excluir = false
                                  };
                    }
                    else
                    {
                        retorno = from obj in documentos
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      Chave = obj.ChaveNFE,
                                      Remetente = obj.CNPJRemetente,
                                      DestinatarioUF = obj.DestinatarioUF,
                                      RemetenteUF = obj.RemetenteUF,
                                      DataEmissao = string.Format("{0:dd/MM/yyyy}", obj.DataEmissao),
                                      obj.Numero,
                                      ValorTotal = obj.Valor,
                                      Peso = obj.Peso,
                                      Excluir = false
                                  };
                    }
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Parâmetros inválidos.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os documentos do remetente do CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult VerificarSeJaUtilizouNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string chaveNFe = Request.Params["ChaveNFe"];
                if (string.IsNullOrWhiteSpace(chaveNFe) || chaveNFe.Length != 44)
                    return Json<bool>(false, false, "Chave da NF-e inválida.");
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
                if (!serDocumento.ValidarChave(chaveNFe))
                    return Json<bool>(false, false, "Chave da NF-e informada está inválida.");

                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                List<string> numerosCTeUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(this.EmpresaUsuario.Codigo, chaveNFe, codigoCTe); //repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(this.EmpresaUsuario.Codigo, chaveNFe, codigoCTe);

                var retorno = new { NumerosCTeUtilizados = numerosCTeUtilizados };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Não foi possível verificar se a nota selecionada já foi utilizada em outro CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult VerificarSeJaOutrosDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string numeroDocumento = Request.Params["Numero"];
                if (string.IsNullOrWhiteSpace(numeroDocumento))
                    return Json<bool>(false, false, "Número do documeto invalido da NF-e inválida.");
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                IList<int> numerosCTeUtilizados = repDocumentosCTe.BuscarNumeroDoCTePorOutrosDocumentosEEmpresa(this.EmpresaUsuario.Codigo, numeroDocumento, codigoCTe);

                var retorno = new { NumerosCTeUtilizados = numerosCTeUtilizados };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Não foi possível verificar se documento selecionado já foi utilizado em outro CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPesoTotalPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                decimal peso = repDocumentosCTe.BuscarPesoPorCTe(codigoCTe, this.EmpresaUsuario.Codigo);
                return Json(peso, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Não foi possível obter o peso total do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
