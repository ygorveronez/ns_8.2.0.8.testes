using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ImportacaoPreCTeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("importacaodeprecte.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                int inicioRegistros, numeroInicial, numeroFinal = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                Dominio.Enumeradores.TipoCTE finalidade = Dominio.Enumeradores.TipoCTE.Todos;
                Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["Finalidade"], out finalidade);

                DateTime dataInicial, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string empresa = Request.Params["Empresa"];
                string status = Request.Params["Status"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.ObservacaoContribuinteCTE repObservacao = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarParaImportacaoDePreCTe(this.EmpresaUsuario.Codigo, empresa, numeroInicial, numeroFinal, dataInicial, dataFinal, status, finalidade, inicioRegistros, 50);
                int countCTes = repCTe.ContarConsultaParaImportacaoDePreCTe(this.EmpresaUsuario.Codigo, empresa, numeroInicial, numeroFinal, dataInicial, dataFinal, status, finalidade);

                var retorno = new List<object>();

                for (var i = 0; i < listaCTes.Count(); i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = listaCTes[i];
                    
                    List<Dominio.Entidades.ObservacaoContribuinteCTE> listaObs = repObservacao.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                    retorno.Add(new
                    {
                        cte.Codigo,
                        CodigoEmpresa = cte.Empresa.Codigo,
                        cte.Empresa.NomeFantasia,
                        NumeroPreCTe = listaObs.Count() > 0 ? (from obj in listaObs where obj.Identificador.Equals("precte") select obj.Descricao).FirstOrDefault() : "Não Informado",
                        cte.Numero,
                        Serie = cte.Serie.Numero,
                        cte.DataEmissao,
                        Remetente = cte.Remetente != null ? cte.Remetente.Nome : string.Empty,
                        LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Exterior ? string.Concat(cte.Remetente.Cidade, " / ", cte.Remetente.Pais.Nome) : string.Concat(cte.Remetente.Localidade.Estado.Sigla, " / ", cte.Remetente.Localidade.Descricao) : string.Empty,
                        Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                        LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Exterior ? string.Concat(cte.Destinatario.Cidade, " / ", cte.Destinatario.Pais.Nome) : string.Concat(cte.Destinatario.Localidade.Estado.Sigla, " / ", cte.Destinatario.Localidade.Descricao) : string.Empty,
                        Valor = string.Format("{0:n2}", cte.ValorFrete),
                        cte.DescricaoStatus,
                        MensagemRetornoSefaz = cte.MensagemStatus == null ? string.IsNullOrEmpty(cte.MensagemRetornoSefaz) ? string.Empty : System.Web.HttpUtility.HtmlEncode(cte.MensagemRetornoSefaz) : cte.MensagemStatus.MensagemDoErro
                    });
                }

                return Json(retorno, true, null, new string[] { "Codigo", "CodigoEmpresa", "Empresa|11", "Pré CT-e|5", "Núm.|5", "Sér.|3", "Dt. Emi.|6", "Remetente|11", "Loc. Remet.|9", "Destinatário|11", "Loc. Destin.|9", "Valor|5", "Status|8", "Retorno Sefaz|10" }, countCTes);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os pré CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult FinalizarImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return Json<bool>(false, false, "CT-e não encontrado.");

                if (cte.Status != "A")
                    return Json<bool>(false, false, "O status do CT-e não permite que ele seja finalizado.");

                this.TransferirXMLCTe(cte);

                cte.StatusImportacaoPreCTe = Dominio.Enumeradores.StatusImportacaoPreCTe.Finalizada;
                repCTe.Atualizar(cte);

                return Json<bool>(true, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao finalizar a importação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void TransferirXMLCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            UnitOfWork unitOfWork = new UnitOfWork(Conexao.StringConexao);
            try
            { 
                string caminhoTransferencia = System.Configuration.ConfigurationManager.AppSettings["CaminhoTransferenciaPreCTes"];
                if (!string.IsNullOrWhiteSpace(caminhoTransferencia))
                {
                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                    byte[] arquivoXML = svcCTe.ObterXMLAutorizacao(cte);

                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(caminhoTransferencia, string.Concat(cte.Chave, ".xml")), arquivoXML);

                    svcCTe = null;
                    arquivoXML = null;
                }
                else
                {
                    throw new Exception("Diretório não configurado para a transferência do XML.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

    }


}
