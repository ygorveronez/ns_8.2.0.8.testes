using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    public class FretesController : BaseController
    {
		#region Construtores

		public FretesController(Conexao conexao) : base(conexao) { }

		#endregion


        [CustomAuthorize("Fretes/TabelaFreteComissaoProduto")]
        public async Task<IActionResult> TabelaFreteComissaoProduto()
        {
            return View();
        }

        [CustomAuthorize("Fretes/TabelaFreteComissaoGrupoProduto")]
        public async Task<IActionResult> TabelaFreteComissaoGrupoProduto()
        {
            return View();
        }

        [CustomAuthorize("Fretes/TabelaFreteComissaoImportacao")]
        public async Task<IActionResult> TabelaFreteComissaoImportacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/TabelaFrete")]
        public async Task<IActionResult> TabelaFrete()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao tipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                bool existeIntegracaoLoggi = tipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Loggi);

                var configuracoesAbasTabelaFrete = new
                {
                    ExisteIntegracaoLoggi = existeIntegracaoLoggi,
                };

                ViewBag.ConfiguracoesAbasTabelaFrete = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesAbasTabelaFrete);

                return View();
            }
        }

        [CustomAuthorize("Fretes/SimuladorFrete")]
        public async Task<IActionResult> SimuladorFrete()
        {
            return View();
        }

        [CustomAuthorize("Fretes/TabelaFreteRota")]
        public async Task<IActionResult> TabelaFreteRota()
        {
            return View();
        }


        [CustomAuthorize("Fretes/TabelaFreteTipoOperacao")]
        public async Task<IActionResult> TabelaFreteTipoOperacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/TabelaFreteCliente")]
        public async Task<IActionResult> TabelaFreteCliente()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = reTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);

                var configuracoesTabelaFrete = new
                {
                    NaoBuscarAutomaticamenteVigenciaTabelaFrete = configuracaoTabelaFrete?.NaoBuscarAutomaticamenteVigenciaTabelaFrete ?? false,
                    PossuiIntegracaoLBC = tipoIntegracao?.Ativo ?? false
                };

                ViewBag.ConfiguracoesTabelaFrete = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTabelaFrete);

                return View();
            }
        }

        [CustomAuthorize("Fretes/ComponenteFrete")]
        public async Task<IActionResult> ComponenteFrete()
        {
            return View();
        }

        [CustomAuthorize("Fretes/UnidadeMedida")]
        public async Task<IActionResult> UnidadeMedida()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ConsultaTabelaFrete")]
        public async Task<IActionResult> ConsultaTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

            var configuracoesTabelaFrete = new
            {
                NaoPermiteEdicoesEmValoresNaConsultaDeTabelaFrete = configuracaoTabelaFrete?.NaoPermiteEdicoesEmValoresNaConsultaDeTabelaFrete ?? false
            };
            ViewBag.ConfiguracaoTabelaFrete = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTabelaFrete);

            return View();
        }

        [CustomAuthorize("Fretes/MotivoRejeicaoAjuste")]
        public async Task<IActionResult> MotivoRejeicaoAjuste()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ContratoFreteTransportador")]
        public async Task<IActionResult> ContratoFreteTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Fretes/ContratoFreteTransportador");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = reTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);
            var configuracoesTabelaFrete = new
            {
                PossuiIntegracaoLBC = tipoIntegracao?.Ativo ?? false
            };

            ViewBag.ConfiguracoesContratoFreteTransportador = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTabelaFrete);

            bool permissaoDelegar = true;

            if (!this.Usuario.UsuarioAdministrador)
            {
                Controllers.Modulos controllerModulo = new Controllers.Modulos(_conexao);
                List<Controllers.CacheFormulario> formulariosEmCache = controllerModulo.RetornarFormulariosEmCache();

                IEnumerable<CacheFormulario> formularioBuscar = (from obj in formulariosEmCache where obj != null && obj.CaminhoFormulario == "Fretes/RegraContratoFreteTransportador" select obj);
                if (formularioBuscar.Count() > 0)
                {
                    Controllers.CacheFormulario cacheFormulario = formularioBuscar.FirstOrDefault();
                    if (!this.Usuario.ModulosLiberados.Contains(cacheFormulario.CacheModulo.CodigoModulo))
                        permissaoDelegar = false;

                    if (!permissaoDelegar)
                        permissaoDelegar = controllerModulo.VerificarModulosPaiLiberadoRecursivamente(cacheFormulario.CacheModulo, this.Usuario.ModulosLiberados.ToList());

                    if (!permissaoDelegar)
                    {
                        Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario formularioFuncionario = (from obj in this.Usuario.FormulariosLiberados where obj.CodigoFormulario == cacheFormulario.CodigoFormulario select obj).FirstOrDefault();
                        permissaoDelegar = formularioFuncionario != null;

                        if (permissaoDelegar && formularioFuncionario.SomenteLeitura)
                            permissaoDelegar = false;
                    }
                }
            }

            ViewBag.PermissaoDelegar = permissaoDelegar ? "true" : "false";

            return View();
        }

        [CustomAuthorize("Fretes/AjusteTabelaFrete")]
        public async Task<IActionResult> AjusteTabelaFrete()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                var configuracaoAjuste = new
                {
                    configuracaoEmbarcador.ObrigarVigenciaNoAjusteFrete,
                    configuracaoTabelaFrete.ObrigatorioInformarTransportadorAjusteTabelaFrete,
                    configuracaoTabelaFrete.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete
                };

                ViewBag.ConfiguracaoAjuste = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoAjuste);

                return View();
            }
        }

        [CustomAuthorize("Fretes/MotivoReajuste")]
        public async Task<IActionResult> MotivoReajuste()
        {
            return View();
        }

        [CustomAuthorize("Fretes/MotivoAdicionalFrete")]
        public async Task<IActionResult> MotivoAdicionalFrete()
        {
            return View();
        }

        [CustomAuthorize("Fretes/RegrasAutorizacaoValorFrete")]
        public async Task<IActionResult> RegrasAutorizacaoValorFrete()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                var configuracaoAutorizacao = new
                {
                    configuracaoTabelaFrete.ObrigatorioInformarTransportadorAjusteTabelaFrete,
                    configuracaoTabelaFrete.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete
                };

                ViewBag.ConfiguracaoAutorizacao = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoAutorizacao);

                return View();
            }
        }

        [CustomAuthorize("Fretes/ConsultaReajusteTabelaFrete")]
        public async Task<IActionResult> ConsultaReajusteTabelaFrete()
        {
            return View();
        }

        [CustomAuthorize("Fretes/BonificacaoTransportador")]
        public async Task<IActionResult> BonificacaoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Fretes/RegraContratoFreteTransportador")]
        public async Task<IActionResult> RegraContratoFreteTransportador()
        {
            return View();
        }

        [CustomAuthorize("Fretes/AutorizacaoContratoFreteTransportador")]
        public async Task<IActionResult> AutorizacaoContratoFreteTransportador()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ControleReajusteFretePlanilha")]
        public async Task<IActionResult> ControleReajusteFretePlanilha()
        {
            return View();
        }

        [CustomAuthorize("Fretes/RegraControleReajusteFretePlanilha")]
        public async Task<IActionResult> RegraControleReajusteFretePlanilha()
        {
            return View();
        }

        [CustomAuthorize("Fretes/AutorizacaoControleReajusteFretePlanilha")]
        public async Task<IActionResult> AutorizacaoControleReajusteFretePlanilha()
        {
            return View();
        }

        [CustomAuthorize("Fretes/TipoContratoFrete")]
        public async Task<IActionResult> TipoContratoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfig = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfig.BuscarConfiguracaoPadrao();

            ViewBag.UsarContratoFreteAditivo = config.UsarContratoFreteAditivo ? "true" : "false";

            return View();
        }

        [CustomAuthorize("Fretes/ConfiguracaoNotificacaoContrato")]
        public async Task<IActionResult> ConfiguracaoNotificacaoContrato()
        {
            return View();
        }

        [CustomAuthorize("Fretes/DestinoPrioritarioCalculoFrete")]
        public async Task<IActionResult> DestinoPrioritarioCalculoFrete()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ConfiguracaoDescargaCliente")]
        public async Task<IActionResult> ConfiguracaoDescargaCliente()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                var configuracoes = new
                {
                    configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente
                };

                ViewBag.Configuracoes = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoes);

                return View();
            }
        }

        [CustomAuthorize("Fretes/Licitacao")]
        public async Task<IActionResult> Licitacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/LicitacaoAnexo")]
        public async Task<IActionResult> LicitacaoAnexo()
        {
            return View();
        }

        [CustomAuthorize("Fretes/LicitacaoParticipacao")]
        public async Task<IActionResult> LicitacaoParticipacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/LicitacaoParticipacaoAvaliacao")]
        public async Task<IActionResult> LicitacaoParticipacaoAvaliacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/AgruparTabelaFreteCliente")]
        public async Task<IActionResult> AgruparTabelaFreteCliente()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ExportacaoTabelaFrete")]
        public async Task<IActionResult> ExportacaoTabelaFrete()
        {
            return View();
        }

        [CustomAuthorize("Fretes/RegraAutorizacaoTabelaFrete")]
        public async Task<IActionResult> RegraAutorizacaoTabelaFrete()
        {
            return View();
        }

        [CustomAuthorize("Fretes/AutorizacaoTabelaFrete")]
        public async Task<IActionResult> AutorizacaoTabelaFrete()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarPrimeiroRegistro();

                var configuracoes = new
                {
                    configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente
                };

                ViewBag.Configuracoes = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoes);

                return View();
            }
        }

        [CustomAuthorize("Fretes/RegraAutorizacaoContratoPrestacaoServico")]
        public async Task<IActionResult> RegraAutorizacaoContratoPrestacaoServico()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ContratoPrestacaoServico")]
        public async Task<IActionResult> ContratoPrestacaoServico()
        {
            return View();
        }

        [CustomAuthorize("Fretes/AutorizacaoContratoPrestacaoServico")]
        public async Task<IActionResult> AutorizacaoContratoPrestacaoServico()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ContratoPrestacaoServicoSaldo")]
        public async Task<IActionResult> ContratoPrestacaoServicoSaldo()
        {
            return View();
        }

        [CustomAuthorize("Fretes/TabelaPontuacao")]
        public async Task<IActionResult> TabelaPontuacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/FechamentoPontuacao")]
        public async Task<IActionResult> FechamentoPontuacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/TempoEsperaPorPontuacao")]
        public async Task<IActionResult> TempoEsperaPorPontuacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/MotivoAdvertenciaTransportador")]
        public async Task<IActionResult> MotivoAdvertenciaTransportador()
        {
            return View();
        }

        [CustomAuthorize("Fretes/AdvertenciaTransportador")]
        public async Task<IActionResult> AdvertenciaTransportador()
        {
            return View();
        }

        [CustomAuthorize("Fretes/SolicitacaoLicitacao")]
        public async Task<IActionResult> SolicitacaoLicitacao()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ContratoTransportadorFrete")]
        public async Task<IActionResult> ContratoTransportadorFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfig = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = reTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro config = repConfig.BuscarConfiguracaoPadrao();

            ViewBag.ObrigarAnexosContratoTransportadorFrete = config.ObrigarAnexosContratoTransportadorFrete ? "true" : "false";
            ViewBag.GerarNumeroContratoTransportadorFreteSequencial = config.GerarNumeroContratoTransportadorFreteSequencial ? "true" : "false";
            var configuracoesTabelaFrete = new
            {
                PossuiIntegracaoLBC = tipoIntegracao?.Ativo ?? false
            };

            ViewBag.ConfiguracoesContratoTransportadorFrete = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTabelaFrete);


            return View();
        }

        [CustomAuthorize("Fretes/AprovacaoContratoTransporteFrete")]
        public async Task<IActionResult> AprovacaoContratoTransporteFrete()
        {
            return View();
        }

        [CustomAuthorize("Fretes/RegraAutorizacaoRecusaCheckin")]
        public async Task<IActionResult> RegraAutorizacaoRecusaCheckin()
        {
            return View();
        }

        [CustomAuthorize("Fretes/RegraAutorizacaoTaxaDescarga")]
        public async Task<IActionResult> RegraAutorizacaoTaxaDescarga()
        {
            return View();
        }

        [CustomAuthorize("Fretes/RegrasInclusaoICMS")]
        public async Task<IActionResult> RegrasInclusaoICMS()
        {
            return View();
        }


        [CustomAuthorize("Fretes/AutorizacaoRecusaCheckin")]
        public async Task<IActionResult> AutorizacaoRecusaCheckin()
        {
            return View();
        }

        [CustomAuthorize("Fretes/AutorizacaoTaxaDescarga")]
        public async Task<IActionResult> AutorizacaoTaxaDescarga()
        {
            return View();
        }

        [CustomAuthorize("Fretes/RotaTabelaFreteCliente")]
        public async Task<IActionResult> RotaTabelaFreteCliente()
        {
            return View();
        }

        [CustomAuthorize("Fretes/ApuracaoBonificacao")]
        public async Task<IActionResult> ApuracaoBonificacao()
        {
            return View();
        }
    }
}
