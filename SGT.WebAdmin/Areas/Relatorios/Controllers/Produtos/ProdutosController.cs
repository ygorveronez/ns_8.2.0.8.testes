using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Produtos
{
	[Area("Relatorios")]
	public class ProdutosController : Controller
    {
        [CustomAuthorize("Relatorios/Produtos/Produto")]
        public async Task<IActionResult> Produto()
        {
            return View();
        }
        
        [CustomAuthorize("Relatorios/Produtos/ProdutoEmbarcador")]
        public async Task<IActionResult> ProdutoEmbarcador()
        {
            return View();
        }
    }
}