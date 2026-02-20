using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize(new string[] { "BuscarPorCodigo", "PesquisaTipoCombustivel", "PesquisaTipoCombustivelTMS", "PesquisaNCMS", "PesquisaCESTS", "EnviarFoto", "InativarFoto", "AdicionarProdutoLote", "CarregarFotoProduto" }, "Produtos/Produto")]
    public class ProdutoController : BaseController
    {
		#region Construtores

		public ProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Produto.Produto serProduto = new Servicos.Embarcador.Produto.Produto(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao natureza = null;
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos repositorioConfiguracaoAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos configuracaoAbastecimento = repositorioConfiguracaoAbastecimento.BuscarConfiguracaoPadrao();

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("NaturezaOperacao"), out int codigoNatureza);
                int.TryParse(Request.Params("OrdemCompra"), out int codigoOrdemCompra);
                int codigoLocalArmazenamento = Request.GetIntParam("LocalArmazenamento");

                bool.TryParse(Request.Params("SomenteComEstoque"), out bool somenteComEstoque);

                string codigoBarrasEAN = Request.Params("CodigoBarrasEAN");
                string descricao = Request.Params("Descricao");
                string codigoProdutoOriginal = Request.Params("CodigoProdutoEmbarcador");
                Boolean IsCodigoDeBarras = VerificarSeCodigoDeBarras(codigoProdutoOriginal);
                string codigoProdutoEmbarcador = serProduto.ContornarLeituraCodigoBarras(codigoProdutoOriginal);
                string codigoProduto = Request.Params("CodigoProduto");
                string codigoNCM = Request.Params("CodigoNCM");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : SituacaoAtivoPesquisa.Ativo;

                TipoAbastecimento tipoAbastecimentoAux;
                TipoAbastecimento? tipoAbastecimento = null;
                if (Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimentoAux))
                    tipoAbastecimento = tipoAbastecimentoAux;

                if (codigoNatureza > 0)
                    natureza = repNaturezaDaOperacao.BuscarPorId(codigoNatureza);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.right, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.CodigoProduto, "CodigoProduto", 15, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.LocalArmazenamento, "LocalArmazenamentoProduto", 15, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.NCM, "CodigoNCM", 10, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.Unidade, "UnidadeMedida", 15, Models.Grid.Align.left, false, false, true);
                if (ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false, true, true);

                grid.AdicionarCabecalho("CodigoUnidadeMedida", false);
                grid.AdicionarCabecalho("UltimoCusto", false);
                grid.AdicionarCabecalho("UltimoCustoCombustivel", false);
                grid.AdicionarCabecalho("ValorVenda", false);
                grid.AdicionarCabecalho("CodigoCFOP", false);
                grid.AdicionarCabecalho("CFOP", false);

                grid.AdicionarCabecalho("CodigoANP", false);
                grid.AdicionarCabecalho("PercentualGLP", false);
                grid.AdicionarCabecalho("PercentualGNN", false);
                grid.AdicionarCabecalho("PercentualGNI", false);
                grid.AdicionarCabecalho("ValorPartidaANP", false);
                grid.AdicionarCabecalho("PercentualOrigemCombustivel", false);
                grid.AdicionarCabecalho("PercentualMisturaBiodiesel", false);
                grid.AdicionarCabecalho("IndicadorEscalaRelevante", false);
                grid.AdicionarCabecalho("IndicadorImportacaoCombustivel", false);
                grid.AdicionarCabecalho("CNPJFabricante", false);
                grid.AdicionarCabecalho("CodigoBeneficioFiscal", false);
                grid.AdicionarCabecalho("ValorMinimoVenda", false);
                grid.AdicionarCabecalho("GrupoProduto", false);
                grid.AdicionarCabecalho("OrigemMercadoria", false);
                grid.AdicionarCabecalho("DescricaoNotaFiscal", false);

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                List<Dominio.Entidades.Produto> listaProduto = repProduto.Consulta(0, codigoBarrasEAN, codigo, codigoProduto, descricao, codigoNCM, ativo, tipoAbastecimento, codigoEmpresa, codigoProdutoEmbarcador, codigoOrdemCompra, somenteComEstoque, IsCodigoDeBarras, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, codigoLocalArmazenamento);
                grid.setarQuantidadeTotal(repProduto.ContaConsulta(0, codigoBarrasEAN, codigo, codigoProduto, descricao, codigoNCM, ativo, tipoAbastecimento, codigoEmpresa, codigoProdutoEmbarcador, codigoOrdemCompra, somenteComEstoque, IsCodigoDeBarras, codigoLocalArmazenamento));
                var lista = (from p in listaProduto
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoProduto,
                                 p.Descricao,
                                 LocalArmazenamentoProduto = p.LocalArmazenamentoProduto?.Descricao,
                                 p.CodigoNCM,
                                 p.DescricaoStatus,
                                 UnidadeMedida = UnidadeDeMedidaHelper.ObterDescricao(p.UnidadeDeMedida),
                                 CodigoUnidadeMedida = p.UnidadeDeMedida.HasValue ? p.UnidadeDeMedida.Value : UnidadeDeMedida.Unidade,
                                 ValorVenda = p.ValorVenda.ToString("n" + this.Usuario.Empresa?.CasasValorProdutoNFe.ToString() ?? "2"),
                                 UltimoCusto = configuracaoAbastecimento.UtilizarCustoMedioParaLancamentoAbastecimentos ? p.CustoMedio.ToString("n4") : p.UltimoCusto.ToString("n4"),
                                 UltimoCustoCombustivel = configuracaoAbastecimento.UtilizarCustoMedioParaLancamentoAbastecimentos ? p.CustoMedio.ToString("n4") : p.UltimoCusto.ToString("n4"),
                                 CodigoCFOP = natureza != null && natureza.CFOP != null ? natureza.CFOP.Codigo : 0,
                                 CFOP = natureza != null && natureza.CFOP != null ? natureza.CFOP.CodigoCFOP : 0,
                                 p.CodigoANP,
                                 PercentualGLP = p.PercentualGLP.ToString("n4"),
                                 PercentualGNN = p.PercentualGNN.ToString("n4"),
                                 PercentualGNI = p.PercentualGNI.ToString("n4"),
                                 PercentualOrigemCombustivel = p.PercentualOrigemCombustivel.ToString("n4"),
                                 PercentualMisturaBiodiesel = p.PercentualMisturaBiodiesel.ToString("n4"),
                                 ValorPartidaANP = p.ValorPartidaANP.ToString("n2"),
                                 p.IndicadorEscalaRelevante,
                                 p.IndicadorImportacaoCombustivel,
                                 p.CNPJFabricante,
                                 p.CodigoBeneficioFiscal,
                                 ValorMinimoVenda = p.ValorMinimoVenda.ToString("n2"),
                                 GrupoProduto = p.GrupoProdutoTMS?.Descricao ?? string.Empty,
                                 p.OrigemMercadoria,
                                 p.DescricaoNotaFiscal
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
        public async Task<IActionResult> PesquisaComEstoque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);

                int codigoNatureza = Request.GetIntParam("NaturezaOperacao");

                string codigoBarrasEAN = Request.GetStringParam("CodigoBarrasEAN");
                string descricao = Request.GetStringParam("Descricao");
                string codigoProdutoEmbarcador = Request.GetStringParam("CodigoProdutoEmbarcador");
                string codigoNCM = Request.GetStringParam("CodigoNCM");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                SituacaoAtivoPesquisa ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo);

                Dominio.Entidades.NaturezaDaOperacao natureza = null;
                if (codigoNatureza > 0)
                    natureza = repNaturezaDaOperacao.BuscarPorId(codigoNatureza);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.right, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.CodigoProduto, "CodigoProduto", 15, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.NCM, "CodigoNCM", 10, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.Unidade, "UnidadeMedida", 15, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.EstoqueAtual, "Quantidade", 15, Models.Grid.Align.left, true);
                if (ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false, true, true);

                grid.AdicionarCabecalho("CodigoUnidadeMedida", false);
                grid.AdicionarCabecalho("UltimoCusto", false);
                grid.AdicionarCabecalho("UltimoCustoCombustivel", false);
                grid.AdicionarCabecalho("ValorVenda", false);
                grid.AdicionarCabecalho("CodigoCFOP", false);
                grid.AdicionarCabecalho("CFOP", false);

                grid.AdicionarCabecalho("CodigoANP", false);
                grid.AdicionarCabecalho("PercentualGLP", false);
                grid.AdicionarCabecalho("PercentualGNN", false);
                grid.AdicionarCabecalho("PercentualGNI", false);
                grid.AdicionarCabecalho("PercentualOrigemCombustivel", false);
                grid.AdicionarCabecalho("PercentualMisturaBiodiesel", false);
                grid.AdicionarCabecalho("ValorPartidaANP", false);
                grid.AdicionarCabecalho("IndicadorEscalaRelevante", false);
                grid.AdicionarCabecalho("IndicadorImportacaoCombustivel", false);
                grid.AdicionarCabecalho("CNPJFabricante", false);
                grid.AdicionarCabecalho("CodigoBeneficioFiscal", false);
                grid.AdicionarCabecalho("ValorMinimoVenda", false);
                grid.AdicionarCabecalho("GrupoProduto", false);
                grid.AdicionarCabecalho("OrigemMercadoria", false);
                grid.AdicionarCabecalho("DescricaoNotaFiscal", false);

                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propriedadeOrdenar.Equals("Codigo") || propriedadeOrdenar.Equals("CodigoProduto") || propriedadeOrdenar.Equals("Descricao") || propriedadeOrdenar.Equals("CodigoNCM"))
                    propriedadeOrdenar = "Produto." + propriedadeOrdenar;

                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> listaProdutoEstoque = repProdutoEstoque.ConsultaProdutoComEstoque(codigoBarrasEAN, descricao, codigoNCM, ativo, codigoEmpresa, codigoProdutoEmbarcador, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repProdutoEstoque.ContaConsultaProdutoComEstoque(codigoBarrasEAN, descricao, codigoNCM, ativo, codigoEmpresa, codigoProdutoEmbarcador));

                var lista = (from p in listaProdutoEstoque
                             select new
                             {
                                 Quantidade = p.Quantidade.ToString("n4"),
                                 p.Produto.Codigo,
                                 p.Produto.CodigoProduto,
                                 p.Produto.Descricao,
                                 p.Produto.CodigoNCM,
                                 p.Produto.DescricaoStatus,
                                 UnidadeMedida = UnidadeDeMedidaHelper.ObterDescricao(p.Produto.UnidadeDeMedida),
                                 CodigoUnidadeMedida = p.Produto.UnidadeDeMedida.HasValue ? p.Produto.UnidadeDeMedida.Value : UnidadeDeMedida.Unidade,
                                 ValorVenda = p.Produto.ValorVenda.ToString("n" + Empresa?.CasasValorProdutoNFe.ToString() ?? "2"),
                                 UltimoCusto = p.Produto.UltimoCusto.ToString("n4"),
                                 UltimoCustoCombustivel = p.Produto.UltimoCusto.ToString("n4"),
                                 CodigoCFOP = natureza?.CFOP?.Codigo ?? 0,
                                 CFOP = natureza?.CFOP?.CodigoCFOP ?? 0,
                                 p.Produto.CodigoANP,
                                 PercentualGLP = p.Produto.PercentualGLP.ToString("n4"),
                                 PercentualGNN = p.Produto.PercentualGNN.ToString("n4"),
                                 PercentualGNI = p.Produto.PercentualGNI.ToString("n4"),
                                 PercentualOrigemCombustivel = p.Produto.PercentualOrigemCombustivel.ToString("n4"),
                                 PercentualMisturaBiodiesel = p.Produto.PercentualMisturaBiodiesel.ToString("n4"),
                                 ValorPartidaANP = p.Produto.ValorPartidaANP.ToString("n2"),
                                 p.Produto.IndicadorEscalaRelevante,
                                 p.Produto.IndicadorImportacaoCombustivel,
                                 p.Produto.CNPJFabricante,
                                 p.Produto.CodigoBeneficioFiscal,
                                 ValorMinimoVenda = p.Produto.ValorMinimoVenda.ToString("n2"),
                                 GrupoProduto = p.Produto.GrupoProdutoTMS?.Descricao ?? string.Empty,
                                 p.Produto.OrigemMercadoria,
                                 p.Produto.DescricaoNotaFiscal
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
        public async Task<IActionResult> PesquisaComEstoqueAgrupado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Produto.Produto serProduto = new Servicos.Embarcador.Produto.Produto(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);

                int codigoNatureza = Request.GetIntParam("NaturezaOperacao");
                string codigo = Request.GetStringParam("Codigo");
                string codigoBarrasEAN = Request.GetStringParam("CodigoBarrasEAN");
                string descricao = Request.GetStringParam("Descricao");
                string codigoProdutoOriginal = Request.Params("CodigoProdutoEmbarcador");
                bool IsCodigoDeBarras = VerificarSeCodigoDeBarras(codigoProdutoOriginal);
                string codigoProdutoEmbarcador = serProduto.ContornarLeituraCodigoBarras(codigoProdutoOriginal);
                string codigoNCM = Request.GetStringParam("CodigoNCM");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                SituacaoAtivoPesquisa ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo);

                Dominio.Entidades.NaturezaDaOperacao natureza = null;
                if (codigoNatureza > 0)
                    natureza = repNaturezaDaOperacao.BuscarPorId(codigoNatureza);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.right, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.CodigoProduto, "CodigoProduto", 15, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.NCM, "CodigoNCM", 10, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.Unidade, "UnidadeMedidaFormatada", 15, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.EstoqueAtual, "Estoque", 15, Models.Grid.Align.left, true);
                if (ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false, true, true);
                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoComEstoqueAgrupado> listaProdutoEstoqueAgrupado = repProdutoEstoque.ConsultaProdutoComEstoqueAgrupado(codigo, codigoBarrasEAN, descricao, codigoNCM, ativo, codigoEmpresa, codigoProdutoEmbarcador, IsCodigoDeBarras, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repProdutoEstoque.ContaConsultaProdutoComEstoqueAgrupado(codigo, codigoBarrasEAN, descricao, codigoNCM, ativo, codigoEmpresa, codigoProdutoEmbarcador, IsCodigoDeBarras));
                grid.AdicionaRows(listaProdutoEstoqueAgrupado);
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
        public async Task<IActionResult> PesquisaProdutoEstoque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);

                string codigoBarrasEAN = Request.Params("CodigoBarrasEAN");
                string descricao = Request.Params("Descricao");
                string codigoProdutoEmbarcador = Request.Params("CodigoProdutoEmbarcador");
                int empresa = this.Usuario.Empresa?.Codigo ?? 0;
                int.TryParse(Request.Params("Filial"), out int filial);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    empresa = 0;

                SituacaoAtivoPesquisa ativo = SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoProduto", false);
                grid.AdicionarCabecalho("Unidade", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.EstoqueAtual, "Quantidade", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.CustoUnitario, "CustoUnitario", 15, Models.Grid.Align.right, true);

                //grid.AdicionarCabecalho("Estoque Mín.", "EstoqueMinimo", 15, Models.Grid.Align.right, true);

                List<Dominio.Entidades.Produto> listaProduto = repProduto.ConsultarProdutoEstoque(empresa, codigoBarrasEAN, descricao, ativo, empresa, codigoProdutoEmbarcador, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repProduto.ContarConsultaProdutoEstoque(empresa, codigoBarrasEAN, descricao, ativo, empresa, codigoProdutoEmbarcador);

                var lista = (from p in listaProduto
                             select new
                             {
                                 Codigo = repProdutoEstoque.BuscarPorProdutoEFilial(p.Codigo, filial)?.Codigo,
                                 CodigoProduto = p.Codigo,
                                 p.Descricao,
                                 Quantidade = repProdutoEstoque.BuscarPorProdutoEFilial(p.Codigo, filial)?.Quantidade,
                                 CustoUnitario = p.UltimoCusto.ToString("n2"),
                                 Unidade = UnidadeDeMedidaHelper.ObterSigla(p?.UnidadeDeMedida ?? null)
                                 //p.EstoqueMinimo,
                             }).ToList();

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaEstoqueMinimo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                string codigoBarrasEAN = Request.Params("CodigoBarrasEAN");
                string descricao = Request.Params("Descricao");
                string codigoProdutoEmbarcador = Request.Params("CodigoProdutoEmbarcador");
                int grupoProdutoTms = Request.GetIntParam("GrupoProdutoTMS");
                bool somenteEstoqueMinimo = Request.GetBoolParam("SomenteComEstoque");
                int empresa = this.Usuario.Empresa?.Codigo ?? 0;
                int.TryParse(Request.Params("Filial"), out int filial);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    empresa = 0;

                SituacaoAtivoPesquisa ativo = SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoProduto", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.EstoqueAtual, "Quantidade", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.EstoqueMinimo, "EstoqueMinimo", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.Diferenca, "Diferenca", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UltimoCusto", false);

                List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> listaProduto = repProduto.ConsultarEstoqueMinimo(filial, codigoBarrasEAN, descricao, ativo, empresa, codigoProdutoEmbarcador, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, somenteEstoqueMinimo, grupoProdutoTms);
                int totalRegistros = repProduto.ContarConsultaEstoqueMinimo(filial, codigoBarrasEAN, descricao, ativo, empresa, codigoProdutoEmbarcador, somenteEstoqueMinimo, grupoProdutoTms);

                var lista = (from p in listaProduto
                             select new
                             {
                                 p.Codigo,
                                 CodigoProduto = p.Produto.Codigo,
                                 Descricao = p.Produto.Descricao,
                                 p.Quantidade,
                                 p.EstoqueMinimo,
                                 Diferenca = p.EstoqueMinimo - p.Quantidade,
                                 UltimoCusto = p.Produto.UltimoCusto.ToString("n2"),
                             }).ToList();

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaComTabelaPrecoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda repTabelaPrecoVenda = new Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                bool.TryParse(Request.Params("SomenteComEstoque"), out bool somenteComEstoque);
                double.TryParse(Request.Params("Pessoa"), out double codigoPessoa);

                string codigoBarrasEAN = Request.Params("CodigoBarrasEAN");
                string descricao = Request.Params("Descricao");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.CodigoProduto, "CodigoProduto", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.NCM, "CodigoNCM", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.Unidade, "UnidadeMedida", 15, Models.Grid.Align.left, false);
                if (ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Valor, "ValorVenda", 10, Models.Grid.Align.right, true);

                grid.AdicionarCabecalho("ValorMinimoVenda", false);
                grid.AdicionarCabecalho("GrupoProduto", false);
                grid.AdicionarCabecalho("CodigoTabelaPreco", false);

                Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(codigoPessoa);
                List<Dominio.Entidades.Produto> listaProduto = repProduto.Consulta(0, codigoBarrasEAN, codigo, "", descricao, "", ativo, null, codigoEmpresa, "", 0, somenteComEstoque, false, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repProduto.ContaConsulta(0, codigoBarrasEAN, codigo, "", descricao, "", ativo, null, codigoEmpresa, "", 0, somenteComEstoque, false));

                var lista = (from p in listaProduto
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoProduto,
                                 p.Descricao,
                                 p.CodigoNCM,
                                 p.DescricaoStatus,
                                 UnidadeMedida = UnidadeDeMedidaHelper.ObterDescricao(p.UnidadeDeMedida),
                                 ValorVenda = repTabelaPrecoVenda.ValorProdutoPorTabelaPrecoPessoa(codigoEmpresa, codigoPessoa, pessoa?.GrupoPessoas?.Codigo ?? -1, p.GrupoProdutoTMS?.Codigo ?? -1, p.ValorVenda).ToString("n2"),
                                 ValorMinimoVenda = p.ValorMinimoVenda.ToString("n2"),
                                 GrupoProduto = p.GrupoProdutoTMS?.Descricao ?? string.Empty,
                                 CodigoTabelaPreco = repTabelaPrecoVenda.CodigoTabelaPrecoPessoa(codigoEmpresa, codigoPessoa, pessoa?.GrupoPessoas?.Codigo ?? -1, p.GrupoProdutoTMS?.Codigo ?? -1)
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

        public async Task<IActionResult> AtualizarArmazenamentoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico repProdutoEstoqueHistorico = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico(unitOfWork);
                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Filial"), out int codigoEmpresa);
                int.TryParse(Request.Params("LocalArmazenamento"), out int codigoLocalArmazenamento);
                decimal.TryParse(Request.Params("EstoqueMinimo"), out decimal estoqueMinimo);
                decimal.TryParse(Request.Params("EstoqueMaximo"), out decimal estoqueMaximo);
                decimal CustoMedio = Request.GetDecimalParam("CustoMedio");
                decimal UltimoCusto = Request.GetDecimalParam("UltimoCusto");
                decimal QuantidadeEstoqueReservada = Request.GetDecimalParam("QuantidadeEstoqueReservada");

                int codigoEmpresaAnterior = 0;
                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoque = repProdutoEstoque.BuscarPorCodigo(codigo);

                if (produtoEstoque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (repProdutoEstoque.ExisteLocalArmazenamentoEmOutroProduto(codigo, produtoEstoque.Produto.Codigo, codigoEmpresa, codigoLocalArmazenamento))
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoqueAnterior = repProdutoEstoque.BuscarLocalArmazenamentoEmOutroProduto(codigo, produtoEstoque.Produto.Codigo, codigoEmpresa, codigoLocalArmazenamento);

                    produtoEstoqueAnterior.Quantidade += produtoEstoque.Quantidade;
                    produtoEstoqueAnterior.CustoMedio = CustoMedio;
                    produtoEstoqueAnterior.UltimoCusto = UltimoCusto;

                    repProdutoEstoque.Atualizar(produtoEstoqueAnterior);

                    repProdutoEstoqueHistorico.TransferirHistoricoEstoque(produtoEstoque.Codigo, produtoEstoqueAnterior.Codigo, codigoEmpresa, codigoLocalArmazenamento);

                    repProdutoEstoque.Deletar(produtoEstoque);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoEstoque.Produto, null, "Agrupou o armazenamento do produto.", unitOfWork);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true);
                }
                else
                {
                    produtoEstoque.Initialize();
                    codigoEmpresaAnterior = produtoEstoque.Empresa?.Codigo ?? 0;
                    produtoEstoque.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                    produtoEstoque.EstoqueMinimo = estoqueMinimo;
                    produtoEstoque.EstoqueMaximo = estoqueMaximo;
                    produtoEstoque.LocalArmazenamento = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;
                    produtoEstoque.CustoMedio = CustoMedio;
                    produtoEstoque.UltimoCusto = UltimoCusto;
                    produtoEstoque.QuantidadeEstoqueReservada = QuantidadeEstoqueReservada;

                    unitOfWork.Start();

                    repProdutoEstoque.Atualizar(produtoEstoque, Auditado);
                    repProdutoEstoqueHistorico.AtualizarHistoricoEstoque(produtoEstoque.Produto.Codigo, codigoEmpresa, codigoEmpresaAnterior);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoEstoque.Produto, null, "Atualizou o armazenamento do produto", unitOfWork);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true);
                }
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

        [AllowAuthenticate]
        public async Task<IActionResult> ArmazenamentoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);

                int produto = Request.GetIntParam("Produto");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("CodigoLocalArmazenamento", false);
                grid.AdicionarCabecalho("Empresa", "Filial", 30, Models.Grid.Align.left, true);
                if (ConfiguracaoEmbarcador.UtilizaMultiplosLocaisArmazenamento || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Local Armazenamento", "LocalArmazenamento", 10, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("LocalArmazenamento", false);
                grid.AdicionarCabecalho("Quantidade Atual", "QuantidadeAtual", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Disponível", "QuantidadeDisponivel", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Custo Médio", "CustoMedio", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Último Custo", "UltimoCusto", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Estoque Mínimo", "EstoqueMinimo", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Estoque Máximo", "EstoqueMaximo", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Quantidade de Estoque Reservada", "QuantidadeEstoqueReservada", 16, Models.Grid.Align.right, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena.Equals("Filial"))
                    propOrdena = "Empresa";
                else if (propOrdena.Equals("QuantidadeAtual"))
                    propOrdena = "Quantidade";

                List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> listaProduto = repProdutoEstoque.Consultar(produto, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repProdutoEstoque.ContarConsulta(produto);

                var lista = (from o in listaProduto
                             select new
                             {
                                 o.Codigo,
                                 CodigoFilial = o.Empresa?.Codigo ?? 0,
                                 Filial = o.Empresa?.Descricao ?? string.Empty,
                                 QuantidadeAtual = o.Quantidade.ToString("n4"),
                                 QuantidadeDisponivel = (o.Quantidade - o.QuantidadeEstoqueReservada).ToString("n2"),
                                 CustoMedio = o.CustoMedio.ToString("n4"),
                                 UltimoCusto = o.UltimoCusto.ToString("n4"),
                                 EstoqueMinimo = o.EstoqueMinimo.ToString("n2"),
                                 EstoqueMaximo = o.EstoqueMaximo.ToString("n2"),
                                 CodigoLocalArmazenamento = o.LocalArmazenamento?.Codigo ?? 0,
                                 LocalArmazenamento = o.LocalArmazenamento?.Descricao ?? string.Empty,
                                 QuantidadeEstoqueReservada = o.QuantidadeEstoqueReservada.ToString("n2")
                             }).ToList();

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

        [AllowAuthenticate]
        public async Task<IActionResult> HistoricoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico repProdutoEstoqueHistorico = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Produto"), out int produto);
                int.TryParse(Request.Params("Filial"), out int filial);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Documento", "Documento", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 15, Models.Grid.Align.right, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico> listaProduto = repProdutoEstoqueHistorico.Consultar(codigo, TipoServicoMultisoftware, produto, filial, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repProdutoEstoqueHistorico.ContarConsulta(codigo, TipoServicoMultisoftware, produto, filial);

                var lista = (from o in listaProduto
                             select new
                             {
                                 o.Codigo,
                                 Tipo = o.Tipo.ObterDescricao(),
                                 o.Documento,
                                 Data = o.Data?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                 Quantidade = o.Quantidade.ToString("n4")
                             }).ToList();

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

        [AllowAuthenticate]
        public async Task<IActionResult> CustoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);

                int.TryParse(Request.Params("Produto"), out int produto);
                string filial = Request.Params("Filial");
                filial = Utilidades.String.OnlyNumbers(filial);
                filial = filial.PadLeft(14, '0');

                double cnpjPosto = Request.GetDoubleParam("Filial");
                DateTime dataAbastecimento = Request.GetDateTimeParam("Data");

                if (dataAbastecimento == DateTime.MinValue)
                       dataAbastecimento = DateTime.Now;

                Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores valorFornecedor = repPostoCombustivelTabelaValores.BuscarModalidadeFornecedor(produto, cnpjPosto, dataAbastecimento);
                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoque = repProdutoEstoque.BuscarPorProdutoECNPJFilial(produto, filial);
                Dominio.Entidades.Produto prod = repProduto.BuscarPorCodigo(produto);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos repositorioConfiguracaoAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos configuracaoAbastecimento = repositorioConfiguracaoAbastecimento.BuscarConfiguracaoPadrao();

                var dynProduto = new
                {
                    UltimoCusto = valorFornecedor != null && valorFornecedor.ValorFixo > 0 ? valorFornecedor.ValorFixo.ToString("n4") : produtoEstoque != null && produtoEstoque.UltimoCusto > 0 ? produtoEstoque.UltimoCusto.ToString("n4") : (configuracaoAbastecimento.UtilizarCustoMedioParaLancamentoAbastecimentos ? prod.CustoMedio.ToString("n4") : (prod?.UltimoCusto.ToString("n4") ?? "0,000")),
                    ValorMoedaCotacao = valorFornecedor != null && valorFornecedor.ValorMoedaCotacao > 0 ? valorFornecedor.ValorMoedaCotacao.ToString("n10") : "0,0000000000",
                    MoedaCotacaoBancoCentral = valorFornecedor != null && valorFornecedor.MoedaCotacaoBancoCentral.HasValue ? valorFornecedor.MoedaCotacaoBancoCentral.Value : MoedaCotacaoBancoCentral.Real
                };

                return new JsonpResult(dynProduto, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o custo do produto.");
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

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unitOfWork);
                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumento = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
                Dominio.Entidades.Produto produto = new Dominio.Entidades.Produto();

                PreencherProduto(produto, unitOfWork);

                SalvarPneus(produto, unitOfWork);
                SalvarEPI(produto);
                SalvarCombustivel(produto);
                SalvarBem(produto, unitOfWork);

                if (!string.IsNullOrWhiteSpace(produto.CodigoProduto) && (configuracaoDocumento?.BloquearCadastroProdutoComMesmoCodigo ?? false))
                {
                    if (repProduto.ContemProdutoMesmoCodigo(produto.CodigoProduto, 0))
                        throw new ControllerException("Já existe um produto com o mesmo Código informado.");
                }

                repProduto.Inserir(produto, Auditado);

                if (string.IsNullOrWhiteSpace(produto.CodigoProduto))
                {
                    produto = repProduto.BuscarPorCodigo(produto.Codigo);
                    if (produto != null)
                    {
                        produto.CodigoProduto = produto.Codigo.ToString();
                        repProduto.Atualizar(produto);
                    }
                }

                SalvarFornecedores(produto, unitOfWork);
                SalvarComposicoes(produto, unitOfWork);

                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);
                servicoEstoque.AdicionarEstoque(produto, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? produto.Empresa : null, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                unitOfWork.CommitChanges();
                object retorno = new { produto.Codigo };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarProdutoSimplificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unitOfWork);
                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumento = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();

                Dominio.Entidades.Produto produto = new Dominio.Entidades.Produto();
                produto.Descricao = Request.Params("Descricao");
                produto.DescricaoNotaFiscal = produto.Descricao;
                produto.CodigoBarrasEAN = Request.Params("CodigoBarrasEAN");
                produto.CodigoProduto = Request.Params("CodigoProduto");
                produto.CodigoNCM = Request.Params("CodigoNCM");
                produto.UnidadeDeMedida = Request.GetEnumParam<UnidadeDeMedida>("UnidadeMedida");

                if (produto.CodigoNCM == "0")
                    produto.CodigoNCM = "00000000";
                if (produto.CodigoNCM.Length < 8)
                    throw new ControllerException("Favor verifique o NCM informado.");

                produto.Status = "A";
                produto.CategoriaProduto = Request.GetEnumParam("CategoriaProduto", CategoriaProduto.MercadoriaRevenda);
                produto.CSTIPIVenda = 0;
                produto.CSTIPICompra = 0;
                produto.OrigemMercadoria = OrigemMercadoria.Origem0;
                produto.GeneroProduto = GeneroProduto.Genero0;
                produto.IndicadorEscalaRelevante = IndicadorEscalaRelevante.Nenhum;
                produto.IndicadorImportacaoCombustivel = IndicadorImportacaoCombustivel.Nenhum;
                produto.GerarPneuAutomatico = false;
                produto.ProdutoEPI = false;
                produto.ProdutoCombustivel = false;
                produto.ProdutoKIT = false;
                produto.ProdutoBem = false;

                if (ncmsAbastecimento != null && ncmsAbastecimento.Count() > 0 && !string.IsNullOrWhiteSpace(produto.CodigoNCM) && !produto.ProdutoCombustivel.Value)
                {
                    if (ncmsAbastecimento.Where(o => produto.CodigoNCM.Contains(o.NCM)).Count() > 0)
                        produto.ProdutoCombustivel = true;
                    else
                        produto.ProdutoCombustivel = false;
                }
                else
                    produto.ProdutoCombustivel = false;

                if (!string.IsNullOrWhiteSpace(produto.CodigoProduto) && (configuracaoDocumento?.BloquearCadastroProdutoComMesmoCodigo ?? false))
                {
                    if (repProduto.ContemProdutoMesmoCodigo(produto.CodigoProduto, 0))
                        throw new ControllerException("Já existe um produto com o mesmo Código informado.");
                }

                produto.CalculoCustoProduto = Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unitOfWork);

                if (produto.Codigo == 0)
                {
                    int codigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;
                    produto.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                }

                repProduto.Inserir(produto, Auditado);

                if (string.IsNullOrWhiteSpace(produto.CodigoProduto))
                {
                    produto = repProduto.BuscarPorCodigo(produto.Codigo);
                    if (produto != null)
                    {
                        produto.CodigoProduto = produto.Codigo.ToString();
                        repProduto.Atualizar(produto);
                    }
                }

                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);
                servicoEstoque.AdicionarEstoque(produto, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? produto.Empresa : null, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumento = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(codigo, true);

                if (produto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherProduto(produto, unitOfWork);

                if (!string.IsNullOrWhiteSpace(produto.CodigoProduto) && (configuracaoDocumento?.BloquearCadastroProdutoComMesmoCodigo ?? false))
                {
                    if (repProduto.ContemProdutoMesmoCodigo(produto.CodigoProduto, produto.Codigo))
                        throw new ControllerException("Já existe um produto com o mesmo Código informado.");
                }

                if (string.IsNullOrWhiteSpace(produto.CodigoProduto))
                    produto.CodigoProduto = produto.Codigo.ToString();

                SalvarPneus(produto, unitOfWork);
                SalvarEPI(produto);
                SalvarCombustivel(produto);
                SalvarBem(produto, unitOfWork);

                repProduto.Atualizar(produto, Auditado);

                SalvarFornecedores(produto, unitOfWork);
                SalvarComposicoes(produto, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(0, codigo);
                var dynProduto = new
                {
                    produto.Codigo,
                    produto.CodigoProduto,
                    produto.Descricao,
                    produto.CodigoNCM,
                    produto.Status,
                    UnidadeMedida = produto.UnidadeDeMedida,
                    produto.DescricaoNotaFiscal,
                    produto.OrigemMercadoria,
                    UltimoCusto = produto.UltimoCusto.ToString("n4"),
                    CustoMedio = produto.CustoMedio.ToString("n4"),
                    produto.MargemLucro,
                    ValorVenda = produto.ValorVenda.ToString("n" + this.Usuario.Empresa?.CasasValorProdutoNFe.ToString() ?? "2"),
                    produto.PesoBruto,
                    produto.PesoLiquido,
                    produto.CodigoEAN,
                    produto.CodigoEnquadramentoIPI,
                    produto.CSTIPIVenda,
                    produto.CSTIPICompra,
                    produto.AliquotaIPIVenda,
                    produto.AliquotaIPICompra,
                    produto.CategoriaProduto,
                    produto.GeneroProduto,
                    produto.CodigoCEST,
                    produto.CodigoBarrasEAN,
                    produto.CodigoAnvisa,                    
                    ValorMinimoVenda = produto.ValorMinimoVenda.ToString("n2"),
                    Fornecedores = (from obj in produto.Fornecedores
                                    select new
                                    {
                                        obj.Codigo,
                                        obj.CodigoProduto,
                                        FatorConversao = obj.FatorConversao.ToString("n5"),
                                        Fornecedor = new
                                        {
                                            Codigo = obj.Fornecedor.CPF_CNPJ_SemFormato,
                                            Descricao = obj.Fornecedor.CPF_CNPJ_Formatado + " - " + obj.Fornecedor.Nome
                                        }
                                    }).ToList(),
                    GrupoImposto = produto.GrupoImposto != null ? new { produto.GrupoImposto.Codigo, produto.GrupoImposto.Descricao } : null,
                    GrupoProdutoTMS = produto.GrupoProdutoTMS != null ? new { produto.GrupoProdutoTMS.Codigo, produto.GrupoProdutoTMS.Descricao } : null,
                    Empresa = produto.Empresa != null ? new { produto.Empresa.Codigo, Descricao = produto.Empresa.RazaoSocial } : null,
                    produto.CalculoCustoProduto,
                    LocalArmazenamentoProduto = produto.LocalArmazenamentoProduto != null ? new { produto.LocalArmazenamentoProduto.Codigo, produto.LocalArmazenamentoProduto.Descricao } : null,
                    MarcaProduto = produto.MarcaProduto != null ? new { produto.MarcaProduto.Codigo, produto.MarcaProduto.Descricao } : null,
                    FinalidadeProdutoOrdemServico = produto.FinalidadeProdutoOrdemServico != null ? new { produto.FinalidadeProdutoOrdemServico.Codigo, produto.FinalidadeProdutoOrdemServico.Descricao } : null,

                    Foto = produto.Foto != null ? (from obj in produto.Foto
                                                   where obj.Status == true
                                                   select new
                                                   {
                                                       obj.Codigo,
                                                       DescricaoFoto = obj.Descricao,
                                                       Arquivo = obj.NomeArquivo,
                                                       FotoProduto = ObterFotoBase64(System.IO.Path.GetFileName(obj.CaminhoArquivo), new string[] { }, unitOfWork)
                                                   }).ToList() : null,
                    Pneu = new
                    {
                        produto.GerarPneuAutomatico,
                        ModeloPneu = produto.Modelo != null ? new { Codigo = produto.Modelo.Codigo, Descricao = produto.Modelo.Descricao } : null,
                        BandaRodagemPneu = produto.BandaRodagem != null ? new { Codigo = produto.BandaRodagem.Codigo, Descricao = produto.BandaRodagem.Descricao } : null
                    },
                    IndicadorEscalaRelevante = produto.IndicadorEscalaRelevante,                    
                    CNPJFabricante = !string.IsNullOrWhiteSpace(produto.CNPJFabricante) ? produto.CNPJFabricante : string.Empty,
                    CodigoBeneficioFiscal = !string.IsNullOrWhiteSpace(produto.CodigoBeneficioFiscal) ? produto.CodigoBeneficioFiscal : string.Empty,
                    EPI = new
                    {
                        produto.ProdutoEPI,
                        produto.NumeroCA
                    },
                    Combustivel = new
                    {
                        produto.ProdutoCombustivel,
                        produto.ControlaEstoqueCombustivel,
                        produto.CodigoANP,
                        PercentualGLP = produto.PercentualGLP.ToString("n4"),
                        PercentualGNN = produto.PercentualGNN.ToString("n4"),
                        PercentualGNI = produto.PercentualGNI.ToString("n4"),
                        PercentualOrigemCombustivel = produto.PercentualOrigemCombustivel.ToString("n4"),
                        PercentualMisturaBiodiesel = produto.PercentualMisturaBiodiesel.ToString("n4"),
                        produto.ValorPartidaANP,
                        IndicadorImportacaoCombustivel = produto.IndicadorImportacaoCombustivel
                    },
                    Composicao = new
                    {
                        produto.ProdutoKIT
                    },
                    Composicoes = (from obj in produto.Composicoes
                                   select new
                                   {
                                       obj.Codigo,
                                       Quantidade = obj.Quantidade.ToString("n4"),
                                       Produto = new
                                       {
                                           obj.Insumo.Codigo,
                                           obj.Insumo.Descricao
                                       }
                                   }).ToList(),
                    Bem = new
                    {
                        produto.ProdutoBem,
                        Almoxarifado = produto.Almoxarifado != null ? new { produto.Almoxarifado.Codigo, produto.Almoxarifado.Descricao } : null,
                        CentroResultado = produto.CentroResultado != null ? new { produto.CentroResultado.Codigo, produto.CentroResultado.Descricao } : null
                    }
                };
                return new JsonpResult(dynProduto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(codigo, true);

                repProduto.RemoverVinculoEntidades(produto.Codigo);

                if (produto == null)
                    return new JsonpResult(false, "Produto não encontrado.");

                repProduto.Deletar(produto, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTipoCombustivel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                int codigo = 0;
                if (!String.IsNullOrWhiteSpace(Request.Params("Codigo")))
                    codigo = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.NCM, "NumeroNCM", 30, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                List<Dominio.Entidades.Produto> listaProduto = repProduto.ConsultarTipoCombustivel(codigo, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repProduto.ContarConsultaTipoCombustivel(codigo, descricao));

                var lista = (from p in listaProduto select new { p.Codigo, p.Descricao, NumeroNCM = p.NCM.Numero }).ToList();

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
        public async Task<IActionResult> PesquisaTipoCombustivelTMS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                int codigo = Request.GetIntParam("Codigo");

                bool.TryParse(Request.Params("SomenteAbastecimentos"), out bool somenteAbatecimentos);

                TipoAbastecimento? tipoAbastecimento = Request.GetNullableEnumParam<TipoAbastecimento>("TipoAbastecimento");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true); ;
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.NCM, "NumeroNCM", 30, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                List<Dominio.Entidades.Produto> listaProduto = repProduto.ConsultarTipoCombustivelTMS(somenteAbatecimentos, codigo, descricao, tipoAbastecimento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repProduto.ContarConsultaTipoCombustivelTMS(somenteAbatecimentos, codigo, descricao, tipoAbastecimento));

                var lista = (from p in listaProduto select new { p.Codigo, p.Descricao, NumeroNCM = p.NCM != null ? p.NCM.Numero : p.CodigoNCM }).ToList();

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
        public async Task<IActionResult> PesquisaNCMS()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                string codigoNCM = Utilidades.String.OnlyNumbers(Request.Params("Descricao"));
                string descricao = Request.Params("CodigoNCM");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.NCM, "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "NumeroNCM", 60, Models.Grid.Align.left, true);

                AdminMultisoftware.Repositorio.Produtos.NCM repNCM = new AdminMultisoftware.Repositorio.Produtos.NCM(unitOfWork);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Descricao")
                    propOrdenar = "Numero";
                else if (propOrdenar == "NumeroNCM")
                    propOrdenar = "Descricao";

                List<AdminMultisoftware.Dominio.Entidades.Produtos.NCM> listaNCM = repNCM.ConsultarNCM(codigoNCM, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNCM.ContarConsultaNCM(codigoNCM, descricao));

                var lista = (from p in listaNCM select new { p.Codigo, NumeroNCM = p.Descricao, Descricao = p.Numero }).ToList();

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
        public async Task<IActionResult> PesquisaCESTS()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                string codigoNCM = Utilidades.String.OnlyNumbers(Request.Params("CodigoNCM"));
                string descricao = Request.Params("Descricao");
                string codigoCEST = Utilidades.String.OnlyNumbers(Request.Params("CodigoCEST"));
                if (!string.IsNullOrWhiteSpace(codigoNCM) && codigoNCM.Length > 4)
                    codigoNCM = codigoNCM.Substring(0, 4);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.CEST, "CEST", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Produto.NCM, "NCM", 20, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                AdminMultisoftware.Repositorio.Produtos.CEST repCEST = new AdminMultisoftware.Repositorio.Produtos.CEST(unitOfWork);

                List<AdminMultisoftware.Dominio.Entidades.Produtos.CEST> listaNCM = repCEST.ConsultarCEST(codigoCEST, codigoNCM, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCEST.ContarConsultaCEST(codigoCEST, codigoNCM, descricao));

                var lista = (from p in listaNCM select new { p.Codigo, CEST = p.Numero, p.Descricao, NCM = p.CodigoNCM }).ToList();

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
        public async Task<IActionResult> BuscarCustoMedio()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);

                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(0, codigo);

                var dynProduto = new
                {
                    produto.Codigo,
                    produto.CustoMedio
                };

                return new JsonpResult(dynProduto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o custo médio do produto.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarFormulaCusto()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo, codigoItem;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoItem"), out codigoItem);
                string formulaAtual = Request.Params("Formula");

                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);

                string formulaCusto = "";
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item = null;

                if (codigoItem > 0)
                    item = repDocumentoEntradaItem.BuscarPorCodigo(codigoItem);

                if (!string.IsNullOrWhiteSpace(formulaAtual))
                    formulaCusto = formulaAtual;
                else if (item != null && !string.IsNullOrWhiteSpace(item.CalculoCustoProduto))
                    formulaCusto = item?.CalculoCustoProduto;
                else
                    formulaCusto = produto?.CalculoCustoProduto;

                if (string.IsNullOrWhiteSpace(formulaCusto))
                    formulaCusto = Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unidadeTrabalho);

                if (!string.IsNullOrWhiteSpace(formulaCusto))
                {
                    string[] campos = formulaCusto.Split('#');
                    for (int i = 0; i < campos.Length; i++)
                    {
                        if (campos[i].Trim() == "")
                        {
                            campos.ToList().Remove(campos[i]);
                        }
                        else
                        {
                            campos[i] = campos[i].Trim();
                        }

                    }
                    var dynProduto = new
                    {
                        ValorDesconto = campos.Contains("ValorDesconto") ? campos[campos.ToList().IndexOf("ValorDesconto") - 1] : "",
                        ValorICMS = campos.Contains("ValorICMS") ? campos[campos.ToList().IndexOf("ValorICMS") - 1] : "",
                        ValorDiferencial = campos.Contains("ValorDiferencial") ? campos[campos.ToList().IndexOf("ValorDiferencial") - 1] : "",
                        ValorICMSST = campos.Contains("ValorICMSST") ? campos[campos.ToList().IndexOf("ValorICMSST") - 1] : "",
                        ValorIPI = campos.Contains("ValorIPI") ? campos[campos.ToList().IndexOf("ValorIPI") - 1] : "",
                        ValorFrete = campos.Contains("ValorFrete") ? campos[campos.ToList().IndexOf("ValorFrete") - 1] : "",
                        ValorSeguro = campos.Contains("ValorSeguro") ? campos[campos.ToList().IndexOf("ValorSeguro") - 1] : "",
                        ValorOutras = campos.Contains("ValorOutras") ? campos[campos.ToList().IndexOf("ValorOutras") - 1] : "",
                        ValorDescontoFora = campos.Contains("ValorDescontoFora") ? campos[campos.ToList().IndexOf("ValorDescontoFora") - 1] : "",
                        ValorImpostoFora = campos.Contains("ValorImpostoFora") ? campos[campos.ToList().IndexOf("ValorImpostoFora") - 1] : "",
                        ValorOutrasFora = campos.Contains("ValorOutrasFora") ? campos[campos.ToList().IndexOf("ValorOutrasFora") - 1] : "",
                        ValorFreteFora = campos.Contains("ValorFreteFora") ? campos[campos.ToList().IndexOf("ValorFreteFora") - 1] : "",
                        ValorICMSFreteFora = campos.Contains("ValorICMSFreteFora") ? campos[campos.ToList().IndexOf("ValorICMSFreteFora") - 1] : "",
                        ValorDiferencialFreteFora = campos.Contains("ValorDiferencialFreteFora") ? campos[campos.ToList().IndexOf("ValorDiferencialFreteFora") - 1] : "",
                        ValorPIS = campos.Contains("ValorPIS") ? campos[campos.ToList().IndexOf("ValorPIS") - 1] : "",
                        ValorCOFINS = campos.Contains("ValorCOFINS") ? campos[campos.ToList().IndexOf("ValorCOFINS") - 1] : "",
                        ValorCreditoPresumido = campos.Contains("ValorCreditoPresumido") ? campos[campos.ToList().IndexOf("ValorCreditoPresumido") - 1] : "",
                    };

                    return new JsonpResult(dynProduto);
                }
                else
                    return new JsonpResult(null, true, "Produto sem fórmula de custo cadastrada.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a fórmula do custo do produto.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarFoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                Repositorio.ProdutoFoto repProdutoFoto = new Repositorio.ProdutoFoto(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                int codigoProduto = Request.GetIntParam("CodigoProduto");

                Dominio.Entidades.ProdutoFoto produtoFoto = repProdutoFoto.BuscarPorProduto(codigoProduto);

                if (produtoFoto == null)
                    produtoFoto = new Dominio.Entidades.ProdutoFoto();

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "FotosProdutos");

                for (var i = 0; i < files.Count; i++)
                {
                    string descricao = Request.GetStringParam("DescricaoFoto");
                    Servicos.DTO.CustomFile file = files[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    produtoFoto.CaminhoArquivo = caminho;
                    produtoFoto.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));
                    produtoFoto.Descricao = descricao;
                    produtoFoto.Status = true;
                    produtoFoto.Produto = repProduto.BuscarPorCodigo(codigoProduto);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoFoto.Produto, null, "Adicionou a foto " + produtoFoto.Descricao + ".", unitOfWork);

                    if (produtoFoto.Codigo > 0)
                        repProdutoFoto.Atualizar(produtoFoto);
                    else
                        repProdutoFoto.Inserir(produtoFoto);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InativarFoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFoto = Request.GetIntParam("Codigo");

                Repositorio.ProdutoFoto repProdutoFoto = new Repositorio.ProdutoFoto(unitOfWork);
                Dominio.Entidades.ProdutoFoto produtoFoto = repProdutoFoto.BuscarPorCodigo(codigoFoto);

                produtoFoto.Status = false;
                repProdutoFoto.Atualizar(produtoFoto);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoFoto.Produto, null, "Inativou a foto " + produtoFoto.Descricao + ".", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a foto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarProdutoLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

                Dominio.Entidades.Produto produto = new Dominio.Entidades.Produto();

                int.TryParse(Request.Params("GrupoProduto"), out int codigoGrupo);

                decimal.TryParse(Request.Params("ValorVenda"), out decimal valorVenda);
                decimal.TryParse(Request.Params("ValorMinimoVenda"), out decimal valorMinimoVenda);
                decimal.TryParse(Request.Params("Quantidade"), out decimal quantidadeEstoque);

                produto.Descricao = Request.Params("Descricao");
                produto.DescricaoNotaFiscal = produto.Descricao;
                produto.CodigoNCM = Request.Params("CodigoNCM");
                if (produto.CodigoNCM == "0")
                    produto.CodigoNCM = "00000000";
                if (produto.CodigoNCM.Length < 8)
                    return new JsonpResult(false, "Favor verifique o NCM informado.");
                produto.UnidadeDeMedida = (UnidadeDeMedida)int.Parse(Request.Params("UnidadeMedida"));
                produto.ValorVenda = valorVenda;
                produto.ValorMinimoVenda = valorMinimoVenda;

                produto.Status = "A";
                produto.OrigemMercadoria = OrigemMercadoria.Origem0;
                produto.GrupoProdutoTMS = codigoGrupo > 0 ? repGrupoProduto.BuscarPorCodigo(codigoGrupo) : null;
                produto.Empresa = this.Usuario.Empresa;

                repProduto.Inserir(produto, Auditado);

                string erro = string.Empty;
                if (!servicoEstoque.MovimentarEstoque(out erro, produto, quantidadeEstoque, Dominio.Enumeradores.TipoMovimento.Entrada, "LOTE", "1", 0, produto.Empresa, DateTime.Now, TipoServicoMultisoftware))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirLocalArmazenamentoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico repProdutoEstoqueHistorico = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico(unitOfWork);
                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                if (codigo == 0)
                    return new JsonpResult(false, false, "Estoque não encontrado!");

                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoque = repProdutoEstoque.BuscarPorCodigo(codigo);

                if (produtoEstoque == null)
                    return new JsonpResult(false, false, "Estoque não encontrado!");

                if (produtoEstoque.Quantidade != 0)
                    return new JsonpResult(false, false, "Local de Armazenamento selecionado possui saldo, favor zerar a sua quantidade atual.");

                unitOfWork.Start();

                repProdutoEstoqueHistorico.DeletarTodosPorCodigo(codigo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoEstoque.Produto, null, "Excluiu local de armazenamento do produto", unitOfWork);
                repProdutoEstoque.Deletar(produtoEstoque);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o local.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarLocalArmazenamentoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

                int.TryParse(Request.Params("Produto"), out int codigoProduto);
                int.TryParse(Request.Params("Filial"), out int codigoEmpresa);
                int.TryParse(Request.Params("LocalArmazenamento"), out int codigoLocalArmazenamento);
                decimal.TryParse(Request.Params("EstoqueMinimo"), out decimal estoqueMinimo);
                decimal.TryParse(Request.Params("EstoqueMaximo"), out decimal estoqueMaximo);

                if (codigoProduto == 0)
                    return new JsonpResult(false, true, "Produto não encontrado!");

                if (repProdutoEstoque.ExisteLocalArmazenamentoEmOutroProduto(0, codigoProduto, codigoEmpresa, codigoLocalArmazenamento))
                    return new JsonpResult(false, true, "Local de Armazenamento selecionado já existe em outro registro!");

                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoque = new Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque();
                produtoEstoque.Produto = repProduto.BuscarPorCodigo(codigoProduto);
                produtoEstoque.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                produtoEstoque.EstoqueMinimo = estoqueMinimo;
                produtoEstoque.EstoqueMaximo = estoqueMaximo;
                produtoEstoque.LocalArmazenamento = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;

                unitOfWork.Start();

                repProdutoEstoque.Inserir(produtoEstoque);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoEstoque.Produto, null, "Adicionou novo local de armazenamento do produto", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o local.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> TransferirLocalArmazenamentoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico repProdutoEstoqueHistorico = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico(unitOfWork);
                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("LocalArmazenamentoTransferencia"), out int codigoLocalArmazenamento);
                decimal.TryParse(Request.Params("QuantidadeTransferencia"), out decimal quantidadeTransferencia);

                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoque = repProdutoEstoque.BuscarPorCodigo(codigo);

                if (produtoEstoque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (quantidadeTransferencia > produtoEstoque.Quantidade)
                    return new JsonpResult(false, true, "Não é possível transferir uma quantidade maior que a existente.");

                if (produtoEstoque.LocalArmazenamento.Codigo == codigoLocalArmazenamento)
                    return new JsonpResult(false, true, "Não é possível transferir para o mesmo local.");

                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoqueTransferencia = repProdutoEstoque.BuscarPorProdutoEmpresaLocalArmazenamento(produtoEstoque.Produto.Codigo, produtoEstoque.Empresa?.Codigo ?? 0, codigoLocalArmazenamento);

                if (produtoEstoqueTransferencia == null)
                    produtoEstoqueTransferencia = repProdutoEstoque.BuscarPorProdutoEmpresaLocalArmazenamento(produtoEstoque.Produto.Codigo, 0, codigoLocalArmazenamento);

                if (produtoEstoqueTransferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar um registro com o Local a ser transferido para esse produto.");

                produtoEstoque.Initialize();
                produtoEstoqueTransferencia.Initialize();

                produtoEstoque.Quantidade = produtoEstoque.Quantidade - quantidadeTransferencia;
                produtoEstoqueTransferencia.Quantidade = produtoEstoqueTransferencia.Quantidade + quantidadeTransferencia;

                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico estoqueHistorico = new Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico()
                {
                    Custo = produtoEstoque.CustoMedio,
                    Data = DateTime.Now,
                    LocalArmazenamento = produtoEstoqueTransferencia.LocalArmazenamento,
                    Documento = "Transferência usuário " + this.Usuario.Nome,
                    Empresa = produtoEstoqueTransferencia.Empresa,
                    Produto = produtoEstoqueTransferencia.Produto,
                    ProdutoEstoque = produtoEstoqueTransferencia,
                    Quantidade = quantidadeTransferencia,
                    Tipo = TipoMovimento.Entrada,
                    TipoDocumento = "Trans"
                };

                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico estoqueHistoricoSaida = new Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico()
                {
                    Custo = produtoEstoque.CustoMedio,
                    Data = DateTime.Now,
                    LocalArmazenamento = produtoEstoque.LocalArmazenamento,
                    Documento = "Transferência usuário " + this.Usuario.Nome,
                    Empresa = produtoEstoque.Empresa,
                    Produto = produtoEstoque.Produto,
                    ProdutoEstoque = produtoEstoque,
                    Quantidade = quantidadeTransferencia,
                    Tipo = TipoMovimento.Saida,
                    TipoDocumento = "Trans"
                };

                unitOfWork.Start();

                repProdutoEstoqueHistorico.Inserir(estoqueHistorico);
                repProdutoEstoqueHistorico.Inserir(estoqueHistoricoSaida);
                repProdutoEstoque.Atualizar(produtoEstoque, Auditado);
                repProdutoEstoque.Atualizar(produtoEstoqueTransferencia, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoEstoque.Produto, null, "Realizou a transferência de estoque entre Locais de Armazenamento do produto", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a transferência do local.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CarregarFotoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhuma foto selecionada para adicionar.");

                Servicos.DTO.CustomFile arquivoFoto = arquivos[0];
                
                string[] pasta = { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "FotosProdutos", "Temp" };
                string extensaoArquivo = System.IO.Path.GetExtension(arquivoFoto.FileName).ToLower();
                string caminho = ObterCaminhoArquivoFoto(pasta);
                string nomeArquivo = "FotoTemporariaProduto";
                string nomeArquivoFotoExistente = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{nomeArquivo}.*").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(nomeArquivoFotoExistente))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivoFotoExistente);

                arquivoFoto.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{nomeArquivo}{extensaoArquivo}"));

                return new JsonpResult(new
                {
                    FotoProduto = ObterFotoBase64(nomeArquivo, pasta, unitOfWork)
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a foto do produto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarEtiqueta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Produto repositoriProduto = new Repositorio.Produto(unitOfWork);
                Servicos.Embarcador.Produto.Produto servicoProduto = new Servicos.Embarcador.Produto.Produto(unitOfWork);

                Dominio.Entidades.Produto produto = repositoriProduto.BuscarPorCodigo(codigo);

                if (produto == null)
                    return new JsonpResult(false, true, "Nenhum registro encontrado");

                byte[] pdf = servicoProduto.ObterEtiquetaProduto(produto);

                return Arquivo(pdf, "application/pdf", $"Etiqueta Produto {produto.Descricao}.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um problema ao gerar etiqueta do produto ");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherProduto(Dominio.Entidades.Produto produto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);
            Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProdutoOrdemServico = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unitOfWork);

            produto.CodigoBarrasEAN = Request.Params("CodigoBarrasEAN");
            produto.Status = Request.Params("Status");
            produto.Descricao = Request.Params("Descricao");
            produto.CodigoAnvisa = Request.Params("CodigoAnvisa");            
            produto.CodigoNCM = Request.Params("CodigoNCM");
            if (produto.CodigoNCM == "0")
                produto.CodigoNCM = "00000000";
            if (produto.CodigoNCM.Length < 8)
                throw new ControllerException("Favor verifique o NCM informado.");
            produto.CodigoProduto = Request.Params("CodigoProduto");
            produto.UnidadeDeMedida = Request.GetEnumParam<UnidadeDeMedida>("UnidadeMedida");

            decimal.TryParse(Request.Params("UltimoCusto"), out decimal ultimoCusto);
            decimal.TryParse(Request.Params("CustoMedio"), out decimal custoMedio);
            decimal.TryParse(Request.Params("MargemLucro"), out decimal margemLucro);
            decimal.TryParse(Request.Params("ValorVenda"), out decimal valorVenda);
            decimal.TryParse(Request.Params("PesoBruto"), out decimal pesoBruto);
            decimal.TryParse(Request.Params("PesoLiquido"), out decimal pesoLiquido);
            decimal.TryParse(Request.Params("AliquotaIPIVenda"), out decimal aliquotaIPIVenda);
            decimal.TryParse(Request.Params("AliquotaIPICompra"), out decimal aliquotaIPICompra);

            decimal.TryParse(Request.Params("ValorMinimoVenda"), out decimal valorMinimoVenda);
            

            IndicadorEscalaRelevante indicadorEscalaRelevante = IndicadorEscalaRelevante.Nenhum;
            Enum.TryParse(Request.Params("IndicadorEscalaRelevante"), out indicadorEscalaRelevante);
            string cnpjFabricante = Request.Params("CNPJFabricante");
            string codigoBeneficioFiscal = Request.Params("CodigoBeneficioFiscal");

            Enum.TryParse(Request.Params("OrigemMercadoria"), out OrigemMercadoria origemMercadoria);
            Enum.TryParse(Request.Params("CSTIPIVenda"), out CSTIPI CSTIPIVenda);
            Enum.TryParse(Request.Params("CSTIPICompra"), out CSTIPI CSTIPICompra);
            Enum.TryParse(Request.Params("CategoriaProduto"), out CategoriaProduto categoriaProduto);
            Enum.TryParse(Request.Params("GeneroProduto"), out GeneroProduto generoProduto);

           
            produto.IndicadorEscalaRelevante = indicadorEscalaRelevante;
            produto.CNPJFabricante = cnpjFabricante;
            produto.CodigoBeneficioFiscal = codigoBeneficioFiscal;
            produto.DescricaoNotaFiscal = Request.Params("DescricaoNotaFiscal");
            produto.CodigoEAN = Request.Params("CodigoEAN");
            produto.CodigoEnquadramentoIPI = Request.Params("CodigoEnquadramentoIPI");
            string codigoCEST = Request.Params("CodigoCEST");
            produto.CodigoCEST = !string.IsNullOrEmpty(codigoCEST) && codigoCEST != "0" ? codigoCEST : null;

            produto.OrigemMercadoria = origemMercadoria;
            produto.CSTIPIVenda = CSTIPIVenda;
            produto.CSTIPICompra = CSTIPICompra;
            produto.CategoriaProduto = categoriaProduto;
            produto.GeneroProduto = generoProduto;

            produto.UltimoCusto = ultimoCusto;
            produto.CustoMedio = custoMedio;
            produto.MargemLucro = margemLucro;
            produto.ValorVenda = valorVenda;
            produto.PesoBruto = pesoBruto;
            produto.PesoLiquido = pesoLiquido;
            produto.AliquotaIPIVenda = aliquotaIPIVenda;
            produto.AliquotaIPICompra = aliquotaIPICompra;           
            produto.ValorMinimoVenda = valorMinimoVenda;

            string calculoPadrao = Request.Params("CalculoCustoProduto");
            if (string.IsNullOrWhiteSpace(calculoPadrao))
                calculoPadrao = Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unitOfWork);
            produto.CalculoCustoProduto = calculoPadrao;

            produto.ProdutoKIT = Request.GetBoolParam("ProdutoKIT");

            int codigoGrupoImposto = Request.GetIntParam("GrupoImposto");
            int codigoGrupoProduto = Request.GetIntParam("GrupoProdutoTMS");
            int codigoLocalArmazenamentoProduto = Request.GetIntParam("LocalArmazenamentoProduto");
            int codigoMarcaProduto = Request.GetIntParam("MarcaProduto");
            int codigoFinalidadeProdutoOS = Request.GetIntParam("FinalidadeProdutoOrdemServico");

            produto.GrupoImposto = codigoGrupoImposto > 0 ? repGrupoImposto.BuscarPorCodigo(codigoGrupoImposto) : null;
            produto.GrupoProdutoTMS = codigoGrupoProduto > 0 ? repGrupoProduto.BuscarPorCodigo(codigoGrupoProduto) : null;
            produto.LocalArmazenamentoProduto = codigoLocalArmazenamentoProduto > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamentoProduto, false) : null;
            produto.MarcaProduto = codigoMarcaProduto > 0 ? repMarcaProduto.BuscarPorCodigo(codigoMarcaProduto, false) : null;
            produto.FinalidadeProdutoOrdemServico = codigoFinalidadeProdutoOS > 0 ? repFinalidadeProdutoOrdemServico.BuscarPorCodigo(codigoFinalidadeProdutoOS, false) : null;

            if (produto.Codigo == 0)
            {
                int codigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;
                produto.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            }
        }

        private void SalvarFornecedores(Dominio.Entidades.Produto produto, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            dynamic fornecedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Fornecedores"));

            if (produto.Fornecedores != null && produto.Fornecedores.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var fornecedor in fornecedores)
                    if (fornecedor.Codigo != null)
                        codigos.Add((int)fornecedor.Codigo);

                List<Dominio.Entidades.ProdutoFornecedor> produtoFornecedorDeletar = (from obj in produto.Fornecedores where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < produtoFornecedorDeletar.Count; i++)
                    repProdutoFornecedor.Deletar(produtoFornecedorDeletar[i]);
            }
            else
            {
                produto.Fornecedores = new List<Dominio.Entidades.ProdutoFornecedor>();
            }

            foreach (var fornecedor in fornecedores)
            {
                double cpfCnpjFornecedor;
                double.TryParse((string)fornecedor.Fornecedor.Codigo, out cpfCnpjFornecedor);

                Dominio.Entidades.ProdutoFornecedor produtoFornecedor = fornecedor.Codigo != null ? repProdutoFornecedor.BuscarPorCodigo((int)fornecedor.Codigo) : null;

                if (produtoFornecedor == null)
                    produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor();

                produtoFornecedor.Produto = produto;
                produtoFornecedor.Fornecedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor);
                produtoFornecedor.CodigoProduto = (string)fornecedor.CodigoProduto;
                decimal fatorConversao = 0;
                if (!string.IsNullOrWhiteSpace((string)fornecedor.FatorConversao))
                {
                    if (((string)fornecedor.FatorConversao).Contains("-"))
                        fatorConversao = decimal.Parse(((string)fornecedor.FatorConversao).Replace(",", "").Replace(".", ","));
                    else
                        fatorConversao = Utilidades.Decimal.Converter((string)fornecedor.FatorConversao);
                }
                produtoFornecedor.FatorConversao = fatorConversao;

                if (produtoFornecedor.Codigo > 0)
                    repProdutoFornecedor.Atualizar(produtoFornecedor);
                else
                    repProdutoFornecedor.Inserir(produtoFornecedor);
            }
        }

        private void SalvarPneus(Dominio.Entidades.Produto produto, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.ModeloPneu repModeloPneu = new Repositorio.Embarcador.Frota.ModeloPneu(unidadeTrabalho);
            Repositorio.Embarcador.Frota.BandaRodagemPneu repBandaRodagemPneu = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unidadeTrabalho);

            dynamic pneu = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Pneu"));

            bool gerarPneuAutomatico;
            bool.TryParse((string)pneu.GerarPneuAutomatico, out gerarPneuAutomatico);

            int codigoModeloPneu, codigoBandaRodagemPneu;
            int.TryParse((string)pneu.ModeloPneu, out codigoModeloPneu);
            int.TryParse((string)pneu.BandaRodagemPneu, out codigoBandaRodagemPneu);

            produto.GerarPneuAutomatico = gerarPneuAutomatico;
            if (codigoModeloPneu > 0)
                produto.Modelo = repModeloPneu.BuscarPorCodigo(codigoModeloPneu);
            else
                produto.Modelo = null;
            if (codigoBandaRodagemPneu > 0)
                produto.BandaRodagem = repBandaRodagemPneu.BuscarPorCodigo(codigoBandaRodagemPneu);
            else
                produto.BandaRodagem = null;
        }

        private void SalvarEPI(Dominio.Entidades.Produto produto)
        {
            dynamic epi = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EPI"));

            bool.TryParse((string)epi.ProdutoEPI, out bool produtoEPI);
            produto.ProdutoEPI = produtoEPI;
            produto.NumeroCA = (string)epi.NumeroCA;
        }

        private void SalvarCombustivel(Dominio.Entidades.Produto produto)
        {
            dynamic combustivel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Combustivel"));

            bool.TryParse((string)combustivel.ProdutoCombustivel, out bool produtoCombustivel);
            bool.TryParse((string)combustivel.ControlaEstoqueCombustivel, out bool controlaEstoqueCombustivel);

            decimal.TryParse((string)combustivel.PercentualGLP, out decimal percentualGLP);
            decimal.TryParse((string)combustivel.PercentualGNN, out decimal percentualGNN);
            decimal.TryParse((string)combustivel.PercentualGNI, out decimal percentualGNI);
            decimal.TryParse((string)combustivel.ValorPartidaANP, out decimal valorPartidaANP);
            decimal.TryParse((string)combustivel.PercentualOrigemCombustivel, out decimal percentualOrigemCombustivel);
            decimal.TryParse((string)combustivel.PercentualMisturaBiodiesel, out decimal percentualMisturaBiodiesel);

            IndicadorImportacaoCombustivel indicadorImportacaoCombustivel = IndicadorImportacaoCombustivel.Nenhum;
            Enum.TryParse((string)combustivel.IndicadorImportacaoCombustivel, out indicadorImportacaoCombustivel);

            produto.ProdutoCombustivel = produtoCombustivel;
            produto.ControlaEstoqueCombustivel = controlaEstoqueCombustivel;
            produto.CodigoANP = (string)combustivel.CodigoANP;
            produto.IndicadorImportacaoCombustivel = indicadorImportacaoCombustivel;
            produto.PercentualGLP = percentualGLP;
            produto.PercentualGNN = percentualGNN;
            produto.PercentualGNI = percentualGNI;
            produto.ValorPartidaANP = valorPartidaANP;
            produto.PercentualOrigemCombustivel = percentualOrigemCombustivel;
            produto.PercentualMisturaBiodiesel = percentualMisturaBiodiesel;
        }

        private void SalvarComposicoes(Dominio.Entidades.Produto produto, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Produtos.ProdutoComposicao repProdutoComposicao = new Repositorio.Embarcador.Produtos.ProdutoComposicao(unidadeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);

            dynamic composicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Composicoes"));
            if (produto.Composicoes != null && produto.Composicoes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var composicao in composicoes)
                    if (composicao.Codigo != null)
                        codigos.Add((int)composicao.Codigo);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoComposicao> produtoComposicaoDeletar = (from obj in produto.Composicoes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < produtoComposicaoDeletar.Count; i++)
                    repProdutoComposicao.Deletar(produtoComposicaoDeletar[i]);
            }
            else
                produto.Composicoes = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoComposicao>();

            foreach (var composicao in composicoes)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoComposicao produtoComposicao = composicao.Codigo != null ? repProdutoComposicao.BuscarPorCodigo((int)composicao.Codigo) : null;
                if (produtoComposicao == null)
                    produtoComposicao = new Dominio.Entidades.Embarcador.Produtos.ProdutoComposicao();

                produtoComposicao.Produto = produto;
                produtoComposicao.Insumo = repProduto.BuscarPorCodigo((int)composicao.Produto.Codigo);
                produtoComposicao.Quantidade = Utilidades.Decimal.Converter((string)composicao.Quantidade);

                if (produtoComposicao.Codigo > 0)
                    repProdutoComposicao.Atualizar(produtoComposicao);
                else
                    repProdutoComposicao.Inserir(produtoComposicao);
            }
        }

        private void SalvarBem(Dominio.Entidades.Produto produto, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unidadeTrabalho);

            dynamic bem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Bem"));

            bool.TryParse((string)bem.ProdutoBem, out bool produtoBem);

            int codigoAlmoxarifado, codigoCentroResultado;
            int.TryParse((string)bem.Almoxarifado, out codigoAlmoxarifado);
            int.TryParse((string)bem.CentroResultado, out codigoCentroResultado);

            produto.ProdutoBem = produtoBem;
            if (codigoAlmoxarifado > 0)
                produto.Almoxarifado = repAlmoxarifado.BuscarPorCodigo(codigoAlmoxarifado);
            else
                produto.Almoxarifado = null;
            if (codigoCentroResultado > 0)
                produto.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);
            else
                produto.CentroResultado = null;
        }

        private string ObterCaminhoArquivoFoto(string[] caminho)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(caminho);
        }

        private string ObterFotoBase64(string arquivo, string[] pasta, Repositorio.UnitOfWork unitOfWork)
        {
            if (pasta.Length <= 0)
                pasta = new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "FotosProdutos" };
            
            string caminho = ObterCaminhoArquivoFoto(pasta);
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{arquivo}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        #endregion

        #region Importação

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoProduto()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Cód Produto", Propriedade = "CodigoProduto", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Descrição", Propriedade = "Descricao", Tamanho = 400, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "NCM", Propriedade = "NCM", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Un. Medida", Propriedade = "UM", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "CEST", Propriedade = "CEST", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Cód. EAN", Propriedade = "EAN", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Custo", Propriedade = "Custo", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Valor Venda", Propriedade = "ValorVenda", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Peso Bruto", Propriedade = "PesoBruto", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Peso Líquido", Propriedade = "PesoLiquido", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Cód. Barras", Propriedade = "Barras", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Estoque", Propriedade = "Estoque", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Grupo Produto", Propriedade = "GrupoProduto", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Tipo Item", Propriedade = "TipoItem", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProduto();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProduto();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                        Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                        Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                        Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico repProdutoEstoqueHistorico = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico(unitOfWork);
                        Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProdutoTMS = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);

                        Dominio.Entidades.Produto produto = new Dominio.Entidades.Produto();
                        produto.Empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa?.Codigo ?? 0);
                        produto.IndicadorEscalaRelevante = IndicadorEscalaRelevante.Nenhum;
                        produto.IndicadorImportacaoCombustivel = IndicadorImportacaoCombustivel.Nenhum;
                        produto.OrigemMercadoria = OrigemMercadoria.Origem0;
                        produto.CategoriaProduto = CategoriaProduto.MercadoriaRevenda;
                        produto.Status = "A";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoProduto = (from obj in linha.Colunas where obj.NomeCampo == "CodigoProduto" select obj).FirstOrDefault();
                        if (colCodigoProduto != null)
                            produto.CodigoProduto = colCodigoProduto.Valor.Trim();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricao = (from obj in linha.Colunas where obj.NomeCampo == "Descricao" select obj).FirstOrDefault();
                        if (colDescricao != null)
                        {
                            produto.Descricao = colDescricao.Valor.Trim();
                            produto.DescricaoNotaFiscal = colDescricao.Valor.Trim();
                        }
                        else
                            retorno = "Descrição é obrigatória";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNCM = (from obj in linha.Colunas where obj.NomeCampo == "NCM" select obj).FirstOrDefault();
                        if (colNCM != null)
                        {
                            string codigoNCM = ((string)colNCM.Valor).ObterSomenteNumeros();
                            if (colNCM.Valor.Trim() == "0")
                                codigoNCM = "00000000";
                            if (colNCM.Valor.Trim().Length < 8)
                                retorno = "NCM é inválido";
                            else
                                produto.CodigoNCM = codigoNCM;
                        }
                        else
                            retorno = "NCM é obrigatório";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUM = (from obj in linha.Colunas where obj.NomeCampo == "UM" select obj).FirstOrDefault();
                        if (colUM != null)
                        {
                            UnidadeDeMedida unidadeDeMedida = UnidadeDeMedida.Unidade;
                            unidadeDeMedida = repProduto.ObterUnidadeMedida(colUM.Valor, unitOfWork, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? produto.Empresa.Codigo : 0);

                            if (unidadeDeMedida > 0)
                                produto.UnidadeDeMedida = unidadeDeMedida;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEST = (from obj in linha.Colunas where obj.NomeCampo == "CEST" select obj).FirstOrDefault();
                        if (colCEST != null)
                        {
                            string codigoCEST = colCEST.Valor.Trim();
                            if (!string.IsNullOrEmpty(codigoCEST) && codigoCEST != "0" && codigoCEST.Length == 7)
                                produto.CodigoCEST = codigoCEST;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEAN = (from obj in linha.Colunas where obj.NomeCampo == "EAN" select obj).FirstOrDefault();
                        if (colEAN != null)
                            produto.CodigoBarrasEAN = colEAN.Valor.Trim();

                        decimal custo = 0;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCusto = (from obj in linha.Colunas where obj.NomeCampo == "Custo" select obj).FirstOrDefault();
                        if (colCusto != null)
                        {
                            decimal.TryParse(colCusto.Valor, out custo);
                            produto.UltimoCusto = custo;
                            produto.CustoMedio = custo;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorVenda = (from obj in linha.Colunas where obj.NomeCampo == "ValorVenda" select obj).FirstOrDefault();
                        if (colValorVenda != null)
                        {
                            decimal valorVenda = 0;
                            decimal.TryParse(colValorVenda.Valor, out valorVenda);
                            produto.ValorVenda = valorVenda;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoBruto = (from obj in linha.Colunas where obj.NomeCampo == "PesoBruto" select obj).FirstOrDefault();
                        if (colPesoBruto != null)
                        {
                            decimal pesoBruto = 0;
                            decimal.TryParse(colPesoBruto.Valor, out pesoBruto);
                            produto.PesoBruto = pesoBruto;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoLiquido = (from obj in linha.Colunas where obj.NomeCampo == "PesoLiquido" select obj).FirstOrDefault();
                        if (colPesoLiquido != null)
                        {
                            decimal pesoLiquido = 0;
                            decimal.TryParse(colPesoLiquido.Valor, out pesoLiquido);
                            produto.PesoLiquido = pesoLiquido;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBarras = (from obj in linha.Colunas where obj.NomeCampo == "Barras" select obj).FirstOrDefault();
                        if (colBarras != null)
                            produto.CodigoEAN = colBarras.Valor.Trim();

                        if (ncmsAbastecimento != null && ncmsAbastecimento.Count() > 0 && !string.IsNullOrWhiteSpace(produto.CodigoNCM))
                        {
                            if (ncmsAbastecimento.Where(o => produto.CodigoNCM.Contains(o.NCM)).Count() > 0)
                                produto.ProdutoCombustivel = true;
                            else
                                produto.ProdutoCombustivel = false;
                        }
                        else
                            produto.ProdutoCombustivel = false;

                        CategoriaProduto tipoItem;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoItem = (from obj in linha.Colunas where obj.NomeCampo == "TipoItem" select obj).FirstOrDefault();
                        if (colTipoItem != null)
                        {
                            int.TryParse(colTipoItem.Valor, out int tipoItemNumero);
                            tipoItem = (CategoriaProduto)tipoItemNumero;
                            if (!string.IsNullOrWhiteSpace(colTipoItem.Valor))
                                produto.CategoriaProduto = tipoItem;
                        }

                        repProduto.Inserir(produto, Auditado);

                        decimal estoque = 0;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEstoque = (from obj in linha.Colunas where obj.NomeCampo == "Estoque" select obj).FirstOrDefault();
                        if (colEstoque != null)
                            decimal.TryParse(colEstoque.Valor, out estoque);

                        string grupoProdutoDescricao = "";
                        Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProdutoTMS = null;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoProduto = (from obj in linha.Colunas where obj.NomeCampo == "GrupoProduto" select obj).FirstOrDefault();
                        if (colGrupoProduto != null)
                        {
                            grupoProdutoDescricao = colGrupoProduto.Valor;
                            grupoProdutoTMS = repGrupoProdutoTMS.BuscarPorDescricao(grupoProdutoDescricao);
                        }

                        produto.GrupoProdutoTMS = grupoProdutoTMS;

                        servicoEstoque.MovimentarEstoque(out string erro, produto, estoque, TipoMovimento.Entrada, "IMP", (i + 1).ToString(), custo, produto.Empresa, DateTime.Now, TipoServicoMultisoftware);

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                        }
                        else
                        {
                            contador++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
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

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private bool VerificarSeCodigoDeBarras(string codigoBarras)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras) || codigoBarras.Length < 6)
                return false;

            var catchBarCode = codigoBarras.Substring(0, 6);

            if (catchBarCode == "990000")
                return true;
            else
                return false;
        }


        #endregion
    }
}
