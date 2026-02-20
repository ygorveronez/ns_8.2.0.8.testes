using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Layout
{
    public class MasterLayoutController : BaseController
    {
        #region Métodos Globais

        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.AnoCorrente = DateTime.Now.Year.ToString();
            ViewBag.AcessoModulos = true;
            ViewBag.AcessoFormulario = true;
            ViewBag.AcessoPermissaoPersonalizada = true;
            ViewBag.AcessoUsuario = true;
            ViewBag.AcessoCliente = true;
            ViewBag.AcessoClienteURLAcesso = true;
            ViewBag.AcessoClienteFormulario = true;
            ViewBag.AcessoClienteModulo = true;
            ViewBag.AcessoInstanciaBase = true;
            ViewBag.AcessoMensagensAviso = true;

            if (Session["GrupoRedmine"] != null)
            {
                List<String> grupos = Session["GrupoRedmine"] as List<String>;

                List<String> gruposLiberacaoCadastro = new List<string> { "Head", "Dev" };
                List<String> gruposLiberacaoVinculo = new List<string> { "Head", "Dev", "PO", "GP" };

                ViewBag.AcessoModulos = gruposLiberacaoCadastro.Intersect(grupos).Any();
                ViewBag.AcessoFormulario = gruposLiberacaoCadastro.Intersect(grupos).Any();
                ViewBag.AcessoPermissaoPersonalizada = gruposLiberacaoCadastro.Intersect(grupos).Any();
                ViewBag.AcessoUsuario = false;
                ViewBag.AcessoClienteURLAcesso = false;
                ViewBag.AcessoCliente = gruposLiberacaoCadastro.Intersect(grupos).Any();
                ViewBag.AcessoClienteFormulario = gruposLiberacaoVinculo.Intersect(grupos).Any();
                ViewBag.AcessoClienteModulo = gruposLiberacaoVinculo.Intersect(grupos).Any();
                ViewBag.AcessoInstanciaBase = gruposLiberacaoCadastro.Intersect(grupos).Any();
                ViewBag.AcessoMensagensAviso = gruposLiberacaoVinculo.Intersect(grupos).Any();
            }

            return View();
        }

        #endregion
    }
}