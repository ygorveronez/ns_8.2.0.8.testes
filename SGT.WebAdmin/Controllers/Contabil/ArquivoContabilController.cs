using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

using Dominio.Excecoes.Embarcador;
using NUglify.Helpers;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/ArquivoContabil")]
    public class ArquivoContabilController : BaseController
    {
		#region Construtores

		public ArquivoContabilController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoEContab()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);
                int.TryParse(Request.Params("Empresa"), out int empresaSelecionada);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa > 0 ? codigoEmpresa : empresaSelecionada);

                List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabil(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal);
                if (movimentoFinanceiro.Count() > 0)
                {
                    MemoryStream arquivoINPUT = new MemoryStream();
                    StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                    for (int i = 0; i < movimentoFinanceiro.Count(); i++)
                    {
                        var arquivoContabil = movimentoFinanceiro[i];
                        var observacao = arquivoContabil.Observacao != null ? arquivoContabil.Observacao.Trim() : string.Empty;
                        var documento = arquivoContabil.Documento.Trim().Length > 12 ? arquivoContabil.Documento.Trim().Substring(0, 12).Trim() : arquivoContabil.Documento.Trim();

                        if (!string.IsNullOrWhiteSpace(observacao))
                            observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                        int sizeObservacao = observacao.Length;

                        var observacao1 = sizeObservacao > 30 ? observacao.Substring(0, 30) : observacao;
                        var observacao2 = sizeObservacao > 60 ? observacao.Substring(30, 30) : sizeObservacao > 30 && sizeObservacao < 60 ? observacao.Substring(30, sizeObservacao - 30) : string.Empty;
                        var observacao3 = sizeObservacao > 90 ? observacao.Substring(60, 30) : sizeObservacao > 60 && sizeObservacao < 90 ? observacao.Substring(60, sizeObservacao - 60) : string.Empty;
                        var observacao4 = sizeObservacao > 120 ? observacao.Substring(90, 30) : sizeObservacao > 90 && sizeObservacao < 120 ? observacao.Substring(90, sizeObservacao - 90) : string.Empty;
                        var observacao5 = sizeObservacao > 150 ? observacao.Substring(120, 30) : sizeObservacao > 120 && sizeObservacao < 150 ? observacao.Substring(120, sizeObservacao - 120) : string.Empty;
                        var observacao6 = sizeObservacao > 180 ? observacao.Substring(150, 30) : sizeObservacao > 150 && sizeObservacao < 180 ? observacao.Substring(150, sizeObservacao - 150) : string.Empty;
                        var observacao7 = sizeObservacao > 210 ? observacao.Substring(180, 30) : sizeObservacao > 180 && sizeObservacao < 210 ? observacao.Substring(180, sizeObservacao - 180) : string.Empty;
                        var observacao8 = sizeObservacao > 240 ? observacao.Substring(210, 30) : sizeObservacao > 210 && sizeObservacao < 240 ? observacao.Substring(210, sizeObservacao - 210) : string.Empty;
                        var observacao9 = sizeObservacao > 270 ? observacao.Substring(240, 30) : sizeObservacao > 240 && sizeObservacao < 270 ? observacao.Substring(240, sizeObservacao - 240) : string.Empty;
                        var observacao10 = sizeObservacao > 300 ? observacao.Substring(270, 30) : sizeObservacao > 270 && sizeObservacao < 300 ? observacao.Substring(270, sizeObservacao - 270) : string.Empty;

                        x.WriteLine(empresa.RazaoSocial.Substring(0, 8).Trim() + "|" + //Nome abreviado da empresa
                            (i + 1).ToString().PadLeft(6, ' ') + "|" + //Código Sequencial alinhado a direita (Na importação o código será reprocessado)
                            arquivoContabil.DataMovimento.ToString("dd/MM/yyyy") + "|" + //Data do lançamento contábil
                            arquivoContabil.PlanoDeContaDebito.PlanoContabilidade + "|" + //Conta Debitada(relacionada ao PLANO do Exerc.)
                            arquivoContabil.PlanoDeContaCredito.PlanoContabilidade + "|" + //Conta Creditada (relacionada ao PLANO do Exerc.)
                            "|" + //Identificação do Lote
                            arquivoContabil.Valor.ToString("n3") + "|" + //Valor do Lançamento
                            observacao1.Trim() + "|" + //1ª Linha do Histórico
                            observacao2.Trim() + "|" + //2ª Linha do Histórico
                            observacao3.Trim() + "|" + //3ª Linha do Histórico
                            "N|" + //S-Sim ou N-Não (indica se é lançto. de zeramento de despesas e receitas para encerramento do balanço)
                            "|" + //Código do Centro de Custo a Débito (informado apenas se a Conta Debitada for de resultado)
                            "|" + //Código do Centro de Custo a Crédito (informado apenas se a Conta Creditada for de resultado)
                            "|" + //Código do Histórico (relacionado ao HISTORCB)
                            documento + "|" + //Número do Documento Contabilizado
                            "N|" + //S-Sim ou N-Não (indica se o lançto foi atualizado) (Obs: informar sempre Não)
                            "|" + //Informação específica para integração Contabilidade
                            "|" + //Informação específica para integração Contabilidade
                            observacao4.Trim() + "|" + //4ª Linha de Histórico
                            arquivoContabil.DataMovimento.ToString("MM") + "|" + //Informar o Mês da data do lançamento (com zero a esquerda)
                            observacao5.Trim() + "|" + //5ª Linha de Histórico
                            observacao6.Trim() + "|" + //6ª Linha de Histórico
                            observacao7.Trim() + "|" + //7ª Linha de Histórico
                            observacao8.Trim() + "|" + //8ª Linha de Histórico
                            observacao9.Trim() + "|" + //9ª Linha de Histórico
                            observacao10.Trim() + "|" + //10ª Linha de Histórico
                            arquivoContabil.Codigo + "|" + //Código sequencial gerado pelo sistema.
                            "N|" + //Lançamentos conciliados - default = N
                            "N|" + //Informe "S" ou "N" para Lançamentos de Ajuste (Informação para F-Cont)
                            "|" + //Deixar vazio no caso de "N" no campo Ajuste. Se "S" no campo Ajuste informar "F" para Lançamento Fiscal (a ser incluído) ou "N" paraLançamento Normal (a ser expurgado)
                            "|" + //Código da moeda estrangeira vinculada ao lançamento contábil.
                            "0,000|" //Valor do lançamento contábil em moeda estrangeira.
                            );
                    }
                    x.Flush();

                    return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoEContab_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                }
                else
                    return new JsonpResult(false, true, "Nenhum registro de Movimentos Financeiros ou Plano de Contas configurado para geração do arquivo contábil.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo E-contab.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoEuro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                bool gerarComCodigoHistoricoTipoMovimento = Request.GetBoolParam("GerarComCodigoHistoricoTipoMovimento");

                int codigoTipoMovimentoArquivoContabil = Request.GetIntParam("TipoMovimentoArquivoContabil");

                List<int> codigosModeloDocumentoFiscal = null;
                string codigosModelos = Request.Params("ModeloDocumentoFiscal");
                if (!string.IsNullOrEmpty(codigosModelos))
                    codigosModeloDocumentoFiscal = JsonConvert.DeserializeObject<List<int>>(codigosModelos);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                TipoMovimentoArquivoContabilEuro tipoMovimento = TipoMovimentoArquivoContabilEuro.ContasPagarNotaFiscal;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                if (tipoMovimento != TipoMovimentoArquivoContabilEuro.NFSeEntrada)
                {
                    Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilEuro(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal, tipoMovimento, codigoTipoMovimentoArquivoContabil, TipoServicoMultisoftware);
                    if (movimentoFinanceiro.Count() > 0)
                    {
                        MemoryStream arquivoINPUT = new MemoryStream();
                        StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                        for (int i = 0; i < movimentoFinanceiro.Count(); i++)
                        {
                            Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil arquivoContabil = movimentoFinanceiro[i];
                            Dominio.Entidades.Cliente pessoa = null;
                            if (arquivoContabil.CNPJPessoaTitulo > 0)
                                pessoa = repCliente.BuscarPorCPFCNPJ(arquivoContabil.CNPJPessoaTitulo);

                            string contaDebito = "";
                            string contaCredito = "";
                            string colunaCnpjFornecedor = "";
                            if (tipoMovimento == TipoMovimentoArquivoContabilEuro.DespesasAcertoViagem || tipoMovimento == TipoMovimentoArquivoContabilEuro.PagamentoMotorista)
                            {
                                contaDebito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaDebitoContabil) ? arquivoContabil.PlanoDeContaDebitoContabil : arquivoContabil.PlanoDeContaDebito;
                                contaCredito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaCreditoContabil) ? arquivoContabil.PlanoDeContaCreditoContabil : arquivoContabil.PlanoDeContaCredito;
                            }
                            else if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasReceberNotaFiscal || tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasReceber)
                            {
                                contaDebito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaDebitoContabil) ? arquivoContabil.PlanoDeContaDebitoContabil : arquivoContabil.PlanoDeContaDebito;

                                if (pessoa != null)
                                    contaCredito = pessoa.CPF_CNPJ_SemFormato;
                                else
                                    contaCredito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaCreditoContabil) ? arquivoContabil.PlanoDeContaCreditoContabil : arquivoContabil.PlanoDeContaCredito;
                            }
                            else
                            {
                                if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagarNotaFiscal && pessoa != null)
                                    contaDebito = pessoa.CPF_CNPJ_SemFormato;
                                else
                                    contaDebito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaDebitoContabil) ? arquivoContabil.PlanoDeContaDebitoContabil : arquivoContabil.PlanoDeContaDebito;

                                contaCredito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaCreditoContabil) ? arquivoContabil.PlanoDeContaCreditoContabil : arquivoContabil.PlanoDeContaCredito;

                                if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagar)
                                    colunaCnpjFornecedor = pessoa != null ? pessoa.CPF_CNPJ_SemFormato + ";" : ";";
                            }

                            string observacao = string.Empty;
                            if (!string.IsNullOrWhiteSpace(arquivoContabil.Documento))
                                observacao = "N Doc. " + arquivoContabil.Documento;
                            if (pessoa != null)
                                observacao += ", " + pessoa.Nome + " (" + pessoa.CPF_CNPJ_Formatado + ")";
                            if (!string.IsNullOrWhiteSpace(arquivoContabil.NumeroDocumento))
                                observacao += " - N. " + arquivoContabil.NumeroDocumento;
                            if (!string.IsNullOrWhiteSpace(arquivoContabil.Observacao))
                                observacao += ", " + arquivoContabil.Observacao.Trim();

                            string codigoHistoricoMovimentoFinanceiro = string.Empty;
                            if (gerarComCodigoHistoricoTipoMovimento && !string.IsNullOrWhiteSpace(arquivoContabil.CodigoHistoricoMovimentoFinanceiro))
                                codigoHistoricoMovimentoFinanceiro = arquivoContabil.CodigoHistoricoMovimentoFinanceiro;

                            if (!string.IsNullOrWhiteSpace(observacao))
                                observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                            observacao = observacao.Trim();

                            x.WriteLine(
                                (arquivoContabil.DataBaseSistema != null && arquivoContabil.DataBaseSistema > DateTime.MinValue ? arquivoContabil.DataBaseSistema.ToString("dd/MM/yyyy") : arquivoContabil.Data.ToString("dd/MM/yyyy")) + ";" + //Data do Lançamento
                                colunaCnpjFornecedor + //CPF/CNPJ - apenas pro tipo "Demais pagamentos"
                                contaDebito + ";" + //Conta Débito
                                contaCredito + ";" + //Conta Crédito
                                arquivoContabil.Valor.ToString("n2") + ";" + //Valor
                                codigoHistoricoMovimentoFinanceiro + ";" + //Código do Histórico                                
                                observacao + ";" //Histórico
                                );

                            unitOfWork.FlushAndClear();
                        }
                        x.Flush();

                        return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoEuro_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                    }
                    else
                        return new JsonpResult(false, true, "Nenhum registro de Movimentos Financeiros ou Plano de Contas configurado para geração do arquivo contábil.");
                }
                else //Arquivo de NFSe Entrada
                {
                    Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaDocumentoEntrada = repDocumentoEntrada.BuscarNFSePorEmpresa(codigoEmpresa, dataInicial, dataFinal, codigosModeloDocumentoFiscal, DateTime.MinValue, DateTime.MinValue);
                    if (listaDocumentoEntrada.Count() > 0)
                    {
                        MemoryStream arquivoINPUT = new MemoryStream();
                        StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                        for (int i = 0; i < listaDocumentoEntrada.Count(); i++)
                        {
                            var documentoEntrada = listaDocumentoEntrada[i];

                            //Registro 000 - Cabeçalho da NF - Ocorre Obrigatoriamente em cada NF - Uma única vez por NF
                            x.WriteLine(
                                "lcto;" + //Identificador de Registro
                                documentoEntrada.Fornecedor.CPF_CNPJ_SemFormato + ";" + //Fornecedor
                                documentoEntrada.Numero + ";" + //Nota Fiscal
                                "NFS;" + //Espécie
                                documentoEntrada.Serie + ";" + //Série
                                documentoEntrada.DataEntrada.ToString("dd/MM/yyyy") + ";" + //Data Entrada
                                documentoEntrada.DataEmissao.ToString("dd/MM/yyyy") + ";" + //Data Emissão
                                documentoEntrada.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor
                                documentoEntrada.BaseIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Base
                                documentoEntrada.ValorTotalIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Valor
                                "0,00;" + //[IPI] Isentas
                                "0,00;" + //[IPI] Outras
                                ";" + //Complemento
                                documentoEntrada.ValorTotalDesconto.ToString("n2").Replace(".", "") + ";" + //Desconto
                                "0,00;" + //Abatimento Não-Tributado
                                documentoEntrada.ValorTotalFrete.ToString("n2").Replace(".", "") + ";" + //Frete
                                "0,00;" + //Pedágio
                                documentoEntrada.ValorTotalSeguro.ToString("n2").Replace(".", "") + ";" + //Seguro
                                documentoEntrada.ValorTotalOutrasDespesas.ToString("n2").Replace(".", "") + ";" + //Outras Despesas
                                "99;" + //Modelo Doc
                                ";" + //Chave Acesso
                                ";" + //Chave Acesso  Ref
                                "0;" + //Finalidade
                                (int)documentoEntrada.IndicadorPagamento + ";" + //Pagamento
                                "99;" + //Meio Pagto
                                "9;" + //Mod.Frete
                                "0;" //Sit.Doc.Fiscal
                                );

                            var listaDocumentoEntradaItens = documentoEntrada.Itens;

                            //Registro 001 - CFOP Da NF - Ocorre Obrigatoriamente em cada NF -
                            var listaCFOPs = listaDocumentoEntradaItens.GroupBy(
                                obj => obj.CFOP != null ? obj.CFOP.CodigoCFOP : -1).Select(obj => new
                                {
                                    CFOP = obj.Key,
                                    ValorTotal = obj.Sum(dc => dc.ValorTotal),
                                    BaseCalculoICMS = obj.Sum(dc => dc.BaseCalculoICMS),
                                    AliquotaICMS = obj.Sum(dc => dc.AliquotaICMS),
                                    ValorICMS = obj.Sum(dc => dc.ValorICMS)
                                }).ToList();
                            for (int j = 0; j < listaCFOPs.Count(); j++)
                            {
                                var cfopItens = listaCFOPs[j];

                                if (cfopItens.CFOP > 0)
                                {
                                    x.WriteLine(
                                        "cfop;" + //Identificador de Registro
                                        cfopItens.CFOP + ";" + //CFOP
                                        cfopItens.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor
                                        cfopItens.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Base Cálculo
                                        "0,00;" + //Alíquota Imposto
                                        "0,00;" + //Valor Imposto
                                        "0,00;" + //Isentas Imposto
                                        "0,00;" //Outras
                                        );
                                }
                            }

                            //Registro 002 - Produtos Da NF - Ocorre Obrigatoriamente em cada NF - Pode ocorrer mais vezes por NF
                            for (int j = 0; j < listaDocumentoEntradaItens.Count(); j++)
                            {
                                var documentoEntradaItem = listaDocumentoEntradaItens[j];
                                var cfop = documentoEntradaItem.CFOP != null ? documentoEntradaItem.CFOP.CodigoCFOP.ToString() : string.Empty;

                                x.WriteLine(
                                "prod;" + //Identificador de Registro
                                documentoEntradaItem.Produto.Codigo + ";" + //Cod. Prod
                                cfop + ";" + //CFOP
                                UnidadeDeMedidaHelper.ObterSigla(documentoEntradaItem.UnidadeMedida) + ";" + //Un. Med
                                documentoEntradaItem.Quantidade.ToString("n2").Replace(".", "") + ";" + //Quantidade
                                documentoEntradaItem.ValorUnitario.ToString("n2").Replace(".", "") + ";" + //Valor Unitário
                                documentoEntradaItem.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor Total
                                (!string.IsNullOrWhiteSpace(documentoEntradaItem.CSTICMS) ? documentoEntradaItem.CSTICMS : string.Empty) + ";" + //[ICMS] CST
                                documentoEntradaItem.BaseCalculoICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Base Cálculo
                                documentoEntradaItem.AliquotaICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Alíquota
                                documentoEntradaItem.ValorICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Valor
                                "0,00;" + //[ICMS] Isentas
                                "0,00;" + //[ICMS] Outras
                                (!string.IsNullOrWhiteSpace(documentoEntradaItem.CSTIPI) ? documentoEntradaItem.CSTIPI : string.Empty) + ";" + //[IPI] CST
                                documentoEntradaItem.BaseCalculoIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Base Cálculo
                                documentoEntradaItem.AliquotaIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Alíquota
                                documentoEntradaItem.ValorIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Valor
                                "0,00;" + //[IPI] Isentas
                                "0,00;" + //[IPI] Outras
                                ";" + //[ISS] CST
                                "0,00;" + //[ISS] Base Cálculo
                                "0,00;" + //[ISS] Alíquota
                                "0,00;" + //[ISS] Valor
                                "0,00;" + //[ISS] Isentas
                                "0,00;" + //[ISS] Outras
                                ";" + //[Sub.Trib.] CST
                                documentoEntradaItem.BaseCalculoICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Base Cálculo
                                documentoEntradaItem.AliquotaICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Alíquota
                                documentoEntradaItem.ValorICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Valor
                                "0,00;" + //[Sub.Trib.] Isentas
                                "0,00;" + //[Sub.Trib.] Outras
                                documentoEntradaItem.Desconto.ToString("n2").Replace(".", "") + ";" + //Val. Desconto
                                "0,00;" + //Val. Despesa Acessória
                                "1;" + //Tipo Estoque
                                ";" + //Qtd.Selo.Contr.IPI
                                documentoEntradaItem.ValorFrete.ToString("n2").Replace(".", "") + ";" + //Val. Frete
                                "0,00;" + //Val. Pedágio
                                documentoEntradaItem.ValorSeguro.ToString("n2").Replace(".", "") + ";" + //Val. Seguro
                                "0,00;" + //Val.Abat.Não Trib
                                "1;" + //Movimenta ?
                                ";" //Indic.Nat.Frete
                                );
                            }

                            //Registro 003 Retenções Da NF - Não Ocorre Obrigatoriamente em cada NF - Não pode ocorrer mais vezes por NF
                            x.WriteLine(
                                "reti;" + //Identificador de Registro
                                "0,00;" + //[INSS] Base Cálculo
                                "0,00;" + //[INSS] Alíquota
                                documentoEntrada.ValorTotalRetencaoINSS.ToString("n2").Replace(".", "") + ";" + //[INSS] Valor
                                documentoEntrada.BaseISS.ToString("n2").Replace(".", "") + ";" + //[ISS] Base Cálculo
                                "0,00;" + //[ISS] Alíquota
                                documentoEntrada.ValorTotalRetencaoISS.ToString("n2").Replace(".", "") + ";" + //[ISS] Valor
                                "0,00;" + //[IRRF] Base Cálculo
                                "0,00;" + //[IRRF] Alíquota
                                documentoEntrada.ValorTotalRetencaoIR.ToString("n2").Replace(".", "") + ";" + //[IRRF] Valor
                                ";" + //[IRRF] Código
                                ";" + //[IRRF] Variação
                                ";" + //[IRRF] Data Pagto
                                (documentoEntrada.ValorTotalRetencaoPIS + documentoEntrada.ValorTotalRetencaoCOFINS + documentoEntrada.ValorTotalRetencaoCSLL).ToString("n2").Replace(".", "") + ";" + //[PCC] Total
                                "1;" + //[PCC] Tipo
                                documentoEntrada.ValorTotalRetencaoPIS.ToString("n2").Replace(".", "") + ";" + //[PIS] Valor
                                documentoEntrada.ValorTotalRetencaoCOFINS.ToString("n2").Replace(".", "") + ";" + //[COFINS] Valor
                                documentoEntrada.ValorTotalRetencaoCSLL.ToString("n2").Replace(".", "") + ";" + //[CSLL] Valor
                                ";" //[PCC] Data Pagto
                                );
                        }
                        x.Flush();

                        return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoEuro_NFSeEntrada_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                    }
                    else
                        return new JsonpResult(false, true, "Nenhum registro de NFS-e de Entrada lançadas no período para geração do arquivo contábil.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Euro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoJB()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);
                int empresaSelecionada = 0;
                int.TryParse(Request.Params("Empresa"), out empresaSelecionada);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa > 0 ? codigoEmpresa : empresaSelecionada);

                List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilJB(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal);
                if (movimentoFinanceiro.Count() > 0)
                {
                    MemoryStream arquivoINPUT = new MemoryStream();
                    StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                    foreach (var mov in movimentoFinanceiro)
                    {
                        string contaMovimento = "";

                        if (mov.TipoDocumentoMovimento == TipoDocumentoMovimento.Pagamento)
                            contaMovimento = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : string.Empty;
                        else
                            contaMovimento = !string.IsNullOrWhiteSpace(mov.PlanoDeContaCredito.PlanoContabilidade) ? mov.PlanoDeContaCredito.PlanoContabilidade : mov.PlanoDeContaCredito.Plano;

                        string observacao = mov.Observacao;
                        if (!string.IsNullOrWhiteSpace(observacao))
                            observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                        x.WriteLine("00;" + //TPREGISTRO
                            ";" + //EMPRESA
                            ";" + //FILIAL
                            mov.DataMovimento.ToString("ddMMyyyy") + ";" + //DATA
                            mov.Codigo.ToString("n0").Replace(".", "") + ";" + //NRLCTOERP
                            "D" + ";" + //TP
                            contaMovimento + ";" + //CONTA
                            ";" + //SUBCONTA
                            mov.Valor.ToString("n2").Replace(".", "").Replace(",", ".") + ";" + //VALOR
                            "0;" + //ACAO
                            "2;" + //PRIMEIROHISTCTA
                            ";" + //CODHISTORICO
                            (observacao.Length > 200 ? observacao.Substring(0, 199).Replace(";", "") : observacao.Replace(";", "")) + ";" + //COMPLHISTORICO
                            mov.Codigo.ToString("n0").Replace(".", "") + ";" + //GRUPOLCTO
                            (mov.Titulo != null && mov.Titulo.Pessoa != null ? mov.Titulo.Pessoa.CPF_CNPJ_Formatado :
                                mov.Pessoa != null ? mov.Pessoa.CPF_CNPJ_Formatado : string.Empty) + ";" + //CNPJ
                            (mov.Titulo != null && mov.Titulo.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Titulo.Pessoa.IE_RG) && mov.Titulo.Pessoa.IE_RG != "ISENTO" ? mov.Titulo.Pessoa.IE_RG :
                                mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.IE_RG) && mov.Pessoa.IE_RG != "ISENTO" ? mov.Pessoa.IE_RG : string.Empty) + ";" + //IESTADUAL
                            (mov.Titulo != null ? mov.Titulo.TipoTitulo.ToString("D") : string.Empty) + ";" + //TPCNPJ
                            contaMovimento + ";" + //CONTAORIGEM
                            empresa.CNPJ_SemFormato + ";" + //CNPJEMPRESA
                            empresa.InscricaoEstadual + ";" //IEEMPRESA
                            );

                        if (mov.TipoDocumentoMovimento == TipoDocumentoMovimento.Pagamento)
                            contaMovimento = !string.IsNullOrWhiteSpace(mov.PlanoDeContaDebito.PlanoContabilidade) ? mov.PlanoDeContaDebito.PlanoContabilidade : mov.PlanoDeContaDebito.Plano;
                        else
                            contaMovimento = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : string.Empty;

                        //processo inverso solicitado pela MZ
                        x.WriteLine("00;" + //TPREGISTRO
                            ";" + //EMPRESA
                            ";" + //FILIAL
                            mov.DataMovimento.ToString("ddMMyyyy") + ";" + //DATA
                            mov.Codigo.ToString("n0").Replace(".", "") + ";" + //NRLCTOERP
                            "C" + ";" + //TP
                            contaMovimento + ";" + //CONTA
                            ";" + //SUBCONTA
                            mov.Valor.ToString("n2").Replace(".", "").Replace(",", ".") + ";" + //VALOR
                            "0;" + //ACAO
                            "2;" + //PRIMEIROHISTCTA
                            ";" + //CODHISTORICO
                            (observacao.Length > 200 ? observacao.Substring(0, 199).Replace(";", "") : observacao.Replace(";", "")) + ";" + //COMPLHISTORICO
                            mov.Codigo.ToString("n0").Replace(".", "") + ";" + //GRUPOLCTO
                            (mov.Titulo != null && mov.Titulo.Pessoa != null ? mov.Titulo.Pessoa.CPF_CNPJ_Formatado :
                                mov.Pessoa != null ? mov.Pessoa.CPF_CNPJ_Formatado : string.Empty) + ";" + //CNPJ
                            (mov.Titulo != null && mov.Titulo.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Titulo.Pessoa.IE_RG) && mov.Titulo.Pessoa.IE_RG != "ISENTO" ? mov.Titulo.Pessoa.IE_RG :
                                mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.IE_RG) && mov.Pessoa.IE_RG != "ISENTO" ? mov.Pessoa.IE_RG : string.Empty) + ";" + //IESTADUAL
                            (mov.Titulo != null ? mov.Titulo.TipoTitulo.ToString("D") : string.Empty) + ";" + //TPCNPJ
                            contaMovimento + ";" + //CONTAORIGEM
                            empresa.CNPJ_SemFormato + ";" + //CNPJEMPRESA
                            empresa.InscricaoEstadual + ";" //IEEMPRESA
                            );
                    }
                    x.Flush();

                    return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoJB_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                }
                else
                    return new JsonpResult(false, true, "Nenhum registro de Movimentos Financeiros ou Plano de Contas configurado para geração do arquivo contábil.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo JB.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoPH()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                if (dataFinal != DateTime.MinValue)
                    dataFinal = dataFinal.AddHours(23).AddMinutes(59).AddSeconds(59);

                int empresaSelecionada = Request.GetIntParam("Empresa");
                int codigoTipoMovimentoArquivoContabil = Request.GetIntParam("TipoMovimentoArquivoContabil");
                int codigoPlanoConta = Request.GetIntParam("PlanoConta");

                TipoMovimentoArquivoContabilQuestor tipoMovimento = TipoMovimentoArquivoContabilQuestor.ContasPagar;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = null;

                if (empresaSelecionada > 0)
                    empresa = repEmpresa.BuscarPorCodigo(empresaSelecionada);

                MemoryStream arquivoINPUT = new MemoryStream();
                StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.ASCII);
                string observacao = "";
                int qtdRegistros = 0;

                if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.NFeEntrada)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentos = repDocumentoEntrada.BuscarDocumentoPorEmpresa(empresaSelecionada, dataInicial, dataFinal);
                    if (documentos == null || documentos.Count <= 0)
                        return new JsonpResult(false, true, "Nenhum documento de entrada encontrado para o filtro selecionado.");

                    qtdRegistros++;
                    x.WriteLine("10" + //001 Tipo do Registro 2 001 002 N Conteúdo '10'
                                documentos[0].Destinatario.CNPJ_SemFormato + //002 CNPJ do Estabelecimento 14 003 016 N CNPJ Válido
                                Utilidades.String.Left(documentos[0].Destinatario.InscricaoEstadual.PadLeft(15, ' '), 15) + //003 Inscrição Estadual 15 017 031 A
                                dataInicial.Date.ToString("MMyyyy") + //004 Mês/Ano 6 032 037 N MMAAAA
                                (" ".PadLeft(219, ' '))//005 Espaços 219 038 256 A Livre
                                );
                    foreach (var documento in documentos)
                    {
                        qtdRegistros++;
                        string cstItem = documento.Itens.Select(i => i.CSTICMS).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(cstItem))
                            cstItem = "090";
                        //20E
                        x.WriteLine("20" + //001 Tipo do Registro 2 001 002 N Conteúdo '20'
                                    documento.DataEmissao.ToString("dd") + //002 Dia 2 003 004 N Dia da Entrada
                                    documento.DataEmissao.ToString("ddMMyyyy") + //003 Data do Documento 8 005 012 N DDMMAAAA
                                    Utilidades.String.Left((documento.Modelo?.Abreviacao ?? "").PadRight(5, ' '), 5) + //004 Espécie 5 013 017 A
                                    Utilidades.String.Left((string.IsNullOrWhiteSpace(documento.Serie) ? "1" : documento.Serie).PadRight(5, ' '), 5) + //005 Série/Subsérie 5 018 022 A
                                    (" ".PadRight(6, ' ')) + //006 Número Documento (Opcional) 6 023 028 N
                                    (Utilidades.String.Left(Utilidades.String.OnlyNumbers(documento.CFOP?.CodigoCFOP.ToString("n0")).PadRight(4, ' '), 4)) + //007 Código Fiscal de Operações 4 029 032 N
                                    Utilidades.String.Left((documento.CFOP?.Extensao ?? "").PadRight(4, ' '), 4) + //008 Histórico Identificador 4 033 036 N Conforme Cadastro EFPH
                                    (" ".PadRight(25, ' ')) + //009 Observações 25 037 061 A Observações a constarem no livro de entradas.
                                    Utilidades.String.Left(RetornarModeloDocumentoPH(documento.Modelo?.Numero).PadRight(1, ' '), 1) + //010 Modelo do Documento 1 062 062 A TABELA D
                                    ("0".PadRight(1, ' ')) + //011 Modalidade Frete 1 063 063 N 0 = Sem Frete
                                    (documento.Situacao == SituacaoDocumentoEntrada.Cancelado ? "1" : "0") + //012 Situação do Documento 1 064 064 N 0 = Normal 1=Cancelado
                                    ("00".PadRight(2, ' ')) + //013 Sub-Código Fiscal (SC) 2 065 066 N 2 dígitos adicionais
                                    ("N".PadRight(1, ' ')) + //014 Indicador NF Entrada 1 067 067 A Informe 'N' p/documentos
                                    ((documento.Modelo?.Abreviacao != "NFS-e" && documento.Modelo?.Abreviacao != "NFS") ? " " : documento.Fornecedor.Localidade.CodigoIBGE == documento.Destinatario.Localidade.CodigoIBGE ? "N" : "S") + //015 Execução fora do Município 1 068 068 A Informe 'S' quando serviço
                                    ((documento.Modelo?.Abreviacao != "NFS-e" && documento.Modelo?.Abreviacao != "NFS") ? " " : "N".PadRight(1, ' ')) + //016 Exec.em bens de Terceiros 1 069 069 A
                                    (" ".PadRight(10, ' ')) + //017 Código Atividade/Serviço 10 070 079 A Necessário para geração
                                    ((documento.Modelo?.Abreviacao != "NFS-e" && documento.Modelo?.Abreviacao != "NFS") ? " " : "1".PadRight(1, ' ')) + //018 Tipo NF Serviço 1 080 080 N 1 = Normal 
                                    (documento.Modelo?.Abreviacao == "NFS-e" || documento.Modelo?.Abreviacao == "NF-e" || documento.Modelo?.Abreviacao == "NFe" || documento.Modelo?.Abreviacao == "CT-e" || documento.Modelo?.Abreviacao == "CTe" ? "S" : "N") + //019 Documento Eletrônico 1 081 081 A No caso de NF,NFPS ou CTRC
                                    Utilidades.String.Left(documento.Chave.PadRight(44, ' '), 44) + //020 Chave Eletrônica 44 082 125 N Para Doc.Eletrônico
                                    (" ".PadRight(75, ' ')) + //021 Complemento de Observações 75 126 200 A Somente para empresas
                                                              //Utilidades.String.Left(Utilidades.String.OnlyNumbers(documento.CFOP?.CodigoCFOP.ToString("n0")).PadRight(4, ' '), 4) + //022 CFOP do Documento 4 201 204 N
                                    (" ".PadRight(4, ' ')) + //022 CFOP do Documento 4 201 204 N
                                    (documento.IndicadorPagamento == IndicadorPagamentoDocumentoEntrada.APrazo ? "2" : documento.IndicadorPagamento == IndicadorPagamentoDocumentoEntrada.AVista ? "1" : "4") + //023 Tipo da Fatura 1 205 205 N 1 = A Vista 2 = A Prazo
                                    (" ".PadRight(4, ' ')) + //024 Compl.Código da Atividade 4 206 209 A Complemento do campo 17.
                                    Utilidades.String.Right(Utilidades.String.OnlyNumbers(documento.Numero.ToString("n0")).PadLeft(9, '0'), 9) + //025 Número Documento 9 210 218 N
                                    Utilidades.String.Left((cstItem ?? "").PadRight(3, ' '), 3) + //026 CST ICMS 3 253 255 N
                                    ("1".PadRight(1, ' ')) + //027 Finalidade Documento 1 255 256 A 1-Normal 
                                    (" ".PadRight(34, ' ')) //028 Espaços 34 223 256 A Livre
                        );

                        //21E
                        qtdRegistros++;
                        x.WriteLine("21" + //001 Tipo do Registro 2 001 002 N Conteúdo '21'
                                    (documento.ValorTotal.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //002 Valor Contábil 12 003 014 V
                                    (documento.ValorTotalFrete.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //003 Frete 12 015 026 V
                                    (documento.ValorTotalSeguro.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //004 Seguro 12 027 038
                                    (documento.ValorTotalOutrasDespesas.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //005 Outras Despesas Acessórias 12 039 050 V
                                    ("0".PadLeft(12, '0')) + //006 Desconto Global 12 051 062 V
                                    ("0".PadLeft(12, '0')) + //007 Valor Funrural 12 063 074 V
                                    (documento.BaseCalculoICMSST.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //008 Base ICMS Subst. Retido 12 075 086 V Valor Base ICMS Retido por
                                    (documento.ValorTotalICMSST.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //009 ICMS Substituto Retido 12 087 098 V Valor de ICMS Retido por
                                    ("0".PadLeft(1, '0')) + //010 Cód.Antecip.ICMS Subst. 1 099 099 N Código de Antecipação do
                                    ("E".PadRight(1, ' ')) + //011 Tipo ICMS Subst. Retido 1 100 100 A TABELA E
                                    ("0".PadLeft(12, '0')) + //012 Desmembra Valor Contábil 1 12 101 112 V
                                    ("0".PadLeft(12, '0')) + //013 Desmembra Valor Contábil 2 12 113 124 V
                                    ("0".PadLeft(12, '0')) + //014 Desmembra Valor Contábil 3 12 125 136 V
                                    ("0".PadLeft(12, '0')) + //015 Desmembra Valor Contábil 4 12 137 148 V
                                    ("0".PadLeft(12, '0')) + //016 Desmembra Valor Contábil 5 12 149 160 V
                                    ((documento.ValorTotalPIS + documento.ValorTotalCOFINS).ToString("n2").Replace(".", "").Replace(",", "").PadLeft(9, '0')) + //017 PIS/COFINS 9 161 169 V Os Campos 18 a 23 são
                                    ("0".PadLeft(9, '0')) + //019 Ressarc.de Subst.Tribut. 9 179 187 V Registro Tipo 54 do Convê
                                    ("0".PadLeft(9, '0')) + //020 Transferência de Crédito 9 188 196 V nio ICMS 57/95 e 69/2002.
                                    ("0".PadLeft(9, '0')) + //021 Compl.Valor NF/ICMS 9 197 205 V São itens constantes no
                                    ("0".PadLeft(9, '0')) + //022 Serviço não Tributado 9 206 214 V documento.
                                    ("0".PadLeft(3, '0')) + //023 Reservado 3 215 217 N Reservado
                                    ("0".PadLeft(3, '0')) + //024 Reservado 3 218 220 N Reservado
                                    ("0".PadLeft(9, '0')) + //025 Abatimento NT 9 221 229 N Abatimento Não Tributado
                                    ("0".PadLeft(9, '0')) + //025 Abatimento NT 9 221 229 N Abatimento Não Tributado
                                    Utilidades.String.Left((documento.TipoMovimento?.PlanoDeContaDebito.PlanoContabilidade ?? "").PadLeft(4, '0'), 4) + //026 Centro de Custos Débito 4 230 233 N Código do Centro de Custos
                                    Utilidades.String.Left((documento.TipoMovimento?.PlanoDeContaCredito.PlanoContabilidade ?? "").PadLeft(4, '0'), 4) + //027 Centro de Custos Crédito 4 234 237 N Código do Centro de Custos
                                    (" ".PadLeft(19, ' '))//028 Espaços 19 238 256 A Livre
                        );

                        //22
                        qtdRegistros++;
                        x.WriteLine("22" + //001 Tipo do Registro 2 001 002 N Conteúdo '22'
                                    Utilidades.String.Left((documento.Fornecedor.Nome ?? "").PadRight(40, ' '), 40) + //002 Nome 40 003 042 A
                                    Utilidades.String.Right(documento.Fornecedor.CPF_CNPJ_SemFormato.PadLeft(18, '0'), 18) + //003 Inscrição(CNPJ/CPF) 18 043 060 N
                                    (documento.Fornecedor.Tipo == "F" ? "2" : documento.Fornecedor.Tipo == "J" ? "1" : "3") + //019 Documento Eletrônico 1 081 081 A No caso de NF,NFPS ou CTRC
                                    Utilidades.String.Left((documento.Fornecedor.IE_RG ?? "").PadRight(14, ' '), 14) + //005 Inscrição Estadual 14 062 075 A Obrigatório se Tipo
                                    Utilidades.String.Left((documento.Fornecedor.Endereco ?? "").PadRight(40, ' '), 40) + //006 Endereço 40 076 115 A
                                    Utilidades.String.Left((documento.Fornecedor.Bairro ?? "").PadRight(14, ' '), 14) + //007 Bairro 14 116 129 A
                                    Utilidades.String.Left((documento.Fornecedor.CEP ?? "").PadRight(9, ' '), 9) + //008 CEP 9 130 138 A
                                    Utilidades.String.Left((documento.Fornecedor.Localidade?.Estado?.Sigla ?? "").PadRight(2, ' '), 2) + //009 Estado 2 139 140 A
                                    Utilidades.String.Left(Utilidades.String.OnlyNumbers(documento.Fornecedor.Localidade?.CodigoIBGE.ToString("n0")).PadRight(25, ' '), 25) + //010 Município 25 141 165 A É possível informar 0
                                    (" ".PadLeft(8, ' ')) + //011 Código Contábil 8 166 173 N Informar quando EFPH for
                                    Utilidades.String.Right((documento.Fornecedor.NumeroCUITRUT ?? "").PadLeft(12, '0'), 12) + //012 CEI/NIT 12 174 185 N Informar o CEI/NIT do
                                    Utilidades.String.Left((documento.Fornecedor.InscricaoMunicipal ?? "").PadRight(14, ' '), 14) + //013 Inscrição Municipal 14 186 199 A Informar Num. da Inscrição
                                    Utilidades.String.Right((documento.Fornecedor.Telefone1 ?? "").PadLeft(11, '0'), 11) + //014 Telefone 11 200 210 N
                                    Utilidades.String.Left((documento.Fornecedor.NomeFantasia ?? "").PadRight(40, ' '), 40) + //015 Nome Fantasia 40 211 250 A
                                    Utilidades.String.Right(Utilidades.String.OnlyNumbers(documento.Fornecedor.Localidade?.Pais?.Codigo.ToString("n0")).PadLeft(4, '0'), 4) + //016 Código do País 4 251 254 N
                                    ("0".PadLeft(1, '0')) + //017 Tipo da Entidade 1 255 255 N 1 = Órgãos, Autarquias e
                                    ("0".PadLeft(1, '0'))//018 Atacadista 1 256 256 N 1 = Atacadista
                        );

                        //23E
                        decimal aliquotaICMS = 0;
                        if (documento.ValorTotalICMS > 0 && documento.BaseCalculoICMS > 0)
                            aliquotaICMS = Math.Round((documento.ValorTotalICMS / documento.BaseCalculoICMS) * 100);
                        qtdRegistros++;
                        x.WriteLine("23" + //001 Tipo do Registro 2 001 002 N Conteúdo '23'
                                    (documento.BaseCalculoICMS.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //002 Base de Cálculo 1 12 003-014 V
                                    (aliquotaICMS.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(4, '0')) + //003 Alíquota 1 4 015-018 V
                                    (documento.ValorTotalICMS.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //004 Valor ICMS 1 12 019-030 V

                                    ("0".PadLeft(12, '0')) + //005 Base de Cálculo 2 12 031-042 V
                                    ("0".PadLeft(4, '0')) + //006 Alíquota 2 4 043-046 V
                                    ("0".PadLeft(12, '0')) + //007 Valor ICMS 2 12 047-058 V

                                    ("0".PadLeft(12, '0')) + //008 Base de Cálculo 3 12 059-070 V
                                    ("0".PadLeft(4, '0')) + //009 Alíquota 3 4 071-074 V
                                    ("0".PadLeft(12, '0')) + //010 Valor ICMS 3 12 075-086 V

                                    ("0".PadLeft(12, '0')) + //011 Base de Cálculo 4 12 087-098 V
                                    ("0".PadLeft(4, '0')) + //012 Alíquota 4 4 099-102 V
                                    ("0".PadLeft(12, '0')) + //013 Valor ICMS 4 12 103-114 V

                                    ("0".PadLeft(12, '0')) + //014 Base de Cálculo 5 12 115-126 V
                                    ("0".PadLeft(4, '0')) + //015 Alíquota 5 4 127-130 V
                                    ("0".PadLeft(12, '0')) + //016 Valor ICMS 5 12 131-142 V

                                    ("0".PadLeft(12, '0')) + //017 Isentas 12 143-154 V
                                    ((documento.BaseCalculoICMS <= 0 ? documento.ValorTotal : 0m).ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //018 Outras 12 155-166 V

                                    ("0".PadLeft(7, '0')) + //019 Redução da Base de ICMS 7 167-173 V4

                                    ("0".PadLeft(12, '0')) + //020 Total FCP Destino(DIFAL) 12 174-185 N
                                    ("0".PadLeft(12, '0')) + //021 Total ICMS Inter. Destino(DIFAL) 12 186-197 N
                                    ("0".PadLeft(12, '0')) + //022 Total ICMS Inter. Remetente(DIFAL) 12 198-209 N

                                    (" ".PadRight(47, ' '))//023 Espaços 47 217-256 A Livre
                        );

                        //25
                        if (documento.ValorTotalIPI > 0)
                        {
                            decimal aliquotaIPI = 0;
                            if (documento.ValorTotalIPI > 0 && documento.BaseIPI > 0)
                                aliquotaIPI = Math.Round((documento.ValorTotalIPI / documento.BaseIPI) * 100);
                            qtdRegistros++;
                            x.WriteLine("25" + //001 Tipo do Registro 2 001 002 N Conteúdo '25 '
                                    (documento.BaseIPI.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //002 Base de Cálculo 1 12 003-014 V
                                    (aliquotaIPI.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(4, '0')) + //003 Alíquota 1 4 015-018 V
                                    (documento.ValorTotalIPI.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //004 Valor ICMS 1 12 019-030 V

                                    ("0".PadLeft(12, '0')) + //005 Isentas 12 032 043 V
                                    ("0".PadLeft(12, '0')) + //006 Outras 12 044 055 V
                                    (documento.ValorTotalIPI.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //007 Valor Total do IPI 12 056 067 V Nas Entrada

                                    (" ".PadRight(189, ' '))//008 Espaços 189 068 256 A Livre
                            );
                        }

                        //27
                        if (documento.ValorTotalICMSST > 0)
                        {
                            decimal aliquotaST = 0;
                            if (documento.ValorTotalICMSST > 0 && documento.BaseCalculoICMSST > 0)
                                aliquotaST = Math.Round((documento.ValorTotalICMSST / documento.BaseCalculoICMSST) * 100);
                            qtdRegistros++;
                            x.WriteLine("27" + //001 Tipo do Registro 2 001 002 N Conteúdo '27'
                                    (documento.BaseCalculoICMSST.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //002 Base de Cálculo 1 12 003-014 V
                                    (aliquotaST.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(4, '0')) + //003 Alíquota 1 4 015-018 V
                                    (documento.ValorTotalICMSST.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //004 Valor ICMS 1 12 019-030 V
                                    ("0".PadLeft(1, '0')) + //005 Tipo Lançamento (Entradas) 1 031 031 N TABELA H
                                    ("2".PadLeft(1, '2')) + //006 Apuração (Entradas) 1 032 032 N Quando for informado '1'
                                    ("1".PadLeft(1, '1')) + //007 Operação (Saídas) 1 033 033 N TABELA J
                                    (" ".PadRight(223, ' '))//008 Espaços 189 068 256 A Livre
                            );
                        }

                        if (documento.Modelo?.Abreviacao == "NFS-e" || documento.Modelo?.Abreviacao == "NFS")
                        {
                            foreach (var item in documento.Itens)
                            {
                                qtdRegistros++;
                                x.WriteLine("33" + //001 Tipo do Registro 2 001 002 N Conteúdo '33'
                                            Utilidades.String.Left((item.Produto?.Descricao ?? "").PadRight(45, ' '), 45) + //002 Descrição do serviço 45 003 047 A
                                            (item.ValorTotal.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //003 Valor do servico 12 048 059 N.
                                            (item.Desconto.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //004 Valor do desconto 12 060 071 N
                                            ("0".PadLeft(12, '0')) + //005 Valor do material 12 072 083 N
                                            ("0".PadLeft(12, '0')) + //006 Base do ISS 12 084 095 N Somente nas Saídas
                                            ("0".PadLeft(4, '0')) + //007 Alíquota do ISS 4 096 099 N Somente nas Saídas
                                            ("0".PadLeft(12, '0')) + //008 Valor do ISS 12 100 111 N Somente nas Saíd
                                            ("0".PadLeft(12, '0')) + //009 Base retenção INSS 12 112 123 N
                                            ("0".PadLeft(4, '0')) + //010 Alíquota retenção INSS 4 124 127 N
                                            ("0".PadLeft(12, '0')) + //011 Valor retenção INSS 12 128 139 N
                                            (" ".PadRight(8, ' ')) + //012 Código da Atividade 08 140 147 A
                                            Utilidades.String.Left(((string.IsNullOrWhiteSpace(item.CSTICMS) ? "090" : item.CSTICMS) ?? "090").PadLeft(3, '0'), 3) + //013 CST 03 148 150 N
                                            Utilidades.String.Left(Utilidades.String.OnlyNumbers(item.Produto == null ? item.Codigo.ToString("n0") : item.Produto.Codigo.ToString("n0")).PadRight(14, ' '), 14) + //014 Código Próprio 14 151 164 A Código interno utilizado
                                            ((item.CSTCOFINS == "70" || item.CSTIPI == "70" || item.CSTCOFINS == "070" || item.CSTIPI == "070") ? '0' : (item.CSTCOFINS == "50" || item.CSTIPI == "50" || item.CSTCOFINS == "050" || item.CSTIPI == "050") ? '1' : ' ') + //15 Base Cálculo Crédito 1 165 165 N 0 = Não se Aplica
                                            (" ".PadRight(7, ' ')) + //016 Desmembramento Contábil 7 166 172 N Informar o campo dos re
                                            (" ".PadRight(4, ' ')) + //017 Código Serviço LC 116/03 4 173 176 N Observar a Lista de
                                            (" ".PadRight(4, ' ')) + //018 Complemento cód. atividade 4 177 180
                                            (" ".PadRight(60, ' ')) + //019 Complemento da descrição 60 181 240
                                            (" ".PadRight(16, ' '))//020 Espaços 16 241 256 A Livre
                                    );
                            }
                        }

                        //60
                        if (documento.Modelo?.Abreviacao != "NFS-e" && documento.Modelo?.Abreviacao != "NFS")
                        {
                            qtdRegistros++;
                            x.WriteLine("60" + //001 Tipo do Registro 2 001 002 N Conteúdo '60'
                                        (" ".PadRight(2, ' ')) + //02 Modelo Documento 2 003 004 A Deve ser:
                                        (" ".PadRight(2, ' ')) + //003 Situação do Documento 2 005 006 N Deve ser:
                                        (" ".PadRight(9, ' ')) + //004 Número do Documento (COO) 9 007 015 N
                                        (" ".PadRight(8, ' ')) + //005 Data da Emissão 8 016 023 D Somente quando fo
                                        (" ".PadRight(12, ' ')) + //006 Valor Total 12 024 035 V
                                        (" ".PadRight(14, ' ')) + //007 CPF/CNPJ Adquirente 14 036 049 I
                                        (" ".PadRight(70, ' ')) + //008 Nome Adquirente 70 050 119 A
                                        (" ".PadRight(44, ' ')) + //009 Chave Eletrônica 44 120 163 A
                                        (" ".PadRight(9, ' ')) + //010 Contador de Cupom Fiscal 9 164 173 N
                                        (" ".PadRight(84, ' '))//011 Espaços 84 174 256 A Brancos
                                );
                            int qtdSeqInten = 0;
                            foreach (var item in documento.Itens)
                            {
                                if (item.CFOP != null && item.CFOP.Tipo == Dominio.Enumeradores.TipoCFOP.Entrada)
                                {
                                    qtdSeqInten++;
                                    qtdRegistros++;
                                    x.WriteLine("61" + //001 Tipo do Registro 2 001 002 N Conteúdo '61'
                                                Utilidades.String.Left(Utilidades.String.OnlyNumbers(item.Produto == null ? item.Codigo.ToString("n0") : string.IsNullOrWhiteSpace(item.Produto.CodigoProduto) ? item.Produto.Codigo.ToString("n0") : item.Produto.CodigoProduto).PadRight(14, ' '), 14) + //002 Código Próprio 14 003 016 A
                                                (" ".PadRight(14, ' ')) + //003 Código Anterior do Item 14 017 030 A
                                                Utilidades.String.Left((item.Produto?.NCM?.Numero ?? "").PadRight(11, ' '), 11) + //004 NCM-Ex 11 031 041 N Informar o código do item
                                                Utilidades.String.Left((item.Produto?.Descricao ?? "").PadRight(50, ' '), 50) + //005 Descrição 50 042 091 A Se houver uma descriçã
                                                (" ".PadRight(30, ' ')) + //006 Descrição Complementar 30 092 121 A Complemento do campo '005'
                                                Utilidades.String.Left(UnidadeDeMedidaHelper.ObterSigla(item.UnidadeMedida).PadRight(10, ' '), 10) + //007 Unidade de Medida no Documento 10 122 131 A
                                                Utilidades.String.Left(UnidadeDeMedidaHelper.ObterDescricao(item.UnidadeMedida).PadRight(20, ' '), 20) + //08 Descrição da Unidade de Medida 20 132 151 A Descrição completa da
                                                Utilidades.String.Left(UnidadeDeMedidaHelper.ObterSigla(item.UnidadeMedida).PadRight(10, ' '), 10) + //009 Unidade de Medida nos Estoques 10 152 161 A Quando a unidade abreviada
                                                Utilidades.String.Left(UnidadeDeMedidaHelper.ObterDescricao(item.UnidadeMedida).PadRight(20, ' '), 20) + //010 Descrição da Unidade de Medida 20 162 181 A Descrição completa do
                                                ("0".PadLeft(15, '0')) + //011 Fator de Conversão 15 182 196 V6 Quando as unidades forem
                                                (item.Produto == null ? "P" : item.Produto.CategoriaProduto == CategoriaProduto.Servicos ? "S" : "P") + //012 Produto/Serviço 1 197 197 A P = Produto S = Serviço.
                                                Utilidades.String.Left((item.Produto?.CodigoANP ?? "").PadRight(9, ' '), 9) + //013 Código Produto Tabela ANP 9 198 206 A Somente para contribuintes
                                                Utilidades.String.Left((item.Produto?.CodigoBarrasEAN ?? "").PadRight(14, ' '), 14) + //014 Código de Barras 14 207 220 A Representação alfanumérica
                                                (item.Produto == null ? "99" : CategoriaProdutoHelper.ObterSigla(item.Produto.CategoriaProduto)) + //015 Tipo do Item 2 221 222 N Deve ser:
                                                (item.Produto == null ? "99" : GeneroProdutoHelper.ObterSigla(item.Produto.GeneroProduto).PadRight(2, ' ')) + //016 Código do Gênero do Item 2 223 224 N Observar a Lista de
                                                (" ".PadRight(4, ' ')) + //017 Código Serviço LC 116/03 4 225 228 N Observar a Lista de
                                                (item.AliquotaICMS.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(4, '0')) + //018 Alíquota Interna ICMS 4 229 232 V Alíquota normal de ICMS
                                                Utilidades.String.Left((item.CSTPIS ?? "").PadRight(2, ' '), 2) + //019 CST Pis/Cofins 2 233 234 N Opcional. Obrigatório
                                                ("0".PadLeft(1, '0')) + //020 Tabela PIS/COFINS 1 235 235 N 0 = Não se aplica
                                                (" ".PadRight(3, ' ')) + //021 Item Tabela PIS/COFINS 3 236 238 N Códigos encontrados nas
                                                Utilidades.String.Left((item.Produto?.CEST?.Numero ?? "").PadRight(7, ' '), 7) + //22 CEST-Cód.Especif.de ST 7 239 245 N
                                                (Utilidades.String.OnlyNumbers(qtdSeqInten.ToString("n0")).PadLeft(3, '0')) + //023 Item 3 246 248 N Número sequencial do item
                                                (" ".PadRight(8, ' '))//024 Espaços 8 249 256 A Brancos
                                        );

                                    //qtdRegistros++;
                                    //x.WriteLine("61A" + //001 Tipo do Registro 2 001 002 N Conteúdo '61'
                                    //            Utilidades.String.Left(Utilidades.String.OnlyNumbers(item.Produto == null ? item.Codigo.ToString("n0") : item.Produto.Codigo.ToString("n0")).PadRight(60, ' '), 60) + //002 Código Próprio 14 003 016 A
                                    //            (" ".PadRight(60, ' ')) + //003 Código Anterior do Item 14 017 030 A
                                    //            Utilidades.String.Left((item.Produto?.Descricao ?? "").PadRight(120, ' '), 120) + //004 Descrição 120 124 243 A Descrição do Produto/Serviço.
                                    //            (" ".PadRight(13, ' '))//005 Espaços 13 244 256 A Brancos.
                                    //    );

                                    qtdRegistros++;
                                    x.WriteLine("62" + //001 Tipo do Registro 2 001 002 N Conteúdo '62'
                                                (item.Quantidade.ToString("n5").Replace(".", "").Replace(",", "").PadLeft(13, '0')) + //002 Quantidade 13 003 015 Q5 Quantidade do Item com até
                                                ("0".PadLeft(13, '0')) + //003 Quantidade Cancelada 13 016 028 Q5 Quantidade cancelada no
                                                (item.ValorTotal.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //004 Valor Total do Item 12 029 040 V Quantidade x Unidade.
                                                (item.Desconto.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //005 Desconto 12 041 052 V Desconto comercial
                                                ("S".PadLeft(1, 'S')) + //006 Movimentação Física 1 053 053 A S = Sim N= Não
                                                Utilidades.String.Left(((string.IsNullOrWhiteSpace(item.CSTICMS) ? "090" : item.CSTICMS) ?? "090").PadRight(3, ' '), 3) + //007 CST-ICMS 3 054 056 N Código de Situação
                                                Utilidades.String.Left((Utilidades.String.OnlyNumbers(item.CFOP?.CodigoCFOP.ToString("n0")) ?? "").PadRight(4, ' '), 4) + //008 CFOP 4 057 060 N Código Fiscal de Operações
                                                Utilidades.String.Left((item.NaturezaOperacao?.Descricao ?? "").PadRight(25, ' '), 25) + //008 CFOP 4 057 060 N Código Fiscal de Operações
                                                (item.BaseCalculoICMS.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //010 Base de Cálculo do ICMS 12 086 097 V
                                                (item.AliquotaICMS.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(4, '0')) + //011 Alíquota do ICMS 4 098 101 V
                                                (item.ValorICMS.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //010 Base de Cálculo do ICMS 12 086 097 V
                                                (item.BaseCalculoICMSST.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //013 Base de Cálculo ICMS-ST 12 114 125 V
                                                (item.AliquotaICMSST.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(4, '0')) + //14 Alíquota ICMS-ST 4 126 129 V
                                                (item.ValorICMSST.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //015 Valor ICMS-ST 12 130 141 V
                                                ("0".PadLeft(1, '0')) + //016 Período Apuração IPI 1 142 142 N 0 = Mensal 1 = Decendial
                                                Utilidades.String.Left((item.CSTIPI ?? "").PadRight(2, ' '), 2) + //017 CST-IPI 2 143 144 N Código de Situação
                                                Utilidades.String.Left((item.Produto?.CodigoEnquadramentoIPI ?? "").PadRight(3, ' '), 3) + //018 Código Enquadramento IPI 3 145 147 N Código de Enquadramento
                                                (item.BaseCalculoIPI.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //019 Base de Cálculo do IPI 12 148 159 V
                                                (item.AliquotaIPI.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(5, '0')) + //020 Alíquota do IPI 5 160 164 V
                                                (" ".PadLeft(6, ' ')) + //021 Código Selo Controle IPI 6 165 170 A Quando o item está sujeito
                                                ("0".PadLeft(9, '0')) + //022 Quantidade de selos 9 171 179 N
                                                ("0".PadLeft(5, '0')) + //023 Classe Enquadramento IPI 5 180 184 N Para produtos que o IPI
                                                ("0".PadLeft(8, '0')) + //024 Valor por unidade padrão 8 185 192 V Produtos IPI por unidade.
                                                ("0".PadLeft(8, '0')) + //025 Quantidade total produtos
                                                (item.ValorIPI.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(12, '0')) + //026 IPI do Documento 12 201 212 V
                                                ("0".PadLeft(4, '0')) + //027 Enquadramento PIS 4 213 216 N '0000
                                                ("0".PadLeft(4, '0')) + //028 Enquadramento COFINS 4 217 220 N Idem ao PIS.
                                                ("0".PadLeft(7, '0')) + //029 Desmembramento Contábil 7 221 227 N 
                                                (" ".PadLeft(4, ' ')) + //030 Código Classif.Energia 4 228 231 N Conforme Tabela do Manual
                                                (" ".PadLeft(1, ' ')) + //031 Tipo Receita Energia/Telec. 1 232 232 N Conforme Manual.
                                                (" ".PadLeft(14, ' ')) + //032 CNPJ Participante 14 233 246 N Quando o campo 31 indicar
                                                ("0".PadLeft(10, '0')) //033 Comunicação Telecomun. 10 247 256 V Valor não tributado em
                                        );
                                }
                            }
                        }
                    }
                    qtdRegistros++;
                    //90
                    x.WriteLine("90" + //001 Tipo do Registro 2 001 002 N Conteúdo '90'
                                (Utilidades.String.OnlyNumbers(qtdRegistros.ToString("n0")).PadLeft(6, '0')) + //002 Quantidade Total de Registros Inclusive este 6 003 008 N
                                (" ".PadRight(248, ' '))//008 Espaços 189 068 256 A Livre
                        );

                    x.Flush();

                    return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ImpFormPH_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".IMP"));
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilQuestor(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal, tipoMovimento, null, codigoTipoMovimentoArquivoContabil, codigoPlanoConta);
                    if (movimentoFinanceiro.Count() > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro mov in movimentoFinanceiro)
                        {
                            observacao = (string.IsNullOrWhiteSpace(mov.Observacao) ? "" : mov.Observacao.Length > 64 ? mov.Observacao.Substring(0, 63).Replace(";", "") : mov.Observacao.Replace(";", ""));
                            bool movimentoReversao = false;
                            if (observacao.ToLower().Contains("reversao") || observacao.ToLower().Contains("reversão"))
                                movimentoReversao = true;

                            if (!string.IsNullOrWhiteSpace(observacao))
                                observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                            string contaDebito = "";
                            string contaCredito = "";

                            string nomeContaDebito = " ".PadRight(40, ' ');
                            string identificadorContaDebito = " ".PadRight(20, ' ');
                            string tipoContaDebito = " ";
                            string tipoIdentificadorContaDebito = " ";

                            string nomeContaCredito = " ".PadRight(40, ' ');
                            string identificadorContaCredito = " ".PadRight(20, ' ');
                            string tipoContaCredito = " ";
                            string tipoIdentificadorContaCredito = " ";

                            if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasReceber || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                            {
                                if (!movimentoReversao)
                                {
                                    contaDebito = "99999999";
                                    contaCredito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    if (mov.Pessoa != null)
                                    {
                                        nomeContaDebito = mov.Pessoa.Nome.PadRight(40, ' ');
                                        identificadorContaDebito = mov.Pessoa.CPF_CNPJ_SemFormato.PadRight(20, ' ');
                                        tipoContaDebito = "1";
                                        tipoIdentificadorContaDebito = mov.Pessoa.Tipo == "F" ? "2" : "1";
                                    }
                                    else
                                    {
                                        nomeContaDebito = " ".PadRight(40, ' ');
                                        identificadorContaDebito = " ".PadRight(20, ' ');
                                        tipoContaDebito = " ";
                                        tipoIdentificadorContaDebito = " ";
                                    }
                                }
                                else
                                {
                                    contaDebito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                    contaCredito = "99999999";

                                    if (mov.Pessoa != null)
                                    {
                                        nomeContaCredito = mov.Pessoa.Nome.PadRight(40, ' ');
                                        identificadorContaCredito = mov.Pessoa.CPF_CNPJ_SemFormato.PadRight(20, ' ');
                                        tipoContaCredito = "2";
                                        tipoIdentificadorContaCredito = mov.Pessoa.Tipo == "F" ? "2" : "1";
                                    }
                                    else
                                    {
                                        nomeContaCredito = " ".PadRight(40, ' ');
                                        identificadorContaCredito = " ".PadRight(20, ' ');
                                        tipoContaCredito = " ";
                                        tipoIdentificadorContaCredito = " ";
                                    }
                                }
                            }
                            else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasPagar || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                            {
                                if (!movimentoReversao)
                                {
                                    contaCredito = "99999999";
                                    contaDebito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";

                                    if (mov.Pessoa != null)
                                    {
                                        nomeContaCredito = mov.Pessoa.Nome.PadRight(40, ' ');
                                        identificadorContaCredito = mov.Pessoa.CPF_CNPJ_SemFormato.PadRight(20, ' ');
                                        tipoContaCredito = "2";
                                        tipoIdentificadorContaCredito = mov.Pessoa.Tipo == "F" ? "2" : "1";
                                    }
                                    else
                                    {
                                        nomeContaCredito = " ".PadRight(40, ' ');
                                        identificadorContaCredito = " ".PadRight(20, ' ');
                                        tipoContaCredito = " ";
                                        tipoIdentificadorContaCredito = " ";
                                    }
                                }
                                else
                                {
                                    contaCredito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    contaDebito = "99999999";

                                    if (mov.Pessoa != null)
                                    {
                                        nomeContaDebito = mov.Pessoa.Nome.PadRight(40, ' ');
                                        identificadorContaDebito = mov.Pessoa.CPF_CNPJ_SemFormato.PadRight(20, ' ');
                                        tipoContaDebito = "1";
                                        tipoIdentificadorContaDebito = mov.Pessoa.Tipo == "F" ? "2" : "1";
                                    }
                                    else
                                    {
                                        nomeContaDebito = " ".PadRight(40, ' ');
                                        identificadorContaDebito = " ".PadRight(20, ' ');
                                        tipoContaDebito = " ";
                                        tipoIdentificadorContaDebito = " ";
                                    }
                                }
                            }
                            else
                            {
                                contaCredito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                contaDebito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                            }

                            contaDebito = contaDebito.PadLeft(8, '0');
                            contaCredito = contaCredito.PadLeft(8, '0');

                            x.WriteLine(
                                    (mov.DataMovimento.ToString("ddMMyyyy")) + //Data                               8  001-008 Formato ddmmaaaaa.
                                    (Utilidades.String.Left(contaDebito, 8)) + //Conta Débito                       8  009-016 Numérico.
                                    (Utilidades.String.Left(contaCredito, 8)) + //Conta Crédito                      8  017-024 Numérico.
                                    (mov.Valor.ToString("n2").Replace(".", "").Replace(",", ".").PadLeft(17, '0')) + //Valor                             17  025-041 Numérico c/ponto e 2 dec.
                                    ("0".PadLeft(8, '0')) + //Histórico                          8  042-049 Numérico.
                                    (Utilidades.String.Left(observacao.PadRight(64, ' '), 64)) + //Complemento (Alternativo 264)     64  050-113#Alfa Numérico. 
                                    ("0".PadLeft(3, '0')) + //Débito-Estabelecimento             3  114-116 Numérico.
                                    ("0".PadLeft(3, '0')) + //Crédito-Estabelecimento            3  117-119 Numérico.
                                    ("0".PadLeft(3, '0')) + //Débito-Centro de Custos            3  120-122 Numérico. (Até 3 dígitos)
                                    ("0".PadLeft(3, '0')) + //Crédito-Centro de Custos           3  123-125 Numérico. (Até 3 dígitos)
                                    ("9".PadLeft(8, '9')) + //Contrapartida                      8  126-133 Numérico.
                                    ("0".PadLeft(6, '0')) + //Conjunto de Lançamentos            6  134-139 Numérico.

                                    (Utilidades.String.Left(nomeContaDebito, 40)) + //* Nome Conta Débito               40  140-179 AlfaNumérico.
                                    (Utilidades.String.Left(identificadorContaDebito, 20)) + //* Identificador Conta Débito      20  180-199 AlfaNumérico.
                                    (tipoContaDebito) + //* Tipo da Conta Débito             1  200-200 Numérico.
                                    (tipoIdentificadorContaDebito) + //* Tipo Identificador Conta Débito  1  201-201 Numérico.

                                    (Utilidades.String.Left(nomeContaCredito, 40)) + //* Nome Conta Crédito              40  202-241 AlfaNumérico.
                                    (Utilidades.String.Left(identificadorContaCredito, 20)) + //* Identificador Conta Crédito     20  242-261 AlfaNumérico.
                                    (tipoContaCredito) + //* Tipo da Conta Crédito            1  262-262 Numérico.
                                    (tipoIdentificadorContaCredito) + //* Tipo Identificador C.Crédito     1  263-263 Numérico.

                                    (" ".PadLeft(40, ' ')) + //* Nome Conta Contrapartida        40  264-303 AlfaNumérico.
                                    (" ".PadLeft(20, ' ')) + //* Identificador C.Contrapartida   20  304-323 AlfaNumérico.
                                    ("0") + //* Tipo da Conta Contrapartida      1  324-324 Numérico.
                                    ("0") + //* Tipo Ident. C.Contrapartida      1  325-325 Numérico.
                                    (" ".PadLeft(12, ' ')) + //Arquivamento                      12  326-337 AlfaNumérico.
                                    ("0".PadLeft(5, '0')) + //Débito-Centro de Custos            5  338-342 Numérico.(Quando exceder 3
                                    ("0".PadLeft(5, '0')) + //Crédito-Centro de Custos           5  343-347 Numérico. dígitos)
                                    (" ".PadLeft(51, ' '))  //Reservado                         51  348-398 Espaços (ASCII 32).                         
                                );
                        }
                    }
                }
                x.Flush();

                return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoPH_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo PH.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoAlterdata()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                if (dataFinal != DateTime.MinValue)
                    dataFinal = dataFinal.AddHours(23).AddMinutes(59).AddSeconds(59);

                int codigoTipoMovimentoArquivoContabil = Request.GetIntParam("TipoMovimentoArquivoContabil");

                TipoMovimentoArquivoContabilQuestor tipoMovimento = TipoMovimentoArquivoContabilQuestor.ContasPagar;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);
                List<TipoMovimentoArquivoContabilQuestor> tiposMovimentos = new List<TipoMovimentoArquivoContabilQuestor>();
                if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.Todos)
                {
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.ContasReceber);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.ContasPagar);
                    //tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.DemaisMovimentos);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos);
                }
                else
                    tiposMovimentos.Add(tipoMovimento);

                bool gerarFormatoTXT = Request.GetBoolParam("GerarFormatoTXT");

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);

                MemoryStream arquivoINPUT = new MemoryStream();
                StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                string aspas = gerarFormatoTXT ? "\u0022" : string.Empty;

                x.WriteLine($"{aspas}lancto auto{aspas};{aspas}debito{aspas};{aspas}credito{aspas};{aspas}data{aspas};{aspas}valor{aspas};{aspas}cód histórico{aspas};{aspas}complemento historico{aspas};{aspas}Ccusto debito{aspas};{aspas}Ccusto credito{aspas};{aspas}NrDocumento{aspas};");

                if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.NFeEntrada)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentos = repDocumentoEntrada.BuscarDocumentoPorEmpresa(codigoEmpresa, dataInicial, dataFinal);
                    if (documentos.Count == 0)
                        return new JsonpResult(false, true, "Nenhum documento de entrada encontrado para o filtro selecionado.");

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento in documentos)
                    {
                        string observacao = $"NF { documento.Numero } { documento.Fornecedor.Nome }";
                        observacao += " Doc. " + documento.Numero.ToString();
                        observacao += " Cod. Titulo " + string.Join(", ", documento.Duplicatas.Select(o => o.CodigoTitulo));
                        observacao += " Pessoa: " + documento.Fornecedor.Nome;
                        observacao += " Referente duplicata n " + string.Join(", ", documento.Duplicatas.Select(o => o.Numero));

                        observacao = observacao.Replace(";", "");

                        if (!string.IsNullOrWhiteSpace(observacao))
                            observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                        string contaDebito = documento.TipoMovimento?.PlanoDeContaDebito.PlanoContabilidade ?? "";
                        string contaCredito = !string.IsNullOrWhiteSpace(documento.Fornecedor.ContaFornecedorEBS) ? documento.Fornecedor.ContaFornecedorEBS : (documento.TipoMovimento?.PlanoDeContaCredito.PlanoContabilidade ?? "");

                        x.WriteLine(aspas + aspas + ";" + //lancto auto
                                    aspas + contaDebito + aspas + ";" + //debito
                                    aspas + contaCredito + aspas + ";" + //credito
                                    aspas + documento.DataEntrada.ToString("dd/MM/yyyy") + aspas + ";" + //data
                                    aspas + documento.ValorTotal.ToString("n2").Replace(".", "") + aspas + ";" + //valor
                                    aspas + "100" + aspas + ";" + //cód histórico
                                    aspas + observacao + aspas + ";" +//complemento historico
                                    aspas + aspas + ";" + //Ccusto debito
                                    aspas + aspas + ";" + //Ccusto credito
                                    aspas + aspas + ";"  //NrDocumento
                                );
                    }
                }
                else
                {
                    foreach (TipoMovimentoArquivoContabilQuestor tipo in tiposMovimentos)
                    {
                        List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilQuestor(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal, tipo, null, codigoTipoMovimentoArquivoContabil);
                        if (movimentoFinanceiro.Count() > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro mov in movimentoFinanceiro)
                            {
                                string observacao = (string.IsNullOrWhiteSpace(mov.Observacao) ? "" : mov.Observacao.Length > 300 ? mov.Observacao.Substring(0, 299).Replace(";", "") : mov.Observacao.Replace(";", ""));
                                if (mov.Titulo != null && mov.Titulo.DocumentoEntradaGuia != null)
                                    observacao += " Doc. " + mov.Titulo.DocumentoEntradaGuia.DocumentoEntrada?.Numero.ToString("n0") ?? string.Empty;
                                else if (mov.Titulo != null && mov.Titulo.DuplicataDocumentoEntrada != null)
                                    observacao += " Doc. " + mov.Titulo.DuplicataDocumentoEntrada.DocumentoEntrada?.Numero.ToString("n0") ?? string.Empty;
                                else if (mov.Titulo != null && !string.IsNullOrWhiteSpace(mov.Titulo.NumeroDocumentoTituloOriginal) && !observacao.Contains(mov.Titulo.NumeroDocumentoTituloOriginal))
                                    observacao += " Doc. " + mov.Titulo.NumeroDocumentoTituloOriginal;

                                if (mov.Titulo != null)
                                {
                                    observacao += " Cod. Titulo: " + Utilidades.String.OnlyNumbers(mov.Titulo.Codigo.ToString("n0"));
                                    observacao += " Pessoa: " + (mov.Titulo.Pessoa.Nome ?? "");
                                    observacao += " Obs Tit.: " + (mov.Titulo.Observacao ?? "");
                                }

                                observacao = observacao.Replace(";", "");
                                //if (observacao.Length > 300)
                                //    observacao = observacao.Substring(0, 299).Replace(";", "");

                                bool movimentoReversao = false;
                                if (observacao.ToLower().Contains("reversao") || observacao.ToLower().Contains("reversão"))
                                    movimentoReversao = true;

                                if (!string.IsNullOrWhiteSpace(observacao))
                                    observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                                bool movimentoDesconto = observacao.ToLower().Contains("desconto");

                                string contaDebito = "";
                                string contaCredito = "";
                                if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasReceber || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                                {
                                    if (!movimentoReversao)
                                    {
                                        if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                                        {
                                            if (movimentoDesconto)
                                            {
                                                contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                                contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                            }
                                            else
                                            {
                                                contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                                contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                            }
                                        }
                                        else
                                        {
                                            contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                            contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                        }
                                    }
                                    else
                                    {
                                        if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                                        {
                                            if (movimentoDesconto)
                                            {
                                                contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                                contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                            }
                                            else
                                            {
                                                contaCredito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                                contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                            }
                                        }
                                        else
                                        {
                                            contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                            contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                        }
                                    }
                                }
                                else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasPagar || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                                {
                                    if (!movimentoReversao)
                                    {
                                        if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                                        {
                                            if (movimentoDesconto)
                                            {
                                                contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                                contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                            }
                                            else
                                            {
                                                contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                                contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                            }
                                        }
                                        else
                                        {
                                            contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                            contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                        }
                                    }
                                    else
                                    {
                                        if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                                        {
                                            if (movimentoDesconto)
                                            {
                                                contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                                contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                            }
                                            else
                                            {
                                                contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                                contaDebito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                            }
                                        }
                                        else
                                        {
                                            contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                            contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                        }
                                    }
                                }
                                else
                                {
                                    contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                    contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                }

                                x.WriteLine(aspas + aspas + ";" + //lancto auto
                                    aspas + contaCredito + aspas + ";" + //debito invertido as contas no dia 21/01 tarefa #16515
                                    aspas + contaDebito + aspas + ";" + //credito
                                    aspas + mov.DataMovimento.ToString("dd/MM/yyyy") + aspas + ";" + //data
                                    aspas + mov.Valor.ToString("n2").Replace(".", "") + aspas + ";" + //valor
                                    aspas + "100" + aspas + ";" + //cód histórico
                                    aspas + observacao + aspas + ";" +//complemento historico
                                    aspas + aspas + ";" + //Ccusto debito
                                    aspas + aspas + ";" + //Ccusto credito
                                    aspas + aspas + ";"  //NrDocumento
                                );
                            }
                        }
                    }
                }

                x.Flush();
                return Arquivo(arquivoINPUT.ToArray(), "text/csv", string.Concat("ArquivoAlterdata_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), gerarFormatoTXT ? ".txt" : ".csv"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Alterdata.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoQuestor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                DateTime.TryParse(Request.Params("DataEntradaInicial"), out DateTime dataEntradaInicial);
                DateTime.TryParse(Request.Params("DataEntradaFinal"), out DateTime dataEntradaFinal);
                if (dataFinal != DateTime.MinValue)
                    dataFinal = dataFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                if (dataEntradaFinal != DateTime.MinValue)
                    dataEntradaFinal = dataEntradaFinal.AddHours(23).AddMinutes(59).AddSeconds(59);

                int codigoEmpresaSelecionada = Request.GetIntParam("Empresa");
                int codigoTipoMovimentoArquivoContabil = Request.GetIntParam("TipoMovimentoArquivoContabil");
                int codigoPlanoConta = Request.GetIntParam("PlanoConta");

                List<int> codigosModeloDocumentoFiscal = null;
                string codigosModelos = Request.Params("ModeloDocumentoFiscal");
                if (!string.IsNullOrEmpty(codigosModelos))
                    codigosModeloDocumentoFiscal = JsonConvert.DeserializeObject<List<int>>(codigosModelos);

                List<int> codigosTipoMovimento = null;
                string codigosTipos = Request.Params("TiposMovimentos");
                if (!string.IsNullOrEmpty(codigosTipos))
                    codigosTipoMovimento = JsonConvert.DeserializeObject<List<int>>(codigosTipos);

                bool.TryParse(Request.Params("ExtensaoCFOP"), out bool extensaoCFOP);

                TipoMovimentoArquivoContabilQuestor tipoMovimento = TipoMovimentoArquivoContabilQuestor.ContasPagar;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);
                List<TipoMovimentoArquivoContabilQuestor> tiposMovimentos = new List<TipoMovimentoArquivoContabilQuestor>();
                if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.Todos)
                {
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.ContasReceber);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.ContasPagar);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.DemaisMovimentos);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos);
                }
                else
                    tiposMovimentos.Add(tipoMovimento);

                TipoArquivoContabilQuestor tipoArquivo = Request.GetEnumParam<TipoArquivoContabilQuestor>("TipoArquivo");

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa > 0 ? codigoEmpresa : codigoEmpresaSelecionada);

                if (tipoArquivo == TipoArquivoContabilQuestor.Contabil)
                {
                    byte[] arquivo = ObterByteTipoArquivoContabilQuestor(dataInicial, dataFinal, codigosTipoMovimento, codigoTipoMovimentoArquivoContabil, codigoPlanoConta, codigoEmpresa, tipoAmbiente, tiposMovimentos, tipoMovimento, empresa, unitOfWork);

                    return Arquivo(arquivo, "text/txt", string.Concat("ArquivoQuestor_TipoContabil_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                }
                else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.NFSeEntrada)
                {
                    byte[] arquivo = ObterByteTipoArquivoPadraoNFSeEntradaQuestor(dataInicial, dataFinal, dataEntradaInicial, dataEntradaFinal, codigosModeloDocumentoFiscal, codigoEmpresa, codigoEmpresaSelecionada, extensaoCFOP, unitOfWork);

                    return Arquivo(arquivo, "text/txt", string.Concat("ArquivoQuestor_NFSeEntrada_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                }
                else
                {
                    byte[] arquivo = ObterByteTipoArquivoPadraoQuestor(dataInicial, dataFinal, codigosTipoMovimento, codigoTipoMovimentoArquivoContabil, codigoEmpresa, tipoAmbiente, tiposMovimentos, tipoMovimento, empresa, unitOfWork);

                    return Arquivo(arquivo, "text/txt", string.Concat("ArquivoQuestor_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                }
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo Questor.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoESocial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                int codigoEmpresa = Request.GetIntParam("Empresa");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                MemoryStream arquivoINPUT = new MemoryStream();
                StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> contratos = repContratoFrete.BuscarContratosESocial(codigoEmpresa, dataInicial, dataFinal);
                if (contratos == null || contratos.Count() == 0)
                    return new JsonpResult(false, true, "Nenhum contrato de autônomo encontrado no período de quitação selecionado.");

                contratos = contratos.Distinct().ToList();

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                //cabeçalho 
                x.WriteLine("AUTONONOS" + dataFinal.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss") + "00000");
                DateTime dataCompetencia = new DateTime(dataFinal.Year, dataFinal.Month, DateTime.DaysInMonth(dataFinal.Year, dataFinal.Month));
                foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato in contratos)
                {
                    dataCompetencia = new DateTime(contrato.DataEmissaoContrato.Year, contrato.DataEmissaoContrato.Month, DateTime.DaysInMonth(contrato.DataEmissaoContrato.Year, contrato.DataEmissaoContrato.Month));

                    string dadosPadroes = Utilidades.String.RemoveAccents(contrato.TransportadorTerceiro.CPF_CNPJ_SemFormato.PadLeft(11, '0').Substring(0, 11) + //Número do CPF 1 a 11
                        (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(contrato.TransportadorTerceiro.PISPASEP)) ? Utilidades.String.OnlyNumbers(contrato.TransportadorTerceiro.PISPASEP).PadLeft(11, '0').Substring(0, 11) : "".PadLeft(11, '0').Substring(0, 11)) + //NIS - Numero de inscrição do trabalhador 12 a 22
                        contrato.TransportadorTerceiro.Nome.PadRight(60, ' ').Substring(0, 60) + //Nome 23 a 82
                        (contrato.TransportadorTerceiro.DataNascimento.HasValue ? contrato.TransportadorTerceiro.DataNascimento.Value.ToString("dd/MM/yyyy") : "".PadRight(10, ' ')) + //Data Nascimento 83 a 92
                        "M" + //Sexo 93 a 93
                        empresa.CNPJ_SemFormato.PadLeft(15, '0').Substring(0, 15) + //CNPJ contratante 94 a 108
                        contrato.NumeroContrato.ToString("D").PadLeft(10, '0').Substring(0, 10) + //Numero da carta frete 109 a 118
                        contrato.DataEmissaoContrato.ToString("dd/MM/yyyy") + //Data de emissão 119 a 128
                        dataCompetencia.ToString("dd/MM/yyyy") //Data Competencia 129 a 138 
                        );

                    DateTime? dataPagamentoValor = repTitulo.BuscarDataPagamentoContratoFrete(contrato.Codigo, 2);
                    DateTime? dataPagamentoAdiantamento = repTitulo.BuscarDataPagamentoContratoFrete(contrato.Codigo, 1);

                    if (!dataPagamentoAdiantamento.HasValue)
                        dataPagamentoAdiantamento = repTitulo.BuscarDataVencimentoContratoFrete(contrato.Codigo, 1);

                    if (!dataPagamentoValor.HasValue)
                    {
                        dataPagamentoValor = repTitulo.BuscarDataVencimentoContratoFrete(contrato.Codigo, 2);
                        if (!dataPagamentoValor.HasValue)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contrato.Codigo);
                            if (cargaCIOT != null)
                                dataPagamentoValor = cargaCIOT.CIOT.DataEncerramento;
                        }
                    }

                    if (!dataPagamentoAdiantamento.HasValue)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contrato.Codigo);
                        if (cargaCIOT != null)
                            dataPagamentoAdiantamento = cargaCIOT.CIOT.DataEncerramento;
                    }

                    //Valor bruto da carta frete
                    if (dataPagamentoValor.HasValue && dataPagamentoValor.Value <= dataFinal && dataPagamentoValor.Value >= dataInicial)
                    {
                        x.WriteLine(dadosPadroes +
                            (dataPagamentoValor.HasValue ? dataPagamentoValor.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                            "Total carta frete".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                            Utilidades.String.OnlyNumbers(contrato.ValorFreteSubcontratacao.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                        );
                    }

                    //Desconto do adiantamento pago anterioemente
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Adiantamento pago".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(contrato.ValorAdiantamento.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Desconto de INSS efetuado na carta
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Valor INSS".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(contrato.ValorINSS.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Desconto de SENAT efetuado na carta
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Valor SENAT".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(contrato.ValorSENAT.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Desconto de SEST efetuado na carta
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Valor SEST".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(contrato.ValorSEST.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Desconto de IRRF efetuado no pagamento (saldo ou adiantamento)
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "IRRF adiant".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Líquido pago (saldo ou adiantamento)
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Liquido adiant".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(contrato.ValorAdiantamento.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Base de cálculo de IRRF (saldo ou adiantamento)
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Base Calculo IR adiant".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(contrato.BaseCalculoIRRF.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Base de cálculo de INSS
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Base Calculo INSS".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(contrato.BaseCalculoINSS.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Base de cálculo de SEST/SENAT descontado na carta
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Base Calculo SEST/SENAT".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers((contrato.BaseCalculoSEST > 0 ? contrato.BaseCalculoSEST : contrato.BaseCalculoSENAT).ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );

                    //Dedução de INSS utilizado no abatimento do cálculo de IR do pagamento (saldo ou adiantamento)
                    x.WriteLine(dadosPadroes +
                        (dataPagamentoAdiantamento.HasValue ? dataPagamentoAdiantamento.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                        "Deducao INSS adiant".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                        Utilidades.String.OnlyNumbers(contrato.ValorINSS.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                    );


                    //Desc adiantamento
                    if (dataPagamentoValor.HasValue && dataPagamentoValor.Value <= dataFinal && dataPagamentoValor.Value >= dataInicial)
                    {
                        x.WriteLine(dadosPadroes +
                            (dataPagamentoValor.HasValue ? dataPagamentoValor.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                            "Desc adiantamento".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                            Utilidades.String.OnlyNumbers(contrato.ValorAdiantamento.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                        );
                    }

                    //IRRF Desconto de IRRF efetuado no pagamento do saldo
                    if (dataPagamentoValor.HasValue && dataPagamentoValor.Value <= dataFinal && dataPagamentoValor.Value >= dataInicial)
                    {
                        x.WriteLine(dadosPadroes +
                            (dataPagamentoValor.HasValue ? dataPagamentoValor.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                            "IRRF".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                            Utilidades.String.OnlyNumbers(contrato.ValorIRRF.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                        );
                    }

                    //Líquido pago no saldo
                    if (dataPagamentoValor.HasValue && dataPagamentoValor.Value <= dataFinal && dataPagamentoValor.Value >= dataInicial)
                    {
                        x.WriteLine(dadosPadroes +
                            (dataPagamentoValor.HasValue ? dataPagamentoValor.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                            "Liquido".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                            Utilidades.String.OnlyNumbers(contrato.ValorSaldo.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                        );
                    }

                    //Base Calculo IR Base de cálculo de IRRF do saldo
                    if (dataPagamentoValor.HasValue && dataPagamentoValor.Value <= dataFinal && dataPagamentoValor.Value >= dataInicial)
                    {
                        x.WriteLine(dadosPadroes +
                            (dataPagamentoValor.HasValue ? dataPagamentoValor.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                            "Base Calculo IR".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                            Utilidades.String.OnlyNumbers(contrato.BaseCalculoIRRF.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                        );
                    }

                    //Dedução INSS Dedução de INSS utilizado no abatimento do cálculo de IR do pagamento do saldo
                    if (dataPagamentoValor.HasValue && dataPagamentoValor.Value <= dataFinal && dataPagamentoValor.Value >= dataInicial)
                    {
                        x.WriteLine(dadosPadroes +
                            (dataPagamentoValor.HasValue ? dataPagamentoValor.Value.ToString("dd/MM/yyyy") : contrato.DataEncerramentoContrato.HasValue ? contrato.DataEncerramentoContrato.Value.ToString("dd/MM/yyyy") : contrato.DataEmissaoContrato.ToString("dd/MM/yyyy")) + //Data Pagamento 139 a 148
                            "Deducao INSS".PadRight(30, ' ').Substring(0, 30) + //Nome ou código rubrica 149 a 178
                            Utilidades.String.OnlyNumbers(contrato.ValorINSS.ToString("n2")).Replace(",", "").PadLeft(14, '0') //Valor rubrica 179 a 192
                        );
                    }

                }

                //rodape
                x.WriteLine(contratos.Count().ToString("n0").PadLeft(5, '0'));

                x.Flush();

                return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoESocial_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo E-Social.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoExactus()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);
                int empresaSelecionada = 0;
                int.TryParse(Request.Params("Empresa"), out empresaSelecionada);

                List<TipoMovimentoArquivoContabilQuestor> tiposMovimentos = new List<TipoMovimentoArquivoContabilQuestor>();
                tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.ContasReceber);
                tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.ContasPagar);
                tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.DemaisMovimentos);
                tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos);
                tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa > 0 ? codigoEmpresa : empresaSelecionada);

                MemoryStream arquivoINPUT = new MemoryStream();
                StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);
                string observacao = "";
                foreach (var tipo in tiposMovimentos)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilQuestor(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal, tipo, null);
                    if (movimentoFinanceiro.Count() > 0)
                    {
                        foreach (var mov in movimentoFinanceiro)
                        {
                            observacao = (string.IsNullOrWhiteSpace(mov.Observacao) ? "" : mov.Observacao.Length > 300 ? mov.Observacao.Substring(0, 299).Replace("|", "") : mov.Observacao.Replace("|", ""));
                            if (mov.Titulo != null && mov.Titulo.DocumentoEntradaGuia != null)
                                observacao += " Doc. " + mov.Titulo.DocumentoEntradaGuia.DocumentoEntrada?.Numero.ToString("n0") ?? string.Empty;
                            else if (mov.Titulo != null && mov.Titulo.DuplicataDocumentoEntrada != null)
                                observacao += " Doc. " + mov.Titulo.DuplicataDocumentoEntrada.DocumentoEntrada?.Numero.ToString("n0") ?? string.Empty;
                            else if (mov.Titulo != null && !string.IsNullOrWhiteSpace(mov.Titulo.NumeroDocumentoTituloOriginal))
                                observacao += " Doc. " + mov.Titulo.NumeroDocumentoTituloOriginal;

                            observacao = observacao.Replace("|", "");
                            if (observacao.Length > 100)
                                observacao = observacao.Substring(0, 99).Replace("|", "");

                            if (!string.IsNullOrWhiteSpace(observacao))
                                observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                            x.WriteLine(mov.DataMovimento.ToString("dd").PadLeft(2, '0').Substring(0, 2) + "|" + //DIA
                                mov.DataMovimento.ToString("MM").PadLeft(2, '0').Substring(0, 2) + "|" + //MÊS
                                ((tipo == TipoMovimentoArquivoContabilQuestor.ContasReceber ? mov.Pessoa != null && !string.IsNullOrEmpty(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaDebito?.PlanoContabilidade ?? "" : mov.PlanoDeContaDebito?.PlanoContabilidade ?? "")).PadLeft(7, '0').Substring(0, 7) + "|" + //CONTA DEVEDOR
                                ((tipo == TipoMovimentoArquivoContabilQuestor.ContasPagar ? mov.Pessoa != null && !string.IsNullOrEmpty(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaCredito?.PlanoContabilidade ?? "" : mov.PlanoDeContaCredito?.PlanoContabilidade ?? "")).PadLeft(7, '0').Substring(0, 7) + "|" + //CONTA CREDORA
                                 observacao.PadRight(100, ' ').Substring(0, 100) + "|" + //HISTÓRICO
                                 Utilidades.String.OnlyNumbers(mov.Valor.ToString("n2")).PadLeft(12, '0').Substring(0, 12) + "|" //VALOR
                            );
                        }
                    }
                }
                x.Flush();

                return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoExactus_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Exactus.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarArquivoMercante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();
                if (files.Count > 0)
                {
                    //unitOfWork.Start();

                    string msgRetorno = "";
                    Servicos.DTO.CustomFile file = files[0];
                    StreamReader streamReader = new StreamReader(file.InputStream);
                    if (!ProcessarRetornoArquivoMercante(out msgRetorno, streamReader, unitOfWork, Auditado))
                    {
                        //unitOfWork.Rollback();
                        return new JsonpResult(false, msgRetorno);
                    }

                    //unitOfWork.CommitChanges();

                    return new JsonpResult(true, "Importação do retorno do mercante foi realizada com sucesso.");
                }
                else
                {
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");
                }
            }
            catch (Exception ex)
            {
                //unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo do mercante. Erro: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarDocumentacaoMercantePendente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int empresaSelecionada = 0;
                int.TryParse(Request.Params("Empresa"), out empresaSelecionada);
                int codigoPedidoNavioViagem = 0;
                int.TryParse(Request.Params("PedidoNavioViagem"), out codigoPedidoNavioViagem);

                int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");

                int tipoCargaPerigosa = Request.GetIntParam("TipoCargaPerigosa");
                int tipoTransbordo = Request.GetIntParam("TipoTransbordo");

                bool comConhecimentosCancelados = Request.GetBoolParam("ComConhecimentosCancelados");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = repPorto.BuscarPorCodigo(codigoPortoOrigem);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = repTerminal.BuscarPorCodigo(codigoTerminalOrigem);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = repTerminal.BuscarPorCodigo(codigoTerminalDestino);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(empresaSelecionada);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoNavioViagem);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> terminais = repTerminal.BuscarTodosCadastros(codigoTerminalDestino);

                string retornoBookings = "";
                foreach (var terminal in terminais)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctes = null;
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasTransbordo = null;
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasTransbordoNavioCTe = null;
                    if (tipoTransbordo == 0 || tipoTransbordo == 2)
                    {
                        cargas = repCargaCTe.ConsultarAquaviarioMercantePendenteAutorizacao(codigoPedidoNavioViagem, terminalOrigem.Codigo, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2);
                        cargasTransbordo = repCargaCTe.ConsultarAquaviarioMercanteTransbordoPendenteAutorizacao(codigoPedidoNavioViagem, terminalOrigem.Codigo, 0, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminal.Codigo, 0);
                    }
                    else
                    {
                        cargasTransbordoNavioCTe = repCargaCTe.ConsultarAquaviarioMercanteTransbordoPendenteAutorizacao(0, 0, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminalOrigem.Codigo, codigoPedidoNavioViagem);
                        ctes = repCargaCTe.ConsultarCTesSemNumeroManifesto(0, 0, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminalOrigem.Codigo, codigoPedidoNavioViagem, comConhecimentosCancelados);
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPendentes = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                    if (cargas != null && cargas.Count > 0)
                        cargasPendentes.AddRange(cargas.Distinct().ToList());
                    if (cargasTransbordo != null && cargasTransbordo.Count > 0)
                        cargas.AddRange(cargasTransbordo.Distinct().ToList());
                    if (cargasTransbordoNavioCTe != null && cargasTransbordoNavioCTe.Count > 0)
                        cargas.AddRange(cargasTransbordoNavioCTe.Distinct().ToList());

                    if (cargas != null && cargas.Count > 0)
                    {
                        cargas = cargas.Distinct().ToList();
                        foreach (var carga in cargas)
                        {
                            retornoBookings += "Carga: " + carga.CodigoCargaEmbarcador + " -> Carga pendêntes de emissão para esta geração do arquivo Mercante, favor verifique no relatório de cargas/conhecimento.<br/>";
                        }
                    }
                    if (ctes != null && ctes.Count > 0)
                    {
                        ctes = ctes.Distinct().ToList();
                        foreach (var cte in ctes)
                        {
                            retornoBookings += "CT-e: " + cte.CTe.NumeroControle + " POL: " + cte.CTe.PortoOrigem?.Descricao + " POD: " + cte.CTe.PortoDestino?.Descricao + " VVD: " + cte.CTe.Viagem?.Descricao + " -> Não possui o número do manifesto importado anteriormente.<br/>";
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(retornoBookings))
                    return new JsonpResult(false, true, retornoBookings);
                else
                    return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Mercante.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoMercante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int empresaSelecionada = 0;
                int.TryParse(Request.Params("Empresa"), out empresaSelecionada);
                int codigoPedidoNavioViagem = 0;
                int.TryParse(Request.Params("PedidoNavioViagem"), out codigoPedidoNavioViagem);

                int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");

                int tipoCargaPerigosa = Request.GetIntParam("TipoCargaPerigosa");
                int tipoTransbordo = Request.GetIntParam("TipoTransbordo");

                bool comConhecimentosCancelados = Request.GetBoolParam("ComConhecimentosCancelados");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);


                MemoryStream arquivoINPUT = new MemoryStream();
                StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.ASCII);

                Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = repPorto.BuscarPorCodigo(codigoPortoOrigem);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = repTerminal.BuscarPorCodigo(codigoTerminalOrigem);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = repTerminal.BuscarPorCodigo(codigoTerminalDestino);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(empresaSelecionada);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoNavioViagem);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> terminais = repTerminal.BuscarTodosCadastros(codigoTerminalDestino);
                foreach (var terminal in terminais)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = null;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoNavioCTe = null;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoTerminal = null;

                    if (tipoTransbordo == 0 || tipoTransbordo == 2)
                    {
                        cargaCTes = repCargaCTe.ConsultarAquaviarioMercante(codigoPedidoNavioViagem, terminalOrigem.Codigo, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, comConhecimentosCancelados, tipoTransbordo == 2);
                        cargaCTesTransbordo = repCargaCTe.ConsultarAquaviarioMercanteTransbordo(codigoPedidoNavioViagem, terminalOrigem.Codigo, 0, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminal.Codigo, 0, comConhecimentosCancelados, tipoTransbordo == 2);
                    }
                    else
                        cargaCTesTransbordoNavioCTe = repCargaCTe.ConsultarAquaviarioMercanteTransbordo(0, 0, terminal.Codigo, tipoCargaPerigosa == 1, tipoCargaPerigosa == 2, terminalOrigem.Codigo, codigoPedidoNavioViagem, comConhecimentosCancelados, false);

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                    if (cargaCTes != null && cargaCTes.Count > 0)
                        ctes.AddRange(cargaCTes.Select(o => o.CTe).Distinct().ToList());
                    if (cargaCTesTransbordo != null && cargaCTesTransbordo.Count > 0)
                        ctes.AddRange(cargaCTesTransbordo.Select(o => o.CTe).Distinct().ToList());
                    if (cargaCTesTransbordoNavioCTe != null && cargaCTesTransbordoNavioCTe.Count > 0)
                        ctes.AddRange(cargaCTesTransbordoNavioCTe.Select(o => o.CTe).Distinct().ToList());
                    if (cargaCTesTransbordoTerminal != null && cargaCTesTransbordoTerminal.Count > 0)
                        ctes.AddRange(cargaCTesTransbordoTerminal.Select(o => o.CTe).Distinct().ToList());

                    if (ctes != null && ctes.Count > 0)
                        ctes = ctes.Distinct().ToList();

                    if (tipoTransbordo == 0 || tipoTransbordo == 2)
                        ProcessarDadosArquivoMercante(unitOfWork, ref x, ctes, empresa, portoOrigem, terminal.Porto, viagem, terminalOrigem, terminal, false);
                    else
                    {
                        int qtdConhecimentos = ctes.Where(c => c.NumeroManifesto != null && c.NumeroManifesto != "").Distinct().Count();
                        List<string> numerosManifestos = ctes.Select(c => c.NumeroManifesto).Distinct().ToList();
                        bool gerarM4 = true;
                        if (numerosManifestos != null && numerosManifestos.Count > 0)
                        {
                            foreach (var numeroManifesto in numerosManifestos)
                            {
                                if (!string.IsNullOrWhiteSpace(numeroManifesto))
                                {
                                    ProcessarDadosArquivoBaldeacao(unitOfWork, ref x, ctes, empresa, portoOrigem, terminal.Porto, viagem, terminalOrigem, terminal, numeroManifesto, gerarM4, qtdConhecimentos);
                                    gerarM4 = false;
                                }
                            }
                        }
                    }
                }
                x.Flush();
                Random random = new Random();
                return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("H6DDUSIC.", random.Next(100, 999).ToString()));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Mercante.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoContaOeste()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                DateTime.TryParse(Request.Params("DataEntradaInicial"), out DateTime dataEntradaInicial);
                DateTime.TryParse(Request.Params("DataEntradaFinal"), out DateTime dataEntradaFinal);

                if (dataFinal != DateTime.MinValue)
                    dataFinal = dataFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                if (dataEntradaFinal != DateTime.MinValue)
                    dataEntradaFinal = dataEntradaFinal.AddHours(23).AddMinutes(59).AddSeconds(59);

                int empresaSelecionada = Request.GetIntParam("Empresa");
                int codigoTipoMovimentoArquivoContabil = Request.GetIntParam("TipoMovimentoArquivoContabil");

                List<int> codigosModeloDocumentoFiscal = null;
                string codigosModelos = Request.Params("ModeloDocumentoFiscal");
                if (!string.IsNullOrEmpty(codigosModelos))
                    codigosModeloDocumentoFiscal = JsonConvert.DeserializeObject<List<int>>(codigosModelos);

                bool.TryParse(Request.Params("ExtensaoCFOP"), out bool extensaoCFOP);
                bool.TryParse(Request.Params("GerarComCodigoHistoricoTipoMovimento"), out bool gerarCodigoHistorico);
                bool.TryParse(Request.Params("NaoRemoverCaracteresEspeciaisAcentuados"), out bool naoRemoverCaracteresEspeciaisOuAcentuados);
                bool gerarRegistroRetNovaEspecificacao = Request.GetBoolParam("GerarRegistroRetNovaEspecificacao");

                TipoMovimentoArquivoContabilQuestor tipoMovimento = TipoMovimentoArquivoContabilQuestor.ContasPagar;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);
                List<TipoMovimentoArquivoContabilQuestor> tiposMovimentos = new List<TipoMovimentoArquivoContabilQuestor>();
                if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.Todos)
                {
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.ContasReceber);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.ContasPagar);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.DemaisMovimentos);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos);
                    tiposMovimentos.Add(TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos);
                }
                else
                    tiposMovimentos.Add(tipoMovimento);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroEntidade repMovimentoFinanceiroEntidade = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroEntidade(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa > 0 ? codigoEmpresa : empresaSelecionada);

                if (tipoMovimento != TipoMovimentoArquivoContabilQuestor.NFSeEntrada)
                {
                    MemoryStream arquivoINPUT = new MemoryStream();
                    StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);
                    string observacao = " ";
                    foreach (TipoMovimentoArquivoContabilQuestor tipo in tiposMovimentos)
                    {
                        List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilQuestor(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal, tipo, null, codigoTipoMovimentoArquivoContabil);
                        if (movimentoFinanceiro.Count() > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro mov in movimentoFinanceiro)
                            {
                                observacao = (string.IsNullOrWhiteSpace(mov.Observacao) ? "" : mov.Observacao.Length > 300 ? mov.Observacao.Substring(0, 299).Replace(";", "") : mov.Observacao.Replace(";", ""));
                                if (mov.Titulo != null && mov.Titulo.DocumentoEntradaGuia != null && !Utilidades.String.OnlyNumbers(observacao).Contains(Utilidades.String.OnlyNumbers(Utilidades.String.OnlyNumbers(mov.Titulo.DocumentoEntradaGuia.DocumentoEntrada?.Numero.ToString("n0")))))
                                    observacao += " Doc. " + Utilidades.String.OnlyNumbers(mov.Titulo.DocumentoEntradaGuia.DocumentoEntrada?.Numero.ToString("n0")) ?? string.Empty;
                                else if (mov.Titulo != null && mov.Titulo.DuplicataDocumentoEntrada != null && !Utilidades.String.OnlyNumbers(observacao).Contains(Utilidades.String.OnlyNumbers(Utilidades.String.OnlyNumbers(mov.Titulo.DuplicataDocumentoEntrada.DocumentoEntrada?.Numero.ToString("n0")))))
                                    observacao += " Doc. " + Utilidades.String.OnlyNumbers(mov.Titulo.DuplicataDocumentoEntrada.DocumentoEntrada?.Numero.ToString("n0")) ?? string.Empty;
                                else if (mov.Titulo != null && !string.IsNullOrWhiteSpace(mov.Titulo.NumeroDocumentoTituloOriginal) && !Utilidades.String.OnlyNumbers(observacao).Contains(Utilidades.String.OnlyNumbers(mov.Titulo.NumeroDocumentoTituloOriginal)))
                                    observacao += " Doc. " + Utilidades.String.OnlyNumbers(mov.Titulo.NumeroDocumentoTituloOriginal);
                                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade entidade = repMovimentoFinanceiroEntidade.BuscarPorMovimento(mov.Codigo);
                                if (entidade != null && entidade.Motorista != null && !observacao.Contains(entidade.Motorista.Nome))
                                    observacao += " " + entidade.Motorista.Nome;
                                if (mov.Pessoa != null && !observacao.Contains(mov.Pessoa.Nome))
                                    observacao += " " + mov.Pessoa.Nome;
                                if (mov.Titulo != null && mov.Titulo.Pessoa != null && !observacao.Contains(mov.Titulo.Pessoa.Nome))
                                    observacao += " " + mov.Titulo.Pessoa.Nome;

                                observacao = observacao.Replace(";", "");
                                if (observacao.Length > 300)
                                    observacao = observacao.Substring(0, 299).Replace(";", "");

                                bool movimentoReversao = false;
                                if (observacao.ToLower().Contains("reversao") || observacao.ToLower().Contains("reversão"))
                                    movimentoReversao = true;

                                if (!string.IsNullOrWhiteSpace(observacao))
                                    observacao = naoRemoverCaracteresEspeciaisOuAcentuados ? observacao : Utilidades.String.RemoveAllSpecialCharacters(observacao);

                                bool movimentoDesconto = observacao.ToLower().Contains("desconto");

                                string contaDebito = "";
                                string contaCredito = "";
                                if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasReceber || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                                {
                                    if (!movimentoReversao)
                                    {
                                        if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                                        {
                                            if (movimentoDesconto)
                                            {
                                                contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                                contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                            }
                                            else
                                            {
                                                contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                                contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                            }
                                        }
                                        else
                                        {
                                            contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                            contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                        }
                                    }
                                    else
                                    {
                                        if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                                        {
                                            if (movimentoDesconto)
                                            {
                                                contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                                contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                            }
                                            else
                                            {
                                                contaCredito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                                contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                            }
                                        }
                                        else
                                        {
                                            contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                            contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                        }
                                    }
                                }
                                else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasPagar || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                                {
                                    if (!movimentoReversao)
                                    {
                                        if (observacao.Contains("ACRESCIMO NA BAIXA"))
                                        {
                                            contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                            contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                        }
                                        else
                                        {
                                            contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                            contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                        }
                                    }
                                    else
                                    {
                                        contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                        contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.CPF_CNPJ_SemFormato) ? mov.Pessoa.CPF_CNPJ_SemFormato : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                    }
                                }
                                else
                                {
                                    contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                    contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                }

                                x.WriteLine("C;" + //Identificador
                                    empresa.CNPJ_Formatado + ";" + //CODIGOESTAB
                                    mov.DataMovimento.ToString("dd/MM/yyyy") + ";" + //DATALCTOCTB
                                    mov.Documento + ";" + //NUMERODCTO
                                    contaCredito + ";" + //CONTACTBDEB
                                    contaDebito + ";" + //CONTACTBCRED
                                    mov.Valor.ToString("n2").Replace(".", "") + ";" + //VALORLCTOCTB
                                    (gerarCodigoHistorico ? mov.TipoMovimento?.CodigoHistorico ?? "" : "") + ";" + //CODIGOHISTCTB
                                    observacao + ";"//COMPLHIST
                                );
                            }
                        }
                    }
                    x.Flush();

                    return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoContaOeste_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                }
                else
                {
                    Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaDocumentoEntrada = repDocumentoEntrada.BuscarNFSePorEmpresa(codigoEmpresa > 0 ? codigoEmpresa : empresaSelecionada, dataInicial, dataFinal, codigosModeloDocumentoFiscal, dataEntradaInicial, dataEntradaFinal);
                    if (listaDocumentoEntrada.Count() > 0)
                    {
                        MemoryStream arquivoINPUT = new MemoryStream();
                        StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                        for (int i = 0; i < listaDocumentoEntrada.Count(); i++)
                        {
                            var documentoEntrada = listaDocumentoEntrada[i];

                            //Registro 000 - Cabeçalho da NF - Ocorre Obrigatoriamente em cada NF - Uma única vez por NF
                            x.WriteLine(
                                "lcto;" + //Identificador de Registro
                                documentoEntrada.Fornecedor.CPF_CNPJ_SemFormato + ";" + //Fornecedor
                                documentoEntrada.Numero + ";" + //Nota Fiscal
                                "NFS;" + //Espécie
                                documentoEntrada.Serie + ";" + //Série
                                documentoEntrada.DataEntrada.ToString("dd/MM/yyyy") + ";" + //Data Entrada
                                documentoEntrada.DataEmissao.ToString("dd/MM/yyyy") + ";" + //Data Emissão
                                documentoEntrada.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor
                                documentoEntrada.BaseIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Base
                                documentoEntrada.ValorTotalIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Valor
                                "0,00;" + //[IPI] Isentas
                                "0,00;" + //[IPI] Outras
                                ";" + //Complemento
                                documentoEntrada.ValorTotalDesconto.ToString("n2").Replace(".", "") + ";" + //Desconto
                                "0,00;" + //Abatimento Não-Tributado
                                documentoEntrada.ValorTotalFrete.ToString("n2").Replace(".", "") + ";" + //Frete
                                "0,00;" + //Pedágio
                                documentoEntrada.ValorTotalSeguro.ToString("n2").Replace(".", "") + ";" + //Seguro
                                documentoEntrada.ValorTotalOutrasDespesas.ToString("n2").Replace(".", "") + ";" + //Outras Despesas
                                "99;" + //Modelo Doc
                                ";" + //Chave Acesso
                                ";" + //Chave Acesso  Ref
                                "0;" + //Finalidade
                                (int)documentoEntrada.IndicadorPagamento + ";" + //Pagamento
                                "99;" + //Meio Pagto
                                "9;" + //Mod.Frete
                                "0;" //Sit.Doc.Fiscal
                                );

                            var listaDocumentoEntradaItens = documentoEntrada.Itens;

                            //Registro 001 - CFOP Da NF - Ocorre Obrigatoriamente em cada NF -
                            var listaCFOPs = listaDocumentoEntradaItens.Where(obj => obj.CFOP != null).GroupBy(
                                obj => obj.CFOP).Select(obj => new
                                {
                                    CFOP = obj.Key.CodigoCFOP,
                                    Extensao = obj.Key.Extensao,
                                    ValorTotal = obj.Sum(dc => dc.ValorTotal),
                                    BaseCalculoICMS = obj.Sum(dc => dc.BaseCalculoICMS),
                                    AliquotaICMS = obj.Sum(dc => dc.AliquotaICMS),
                                    ValorICMS = obj.Sum(dc => dc.ValorICMS)
                                }).ToList();
                            for (int j = 0; j < listaCFOPs.Count(); j++)
                            {
                                var cfopItens = listaCFOPs[j];

                                if (cfopItens.CFOP > 0)
                                {
                                    var cfop = cfopItens.CFOP.ToString();
                                    if (extensaoCFOP && !string.IsNullOrWhiteSpace(cfopItens.Extensao))
                                        cfop += "." + cfopItens.Extensao;

                                    x.WriteLine(
                                        "cfop;" + //Identificador de Registro
                                        cfop + ";" + //CFOP
                                        cfopItens.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor
                                        cfopItens.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Base Cálculo
                                        "0,00;" + //Alíquota Imposto
                                        "0,00;" + //Valor Imposto
                                        "0,00;" + //Isentas Imposto
                                        "0,00;" //Outras
                                        );
                                }
                            }

                            //Registro 002 - Produtos Da NF - Ocorre Obrigatoriamente em cada NF - Pode ocorrer mais vezes por NF
                            for (int j = 0; j < listaDocumentoEntradaItens.Count(); j++)
                            {
                                var documentoEntradaItem = listaDocumentoEntradaItens[j];
                                var cfop = documentoEntradaItem.CFOP != null ? documentoEntradaItem.CFOP.CodigoCFOP.ToString() : string.Empty;
                                if (extensaoCFOP && documentoEntradaItem.CFOP != null && !string.IsNullOrWhiteSpace(documentoEntradaItem.CFOP.Extensao))
                                    cfop += "." + documentoEntradaItem.CFOP.Extensao;

                                x.WriteLine(
                                "prod;" + //Identificador de Registro
                                documentoEntradaItem.Produto.Codigo + ";" + //Cod. Prod
                                cfop + ";" + //CFOP
                                UnidadeDeMedidaHelper.ObterSigla(documentoEntradaItem.UnidadeMedida) + ";" + //Un. Med
                                documentoEntradaItem.Quantidade.ToString("n2").Replace(".", "") + ";" + //Quantidade
                                documentoEntradaItem.ValorUnitario.ToString("n2").Replace(".", "") + ";" + //Valor Unitário
                                documentoEntradaItem.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor Total
                                (!string.IsNullOrWhiteSpace(documentoEntradaItem.CSTICMS) ? documentoEntradaItem.CSTICMS : string.Empty) + ";" + //[ICMS] CST
                                documentoEntradaItem.BaseCalculoICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Base Cálculo
                                documentoEntradaItem.AliquotaICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Alíquota
                                documentoEntradaItem.ValorICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Valor
                                "0,00;" + //[ICMS] Isentas
                                "0,00;" + //[ICMS] Outras
                                (!string.IsNullOrWhiteSpace(documentoEntradaItem.CSTIPI) ? documentoEntradaItem.CSTIPI : string.Empty) + ";" + //[IPI] CST
                                documentoEntradaItem.BaseCalculoIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Base Cálculo
                                documentoEntradaItem.AliquotaIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Alíquota
                                documentoEntradaItem.ValorIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Valor
                                "0,00;" + //[IPI] Isentas
                                "0,00;" + //[IPI] Outras
                                ";" + //[ISS] CST
                                "0,00;" + //[ISS] Base Cálculo
                                "0,00;" + //[ISS] Alíquota
                                "0,00;" + //[ISS] Valor
                                "0,00;" + //[ISS] Isentas
                                "0,00;" + //[ISS] Outras
                                ";" + //[Sub.Trib.] CST
                                documentoEntradaItem.BaseCalculoICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Base Cálculo
                                documentoEntradaItem.AliquotaICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Alíquota
                                documentoEntradaItem.ValorICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Valor
                                "0,00;" + //[Sub.Trib.] Isentas
                                "0,00;" + //[Sub.Trib.] Outras
                                documentoEntradaItem.Desconto.ToString("n2").Replace(".", "") + ";" + //Val. Desconto
                                "0,00;" + //Val. Despesa Acessória
                                "1;" + //Tipo Estoque
                                ";" + //Qtd.Selo.Contr.IPI
                                documentoEntradaItem.ValorFrete.ToString("n2").Replace(".", "") + ";" + //Val. Frete
                                "0,00;" + //Val. Pedágio
                                documentoEntradaItem.ValorSeguro.ToString("n2").Replace(".", "") + ";" + //Val. Seguro
                                "0,00;" + //Val.Abat.Não Trib
                                "1;" + //Movimenta ?
                                ";" //Indic.Nat.Frete
                                );
                            }

                            //Registro 003 Retenções Da NF - Não Ocorre Obrigatoriamente em cada NF - Não pode ocorrer mais vezes por NF
                            if (gerarRegistroRetNovaEspecificacao)
                            {
                                x.WriteLine(
                                    "reti;" + //Identificador de Registro
                                    "0,00;" + //[INSS] Base Cálculo
                                    documentoEntrada.Fornecedor.CPF_CNPJ_SemFormato + ";" + //Fornecedor
                                    documentoEntrada.Numero + ";" + //Nota Fiscal
                                    "NFS;" + //Espécie
                                    documentoEntrada.Serie + ";" + //Série
                                    documentoEntrada.DataEntrada.ToString("dd/MM/yyyy") + ";" + //Data Entrada
                                    documentoEntrada.DataEmissao.ToString("dd/MM/yyyy") + ";" + //Data Emissão
                                    "0,00;" + //[IRRF] Alíquota
                                    documentoEntrada.ValorTotalRetencaoIR.ToString("n2").Replace(".", "") + ";" + //[IRRF] Valor
                                    ";" + //[IRRF] Código
                                    ";" + //[IRRF] Variação
                                    ";" + //[IRRF] Data Pagto
                                    (documentoEntrada.ValorTotalRetencaoPIS + documentoEntrada.ValorTotalRetencaoCOFINS + documentoEntrada.ValorTotalRetencaoCSLL).ToString("n2").Replace(".", "") + ";" + //[PCC] Total
                                    "1;" + //[PCC] Tipo
                                    documentoEntrada.ValorTotalRetencaoPIS.ToString("n2").Replace(".", "") + ";" + //[PIS] Valor
                                    documentoEntrada.ValorTotalRetencaoCOFINS.ToString("n2").Replace(".", "") + ";" + //[COFINS] Valor
                                    documentoEntrada.ValorTotalRetencaoCSLL.ToString("n2").Replace(".", "") + ";" + //[CSLL] Valor
                                    ";" //[PCC] Data Pagto
                                    );
                            }
                            else
                            {
                                x.WriteLine(
                                    "reti;" + //Identificador de Registro
                                    "0,00;" + //[INSS] Base Cálculo
                                    "0,00;" + //[INSS] Alíquota
                                    documentoEntrada.ValorTotalRetencaoINSS.ToString("n2").Replace(".", "") + ";" + //[INSS] Valor
                                    documentoEntrada.BaseISS.ToString("n2").Replace(".", "") + ";" + //[ISS] Base Cálculo
                                    "0,00;" + //[ISS] Alíquota
                                    documentoEntrada.ValorTotalRetencaoISS.ToString("n2").Replace(".", "") + ";" + //[ISS] Valor
                                    "0,00;" + //[IRRF] Base Cálculo
                                    "0,00;" + //[IRRF] Alíquota
                                    documentoEntrada.ValorTotalRetencaoIR.ToString("n2").Replace(".", "") + ";" + //[IRRF] Valor
                                    ";" + //[IRRF] Código
                                    ";" + //[IRRF] Variação
                                    ";" + //[IRRF] Data Pagto
                                    (documentoEntrada.ValorTotalRetencaoPIS + documentoEntrada.ValorTotalRetencaoCOFINS + documentoEntrada.ValorTotalRetencaoCSLL).ToString("n2").Replace(".", "") + ";" + //[PCC] Total
                                    "1;" + //[PCC] Tipo
                                    documentoEntrada.ValorTotalRetencaoPIS.ToString("n2").Replace(".", "") + ";" + //[PIS] Valor
                                    documentoEntrada.ValorTotalRetencaoCOFINS.ToString("n2").Replace(".", "") + ";" + //[COFINS] Valor
                                    documentoEntrada.ValorTotalRetencaoCSLL.ToString("n2").Replace(".", "") + ";" + //[CSLL] Valor
                                    ";" //[PCC] Data Pagto
                                    );
                            };
                        }
                        x.Flush();

                        return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoContaOeste_NFSeEntrada_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                    }
                    else
                        return new JsonpResult(false, true, "Nenhum registro de NFS-e de Entrada lançadas no período para geração do arquivo contábil.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Conta Oeste.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoPadraoTransben()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");
               
                int codigoTipoMovimentoArquivoContabil = Request.GetIntParam("TipoMovimentoArquivoContabil");

                List<int> codigosModeloDocumentoFiscal = null;
                string codigosModelos = Request.Params("ModeloDocumentoFiscal");
                if (!string.IsNullOrEmpty(codigosModelos))
                    codigosModeloDocumentoFiscal = JsonConvert.DeserializeObject<List<int>>(codigosModelos);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                TipoMovimentoArquivoContabilPadraoTransben tipoMovimento = Request.GetEnumParam<TipoMovimentoArquivoContabilPadraoTransben>("TipoMovimento");

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilPadraoTransben(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal, tipoMovimento, codigoTipoMovimentoArquivoContabil, TipoServicoMultisoftware);

                if (movimentoFinanceiro.Count() > 0)
                {
                    MemoryStream arquivoINPUT = new MemoryStream();
                    StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                    for (int i = 0; i < movimentoFinanceiro.Count(); i++)
                    {

                        string contaDebito = string.Empty;
                        string contaCredito = string.Empty;
                        string colunaCnpjFornecedor = string.Empty;
                        string observacao = string.Empty;
                        string codigoHistoricoMovimentoFinanceiro = string.Empty;
                        string linhaArquivoGeracao = string.Empty;

                        Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil arquivoContabil = movimentoFinanceiro[i];
                        Dominio.Entidades.Cliente pessoa = null;
                        if (arquivoContabil.CNPJPessoaTitulo > 0)
                            pessoa = repCliente.BuscarPorCPFCNPJ(arquivoContabil.CNPJPessoaTitulo);

                        if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasRecebidasDocumento || tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisRecebimentos)
                        {
                            contaDebito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaDebitoContabil) ? arquivoContabil.PlanoDeContaDebitoContabil : arquivoContabil.PlanoDeContaDebito;

                            if (pessoa != null)
                                contaCredito = pessoa.CPF_CNPJ_SemFormato;
                            else
                                contaCredito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaCreditoContabil) ? arquivoContabil.PlanoDeContaCreditoContabil : arquivoContabil.PlanoDeContaCredito;
                        }
                        else
                        {
                            if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasPagasDocumento && pessoa != null)
                                contaDebito = pessoa.CPF_CNPJ_SemFormato;
                            else
                                contaDebito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaDebitoContabil) ? arquivoContabil.PlanoDeContaDebitoContabil : arquivoContabil.PlanoDeContaDebito;

                            contaCredito = !string.IsNullOrWhiteSpace(arquivoContabil.PlanoDeContaCreditoContabil) ? arquivoContabil.PlanoDeContaCreditoContabil : arquivoContabil.PlanoDeContaCredito;

                            if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisPagamentos)
                                colunaCnpjFornecedor = pessoa != null ? pessoa.CPF_CNPJ_SemFormato + ";" : ";";
                        }

                        if (!string.IsNullOrWhiteSpace(arquivoContabil.CodigoHistoricoMovimentoFinanceiro))
                            codigoHistoricoMovimentoFinanceiro = arquivoContabil.CodigoHistoricoMovimentoFinanceiro;

                        if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasPagasDocumento || tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisPagamentos)
                        {

                            if (arquivoContabil?.CodigoTitulo > 0)
                                observacao = "N Titulo. " + arquivoContabil.CodigoTitulo;
                            if (pessoa != null)
                                observacao += ", " + pessoa.Nome + " (" + pessoa.CPF_CNPJ_Formatado + ")";
                            if (!string.IsNullOrWhiteSpace(arquivoContabil.NumeroDocumento))
                                observacao += " - N Doc. " + arquivoContabil.NumeroDocumento;

                            if (!string.IsNullOrWhiteSpace(observacao))
                                observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                            observacao = observacao.Trim();

                            linhaArquivoGeracao = "1;" + //Filial
                                                   $"{(arquivoContabil?.DataBaseSistema != null && arquivoContabil.DataBaseSistema > DateTime.MinValue ? arquivoContabil.DataBaseSistema.ToString("dd/MM/yyyy") : arquivoContabil.Data.ToString("dd/MM/yyyy"))}" + ";"  + //Data de Pagamento
                                                   $"{colunaCnpjFornecedor}" + //CNPJ
                                                   $"{contaDebito};" + 
                                                   $"{contaCredito};" + 
                                                   $"{arquivoContabil.Valor.ToString("n2")}" + ";" + //Valor
                                                   $"{codigoHistoricoMovimentoFinanceiro}" + ";" + //Histórico 
                                                   $"{observacao};"; //Complemento 
                        }
                        else
                        {

                            if (!string.IsNullOrWhiteSpace(arquivoContabil.Documento))
                                observacao = "N Doc. " + arquivoContabil.Documento;
                            if (pessoa != null)
                                observacao += ", " + pessoa.Nome + " (" + pessoa.CPF_CNPJ_Formatado + ")";
                            if (!string.IsNullOrWhiteSpace(arquivoContabil.NumeroDocumento))
                                observacao += " - N. " + arquivoContabil.NumeroDocumento;
                            if (!string.IsNullOrWhiteSpace(arquivoContabil.Observacao))
                                observacao += ", " + arquivoContabil.Observacao.Trim();

                            if (!string.IsNullOrWhiteSpace(observacao))
                                observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                            observacao = observacao.Trim();

                            linhaArquivoGeracao = $"{(arquivoContabil?.DataBaseSistema != null && arquivoContabil.DataBaseSistema > DateTime.MinValue ? arquivoContabil.DataBaseSistema.ToString("dd/MM/yyyy") : arquivoContabil.Data.ToString("dd/MM/yyyy"))}" + ";" + //Data do Lançamento
                                                  $"{colunaCnpjFornecedor}" + //CPF/CNPJ - apenas pro tipo "Demais pagamentos"
                                                  $"{contaDebito}" + ";" + //Conta Débito
                                                  $"{contaCredito}" + ";" + //Conta Crédito
                                                  $"{arquivoContabil.Valor.ToString("n2")}" + ";" + //Valor
                                                  $"{codigoHistoricoMovimentoFinanceiro}" + ";" + //Código do Histórico                                
                                                  $"{observacao};"; //Histórico
                        }

                        x.WriteLine(linhaArquivoGeracao);
                        unitOfWork.FlushAndClear();
                    }
                    x.Flush();

                    return await Task.FromResult(Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("ArquivoPadraoTransben_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt")));
                }
                else
                    return await Task.FromResult(new JsonpResult(false, true, "Nenhum registro de Movimentos Financeiros ou Plano de Contas configurado para geração do arquivo Padrão Transben."));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return await Task.FromResult(new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo Padrão Transben."));
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private byte[] ObterByteTipoArquivoPadraoQuestor(DateTime dataInicial, DateTime dataFinal, List<int> codigosTipoMovimento, int codigoTipoMovimentoArquivoContabil, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<TipoMovimentoArquivoContabilQuestor> tiposMovimentos, TipoMovimentoArquivoContabilQuestor tipoMovimento, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

            MemoryStream arquivoINPUT = new MemoryStream();
            StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

            foreach (TipoMovimentoArquivoContabilQuestor tipo in tiposMovimentos)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilQuestor(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal, tipo, codigosTipoMovimento, codigoTipoMovimentoArquivoContabil);

                if (movimentoFinanceiro.Count == 0)
                    continue;

                foreach (Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro mov in movimentoFinanceiro)
                {
                    string observacao = (string.IsNullOrWhiteSpace(mov.Observacao) ? "" : mov.Observacao.Length > 300 ? mov.Observacao.Substring(0, 299).Replace(";", "") : mov.Observacao.Replace(";", ""));
                    if (mov.Titulo != null && mov.Titulo.DocumentoEntradaGuia != null)
                        observacao += " Doc. " + mov.Titulo.DocumentoEntradaGuia.DocumentoEntrada?.Numero.ToString("n0") ?? string.Empty;
                    else if (mov.Titulo != null && mov.Titulo.DuplicataDocumentoEntrada != null)
                        observacao += " Doc. " + mov.Titulo.DuplicataDocumentoEntrada.DocumentoEntrada?.Numero.ToString("n0") ?? string.Empty;
                    else if (mov.Titulo != null && !string.IsNullOrWhiteSpace(mov.Titulo.NumeroDocumentoTituloOriginal) && !observacao.Contains(mov.Titulo.NumeroDocumentoTituloOriginal))
                        observacao += " Doc. " + mov.Titulo.NumeroDocumentoTituloOriginal;

                    observacao = observacao.Replace(";", "");
                    if (observacao.Length > 300)
                        observacao = observacao.Substring(0, 299).Replace(";", "");

                    bool movimentoReversao = false;
                    if (observacao.ToLower().Contains("reversao") || observacao.ToLower().Contains("reversão"))
                        movimentoReversao = true;

                    if (!string.IsNullOrWhiteSpace(observacao))
                        observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                    bool movimentoDesconto = observacao.ToLower().Contains("desconto");

                    string contaDebito = "";
                    string contaCredito = "";
                    if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasReceber || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                    {
                        if (!movimentoReversao)
                        {
                            if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                            {
                                if (movimentoDesconto)
                                {
                                    contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                    contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                }
                                else
                                {
                                    contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                }
                            }
                            else
                            {
                                contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                                contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                            }
                        }
                        else
                        {
                            if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                            {
                                if (movimentoDesconto)
                                {
                                    contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                }
                                else
                                {
                                    contaCredito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                                }
                            }
                            else
                            {
                                contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                            }
                        }
                    }
                    else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasPagar || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                    {
                        if (!movimentoReversao)
                        {
                            contaCredito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaDebito?.PlanoContabilidade ?? "") : "";
                            contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                        }
                        else
                        {
                            contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                            contaDebito = mov.Pessoa != null && !string.IsNullOrWhiteSpace(mov.Pessoa.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos ? (mov.PlanoDeContaCredito?.PlanoContabilidade ?? "") : "";
                        }
                    }
                    else
                    {
                        contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                        contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                    }


                    x.WriteLine("C;" + //Identificador
                        empresa.CNPJ_Formatado + ";" + //CODIGOESTAB
                        mov.DataMovimento.ToString("dd/MM/yyyy") + ";" + //DATALCTOCTB
                        mov.Documento + ";" + //NUMERODCTO
                        "" + ";" + //CONTACTBDEB
                        contaDebito + ";" + //CONTACTBCRED
                        mov.Valor.ToString("n2").Replace(".", "") + ";" + //VALORLCTOCTB
                        ";" + //CODIGOHISTCTB
                        observacao + ";"//COMPLHIST
                    );

                    x.WriteLine("C;" + //Identificador
                        empresa.CNPJ_Formatado + ";" + //CODIGOESTAB
                        mov.DataMovimento.ToString("dd/MM/yyyy") + ";" + //DATALCTOCTB
                        mov.Documento + ";" + //NUMERODCTO
                        contaCredito + ";" + //CONTACTBDEB
                        "" + ";" + //CONTACTBCRED
                        mov.Valor.ToString("n2").Replace(".", "") + ";" + //VALORLCTOCTB
                        ";" + //CODIGOHISTCTB
                        observacao + ";"//COMPLHIST
                    );
                }
            }

            x.Flush();

            return arquivoINPUT.ToArray();
        }

        private byte[] ObterByteTipoArquivoPadraoNFSeEntradaQuestor(DateTime dataInicial, DateTime dataFinal, DateTime dataEntradaInicial, DateTime dataEntradaFinal, List<int> codigosModeloDocumentoFiscal, int codigoEmpresa, int codigoEmpresaSelecionada, bool extensaoCFOP, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaDocumentoEntrada = repDocumentoEntrada.BuscarNFSePorEmpresa(codigoEmpresa > 0 ? codigoEmpresa : codigoEmpresaSelecionada, dataInicial, dataFinal, codigosModeloDocumentoFiscal, dataEntradaInicial, dataEntradaFinal);

            if (listaDocumentoEntrada.Count == 0)
                throw new ControllerException("Nenhum registro de NFS-e de Entrada lançadas no período para geração do arquivo contábil.");

            MemoryStream arquivoINPUT = new MemoryStream();
            StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

            for (int i = 0; i < listaDocumentoEntrada.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = listaDocumentoEntrada[i];

                //Registro 000 - Cabeçalho da NF - Ocorre Obrigatoriamente em cada NF - Uma única vez por NF
                x.WriteLine(
                    "lcto;" + //Identificador de Registro
                    documentoEntrada.Fornecedor.CPF_CNPJ_SemFormato + ";" + //Fornecedor
                    documentoEntrada.Numero + ";" + //Nota Fiscal
                    "NFS;" + //Espécie
                    documentoEntrada.Serie + ";" + //Série
                    documentoEntrada.DataEntrada.ToString("dd/MM/yyyy") + ";" + //Data Entrada
                    documentoEntrada.DataEmissao.ToString("dd/MM/yyyy") + ";" + //Data Emissão
                    documentoEntrada.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor
                    documentoEntrada.BaseIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Base
                    documentoEntrada.ValorTotalIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Valor
                    "0,00;" + //[IPI] Isentas
                    "0,00;" + //[IPI] Outras
                    ";" + //Complemento
                    documentoEntrada.ValorTotalDesconto.ToString("n2").Replace(".", "") + ";" + //Desconto
                    "0,00;" + //Abatimento Não-Tributado
                    documentoEntrada.ValorTotalFrete.ToString("n2").Replace(".", "") + ";" + //Frete
                    "0,00;" + //Pedágio
                    documentoEntrada.ValorTotalSeguro.ToString("n2").Replace(".", "") + ";" + //Seguro
                    documentoEntrada.ValorTotalOutrasDespesas.ToString("n2").Replace(".", "") + ";" + //Outras Despesas
                    "99;" + //Modelo Doc
                    ";" + //Chave Acesso
                    ";" + //Chave Acesso  Ref
                    "0;" + //Finalidade
                    (int)documentoEntrada.IndicadorPagamento + ";" + //Pagamento
                    "99;" + //Meio Pagto
                    "9;" + //Mod.Frete
                    "0;" //Sit.Doc.Fiscal
                    );

                var listaDocumentoEntradaItens = documentoEntrada.Itens;

                //Registro 001 - CFOP Da NF - Ocorre Obrigatoriamente em cada NF -
                var listaCFOPs = listaDocumentoEntradaItens.Where(obj => obj.CFOP != null).GroupBy(
                    obj => obj.CFOP).Select(obj => new
                    {
                        CFOP = obj.Key.CodigoCFOP,
                        Extensao = obj.Key.Extensao,
                        ValorTotal = obj.Sum(dc => dc.ValorTotal),
                        BaseCalculoICMS = obj.Sum(dc => dc.BaseCalculoICMS),
                        AliquotaICMS = obj.Sum(dc => dc.AliquotaICMS),
                        ValorICMS = obj.Sum(dc => dc.ValorICMS)
                    }).ToList();
                for (int j = 0; j < listaCFOPs.Count(); j++)
                {
                    var cfopItens = listaCFOPs[j];

                    if (cfopItens.CFOP > 0)
                    {
                        var cfop = cfopItens.CFOP.ToString();
                        if (extensaoCFOP && !string.IsNullOrWhiteSpace(cfopItens.Extensao))
                            cfop += "." + cfopItens.Extensao;

                        x.WriteLine(
                            "cfop;" + //Identificador de Registro
                            cfop + ";" + //CFOP
                            cfopItens.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor
                            cfopItens.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Base Cálculo
                            "0,00;" + //Alíquota Imposto
                            "0,00;" + //Valor Imposto
                            "0,00;" + //Isentas Imposto
                            "0,00;" //Outras
                            );
                    }
                }

                //Registro 002 - Produtos Da NF - Ocorre Obrigatoriamente em cada NF - Pode ocorrer mais vezes por NF
                for (int j = 0; j < listaDocumentoEntradaItens.Count(); j++)
                {
                    var documentoEntradaItem = listaDocumentoEntradaItens[j];
                    var cfop = documentoEntradaItem.CFOP != null ? documentoEntradaItem.CFOP.CodigoCFOP.ToString() : string.Empty;
                    if (extensaoCFOP && documentoEntradaItem.CFOP != null && !string.IsNullOrWhiteSpace(documentoEntradaItem.CFOP.Extensao))
                        cfop += "." + documentoEntradaItem.CFOP.Extensao;

                    x.WriteLine(
                    "prod;" + //Identificador de Registro
                    documentoEntradaItem.Produto.Codigo + ";" + //Cod. Prod
                    cfop + ";" + //CFOP
                    UnidadeDeMedidaHelper.ObterSigla(documentoEntradaItem.UnidadeMedida) + ";" + //Un. Med
                    documentoEntradaItem.Quantidade.ToString("n2").Replace(".", "") + ";" + //Quantidade
                    documentoEntradaItem.ValorUnitario.ToString("n2").Replace(".", "") + ";" + //Valor Unitário
                    documentoEntradaItem.ValorTotal.ToString("n2").Replace(".", "") + ";" + //Valor Total
                    (!string.IsNullOrWhiteSpace(documentoEntradaItem.CSTICMS) ? documentoEntradaItem.CSTICMS : string.Empty) + ";" + //[ICMS] CST
                    documentoEntradaItem.BaseCalculoICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Base Cálculo
                    documentoEntradaItem.AliquotaICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Alíquota
                    documentoEntradaItem.ValorICMS.ToString("n2").Replace(".", "") + ";" + //[ICMS] Valor
                    "0,00;" + //[ICMS] Isentas
                    "0,00;" + //[ICMS] Outras
                    (!string.IsNullOrWhiteSpace(documentoEntradaItem.CSTIPI) ? documentoEntradaItem.CSTIPI : string.Empty) + ";" + //[IPI] CST
                    documentoEntradaItem.BaseCalculoIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Base Cálculo
                    documentoEntradaItem.AliquotaIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Alíquota
                    documentoEntradaItem.ValorIPI.ToString("n2").Replace(".", "") + ";" + //[IPI] Valor
                    "0,00;" + //[IPI] Isentas
                    "0,00;" + //[IPI] Outras
                    ";" + //[ISS] CST
                    "0,00;" + //[ISS] Base Cálculo
                    "0,00;" + //[ISS] Alíquota
                    "0,00;" + //[ISS] Valor
                    "0,00;" + //[ISS] Isentas
                    "0,00;" + //[ISS] Outras
                    ";" + //[Sub.Trib.] CST
                    documentoEntradaItem.BaseCalculoICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Base Cálculo
                    documentoEntradaItem.AliquotaICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Alíquota
                    documentoEntradaItem.ValorICMSST.ToString("n2").Replace(".", "") + ";" + //[Sub.Trib.] Valor
                    "0,00;" + //[Sub.Trib.] Isentas
                    "0,00;" + //[Sub.Trib.] Outras
                    documentoEntradaItem.Desconto.ToString("n2").Replace(".", "") + ";" + //Val. Desconto
                    "0,00;" + //Val. Despesa Acessória
                    "1;" + //Tipo Estoque
                    ";" + //Qtd.Selo.Contr.IPI
                    documentoEntradaItem.ValorFrete.ToString("n2").Replace(".", "") + ";" + //Val. Frete
                    "0,00;" + //Val. Pedágio
                    documentoEntradaItem.ValorSeguro.ToString("n2").Replace(".", "") + ";" + //Val. Seguro
                    "0,00;" + //Val.Abat.Não Trib
                    "1;" + //Movimenta ?
                    ";" //Indic.Nat.Frete
                    );
                }

                //Registro 003 Retenções Da NF - Não Ocorre Obrigatoriamente em cada NF - Não pode ocorrer mais vezes por NF
                x.WriteLine(
                    "reti;" + //Identificador de Registro
                    "0,00;" + //[INSS] Base Cálculo
                    "0,00;" + //[INSS] Alíquota
                    documentoEntrada.ValorTotalRetencaoINSS.ToString("n2").Replace(".", "") + ";" + //[INSS] Valor
                    documentoEntrada.BaseISS.ToString("n2").Replace(".", "") + ";" + //[ISS] Base Cálculo
                    "0,00;" + //[ISS] Alíquota
                    documentoEntrada.ValorTotalRetencaoISS.ToString("n2").Replace(".", "") + ";" + //[ISS] Valor
                    "0,00;" + //[IRRF] Base Cálculo
                    "0,00;" + //[IRRF] Alíquota
                    documentoEntrada.ValorTotalRetencaoIR.ToString("n2").Replace(".", "") + ";" + //[IRRF] Valor
                    ";" + //[IRRF] Código
                    ";" + //[IRRF] Variação
                    ";" + //[IRRF] Data Pagto
                    (documentoEntrada.ValorTotalRetencaoPIS + documentoEntrada.ValorTotalRetencaoCOFINS + documentoEntrada.ValorTotalRetencaoCSLL).ToString("n2").Replace(".", "") + ";" + //[PCC] Total
                    "1;" + //[PCC] Tipo
                    documentoEntrada.ValorTotalRetencaoPIS.ToString("n2").Replace(".", "") + ";" + //[PIS] Valor
                    documentoEntrada.ValorTotalRetencaoCOFINS.ToString("n2").Replace(".", "") + ";" + //[COFINS] Valor
                    documentoEntrada.ValorTotalRetencaoCSLL.ToString("n2").Replace(".", "") + ";" + //[CSLL] Valor
                    ";" //[PCC] Data Pagto
                    );

            }

            x.Flush();

            return arquivoINPUT.ToArray();
        }

        private byte[] ObterByteTipoArquivoContabilQuestor(DateTime dataInicial, DateTime dataFinal, List<int> codigosTipoMovimento, int codigoTipoMovimentoArquivoContabil, int codigoPlanoConta, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<TipoMovimentoArquivoContabilQuestor> tiposMovimentos, TipoMovimentoArquivoContabilQuestor tipoMovimento, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

            MemoryStream arquivoINPUT = new MemoryStream();
            StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

            foreach (TipoMovimentoArquivoContabilQuestor tipo in tiposMovimentos)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> movimentoFinanceiro = repMovimentoFinanceiro.BuscarDadosParaArquivoContabilQuestor(codigoEmpresa, tipoAmbiente, dataInicial, dataFinal, tipo, codigosTipoMovimento, codigoTipoMovimentoArquivoContabil, codigoPlanoConta);

                if (movimentoFinanceiro.Count == 0)
                    continue;

                foreach (Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro mov in movimentoFinanceiro)
                {
                    string observacao = (string.IsNullOrWhiteSpace(mov.Observacao) ? "" : mov.Observacao.Length > 300 ? mov.Observacao.Substring(0, 299).Replace(";", "") : mov.Observacao.Replace(";", ""));
                    if (mov.Titulo != null && mov.Titulo.DocumentoEntradaGuia != null)
                        observacao += " Doc. " + mov.Titulo.DocumentoEntradaGuia.DocumentoEntrada?.Numero.ToString("n0") ?? string.Empty;
                    else if (mov.Titulo != null && mov.Titulo.DuplicataDocumentoEntrada != null)
                        observacao += " Doc. " + mov.Titulo.DuplicataDocumentoEntrada.DocumentoEntrada?.Numero.ToString("n0") ?? string.Empty;
                    else if (mov.Titulo != null && !string.IsNullOrWhiteSpace(mov.Titulo.NumeroDocumentoTituloOriginal) && !observacao.Contains(mov.Titulo.NumeroDocumentoTituloOriginal))
                        observacao += " Doc. " + mov.Titulo.NumeroDocumentoTituloOriginal;

                    if (mov.Pessoa != null)
                        observacao += " " + mov.Pessoa.Nome;

                    observacao = observacao.Replace(";", "");
                    if (observacao.Length > 300)
                        observacao = observacao.Substring(0, 299).Replace(";", "");

                    bool movimentoReversao = false;
                    if (observacao.ToLower().Contains("reversao") || observacao.ToLower().Contains("reversão"))
                        movimentoReversao = true;

                    if (!string.IsNullOrWhiteSpace(observacao))
                        observacao = Utilidades.String.RemoveAllSpecialCharacters(observacao);

                    bool movimentoDesconto = observacao.ToLower().Contains("desconto");

                    string contaDebito = "";
                    string contaCredito = "";
                    if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasReceber || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                    {
                        if (!movimentoReversao)
                        {
                            if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                            {
                                if (movimentoDesconto)
                                {
                                    contaDebito = !string.IsNullOrWhiteSpace(mov.Pessoa?.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                }
                                else
                                {
                                    contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    contaCredito = !string.IsNullOrWhiteSpace(mov.Pessoa?.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                }
                            }
                            else
                            {
                                contaDebito = !string.IsNullOrWhiteSpace(mov.Pessoa?.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                            }
                        }
                        else
                        {
                            if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                            {
                                if (movimentoDesconto)
                                {
                                    contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    contaCredito = !string.IsNullOrWhiteSpace(mov.Pessoa?.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                }
                                else
                                {
                                    contaCredito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                    contaDebito = !string.IsNullOrWhiteSpace(mov.Pessoa?.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                                }
                            }
                            else
                            {
                                contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                                contaCredito = !string.IsNullOrWhiteSpace(mov.Pessoa?.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                            }
                        }
                    }
                    else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasPagar || tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                    {
                        if (!movimentoReversao)
                        {
                            contaCredito = !string.IsNullOrWhiteSpace(mov.Pessoa?.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                            contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                        }
                        else
                        {
                            contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                            contaDebito = !string.IsNullOrWhiteSpace(mov.Pessoa?.ContaFornecedorEBS) ? mov.Pessoa.ContaFornecedorEBS : mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                        }
                    }
                    else
                    {
                        contaCredito = mov.PlanoDeContaDebito?.PlanoContabilidade ?? "";
                        contaDebito = mov.PlanoDeContaCredito?.PlanoContabilidade ?? "";
                    }

                    int codigoHistorico = 299;
                    if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasReceber)
                        codigoHistorico = 2706;
                    else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                        codigoHistorico = movimentoDesconto ? 607 : 7;
                    else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                        codigoHistorico = 265;

                    x.WriteLine(empresa.CodigoIntegracao + ", " + //Código da Matriz/Filial
                        mov.DataMovimento.ToString("ddMMyyyy") + ", " + //Data
                        contaCredito + ", " + //Conta Débito
                        contaDebito + ", " + //Conta Crédito
                        mov.Valor.ToString("n2").Replace(".", "").Replace(",", ".") + ", " + //Valor
                        codigoHistorico + ", " + //Código Histórico
                        (!string.IsNullOrWhiteSpace(observacao) ? '"' + observacao + '"' : "")//Complemento
                    );
                }
            }

            x.Flush();

            return arquivoINPUT.ToArray();
        }

        private void ProcessarDadosArquivoBaldeacao(Repositorio.UnitOfWork unitOfWork, ref StreamWriter x, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem, Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino, Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino, string numeroManifesto, bool gerarM4, int qtdConhecimentos)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            ctes = ctes.Where(c => c.NumeroManifesto == numeroManifesto).ToList();

            if (ctes.Count() > 0)
            {
                if (gerarM4)
                {
                    string linhaM4 = "M4"; //TIPO
                    linhaM4 += "I3";//IND-FUNÇÃO-CARGA
                    linhaM4 += Utilidades.String.Right(numeroManifesto.PadRight(13, ' '), 13);//NRO-MANIFESTO
                    linhaM4 += (qtdConhecimentos.ToString("D")).PadLeft(4, '0');//QTDE-CE
                    linhaM4 += "0".PadLeft(4, '0');//QTDE-CONT-VZ
                    linhaM4 += empresa.CNPJ_SemFormato;//COD-EMP-NAV
                    linhaM4 += empresa.CNPJ_SemFormato;//COD-AGEN-NAV
                    linhaM4 += (DateTime.Now.Date.AddDays(1)).ToString("yyyyMMdd");//DT-ENC-MANIF
                    linhaM4 += (DateTime.Now.Date.AddDays(1)).ToString("yyyyMMdd");//DT-DESCARGA

                    if (!string.IsNullOrWhiteSpace(portoOrigem.CodigoMercante))
                        linhaM4 += Utilidades.String.Right((portoOrigem.CodigoMercante).PadRight(5, ' '), 5);//CO-PORTO-ORIG
                    else
                        linhaM4 += Utilidades.String.Right(("BR" + portoOrigem.CodigoIATA).PadRight(5, ' '), 5);//CO-PORTO-ORIG                

                    if (!string.IsNullOrWhiteSpace(portoDestino.CodigoMercante))
                        linhaM4 += Utilidades.String.Right((portoDestino.CodigoMercante).PadRight(5, ' '), 5);//CO-PORTO-DEST
                    else
                        linhaM4 += Utilidades.String.Right(("BR" + portoDestino.CodigoIATA).PadRight(5, ' '), 5);//CO-PORTO-DEST

                    linhaM4 += (viagem.NumeroViagem.ToString("D").PadLeft(3, '0') + DirecaoViagemMultimodalHelper.ObterAbreviacao(viagem.DirecaoViagemMultimodal)).PadRight(10, ' ');//NR-VIAGEM
                    linhaM4 += Utilidades.String.Right(viagem.Navio.CodigoIMO.PadRight(10, ' '), 10);//COD-IMO

                    if (!string.IsNullOrWhiteSpace(terminalOrigem.CodigoMercante))
                        linhaM4 += Utilidades.String.Right(terminalOrigem.CodigoMercante.PadRight(8, ' '), 8);//CO-TERM-CARR
                    else
                        linhaM4 += Utilidades.String.Right(terminalOrigem.CodigoTerminal.PadRight(8, ' '), 8);//CO-TERM-CARR

                    linhaM4 += " ".PadLeft(32, ' ');//espaços

                    if (!string.IsNullOrWhiteSpace(terminalDestino.CodigoMercante))
                        linhaM4 += Utilidades.String.Right(terminalDestino.CodigoMercante.PadRight(8, ' '), 8);//CO-TERM-DESC
                    else
                        linhaM4 += Utilidades.String.Right(terminalDestino.CodigoTerminal.PadRight(8, ' '), 8);//CO-TERM-DESC

                    x.WriteLine(linhaM4);
                }

                foreach (var cte in ctes)
                {
                    string linhaC4 = "C4";
                    linhaC4 += Utilidades.String.Right(cte.NumeroCEMercante.PadRight(18, ' '), 18);//NUMERO-CE

                    if (!string.IsNullOrWhiteSpace(terminalOrigem?.CodigoMercante))
                        linhaC4 += (terminalOrigem?.CodigoMercante ?? "").PadRight(8, ' ');//COD-TERM-PORT DESCARREG
                    else
                        linhaC4 += (terminalOrigem?.CodigoTerminal ?? "").PadRight(8, ' ');//COD-TERM-PORT DESCARREG

                    if (!string.IsNullOrWhiteSpace(terminalDestino?.CodigoMercante))
                        linhaC4 += (terminalDestino?.CodigoMercante ?? "").PadRight(8, ' ');//COD-TERM-PORT DESCARREG
                    else
                        linhaC4 += (terminalDestino?.CodigoTerminal ?? "").PadRight(8, ' ');//COD-TERM-PORT DESCARREG

                    linhaC4 += " ".PadLeft(14, ' ');//cte.ValorFrete.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(13, '0');//VL-FRETE-TOTAL-BALDEACAO
                    linhaC4 += "X".PadLeft(3, ' ');

                    x.WriteLine(linhaC4);
                }
            }
        }

        private void ProcessarDadosArquivoMercante(Repositorio.UnitOfWork unitOfWork, ref StreamWriter x, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem, Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino, Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino, bool transbordo)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.InformacaoCargaCTE informacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            if (ctes.Count() > 0)
            {
                string linhaM3 = "M3"; //TIPO
                linhaM3 += (ctes.Count().ToString("D")).PadLeft(4, '0');//QTDE-CE
                linhaM3 += "0".PadLeft(4, '0');//QTDE-CONT-VZ
                linhaM3 += empresa.CNPJ_SemFormato;//COD-EMP-NAV
                linhaM3 += empresa.CNPJ_SemFormato;//COD-AGEN-NAV
                linhaM3 += (DateTime.Now.Date.AddDays(1)).ToString("yyyyMMdd");//DT-ENC-MANIF
                linhaM3 += (DateTime.Now.Date.AddDays(1)).ToString("yyyyMMdd");//DT-DESCARGA

                if (!string.IsNullOrWhiteSpace(portoOrigem.CodigoMercante))
                    linhaM3 += Utilidades.String.Right((portoOrigem.CodigoMercante).PadRight(5, ' '), 5);//CO-PORTO-ORIG
                else
                    linhaM3 += Utilidades.String.Right(("BR" + portoOrigem.CodigoIATA).PadRight(5, ' '), 5);//CO-PORTO-ORIG

                if (!string.IsNullOrWhiteSpace(portoDestino.CodigoMercante))
                    linhaM3 += Utilidades.String.Right((portoDestino.CodigoMercante).PadRight(5, ' '), 5);//CO-PORTO-DEST
                else
                    linhaM3 += Utilidades.String.Right(("BR" + portoDestino.CodigoIATA).PadRight(5, ' '), 5);//CO-PORTO-DEST
                linhaM3 += (viagem.NumeroViagem.ToString("D").PadLeft(3, '0') + DirecaoViagemMultimodalHelper.ObterAbreviacao(viagem.DirecaoViagemMultimodal)).PadRight(10, ' ');//NR-VIAGEM
                linhaM3 += Utilidades.String.Right(viagem.Navio.CodigoIMO.PadRight(10, ' '), 10);//COD-IMO

                if (!string.IsNullOrWhiteSpace(terminalOrigem.CodigoMercante))
                    linhaM3 += Utilidades.String.Right(terminalOrigem.CodigoMercante.PadRight(8, ' '), 8);//CO-TERM-CARR
                else
                    linhaM3 += Utilidades.String.Right(terminalOrigem.CodigoTerminal.PadRight(8, ' '), 8);//CO-TERM-CARR

                linhaM3 += " ".PadLeft(32, ' ');//espaços

                if (!string.IsNullOrWhiteSpace(terminalDestino.CodigoMercante))
                    linhaM3 += Utilidades.String.Right(terminalDestino.CodigoMercante.PadRight(8, ' '), 8);//CO-TERM-DESC
                else
                    linhaM3 += Utilidades.String.Right(terminalDestino.CodigoTerminal.PadRight(8, ' '), 8);//CO-TERM-DESC

                x.WriteLine(linhaM3);

                foreach (var cte in ctes)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repCargaCTe.BuscarPedidoPorCTe(cte.Codigo);
                    Dominio.Entidades.ParticipanteCTe tomador = null;
                    Dominio.Entidades.ParticipanteCTe remetente = null;
                    if (cte.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                    {
                        if (cte.DocumentosTransporteAnterior != null && cte.DocumentosTransporteAnterior.Count > 0)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnterior = repCTe.BuscarPorChave(cte.DocumentosTransporteAnterior.FirstOrDefault().Chave);
                            if (cteAnterior != null)
                            {
                                tomador = cteAnterior.TomadorPagador;
                                remetente = cteAnterior.Remetente;
                            }
                            else
                            {
                                tomador = cte.TomadorPagador;
                                remetente = cte.Remetente;
                            }
                        }
                        else
                        {
                            tomador = cte.TomadorPagador;
                            remetente = cte.Remetente;
                        }
                    }
                    else
                    {
                        tomador = cte.TomadorPagador;
                        remetente = cte.Remetente;
                    }

                    string linhaC3 = "C3";
                    linhaC3 += (cte.XMLNotaFiscais.Count().ToString("D").PadLeft(4, '0'));//QTDE-NF
                    linhaC3 += (cte.Containers.Count().ToString("D").PadLeft(4, '0'));//QTDE-NF
                    linhaC3 += Utilidades.String.Right(cte.NumeroControle.PadRight(18, ' '), 18);//NUMERO-CE
                    linhaC3 += "N";//ND-BL-A-ORDEM
                    linhaC3 += cte.TomadorPagador.Cliente.Tipo == "E" ? "S" : "N";//IND-CONSIG-ESTR
                    linhaC3 += Utilidades.String.Right((cte.TomadorPagador.Nome + ", " + cte.TomadorPagador.Numero + " " + cte.TomadorPagador.Endereco + " " + (cte.TomadorPagador.Localidade?.DescricaoCidadeEstado ?? cte.TomadorPagador.Cidade) + " " + cte.TomadorPagador.Bairro).PadRight(253, ' '), 253);//IDENT-CONSIGNAT
                    linhaC3 += " ".PadRight(30, ' ');//NR-PASS-ESTR
                    linhaC3 += Utilidades.String.Right(cte.TomadorPagador.Cliente.Tipo == "E" ? cte.TomadorPagador.Nome.PadRight(55, ' ') : " ".PadRight(55, ' '), 55);//NOM-CONSIG-ESTR
                    linhaC3 += (!string.IsNullOrWhiteSpace(tomador.CPF_CNPJ) ? tomador.CPF_CNPJ_SemFormato : "").PadRight(14, ' ');//CNPJ-CPF-CONSIG
                    linhaC3 += (!string.IsNullOrWhiteSpace(remetente.CPF_CNPJ) ? remetente.CPF_CNPJ_SemFormato : "").PadRight(14, ' ');//CNPJ / CPF-EMBARC
                    linhaC3 += Utilidades.String.Right((remetente.Nome + ", " + remetente.Numero + " " + remetente.Endereco + " " + (remetente.Localidade?.DescricaoCidadeEstado ?? "") + " " + remetente.Bairro).PadRight(253, ' '), 253);//IDENT-EMBARC
                    linhaC3 += cte.DataEmissao.Value.ToString("yyyyMMdd");//DT-EMIS-CONHEC
                                                                          //linhaC3 += Utilidades.String.Right(((cte.Containers != null ? cte.Containers.Count().ToString("D") : "1") + " Container de " + (cte.Containers != null && cte.Containers.Count > 0 ? (cte.Containers.FirstOrDefault().Container?.ContainerTipo?.Descricao ?? "") : " ") + " p s dizendo conter" + cte.Volumes.ToString("D") + " Volumes de " + cte.ProdutoPredominanteCTe).PadRight(506, ' '), 506);//DESCR-MERC
                    linhaC3 += Utilidades.String.Right(cte.ProdutoPredominanteCTe.PadRight(506, ' '), 506);//DESCR-MERC
                    linhaC3 += Utilidades.String.Right(Utilidades.String.RemoveAllSpecialCharacters(cte.ObservacoesGerais).PadRight(253, ' '), 253);//OBSERVACOES
                    linhaC3 += cte.PesoCubado.ToString("n3").Replace(".", "").Replace(",", "").PadLeft(13, '0');//CUBAGEM-M3

                    if (!string.IsNullOrWhiteSpace(cte.PortoOrigem?.CodigoMercante))
                        linhaC3 += (((cte.PortoOrigem?.CodigoMercante ?? ""))).PadLeft(5, ' ');//PORTO-ORIGEM
                    else
                        linhaC3 += (("BR" + (cte.PortoOrigem?.CodigoIATA ?? ""))).PadLeft(5, ' ');//PORTO-ORIGEM

                    if (!string.IsNullOrWhiteSpace(cte.PortoDestino?.CodigoMercante))
                        linhaC3 += (((cte.PortoDestino?.CodigoMercante ?? ""))).PadLeft(5, ' ');//PORTO-ORIGEM
                    else
                        linhaC3 += (("BR" + (cte.PortoDestino?.CodigoIATA ?? ""))).PadLeft(5, ' ');//PORTO-ORIGEM

                    linhaC3 += cte.ValorPrestacaoServico.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(13, '0');//VL-FRETE-BASICO
                    linhaC3 += "790";//CD-MOEDA-FRETE
                    linhaC3 += "P";//CD-RECOLH-FRETE
                    linhaC3 += "HH";//MODAL-FRETE
                    linhaC3 += "N";//CATEGORIA-CARGA
                    linhaC3 += cte.LocalidadeTerminoPrestacao.Estado.Sigla;//UF-DEST-CARGA

                    if (!string.IsNullOrWhiteSpace(terminalOrigem?.CodigoMercante))
                        linhaC3 += (terminalOrigem?.CodigoMercante ?? "").PadLeft(5, ' ');//COD-TERM-PORT DESCARREG
                    else
                        linhaC3 += (terminalOrigem?.CodigoTerminal ?? "").PadLeft(5, ' ');//COD-TERM-PORT DESCARREG

                    if (!string.IsNullOrWhiteSpace(terminalDestino?.CodigoMercante))
                        linhaC3 += (terminalDestino?.CodigoMercante ?? "").PadLeft(5, ' ');//COD-TERM-PORT DESCARREG
                    else
                        linhaC3 += (terminalDestino?.CodigoTerminal ?? "").PadLeft(5, ' ');//COD-TERM-PORT DESCARREG

                    linhaC3 += "N";//ND-BL-SERVICO
                    linhaC3 += "0".PadLeft(15, '0');//NUMERO-CE-MERCANTEORIGINAL
                    linhaC3 += " ".PadLeft(1657, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS
                    linhaC3 += " ".PadLeft(7, ' ');//BRANCOS
                    linhaC3 += "0".PadLeft(13, '0');//ZEROS

                    x.WriteLine(linhaC3);
                    List<string> ncms = new List<string>();
                    foreach (var nota in cte.XMLNotaFiscais)
                    {
                        string linhaN3 = "N3";
                        linhaN3 += Utilidades.String.Right(nota.Numero.ToString("D").PadRight(15, ' '), 15);//NR-NOTA-FISCAL

                        string serie = nota.SerieOuSerieDaChave;
                        if (string.IsNullOrWhiteSpace(serie))
                            serie = nota.Serie;
                        if (string.IsNullOrWhiteSpace(serie))
                            serie = "1";

                        linhaN3 += Utilidades.String.Right(serie.PadRight(10, ' '), 10);//NR-SERIE-NFISCAL
                        linhaN3 += nota.DataEmissao.ToString("yyyyMMdd");//DT-EMISSAO-NF
                        linhaN3 += (nota.Volumes > 0 ? nota.Volumes : 1m).ToString("n3").Replace(".", "").Replace(",", "").PadLeft(13, '0');//VL-FRETE-BASICO
                        linhaN3 += nota.Emitente.CPF_CNPJ_SemFormato.PadRight(14, ' ');//CNPJ-EMIT-NF
                        linhaN3 += Utilidades.String.Right(nota.Emitente.IE_RG.PadRight(14, ' '), 14);//INSC-EST-EMIT-NF

                        if (!string.IsNullOrWhiteSpace(nota.NCM) && !ncms.Contains(nota.NCM))
                            ncms.Add(nota.NCM);

                        x.WriteLine(linhaN3);
                    }

                    int sequencialContainer = 1;
                    foreach (var container in cte.Containers)
                    {
                        decimal tara = container.Container?.Tara ?? 0;
                        if (tara <= 0)
                            tara = 2000;

                        decimal pesoContainer = 1;
                        decimal pesoCubado = 1;

                        pesoContainer = repCTe.BuscarPesoNotasConhecimento(container.Codigo, cte.Codigo);

                        if (pesoContainer <= 0)
                            pesoContainer = repCTe.BuscarPesoBrutoContainer(container.Codigo, cte.Codigo);

                        if (pesoContainer <= 0)
                            pesoContainer = 1;

                        pesoCubado = repCTe.BuscarPesoCubicoContainer(container.Codigo);
                        if (pesoCubado <= 0)
                            pesoCubado = 1;


                        string linhaI3 = "I3";
                        linhaI3 += "1";//TIPO-ÍTEM-CARGA
                        linhaI3 += sequencialContainer.ToString("D").PadLeft(4, '0');//R-ITEM
                        linhaI3 += pesoContainer.ToString("n3").Replace(".", "").Replace(",", "").PadLeft(12, '0');//PESO-BRUTO-KG

                        //40 = 40G0
                        //20 = 22G0
                        if (container.Container.ContainerTipo == null)
                            linhaI3 += Utilidades.String.Right(("45G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Contains("40"))
                            linhaI3 += Utilidades.String.Right(("45G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Contains("20TK"))
                            linhaI3 += Utilidades.String.Right(("22T0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else if (container.Container.ContainerTipo.Descricao.Contains("20"))
                            linhaI3 += Utilidades.String.Right(("22G0").PadRight(4, ' '), 4);//TIPO-CONTEINER
                        else
                            linhaI3 += Utilidades.String.Right((container.Container?.ContainerTipo?.Descricao ?? "").PadRight(4, ' '), 4);//TIPO-CONTEINER

                        linhaI3 += Utilidades.String.Right((container.Container?.Numero ?? "").PadRight(11, ' '), 11);//NR-CONTEINER
                        linhaI3 += tara.ToString("n3").Replace(".", "").Replace(",", "").PadLeft(9, '0');//PB- TR-CONT
                        linhaI3 += "S";//N-PARCIAL-CONT
                        linhaI3 += " ".PadRight(2, ' ');//TIPO-EMBAL-SOLTA
                        linhaI3 += "0".PadLeft(7, '0');//QT-IT-CARG-SOLTA
                        linhaI3 += " ".PadRight(2, ' ');//CD-TIPO-GRANEL
                        linhaI3 += " ".PadRight(253, ' ');//TX-DESCR-GRANEL
                        linhaI3 += " ".PadRight(30, ' ');//NR-CHASSI-VEIC
                        linhaI3 += " ".PadRight(55, ' ');//NOME-MARCA
                        linhaI3 += " ".PadRight(55, ' ');//NOM-CONTRAMARC

                        linhaI3 += Utilidades.String.Right((pedido?.IMOUnidade ?? " ").PadRight(6, ' '), 6);//" ".PadRight(6, ' ');//CD-MERC-PERIGO
                        linhaI3 += Utilidades.String.Right((pedido?.IMOClasse ?? " ").PadRight(4, ' '), 4);//" ".PadRight(4, ' ');//CLASS-MERC-PERIG

                        linhaI3 += pesoCubado.ToString("n3").Replace(".", "").Replace(",", "").PadLeft(13, '0');//VL-CUBAGEM-M3
                        linhaI3 += !string.IsNullOrWhiteSpace(container.Lacre1) ? Utilidades.String.Right(container.Lacre1?.PadRight(15, ' '), 15) : " ".PadRight(15, ' ');//LACRE1
                        linhaI3 += !string.IsNullOrWhiteSpace(container.Lacre2) ? Utilidades.String.Right(container.Lacre2?.PadRight(15, ' '), 15) : " ".PadRight(15, ' ');//LACRE2
                        linhaI3 += !string.IsNullOrWhiteSpace(container.Lacre3) ? Utilidades.String.Right(container.Lacre3?.PadRight(15, ' '), 15) : " ".PadRight(15, ' ');//LACRE3
                        linhaI3 += " ".PadRight(15, ' ');//LACRE4
                        if (ncms.Count == 0)
                            ncms.Add("2710");

                        int qtdNCMs = 1;
                        foreach (var ncm in ncms)
                        {
                            if (qtdNCMs <= 191)
                            {
                                linhaI3 += Utilidades.String.Right(ncm.PadRight(8, ' '), 8).Replace("2016", "4415");//LACRE4                            
                            }
                            qtdNCMs++;
                        }

                        x.WriteLine(linhaI3);

                        sequencialContainer++;
                    }
                }
            }
        }

        private bool ProcessarRetornoArquivoMercante(out string msgRetorno, StreamReader streamReader, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            msgRetorno = "";

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            int linha = 0;
            var cellValue = "";
            string numeroManifesto = "";
            string numeroCEMertante = "";
            string numeroControle = "";

            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();

            while ((cellValue = streamReader.ReadLine()) != null)
            {
                if (linha == 0)
                {
                    if (cellValue.Substring(0, 2).Trim() != "M3")
                    {
                        msgRetorno = "O layout do arquivo não se encontra homologado!";
                        return false;
                    }
                }
                if (cellValue.Substring(0, 2).Trim() == "I3")
                {
                    numeroManifesto = cellValue.Substring(10, 13).Trim();
                    numeroCEMertante = cellValue.Substring(25, 15).Trim();
                    numeroControle = cellValue.Substring(1186, 18).Trim();
                    if (!string.IsNullOrWhiteSpace(numeroControle))
                    {
                        List<int> ctes = repCTe.BuscarPorNumeroControle(numeroControle);
                        foreach (var codigo in ctes)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);
                            if (cte != null && !string.IsNullOrWhiteSpace(numeroManifesto))
                            {
                                cte.Initialize();
                                cte.NumeroManifesto = numeroManifesto;
                                cte.NumeroCEMercante = numeroCEMertante;

                                repCTe.Atualizar(cte, Auditado);
                            }

                            unitOfWork.FlushAndClear();
                        }
                    }
                }
                linha++;
            }
            return true;
        }

        private string RetornarModeloDocumentoPH(string numeroDoc)
        {
            switch (numeroDoc)
            {
                case "55":
                    return "2";
                case "57":
                    return "2";
                default:
                    return "0";
            }
        }

        #endregion
    }
}
