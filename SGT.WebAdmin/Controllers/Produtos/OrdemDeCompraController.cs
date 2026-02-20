using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Produtos;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/OrdemDeCompra")]
    public class OrdemDeCompraController : BaseController
    {
		#region Construtores

		public OrdemDeCompraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Produtos.OrdemCompraPrincipal repostororioOrdemCompraPrincipal = new Repositorio.Embarcador.Produtos.OrdemCompraPrincipal(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal ordemDeCompraPrincipal = new Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal();

                string codigoIntegracao = Request.GetStringParam("ControleIntegracaoEmbarcador");

                ordemDeCompraPrincipal.ControleIntegracaoEmbarcador = !string.IsNullOrWhiteSpace(codigoIntegracao) ? codigoIntegracao : throw new ControllerException("Codigo de integração da ordem é obrigatorio");
                repostororioOrdemCompraPrincipal.Inserir(ordemDeCompraPrincipal);

                PreencherDocumentos(ordemDeCompraPrincipal, unitOfWork);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
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
                unitOfWork.Start();
                Repositorio.Embarcador.Produtos.OrdemCompraPrincipal repostororioOrdemCompraPrincipal = new Repositorio.Embarcador.Produtos.OrdemCompraPrincipal(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal ordemDeCompraPrincipal = repostororioOrdemCompraPrincipal.BuscarPorCodigo(Request.GetIntParam("Codigo"), false); ;

                if (ordemDeCompraPrincipal == null)
                    return new JsonpResult(false, "Registro não encontrado!.");

                string codigoIntegracao = Request.GetStringParam("ControleIntegracaoEmbarcador");

                ordemDeCompraPrincipal.ControleIntegracaoEmbarcador = !string.IsNullOrWhiteSpace(codigoIntegracao) ? codigoIntegracao : ordemDeCompraPrincipal.ControleIntegracaoEmbarcador;
                repostororioOrdemCompraPrincipal.Atualizar(ordemDeCompraPrincipal);
                PreencherDocumentos(ordemDeCompraPrincipal, unitOfWork);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Produtos.OrdemCompraDocumento repositorioOrdemCompraDocumento = new Repositorio.Embarcador.Produtos.OrdemCompraDocumento(unitOfWork);
                Repositorio.Embarcador.Produtos.OrdemCompraItem repositorioOrdemCompraItem = new Repositorio.Embarcador.Produtos.OrdemCompraItem(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");


                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento ordeCompraDocumentos = repositorioOrdemCompraDocumento.BuscarPorCodigo(codigo, false);
                List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem> ordemCompraItem = repositorioOrdemCompraItem.BuscarPorCodigoOrdemDocumento(codigo);

                if (ordeCompraDocumentos == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");
                var retorno = new
                {
                    Codigo = ordeCompraDocumentos.Codigo,
                    ControleIntegracaoEmbarcador = ordeCompraDocumentos.OrdemDeCompraPrincipal.ControleIntegracaoEmbarcador,
                    OrdemItem = (from obj in ordemCompraItem
                                 select new
                                 {
                                     obj.Codigo,
                                     DataAlteracao = obj?.DataAlteracao?.ToString("dd/MM/yyyy") ?? "",
                                     NumeroItemDocumento = obj?.NumeroItemDocumento ?? "",
                                     CodigoProdutoEmbarcador = obj?.Produto?.CodigoProdutoEmbarcador ?? "",
                                     EntregaConcluida = (obj?.EntregaConcluida ?? false) ? "Entrega Concluida" : "Entrega não concluida",
                                     ProdutoProduzidoInternamente = (obj?.ProdutoProduzidoInternamente ?? false) ? "SIM" : "NÃO",
                                     QuantidadeOrdemCompra = obj?.QuantidadeOrdemCompra ?? 0m,
                                     LimiteTolerancia = obj?.LimiteTolerancia ?? 0m,
                                     Tolerancia = $"{obj.LimiteTolerancia}",
                                     TotalItemsDisponiveis = ConsultarTotalHistorico(obj, unitOfWork)
                                 }).ToList()
                };
                return new JsonpResult(retorno);

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Produtos.OrdemCompraPrincipal repositorioOrdemCompra = new Repositorio.Embarcador.Produtos.OrdemCompraPrincipal(unitOfWork);
                Repositorio.Embarcador.Produtos.OrdemCompraDocumento repositorioOrdemCompraDocumento = new Repositorio.Embarcador.Produtos.OrdemCompraDocumento(unitOfWork);

                int codigoOrdemPrincipal = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal ordemDeCompraPrincipal = repositorioOrdemCompra.BuscarPorCodigo(codigoOrdemPrincipal, false);
                if (ordemDeCompraPrincipal == null)
                    return new JsonpResult(false, "Registro Não encontrado");

                List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento> ordeCompraDocumentos = repositorioOrdemCompraDocumento.BuscarPorCodigoOrdemPrincipal(codigoOrdemPrincipal);

                foreach (var item in ordeCompraDocumentos)
                    repositorioOrdemCompraDocumento.Deletar(item);

                repositorioOrdemCompra.Deletar(ordemDeCompraPrincipal);


                return new JsonpResult(true);
            }
            catch (Exception ex)
            {

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");

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

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Produtos.OrdemCompraPrincipal repositorioOrdemCompra = new Repositorio.Embarcador.Produtos.OrdemCompraPrincipal(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 30, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal ordeCompra = repositorioOrdemCompra.BuscarPorCodigo(codigo, false);
                if (ordeCompra == null)
                    return new JsonpResult(false, "Não encontrado registro para esta ordem de compra");

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> arquivosIntegracao = ordeCompra?.ArquivosTransacao.ToList();

                var retorno = (from obj in arquivosIntegracao
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data,
                                   Mensagem = obj.Mensagem

                               }).ToList();

                grid.setarQuantidadeTotal(arquivosIntegracao.Count);
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracao = repositorioArquivo.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(true, false, "Arquivo  não localizado");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Integracao PO Data" + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do histórico");
            }
        }

        public async Task<IActionResult> ConsultarHistoricoMovimentacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Produtos.OrdemCompraItem repositorioOrdemCompra = new Repositorio.Embarcador.Produtos.OrdemCompraItem(unitOfWork);
                Repositorio.Embarcador.Produtos.OrdemDeCompraHistorial repositorioOrdemCompraHistorial = new Repositorio.Embarcador.Produtos.OrdemDeCompraHistorial(unitOfWork);
                Models.Grid.Grid grid = new Models.Grid.Grid();

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Item no Documento", "ItemNoDocumento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Ano Documento", "AnoDocumento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Categoria", "CategoriaHistorico", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Movimento", "TipoMovimento", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Lançamento", "DataLancamentoDocumento", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Operação", "Operacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Codigo Produto", "CodigoProduto", 20, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem ordeCompra = repositorioOrdemCompra.BuscarPorCodigo(codigo, false);
                var parametroPesquisa = grid.ObterParametrosConsulta();
                if (ordeCompra == null)
                    return new JsonpResult(false, "Não encontrado registro para este item");

                List<Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial> historial = repositorioOrdemCompraHistorial.BuscarPorNumeroItemDocumento(ordeCompra.OrdemDeCompraDocumento.NumeroDocumento, ordeCompra?.Produto?.CodigoProdutoEmbarcador, parametroPesquisa);

                dynamic retorno = (from obj in historial
                                   select new
                                   {
                                       obj.Codigo,
                                       ItemNoDocumento = obj?.ItemNoDocumento ?? "",
                                       AnoDocumento = obj?.AnoDocumento ?? 0m,
                                       CategoriaHistorico = obj.CategoriaHistorico ?? "",
                                       TipoMovimento = obj?.TipoMovimento ?? "",
                                       DataLancamentoDocumento = obj?.DataLancamentoDocumento?.ToString("dd/MM/yyyy hh:mm:ss") ?? "",
                                       Quantidade = obj?.Quantidade ?? 0m,
                                       Operacao = obj?.Operacao ?? "",
                                       CodigoProduto = obj?.Produto?.CodigoProdutoEmbarcador ?? "",
                                       DT_RowColor = obj.Operacao == "H" ? CorGrid.Danger : CorGrid.Success
                                   }).ToList();

                grid.setarQuantidadeTotal(retorno?.Count ?? 0);
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
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Produtos.OrdemCompraPrincipal repositorioOrdemCompra = new Repositorio.Embarcador.Produtos.OrdemCompraPrincipal(unitOfWork);
                Repositorio.Embarcador.Produtos.OrdemCompraDocumento repositorioOrdemCompraDocumento = new Repositorio.Embarcador.Produtos.OrdemCompraDocumento(unitOfWork);

                FiltroPesquisaOrdemDeCompra filtroPesquisa = ObterFiltroPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Codigo Controle Integração", "ControleIntegracaoEmbarcador", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 5, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 5, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Numero Ordem", "NumeroOrdem", 5, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Inicio Validade", "InicioValidade", 5, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Fim Validade", "FimValidade", 5, Models.Grid.Align.left, false,true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioOrdemCompra.ContarConsulta(filtroPesquisa);
                List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal> OrdemsDeCompras = repositorioOrdemCompra.Consultar(filtroPesquisa, parametrosConsulta);

                List<dynamic> lista = new List<dynamic>();

                foreach (var ordemDeCompra in OrdemsDeCompras)
                {
                    var listadaDocumentos = repositorioOrdemCompraDocumento.BuscarPorCodigoOrdemPrincipal(ordemDeCompra.Codigo);
                    foreach (var documento in listadaDocumentos)
                    {
                        lista.Add(new
                        {
                            ControleIntegracaoEmbarcador = ordemDeCompra.ControleIntegracaoEmbarcador,
                            documento.Codigo,
                            Filial = documento?.Filial?.Descricao ?? "",
                            Fornecedor = documento?.Fornecedor?.Descricao ?? "",
                            NumeroOrdem = documento?.NumeroDocumento ?? "",
                            InicioValidade = documento?.InicioValidade?.ToString("dd/MM/yyyy") ?? "",
                            FimValidade = documento?.FinValidade?.ToString("dd/MM/yyyy") ?? ""
                        });
                    }

                }

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(lista.Count);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void PreencherDocumentos(Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal ordemCompraPrincipal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.OrdemCompraDocumento repositorioOrdeCompraDocumento = new Repositorio.Embarcador.Produtos.OrdemCompraDocumento(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Cliente repostorioCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento> existeListaDocumentoPorOrdeProduto = repositorioOrdeCompraDocumento.BuscarPorCodigoOrdemPrincipal(ordemCompraPrincipal.Codigo);

            dynamic documentosOrdemCompra = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OrdemDocumento"));

            if (existeListaDocumentoPorOrdeProduto != null && existeListaDocumentoPorOrdeProduto.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic documentoOrdeCompra in documentosOrdemCompra)
                    if (documentoOrdeCompra.Codigo != null)
                        codigos.Add((int)documentoOrdeCompra.Codigo);

                List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento> ordemCompraDocumentosRemover = existeListaDocumentoPorOrdeProduto.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (var ordemRemover in ordemCompraDocumentosRemover)
                    repositorioOrdeCompraDocumento.Deletar(ordemRemover);
            }
            else
                existeListaDocumentoPorOrdeProduto = new List<Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento>();

            foreach (var documentoOrdemCompra in documentosOrdemCompra)
            {
                Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento existeOrdeCompraDocumento = repositorioOrdeCompraDocumento.BuscarPorCodigo((int)documentoOrdemCompra.Codigo, false);

                if (existeOrdeCompraDocumento == null)
                    existeOrdeCompraDocumento = new Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento();

                Dominio.Entidades.Embarcador.Filiais.Filial existeFilial = repositorioFilial.BuscarPorCodigo((int)documentoOrdemCompra.FilialCodigo);
                if (existeFilial == null)
                    throw new ControllerException($"A filial informado num dos documentos não existe");

                Dominio.Entidades.Cliente existeCliente = repostorioCliente.BuscarPorCPFCNPJ((double)documentoOrdemCompra.CodigoFornecedor);
                if (existeCliente == null)
                    throw new ControllerException($"A Fornecedor informado num dos documentos não existe");

                DateTime.TryParse((string)documentoOrdemCompra.InicioValidade, out DateTime inicioValidade);
                DateTime.TryParse((string)documentoOrdemCompra.FinValidade, out DateTime finValidade);

                existeOrdeCompraDocumento.NumeroDocumento = (string)documentoOrdemCompra.NumeroDocumento;
                existeOrdeCompraDocumento.Fornecedor = existeCliente;
                existeOrdeCompraDocumento.Filial = existeFilial;
                existeOrdeCompraDocumento.InicioValidade = inicioValidade;
                existeOrdeCompraDocumento.FinValidade = finValidade;
                existeOrdeCompraDocumento.OrdemDeCompraPrincipal = ordemCompraPrincipal;

                if (existeOrdeCompraDocumento.Codigo > 0)
                    repositorioOrdeCompraDocumento.Atualizar(existeOrdeCompraDocumento);
                else
                    repositorioOrdeCompraDocumento.Inserir(existeOrdeCompraDocumento);

            }
        }
        public FiltroPesquisaOrdemDeCompra ObterFiltroPesquisa()
        {
            return new FiltroPesquisaOrdemDeCompra
            {
                CodigoControleIntegracao = Request.GetStringParam("NumeroOrdem"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoFornecedor = Request.GetDoubleParam("Fornecedor")
            };
        }

        private string ConsultarTotalHistorico(Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem ordemCompraItem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.OrdemDeCompraHistorial repositorioOrdemCompraHistorial = new Repositorio.Embarcador.Produtos.OrdemDeCompraHistorial(unitOfWork);
            List<Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial> historial = repositorioOrdemCompraHistorial.BuscarPorNumeroItemDocumento(ordemCompraItem?.OrdemDeCompraDocumento.NumeroDocumento, ordemCompraItem?.Produto?.CodigoProdutoEmbarcador, null);
            decimal totalValorItem = ordemCompraItem.QuantidadeOrdemCompra + (ordemCompraItem.LimiteTolerancia / 100);
            decimal totalValorCredito = historial.Where(x => x.Operacao == "H").Select(x => x.Quantidade).Sum();
            decimal totalValorDebito = historial.Where(x => x.Operacao == "S").Select(x => x.Quantidade).Sum();

            decimal totalValor = (totalValorItem + totalValorCredito) + totalValorDebito;

            return totalValor.ToString("n2");
        }

        #endregion


    }
}
