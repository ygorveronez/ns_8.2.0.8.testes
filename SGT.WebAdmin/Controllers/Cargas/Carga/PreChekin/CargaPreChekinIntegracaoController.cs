using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.PreChekin
{
    [CustomAuthorize(new string[] { "DownloadArquivosHistoricoIntegracao", "ConsultarHistoricoIntegracaoCargaPreChekin", "ObterTotaisIntegracoesCargaPreChekin", "PesquisaCargaPreChekinIntegracoes" }, "Cargas/Carga")]
    public class CargaPreChekinIntegracaoController : BaseController
    {
		#region Construtores

		public CargaPreChekinIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos

        public async Task<IActionResult> PesquisaCargaPreChekinIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Tentativas", "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 30, Models.Grid.Align.left, false);


                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao filtroPesquisa = ObterFiltroPesquisaIntegracao();
                Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaPreChekinIntegracao = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao> listaIntegracoes = repositorioCargaPreChekinIntegracao.Consultar(filtroPesquisa, grid.ObterParametrosConsulta());
                int totalIntegracoes = repositorioCargaPreChekinIntegracao.ContarConsulta(filtroPesquisa);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracaoCargaPreChekin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaFreIntegracao = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao existeCargaFrete = repositorioCargaFreIntegracao.BuscarPorCodigo(codigo, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = existeCargaFrete.ArquivosTransacao.ToList();

                grid.setarQuantidadeTotal(integracoesArquivos.Count);

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotaisIntegracoesCargaPreChekin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioCargaFreteIntegracao.ContarConsulta(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigo, Situacao = SituacaoIntegracao.AgIntegracao });
                int totalIntegrado = repositorioCargaFreteIntegracao.ContarConsulta(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigo, Situacao = SituacaoIntegracao.Integrado });
                int totalProblemaIntegracao = repositorioCargaFreteIntegracao.ContarConsulta(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigo, Situacao = SituacaoIntegracao.ProblemaIntegracao });
                int totalAguardandoRetorno = repositorioCargaFreteIntegracao.ContarConsulta(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigo, Situacao = SituacaoIntegracao.AgRetorno });

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoIntegracao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCtArquivos = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo cargaFreteIntegracao = repositorioCtArquivos.BuscarPorCodigo(codigoIntegracao, false);

                if (cargaFreteIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> aquivosBaixar = new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>();

                aquivosBaixar.Add(cargaFreteIntegracao.ArquivoRequisicao);
                aquivosBaixar.Add(cargaFreteIntegracao.ArquivoResposta);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(aquivosBaixar);

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Frete.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarPlacasPreChekin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool permiteIntegrarComFalha = Request.GetBoolParam("PermitirIntegrarConFalha");

                Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaPreChekinIntegracao = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(unitOfWork);
                Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever integracaoUnilever = new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao cargaPrechekinIntegracao = CriarRegistroIntegracao(unitOfWork);
                string msg = integracaoUnilever.ConfirmarPlacasPreChekin(cargaPrechekinIntegracao, permiteIntegrarComFalha, Auditado, TipoServicoMultisoftware);

                if (cargaPrechekinIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, "Falha ao tentar confirmar placas.");

                return new JsonpResult(true, msg);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverPlacasPreChekin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaPreChekinIntegracao = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(unitOfWork);
                Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever integracaoUnilever = new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao cargaPrechekinIntegracao = CriarRegistroIntegracao(unitOfWork);

                integracaoUnilever.RemoverPlacasPreChekin(cargaPrechekinIntegracao, TipoServicoMultisoftware);

                if (cargaPrechekinIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, cargaPrechekinIntegracao.ProblemaIntegracao);

                return new JsonpResult(true, "Placas Confirmadas com sucesso!");
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarTransportes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaPreChekinIntegracao = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga exiteCarga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (exiteCarga == null)
                    throw new ControllerException($"Não foi encontrado registro de carga com o codigo {codigoCarga}");

                Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever integracaoUnilever = new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork);
                integracaoUnilever.GerarTransportesCargasPreChekin(exiteCarga, Auditado);


                return new JsonpResult(true, "Transportes gerados com sucesso!");
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao ObterFiltroPesquisaIntegracao()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao()
            {
                Codigo = Request.GetIntParam("Codigo"),
                Situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao")
            };
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao CriarRegistroIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaPreChekinIntegracao = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            int.TryParse(Request.Params("Carga"), out int codigoCarga);

            Dominio.Entidades.Embarcador.Cargas.Carga exiteCarga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            if (exiteCarga == null)
                throw new ControllerException($"Não foi encontrado registro de carga com o codigo {codigoCarga}");

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

            if (tipoIntegracao == null)
                throw new ControllerException("Tipo de integração não encontrado");

            Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao cargaPreChekinIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao()
            {
                Carga = exiteCarga,
                NumeroTentativas = 0,
                ProblemaIntegracao = "",
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracao,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao
            };

            repositorioCargaPreChekinIntegracao.Inserir(cargaPreChekinIntegracao);

            return cargaPreChekinIntegracao;
        }

        #endregion
    }
}
