using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class INTPFAR
    {
        public Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR ConverterFaturaAntigaParaINTPFAR(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Dominio.Entidades.Empresa empresaLayout, Repositorio.UnitOfWork unidadeTrabalho)
        {
            faturaIntegracao.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaIntegracao.Fatura;

            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao serConfiguracaoNaturezaOperacao = new ConfiguracaoContabil.ConfiguracaoNaturezaOperacao();


            // Repositorios
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);

            //Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto repFaturaAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto(unidadeTrabalho);

            // Objeto de valor principal
            Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR intpfar = new Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR();

            // Preenche cabecalho do EDI

            //intpfar.Destinatario = cargaPedido.ObterTomador().CPF_CNPJ.ToString();
            //intpfar.Remetente = carga.Empresa.CNPJ;

            intpfar.CabecalhosFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura>();


            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(null, null, fatura.ClienteTomadorFatura, null, empresaLayout, fatura.TipoOperacao, null, null, null, unidadeTrabalho);
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(null, null, null, null, fatura.ClienteTomadorFatura, null, empresaLayout, fatura.Empresa, fatura.TipoOperacao, null, null, null, null, unidadeTrabalho);
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = serConfiguracaoNaturezaOperacao.ObterConfiguracaoNaturezaOperacao(null, null, fatura.ClienteTomadorFatura, null, null, null, null, fatura.TipoOperacao, null, empresaLayout, fatura.Empresa, unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();


            // Preenche cabecalho da fatura

            if (configuracaoContaContabil != null && configuracaoCentroResultado != null)
            {
                //List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> documentosFatura = repFaturaCargaDocumento.BuscarDocumentosFatura(fatura.Codigo);

                List<string> tipoConsultas = new List<string>();
                tipoConsultas.Add("Isenta");
                tipoConsultas.Add("ST");
                tipoConsultas.Add("Normal");
                tipoConsultas.Add("ISS");
                tipoConsultas.Add("ISSRetido");
                for (int i = 0; i < tipoConsultas.Count; i++)
                {
                    bool modeloCTe = true;
                    bool usarCST = false;
                    List<string> CSTs = new List<string>();

                    if (tipoConsultas[i] == "Isenta")
                    {
                        usarCST = true;
                        CSTs.Add("");
                        CSTs.Add("40");
                        CSTs.Add("51");
                    }
                    else if (tipoConsultas[i] == "ST")
                    {
                        usarCST = true;
                        CSTs.Add("60");
                    }
                    else if (tipoConsultas[i] == "Normal")
                    {
                        usarCST = false;
                        CSTs.Add("");
                        CSTs.Add("40");
                        CSTs.Add("51");
                        CSTs.Add("60");
                    }
                    else if (tipoConsultas[i] == "ISS")
                    {
                        modeloCTe = false;
                    }
                    else
                        continue;

                    faturaIntegracao.CodigosCTes.AddRange(repFaturaCargaDocumento.BuscarCodigosCTesCSTs(fatura.Codigo, CSTs, usarCST, modeloCTe));

                    decimal totalReceber = repFaturaCargaDocumento.BuscarTotalReceberCSTs(fatura.Codigo, CSTs, usarCST, modeloCTe);

                    if (totalReceber <= 0)
                        continue;

                    Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();

                    int.TryParse(i.ToString() + fatura.Codigo.ToString(), out int idDocumento);
                    cabecalhoFatura.IdDocumento = idDocumento;
                    cabecalhoFatura.CodigoTransportadora = fatura?.Empresa?.CodigoIntegracao ?? "";
                    cabecalhoFatura.CNPJTomador = fatura.ClienteTomadorFatura.CPF_CNPJ_SemFormato;
                    cabecalhoFatura.CodigoReferencia = fatura.Numero.ToString() + "-" + DateTime.Now.ToString("yyyy");
                    cabecalhoFatura.DataLancamento = fatura.DataFatura;
                    cabecalhoFatura.DataVencimento = fatura.Parcelas != null && fatura.Parcelas.Count() > 0 ? (from o in fatura.Parcelas select o.DataVencimento).FirstOrDefault() : DateTime.Now;
                    cabecalhoFatura.DataEmissao = fatura.DataFatura;
                    cabecalhoFatura.TipoLancamento = "2";
                    cabecalhoFatura.CodigoCompanhia = fatura.ClienteTomadorFatura.CodigoCompanhia; //configuracaoCentroResultado.CentroResultadoContabilizacao.CodigoCompanhia;

                    cabecalhoFatura.Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>();

                    Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura ediFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura();
                    ediFatura.CNPJTransportadora = fatura.Empresa?.CNPJ_SemFormato ?? "";
                    ediFatura.CNPJCompanhia = fatura.Cliente?.CPF_CNPJ_SemFormato ?? "";
                    ediFatura.NumeroFatura = fatura.Numero.ToString() + "_" + (i + 1).ToString();
                    //ediFatura.CodigoFatura = carga.CodigoCargaEmbarcador.ToString();
                    ediFatura.DataFatura = fatura.DataFatura;
                    ediFatura.SerieFatura = ""; //cte.Serie.Numero.ToString();
                    ediFatura.DataEmissao = fatura.DataFechamento.Value;
                    ediFatura.CodigoTipoFatura = ""; //cte.ModeloDocumentoFiscal.Numero;
                    ediFatura.STFatura = tipoConsultas[i] == "ST" ? "S" : "N";
                    ediFatura.CFOP = configuracaoNaturezaOperacao != null ? configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda.CodigoIntegracao : "";

                    decimal totalDaPrestacao = repFaturaCargaDocumento.BuscarTotalDaPrestacaoComCST(fatura.Codigo, CSTs, usarCST, modeloCTe);
                    ediFatura.ValorFatura = totalReceber;
                    ediFatura.CodigoFatura = fatura.Numero.ToString();
                    if (tipoConsultas[i] == "ISS" || tipoConsultas[i] == "ISSRetido")
                        ediFatura.CodigoImposto = 8; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? 7 : 8;
                    else
                        ediFatura.CodigoImposto = 7;

                    ediFatura.BaseCalculoImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.BaseCalculoICMS : cte.BaseCalculoISS;
                    ediFatura.AliquotaImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.AliquotaICMS : cte.AliquotaISS;
                    ediFatura.ValorImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.ValorICMS : cte.ValorISS;
                    ediFatura.CNPJEmitente = fatura.Cliente?.CPF_CNPJ_SemFormato ?? ""; //cte.Remetente.CPF_CNPJ_SemFormato;

                    ediFatura.CNPJTransportadorCTe = (fatura.Empresa?.CNPJ_SemFormato ?? "") + "_1";
                    ediFatura.ItemFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura>();


                    //decimal valorBC = repFaturaDocumento.BuscarTotalBaseCalculoICMSComCSTs(fatura.Codigo, CSTs, usarCST, modeloCTe);

                    decimal valorICMS = 0;
                    if (tipoConsultas[i] == "Normal")
                        valorICMS = repFaturaCargaDocumento.BuscarTotalICMS(fatura.Codigo);

                    decimal valorICMSST = 0;
                    if (tipoConsultas[i] == "ST")
                        valorICMSST = repFaturaCargaDocumento.BuscarTotalICMSSST(fatura.Codigo);

                    decimal valorISS = 0;
                    decimal valorISSRetido = 0;
                    if (tipoConsultas[i] == "ISS")
                    {
                        valorISS = repFaturaCargaDocumento.BuscarTotalISS(fatura.Codigo);
                        valorISSRetido = repFaturaCargaDocumento.BuscarTotalISSRetido(fatura.Codigo);
                    }

                    foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
                    {

                        Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura();
                        itemFatura.GrupoTomador = fatura.ClienteTomadorFatura?.GrupoPessoas?.Descricao ?? "";
                        itemFatura.CodigoTransportador = fatura.Empresa?.CodigoIntegracao ?? "";
                        itemFatura.CodigoCliente = fatura.ClienteTomadorFatura != null ? fatura.ClienteTomadorFatura.CodigoIntegracao : "";
                        itemFatura.CodigoContaContabil = configuracao.PlanoContabilidade;
                        itemFatura.CodigoCentroCusto = configuracaoCentroResultado?.CentroResultadoContabilizacao?.PlanoContabilidade ?? "";
                        itemFatura.CodigoItem = "";
                        itemFatura.NumeroNF = fatura.Numero;
                        itemFatura.SerieNF = 0;//cte.Serie.Numero;
                        itemFatura.IdDocumento = 0;
                        itemFatura.NumeroFatura = fatura.Numero.ToString(); //cte.Numero + "/" + cte.Serie.Numero;
                        itemFatura.TipoFatura = ""; //cte.ModeloDocumentoFiscal.Numero;
                        itemFatura.DebitoOuCredito = configuracao.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito ? "C" : "D";

                        decimal basePisCofins = totalDaPrestacao;
                        if (configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins)
                            basePisCofins = totalDaPrestacao - valorICMS;


                        decimal valorEmpresaCOFINS = fatura.Empresa?.Configuracao.AliquotaCOFINS ?? 0;
                        if (valorEmpresaCOFINS == 0)
                            valorEmpresaCOFINS = fatura.Empresa.EmpresaPai?.Configuracao.AliquotaCOFINS ?? 0;

                        decimal valorCOFINS = Math.Round(basePisCofins * (valorEmpresaCOFINS / 100), 2, MidpointRounding.AwayFromZero);

                        decimal valorEmpresaPIS = fatura.Empresa?.Configuracao.AliquotaPIS ?? 0;
                        if (valorEmpresaPIS == 0)
                            valorEmpresaPIS = fatura.Empresa.EmpresaPai?.Configuracao.AliquotaPIS ?? 0;


                        decimal valorPIS = Math.Round(basePisCofins * (valorEmpresaPIS / 100), 2, MidpointRounding.AwayFromZero);


                        if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS)
                        {
                            if (valorICMS <= 0)//quando isento não insere a linha
                                continue;

                            itemFatura.ValorLancamento = valorICMS;
                            itemFatura.TipoLancamento = "11";
                            itemFatura.CodigoImposto = "70";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                        {
                            if (valorICMSST <= 0)//quando não for ST não insere a linha
                                continue;

                            itemFatura.ValorLancamento = valorICMSST;
                            itemFatura.TipoLancamento = "03";
                            itemFatura.CodigoImposto = "70";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS)
                        {
                            if (valorISS <= 0)
                                continue;

                            itemFatura.ValorLancamento = valorISS;
                            itemFatura.TipoLancamento = "11";
                            itemFatura.CodigoImposto = "80";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido)
                        {
                            if (valorISSRetido <= 0)
                                continue;

                            itemFatura.ValorLancamento = valorISSRetido;
                            itemFatura.TipoLancamento = "11";
                            itemFatura.CodigoImposto = "80";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                        {
                            if (fatura.Empresa?.Configuracao != null)
                            {
                                if (valorPIS > 0)
                                {
                                    itemFatura.ValorLancamento = valorPIS;
                                    itemFatura.TipoLancamento = "11";
                                    itemFatura.CodigoImposto = "50";
                                }
                            }
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                        {
                            if (fatura.Empresa?.Configuracao != null)
                            {
                                if (valorCOFINS > 0)
                                {
                                    itemFatura.ValorLancamento = valorCOFINS;
                                    itemFatura.TipoLancamento = "11";
                                    itemFatura.CodigoImposto = "60";
                                }
                            }
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido)
                        {
                            decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                            itemFatura.ValorLancamento = freteLiquido;
                            itemFatura.TipoLancamento = "01";
                            itemFatura.CodigoImposto = "  ";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido2)
                        {
                            decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                            //itemFatura.ValorLancamento = cte.ValorFrete;
                            itemFatura.ValorLancamento = freteLiquido;

                            itemFatura.TipoLancamento = "02";
                            itemFatura.CodigoImposto = "  ";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido9)
                        {
                            decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                            //itemFatura.ValorLancamento = cte.ValorFrete;
                            itemFatura.ValorLancamento = freteLiquido;
                            itemFatura.TipoLancamento = "09";
                            itemFatura.CodigoImposto = "  ";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber)
                        {
                            itemFatura.ValorLancamento = totalReceber;
                            itemFatura.TipoLancamento = "10";
                            itemFatura.CodigoImposto = "  ";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteValor)
                        {
                            decimal freteValor = totalReceber - valorICMS - valorISS;
                            //itemFatura.ValorLancamento = cte.ValorFrete;
                            itemFatura.ValorLancamento = freteValor;
                            itemFatura.TipoLancamento = "01";
                            itemFatura.CodigoImposto = "  ";
                        }
                        ediFatura.ItemFatura.Add(itemFatura);
                    }
                    cabecalhoFatura.Fatura.Add(ediFatura);
                    intpfar.CabecalhosFatura.Add(cabecalhoFatura);
                }
            }
            else
            {
                Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();
                if (configuracaoContaContabil == null)
                    cabecalhoFatura.CodigoTransportadora = "Sem Conta contabil";

                if (configuracaoCentroResultado == null)
                    cabecalhoFatura.CodigoTransportadora = "Sem Centro custo";
                intpfar.CabecalhosFatura.Add(cabecalhoFatura);
            }


            return intpfar;
        }

        public Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR ConverterFaturaParaINTPFAR(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Dominio.Entidades.Empresa empresaLayout, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto repFaturaAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto(unidadeTrabalho);

            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaIntegracao.Fatura;
            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(null, null, fatura.ClienteTomadorFatura, null, empresaLayout, fatura.TipoOperacao, null, null, null, unidadeTrabalho);
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(null, null, null, null, fatura.ClienteTomadorFatura, null, empresaLayout, fatura.Empresa, fatura.TipoOperacao, null, null, null, null, unidadeTrabalho);

            faturaIntegracao.CodigosCTes = new List<int>();
            Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR intpfar = new Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR
            {
                CabecalhosFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura>()
            };

            if (configuracaoContaContabil != null && configuracaoCentroResultado != null)
            {

                if (!string.IsNullOrWhiteSpace(faturaIntegracao.TipoImposto))
                {
                    List<string> CSTs = (from o in faturaIntegracao.CSTs select o.CST).ToList();
                    int indexImposto = 0;
                    if (faturaIntegracao.TipoImposto == "Isenta")
                        indexImposto = 1;
                    else if (faturaIntegracao.TipoImposto == "ST")
                        indexImposto = 2;
                    else if (faturaIntegracao.TipoImposto == "ISS")
                        indexImposto = 3;
                    else if (faturaIntegracao.TipoImposto == "ISSRetido")
                        indexImposto = 4;

                    Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = GeraINTPFARPorImpostos(configuracaoContaContabil, configuracaoCentroResultado, faturaIntegracao, empresaLayout, CSTs, faturaIntegracao.UsarCST, faturaIntegracao.ModeloCTe, faturaIntegracao.TipoImposto, indexImposto, unidadeTrabalho);
                    if (cabecalhoFatura != null)
                        intpfar.CabecalhosFatura.Add(cabecalhoFatura);
                }
                else
                {
                    List<string> tipoConsultas = new List<string>
                    {
                        "Isenta",
                        "ST",
                        "Normal",
                        "ISS",
                        "ISSRetido"
                    };

                    for (int i = 0; i < tipoConsultas.Count; i++)
                    {
                        bool modeloCTe = true;
                        bool usarCST = false;
                        List<string> CSTs = new List<string>();

                        if (tipoConsultas[i] == "Isenta")
                        {
                            usarCST = true;
                            CSTs.Add("");
                            CSTs.Add("40");
                            CSTs.Add("51");
                        }
                        else if (tipoConsultas[i] == "ST")
                        {
                            usarCST = true;
                            CSTs.Add("60");
                        }
                        else if (tipoConsultas[i] == "Normal")
                        {
                            usarCST = false;
                            CSTs.Add("");
                            CSTs.Add("40");
                            CSTs.Add("51");
                            CSTs.Add("60");
                        }
                        else if (tipoConsultas[i] == "ISS")
                        {
                            modeloCTe = false;
                        }
                        else
                            continue;

                        Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = GeraINTPFARPorImpostos(configuracaoContaContabil, configuracaoCentroResultado, faturaIntegracao, empresaLayout, CSTs, usarCST, modeloCTe, tipoConsultas[i], i, unidadeTrabalho);
                        if (cabecalhoFatura != null)
                            intpfar.CabecalhosFatura.Add(cabecalhoFatura);
                    }
                }
            }
            else
            {
                Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();
                if (configuracaoContaContabil == null)
                    cabecalhoFatura.CodigoTransportadora = "Sem Conta contabil";

                if (configuracaoCentroResultado == null)
                    cabecalhoFatura.CodigoTransportadora = "Sem Centro custo";

                intpfar.CabecalhosFatura.Add(cabecalhoFatura);
            }



            return intpfar;
        }

        public Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR ConverterFaturaParaINTPFARPorCte(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Dominio.Entidades.Empresa empresaLayout, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaIntegracao.Fatura;
            ConfiguracaoContabil.ConfiguracaoCentroResultado servicoConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            ConfiguracaoContabil.ConfiguracaoContaContabil servicoConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();
            ConfiguracaoContabil.ConfiguracaoNaturezaOperacao servicoConfiguracaoNaturezaOperacao = new ConfiguracaoContabil.ConfiguracaoNaturezaOperacao();
            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = servicoConfiguracaoContaContabil.ObterConfiguracaoContaContabil(null, null, fatura.ClienteTomadorFatura, null, empresaLayout, fatura.TipoOperacao, null, null, null, unidadeTrabalho);
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = servicoConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(null, null, null, null, fatura.ClienteTomadorFatura, null, empresaLayout, fatura.Empresa, fatura.TipoOperacao, null, null, null, null, unidadeTrabalho);
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = servicoConfiguracaoNaturezaOperacao.ObterConfiguracaoNaturezaOperacao(null, null, fatura.ClienteTomadorFatura, null, null, null, null, fatura.TipoOperacao, null, empresaLayout, fatura.Empresa, unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR intpfar = new Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR()
            {
                CabecalhosFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura>()
            };

            if (configuracaoContaContabil != null && configuracaoCentroResultado != null)
            {
                Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = (from o in fatura.Documentos where o.Documento.CTe != null select o.Documento.CTe).ToList();

                cabecalhoFatura.IdDocumento = fatura.Codigo;
                cabecalhoFatura.CodigoCompanhia = fatura.ClienteTomadorFatura?.CodigoCompanhia;
                cabecalhoFatura.CodigoTransportadora = fatura?.Transportador?.CodigoIntegracao ?? "";
                cabecalhoFatura.CNPJTomador = fatura.ClienteTomadorFatura?.CPF_CNPJ_SemFormato;
                cabecalhoFatura.CodigoReferencia = fatura.Numero.ToString() + "-" + DateTime.Now.ToString("yyyy");
                cabecalhoFatura.DataLancamento = fatura.DataFatura;
                cabecalhoFatura.DataVencimento = fatura.Parcelas != null && fatura.Parcelas.Count() > 0 ? (from o in fatura.Parcelas select o.DataVencimento).FirstOrDefault() : DateTime.Now;
                cabecalhoFatura.DataEmissao = fatura.DataFatura;
                cabecalhoFatura.TipoLancamento = "2";
                cabecalhoFatura.Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>();

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura ediFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura();

                    ediFatura.CNPJTransportadora = cte.Empresa.CNPJ_SemFormato;
                    ediFatura.CNPJCompanhia = cte.TomadorPagador.CPF_CNPJ_SemFormato;
                    ediFatura.NumeroFatura = cte.Numero.ToString();

                    string ultimosCaracteresCodigoEmpresa = "";
                    if (cabecalhoFatura.CodigoTransportadora.Length > 5)
                        ultimosCaracteresCodigoEmpresa = cabecalhoFatura.CodigoTransportadora.Substring(cabecalhoFatura.CodigoTransportadora.Length - 5);
                    else
                        ultimosCaracteresCodigoEmpresa = cabecalhoFatura.CodigoTransportadora;

                    ediFatura.NumeroFaturaCodTransportador = ultimosCaracteresCodigoEmpresa + ediFatura.NumeroFatura;

                    if (ediFatura.NumeroFaturaCodTransportador.Length > 12)
                        ediFatura.NumeroFaturaCodTransportador = ediFatura.NumeroFaturaCodTransportador.Substring(0, 12);

                    ediFatura.DataFatura = fatura.DataFatura;
                    ediFatura.SerieFatura = cte.Serie.Numero.ToString();
                    ediFatura.DataEmissao = cte.DataEmissao.Value;
                    ediFatura.CodigoTipoFatura = cte.ModeloDocumentoFiscal.Numero;
                    ediFatura.STFatura = cte.CST == "60" ? "S" : "N";
                    ediFatura.CFOP = configuracaoNaturezaOperacao != null ? configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda.CodigoIntegracao : "";
                    ediFatura.ValorFatura = cte.ValorAReceber;
                    ediFatura.CodigoFatura = fatura.Numero.ToString();
                    ediFatura.CodigoImposto = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? 7 : 8;
                    ediFatura.BaseCalculoImposto = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.BaseCalculoICMS : cte.BaseCalculoISS;
                    ediFatura.AliquotaImposto = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.AliquotaICMS : cte.AliquotaISS;
                    ediFatura.ValorImposto = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.ValorICMS : cte.ValorISS;
                    ediFatura.CNPJEmitente = cte.Remetente.CPF_CNPJ_SemFormato;
                    ediFatura.ItemFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura>();
                    ediFatura.CNPJTransportadorCTe = cte.Empresa.CNPJ_SemFormato + "_" + cte.Serie.ToString();

                    foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
                    {
                        Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura();

                        itemFatura.CodigoTransportador = cte.Empresa.CodigoIntegracao;
                        itemFatura.GrupoTomador = cte.TomadorPagador.Cliente.GrupoPessoas?.Descricao ?? "";
                        itemFatura.CodigoContaContabil = configuracao.PlanoContabilidade;
                        itemFatura.CodigoCentroCusto = configuracaoCentroResultado?.CentroResultadoContabilizacao?.PlanoContabilidade ?? "";
                        itemFatura.CodigoItem = "";
                        itemFatura.NumeroNF = cte.Numero;
                        itemFatura.SerieNF = cte.Serie.Numero;
                        itemFatura.IdDocumento = 0;
                        itemFatura.NumeroFatura = cte.Numero + "/" + cte.Serie.Numero;
                        itemFatura.TipoFatura = cte.ModeloDocumentoFiscal.Numero;
                        itemFatura.DebitoOuCredito = configuracao.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito ? "C" : "D";

                        decimal basePisCofins = cte.ValorPrestacaoServico;
                        if (configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                            basePisCofins = cte.ValorPrestacaoServico - cte.ValorICMS;


                        decimal valorEmpresaCOFINS = cte.Empresa.Configuracao.AliquotaCOFINS ?? 0;

                        if (valorEmpresaCOFINS == 0)
                            valorEmpresaCOFINS = fatura.Empresa.EmpresaPai?.Configuracao.AliquotaCOFINS ?? 0;

                        decimal valorCOFINS = Math.Round(basePisCofins * (valorEmpresaCOFINS / 100), 2, MidpointRounding.AwayFromZero);
                        decimal valorEmpresaPIS = cte.Empresa.Configuracao.AliquotaPIS ?? 0;

                        if (valorEmpresaPIS == 0)
                            valorEmpresaPIS = fatura.Empresa.EmpresaPai?.Configuracao.AliquotaPIS ?? 0;

                        decimal valorPIS = Math.Round(basePisCofins * (valorEmpresaPIS / 100), 2, MidpointRounding.AwayFromZero);

                        if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS)
                        {
                            if (cte.CST == "40" || cte.CST == "")
                                continue;

                            itemFatura.ValorLancamento = cte.ValorICMS;
                            itemFatura.TipoLancamento = "11";
                            itemFatura.CodigoImposto = "70";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                        {
                            if (cte.CST != "60")//quando não for ST não insere a linha
                                continue;

                            itemFatura.ValorLancamento = cte.ValorICMS;
                            itemFatura.TipoLancamento = "03";
                            itemFatura.CodigoImposto = "70";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                        {
                            if (cte.Empresa.Configuracao != null)
                            {
                                if (valorPIS > 0)
                                {
                                    itemFatura.ValorLancamento = valorPIS;
                                    itemFatura.TipoLancamento = "11";
                                    itemFatura.CodigoImposto = "50";
                                }
                            }
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                        {
                            if (cte.Empresa.Configuracao != null)
                            {
                                if (valorCOFINS > 0)
                                {
                                    itemFatura.ValorLancamento = valorCOFINS;
                                    itemFatura.TipoLancamento = "11";
                                    itemFatura.CodigoImposto = "60";
                                }
                            }
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido)
                        {
                            decimal freteLiquido = cte.ValorAReceber - valorPIS - valorCOFINS - (cte.CST != "60" ? cte.ValorICMS : 0);

                            itemFatura.ValorLancamento = freteLiquido;
                            itemFatura.TipoLancamento = "01";
                            itemFatura.CodigoImposto = "  ";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido2)
                        {
                            decimal freteLiquido = cte.ValorAReceber - valorPIS - valorCOFINS - (cte.CST != "60" ? cte.ValorICMS : 0);

                            itemFatura.ValorLancamento = freteLiquido;
                            itemFatura.TipoLancamento = "02";
                            itemFatura.CodigoImposto = "  ";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido9)
                        {
                            decimal freteLiquido = cte.ValorAReceber - valorPIS - valorCOFINS - (cte.CST != "60" ? cte.ValorICMS : 0);

                            itemFatura.ValorLancamento = freteLiquido;
                            itemFatura.TipoLancamento = "09";
                            itemFatura.CodigoImposto = "  ";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber)
                        {
                            itemFatura.ValorLancamento = cte.ValorAReceber;
                            itemFatura.TipoLancamento = "10";
                            itemFatura.CodigoImposto = "  ";
                        }
                        else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteValor)
                        {
                            decimal freteValor = cte.ValorAReceber - (cte.CST != "60" ? cte.ValorICMS : 0);

                            itemFatura.ValorLancamento = freteValor;
                            itemFatura.TipoLancamento = "01";
                            itemFatura.CodigoImposto = "  ";
                        }

                        ediFatura.ItemFatura.Add(itemFatura);
                    }

                    cabecalhoFatura.Fatura.Add(ediFatura);
                }

                intpfar.CabecalhosFatura.Add(cabecalhoFatura);
            }
            else
            {
                Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();

                if (configuracaoContaContabil == null)
                    cabecalhoFatura.CodigoTransportadora = "Sem Conta contabil";

                if (configuracaoCentroResultado == null)
                    cabecalhoFatura.CodigoTransportadora = "Sem Centro custo";

                intpfar.CabecalhosFatura.Add(cabecalhoFatura);
            }

            return intpfar;
        }

        private int ajustaMesAno(DateTime d)
        {
            return d.Year * 12 + d.Month;
        }

        public Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR ConverterPagamentoParaINTPFAR(Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDI, Repositorio.UnitOfWork unidadeTrabalho)
        {
            pagamentoEDI.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = pagamentoEDI.Pagamento;

            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentoContabils = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil>();

            documentoContabils.AddRange(repDocumentoContabil.BuscarNFSParaGeracaoEDIPorPagamento(pagamento.Codigo, pagamento.LotePagamentoLiberado));
            documentoContabils.AddRange(repDocumentoContabil.BuscarCTesParaGeracaoEDIPorPagamento(pagamento.Codigo, pagamento.LotePagamentoLiberado));

            if (pagamentoEDI.Inicio > 0 || pagamentoEDI.Limite > 0)
                documentoContabils = documentoContabils.Skip(pagamentoEDI.Inicio).Take(pagamentoEDI.Limite).ToList();

            List<string> empresas = (from obj in documentoContabils select obj.CNPJEmpresa).Distinct().ToList();

            Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR intpfar = new Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR();
            intpfar.CabecalhosFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura>();
            intpfar.Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>();
            intpfar.RodapeFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.RodapeFatura();
            intpfar.DataGeracao = DateTime.Now;

            for (int e = 0; e < empresas.Count; e++)
            {
                string cnpjEmpresa = empresas[e];
                string codigoEmpresa = (from obj in documentoContabils where obj.CNPJEmpresa == cnpjEmpresa select obj.CodigoEmpresa).FirstOrDefault();
                List<double> tomadores = (from obj in documentoContabils where obj.CNPJEmpresa == cnpjEmpresa select obj.CNPJCPFTomador).Distinct().ToList();

                for (int t = 0; t < tomadores.Count; t++)
                {
                    double cnpjTomador = tomadores[t];
                    Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil documentoTomador = (from obj in documentoContabils where obj.CNPJCPFTomador == cnpjTomador select obj).FirstOrDefault();
                    List<DateTime?> datasVencimento = (from obj in documentoContabils where obj.CNPJEmpresa == cnpjEmpresa && obj.CNPJCPFTomador == cnpjTomador select obj.DataVencimento).Distinct().ToList();

                    for (int dt = 0; dt < datasVencimento.Count; dt++)
                    {
                        DateTime? dataVencimento = datasVencimento[dt];

                        List<DateTime> datasEmissao = (from obj in documentoContabils where obj.CNPJEmpresa == cnpjEmpresa && obj.CNPJCPFTomador == cnpjTomador && obj.DataVencimento == dataVencimento select obj.DataEmissao).Distinct().ToList();

                        for (int de = 0; de < datasEmissao.Count; de++)
                        {
                            DateTime dataEmissao = datasEmissao[de];
                            // Preenche cabecalho da fatura
                            Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();
                            int.TryParse(dt.ToString() + pagamento.Codigo.ToString(), out int idDocumento);
                            cabecalhoFatura.IdDocumento = idDocumento;
                            cabecalhoFatura.CodigoTransportadora = codigoEmpresa;
                            cabecalhoFatura.CNPJTomador = documentoTomador.CPF_CNPJ_Tomador_SemFormato;
                            cabecalhoFatura.CodigoReferencia = pagamento.Numero.ToString() + "-" + DateTime.Now.ToString("yyyy");
                            cabecalhoFatura.DataLancamento = pagamento.DataCriacao;
                            cabecalhoFatura.DataVencimento = dataVencimento ?? DateTime.Now;
                            cabecalhoFatura.TipoLancamento = "2";
                            cabecalhoFatura.DataEmissao = dataEmissao;

                            cabecalhoFatura.Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>();

                            List<int> codigosCTes = (from obj in documentoContabils where obj.CNPJEmpresa == cnpjEmpresa && obj.CNPJCPFTomador == cnpjTomador && obj.DataVencimento == dataVencimento && obj.DataEmissao == dataEmissao select obj.CodigoCTe).Distinct().ToList();

                            for (int i = 0; i < codigosCTes.Count; i++)
                            {
                                int codigoCTe = codigosCTes[i];
                                pagamentoEDI.CodigosCTes.Add(codigoCTe);

                                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentosContabeis = (from obj in documentoContabils where obj.CodigoCTe == codigoCTe select obj).ToList();
                                Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil documentoCTe = documentosContabeis.FirstOrDefault();

                                Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura ediFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura();
                                ediFatura.CNPJTransportadora = documentoCTe.CNPJEmpresa;
                                ediFatura.CNPJCompanhia = documentoCTe.CPF_CNPJ_Tomador_SemFormato;
                                ediFatura.NumeroFatura = documentoCTe.Numero.ToString();
                                string ultimosCaracteresCodigoEmpresa = "";
                                if (cabecalhoFatura.CodigoTransportadora.Length > 5)
                                    ultimosCaracteresCodigoEmpresa = cabecalhoFatura.CodigoTransportadora.Substring(cabecalhoFatura.CodigoTransportadora.Length - 5);
                                else
                                    ultimosCaracteresCodigoEmpresa = cabecalhoFatura.CodigoTransportadora;

                                ediFatura.NumeroFaturaCodTransportador = ultimosCaracteresCodigoEmpresa + ediFatura.NumeroFatura;

                                if (ediFatura.NumeroFaturaCodTransportador.Length > 12)
                                    ediFatura.NumeroFaturaCodTransportador = ediFatura.NumeroFaturaCodTransportador.Substring(0, 12);

                                ediFatura.DataFatura = pagamento.DataCriacao;
                                ediFatura.SerieFatura = documentoCTe.Serie.ToString();
                                ediFatura.DataEmissao = documentoCTe.DataEmissao;

                                if ((pagamento.DataCriacao.Year > documentoCTe.DataEmissao.Year || pagamento.DataCriacao.Month > documentoCTe.DataEmissao.Month))
                                {
                                    DateTime dataContabil = new DateTime(pagamento.DataCriacao.Year, pagamento.DataCriacao.Month, 1);

                                    int mesesDiff = ajustaMesAno(pagamento.DataCriacao) - ajustaMesAno(documentoCTe.DataEmissao);
                                    if (mesesDiff > 2)
                                        ediFatura.DataEmissaoNCMesContabil = dataContabil.AddMonths(-1);
                                    else
                                    {
                                        if (pagamento.DataCriacao.Day >= 2)
                                            ediFatura.DataEmissaoNCMesContabil = new DateTime(pagamento.DataCriacao.Year, pagamento.DataCriacao.Month, 1);
                                        else
                                            ediFatura.DataEmissaoNCMesContabil = documentoCTe.DataEmissao;
                                    }
                                }
                                else
                                    ediFatura.DataEmissaoNCMesContabil = documentoCTe.DataEmissao;


                                ediFatura.TipoModeloDocFatura = documentoCTe.NumeroModelo;
                                if (ediFatura.TipoModeloDocFatura == "57")
                                    ediFatura.TipoModeloDocFatura = "CE";
                                else if (ediFatura.TipoModeloDocFatura == "39" || ediFatura.TipoModeloDocFatura == "NF")
                                    ediFatura.TipoModeloDocFatura = "NE";
                                else if (ediFatura.TipoModeloDocFatura == "ND")
                                {
                                    if (!string.IsNullOrWhiteSpace(documentoCTe.PrefixoOcorrenciaOutrosDocumentos))
                                        ediFatura.TipoModeloDocFatura = documentoCTe.PrefixoOcorrenciaOutrosDocumentos;
                                    else
                                        ediFatura.TipoModeloDocFatura = "DE";
                                }


                                ediFatura.CodigoTipoFatura = documentoCTe.NumeroModelo;
                                ediFatura.STFatura = documentoCTe.CST == "60" ? "S" : "N";
                                ediFatura.CFOP = "";//configuracaoNaturezaOperacao != null ? configuracaoNaturezaOperacao.NaturezaDaOperacaoContabilizacao.CodigoIntegracao : "";
                                ediFatura.ValorFatura = documentoCTe.ValorAReceber;

                                if (documentoCTe.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                    ediFatura.CodigoFatura = documentoCTe.NumeroOcorrencia.HasValue ? documentoCTe.NumeroOcorrencia.ToString() : documentoCTe.NumeroCarga;
                                else
                                    ediFatura.CodigoFatura = pagamento.Numero + "/" + documentoCTe.Numero.ToString();

                                ediFatura.CodigoImposto = documentoCTe.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? 7 : 8;
                                ediFatura.BaseCalculoImposto = documentoCTe.BaseCalculoImposto;
                                ediFatura.AliquotaImposto = documentoCTe.AliquotaISS;
                                ediFatura.ValorImposto = documentoCTe.ValorImposto;
                                ediFatura.CNPJEmitente = documentoCTe.CPF_CNPJ_Remetente_SemFormato;
                                ediFatura.DataLancamento = pagamento.DataCriacao;
                                ediFatura.CNPJTransportadorCTe = documentoCTe.CNPJEmpresa + "_" + documentoCTe.Serie.ToString();
                                ediFatura.ItemFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura>();

                                foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil documentoContabil in documentosContabeis)
                                {
                                    Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura();

                                    itemFatura.CodigoTransportador = codigoEmpresa;
                                    if (documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.ContasAPagar
                                        || documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.ContasAReceber
                                        || documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.NotaDebito)
                                    {
                                        itemFatura.CodigoEmpresa = codigoEmpresa;
                                        itemFatura.CNPJTransportador = cnpjEmpresa;
                                    }

                                    itemFatura.GrupoTomador = !string.IsNullOrWhiteSpace(documentoCTe.GrupoTomador) ? documentoCTe.GrupoTomador : "";
                                    itemFatura.CodigoCliente = documentoCTe.CodigoTomador;
                                    itemFatura.CodigoContaContabil = documentoContabil.CodigoContaContabil;
                                    itemFatura.CodigoCentroCusto = documentoContabil.CodigoCentroResultado;
                                    itemFatura.CodigoItem = "";
                                    itemFatura.TipoModeloDocFatura = ediFatura.TipoModeloDocFatura;
                                    itemFatura.NumeroNF = documentoCTe.Numero;
                                    itemFatura.DataVencimento = documentoCTe.DataVencimento ?? documentoCTe.DataEmissao;
                                    itemFatura.SerieNF = documentoCTe.Serie;
                                    itemFatura.IdDocumento = 0;
                                    itemFatura.ValorLancamento = Math.Round(documentoContabil.ValorContabilizacao, 2, MidpointRounding.AwayFromZero);
                                    itemFatura.NumeroFatura = documentoCTe.Numero + "/" + documentoCTe.Serie;
                                    itemFatura.TipoFatura = documentoCTe.NumeroModelo;
                                    itemFatura.DebitoOuCredito = documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito ? "C" : "D";
                                    if (documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito)
                                        itemFatura.TipoDebitoOuCredito = "50";
                                    else if (documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito)
                                        itemFatura.TipoDebitoOuCredito = "40";
                                    else if (documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.NotaDebito)
                                        itemFatura.TipoDebitoOuCredito = "09";
                                    else if (documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.ContasAPagar)
                                        itemFatura.TipoDebitoOuCredito = "31";
                                    else if (documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.ContasAReceber)
                                        itemFatura.TipoDebitoOuCredito = "21";

                                    if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS)
                                    {
                                        itemFatura.TipoLancamento = "11";
                                        itemFatura.CodigoImposto = "70";
                                    }
                                    else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                                    {
                                        itemFatura.TipoLancamento = "03";
                                        itemFatura.CodigoImposto = "70";
                                    }
                                    else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS)
                                    {
                                        itemFatura.TipoLancamento = "11";
                                        itemFatura.CodigoImposto = "80";
                                    }
                                    else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido)
                                    {
                                        itemFatura.TipoLancamento = "11";
                                        itemFatura.CodigoImposto = "80";
                                    }
                                    else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                                    {
                                        itemFatura.TipoLancamento = "11";
                                        itemFatura.CodigoImposto = "50";
                                    }
                                    else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                                    {
                                        itemFatura.TipoLancamento = "11";
                                        itemFatura.CodigoImposto = "60";
                                    }
                                    else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido)
                                    {
                                        itemFatura.TipoLancamento = "01";
                                        itemFatura.CodigoImposto = "  ";
                                    }
                                    else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteValor)
                                    {
                                        itemFatura.TipoLancamento = "01";
                                        itemFatura.CodigoImposto = "  ";
                                    }
                                    if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido2)
                                    {
                                        itemFatura.TipoLancamento = "02";
                                        itemFatura.CodigoImposto = "  ";
                                    }
                                    if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido9)
                                    {
                                        itemFatura.TipoLancamento = "09";
                                        itemFatura.CodigoImposto = "  ";
                                    }
                                    else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber)
                                    {
                                        itemFatura.TipoLancamento = "10";
                                        itemFatura.CodigoImposto = "  ";
                                    }
                                    //regra fixa para grupo big, onde quando for iss ou frete valor a conta pode ser a mesma neste casos eles pediram para sumarizar tudo em uma linha, se sugir a necessidade de outros parametros ou outro cliente vamos criar uma configuração para determinar se agrupa ou não.

                                    bool somar = true;
                                    if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteValor
                                        || documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS)
                                    {
                                        Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFaturaExiste = (from obj in ediFatura.ItemFatura where obj.TipoDebitoOuCredito == itemFatura.TipoDebitoOuCredito && obj.CodigoContaContabil == itemFatura.CodigoContaContabil && obj.CodigoCentroCusto == itemFatura.CodigoCentroCusto && obj.NumeroFatura == itemFatura.NumeroFatura select obj).FirstOrDefault();
                                        if (itemFaturaExiste != null)
                                        {
                                            itemFaturaExiste.ValorLancamento += itemFatura.ValorLancamento;
                                            somar = false;
                                        }
                                        else
                                        {
                                            ediFatura.ItemFatura.Add(itemFatura);
                                            intpfar.RodapeFatura.Somatorios++;
                                        }
                                    }
                                    else
                                    {
                                        ediFatura.ItemFatura.Add(itemFatura);
                                        intpfar.RodapeFatura.Somatorios++;
                                    }

                                    intpfar.ValorTotal += itemFatura.ValorLancamento;

                                    if (somar)
                                        intpfar.TotalLancamentos++;

                                }

                                cabecalhoFatura.Fatura.Add(ediFatura);
                                intpfar.Fatura.Add(ediFatura);
                                intpfar.RodapeFatura.Contadores++;
                            }

                            intpfar.CabecalhosFatura.Add(cabecalhoFatura);
                        }
                    }
                }
            }

            string extensao = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterExtencaoPadrao(pagamentoEDI.LayoutEDI);
            intpfar.NomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(pagamentoEDI, false, unidadeTrabalho);
            intpfar.NomeArquivoSemExtencao = intpfar.NomeArquivo.Replace(extensao, "");

            return intpfar;
        }

        public Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR ConverterCancelamentoProvisaoParaINTPFAR(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Repositorio.UnitOfWork unidadeTrabalho)
        {

            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentoContabils = repDocumentoContabil.BuscarParaGeracaoEDIPorCancelamentoProvisao(cancelamentoProvisao.Codigo);

            List<double> tomadores = (from obj in documentoContabils select obj.CNPJCPFTomador).Distinct().ToList();

            // Objeto de valor principal
            Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR intpfar = new Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR();

            // Preenche cabecalho do EDI

            //intpfar.Destinatario = cargaPedido.ObterTomador().CPF_CNPJ.ToString();
            //intpfar.Remetente = carga.Empresa.CNPJ;

            intpfar.CabecalhosFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura>();

            for (int i = 0; i < tomadores.Count; i++)
            {
                double cpnjtomador = tomadores[i];
                Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(cpnjtomador);

                List<string> centrosCusto = (from obj in documentoContabils where obj.CNPJCPFTomador == cpnjtomador select obj.CodigoCentroResultado).Distinct().ToList();

                for (int j = 0; j < centrosCusto.Count; j++)
                {
                    List<string> csts = (from obj in documentoContabils where obj.CNPJCPFTomador == cpnjtomador && obj.CodigoCentroResultado == centrosCusto[j] select obj.CST).Distinct().ToList();

                    for (int c = 0; c < csts.Count; c++)
                    {
                        // Preenche cabecalho da fatura
                        Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();

                        int.TryParse(c.ToString() + cancelamentoProvisao.Codigo.ToString(), out int idDocumento);
                        cabecalhoFatura.IdDocumento = idDocumento;
                        cabecalhoFatura.CodigoTransportadora = "";
                        cabecalhoFatura.CNPJTomador = tomador.CPF_CNPJ_SemFormato;
                        cabecalhoFatura.CodigoReferencia = cancelamentoProvisao.Numero.ToString() + "-" + DateTime.Now.ToString("yyyy");
                        cabecalhoFatura.DataLancamento = cancelamentoProvisao.DataLancamento.HasValue ? cancelamentoProvisao.DataLancamento.Value : cancelamentoProvisao.DataCriacao;
                        cabecalhoFatura.DataVencimento = DateTime.Now;
                        cabecalhoFatura.DataEmissao = cancelamentoProvisao.DataCriacao;
                        cabecalhoFatura.TipoLancamento = "3";
                        cabecalhoFatura.CodigoCompanhia = ""; //fatura.ClienteTomadorFatura.CodigoCompanhia; //configuracaoCentroResultado.CentroResultadoContabilizacao.CodigoCompanhia;

                        cabecalhoFatura.Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>();

                        List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentoContabilTomador = (from obj in documentoContabils where obj.CNPJCPFTomador == cpnjtomador && obj.CodigoCentroResultado == centrosCusto[j] && obj.CST == csts[c] select obj).ToList();

                        Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura ediFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura();
                        ediFatura.CNPJTransportadora = "";
                        ediFatura.CNPJCompanhia = tomador.CPF_CNPJ_SemFormato;
                        ediFatura.NumeroFatura = cancelamentoProvisao.Numero.ToString() + "_" + (i + 1).ToString();

                        //ediFatura.CodigoFatura = carga.CodigoCargaEmbarcador.ToString();
                        ediFatura.DataFatura = cancelamentoProvisao.DataCriacao;
                        ediFatura.SerieFatura = ""; //cte.Serie.Numero.ToString();
                        ediFatura.DataEmissao = cancelamentoProvisao.DataCriacao;
                        ediFatura.CodigoTipoFatura = ""; //cte.ModeloDocumentoFiscal.Numero;


                        ediFatura.STFatura = documentoContabilTomador.Any(obj => obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST) ? "S" : "N";
                        ediFatura.CFOP = "";

                        decimal totalReceber = (from obj in documentoContabilTomador where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber select obj.ValorContabilizacao).Sum();
                        ediFatura.ValorFatura = totalReceber;
                        ediFatura.CodigoFatura = cancelamentoProvisao.Numero.ToString();
                        if (documentoContabilTomador.Any(obj => obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS) || documentoContabilTomador.Any(obj => obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido))
                            ediFatura.CodigoImposto = 8;
                        else
                            ediFatura.CodigoImposto = 7;

                        ediFatura.BaseCalculoImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.BaseCalculoICMS : cte.BaseCalculoISS;
                        ediFatura.AliquotaImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.AliquotaICMS : cte.AliquotaISS;
                        ediFatura.ValorImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.ValorICMS : cte.ValorISS;
                        ediFatura.CNPJEmitente = tomador.CPF_CNPJ_SemFormato; //cte.Remetente.CPF_CNPJ_SemFormato;
                        ediFatura.ItemFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura>();

                        foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil documentoContabil in documentoContabilTomador)
                        {
                            Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura();

                            itemFatura.CodigoTransportador = "";
                            itemFatura.GrupoTomador = tomador.GrupoPessoas?.Descricao ?? "";
                            itemFatura.CodigoCliente = tomador.CodigoIntegracao;
                            itemFatura.CodigoContaContabil = documentoContabil.CodigoContaContabil;
                            itemFatura.CodigoCentroCusto = documentoContabil.CodigoCentroResultado;
                            itemFatura.CodigoItem = "";
                            itemFatura.NumeroNF = cancelamentoProvisao.Numero;
                            itemFatura.SerieNF = 0;//cte.Serie.Numero;
                            itemFatura.IdDocumento = 0;
                            itemFatura.NumeroFatura = cancelamentoProvisao.Numero.ToString(); //cte.Numero + "/" + cte.Serie.Numero;
                            itemFatura.TipoFatura = ""; //cte.ModeloDocumentoFiscal.Numero;
                            itemFatura.DebitoOuCredito = documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito ? "C" : "D";
                            itemFatura.ValorLancamento = documentoContabil.ValorContabilizacao;
                            if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "70";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                            {
                                itemFatura.TipoLancamento = "03";
                                itemFatura.CodigoImposto = "70";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "80";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "80";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "50";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "60";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido)
                            {
                                itemFatura.TipoLancamento = "01";
                                itemFatura.CodigoImposto = "  ";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteValor)
                            {
                                itemFatura.TipoLancamento = "01";
                                itemFatura.CodigoImposto = "  ";
                            }
                            if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido2)
                            {
                                itemFatura.TipoLancamento = "02";
                                itemFatura.CodigoImposto = "  ";
                            }
                            if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido9)
                            {
                                itemFatura.TipoLancamento = "09";
                                itemFatura.CodigoImposto = "  ";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber)
                            {
                                itemFatura.TipoLancamento = "10";
                                itemFatura.CodigoImposto = "  ";
                            }
                            ediFatura.ItemFatura.Add(itemFatura);
                        }

                        cabecalhoFatura.Fatura.Add(ediFatura);

                        intpfar.CabecalhosFatura.Add(cabecalhoFatura);
                    }
                }

            }
            return intpfar;
        }

        public Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR ConverterProvisaoParaINTPFAR(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, Repositorio.UnitOfWork unidadeTrabalho)
        {

            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentoContabils = repDocumentoContabil.BuscarParaGeracaoEDIPorProvisao(provisao.Codigo);

            List<double> tomadores = (from obj in documentoContabils select obj.CNPJCPFTomador).Distinct().ToList();



            // Objeto de valor principal
            Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR intpfar = new Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR();

            // Preenche cabecalho do EDI

            //intpfar.Destinatario = cargaPedido.ObterTomador().CPF_CNPJ.ToString();
            //intpfar.Remetente = carga.Empresa.CNPJ;

            intpfar.CabecalhosFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura>();


            for (int i = 0; i < tomadores.Count; i++)
            {
                double cpnjtomador = tomadores[i];

                List<string> centrosCusto = (from obj in documentoContabils where obj.CNPJCPFTomador == cpnjtomador select obj.CodigoCentroResultado).Distinct().ToList();

                for (int j = 0; j < centrosCusto.Count; j++)
                {

                    List<string> csts = (from obj in documentoContabils where obj.CNPJCPFTomador == cpnjtomador && obj.CodigoCentroResultado == centrosCusto[j] select obj.CST).Distinct().ToList();

                    for (int c = 0; c < csts.Count; c++)
                    {

                        Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(cpnjtomador);

                        // Preenche cabecalho da fatura
                        Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();

                        int.TryParse(c.ToString() + provisao.Codigo.ToString(), out int idDocumento);
                        cabecalhoFatura.IdDocumento = idDocumento;
                        cabecalhoFatura.CodigoTransportadora = "";
                        cabecalhoFatura.CNPJTomador = tomador.CPF_CNPJ_SemFormato;
                        cabecalhoFatura.CodigoReferencia = provisao.Numero.ToString() + "-" + DateTime.Now.ToString("yyyy");
                        cabecalhoFatura.DataLancamento = provisao.DataLancamento.HasValue ? provisao.DataLancamento.Value : provisao.DataCriacao;
                        cabecalhoFatura.DataVencimento = DateTime.Now;
                        cabecalhoFatura.DataEmissao = provisao.DataCriacao;
                        cabecalhoFatura.TipoLancamento = "1";
                        cabecalhoFatura.CodigoCompanhia = ""; //fatura.ClienteTomadorFatura.CodigoCompanhia; //configuracaoCentroResultado.CentroResultadoContabilizacao.CodigoCompanhia;

                        cabecalhoFatura.Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>();

                        List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentoContabilTomador = (from obj in documentoContabils where obj.CNPJCPFTomador == cpnjtomador && obj.CodigoCentroResultado == centrosCusto[j] && obj.CST == csts[c] select obj).ToList();

                        Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura ediFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura();
                        ediFatura.CNPJTransportadora = "";
                        ediFatura.CNPJCompanhia = tomador.CPF_CNPJ_SemFormato;
                        ediFatura.NumeroFatura = provisao.Numero.ToString() + "_" + (i + 1).ToString();
                        //ediFatura.CodigoFatura = carga.CodigoCargaEmbarcador.ToString();
                        ediFatura.DataFatura = provisao.DataCriacao;
                        ediFatura.SerieFatura = ""; //cte.Serie.Numero.ToString();
                        ediFatura.DataEmissao = provisao.DataCriacao;
                        ediFatura.CodigoTipoFatura = ""; //cte.ModeloDocumentoFiscal.Numero;


                        ediFatura.STFatura = documentoContabilTomador.Any(obj => obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST) ? "S" : "N";
                        ediFatura.CFOP = "";

                        decimal totalReceber = (from obj in documentoContabilTomador where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber select obj.ValorContabilizacao).Sum();
                        ediFatura.ValorFatura = totalReceber;
                        ediFatura.CodigoFatura = provisao.Numero.ToString();
                        if (documentoContabilTomador.Any(obj => obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS) || documentoContabilTomador.Any(obj => obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido))
                            ediFatura.CodigoImposto = 8;
                        else
                            ediFatura.CodigoImposto = 7;

                        ediFatura.BaseCalculoImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.BaseCalculoICMS : cte.BaseCalculoISS;
                        ediFatura.AliquotaImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.AliquotaICMS : cte.AliquotaISS;
                        ediFatura.ValorImposto = 0; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.ValorICMS : cte.ValorISS;
                        ediFatura.CNPJEmitente = tomador.CPF_CNPJ_SemFormato; //cte.Remetente.CPF_CNPJ_SemFormato;
                        ediFatura.ItemFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura>();

                        foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil documentoContabil in documentoContabilTomador)
                        {
                            Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura();
                            itemFatura.GrupoTomador = tomador.GrupoPessoas?.Descricao ?? "";
                            itemFatura.CodigoTransportador = "";
                            itemFatura.CodigoCliente = tomador.CodigoIntegracao;
                            itemFatura.CodigoContaContabil = documentoContabil.CodigoContaContabil;
                            itemFatura.CodigoCentroCusto = documentoContabil.CodigoCentroResultado;
                            itemFatura.CodigoItem = "";
                            itemFatura.NumeroNF = provisao.Numero;
                            itemFatura.SerieNF = 0;//cte.Serie.Numero;
                            itemFatura.IdDocumento = 0;
                            itemFatura.NumeroFatura = provisao.Numero.ToString(); //cte.Numero + "/" + cte.Serie.Numero;
                            itemFatura.TipoFatura = ""; //cte.ModeloDocumentoFiscal.Numero;
                            itemFatura.DebitoOuCredito = documentoContabil.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito ? "C" : "D";
                            itemFatura.ValorLancamento = documentoContabil.ValorContabilizacao;
                            if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "70";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                            {
                                itemFatura.TipoLancamento = "03";
                                itemFatura.CodigoImposto = "70";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "80";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "80";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "50";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                            {
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "60";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido)
                            {
                                itemFatura.TipoLancamento = "01";
                                itemFatura.CodigoImposto = "  ";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteValor)
                            {
                                itemFatura.TipoLancamento = "01";
                                itemFatura.CodigoImposto = "  ";
                            }
                            if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido2)
                            {
                                itemFatura.TipoLancamento = "02";
                                itemFatura.CodigoImposto = "  ";
                            }
                            if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido9)
                            {
                                itemFatura.TipoLancamento = "09";
                                itemFatura.CodigoImposto = "  ";
                            }
                            else if (documentoContabil.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber)
                            {
                                itemFatura.TipoLancamento = "10";
                                itemFatura.CodigoImposto = "  ";
                            }
                            ediFatura.ItemFatura.Add(itemFatura);
                        }

                        cabecalhoFatura.Fatura.Add(ediFatura);


                        intpfar.CabecalhosFatura.Add(cabecalhoFatura);

                    }

                }


            }


            return intpfar;
        }

        private Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura BaseItemFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura();

            itemFatura.CodigoTransportador = fatura.Empresa.CodigoIntegracao;
            itemFatura.GrupoTomador = fatura.ClienteTomadorFatura?.GrupoPessoas?.Descricao ?? "";
            itemFatura.CodigoCliente = fatura.ClienteTomadorFatura != null ? fatura.ClienteTomadorFatura.CodigoIntegracao : "";
            itemFatura.CodigoContaContabil = "";
            itemFatura.CodigoCentroCusto = "";
            itemFatura.CodigoItem = "";

            itemFatura.NumeroNF = 0;
            itemFatura.SerieNF = 0;
            itemFatura.IdDocumento = 0;

            itemFatura.NumeroFatura = "";
            itemFatura.TipoFatura = "";

            return itemFatura;
        }

        public Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR ConverterCargaParaINTPFAR(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            faturaIntegracao.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaIntegracao.Fatura;

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
            List<int> codigosCtes = new List<int>();
            if (!fatura.NovoModelo)
                cargas = (from obj in fatura.Cargas select obj.Carga).ToList();
            else
            {
                codigosCtes = (from obj in fatura.Documentos select obj.Documento.CTe.Codigo).ToList();
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                cargas = repCargaCte.BuscarPorCTes(codigosCtes);
            }


            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao serConfiguracaoNaturezaOperacao = new ConfiguracaoContabil.ConfiguracaoNaturezaOperacao();

            // Repositorios
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto repFaturaAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            // Objeto de valor principal
            Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR intpfar = new Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR();

            // Preenche cabecalho do EDI

            //intpfar.Destinatario = cargaPedido.ObterTomador().CPF_CNPJ.ToString();
            //intpfar.Remetente = carga.Empresa.CNPJ;

            intpfar.CabecalhosFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura>();

            List<int> ctesGerados = new List<int>();

            for (int c = 0; c < cargas.Count; c++)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas[c];
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                if (fatura.Cliente != null)
                    cargaPedido = (from obj in carga.Pedidos where obj.ObterTomador().CPF_CNPJ == fatura.Cliente.CPF_CNPJ select obj).FirstOrDefault();

                if (cargaPedido == null)
                    cargaPedido = carga.Pedidos.FirstOrDefault();

                // Preenche cabecalho da fatura
                Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();
                cabecalhoFatura.IdDocumento = fatura.Codigo + c;
                cabecalhoFatura.CodigoTransportadora = carga.Empresa.CodigoIntegracao;
                cabecalhoFatura.CNPJTomador = cargaPedido.ObterTomador().CPF_CNPJ_SemFormato;
                cabecalhoFatura.CodigoReferencia = fatura.Numero.ToString() + "-" + DateTime.Now.ToString("yyyy");
                cabecalhoFatura.DataLancamento = DateTime.Now;
                cabecalhoFatura.DataVencimento = fatura.Parcelas != null && fatura.Parcelas.Count() > 0 ? (from o in fatura.Parcelas select o.DataVencimento).FirstOrDefault() : DateTime.Now;
                cabecalhoFatura.DataEmissao = fatura.DataFatura;
                cabecalhoFatura.TipoLancamento = "2";
                cabecalhoFatura.Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>();

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = null;
                if (!fatura.NovoModelo)
                {
                    if (!string.IsNullOrWhiteSpace(faturaIntegracao.TipoImposto))
                    {
                        List<string> CSTs = (from obj in faturaIntegracao.CSTs select obj.CST).ToList();
                        if (faturaIntegracao.UsarCST)
                            ctes = carga.CargaCTes.Where(o => CSTs.Contains(o.CTe.CST)).Select(obj => obj.CTe).Distinct().ToList();
                        else
                            ctes = carga.CargaCTes.Where(o => !CSTs.Contains(o.CTe.CST)).Select(obj => obj.CTe).Distinct().ToList();
                    }
                    else
                        ctes = carga.CargaCTes.Select(obj => obj.CTe).Distinct().ToList();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(faturaIntegracao.TipoImposto))
                    {
                        List<string> CSTs = (from obj in faturaIntegracao.CSTs select obj.CST).ToList();
                        if (faturaIntegracao.UsarCST)
                            ctes = (from obj in carga.CargaCTes where obj.CTe != null && codigosCtes.Contains(obj.CTe.Codigo) && CSTs.Contains(obj.CTe.CST) select obj.CTe).Distinct().ToList();
                        else
                            ctes = (from obj in carga.CargaCTes where obj.CTe != null && codigosCtes.Contains(obj.CTe.Codigo) && !CSTs.Contains(obj.CTe.CST) select obj.CTe).Distinct().ToList();
                    }
                    else
                        ctes = (from obj in carga.CargaCTes where obj.CTe != null && codigosCtes.Contains(obj.CTe.Codigo) select obj.CTe).Distinct().ToList();
                }

                if (ctes.Count == 0)
                    continue;

                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), null, carga.Empresa, carga.TipoOperacao, carga.Rota, null, null, unidadeTrabalho);
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.Pedido.Expedidor, cargaPedido.Pedido.Recebedor, cargaPedido.ObterTomador(), null, null, carga.Empresa, carga.TipoOperacao, null, carga.Rota, carga.Filial, cargaPedido.Origem, unidadeTrabalho);
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = serConfiguracaoNaturezaOperacao.ObterConfiguracaoNaturezaOperacao(cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.ModeloDocumentoFiscal, cargaPedido.Origem.Estado, cargaPedido.Destino.Estado, null, carga.TipoOperacao, carga.Rota, carga.Empresa, carga.Empresa, unidadeTrabalho);

                if (configuracaoContaContabil != null && configuracaoCentroResultado != null)
                {

                    for (int i = 0; i < ctes.Count; i++)
                    {

                        if (ctesGerados.Contains(ctes[i].Codigo))
                            continue;

                        ctesGerados.Add(ctes[i].Codigo);
                        faturaIntegracao.CodigosCTes.Add(ctes[i].Codigo);

                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];

                        Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura ediFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura();
                        ediFatura.CNPJTransportadora = cte.Empresa.CNPJ_SemFormato;
                        ediFatura.CNPJCompanhia = cte.TomadorPagador.CPF_CNPJ_SemFormato;
                        ediFatura.NumeroFatura = cte.Numero.ToString();

                        string ultimosCaracteresCodigoEmpresa = "";
                        if (cabecalhoFatura.CodigoTransportadora.Length > 5)
                            ultimosCaracteresCodigoEmpresa = cabecalhoFatura.CodigoTransportadora.Substring(cabecalhoFatura.CodigoTransportadora.Length - 5);
                        else
                            ultimosCaracteresCodigoEmpresa = cabecalhoFatura.CodigoTransportadora;

                        ediFatura.NumeroFaturaCodTransportador = ultimosCaracteresCodigoEmpresa + ediFatura.NumeroFatura;

                        if (ediFatura.NumeroFaturaCodTransportador.Length > 12)
                            ediFatura.NumeroFaturaCodTransportador = ediFatura.NumeroFaturaCodTransportador.Substring(0, 12);

                        //ediFatura.CodigoFatura = carga.CodigoCargaEmbarcador.ToString();
                        ediFatura.DataFatura = fatura.DataFatura;
                        ediFatura.SerieFatura = cte.Serie.Numero.ToString();
                        ediFatura.DataEmissao = cte.DataEmissao.Value;
                        ediFatura.CodigoTipoFatura = cte.ModeloDocumentoFiscal.Numero;
                        ediFatura.STFatura = cte.CST == "60" ? "S" : "N";
                        ediFatura.CFOP = configuracaoNaturezaOperacao != null ? configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda.CodigoIntegracao : "";
                        ediFatura.ValorFatura = cte.ValorAReceber;

                        ediFatura.CodigoFatura = fatura.Numero + "/" + carga.CodigoCargaEmbarcador;

                        ediFatura.CodigoImposto = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? 7 : 8;
                        ediFatura.BaseCalculoImposto = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.BaseCalculoICMS : cte.BaseCalculoISS;
                        ediFatura.AliquotaImposto = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.AliquotaICMS : cte.AliquotaISS;
                        ediFatura.ValorImposto = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.ValorICMS : cte.ValorISS;
                        ediFatura.CNPJEmitente = cte.Remetente.CPF_CNPJ_SemFormato;
                        ediFatura.ItemFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura>();
                        ediFatura.CNPJTransportadorCTe = cte.Empresa.CNPJ_SemFormato + "_" + cte.Serie.ToString();

                        decimal valorBC = cte.BaseCalculoICMS;

                        foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
                        {

                            Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura();
                            itemFatura.CodigoTransportador = cte.Empresa.CodigoIntegracao;
                            itemFatura.GrupoTomador = cte.TomadorPagador.Cliente.GrupoPessoas?.Descricao ?? "";
                            //itemFatura.CodigoCliente = fatura.ClienteTomadorFatura != null ? fatura.ClienteTomadorFatura.CodigoIntegracao : "";
                            itemFatura.CodigoContaContabil = configuracao.PlanoContabilidade;
                            itemFatura.CodigoCentroCusto = configuracaoCentroResultado?.CentroResultadoContabilizacao?.PlanoContabilidade ?? "";
                            itemFatura.CodigoItem = "";
                            itemFatura.NumeroNF = cte.Numero;
                            itemFatura.SerieNF = cte.Serie.Numero;
                            itemFatura.IdDocumento = 0;
                            itemFatura.NumeroFatura = cte.Numero + "/" + cte.Serie.Numero;
                            itemFatura.TipoFatura = cte.ModeloDocumentoFiscal.Numero;
                            itemFatura.DebitoOuCredito = configuracao.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito ? "C" : "D";

                            decimal basePisCofins = cte.ValorPrestacaoServico;
                            if (configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                                basePisCofins = cte.ValorPrestacaoServico - cte.ValorICMS;

                            decimal valorEmpresaCOFINS = cte.Empresa.Configuracao.AliquotaCOFINS ?? 0;
                            if (valorEmpresaCOFINS == 0)
                                valorEmpresaCOFINS = fatura.Empresa.EmpresaPai?.Configuracao.AliquotaCOFINS ?? 0;

                            decimal valorCOFINS = Math.Round(basePisCofins * (valorEmpresaCOFINS / 100), 2, MidpointRounding.AwayFromZero);

                            decimal valorEmpresaPIS = cte.Empresa.Configuracao.AliquotaPIS ?? 0;
                            if (valorEmpresaPIS == 0)
                                valorEmpresaPIS = fatura.Empresa.EmpresaPai?.Configuracao.AliquotaPIS ?? 0;
                            decimal valorPIS = Math.Round(basePisCofins * (valorEmpresaPIS / 100), 2, MidpointRounding.AwayFromZero);
                            if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS)
                            {
                                if (cte.CST == "40" || cte.CST == "")//quando isento não insere a linha
                                    continue;

                                decimal valorICMS = cte.ValorICMS;
                                itemFatura.ValorLancamento = valorICMS;
                                itemFatura.TipoLancamento = "11";
                                itemFatura.CodigoImposto = "70";
                            }
                            else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                            {
                                if (cte.CST != "60")//quando não for ST não insere a linha
                                    continue;

                                decimal valorICMSST = cte.ValorICMS;
                                itemFatura.ValorLancamento = valorICMSST;
                                itemFatura.TipoLancamento = "03";
                                itemFatura.CodigoImposto = "70";
                            }
                            else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                            {
                                if (cte.Empresa.Configuracao != null)
                                {
                                    if (valorPIS > 0)
                                    {
                                        itemFatura.ValorLancamento = valorPIS;
                                        itemFatura.TipoLancamento = "11";
                                        itemFatura.CodigoImposto = "50";
                                    }
                                }
                            }
                            else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                            {
                                if (cte.Empresa.Configuracao != null)
                                {
                                    if (valorCOFINS > 0)
                                    {
                                        itemFatura.ValorLancamento = valorCOFINS;
                                        itemFatura.TipoLancamento = "11";
                                        itemFatura.CodigoImposto = "60";
                                    }
                                }
                            }
                            else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido)
                            {
                                decimal freteLiquido = cte.ValorAReceber - valorPIS - valorCOFINS - (cte.CST != "60" ? cte.ValorICMS : 0);
                                //itemFatura.ValorLancamento = cte.ValorFrete;
                                itemFatura.ValorLancamento = freteLiquido;
                                itemFatura.TipoLancamento = "01";
                                itemFatura.CodigoImposto = "  ";
                            }
                            else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido2)
                            {
                                decimal freteLiquido = cte.ValorAReceber - valorPIS - valorCOFINS - (cte.CST != "60" ? cte.ValorICMS : 0);
                                //itemFatura.ValorLancamento = cte.ValorFrete;
                                itemFatura.ValorLancamento = freteLiquido;

                                itemFatura.TipoLancamento = "02";
                                itemFatura.CodigoImposto = "  ";
                            }
                            else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido9)
                            {
                                decimal freteLiquido = cte.ValorAReceber - valorPIS - valorCOFINS - (cte.CST != "60" ? cte.ValorICMS : 0);
                                //itemFatura.ValorLancamento = cte.ValorFrete;
                                itemFatura.ValorLancamento = freteLiquido;
                                itemFatura.TipoLancamento = "09";
                                itemFatura.CodigoImposto = "  ";
                            }
                            else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber)
                            {
                                itemFatura.ValorLancamento = cte.ValorAReceber;
                                itemFatura.TipoLancamento = "10";
                                itemFatura.CodigoImposto = "  ";
                            }
                            else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteValor)
                            {
                                decimal freteValor = cte.ValorAReceber - (cte.CST != "60" ? cte.ValorICMS : 0);
                                //itemFatura.ValorLancamento = cte.ValorFrete;
                                itemFatura.ValorLancamento = freteValor;

                                itemFatura.TipoLancamento = "01";
                                itemFatura.CodigoImposto = "  ";
                            }
                            ediFatura.ItemFatura.Add(itemFatura);
                        }
                        cabecalhoFatura.Fatura.Add(ediFatura);
                    }
                }

                intpfar.CabecalhosFatura.Add(cabecalhoFatura);
            }

            return intpfar;
        }

        private Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura GeraINTPFARPorImpostos(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil, Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado, Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Dominio.Entidades.Empresa empresaLayout, List<string> CSTs, bool usarCST, bool modeloCTe, string tipoImposto, int indexImposto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao serConfiguracaoNaturezaOperacao = new ConfiguracaoContabil.ConfiguracaoNaturezaOperacao();

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaIntegracao.Fatura;
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = serConfiguracaoNaturezaOperacao.ObterConfiguracaoNaturezaOperacao(null, null, fatura.ClienteTomadorFatura, null, null, null, null, fatura.TipoOperacao, null, empresaLayout, fatura.Empresa, unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            faturaIntegracao.CodigosCTes.AddRange(repFaturaDocumento.BuscarCodigosCTesCSTs(fatura.Codigo, CSTs, usarCST, modeloCTe));
            decimal totalReceber = repFaturaDocumento.BuscarTotalReceberCSTs(fatura.Codigo, CSTs, usarCST, modeloCTe);

            if (totalReceber <= 0)
                return null;

            // Preenche cabecalho da fatura
            int.TryParse(indexImposto.ToString() + fatura.Codigo.ToString(), out int idDocumento);

            Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura
            {
                IdDocumento = idDocumento,
                CodigoTransportadora = fatura?.Transportador?.CodigoIntegracao ?? "",
                CNPJTomador = fatura.ClienteTomadorFatura.CPF_CNPJ_SemFormato,
                CodigoReferencia = fatura.Numero.ToString() + "-" + DateTime.Now.ToString("yyyy"),
                DataLancamento = fatura.DataFatura,
                DataVencimento = fatura.Parcelas != null && fatura.Parcelas.Count() > 0 ? (from o in fatura.Parcelas select o.DataVencimento).FirstOrDefault() : DateTime.Now,
                DataEmissao = fatura.DataFatura,
                TipoLancamento = "2",
                CodigoCompanhia = fatura.ClienteTomadorFatura.CodigoCompanhia, //configuracaoCentroResultado.CentroResultadoContabilizacao.CodigoCompanhia;

                Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>()
            };


            decimal totalDaPrestacao = repFaturaDocumento.BuscarTotalDaPrestacaoComCST(fatura.Codigo, CSTs, usarCST, modeloCTe);
            Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura ediFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura
            {
                CNPJTransportadora = fatura.Transportador?.CNPJ_SemFormato ?? "",
                CNPJCompanhia = fatura.Cliente?.CPF_CNPJ_SemFormato ?? "",
                NumeroFatura = fatura.Numero.ToString() + "_" + (indexImposto + 1).ToString(),
                //ediFatura.CodigoFatura = carga.CodigoCargaEmbarcador.ToString();
                DataFatura = fatura.DataFatura,
                SerieFatura = "", //cte.Serie.Numero.ToString();
                DataEmissao = fatura.DataFechamento.Value,
                CodigoTipoFatura = "", //cte.ModeloDocumentoFiscal.Numero;
                STFatura = faturaIntegracao.TipoImposto == "ST" ? "S" : "N",
                CFOP = configuracaoNaturezaOperacao?.NaturezaDaOperacaoVenda?.CodigoIntegracao ?? "",
                ValorFatura = totalReceber,
                CodigoFatura = fatura.Numero.ToString(),
                BaseCalculoImposto = 0, //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.BaseCalculoICMS : cte.BaseCalculoISS;
                AliquotaImposto = 0, //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.AliquotaICMS : cte.AliquotaISS;
                ValorImposto = 0, //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cte.ValorICMS : cte.ValorISS;
                CNPJEmitente = fatura.Cliente?.CPF_CNPJ_SemFormato ?? ""//cte.Remetente.CPF_CNPJ_SemFormato;
            };

            string codigoTransp = fatura.Transportador?.CodigoIntegracao ?? "";

            string ultimosCaracteresCodigoEmpresa = "";
            if (codigoTransp.Length > 5)
                ultimosCaracteresCodigoEmpresa = cabecalhoFatura.CodigoTransportadora.Substring(codigoTransp.Length - 5);
            else
                ultimosCaracteresCodigoEmpresa = codigoTransp;

            ediFatura.NumeroFaturaCodTransportador = ultimosCaracteresCodigoEmpresa + ediFatura.NumeroFatura;

            if (ediFatura.NumeroFaturaCodTransportador.Length > 12)
                ediFatura.NumeroFaturaCodTransportador = ediFatura.NumeroFaturaCodTransportador.Substring(0, 12);

            if (faturaIntegracao.TipoImposto == "ISS" || faturaIntegracao.TipoImposto == "ISSRetido")
                ediFatura.CodigoImposto = 8; //cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? 7 : 8;
            else
                ediFatura.CodigoImposto = 7;
            ediFatura.CNPJTransportadorCTe = (fatura.Empresa?.CNPJ_SemFormato ?? "") + "_1";
            ediFatura.ItemFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura>();


            //decimal valorBC = repFaturaDocumento.BuscarTotalBaseCalculoICMSComCSTs(fatura.Codigo, CSTs, usarCST, modeloCTe);

            decimal valorICMS = 0;
            if (faturaIntegracao.TipoImposto == "Normal")
                valorICMS = repFaturaDocumento.BuscarTotalICMS(fatura.Codigo);

            decimal valorICMSST = 0;
            if (faturaIntegracao.TipoImposto == "ST")
                valorICMSST = repFaturaDocumento.BuscarTotalICMSSST(fatura.Codigo);

            decimal valorISS = 0;
            decimal valorISSRetido = 0;
            if (faturaIntegracao.TipoImposto == "ISS")
            {
                valorISS = repFaturaDocumento.BuscarTotalISS(fatura.Codigo);
                valorISSRetido = repFaturaDocumento.BuscarTotalISSRetido(fatura.Codigo);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
            {

                Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura
                {
                    CodigoTransportador = fatura.Transportador?.CodigoIntegracao ?? "",
                    GrupoTomador = fatura.ClienteTomadorFatura?.GrupoPessoas?.Descricao ?? "",
                    CodigoCliente = fatura.ClienteTomadorFatura != null ? fatura.ClienteTomadorFatura.CodigoIntegracao : "",
                    CodigoContaContabil = configuracao.PlanoContabilidade,
                    CodigoCentroCusto = configuracaoCentroResultado?.CentroResultadoContabilizacao?.PlanoContabilidade ?? "",
                    CodigoItem = "",
                    NumeroNF = fatura.Numero,
                    SerieNF = 0,//cte.Serie.Numero;
                    IdDocumento = 0,
                    NumeroFatura = fatura.Numero.ToString(), //cte.Numero + "/" + cte.Serie.Numero;
                    TipoFatura = "", //cte.ModeloDocumentoFiscal.Numero;
                    DebitoOuCredito = configuracao.TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito ? "C" : "D"
                };


                decimal valorEmpresaCOFINS = fatura.Transportador?.Configuracao.AliquotaCOFINS ?? 0;
                if (valorEmpresaCOFINS == 0)
                    valorEmpresaCOFINS = fatura.Empresa.EmpresaPai?.Configuracao.AliquotaCOFINS ?? 0;

                decimal basePisCofins = totalDaPrestacao;
                if (configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins)
                    basePisCofins = totalReceber - valorICMS;

                decimal valorCOFINS = Math.Round(basePisCofins * (valorEmpresaCOFINS / 100), 2, MidpointRounding.AwayFromZero);

                decimal valorEmpresaPIS = fatura.Transportador?.Configuracao.AliquotaPIS ?? 0;
                if (valorEmpresaPIS == 0)
                    valorEmpresaPIS = fatura.Empresa.EmpresaPai?.Configuracao.AliquotaPIS ?? 0;

                decimal valorPIS = Math.Round(basePisCofins * (valorEmpresaPIS / 100), 2, MidpointRounding.AwayFromZero);

                if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS)
                {
                    if (valorICMS <= 0)//quando isento não insere a linha
                        continue;

                    itemFatura.ValorLancamento = valorICMS;
                    itemFatura.TipoLancamento = "11";
                    itemFatura.CodigoImposto = "70";
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                {
                    if (valorICMSST <= 0)//quando não for ST não insere a linha
                        continue;

                    itemFatura.ValorLancamento = valorICMSST;
                    itemFatura.TipoLancamento = "03";
                    itemFatura.CodigoImposto = "70";
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS)
                {
                    if (valorISS <= 0)
                        continue;

                    itemFatura.ValorLancamento = valorISS;
                    itemFatura.TipoLancamento = "11";
                    itemFatura.CodigoImposto = "80";
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido)
                {
                    if (valorISSRetido <= 0)
                        continue;

                    itemFatura.ValorLancamento = valorISSRetido;
                    itemFatura.TipoLancamento = "11";
                    itemFatura.CodigoImposto = "80";
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                {
                    if (fatura.Transportador?.Configuracao != null)
                    {
                        if (valorPIS > 0)
                        {
                            itemFatura.ValorLancamento = valorPIS;
                            itemFatura.TipoLancamento = "11";
                            itemFatura.CodigoImposto = "50";
                        }
                    }
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                {
                    if (fatura.Transportador?.Configuracao != null)
                    {
                        if (valorCOFINS > 0)
                        {
                            itemFatura.ValorLancamento = valorCOFINS;
                            itemFatura.TipoLancamento = "11";
                            itemFatura.CodigoImposto = "60";
                        }
                    }
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido)
                {
                    decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                    itemFatura.ValorLancamento = freteLiquido;
                    itemFatura.TipoLancamento = "01";
                    itemFatura.CodigoImposto = "  ";
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido2)
                {
                    decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                    //itemFatura.ValorLancamento = cte.ValorFrete;
                    itemFatura.ValorLancamento = freteLiquido;

                    itemFatura.TipoLancamento = "02";
                    itemFatura.CodigoImposto = "  ";
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido9)
                {
                    decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                    //itemFatura.ValorLancamento = cte.ValorFrete;
                    itemFatura.ValorLancamento = freteLiquido;
                    itemFatura.TipoLancamento = "09";
                    itemFatura.CodigoImposto = "  ";
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber)
                {
                    itemFatura.ValorLancamento = totalReceber;
                    itemFatura.TipoLancamento = "10";
                    itemFatura.CodigoImposto = "  ";
                }
                else if (configuracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteValor)
                {
                    decimal freteValor = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                    //itemFatura.ValorLancamento = cte.ValorFrete;
                    itemFatura.ValorLancamento = freteValor;
                    itemFatura.TipoLancamento = "01";
                }

                ediFatura.ItemFatura.Add(itemFatura);
            }
            cabecalhoFatura.Fatura.Add(ediFatura);

            return cabecalhoFatura;
        }

        public Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR ConverterLoteContabilizacaoParaINTPFAR(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI loteContabilizacaoIntegracaoEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao repConfiguracaoFechamentoContabilizacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabilConta repDocumentoExportacaoContabilConta = new Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabilConta(unitOfWork);

            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao configuracaoFechamentoContabilizacao = repConfiguracaoFechamentoContabilizacao.BuscarConfiguracaoAtual(loteContabilizacaoIntegracaoEDI.LoteContabilizacao.DataGeracaoLote.Value);

            DateTime dataInicialFechamentoContabilizacao = DateTime.Now;

            if (configuracaoFechamentoContabilizacao != null)
                dataInicialFechamentoContabilizacao = new DateTime(configuracaoFechamentoContabilizacao.AnoReferencia, configuracaoFechamentoContabilizacao.MesReferencia, 1);

            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao> dados = repDocumentoExportacaoContabilConta.BuscarDadosParaExportacao(loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Codigo);

            List<string> empresas = dados.Select(o => o.CNPJEmpresa).Distinct().ToList();

            Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR intpfar = new Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR
            {
                CabecalhosFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura>(),
                Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>(),
                RodapeFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.RodapeFatura(),
                DataGeracao = DateTime.Now,
                NomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(loteContabilizacaoIntegracaoEDI, null, unitOfWork),
                TotalLancamentos = dados.Count(),
                ValorTotal = dados.Sum(o => o.Valor)
            };

            string extensao = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterExtencaoPadrao(loteContabilizacaoIntegracaoEDI.LayoutEDI);

            intpfar.NomeArquivoSemExtencao = intpfar.NomeArquivo.Replace(extensao, "");

            for (int e = 0; e < empresas.Count; e++)
            {
                string cnpjEmpresa = empresas[e];
                string codigoEmpresa = dados.Where(o => o.CNPJEmpresa == cnpjEmpresa).Select(o => o.CodigoIntegracaoEmpresa).FirstOrDefault();
                List<double> tomadores = dados.Where(o => o.CNPJEmpresa == cnpjEmpresa).Select(o => o.CPFCNPJTomador).Distinct().ToList();

                for (int t = 0; t < tomadores.Count; t++)
                {
                    double cnpjTomador = tomadores[t];

                    List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao> documentosTomador = dados.Where(o => o.CNPJEmpresa == cnpjEmpresa && o.CPFCNPJTomador == cnpjTomador).ToList();

                    Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao documentoTomador = documentosTomador.FirstOrDefault();

                    List<DateTime> datasMovimentos = documentosTomador.Select(o => o.DataMovimento).Distinct().ToList();

                    for (int dt = 0; dt < datasMovimentos.Count; dt++)
                    {
                        DateTime dataMovimento = datasMovimentos[dt];

                        int.TryParse(dt.ToString() + loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Codigo.ToString(), out int idDocumento);

                        // Preenche cabecalho da fatura
                        Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura cabecalhoFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.CabecalhoFatura();
                        cabecalhoFatura.IdDocumento = idDocumento;
                        cabecalhoFatura.CodigoTransportadora = codigoEmpresa;
                        cabecalhoFatura.CNPJTomador = documentoTomador.CPFCNPJTomadorSemFormato;
                        cabecalhoFatura.CodigoReferencia = loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Numero.ToString() + "-" + DateTime.Now.ToString("yyyy");

                        DateTime dataLancamento = /*loteContabilizacaoIntegracaoEDI.LoteContabilizacao.DataGeracaoLote ??*/ dataMovimento;

                        if (configuracaoFechamentoContabilizacao != null && dataLancamento < dataInicialFechamentoContabilizacao)
                            dataLancamento = dataInicialFechamentoContabilizacao;

                        cabecalhoFatura.DataLancamento = dataLancamento;
                        cabecalhoFatura.DataVencimento = dataMovimento;
                        cabecalhoFatura.TipoLancamento = "2";
                        cabecalhoFatura.DataEmissao = dataMovimento;

                        cabecalhoFatura.Fatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura>();

                        List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacaoTipo> documentos = documentosTomador.Where(o => o.DataMovimento.Date == dataMovimento.Date).Select(o => new Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacaoTipo() { Codigo = o.CodigoCTe ?? o.CodigoContratoFrete ?? 0, TipoDocumento = o.TipoDocumento, TipoMovimentoExportacao = o.TipoMovimentoExportacao }).Distinct().ToList();

                        for (int i = 0; i < documentos.Count; i++)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacaoTipo documento = documentos[i];

                            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao> contasExportarDocumento = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao>();

                            if (documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoExportacaoContabil.CTe)
                                contasExportarDocumento = documentosTomador.Where(o => o.DataMovimento.Date == dataMovimento.Date && o.CodigoCTe == documento.Codigo && o.TipoMovimentoExportacao == documento.TipoMovimentoExportacao).ToList();
                            else if (documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoExportacaoContabil.ContratoFrete)
                                contasExportarDocumento = documentosTomador.Where(o => o.DataMovimento.Date == dataMovimento.Date && o.CodigoContratoFrete == documento.Codigo && o.TipoMovimentoExportacao == documento.TipoMovimentoExportacao).ToList();

                            if (contasExportarDocumento.Count == 0)
                                continue;

                            if (contasExportarDocumento.Sum(o => o.Valor) <= 0m)
                            {
                                intpfar.TotalLancamentos -= contasExportarDocumento.Count;
                                continue;
                            }

                            Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao primeiraContaExportar = contasExportarDocumento.FirstOrDefault();

                            Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura ediFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.Fatura
                            {
                                NumeroFatura = primeiraContaExportar.Numero,
                                DataEmissao = primeiraContaExportar.DataEmissao,
                                DataLancamento = dataLancamento,
                                CNPJTransportadorCTe = primeiraContaExportar.CNPJEmpresa + "_" + (primeiraContaExportar.SerieCTe ?? 1),
                                ItemFatura = new List<Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura>()
                            };

                            if (primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao ||
                                primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento)
                            {
                                if (primeiraContaExportar.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                    if (primeiraContaExportar.TomadorFazParteGrupoEconomico)
                                    {
                                        ediFatura.CodigoTipoFatura = "CI";
                                    }
                                    else
                                    {
                                        ediFatura.CodigoTipoFatura = "CT";
                                    }

                                else if (primeiraContaExportar.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || primeiraContaExportar.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)

                                    if (primeiraContaExportar.TomadorFazParteGrupoEconomico)
                                    {
                                        ediFatura.CodigoTipoFatura = "NI";
                                    }
                                    else
                                    {
                                        ediFatura.CodigoTipoFatura = "NT";
                                    }
                                else if (primeiraContaExportar.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                                    if (primeiraContaExportar.TomadorFazParteGrupoEconomico)
                                    {
                                        ediFatura.CodigoTipoFatura = "DI";
                                    }
                                    else
                                    {
                                        ediFatura.CodigoTipoFatura = "DT";
                                    }

                            }
                            else if (primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AcrescimoDescontoBaixaTituloReceber ||
                                     primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.CancelamentoAcrescimoDescontoBaixaTituloReceber ||
                                     primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AcrescimoDescontoFatura ||
                                     primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.CancelamentoAcrescimoDescontoFatura)
                            {
                                if (primeiraContaExportar.TomadorFazParteGrupoEconomico)
                                {
                                    ediFatura.CodigoTipoFatura = "DI";
                                }
                                else
                                {
                                    ediFatura.CodigoTipoFatura = "DT";
                                }
                            }
                            else if (primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.BaixaTituloReceber ||
                                     primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.CancelamentoBaixaTituloReceber ||
                                     primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.PagamentoContratoFrete ||
                                     primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoPagamentoContratoFrete)
                            {
                                ediFatura.CodigoTipoFatura = "PT";
                            }
                            else if (primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AprovacaoContratoFrete ||
                                     primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.ReversaoContratoFrete)
                            {
                                ediFatura.CodigoTipoFatura = "AF";
                            }

                            foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao contaExportarDocumento in contasExportarDocumento)
                            {
                                Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura itemFatura = new Dominio.ObjetosDeValor.EDI.INTPFAR.ItemFatura
                                {
                                    CodigoTransportador = codigoEmpresa,
                                    CodigoCliente = contaExportarDocumento.CodigoIntegracaoTomador,
                                    DataVencimento = dataMovimento,
                                    ValorLancamento = contaExportarDocumento.Valor,
                                    TipoModeloDocFatura = ediFatura.CodigoTipoFatura
                                };

                                if (primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.BaixaTituloReceber ||
                                    primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AcrescimoDescontoBaixaTituloReceber ||
                                    primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.CancelamentoAcrescimoDescontoBaixaTituloReceber ||
                                    primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.CancelamentoBaixaTituloReceber ||
                                    primeiraContaExportar.TipoMovimentoExportacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.PagamentoContratoFrete)
                                    itemFatura.CNPJTransportador = contaExportarDocumento.CNPJEmpresa;

                                if (!string.IsNullOrWhiteSpace(contaExportarDocumento.CodigoCentroResultadoEmpresa))
                                    itemFatura.CodigoCentroCusto = contaExportarDocumento.CodigoCentroResultadoEmpresa;
                                else if (!string.IsNullOrWhiteSpace(contaExportarDocumento.CodigoCentroResultadoCadastro))
                                    itemFatura.CodigoCentroCusto = contaExportarDocumento.CodigoCentroResultadoCadastro;
                                else
                                    itemFatura.CodigoCentroCusto = contaExportarDocumento.CodigoCentroResultado;

                                if (!string.IsNullOrWhiteSpace(contaExportarDocumento.CodigoContaContabilCadastro))
                                    itemFatura.CodigoContaContabil = contaExportarDocumento.CodigoContaContabilCadastro;
                                else
                                    itemFatura.CodigoContaContabil = contaExportarDocumento.CodigoContaContabil;

                                if (contaExportarDocumento.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
                                    itemFatura.TipoDebitoOuCredito = "50";
                                else if (contaExportarDocumento.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                                    itemFatura.TipoDebitoOuCredito = "40";
                                else if (contaExportarDocumento.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.ContasAPagar)
                                {
                                    itemFatura.CNPJTransportador = contaExportarDocumento.CNPJEmpresa;
                                    itemFatura.TipoDebitoOuCredito = "31";
                                }
                                else if (contaExportarDocumento.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.ContasAReceber)
                                {
                                    itemFatura.CNPJTransportador = contaExportarDocumento.CNPJEmpresa;
                                    itemFatura.TipoDebitoOuCredito = "21";
                                }
                                else if (contaExportarDocumento.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.NotaDebito || contaExportarDocumento.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.NotaCredito)
                                    itemFatura.TipoDebitoOuCredito = "09";

                                itemFatura.CNPJCPFTomador = contaExportarDocumento.CPFCNPJTomadorSemFormato;

                                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(contaExportarDocumento.CPFCNPJTomador);

                                if (cliente.FazParteGrupoEconomico)
                                    itemFatura.TipoMovimentoGrupoEmpresa = 2;
                                else
                                    itemFatura.TipoMovimentoGrupoEmpresa = 1;

                                ediFatura.ItemFatura.Add(itemFatura);

                                intpfar.RodapeFatura.Somatorios++;
                            }

                            cabecalhoFatura.Fatura.Add(ediFatura);

                            intpfar.Fatura.Add(ediFatura);

                            intpfar.RodapeFatura.Contadores++;
                        }

                        if (cabecalhoFatura.Fatura.Any(o => o.ItemFatura?.Count > 0))
                            intpfar.CabecalhosFatura.Add(cabecalhoFatura);
                    }
                }
            }

            return intpfar;
        }

    }
}
