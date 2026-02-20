using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/VincularProdutosFornecedorEmbarcadorPorNFe")]
	public class VincularProdutosFornecedorEmbarcadorPorNFeController : BaseController
	{
		#region Construtores

		public VincularProdutosFornecedorEmbarcadorPorNFeController(Conexao conexao) : base(conexao) { }

		#endregion

		public async Task<IActionResult> CarregarArquivoXML()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
					return new JsonpResult(false, true, "Nenhum arquivo foi selecionado.");

				Servicos.NFe servicoNFe = new Servicos.NFe(unitOfWork);

				dynamic produtos = new { };

				Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe dadosNFe = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe();

				for (int i = 0; i < files.Count; i++)
				{
					Servicos.DTO.CustomFile file = files[i];
					string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
					string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

					System.IO.Stream xml = file.InputStream;
					xml.Position = 0;

					System.IO.StreamReader streamReaderXML = new System.IO.StreamReader(xml);
					streamReaderXML.BaseStream.Position = 0;

					dadosNFe = servicoNFe.ObterDocumentoPorXML(streamReaderXML.BaseStream, unitOfWork, true);
					produtos = ObterProdutosCadastrados(dadosNFe, unitOfWork);
				}

				return new JsonpResult(new
				{
					Fornecedor = new { Codigo = dadosNFe.Remetente?.Codigo ?? 0, Descricao = dadosNFe.Remetente?.Descricao ?? string.Empty },
					Produtos = produtos
				});
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();

				Servicos.Log.TratarErro(ex);

				return new JsonpResult(false, "Ocorreu uma falha ao carregar o XML da NF-e.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> VincularProduto()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
				Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
				Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFornecedor repositorioprodutoEmbarcadorFornecedor = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFornecedor(unitOfWork);
				Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
				Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

				string codigoInterno = Request.GetStringParam("CodigoInterno");
				double cnpjFornecedor = Request.GetDoubleParam("Fornecedor");
				List<int> codigosFiliais = Request.GetListParam<int>("Filiais");
				int codigoProdutoEmbarcador = Request.GetIntParam("ProdutoEmbarcador");

				Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repositorioProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador);
				Dominio.Entidades.Cliente fornecedor = repositorioCliente.BuscarPorCPFCNPJ(cnpjFornecedor);
				List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = repositorioFilial.BuscarPorCodigos(codigosFiliais);

				if (produtoEmbarcador == null)
					return new JsonpResult(false, true, "Produto não encontrado.");

				if (fornecedor == null)
					return new JsonpResult(false, true, "Fornecedor não encontrado.");

				if (filiais == null || filiais.Count == 0)
					return new JsonpResult(false, true, "Filial não encontrada.");

				if (string.IsNullOrWhiteSpace(codigoInterno))
					return new JsonpResult(false, true, "Ocorreu um erro ao vincular o código do produto do fornecedor.");

				unitOfWork.Start();

				foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiais)
				{
					Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor produtoEmbarcadorFornecedor = repositorioprodutoEmbarcadorFornecedor.BuscarPorProdutoCodigoInternoEFornecedor(produtoEmbarcador.Codigo, codigoInterno, cnpjFornecedor, filial.Codigo);

					if (produtoEmbarcadorFornecedor == null)
					{
						produtoEmbarcadorFornecedor = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor()
						{
							ProdutoEmbarcador = produtoEmbarcador,
							Fornecedor = fornecedor,
							Filial = filial,
							CodigoInterno = codigoInterno
						};

						repositorioprodutoEmbarcadorFornecedor.Inserir(produtoEmbarcadorFornecedor);
					}
					else
					{
						produtoEmbarcadorFornecedor.Filial = filial;

						repositorioprodutoEmbarcadorFornecedor.Atualizar(produtoEmbarcadorFornecedor);
					}
				}

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception excecao)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(excecao);
				return new JsonpResult(false, "Ocorreu uma falha ao vincular o produto da NF-e.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		private dynamic ObterProdutosCadastrados(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe dadosNFe, Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFornecedor repositorioProdutoEmbarcadorFornecedor = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFornecedor(unitOfWork);
			Repositorio.Embarcador.Filiais.Filial repFiliais = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

			List<dynamic> produtos = new List<dynamic>();

			foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produtoNFe in dadosNFe.Produtos)
			{
				Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor produtoEmbarcadorFornecedor = repositorioProdutoEmbarcadorFornecedor.BuscarPorCodigoInternoEFornecedor(produtoNFe.Codigo, dadosNFe.Remetente.CPF_CNPJ);
				List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = repFiliais.BuscarPorCodigoInternoProduto(produtoEmbarcadorFornecedor?.CodigoInterno);

				if (!produtos.Any(p => p.Codigo == produtoNFe.Codigo))
				{
					var produto = new
					{
						produtoNFe.Codigo,
						CodigoProdutoFornecedor = produtoNFe.Codigo,
						DescricaoProdutoFornecedor = produtoNFe.Descricao,
						ProdutoEmbarcadorCodigo = produtoEmbarcadorFornecedor?.ProdutoEmbarcador?.Codigo ?? 0,
						CodigoProdutoEmbarcador = produtoEmbarcadorFornecedor?.ProdutoEmbarcador?.CodigoProdutoEmbarcador ?? string.Empty,
						DescricaoProdutoEmbarcador = produtoEmbarcadorFornecedor?.ProdutoEmbarcador?.Descricao ?? string.Empty,
						Filiais = (from obj in filiais select new { obj?.Codigo, obj?.Descricao }).ToList(),
						Incluso = produtoEmbarcadorFornecedor != null ? true : false,
						Status = produtoEmbarcadorFornecedor != null ? "Já cadastrado" : "Não cadastrado",
						DescricoesFiliais = string.Join(", ", filiais.Select(x => x?.Descricao).ToList()),

					};

					produtos.Add(produto);
				}
			}

			return produtos;
		}
	}
}
