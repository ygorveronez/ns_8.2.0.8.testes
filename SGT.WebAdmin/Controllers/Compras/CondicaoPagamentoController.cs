using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SGT.WebAdmin.Controllers.Compras
{
	[CustomAuthorize("Compras/CondicaoPagamento")]
	public class CondicaoPagamentoController : BaseController
	{
		#region Construtores

		public CondicaoPagamentoController(Conexao conexao) : base(conexao) { }

		#endregion


		#region Métodos Globais
		[AllowAuthenticate]
		public async Task<IActionResult> Pesquisa()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Manipula grids
				Models.Grid.Grid grid = GridPesquisa();

				// Ordenacao da grid
				string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
				PropOrdena(ref propOrdenar);

				// Busca Dados
				int totalRegistros = 0;
				var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

				// Seta valores na grid
				grid.AdicionaRows(lista);
				grid.setarQuantidadeTotal(totalRegistros);

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

		public async Task<IActionResult> BuscarPorCodigo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Instancia repositorios
				Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);

				// Parametros
				int.TryParse(Request.Params("Codigo"), out int codigo);

				// Busca informacoes
				Dominio.Entidades.Embarcador.Compras.CondicaoPagamento condicao = repCondicaoPagamento.BuscarPorCodigo(codigo);

				// Valida
				if (condicao == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				// Formata retorno
				var retorno = new
				{
					condicao.Codigo,
					condicao.Descricao,
					Status = condicao.Ativo,
                    condicao.QuantidadeParcelas,
					condicao.IntervaloDias,
					condicao.DiasParaPrimeiroVencimento,
                    condicao.Observacao
				};

				// Retorna informacoes
				return new JsonpResult(retorno);
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

		private Dominio.Entidades.Embarcador.CIOT.CIOTPamcard ObterConfiguracaoPamcard(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unidadeTrabalho)
		{
			Repositorio.Embarcador.CIOT.CIOTPamcard repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPamcard(unidadeTrabalho);

			Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = repCIOTPamcard.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

			return configuracao;
		}

		public async Task<IActionResult> Adicionar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{

				Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);
				Dominio.Entidades.Embarcador.Compras.CondicaoPagamento condicao = new Dominio.Entidades.Embarcador.Compras.CondicaoPagamento();

				// Preenche entidade com dados
				PreencheEntidade(ref condicao, unitOfWork);

				// Valida entidade
				if (!ValidaEntidade(condicao, out string erro))
					return new JsonpResult(false, true, erro);

				// Persiste dados
				repCondicaoPagamento.Inserir(condicao, Auditado);
				unitOfWork.CommitChanges();

				// Retorna sucesso
				return new JsonpResult(true);

				//Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
				//List<long> destinados = repDocumentoDestinadoEmpresa.BuscarPorTipoDocumento(TipoDocumentoDestinadoEmpresa.NFeTransporte);
				//foreach (var codigoDestinado in destinados)
				//{
				//    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigoDestinado);

				//    string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;  

				//    if (documentoDestinado.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFSeDestinada)
				//        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFSe", documentoDestinado.Chave + "_NFSe.xml");
				//    else
				//        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");

				//    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
				//        Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _conexao.StringConexao, null, out string msgErro, documentoDestinado.NumeroSequencialUnico);
				//}

				//Servicos.Embarcador.Carga.Carga.VincularNotasFiscaisParciaisDosPedidos(TipoServicoMultisoftware, unitOfWork, Auditado);
				//Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
				//string[] cpfsMotoristas = CPFsMotoristas();
				//for (int i = 0; i < cpfsMotoristas.Length; i++)
				//{
				//    List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> estoques = repProdutoEstoque.BuscarTodosPorProduto(Utilidades.String.RemoveAllSpecialCharacters(cpfsMotoristas[i]).ToInt());
				//    if (estoques != null && estoques.Count > 1)
				//    {
				//        try
				//        {
				//            int codigoPrincipal = estoques.LastOrDefault().Codigo;
				//            int codigoDeletar = estoques.FirstOrDefault().Codigo;
				//            if (codigoDeletar > 0 && codigoPrincipal > 0 && codigoPrincipal != codigoDeletar)
				//            {
				//                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoqueManter = repProdutoEstoque.BuscarPorCodigo(codigoPrincipal);
				//                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoqueDeletar = repProdutoEstoque.BuscarPorCodigo(codigoPrincipal);

				//                estoqueManter.Quantidade += estoqueDeletar.Quantidade;
				//                estoqueManter.CustoMedio = estoqueManter.Produto.CustoMedio;
				//                estoqueManter.UltimoCusto = estoqueManter.Produto.UltimoCusto;

				//                repProdutoEstoque.Atualizar(estoqueManter);

				//                repProdutoEstoque.DeletarEstoque(codigoDeletar, codigoPrincipal);                                
				//            }
				//        }
				//        catch (Exception ex)
				//        {
				//            Servicos.Log.TratarErro(ex);
				//        }

				//    }
				//}

				//Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente = repCargaCancelamentoCargaIntegracao.BuscarPorCodigo(161);
				//new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarCargaCancelamento(integracaoPendente);

				//Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
				//Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave("42230918886577000108570030000027981000279892");
				//Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);
				//Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(cargaCTe.CargaOrigem, cte, null, null, null, null, false, false, cargaCTe.Carga.SituacaoLiberacaoEscrituracaoPagamentoCarga, false, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

				//Servicos.CTe serCTE = new Servicos.CTe(unidadeDeTrabalho);
				//serCTE.SetarPartilhaICMS(ref cte, unitOfWork);
				//Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoPendente = repFaturaIntegracao.BuscarPorCodigo(692328);
				//new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarFatura(integracaoPendente);

				//Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
				//Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
				//Servicos.Embarcador.Integracao.Intercab.IntegracaoVessel svcIntegracaoVessel = new Servicos.Embarcador.Integracao.Intercab.IntegracaoVessel(unitOfWork);
				//Servicos.Embarcador.Integracao.Intercab.IntegracaoCustomer svcIntegracaoCustomer = new Servicos.Embarcador.Integracao.Intercab.IntegracaoCustomer(unitOfWork);
				//Servicos.Embarcador.Integracao.Intercab.IntegracaoBooking svcIntegracaoBooking = new Servicos.Embarcador.Integracao.Intercab.IntegracaoBooking(unitOfWork, TipoServicoMultisoftware);

				//CancellationTokenSource cts = new CancellationTokenSource();
				//svcIntegracaoCustomer.IntegrarConsumerEMP(configuracaoIntegracaoIntegracaoEMP);
				//CancellationToken cancellationToken;
				//svcIntegracaoVessel.IntegrarConsumerEMP(configuracaoIntegracaoIntegracaoEMP, cancellationToken);
				//svcIntegracaoBooking.IntegrarConsumerEMP(configuracaoIntegracaoIntegracaoEMP, cancellationToken);

				//Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoPendente = repFaturaIntegracao.BuscarPorCodigo(694332);
				//new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarFatura(integracaoPendente, ClienteAcesso.URLAcesso);

				//Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente = repCargaCancelamentoCargaIntegracao.BuscarPorCodigo(10429);
				//new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarCarga(integracaoPendente, ClienteAcesso.URLAcesso);

				//Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao integracaoPendente = repCargaCancelamentoCargaIntegracao.BuscarPorCodigo(62);
				//new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarCartaCorrecaoCTe(integracaoPendente);

				//Servicos.Embarcador.Carga.MICDTA serMICDTA = new Servicos.Embarcador.Carga.MICDTA(_conexao.StringConexao);

				//Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
				//Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(760);
				//List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = repCargaPedido.BuscarPorCarga(760);

				//serMICDTA.EmitirMICDTA(pedidos, carga, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

				//Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
				//serCargaDadosSumarizados.ConsultarMDFeAquaviarioJaGeradoPorMDFe(73, unitOfWork);

				//Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoPendente = repCargaDadosTransporteIntegracao.BuscarPorCodigo(99);
				//new Servicos.Embarcador.Integracao.Moniloc.IntegracaoMoniloc(unitOfWork).IntegrarCarga(integracaoPendente);

				//Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente = repCargaCargaIntegracao.BuscarPorCodigo(9380);
				//new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork).IntegrarCargaCompleta(integracaoPendente);                

				//Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
				//string[] codigosProdutos = CodigosProdutos();
				//for (int i = 0; i < codigosProdutos.Length; i++)
				//{
				//    List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = repProdutoEmbarcador.BuscarListaPorCodigo(Utilidades.String.RemoveAllSpecialCharacters(codigosProdutos[i]));
				//    if (produtos != null && produtos.Count > 1)
				//    {
				//        try
				//        {
				//            int codigoPrincipal = produtos.FirstOrDefault().Codigo;
				//            if (codigoPrincipal > 0)
				//            {
				//                repProdutoEmbarcador.UnificarProduto(codigoPrincipal, Utilidades.String.RemoveAllSpecialCharacters(codigosProdutos[i]));
				//                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador = repProdutoEmbarcador.BuscarProdutosPorCodigoEmbarcador(Utilidades.String.RemoveAllSpecialCharacters(codigosProdutos[i]), codigoPrincipal);
				//                foreach (var prod in produtosEmbarcador)
				//                {
				//                    repProdutoEmbarcador.Deletar(prod);
				//                }
				//                //repProdutoEmbarcador.DeletarProduto(codigoPrincipal, Utilidades.String.RemoveAllSpecialCharacters(codigosProdutos[i]));
				//            }
				//        }
				//        catch (Exception ex)
				//        {
				//            Servicos.Log.TratarErro(ex);
				//        }

				//    }
				//}

				//Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
				//string[] cpfsMotoristas = CPFsMotoristas();

				//for (int i = 0; i < cpfsMotoristas.Length; i++)
				//{
				//    List<Dominio.Entidades.Usuario> motoristas = repUsuario.BuscarListaMotoristaPorCPF(Utilidades.String.RemoveAllSpecialCharacters(cpfsMotoristas[i]));
				//    if (motoristas != null && motoristas.Count > 1)
				//    {
				//        try
				//        {
				//            int codigoPrincipal = motoristas.FirstOrDefault().Codigo;
				//            int codigoDeletar = motoristas.LastOrDefault().Codigo;
				//            if (codigoDeletar > 0 && codigoPrincipal > 0 && codigoPrincipal != codigoDeletar)
				//            {
				//                repUsuario.UnificarFichaMotorista(codigoDeletar, codigoPrincipal);
				//                repUsuario.DeletarMotorista(codigoDeletar);
				//            }
				//        }
				//        catch (Exception ex)
				//        {
				//            Servicos.Log.TratarErro(ex);
				//        }

				//    }
				//}

				//Servicos.Embarcador.Carga.Documentos svcDocumentos = new Servicos.Embarcador.Carga.Documentos(_conexao.StringConexao);

				//Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(537);

				//svcDocumentos.GerarTitulosAutorizacaoCarga(ref carga, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado);

				//Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
				//Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
				//Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(823498);
				//Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(933782);
				//Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

				//if (cargaPedido != null && cargaPedido.Carga != null)
				//    svcCTe.SalvarInformacoesMultiModal(cte, cargaPedido, cte.ValorAReceber, unitOfWork);

				//Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente = repCargaCargaIntegracao.BuscarPorCodigo(95);
				//new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork).IntegrarTodosDocumentos(integracaoPendente);

				//Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
				//Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
				//Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
				//Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

				//List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarCTesImportadosSemDadosMultimodal();

				//foreach (var cte in ctes)
				//{
				//    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCTe(cte.Codigo);

				//    if (cargaPedido != null && cargaPedido.Carga != null)
				//    {
				//        svcCTe.SalvarInformacoesMultiModal(cte, cargaPedido, cte.ValorAReceber, unitOfWork);
				//        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);
				//        carga.CargaIntegradaEmbarcador = false;
				//        repCarga.Atualizar(carga);

				//        repCTe.Atualizar(cte);
				//    }
				//}

				//Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
				//Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
				//Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
				//Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
				//Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
				//Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
				//Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

				//List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarCTesEmitidosManualmenteSemXML();

				//foreach (var cte in ctes)
				//{
				//    foreach (var doc in cte.Documentos)
				//    {
				//        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repXMLNotaFiscal.BuscarPorChave(Utilidades.String.OnlyNumbers(doc.ChaveNFE));
				//        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCTe(cte.Codigo);
				//        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

				//        if (notaFiscal == null)
				//        {
				//            int.TryParse(doc.Numero, out int numeroNota);
				//            notaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
				//            {
				//                Chave = doc.ChaveNFE,
				//                NCM = doc.NCMPredominante,
				//                nfAtiva = true,
				//                Numero = numeroNota,
				//                Peso = doc.Peso,
				//                Valor = doc.Valor,
				//                DataEmissao = doc.DataEmissao,
				//                Destinatario = cargaPedido.Pedido.Destinatario,
				//                Emitente = cargaPedido.Pedido.Remetente,
				//                BaseCalculoICMS = (decimal)0,
				//                ValorICMS = (decimal)0,
				//                BaseCalculoST = (decimal)0,
				//                ValorST = (decimal)0,
				//                ValorTotalProdutos = (decimal)0,
				//                ValorSeguro = (decimal)0,
				//                ValorDesconto = (decimal)0,
				//                ValorImpostoImportacao = (decimal)0,
				//                ValorPIS = (decimal)0,
				//                PesoLiquido = doc.Peso,
				//                Volumes = 0,
				//                ValorCOFINS = (decimal)0,
				//                ValorOutros = (decimal)0,
				//                ValorIPI = (decimal)0,
				//                TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida,
				//                CNPJTranposrtador = cargaPedido.Pedido.Empresa?.CNPJ_SemFormato ?? "",
				//                ValorFrete = (decimal)0,
				//                TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros,
				//                Descricao = doc.ChaveNFE,
				//                ModalidadeFrete = ModalidadePagamentoFrete.Pago,
				//                RetornoNotaIntegrada = false,
				//                Empresa = cargaPedido.Pedido.Empresa,
				//                CanceladaPeloEmitente = false,
				//                NumeroPedido = 0,
				//                QuantidadePallets = (decimal)0,
				//                Altura = (decimal)0,
				//                Largura = (decimal)0,
				//                Comprimento = (decimal)0,
				//                MetrosCubicos = (decimal)0,
				//                PesoBaseParaCalculo = doc.Peso,
				//                PesoCubado = (decimal)0,
				//                PesoPaletizado = (decimal)0,
				//                ValorFreteEmbarcador = (decimal)0,
				//                SemCarga = false,
				//                FatorCubagem = (decimal)0,
				//                PesoPorPallet = (decimal)0,
				//                KMRota = (decimal)0,
				//                XML = "",
				//                PlacaVeiculoNotaFiscal = "",
				//                DataRecebimento = DateTime.Now,
				//                NumeroReferenciaEDI = doc.NumeroReferenciaEDI,
				//                NumeroControleCliente = doc.NumeroControleCliente,
				//                PINSUFRAMA = doc.PINSuframa
				//            };
				//            repXMLNotaFiscal.Inserir(notaFiscal);
				//        }

				//        if (notaFiscal != null && cargaPedido != null && cargaCTe != null)
				//        {
				//            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNota = repPedidoXMLNotaFiscal.BuscarPorCargaPedidoEXMLNotaFiscal(cargaPedido.Codigo, notaFiscal.Codigo);
				//            if (pedidoXMLNota == null)
				//            {
				//                pedidoXMLNota = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork, ConfiguracaoEmbarcador).PreencherDadosNotaFiscal(notaFiscal, cargaPedido);
				//                repPedidoXMLNotaFiscal.Inserir(pedidoXMLNota);
				//            }

				//            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe pedidoXMLNotaFiscalCTe = null;
				//            if (pedidoXMLNotaFiscalCTe == null)
				//            {
				//                pedidoXMLNotaFiscalCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe()
				//                {
				//                    CargaCTe = cargaCTe,
				//                    PedidoXMLNotaFiscal = pedidoXMLNota
				//                };
				//                repCargaPedidoXMLNotaFiscalCTe.Inserir(pedidoXMLNotaFiscalCTe);
				//            }


				//            if (cte.XMLNotaFiscais == null || cte.XMLNotaFiscais.Count == 0)
				//                cte.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

				//            if (!cte.XMLNotaFiscais.Any(o => o.Codigo == notaFiscal.Codigo))
				//                cte.XMLNotaFiscais.Add(new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal() { Codigo = notaFiscal.Codigo });

				//            cargaCTe.Carga.CargaIntegradaEmbarcador = false;
				//            repCarga.Atualizar(cargaCTe.Carga);
				//        }
				//    }

				//}

				//Servicos.Embarcador.Carga.Documentos svcDocumentos = new Servicos.Embarcador.Carga.Documentos(unitOfWork.StringConexao);
				//Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(52);

				//svcDocumentos.AtualizarSumarizacaoViagem(carga, unitOfWork, false);

				//Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

				//Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(60406);
				//Dominio.Entidades.Embarcador.CIOT.CIOTPamcard configuracao = ObterConfiguracaoPamcard(ciot.ConfiguracaoCIOT, unitOfWork);
				//Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);

				//Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Servicos.Embarcador.CIOT.Pamcard();
				//svcPamcard.IntegrarContratoFrete(ciot, modalidade, configuracao, unitOfWork, out string mensagemErro);

				//string msgErro = "";

				//Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
				//Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);

				//List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> emails = repConfigEmailDocTransporte.BuscarEmailLerDocumentos();

				//foreach (Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail in emails)
				//{
				//    Servicos.Email serEmail = new Servicos.Email(unitOfWork);
				//    serEmail.ReceberEmail(configEmail, TipoServicoMultisoftware, configEmail.Email, configEmail.Senha, configEmail.Pop3, configEmail.RequerAutenticacaoPop3, configEmail.PortaPop3, unitOfWork);
				//}

				//Servicos.Embarcador.Carga.Documentos svcDocumentos = new Servicos.Embarcador.Carga.Documentos(_conexao.StringConexao);
				//svcDocumentos.FinalizarCargaEmFinalizacao(85527, TipoServicoMultisoftware, unitOfWork);

				//Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
				//Servicos.WebService.Carga.DadosAverbacao serWSDadosAverbacao = new Servicos.WebService.Carga.DadosAverbacao(_conexao.StringConexao);

				//Dominio.Entidades.AverbacaoCTe averbacao = repAverbacaoCTe.BuscarPorCodigo(634736);

				//Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao dadosAverbacao = serWSDadosAverbacao.ConverterDadosAverbacaoCTe(averbacao, unitOfWork);

				//Servicos.Embarcador.Carga.Documentos svcDocumentos = new Servicos.Embarcador.Carga.Documentos(_conexao.StringConexao);

				//Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
				//Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
				//Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
				//Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(362);
				//Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigo(85);

				//List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
				//List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
				//Servicos.Embarcador.Carga.RotaFrete.SetarRotaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, rotaFrete, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

				//Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarOcorrenciaEstadia(unitOfWork, carga, TipoServicoMultisoftware, Auditado);

				//svcDocumentos.GerarTitulosAutorizacaoCarga(ref carga, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware);


				//Servicos.Embarcador.Integracao.AX.IntegracaoAX.IntegrarCancelamentoCTe(integracaoPendente, unitOfWork);

				//Models.Threads.IntegracaoCarga.ConsultarValoresPedagioPendente(unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);

				//Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente = repCargaCargaIntegracao.BuscarPorCodigo(4);
				//Servicos.Embarcador.Integracao.MICDTA.IntegracaoMICDTA.IntegrarMICDTA(integracaoPendente, unitOfWork, _conexao.StringConexao);

				//AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
				//Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
				//AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
				//AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(46);
				//Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(_conexao.StringConexao);
				//Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(5);
				//serComissaoFuncionario.GerarComissaoMotoristas(comissaoFuncionario.Motorista != null ? comissaoFuncionario.Motorista.Codigo : 0, comissaoFuncionario.Codigo, cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);

				//Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
				//Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorNumero(980);

				//if (!Servicos.Embarcador.Fatura.Fatura.FinalizarFatura(out string erro2, fatura, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware))
				//    throw new Exception(erro2);

				//Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente = repCargaCancelamentoCargaIntegracao.BuscarPorCodigo(18);
				//Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarApiIsRomaneio(integracaoPendente, unitOfWork, integracaoPendente.CargaCancelamento.MotivoCancelamento);

				//Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente = repCargaCargaIntegracao.BuscarPorCodigo(0);
				//Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarApiIsRomaneio(integracaoPendente, unitOfWork, _conexao.StringConexao);

				//Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoPendente = repCargaDadosTransporteIntegracao.BuscarPorCodigo(556);
				//Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarApiEnvioComprovante(integracaoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteTrizy.ComprovanteEntrega, unitOfWork, null, null, TipoServicoMultisoftware, _conexao.StringConexao);

				//Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(_conexao.StringConexao);
				//Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
				//Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorNumero(21000230);
				//servFatura.GerarIntegracoesFatura(fatura, unitOfWork, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador);            

				//Servicos.Embarcador.CIOT.Pagbem serPagbem = new Servicos.Embarcador.CIOT.Pagbem();
				//Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

				//Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorCodigo(369);
				//Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaValePedagio.Carga;

				//serPagbem.GerarCompraValePedagio(cargaValePedagio, carga, unitOfWork, TipoServicoMultisoftware);

				//Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unitOfWork);
				//Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin integracaoMichelin = new Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin(unitOfWork);
				//Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao integracaoPendente = repOcorrenciaEDIIntegracao.BuscarPorCodigo(2040);
				//integracaoMichelin.EnviarOCORREN(integracaoPendente, unitOfWork);
				//repOcorrenciaEDIIntegracao.Atualizar(integracaoPendente);

				//Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
				//Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin integracaoMichelin = new Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao integracaoPendente = repCargaEDIIntegracao.BuscarPorCodigo(76997);
				//integracaoMichelin.EnviarCONEMB(ref integracaoPendente, TipoServicoMultisoftware, unitOfWork, _conexao.StringConexao);
				//repCargaEDIIntegracao.Atualizar(integracaoPendente);

				//Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
				//Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(249217);
				//Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(406718);
				//Servicos.Embarcador.CTe.CTeAgrupado.SetarImpostos(cte, carga, carga.Pedidos.FirstOrDefault(), ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

				//Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repcarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao = repcarregamentoIntegracao.BuscarPorCodigo(12);
				//Servicos.Embarcador.Integracao.IntegracaoCarregamento integracaoCarregamento = new Servicos.Embarcador.Integracao.IntegracaoCarregamento(unitOfWork);

				//integracaoCarregamento.Integrar(carregamentoIntegracao);

				//Servicos.Embarcador.Integracao.AX.IntegracaoAX.IntegrarTransportadora(carregamentoIntegracao, unitOfWork);
				//Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorio = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);

				//Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao = repositorio.BuscarPorCodigo(3);
				//Servicos.Embarcador.Integracao.AX.IntegracaoAX.IntegrarComplementoCTe(integracao, unitOfWork);

				//Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repCargaCargaIntegracao.BuscarPorCodigo(8);

				//Servicos.Embarcador.Integracao.AX.IntegracaoAX.IntegrarContratoFrete(cargaCargaIntegracao, unitOfWork);

				//Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
				//Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

				//Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(221009);

				//List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorTomador(cargaPedido.Codigo, "07526557001262");

				//List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigosComFetch(pedidoCTesParaSubContratacao);

				//decimal valorFreteNegociado = repPedidoCTeParaSubContratacao.ObterValorFreteNegociavelPedido(cargaPedido.Codigo);
				//decimal valorCusteioSVM = repPedidoCTeParaSubContratacao.ObterValorCusteioSVMPedido(cargaPedido.Codigo);
				//decimal valorICMS = pedidosCTeParaSubContratacao.Sum(o => o.ValorICMS);
				//decimal aliquota = repPedidoCTeParaSubContratacao.ObterAliquota(cargaPedido.Codigo);
				//decimal valorFrete = 1;
				//decimal baseICMS = ((100 - aliquota) / 100);
				//List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repPedidoCTeParaSubContratacao.ObterCargaPedido(cargaPedido.Codigo);
				//int qtdContainer = cargasPedidos != null && cargasPedidos.Count > 0 ? cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().Count() : 1;

				//valorFrete = CalcularValorSVM(cargaPedido.TipoPropostaMultimodal, pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault(), pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).ToList(),
				//                                    pedidosCTeParaSubContratacao.Count(), valorCusteioSVM, valorFreteNegociado, baseICMS, "", unitOfWork, qtdContainer);

				//Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
				//Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
				//List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentosCTe = repCargaPedidoDocumentoCTe.BuscarPorCarga(127258);
				//Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);;
				//Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
				//Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLnotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
				//Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
				//Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
				//Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
				//Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

				////foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe in cargaPedidoDocumentosCTe)
				//List<int> codigos = repCTe.BuscarCTesMultimodalPendenteTerceiro("15/08/2020".ToDateTime(), 127258); //cargaPedidoDocumentosCTe.Select(c => c.Codigo).ToList();
				//for (int i = 0; i < codigos.Count; i++)
				//{
				//    //Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCodigo(codigos[i]);
				//    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigos[i]);
				//    //cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(137303);

				//    string retorno = "";
				//    Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao = repCTeTerceiro.BuscarPorChave(cte.Chave);
				//    if (cteParaSubContratacao == null)
				//    {
				//        string descricaoItemPeso = "";// serCTeSubContratacao.ObterDescricaoItemPeso(cargaPedido, unitOfWork);                        
				//        Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterEntidadeCTeParaObjeto(cte, unitOfWork);                        
				//        cteParaSubContratacao = serCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref retorno, null, cteIntegracao, descricaoItemPeso, 0, false, false, TipoServicoMultisoftware);

				//        unitOfWork.FlushAndClear();
				//    }
				//    //Log.TratarErro($"CriarCTeTerceiro - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
				//    //if (string.IsNullOrEmpty(retorno))
				//    //{
				//    //    cteParaSubContratacao.Ativo = true;
				//    //    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = serCTeSubContratacao.InserirCTeSubContratacaoFilialEmissora(cteParaSubContratacao, null, cargaPedido, tipoServicoMultisoftware, unitOfWork);
				//    //    Log.TratarErro($"InserirCTeSubContratacaoFilialEmissora - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");
				//    //}

				//    //AtualizarTipoContratacao(cargaPedido, unitOfWork);
				//}
				//Log.TratarErro($"Fim GerarSubContratacaoSVM - {cargaPedido?.Carga?.CodigoCargaEmbarcador ?? ""} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "CTeSubContratacao");

				//Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(_conexao.StringConexao);
				//Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
				//int[] numerosFaturas = NumerosFaturas();

				//for (int i = 0; i < numerosFaturas.Length; i++)
				//{
				//    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorNumero(numerosFaturas[i]);
				//    if (fatura != null && fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado)
				//    {
				//        unitOfWork.Start();
				//        servFatura.CancelarTitulosBoletos(fatura.Codigo, unitOfWork, Auditado, this.Usuario.Empresa);

				//        fatura.SituacaoNoCancelamento = fatura.Situacao;
				//        fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmCancelamento;
				//        fatura.DataCancelamentoFatura = DateTime.Now;
				//        fatura.MotivoCancelamento = "Cancelamento – Fatura não enviada na primeira tentativa";

				//        servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.CancelouFatura, this.Usuario);

				//        Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Cancelou a fatura.", unitOfWork);

				//        repFatura.Atualizar(fatura);
				//        unitOfWork.CommitChanges();

				//        unitOfWork.FlushAndClear();
				//    }
				//}

				//Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
				//List<int> codigosFatura = new List<int>();
				//codigosFatura.Add(2328);
				//Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao = repFaturaIntegracao.BuscarPorFatura(2328).FirstOrDefault();

				//Servicos.Embarcador.Fatura.Fatura.EnviarFaturaLote(codigosFatura, _conexao.StringConexao, TipoServicoMultisoftware, faturaIntegracao);

				//Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
				//Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repXMLNotaFiscal.BuscarPorCodigo(205224);
				//string caminhoDANFE = "C:\\Arquivos\\DANFE Documentos Emitidos\\" + notaFiscal.Chave + ".pdf";

				//if (!Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro2, notaFiscal.XML, caminhoDANFE, true))
				//    return new JsonpResult(false, true, erro2);

				//Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
				//Servicos.CTe serCTE = new Servicos.CTe(unidadeDeTrabalho);
				//string numeroControle = serCTE.RetornarNumeroControleCTe(out int numeroSequencia, "0TSN009516A", "", Dominio.Enumeradores.TipoPropostaFeeder.Mercosul, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario, Dominio.Enumeradores.TipoServico.Normal, 156732, false, unitOfWork);
				//if (repCTe.ContemNumeroControleDuplicado(numeroControle, 0))
				//{
				//    numeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, "0AIBAK47CB", "AK", Dominio.Enumeradores.TipoPropostaFeeder.Mercosul, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario, Dominio.Enumeradores.TipoServico.Normal, 0, false, unitOfWork, true);
				//}

				//Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(_conexao.StringConexao);

				//Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
				//Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(26846);

				//int? diasPrazoFatura = 0;
				//int? diaMes = 0;
				//bool? permiteFinalSemana = false;
				//Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? diaSemana = null;
				//Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento? tipoPrazoFatura = null;

				//List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasSemana = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();
				//List<int> diasMes = new List<int>();

				//servFatura.RetornarParametrosFaturamento(cte, unitOfWork, true, out diaMes, out diaSemana, out permiteFinalSemana, out diasPrazoFatura, out diasSemana, out diasMes, out tipoPrazoFatura, 0);
				//DateTime? dataBaseParcela = servFatura.RetornarDataBase(tipoPrazoFatura, cte, true, unitOfWork);

				//if (dataBaseParcela.HasValue && (diaMes != null || diaSemana != null || diasPrazoFatura != null || tipoPrazoFatura != null || diasMes.Count > 0 || diasSemana.Count > 0))
				//    cte.DataPreviaVencimento = servFatura.RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes);

				//repCTe.Atualizar(cte);

				unitOfWork.Start();

				//Servicos.Embarcador.Carga.RateioCTe serRateioCTe = new Servicos.Embarcador.Carga.RateioCTe(_conexao.StringConexao);

				//Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
				//Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

				//List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(115025);
				//List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(115025);

				//serRateioCTe.AjustarFretePorCTes(cargaPedidos[0], cargaCTes, TipoServicoMultisoftware, unitOfWork);


				//return new JsonpResult(true);


			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> Atualizar()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Inicia transacao
				unitOfWork.Start();

				//Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoPendenteEmail = repFaturaIntegracao.BuscarIntegracoesPorCargaEnvioEmail(106);

				//Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarFatura(integracaoPendenteEmail, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);
				//Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
				//Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = repCargaEDIIntegracao.BuscarPorCodigo(75982);

				//Servicos.Embarcador.Integracao.EDI.VGM svcVGM = new Servicos.Embarcador.Integracao.EDI.VGM();
				//MemoryStream arquivoINPUT = svcVGM.GerarArquivoVGM(cargaEDIIntegracao, unitOfWork);

				//return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoQuestor_NFSeEntrada_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));

				// Instancia repositorios
				Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);

				// Parametros
				int.TryParse(Request.Params("Codigo"), out int codigo);

				// Busca informacoes
				Dominio.Entidades.Embarcador.Compras.CondicaoPagamento condicao = repCondicaoPagamento.BuscarPorCodigo(codigo);

				// Valida
				if (condicao == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				// Preenche entidade com dados
				PreencheEntidade(ref condicao, unitOfWork);

				// Valida entidade
				if (!ValidaEntidade(condicao, out string erro))
					return new JsonpResult(false, true, erro);

				// Persiste dados
				repCondicaoPagamento.Atualizar(condicao);
				unitOfWork.CommitChanges();

				// Retorna sucesso
				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ExcluirPorCodigo()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Inicia transacao
				unitOfWork.Start();

				// Instancia repositorios
				Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);

				// Parametros
				int.TryParse(Request.Params("Codigo"), out int codigo);

				// Busca informacoes
				Dominio.Entidades.Embarcador.Compras.CondicaoPagamento condicao = repCondicaoPagamento.BuscarPorCodigo(codigo);

				// Valida
				if (condicao == null)
					return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

				// Persiste dados
				repCondicaoPagamento.Deletar(condicao, Auditado);
				unitOfWork.CommitChanges();

				// Retorna informacoes
				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> EnviarWhatsApp()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				string descricao = Request.Params("Descricao") ?? string.Empty;
				string observacao = Request.Params("Observacao") ?? string.Empty;
				string numeroEnvio = "5511948377404";

				System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

				HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(CondicaoPagamentoController));

                requisicao.BaseAddress = new Uri("https://api.zenvia.com/v2/channels/whatsapp/messages");
				requisicao.DefaultRequestHeaders.Accept.Clear();
				requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				requisicao.DefaultRequestHeaders.Add("X-API-TOKEN", "mLAB3UB4tfFbqUrxC5-bMwBg1JNhJSIgihgj");

				string jsonRequest = "{ \"from\": \"" + numeroEnvio + "\", \"to\": \"" + descricao + "\", \"contents\": [  { \"type\": \"text\", \"text\": \"" + observacao + "\"  } ] }";

				StringContent conteudoRequisicao = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
				HttpResponseMessage retornoRequisicao = requisicao.PostAsync("https://api.zenvia.com/v2/channels/whatsapp/messages", conteudoRequisicao).Result;
				string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

				dynamic retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
				if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
				{
					return new JsonpResult(true);
				}
				else
				{
					return new JsonpResult(false, "Ocorreu uma falha ao enviar o WhatsApp dados.");
				}


			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ReprocessarDACTEsCTesImportados()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Inicia transacao
				if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
				{
					Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
					List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.ConsultarConhecimentosImportados();
					foreach (var cte in ctes)
					{
						if (cte != null)
						{
							if (cte.Status.Equals("A"))
							{
								Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
								string retorno = svcCTe.RegerarDACTE(cte.Codigo, this.Usuario.Empresa.Codigo, unitOfWork);
								Servicos.Log.TratarErro(retorno);
								if (!string.IsNullOrWhiteSpace(retorno) && (retorno.Contains("Protocolo interno informado não encontrado") || retorno.Contains("CTe sem XML para geração da DACTE.")))
								{
									svcCTe.IntegrareCTeOracle(cte.Empresa, cte.Codigo, unitOfWork);
									retorno = svcCTe.RegerarDACTE(cte.Codigo, this.Usuario.Empresa.Codigo, unitOfWork);
									Servicos.Log.TratarErro(retorno);
								}

								svcCTe.ObterESalvarDACTE(cte, cte.Empresa.Codigo, null, unitOfWork);
							}
						}
					}
				}

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		public async Task<IActionResult> ReprocessarDataPreviaVencimento()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				// Inicia transacao
				if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
				{
					Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
					List<int> ctes = repCTe.ConsultarConhecimentosNaoImportados();
					Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

					int contador = 0;
					foreach (var codigo in ctes)
					{
						Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

						if (cte != null)
						{
							unitOfWork.Start();

							int? diasPrazoFatura = 0;
							int? diaMes = 0;
							bool? permiteFinalSemana = false;
							FormaTitulo formaTitulo = FormaTitulo.Outros;
							Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? diaSemana = null;
							Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento? tipoPrazoFatura = null;

							List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasSemana = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();
							List<int> diasMes = new List<int>();

							servFatura.RetornarParametrosFaturamento(cte, unitOfWork, true, out diaMes, out diaSemana, out permiteFinalSemana, out diasPrazoFatura, out diasSemana, out diasMes, out tipoPrazoFatura, 0, out formaTitulo);
							DateTime? dataBaseParcela = servFatura.RetornarDataBase(tipoPrazoFatura, cte, true, unitOfWork);

							if (dataBaseParcela.HasValue && (diaMes != null || diaSemana != null || diasPrazoFatura != null || tipoPrazoFatura != null || diasMes.Count > 0 || diasSemana.Count > 0))
								cte.DataPreviaVencimento = servFatura.RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes, cte.TomadorPagador.Cliente, cte.TomadorPagador.GrupoPessoas, false, unitOfWork);

							repCTe.Atualizar(cte);


							unitOfWork.CommitChanges();

							contador += 1;
							if ((contador % 10) == 0)
							{
								unitOfWork.FlushAndClear();

								unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
							}
						}
					}

				}

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}


		#endregion

		#region Métodos Privados


		/* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
		private Models.Grid.Grid GridPesquisa()
		{
			// Manipula grids
			Models.Grid.Grid grid = new Models.Grid.Grid(Request)
			{
				header = new List<Models.Grid.Head>()
			};

			// Cabecalhos grid
			grid.Prop("Codigo");
			grid.Prop("Descricao").Nome("Descrição").Tamanho(35).Align(Models.Grid.Align.left);
			grid.Prop("Ativo").Nome("Status").Tamanho(25).Align(Models.Grid.Align.left);
			grid.Prop("QuantidadeParcelas").Nome("Qnt. Parcelas").Tamanho(20).Align(Models.Grid.Align.left);
			grid.Prop("IntervaloDias").Nome("Intervalo de dias").Tamanho(25).Align(Models.Grid.Align.left);

			return grid;
		}

		/* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
		private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
		{
			// Instancia repositorios
			Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);

			// Dados do filtro
			Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);

			string descricao = Request.Params("Descricao");

			int codigoEmpresa = 0;
			if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
				codigoEmpresa = this.Usuario.Empresa.Codigo;

			// Consulta
			List<Dominio.Entidades.Embarcador.Compras.CondicaoPagamento> listaGrid = repCondicaoPagamento.Consultar(codigoEmpresa, descricao, status, propOrdenar, dirOrdena, inicio, limite);
			totalRegistros = repCondicaoPagamento.ContarConsulta(codigoEmpresa, descricao, status);

			var lista = from obj in listaGrid
						select new
						{
							obj.Codigo,
							obj.Descricao,
							Ativo = obj.DescricaoAtivo,
							obj.QuantidadeParcelas,
							obj.IntervaloDias
						};

			return lista.ToList();
		}

		/* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
		private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Compras.CondicaoPagamento condicao, Repositorio.UnitOfWork unitOfWork)
		{
			string descricao = Request.Params("Descricao") ?? string.Empty;
			bool.TryParse(Request.Params("Status"), out bool ativo);
			int.TryParse(Request.Params("QuantidadeParcelas"), out int quantidadeParcelas);
			string intervaloDias = Request.Params("IntervaloDias") ?? string.Empty;
            int.TryParse(Request.Params("DiasParaPrimeiroVencimento"), out int diasParaPrimeiroVencimento);
            string observacao = Request.Params("Observacao") ?? string.Empty;

			// Vincula dados
			condicao.Descricao = descricao;
			condicao.Ativo = ativo;
			condicao.QuantidadeParcelas = quantidadeParcelas;
			condicao.IntervaloDias = intervaloDias;
			condicao.DiasParaPrimeiroVencimento = diasParaPrimeiroVencimento;
			condicao.Observacao = observacao;

			condicao.Empresa = this.Usuario.Empresa;
		}

		/* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
		private bool ValidaEntidade(Dominio.Entidades.Embarcador.Compras.CondicaoPagamento condicao, out string msgErro)
		{
			msgErro = "";

			if (string.IsNullOrWhiteSpace(condicao.Descricao))
			{
				msgErro = "Descrição é obrigatória.";
				return false;
			}

			return true;
		}

		/* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
		private void PropOrdena(ref string propOrdenar)
		{
		}

		//private int[] NumerosFaturas()
		//{
		//    string numero = @"1705,
		//                    1692,
		//                    1781,
		//                    1880,
		//                    1880,
		//                    1786,
		//                    1882,
		//                    16129,
		//                    16130,
		//                    16164,
		//                    10907,
		//                    10908,
		//                    10909,
		//                    10910,
		//                    10911,
		//                    10887,
		//                    10888,
		//                    10889,
		//                    10890,
		//                    10891,
		//                    10892,
		//                    10893,
		//                    10894,
		//                    10895,
		//                    10896,
		//                    10897,
		//                    10898,
		//                    10899,
		//                    10900,
		//                    10901,
		//                    10902,
		//                    10903,
		//                    10904,
		//                    10905,
		//                    10906,
		//                    10912,
		//                    10913,
		//                    10914,
		//                    10915,
		//                    10916,
		//                    10917,
		//                    10918,
		//                    10919,
		//                    10920,
		//                    10921,
		//                    10922,
		//                    10923,
		//                    10924,
		//                    10925,
		//                    10926,
		//                    10927,
		//                    10928,
		//                    10929,
		//                    10930,
		//                    10931,
		//                    10932,
		//                    10933,
		//                    10934,
		//                    10935,
		//                    10936,
		//                    10937,
		//                    10938,
		//                    10939,
		//                    10940,
		//                    10941,
		//                    10942,
		//                    10943,
		//                    23101,
		//                    16307,
		//                    16308,
		//                    16309,
		//                    16310,
		//                    16311,
		//                    19645,
		//                    19648,
		//                    2760,
		//                    2762,
		//                    2769,
		//                    2771,
		//                    12174,
		//                    12175,
		//                    12176,
		//                    12177,
		//                    12178,
		//                    12179,
		//                    12180,
		//                    12181,
		//                    12182,
		//                    12183,
		//                    12184,
		//                    12185,
		//                    12186,
		//                    12187,
		//                    12188,
		//                    12189,
		//                    12190,
		//                    12191,
		//                    12192,
		//                    12193,
		//                    12194,
		//                    12195,
		//                    12196,
		//                    12197,
		//                    12198,
		//                    12199,
		//                    12200,
		//                    12201,
		//                    12202,
		//                    16370,
		//                    16359,
		//                    19749,
		//                    12214,
		//                    12215,
		//                    12216,
		//                    12217,
		//                    12218,
		//                    12219,
		//                    19769,
		//                    17285,
		//                    17300,
		//                    17298,
		//                    17298,
		//                    17298,
		//                    17299,
		//                    17299,
		//                    17299,
		//                    12203,
		//                    12204,
		//                    12205,
		//                    12206,
		//                    12208,
		//                    12209,
		//                    12210,
		//                    12211,
		//                    12212,
		//                    12213,
		//                    19707,
		//                    19708,
		//                    19748,
		//                    19748,
		//                    19748,
		//                    14336,
		//                    14337,
		//                    17302,
		//                    17303,
		//                    17304,
		//                    14317,
		//                    14318,
		//                    14319,
		//                    14320,
		//                    14321,
		//                    14322,
		//                    14323,
		//                    14324,
		//                    14325,
		//                    14326,
		//                    14327,
		//                    14328,
		//                    14329,
		//                    14330,
		//                    14331,
		//                    14332,
		//                    14333,
		//                    14334,
		//                    14335,
		//                    14395,
		//                    14396,
		//                    14397,
		//                    14398,
		//                    14399,
		//                    14400,
		//                    14401,
		//                    14402,
		//                    14403,
		//                    14404,
		//                    14405,
		//                    14406,
		//                    14407,
		//                    14408,
		//                    14338,
		//                    14409,
		//                    14410,
		//                    13502,
		//                    13503,
		//                    13504,
		//                    13505,
		//                    13506,
		//                    13507,
		//                    13508,
		//                    13509,
		//                    13510,
		//                    13511,
		//                    13512,
		//                    13513,
		//                    13514,
		//                    13515,
		//                    13516,
		//                    13517,
		//                    13518,
		//                    13519,
		//                    13520,
		//                    13521,
		//                    13522,
		//                    13523,
		//                    13524,
		//                    13525,
		//                    13526,
		//                    13527,
		//                    13528,
		//                    13529,
		//                    13530,
		//                    13531,
		//                    13532,
		//                    18550,
		//                    2991,
		//                    2992,
		//                    2993,
		//                    2994,
		//                    3230,
		//                    3231,
		//                    3232,
		//                    3233,
		//                    3234,
		//                    3235,
		//                    3236,
		//                    3237,
		//                    3238,
		//                    2995,
		//                    2996,
		//                    2997,
		//                    2998,
		//                    1843,
		//                    1860,
		//                    1843,
		//                    1877,
		//                    1886,
		//                    3229,
		//                    1870,
		//                    21170,
		//                    21156,
		//                    21126,
		//                    21126,
		//                    21126,
		//                    21152,
		//                    21153,
		//                    1693,
		//                    1693,
		//                    1693,
		//                    1693,
		//                    1693,
		//                    1693,
		//                    1693,
		//                    1693,
		//                    1666,
		//                    1765,
		//                    1694,
		//                    21741,
		//                    21742,
		//                    21743,
		//                    21744,
		//                    21745,
		//                    21746,
		//                    21661,
		//                    21734,
		//                    21708,
		//                    21708,
		//                    21708,
		//                    21688,
		//                    21735,
		//                    21735,
		//                    21735,
		//                    21736,
		//                    21736,
		//                    21736,
		//                    21689,
		//                    21737,
		//                    21737,
		//                    21737,
		//                    21738,
		//                    21738,
		//                    21738,
		//                    21739,
		//                    21739,
		//                    21739,
		//                    21740,
		//                    21740,
		//                    21740,
		//                    21660,
		//                    21752,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    11408,
		//                    2821,
		//                    7377,
		//                    7295,
		//                    7296,
		//                    12947,
		//                    12948,
		//                    12949,
		//                    12950,
		//                    12951,
		//                    12952,
		//                    12953,
		//                    12956,
		//                    12957,
		//                    12958,
		//                    12959,
		//                    12960,
		//                    12962,
		//                    12967,
		//                    12968,
		//                    12969,
		//                    12970,
		//                    12971,
		//                    15919,
		//                    15923,
		//                    15925,
		//                    15928,
		//                    15929,
		//                    15930,
		//                    15931,
		//                    2120,
		//                    2447,
		//                    9232,
		//                    2123,
		//                    2439,
		//                    2440,
		//                    2465,
		//                    2438,
		//                    1863,
		//                    3218,
		//                    3223,
		//                    19285,
		//                    19634,
		//                    23490,
		//                    21157,
		//                    21158,
		//                    21045,
		//                    21046,
		//                    21132,
		//                    21133,
		//                    21134,
		//                    19714,
		//                    19715,
		//                    19734,
		//                    19724,
		//                    19509,
		//                    19510,
		//                    19511,
		//                    19767,
		//                    23418,
		//                    23457,
		//                    19374,
		//                    19351,
		//                    21168,
		//                    21160,
		//                    21160,
		//                    21160,
		//                    21160,
		//                    21169,
		//                    19741,
		//                    19762,
		//                    19763,
		//                    19608,
		//                    19609,
		//                    19610,
		//                    19675,
		//                    19676,
		//                    19764,
		//                    23218,
		//                    23460,
		//                    16212,
		//                    16213,
		//                    16214,
		//                    16215,
		//                    16216,
		//                    16217,
		//                    16218,
		//                    16219,
		//                    16220,
		//                    16221,
		//                    16222,
		//                    16223,
		//                    16224,
		//                    16225,
		//                    16226,
		//                    16227,
		//                    16228,
		//                    16229,
		//                    16230,
		//                    16231,
		//                    16232,
		//                    16233,
		//                    16234,
		//                    16235,
		//                    16236,
		//                    16237,
		//                    16238,
		//                    16239,
		//                    16240,
		//                    16241,
		//                    21686,
		//                    21687,
		//                    21719,
		//                    12974,
		//                    3123,
		//                    1854,
		//                    1854,
		//                    2812,
		//                    19542,
		//                    19542,
		//                    19542,
		//                    9187,
		//                    1848,
		//                    1850,
		//                    1875,
		//                    1875,
		//                    1850,
		//                    19512,
		//                    3085,
		//                    7845,
		//                    7836,
		//                    7633,
		//                    7779,
		//                    7634,
		//                    7844,
		//                    7838,
		//                    7839,
		//                    7840,
		//                    7841,
		//                    7842,
		//                    7843,
		//                    14877,
		//                    14878,
		//                    14879,
		//                    14880,
		//                    14881,
		//                    14882,
		//                    14883,
		//                    14884,
		//                    14885,
		//                    14886,
		//                    14887,
		//                    14888,
		//                    14889,
		//                    14890,
		//                    12897,
		//                    19234,
		//                    24616,
		//                    23416,
		//                    23450,
		//                    23352,
		//                    23417,
		//                    23423,
		//                    23525,
		//                    19458,
		//                    19459,
		//                    23099,
		//                    23526,
		//                    23529,
		//                    23527,
		//                    23530,
		//                    23190,
		//                    23190,
		//                    23190,
		//                    23100,
		//                    23452,
		//                    23351,
		//                    23528,
		//                    23456,
		//                    23485,
		//                    23306,
		//                    23307,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1725,
		//                    1743,
		//                    1743,
		//                    1743,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    1674,
		//                    19705,
		//                    19471,
		//                    19472,
		//                    19717,
		//                    19718,
		//                    19473,
		//                    19474,
		//                    19475,
		//                    19476,
		//                    19477,
		//                    19478,
		//                    19479,
		//                    19480,
		//                    19481,
		//                    19482,
		//                    19483,
		//                    19484,
		//                    19485,
		//                    19486,
		//                    19487,
		//                    19488,
		//                    19489,
		//                    19490,
		//                    19491,
		//                    19755,
		//                    19756,
		//                    19757,
		//                    19492,
		//                    19493,
		//                    19494,
		//                    19495,
		//                    19758,
		//                    19759,
		//                    19760,
		//                    19761,
		//                    2623,
		//                    2623,
		//                    2624,
		//                    2624,
		//                    2625,
		//                    2625,
		//                    2626,
		//                    2627,
		//                    2628,
		//                    13680,
		//                    13661,
		//                    13662,
		//                    7595,
		//                    2031,
		//                    2032,
		//                    2033,
		//                    2034,
		//                    2035,
		//                    2036,
		//                    2037,
		//                    2038,
		//                    2372,
		//                    2373,
		//                    2374,
		//                    2774,
		//                    2775,
		//                    2776,
		//                    2777,
		//                    9159,
		//                    9160,
		//                    9161,
		//                    9185,
		//                    11831,
		//                    11909,
		//                    11913,
		//                    11914,
		//                    19321,
		//                    11193,
		//                    11091,
		//                    21022,
		//                    21023,
		//                    21146,
		//                    21031,
		//                    21032,
		//                    23239,
		//                    23239,
		//                    23240,
		//                    23240,
		//                    23284,
		//                    23285,
		//                    23286,
		//                    23281,
		//                    23282,
		//                    23287,
		//                    23288,
		//                    23289,
		//                    23290,
		//                    23291,
		//                    23292,
		//                    23293,
		//                    23294,
		//                    23295,
		//                    23296,
		//                    23297,
		//                    23298,
		//                    23299,
		//                    21109,
		//                    21024,
		//                    21025,
		//                    21026,
		//                    21110,
		//                    21027,
		//                    21028,
		//                    21029,
		//                    21030,
		//                    21033,
		//                    21034,
		//                    21035,
		//                    21036,
		//                    21037,
		//                    21038,
		//                    21039,
		//                    21151,
		//                    21040,
		//                    21041,
		//                    21042,
		//                    21043,
		//                    21044,
		//                    12789,
		//                    12790,
		//                    12917,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    1764,
		//                    15946,
		//                    15947,
		//                    16030,
		//                    15948,
		//                    15949,
		//                    15950,
		//                    15951,
		//                    13602,
		//                    15365,
		//                    15366,
		//                    15367,
		//                    15368,
		//                    15369,
		//                    15370,
		//                    15371,
		//                    15372,
		//                    15373,
		//                    15374,
		//                    15352,
		//                    15352,
		//                    15353,
		//                    15353,
		//                    15354,
		//                    15354,
		//                    1741,
		//                    1742,
		//                    1742,
		//                    1742,
		//                    1742,
		//                    1742,
		//                    1742,
		//                    1742,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    1805,
		//                    23353,
		//                    23108,
		//                    23109,
		//                    23110,
		//                    23111,
		//                    23112,
		//                    23113,
		//                    23114,
		//                    23115,
		//                    23116,
		//                    23117,
		//                    23118,
		//                    23119,
		//                    23120,
		//                    23121,
		//                    23122,
		//                    23123,
		//                    23124,
		//                    23125,
		//                    23228,
		//                    23229,
		//                    23230,
		//                    23231,
		//                    23232,
		//                    23233,
		//                    23234,
		//                    23235,
		//                    23522,
		//                    23126,
		//                    23127,
		//                    23128,
		//                    23129,
		//                    23130,
		//                    23131,
		//                    23132,
		//                    23133,
		//                    23134,
		//                    23135,
		//                    23136,
		//                    23137,
		//                    23138,
		//                    23139,
		//                    23140,
		//                    23141,
		//                    23142,
		//                    23143,
		//                    23144,
		//                    23145,
		//                    23146,
		//                    23147,
		//                    23148,
		//                    23149,
		//                    23150,
		//                    23151,
		//                    23152,
		//                    23153,
		//                    23154,
		//                    23155,
		//                    23156,
		//                    23157,
		//                    23158,
		//                    23159,
		//                    23160,
		//                    23161,
		//                    23162,
		//                    23163,
		//                    23164,
		//                    23165,
		//                    23166,
		//                    23167,
		//                    23168,
		//                    23169,
		//                    23170,
		//                    23171,
		//                    23172,
		//                    23173,
		//                    23174,
		//                    23175,
		//                    23176,
		//                    13660,
		//                    13663,
		//                    13664,
		//                    13713,
		//                    11061,
		//                    14459,
		//                    14460,
		//                    14490,
		//                    14471,
		//                    14466,
		//                    14472,
		//                    14473,
		//                    16371,
		//                    23255,
		//                    23256,
		//                    23257,
		//                    23258,
		//                    23259,
		//                    23260,
		//                    23261,
		//                    23262,
		//                    12728,
		//                    12729,
		//                    12730,
		//                    12731,
		//                    12732,
		//                    12733,
		//                    12734,
		//                    12735,
		//                    12736,
		//                    12737,
		//                    12738,
		//                    12739,
		//                    12740,
		//                    12741,
		//                    12742,
		//                    12743,
		//                    12744,
		//                    12745,
		//                    12746,
		//                    12747,
		//                    12748,
		//                    12755,
		//                    12757,
		//                    12758,
		//                    12759,
		//                    12767,
		//                    12768,
		//                    12769,
		//                    12770,
		//                    12771,
		//                    15363,
		//                    15364,
		//                    15350,
		//                    15350,
		//                    15350,
		//                    15351,
		//                    15351,
		//                    15351,
		//                    15346,
		//                    15345,
		//                    15345,
		//                    15345,
		//                    20596,
		//                    20596,
		//                    20596,
		//                    20596,
		//                    20597,
		//                    20597,
		//                    20597,
		//                    20597,
		//                    20598,
		//                    20598,
		//                    20598,
		//                    20598,
		//                    20599,
		//                    20599,
		//                    20599,
		//                    20599,
		//                    20600,
		//                    20600,
		//                    20600,
		//                    20600,
		//                    20601,
		//                    20601,
		//                    20601,
		//                    20601,
		//                    20602,
		//                    20602,
		//                    20602,
		//                    20602,
		//                    20603,
		//                    20603,
		//                    20603,
		//                    20603,
		//                    20604,
		//                    20604,
		//                    20604,
		//                    20604,
		//                    20605,
		//                    20605,
		//                    20605,
		//                    20605,
		//                    20606,
		//                    20606,
		//                    20606,
		//                    20606,
		//                    20607,
		//                    20607,
		//                    20607,
		//                    20607,
		//                    20608,
		//                    20608,
		//                    20608,
		//                    20608,
		//                    20609,
		//                    20609,
		//                    20609,
		//                    20609,
		//                    20610,
		//                    20610,
		//                    20610,
		//                    20610,
		//                    19235,
		//                    19234,
		//                    19233,
		//                    19232,
		//                    19231,
		//                    19230,
		//                    19229,
		//                    19228,
		//                    19227,
		//                    19226,
		//                    19225,
		//                    19224,
		//                    19223,
		//                    19222,
		//                    19221,
		//                    19220,
		//                    19219,
		//                    19218,
		//                    19217,
		//                    12773,
		//                    12772,
		//                    12771,
		//                    12770,
		//                    12769,
		//                    12768,
		//                    12767,
		//                    12766,
		//                    12765,
		//                    12764,
		//                    12763,
		//                    12762,
		//                    12761,
		//                    12760,
		//                    12759,
		//                    12758,
		//                    12757,
		//                    12756,
		//                    12755,
		//                    12754,
		//                    12753,
		//                    12752,
		//                    12751,
		//                    12750,
		//                    12749,
		//                    12748,
		//                    12747,
		//                    12746,
		//                    12745,
		//                    12744,
		//                    12743,
		//                    12742,
		//                    12741,
		//                    12740,
		//                    12739,
		//                    12738,
		//                    12737,
		//                    12736,
		//                    12735,
		//                    12734,
		//                    12733,
		//                    12732,
		//                    12731,
		//                    12730,
		//                    12729,
		//                    12728,
		//                    12727,
		//                    12726,
		//                    12725,
		//                    12724,
		//                    12723,
		//                    12722,
		//                    12721,
		//                    12720,
		//                    12719,
		//                    12718,
		//                    12717,
		//                    12716,
		//                    19216,
		//                    19215,
		//                    19214,
		//                    12715,
		//                    12714,
		//                    12713,
		//                    12712,
		//                    12711,
		//                    12710,
		//                    12709,
		//                    12708,
		//                    12707,
		//                    12706,
		//                    12705,
		//                    12704,
		//                    12703,
		//                    12702,
		//                    12701,
		//                    12700,
		//                    12699,
		//                    12698,
		//                    12697,
		//                    12696,
		//                    12695,
		//                    12694,
		//                    12693"; 

		//    return numero.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
		//}


		//private decimal CalcularValorSVM(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal, string chaveCTe, List<string> listaChavesCTe, int qtdDocumentos, decimal valorCusteioSVM, decimal valorFreteNegociado, decimal baseICMS, string numeroContainer, Repositorio.UnitOfWork unitOfWork, int qtdContainer)
		//{
		//    Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
		//    Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

		//    decimal valorFrete = 1;
		//    if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
		//    {
		//        decimal pesoTotalNotas = repContainerCTE.BuscarPesoNotasPorCTe(listaChavesCTe, numeroContainer);
		//        decimal volumeM3Notas = repContainerCTE.BuscarMetrosCubicosNotasPorCTe(listaChavesCTe, numeroContainer);
		//        decimal dencidadeProduto = repPedidoCTeParaSubContratacao.ObterDencidadeProdutoPedido(chaveCTe);
		//        decimal dencidadeParametroFixo = 300;
		//        decimal valorBaseCalculoSVM = 0;
		//        if (pesoTotalNotas >= (volumeM3Notas * dencidadeParametroFixo))
		//            valorBaseCalculoSVM = (valorFreteNegociado * (pesoTotalNotas / 1000));
		//        else
		//            valorBaseCalculoSVM = (valorFreteNegociado * ((volumeM3Notas * dencidadeProduto) / 100));

		//        valorFrete = (valorFreteNegociado - valorCusteioSVM - valorBaseCalculoSVM);
		//        valorFrete = (valorFrete / baseICMS);
		//        if (valorFrete <= 0)
		//            valorFrete = 1;
		//        else
		//            valorFrete = (valorFrete * qtdContainer);
		//    }
		//    else
		//    {
		//        valorFrete = (valorFreteNegociado - valorCusteioSVM);
		//        valorFrete = (valorFrete / baseICMS);
		//        if (valorFrete <= 0)
		//            valorFrete = 1;
		//        else
		//            valorFrete = (valorFrete * qtdContainer);
		//    }
		//    return valorFrete;
		//}

		private string[] CPFsMotoristas()
		{
			string numero = @"20756,
23233";
			return numero.Split(',').Select(n => n).ToArray();
		}

		private string[] CodigosProdutos()
		{
			string numero = @"144,
167,
2520,
256,
279,
298,
303,
306,
340,
342,
4,
407,
408,
409,
458,
460,
463,
470,
537,
545,
560,
653,
6536,
680,
681,
690,
7070,
7072,
7080,
7081,
7084,
7087,
7094,
7108,
7109,
7110,
7111,
7112,
7113,
7116,
7117,
7122,
7127,
7128,
7129,
7132,
722,
750,
772,
812,
924,
937,
938";
			return numero.Split(',').Select(n => n).ToArray();
		}

		#endregion

	}
}
