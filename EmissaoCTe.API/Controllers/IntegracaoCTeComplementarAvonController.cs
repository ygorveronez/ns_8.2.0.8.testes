using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoCTeComplementarAvonController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("integracaoctecomplementaravon.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        public ActionResult AdicionarCTeAoManifesto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe, codigoManifesto;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoManifesto"], out codigoManifesto);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoCTe);
                Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorCodigo(codigoManifesto, this.EmpresaUsuario.Codigo);

                if (cte == null)
                    return Json<bool>(false, false, "O CT-e não foi encontrado.");

                if (manifesto == null)
                    return Json<bool>(false, false, "O Manifesto não foi encontrado.");

                if (manifesto.Faturas.Count() > 0)
                    return Json<bool>(false, false, "O Manifesto já está vinculado a uma fatura.");

                //if (cte.Status != "A")
                //    return Json<bool>(false, false, "O status do CT-e não permite que ele seja vinculado à um manifesto.");

                Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                Dominio.Entidades.DocumentoManifestoAvon documento = repDocumento.BuscarPorCTe(cte.Codigo);

                if (documento != null)
                    return Json<bool>(false, false, "O CT-e já está vinculado ao manifesto " + documento.Manifesto.Numero + ".");

                Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);
                List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);

                documento = new Dominio.Entidades.DocumentoManifestoAvon();

                documento.CTe = cte;
                documento.Documento = string.Empty;
                documento.Manifesto = manifesto;
                documento.Numero = (from obj in documentosCTe where !string.IsNullOrWhiteSpace(obj.Numero) select int.Parse(obj.Numero)).FirstOrDefault();
                documento.Serie = (from obj in documentosCTe where !string.IsNullOrWhiteSpace(obj.SerieOuSerieDaChave) select int.Parse(obj.SerieOuSerieDaChave)).FirstOrDefault();

                if (manifesto.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Manual)
                    documento.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Finalizado;
                else
                    documento.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Emitido;

                documento.ValorFrete = documento.CTe.ValorFrete;

                repDocumento.Inserir(documento);

                if (manifesto.TipoIntegradora != Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Manual)
                {
                    if (manifesto.Status == Dominio.Enumeradores.StatusManifestoAvon.Finalizado)
                    {
                        manifesto.Status = Dominio.Enumeradores.StatusManifestoAvon.Emitido;

                        repManifesto.Atualizar(manifesto);
                    }
                }
                else
                {
                    manifesto.ValorAReceber += cte.ValorAReceber;
                    manifesto.ValorFrete += cte.ValorFrete;
                    repManifesto.Atualizar(manifesto);
                }
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao vincular o CT-e ao manifesto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
