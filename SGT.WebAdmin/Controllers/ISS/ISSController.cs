using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ISS
{
    public class ISSController : BaseController
    {
		#region Construtores

		public ISSController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("ISS/AliquotaISS")]
        public async Task<IActionResult> AliquotaISS()
        {
            return View();
        }
    }
}
