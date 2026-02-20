using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Integracao
{
    public class CargaIntegracaoEventoController : BaseController
    {
		#region Construtores

		public CargaIntegracaoEventoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos

        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaIntegracaoEvento repositorioIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEvento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaIntegracaoEvento filtroPequisa = ObterFiltrosPesquisa();
                Models.Grid.Grid grid = ObterGrid();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegitrosIntegracao = repositorioIntegracao.ContarConsultar(filtroPequisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> integracoes = totalRegitrosIntegracao > 0 ? repositorioIntegracao.Consultar(filtroPequisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento>();

                grid.setarQuantidadeTotal(totalRegitrosIntegracao);
                grid.AdicionaRows(FormatarRetorno(integracoes));

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
        }
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaIntegracaoEvento repIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEvento(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.NaoHaRegistrosParaEsseHistorico);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração - " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaIntegracaoEvento repIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEvento(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento integracao = repIntegracao.BuscarPorCodigo(codigo, false);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Cargas.CargaIntegracaoEvento repositorioIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento integracao = repositorioIntegracao.BuscarPorCodigo(codigo, false);

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null,"Reenvio de integração", unitOfWork);

                repositorioIntegracao.Atualizar(integracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReenviarArquivoParaIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaIntegracaoEvento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaIntegracaoEvento()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao"),
                DataIntegracaoInicial = Request.GetDateTimeParam("DataIntegracaoInicial"),
                DataIntegracaoFinal = Request.GetDateTimeParam("DataIntegracaoFinal")
            };
        }

        private Models.Grid.Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Protocolo Carga", "ProtocoloCarga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação Integração", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Integração", "DataIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Integradora", "Integradora", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("N° Envios", "NumeroEnvios", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Etapa", "Etapa", 10, Models.Grid.Align.left, true);
            return grid;

        }

        private dynamic FormatarRetorno(List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> integracoes)
        {
            return from obj in integracoes
                   select new
                   {
                       obj.Codigo,
                       NumeroCarga = obj.Carga.CodigoCargaEmbarcador,
                       ProtocoloCarga = obj.Carga.Codigo,
                       SituacaoIntegracao = obj.SituacaoIntegracao.ObterDescricao(),
                       DataIntegracao = obj.DataIntegracao.ToString("g"),
                       Retorno = obj.ProblemaIntegracao ?? string.Empty,
                       Integradora = obj.TipoIntegracao?.Descricao ?? string.Empty,
                       NumeroEnvios = obj.NumeroTentativas,
                       Etapa = obj.Etapa.ObterDescricao() ?? string.Empty,
                   };
        }

        #endregion
    }
}
