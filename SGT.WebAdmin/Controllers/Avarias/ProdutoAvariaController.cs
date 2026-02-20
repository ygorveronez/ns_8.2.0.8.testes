using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/ProdutoAvaria")]
    public class ProdutoAvariaController : BaseController
    {
		#region Construtores

		public ProdutoAvariaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar a pesquisa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ProdutoSolicitacaoAvaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código", "CodigoProdutoEmbarcador", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", 20, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "GrupoProduto")
                    propOrdenar = "GrupoProduto.Descricao";

                // Dados do filtro
                string descricao = Request.Params("Descricao");
                string codigoProdutoEmbarcador = Request.Params("CodigoProdutoEmbarcador");

                int carga = Request.GetIntParam("Carga");
                List<int> codigosProdutos = null;
                if (carga > 0 && ConfiguracaoEmbarcador.PermitirLancarAvariasSomenteParaProdutosDaCarga)
                {
                    codigosProdutos = repCargaPedidoProduto.BuscarCodigosProdutosPorCarga(carga);
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                // Consulta
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaGrid = repProdutoEmbarcador.ConsultarProdutosAvaria(descricao, codigoProdutoEmbarcador, ativo, codigosProdutos, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repProdutoEmbarcador.ContarConsultaProdutosAvaria(descricao, codigoProdutoEmbarcador, ativo, codigosProdutos);

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 CodigoProdutoEmbarcador = obj.CodigoProdutoEmbarcador,
                                 Descricao = obj.Descricao,
                                 GrupoProduto = obj.GrupoProduto?.Descricao ?? string.Empty,
                             }).ToList();

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);
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
                Repositorio.Embarcador.Avarias.ProdutoAvaria repProdutoAvaria = new Repositorio.Embarcador.Avarias.ProdutoAvaria(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produtoAvaria = repProdutoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (produtoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o produto.");

                // Formata retorno
                var retorno = new
                {
                    produtoAvaria.Codigo,
                    Produto = new { produtoAvaria.ProdutoEmbarcador.Codigo, produtoAvaria.ProdutoEmbarcador.Descricao },
                    produtoAvaria.QuantidadeCaixa,
                    produtoAvaria.CaixasPallet,
                    PesoUnitario = produtoAvaria.PesoUnitario.ToString("n6"),
                    PesoCaixa = produtoAvaria.PesoCaixa.ToString("n6"),
                    ValorEstorno = produtoAvaria.ValorEstorno.ToString("n2"),
                    ValorProducao = produtoAvaria.ValorProducao.ToString("n2"),
                    PrecoTransferencia = produtoAvaria.PrecoTransferencia.ToString("n2"),
                    CustoPrimario = produtoAvaria.CustoPrimario.ToString("n2"),
                    CustoSecundario = produtoAvaria.CustoSecundario.ToString("n2"),
                    Observacao = produtoAvaria.Observacao ?? string.Empty
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutoAvaria repProdutoAvaria = new Repositorio.Embarcador.Avarias.ProdutoAvaria(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produtoAvaria = new Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria();

                // Preenche entidade com dados
                PreencheEntidade(ref produtoAvaria, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(produtoAvaria, out erro))
                    return new JsonpResult(false, true, erro);

                // Valida se ja existe algum cadastro com esse produto
                if (!repProdutoAvaria.ValidaPorProduto(produtoAvaria.ProdutoEmbarcador.Codigo))
                    return new JsonpResult(false, true, "Já existe um cadastro com esse produto.");

                // Persiste dados
                repProdutoAvaria.Inserir(produtoAvaria, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
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

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutoAvaria repProdutoAvaria = new Repositorio.Embarcador.Avarias.ProdutoAvaria(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produtoAvaria = repProdutoAvaria.BuscarPorCodigo(codigo, true);

                // Valida
                if (produtoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o produto.");

                // Preenche entidade com dados
                PreencheEntidade(ref produtoAvaria, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(produtoAvaria, out erro))
                    return new JsonpResult(false, true, erro);

                // Valida se ja existe algum cadastro com esse produto
                if (!repProdutoAvaria.ValidaPorProduto(codigo, produtoAvaria.ProdutoEmbarcador.Codigo))
                    return new JsonpResult(false, true, "Já existe um cadastro com esse produto.");

                // Persiste dados
                repProdutoAvaria.Atualizar(produtoAvaria, Auditado);
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
                Repositorio.Embarcador.Avarias.ProdutoAvaria repProdutoAvaria = new Repositorio.Embarcador.Avarias.ProdutoAvaria(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produtoAvaria = repProdutoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (produtoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o produto.");

                // Persiste dados
                repProdutoAvaria.Deletar(produtoAvaria, Auditado);
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

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProdutoAvaria();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia


                // Repositorios
                Repositorio.Embarcador.Avarias.ProdutoAvaria repProdutoAvaria = new Repositorio.Embarcador.Avarias.ProdutoAvaria(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                // Configucarção de importacao
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProdutoAvaria();

                // Erro de campo
                string erro = string.Empty;

                // Lista integrada em cada linha
                List<Dictionary<string, dynamic>> dadosLinhas = new List<Dictionary<string, dynamic>>();

                // Entidade para importacao
                List<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria> produtosAvarias = new List<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria>();

                // Chama serviço de importação
                var retorno = Servicos.Embarcador.Importacao.Importacao.ImportarInformacoes(Request, configuracoes, ref produtosAvarias, ref dadosLinhas, out erro, (dadoLinha) =>
                {
                    string codigoProdutoEmbarcador = "";
                    try { codigoProdutoEmbarcador = dadoLinha["CodigoProdutoEmbarcador"]; } 
                    catch (Exception ex) 
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter CodigoProdutoEmbarcador na importação de avarias: {ex.ToString()}", "CatchNoAction");
                    }

                    // Busca o produto pelo codigo de integracao
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();
                    if (!string.IsNullOrWhiteSpace(codigoProdutoEmbarcador))
                        produtoEmbarcador = repProdutoEmbarcador.buscarPorCodigoEmbarcador(codigoProdutoEmbarcador);

                    // Nova instancia
                    Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produto = new Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria();

                    // Tenta procurar se o produto ja esta cadastrado
                    if (produtoEmbarcador != null)
                        produto = repProdutoAvaria.BuscarPorProduto(produtoEmbarcador.Codigo);

                    // Caso não tenha nada, instancia uma nova classe
                    if (produto == null) produto = new Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria();
                    else produto.Initialize();

                    produto.ProdutoEmbarcador = produtoEmbarcador;

                    return produto;
                });

                if (retorno == null && !string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);
                else if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                retorno.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                unitOfWork.Start();

                // Insere registros
                int processados = 0;
                for (var i = 0; i < produtosAvarias.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produto = repProdutoAvaria.BuscarPorProduto(produtosAvarias[i].ProdutoEmbarcador?.Codigo ?? 0);
                    if (produto == null)
                    {
                        if (string.IsNullOrWhiteSpace(produtosAvarias[i]?.ProdutoEmbarcador?.CodigoProdutoEmbarcador))
                            continue;

                        produto = produtosAvarias[i];
                    }
                    else
                        produto.Initialize();

                    if (produto.Codigo > 0)
                    {
                        repProdutoAvaria.Atualizar(produto, Auditado);
                        processados++;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, produto, null, "Atualizou por importação.", unitOfWork);
                    }
                    else
                    {
                        if (produto.ProdutoEmbarcador != null)
                        {
                            produto.DataCadastro = DateTime.Now;
                            repProdutoAvaria.Inserir(produto, Auditado);
                            processados++;
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, produto, null, "Inseriu por importação.", unitOfWork);
                        }
                        else
                            retorno.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, mensagemFalha = "Produto não encontrado", processou = false });
                    }
                }

                // Commita alterações
                unitOfWork.CommitChanges();

                retorno.Importados = processados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoProdutoAvaria()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>() {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 1, Descricao = "Código Integração", Propriedade = "CodigoProdutoEmbarcador", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { }, CampoInformacao = true
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 2, Descricao = "Quantidade por Caixa", Propriedade = "QuantidadeCaixa", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 3, Descricao = "Caixas por Pallet", Propriedade = "CaixasPallet", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 4, Descricao = "Peso Unitário", Propriedade = "PesoUnitario", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 5, Descricao = "Peso Caixa", Propriedade = "PesoCaixa", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 6, Descricao = "Valor Estorno", Propriedade = "ValorEstorno", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 7, Descricao = "Valor Produção", Propriedade = "ValorProducao", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 8, Descricao = "Preço Transferência", Propriedade = "PrecoTransferencia", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 9, Descricao = "Custo Primário", Propriedade = "CustoPrimario", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao()
                {
                     Id = 10, Descricao = "Custo Secundário", Propriedade = "CustoSecundario", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" }
                },
            };

            return configuracoes;
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produtoAvaria, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Instancia Repositorios
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            // Converte valores
            int codigoProduto = 0;
            int.TryParse(Request.Params("Produto"), out codigoProduto);
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

            int quantidadeCaixa = 0;
            int.TryParse(Request.Params("QuantidadeCaixa"), out quantidadeCaixa);

            int caixasPallet = 0;
            int.TryParse(Request.Params("CaixasPallet"), out caixasPallet);

            decimal pesoUnitario = 0;
            decimal.TryParse(Request.Params("PesoUnitario"), out pesoUnitario);

            decimal pesoCaixa = 0;
            decimal.TryParse(Request.Params("PesoCaixa"), out pesoCaixa);

            decimal valorEstorno = 0;
            decimal.TryParse(Request.Params("ValorEstorno"), out valorEstorno);

            decimal valorProducao = 0;
            decimal.TryParse(Request.Params("ValorProducao"), out valorProducao);

            decimal precoTransferencia = 0;
            decimal.TryParse(Request.Params("PrecoTransferencia"), out precoTransferencia);

            decimal custoPrimario = 0;
            decimal.TryParse(Request.Params("CustoPrimario"), out custoPrimario);

            decimal custoSecundario = 0;
            decimal.TryParse(Request.Params("CustoSecundario"), out custoSecundario);

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            // Vincula dados
            produtoAvaria.ProdutoEmbarcador = produto;
            produtoAvaria.QuantidadeCaixa = quantidadeCaixa;
            produtoAvaria.CaixasPallet = caixasPallet;
            produtoAvaria.PesoUnitario = pesoUnitario;
            produtoAvaria.PesoCaixa = pesoCaixa;
            produtoAvaria.ValorEstorno = valorEstorno;
            produtoAvaria.ValorProducao = valorProducao;
            produtoAvaria.PrecoTransferencia = precoTransferencia;
            produtoAvaria.CustoPrimario = custoPrimario;
            produtoAvaria.CustoSecundario = custoSecundario;
            produtoAvaria.Observacao = observacao;

            if (produtoAvaria.Codigo == 0)
                produtoAvaria.DataCadastro = DateTime.Now;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produtoAvaria, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (produtoAvaria.ProdutoEmbarcador == null)
            {
                msgErro = "Nenhum produto selecionado.";
                return false;
            }

            if (produtoAvaria.Observacao.Length > 2000)
            {
                msgErro = "Observação não pode passar de 2000 caracteres.";
                return false;
            }

            return true;
        }

        private string ObterPropriedadeOrdenacao(string propriedadeOrdenacao)
        {
            if (propriedadeOrdenacao == "Produto")
                return "ProdutoEmbarcador.Descricao";
            else if (propriedadeOrdenacao == "GrupoProduto")
                return "ProdutoEmbarcador.GrupoProduto.Descricao";

            return propriedadeOrdenacao;
        }

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.ProdutoAvaria repositorioProdutoAvaria = new Repositorio.Embarcador.Avarias.ProdutoAvaria(unitOfWork);
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Produto", "Produto", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Código do Produto", "CodigoProduto", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Grupo de Produto", "GrupoProduto", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Quantidade Caixa", "QuantidadeCaixa", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Custo Primário", "CustoPrimario", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Custo Secundário", "CustoSecundario", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Peso Unitário", "PesoUnitario", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Peso Caixa", "PesoCaixa", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Cadastro", "DataCadastro", 10, Models.Grid.Align.center, true);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta();
            parametroConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenacao(parametroConsulta.PropriedadeOrdenar);

            Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaProdutoAvaria filtrosPesquisa = ObterFiltrosPesquisa();

            List<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria> listaGrid = repositorioProdutoAvaria.Consultar(filtrosPesquisa, parametroConsulta);
            int totalRegistros = repositorioProdutoAvaria.ContarConsulta(filtrosPesquisa);

            var lista = (from obj in listaGrid
                         select new
                         {
                             Codigo = obj.Codigo,
                             Produto = obj.ProdutoEmbarcador.Descricao,
                             GrupoProduto = obj.ProdutoEmbarcador.GrupoProduto?.Descricao ?? string.Empty,
                             QuantidadeCaixa = obj.QuantidadeCaixa,
                             CustoPrimario = obj.CustoPrimario.ToString("n2"),
                             CustoSecundario = obj.CustoSecundario.ToString("n2"),
                             PesoUnitario = obj.PesoUnitario.ToString("n6"),
                             PesoCaixa = obj.PesoCaixa.ToString("n6"),
                             DataCadastro = obj.DataCadastro.ToString("dd/MM/yyyy"),
                             CodigoProduto = obj.ProdutoEmbarcador.CodigoProdutoEmbarcador
                         }).ToList();


            grid.setarQuantidadeTotal(totalRegistros);
            grid.AdicionaRows(lista);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaProdutoAvaria ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaProdutoAvaria()
            {
                CodigoGrupoProduto = Request.GetIntParam("GrupoProduto"),
                CodigoProduto = Request.GetIntParam("Produto"),
                DataInicial = Request.GetDateTimeParam("DataInicio"),
                DataFinal = Request.GetDateTimeParam("DataFim")
            };
        }

        #endregion
    }
}

