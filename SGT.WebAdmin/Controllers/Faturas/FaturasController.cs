using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Faturas
{
    public class FaturasController : BaseController
    {
		#region Construtores

		public FaturasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Faturas/Fatura")]
        public async Task<IActionResult> Fatura()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                var confFinanceiro = new
                {
                    configuracaoFinanceiro.ExigirInformarFilialEmissaoFaturas,
                    configuracaoFinanceiro.AtivarColunaNumeroContainerConsultaDocumentosFatura,
                    configuracaoGeral.HabilitarFuncionalidadesProjetoGollum,
                };
                ViewBag.ConfiguracoesFinanceiro = Newtonsoft.Json.JsonConvert.SerializeObject(confFinanceiro);
                return View();
            }
        }

        [CustomAuthorize("Faturas/Justificativa")]
        public async Task<IActionResult> Justificativa()
        {
            return View();
        }

        [CustomAuthorize("Faturas/FaturaLote")]
        public async Task<IActionResult> FaturaLote()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/FaturaLote");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Faturas/FaturaCancelamentoLote")]
        public async Task<IActionResult> FaturaCancelamentoLote()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/FaturaCancelamentoLote");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Faturas/RegraAutorizacaoFatura")]
        public async Task<IActionResult> RegraAutorizacaoFatura()
        {
            return View();
        }


        [CustomAuthorize("Faturas/AutorizacaoFatura")]
        public async Task<IActionResult> AutorizacaoFatura()
        {
            return View();
        }

    }
}
