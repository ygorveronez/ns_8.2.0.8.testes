using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.NotaFiscal
{
    public class NotaFiscalProduto : ServicoBase
    {
        public NotaFiscalProduto(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public void SalvarItensNFe(ref Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto> itesNFe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa, out decimal valorIBPT, out decimal valorCreditoICMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            valorIBPT = 0;
            valorCreditoICMS = 0;

            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutosLotes repNotaFiscalProdutosLotes = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutosLotes(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
            if (nfe.Codigo > 0)
            {
                repNotaFiscalProdutosLotes.DeletarPorNFe(nfe.Codigo);
                repNotaFiscalProdutos.DeletarPorNFe(nfe.Codigo);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto prod in itesNFe)
            {
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos produto = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos();
                if (!string.IsNullOrWhiteSpace(prod.CodigoItem) && prod.CodigoItem.Length >= 60)
                    produto.CodigoItem = prod.CodigoItem.Substring(0, 59).Trim().TrimEnd().TrimStart();
                else
                    produto.CodigoItem = prod.CodigoItem;
                if (!string.IsNullOrWhiteSpace(prod.DescricaoItem) && prod.DescricaoItem.Length >= 120)
                    produto.DescricaoItem = prod.DescricaoItem.Substring(0, 119).Trim().TrimEnd().TrimStart();
                else
                    produto.DescricaoItem = prod.DescricaoItem;
                produto.AliquotaCOFINS = prod.AliquotaCOFINS;
                produto.AliquotaFCP = prod.AliquotaFCP;
                produto.AliquotaICMS = prod.AliquotaICMS;
                produto.AliquotaICMSDestino = prod.AliquotaICMSDestino;
                produto.AliquotaICMSInterno = prod.AliquotaICMSInterno;
                produto.AliquotaICMSOperacao = prod.AliquotaICMSOperacao;
                produto.AliquotaICMSST = prod.AliquotaICMSST;
                produto.AliquotaICMSSTInterestadual = prod.AliquotaICMSSTInterestadual;
                produto.AliquotaIPI = prod.AliquotaIPI;
                produto.AliquotaISS = prod.AliquotaISS;
                produto.AliquotaPIS = prod.AliquotaPIS;
                produto.BaseII = prod.BaseII;
                produto.BaseISS = prod.BaseISS;
                produto.BCCOFINS = prod.BCCOFINS;
                produto.BCDeducao = prod.BCDeducao;
                produto.AliquotaRemICMSRet = prod.AliquotaRemICMSRet;
                produto.ValorICMSMonoRet = prod.ValorICMSMonoRet;
                produto.BCICMS = prod.BCICMS;
                produto.BCICMSDestino = prod.BCICMSDestino;
                produto.BCICMSST = prod.BCICMSST;
                produto.BCIPI = prod.BCIPI;
                produto.BCPIS = prod.BCPIS;
                produto.CFOP = repCFOP.BuscarPorCodigo(prod.CFOP);
                produto.CNPJAdquirente = Utilidades.String.OnlyNumbers(prod.CNPJAdquirente);
                produto.CSTCOFINS = prod.CSTCOFINS;
                produto.CSTICMS = prod.CSTICMS;
                produto.CSTIPI = prod.CSTIPI;
                produto.CSTPIS = prod.CSTPIS;
                if (prod.DataDesembaraco > DateTime.MinValue)
                    produto.DataDesembaraco = prod.DataDesembaraco;
                if (prod.DataRegistroImportacao > DateTime.MinValue)
                    produto.DataRegistroImportacao = prod.DataRegistroImportacao;
                produto.DescontoCondicional = prod.DescontoCondicional;
                produto.DescontoIncondicional = prod.DescontoIncondicional;
                produto.ExigibilidadeISS = prod.ExigibilidadeISS;
                produto.IncentivoFiscal = prod.IncentivoFiscal;
                produto.IntermediacaoII = prod.IntermediacaoII;
                produto.LocalDesembaraco = prod.LocalDesembaraco;
                produto.MotivoDesoneracao = prod.MotivoDesoneracao;
                produto.MVAICMSST = prod.MVAICMSST;
                produto.NotaFiscal = nfe;
                produto.NumeroDocImportacao = prod.NumeroDocImportacao;
                produto.NumeroItemOrdemCompra = prod.NumeroItemOrdemCompra;
                produto.NumeroOrdemCompra = prod.NumeroOrdemCompra;
                produto.OutrasRetencoes = prod.OutrasRetencoes;
                produto.PercentualPartilha = prod.PercentualPartilha;
                produto.ProcessoJudicial = prod.ProcessoJudicial;
                if (prod.Produto != null && prod.Produto.Codigo > 0)
                    produto.Produto = repProduto.BuscarPorCodigo(prod.Produto.Codigo);
                else if (prod.Produto != null && !string.IsNullOrWhiteSpace(prod.Produto.CodigoIntegracao))
                {
                    produto.Produto = repProduto.BuscarPorCodigoIntegracao(empresa.Codigo, prod.Produto.CodigoIntegracao);
                    if (produto.Produto == null)
                    {
                        int codigoProduto = CadastrarProduto(prod.Produto, prod.Produto.NCM, empresa.Codigo, unitOfWork, ncmsAbastecimento, tipoServicoMultisoftware, configuracao);
                        produto.Produto = repProduto.BuscarPorCodigo(codigoProduto);
                    }
                }
                produto.Quantidade = prod.Quantidade;
                produto.ReducaoBCCOFINS = prod.ReducaoBCCOFINS;
                produto.ReducaoBCICMS = prod.ReducaoBCICMS;
                produto.ReducaoBCICMSST = prod.ReducaoBCICMSST;
                produto.ReducaoBCIPI = prod.ReducaoBCIPI;
                produto.ReducaoBCPIS = prod.ReducaoBCPIS;
                produto.RetencaoISS = prod.RetencaoISS;
                if (prod.Servico != null && prod.Servico.Codigo > 0)
                    produto.Servico = repServico.BuscarPorCodigo(prod.Servico.Codigo);
                else if (prod.Servico != null && !string.IsNullOrWhiteSpace(prod.Servico.CodigoIntegracao))
                {
                    produto.Servico = repServico.BuscarPorCodigoIntegracao(prod.Servico.CodigoIntegracao, empresa.Codigo);
                    if (produto.Servico == null)
                    {
                        int codigoServico = CadastrarServico(prod.Servico, empresa.Codigo, unitOfWork);
                        produto.Servico = repServico.BuscarPorCodigo(codigoServico);
                    }
                }
                produto.UFDesembaraco = prod.UFDesembaraco;
                produto.ValorCOFINS = prod.ValorCOFINS;
                produto.ValorDesconto = prod.ValorDesconto;
                produto.ValorDespesaII = prod.ValorDespesaII;
                produto.ValorFCP = prod.ValorFCP;
                produto.ValorFrete = prod.ValorFrete;
                produto.ValorFreteMarinho = prod.ValorFreteMarinho;
                produto.ValorICMS = prod.ValorICMS;
                produto.ValorICMSDesonerado = prod.ValorICMSDesonerado;
                produto.ValorICMSDestino = prod.ValorICMSDestino;
                produto.ValorICMSDiferido = prod.ValorICMSDiferido;
                produto.ValorICMSOperacao = prod.ValorICMSOperacao;
                produto.ValorICMSRemetente = prod.ValorICMSRemetente;
                produto.ValorICMSST = prod.ValorICMSST;
                produto.ValorII = prod.ValorII;
                produto.ValorIOFII = prod.ValorIOFII;
                produto.ValorIPI = prod.ValorIPI;
                produto.ValorISS = prod.ValorISS;
                produto.ValorOutrasDespesas = prod.ValorOutrasDespesas;
                produto.ValorPIS = prod.ValorPIS;
                produto.ValorSeguro = prod.ValorSeguro;
                produto.ValorTotal = prod.ValorTotal;
                produto.ValorUnitario = prod.ValorUnitario;
                produto.ViaTransporteII = prod.ViaTransporteII;

                if (empresa.OptanteSimplesNacional && empresa.AliquotaICMSSimples > 0 && (produto.CSTICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101 || produto.CSTICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201 || produto.CSTICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900))
                {
                    produto.AliquotaICMSSimples = empresa.AliquotaICMSSimples;
                    produto.ValorICMSSimples = (empresa.AliquotaICMSSimples / 100) * prod.ValorTotal;
                    valorCreditoICMS += produto.ValorICMSSimples;
                }
                else
                {
                    produto.AliquotaICMSSimples = 0;
                    produto.ValorICMSSimples = 0;
                }
                int empresaPai = 0;
                if (empresa.EmpresaPai != null)
                    empresaPai = empresa.EmpresaPai.Codigo;

                if (empresa.CalculaIBPTNFe)
                {
                    if (produto.Produto != null)
                        produto.ValorImpostoIBPT = Math.Round(RetornaValorIBPT(empresaPai, empresa.Codigo, Utilidades.String.OnlyNumbers(produto.Produto.CodigoNCM), produto.ValorTotal, unitOfWork, 3), 2);
                    else if (produto.Servico != null)
                        produto.ValorImpostoIBPT = Math.Round(RetornaValorIBPT(empresaPai, empresa.Codigo, Utilidades.String.OnlyNumbers(produto.Servico.NumeroCodigoServico), produto.ValorTotal, unitOfWork, 3), 2);
                    valorIBPT += produto.ValorImpostoIBPT;
                }

                produto.BCFCPICMS = prod.BaseFCPICMS;
                produto.PercentualFCPICMS = prod.PercentualFCPICMS;
                produto.ValorFCPICMS = prod.ValorFCPICMS;
                produto.BCFCPICMSST = prod.BaseFCPICMSST;
                produto.PercentualFCPICMSST = prod.PercentualFCPICMSST;
                produto.ValorFCPICMSST = prod.ValorFCPICMSST;
                produto.AliquotaFCPICMSST = prod.AliquotaFCPICMSST;
                produto.BCFCPDestino = prod.BaseFCPDestino;
                produto.PercentualIPIDevolvido = prod.PercentualIPIDevolvido;
                produto.ValorIPIDevolvido = prod.ValorIPIDevolvido;
                produto.InformacoesAdicionais = prod.InformacoesAdicionais;
                produto.IndicadorEscalaRelevante = prod.IndicadorEscalaRelevante;
                produto.CNPJFabricante = prod.CNPJFabricante;
                produto.CodigoBeneficioFiscal = prod.CodigoBeneficioFiscal;
                produto.CodigoANP = prod.CodigoANP;
                produto.PercentualGLP = prod.PercentualGLP;
                produto.PercentualGNN = prod.PercentualGNN;
                produto.PercentualGNI = prod.PercentualGNI;
                produto.PercentualOrigemComb = prod.PercentualOrigemComb;
                produto.PercentualMisturaBiodiesel = prod.PercentualMisturaBiodiesel;
                produto.ValorPartidaANP = prod.ValorPartidaANP;
                produto.QuantidadeTributavel = prod.QuantidadeTributavel;
                produto.ValorUnitarioTributavel = prod.ValorUnitarioTributavel;
                produto.UnidadeDeMedidaTributavel = prod.UnidadeDeMedidaTributavel;
                produto.OrigemMercadoria = prod.OrigemMercadoria;
                produto.CodigoNFCI = prod.CodigoNFCI;
                produto.CodigoEANTributavel = prod.CodigoEANTributavel;
                produto.BCICMSSTRetido = prod.BCICMSSTRetido;
                produto.AliquotaICMSSTRetido = prod.AliquotaICMSSTRetido;
                produto.ValorICMSSTSubstituto = prod.ValorICMSSTSubstituto;
                produto.ValorICMSSTRetido = prod.ValorICMSSTRetido;
                produto.BCICMSEfetivo = prod.BCICMSEfetivo;
                produto.AliquotaICMSEfetivo = prod.AliquotaICMSEfetivo;
                produto.ReducaoBCICMSEfetivo = prod.ReducaoBCICMSEfetivo;
                produto.ValorICMSEfetivo = prod.ValorICMSEfetivo;
                produto.NumeroDrawback = prod.NumeroDrawback;
                produto.NumeroRegistroExportacao = prod.NumeroRegistroExportacao;
                produto.ChaveAcessoExportacao = prod.ChaveAcessoExportacao;
                produto.LocalArmazenamento = prod.LocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(prod.LocalArmazenamento) : null;
                produto.BCICMSSTDestino = prod.BCICMSSTDestino;
                produto.ValorICMSSTDestino = prod.ValorICMSSTDestino;
                produto.Sequencial = prod.Sequencial;

                repNotaFiscalProdutos.Inserir(produto);

                if (prod.LotesItem != null && prod.LotesItem.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProdutoLote lot in prod.LotesItem)
                    {
                        Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes lote = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes();
                        lote.NumeroLote = lot.NumeroLote;
                        lote.CodigoAgregacao = lot.CodigoAgregacao;
                        lote.QuantidadeLote = lot.QuantidadeLote;
                        lote.DataFabricacao = lot.DataFabricacao;
                        lote.DataValidade = lot.DataValidade;
                        lote.NotaFiscalProdutos = produto;

                        repNotaFiscalProdutosLotes.Inserir(lote);
                    }
                }
            }
        }

        public int CadastrarProduto(string descricao, string codigoProduto, string ncm, Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida unidadeMedida, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);

            Dominio.Entidades.Produto produto = null;
            produto = repProduto.BuscarPorCodigoProduto(codigoProduto);
            if (produto != null)
                return produto.Codigo;
            else
                produto = new Dominio.Entidades.Produto();

            new Dominio.Entidades.Produto();
            produto.Status = "A";
            produto.Descricao = descricao;
            produto.CodigoNCM = ncm;
            produto.CodigoProduto = codigoProduto;
            produto.CodigoBarrasEAN = "";
            produto.UnidadeDeMedida = unidadeMedida;
            produto.GrupoImposto = null;
            produto.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            produto.DescricaoNotaFiscal = descricao;
            produto.CodigoCEST = null;
            produto.OrigemMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;

            produto.UltimoCusto = 0;
            produto.CustoMedio = 0;
            produto.MargemLucro = 0;
            produto.ValorVenda = 0;
            produto.PesoBruto = 0;
            produto.PesoLiquido = 0;
            produto.ProdutoCombustivel = false;

            repProduto.Inserir(produto);

            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);
            servicoEstoque.AdicionarEstoque(produto, produto.Empresa, tipoServicoMultisoftware, configuracao);

            return produto.Codigo;
        }

        public int CadastrarProduto(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto produtoNFe, string ncm, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);

            Dominio.Entidades.Produto produto = new Dominio.Entidades.Produto();
            produto.Status = "A";
            produto.Descricao = produtoNFe.Descricao;
            produto.CodigoNCM = ncm;
            produto.CodigoProduto = produtoNFe.CodigoIntegracao;
            produto.UnidadeDeMedida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade;
            produto.GrupoImposto = null;
            produto.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            produto.DescricaoNotaFiscal = produtoNFe.Descricao;
            produto.CodigoCEST = null;
            produto.OrigemMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;

            produto.UltimoCusto = 0;
            produto.CustoMedio = 0;
            produto.MargemLucro = 0;
            produto.ValorVenda = 0;
            produto.PesoBruto = 0;
            produto.PesoLiquido = 0;

            if (ncmsAbastecimento != null && ncmsAbastecimento.Count() > 0 && !string.IsNullOrWhiteSpace(produto.CodigoNCM))
            {
                if (ncmsAbastecimento.Where(o => produto.CodigoNCM.Contains(o.NCM)).Count() > 0)
                    produto.ProdutoCombustivel = true;
                else
                    produto.ProdutoCombustivel = false;
            }
            else
                produto.ProdutoCombustivel = false;

            repProduto.Inserir(produto);

            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);
            servicoEstoque.AdicionarEstoque(produto, produto.Empresa, tipoServicoMultisoftware, configuracao);

            return produto.Codigo;
        }

        public int CadastrarServico(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Servico servicoNFe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = new Dominio.Entidades.Embarcador.NotaFiscal.Servico();

            servico.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            servico.CodigoServico = servico.CodigoServicoParaEnum(servicoNFe.CodigoServico);

            servico.Descricao = servicoNFe.Descricao;
            servico.DescricaoNFE = servicoNFe.Descricao;
            servico.CodigoIntegracao = servicoNFe.CodigoIntegracao;

            servico.ValorVenda = 0;
            servico.AliquotaISS = 0;
            servico.Status = true;

            repServico.Inserir(servico);

            return servico.Codigo;
        }

        public decimal RetornaValorIBPT(int codigoEmpresaPai, int codigoEmpresa, string ncm, decimal valorTotal, Repositorio.UnitOfWork unitOfWork, int tipoImposto)
        {
            if (valorTotal > 0)
            {
                Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe repImpostoIBPTNFe = new Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe(unitOfWork);
                if (tipoImposto == 0)//Nacional
                {
                    decimal aliquota = repImpostoIBPTNFe.BuscarAliquotaNacionalNCM(codigoEmpresaPai, codigoEmpresa, ncm);
                    if (aliquota > 0)
                        return Math.Round(valorTotal * (aliquota / 100), 2);
                    else
                        return 0;
                }
                else if (tipoImposto == 1)//Estadual
                {
                    decimal aliquota = repImpostoIBPTNFe.BuscarAliquotaEstadualNCM(codigoEmpresaPai, codigoEmpresa, ncm);
                    if (aliquota > 0)
                        return Math.Round(valorTotal * (aliquota / 100), 2);
                    else
                        return 0;
                }
                else if (tipoImposto == 2)//Municipal
                {
                    decimal aliquota = repImpostoIBPTNFe.BuscarAliquotaMunicipalNCM(codigoEmpresaPai, codigoEmpresa, ncm);
                    if (aliquota > 0)
                        return Math.Round(valorTotal * (aliquota / 100), 2);
                    else
                        return 0;
                }
                else if (tipoImposto == 3)//Todos
                {
                    decimal valorImposto = 0;
                    decimal aliquotaNacional = repImpostoIBPTNFe.BuscarAliquotaNacionalNCM(codigoEmpresaPai, codigoEmpresa, ncm);
                    decimal aliquotaEstadual = repImpostoIBPTNFe.BuscarAliquotaEstadualNCM(codigoEmpresaPai, codigoEmpresa, ncm);
                    decimal aliquotaMunicipal = repImpostoIBPTNFe.BuscarAliquotaMunicipalNCM(codigoEmpresaPai, codigoEmpresa, ncm);
                    if (aliquotaNacional > 0)
                        valorImposto += valorTotal * (aliquotaNacional / 100);
                    if (aliquotaEstadual > 0)
                        valorImposto += valorTotal * (aliquotaEstadual / 100);
                    if (aliquotaMunicipal > 0)
                        valorImposto += valorTotal * (aliquotaMunicipal / 100);
                    return Math.Round(valorImposto, 2);
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto> ConverterNotaFiscalProdutos(List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> produtosNFe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto>();

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos prod in produtosNFe)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto produto = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto();

                produto.AliquotaCOFINS = prod.AliquotaCOFINS;
                produto.AliquotaFCP = prod.AliquotaFCP.Value;
                produto.AliquotaICMS = prod.AliquotaICMS;
                produto.AliquotaICMSDestino = prod.AliquotaICMSDestino;
                produto.AliquotaICMSInterno = prod.AliquotaICMSInterno;
                produto.AliquotaICMSOperacao = prod.AliquotaICMSOperacao.Value;
                produto.AliquotaICMSST = prod.AliquotaICMSST;
                produto.AliquotaICMSSTInterestadual = prod.AliquotaICMSSTInterestadual;
                produto.AliquotaIPI = prod.AliquotaIPI;
                produto.AliquotaISS = prod.AliquotaISS;
                produto.AliquotaPIS = prod.AliquotaPIS;
                produto.BaseII = prod.BaseII;
                produto.BaseISS = prod.BaseISS;
                produto.BCCOFINS = prod.BCCOFINS;
                produto.BCDeducao = prod.BCDeducao.Value;
                produto.AliquotaRemICMSRet = prod.AliquotaRemICMSRet;
                produto.ValorICMSMonoRet = prod.ValorICMSMonoRet;
                produto.BCICMS = prod.BCICMS;
                produto.BCICMSDestino = prod.BCICMSDestino;
                produto.BCICMSST = prod.BCICMSST;
                produto.BCIPI = prod.BCIPI;
                produto.BCPIS = prod.BCPIS;
                produto.CFOP = prod.CFOP.CodigoCFOP;
                produto.DescricaoItem = prod.DescricaoItem;
                produto.CodigoItem = prod.CodigoItem;
                produto.CNPJAdquirente = prod.CNPJAdquirente;
                produto.CSTCOFINS = prod.CSTCOFINS;
                produto.CSTICMS = prod.CSTICMS;
                produto.CSTIPI = prod.CSTIPI;
                produto.CSTPIS = prod.CSTPIS;
                produto.DataDesembaraco = prod.DataDesembaraco;
                produto.DataRegistroImportacao = prod.DataRegistroImportacao;
                produto.DescontoCondicional = prod.DescontoCondicional.Value;
                produto.DescontoIncondicional = prod.DescontoIncondicional.Value;
                produto.ExigibilidadeISS = prod.ExigibilidadeISS;
                produto.IncentivoFiscal = prod.IncentivoFiscal;
                produto.IntermediacaoII = prod.IntermediacaoII;
                produto.LocalDesembaraco = prod.LocalDesembaraco;
                produto.MotivoDesoneracao = prod.MotivoDesoneracao;
                produto.MVAICMSST = prod.MVAICMSST;
                produto.NumeroDocImportacao = prod.NumeroDocImportacao;
                produto.NumeroItemOrdemCompra = prod.NumeroItemOrdemCompra;
                produto.CodigoANP = prod.CodigoANP;
                produto.NumeroOrdemCompra = prod.NumeroOrdemCompra;
                produto.OutrasRetencoes = prod.OutrasRetencoes.Value;
                produto.PercentualPartilha = prod.PercentualPartilha;
                produto.ProcessoJudicial = prod.ProcessoJudicial;
                if (prod.Produto != null && prod.Produto.Codigo > 0)
                    produto.Produto = ConverterObjetoProduto(prod.Produto);
                produto.Quantidade = prod.Quantidade;
                produto.ReducaoBCCOFINS = prod.ReducaoBCCOFINS;
                produto.ReducaoBCICMS = prod.ReducaoBCICMS;
                produto.ReducaoBCICMSST = prod.ReducaoBCICMSST;
                produto.ReducaoBCIPI = prod.ReducaoBCIPI;
                produto.ReducaoBCPIS = prod.ReducaoBCPIS;
                produto.RetencaoISS = prod.RetencaoISS.Value;
                if (prod.Servico != null && prod.Servico.Codigo > 0)
                    produto.Servico = ConverterObjetoServico(prod.Servico);
                produto.UFDesembaraco = prod.UFDesembaraco;
                produto.ValorCOFINS = prod.ValorCOFINS;
                produto.ValorDesconto = prod.ValorDesconto;
                produto.ValorDespesaII = prod.ValorDespesaII;
                produto.ValorFCP = prod.ValorFCP.Value;
                produto.ValorFrete = prod.ValorFrete;
                produto.ValorFreteMarinho = prod.ValorFreteMarinho;
                produto.ValorICMS = prod.ValorICMS;
                produto.ValorICMSDesonerado = prod.ValorICMSDesonerado.Value;
                produto.ValorICMSDestino = prod.ValorICMSDestino;
                produto.ValorICMSDiferido = prod.ValorICMSDiferido.Value;
                produto.ValorICMSOperacao = prod.ValorICMSOperacao.Value;
                produto.ValorICMSRemetente = prod.ValorICMSRemetente;
                produto.ValorICMSST = prod.ValorICMSST;
                produto.ValorII = prod.ValorII;
                produto.ValorIOFII = prod.ValorIOFII;
                produto.ValorIPI = prod.ValorIPI;
                produto.ValorISS = prod.ValorISS;
                produto.ValorOutrasDespesas = prod.ValorOutrasDespesas;
                produto.ValorPIS = prod.ValorPIS;
                produto.ValorSeguro = prod.ValorSeguro;
                produto.ValorTotal = prod.ValorTotal;
                produto.ValorUnitario = prod.ValorUnitario;
                produto.ViaTransporteII = prod.ViaTransporteII;
                produto.BaseFCPDestino = prod.BCFCPDestino.Value;
                produto.BaseFCPICMS = prod.BCFCPICMS.Value;
                produto.PercentualFCPICMS = prod.PercentualFCPICMS.Value;
                produto.ValorFCPICMS = prod.ValorFCPICMS.Value;
                produto.BaseFCPICMSST = prod.BCFCPICMSST.Value;
                produto.PercentualFCPICMSST = prod.PercentualFCPICMSST.Value;
                produto.ValorFCPICMSST = prod.ValorFCPICMSST.Value;
                produto.AliquotaFCPICMSST = prod.AliquotaFCPICMSST.Value;
                produto.PercentualIPIDevolvido = prod.PercentualIPIDevolvido;
                produto.ValorIPIDevolvido = prod.ValorIPIDevolvido;
                produtos.Add(produto);
            }
            return produtos;
        }

        public void SetarProdutoXMLParaProdutos(Dominio.Entidades.CFOP cfop, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> mercadorias, ref Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfe, Repositorio.UnitOfWork unitOfWork, List<int> codigosRecebimentoMercadoria)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);

            nfe.ItensNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto>();

            foreach (var prod in mercadorias)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto produto = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto();

                decimal valorUnitarioWMS = 0;
                if (prod.ProdutoInterno != null && codigosRecebimentoMercadoria != null && codigosRecebimentoMercadoria.Count > 0)
                    valorUnitarioWMS = repRecebimentoMercadoria.BuscarValorUnitario(prod.ProdutoInterno.Codigo, codigosRecebimentoMercadoria);

                decimal aliquotaCOFINS = 0, aliquotaFCP = 0, aliquotaICMS = 0, aliquotaICMSDestino = 0, aliquotaICMSInterno = 0, aliquotaICMSOperacao = 0, aliquotaICMSST = 0, aliquotaIPI = 0, aliquotaISS = 0, aliquotaPIS = 0, aliquotaInterestadual = 0;
                decimal baseII = 0, baseISS = 0, bCCOFINS = 0, bCDeducao = 0, bCICMS = 0, bCICMSDestino = 0, bCICMSST = 0, bCIPI = 0, bCPIS = 0;
                produto.AliquotaCOFINS = aliquotaCOFINS;
                produto.AliquotaFCP = aliquotaFCP;
                produto.AliquotaICMS = aliquotaICMS;
                produto.AliquotaICMSDestino = aliquotaICMSDestino;
                produto.AliquotaICMSInterno = aliquotaICMSInterno;
                produto.AliquotaICMSOperacao = aliquotaICMSOperacao;
                produto.AliquotaICMSST = aliquotaICMSST;
                produto.AliquotaICMSSTInterestadual = aliquotaInterestadual;
                produto.AliquotaIPI = aliquotaIPI;
                produto.AliquotaISS = aliquotaISS;
                produto.AliquotaPIS = aliquotaPIS;
                produto.BaseII = baseII;
                produto.BaseISS = baseISS;
                produto.BCCOFINS = bCCOFINS;
                produto.BCDeducao = bCDeducao;
                produto.BCICMS = bCICMS;
                produto.BCICMSDestino = bCICMSDestino;
                produto.BCICMSST = bCICMSST;
                produto.BCIPI = bCIPI;
                produto.BCPIS = bCPIS;

                produto.CodigoItem = prod.ProdutoInterno.CodigoProduto;
                produto.DescricaoItem = prod.ProdutoInterno.Descricao;
                produto.CFOP = cfop.Codigo;
                produto.CNPJAdquirente = "";

                produto.CSTCOFINS = cfop.CSTCOFINS;
                produto.CSTICMS = !string.IsNullOrEmpty(prod.CST) ? EnumeradorCSTICMS(prod.CST) : cfop.CSTICMS;
                produto.CSTIPI = cfop.CSTIPI;
                produto.CSTPIS = cfop.CSTPIS;
                produto.OrigemMercadoria = !string.IsNullOrEmpty(prod.Origem) ? EnumeradorOrigemMercadoria(prod.Origem) : prod.ProdutoInterno?.OrigemMercadoria;
                produto.CodigoNFCI = prod.CodigoNFCI;

                DateTime dataDesembaraco = new DateTime();
                produto.DataDesembaraco = dataDesembaraco;
                DateTime dataRegistroImportacao = new DateTime();
                produto.DataRegistroImportacao = dataRegistroImportacao;

                decimal descontoCondicional = 0, descontoIncondicional = 0;
                produto.DescontoCondicional = descontoCondicional;
                produto.DescontoIncondicional = descontoIncondicional;

                bool incentivoFiscal = false;
                produto.IncentivoFiscal = incentivoFiscal;

                produto.LocalDesembaraco = "";
                produto.NumeroDocImportacao = "";
                produto.NumeroItemOrdemCompra = "";
                produto.NumeroOrdemCompra = "";
                produto.ProcessoJudicial = "";
                produto.UFDesembaraco = "";

                if (prod.Produto != null)
                    produto.Produto = ConverterObjetoProduto(prod.ProdutoInterno);

                decimal outrasRetencoes = 0, percentualPartilha = 0, mVAICMSST = 0, quantidade = prod.Quantidade, reducaoBCCOFINS = 0, reducaoBCICMS = 0, reducaoBCICMSST = 0, reducaoBCIPI = 0, reducaoBCPIS = 0, retencaoISS = 0;

                produto.OutrasRetencoes = outrasRetencoes;
                produto.PercentualPartilha = percentualPartilha;
                produto.MVAICMSST = mVAICMSST;
                produto.Quantidade = quantidade;
                produto.ReducaoBCCOFINS = reducaoBCCOFINS;
                produto.ReducaoBCICMS = reducaoBCICMS;
                produto.ReducaoBCICMSST = reducaoBCICMSST;
                produto.ReducaoBCIPI = reducaoBCIPI;
                produto.ReducaoBCPIS = reducaoBCPIS;
                produto.RetencaoISS = retencaoISS;

                decimal valorCOFINS = 0, valorDesconto = 0, valorDespesaII = 0, valorFCP = 0, valorFrete = 0, valorFreteMarinho = 0, valorICMS = 0, valorICMSDesonerado = 0, valorICMSDestino = 0, valorICMSDiferido = 0;
                produto.ValorCOFINS = valorCOFINS;
                produto.ValorDesconto = valorDesconto;
                produto.ValorDespesaII = valorDespesaII;
                produto.ValorFCP = valorFCP;
                produto.ValorFrete = valorFrete;
                produto.ValorFreteMarinho = valorFreteMarinho;
                produto.ValorICMS = valorICMS;
                produto.ValorICMSDesonerado = valorICMSDesonerado;
                produto.ValorICMSDestino = valorICMSDestino;
                produto.ValorICMSDiferido = valorICMSDiferido;

                decimal valorICMSOperacao = 0, valorICMSRemetente = 0, valorICMSST = 0, valorII = 0, valorIOFII = 0, valorIPI = 0, valorISS = 0, valorOutrasDespesas = 0, valorPIS = 0, valorSeguro = 0, valorTotal = (prod.Quantidade * (valorUnitarioWMS > 0 ? valorUnitarioWMS : prod.ValorProduto)), valorUnitario = (valorUnitarioWMS > 0 ? valorUnitarioWMS : prod.ValorProduto);
                produto.ValorICMSOperacao = valorICMSOperacao;
                produto.ValorICMSRemetente = valorICMSRemetente;
                produto.ValorICMSST = valorICMSST;
                produto.ValorII = valorII;
                produto.ValorIOFII = valorIOFII;
                produto.ValorIPI = valorIPI;
                produto.ValorISS = valorISS;
                produto.ValorOutrasDespesas = valorOutrasDespesas;
                produto.ValorPIS = valorPIS;
                produto.ValorSeguro = valorSeguro;
                produto.ValorTotal = valorTotal;
                produto.ValorUnitario = valorUnitario;

                decimal baseFCPICMS = 0, percentualFCPICMS = 0, valorFCPICMS = 0, baseFCPICMSST = 0, percentualFCPICMSST = 0, valorFCPICMSST = 0, aliquotaFCPICMSST = 0, baseFCPDestino = 0, percentualIPIDevolvido = 0, valorIPIDevolvido = 0;
                produto.BaseFCPICMS = baseFCPICMS;
                produto.PercentualFCPICMS = percentualFCPICMS;
                produto.ValorFCPICMS = valorFCPICMS;
                produto.BaseFCPICMSST = baseFCPICMSST;
                produto.PercentualFCPICMSST = percentualFCPICMSST;
                produto.ValorFCPICMSST = valorFCPICMSST;
                produto.AliquotaFCPICMSST = aliquotaFCPICMSST;
                produto.BaseFCPDestino = baseFCPDestino;
                produto.PercentualIPIDevolvido = percentualIPIDevolvido;
                produto.ValorIPIDevolvido = valorIPIDevolvido;
                produto.InformacoesAdicionais = "";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante indicadorEscalaRelevante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.Nenhum;
                produto.IndicadorEscalaRelevante = indicadorEscalaRelevante;
                produto.CNPJFabricante = "";
                produto.CodigoBeneficioFiscal = prod.ProdutoInterno.CodigoBeneficioFiscal;
                produto.CodigoANP = prod.ProdutoInterno.CodigoANP;

                decimal percentualGLP = 0, percentualGNN = 0, percentualGNI = 0, valorPartidaANP = 0, percentualOrigemComb = 0;
                produto.PercentualGLP = percentualGLP;
                produto.PercentualGNN = percentualGNN;
                produto.PercentualGNI = percentualGNI;
                produto.PercentualOrigemComb = percentualOrigemComb;
                produto.ValorPartidaANP = valorPartidaANP;

                produto.CodigoEANTributavel = prod.ProdutoInterno.CodigoBarrasEAN;
                produto.QuantidadeTributavel = prod.Quantidade;
                produto.ValorUnitarioTributavel = (valorUnitarioWMS > 0 ? valorUnitarioWMS : prod.ValorProduto);
                produto.UnidadeDeMedidaTributavel = prod.ProdutoInterno.UnidadeDeMedida;

                decimal bCICMSSTRetido = 0, aliquotaICMSSTRetido = 0, valorICMSSTSubstituto = 0, valorICMSSTRetido = 0;
                produto.BCICMSSTRetido = bCICMSSTRetido;
                produto.AliquotaICMSSTRetido = aliquotaICMSSTRetido;
                produto.ValorICMSSTSubstituto = valorICMSSTSubstituto;
                produto.ValorICMSSTRetido = valorICMSSTRetido;

                decimal bCICMSEfetivo = 0, aliquotaICMSEfetivo = 0, reducaoBCICMSEfetivo = 0, valorICMSEfetivo = 0;
                produto.BCICMSEfetivo = bCICMSEfetivo;
                produto.AliquotaICMSEfetivo = aliquotaICMSEfetivo;
                produto.ReducaoBCICMSEfetivo = reducaoBCICMSEfetivo;
                produto.ValorICMSEfetivo = valorICMSEfetivo;

                nfe.ItensNFe.Add(produto);
            }
        }

        public void SetarRecebimentoMercadoriaParaProdutos(Dominio.Entidades.CFOP cfop, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> mercadorias, ref Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
            nfe.ItensNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto>();

            foreach (var prod in mercadorias)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto produto = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto();

                decimal aliquotaCOFINS = 0, aliquotaFCP = 0, aliquotaICMS = 0, aliquotaICMSDestino = 0, aliquotaICMSInterno = 0, aliquotaICMSOperacao = 0, aliquotaICMSST = 0, aliquotaIPI = 0, aliquotaISS = 0, aliquotaPIS = 0, aliquotaInterestadual = 0;
                decimal baseII = 0, baseISS = 0, bCCOFINS = 0, bCDeducao = 0, bCICMS = 0, bCICMSDestino = 0, bCICMSST = 0, bCIPI = 0, bCPIS = 0;
                produto.AliquotaCOFINS = aliquotaCOFINS;
                produto.AliquotaFCP = aliquotaFCP;
                produto.AliquotaICMS = aliquotaICMS;
                produto.AliquotaICMSDestino = aliquotaICMSDestino;
                produto.AliquotaICMSInterno = aliquotaICMSInterno;
                produto.AliquotaICMSOperacao = aliquotaICMSOperacao;
                produto.AliquotaICMSST = aliquotaICMSST;
                produto.AliquotaICMSSTInterestadual = aliquotaInterestadual;
                produto.AliquotaIPI = aliquotaIPI;
                produto.AliquotaISS = aliquotaISS;
                produto.AliquotaPIS = aliquotaPIS;
                produto.BaseII = baseII;
                produto.BaseISS = baseISS;
                produto.BCCOFINS = bCCOFINS;
                produto.BCDeducao = bCDeducao;
                produto.BCICMS = bCICMS;
                produto.BCICMSDestino = bCICMSDestino;
                produto.BCICMSST = bCICMSST;
                produto.BCIPI = bCIPI;
                produto.BCPIS = bCPIS;

                produto.CodigoItem = prod.CodigoBarras;
                produto.DescricaoItem = prod.Descricao;
                produto.CFOP = cfop.Codigo;
                produto.CNPJAdquirente = "";

                produto.CSTCOFINS = cfop.CSTCOFINS;
                produto.CSTICMS = cfop.CSTICMS;
                produto.CSTIPI = cfop.CSTIPI;
                produto.CSTPIS = cfop.CSTPIS;

                DateTime dataDesembaraco = new DateTime();
                produto.DataDesembaraco = dataDesembaraco;
                DateTime dataRegistroImportacao = new DateTime();
                produto.DataRegistroImportacao = dataRegistroImportacao;

                decimal descontoCondicional = 0, descontoIncondicional = 0;
                produto.DescontoCondicional = descontoCondicional;
                produto.DescontoIncondicional = descontoIncondicional;

                bool incentivoFiscal = false;
                produto.IncentivoFiscal = incentivoFiscal;

                produto.LocalDesembaraco = "";
                produto.NumeroDocImportacao = "";
                produto.NumeroItemOrdemCompra = "";
                produto.NumeroOrdemCompra = "";
                produto.ProcessoJudicial = "";
                produto.UFDesembaraco = "";

                if (prod.Produto != null)
                    produto.Produto = ConverterObjetoProduto(prod.Produto);

                decimal outrasRetencoes = 0, percentualPartilha = 0, mVAICMSST = 0, quantidade = prod.QuantidadeLote, reducaoBCCOFINS = 0, reducaoBCICMS = 0, reducaoBCICMSST = 0, reducaoBCIPI = 0, reducaoBCPIS = 0, retencaoISS = 0;

                produto.OutrasRetencoes = outrasRetencoes;
                produto.PercentualPartilha = percentualPartilha;
                produto.MVAICMSST = mVAICMSST;
                produto.Quantidade = quantidade;
                produto.ReducaoBCCOFINS = reducaoBCCOFINS;
                produto.ReducaoBCICMS = reducaoBCICMS;
                produto.ReducaoBCICMSST = reducaoBCICMSST;
                produto.ReducaoBCIPI = reducaoBCIPI;
                produto.ReducaoBCPIS = reducaoBCPIS;
                produto.RetencaoISS = retencaoISS;

                decimal valorCOFINS = 0, valorDesconto = 0, valorDespesaII = 0, valorFCP = 0, valorFrete = 0, valorFreteMarinho = 0, valorICMS = 0, valorICMSDesonerado = 0, valorICMSDestino = 0, valorICMSDiferido = 0;
                produto.ValorCOFINS = valorCOFINS;
                produto.ValorDesconto = valorDesconto;
                produto.ValorDespesaII = valorDespesaII;
                produto.ValorFCP = valorFCP;
                produto.ValorFrete = valorFrete;
                produto.ValorFreteMarinho = valorFreteMarinho;
                produto.ValorICMS = valorICMS;
                produto.ValorICMSDesonerado = valorICMSDesonerado;
                produto.ValorICMSDestino = valorICMSDestino;
                produto.ValorICMSDiferido = valorICMSDiferido;

                decimal valorICMSOperacao = 0, valorICMSRemetente = 0, valorICMSST = 0, valorII = 0, valorIOFII = 0, valorIPI = 0, valorISS = 0, valorOutrasDespesas = 0, valorPIS = 0, valorSeguro = 0, valorTotal = (prod.QuantidadeLote * prod.ValorUnitario), valorUnitario = prod.ValorUnitario;
                produto.ValorICMSOperacao = valorICMSOperacao;
                produto.ValorICMSRemetente = valorICMSRemetente;
                produto.ValorICMSST = valorICMSST;
                produto.ValorII = valorII;
                produto.ValorIOFII = valorIOFII;
                produto.ValorIPI = valorIPI;
                produto.ValorISS = valorISS;
                produto.ValorOutrasDespesas = valorOutrasDespesas;
                produto.ValorPIS = valorPIS;
                produto.ValorSeguro = valorSeguro;
                produto.ValorTotal = valorTotal;
                produto.ValorUnitario = valorUnitario;

                decimal baseFCPICMS = 0, percentualFCPICMS = 0, valorFCPICMS = 0, baseFCPICMSST = 0, percentualFCPICMSST = 0, valorFCPICMSST = 0, aliquotaFCPICMSST = 0, baseFCPDestino = 0, percentualIPIDevolvido = 0, valorIPIDevolvido = 0;
                produto.BaseFCPICMS = baseFCPICMS;
                produto.PercentualFCPICMS = percentualFCPICMS;
                produto.ValorFCPICMS = valorFCPICMS;
                produto.BaseFCPICMSST = baseFCPICMSST;
                produto.PercentualFCPICMSST = percentualFCPICMSST;
                produto.ValorFCPICMSST = valorFCPICMSST;
                produto.AliquotaFCPICMSST = aliquotaFCPICMSST;
                produto.BaseFCPDestino = baseFCPDestino;
                produto.PercentualIPIDevolvido = percentualIPIDevolvido;
                produto.ValorIPIDevolvido = valorIPIDevolvido;
                produto.InformacoesAdicionais = "";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante indicadorEscalaRelevante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.Nenhum;
                produto.IndicadorEscalaRelevante = indicadorEscalaRelevante;
                produto.CNPJFabricante = "";
                produto.CodigoBeneficioFiscal = prod.Produto.CodigoBeneficioFiscal;
                produto.CodigoANP = prod.Produto.CodigoANP;

                decimal percentualGLP = 0, percentualGNN = 0, percentualGNI = 0, valorPartidaANP = 0, percentualOrigemComb = 0;
                produto.PercentualGLP = percentualGLP;
                produto.PercentualGNN = percentualGNN;
                produto.PercentualGNI = percentualGNI;
                produto.PercentualOrigemComb = percentualOrigemComb;
                produto.ValorPartidaANP = valorPartidaANP;

                produto.CodigoEANTributavel = prod.Produto.CodigoBarrasEAN;
                produto.QuantidadeTributavel = prod.QuantidadeLote;
                produto.ValorUnitarioTributavel = prod.ValorUnitario;
                produto.UnidadeDeMedidaTributavel = prod.Produto.UnidadeDeMedida;

                decimal bCICMSSTRetido = 0, aliquotaICMSSTRetido = 0, valorICMSSTSubstituto = 0, valorICMSSTRetido = 0;
                produto.BCICMSSTRetido = bCICMSSTRetido;
                produto.AliquotaICMSSTRetido = aliquotaICMSSTRetido;
                produto.ValorICMSSTSubstituto = valorICMSSTSubstituto;
                produto.ValorICMSSTRetido = valorICMSSTRetido;

                decimal bCICMSEfetivo = 0, aliquotaICMSEfetivo = 0, reducaoBCICMSEfetivo = 0, valorICMSEfetivo = 0;
                produto.BCICMSEfetivo = bCICMSEfetivo;
                produto.AliquotaICMSEfetivo = aliquotaICMSEfetivo;
                produto.ReducaoBCICMSEfetivo = reducaoBCICMSEfetivo;
                produto.ValorICMSEfetivo = valorICMSEfetivo;

                nfe.ItensNFe.Add(produto);
            }
        }

        public void SetarDynamicParaProdutos(dynamic dynNFe, ref Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
            nfe.ItensNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto>();
            int sequencialItens = 1;

            foreach (dynamic prod in dynNFe.ProdutosServicos)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto produto = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto();

                decimal aliquotaCOFINS, aliquotaFCP, aliquotaICMS, aliquotaICMSDestino, aliquotaICMSInterno, aliquotaICMSOperacao, aliquotaICMSST, aliquotaIPI, aliquotaISS, aliquotaPIS, aliquotaInterestadual;
                decimal baseII, baseISS, bCCOFINS, bCDeducao, bCICMS, bCICMSDestino, bCICMSST, bCIPI, bCPIS, aliquotaRemICMSRet, valorICMSMonoRet;
                decimal.TryParse((string)prod.AliquotaCOFINS, out aliquotaCOFINS);
                produto.AliquotaCOFINS = aliquotaCOFINS;
                decimal.TryParse((string)prod.AliquotaFCP, out aliquotaFCP);
                produto.AliquotaFCP = aliquotaFCP;
                decimal.TryParse((string)prod.AliquotaICMS, out aliquotaICMS);
                produto.AliquotaICMS = aliquotaICMS;
                decimal.TryParse((string)prod.AliquotaICMSDestino, out aliquotaICMSDestino);
                produto.AliquotaICMSDestino = aliquotaICMSDestino;
                decimal.TryParse((string)prod.AliquotaICMSInterno, out aliquotaICMSInterno);
                produto.AliquotaICMSInterno = aliquotaICMSInterno;
                decimal.TryParse((string)prod.AliquotaICMSOperacao, out aliquotaICMSOperacao);
                produto.AliquotaICMSOperacao = aliquotaICMSOperacao;
                decimal.TryParse((string)prod.AliquotaICMSST, out aliquotaICMSST);
                produto.AliquotaICMSST = aliquotaICMSST;
                decimal.TryParse((string)prod.AliquotaInterestadual, out aliquotaInterestadual);
                produto.AliquotaICMSSTInterestadual = aliquotaInterestadual;
                decimal.TryParse((string)prod.AliquotaIPI, out aliquotaIPI);
                produto.AliquotaIPI = aliquotaIPI;
                decimal.TryParse((string)prod.AliquotaISS, out aliquotaISS);
                produto.AliquotaISS = aliquotaISS;
                decimal.TryParse((string)prod.AliquotaPIS, out aliquotaPIS);
                produto.AliquotaPIS = aliquotaPIS;
                decimal.TryParse((string)prod.BCII, out baseII);
                produto.BaseII = baseII;
                decimal.TryParse((string)prod.BCISS, out baseISS);
                produto.BaseISS = baseISS;
                decimal.TryParse((string)prod.BCCOFINS, out bCCOFINS);
                produto.BCCOFINS = bCCOFINS;
                decimal.TryParse((string)prod.BaseDeducaoISS, out bCDeducao);
                produto.BCDeducao = bCDeducao;
                decimal.TryParse((string)prod.BCICMS, out bCICMS);
                produto.BCICMS = bCICMS;
                decimal.TryParse((string)prod.BCICMSDestino, out bCICMSDestino);
                produto.BCICMSDestino = bCICMSDestino;
                decimal.TryParse((string)prod.BCICMSST, out bCICMSST);
                produto.BCICMSST = bCICMSST;
                decimal.TryParse((string)prod.BCIPI, out bCIPI);
                produto.BCIPI = bCIPI;
                decimal.TryParse((string)prod.BCPIS, out bCPIS);
                produto.BCPIS = bCPIS;
                decimal.TryParse((string)prod.AliquotaRemICMSRet, out aliquotaRemICMSRet);
                produto.AliquotaRemICMSRet = aliquotaRemICMSRet;
                decimal.TryParse((string)prod.ValorICMSMonoRet, out valorICMSMonoRet);
                produto.ValorICMSMonoRet = valorICMSMonoRet;

                produto.CodigoItem = prod.CodigoItem;
                produto.DescricaoItem = prod.DescricaoItem;
                produto.CFOP = (int)prod.CodigoCFOP;
                produto.CNPJAdquirente = prod.CNPJAdquirente;
                produto.CodigoNFCI = prod.CodigoNFCI;

                if ((int)prod.CodigoCSTCOFINS > 0)
                    produto.CSTCOFINS = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS)prod.CodigoCSTCOFINS;
                if ((int)prod.CodigoCSTICMS > 0)
                    produto.CSTICMS = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS)prod.CodigoCSTICMS;
                if ((int)prod.CodigoCSTIPI > 0)
                    produto.CSTIPI = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI)prod.CodigoCSTIPI;
                if ((int)prod.CodigoCSTPIS > 0)
                    produto.CSTPIS = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS)prod.CodigoCSTPIS;
                if ((int)prod.OrigemMercadoria > 0)
                    produto.OrigemMercadoria = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria)prod.OrigemMercadoria;

                DateTime dataDesembaraco = new DateTime();
                DateTime.TryParseExact((string)prod.DataDesembaracoII, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataDesembaraco);
                produto.DataDesembaraco = dataDesembaraco;
                DateTime dataRegistroImportacao = new DateTime();
                DateTime.TryParseExact((string)prod.DataRegistroII, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataRegistroImportacao);
                produto.DataRegistroImportacao = dataRegistroImportacao;

                decimal descontoCondicional, descontoIncondicional = 0;
                decimal.TryParse((string)prod.DescontoCondicional, out descontoCondicional);
                produto.DescontoCondicional = descontoCondicional;
                decimal.TryParse((string)prod.DescontoIncondicional, out descontoIncondicional);
                produto.DescontoIncondicional = descontoIncondicional;

                if ((int)prod.CodigoExigibilidadeISS > 0)
                    produto.ExigibilidadeISS = (Dominio.Enumeradores.ExigibilidadeISS)prod.CodigoExigibilidadeISS;
                if ((int)prod.CodigoIntermediacaoII > 0)
                    produto.IntermediacaoII = (Dominio.Enumeradores.IntermediacaoImportacao)prod.CodigoIntermediacaoII;
                if ((int)prod.CodigoMotivoDesoneracao > 0)
                    produto.MotivoDesoneracao = (Dominio.Enumeradores.MotivoDesoneracaoICMS)prod.CodigoMotivoDesoneracao;
                if ((int)prod.CodigoViaTransporteII > 0)
                    produto.ViaTransporteII = (Dominio.Enumeradores.ViaTransporteInternacional)prod.CodigoViaTransporteII;

                bool incentivoFiscal = false;
                bool.TryParse((string)prod.GeraIncentivoFiscal, out incentivoFiscal);
                produto.IncentivoFiscal = incentivoFiscal;

                produto.LocalDesembaraco = prod.LocalDesembaracoII;
                produto.NumeroDocImportacao = prod.NumeroDocumentoII;
                produto.NumeroItemOrdemCompra = prod.NumeroItemOrdemCompra;
                produto.NumeroOrdemCompra = prod.NumeroOrdemCompra;
                produto.ProcessoJudicial = prod.ProcessoJudicial;
                produto.UFDesembaraco = prod.EstadoDesembaracoII;

                if (prod.CodigoProduto != null && (int)prod.CodigoProduto > 0)
                    produto.Produto = ConverterObjetoProduto(repProduto.BuscarPorCodigo((int)prod.CodigoProduto));
                if (prod.CodigoServico != null && (int)prod.CodigoServico > 0)
                    produto.Servico = ConverterObjetoServico(repServico.BuscarPorCodigo((int)prod.CodigoServico));

                decimal outrasRetencoes, percentualPartilha, mVAICMSST, quantidade, reducaoBCCOFINS, reducaoBCICMS, reducaoBCICMSST, reducaoBCIPI, reducaoBCPIS, retencaoISS = 0;
                decimal.TryParse((string)prod.OutrasRetencoesISS, out outrasRetencoes);
                produto.OutrasRetencoes = outrasRetencoes;
                decimal.TryParse((string)prod.PercentualPartilha, out percentualPartilha);
                produto.PercentualPartilha = percentualPartilha;
                decimal.TryParse((string)prod.PercentualMVA, out mVAICMSST);
                produto.MVAICMSST = mVAICMSST;
                decimal.TryParse((string)prod.Qtd, out quantidade);
                produto.Quantidade = quantidade;
                decimal.TryParse((string)prod.ReducaoBCCOFINS, out reducaoBCCOFINS);
                produto.ReducaoBCCOFINS = reducaoBCCOFINS;
                decimal.TryParse((string)prod.ReducaoBCICMS, out reducaoBCICMS);
                produto.ReducaoBCICMS = reducaoBCICMS;
                decimal.TryParse((string)prod.ReducaoBCICMSST, out reducaoBCICMSST);
                produto.ReducaoBCICMSST = reducaoBCICMSST;
                decimal.TryParse((string)prod.ReducaoBCIPI, out reducaoBCIPI);
                produto.ReducaoBCIPI = reducaoBCIPI;
                decimal.TryParse((string)prod.ReducaoBCPIS, out reducaoBCPIS);
                produto.ReducaoBCPIS = reducaoBCPIS;
                decimal.TryParse((string)prod.RetencaoISS, out retencaoISS);
                produto.RetencaoISS = retencaoISS;

                decimal valorCOFINS, valorDesconto, valorDespesaII, valorFCP, valorFrete, valorFreteMarinho, valorICMS, valorICMSDesonerado, valorICMSDestino, valorICMSDiferido = 0;
                decimal.TryParse((string)prod.ValorCOFINS, out valorCOFINS);
                produto.ValorCOFINS = valorCOFINS;
                decimal.TryParse((string)prod.ValorDesconto, out valorDesconto);
                produto.ValorDesconto = valorDesconto;
                decimal.TryParse((string)prod.DespesaII, out valorDespesaII);
                produto.ValorDespesaII = valorDespesaII;
                decimal.TryParse((string)prod.ValorFCP, out valorFCP);
                produto.ValorFCP = valorFCP;
                decimal.TryParse((string)prod.ValorFrete, out valorFrete);
                produto.ValorFrete = valorFrete;
                decimal.TryParse((string)prod.ValorFreteMaritimoII, out valorFreteMarinho);
                produto.ValorFreteMarinho = valorFreteMarinho;
                decimal.TryParse((string)prod.ValorICMS, out valorICMS);
                produto.ValorICMS = valorICMS;
                decimal.TryParse((string)prod.ValorICMSDesonerado, out valorICMSDesonerado);
                produto.ValorICMSDesonerado = valorICMSDesonerado;
                decimal.TryParse((string)prod.ValorICMSDestino, out valorICMSDestino);
                produto.ValorICMSDestino = valorICMSDestino;
                decimal.TryParse((string)prod.ValorICMSDeferido, out valorICMSDiferido);
                produto.ValorICMSDiferido = valorICMSDiferido;

                decimal valorICMSOperacao, valorICMSRemetente, valorICMSST, valorII, valorIOFII, valorIPI, valorISS, valorOutrasDespesas, valorPIS, valorSeguro, valorTotal, valorUnitario = 0;
                decimal.TryParse((string)prod.ValorICMSOperacao, out valorICMSOperacao);
                produto.ValorICMSOperacao = valorICMSOperacao;
                decimal.TryParse((string)prod.ValorICMSRemetente, out valorICMSRemetente);
                produto.ValorICMSRemetente = valorICMSRemetente;
                decimal.TryParse((string)prod.ValorST, out valorICMSST);
                produto.ValorICMSST = valorICMSST;
                decimal.TryParse((string)prod.ValorII, out valorII);
                produto.ValorII = valorII;
                decimal.TryParse((string)prod.ValorIOFII, out valorIOFII);
                produto.ValorIOFII = valorIOFII;
                decimal.TryParse((string)prod.ValorIPI, out valorIPI);
                produto.ValorIPI = valorIPI;
                decimal.TryParse((string)prod.ValorISS, out valorISS);
                produto.ValorISS = valorISS;
                decimal.TryParse((string)prod.ValorOutras, out valorOutrasDespesas);
                produto.ValorOutrasDespesas = valorOutrasDespesas;
                decimal.TryParse((string)prod.ValorPIS, out valorPIS);
                produto.ValorPIS = valorPIS;
                decimal.TryParse((string)prod.ValorSeguro, out valorSeguro);
                produto.ValorSeguro = valorSeguro;
                decimal.TryParse((string)prod.ValorTotal, out valorTotal);
                produto.ValorTotal = valorTotal;
                decimal.TryParse((string)prod.ValorUnitario, out valorUnitario);
                produto.ValorUnitario = valorUnitario;

                decimal baseFCPICMS, percentualFCPICMS, valorFCPICMS, baseFCPICMSST, percentualFCPICMSST, valorFCPICMSST, aliquotaFCPICMSST, baseFCPDestino, percentualIPIDevolvido, valorIPIDevolvido;
                decimal.TryParse((string)prod.BaseFCPICMS, out baseFCPICMS);
                produto.BaseFCPICMS = baseFCPICMS;
                decimal.TryParse((string)prod.PercentualFCPICMS, out percentualFCPICMS);
                produto.PercentualFCPICMS = percentualFCPICMS;
                decimal.TryParse((string)prod.ValorFCPICMS, out valorFCPICMS);
                produto.ValorFCPICMS = valorFCPICMS;
                decimal.TryParse((string)prod.BaseFCPICMSST, out baseFCPICMSST);
                produto.BaseFCPICMSST = baseFCPICMSST;
                decimal.TryParse((string)prod.PercentualFCPICMSST, out percentualFCPICMSST);
                produto.PercentualFCPICMSST = percentualFCPICMSST;
                decimal.TryParse((string)prod.ValorFCPICMSST, out valorFCPICMSST);
                produto.ValorFCPICMSST = valorFCPICMSST;
                decimal.TryParse((string)prod.AliquotaFCPICMSST, out aliquotaFCPICMSST);
                produto.AliquotaFCPICMSST = aliquotaFCPICMSST;
                decimal.TryParse((string)prod.BaseFCPDestino, out baseFCPDestino);
                produto.BaseFCPDestino = baseFCPDestino;
                decimal.TryParse((string)prod.PercentualIPIDevolvido, out percentualIPIDevolvido);
                produto.PercentualIPIDevolvido = percentualIPIDevolvido;
                decimal.TryParse((string)prod.ValorIPIDevolvido, out valorIPIDevolvido);
                produto.ValorIPIDevolvido = valorIPIDevolvido;
                produto.InformacoesAdicionais = prod.InformacoesAdicionaisItem;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante indicadorEscalaRelevante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.Nenhum;
                if ((int)prod.IndicadorEscalaRelevante > 0)
                    produto.IndicadorEscalaRelevante = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante)prod.IndicadorEscalaRelevante;
                else
                    produto.IndicadorEscalaRelevante = indicadorEscalaRelevante;
                produto.CNPJFabricante = prod.CNPJFabricante;
                produto.CodigoBeneficioFiscal = prod.CodigoBeneficioFiscal;
                produto.LocalArmazenamento = prod.CodigoLocalArmazenamento != null ? (int)prod.CodigoLocalArmazenamento : 0;

                produto.CodigoANP = prod.CodigoANP;
                decimal percentualGLP, percentualGNN, percentualGNI, valorPartidaANP, percentualOrigemComb, percentualMisturaBiodiesel;
                decimal.TryParse((string)prod.PercentualGLP, out percentualGLP);
                produto.PercentualGLP = percentualGLP;
                decimal.TryParse((string)prod.PercentualGNN, out percentualGNN);
                produto.PercentualGNN = percentualGNN;
                decimal.TryParse((string)prod.PercentualGNI, out percentualGNI);
                produto.PercentualGNI = percentualGNI;
                decimal.TryParse((string)prod.PercentualOrigemComb, out percentualOrigemComb);
                produto.PercentualOrigemComb = percentualOrigemComb;
                decimal.TryParse((string)prod.PercentualMisturaBiodiesel, out percentualMisturaBiodiesel);
                produto.PercentualMisturaBiodiesel = percentualMisturaBiodiesel;
                decimal.TryParse((string)prod.ValorPartidaANP, out valorPartidaANP);
                produto.ValorPartidaANP = valorPartidaANP;

                produto.CodigoEANTributavel = prod.CodigoEANTributavel;
                decimal.TryParse((string)prod.QuantidadeTributavel, out decimal quantidadeTributavel);
                produto.QuantidadeTributavel = quantidadeTributavel;
                decimal.TryParse((string)prod.ValorUnitarioTributavel, out decimal valorUnitarioTributavel);
                produto.ValorUnitarioTributavel = valorUnitarioTributavel;
                if (prod.UnidadeDeMedidaTributavel != null && !string.IsNullOrWhiteSpace((string)prod.UnidadeDeMedidaTributavel) && (int)prod.UnidadeDeMedidaTributavel > 0)
                    produto.UnidadeDeMedidaTributavel = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)prod.UnidadeDeMedidaTributavel;
                else
                    produto.UnidadeDeMedidaTributavel = null;

                decimal.TryParse((string)prod.BCICMSSTRetido, out decimal bCICMSSTRetido);
                decimal.TryParse((string)prod.AliquotaICMSSTRetido, out decimal aliquotaICMSSTRetido);
                decimal.TryParse((string)prod.ValorICMSSTSubstituto, out decimal valorICMSSTSubstituto);
                decimal.TryParse((string)prod.ValorICMSSTRetido, out decimal valorICMSSTRetido);
                produto.BCICMSSTRetido = bCICMSSTRetido;
                produto.AliquotaICMSSTRetido = aliquotaICMSSTRetido;
                produto.ValorICMSSTSubstituto = valorICMSSTSubstituto;
                produto.ValorICMSSTRetido = valorICMSSTRetido;

                decimal.TryParse((string)prod.BCICMSEfetivo, out decimal bCICMSEfetivo);
                decimal.TryParse((string)prod.AliquotaICMSEfetivo, out decimal aliquotaICMSEfetivo);
                decimal.TryParse((string)prod.ReducaoBCICMSEfetivo, out decimal reducaoBCICMSEfetivo);
                decimal.TryParse((string)prod.ValorICMSEfetivo, out decimal valorICMSEfetivo);
                produto.BCICMSEfetivo = bCICMSEfetivo;
                produto.AliquotaICMSEfetivo = aliquotaICMSEfetivo;
                produto.ReducaoBCICMSEfetivo = reducaoBCICMSEfetivo;
                produto.ValorICMSEfetivo = valorICMSEfetivo;

                produto.NumeroDrawback = prod.NumeroDrawback;
                produto.NumeroRegistroExportacao = prod.NumeroRegistroExportacao;
                produto.ChaveAcessoExportacao = prod.ChaveAcessoExportacao;

                decimal.TryParse((string)prod.BCICMSSTDestino, out decimal bcICMSSTDestino);
                decimal.TryParse((string)prod.ValorICMSSTDestino, out decimal valorICMSSTDestino);
                produto.BCICMSSTDestino = bcICMSSTDestino;
                produto.ValorICMSSTDestino = valorICMSSTDestino;

                produto.Sequencial = sequencialItens++;

                produto.LotesItem = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProdutoLote>();
                foreach (dynamic lot in dynNFe.LotesProdutos)
                {
                    if ((string)lot.CodigoItem == (string)prod.Codigo)
                    {
                        Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProdutoLote lote = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProdutoLote();
                        lote.NumeroLote = lot.NumeroLote;
                        lote.CodigoAgregacao = lot.CodigoAgregacao;

                        decimal quantidadeLote;
                        decimal.TryParse((string)lot.QuantidadeLote, out quantidadeLote);
                        lote.QuantidadeLote = quantidadeLote;
                        DateTime dataFabricacao = new DateTime();
                        DateTime.TryParseExact((string)lot.DataFabricacao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFabricacao);
                        lote.DataFabricacao = dataFabricacao;
                        DateTime dataValidade = new DateTime();
                        DateTime.TryParseExact((string)lot.DataValidade, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataValidade);
                        lote.DataValidade = dataValidade;

                        produto.LotesItem.Add(lote);
                    }
                }

                nfe.ItensNFe.Add(produto);
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Servico ConverterObjetoServico(Dominio.Entidades.Embarcador.NotaFiscal.Servico servico)
        {
            if (servico != null)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Servico servicoIntegracao = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Servico();
                servicoIntegracao.Codigo = servico.Codigo;
                servicoIntegracao.Descricao = servico.Descricao;

                return servicoIntegracao;
            }
            else
            {
                return null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto ConverterObjetoProduto(Dominio.Entidades.Produto produto)
        {
            if (produto != null)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto produtoIntegracao = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto();
                produtoIntegracao.Codigo = produto.Codigo;
                produtoIntegracao.Descricao = produto.Descricao;

                return produtoIntegracao;
            }
            else
            {
                return null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria? EnumeradorOrigemMercadoria(string cst)
        {
            switch (cst)
            {
                case "0":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;
                case "1":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem1;
                case "2":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem2;
                case "3":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem3;
                case "4":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem4;
                case "5":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem5;
                case "6":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem6;
                case "7":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem7;
                case "8":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem8;
                case "":
                    return 0;
                case " ":
                    return 0;
                default:
                    return null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? EnumeradorCSTICMS(string cst)
        {
            switch (cst)
            {
                case "101":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101;
                case "102":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102;
                case "103":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103;
                case "201":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201;
                case "202":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202;
                case "203":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203;
                case "300":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300;
                case "400":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400;
                case "500":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500;
                case "900":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900;
                case "00":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00;
                case "10":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10;
                case "20":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20;
                case "30":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30;
                case "40":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40;
                case "41":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41;
                case "50":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50;
                case "51":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51;
                case "60":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60;
                case "70":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70;
                case "90":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90;
                case "":
                    return 0;
                case " ":
                    return 0;
                default:
                    return null;
            }
        }

        #endregion
    }
}
