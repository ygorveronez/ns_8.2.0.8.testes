using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class INTNC
    {
        private Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento ObterCTeINTNC(string numeroFatura, DateTime dataCriacaoArquivo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresaLayout, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Empresa empresa, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabilEscrituracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento cteINTNC = new Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento();

            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork);

            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao serConfiguracaoNaturezaOperacao = new ConfiguracaoContabil.ConfiguracaoNaturezaOperacao();

            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(cte.Remetente.Cliente, cte.Destinatario.Cliente, cte.TomadorPagador?.Cliente, null, empresaLayout, tipoOperacao, rotaFrete, cte.ModeloDocumentoFiscal, tipoDeOcorrencia, unitOfWork);
            //Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(cte.Remetente.Cliente, cte.Destinatario.Cliente, cte.TomadorPagador.Cliente, null, tipoOperacao, rotaFrete, unitOfWork);
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = serConfiguracaoNaturezaOperacao.ObterConfiguracaoNaturezaOperacao(cte.Remetente.Cliente, cte.Destinatario.Cliente, cte.TomadorPagador?.Cliente, cte.ModeloDocumentoFiscal, cte.LocalidadeInicioPrestacao.Estado, cte.LocalidadeTerminoPrestacao.Estado, null, tipoOperacao, rotaFrete, empresaLayout, empresa, unitOfWork);

            if (empresaLayout != null && configuracaoNaturezaOperacao != null)
            {
                if (configuracaoNaturezaOperacao.Empresa == null || configuracaoNaturezaOperacao.Empresa.Codigo != empresaLayout.Codigo)
                    configuracaoNaturezaOperacao = null;
            }

            string codigoNatureza = cte.CFOP.CodigoCFOP.ToString() + "AA";
            string codigoNaturezaCompras = cte.CFOP.CodigoCFOPCompra.ToString() + "AA";
            int codigoNaturezaInt = cte.CFOP.CodigoCFOP;
            int codigoNaturezaComprasInt = cte.CFOP.CodigoCFOPCompra;
            if (configuracaoNaturezaOperacao != null)
            {
                codigoNatureza = configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda.CodigoIntegracao;
                codigoNaturezaCompras = configuracaoNaturezaOperacao.NaturezaDaOperacaoCompra.CodigoIntegracao;

                codigoNaturezaInt = int.Parse(Utilidades.String.OnlyNumbers(configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda.CodigoIntegracao));
                codigoNaturezaComprasInt = int.Parse(Utilidades.String.OnlyNumbers(configuracaoNaturezaOperacao.NaturezaDaOperacaoCompra.CodigoIntegracao));
            }

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
            {
                codigoNatureza = "000000";
                codigoNaturezaCompras = "000000";
            }

            cteINTNC.IdNotaCobranca = cte.Codigo;
            cteINTNC.NumeroNotaCobranca = cte.Numero;
            cteINTNC.SequencialUnico = cte.Codigo + 5000000000;
            cteINTNC.CodModelo = cte.ModeloDocumentoFiscal.Numero;
            cteINTNC.NumeroNC = cte.Numero.ToString();
            cteINTNC.SerieNC = cte.Serie.Numero.ToString();
            cteINTNC.TipoPessoaTransportadora = "2";


            cteINTNC.CNPJTransportadora = empresa.CNPJ;
            cteINTNC.CodigoTransportadora = empresa.CodigoIntegracao;
            cteINTNC.IETransportadora = empresa.InscricaoEstadual;
            cteINTNC.CodResponsavelFrete = cte.TomadorPagador.Cliente.CodigoIntegracao; //carga.Empresa.CodigoIntegracao;
            cteINTNC.TipoPessoaResponsavelFrete = cte.TomadorPagador.Cliente.Tipo;
            cteINTNC.CNPJResponsavelFrete = cte.TomadorPagador.Cliente.CPF_CNPJ_SemFormato;

            cteINTNC.CNPJRemetente = cte.Remetente.Cliente.CPF_CNPJ_SemFormato;
            cteINTNC.CodRemetente = cte.Remetente.Cliente.CodigoIntegracao;
            cteINTNC.TipoPessoaRemetente = cte.Remetente.Cliente.Tipo;
            cteINTNC.UFRemetente = cte.Remetente.Cliente.Localidade.Estado.Sigla;

            cteINTNC.CNPJDestinatario = cte.Destinatario.Cliente.CPF_CNPJ_SemFormato;
            cteINTNC.CodDestinatario = cte.Destinatario.Cliente.CodigoIntegracao;
            cteINTNC.TipoPessoaDestinatario = cte.Destinatario.Cliente.Tipo;
            cteINTNC.UFDestinatario = cte.Destinatario.Cliente.Localidade.Estado.Sigla;

            cteINTNC.DataRegistroNC = string.Empty;
            cteINTNC.DataAtual = DateTime.Now;
            cteINTNC.DataEmissaoNC = cte.DataEmissao;

            if (cte.Status == "C" || cte.Status == "I")
                cteINTNC.DataCancelamento = cte.DataCancelamento;
            else if (cte.Status == "Z")
                cteINTNC.DataCancelamento = cte.DataAnulacao;

            if (dataCriacaoArquivo.Day >= 2 && (dataCriacaoArquivo.Year > cte.DataEmissao.Value.Year || dataCriacaoArquivo.Month > cte.DataEmissao.Value.Month))
                cteINTNC.DataEmissaoNCMesContabil = new DateTime(dataCriacaoArquivo.Year, dataCriacaoArquivo.Month, 1);
            else
                cteINTNC.DataEmissaoNCMesContabil = cte.DataEmissao;

            cteINTNC.CodNaturezaOperacao = codigoNatureza;
            cteINTNC.CodNaturezaOperacaoInt = codigoNaturezaInt;
            cteINTNC.CodNaturezaOperacaoCompra = codigoNaturezaCompras;
            cteINTNC.CodNaturezaOperacaoCompraInt = codigoNaturezaComprasInt;
            cteINTNC.ValorFreteApagarNC = cte.ValorAReceber;
            cteINTNC.ValorTotalMercadoria = cte.ValorTotalMercadoria;
            cteINTNC.ValorDescontoNC = 0;
            cteINTNC.Zero = 0;
            cteINTNC.UFOrigemFrete = cte.LocalidadeInicioPrestacao.Estado.Sigla;
            cteINTNC.UFDestinoFrete = cte.LocalidadeTerminoPrestacao.Estado.Sigla;
            cteINTNC.IBGEOrigemFrete = cte.LocalidadeInicioPrestacao.CodigoIBGE.ToString();
            cteINTNC.IBGEDestinoFrete = cte.LocalidadeTerminoPrestacao.CodigoIBGE.ToString();
            cteINTNC.TipoCTe = cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? "1" : "0";
            cteINTNC.CodFatura = numeroFatura;
            cteINTNC.ValorFreteNC = cte.ValorAReceber;
            cteINTNC.valorItem = cte.ValorTotalMercadoria + cte.ValorAReceber + cte.ValorPrestacaoServico + cte.ValorImposto;
            cteINTNC.ValorPrestacao = cte.ValorPrestacaoServico;
            cteINTNC.NumeroDoc = cte.Codigo;

            if (cte.TomadorPagador.Cliente.VerificarUnidadeNegocioPorDestinatario && cte.Destinatario.Cliente.GrupoPessoas != null)
            {
                int unidadeNegocio = 0;
                int.TryParse(cte.Destinatario.Cliente.GrupoPessoas.CodigoIntegracao, out unidadeNegocio);
                cteINTNC.CodigoUnidadeNegocio = unidadeNegocio;
            }
            else if (cte.TomadorPagador.Cliente.GrupoPessoas != null)
            {
                int unidadeNegocio = 0;
                int.TryParse(cte.TomadorPagador.Cliente.GrupoPessoas.CodigoIntegracao, out unidadeNegocio);
                cteINTNC.CodigoUnidadeNegocio = unidadeNegocio;
            }

            bool escriturarSomenteDocsNFe = false;

            if (cte.TomadorPagador.Cliente.NaoUsarConfiguracaoEmissaoGrupo)
                escriturarSomenteDocsNFe = cte.TomadorPagador.Cliente.EscriturarSomenteDocumentosEmitidosParaNFe;
            else if (cte.TomadorPagador.Cliente.GrupoPessoas != null)
                escriturarSomenteDocsNFe = cte.TomadorPagador.Cliente.GrupoPessoas.EscriturarSomenteDocumentosEmitidosParaNFe;

            bool icmsST = false;
            decimal basepiscofins = cte.ValorPrestacaoServico;
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                icmsST = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? true : false;
                cteINTNC.SubstituicaoTributariaICMS = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? "1" : "0";
                cteINTNC.ValorBaseICMS = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? 0 : cte.BaseCalculoICMS;
                cteINTNC.ValorICMS = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? 0 : cte.ValorICMS;
                cteINTNC.AliquotaICMS = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? 0 : cte.AliquotaICMS;

                cteINTNC.ValorBaseICMSST = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? cte.BaseCalculoICMS : 0;
                cteINTNC.ValorICMSST = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? cte.ValorICMS : 0;
                cteINTNC.AliquotaICMSST = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? cte.AliquotaICMS : 0;
                cteINTNC.TipoICMS = !string.IsNullOrWhiteSpace(cte.CST) && cte.CST == "60" ? "S" : "A";

                if (configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                    basepiscofins = cte.ValorPrestacaoServico - cte.ValorICMS;


            }
            else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                cteINTNC.SubstituicaoTributariaICMS = "0";
                cteINTNC.ValorBaseICMS = cte.BaseCalculoISS;
                cteINTNC.ValorBaseICMSST = 0;
                cteINTNC.ValorBaseISS = cte.BaseCalculoISS;
                cteINTNC.ValorISS = cte.ValorISS;
                cteINTNC.AliquotaISS = cte.AliquotaISS;
            }

            cteINTNC.CSTIBSCBS = cte.CSTIBSCBS;
            cteINTNC.ClassificacaoTributariaIBSCBS = cte.ClassificacaoTributariaIBSCBS;

            cteINTNC.ValorBaseICMSDiferencialAliquota = 0;
            cteINTNC.PercentualDiferencialAliquota = 0;
            cteINTNC.GrupoCFOP = cte.CFOP.CodigoCFOP.ToString().Substring(0, 1) + "000";
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
                cteINTNC.GrupoCFOP = "00000";

            if ((cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || cte.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho || cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                && (tipoOperacao != null && tipoOperacao.EmiteCTeFilialEmissora && (cte.TomadorPagador.Cliente.GrupoPessoas == null || !cte.TomadorPagador.Cliente.GrupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao)))
            {
                cteINTNC.CodigoTipoNota = cte.TipoTomador != Dominio.Enumeradores.TipoTomador.Destinatario ? "VEND" : "TRAN";
                cteINTNC.TipoFrete = cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? "FOB" : "CIF";
            }
            else
            {
                cteINTNC.CodigoTipoNota = cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? "VEND" : "TRAN";
                cteINTNC.TipoFrete = cte.CondicaoPagamento == "C" ? "CIF" : "FOB";
            }

            cteINTNC.FinalidadeOperacao = cte.Remetente.Atividade.Codigo.ToString();
            cteINTNC.IdNotaEntrada = 0;
            cteINTNC.CFOP = cte.CFOP.CodigoCFOP.ToString();
            cteINTNC.CodCFOPGrupo = cte.CFOP.CodigoCFOP.ToString().Substring(0, 1) + "000";
            cteINTNC.PesoLiquido = cte.Peso; //.Documentos.Sum(o => o.Peso);
            cteINTNC.PesoBruto = cte.Peso; //Documentos.Sum(o => o.Peso);
            cteINTNC.QtdVolumes = 0;

            if (cte.Status == "I")
                cteINTNC.ChaveCTe = "";
            else
                cteINTNC.ChaveCTe = cte.ChaveAcesso;

            cteINTNC.DataHoraAutorizacaoCTe = cte.DataEmissao;
            cteINTNC.ProtocoloCTe = cte.Protocolo;

            cteINTNC.Impostos = new List<Dominio.ObjetosDeValor.EDI.INTNC.Imposto>();


            if (configuracaoContaContabil != null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabil = (from obj in configuracoesContaContabilEscrituracao where obj.CodigoConfiguracaoContaContabil == configuracaoContaContabil.Codigo select obj).ToList();
                if (configuracoesContaContabil.Count == 0)
                {
                    configuracoesContaContabilEscrituracao.AddRange(configuracaoContaContabil.ConfiguracaoContaContabilEscrituracoes.ToList());
                    configuracoesContaContabil = (from obj in configuracoesContaContabilEscrituracao where obj.CodigoConfiguracaoContaContabil == configuracaoContaContabil.Codigo select obj).ToList();
                }

                //precisa estar nessa ordem quando geraer o arquivo.
                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> ordenada = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();

                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configIcmsST = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST select obj).FirstOrDefault();
                if (configIcmsST != null)
                    ordenada.Add(configIcmsST);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configIcms = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS select obj).FirstOrDefault();
                if (configIcms != null)
                    ordenada.Add(configIcms);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configISSRetido = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido select obj).FirstOrDefault();
                if (configISSRetido != null)
                    ordenada.Add(configISSRetido);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configISS = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS select obj).FirstOrDefault();
                if (configISS != null)
                    ordenada.Add(configISS);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configPIS = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS select obj).FirstOrDefault();
                if (configPIS != null)
                    ordenada.Add(configPIS);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configCOFINS = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS select obj).FirstOrDefault();
                if (configCOFINS != null)
                    ordenada.Add(configCOFINS);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configCBS = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.CBS select obj).FirstOrDefault();
                if (configCBS != null)
                    ordenada.Add(configCBS);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configIBSEstadual = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.IBSEstadual select obj).FirstOrDefault();
                if (configIBSEstadual != null)
                    ordenada.Add(configIBSEstadual);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configIBSMunicipal = (from obj in configuracoesContaContabil where obj.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.IBSMunicipal select obj).FirstOrDefault();
                if (configIBSMunicipal != null)
                    ordenada.Add(configIBSMunicipal);

                decimal? aliquotaPIS = 1.65m;
                decimal? aliquotaCOFINS = 7.60m;

                cteINTNC.BasePIS = basepiscofins;
                cteINTNC.AliquotaPIS = aliquotaPIS.Value;
                cteINTNC.ValorPIS = basepiscofins > 0 ? basepiscofins * (cteINTNC.AliquotaPIS / 100) : 0;
                cteINTNC.BaseCofins = basepiscofins;
                cteINTNC.AliquotaCofins = aliquotaCOFINS.Value;
                cteINTNC.ValorCofins = basepiscofins > 0 ? basepiscofins * (cteINTNC.AliquotaCofins / 100) : 0;

                foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao configuracaoContaContabilEscrituracao in ordenada)
                {
                    Dominio.ObjetosDeValor.EDI.INTNC.Imposto imposto = new Dominio.ObjetosDeValor.EDI.INTNC.Imposto();
                    imposto.IdNotaCobranca = cte.Codigo;
                    imposto.NumeroNotaCobranca = cte.Numero;
                    imposto.SequencialUnico = cte.Codigo + 5000000000;

                    if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS && cte.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao)
                    {
                        imposto.CodImposto = "001";
                        if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe || ((cte.CST == "40" || cte.CST == "") && !configuracaoContaContabilEscrituracao.SempreGerarRegistro))
                            continue;

                        bool enviarRegraExclusivaCodigoImpostoLayoutINTNC = configuracaoEmbarcador.EnviarRegraExclusivaCodigoImpostoLayoutINTNC
                            && !string.IsNullOrEmpty(cte.CST)
                            && cte.CST == "90"
                            && (cte.CFOP.CodigoCFOP == 5932 || cte.CFOP.CodigoCFOP == 6932);

                        if (enviarRegraExclusivaCodigoImpostoLayoutINTNC)
                            imposto.CodImposto = "090";
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                    {
                        imposto.CodImposto = "003";
                        if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe || (!icmsST && !configuracaoContaContabilEscrituracao.SempreGerarRegistro))
                            continue;
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                    {
                        imposto.CodImposto = "054";
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                    {
                        imposto.CodImposto = "064";
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS)
                    {
                        imposto.CodImposto = "008";
                        if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                            continue;
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido)
                    {
                        imposto.CodImposto = "008";
                        if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                            continue;
                    }

                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.CBS)
                    {
                        imposto.CodImposto = "011";
                    }

                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.IBSEstadual)
                    {
                        imposto.CodImposto = "012";
                    }

                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.IBSMunicipal)
                    {
                        imposto.CodImposto = "013";
                    }
                    else
                    {
                        continue;
                    }

                    imposto.ValorBaseImposto = cte.BaseCalculoImposto;

                    imposto.TipoTributacaoICMS = "00";
                    imposto.TipoTributacao = "00";

                    imposto.ValorBasePIS = basepiscofins;
                    imposto.AliquotaPIS = aliquotaPIS.Value;
                    imposto.ValorPIS = basepiscofins > 0 ? basepiscofins * (imposto.AliquotaPIS / 100) : 0;

                    imposto.ValorBaseCOFINS = basepiscofins;
                    imposto.AliquotaCOFINS = aliquotaCOFINS.Value;
                    imposto.ValorCOFINS = basepiscofins > 0 ? basepiscofins * (imposto.AliquotaCOFINS / 100) : 0;


                    bool mudaRegraICMS = false;
                    if (empresaLayout != null)
                        mudaRegraICMS = true;

                    if ((!icmsST || !mudaRegraICMS) && configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMS)
                    {
                        if (cte.CST != "40" && cte.CST != "")
                        {
                            imposto.ValorImposto = cte.ValorImposto;
                            imposto.PercentualAliquitaImposto = cte.AliquotaImposto;
                            imposto.ValorBaseParaCreditoICMS = cte.BaseCalculoICMS;
                            imposto.ValorImpostoComCredito = cte.ValorICMS;
                            imposto.ValorApagarComCredito = cte.ValorAReceber;

                            imposto.PercentualAliquotaTipoImposto = imposto.PercentualAliquitaImposto;
                            imposto.ValorDoTipoImposto = imposto.ValorImposto;
                            imposto.ValorBaseTipoImposto = imposto.ValorBaseImposto;
                        }

                    }
                    else if (icmsST && configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ICMSST)
                    {
                        imposto.ValorImposto = cte.ValorImposto;
                        imposto.PercentualAliquitaImposto = cte.AliquotaImposto;

                        imposto.ValorBaseSemCreditoICMS = cte.BaseCalculoICMS;
                        imposto.ValorImpostoSemCredito = cte.ValorICMS;
                        imposto.ValorApagarSemCredito = cte.ValorAReceber;

                        imposto.PercentualAliquotaTipoImposto = imposto.PercentualAliquitaImposto;
                        imposto.ValorDoTipoImposto = imposto.ValorImposto;
                        imposto.ValorBaseTipoImposto = imposto.ValorBaseImposto;

                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISS)
                    {
                        imposto.ValorImposto = cte.ValorImposto - cte.ValorISSRetido;

                        if (imposto.ValorImposto <= 0 && !configuracaoContaContabilEscrituracao.SempreGerarRegistro)
                            continue;

                        imposto.PercentualAliquitaImposto = cte.AliquotaImposto;
                        imposto.PercentualAliquotaTipoImposto = imposto.PercentualAliquitaImposto;
                        imposto.ValorDoTipoImposto = imposto.ValorImposto;
                        imposto.ValorBaseTipoImposto = imposto.ValorBaseImposto;
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.ISSRetido)
                    {
                        imposto.ValorImposto = cte.ValorISSRetido;

                        if (imposto.ValorImposto <= 0 && !configuracaoContaContabilEscrituracao.SempreGerarRegistro)
                            continue;

                        imposto.PercentualAliquitaImposto = cte.AliquotaImposto;
                        imposto.PercentualAliquotaTipoImposto = imposto.PercentualAliquitaImposto;
                        imposto.ValorDoTipoImposto = imposto.ValorImposto;
                        imposto.ValorBaseTipoImposto = imposto.ValorBaseImposto;
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.PIS)
                    {
                        imposto.PercentualAliquotaTipoImposto = imposto.AliquotaPIS;
                        imposto.ValorDoTipoImposto = imposto.ValorPIS;
                        imposto.ValorBaseTipoImposto = imposto.ValorBasePIS;
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.COFINS)
                    {
                        imposto.PercentualAliquotaTipoImposto = imposto.AliquotaCOFINS;
                        imposto.ValorDoTipoImposto = imposto.ValorCOFINS;
                        imposto.ValorBaseTipoImposto = imposto.ValorBaseCOFINS;
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.CBS)
                    {
                        imposto.PercentualAliquitaImposto = cte.AliquotaCBS;
                        imposto.ValorImposto = cte.ValorCBS;
                        imposto.ValorBaseImposto = cte.BaseCalculoIBSCBS;
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.IBSEstadual)
                    {
                        imposto.PercentualAliquitaImposto = cte.AliquotaIBSEstadual;
                        imposto.ValorImposto = cte.ValorIBSEstadual;
                        imposto.ValorBaseImposto = cte.BaseCalculoIBSCBS;
                    }
                    else if (configuracaoContaContabilEscrituracao.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.IBSMunicipal)
                    {
                        imposto.PercentualAliquitaImposto = cte.AliquotaIBSMunicipal;
                        imposto.ValorImposto = cte.ValorIBSMunicipal;
                        imposto.ValorBaseImposto = cte.BaseCalculoIBSCBS;
                    }

                    Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas outrasAliquotas = cte?.OutrasAliquotas ?? repositorioOutrasAliquotas.BuscarPorCSTClassificacaoTributaria(cte.CSTIBSCBS, cte.ClassificacaoTributariaIBSCBS);

                    imposto.ImpostoEstatisticoCBS = (outrasAliquotas?.CalcularImpostoDocumento ?? false) ? "0" : "1";
                    imposto.PercentualAliquotaCBS = cte.AliquotaCBS;
                    imposto.ValorCBS = cte.ValorCBS;
                    imposto.ValorBaseCalculoIBSCBS = cte.BaseCalculoIBSCBS;
                    imposto.ImpostoEstatisticoIBSEstadual = (outrasAliquotas?.CalcularImpostoDocumento ?? false) ? "0" : "1";
                    imposto.PercentualAliquotaIBSEstadual = cte.AliquotaIBSEstadual;
                    imposto.ValorIBSEstadual = cte.ValorIBSEstadual;
                    imposto.ImpostoEstatisticoIBSMunicipal = (outrasAliquotas?.CalcularImpostoDocumento ?? false) ? "0" : "1";
                    imposto.PercentualAliquotaIBSMunicipal = cte.AliquotaIBSMunicipal;
                    imposto.ValorIBSMunicipal = cte.ValorIBSMunicipal;
                    imposto.CSTIBSCBS = cte.CSTIBSCBS;
                    imposto.ClassificacaoTributariaIBSCBS = cte.ClassificacaoTributariaIBSCBS;


                    cteINTNC.Impostos.Add(imposto);
                }
            }

            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);

            Dominio.Entidades.DocumentosCTE documentosCTe = repDocumentoCTe.BuscarPrimeiroPorCTe(cte.Codigo); //cte.Documentos.ToList();

            if (documentosCTe == null && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && !string.IsNullOrWhiteSpace(cte.ChaveCTESubComp))
                documentosCTe = repDocumentoCTe.BuscarPrimeiroPorChaveCTe(cte.ChaveCTESubComp);

            Dominio.ObjetosDeValor.EDI.INTNC.NotaFiscal notas = new Dominio.ObjetosDeValor.EDI.INTNC.NotaFiscal
            {
                IdNotaCobranca = cte.Codigo,
                NumeroNotaCobranca = cte.Numero,
                SequencialUnico = cte.Codigo + 5000000000,
                CNPJTomador = cte.TomadorPagador?.CPF_CNPJ_SemFormato,
                CodNotaFiscal = documentosCTe?.Numero ?? "",
                CodSerieNotaFiscal = documentosCTe?.SerieOuSerieDaChave ?? "",
                TipoPessoaEmitente = "2",
                CNPJEmitente = cte.Remetente?.CPF_CNPJ_SemFormato,
                CNPJDestinatario = cte.Destinatario?.CPF_CNPJ_SemFormato,
                CodNaturezaOperacao = string.Empty,
                PesoBruto = documentosCTe?.Peso ?? 0,
                PesoLiquido = documentosCTe?.Peso ?? 0,
                ValorTotalNotas = documentosCTe?.Peso ?? 0,
                DataEmissaoNotaFiscal = documentosCTe?.DataEmissao ?? DateTime.Now,
                StatusCreditoICMS = "1"
            };

            cteINTNC.Notas = new List<Dominio.ObjetosDeValor.EDI.INTNC.NotaFiscal> { notas };

            cteINTNC.NotasFiscaisCTes = new List<Dominio.ObjetosDeValor.EDI.INTNC.NotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal in cte.XMLNotaFiscais.ToList())
            {
                if (escriturarSomenteDocsNFe && xMLNotaFiscal.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe)
                    return null;

                Dominio.ObjetosDeValor.EDI.INTNC.NotaFiscal notaCTe = new Dominio.ObjetosDeValor.EDI.INTNC.NotaFiscal
                {
                    IdNotaCobranca = xMLNotaFiscal.Codigo,
                    NumeroNotaCobranca = cte.Numero,
                    SequencialUnico = cte.Codigo + 5000000000,
                    CNPJTomador = cte.TomadorPagador?.CPF_CNPJ_SemFormato,
                    CodNotaFiscal = xMLNotaFiscal.Numero.ToString(),
                    CodSerieNotaFiscal = xMLNotaFiscal.Serie,
                    TipoPessoaEmitente = xMLNotaFiscal.Emitente.Tipo == "J" ? "2" : "1",
                    TipoPessoaEmitentePF = xMLNotaFiscal.Emitente.Tipo == "F" ? "F" : "J",
                    CNPJEmitente = xMLNotaFiscal.Emitente.CPF_CNPJ_SemFormato,
                    IEEmitente = xMLNotaFiscal.Emitente.Tipo == "J" ? xMLNotaFiscal.Emitente.IE_RG : string.Empty,
                    UFEmitente = xMLNotaFiscal.Emitente.Localidade.Estado.Sigla,
                    CNPJDestinatario = xMLNotaFiscal.Destinatario.CPF_CNPJ_SemFormato,
                    CodNaturezaOperacao = xMLNotaFiscal.NaturezaOP,
                    PesoBruto = xMLNotaFiscal.Peso,
                    PesoLiquido = xMLNotaFiscal.PesoLiquido,
                    ValorTotalNotas = xMLNotaFiscal.Valor,
                    DataEmissaoNotaFiscal = xMLNotaFiscal.DataEmissao,
                    StatusCreditoICMS = "1"
                };

                int.TryParse(Utilidades.String.OnlyNumbers(xMLNotaFiscal.NaturezaOP), out int cfop);
                notaCTe.CFOP = cfop;

                cteINTNC.NotasFiscaisCTes.Add(notaCTe);
            }

            cteINTNC.ItemConhecimento = cteINTNC;

            Dominio.ObjetosDeValor.EDI.INTNC.ColetaEntrega coletaEntrega = new Dominio.ObjetosDeValor.EDI.INTNC.ColetaEntrega();

            Dominio.Entidades.ParticipanteCTe participanteColeta = cte.Expedidor;
            Dominio.Entidades.ParticipanteCTe participanteEntrega = cte.Recebedor;

            if (participanteColeta == null)
                participanteColeta = cte.Remetente;

            if (participanteEntrega == null)
                participanteEntrega = cte.Destinatario;

            if (participanteColeta != null)
            {
                coletaEntrega.CPFCNPJColeta = participanteColeta.CPF_CNPJ;
                coletaEntrega.TipoPessoaColeta = participanteColeta.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? "F" : "J";
                coletaEntrega.UFColeta = participanteColeta.Localidade?.Estado?.Sigla;
                coletaEntrega.IBGEColeta = participanteColeta.Localidade?.CodigoIBGE ?? 0;
                coletaEntrega.IEColeta = participanteColeta.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? participanteColeta.IE_RG : string.Empty;
            }

            if (participanteEntrega != null)
            {
                coletaEntrega.CPFCNPJEntrega = participanteEntrega.CPF_CNPJ;
                coletaEntrega.TipoPessoaEntrega = participanteEntrega.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? "F" : "J";
                coletaEntrega.IEEntrega = participanteEntrega.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? participanteEntrega.IE_RG : string.Empty;
                coletaEntrega.UFEntrega = participanteEntrega.Localidade?.Estado?.Sigla;
                coletaEntrega.IBGEEntrega = participanteEntrega.Localidade?.CodigoIBGE ?? 0;
            }

            cteINTNC.ColetasEntregas = new List<Dominio.ObjetosDeValor.EDI.INTNC.ColetaEntrega>() { coletaEntrega };

            cteINTNC.Tomadores = new List<Dominio.ObjetosDeValor.EDI.INTNC.Tomador>()
            {
                 new Dominio.ObjetosDeValor.EDI.INTNC.Tomador()
                 {
                      Nome = cte.TomadorPagador?.Nome,
                      TipoPessoa = cte.TomadorPagador?.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? "F" : "J",
                      CPFCNPJ = cte.TomadorPagador?.CPF_CNPJ,
                      SiglaPais = "BR",
                      SiglaEstado = cte.TomadorPagador?.Localidade?.Estado.Sigla,
                      Cidade = cte.TomadorPagador?.Localidade?.Descricao,
                      NumeroCidade = string.Format("{0:0000000}", cte.TomadorPagador?.Localidade?.CodigoIBGE ?? 0).Left(5),
                      Endereco = cte.TomadorPagador?.Endereco,
                      Bairro = cte.TomadorPagador?.Bairro,
                      Telefone = Utilidades.String.OnlyNumbers( cte.TomadorPagador?.Telefone1),
                      Fax = Utilidades.String.OnlyNumbers(cte.TomadorPagador?.Telefone2),
                      CEP = Utilidades.String.OnlyNumbers(cte.TomadorPagador?.CEP),
                      InscricaoEstadual = cte.TomadorPagador?.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica ? cte.TomadorPagador?.IE_RG : "",
                      InscricaoMunicipal = cte.TomadorPagador?.InscricaoMunicipal,
                      InscricaoSuframa = cte.TomadorPagador?.InscricaoSuframa,
                      //ProdutorRural = cte.TomadorPagador?.Atividade?.Codigo == 6 ? "S":"N",
                      //Autonomo = "N",
                      //AssistenciaTecnica = "N",
                      //DistribuidorComum = "N",
                      //DistribuidorEquiparado = "N",
                      //Industria = cte.TomadorPagador?.Atividade?.Codigo == 2 ? "S":"N",
                      //MicroEmpresa = "N",
                      //PrestadorServicos = cte.TomadorPagador?.Atividade?.Codigo == 4 ? "S":"N",
                      //GeraCreditoICMS = "N"
                 }
            };

            return cteINTNC;
        }

        public Dominio.ObjetosDeValor.EDI.INTNC.INTNC ConverterLoteEscrituracaoParaINTNC(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao loteEscrituracaoEDIIntegracao, Dominio.Entidades.Empresa empresaEDI, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            loteEscrituracaoEDIIntegracao.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao = loteEscrituracaoEDIIntegracao.LoteEscrituracao;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Log.TratarErro("Iniciou lote " + loteEscrituracao.Numero + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "LoteEscrituracao");

            Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = new Dominio.ObjetosDeValor.EDI.INTNC.INTNC();
            intnc.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento>();
            intnc.DataGeracao = DateTime.Now;

            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentoEscrituracaos = repDocumentoEscrituracao.BuscarPorloteEscrituracao(loteEscrituracao.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabilEscrituracao = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();


            for (var i = 0; i < documentoEscrituracaos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao documentoEscrituracao = documentoEscrituracaos[i];
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                if (documentoEscrituracao.Carga != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = (from obj in cargas where obj.Codigo == documentoEscrituracao.Carga.Codigo select obj).FirstOrDefault();
                    if (carga == null)
                    {
                        carga = documentoEscrituracao.Carga;
                        cargas.Add(carga);
                    }
                    tipoOperacao = carga.TipoOperacao;
                }

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = null;
                if (documentoEscrituracao.CargaOcorrencia != null)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = (from obj in ocorrencias where obj.Codigo == documentoEscrituracao.CargaOcorrencia.Codigo select obj).FirstOrDefault();
                    if (ocorrencia == null)
                    {
                        ocorrencia = documentoEscrituracao.CargaOcorrencia;
                        ocorrencias.Add(ocorrencia);
                    }
                    tipoDeOcorrencia = ocorrencia.TipoOcorrencia;
                    tipoOperacao = ocorrencia.Carga?.TipoOperacao;
                }

                if (documentoEscrituracao.FechamentoFrete != null)
                    tipoOperacao = documentoEscrituracao.TipoOperacao;


                Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento doc = ObterCTeINTNC(loteEscrituracao.Descricao, loteEscrituracao.DataGeracaoLote.HasValue ? loteEscrituracao.DataGeracaoLote.Value : DateTime.Now, documentoEscrituracao.CTe, empresaEDI, tipoOperacao, null, documentoEscrituracao.CTe.Empresa, tipoDeOcorrencia, unidadeTrabalho, configuracoesContaContabilEscrituracao, configuracaoFinanceiro, configuracaoEmbarcador);
                if (doc != null)
                {
                    intnc.Conhecimentos.Add(doc);
                    loteEscrituracaoEDIIntegracao.CodigosCTes.Add(documentoEscrituracao.CTe.Codigo);
                }

                Servicos.Log.TratarErro(i.ToString() + " - " + documentoEscrituracao.CTe.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "LoteEscrituracao");
            }

            SetarTotaisRodape(ref intnc);
            Servicos.Log.TratarErro("Finalizou lote " + loteEscrituracao.Numero + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "LoteEscrituracao");

            return intnc;
        }

        public Dominio.ObjetosDeValor.EDI.INTNC.INTNC ConverterLoteEscrituracaoCancelamentoParaINTNC(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao, Dominio.Entidades.Empresa empresaEDI, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            loteEscrituracaoCancelamentoEDIIntegracao.CodigosCTes = new List<int>();

            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento = loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento;

            Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = new Dominio.ObjetosDeValor.EDI.INTNC.INTNC
            {
                Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento>(),
                DataGeracao = DateTime.Now
            };

            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> documentosEscrituracaoCancelamento = repDocumentoEscrituracaoCancelamento.BuscarPorLoteEscrituracaoCancelamento(loteEscrituracaoCancelamento.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabilEscrituracao = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            int countDocumentos = documentosEscrituracaoCancelamento.Count();

            for (int i = 0; i < countDocumentos; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento documentoEscrituracaoCancelamento = documentosEscrituracaoCancelamento[i];
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = null;

                if (documentoEscrituracaoCancelamento.Carga != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas.Where(o => o.Codigo == documentoEscrituracaoCancelamento.Carga.Codigo).FirstOrDefault();

                    if (carga == null)
                    {
                        carga = documentoEscrituracaoCancelamento.Carga;

                        cargas.Add(carga);
                    }

                    tipoOperacao = carga.TipoOperacao;
                }

                if (documentoEscrituracaoCancelamento.CargaOcorrencia != null)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = ocorrencias.Where(o => o.Codigo == documentoEscrituracaoCancelamento.CargaOcorrencia.Codigo).FirstOrDefault();

                    if (ocorrencia == null)
                    {
                        ocorrencia = documentoEscrituracaoCancelamento.CargaOcorrencia;

                        ocorrencias.Add(ocorrencia);
                    }

                    tipoDeOcorrencia = ocorrencia.TipoOcorrencia;
                    tipoOperacao = ocorrencia.Carga?.TipoOperacao;
                }

                Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento doc = ObterCTeINTNC(loteEscrituracaoCancelamento.Descricao, loteEscrituracaoCancelamento.DataGeracaoLote ?? DateTime.Now, documentoEscrituracaoCancelamento.CTe, empresaEDI, tipoOperacao, null, documentoEscrituracaoCancelamento.CTe.Empresa, tipoDeOcorrencia, unidadeTrabalho, configuracoesContaContabilEscrituracao, configuracaoFinanceiro, configuracaoEmbarcador);

                if (doc != null)
                {
                    intnc.Conhecimentos.Add(doc);

                    loteEscrituracaoCancelamentoEDIIntegracao.CodigosCTes.Add(documentoEscrituracaoCancelamento.CTe.Codigo);
                }
            }

            SetarTotaisRodape(ref intnc);

            return intnc;
        }

        private void SetarTotaisRodape(ref Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc)
        {
            intnc.Rodape = new Dominio.ObjetosDeValor.EDI.INTNC.Rodape();
            intnc.Rodape.Contadores = intnc.Conhecimentos.Count();
            intnc.Rodape.Somatorios = intnc.Conhecimentos.Count();
        }

        public Dominio.ObjetosDeValor.EDI.INTNC.INTNC ConverterCargaParaINTNC(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI nfsManualCancelamentoIntegracaoEDI, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = new Dominio.ObjetosDeValor.EDI.INTNC.INTNC();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            intnc.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento>();
            intnc.DataGeracao = DateTime.Now;
            nfsManualCancelamentoIntegracaoEDI.CodigosCTes = new List<int>();
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabilEscrituracao = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();
            Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento doc = ObterCTeINTNC(nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.CTe.Numero.ToString(), DateTime.Now, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.CTe, nfsManualCancelamentoIntegracaoEDI.Empresa, null, null, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.Transportador, null, unidadeTrabalho, configuracoesContaContabilEscrituracao, configuracaoFinanceiro, configuracaoEmbarcador);
            if (doc != null)
            {
                intnc.Conhecimentos.Add(doc);
                nfsManualCancelamentoIntegracaoEDI.CodigosCTes.Add(nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.CTe.Codigo);
                SetarTotaisRodape(ref intnc);
            }
            return intnc;
        }

        public Dominio.ObjetosDeValor.EDI.INTNC.INTNC ConverterCargaParaINTNC(Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao nfsManualEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = new Dominio.ObjetosDeValor.EDI.INTNC.INTNC();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            intnc.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento>();
            intnc.DataGeracao = DateTime.Now;
            nfsManualEDIIntegracao.CodigosCTes = new List<int>();
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabilEscrituracao = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();
            Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento doc = ObterCTeINTNC(nfsManualEDIIntegracao.LancamentoNFSManual.CTe.Numero.ToString(), DateTime.Now, nfsManualEDIIntegracao.LancamentoNFSManual.CTe, nfsManualEDIIntegracao.Empresa, null, null, nfsManualEDIIntegracao.LancamentoNFSManual.Transportador, null, unidadeTrabalho, configuracoesContaContabilEscrituracao, configuracaoFinanceiro, configuracaoEmbarcador);
            if (doc != null)
            {
                intnc.Conhecimentos.Add(doc);
                nfsManualEDIIntegracao.CodigosCTes.Add(nfsManualEDIIntegracao.LancamentoNFSManual.CTe.Codigo);
                SetarTotaisRodape(ref intnc);
            }

            return intnc;
        }

        public Dominio.ObjetosDeValor.EDI.INTNC.INTNC ConverterCargaParaINTNC(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            cargaEDIIntegracao.CodigosCTes = new List<int>();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEDIIntegracao.Carga;
            Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = new Dominio.ObjetosDeValor.EDI.INTNC.INTNC();
            intnc.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento>();
            intnc.DataGeracao = DateTime.Now;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<int> empresas = new List<int>();
            if (cargaEDIIntegracao.Empresa != null)
            {
                if (cargaEDIIntegracao.Empresa.Codigo == (cargaEDIIntegracao.Carga.EmpresaFilialEmissora?.Codigo ?? 0))
                    empresas = repCargaPedido.ObterEmpresasCargaFilialEmissora(carga.Codigo);
                else
                    empresas.Add(cargaEDIIntegracao.Empresa.Codigo);
            }
            else if (cargaEDIIntegracao.Carga.EmpresaFilialEmissora != null)
                empresas = repCargaPedido.ObterEmpresasCargaFilialEmissora(carga.Codigo);


            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargaCTesSemSubcontratacaoFilialEmissora(carga.Codigo, empresas, cargaEDIIntegracao.LayoutEDI.ModeloDocumentoFiscais?.Select(o => o.Codigo).ToList());

            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabilEscrituracao = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();
            for (var i = 0; i < cargaCTes.Count; i++)
            {
                Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento doc = ObterCTeINTNC(carga.CodigoCargaEmbarcador, carga.DataFinalizacaoEmissao.HasValue ? carga.DataFinalizacaoEmissao.Value : DateTime.Now, cargaCTes[i].CTe, cargaEDIIntegracao.Empresa, carga.TipoOperacao, carga.Rota, cargaCTes[i].CTe.Empresa, null, unidadeTrabalho, configuracoesContaContabilEscrituracao, configuracaoFinanceiro, configuracaoEmbarcador);
                if (doc != null)
                {
                    intnc.Conhecimentos.Add(doc);
                    cargaEDIIntegracao.CodigosCTes.Add(cargaCTes[i].CTe.Codigo);
                }
            }

            SetarTotaisRodape(ref intnc);

            return intnc;
        }

        public Dominio.ObjetosDeValor.EDI.INTNC.INTNC ConverterCargaCancelamentoParaINTNC(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI, Repositorio.UnitOfWork unidadeTrabalho)
        {
            cargaCancelamentoIntegracaoEDI.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga;
            Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = new Dominio.ObjetosDeValor.EDI.INTNC.INTNC();
            intnc.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento>();
            intnc.DataGeracao = DateTime.Now;
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargaCTesCanceladosSemSubcontratacaoFilialEmissora(carga.Codigo, cargaCancelamentoIntegracaoEDI.Empresa?.Codigo ?? 0);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabilEscrituracao = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();
            for (var i = 0; i < cargaCTes.Count; i++)
            {
                Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento doc = ObterCTeINTNC(carga.CodigoCargaEmbarcador, DateTime.Now, cargaCTes[i].CTe, cargaCancelamentoIntegracaoEDI.Empresa, carga.TipoOperacao, carga.Rota, cargaCTes[i].CTe.Empresa, null, unidadeTrabalho, configuracoesContaContabilEscrituracao, configuracaoFinanceiro, configuracaoEmbarcador);
                if (doc != null)
                {
                    intnc.Conhecimentos.Add(doc);
                    cargaCancelamentoIntegracaoEDI.CodigosCTes.Add(cargaCTes[i].CTe.Codigo);
                }
            }
            SetarTotaisRodape(ref intnc);
            return intnc;
        }

        public Dominio.ObjetosDeValor.EDI.INTNC.INTNC ConverterOcorrenciaParaINTNC(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao ocorrenciaEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            ocorrenciaEDIIntegracao.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = ocorrenciaEDIIntegracao.CargaOcorrencia;
            Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = new Dominio.ObjetosDeValor.EDI.INTNC.INTNC();
            intnc.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo, ocorrenciaEDIIntegracao.FilialEmissora);
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracoesContaContabilEscrituracao = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();

            for (var i = 0; i < cargaCTesComplementoInfo.Count; i++)
            {
                Dominio.ObjetosDeValor.EDI.INTNC.Conhecimento doc = ObterCTeINTNC(cargaOcorrencia.Carga.CodigoCargaEmbarcador, cargaOcorrencia.DataFinalizacaoEmissaoOcorrencia ?? DateTime.Now, cargaCTesComplementoInfo[i].CTe, ocorrenciaEDIIntegracao.Empresa, cargaOcorrencia.Carga.TipoOperacao, cargaOcorrencia.Carga.Rota, cargaCTesComplementoInfo[i].CTe.Empresa, cargaOcorrencia.TipoOcorrencia, unidadeTrabalho, configuracoesContaContabilEscrituracao, configuracaoFinanceiro, configuracaoEmbarcador);
                if (doc != null)
                {
                    intnc.Conhecimentos.Add(doc);
                    ocorrenciaEDIIntegracao.CodigosCTes.Add(cargaCTesComplementoInfo[i].CTe.Codigo);
                }
            }
            return intnc;
        }
    }
}
