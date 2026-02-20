using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Formularios
{
    public class FormulariosController : BaseController
    {
		#region Construtores

		public FormulariosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Formularios/FormularioFavorito")]
        public async Task<IActionResult> FormularioFavorito()
        {
            return View();
        }
    }
}
