using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Chamados
{
    public class ChamadosController : BaseController
    {
		#region Construtores

		public ChamadosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Chamados/MotivoChamado")]
        public async Task<IActionResult> MotivoChamado()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Chamados.GrupoMotivoChamado repGrupoMotivoChamado = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(unitOfWork);

                var retorno = new
                {
                    PossuiIntegracaoJJ = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.JJ),
                };
                var config = new
                {
                    ExisteGrupoMotivoChamado = repGrupoMotivoChamado.BuscarPrimeiroRegistro() != null
                };

                ViewBag.Integracoes = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
                ViewBag.Configuracoes = Newtonsoft.Json.JsonConvert.SerializeObject(config);

                return View();
            }
        }

        [CustomAuthorize("Chamados/RegrasAnaliseChamados")]
        public async Task<IActionResult> RegrasAnaliseChamados()
        {
            return View();
        }

        [CustomAuthorize("Chamados/RegrasAtendimentoChamado")]
        public async Task<IActionResult> RegrasAtendimentoChamado()
        {
            return View();
        }

        [CustomAuthorize("Chamados/ChamadoOcorrencia")]
        public async Task<IActionResult> ChamadoOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCarga = ObterPermissoesPersonalizadas("Cargas/Carga");
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");

            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            bool permissaoDelegar = false;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                permissaoDelegar = UsuarioPossuiPermissao("Chamados/RegrasAnaliseChamados");

            ViewBag.PermissaoDelegar = permissaoDelegar ? "true" : "false";
            ViewBag.PermissoesPersonalizadasChamado = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasCarga);
            ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);
            ViewBag.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoOcorrencia.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos);

            return View();
        }

        [CustomAuthorize("Chamados/ChamadoTMS")]
        public async Task<IActionResult> ChamadoTMS()
        {
            return View();
        }

        [CustomAuthorize("Chamados/ControleChamadoTMS")]
        public async Task<IActionResult> ControleChamadoTMS()
        {
            return View();
        }

        [CustomAuthorize("Chamados/MotivoRecusaCancelamento")]
        public async Task<IActionResult> MotivoRecusaCancelamento()
        {
            return View();
        }

        [CustomAuthorize("Chamados/ConfiguracaoTempoChamado")]
        public async Task<IActionResult> ConfiguracaoTempoChamado()
        {
            return View();
        }

		[CustomAuthorize("Chamados/GeneroMotivoChamado")]
		public async Task<IActionResult> GeneroMotivoChamado()
		{
			return View();
		}

		[CustomAuthorize("Chamados/AreaEnvolvidaMotivoChamado")]
		public async Task<IActionResult> AreaEnvolvidaMotivoChamado()
		{
			return View();
		}

        [CustomAuthorize("Chamados/LoteChamadoOcorrencia")]
        public async Task<IActionResult> LoteChamadoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Chamados/GrupoMotivoChamado")]
        public async Task<IActionResult> GrupoMotivoChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoes = repTipoIntegracao.BuscarTipos();

            var retorno = new
            {
                IntegracoesAtivas = (from tipo in tiposIntegracoes
                                     select new
                                     {
                                         value = (int)tipo,
                                         text = tipo.ObterDescricao()
                                     }).ToList(),
                ExisteIntegracaoUnilever = tiposIntegracoes.Any(x => x == TipoIntegracao.Unilever)
            };

            ViewBag.ConfiguracaoInterface = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

            return View();
        }
    }
}
