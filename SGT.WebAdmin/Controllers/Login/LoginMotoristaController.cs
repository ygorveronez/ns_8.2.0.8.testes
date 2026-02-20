using Dominio.Excecoes.Embarcador;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Login
{
    public class LoginMotoristaController : Controller
    {
		#region Propriedades

		private readonly Conexao _conexao;

		#endregion

		#region Construtores

		public LoginMotoristaController(Conexao conexao)
		{
			_conexao = conexao;
		}

		#endregion

		#region Métodos Públicos

		[AllowAnonymous]
        public async Task<IActionResult> IndexMotorista()
        {
            using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao))
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repositorioClienteUrlAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repositorioClienteUrlAcesso.BuscarPorURL(_conexao.ObterHost);

                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, new Repositorio.UnitOfWork(_conexao.StringConexao));
            }

            string caminhoBaseViews = "~/Views/Login";

            Models.AcessoMotorista viewModel = new Models.AcessoMotorista()
            {
                Acesso = "",
                Pager = "",
                ExibirNumeroPager = false
            };

            return View($"{caminhoBaseViews}/IndexMotorista.cshtml", viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IndexMotorista(Models.AcessoMotorista viewModel)
        {
            string caminhoBaseViews = "~/Views/Login";
            string valorAcesso = viewModel.Acesso;

            bool autenticacaoValida = false;          

            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {                
                using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao))
                {
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repositorioClienteUrlAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminUnitOfWork);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repositorioClienteUrlAcesso.BuscarPorURL(_conexao.ObterHost);

                    Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
                }

                Servicos.Embarcador.Integracao.Agraria.IntegracaoAgraria integracaoAgraria = new Servicos.Embarcador.Integracao.Agraria.IntegracaoAgraria(unitOfWork);
                bool isOrdemEmbarque = integracaoAgraria.IsOrdemEmbarque(valorAcesso);

                if (isOrdemEmbarque)
                {
                    string viewRetornar = "IndexMotoristaConfirmacaoIdentificacao.cshtml";

                    try
                    {
                        viewModel.DadosCarga = integracaoAgraria.ObterDadosCarga(valorAcesso);
                    }
                    catch
                    {
                        ModelState.AddModelError("DadosCarga", "Não foi possível obter os dados da carga.");
                        viewRetornar = "IndexMotorista.cshtml";
                    }

                    return View($"{caminhoBaseViews}/{viewRetornar}", viewModel);
                }

                if (autenticacaoValida)
                    return Redirect("/#Home");
                else
                {
                    ModelState.AddModelError("Acesso", "Falha na autenticação do motorista.");
                    return View($"{caminhoBaseViews}/IndexMotorista.cshtml", viewModel);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarDados(Models.AcessoMotorista viewModel)
        {
            string imagemBase64 = viewModel.ImagemBase64.Split(',')[1];
            
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Login.LoginMotorista servicoLoginMotorista = new Servicos.Embarcador.Login.LoginMotorista(unitOfWork);
                try
                {
                    servicoLoginMotorista.AutenticarMotorista(imagemBase64, viewModel.DadosCarga.CodigoCarga);
                }
                catch(ServicoException ex)
                {

                }
                catch (Exception ex)
                {

                }
            }
            
            return View();
        }

        #endregion

    }
}