using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class DocumentoEntrada : ServicoBase
    {
        public DocumentoEntrada(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.DocumentoEntrada ObterDetalhesPorNFe(Dominio.Entidades.Empresa empresa, object notaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (notaFiscal.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                return this.ObterDetalhesPorNFe(empresa, (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)notaFiscal, unitOfWork);
            else if (notaFiscal.GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                return this.ObterDetalhesPorNFe(empresa, (MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)notaFiscal, unitOfWork);
            if (notaFiscal.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                return this.ObterDetalhesPorNFe(empresa, (MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)notaFiscal, unitOfWork);
            else
                return null;
        }

        private Dominio.ObjetosDeValor.DocumentoEntrada ObterDetalhesPorNFe(Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("55");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("nfe");

            Servicos.NFe svcNFe = new NFe(unitOfWork);

            Dominio.Entidades.Cliente emitente = svcNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa.Codigo);

            Dominio.ObjetosDeValor.DocumentoEntrada documento = new Dominio.ObjetosDeValor.DocumentoEntrada();
            documento.Cobrancas = new List<Dominio.ObjetosDeValor.ParcelaDocumentoEntrada>();
            documento.Itens = new List<Dominio.ObjetosDeValor.ItemDocumentoEntrada>();

            documento.BaseCalculoICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBC, cultura).ToString("n2");
            documento.BaseCalculoICMSST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBCST, cultura).ToString("n2");
            documento.Chave = nfe.protNFe != null ? nfe.protNFe.infProt.chNFe : string.Empty;
            documento.CPFCNPJFornecedor = emitente.CPF_CNPJ_Formatado;
            documento.DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi.Split('T')[0], "yyyy-MM-dd", null).ToString("dd/MM/yyyy");
            documento.DataEntrada = DateTime.Now.ToString("dd/MM/yyyy");
            documento.IndicadorPagamento = (Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada)nfe.NFe.infNFe.ide.indPag;
            documento.NomeFornecedor = emitente.Nome;
            documento.Numero = int.Parse(nfe.NFe.infNFe.ide.nNF);
            documento.Serie = nfe.NFe.infNFe.ide.serie;
            documento.Status = Dominio.Enumeradores.StatusDocumentoEntrada.Aberto;
            documento.ValorProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura).ToString("n2");
            documento.ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n2");
            documento.ValorTotalCOFINS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vCOFINS, cultura).ToString("n2");
            documento.ValorTotalDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura).ToString("n2");
            documento.ValorTotalFrete = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vFrete, cultura).ToString("n2");
            documento.ValorTotalICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vICMS, cultura).ToString("n2");
            documento.ValorTotalICMSST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vST, cultura).ToString("n2");
            documento.ValorTotalIPI = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vIPI, cultura).ToString("n2");
            documento.ValorTotalOutrasDespesas = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura).ToString("n2");
            documento.ValorTotalPIS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vPIS, cultura).ToString("n2");
            documento.SiglaEspecie = especie.Sigla;
            documento.CodigoModelo = modelo.Codigo;

            int index = 1;
            List<object> itens = new List<object>();
            List<object> duplicatas = new List<object>();

            foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDet det in nfe.NFe.infNFe.det)
            {
                object icms = this.ObterICMS(det.imposto);

                decimal baseCalculoICMS = icms != null && icms.GetType().GetProperty("BaseCalculoICMS") != null ? (decimal)icms.GetType().GetProperty("BaseCalculoICMS").GetValue(icms, null) : 0m;
                decimal aliquotaICMS = icms != null && icms.GetType().GetProperty("AliquotaICMS") != null ? (decimal)icms.GetType().GetProperty("AliquotaICMS").GetValue(icms, null) : 0m;
                decimal valorICMS = icms != null && icms.GetType().GetProperty("ValorICMS") != null ? (decimal)icms.GetType().GetProperty("ValorICMS").GetValue(icms, null) : 0m;
                string cst = icms != null && icms.GetType().GetProperty("CST") != null ? int.Parse((string)icms.GetType().GetProperty("CST").GetValue(icms, null)).ToString("D3") : "";
                decimal baseCalculoICMSST = icms != null && icms.GetType().GetProperty("BaseCalculoICMSST") != null ? (decimal)icms.GetType().GetProperty("BaseCalculoICMSST").GetValue(icms, null) : 0m;
                decimal valorICMSST = icms != null && icms.GetType().GetProperty("ValorICMSST") != null ? (decimal)icms.GetType().GetProperty("ValorICMSST").GetValue(icms, null) : 0m;

                object ipi = this.ObterIPI(det.imposto);

                string cstIPI = ipi != null && ipi.GetType().GetProperty("CST") != null ? (string)ipi.GetType().GetProperty("CST").GetValue(ipi, null) : "";
                decimal baseCalculoIPI = ipi != null && ipi.GetType().GetProperty("BaseCalculoIPI") != null ? (decimal)ipi.GetType().GetProperty("BaseCalculoIPI").GetValue(ipi, null) : 0m;
                decimal aliquotaIPI = ipi != null && ipi.GetType().GetProperty("AliquotaIPI") != null ? (decimal)ipi.GetType().GetProperty("AliquotaIPI").GetValue(ipi, null) : 0m;
                decimal valorIPI = ipi != null && ipi.GetType().GetProperty("ValorIPI") != null ? (decimal)ipi.GetType().GetProperty("ValorIPI").GetValue(ipi, null) : 0m;

                object pis = this.ObterPIS(det.imposto.PIS);

                string cstPIS = pis != null && pis.GetType().GetProperty("CST") != null ? (string)pis.GetType().GetProperty("CST").GetValue(pis, null) : "";
                decimal valorPIS = pis != null && pis.GetType().GetProperty("ValorPIS") != null ? (decimal)pis.GetType().GetProperty("ValorPIS").GetValue(pis, null) : 0m;

                object cofins = this.ObterCOFINS(det.imposto.COFINS);

                string cstCOFINS = cofins != null && cofins.GetType().GetProperty("CST") != null ? (string)cofins.GetType().GetProperty("CST").GetValue(cofins, null) : "";
                decimal valorCOFINS = cofins != null && cofins.GetType().GetProperty("ValorCOFINS") != null ? (decimal)cofins.GetType().GetProperty("ValorCOFINS").GetValue(cofins, null) : 0m;

                object produto = this.ObterProduto(empresa, emitente, det.prod, unitOfWork, ncmsAbastecimento);

                int codigoProduto = produto != null && produto.GetType().GetProperty("CodigoProduto") != null ? (int)produto.GetType().GetProperty("CodigoProduto").GetValue(produto, null) : 0;
                string descricaoProduto = produto != null && produto.GetType().GetProperty("DescricaoProduto") != null ? (string)produto.GetType().GetProperty("DescricaoProduto").GetValue(produto, null) : "";
                int codigoUnidadeMedida = produto != null && produto.GetType().GetProperty("CodigoUnidadeMedida") != null ? (int)produto.GetType().GetProperty("CodigoUnidadeMedida").GetValue(produto, null) : 0;
                string descricaoUnidadeMedida = produto != null && produto.GetType().GetProperty("DescricaoUnidadeMedida") != null ? (string)produto.GetType().GetProperty("DescricaoUnidadeMedida").GetValue(produto, null) : "";
                int cfop = produto != null && produto.GetType().GetProperty("CFOP") != null ? (int)produto.GetType().GetProperty("CFOP").GetValue(produto, null) : 0;
                string descricaoCFOP = produto != null && produto.GetType().GetProperty("DescricaoCFOP") != null ? (string)produto.GetType().GetProperty("DescricaoCFOP").GetValue(produto, null) : "";

                dynamic item = new
                {
                    AliquotaICMS = aliquotaICMS,
                    AliquotaIPI = aliquotaIPI,
                    BaseCalculoICMS = baseCalculoICMS,
                    BaseCalculoICMSST = baseCalculoICMSST,
                    BaseCalculoIPI = baseCalculoIPI,
                    Codigo = -index,
                    CodigoCFOP = cfop,
                    CodigoProduto = codigoProduto,
                    CodigoProdutoFornecedor = det.prod.cProd,
                    CodigoUnidadeMedida = codigoUnidadeMedida,
                    CST = cst,
                    CSTIPI = cstIPI,
                    CSTPIS = cstPIS,
                    CSTCOFINS = cstCOFINS,
                    Desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m,
                    DescricaoCFOP = descricaoCFOP,
                    DescricaoProduto = descricaoProduto,
                    DescricaoUnidadeMedida = descricaoUnidadeMedida,
                    Excluir = false,
                    Quantidade = det.prod.qCom != null ? decimal.Parse(det.prod.qCom, cultura) : 0m,
                    Sequencial = int.Parse(det.nItem),
                    ValorCOFINS = valorCOFINS,
                    ValorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m,
                    ValorICMS = valorICMS,
                    ValorICMSST = valorICMSST,
                    ValorIPI = valorIPI,
                    ValorOutrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m,
                    ValorPIS = valorPIS,
                    ValorTotal = det.prod.vProd != null ? decimal.Parse(det.prod.vProd, cultura) : 0m,
                    ValorUnitario = det.prod.vUnCom != null ? decimal.Parse(det.prod.vUnCom, cultura) : 0m
                };

                itens.Add(item);

                index++;
            }

            documento.Itens = itens;

            index = 1;

            if (nfe.NFe.infNFe.cobr != null && nfe.NFe.infNFe.cobr.dup != null)
            {
                foreach (var obj in nfe.NFe.infNFe.cobr.dup)
                {
                    duplicatas.Add(new
                    {
                        Codigo = -index,
                        DataVencimento = obj.dVenc != null ? DateTime.ParseExact(obj.dVenc, "yyyy-MM-dd", null).ToString("dd/MM/yyyy") : string.Empty,
                        Excluir = false,
                        Numero = obj.nDup,
                        Valor = decimal.Parse(obj.vDup, cultura)
                    });

                    index++;
                }
            }

            documento.Cobrancas = duplicatas;

            return documento;
        }

        private Dominio.ObjetosDeValor.DocumentoEntrada ObterDetalhesPorNFe(Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unitOfWork);

            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("55");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("nfe");

            Servicos.NFe svcNFe = new NFe(unitOfWork);

            Dominio.Entidades.Cliente emitente = svcNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa.Codigo);

            Dominio.ObjetosDeValor.DocumentoEntrada documento = new Dominio.ObjetosDeValor.DocumentoEntrada();
            documento.Cobrancas = new List<Dominio.ObjetosDeValor.ParcelaDocumentoEntrada>();
            documento.Itens = new List<Dominio.ObjetosDeValor.ItemDocumentoEntrada>();

            documento.BaseCalculoICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBC, cultura).ToString("n2");
            documento.BaseCalculoICMSST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBCST, cultura).ToString("n2");
            documento.Chave = nfe.protNFe.infProt.chNFe;
            documento.CPFCNPJFornecedor = emitente.CPF_CNPJ_Formatado;
            documento.DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dEmi, "yyyy-MM-dd", null).ToString("dd/MM/yyyy");
            documento.DataEntrada = DateTime.Now.ToString("dd/MM/yyyy");
            documento.IndicadorPagamento = (Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada)nfe.NFe.infNFe.ide.indPag;
            documento.NomeFornecedor = emitente.Nome;
            documento.Numero = int.Parse(nfe.NFe.infNFe.ide.nNF);
            documento.Serie = nfe.NFe.infNFe.ide.serie;
            documento.Status = Dominio.Enumeradores.StatusDocumentoEntrada.Aberto;
            documento.ValorProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura).ToString("n2");
            documento.ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n2");
            documento.ValorTotalCOFINS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vCOFINS, cultura).ToString("n2");
            documento.ValorTotalDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura).ToString("n2");
            documento.ValorTotalFrete = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vFrete, cultura).ToString("n2");
            documento.ValorTotalICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vICMS, cultura).ToString("n2");
            documento.ValorTotalICMSST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vST, cultura).ToString("n2");
            documento.ValorTotalIPI = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vIPI, cultura).ToString("n2");
            documento.ValorTotalOutrasDespesas = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura).ToString("n2");
            documento.ValorTotalPIS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vPIS, cultura).ToString("n2");
            documento.SiglaEspecie = especie.Sigla;
            documento.CodigoModelo = modelo.Codigo;

            int index = 1;
            List<object> itens = new List<object>();
            List<object> duplicatas = new List<object>();

            foreach (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDet det in nfe.NFe.infNFe.det)
            {
                object icms = this.ObterICMS(det.imposto);

                decimal baseCalculoICMS = icms != null && icms.GetType().GetProperty("BaseCalculoICMS") != null ? (decimal)icms.GetType().GetProperty("BaseCalculoICMS").GetValue(icms, null) : 0m;
                decimal aliquotaICMS = icms != null && icms.GetType().GetProperty("AliquotaICMS") != null ? (decimal)icms.GetType().GetProperty("AliquotaICMS").GetValue(icms, null) : 0m;
                decimal valorICMS = icms != null && icms.GetType().GetProperty("ValorICMS") != null ? (decimal)icms.GetType().GetProperty("ValorICMS").GetValue(icms, null) : 0m;
                string cst = icms != null && icms.GetType().GetProperty("CST") != null ? int.Parse((string)icms.GetType().GetProperty("CST").GetValue(icms, null)).ToString("D3") : "";
                decimal baseCalculoICMSST = icms != null && icms.GetType().GetProperty("BaseCalculoICMSST") != null ? (decimal)icms.GetType().GetProperty("BaseCalculoICMSST").GetValue(icms, null) : 0m;
                decimal valorICMSST = icms != null && icms.GetType().GetProperty("ValorICMSST") != null ? (decimal)icms.GetType().GetProperty("ValorICMSST").GetValue(icms, null) : 0m;

                object ipi = this.ObterIPI(det.imposto);

                string cstIPI = ipi != null && ipi.GetType().GetProperty("CST") != null ? (string)ipi.GetType().GetProperty("CST").GetValue(ipi, null) : "";
                decimal baseCalculoIPI = ipi != null && ipi.GetType().GetProperty("BaseCalculoIPI") != null ? (decimal)ipi.GetType().GetProperty("BaseCalculoIPI").GetValue(ipi, null) : 0m;
                decimal aliquotaIPI = ipi != null && ipi.GetType().GetProperty("AliquotaIPI") != null ? (decimal)ipi.GetType().GetProperty("AliquotaIPI").GetValue(ipi, null) : 0m;
                decimal valorIPI = ipi != null && ipi.GetType().GetProperty("ValorIPI") != null ? (decimal)ipi.GetType().GetProperty("ValorIPI").GetValue(ipi, null) : 0m;

                object pis = this.ObterPIS(det.imposto.PIS);

                string cstPIS = pis != null && pis.GetType().GetProperty("CST") != null ? (string)pis.GetType().GetProperty("CST").GetValue(pis, null) : "";
                decimal valorPIS = pis != null && pis.GetType().GetProperty("ValorPIS") != null ? (decimal)pis.GetType().GetProperty("ValorPIS").GetValue(pis, null) : 0m;

                object cofins = this.ObterCOFINS(det.imposto.COFINS);

                string cstCOFINS = cofins != null && cofins.GetType().GetProperty("CST") != null ? (string)cofins.GetType().GetProperty("CST").GetValue(cofins, null) : "";
                decimal valorCOFINS = cofins != null && cofins.GetType().GetProperty("ValorCOFINS") != null ? (decimal)cofins.GetType().GetProperty("ValorCOFINS").GetValue(cofins, null) : 0m;

                object produto = this.ObterProduto(empresa, emitente, det.prod, unitOfWork);

                int codigoProduto = produto != null && produto.GetType().GetProperty("CodigoProduto") != null ? (int)produto.GetType().GetProperty("CodigoProduto").GetValue(produto, null) : 0;
                string descricaoProduto = produto != null && produto.GetType().GetProperty("DescricaoProduto") != null ? (string)produto.GetType().GetProperty("DescricaoProduto").GetValue(produto, null) : "";
                int codigoUnidadeMedida = produto != null && produto.GetType().GetProperty("CodigoUnidadeMedida") != null ? (int)produto.GetType().GetProperty("CodigoUnidadeMedida").GetValue(produto, null) : 0;
                string descricaoUnidadeMedida = produto != null && produto.GetType().GetProperty("DescricaoUnidadeMedida") != null ? (string)produto.GetType().GetProperty("DescricaoUnidadeMedida").GetValue(produto, null) : "";

                dynamic item = new
                {
                    AliquotaICMS = aliquotaICMS,
                    AliquotaIPI = aliquotaIPI,
                    BaseCalculoICMS = baseCalculoICMS,
                    BaseCalculoICMSST = baseCalculoICMSST,
                    BaseCalculoIPI = baseCalculoIPI,
                    Codigo = -index,
                    CodigoCFOP = 0,
                    CodigoProduto = codigoProduto,
                    CodigoProdutoFornecedor = det.prod.cProd,
                    CodigoUnidadeMedida = codigoUnidadeMedida,
                    CST = cst,
                    CSTIPI = cstIPI,
                    CSTPIS = cstPIS,
                    CSTCOFINS = cstCOFINS,
                    Desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m,
                    DescricaoCFOP = "",
                    DescricaoProduto = descricaoProduto,
                    DescricaoUnidadeMedida = descricaoUnidadeMedida,
                    Excluir = false,
                    Quantidade = det.prod.qCom != null ? decimal.Parse(det.prod.qCom, cultura) : 0m,
                    Sequencial = int.Parse(det.nItem),
                    ValorCOFINS = valorCOFINS,
                    ValorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m,
                    ValorICMS = valorICMS,
                    ValorICMSST = valorICMSST,
                    ValorIPI = valorIPI,
                    ValorOutrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m,
                    ValorPIS = valorPIS,
                    ValorTotal = det.prod.vProd != null ? decimal.Parse(det.prod.vProd, cultura) : 0m,
                    ValorUnitario = det.prod.vUnCom != null ? decimal.Parse(det.prod.vUnCom, cultura) : 0m
                };

                itens.Add(item);

                index++;
            }

            documento.Itens = itens;

            index = 1;

            if (nfe.NFe.infNFe.cobr != null && nfe.NFe.infNFe.cobr.dup != null)
            {
                foreach (var obj in nfe.NFe.infNFe.cobr.dup)
                {
                    duplicatas.Add(new
                    {
                        Codigo = -index,
                        DataVencimento = obj.dVenc != null ? DateTime.ParseExact(obj.dVenc, "yyyy-MM-dd", null).ToString("dd/MM/yyyy") : string.Empty,
                        Excluir = false,
                        Numero = obj.nDup,
                        Valor = decimal.Parse(obj.vDup, cultura)
                    });

                    index++;
                }
            }

            documento.Cobrancas = duplicatas;

            return documento;
        }

        private Dominio.ObjetosDeValor.DocumentoEntrada ObterDetalhesPorNFe(Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("55");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("nfe");

            Servicos.NFe svcNFe = new NFe(unitOfWork);

            Dominio.Entidades.Cliente emitente = svcNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa.Codigo);

            Dominio.ObjetosDeValor.DocumentoEntrada documento = new Dominio.ObjetosDeValor.DocumentoEntrada();
            documento.Cobrancas = new List<Dominio.ObjetosDeValor.ParcelaDocumentoEntrada>();
            documento.Itens = new List<Dominio.ObjetosDeValor.ItemDocumentoEntrada>();

            documento.BaseCalculoICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBC, cultura).ToString("n2");
            documento.BaseCalculoICMSST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBCST, cultura).ToString("n2");
            documento.Chave = nfe.protNFe != null ? nfe.protNFe.infProt.chNFe : string.Empty;
            documento.CPFCNPJFornecedor = emitente.CPF_CNPJ_Formatado;
            documento.DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi.Split('T')[0], "yyyy-MM-dd", null).ToString("dd/MM/yyyy");
            documento.DataEntrada = DateTime.Now.ToString("dd/MM/yyyy");
            documento.IndicadorPagamento = Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada.AVista;//(Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada)nfe.NFe.infNFe.ide.indPag;
            documento.NomeFornecedor = emitente.Nome;
            documento.Numero = int.Parse(nfe.NFe.infNFe.ide.nNF);
            documento.Serie = nfe.NFe.infNFe.ide.serie;
            documento.Status = Dominio.Enumeradores.StatusDocumentoEntrada.Aberto;
            documento.ValorProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura).ToString("n2");
            documento.ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n2");
            documento.ValorTotalCOFINS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vCOFINS, cultura).ToString("n2");
            documento.ValorTotalDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura).ToString("n2");
            documento.ValorTotalFrete = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vFrete, cultura).ToString("n2");
            documento.ValorTotalICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vICMS, cultura).ToString("n2");
            documento.ValorTotalICMSST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vST, cultura).ToString("n2");
            documento.ValorTotalIPI = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vIPI, cultura).ToString("n2");
            documento.ValorTotalOutrasDespesas = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura).ToString("n2");
            documento.ValorTotalPIS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vPIS, cultura).ToString("n2");
            documento.SiglaEspecie = especie.Sigla;
            documento.CodigoModelo = modelo.Codigo;

            int index = 1;
            List<object> itens = new List<object>();
            List<object> duplicatas = new List<object>();

            foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDet det in nfe.NFe.infNFe.det)
            {
                object icms = this.ObterICMS(det.imposto);

                decimal baseCalculoICMS = icms != null && icms.GetType().GetProperty("BaseCalculoICMS") != null ? (decimal)icms.GetType().GetProperty("BaseCalculoICMS").GetValue(icms, null) : 0m;
                decimal aliquotaICMS = icms != null && icms.GetType().GetProperty("AliquotaICMS") != null ? (decimal)icms.GetType().GetProperty("AliquotaICMS").GetValue(icms, null) : 0m;
                decimal valorICMS = icms != null && icms.GetType().GetProperty("ValorICMS") != null ? (decimal)icms.GetType().GetProperty("ValorICMS").GetValue(icms, null) : 0m;
                string cst = icms != null && icms.GetType().GetProperty("CST") != null ? int.Parse((string)icms.GetType().GetProperty("CST").GetValue(icms, null)).ToString("D3") : "";
                decimal baseCalculoICMSST = icms != null && icms.GetType().GetProperty("BaseCalculoICMSST") != null ? (decimal)icms.GetType().GetProperty("BaseCalculoICMSST").GetValue(icms, null) : 0m;
                decimal valorICMSST = icms != null && icms.GetType().GetProperty("ValorICMSST") != null ? (decimal)icms.GetType().GetProperty("ValorICMSST").GetValue(icms, null) : 0m;

                object ipi = this.ObterIPI(det.imposto);

                string cstIPI = ipi != null && ipi.GetType().GetProperty("CST") != null ? (string)ipi.GetType().GetProperty("CST").GetValue(ipi, null) : "";
                decimal baseCalculoIPI = ipi != null && ipi.GetType().GetProperty("BaseCalculoIPI") != null ? (decimal)ipi.GetType().GetProperty("BaseCalculoIPI").GetValue(ipi, null) : 0m;
                decimal aliquotaIPI = ipi != null && ipi.GetType().GetProperty("AliquotaIPI") != null ? (decimal)ipi.GetType().GetProperty("AliquotaIPI").GetValue(ipi, null) : 0m;
                decimal valorIPI = ipi != null && ipi.GetType().GetProperty("ValorIPI") != null ? (decimal)ipi.GetType().GetProperty("ValorIPI").GetValue(ipi, null) : 0m;

                object pis = this.ObterPIS(det.imposto.PIS);

                string cstPIS = pis != null && pis.GetType().GetProperty("CST") != null ? (string)pis.GetType().GetProperty("CST").GetValue(pis, null) : "";
                decimal valorPIS = pis != null && pis.GetType().GetProperty("ValorPIS") != null ? (decimal)pis.GetType().GetProperty("ValorPIS").GetValue(pis, null) : 0m;

                object cofins = this.ObterCOFINS(det.imposto.COFINS);

                string cstCOFINS = cofins != null && cofins.GetType().GetProperty("CST") != null ? (string)cofins.GetType().GetProperty("CST").GetValue(cofins, null) : "";
                decimal valorCOFINS = cofins != null && cofins.GetType().GetProperty("ValorCOFINS") != null ? (decimal)cofins.GetType().GetProperty("ValorCOFINS").GetValue(cofins, null) : 0m;

                object produto = this.ObterProduto(empresa, emitente, det.prod, unitOfWork, ncmsAbastecimento);

                int codigoProduto = produto != null && produto.GetType().GetProperty("CodigoProduto") != null ? (int)produto.GetType().GetProperty("CodigoProduto").GetValue(produto, null) : 0;
                string descricaoProduto = produto != null && produto.GetType().GetProperty("DescricaoProduto") != null ? (string)produto.GetType().GetProperty("DescricaoProduto").GetValue(produto, null) : "";
                int codigoUnidadeMedida = produto != null && produto.GetType().GetProperty("CodigoUnidadeMedida") != null ? (int)produto.GetType().GetProperty("CodigoUnidadeMedida").GetValue(produto, null) : 0;
                string descricaoUnidadeMedida = produto != null && produto.GetType().GetProperty("DescricaoUnidadeMedida") != null ? (string)produto.GetType().GetProperty("DescricaoUnidadeMedida").GetValue(produto, null) : "";
                int cfop = produto != null && produto.GetType().GetProperty("CFOP") != null ? (int)produto.GetType().GetProperty("CFOP").GetValue(produto, null) : 0;
                string descricaoCFOP = produto != null && produto.GetType().GetProperty("DescricaoCFOP") != null ? (string)produto.GetType().GetProperty("DescricaoCFOP").GetValue(produto, null) : "";

                dynamic item = new
                {
                    AliquotaICMS = aliquotaICMS,
                    AliquotaIPI = aliquotaIPI,
                    BaseCalculoICMS = baseCalculoICMS,
                    BaseCalculoICMSST = baseCalculoICMSST,
                    BaseCalculoIPI = baseCalculoIPI,
                    Codigo = -index,
                    CodigoCFOP = cfop,
                    CodigoProduto = codigoProduto,
                    CodigoProdutoFornecedor = det.prod.cProd,
                    CodigoUnidadeMedida = codigoUnidadeMedida,
                    CST = cst,
                    CSTIPI = cstIPI,
                    CSTPIS = cstPIS,
                    CSTCOFINS = cstCOFINS,
                    Desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m,
                    DescricaoCFOP = descricaoCFOP,
                    DescricaoProduto = descricaoProduto,
                    DescricaoUnidadeMedida = descricaoUnidadeMedida,
                    Excluir = false,
                    Quantidade = det.prod.qCom != null ? decimal.Parse(det.prod.qCom, cultura) : 0m,
                    Sequencial = int.Parse(det.nItem),
                    ValorCOFINS = valorCOFINS,
                    ValorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m,
                    ValorICMS = valorICMS,
                    ValorICMSST = valorICMSST,
                    ValorIPI = valorIPI,
                    ValorOutrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m,
                    ValorPIS = valorPIS,
                    ValorTotal = det.prod.vProd != null ? decimal.Parse(det.prod.vProd, cultura) : 0m,
                    ValorUnitario = det.prod.vUnCom != null ? decimal.Parse(det.prod.vUnCom, cultura) : 0m
                };

                itens.Add(item);

                index++;
            }

            documento.Itens = itens;

            index = 1;

            if (nfe.NFe.infNFe.cobr != null && nfe.NFe.infNFe.cobr.dup != null)
            {
                foreach (var obj in nfe.NFe.infNFe.cobr.dup)
                {
                    duplicatas.Add(new
                    {
                        Codigo = -index,
                        DataVencimento = obj.dVenc != null ? DateTime.ParseExact(obj.dVenc, "yyyy-MM-dd", null).ToString("dd/MM/yyyy") : string.Empty,
                        Excluir = false,
                        Numero = obj.nDup,
                        Valor = decimal.Parse(obj.vDup, cultura)
                    });

                    index++;
                }
            }

            documento.Cobrancas = duplicatas;

            return documento;
        }

        private object ObterICMS(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var icms = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMS) select (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMS)obj).FirstOrDefault();

                if (icms != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoICMS = icms.Item.GetType();

                    if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00 impICMS00 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS00.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS00.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS00.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS00.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10 impICMS10 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS10.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS10.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS10.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS10.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS10.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS10.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20 impICMS20 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS20.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS20.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS20.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS20.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30 impICMS30 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMS30.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS30.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS30.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40 impICMS40 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS40.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51 impICMS51 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS51.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS51.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS51.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS51.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60 impICMS60 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS60.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70 impICMS70 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS70.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS70.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS70.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS70.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS70.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS70.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90 impICMS90 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS90.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS90.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS90.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS90.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS90.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS90.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart impICMSPart = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMSPart.pICMS, cultura).ToString("n2"),
                            BaseCalculoICMS = decimal.Parse(impICMSPart.vBC, cultura).ToString("n2"),
                            ValorICMS = decimal.Parse(impICMSPart.vICMS, cultura).ToString("n2"),
                            BaseCalculoICMSST = decimal.Parse(impICMSPart.vBCST, cultura).ToString("n2"),
                            ValorICMSST = decimal.Parse(impICMSPart.vICMSST, cultura).ToString("n2"),
                            CST = string.Format("{0:00}", (int)impICMSPart.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST impICMSST = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMSST.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101 impICMSSN101 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN101.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102 impICMSSN102 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN102.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201 impICMSSN201 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMSSN201.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMSSN201.vICMSST, cultura),
                            CST = string.Format("{0:000}", (int)impICMSSN201.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202 impICMSSN202 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMSSN202.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMSSN202.vICMSST, cultura),
                            CST = string.Format("{0:000}", (int)impICMSSN202.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500 impICMSSN500 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN500.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900 impICMSSN900 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMSSN900.pICMS != null ? decimal.Parse(impICMSSN900.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMSSN900.vBC != null ? decimal.Parse(impICMSSN900.vBC, cultura) : 0m,
                            ValorICMS = impICMSSN900.vICMS != null ? decimal.Parse(impICMSSN900.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMSSN900.vBCST != null ? decimal.Parse(impICMSSN900.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN900.vICMSST != null ? decimal.Parse(impICMSSN900.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN900.CSOSN)
                        };
                    }
                }
            }

            return null;
        }

        private object ObterICMS(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var icms = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMS) select (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMS)obj).FirstOrDefault();

                if (icms != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoICMS = icms.Item.GetType();

                    if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00 impICMS00 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS00.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS00.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS00.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS00.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10 impICMS10 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS10.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS10.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS10.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS10.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS10.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS10.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20 impICMS20 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS20.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS20.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS20.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS20.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30 impICMS30 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMS30.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS30.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS30.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40 impICMS40 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS40.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51 impICMS51 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS51.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS51.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS51.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS51.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60 impICMS60 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS60.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70 impICMS70 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS70.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS70.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS70.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS70.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS70.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS70.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90 impICMS90 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS90.pICMS != null ? impICMS90.pICMS : "0", cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS90.vBC != null ?impICMS90.vBC : "0", cultura),
                            ValorICMS = decimal.Parse(impICMS90.vICMS != null ? impICMS90.vICMS : "0", cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS90.vBCST != null ? impICMS90.vBCST : "0", cultura),
                            ValorICMSST = decimal.Parse(impICMS90.vICMSST != null ? impICMS90.vICMSST : "0", cultura),
                            CST = string.Format("{0:00}", (int)impICMS90.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart impICMSPart = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMSPart.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMSPart.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMSPart.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMSPart.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMSPart.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMSPart.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST impICMSST = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMSST.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101 impICMSSN101 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN101.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102 impICMSSN102 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN102.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201 impICMSSN201 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMSSN201.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMSSN201.vICMSST, cultura),
                            CST = string.Format("{0:000}", (int)impICMSSN201.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202 impICMSSN202 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMSSN202.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMSSN202.vICMSST, cultura),
                            CST = string.Format("{0:000}", (int)impICMSSN202.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500 impICMSSN500 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN500.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900 impICMSSN900 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMSSN900.pICMS != null ? decimal.Parse(impICMSSN900.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMSSN900.vBC != null ? decimal.Parse(impICMSSN900.vBC, cultura) : 0m,
                            ValorICMS = impICMSSN900.vICMS != null ? decimal.Parse(impICMSSN900.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMSSN900.vBCST != null ? decimal.Parse(impICMSSN900.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN900.vICMSST != null ? decimal.Parse(impICMSSN900.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN900.CSOSN)
                        };
                    }
                }
            }

            return null;
        }

        private object ObterICMS(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var icms = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMS) select (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMS)obj).FirstOrDefault();

                if (icms != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoICMS = icms.Item.GetType();

                    if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00 impICMS00 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS00.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS00.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS00.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS00.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10 impICMS10 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS10.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS10.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS10.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS10.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS10.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS10.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20 impICMS20 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS20.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS20.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS20.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS20.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30 impICMS30 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMS30.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS30.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS30.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40 impICMS40 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS40.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51 impICMS51 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS51.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS51.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS51.vICMS, cultura),
                            CST = string.Format("{0:00}", (int)impICMS51.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60 impICMS60 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS60.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70 impICMS70 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS70.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS70.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS70.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS70.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS70.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS70.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90 impICMS90 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMS90.pICMS, cultura),
                            BaseCalculoICMS = decimal.Parse(impICMS90.vBC, cultura),
                            ValorICMS = decimal.Parse(impICMS90.vICMS, cultura),
                            BaseCalculoICMSST = decimal.Parse(impICMS90.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMS90.vICMSST, cultura),
                            CST = string.Format("{0:00}", (int)impICMS90.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart impICMSPart = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart)icms.Item;

                        return new
                        {
                            AliquotaICMS = decimal.Parse(impICMSPart.pICMS, cultura).ToString("n2"),
                            BaseCalculoICMS = decimal.Parse(impICMSPart.vBC, cultura).ToString("n2"),
                            ValorICMS = decimal.Parse(impICMSPart.vICMS, cultura).ToString("n2"),
                            BaseCalculoICMSST = decimal.Parse(impICMSPart.vBCST, cultura).ToString("n2"),
                            ValorICMSST = decimal.Parse(impICMSPart.vICMSST, cultura).ToString("n2"),
                            CST = string.Format("{0:00}", (int)impICMSPart.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST impICMSST = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMSST.CST)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101 impICMSSN101 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN101.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102 impICMSSN102 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN102.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201 impICMSSN201 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMSSN201.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMSSN201.vICMSST, cultura),
                            CST = string.Format("{0:000}", (int)impICMSSN201.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202 impICMSSN202 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = decimal.Parse(impICMSSN202.vBCST, cultura),
                            ValorICMSST = decimal.Parse(impICMSSN202.vICMSST, cultura),
                            CST = string.Format("{0:000}", (int)impICMSSN202.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500 impICMSSN500 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN500.CSOSN)
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900 impICMSSN900 = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMSSN900.pICMS != null ? decimal.Parse(impICMSSN900.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMSSN900.vBC != null ? decimal.Parse(impICMSSN900.vBC, cultura) : 0m,
                            ValorICMS = impICMSSN900.vICMS != null ? decimal.Parse(impICMSSN900.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMSSN900.vBCST != null ? decimal.Parse(impICMSSN900.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN900.vICMSST != null ? decimal.Parse(impICMSSN900.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN900.CSOSN)
                        };
                    }
                }
            }

            return null;
        }

        private object ObterIPI(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var ipi = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TIpi) select (MultiSoftware.NFe.v310.NotaFiscal.TIpi)obj).FirstOrDefault();

                if (ipi != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoIPI = ipi.Item.GetType();

                    if (tipoIPI == typeof(MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib impIPITrib = (MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib)ipi.Item;

                        decimal baseCalculo = 0m;
                        decimal aliquota = 0m;

                        if (impIPITrib.ItemsElementName != null && impIPITrib.Items != null)
                        {
                            if (impIPITrib.ItemsElementName[0] == MultiSoftware.NFe.v310.NotaFiscal.ItemsChoiceType.vBC && impIPITrib.ItemsElementName[1] == MultiSoftware.NFe.v310.NotaFiscal.ItemsChoiceType.pIPI)
                            {
                                baseCalculo = decimal.Parse(impIPITrib.Items[0], cultura);
                                aliquota = decimal.Parse(impIPITrib.Items[1], cultura);
                            }
                        }

                        return new
                        {
                            BaseCalculoIPI = baseCalculo,
                            AliquotaIPI = aliquota,
                            ValorIPI = decimal.Parse(impIPITrib.vIPI, cultura),
                            CST = string.Format("{0:00}", (int)impIPITrib.CST)
                        };
                    }
                }
            }

            return null;
        }

        private object ObterIPI(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var ipi = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TIpi) select (MultiSoftware.NFe.v400.NotaFiscal.TIpi)obj).FirstOrDefault();

                if (ipi != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoIPI = ipi.Item.GetType();

                    if (tipoIPI == typeof(MultiSoftware.NFe.v400.NotaFiscal.TIpiIPITrib))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TIpiIPITrib impIPITrib = (MultiSoftware.NFe.v400.NotaFiscal.TIpiIPITrib)ipi.Item;

                        decimal baseCalculo = 0m;
                        decimal aliquota = 0m;

                        if (impIPITrib.ItemsElementName != null && impIPITrib.Items != null)
                        {
                            if (impIPITrib.ItemsElementName[0] == MultiSoftware.NFe.v400.NotaFiscal.ItemsChoiceType.vBC && impIPITrib.ItemsElementName[1] == MultiSoftware.NFe.v400.NotaFiscal.ItemsChoiceType.pIPI)
                            {
                                baseCalculo = decimal.Parse(impIPITrib.Items[0], cultura);
                                aliquota = decimal.Parse(impIPITrib.Items[1], cultura);
                            }
                        }

                        return new
                        {
                            BaseCalculoIPI = baseCalculo,
                            AliquotaIPI = aliquota,
                            ValorIPI = decimal.Parse(impIPITrib.vIPI, cultura),
                            CST = string.Format("{0:00}", (int)impIPITrib.CST)
                        };
                    }
                }
            }

            return null;
        }

        private object ObterIPI(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var ipi = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoIPI) select (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoIPI)obj).FirstOrDefault();

                if (ipi != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoIPI = ipi.Item.GetType();

                    if (tipoIPI == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoIPIIPITrib))
                    {
                        MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoIPIIPITrib impIPITrib = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoIPIIPITrib)ipi.Item;

                        decimal baseCalculo = 0m;
                        decimal aliquota = 0m;

                        if (impIPITrib.ItemsElementName != null && impIPITrib.Items != null)
                        {
                            if (impIPITrib.ItemsElementName[0] == MultiSoftware.NFe.NotaFiscal.ItemsChoiceType.vBC && impIPITrib.ItemsElementName[1] == MultiSoftware.NFe.NotaFiscal.ItemsChoiceType.pIPI)
                            {
                                baseCalculo = decimal.Parse(impIPITrib.Items[0], cultura);
                                aliquota = decimal.Parse(impIPITrib.Items[1], cultura);
                            }
                        }

                        return new
                        {
                            BaseCalculoIPI = baseCalculo,
                            AliquotaIPI = aliquota,
                            ValorIPI = decimal.Parse(impIPITrib.vIPI, cultura),
                            CST = string.Format("{0:00}", (int)impIPITrib.CST)
                        };
                    }
                }
            }

            return null;
        }

        private object ObterPIS(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPIS infNFeDetImpostoPIS)
        {
            if (infNFeDetImpostoPIS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoPIS = infNFeDetImpostoPIS.Item.GetType();

                if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq impPISAliq = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISAliq.CST),
                        ValorPIS = decimal.Parse(impPISAliq.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr impPISOutr = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISOutr.CST),
                        ValorPIS = decimal.Parse(impPISOutr.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde impPISQtde = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISQtde.CST),
                        ValorPIS = decimal.Parse(impPISQtde.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT impPISNT = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISNT.CST),
                        ValorPIS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterPIS(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPIS infNFeDetImpostoPIS)
        {
            if (infNFeDetImpostoPIS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoPIS = infNFeDetImpostoPIS.Item.GetType();

                if (tipoPIS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq impPISAliq = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISAliq.CST),
                        ValorPIS = decimal.Parse(impPISAliq.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr impPISOutr = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISOutr.CST),
                        ValorPIS = decimal.Parse(impPISOutr.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde impPISQtde = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISQtde.CST),
                        ValorPIS = decimal.Parse(impPISQtde.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT impPISNT = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISNT.CST),
                        ValorPIS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterPIS(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPIS infNFeDetImpostoPIS)
        {
            if (infNFeDetImpostoPIS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoPIS = infNFeDetImpostoPIS.Item.GetType();

                if (tipoPIS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq impPISAliq = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISAliq.CST),
                        ValorPIS = decimal.Parse(impPISAliq.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr impPISOutr = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISOutr.CST),
                        ValorPIS = decimal.Parse(impPISOutr.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde impPISQtde = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISQtde.CST),
                        ValorPIS = decimal.Parse(impPISQtde.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT impPISNT = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISNT.CST),
                        ValorPIS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterCOFINS(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINS infNFeDetImpostoCOFINS)
        {
            if (infNFeDetImpostoCOFINS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoCOFINS = infNFeDetImpostoCOFINS.Item.GetType();

                if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq impCOFINSAliq = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSAliq.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSAliq.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr impCOFINSOutr = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSOutr.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSOutr.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde impCOFINSQtde = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSQtde.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSQtde.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST impCOFINSST = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = "",
                        ValorCOFINS = decimal.Parse(impCOFINSST.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT impCOFINSNT = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSNT.CST),
                        ValorCOFINS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterCOFINS(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINS infNFeDetImpostoCOFINS)
        {
            if (infNFeDetImpostoCOFINS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoCOFINS = infNFeDetImpostoCOFINS.Item.GetType();

                if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq impCOFINSAliq = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSAliq.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSAliq.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr impCOFINSOutr = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSOutr.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSOutr.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde impCOFINSQtde = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSQtde.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSQtde.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST impCOFINSST = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = "",
                        ValorCOFINS = decimal.Parse(impCOFINSST.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT impCOFINSNT = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSNT.CST),
                        ValorCOFINS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterCOFINS(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINS infNFeDetImpostoCOFINS)
        {
            if (infNFeDetImpostoCOFINS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoCOFINS = infNFeDetImpostoCOFINS.Item.GetType();

                if (tipoCOFINS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq impCOFINSAliq = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSAliq.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSAliq.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr impCOFINSOutr = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSOutr.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSOutr.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde impCOFINSQtde = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSQtde.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSQtde.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST impCOFINSST = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = "",
                        ValorCOFINS = decimal.Parse(impCOFINSST.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT))
                {
                    MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT impCOFINSNT = (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSNT.CST),
                        ValorCOFINS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterProduto(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente fornecedor, MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetProd prod, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento)
        {
            Repositorio.UnidadeMedidaGeral repUnidadeDeMedida = new Repositorio.UnidadeMedidaGeral(unitOfWork);
            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.NCM repNCM = new Repositorio.NCM(unitOfWork);
            
            Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorCodigoProdutoEFornecedor(empresa.Codigo, prod.cProd, fornecedor.CPF_CNPJ);
            Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCFOP(int.Parse(prod.CFOP), Dominio.Enumeradores.TipoCFOP.Entrada);

            if(produtoFornecedor == null && empresa.Configuracao.CadastrarItemDocumentoEntrada)
            {
                //CFOP que vem no XML é de Saída, e no sistema precisa lançar CFOP de entrada
                //if(cfop == null && (int)prod.CFOP > 0)
                //{
                //    cfop = new Dominio.Entidades.CFOP()
                //    {
                //        Tipo = Dominio.Enumeradores.TipoCFOP.Entrada,
                //        CodigoCFOP = (int)prod.CFOP,
                //        Descricao = "Importado Nota Entrada",
                //        Status = "A"
                //    };
                //    repCFOP.Inserir(cfop);
                //}

                Dominio.Entidades.NCM ncm = repNCM.BuscarPorNumero(prod.NCM);
                if(ncm == null)
                {
                    ncm = new Dominio.Entidades.NCM()
                    {
                        Descricao = "Outros",
                        Numero = prod.NCM
                    };
                    repNCM.Inserir(ncm);
                }

                Dominio.Entidades.UnidadeMedidaGeral un = repUnidadeDeMedida.BuscarPorSigla(empresa.Codigo, prod.uCom);
                if (un == null)
                {
                    un = new Dominio.Entidades.UnidadeMedidaGeral()
                    {
                        Descricao = prod.uCom,
                        Sigla = prod.uCom,
                        Empresa = empresa,
                        Status = "A"
                    };
                    repUnidadeDeMedida.Inserir(un);
                }

                Dominio.Entidades.Produto produto = new Dominio.Entidades.Produto() {
                    Descricao = prod.xProd,
                    CodigoProduto = prod.cProd,
                    NCM = ncm,
                    UnidadeMedida = un,
                    Status = "A",
                    Empresa = empresa
                };

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

                produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor()
                {
                    Produto = produto,
                    CodigoProduto = prod.cProd,
                    Fornecedor = fornecedor
                };
                repProdutoFornecedor.Inserir(produtoFornecedor);
            }
            if (produtoFornecedor != null)
            {
                return new
                {
                    CodigoProduto = produtoFornecedor.Produto.Codigo,
                    DescricaoProduto = produtoFornecedor.Produto.Descricao,
                    CodigoUnidadeMedida = produtoFornecedor.Produto.UnidadeMedida.Codigo,
                    DescricaoUnidadeMedida = produtoFornecedor.Produto.UnidadeMedida.Sigla + " - " + produtoFornecedor.Produto.UnidadeMedida.Descricao,
                    CFOP = cfop?.Codigo ?? 0,
                    DescricaoCFOP = cfop?.CodigoCFOP.ToString() ?? ""
                };
            }

            return null;
        }

        private object ObterProduto(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente fornecedor, MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetProd prod, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento)
        {
            Repositorio.UnidadeMedidaGeral repUnidadeDeMedida = new Repositorio.UnidadeMedidaGeral(unitOfWork);
            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.NCM repNCM = new Repositorio.NCM(unitOfWork);

            Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorCodigoProdutoEFornecedor(empresa.Codigo, prod.cProd, fornecedor.CPF_CNPJ);
            Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCFOP(int.Parse(prod.CFOP), Dominio.Enumeradores.TipoCFOP.Entrada);

            if (produtoFornecedor == null && empresa.Configuracao.CadastrarItemDocumentoEntrada)
            {
                //CFOP que vem no XML é de Saída, e no sistema precisa lançar CFOP de entrada
                //if(cfop == null && (int)prod.CFOP > 0)
                //{
                //    cfop = new Dominio.Entidades.CFOP()
                //    {
                //        Tipo = Dominio.Enumeradores.TipoCFOP.Entrada,
                //        CodigoCFOP = (int)prod.CFOP,
                //        Descricao = "Importado Nota Entrada",
                //        Status = "A"
                //    };
                //    repCFOP.Inserir(cfop);
                //}

                Dominio.Entidades.NCM ncm = repNCM.BuscarPorNumero(prod.NCM);
                if (ncm == null)
                {
                    ncm = new Dominio.Entidades.NCM()
                    {
                        Descricao = "Outros",
                        Numero = prod.NCM
                    };
                    repNCM.Inserir(ncm);
                }

                Dominio.Entidades.UnidadeMedidaGeral un = repUnidadeDeMedida.BuscarPorSigla(empresa.Codigo, prod.uCom);
                if (un == null)
                {
                    un = new Dominio.Entidades.UnidadeMedidaGeral()
                    {
                        Descricao = prod.uCom,
                        Sigla = prod.uCom,
                        Empresa = empresa,
                        Status = "A"
                    };
                    repUnidadeDeMedida.Inserir(un);
                }

                Dominio.Entidades.Produto produto = new Dominio.Entidades.Produto()
                {
                    Descricao = prod.xProd,
                    CodigoProduto = prod.cProd,
                    NCM = ncm,
                    UnidadeMedida = un,
                    Status = "A",
                    Empresa = empresa
                };

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

                produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor()
                {
                    Produto = produto,
                    CodigoProduto = prod.cProd,
                    Fornecedor = fornecedor
                };
                repProdutoFornecedor.Inserir(produtoFornecedor);
            }
            if (produtoFornecedor != null)
            {
                return new
                {
                    CodigoProduto = produtoFornecedor.Produto.Codigo,
                    DescricaoProduto = produtoFornecedor.Produto.Descricao,
                    CodigoUnidadeMedida = produtoFornecedor.Produto.UnidadeMedida.Codigo,
                    DescricaoUnidadeMedida = produtoFornecedor.Produto.UnidadeMedida.Sigla + " - " + produtoFornecedor.Produto.UnidadeMedida.Descricao,
                    CFOP = cfop?.Codigo ?? 0,
                    DescricaoCFOP = cfop?.CodigoCFOP.ToString() ?? ""
                };
            }

            return null;
        }

        private object ObterProduto(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente fornecedor, MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDetProd prod, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unitOfWork);

            Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorCodigoProdutoEFornecedor(empresa.Codigo, prod.cProd, fornecedor.CPF_CNPJ);

            if (produtoFornecedor != null)
            {
                return new
                {
                    CodigoProduto = produtoFornecedor.Produto.Codigo,
                    DescricaoProduto = produtoFornecedor.Produto.Descricao,
                    CodigoUnidadeMedida = produtoFornecedor.Produto.UnidadeMedida.Codigo,
                    DescricaoUnidadeMedida = produtoFornecedor.Produto.UnidadeMedida.Sigla + " - " + produtoFornecedor.Produto.UnidadeMedida.Descricao,
                };
            }

            return null;
        }
    }
}
