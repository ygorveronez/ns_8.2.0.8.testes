using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class DOCCOB
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOB ConverterTituloParaDOCCOB(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOB doccob = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOB();

            doccob.Data = DateTime.Now;
            doccob.Destinatario = titulo.Pessoa.Nome;
            doccob.DestinatarioDetalhes = svcPessoa.ConverterObjetoPessoa(titulo.Pessoa);
            doccob.Remetente = titulo.Empresa?.RazaoSocial ?? "";
            doccob.RemetenteDetalhes = svcPessoa.ConverterObjetoEmpresa(titulo.Empresa);
            doccob.Intercambio = "COB" + DateTime.Now.ToString("ddMMHHmm") + "1";
            doccob.SequenciaGeracaoArquivo = 1;// fatura.Numero;

            doccob.CabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.DOCCOB.CabecalhoDocumento();
            doccob.CabecalhoDocumento.IdentificacaoDocumento = "COBRA" + DateTime.Now.ToString("ddMMHHmm") + "1";
            doccob.CabecalhoDocumento.Intercambio = "COB" + DateTime.Now.ToString("ddMMHHmm") + "1";

            doccob.CabecalhoDocumento.Transportadores = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.Transportador>();

            Dominio.ObjetosDeValor.EDI.DOCCOB.Transportador transportador = new Dominio.ObjetosDeValor.EDI.DOCCOB.Transportador();
            transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(titulo.Empresa);
            transportador.CNPJSacado = titulo.Pessoa?.CPF_CNPJ_SemFormato;
            transportador.IESacado = titulo.Pessoa?.IE_RG;

            doccob.CabecalhoDocumento.Total = new Dominio.ObjetosDeValor.EDI.DOCCOB.Total();
            doccob.CabecalhoDocumento.Total.QuantidadeTotalDocumentosCobranca = 1;
            doccob.CabecalhoDocumento.Total.ValorTotalDocumentosCobranca = titulo.ValorTotal;

            transportador.DocumentosCobranca = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.DocumentoCobranca>();

            string[] conta = !string.IsNullOrWhiteSpace(titulo.Pessoa.NumeroConta) ? titulo.Pessoa.NumeroConta.Split('-') : "".Split('-');

            Dominio.ObjetosDeValor.EDI.DOCCOB.DocumentoCobranca documentoCobranca = new Dominio.ObjetosDeValor.EDI.DOCCOB.DocumentoCobranca
            {
                SequenciaGeracaoArquivo = 1,
                FilialEmissora = titulo.Empresa?.RazaoSocial ?? "",
                FilialEmissoraCidade = titulo.Empresa?.Localidade?.Descricao ?? "",
                FilialEmissoraUF = titulo.Empresa?.Localidade?.Estado?.Sigla ?? "",
                FilialEmissoraNomeFantasia = titulo.Empresa?.NomeFantasia ?? "",
                CodigoCentroCusto = titulo.Empresa?.CodigoCentroCusto ?? "",
                CodigoEstabelecimento = titulo.Empresa?.CodigoEstabelecimento ?? "",
                CodigoAlternativoTomador = titulo.Pessoa?.CodigoAlternativo ?? "",
                TipoDocumento = "0",
                NumeroDocumento = titulo.Codigo,
                DataEmissao = titulo.DataEmissao.Value,
                DataVencimento = titulo.DataVencimento.Value,
                ValorDocumento = titulo.ValorTotal,
                TipoCobranca = "",
                AcaoDocumento = "I",
                SerieDocumento = "UN",
                CodigoTransportadora = "",
                NomeCliente = titulo.Pessoa.Nome,
                TipoFrete = "",
                ModalidadeFrete = "",
                CodigoDeposito = "",
                CNPJSacado = titulo.Pessoa?.CPF_CNPJ_SemFormato,
                IESacado = titulo.Pessoa?.IE_RG,
                CodigoBanco = titulo.Pessoa.Banco?.Codigo.ToString(),
                DescricaoBanco = titulo.Pessoa.Banco?.Descricao,
                DataInicialFatura = DateTime.Now.Date,
                DataFinalFatura = DateTime.Now.Date,
                DigitoVerificadorAgencia = titulo.Pessoa.DigitoAgencia,
                DigitoVerificadorContaCorrente = conta.Length > 1 ? conta[1].Trim() : "",
                ExistePreFatura = "N",
                NumeroPreFatura = 0,
                IdentificacaoAgenteCobranca = titulo.Pessoa.Banco?.Descricao,
                NumeroAgencia = titulo.Pessoa.Agencia,
                NumeroContaCorrente = conta.Length > 1 ? conta[0].Trim() : titulo.Pessoa.NumeroConta,
                DestinatarioDetalhes = doccob.DestinatarioDetalhes,
                RemetenteDetalhes = doccob.RemetenteDetalhes,
                Imposto = new Dominio.ObjetosDeValor.EDI.DOCCOB.DocumentoCobrancaImposto(),
                Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.ConhecimentoCobranca>()
            };

            for (var i = 0; i < titulo.Documentos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.TituloDocumento documento = titulo.Documentos[i];
                if (documento == null || documento.CTe == null)
                    continue;

                documentoCobranca.ValorTotalICMS += documento.CTe.ValorICMS;

                if (documento.CTe != null)
                {
                    documentoCobranca.Imposto.AliquotaISS = documento.CTe.AliquotaISS;
                    documentoCobranca.Imposto.BaseCalculoISS += documento.CTe.BaseCalculoISS;
                    documentoCobranca.Imposto.ValorTotalISS += documento.CTe.ValorISS;

                    if (documento.CTe.CST == "60")
                    {
                        documentoCobranca.Imposto.ValorTotalICMSST += documento.CTe.ValorICMS;
                        documentoCobranca.Imposto.BaseCalculoICMSST += documento.CTe.BaseCalculoICMS;
                        documentoCobranca.Imposto.AliquotaICMSST = documento.CTe.AliquotaICMS;
                    }
                    else
                    {
                        documentoCobranca.Imposto.BaseCalculoICMS += documento.CTe.BaseCalculoICMS;
                        documentoCobranca.Imposto.ValorTotalICMS += documento.CTe.ValorICMS;
                        documentoCobranca.Imposto.AliquotaICMS = documento.CTe.AliquotaICMS;
                    }

                }
                documentoCobranca.Conhecimentos.Add(ObterConhecimentoCobranca(documento.CTe, titulo.Codigo, unidadeTrabalho));
            }

            documentoCobranca.CNPJRemetente = documentoCobranca.Conhecimentos?.Select(c => c.Remetente.CPFCNPJSemFormato)?.FirstOrDefault() ?? "";
            transportador.DocumentosCobranca.Add(documentoCobranca);
            doccob.CabecalhoDocumento.Transportadores.Add(transportador);

            return doccob;
        }

        public Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOB ConverterFaturaParaDOCCOB(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOB doccob = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOB();

            doccob.Data = DateTime.Now;
            doccob.Destinatario = fatura.ClienteTomadorFatura.Nome;
            doccob.DestinatarioDetalhes = svcPessoa.ConverterObjetoPessoa(fatura.ClienteTomadorFatura);
            doccob.Remetente = fatura.Empresa.RazaoSocial;
            doccob.RemetenteDetalhes = svcPessoa.ConverterObjetoEmpresa(fatura.Empresa);
            doccob.DestinatarioCodigoCompanhia = fatura.ClienteTomadorFatura.CodigoCompanhia;
            doccob.RemetenteCNPJ = fatura.Empresa.CNPJ_SemFormato;
            doccob.TomadorCNPJ = fatura.ClienteTomadorFatura.CPF_CNPJ_SemFormato;
            doccob.Intercambio = "COB" + DateTime.Now.ToString("ddMMHHmm") + "1";
            doccob.SequenciaGeracaoArquivo = 1;// fatura.Numero;

            doccob.CabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.DOCCOB.CabecalhoDocumento();
            doccob.CabecalhoDocumento.IdentificacaoDocumento = "COBRA" + DateTime.Now.ToString("ddMMHHmm") + "1";
            doccob.CabecalhoDocumento.Intercambio = "COB" + DateTime.Now.ToString("ddMMHHmm") + "1";

            doccob.CabecalhoDocumento.Transportadores = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.Transportador>();

            Dominio.ObjetosDeValor.EDI.DOCCOB.Transportador transportador = new Dominio.ObjetosDeValor.EDI.DOCCOB.Transportador();
            transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(fatura.Empresa);
            transportador.CNPJSacado = fatura.ClienteTomadorFatura?.CPF_CNPJ_SemFormato;
            transportador.IESacado = fatura.ClienteTomadorFatura?.IE_RG;

            doccob.CabecalhoDocumento.Total = new Dominio.ObjetosDeValor.EDI.DOCCOB.Total();
            doccob.CabecalhoDocumento.Total.QuantidadeTotalDocumentosCobranca = 1;
            doccob.CabecalhoDocumento.Total.QuantidadeTotalConhecimentosCobranca = 1;
            doccob.CabecalhoDocumento.Total.ValorTotalDocumentosCobranca = fatura.Total;

            transportador.DocumentosCobranca = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.DocumentoCobranca>();

            string[] conta = !string.IsNullOrWhiteSpace(fatura.NumeroConta) ? fatura.NumeroConta.Split('-') : "".Split('-');

            Dominio.ObjetosDeValor.EDI.DOCCOB.DocumentoCobranca documentoCobranca = new Dominio.ObjetosDeValor.EDI.DOCCOB.DocumentoCobranca
            {
                SequenciaGeracaoArquivo = 1,
                FilialEmissora = fatura.Empresa.RazaoSocial,
                FilialEmissoraCidade = fatura.Empresa?.Localidade?.Descricao ?? "",
                FilialEmissoraUF = fatura.Empresa?.Localidade?.Estado?.Sigla ?? "",
                FilialEmissoraNomeFantasia = fatura.Empresa?.NomeFantasia ?? "",
                CodigoCentroCusto = fatura.Empresa.CodigoCentroCusto,
                CodigoEstabelecimento = fatura.Empresa.CodigoEstabelecimento,
                CodigoAlternativoTomador = fatura.ClienteTomadorFatura.CodigoAlternativo,
                CNPJFilialEmissora = fatura.Empresa.CNPJ_SemFormato,
                TipoDocumento = "0",
                NumeroDocumento = fatura.Numero,
                DataEmissao = fatura.DataFatura,
                DataVencimento = fatura.Parcelas.OrderBy(o => o.DataVencimento).First().DataVencimento,
                ValorDocumento = fatura.Total,
                TipoCobranca = "",
                AcaoDocumento = "I",
                SerieDocumento = "UN",
                CodigoTransportadora = fatura.CodigoTransportadora,
                NomeCliente = fatura.NomeCliente,
                TipoFrete = fatura.TipoFrete,
                ModalidadeFrete = fatura.ModalidadeFrete,
                CodigoDeposito = fatura.CodigoDeposito,
                CNPJSacado = fatura.ClienteTomadorFatura?.CPF_CNPJ_SemFormato,
                IESacado = fatura.ClienteTomadorFatura?.IE_RG,
                CodigoBanco = fatura.Banco?.Codigo.ToString(),
                DescricaoBanco = fatura.Banco?.Descricao,
                DataInicialFatura = fatura.DataInicial.HasValue ? fatura.DataInicial.Value : DateTime.Now.Date,
                DataFinalFatura = fatura.DataFinal.HasValue ? fatura.DataFinal.Value : DateTime.Now.Date,
                DigitoVerificadorAgencia = fatura.DigitoAgencia,
                DigitoVerificadorContaCorrente = conta.Length > 1 ? conta[1].Trim() : "",
                ExistePreFatura = fatura.NumeroPreFatura > 0L ? "S" : "N",
                NumeroPreFatura = fatura.NumeroPreFatura,
                IdentificacaoAgenteCobranca = fatura.Banco?.Descricao,
                NumeroAgencia = fatura.Agencia,
                DestinatarioDetalhes = doccob.DestinatarioDetalhes,
                RemetenteDetalhes = doccob.RemetenteDetalhes,
                NumeroContaCorrente = conta.Length > 1 ? conta[0].Trim() : fatura.NumeroConta,
                Imposto = new Dominio.ObjetosDeValor.EDI.DOCCOB.DocumentoCobrancaImposto(),
                Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.ConhecimentoCobranca>()
            };

            if (!fatura.NovoModelo)
            {
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> documentosFatura = repFaturaCargaDocumento.BuscarDocumentosFatura(fatura.Codigo);

                for (var i = 0; i < documentosFatura.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documentoCargaFatura = documentosFatura[i];

                    if (documentoCargaFatura.StatusDocumentoFatura != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido)
                    {
                        documentoCobranca.ValorTotalICMS += documentoCargaFatura.ConhecimentoDeTransporteEletronico.ValorICMS;

                        documentoCobranca.Conhecimentos.Add(ObterConhecimentoCobranca(documentoCargaFatura.ConhecimentoDeTransporteEletronico, fatura.Numero, unidadeTrabalho));
                    }
                }
            }
            else
            {
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(fatura.Codigo);

                for (var i = 0; i < documentos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaDocumento documento = documentos[i];

                    documentoCobranca.ValorTotalICMS += documento.Documento.ValorICMS;

                    if (documento.Documento.CTe != null)
                    {
                        documentoCobranca.Imposto.AliquotaISS = documento.Documento.CTe.AliquotaISS;
                        documentoCobranca.Imposto.BaseCalculoISS += documento.Documento.CTe.BaseCalculoISS;
                        documentoCobranca.Imposto.ValorTotalISS += documento.Documento.CTe.ValorISS;

                        if (documento.Documento.CTe.CST == "60")
                        {
                            documentoCobranca.Imposto.ValorTotalICMSST += documento.Documento.CTe.ValorICMS;
                            documentoCobranca.Imposto.BaseCalculoICMSST += documento.Documento.CTe.BaseCalculoICMS;
                            documentoCobranca.Imposto.AliquotaICMSST = documento.Documento.CTe.AliquotaICMS;
                        }
                        else
                        {
                            documentoCobranca.Imposto.BaseCalculoICMS += documento.Documento.CTe.BaseCalculoICMS;
                            documentoCobranca.Imposto.ValorTotalICMS += documento.Documento.CTe.ValorICMS;
                            documentoCobranca.Imposto.AliquotaICMS = documento.Documento.CTe.AliquotaICMS;
                        }

                    }

                    if (documento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe)
                        documentoCobranca.Conhecimentos.Add(ObterConhecimentoCobranca(documento.Documento.CTe, fatura.Numero, unidadeTrabalho));
                    else if (documento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in documento.Documento.Carga.CargaCTes)
                        {
                            if (cargaCTe.CargaCTeComplementoInfo == null)
                                documentoCobranca.Conhecimentos.Add(ObterConhecimentoCobranca(cargaCTe.CTe, fatura.Numero, unidadeTrabalho));
                        }
                    }
                }
            }
            doccob.CabecalhoDocumento.Total.QuantidadeTotalConhecimentosCobranca = documentoCobranca.Conhecimentos != null ? documentoCobranca.Conhecimentos.Count : 1;
            documentoCobranca.CNPJRemetente = documentoCobranca.Conhecimentos?.Select(c => c.Remetente.CPFCNPJSemFormato)?.FirstOrDefault() ?? "";
            transportador.DocumentosCobranca.Add(documentoCobranca);
            doccob.CabecalhoDocumento.Transportadores.Add(transportador);

            return doccob;
        }

        public Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillar ConverterFaturaParaDOCCOBCaterpillar(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(fatura.Codigo);

            Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillar doccob = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillar();
            int qtdRegistros = 0;
            decimal valorTotalRegistros = 0;

            qtdRegistros++;
            doccob.IdentificadorRegistro = "ITP";
            doccob.IdentificadorProcesso = 14;
            doccob.NumeroVersaoTransacao = 1;
            doccob.NumeroControleTransmissao = 7771;
            doccob.IdentificacaoGeracaoMovimento = DateTime.Now;
            doccob.IdentificadorTransmissor = fatura.Empresa.CNPJ;
            doccob.IdentificadorReceptor = fatura.ClienteTomadorFatura.CPF_CNPJ_SemFormato;
            doccob.CodigoInternoTransmissor = "Q5875F1";
            doccob.CodigoInternoReceptor = "GE";
            doccob.NomeTransmissor = fatura.Empresa.RazaoSocial;
            doccob.NomeReceptor = fatura.ClienteTomadorFatura.Nome;
            doccob.DT1 = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillarDT1()
            {
                IdentificadorRegistro = "DT1",
                NumeroDuplicata = fatura.Numero,
                DataEmissao = fatura.DataFatura,
                QuantidadeConhecimentos = documentos.Count(),
                ValorTotalFatura = fatura.Total,
                DataVencimento = fatura.Parcelas.OrderBy(o => o.DataVencimento).First().DataVencimento,
                TipoDocumento = 0,
                NumeroExportacao = 0,
                DataEmbarque = DateTime.MinValue,
                NumeroViagens = 0,
                LocalDescarga = "",
                LocalEmbarque = "",
                NumeroConhecimentoEmbarque = "",
                Material = "",
                RetencaoINSS = 0
            };
            qtdRegistros++;
            doccob.DT3 = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillarDT3()
            {
                IdentificadorRegistro = "DT3",
                DescontoINSS = 0,
                ValorBruto = fatura.Total,
                ValorLiquido = fatura.Total,
                MeioTransporte = "",
                NumeroPedido = "",
                NumeroExportacao = 0,
                NumeroAduaneiro = 0,
                ValorDespesas = 0,
                ValorImpostoRenda = 0
            };
            qtdRegistros++;
            doccob.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillarDT2>();

            for (var i = 0; i < documentos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento documento = documentos[i];
                Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillarDT2 cte = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillarDT2()
                {
                    IdentificadorRegistro = "DT2",
                    Numero = documento.Documento.CTe.Numero,
                    Serie = documento.Documento.CTe.Serie.Numero,
                    Valor = documento.Documento.CTe.ValorAReceber,
                    DataEmissao = documento.Documento.CTe.DataEmissao.Value,
                    Servicos = "",
                    ValorDolar = 0,
                    TaxaDolar = 0,
                    ValorServicos = 0
                };
                qtdRegistros++;
                doccob.Conhecimentos.Add(cte);
                valorTotalRegistros += documento.Documento.CTe.ValorAReceber;
            }

            qtdRegistros++;
            doccob.FTP = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillarFTP()
            {
                IdentificadoRegistro = "FTP",
                NumeroControleTransmissao = 7771,
                QuantidadeRegistros = qtdRegistros,
                TotalValores = valorTotalRegistros,
                CategoriaOperacao = ""
            };

            return doccob;
        }

        public Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBVaxxinova ConverterFaturaParaDOCCOBVaxxinova(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            List<Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBVaxxinovaDocumentos> documentosEDI = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBVaxxinovaDocumentos>();

            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(fatura.Codigo);
            foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento in documentos)
            {
                if (faturaDocumento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga)
                    continue;

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = faturaDocumento.Documento.CTe;
                decimal valorTotalNotas = cte.XMLNotaFiscais.Sum(o => o.Valor);

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cte.XMLNotaFiscais)
                {
                    decimal valorFrete = (cte.ValorAReceber / valorTotalNotas) * xmlNotaFiscal.Valor;

                    Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBVaxxinovaDocumentos eDIDOCCOB = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBVaxxinovaDocumentos()
                    {
                        Fatura = fatura.Numero,
                        DataEmissaoFatura = fatura.DataFatura,
                        DataVencimentoFatura = fatura.Parcelas.OrderBy(o => o.DataVencimento).First().DataVencimento,
                        ValorFatura = fatura.Total,

                        CTe = cte.Numero,
                        DataEmissaoCTe = cte.DataEmissao.Value,
                        ValorFrete = cte.ValorAReceber,
                        BaseICMSFrete = cte.BaseCalculoICMS,
                        ValorICMSFrete = cte.ValorICMS,

                        NotaFiscal = xmlNotaFiscal.Numero,
                        SerieNota = xmlNotaFiscal.Serie,
                        DataEmissaoNota = xmlNotaFiscal.DataEmissao,
                        Cliente = xmlNotaFiscal.Destinatario.Nome,
                        ValorNota = xmlNotaFiscal.Valor,
                        //AliquotaNota = não existe pra preencher,
                        ValorFreteNota = valorFrete,
                        TipoFrete = "N",
                        //TipoModal = fixo na configuração do edi
                        //Transportador = fixo na configuração do edi
                        CFOPNota = xmlNotaFiscal.CFOP
                    };

                    documentosEDI.Add(eDIDOCCOB);
                }
            }

            Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBVaxxinova doccob = new Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBVaxxinova()
            {
                Documentos = documentosEDI
            };

            return doccob;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.EDI.DOCCOB.ConhecimentoCobranca ObterConhecimentoCobranca(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, int numeroFatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unidadeTrabalho);
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTE = new Repositorio.ComponentePrestacaoCTE(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = null;

            if (cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
            {
                string chaveCTeAnterior = cte.DocumentosTransporteAnterior?.FirstOrDefault()?.Chave;

                if (!string.IsNullOrWhiteSpace(chaveCTeAnterior))
                    cteTerceiro = repCTeTerceiro.BuscarPorChave(chaveCTeAnterior);
            }
            decimal valorAdValorem = repComponentePrestacaoCTE.BuscarValorPorCTeTipo(cte.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM);

            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa();

            Dominio.ObjetosDeValor.EDI.DOCCOB.ConhecimentoCobranca conhecimentoCobranca = new Dominio.ObjetosDeValor.EDI.DOCCOB.ConhecimentoCobranca
            {
                FilialEmissora = cte.Empresa.RazaoSocial,
                FilialEmissoraCidade = cte.Empresa?.Localidade?.Descricao ?? "",
                FilialEmissoraUF = cte.Empresa?.Localidade?.Estado?.Sigla ?? "",
                FilialEmissoraNomeFantasia = cte.Empresa?.NomeFantasia ?? "",
                CodigoEstabelecimento = cte.Empresa.CodigoEstabelecimento,
                CodigoAlternativoTomador = cte.TomadorPagador?.Cliente?.ClienteTomadorFatura?.CodigoAlternativo ?? "",
                CodigoCentroCusto = cte.Empresa.CodigoCentroCusto,
                CNPJFilialEmissora = cte.Empresa.CNPJ_SemFormato,
                SerieConhecimento = cte.Serie.Numero.ToString(),
                NumeroConhecimento = cte.Numero.ToString(),
                Emitente = svcPessoa.ConverterObjetoEmpresa(cte.Empresa),
                Remetente = svcPessoa.ConverterObjetoParticipamenteCTe(cte.Remetente),
                Destinatario = svcPessoa.ConverterObjetoParticipamenteCTe(cte.Destinatario),
                ValorFrete = cte.ValorAReceber,
                DataEmissao = cte.DataEmissao.Value,
                BaseCalculoICMS = cte.BaseCalculoICMS,
                AliquotaICMS = cte.AliquotaICMS,
                ChaveCTe = cte.Chave,
                ChaveCTeReferenciaComplementado = cte.ChaveCTESubComp,
                Complemento = cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? "S" : "N",
                CondicaoFrete = cte.CondicaoPagamento,
                ModeloDocumentoFiscal = cte.ModeloDocumentoFiscal.Numero.ToString(),
                AbreviacaoModeloDocumentoFiscal = cte.ModeloDocumentoFiscal.Abreviacao.ToString(),
                NumeroFatura = numeroFatura,
                Peso = cte.QuantidadesCarga.Where(o => o.UnidadeMedida == "01").Sum(o => o.Quantidade),
                SituacaoDocumentoFiscal = "00",
                SubstituicaoTributaria = cte.CST == "60" ? "1" : "2",
                TipoCTe = cte.TipoCTE.ToString("D"),
                ValorICMS = cte.ValorICMS,
                ValorMercadoria = cte.ValorTotalMercadoria,
                PesoDensidadeCubagem = cteTerceiro?.CTesTerceiroNFes?.Sum(o => o.PesoCubado) ?? 0m,
                NumeroRomaneio = cteTerceiro?.NumeroRomaneio ?? cte.Documentos?.Select(o => o.NumeroRomaneio).FirstOrDefault(),
                NumeroPedido = cteTerceiro?.NumeroPedido ?? cte.Documentos?.Select(o => o.NumeroPedido).FirstOrDefault(),
                ProtocoloCliente = cteTerceiro?.ProtocoloCliente,
                NumeroOS = cte.NumeroOS,
                NumeroContainer = cte.Containers?.FirstOrDefault()?.Container?.Numero ?? "",
                TipoContainer = cte.Containers?.FirstOrDefault()?.Container?.ContainerTipo?.Descricao ?? "",
                ValorADValorem = valorAdValorem,
                ValorFreteSemICMS = cte.ValorAReceber,
                ValorADValoremComICMS = valorAdValorem,

                NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.DOCCOB.NotaFiscalCobrancaConhecimento>()
            };

            if (conhecimentoCobranca.AliquotaICMS > 0 && valorAdValorem > 0)
            {
                decimal percentualAliquota = 1 - (conhecimentoCobranca.AliquotaICMS / 100);
                if (percentualAliquota > 0)
                {
                    conhecimentoCobranca.ValorADValoremComICMS = Math.Round(valorAdValorem / percentualAliquota, 2, MidpointRounding.ToEven);
                    conhecimentoCobranca.ValorFreteSemICMS = conhecimentoCobranca.ValorFrete - conhecimentoCobranca.ValorADValoremComICMS;
                }
            }
            else if (valorAdValorem > 0)
            {
                conhecimentoCobranca.ValorADValoremComICMS = Math.Round(valorAdValorem, 2, MidpointRounding.ToEven);
                conhecimentoCobranca.ValorFreteSemICMS = conhecimentoCobranca.ValorFrete - conhecimentoCobranca.ValorADValoremComICMS;
            }

            for (var j = 0; j < cte.Documentos.Count; j++)
            {
                Dominio.Entidades.DocumentosCTE notaFiscal = cte.Documentos[j];

                int numeroNotaFiscal;
                int.TryParse(notaFiscal.Numero?.Trim(), out numeroNotaFiscal);

                Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe = cteTerceiro?.CTesTerceiroNFes?.Where(o => o.Chave == notaFiscal.ChaveNFE || (o.Numero == notaFiscal.Numero && o.Serie == notaFiscal.Serie)).FirstOrDefault();

                string cnpjEmissorNotaFiscal = notaFiscal.CNPJRemetente;

                if (string.IsNullOrWhiteSpace(cnpjEmissorNotaFiscal) && !string.IsNullOrWhiteSpace(notaFiscal.ChaveNFE) && notaFiscal.ChaveNFE.Length == 44)
                    cnpjEmissorNotaFiscal = notaFiscal.ChaveNFE.Substring(6, 14);

                Dominio.ObjetosDeValor.EDI.DOCCOB.NotaFiscalCobrancaConhecimento notaFiscalCobrancaConhecimento = new Dominio.ObjetosDeValor.EDI.DOCCOB.NotaFiscalCobrancaConhecimento
                {
                    CNPJEmissorNotaFiscal = cnpjEmissorNotaFiscal,
                    DataEmissao = notaFiscal.DataEmissao,
                    Numero = numeroNotaFiscal,
                    Peso = notaFiscal.Peso,
                    Serie = notaFiscal.SerieOuSerieDaChave,
                    ValorMercadoria = notaFiscal.Valor,
                    Destinatario = conhecimentoCobranca.Destinatario,
                    Chave = notaFiscal.ChaveNFE,
                    NumeroProtocolo = cteTerceiroNFe?.ProtocoloCliente,
                    Protocolo = cteTerceiroNFe?.Protocolo ?? string.Empty,
                    PesoDensidadeCubagem = cteTerceiroNFe?.PesoCubado ?? 0m,
                    NumeroRomaneio = cteTerceiroNFe?.NumeroRomaneio ?? string.Empty,
                    NumeroPedido = notaFiscal.NumeroPedido
                };

                conhecimentoCobranca.NotasFiscais.Add(notaFiscalCobrancaConhecimento);
            }

            return conhecimentoCobranca;
        }

        #endregion
    }
}
