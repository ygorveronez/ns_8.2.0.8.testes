using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    public class DocumentosController : BaseController
    {
		#region Construtores

		public DocumentosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Documentos/CampoCCe")]
        public async Task<IActionResult> CampoCCe()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                ViewBag.AtivarGeracaoCCePelaRolagemWS = Newtonsoft.Json.JsonConvert.SerializeObject(integracaoIntercab?.AtivarGeracaoCCePelaRolagemWS ?? false);

                return View();
            }
        }

        [CustomAuthorize("Documentos/ModeloDocumentoFiscal")]
        public async Task<IActionResult> ModeloDocumentoFiscal()
        {
            return View();
        }

        [CustomAuthorize("Documentos/EspecieDocumentoFiscal")]
        public async Task<IActionResult> EspecieDocumentoFiscal()
        {
            return View();
        }

        [CustomAuthorize("Documentos/DocumentoDestinadoEmpresa")]
        public async Task<IActionResult> DocumentoDestinadoEmpresa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Documentos/DocumentoDestinadoEmpresa");
                bool existeMotivo = repMotivoDesacordo.ExisteCadastrado();

                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

                ViewBag.Configuracoes = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    ExisteMotivoDesacordo = existeMotivo
                });


                return View();
            }
        }

        [CustomAuthorize("Documentos/CIOT")]
        public async Task<IActionResult> CIOT()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Documentos/CIOT");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Documentos/CIOT")]
        public async Task<IActionResult> CIOTCTe()
        {
            return View();
        }

        [CustomAuthorize("Documentos/GestaoDocumento")]
        public async Task<IActionResult> GestaoDocumento()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Documentos/GestaoDocumento");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Documentos/ControleDocumento")]
        public async Task<IActionResult> ControleDocumento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Documentos/ControleDocumento");

                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
                ViewBag.InformacoesUsuario = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Setor = this.Usuario?.Setor?.Descricao ?? string.Empty
                });

                return View();
            }
        }

        [CustomAuthorize("Documentos/AutorizacaoPagamentoCIOT")]
        public async Task<IActionResult> AutorizacaoPagamentoCIOT()
        {
            return View();
        }

        [CustomAuthorize("Documentos/TrackingDocumentacao")]
        public async Task<IActionResult> TrackingDocumentacao()
        {
            return View();
        }

        [CustomAuthorize("Documentos/GestaoNotasFiscais")]
        public async Task<IActionResult> GestaoNotasFiscais()
        {
            return View();
        }

        [CustomAuthorize("Documentos/RegraAutorizacaoGestaoDocumentos")]
        public async Task<IActionResult> RegraAutorizacaoGestaoDocumentos()
        {
            return View();
        }

        [CustomAuthorize("Documentos/DocumentacaoAFRMM")]
        public async Task<IActionResult> DocumentacaoAFRMM()
        {
            return View();
        }

        [CustomAuthorize("Documentos/AutorizacaoPagamentoCIOTParcela")]
        public async Task<IActionResult> AutorizacaoPagamentoCIOTParcela()
        {
            return View();
        }
    }
}
