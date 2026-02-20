using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.OrdemEmbarque
{
    [CustomAuthorize("Cargas/OrdemEmbarqueIntegracao")]
    public class OrdemEmbarqueIntegracaoController : BaseController
    {
		#region Construtores

		public OrdemEmbarqueIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao integracao = repositorioIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if ((arquivoIntegracao == null) || ((arquivoIntegracao.ArquivoRequisicao == null) && (arquivoIntegracao.ArquivoResposta == null)))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos do {integracao.Descricao}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

                return Arquivo(bArquivo, "text/csv", "PedidoDadosTransporteMaritimo.csv");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao integracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: false);

                var arquivosTransacaoRetornar = (
                    from arquivoTransacao in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                    select new
                    {
                        arquivoTransacao.Codigo,
                        Data = arquivoTransacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        arquivoTransacao.DescricaoTipo,
                        arquivoTransacao.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(arquivosTransacaoRetornar);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao cargaOrdemEmbarqueIntegracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: true);

                if (cargaOrdemEmbarqueIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOrdemEmbarqueIntegracao, null, "Reenviou integração.", unitOfWork);
                new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork).IntegrarCargaOrdemEmbarque(cargaOrdemEmbarqueIntegracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterCargaOrdemEmbarqueIntegracao(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao cargaOrdemEmbarqueIntegracao, List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento)
        {
            int codigoCargaFiltrarJanelaCarregamento = cargaOrdemEmbarqueIntegracao.Carga.CargaAgrupamento?.Codigo ?? cargaOrdemEmbarqueIntegracao.Carga.Codigo;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == codigoCargaFiltrarJanelaCarregamento select o).FirstOrDefault();

            return new
            {
                cargaOrdemEmbarqueIntegracao.Codigo,
                cargaOrdemEmbarqueIntegracao.Carga.CodigoCargaEmbarcador,
                Filial = cargaOrdemEmbarqueIntegracao.Carga.Filial?.Descricao ?? "",
                Veiculo = (cargaOrdemEmbarqueIntegracao.Carga.CargaAgrupamento == null) ? cargaOrdemEmbarqueIntegracao.Carga.RetornarPlacas : cargaOrdemEmbarqueIntegracao.Carga.CargaAgrupamento.RetornarPlacas,
                InicioCarregamento = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? "",
                SituacaoIntegracao = cargaOrdemEmbarqueIntegracao.Cancelada ? "Cancelada" : cargaOrdemEmbarqueIntegracao.DescricaoSituacaoIntegracao,
                cargaOrdemEmbarqueIntegracao.ProblemaIntegracao,
                cargaOrdemEmbarqueIntegracao.NumeroTentativas,
                DataIntegracao = cargaOrdemEmbarqueIntegracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                OrdemEmbarque = string.Join(", ", (from o in ordensEmbarque where o.Carga.Codigo == cargaOrdemEmbarqueIntegracao.Carga.Codigo select o.Numero)),
                DT_RowColor = cargaOrdemEmbarqueIntegracao.Cancelada ? CorGrid.Laranja : cargaOrdemEmbarqueIntegracao.SituacaoIntegracao.ObterCorLinha(),
                DT_FontColor = cargaOrdemEmbarqueIntegracao.Cancelada ? CorGrid.Branco : cargaOrdemEmbarqueIntegracao.SituacaoIntegracao.ObterCorFonte(),
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.OrdemEmbarque.FiltroPesquisaCargaOrdemEmbarqueIntegracao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.OrdemEmbarque.FiltroPesquisaCargaOrdemEmbarqueIntegracao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.OrdemEmbarque.FiltroPesquisaCargaOrdemEmbarqueIntegracao()
            {
                CodigoFilial = Request.GetListParam<int>("Filial"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                DataInicialAgendamento = Request.GetNullableDateTimeParam("DataInicialAgendamento"),
                DataLimiteAgendamento = Request.GetNullableDateTimeParam("DataLimiteAgendamento"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao"),
                NumeroOrdemEmbarque = Request.GetStringParam("NumeroOrdemEmbarque"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                Transportadores = Request.GetListParam<int>("Transportador"),
                SomenteComLicencaInvalida = Request.GetBoolParam("SomenteComLicencaInvalida")
            };

            if (!filtrosPesquisa.SituacaoIntegracao.HasValue)
                filtrosPesquisa.Cancelada = Request.GetStringParam("Situacao") == "99";

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Ordem de Embarque", "OrdemEmbarque", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Filial", "Filial", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data do Agendamento", "InicioCarregamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "ProblemaIntegracao", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.Carga.OrdemEmbarque.FiltroPesquisaCargaOrdemEmbarqueIntegracao filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorio = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(unitOfWork);
                
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao> listaCargaOrdemEmbarqueIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao>();
                List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque = new List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

                if (totalRegistros > 0)
                {
                    listaCargaOrdemEmbarqueIntegracao = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);

                    List<int> codigosCargas = (from o in listaCargaOrdemEmbarqueIntegracao select o.Carga.Codigo).ToList();
                    List<int> codigosCargasComAgrupamento = (from o in listaCargaOrdemEmbarqueIntegracao select o.Carga.CargaAgrupamento?.Codigo ?? o.Carga.Codigo).Distinct().ToList();

                    ordensEmbarque = repositorioOrdemEmbarque.BuscarAtivasPorCargas(codigosCargas);
                    listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargasComAgrupamento);
                }

                var listaCargaOrdemEmbarqueIntegracaoRetornar = (
                   from cargaOrdemEmbarqueIntegracao in listaCargaOrdemEmbarqueIntegracao
                   select ObterCargaOrdemEmbarqueIntegracao(cargaOrdemEmbarqueIntegracao, ordensEmbarque, listaCargaJanelaCarregamento)
               ).ToList();

                grid.AdicionaRows(listaCargaOrdemEmbarqueIntegracaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CodigoCargaEmbarcador")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Filial")
                return "Carga.Filial.Descricao";

            if (propriedadeOrdenar == "InicioCarregamento")
                return "Carga.JanelaCarregamento.InicioCarregamento";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
