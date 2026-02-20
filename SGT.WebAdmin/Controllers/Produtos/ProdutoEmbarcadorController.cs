using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/ProdutoEmbarcador")]
    public class ProdutoEmbarcadorController : BaseController
    {
        #region Construtores

        public ProdutoEmbarcadorController(Conexao conexao) : base(conexao) { }

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

                // Retorna Dados
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string descricao = Request.Params("Descricao");
                string codigoBarras = Request.Params("CodigoBarras");
                string numero = Request.Params("Numero");
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("QuantidadeLote", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Produtos.ProdutoEmbarcador.NumeroLote, "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Produtos.ProdutoEmbarcador.CodigoDeBarras, "CodigoBarras", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Produtos.ProdutoEmbarcador.DataVencimento, "DataVencimento", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Produtos.ProdutoEmbarcador.QtdAtual, "QuantidadeAtual", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Produtos.ProdutoEmbarcador.Armazenamento, "DepositoPosicao", 20, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote> listaLote = repProdutoEmbarcadorLote.Consultar(descricao, codigoBarras, numero, ativo, 0, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repProdutoEmbarcadorLote.ContarConsulta(descricao, codigoBarras, numero, ativo, 0));
                var lista = (from p in listaLote
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.Numero,
                                 p.CodigoBarras,
                                 DataVencimento = p.DataVencimento != null && p.DataVencimento.HasValue ? p.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 QuantidadeAtual = p.QuantidadeAtual.ToString("n3"),
                                 QuantidadeLote = p.QuantidadeLote.ToString("n3"),
                                 DepositoPosicao = p.DepositoPosicao.Abreviacao
                             }).ToList();
                grid.AdicionaRows(lista);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
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

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Produtos.ProdutoEmbarcador.OcorreuUmaFalhaExportar);
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
                unitOfWork.Start();

                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();

                PreencheEntidade(produtoEmbarcador, unitOfWork);

                string erro;
                if (!ValidaEntidade(produtoEmbarcador, out erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                if (repProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoEmbarcador.CodigoProdutoEmbarcador) != null)
                    return new JsonpResult(false, true, Localization.Resources.Produtos.ProdutoEmbarcador.JaExisteProdutoComMesmoCodigoIntegracao);

                repProdutoEmbarcador.Inserir(produtoEmbarcador, Auditado);

                SalvarFiliais(produtoEmbarcador, unitOfWork);
                SalvarOrganizacoes(produtoEmbarcador, unitOfWork);
                SalvarTabelaConversoes(produtoEmbarcador, unitOfWork);
                SalvarFornecedorProduto(produtoEmbarcador, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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

                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigo, true);

                if (produtoEmbarcador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencheEntidade(produtoEmbarcador, unitOfWork);

                SalvarFiliais(produtoEmbarcador, unitOfWork);
                SalvarOrganizacoes(produtoEmbarcador, unitOfWork);
                SalvarTabelaConversoes(produtoEmbarcador, unitOfWork);
                SalvarFornecedorProduto(produtoEmbarcador, unitOfWork);

                string erro;
                if (!ValidaEntidade(produtoEmbarcador, out erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Valida codigo de integração duplicado
                if (repProdutoEmbarcador.ValidarProdutoPorIntegracao(produtoEmbarcador.CodigoProdutoEmbarcador, codigo))
                    return new JsonpResult(false, true, Localization.Resources.Produtos.ProdutoEmbarcador.JaExisteProdutoComMesmoCodigoIntegracao);

                repProdutoEmbarcador.Atualizar(produtoEmbarcador, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigo);

                // Valida
                if (produtoEmbarcador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                // Formata retorno
                var retorno = new
                {
                    produtoEmbarcador.Codigo,
                    produtoEmbarcador.TipoPessoa,
                    Pessoa = produtoEmbarcador.Cliente != null ? new { Codigo = produtoEmbarcador.Cliente.CPF_CNPJ, Descricao = produtoEmbarcador.Cliente.Descricao } : null,
                    GrupoPessoa = produtoEmbarcador.GrupoPessoas != null ? new { Codigo = produtoEmbarcador.GrupoPessoas.Codigo, Descricao = produtoEmbarcador.GrupoPessoas.Descricao } : null,
                    GrupoProduto = produtoEmbarcador.GrupoProduto != null ? new { Codigo = produtoEmbarcador.GrupoProduto.Codigo, Descricao = produtoEmbarcador.GrupoProduto.Descricao } : null,
                    CheckList = produtoEmbarcador.CheckList != null ? new { produtoEmbarcador.CheckList.Codigo, produtoEmbarcador.CheckList.Descricao } : new { Codigo = 1, Descricao = "Teste" },
                    produtoEmbarcador.Descricao,
                    produtoEmbarcador.Observacao,
                    produtoEmbarcador.ExibirExpedicaoEmTempoReal,
                    produtoEmbarcador.DescontarPesoProdutoCalculoFrete,
                    produtoEmbarcador.DescontarValorProdutoCalculoFrete,
                    produtoEmbarcador.CodigoNCM,
                    produtoEmbarcador.ObrigatorioGuiaTransporteAnimal,
                    produtoEmbarcador.ObrigatorioNFProdutor,
                    UnidadeDeMedida = produtoEmbarcador.Unidade != null ? new { Codigo = produtoEmbarcador.Unidade.Codigo, Descricao = produtoEmbarcador.Unidade.Descricao } : null,
                    produtoEmbarcador.SiglaUnidade,
                    FatorConversao = produtoEmbarcador.FatorConversao.ToString("n5"),
                    CodigoIntegracao = produtoEmbarcador.CodigoProdutoEmbarcador,
                    produtoEmbarcador.CodigoDocumentacao,
                    TipoCarga = produtoEmbarcador.TipoDeCarga != null ? new { Codigo = produtoEmbarcador.TipoDeCarga.Codigo, Descricao = produtoEmbarcador.TipoDeCarga.Descricao } : null,
                    produtoEmbarcador.TemperaturaTransporte,
                    produtoEmbarcador.Ativo,
                    PesoUnitario = produtoEmbarcador.PesoUnitario.ToString("n3"),
                    PesoLiquidoUnitario = produtoEmbarcador.PesoLiquidoUnitario.ToString("n3"),
                    QtdPalet = produtoEmbarcador.QtdPalet.ToString("n3"),
                    produtoEmbarcador.QuantidadeCaixa,
                    produtoEmbarcador.QuantidadeCaixaPorPallet,
                    AlturaCM = produtoEmbarcador.AlturaCM.ToString("n3"),
                    LarguraCM = produtoEmbarcador.LarguraCM.ToString("n3"),
                    ComprimentoCM = produtoEmbarcador.ComprimentoCM.ToString("n3"),
                    MetroCubito = produtoEmbarcador.MetroCubito.ToString("n4"),
                    ClassificacaoRiscoONU = produtoEmbarcador.ClassificacaoRiscoONU != null ? new { Codigo = produtoEmbarcador.ClassificacaoRiscoONU.Codigo, Descricao = produtoEmbarcador.ClassificacaoRiscoONU.Descricao } : null,
                    LinhaSeparacao = produtoEmbarcador.LinhaSeparacao != null ? new { Codigo = produtoEmbarcador.LinhaSeparacao.Codigo, Descricao = produtoEmbarcador.LinhaSeparacao.Descricao } : null,
                    TipoEmbalagem = produtoEmbarcador.TipoEmbalagem != null ? new { Codigo = produtoEmbarcador.TipoEmbalagem.Codigo, Descricao = produtoEmbarcador.TipoEmbalagem.Descricao } : null,
                    produtoEmbarcador.PossuiIntegracaoColetaMobile,
                    produtoEmbarcador.ObrigatorioInformarTemperatura,
                    produtoEmbarcador.ExigeInformarCaixas,
                    produtoEmbarcador.ExigeInformarImunos,
                    produtoEmbarcador.QuantidadeCaixaPorCamadaPallet,
                    ConfiguracaoPalletizacao = produtoEmbarcador.ConfiguracaoPalletizacao != null ? new { Codigo = produtoEmbarcador.ConfiguracaoPalletizacao.Codigo, Descricao = produtoEmbarcador.ConfiguracaoPalletizacao.Descricao } : null,
                    MarcaProduto = produtoEmbarcador.MarcaProduto != null ? new { Codigo = produtoEmbarcador.MarcaProduto.Codigo, Descricao = produtoEmbarcador.MarcaProduto.Descricao } : null,
                    Lotes = (from obj in produtoEmbarcador.Lotes
                             where obj.TipoRecebimentoMercadoria == null || obj.TipoRecebimentoMercadoria == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 Descricao = obj.Descricao,
                                 Numero = string.IsNullOrWhiteSpace(obj.Numero) ? "1" : obj.Numero,
                                 CodigoBarras = !string.IsNullOrWhiteSpace(obj.CodigoBarras) ? obj.CodigoBarras : "",
                                 DataVencimento = obj.DataVencimento != null && obj.DataVencimento.HasValue ? obj.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 QuantidadeLote = obj.QuantidadeLote.ToString("n3"),
                                 QuantidadeAtual = obj.QuantidadeAtual.ToString("n3"),
                                 DepositoPosicao = obj.DepositoPosicao.Abreviacao
                             }).ToList(),
                    Volumes = (from obj in produtoEmbarcador.Lotes
                               where obj.TipoRecebimentoMercadoria == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Descricao = obj.Descricao,
                                   Numero = string.IsNullOrWhiteSpace(obj.Numero) ? "1" : obj.Numero,
                                   CodigoBarras = !string.IsNullOrWhiteSpace(obj.CodigoBarras) ? obj.CodigoBarras : "",
                                   QuantidadeAtual = obj.QuantidadeAtual.ToString("n3"),
                                   Remetente = obj.Remetente != null ? obj.Remetente.Nome + " (" + obj.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                   DepositoPosicao = obj.DepositoPosicao.Abreviacao
                               }).ToList(),
                    Clientes = (from obj in produtoEmbarcador.Clientes
                                select new
                                {
                                    Codigo = obj.Codigo,
                                    CodigoBarras = !string.IsNullOrWhiteSpace(obj.CodigoBarras) ? obj.CodigoBarras : "",
                                    Cliente = obj.Cliente != null ? "(" + obj.Cliente.CPF_CNPJ_Formatado + ") " + obj.Cliente.Nome : string.Empty
                                }).ToList(),
                    Filiais = (from obj in produtoEmbarcador.Filiais
                               select new
                               {
                                   obj.Codigo,
                                   CodigoFilial = obj.Filial?.Codigo ?? 0,
                                   Filial = obj.Filial?.Descricao ?? string.Empty,
                                   Situacao = obj.Ativo,
                                   NCM = obj?.NCM ?? string.Empty,
                                   SituacaoCodigo = obj?.FilialSituacao?.SituacaoFilial ?? SituacaoFilial.SemSitaucao,
                                   SituacaoDescricaoCodigo = obj?.FilialSituacao?.SituacaoFilial.ObterDescricao() ?? SituacaoFilial.SemSitaucao.ObterDescricao(),
                                   UsoMaterial = obj?.UsoMaterial ?? 0,
                                   UsoMaterialDescricao = obj?.UsoMaterial.ObterDescricao() ?? string.Empty

                               }).ToList(),
                    Organizacao = (from obj in produtoEmbarcador.Organizacoes
                                   select new
                                   {
                                       obj.Codigo,
                                       Organizacao = obj.Organizacao?.Descricao ?? string.Empty,
                                       Canal = obj.Organizacao?.Canal ?? string.Empty,
                                       Setor = obj.Organizacao?.Setor ?? string.Empty,
                                       Nivel = obj.Organizacao?.Nivel ?? string.Empty,
                                       HierarquiaCodigo = obj.Organizacao?.CodigoHierarquia ?? string.Empty,
                                       HierarquiaDescricao = obj.Organizacao?.DescricaoHierarquia ?? string.Empty,
                                   }).ToList(),
                    produtoEmbarcador.CodigoEAN,
                    CodigocEAN = produtoEmbarcador.CodigoCEAN,
                    TabelaConversao = (from obj in produtoEmbarcador.TabelaConversao
                                       select new
                                       {
                                           obj.Codigo,
                                           CodigoTipoConversao = obj.TipoConversao.Codigo,
                                           TipoConversao = $"({obj.TipoConversao.Sigla}) {obj.TipoConversao.Descricao}  ? ({obj.TipoConversao.UnidadeDeMedida.Sigla}) {obj.TipoConversao.UnidadeDeMedida.Descricao}",
                                           QteDe = obj.QuantidadeDe.ToString(),
                                           QtePara = obj.QuantidadePara.ToString()
                                       }).ToList(),
                    FornecedorProduto = ObterFornecedorProduto(produtoEmbarcador)
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
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
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigo);

                if (produtoEmbarcador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repProdutoEmbarcador.Deletar(produtoEmbarcador, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProdutoEmbarcador();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string dadosLinhas = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dadosLinhas);
                int total = linhas.Count;

                if (total == 0)
                    return new JsonpResult(false, "Nenhuma linha encontrada na planilha");

                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork, cancellationToken);

                List<string> listaCodigosProdutos = ObterValoresLinha(linhas, "CodigoProdutoEmbarcador", total);
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaProdutosExistentes = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

                for (int i = 0; i < total; i += 1000)
                {
                    listaProdutosExistentes.AddRange(await repProduto.BuscarPendentesPorCodigosEmbarcadorAsync(listaCodigosProdutos.Skip(i).Take(1000).ToList()));
                }

                List<string> listaCodigosGrupos = ObterValoresLinha(linhas, "GrupoProduto", total);
                List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> listaGruposExistentes = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();

                for (int i = 0; i < total; i += 1000)
                {
                    listaGruposExistentes.AddRange(await repGrupoProduto.BuscarPorCodigosEmbarcadorAsync(listaCodigosGrupos.Skip(i).Take(1000).ToList()));
                }

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, produtos, (dados) =>
                {
                    Servicos.Embarcador.ProdutoEmbarcador.ProdutoEmbarcadorImportacao servicoVeiculoImportar = new Servicos.Embarcador.ProdutoEmbarcador.ProdutoEmbarcadorImportacao(unitOfWork, dados, listaGruposExistentes, listaProdutosExistentes);

                    return servicoVeiculoImportar.ObterProdutoEmbarcadorImportar();
                });

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;

                foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto in produtos)
                {
                    if (produto.Codigo > 0 && permiteAtualizar)
                    {
                        await repProduto.AtualizarAsync(produto);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = produto.GetChanges();
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, produto, alteracoes, Localization.Resources.Produtos.ProdutoEmbarcador.ProdutoAtualizadoPorImportacao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, cancellationToken);
                    }
                    else if (produto.Codigo == 0 && permiteInserir)
                    {
                        await repProduto.InserirAsync(produto);
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, produto, null, Localization.Resources.Produtos.ProdutoEmbarcador.ProdutoInseridoPorImportacao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro, cancellationToken);
                    }

                    totalRegistrosImportados++;
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("UnidadeMedida", false);
            grid.AdicionarCabecalho("SiglaUnidade", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "CodigoProdutoEmbarcador", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.GrupoProduto, "GrupoProduto", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Pessoa, "Pessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.GrupoPessoas, "GrupoPessoa", 20, Models.Grid.Align.left, true);
            }
            else
            {
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.QuantidadeUnidadePorCaixa, "QuantidadeUnidadePorCaixa", 10, Models.Grid.Align.center, false);
            }
            grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.Temperatura, "Temperatura", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("PesoUnitario", false);
            grid.AdicionarCabecalho("PesoLiquidoUnitario", false);
            grid.AdicionarCabecalho("CodigoLinhaSeparacao", false);
            grid.AdicionarCabecalho("QtdPalet", false);
            grid.AdicionarCabecalho("AlturaCM", false);
            grid.AdicionarCabecalho("LarguraCM", false);
            grid.AdicionarCabecalho("ValorUnitario", false);
            grid.AdicionarCabecalho("ComprimentoCM", false);
            grid.AdicionarCabecalho("MetroCubito", false);
            grid.AdicionarCabecalho("CodigoClassificacaoRiscoONU", false);
            grid.AdicionarCabecalho("LinhaSeparacao", false);
            grid.AdicionarCabecalho("TipoEmbalagem", false);
            grid.AdicionarCabecalho("DescricaoClassificacaoRiscoONU", false);
            grid.AdicionarCabecalho("NumeroONU", false);
            grid.AdicionarCabecalho("ClasseRisco", false);
            grid.AdicionarCabecalho("RiscoSubsidiario", false);
            grid.AdicionarCabecalho("NumeroRisco", false);
            grid.AdicionarCabecalho("QuantidadeCaixaPorPallet", false);

            return grid;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoProdutoEmbarcador()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Gerais.Geral.Descricao, Propriedade = "Descricao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Gerais.Geral.CodigoIntegracao, Propriedade = "CodigoProdutoEmbarcador", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.TemperaturaTransporte, Propriedade = "TemperaturaTransporte", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.GrupoProduto, Propriedade = "GrupoProduto", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.QuantidadePorCaixa, Propriedade = "QuantidadePorCaixa", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.CodigoDocumentacao, Propriedade = "CodigoDocumentacao", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.Peso, Propriedade = "PesoUnitario", Tamanho = 150, CampoInformacao = true });
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.QtdCaixaPorPallet, Propriedade = "QuantidadeCaixaPorPallet", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.AlturaMT, Propriedade = "AlturaCM", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.LarguraMT, Propriedade = "LarguraCM", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.ComprimentoMT, Propriedade = "ComprimentoCM", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.MetroCubito, Propriedade = "MetroCubico", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Produtos.ProdutoEmbarcador.CodigoNCM, Propriedade = "CodigoNCM", Tamanho = 150, CampoInformacao = true });

            return configuracoes;
        }

        private void PreencheEntidade(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU repClassificacaoRiscoONU = new Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU(unitOfWork);
            Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);
            Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
            Repositorio.UnidadeDeMedida repUnidadeDeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Palletizacao repPalletizacao = new Repositorio.Embarcador.Configuracoes.Palletizacao(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("GrupoProduto"), out int codigoGrupoProduto);
            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = codigoGrupoProduto > 0 ? repGrupoProduto.BuscarPorCodigo(codigoGrupoProduto) : null;

            int.TryParse(Request.Params("TipoCarga"), out int codigoTipoCarga);
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = codigoTipoCarga > 0 ? repTipoDeCarga.BuscarPorCodigo(codigoTipoCarga) : null;

            int.TryParse(Request.Params("GrupoPessoa"), out int codigoGrupoPessoa);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = codigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa) : null;

            double.TryParse(Request.Params("Pessoa"), out double codigoPessoa);
            Dominio.Entidades.Cliente pessoa = codigoPessoa > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPessoa) : null;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa;
            Enum.TryParse(Request.Params("TipoPessoa"), out tipoPessoa);

            int.TryParse(Request.Params("UnidadeDeMedida"), out int codigoUnidadeDeMedida);
            Dominio.Entidades.UnidadeDeMedida unidadeDeMedida = codigoUnidadeDeMedida > 0 ? repUnidadeDeMedida.BuscarPorCodigo(codigoUnidadeDeMedida) : null;

            int codigoChecklist = Request.GetIntParam("CheckList");
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkList = codigoChecklist > 0 ? repCheckListTipo.BuscarPorCodigo(codigoChecklist) : null;

            int codigoConfiguracaoPalletizacao = Request.GetIntParam("ConfiguracaoPalletizacao");
            Dominio.Entidades.Embarcador.Configuracoes.Palletizacao configuracaoPalletizacao = codigoConfiguracaoPalletizacao > 0 ? repPalletizacao.BuscarPorCodigo(codigoConfiguracaoPalletizacao) : null;

            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            string codigoDocumentacao = Request.Params("CodigoDocumentacao");
            string temperaturaTransporte = Request.Params("TemperaturaTransporte");

            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;
            if (string.IsNullOrWhiteSpace(codigoIntegracao)) codigoIntegracao = string.Empty;
            if (string.IsNullOrWhiteSpace(codigoDocumentacao)) codigoDocumentacao = string.Empty;
            if (string.IsNullOrWhiteSpace(temperaturaTransporte)) temperaturaTransporte = string.Empty;

            bool ativo, exibirExpedicaoEmTempoReal, descontarPesoProdutoCalculoFrete, descontarValorProdutoCalculoFrete;
            bool.TryParse(Request.Params("Ativo"), out ativo);
            bool.TryParse(Request.Params("ExibirExpedicaoEmTempoReal"), out exibirExpedicaoEmTempoReal);
            bool.TryParse(Request.Params("DescontarPesoProdutoCalculoFrete"), out descontarPesoProdutoCalculoFrete);
            bool.TryParse(Request.Params("DescontarValorProdutoCalculoFrete"), out descontarValorProdutoCalculoFrete);

            decimal qtdPalet = 0, alturaCM = 0, larguraCM = 0, comprimentoCM = 0, metroCubito = 0;

            decimal.TryParse(Request.Params("QtdPalet"), out qtdPalet);
            decimal.TryParse(Request.Params("AlturaCM"), out alturaCM);
            decimal.TryParse(Request.Params("LarguraCM"), out larguraCM);
            decimal.TryParse(Request.Params("ComprimentoCM"), out comprimentoCM);
            decimal.TryParse(Request.Params("MetroCubito"), out metroCubito);

            produtoEmbarcador.QuantidadeCaixa = Request.GetIntParam("QuantidadeCaixa");
            produtoEmbarcador.QuantidadeCaixaPorPallet = Request.GetIntParam("QuantidadeCaixaPorPallet");
            produtoEmbarcador.CodigoNCM = Request.GetStringParam("CodigoNCM");
            produtoEmbarcador.CodigoEAN = Request.GetStringParam("CodigoEAN");
            produtoEmbarcador.CodigoCEAN = Request.GetStringParam("CodigocEAN");
            produtoEmbarcador.ObrigatorioGuiaTransporteAnimal = Request.GetBoolParam("ObrigatorioGuiaTransporteAnimal");
            produtoEmbarcador.ObrigatorioNFProdutor = Request.GetBoolParam("ObrigatorioNFProdutor");
            produtoEmbarcador.PesoUnitario = Request.GetDecimalParam("PesoUnitario");
            produtoEmbarcador.PesoLiquidoUnitario = Request.GetDecimalParam("PesoLiquidoUnitario");
            produtoEmbarcador.FatorConversao = Request.GetDecimalParam("FatorConversao");
            produtoEmbarcador.QuantidadeCaixaPorCamadaPallet = Request.GetIntParam("QuantidadeCaixaPorCamadaPallet");

            produtoEmbarcador.QtdPalet = qtdPalet;
            produtoEmbarcador.AlturaCM = alturaCM;
            produtoEmbarcador.LarguraCM = larguraCM;
            produtoEmbarcador.ComprimentoCM = comprimentoCM;
            produtoEmbarcador.MetroCubito = metroCubito;

            int.TryParse(Request.Params("ClassificacaoRiscoONU"), out int codigoClassificacaoRiscoONU);
            produtoEmbarcador.ClassificacaoRiscoONU = codigoClassificacaoRiscoONU > 0 ? repClassificacaoRiscoONU.BuscarPorCodigo(codigoClassificacaoRiscoONU) : null;

            int codigoTipoEmbalagem = Request.GetIntParam("TipoEmbalagem");
            produtoEmbarcador.TipoEmbalagem = codigoTipoEmbalagem > 0 ? repTipoEmbalagem.BuscarPorCodigo(codigoTipoEmbalagem) : null;

            int.TryParse(Request.Params("LinhaSeparacao"), out int codigoLinhaSeparacao);
            produtoEmbarcador.LinhaSeparacao = codigoLinhaSeparacao > 0 ? repLinhaSeparacao.BuscarPorCodigo(codigoLinhaSeparacao) : null;

            int codigoMarcaProduto = Request.GetIntParam("MarcaProduto");
            produtoEmbarcador.MarcaProduto = codigoMarcaProduto > 0 ? repMarcaProduto.BuscarPorCodigo(codigoMarcaProduto, false) : null;

            // Vincula dados
            produtoEmbarcador.Ativo = ativo;
            produtoEmbarcador.CodigoProdutoEmbarcador = codigoIntegracao;
            produtoEmbarcador.CodigoDocumentacao = codigoDocumentacao;
            produtoEmbarcador.Descricao = descricao;
            produtoEmbarcador.Integrado = false;
            produtoEmbarcador.Observacao = Request.GetStringParam("Observacao");
            produtoEmbarcador.TemperaturaTransporte = temperaturaTransporte;
            produtoEmbarcador.ExibirExpedicaoEmTempoReal = exibirExpedicaoEmTempoReal;
            produtoEmbarcador.DescontarPesoProdutoCalculoFrete = descontarPesoProdutoCalculoFrete;
            produtoEmbarcador.DescontarValorProdutoCalculoFrete = descontarValorProdutoCalculoFrete;
            produtoEmbarcador.GrupoProduto = grupoProduto;
            produtoEmbarcador.Unidade = unidadeDeMedida;
            produtoEmbarcador.ConfiguracaoPalletizacao = configuracaoPalletizacao;
            produtoEmbarcador.SiglaUnidade = Request.GetStringParam("SiglaUnidade");
            produtoEmbarcador.CheckList = checkList;
            produtoEmbarcador.PossuiIntegracaoColetaMobile = Request.GetBoolParam("PossuiIntegracaoColetaMobile");
            produtoEmbarcador.ObrigatorioInformarTemperatura = Request.GetBoolParam("ObrigatorioInformarTemperatura");
            produtoEmbarcador.ExigeInformarImunos = Request.GetBoolParam("ExigeInformarImunos");
            produtoEmbarcador.ExigeInformarCaixas = Request.GetBoolParam("ExigeInformarCaixas");

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                produtoEmbarcador.Cliente = pessoa;
                produtoEmbarcador.GrupoPessoas = grupoPessoa;
                produtoEmbarcador.TipoDeCarga = tipoCarga;
                produtoEmbarcador.TipoPessoa = tipoPessoa;
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

            msgErro = "";

            if (string.IsNullOrWhiteSpace(produtoEmbarcador.CodigoProdutoEmbarcador))
            {
                msgErro = Localization.Resources.Produtos.ProdutoEmbarcador.CodigoEobrigatorio;
                return false;
            }

            if (string.IsNullOrWhiteSpace(produtoEmbarcador.Descricao))
            {
                msgErro = Localization.Resources.Produtos.ProdutoEmbarcador.DescricaoEObrigatorio;
                return false;
            }

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !(configuracaoPedido?.PessoasNaoObrigatorioProdutoEmbarcador ?? false))
            {
                if (produtoEmbarcador.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && produtoEmbarcador.Cliente == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    msgErro = Localization.Resources.Produtos.ProdutoEmbarcador.PessoaEObrigatorio;
                    return false;
                }

                if (produtoEmbarcador.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa && produtoEmbarcador.GrupoPessoas == null)
                {
                    msgErro = Localization.Resources.Produtos.ProdutoEmbarcador.GrupoPessoasEObrigatorio;
                    return false;
                }
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
            if (propOrdenar == "GrupoProduto") propOrdenar += ".Descricao";
            else if (propOrdenar == "Pessoa") propOrdenar = "Cliente.Nome";
            else if (propOrdenar == "GrupoPessoa") propOrdenar = "GrupoPessoas.Descricao";
            else if (propOrdenar == "Situacao") propOrdenar = "Ativo";
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            int codigoGrupoPessoa = 0;
            int codigoGrupoProduto = 0;
            double codigoPessoa = 0;
            Dominio.Entidades.Cliente pessoa = null;
            bool pessoasNaoObrigatorioProdutoEmbarcador = false;

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
                double.TryParse(Request.Params("Pessoa"), out codigoPessoa);

                if (codigoPessoa > 0)
                    pessoa = repCliente.BuscarPorCPFCNPJ(codigoPessoa);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
                pessoasNaoObrigatorioProdutoEmbarcador = configuracaoPedido?.PessoasNaoObrigatorioProdutoEmbarcador ?? false;
            }

            int.TryParse(Request.Params("GrupoProduto"), out codigoGrupoProduto);

            string descricao = Request.Params("Descricao");
            string codigoProdutoEmbarcador = Request.Params("CodigoProdutoEmbarcador");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            double codigoClienteBase = Request.GetDoubleParam("ClienteBase");
            List<int> produtos = Request.GetListParam<int>("Produtos");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
            Enum.TryParse(Request.Params("Ativo"), out ativo);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaGrid = repProdutoEmbarcador.Consultar(descricao, codigoProdutoEmbarcador, pessoa, codigoGrupoPessoa, codigoGrupoProduto, codigoIntegracao, ativo, pessoasNaoObrigatorioProdutoEmbarcador, codigoClienteBase, propOrdenar, dirOrdena, inicio, limite, produtos);
            totalRegistros = repProdutoEmbarcador.ContarConsulta(descricao, codigoProdutoEmbarcador, pessoa, codigoGrupoPessoa, codigoGrupoProduto, codigoIntegracao, ativo, pessoasNaoObrigatorioProdutoEmbarcador, codigoClienteBase, produtos);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            UnidadeMedida = obj.Unidade?.Descricao ?? "",
                            obj.CodigoProdutoEmbarcador,
                            obj.Descricao,
                            GrupoProduto = obj.GrupoProduto != null ? obj.GrupoProduto.Descricao : string.Empty,
                            Pessoa = obj.Cliente != null ? obj.Cliente.Descricao : string.Empty,
                            GrupoPessoa = obj.GrupoPessoas != null ? obj.GrupoPessoas.Descricao : string.Empty,
                            Temperatura = obj.TemperaturaTransporte,
                            Situacao = obj.DescricaoAtivo,
                            PesoUnitario = obj.PesoUnitario.ToString("n3"),
                            PesoLiquidoUnitario = obj.PesoLiquidoUnitario.ToString("n3"),
                            QtdPalet = obj.QtdPalet.ToString("n3"),
                            SiglaUnidade = obj.SiglaUnidade ?? "",
                            AlturaCM = obj.AlturaCM.ToString("n3"),
                            LarguraCM = obj.LarguraCM.ToString("n3"),
                            ComprimentoCM = obj.ComprimentoCM.ToString("n3"),
                            MetroCubito = obj.MetroCubito.ToString("n4"),
                            ValorUnitario = 0m,
                            CodigoClassificacaoRiscoONU = obj.ClassificacaoRiscoONU != null ? obj.ClassificacaoRiscoONU.Codigo : 0,
                            LinhaSeparacao = obj.LinhaSeparacao != null ? obj.LinhaSeparacao.Codigo : 0,
                            TipoEmbalagem = obj.TipoEmbalagem != null ? obj.TipoEmbalagem.Codigo : 0,
                            DescricaoClassificacaoRiscoONU = obj.ClassificacaoRiscoONU != null ? obj.ClassificacaoRiscoONU.Descricao : string.Empty,
                            NumeroONU = obj.ClassificacaoRiscoONU != null ? obj.ClassificacaoRiscoONU.NumeroONU : string.Empty,
                            ClasseRisco = obj.ClassificacaoRiscoONU != null ? obj.ClassificacaoRiscoONU.ClasseRisco : string.Empty,
                            RiscoSubsidiario = obj.ClassificacaoRiscoONU != null ? obj.ClassificacaoRiscoONU.RiscoSubsidiario : string.Empty,
                            NumeroRisco = obj.ClassificacaoRiscoONU != null ? obj.ClassificacaoRiscoONU.NumeroRisco : string.Empty,
                            QuantidadeUnidadePorCaixa = obj.QuantidadeCaixa,
                            CodigoLinhaSeparacao = obj.LinhaSeparacao?.Codigo ?? 0,
                            obj.QuantidadeCaixaPorPallet,
                            MarcaProduto = obj.MarcaProduto != null ? obj.MarcaProduto.Codigo : 0,
                        };

            return lista.ToList();
        }

        private void SalvarTabelaConversoes(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcardor = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Produtos.ConversaoDeUnidade repositorioConversaoDeUnidades = new Repositorio.Embarcador.Produtos.ConversaoDeUnidade(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao repositorioTabelaConversao = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao(unitOfWork);

            dynamic TabelaConversao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TabelaConversao"));

            if (produtoEmbarcador.TabelaConversao != null && produtoEmbarcador.TabelaConversao.Count > 0)
            {
                List<int> listaCodigo = new List<int>();
                foreach (var conversao in TabelaConversao)
                    if ((int)conversao.Codigo > 0)
                        listaCodigo.Add((int)conversao.Codigo);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao> listaTabelaConversaoRemover = produtoEmbarcador.TabelaConversao.Where(t => !listaCodigo.Contains(t.Codigo)).ToList();

                foreach (var conversaoRemover in listaTabelaConversaoRemover)
                    produtoEmbarcador.TabelaConversao.Remove(conversaoRemover);
            }
            else
                produtoEmbarcador.TabelaConversao = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao>();

            foreach (var conversao in TabelaConversao)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao exiteConversaoNoProduto = produtoEmbarcador.TabelaConversao.Where(c => c.Codigo == (int)conversao.Codigo).FirstOrDefault();

                if (exiteConversaoNoProduto == null)
                    exiteConversaoNoProduto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao();

                Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade tipoConversaoUnidade = repositorioConversaoDeUnidades.BuscarPorCodigo((int)conversao.CodigoTipoConversao, false);

                if (tipoConversaoUnidade == null)
                    throw new ControllerException("Tipo de conversão não encontrada");

                decimal.TryParse((string)conversao.QuantidadePara, out decimal quantidadePara);
                decimal.TryParse((string)conversao.QuantidadeDe, out decimal quantidadeDe);

                exiteConversaoNoProduto.QuantidadePara = quantidadePara;
                exiteConversaoNoProduto.QuantidadeDe = quantidadeDe;
                exiteConversaoNoProduto.TipoConversao = tipoConversaoUnidade;

                if (exiteConversaoNoProduto.Codigo > 0)
                    repositorioTabelaConversao.Atualizar(exiteConversaoNoProduto);
                else
                {
                    repositorioTabelaConversao.Inserir(exiteConversaoNoProduto);
                    produtoEmbarcador.TabelaConversao.Add(exiteConversaoNoProduto);
                }
            }

            repositorioProdutoEmbarcardor.Atualizar(produtoEmbarcador);
        }

        #endregion

        #region Métodos Privados

        private void SalvarFiliais(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFilial repositorioProdutoEmbarcadorFilial = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFilial(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcardorFilialSituacao repFiliaisSituacoes = new Repositorio.Embarcador.Produtos.ProdutoEmbarcardorFilialSituacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes> listaSituacoes = repFiliaisSituacoes.BuscarTodos();

            dynamic dynFiliais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            if (produtoEmbarcador.Filiais?.Count > 0)
            {
                List<int> codigos = new List<int>();
                foreach (dynamic filial in dynFiliais)
                    if (filial.Codigo != null)
                        codigos.Add((int)filial.Codigo);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial> produtosEmbarcadorFilialDeletar = (from obj in produtoEmbarcador.Filiais where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (var produtoEmbarcadorFilialDeletar in produtosEmbarcadorFilialDeletar)
                    repositorioProdutoEmbarcadorFilial.Deletar(produtoEmbarcadorFilialDeletar);
            }

            foreach (dynamic dynFilial in dynFiliais)
            {
                int codigoProdutoEmbarcadorFilial = ((string)dynFilial.Codigo).ToInt();
                int codigoFilial = ((string)dynFilial.CodigoFilial).ToInt();

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial produtoEmbarcadorFilial = codigoProdutoEmbarcadorFilial > 0 ? repositorioProdutoEmbarcadorFilial.BuscarPorCodigo(codigoProdutoEmbarcadorFilial, false) : new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial();

                Dominio.Entidades.Embarcador.Filiais.Filial filial = codigoFilial > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilial) : null;

                if (filial == null)
                    continue;

                produtoEmbarcadorFilial.Filial = filial;
                produtoEmbarcadorFilial.ProdutoEmbarcador = produtoEmbarcador;
                produtoEmbarcadorFilial.Ativo = ((string)dynFilial.Situacao).ToBool();
                SituacaoFilial situacao = (SituacaoFilial)dynFilial.SituacaoCodigo;
                produtoEmbarcadorFilial.NCM = (string)dynFilial.NCM;
                produtoEmbarcadorFilial.UsoMaterial = (UsoMaterial)dynFilial.UsoMaterial;

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes exiteSituacaoCadastrada = listaSituacoes.Where(s => s.SituacaoFilial == situacao).FirstOrDefault();
                if (exiteSituacaoCadastrada == null)
                {
                    exiteSituacaoCadastrada = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes()
                    {
                        SituacaoFilial = situacao,
                        Descricao = situacao.ObterDescricao()
                    };
                    repFiliaisSituacoes.Inserir(exiteSituacaoCadastrada);
                }
                produtoEmbarcadorFilial.FilialSituacao = exiteSituacaoCadastrada;

                if (produtoEmbarcadorFilial.Codigo > 0)
                    repositorioProdutoEmbarcadorFilial.Atualizar(produtoEmbarcadorFilial);
                else
                    repositorioProdutoEmbarcadorFilial.Inserir(produtoEmbarcadorFilial);
            }
        }

        private void SalvarOrganizacoes(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao repositorioProdutoEmbarcadorOrganizacao = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao(unitOfWork);
            Repositorio.Embarcador.Produtos.Organizacao repositorioOrganizacao = new Repositorio.Embarcador.Produtos.Organizacao(unitOfWork);

            dynamic dynOrganizacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Organizacao"));

            if (produtoEmbarcador.Organizacoes?.Count > 0)
            {
                List<int> codigos = new List<int>();
                foreach (dynamic organizacao in dynOrganizacoes)
                    if (organizacao.Codigo != null)
                        codigos.Add((int)organizacao.Codigo);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao> produtosEmbarcadorFilialDeletar = (from obj in produtoEmbarcador.Organizacoes where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (var produtoEmbarcadorFilialDeletar in produtosEmbarcadorFilialDeletar)
                    repositorioProdutoEmbarcadorOrganizacao.Deletar(produtoEmbarcadorFilialDeletar);
            }

            foreach (dynamic dynOrganizacao in dynOrganizacoes)
            {
                int codigoProdutoEmbarcadorOrganizacao = ((string)dynOrganizacao.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao produtoEmbarcadorOrganizacao = codigoProdutoEmbarcadorOrganizacao > 0 ? repositorioProdutoEmbarcadorOrganizacao.BuscarPorCodigo(codigoProdutoEmbarcadorOrganizacao, false) : new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao();

                Dominio.Entidades.Embarcador.Produtos.Organizacao organizacao = new Dominio.Entidades.Embarcador.Produtos.Organizacao();

                organizacao.Descricao = (string)dynOrganizacao.Organizacao;
                organizacao.Canal = (string)dynOrganizacao.Canal;
                organizacao.Setor = (string)dynOrganizacao.Setor;
                organizacao.Nivel = (string)dynOrganizacao.Nivel;
                organizacao.CodigoHierarquia = (string)dynOrganizacao.HierarquiaCodigo;
                organizacao.DescricaoHierarquia = (string)dynOrganizacao.HierarquiaDescricao;

                //Se não existe uma organização com a mesma composição da informada, cria-se uma
                Dominio.Entidades.Embarcador.Produtos.Organizacao organizacaoExistente = repositorioOrganizacao.OrganizacaoExistente(organizacao);
                if (organizacaoExistente == null)
                    repositorioOrganizacao.Inserir(organizacao);
                else
                    organizacao = organizacaoExistente;

                produtoEmbarcadorOrganizacao.Organizacao = organizacao;
                produtoEmbarcadorOrganizacao.ProdutoEmbarcador = produtoEmbarcador;

                if (produtoEmbarcadorOrganizacao.Codigo > 0)
                    repositorioProdutoEmbarcadorOrganizacao.Atualizar(produtoEmbarcadorOrganizacao);
                else
                    repositorioProdutoEmbarcadorOrganizacao.Inserir(produtoEmbarcadorOrganizacao);
            }

        }

        private void SalvarFornecedorProduto(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFornecedor repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFornecedor(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            dynamic produtosFornecedor = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FornecedorProduto"));

            if (produtoEmbarcador.FornecedorInternoProduto != null && produtoEmbarcador.FornecedorInternoProduto.Count > 0)
            {
                List<int> listaCodigo = new List<int>();
                foreach (var fornecedor in produtosFornecedor)
                    if ((int)fornecedor.Codigo > 0)
                        listaCodigo.Add((int)fornecedor.Codigo);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor> listaProdutoEmbarcadorFornecedorRemover = produtoEmbarcador.FornecedorInternoProduto.Where(t => !listaCodigo.Contains(t.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor produtoEmbarcadorFornecedorRemover in listaProdutoEmbarcadorFornecedorRemover)
                    repositorioProdutoEmbarcador.Deletar(produtoEmbarcadorFornecedorRemover);
            }

            foreach (dynamic produtoFornecedor in produtosFornecedor)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor exiteFornecedorInternoNoProduto = repositorioProdutoEmbarcador.BuscarPorCodigo((int)produtoFornecedor.Codigo, auditavel: false);

                if (exiteFornecedorInternoNoProduto == null)
                    exiteFornecedorInternoNoProduto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor()
                    {
                        ProdutoEmbarcador = produtoEmbarcador
                    };

                exiteFornecedorInternoNoProduto.CodigoInterno = produtoFornecedor?.CodigoInternoProduto ?? string.Empty;
                exiteFornecedorInternoNoProduto.Filial = (int)produtoFornecedor.CodigoFilial > 0 ? repositorioFilial.BuscarPorCodigo((int)produtoFornecedor.CodigoFilial) : null;
                exiteFornecedorInternoNoProduto.Fornecedor = (double)produtoFornecedor.CodigoFornecedor > 0 ? repositorioCliente.BuscarPorCPFCNPJ((double)produtoFornecedor.CodigoFornecedor) : null;

                if (exiteFornecedorInternoNoProduto.Codigo > 0)
                    repositorioProdutoEmbarcador.Atualizar(exiteFornecedorInternoNoProduto);
                else
                    repositorioProdutoEmbarcador.Inserir(exiteFornecedorInternoNoProduto);
            }
        }

        private dynamic ObterFornecedorProduto(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador)
        {
            return (
                from obj in produtoEmbarcador.FornecedorInternoProduto
                select new
                {
                    obj.Codigo,
                    CodigoFilial = obj.Filial != null ? obj.Filial.Codigo : 0,
                    CodigoFornecedor = obj.Fornecedor != null ? obj.Fornecedor.Codigo : 0,
                    Fornecedor = obj.Fornecedor != null ? obj.Fornecedor.Descricao : string.Empty,
                    Filial = obj.Filial != null ? obj.Filial.Descricao : string.Empty,
                    CodigoInternoProduto = obj?.CodigoInterno ?? string.Empty
                }
            ).ToList();
        }

        private List<string> ObterValoresLinha(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas, string linhaPesquisar, int totalRegistros)
        {
            List<string> valoresRetorno = new List<string>();

            for (int i = 0; i < totalRegistros; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDados = (from obj in linhas[i].Colunas where obj.NomeCampo == linhaPesquisar select obj).FirstOrDefault();
                if (colDados != null)
                    valoresRetorno.Add(((string)colDados.Valor).Trim());
            }

            return valoresRetorno;
        }

        #endregion
    }
}
