using Dominio.Entidades.Embarcador.Cargas.ControleEntrega;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize(new string[] { "PesquisaParadas", "ExportarPesquisaParadas", "PesquisaDocumentos", "ExportarPesquisaDocumentos" }, "Cargas/ControleEntrega")]
    public class ControleEntregaRaioxController : BaseController
    {
        #region Construtores

        public ControleEntregaRaioxController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> PesquisaParadas()
        {
            try
            {
                return new JsonpResult(ObterGridParadas());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ExportarPesquisaParadas()
        {
            try
            {
                var grid = ObterGridParadas();
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

        public async Task<IActionResult> PesquisaDocumentos()
        {
            try
            {
                return new JsonpResult(ObterGridDocumentos());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ExportarPesquisaDocumentos()
        {
            try
            {
                var grid = ObterGridDocumentos();
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridParadas()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                decimal tamanhoCampoData = (decimal)7.92;
                decimal tamanhoCampoSequencia = (decimal)4.3;

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cliente", "Cliente", (decimal)15.3, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cidade", "Cidade", (decimal)6.93, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", (decimal)3.96, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", (decimal)5.45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Peso (kg)", "Peso", 4, Models.Grid.Align.right, true).Ord(false);
                grid.AdicionarCabecalho("Valor Total Produtos (R$)", "ValorTotal", (decimal)5.45, Models.Grid.Align.right, true).Ord(false);
                grid.AdicionarCabecalho("Sequência Planejada", "SequenciaPlanejada", tamanhoCampoSequencia, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Sequência Executada", "SequenciaExecutada", tamanhoCampoSequencia, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Previsão Chegada", "PrevisaoChegada", tamanhoCampoData, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Previsão Chegada Reprogramada", "PrevisaoChegadaReprogramada", tamanhoCampoData, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Entrada Raio", "EntradaRaio", tamanhoCampoData, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Início Entrega", "InicioEntrega", tamanhoCampoData, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Fim Entrega", "FimEntrega", tamanhoCampoData, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Saída Raio", "SaidaRaio", tamanhoCampoData, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Endereço", "Endereco", 16, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Entrega Realizada no Prazo", "EntregaNoPrazo", 4, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("CodigoCarga", false);

                int codigoCarga = Request.GetIntParam("Carga");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarPesquisaParadas);
                parametrosConsulta.LimiteRegistros = 1000;

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int totalRegistros = repCargaEntrega.ContarConsultaParadasPorCarga(codigoCarga);

                List<CargaEntrega> listaCargaEntrega = (totalRegistros > 0) ? repCargaEntrega.ConsultarParadasPorCarga(codigoCarga, parametrosConsulta) : new List<CargaEntrega>();

                List<dynamic> listaCargaEntregaRetornar = (from cargaEntrega in listaCargaEntrega select ObterDetalhesGridParadas(cargaEntrega)).ToList();

                grid.AdicionaRows(listaCargaEntregaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private dynamic ObterDetalhesGridParadas(CargaEntrega cargaEntrega)
        {
            var objCargaEntrega = new
            {
                cargaEntrega.Codigo,
                CodigoCarga = cargaEntrega.Carga.Codigo,
                Cliente = cargaEntrega.Cliente?.Nome,
                Cidade = cargaEntrega.Cliente?.Localidade.DescricaoCidadeEstado,
                Tipo = cargaEntrega.Coleta ? "C" : "E",
                Situacao = cargaEntrega.DescricaoSituacao,
                Peso = cargaEntrega.Pedidos.Sum(p => p.CargaPedido.Pedido.PesoTotal).ToString("n0"),
                ValorTotal = cargaEntrega.Pedidos.Sum(p => p.CargaPedido.Produtos.Sum(o => o.ValorTotal)).ToString("n2"),
                SequenciaPlanejada = cargaEntrega.Ordem.ToString(),
                SequenciaExecutada = cargaEntrega.OrdemRealizada.ToString(),
                PrevisaoChegada = cargaEntrega.DataPrevista?.ToDateTimeString() ?? string.Empty,
                PrevisaoChegadaReprogramada = cargaEntrega.DataReprogramada?.ToDateTimeString() ?? string.Empty,
                EntradaRaio = cargaEntrega.DataEntradaRaio?.ToDateTimeString() ?? string.Empty,
                InicioEntrega = cargaEntrega.DataInicio?.ToDateTimeString() ?? string.Empty,
                FimEntrega = cargaEntrega.DataFim?.ToDateTimeString() ?? string.Empty,
                SaidaRaio = cargaEntrega.DataSaidaRaio?.ToDateTimeString() ?? string.Empty,
                Endereco = cargaEntrega.Cliente?.EnderecoCompleto,
                EntregaNoPrazo = cargaEntrega.DescricaoEntregaNoPrazo,
            };

            return objCargaEntrega;
        }

        private string ObterPropriedadeOrdenarPesquisaParadas(string prop)
        {
            switch (prop)
            {
                case "Cliente": return "Cliente.Nome";
                case "Cidade": return "Cliente.Localidade.Descricao";
                case "Tipo": return "Coleta";
                case "SequenciaPlanejada": return "Ordem";
                case "SequenciaExecutada": return "OrdemRealizada";
                case "PrevisaoChegada": return "DataEntregaPrevista";
                case "PrevisaoChegadaReprogramada": return "DataEntregaReprogramada";
                case "EntradaRaio": return "DataEntradaRaio";
                case "InicioEntrega": return "DataInicioEntrega";
                case "FimEntrega": return "DataEntrega";
                case "SaidaRaio": return "DataSaidaRaio";
                default: return prop;
            }
        }

        private Models.Grid.Grid ObterGridDocumentos()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Sequencia", false);
                grid.AdicionarCabecalho("Pedido", "NumeroPedido", (decimal)9.7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cliente", "Cliente", (decimal)38.8, Models.Grid.Align.left, true).Ord(false);
                grid.AdicionarCabecalho("Cidade", "Cidade", (decimal)16.5, Models.Grid.Align.left, true).Ord(false);
                grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", (decimal)11.6, Models.Grid.Align.center, true).Ord(false);
                grid.AdicionarCabecalho("Situação", "Situacao", (decimal)23.3, Models.Grid.Align.left, true);

                int codigoCarga = Request.GetIntParam("Carga");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarPesquisaDocumentos);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

                int totalRegistros = repCargaEntregaPedido.ContarConsultaDocumentosPorCarga(codigoCarga);
                List<CargaEntregaPedido> listaCargaEntregaPedido = (totalRegistros > 0) ? repCargaEntregaPedido.ConsultarDocumentosPorCarga(codigoCarga, parametrosConsulta) : new List<CargaEntregaPedido>();
                List<dynamic> listaCargaEntregaRetornar = (from cargaEntrega in listaCargaEntregaPedido select ObterDetalhesGridDocumentos(cargaEntrega)).ToList();

                grid.AdicionaRows(listaCargaEntregaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private dynamic ObterDetalhesGridDocumentos(CargaEntregaPedido cargaEntregaPedido)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaEntregaPedido.CargaPedido;
            Dominio.Entidades.Cliente cliente = (cargaPedido.ClienteEntrega ?? cargaPedido.ClienteColeta);

            var objCargaEntrega = new
            {
                cargaEntregaPedido.Codigo,
                Sequencia = cargaEntregaPedido.CargaEntrega.Ordem,
                NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                Cliente = cliente?.Nome ?? string.Empty,
                Cidade = cliente?.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                NotaFiscal = string.Join(", ", (from n in cargaPedido.NotasFiscais select n.XMLNotaFiscal.Numero)),
                Situacao = cargaEntregaPedido.CargaEntrega.DescricaoSituacao,
            };

            return objCargaEntrega;
        }

        private string ObterPropriedadeOrdenarPesquisaDocumentos(string prop)
        {
            switch (prop)
            {
                case "Sequencia": return "CargaEntrega.Ordem";
                case "NumeroPedido": return "CargaPedido.Pedido.NumeroPedidoEmbarcador";
                case "Situacao": return "CargaEntrega.Situacao";
                default: return prop;
            }
        }

        #endregion
    }
}
