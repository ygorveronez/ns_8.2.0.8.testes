using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoCTeRecebimentoController : ApiController
    {
        #region Propriedades

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("integracaocterecebimento.aspx") select obj).FirstOrDefault();
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
                string usuario = Request.Params["Usuario"];

                DateTime data;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out data);

                Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe statusAux;
                Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe? status = null;
                if (Enum.TryParse<Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                Repositorio.IntegracaoCTeRecebimento repIntegracaoCTeRecebimento = new Repositorio.IntegracaoCTeRecebimento(unitOfWork);

                List<Dominio.Entidades.IntegracaoCTeRecebimento> listaCTes = repIntegracaoCTeRecebimento.Consultar(status, numeroCTe, data, empresa, usuario, inicioRegistros, 50);
                int countCTes = repIntegracaoCTeRecebimento.ContarConsulta(status, numeroCTe, data, empresa, usuario);

                var retorno = (from obj in listaCTes
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   CodigoCTe = obj.CTe.Codigo,
                                   CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                   Usuario = obj.Usuario != null ? obj.Usuario.Nome : string.Empty,
                                   Transportador = obj.CTe.Empresa.RazaoSocial + " - " + obj.CTe.Empresa.Localidade.Descricao + "/" + obj.CTe.Empresa.Localidade.Estado.Sigla,
                                   Numero = obj.CTe.Numero + " / " + obj.CTe.Serie.Numero,
                                   DataEmissao = string.Format("{0:dd/MM/yyyy HH:mm}", obj.CTe.DataEmissao.Value),
                                   StatusCTe = obj.CTe.DescricaoStatus,
                                   TipoCTe = obj.CTe.TipoCTE.ToString(),
                                   Veiculo = obj.CTe.Veiculos.Count > 0 ? obj.CTe.Veiculos.FirstOrDefault().Placa : string.Empty,
                                   Remetente = obj.CTe.Remetente.Nome + " - " + obj.CTe.Remetente.Localidade?.Descricao + " / " + obj.CTe.Remetente.Localidade?.Estado.Sigla,
                                   Destinatario = obj.CTe.Destinatario.Nome + " - " + obj.CTe.Destinatario.Localidade?.Descricao + "/" + obj.CTe.Destinatario.Localidade?.Estado.Sigla,
                                   ValorMercadoria = obj.CTe.ValorTotalMercadoria.ToString("n2"),
                                   ValorReceber = obj.CTe.ValorAReceber.ToString("n2"),
                                   CST = obj.CTe.CST != null ? obj.CTe.CST : string.Empty,
                                   Status = obj.Status == Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe.Pendente ? "Pendente" : obj.Status == Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe.Aprovado ? "Aprovado" : "Reprovado",
                                   Log = obj.Log != null ? obj.Log : string.Empty
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "CodigoCTe", "CodigoEmpresa", "Usuário|6", "Transportador|15", "Núm.|5", "Emissão|6", "Status CT-e|5", "Tipo|5", "Placa|5", "Remetente|8", "Destinatário|8", "Valor Mercadoria|4", "Valor Receber|4", "CST|3", "Status|4", "Log|4" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os CTes integrados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.IntegracaoCTeRecebimento repIntegracaoCTeRecebimento = new Repositorio.IntegracaoCTeRecebimento(unitOfWork);
                Dominio.Entidades.IntegracaoCTeRecebimento integracaoCTe = repIntegracaoCTeRecebimento.BuscarPorCodigo(codigo);

                if (integracaoCTe == null)
                    return Json<bool>(false, false, "Integração não encontrada.");

                if (integracaoCTe.Usuario == null)
                    return Json<bool>(false, false, "É necessário Alocar antes de Aprovar.");

                if (integracaoCTe.Usuario != null && integracaoCTe.Usuario != this.Usuario)
                    return Json<bool>(false, false, "CT-e está alocado para "+integracaoCTe.Usuario.Nome);

                integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe.Aprovado;
                integracaoCTe.Log = String.Concat(string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now), " Aprovado por ", this.Usuario.Nome, integracaoCTe.Log != null ? " / " + integracaoCTe.Log : string.Empty);

                repIntegracaoCTeRecebimento.Atualizar(integracaoCTe);

                return Json(integracaoCTe.Codigo, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao Aprovar Integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Reprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.IntegracaoCTeRecebimento repIntegracaoCTeRecebimento = new Repositorio.IntegracaoCTeRecebimento(unitOfWork);
                Dominio.Entidades.IntegracaoCTeRecebimento integracaoCTe = repIntegracaoCTeRecebimento.BuscarPorCodigo(codigo);

                if (integracaoCTe == null)
                    return Json<bool>(false, false, "Integração não encontrada.");

                if (integracaoCTe.Usuario == null)
                    return Json<bool>(false, false, "É necessário Alocar antes de Reprovar.");

                if (integracaoCTe.Usuario != null && integracaoCTe.Usuario != this.Usuario)
                    return Json<bool>(false, false, "CT-e está alocado para " + integracaoCTe.Usuario.Nome);

                integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe.Reprovado;
                integracaoCTe.Log = String.Concat(string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now), " Reprovado por ", this.Usuario.Nome, integracaoCTe.Log != null ? " / " + integracaoCTe.Log : string.Empty);

                repIntegracaoCTeRecebimento.Atualizar(integracaoCTe);

                return Json(integracaoCTe.Codigo, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao Reprovar Integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Alocar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.IntegracaoCTeRecebimento repIntegracaoCTeRecebimento = new Repositorio.IntegracaoCTeRecebimento(unitOfWork);
                Dominio.Entidades.IntegracaoCTeRecebimento integracaoCTe = repIntegracaoCTeRecebimento.BuscarPorCodigo(codigo);

                if (integracaoCTe == null)
                    return Json<bool>(false, false, "Integração não encontrada.");

                if (integracaoCTe.Usuario != null && integracaoCTe.Usuario != this.Usuario)
                    return Json<bool>(false, false, "CT-e está alocado para " + integracaoCTe.Usuario.Nome);

                integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe.Alocado;
                integracaoCTe.Usuario = this.Usuario;
                integracaoCTe.Log = String.Concat(string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now), " Alocado por ", this.Usuario.Nome, integracaoCTe.Log != null ? " / " + integracaoCTe.Log : string.Empty);

                repIntegracaoCTeRecebimento.Atualizar(integracaoCTe);

                return Json(integracaoCTe.Codigo, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao Alocar Integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
