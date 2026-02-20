using Dominio.Entidades;
using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class PREFAT
    {
        public static bool GerarPreFaturas(out string erro, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Dominio.Entidades.Empresa empresa, LayoutEDI layoutEDI, Stream inputStream, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unidadeTrabalho);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unidadeTrabalho);

            Servicos.LeituraEDI svcLeituraEDI = new Servicos.LeituraEDI(empresa, layoutEDI, inputStream, unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoIntercambio prefat = svcLeituraEDI.LerPREFAT();

            if (prefat.PreFaturas != null)
            {
                foreach (Dominio.ObjetosDeValor.EDI.PREFAT.PreFatura preFatura in prefat.PreFaturas)
                {
                    DateTime.TryParseExact(preFatura.DataInicialPreFatura, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicialFatura);
                    DateTime.TryParseExact(preFatura.DataFinalPreFatura, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinalFatura);
                    DateTime.TryParseExact(preFatura.DataPagamentoPreFatura, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimento);

                    long.TryParse(preFatura.NumeroPreFatura, out long numeroPreFatura);
                    double.TryParse(preFatura.CNPJResponsavelFrete, out double cnpjTomadorFatura);

                    if (repFatura.ExistePorPreFaturaEGrupoPessoas(grupoPessoas.Codigo, numeroPreFatura))
                        continue;

                    unidadeTrabalho.Start();

                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura
                    {
                        TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa,
                        TipoArredondamentoParcelas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Primeira,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento,
                        Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento,
                        NovoModelo = true,
                        DataFatura = DateTime.Now,
                        DataInicial = dataInicialFatura,
                        DataFinal = dataFinalFatura,
                        Empresa = repEmpresa.BuscarPorCNPJ(preFatura.CNPJTransportadora),
                        GrupoPessoas = grupoPessoas,
                        NumeroPreFatura = numeroPreFatura,
                        Usuario = usuario,
                        ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ(cnpjTomadorFatura),
                        NomeCliente = preFatura.NomeCliente,
                        TipoFrete = preFatura.TipoFrete,
                        ModalidadeFrete = preFatura.ModalidadeFrete,
                        CodigoDeposito = preFatura.CodigoDeposito,
                        CodigoTransportadora = preFatura.CodigoTransportadora,
                        ControleNumeracao = repFatura.BuscarProximoControleNumeracao()
                    };

                    repFatura.Inserir(fatura);

                    foreach (Dominio.ObjetosDeValor.EDI.PREFAT.DocumentoPreFatura documento in preFatura.Documentos)
                    {
                        int.TryParse(documento.NumeroConhecimentoTransportadora, out int numeroCTe);
                        int.TryParse(documento.SerieConhecimentoTransportadora, out int serieCTe);
                        string cnpjEmitente = Utilidades.String.Right(documento.CNPJEmpresaEmissoraDocumento, 14);

                        string[] chavesNotasFiscais = documento.NotasFiscais.Where(o => !string.IsNullOrWhiteSpace(o.ChaveNFe)).Select(o => o.ChaveNFe).ToArray();

                        List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

                        if (numeroCTe > 0 && serieCTe > 0 && !string.IsNullOrWhiteSpace(cnpjEmitente))
                            documentosFaturamento = repDocumentoFaturamento.BuscarPorEmpresaECTe(cnpjEmitente, numeroCTe, serieCTe, 0, "CT-e");
                        else if (chavesNotasFiscais.Any())
                            documentosFaturamento = repDocumentoFaturamento.BuscarPorChaveNFe(chavesNotasFiscais);

                        if (documentosFaturamento.Count == 0)
                        {
                            unidadeTrabalho.Rollback();

                            if (numeroCTe > 0 && serieCTe > 0 && !string.IsNullOrWhiteSpace(cnpjEmitente))
                                erro = "Não existe um CT-e emitido com o número " + numeroCTe + ", série " + serieCTe + " e CNPJ emitente " + cnpjEmitente + ".";
                            else
                                erro = "Não existe um CT-e emitido para as notas fiscais " + string.Join(", ", chavesNotasFiscais) + ".";

                            return false;
                        }

                        if (documentosFaturamento.Count > 1)
                        {
                            unidadeTrabalho.Rollback();

                            if (numeroCTe > 0 && serieCTe > 0 && !string.IsNullOrWhiteSpace(cnpjEmitente))
                                erro = "Existe mais de um CT-e emitido (" + string.Join(", ", documentosFaturamento.Select(o => o.CTe.Numero)) + ") com o número " + numeroCTe + ", série " + serieCTe + " e CNPJ emitente " + cnpjEmitente + ".";
                            else
                                erro = "Existe mais de um CT-e emitido (" + string.Join(", ", documentosFaturamento.Select(o => o.CTe.Numero)) + ") para as notas fiscais " + string.Join(", ", chavesNotasFiscais) + ".";

                            return false;
                        }

                        Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = documentosFaturamento.First();

                        if (documentoFaturamento.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                        {
                            string chaveCTeAnterior = documentoFaturamento.CTe.DocumentosTransporteAnterior?.FirstOrDefault()?.Chave;

                            if (!string.IsNullOrWhiteSpace(chaveCTeAnterior))
                            {
                                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repCTeTerceiro.BuscarPorChave(chaveCTeAnterior);

                                if (cteTerceiro != null)
                                {
                                    cteTerceiro.ProtocoloCliente = documento.NumeroProtocolo;

                                    repCTeTerceiro.Atualizar(cteTerceiro);

                                    foreach (Dominio.ObjetosDeValor.EDI.PREFAT.DocumentoNotaFiscalPreFatura notaFiscalPREFAT in documento.NotasFiscais)
                                    {
                                        Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe = cteTerceiro.CTesTerceiroNFes.Where(o => o.Chave == notaFiscalPREFAT.ChaveNFe).FirstOrDefault();

                                        if (cteTerceiroNFe != null)
                                        {
                                            cteTerceiroNFe.Protocolo = notaFiscalPREFAT.ProtocoloNFe;
                                            cteTerceiroNFe.ProtocoloCliente = notaFiscalPREFAT.NumeroProtocolo;

                                            repCTeTerceiroNFe.Atualizar(cteTerceiroNFe);
                                        }
                                    }
                                }
                            }
                        }

                        if (!Servicos.Embarcador.Fatura.Fatura.AdicionarDocumentoNaFatura(out erro, ref fatura, documentoFaturamento.Codigo, documentoFaturamento.ValorAFaturar, unidadeTrabalho))
                        {
                            unidadeTrabalho.Rollback();
                            return false;
                        }
                    }

                    SetarDadosGeraisFatura(fatura, dataVencimento, auditado, unidadeTrabalho);

                    unidadeTrabalho.CommitChanges();
                }
            }
            else if (prefat.CabecalhosDocumentos != null)
            {
                foreach (Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoDocumento cabecalhoDocumento in prefat.CabecalhosDocumentos)
                {
                    foreach (Dominio.ObjetosDeValor.EDI.PREFAT.EmpresaPagadora empresaPagadora in cabecalhoDocumento.EmpresasPagadoras)
                    {
                        double.TryParse(empresaPagadora.Pessoa.CPFCNPJ, out double cnpjTomadorFatura);

                        foreach (Dominio.ObjetosDeValor.EDI.PREFAT.PreFatura preFatura in empresaPagadora.PreFaturas)
                        {
                            //DateTime.TryParseExact(preFatura.DataInicialPreFatura, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicialFatura);
                            DateTime.TryParseExact(preFatura.DataEmissaoPreFatura, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoPreFatura);
                            if (dataEmissaoPreFatura == DateTime.MinValue)
                                DateTime.TryParseExact(preFatura.DataEmissaoPreFatura, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoPreFatura);
                            DateTime.TryParseExact(preFatura.DataPagamentoPreFatura, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimento);
                            if (dataVencimento == DateTime.MinValue)
                                DateTime.TryParseExact(preFatura.DataPagamentoPreFatura, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

                            string cnpjEmissorDocumentos = preFatura.Documentos.Select(o => o.CNPJEmpresaEmissoraDocumento).FirstOrDefault();

                            long.TryParse(preFatura.IdentificacaoPreFatura, out long numeroPreFatura);

                            if (repFatura.ExistePorPreFaturaEGrupoPessoas(grupoPessoas.Codigo, numeroPreFatura))
                                continue;

                            unidadeTrabalho.Start();

                            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura
                            {
                                TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa,
                                TipoArredondamentoParcelas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Primeira,
                                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento,
                                Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento,
                                NovoModelo = true,
                                DataFatura = DateTime.Now,
                                DataInicial = dataEmissaoPreFatura,
                                DataFinal = dataEmissaoPreFatura,
                                Empresa = repEmpresa.BuscarPorCNPJ(cnpjEmissorDocumentos),
                                GrupoPessoas = grupoPessoas,
                                NumeroPreFatura = numeroPreFatura,
                                Usuario = usuario,
                                ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ(cnpjTomadorFatura),
                                ControleNumeracao = repFatura.BuscarProximoControleNumeracao()
                            };

                            repFatura.Inserir(fatura);

                            foreach (Dominio.ObjetosDeValor.EDI.PREFAT.DocumentoPreFatura documento in preFatura.Documentos)
                            {
                                int.TryParse(documento.NumeroConhecimentoTransportadora, out int numeroCTe);
                                int.TryParse(documento.SerieConhecimentoTransportadora, out int serieCTe);
                                string cnpjEmitente = Utilidades.String.Right(documento.CNPJEmpresaEmissoraDocumento, 14);

                                string[] chavesNotasFiscais = documento.NotasFiscais != null ? documento.NotasFiscais.Where(o => !string.IsNullOrWhiteSpace(o.ChaveNFe)).Select(o => o.ChaveNFe).ToArray() : new string[] { };

                                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

                                if (layoutEDI.UtilizarInformacoesCTeOriginal)
                                {
                                    documentosFaturamento = repDocumentoFaturamento.BuscarPorCTeOriginal(numeroCTe, serieCTe);
                                }
                                else
                                {
                                    if (numeroCTe > 0 && serieCTe > 0 && !string.IsNullOrWhiteSpace(cnpjEmitente))
                                        documentosFaturamento = repDocumentoFaturamento.BuscarPorEmpresaECTe(cnpjEmitente, numeroCTe, serieCTe, 0, "CT-e");
                                    else if (chavesNotasFiscais.Any())
                                        documentosFaturamento = repDocumentoFaturamento.BuscarPorChaveNFe(chavesNotasFiscais);
                                }

                                if (documentosFaturamento.Count == 0)
                                {
                                    unidadeTrabalho.Rollback();

                                    if (numeroCTe > 0 && serieCTe > 0 && !string.IsNullOrWhiteSpace(cnpjEmitente))
                                        erro = "Não existe um CT-e emitido com o número " + numeroCTe + ", série " + serieCTe + " e CNPJ emitente " + cnpjEmitente + ".";
                                    else
                                        erro = "Não existe um CT-e emitido para as notas fiscais " + string.Join(", ", chavesNotasFiscais) + ".";

                                    return false;
                                }

                                if (documentosFaturamento.Count > 1)
                                {
                                    IEnumerable<int> numeros = documentosFaturamento.Select(o => o.CTe.Numero);

                                    unidadeTrabalho.Rollback();

                                    if (numeroCTe > 0 && serieCTe > 0 && !string.IsNullOrWhiteSpace(cnpjEmitente))
                                        erro = "Existe mais de um CT-e emitido (" + string.Join(", ", numeros) + ") com o número " + numeroCTe + ", série " + serieCTe + " e CNPJ emitente " + cnpjEmitente + ".";
                                    else
                                        erro = "Existe mais de um CT-e emitido (" + string.Join(", ", numeros) + ") para as notas fiscais " + string.Join(", ", chavesNotasFiscais) + ".";

                                    return false;
                                }

                                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = documentosFaturamento.First();

                                if (!Servicos.Embarcador.Fatura.Fatura.AdicionarDocumentoNaFatura(out erro, ref fatura, documentoFaturamento.Codigo, documentoFaturamento.ValorAFaturar, unidadeTrabalho))
                                {
                                    unidadeTrabalho.Rollback();
                                    return false;
                                }
                            }

                            DateTime? dataInicialFatura = repFaturaDocumento.BuscarDataEmissaoDocumento(fatura.Codigo, "asc");
                            DateTime? dataFinalFatura = repFaturaDocumento.BuscarDataEmissaoDocumento(fatura.Codigo, "desc");

                            if (dataInicialFatura.HasValue)
                                fatura.DataInicial = dataInicialFatura.Value.Date;

                            if (dataFinalFatura.HasValue)
                                fatura.DataFinal = dataFinalFatura.Value.Date;

                            SetarDadosGeraisFatura(fatura, dataVencimento, auditado, unidadeTrabalho);

                            unidadeTrabalho.CommitChanges();
                        }
                    }
                }
            }

            erro = "";
            return true;
        }

        public static void GerarPreFaturasXml(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Stream inputStream, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            XDocument documentoXml = XDocument.Load(inputStream);
            XElement bordero = documentoXml.Descendants("Bordero")?.FirstOrDefault();

            if (bordero == null)
                throw new ServicoException("Nenhum registro encontrado no xml selecionado para gerar a pré-fatura.");

            Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            long numeroPreFatura = bordero.Element("NroBordero")?.Value.Trim().ToLong() ?? 0;

            if (repositorioFatura.ExistePorPreFaturaEGrupoPessoas(grupoPessoas.Codigo, numeroPreFatura))
                throw new ServicoException($"Já existe uma fatura com o número de pré-fatura {numeroPreFatura}.");

            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(bordero.Element("CpfCnpj")?.Value.ObterSomenteNumeros() ?? "") ?? throw new ServicoException("Empresa não encontrada.");
            DateTime dataVencimento = bordero.Element("DataVencimento")?.Value.Trim().ToNullableDateTime("yyyy-MM-dd HH:mm:ss") ?? throw new ServicoException("Data de vencimento não encontrada.");

            IEnumerable<XElement> documentos = bordero.Descendants("DocumentosSubstituidos")?.Descendants("Documento");

            if ((documentos == null) || (documentos.Count() == 0))
                throw new ServicoException("Nenhum documento encontrado no xml selecionado para gerar a pré-fatura.");

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);

            foreach (XElement documento in documentos)
            {
                string chaveCTe = documento.Element("ChaveNfe")?.Value.Trim() ?? "";
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamentoPorNotaFiscal = repositorioDocumentoFaturamento.BuscarPorChaveCTe(chaveCTe);

                if (documentosFaturamentoPorNotaFiscal.Count == 0)
                    throw new ServicoException($"Não existe um CT-e emitido para a chave de acesso {chaveCTe}.");

                if (documentosFaturamentoPorNotaFiscal.Count > 1)
                    throw new ServicoException($"Existe mais de um CT-e emitido ({string.Join(", ", documentosFaturamentoPorNotaFiscal.Select(o => o.CTe.Numero))}) com a chave de acesso {chaveCTe}.");

                documentosFaturamento.Add(documentosFaturamentoPorNotaFiscal.First());
            }

            try
            {
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura
                {
                    TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa,
                    TipoArredondamentoParcelas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Primeira,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento,
                    Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento,
                    NovoModelo = true,
                    DataFatura = DateTime.Now,
                    DataInicial = documentosFaturamento.Min(o => o.DataEmissao),
                    DataFinal = documentosFaturamento.Max(o => o.DataEmissao),
                    Empresa = empresa,
                    GrupoPessoas = grupoPessoas,
                    NumeroPreFatura = numeroPreFatura,
                    Usuario = usuario,
                    ClienteTomadorFatura = documentosFaturamento.FirstOrDefault().Tomador,
                    ControleNumeracao = repFatura.BuscarProximoControleNumeracao()
                };

                repositorioFatura.Inserir(fatura);

                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                {
                    if (!Fatura.Fatura.AdicionarDocumentoNaFatura(out string mensagemErroAdicionarDocumentoFatura, ref fatura, documentoFaturamento.Codigo, documentoFaturamento.ValorAFaturar, unidadeTrabalho))
                        throw new ServicoException(mensagemErroAdicionarDocumentoFatura);
                }

                SetarDadosGeraisFatura(fatura, dataVencimento, auditado, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();
            }
            catch (Exception)
            {
                unidadeTrabalho.Rollback();
                throw;
            }
        }

        private static void SetarDadosGeraisFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, DateTime dataVencimento, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unidadeTrabalho);

            Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);
            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unidadeTrabalho);
            
            if (fatura.Empresa == null)
                fatura.Empresa = repFaturaDocumento.ObterPrimeiraEmpresaEmissora(fatura.Codigo);

            if (fatura.Cliente != null && !fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                SetarDadosBancariosFatura(fatura, fatura.Cliente);
            else if (fatura.Cliente?.GrupoPessoas != null)
                SetarDadosBancariosFatura(fatura, fatura.Cliente.GrupoPessoas);
            else if (fatura.GrupoPessoas != null)
                SetarDadosBancariosFatura(fatura, fatura.GrupoPessoas);

            if (fatura.ClienteTomadorFatura == null)
                fatura.ClienteTomadorFatura = repFaturaDocumento.ObterPrimeiroTomador(fatura.Codigo);

            fatura.Numero = repFatura.UltimoNumeracao() + 1;
            fatura.ControleNumeracao = null;

            repFatura.Atualizar(fatura);

            Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = new Dominio.Entidades.Embarcador.Fatura.FaturaParcela
            {
                DataEmissao = fatura.DataFatura,
                DataVencimento = dataVencimento,
                Desconto = 0m,
                Fatura = fatura,
                Sequencia = 1,
                SituacaoFaturaParcela = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto,
                Valor = fatura.Total
            };

            repFaturaParcela.Inserir(parcela);

            servFatura.AtualizarValorVencimento(parcela.DataVencimento, fatura.Codigo, unidadeTrabalho);

            Servicos.Auditoria.Auditoria.Auditar(auditado, fatura, null, "Adicionado à partir de uma pré-fatura.", unidadeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert);
        }

        private static void SetarDadosBancariosFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Cliente cliente)
        {
            fatura.Banco = cliente.Banco;
            fatura.Agencia = cliente.Agencia;
            fatura.DigitoAgencia = cliente.DigitoAgencia;
            fatura.NumeroConta = cliente.NumeroConta;
            fatura.TipoContaBanco = cliente.TipoContaBanco;
            fatura.ClienteTomadorFatura = cliente.ClienteTomadorFatura;
        }

        private static void SetarDadosBancariosFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas)
        {
            fatura.Banco = grupoPessoas.Banco;
            fatura.Agencia = grupoPessoas.Agencia;
            fatura.DigitoAgencia = grupoPessoas.DigitoAgencia;
            fatura.NumeroConta = grupoPessoas.NumeroConta;
            fatura.TipoContaBanco = grupoPessoas.TipoContaBanco;
            fatura.ClienteTomadorFatura = grupoPessoas.ClienteTomadorFatura;
        }

        public Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoIntercambio ConverterFaturaParaPREFAT(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.EDI.PREFAT.DocumentoPreFatura> documentosPreFatura = new List<Dominio.ObjetosDeValor.EDI.PREFAT.DocumentoPreFatura>();

            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(fatura.Codigo);
            foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento in documentos)
            {
                if (faturaDocumento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga)
                    continue;

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = faturaDocumento.Documento.CTe;

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cte.XMLNotaFiscais)
                {
                    Dominio.ObjetosDeValor.EDI.PREFAT.ValorEmbarcador valorEmbarcador = new Dominio.ObjetosDeValor.EDI.PREFAT.ValorEmbarcador()
                    {
                        PercentualTaxaISS = cte.AliquotaISS,
                        ValorISS = cte.ValorISS,
                        BaseCalculoICMS = cte.BaseCalculoICMS,
                        PercentualTaxaICMS = cte.AliquotaICMS,
                        ValorICMS = cte.ValorICMS,
                    };

                    Dominio.ObjetosDeValor.EDI.PREFAT.DocumentoPreFatura documentoPreFatura = new Dominio.ObjetosDeValor.EDI.PREFAT.DocumentoPreFatura()
                    {
                        CNPJEmpresaEmissoraDocumento = cte.Empresa.CNPJ_SemFormato,
                        SerieDocumentoEmbarcador = xmlNotaFiscal.Serie.ToString(),
                        IdentificacaoDocumentoEmbarcador = xmlNotaFiscal.Numero.ToString(),
                        DataEmissaoDocumentoEmbarcador = cte.DataEmissao?.ToString("ddMMyyyy"),
                        SerieConhecimentoTransportadora = cte.Serie.Descricao.ToString(),
                        NumeroConhecimentoTransportadora = cte.Numero.ToString(),
                        DataEmissaoConhecimento = cte.DataEmissao?.ToString("ddMMyyyy"),
                        CPFCNPJLocalOrigemTransporte = cte.Remetente.CPF_CNPJ_SemFormato,
                        CPFCNPJLocalDestinoTransporte = cte.Destinatario.CPF_CNPJ_SemFormato,
                        ValorTotalFreteEmbarcador = cte.ValorAReceber,
                        ValorTotalFreteConhecimento = cte.ValorFrete,
                        ValorCalculadoEmbarcador = valorEmbarcador
                    };

                    documentosPreFatura.Add(documentoPreFatura);
                }
            }

            List<Dominio.ObjetosDeValor.EDI.PREFAT.PreFatura> preFaturas = new List<Dominio.ObjetosDeValor.EDI.PREFAT.PreFatura>();

            Dominio.ObjetosDeValor.EDI.PREFAT.PreFatura preFatura = new Dominio.ObjetosDeValor.EDI.PREFAT.PreFatura()
            {
                IdentificacaoPreFatura = fatura.NumeroPreFatura.ToString(),
                DataEmissaoPreFatura = fatura.DataFatura.ToString("ddMMyyyy"),
                DataPagamentoPreFatura = fatura.DataFechamento?.ToString("ddMMyyyy"),
                QuantidadeDocumentosPreFatura = documentosPreFatura.Count(),
                ValorTotalPreFatura = fatura.Total,
                Documentos = documentosPreFatura,
            };

            preFaturas.Add(preFatura);

            List<Dominio.ObjetosDeValor.EDI.PREFAT.EmpresaPagadora> empresasPagadoras = new List<Dominio.ObjetosDeValor.EDI.PREFAT.EmpresaPagadora>();

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
            {
                CPFCNPJ = fatura.Tomador?.CPF_CNPJ_SemFormato,
                RazaoSocial = fatura.Tomador?.Nome
            };

            Dominio.ObjetosDeValor.EDI.PREFAT.EmpresaPagadora empresasPagadora = new Dominio.ObjetosDeValor.EDI.PREFAT.EmpresaPagadora()
            {
                Pessoa = pessoa,
                PreFaturas = preFaturas
            };

            empresasPagadoras.Add(empresasPagadora);

            List<Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoDocumento> cabecalhoDocumentos = new List<Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoDocumento>();

            Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoDocumento cabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoDocumento()
            {
                IdentificacaoDocumento = "PRE" + DateTime.Now.ToString("ddMMHHmm") + "1",
                EmpresasPagadoras = empresasPagadoras
            };

            cabecalhoDocumentos.Add(cabecalhoDocumento);

            Dominio.ObjetosDeValor.EDI.PREFAT.Total total = new Dominio.ObjetosDeValor.EDI.PREFAT.Total()
            {
                ValorTotalPreFaturas = fatura.Total,
                QuantidadeTotalPreFaturas = 1
            };

            Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoIntercambio cabecalhoIntercambio = new Dominio.ObjetosDeValor.EDI.PREFAT.CabecalhoIntercambio()
            {
                IdentificacaoRemetente = fatura.CTe?.Remetente.Nome,
                IdentificacaoDestinatario = fatura.CTe?.Destinatario.Nome,
                Data = DateTime.Now.ToString("ddMMyy"),
                Hora = DateTime.Now.ToString("HHmm"),
                IdentificacaoIntercambio = "PRE" + DateTime.Now.ToString("ddMMHHmm") + "1",
                CabecalhosDocumentos = cabecalhoDocumentos,
                Total = total
            };

            return cabecalhoIntercambio;
        }
    }
}
