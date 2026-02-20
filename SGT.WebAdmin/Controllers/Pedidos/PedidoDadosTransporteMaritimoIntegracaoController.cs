using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PedidoBookingIntegracao")]
    public class PedidoDadosTransporteMaritimoIntegracaoController : BaseController
    {
		#region Construtores

		public PedidoDadosTransporteMaritimoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao integracao = repositorioIntegracao.BuscarPorCodigoArquivo(codigo);

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
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao integracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: false);

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
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao integracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: true);

                if (integracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(unitOfWork).IntegrarDadosTransporteMaritimoIntegracao(integracao);

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


        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimoIntegracao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimoIntegracao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimoIntegracao()
            {
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedido"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                NumeroEXP = Request.GetStringParam("NumeroExp"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao"),
                CodigoFilial = Request.GetIntParam("Filial")
            };

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
                grid.AdicionarCabecalho("Número EXP", "NumeroEXP", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Filial", "Filial", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pedido", "PedidoEmbarcador", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Booking", "DataBooking", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "ProblemaIntegracao", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repositorio = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(unitOfWork);

                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao> listaIntegracao;

                if (totalRegistros > 0)
                {
                    listaIntegracao = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                }
                else
                {
                    listaIntegracao = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao>();
                }

                var listaCargaExportacaoIntegracaoRetornar = (
                   from integracao in listaIntegracao
                   select new
                   {
                       integracao.Codigo,
                       NumeroEXP = integracao.PedidoDadosTransporteMaritimo.NumeroEXP,
                       PedidoEmbarcador = integracao.PedidoDadosTransporteMaritimo.Pedido?.NumeroPedidoEmbarcador ?? "",
                       Filial = integracao.PedidoDadosTransporteMaritimo.Filial?.Descricao ?? "",
                       DataBooking = integracao.PedidoDadosTransporteMaritimo.DataBooking?.ToString("dd/MM/yyyy HH:mm") ?? "",
                       SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                       ProblemaIntegracao = integracao.ProblemaIntegracao,
                       NumeroTentativas = integracao.NumeroTentativas,
                       DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                       DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                       DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                   }).ToList();

                grid.AdicionaRows(listaCargaExportacaoIntegracaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {

            if (propriedadeOrdenar == "Filial")
                return "Carga.Filial.Descricao";

            return propriedadeOrdenar;
        }

        #endregion

    }
}
