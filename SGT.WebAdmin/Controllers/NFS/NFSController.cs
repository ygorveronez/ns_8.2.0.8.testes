using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS
{
    public class NFSController : BaseController
    {
		#region Construtores

		public NFSController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("NFS/InformarNFSPendentes")]
        public async Task<IActionResult> InformarNFSPendentes()
        {
            return View();
        }

        [CustomAuthorize("NFS/ServicoNFSe")]
        public async Task<IActionResult> ServicoNFSe()
        {
            return View();
        }

        [CustomAuthorize("NFS/NaturezaNFSe")]
        public async Task<IActionResult> NaturezaNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaoExistentes = repTipoIntegracao.BuscarTipos();
            var retorno = new
            {
                PossuiIntegracaoMigrate = tipoIntegracaoExistentes.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Migrate)
            };

            ViewBag.ConfiguracaoNFS = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
            return View();
        }

        [CustomAuthorize("NFS/NFSManual")]
        public async Task<IActionResult> NFSManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
            bool possuiResiduais = repTabelaFrete.ExisteTabelaResidual();

            var retorno = new
            {
                configGeral.AlterarModeloDocumentoNFSManual
            };

            ViewBag.ConfiguracaoNFS = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
            ViewBag.PossuiResiduais = possuiResiduais ? "true" : "false";
            return View();
        }

        [CustomAuthorize("NFS/RegrasAutorizacaoNFSManual")]
        public async Task<IActionResult> RegrasAutorizacaoNFSManual()
        {
            return View();
        }

        [CustomAuthorize("NFS/AutorizacaoNFS")]
        public async Task<IActionResult> AutorizacaoNFS()
        {
            return View();
        }

        [CustomAuthorize("NFS/MotivoRejeicaoLancamentoNFS")]
        public async Task<IActionResult> MotivoRejeicaoLancamentoNFS()
        {
            return View();
        }


        [CustomAuthorize("NFS/NFSManualCancelamento")]
        public async Task<IActionResult> NFSManualCancelamento()
        {
            return View();
        }
        
    }
}
