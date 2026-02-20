using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EmissaoEmailController : ApiController
    {
        #region Propriedades

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("emissaoemail.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int numeroCTe = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["NumeroCTe"], out numeroCTe);

                string empresa = Request.Params["Empresa"];

                DateTime data;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out data);

                Repositorio.EmissaoEmail repEmissaoEmail = new Repositorio.EmissaoEmail(unitOfWork);

                List<Dominio.Entidades.EmissaoEmail> listaEmissaoEmail = repEmissaoEmail.Consultar(numeroCTe, data, empresa, inicioRegistros, 50);
                int countCTes = repEmissaoEmail.ContarConsulta(numeroCTe, data, empresa);

                var retorno = from obj in listaEmissaoEmail
                              select new
                              {
                                  Codigo = obj.CTe != null ? obj.CTe.Codigo : obj.NFSe.Codigo,
                                  CodigoEmpresa = obj.CTe != null ? obj.CTe.Empresa.Codigo : obj.NFSe.Empresa.Codigo,
                                  Tipo = obj.CTe != null ? "CT-e" : "NFs-e",
                                  Numero = obj.CTe != null ? obj.CTe.Numero + " / " + obj.CTe.Serie.Numero : obj.NFSe.Numero + " / " + obj.NFSe.Serie.Numero,
                                  DataEmissao = obj.CTe != null ? string.Format("{0:dd/MM/yyyy HH:mm}", obj.CTe.DataEmissao.Value) : obj.NFSe.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                  RazaoSocial = obj.CTe != null ? obj.CTe.Empresa.RazaoSocial : obj.NFSe.Empresa.RazaoSocial,
                                  Remetente = obj.CTe != null ? obj.CTe.Remetente.Nome : obj.NFSe.Tomador.Nome,
                                  DescricaoStatus = obj.CTe != null ? obj.CTe.DescricaoStatus : obj.NFSe.DescricaoStatus,
                                  RetornoSefaz = obj.CTe != null ? obj.CTe.MensagemStatus != null ? obj.CTe.MensagemStatus.MensagemDoErro : obj.CTe.MensagemRetornoSefaz : obj.NFSe.RPS != null ? obj.NFSe.RPS.MensagemRetorno : string.Empty,
                                  ValorFrete = obj.CTe != null ? obj.CTe.ValorFrete.ToString("n2") : obj.NFSe.ValorServicos.ToString("n2")
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "CodigoEmpresa", "Tipo|4", "Núm.|6", "Emissão|10", "Empresa|19", "Remetente|10", "Status|10", "Retorno Sefaz|20", "Valor|10" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os emissões do e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}