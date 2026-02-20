using SGTAdmin.Controllers;
using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Ajuda
{
    [CustomAuthorize("Ajuda/Ajuda")]
    public class AjudaController : BaseController
    {
		#region Construtores

		public AjudaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarAjuda()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                string issue = Request.Params("issue");

               string responseString = Servicos.Embarcador.Integracao.Redmine.Issue.BuscarConteudoTarefaPorId(issue, unitOfWork);

                return new JsonpResult(responseString);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a ajuda.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> PoliticaPrivacidade(string id)
        {
            string caminhoBaseViews = "~/Views/Ajuda/";
            string idPadrao = "15376";

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                MontaLayoutBase(unitOfWork);

                id = Utilidades.String.OnlyNumbers(id);
                if (string.IsNullOrWhiteSpace(id))
                    id = idPadrao;

                string responseString = Servicos.Embarcador.Integracao.Redmine.Issue.BuscarConteudoTarefaPorId(id, unitOfWorkAdmin);
                dynamic objRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);
                dynamic objIssue = objRetorno?.issue;

                string assunto = objIssue?.subject ?? "Política de Privacidade";
                string conteudo = objIssue?.description ?? string.Empty;

                ViewBag.Assunto = assunto;
                ViewBag.Conteudo = ConverterMarkDownEmHtml(conteudo);

                return View(caminhoBaseViews + "PoliticaPrivacidade/Detalhe.cshtml", caminhoBaseViews + "PoliticaPrivacidade.cshtml");
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View(caminhoBaseViews + "PoliticaPrivacidade/Erro.cshtml", caminhoBaseViews + "PoliticaPrivacidade.cshtml");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }


        private void MontaLayoutBase(Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminMultisoftwareUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminMultisoftwareUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                adminMultisoftwareUnitOfWork.Dispose();
            }
        }


        private string ConverterMarkDownEmHtml(string conteudo)
        {
            string html = conteudo.Replace(Environment.NewLine, "</br>");
            
            Regex rxNegrito = new Regex(@"(?:\*\*)(.*?)(?:\*\*)");
            Regex rxItalico = new Regex(@"(?:\*)(.*?)(?:\*)");

            html = Regex.Replace(html, rxNegrito.ToString(), "<strong>$1</strong>");
            html = Regex.Replace(html, rxItalico.ToString(), "<i>$1</i>");

            return html;
        }
    }
}
