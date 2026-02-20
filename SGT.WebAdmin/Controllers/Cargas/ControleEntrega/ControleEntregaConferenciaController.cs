using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize(new string[] { "ObterDadosConferenciaEntrega" }, "Cargas/ControleEntrega", "Logistica/Monitoramento", "Chamados/ChamadoOcorrencia")]
	public class ControleEntregaConferenciaController : BaseController
	{
		#region Construtores

		public ControleEntregaConferenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterDadosConferenciaEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaEntrega = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                return new JsonpResult(new
                {
					ProdutosConfernecia = ObterProdutosConferencia(cargaEntrega, configuracaoEmbarcador, unitOfWork)
				});
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

		public async Task<IActionResult> SalvarConferenciaProdutos()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

				dynamic produtosConferidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

				foreach(dynamic produto in produtosConferidos)
				{
					int codigo = ((string)produto.Codigo).ToInt();
					decimal quantidade = ((string)produto.Quantidade).ToDecimal();
					decimal quantidadeConferencia = ((string)produto.QuantidadeConferencia).ToDecimal();
					string observacao = (string)produto.Observacao;

					if (quantidadeConferencia < quantidade && string.IsNullOrWhiteSpace(observacao))
						return new JsonpResult(false, true, "Necessário informar a observação para produtos com divergência na quantidade.");

					Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto = repositorioCargaEntregaProduto.BuscarPorCodigo(codigo);

					if (cargaEntregaProduto != null && cargaEntregaProduto.QuantidadeConferencia != quantidadeConferencia)
					{
						cargaEntregaProduto.QuantidadeConferencia = quantidadeConferencia;
						repositorioCargaEntregaProduto.Atualizar(cargaEntregaProduto);
					}
				}

				return new JsonpResult(true);
			}
			catch (Exception excecao)
			{
				Servicos.Log.TratarErro(excecao);
				return new JsonpResult(false, "Ocorreu uma falha ao salvar a observação do produto.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> SalvarObservacaoProdutoConferencia()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

				int codigoCargaEntregaProduto = Request.GetIntParam("CodigoCargaEntregaProduto");
				string observacao = Request.GetStringParam("Observacao");

				Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto = repositorioCargaEntregaProduto.BuscarPorCodigo(codigoCargaEntregaProduto);

				if (cargaEntregaProduto == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				if (string.IsNullOrWhiteSpace(observacao))
					return new JsonpResult(false, true, "Necessário informar algo na observação.");

				cargaEntregaProduto.ObservacaoProdutoDevolucao = observacao;

				repositorioCargaEntregaProduto.Atualizar(cargaEntregaProduto, Auditado);

				return new JsonpResult(true);
			}
			catch (Exception excecao)
			{
				Servicos.Log.TratarErro(excecao);
				return new JsonpResult(false, "Ocorreu uma falha ao salvar a observação do produto.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}
		public async Task<IActionResult> ExportarExcel()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				Models.Grid.Grid grid = new Models.Grid.Grid();
				grid.header = new List<Models.Grid.Head>();

				int codigoCargaEntrega = Request.GetIntParam("Codigo");

				grid.AdicionarCabecalho("Codigo", false);
				grid.AdicionarCabecalho("Observacao", false);
				grid.AdicionarCabecalho("Código Produto", "CodigoProduto", 10, Models.Grid.Align.left);
				grid.AdicionarCabecalho("Produto", "Descricao", 12, Models.Grid.Align.left);
				grid.AdicionarCabecalho("Quantidade", "Quantidade", 10, Models.Grid.Align.left);
				grid.AdicionarCabecalho("Quantidade conferida", "QuantidadeConferencia", 10, Models.Grid.Align.left);


				Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
				Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

				if (cargaEntrega == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

				Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

				var retorno = ObterProdutosConferencia(cargaEntrega, configuracaoEmbarcador, unitOfWork) ?? new List<dynamic>();



				grid.AdicionaRows(retorno[0].Produtos);
				grid.setarQuantidadeTotal(retorno[0].Produtos.Count);

				byte[] bArquivo = grid.GerarExcel();

				if (bArquivo != null)
					return Arquivo(bArquivo, "application/octet-stream", "RecebimentoProdutos." + grid.extensaoCSV);
				else
					return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao exportar os Dados!");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		#endregion

		#region Métodos Privados

		private dynamic ObterProdutosConferencia(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
		{
			if (!(cargaEntrega.Carga?.TipoOperacao?.ConfiguracaoControleEntrega?.ExigirConferenciaProdutosAoConfirmarEntrega ?? false))
				return null;

			Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioControleEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
			List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioControleEntrega.BuscarPedidosPorCargaEntrega(cargaEntrega.Codigo);

			if ((cargaPedidos.Count == 0) || (cargaPedidos.Count > 0 && !cargaPedidos.Any(p => p.Pedido.PedidoDeDevolucao && p.Pedido.PedidoOrigemDevolucao != null)))
				return null;

			Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
			Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

			List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repositorioCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
			List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repositorioCargaEntregaProduto.BuscarPorCargaEntrega(cargaEntrega.Codigo);

			List<dynamic> conferencia = new List<dynamic>();
			bool preencherQuantidade = cargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue;

			foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
			{
				List<dynamic> produtos = new List<dynamic>();

				List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutosPorNota = (
					from produto in cargaEntregaProdutos
					where produto.XMLNotaFiscal != null && produto.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo
					select produto
				).ToList();

				foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto produto in cargaEntregaProdutosPorNota)
				{
					string corFonte = string.Empty;
					decimal quantidade = preencherQuantidade ? produto.Quantidade : produto.QuantidadeConferencia;

					if (quantidade >= produto.Quantidade)
						corFonte = "#00cc00";
					else if (quantidade > 0m)
						corFonte = "#e08506";

					cargaEntregaProdutos.Remove(produto);
					produtos.Add(new
					{
						produto.Codigo,
						produto.Produto.Descricao,
						CodigoProduto = produto.Produto.CodigoProdutoEmbarcador,
						Quantidade = produto.Quantidade.ToString($"n{configuracaoEmbarcador.NumeroCasasDecimaisQuantidadeProduto}"),
						QuantidadeConferencia = quantidade.ToString($"n{configuracaoEmbarcador.NumeroCasasDecimaisQuantidadeProduto}"),
						Observacao = produto.ObservacaoProdutoDevolucao ?? string.Empty,
						DT_FontColor = corFonte
					});
				}

				conferencia.Add(new
				{
					cargaEntregaNotaFiscal.Codigo,
					cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
					Produtos = produtos
				});
			}

			return conferencia;
		}

		#endregion
	}
}
