using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public class DocumentoProvisao
    {
        //public static void AdicionarDocumentosParaFechamento(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        //{
        //    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || fechamento.Contrato.Transportador.ProvisionarDocumentos)
        //    {
        //        Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
        //        Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
        //        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

        //        for (int i = 0; i < cargaCTeComplementoInfos.Count; i++)
        //        {
        //            Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = cargaCTeComplementoInfos[i];
        //            bool provisionar = false;

        //            if (cargaCTeComplementoInfo.TomadorPagador != null && cargaCTeComplementoInfo.TomadorPagador.GrupoPessoas != null)
        //                provisionar = cargaCTeComplementoInfo.TomadorPagador.GrupoPessoas.ProvisionarDocumentos;

        //            if (!provisionar && cargaCTeComplementoInfo.TomadorPagador != null)
        //                provisionar = cargaCTeComplementoInfo.TomadorPagador.ProvisionarDocumentos;

        //            if (!provisionar)
        //                provisionar = fechamento.Contrato.Transportador.ProvisionarDocumentos;

        //            if (provisionar && cargaCTeComplementoInfo.TomadorPagador != null && cargaCTeComplementoInfo.TomadorPagador.DataViradaProvisao.HasValue && cargaCTeComplementoInfo.TomadorPagador.DataViradaProvisao.Value >= fechamento.DataFechamento)
        //                provisionar = false;

        //            if (provisionar && cargaCTeComplementoInfo.CargaCTeComplementado != null)
        //            {
        //                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = cargaCTeComplementoInfo.CargaCTeComplementado.CTe;

        //                decimal valorProvisao = 0;
        //                decimal baseCalculoICMS = 0;
        //                decimal valorICMS = 0;
        //                decimal valorISS = 0;
        //                decimal baseCalculoISS = 0;
        //                decimal valorRetencaoISS = 0;

        //                int quantidade = cteComplementado.XMLNotaFiscais.Count;
        //                for (int j = 0; j < cteComplementado.XMLNotaFiscais.Count; j++)
        //                {
        //                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ultimoXMLNotaFiscal = cteComplementado.XMLNotaFiscais.LastOrDefault();
        //                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = cteComplementado.XMLNotaFiscais.ToList()[j];

        //                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repDocumentoProvisao.BuscarPorXMLNotaFiscalEFechamento(xmlNotaFiscal.Codigo, fechamento.Codigo);
        //                    bool inserir = false;
        //                    if (documentoProvisao == null)
        //                    {
        //                        documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao()
        //                        {
        //                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
        //                        };

        //                        inserir = true;
        //                        if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.Chave))
        //                            documentoProvisao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("55");
        //                        else
        //                            documentoProvisao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("99");
        //                    }

        //                    if (documentoProvisao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao)
        //                    {
        //                        int.TryParse(xmlNotaFiscal.Serie, out int serie);
        //                        documentoProvisao.XMLNotaFiscal = xmlNotaFiscal;
        //                        documentoProvisao.Filial = cargaCTeComplementoInfo.CargaComplementoFrete?.Carga?.Filial;
        //                        documentoProvisao.Empresa = fechamento.Contrato.Transportador;
        //                        documentoProvisao.TipoOperacao = cargaCTeComplementoInfo.CargaComplementoFrete?.Carga?.TipoOperacao;
        //                        documentoProvisao.Tomador = cargaCTeComplementoInfo.TomadorPagador;
        //                        documentoProvisao.NumeroDocumento = xmlNotaFiscal.Numero;
        //                        documentoProvisao.SerieDocumento = serie;
        //                        documentoProvisao.DataEmissao = xmlNotaFiscal.DataEmissao;
        //                        documentoProvisao.Remetente = xmlNotaFiscal.Emitente;
        //                        documentoProvisao.Destinatario = xmlNotaFiscal.Destinatario;
        //                        documentoProvisao.PesoBruto = xmlNotaFiscal.Peso;

        //                        if (cargaCTeComplementoInfo.CargaComplementoFrete?.Carga != null)
        //                        {
        //                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCargaOrigemEXMLNotaFiscal(cargaCTeComplementoInfo.CargaComplementoFrete.Carga.Codigo, xmlNotaFiscal.Codigo);
        //                            documentoProvisao.Origem = pedidoXMLNotaFiscal.CargaPedido.Origem;
        //                            documentoProvisao.Destino = pedidoXMLNotaFiscal.CargaPedido.Destino;
        //                        }
        //                        else
        //                        {
        //                            documentoProvisao.Origem = xmlNotaFiscal.Emitente.Localidade;
        //                            documentoProvisao.Destino = xmlNotaFiscal.Destinatario.Localidade;
        //                        }
        //                    }

        //                    SetarValoresDocumentoProvisaoOcorrecia(cargaCTeComplementoInfo, ref documentoProvisao, ref valorProvisao, ref baseCalculoICMS, ref valorICMS, ref valorISS, ref baseCalculoISS, ref valorRetencaoISS, quantidade, xmlNotaFiscal.Equals(ultimoXMLNotaFiscal));

        //                    if (inserir)
        //                        repDocumentoProvisao.Inserir(documentoProvisao);
        //                    else
        //                        repDocumentoProvisao.Atualizar(documentoProvisao);
        //                }
        //            }
        //        }
        //    }
        //}

        public static void AdicionarDocumentosParaFechamento(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (fechamento.Contrato.Transportador?.ProvisionarDocumentos ?? false))
            {
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                for (int i = 0; i < cargaCTeComplementoInfos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = cargaCTeComplementoInfos[i];
                    bool provisionar = false;

                    if (cargaCTeComplementoInfo.TomadorPagador != null && cargaCTeComplementoInfo.TomadorPagador.GrupoPessoas != null)
                        provisionar = cargaCTeComplementoInfo.TomadorPagador.GrupoPessoas.ProvisionarDocumentos;

                    if (!provisionar && cargaCTeComplementoInfo.TomadorPagador != null)
                        provisionar = cargaCTeComplementoInfo.TomadorPagador.ProvisionarDocumentos;

                    if (!provisionar)
                        provisionar = fechamento.Contrato.Transportador.ProvisionarDocumentos;

                    if (provisionar && cargaCTeComplementoInfo.TomadorPagador != null && cargaCTeComplementoInfo.TomadorPagador.DataViradaProvisao.HasValue && cargaCTeComplementoInfo.TomadorPagador.DataViradaProvisao.Value >= fechamento.DataFechamento)
                        provisionar = false;

                    if (provisionar && cargaCTeComplementoInfo.CTe != null)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = cargaCTeComplementoInfo.CTe;

                        Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repDocumentoProvisao.BuscarPorCTeEFechamento(cteComplementado.Codigo, fechamento.Codigo);
                        bool inserir = false;
                        if (documentoProvisao == null)
                        {
                            documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao
                            {
                                ModeloDocumentoFiscal = cteComplementado.ModeloDocumentoFiscal,
                                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                            };
                            inserir = true;
                        }

                        if (documentoProvisao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao)
                        {
                            documentoProvisao.Filial = cargaCTeComplementoInfo.CargaCTeComplementado.Carga?.Filial;
                            documentoProvisao.Empresa = fechamento.Contrato.Transportador;
                            documentoProvisao.TipoOperacao = cargaCTeComplementoInfo.CargaCTeComplementado.Carga?.TipoOperacao;
                            documentoProvisao.Tomador = cargaCTeComplementoInfo.TomadorPagador;
                            documentoProvisao.CentroResultado = cargaCTeComplementoInfo.CentroResultado;
                            documentoProvisao.NumeroDocumento = cteComplementado.Numero;
                            documentoProvisao.SerieDocumento = cteComplementado.Serie.Numero;
                            documentoProvisao.DataEmissao = cteComplementado.DataEmissao.HasValue ? cteComplementado.DataEmissao.Value : DateTime.Now;
                            documentoProvisao.CTe = cteComplementado;
                            documentoProvisao.Remetente = cteComplementado.Remetente.Cliente;
                            documentoProvisao.Destinatario = cteComplementado.Destinatario.Cliente;
                            documentoProvisao.Expedidor = cteComplementado.Expedidor?.Cliente;
                            documentoProvisao.Recebedor = cteComplementado.Recebedor?.Cliente;
                            documentoProvisao.PesoBruto = cteComplementado.Peso;
                            documentoProvisao.Origem = cteComplementado.LocalidadeInicioPrestacao;
                            documentoProvisao.FechamentoFrete = fechamento;
                            documentoProvisao.Destino = cteComplementado.LocalidadeTerminoPrestacao;
                        }

                        documentoProvisao.ValorProvisaoCompetencia = cargaCTeComplementoInfo.ValorComplemento;
                        documentoProvisao.BaseCalculoICMSCompetencia = cteComplementado.BaseCalculoICMS;
                        documentoProvisao.PercentualAliquotaCompetencia = cteComplementado.AliquotaICMS;
                        documentoProvisao.ValorICMSCompetencia = cteComplementado.ValorICMS;
                        documentoProvisao.CSTCompetencia = cteComplementado.CST;
                        documentoProvisao.ValorISSCompetencia = cteComplementado.ValorISS;
                        documentoProvisao.BaseCalculoISSCompetencia = cteComplementado.BaseCalculoISS;
                        documentoProvisao.PercentualAliquotaISSCompetencia = cteComplementado.AliquotaISS;
                        documentoProvisao.ValorRetencaoISSCompetencia = cteComplementado.ValorISSRetido;
                        documentoProvisao.ICMSInclusoBCCompetencia = cteComplementado.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
                        documentoProvisao.ISSInclusoBCCompetencia = cteComplementado.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;

                        GravarLogImpostosProvisao(documentoProvisao, $"cteComplementado = {cteComplementado?.Codigo ?? 0}");

                        if (documentoProvisao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao)
                        {
                            documentoProvisao.ValorProvisao = documentoProvisao.ValorProvisaoCompetencia;
                            documentoProvisao.BaseCalculoICMS = documentoProvisao.BaseCalculoICMSCompetencia;
                            documentoProvisao.PercentualAliquota = documentoProvisao.PercentualAliquotaCompetencia;
                            documentoProvisao.ValorICMS = documentoProvisao.ValorICMSCompetencia;
                            documentoProvisao.CST = documentoProvisao.CSTCompetencia;
                            documentoProvisao.ValorISS = documentoProvisao.ValorISSCompetencia;
                            documentoProvisao.BaseCalculoISS = documentoProvisao.BaseCalculoISSCompetencia;
                            documentoProvisao.ICMSInclusoBC = documentoProvisao.ICMSInclusoBCCompetencia;
                            documentoProvisao.ISSInclusoBC = documentoProvisao.ISSInclusoBCCompetencia;
                            documentoProvisao.PercentualAliquotaISS = documentoProvisao.PercentualAliquotaISSCompetencia;
                            documentoProvisao.ValorRetencaoISS = documentoProvisao.ValorRetencaoISSCompetencia;
                        }

                        documentoProvisao.OutrasAliquotas = cteComplementado.OutrasAliquotas;
                        documentoProvisao.CSTIBSCBS = cteComplementado.CSTIBSCBS;
                        documentoProvisao.ClassificacaoTributariaIBSCBS = cteComplementado.ClassificacaoTributariaIBSCBS;
                        documentoProvisao.BaseCalculoIBSCBS = cteComplementado.BaseCalculoIBSCBS;
                        documentoProvisao.AliquotaIBSEstadual = cteComplementado.AliquotaIBSEstadual;
                        documentoProvisao.PercentualReducaoIBSEstadual = cteComplementado.PercentualReducaoIBSEstadual;
                        documentoProvisao.ValorIBSEstadual = cteComplementado.ValorIBSEstadual;
                        documentoProvisao.AliquotaIBSMunicipal = cteComplementado.AliquotaIBSMunicipal;
                        documentoProvisao.PercentualReducaoIBSMunicipal = cteComplementado.PercentualReducaoIBSMunicipal;
                        documentoProvisao.ValorIBSMunicipal = cteComplementado.ValorIBSMunicipal;
                        documentoProvisao.AliquotaCBS = cteComplementado.AliquotaCBS;
                        documentoProvisao.PercentualReducaoCBS = cteComplementado.PercentualReducaoCBS;
                        documentoProvisao.ValorCBS = cteComplementado.ValorCBS;

                        if (inserir)
                            repDocumentoProvisao.Inserir(documentoProvisao);
                        else
                            repDocumentoProvisao.Atualizar(documentoProvisao);
                    }

                }
            }
        }

        public static void AdicionarDocumentosParaProvisaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (cargaOcorrencia.Carga?.Empresa?.ProvisionarDocumentos ?? false))
            {
                Servicos.Log.GravarInfo($"Gerando ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");

                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo, false);
                repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorOcorrencia(cargaOcorrencia.Codigo);
                for (int i = 0; i < cargaCTeComplementoInfos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = cargaCTeComplementoInfos[i];
                    bool provisionar = false;

                    if (cargaCTeComplementoInfo.TomadorPagador != null && cargaCTeComplementoInfo.TomadorPagador.GrupoPessoas != null)
                        provisionar = cargaCTeComplementoInfo.TomadorPagador.GrupoPessoas.ProvisionarDocumentos;

                    if (!provisionar && cargaCTeComplementoInfo.TomadorPagador != null)
                        provisionar = cargaCTeComplementoInfo.TomadorPagador.ProvisionarDocumentos;

                    if (!provisionar)
                        provisionar = cargaOcorrencia.Carga?.Empresa?.ProvisionarDocumentos ?? false;

                    if (provisionar && cargaCTeComplementoInfo.TomadorPagador != null && cargaCTeComplementoInfo.TomadorPagador.DataViradaProvisao.HasValue && cargaCTeComplementoInfo.TomadorPagador.DataViradaProvisao.Value >= cargaOcorrencia.DataOcorrencia)
                        provisionar = false;

                    decimal valorProvisao = 0;
                    decimal baseCalculoICMS = 0;
                    decimal valorICMS = 0;
                    decimal valorISS = 0;
                    decimal baseCalculoISS = 0;
                    decimal valorRetencaoISS = 0;

                    if (provisionar && cargaCTeComplementoInfo.CargaCTeComplementado != null)
                    {
                        Servicos.Log.GravarInfo($"Caindo no comando Provisar && CargaCTeComplementado != null, ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");

                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = cargaCTeComplementoInfo.CargaCTeComplementado.CTe;

                        if (cteComplementado.DocumentosTransporteAnterior.Count > 0 && !cargaCTeComplementoInfo.ProvisaoPelaNotaFiscal)
                        {
                            int quantidade = cteComplementado.DocumentosTransporteAnterior.Count;
                            for (int j = 0; j < cteComplementado.DocumentosTransporteAnterior.Count; j++)
                            {
                                Dominio.Entidades.DocumentoDeTransporteAnteriorCTe ultimoDocumentosCTE = cteComplementado.DocumentosTransporteAnterior.LastOrDefault();
                                Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentosCTE = cteComplementado.DocumentosTransporteAnterior[j];
                                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repCTeTerceiro.BuscarPorChave(documentosCTE.Chave);
                                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                                if (cargaOcorrencia.Carga != null)
                                    pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCargaOrigemECTeTerceiro(cargaOcorrencia.Carga.Codigo, cteTerceiro.Codigo);

                                GerarProvisaoPorComplementoCTeSubContratacao(cargaOcorrencia, cargaCTeComplementoInfo, cteTerceiro, pedidoCTeParaSubContratacao, ref valorProvisao, ref baseCalculoICMS, ref valorICMS, ref valorISS, ref baseCalculoISS, ref valorRetencaoISS, quantidade, documentosCTE.Equals(ultimoDocumentosCTE), tipoServicoMultisoftware, unitOfWork, configuracao);
                            }
                        }
                        else if (cteComplementado.XMLNotaFiscais.Count > 0)
                        {
                            Servicos.Log.GravarInfo($"Caindo no comando XMLNotaFiscais.Count > 0, ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");

                            int quantidade = cteComplementado.XMLNotaFiscais.Count;
                            for (int j = 0; j < cteComplementado.XMLNotaFiscais.Count; j++)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ultimoXMLNotaFiscal = cteComplementado.XMLNotaFiscais.LastOrDefault();
                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = cteComplementado.XMLNotaFiscais.ToList()[j];

                                GerarProvisaoPorComplementoNotaFiscal(cargaOcorrencia, cargaCTeComplementoInfo, xmlNotaFiscal, ref valorProvisao, ref baseCalculoICMS, ref valorICMS, ref valorISS, ref baseCalculoISS, ref valorRetencaoISS, quantidade, xmlNotaFiscal.Equals(ultimoXMLNotaFiscal), tipoServicoMultisoftware, unitOfWork, configuracao);
                            }
                        }
                    }
                    else if (provisionar && cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null)
                    {
                        Servicos.Log.GravarInfo($"Caindo no comando provisionar && cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null, ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");

                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado.PedidoCTeParaSubContratacao;
                        if (pedidoCTeParaSubContratacao != null && !cargaCTeComplementoInfo.ProvisaoPelaNotaFiscal)
                        {
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = pedidoCTeParaSubContratacao.CTeTerceiro;
                            GerarProvisaoPorComplementoCTeSubContratacao(cargaOcorrencia, cargaCTeComplementoInfo, cteTerceiro, pedidoCTeParaSubContratacao, ref valorProvisao, ref baseCalculoICMS, ref valorICMS, ref valorISS, ref baseCalculoISS, ref valorRetencaoISS, 1, true, tipoServicoMultisoftware, unitOfWork, configuracao);
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal;
                            GerarProvisaoPorComplementoNotaFiscal(cargaOcorrencia, cargaCTeComplementoInfo, xmlNotaFiscal, ref valorProvisao, ref baseCalculoICMS, ref valorICMS, ref valorISS, ref baseCalculoISS, ref valorRetencaoISS, 1, true, tipoServicoMultisoftware, unitOfWork, configuracao);
                        }
                    }
                }
            }
        }

        private static void GerarProvisaoPorComplementoCTeSubContratacao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, ref decimal valorProvisao, ref decimal baseCalculoICMS, ref decimal valorICMS, ref decimal valorISS, ref decimal baseCalculoISS, ref decimal valorRetencaoISS, int quantidade, bool ultimoRegistro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (cteTerceiro == null)
                return;

            Dominio.Entidades.Embarcador.Pedidos.Stage stage = null;

            if (pedidoCTeParaSubContratacao?.CargaPedido != null)
            {
                if (cargaOcorrencia.Carga?.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                    return;

                stage = pedidoCTeParaSubContratacao.CargaPedido.StageRelevanteCusto;

                if ((stage == null) && (cargaOcorrencia.Carga?.TipoDocumentoTransporte != null))
                    return;
            }

            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repositorioDocumentoProvisao.BuscarPorCTeAnteriorEOcorrencia(cteTerceiro.Codigo, cargaOcorrencia.Codigo);
            bool inserir = false;

            if (documentoProvisao == null)
            {
                Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao
                {
                    ModeloDocumentoFiscal = repositorioModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe),
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                };

                inserir = true;
            }

            if (documentoProvisao.Situacao == SituacaoProvisaoDocumento.AgProvisao)
            {
                int.TryParse(cteTerceiro.Serie, out int serie);
                documentoProvisao.Filial = cargaOcorrencia.Carga?.Filial;
                documentoProvisao.CTeTerceiro = cteTerceiro;
                documentoProvisao.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                documentoProvisao.Empresa = cargaOcorrencia.Carga?.Empresa;
                documentoProvisao.TipoOperacao = cargaOcorrencia.Carga?.TipoOperacao;
                documentoProvisao.Tomador = cargaCTeComplementoInfo.TomadorPagador;
                documentoProvisao.NumeroDocumento = cteTerceiro.Numero;
                documentoProvisao.SerieDocumento = serie;
                documentoProvisao.DataEmissao = cteTerceiro.DataEmissao;
                documentoProvisao.Carga = cargaOcorrencia.Carga;
                documentoProvisao.CargaOcorrencia = cargaOcorrencia;
                documentoProvisao.Remetente = cteTerceiro.Remetente.Cliente;
                documentoProvisao.Expedidor = cteTerceiro.Expedidor?.Cliente;
                documentoProvisao.Recebedor = cteTerceiro.Recebedor?.Cliente;
                documentoProvisao.Destinatario = cteTerceiro.Destinatario.Cliente;
                documentoProvisao.PesoBruto = cteTerceiro.CTeTerceiroQuantidades.Where(obj => obj.Unidade == Dominio.Enumeradores.UnidadeMedida.KG).Sum(obj => obj.Quantidade);
            }

            SetarValoresDocumentoProvisaoOcorrecia(cargaCTeComplementoInfo, ref documentoProvisao, ref valorProvisao, ref baseCalculoICMS, ref valorICMS, ref valorISS, ref baseCalculoISS, ref valorRetencaoISS, quantidade, ultimoRegistro);

            if (documentoProvisao.Situacao == SituacaoProvisaoDocumento.AgProvisao)
            {
                if (cargaOcorrencia.Carga != null && pedidoCTeParaSubContratacao != null)
                {
                    new Servicos.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork).DefinirImpostoValorAgregado(pedidoCTeParaSubContratacao.CargaPedido);

                    documentoProvisao.Origem = pedidoCTeParaSubContratacao.CargaPedido.Origem;
                    documentoProvisao.Destino = pedidoCTeParaSubContratacao.CargaPedido.Destino;
                    documentoProvisao.ImpostoValorAgregado = pedidoCTeParaSubContratacao.CargaPedido.ImpostoValorAgregado;
                    documentoProvisao.Stage = stage;
                }
                else
                {
                    documentoProvisao.Origem = cteTerceiro.LocalidadeInicioPrestacao;
                    documentoProvisao.Destino = cteTerceiro.LocalidadeTerminoPrestacao;
                }
            }

            if (inserir)
                repositorioDocumentoProvisao.Inserir(documentoProvisao);
            else
                repositorioDocumentoProvisao.Atualizar(documentoProvisao);
        }

        private static void GerarProvisaoPorComplementoNotaFiscal(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, ref decimal valorProvisao, ref decimal baseCalculoICMS, ref decimal valorICMS, ref decimal valorISS, ref decimal baseCalculoISS, ref decimal valorRetencaoISS, int quantidade, bool ultimoRegistro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Log.GravarInfo($"Caindo em GerarProvisaoPorComplementoNotaFiscal, ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = (cargaOcorrencia.Carga != null) ? repositorioPedidoXMLNotaFiscal.BuscarPorCargaOrigemEXMLNotaFiscal(cargaOcorrencia.Carga.Codigo, xmlNotaFiscal.Codigo) : null;
            Dominio.Entidades.Embarcador.Pedidos.Stage stage = null;

            Servicos.Log.GravarInfo($"pedidoXMLNotaFiscal {pedidoXMLNotaFiscal?.Codigo ?? 0}, ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");

            if (pedidoXMLNotaFiscal?.CargaPedido != null)
            {
                if (cargaOcorrencia.Carga?.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                    return;

                stage = pedidoXMLNotaFiscal.CargaPedido.StageRelevanteCusto;

                if ((stage == null) && (cargaOcorrencia.Carga?.TipoDocumentoTransporte != null))
                    return;
            }

            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repositorioDocumentoProvisao.BuscarPorXMLNotaFiscalEOcorrencia(xmlNotaFiscal.Codigo, cargaOcorrencia.Codigo);
            bool inserir = false;


            if (documentoProvisao == null)
            {
                Servicos.Log.GravarInfo($"Novo documento de provisão, ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");
                Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

                documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao()
                {
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                };

                inserir = true;

                if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.Chave))
                    documentoProvisao.ModeloDocumentoFiscal = repositorioModeloDocumentoFiscal.BuscarPorModelo("55");
                else
                    documentoProvisao.ModeloDocumentoFiscal = repositorioModeloDocumentoFiscal.BuscarPorModelo("99");
            }
            else
            {
                Servicos.Log.GravarInfo($"Documento de provisão existente {documentoProvisao.Codigo}, ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");
            }

            if (documentoProvisao.Situacao == SituacaoProvisaoDocumento.AgProvisao)
            {
                Servicos.Log.GravarInfo($"Documento de provisão aguardando provisão, ocorrência: {cargaOcorrencia.Codigo}", "LogDocumentoProvisaoOcorrencia");

                int.TryParse(xmlNotaFiscal.Serie, out int serie);
                documentoProvisao.Filial = cargaOcorrencia.Carga?.Filial;
                documentoProvisao.XMLNotaFiscal = xmlNotaFiscal;
                documentoProvisao.Empresa = cargaOcorrencia.Carga?.Empresa;
                documentoProvisao.TipoOperacao = cargaOcorrencia.Carga?.TipoOperacao;
                documentoProvisao.Tomador = cargaCTeComplementoInfo.TomadorPagador;
                documentoProvisao.CentroResultado = cargaCTeComplementoInfo.CentroResultado;
                documentoProvisao.NumeroDocumento = xmlNotaFiscal.Numero;
                documentoProvisao.SerieDocumento = serie;
                documentoProvisao.DataEmissao = xmlNotaFiscal.DataEmissao;
                documentoProvisao.CargaOcorrencia = cargaOcorrencia;
                documentoProvisao.Carga = cargaOcorrencia.Carga;
                documentoProvisao.Remetente = xmlNotaFiscal.Emitente;
                documentoProvisao.Destinatario = xmlNotaFiscal.Destinatario;
                documentoProvisao.Expedidor = pedidoXMLNotaFiscal?.CargaPedido?.Expedidor;
                documentoProvisao.Recebedor = pedidoXMLNotaFiscal?.CargaPedido?.Recebedor;
                documentoProvisao.PesoBruto = xmlNotaFiscal.Peso;
            }

            SetarValoresDocumentoProvisaoOcorrecia(cargaCTeComplementoInfo, ref documentoProvisao, ref valorProvisao, ref baseCalculoICMS, ref valorICMS, ref valorISS, ref baseCalculoISS, ref valorRetencaoISS, quantidade, ultimoRegistro);

            if (documentoProvisao.Situacao == SituacaoProvisaoDocumento.AgProvisao)
            {
                if (pedidoXMLNotaFiscal != null)
                {
                    new Servicos.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork).DefinirImpostoValorAgregado(pedidoXMLNotaFiscal.CargaPedido);

                    documentoProvisao.Origem = pedidoXMLNotaFiscal.CargaPedido.Origem;
                    documentoProvisao.Destino = pedidoXMLNotaFiscal.CargaPedido.Destino;
                    documentoProvisao.ImpostoValorAgregado = pedidoXMLNotaFiscal.CargaPedido.ImpostoValorAgregado;
                    documentoProvisao.Stage = stage;
                }
                else
                {
                    documentoProvisao.Origem = xmlNotaFiscal.Emitente.Localidade;
                    documentoProvisao.Destino = xmlNotaFiscal.Destinatario.Localidade;
                }
            }

            Servicos.Log.GravarInfo($"Valor de provisão final: {documentoProvisao.ValorProvisao.ToString()}", "LogDocumentoProvisaoOcorrencia");

            if (inserir)
                repositorioDocumentoProvisao.Inserir(documentoProvisao);
            else
                repositorioDocumentoProvisao.Atualizar(documentoProvisao);
        }

        public static void SetarDocumentosSelecionados(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, int carga, int cargaOcorrencia, DateTime dataInicio, DateTime dataFim, double tomador, int filial, int empresa, dynamic listaNaoSelecionadas, dynamic listaSelecionadas, bool todosSelecionados, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

            //totalRegistros = repDocumentoProvisao.ContarConsulta(codigoProvisao, somenteSemProvisao, concatenarComDocumentosSemPrevisao, dataInicio, dataFim, transportador, filial, tomador, situacaoProvisaoDocumento);

            int totalRegistros = 0;
            decimal valorProvisao = 0;
            if (todosSelecionados)
            {
                List<int> codigosNaoSelecionadas = new List<int>();
                if (listaNaoSelecionadas != null)
                {
                    foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                        codigosNaoSelecionadas.Add((int)dynNaoSelecionada.Codigo);
                }

                cancelamentoProvisao.QuantidadeDocsProvisao = repDocumentoProvisao.SetarDocumentosParaCancelamentoProvisao(cancelamentoProvisao.Codigo, carga, cargaOcorrencia, dataInicio, dataFim, tomador, filial, empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmCancelamento, codigosNaoSelecionadas, cancelamentoProvisao.CancelamentoProvisaoContraPartida);
                cancelamentoProvisao.ValorCancelamentoProvisao = repDocumentoProvisao.ValorTotalPorCancelamentoProvisao(cancelamentoProvisao.Codigo);
            }
            else
            {
                if (listaSelecionadas != null)
                {
                    foreach (var dynSelecionada in listaSelecionadas)
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repDocumentoProvisao.BuscarPorCodigo((int)dynSelecionada.Codigo, false);
                        documentoProvisao.CancelamentoProvisao = cancelamentoProvisao;
                        documentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmCancelamento;
                        documentoProvisao.MovimentoFinanceiroGerado = false;
                        repDocumentoProvisao.Atualizar(documentoProvisao);
                        totalRegistros++;
                        valorProvisao += documentoProvisao.ValorProvisao;
                    }
                }

                cancelamentoProvisao.ValorCancelamentoProvisao = valorProvisao;
                cancelamentoProvisao.QuantidadeDocsProvisao = totalRegistros;
            }

            cancelamentoProvisao.GerandoMovimentoFinanceiro = true;
            repProvisao.Atualizar(cancelamentoProvisao);
        }

        public static void AdicionarDocumentosParaCancelamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, List<int> codigosDocumentosProvisaoCancelar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

            int totalRegistros = 0;
            decimal valorProvisao = 0;

            if (codigosDocumentosProvisaoCancelar != null)
            {
                foreach (int codigoDocumento in codigosDocumentosProvisaoCancelar)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repDocumentoProvisao.BuscarPorCodigo(codigoDocumento, false);
                    documentoProvisao.CancelamentoProvisao = cancelamentoProvisao;
                    documentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmCancelamento;
                    documentoProvisao.MovimentoFinanceiroGerado = false;
                    repDocumentoProvisao.Atualizar(documentoProvisao);
                    totalRegistros++;
                    valorProvisao += documentoProvisao.ValorProvisao;
                }
            }

            cancelamentoProvisao.ValorCancelamentoProvisao = valorProvisao;
            cancelamentoProvisao.QuantidadeDocsProvisao = totalRegistros;
            cancelamentoProvisao.GerandoMovimentoFinanceiro = totalRegistros > 0;

        }

        public static void AdicionarDocumentoParaCancelamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao, Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

            documentoProvisao.CancelamentoProvisao = cancelamentoProvisao;
            documentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmCancelamento;
            documentoProvisao.MovimentoFinanceiroGerado = false;

            repDocumentoProvisao.Atualizar(documentoProvisao);
        }

        public static void AdicionarDocumentoParaProvisao(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, bool xmlNotaFiscalFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if ((tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (pedidoCTeParaSubContratacao.CargaPedido.Carga?.Empresa?.ProvisionarDocumentos ?? false)) && !xmlNotaFiscalFilialEmissora)
            {
                bool provisionar = false;
                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = pedidoCTeParaSubContratacao.CTeTerceiro;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = pedidoCTeParaSubContratacao.CargaPedido;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.CargaOrigem;
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (tomador.GrupoPessoas != null)
                    provisionar = tomador.GrupoPessoas.ProvisionarDocumentos;

                if (!provisionar)
                    provisionar = tomador.ProvisionarDocumentos;

                if (!provisionar)
                    provisionar = carga.Empresa?.ProvisionarDocumentos ?? false;

                if (provisionar && tomador.DataViradaProvisao.HasValue && tomador.DataViradaProvisao.Value >= carga.DataCriacaoCarga)
                    provisionar = false;

                if (!provisionar)
                    return;

                if (carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                    return;

                if ((cargaPedido.StageRelevanteCusto == null) && (carga.TipoDocumentoTransporte != null))
                    return;

                Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repositorioPedidoPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repositorioDocumentoProvisao.BuscarPorCTeAnteriorECarga(cteTerceiro.Codigo, carga.Codigo, pedidoCTeParaSubContratacao.Codigo);
                List<(decimal Valor, TipoComponenteFrete TipoComponente, int CodigoComponenteFrete)> valoresPorTipoComponente = repositorioPedidoPedidoCteParaSubContratacaoComponenteFrete.BuscarValoresPorPedidoCTeParaSubContratacao(pedidoCTeParaSubContratacao.Codigo);
                int codigoComponenteFreteValorContrato = carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.Codigo ?? 0;
                bool inserir = false;

                new Servicos.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork).DefinirImpostoValorAgregado(cargaPedido);

                if (documentoProvisao == null)
                {
                    documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao
                    {
                        ModeloDocumentoFiscal = repositorioModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe),
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                    };

                    inserir = true;
                }

                documentoProvisao.ValorDesconto = repositorioPedidoPedidoCteParaSubContratacaoComponenteFrete.BuscarValorTotalDescontoPorPedidoCTeParaSubContratacao(pedidoCTeParaSubContratacao.Codigo);
                documentoProvisao.ValorProvisaoCompetencia = pedidoCTeParaSubContratacao.ValorFrete + pedidoCTeParaSubContratacao.ValorTotalComponentes;
                documentoProvisao.BaseCalculoICMSCompetencia = pedidoCTeParaSubContratacao.BaseCalculoICMS;
                documentoProvisao.PercentualAliquotaCompetencia = pedidoCTeParaSubContratacao.PercentualAliquota;
                documentoProvisao.ValorICMSCompetencia = pedidoCTeParaSubContratacao.ValorICMS;
                documentoProvisao.CSTCompetencia = pedidoCTeParaSubContratacao.CST;
                documentoProvisao.ValorISSCompetencia = pedidoCTeParaSubContratacao.ValorISS;
                documentoProvisao.BaseCalculoISSCompetencia = pedidoCTeParaSubContratacao.BaseCalculoISS;
                documentoProvisao.ICMSInclusoBCCompetencia = pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo;
                documentoProvisao.ISSInclusoBCCompetencia = pedidoCTeParaSubContratacao.IncluirISSBaseCalculo;
                documentoProvisao.PercentualAliquotaISSCompetencia = pedidoCTeParaSubContratacao.PercentualAliquotaISS;
                documentoProvisao.ValorRetencaoISSCompetencia = pedidoCTeParaSubContratacao.ValorRetencaoISS;
                documentoProvisao.ImpostoValorAgregado = cargaPedido.ImpostoValorAgregado;
                documentoProvisao.Stage = cargaPedido.StageRelevanteCusto;
                documentoProvisao.ValorAdValorem = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.ADVALOREM).Sum(valorPorTipo => valorPorTipo.Valor);
                documentoProvisao.ValorDescarga = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.DESCARGA).Sum(valorPorTipo => valorPorTipo.Valor);
                documentoProvisao.ValorPedagio = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.PEDAGIO).Sum(valorPorTipo => valorPorTipo.Valor);
                documentoProvisao.ValorGris = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.GRIS).Sum(valorPorTipo => valorPorTipo.Valor);
                documentoProvisao.ValorEntrega = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.ENTREGA).Sum(valorPorTipo => valorPorTipo.Valor);
                documentoProvisao.ValorPernoite = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.PERNOITE).Sum(valorPorTipo => valorPorTipo.Valor);
                documentoProvisao.ValorContratoFrete = (codigoComponenteFreteValorContrato > 0) ? valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.CodigoComponenteFrete == codigoComponenteFreteValorContrato).Sum(valorPorTipo => valorPorTipo.Valor) : 0;

                GravarLogImpostosProvisao(documentoProvisao, $"cteTerceiro = {cteTerceiro?.Codigo ?? 0}, pedidoCTeParaSubContratacao = {pedidoCTeParaSubContratacao?.Codigo ?? 0}");

                SetarValoresFrete(documentoProvisao, carga, pedidoCTeParaSubContratacao.ValorFrete, unitOfWork);

                if (documentoProvisao.Situacao == SituacaoProvisaoDocumento.AgProvisao)
                {
                    documentoProvisao.CentroResultado = pedidoCTeParaSubContratacao.CentroResultado;
                    documentoProvisao.Filial = carga.Filial;
                    documentoProvisao.CTeTerceiro = cteTerceiro;
                    documentoProvisao.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                    documentoProvisao.Empresa = carga.Empresa;
                    documentoProvisao.TipoOperacao = carga.TipoOperacao;
                    documentoProvisao.Tomador = tomador;
                    documentoProvisao.NumeroDocumento = cteTerceiro.Numero;
                    documentoProvisao.SerieDocumento = cteTerceiro.Serie.ToInt();
                    documentoProvisao.DataEmissao = cteTerceiro.DataEmissao;
                    documentoProvisao.Carga = carga;
                    documentoProvisao.Remetente = cteTerceiro.Remetente.Cliente;
                    documentoProvisao.Destinatario = cteTerceiro.Destinatario.Cliente;
                    documentoProvisao.Expedidor = cteTerceiro.Expedidor?.Cliente;
                    documentoProvisao.Recebedor = cteTerceiro.Recebedor?.Cliente;
                    documentoProvisao.Origem = pedidoCTeParaSubContratacao.CargaPedido.Origem;
                    documentoProvisao.Destino = pedidoCTeParaSubContratacao.CargaPedido.Destino;
                    documentoProvisao.PesoBruto = cteTerceiro.CTeTerceiroQuantidades?.Where(obj => obj.Unidade == Dominio.Enumeradores.UnidadeMedida.KG).Sum(obj => obj.Quantidade) ?? pedidoCTeParaSubContratacao.CargaPedido.Peso;
                    documentoProvisao.ValorProvisao = documentoProvisao.ValorProvisaoCompetencia;
                    documentoProvisao.BaseCalculoICMS = documentoProvisao.BaseCalculoICMSCompetencia;
                    documentoProvisao.PercentualAliquota = documentoProvisao.PercentualAliquotaCompetencia;
                    documentoProvisao.ValorICMS = documentoProvisao.ValorICMSCompetencia;
                    documentoProvisao.CST = documentoProvisao.CSTCompetencia;
                    documentoProvisao.ICMSInclusoBC = documentoProvisao.ICMSInclusoBCCompetencia;
                    documentoProvisao.ISSInclusoBC = documentoProvisao.ISSInclusoBCCompetencia;
                    documentoProvisao.ValorISS = documentoProvisao.ValorISSCompetencia;
                    documentoProvisao.BaseCalculoISS = documentoProvisao.BaseCalculoISSCompetencia;
                    documentoProvisao.PercentualAliquotaISS = documentoProvisao.PercentualAliquotaISSCompetencia;
                    documentoProvisao.ValorRetencaoISS = documentoProvisao.ValorRetencaoISSCompetencia;
                }

                documentoProvisao.OutrasAliquotas = pedidoCTeParaSubContratacao.OutrasAliquotas;
                documentoProvisao.CSTIBSCBS = pedidoCTeParaSubContratacao.CSTIBSCBS;
                documentoProvisao.ClassificacaoTributariaIBSCBS = pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS;
                documentoProvisao.BaseCalculoIBSCBS = pedidoCTeParaSubContratacao.BaseCalculoIBSCBS;
                documentoProvisao.AliquotaIBSEstadual = pedidoCTeParaSubContratacao.AliquotaIBSEstadual;
                documentoProvisao.PercentualReducaoIBSEstadual = pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual;
                documentoProvisao.ValorIBSEstadual = pedidoCTeParaSubContratacao.ValorIBSEstadual;
                documentoProvisao.AliquotaIBSMunicipal = pedidoCTeParaSubContratacao.AliquotaIBSMunicipal;
                documentoProvisao.PercentualReducaoIBSMunicipal = pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal;
                documentoProvisao.ValorIBSMunicipal = pedidoCTeParaSubContratacao.ValorIBSMunicipal;
                documentoProvisao.AliquotaCBS = pedidoCTeParaSubContratacao.AliquotaCBS;
                documentoProvisao.PercentualReducaoCBS = pedidoCTeParaSubContratacao.PercentualReducaoCBS;
                documentoProvisao.ValorCBS = pedidoCTeParaSubContratacao.ValorCBS;

                if (inserir)
                    repositorioDocumentoProvisao.Inserir(documentoProvisao);
                else
                    repositorioDocumentoProvisao.Atualizar(documentoProvisao);
            }
        }

        public static void AdicionarDocumentoParaProvisao(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool xmlNotaFiscalFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisaoCarga = null, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosEmissaoNFSCarga = null)
        {
            Servicos.Log.GravarInfo($"1 - Iniciou a provisão da nota {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero} da carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}", "GeracaoDocumentosProvisao");

            if (xmlNotaFiscalFilialEmissora || (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !(pedidoXMLNotaFiscal.CargaPedido.CargaOrigem.Empresa?.ProvisionarDocumentos ?? false)))
                return;

            bool provisionar = false;
            bool gerouDocNfseManual = (documentosEmissaoNFSCarga != null && documentosEmissaoNFSCarga.Any(obj => obj.PedidoXMLNotaFiscal?.Codigo == pedidoXMLNotaFiscal.Codigo));
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = pedidoXMLNotaFiscal.CargaPedido;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.CargaOrigem;
            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            if (tomador.GrupoPessoas != null)
                provisionar = tomador.GrupoPessoas.ProvisionarDocumentos;

            if (!provisionar)
                provisionar = tomador.ProvisionarDocumentos;

            if (!provisionar)
                provisionar = carga.Empresa?.ProvisionarDocumentos ?? false;

            if (provisionar && tomador.DataViradaProvisao.HasValue && tomador.DataViradaProvisao.Value >= carga.DataCriacaoCarga)
                provisionar = false;

            if (!provisionar)
                return;

            if (carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                return;

            if ((cargaPedido.StageRelevanteCusto == null) && (carga.TipoDocumentoTransporte != null))
                return;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repositorioPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            List<(decimal Valor, TipoComponenteFrete TipoComponente, int CodigoComponenteFrete)> valoresPorTipoComponente = repositorioPedidoXMLNotaFiscalComponenteFrete.BuscarValoresPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);
            int codigoComponenteFreteValorContrato = carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = null;
            bool inserir = false;

            new Servicos.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork).DefinirImpostoValorAgregado(cargaPedido);

            if (documentosProvisaoCarga != null)
            {
                documentoProvisao = (from obj in documentosProvisaoCarga
                                     where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo
                                        && obj.Carga.Codigo == carga.Codigo
                                        && obj.CargaOcorrencia == null
                                     select obj).FirstOrDefault();
            }
            else
                documentoProvisao = repositorioDocumentoProvisao.BuscarPorXMLNotaFiscalECarga(xmlNotaFiscal.Codigo, carga.Codigo, considerarProvisaoComOcorrencia: false);

            if (documentoProvisao != null && gerouDocNfseManual)
                return;

            if (documentoProvisao == null)
            {
                documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao()
                {
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                };

                inserir = true;
                Servicos.Embarcador.Documentos.ModeloDocumentoFiscal modelo = Servicos.Embarcador.Documentos.ModeloDocumentoFiscal.GetInstance(unitOfWork);

                if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                    documentoProvisao.ModeloDocumentoFiscal = modelo.ModeloNFe;
                else
                    documentoProvisao.ModeloDocumentoFiscal = modelo.ModeloOutras;
            }

            documentoProvisao.ValorDesconto = repositorioPedidoXMLNotaFiscalComponenteFrete.BuscarValorTotalDescontoPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);
            documentoProvisao.ValorProvisaoCompetencia = pedidoXMLNotaFiscal.ValorFrete + pedidoXMLNotaFiscal.ValorTotalComponentes;
            documentoProvisao.BaseCalculoICMSCompetencia = pedidoXMLNotaFiscal.BaseCalculoICMS;
            documentoProvisao.PercentualAliquotaCompetencia = pedidoXMLNotaFiscal.PercentualAliquota;
            documentoProvisao.ValorICMSCompetencia = pedidoXMLNotaFiscal.ValorICMS;
            documentoProvisao.CSTCompetencia = pedidoXMLNotaFiscal.CST;
            documentoProvisao.ValorISSCompetencia = pedidoXMLNotaFiscal.ValorISS;
            documentoProvisao.BaseCalculoISSCompetencia = pedidoXMLNotaFiscal.BaseCalculoISS;
            documentoProvisao.ICMSInclusoBCCompetencia = pedidoXMLNotaFiscal.IncluirICMSBaseCalculo;
            documentoProvisao.ISSInclusoBCCompetencia = pedidoXMLNotaFiscal.IncluirISSBaseCalculo;
            documentoProvisao.PercentualAliquotaISSCompetencia = pedidoXMLNotaFiscal.PercentualAliquotaISS;
            documentoProvisao.ValorRetencaoISSCompetencia = pedidoXMLNotaFiscal.ValorRetencaoISS;
            documentoProvisao.ImpostoValorAgregado = cargaPedido.ImpostoValorAgregado;
            documentoProvisao.Stage = cargaPedido.StageRelevanteCusto;
            documentoProvisao.ValorAdValorem = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.ADVALOREM).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorDescarga = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.DESCARGA).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorPedagio = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.PEDAGIO).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorGris = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.GRIS).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorEntrega = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.ENTREGA).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorPernoite = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.PERNOITE).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorContratoFrete = (codigoComponenteFreteValorContrato > 0) ? valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.CodigoComponenteFrete == codigoComponenteFreteValorContrato).Sum(valorPorTipo => valorPorTipo.Valor) : 0;

            if (gerouDocNfseManual)
                ZerarValoresISSProvisaoParaNFSEManual(documentoProvisao);

            GravarLogImpostosProvisao(documentoProvisao, $"pedidoXMLNotaFiscal = {pedidoXMLNotaFiscal?.Codigo ?? 0}, xmlNotaFiscal = {xmlNotaFiscal?.Codigo ?? 0}");

            SetarValoresFrete(documentoProvisao, carga, pedidoXMLNotaFiscal.ValorFrete, unitOfWork);

            if (documentoProvisao.Situacao == SituacaoProvisaoDocumento.AgProvisao)
            {
                int.TryParse(xmlNotaFiscal.Serie, out int serie);

                documentoProvisao.CentroResultado = pedidoXMLNotaFiscal.CentroResultado;
                documentoProvisao.Filial = carga.Filial;
                documentoProvisao.XMLNotaFiscal = xmlNotaFiscal;
                documentoProvisao.Empresa = carga.Empresa;
                documentoProvisao.TipoOperacao = carga.TipoOperacao;
                documentoProvisao.Tomador = tomador;
                documentoProvisao.NumeroDocumento = xmlNotaFiscal.Numero;
                documentoProvisao.SerieDocumento = serie;
                documentoProvisao.DataEmissao = xmlNotaFiscal.DataEmissao;
                documentoProvisao.Carga = carga;
                documentoProvisao.ValorProvisao = documentoProvisao.ValorProvisaoCompetencia;
                documentoProvisao.BaseCalculoICMS = documentoProvisao.BaseCalculoICMSCompetencia;
                documentoProvisao.PercentualAliquota = documentoProvisao.PercentualAliquotaCompetencia;
                documentoProvisao.ValorICMS = documentoProvisao.ValorICMSCompetencia;
                documentoProvisao.CST = documentoProvisao.CSTCompetencia;
                documentoProvisao.ValorISS = documentoProvisao.ValorISSCompetencia;
                documentoProvisao.BaseCalculoISS = documentoProvisao.BaseCalculoISSCompetencia;
                documentoProvisao.ICMSInclusoBC = documentoProvisao.ICMSInclusoBCCompetencia;
                documentoProvisao.ISSInclusoBC = documentoProvisao.ISSInclusoBCCompetencia;
                documentoProvisao.PercentualAliquotaISS = documentoProvisao.PercentualAliquotaISSCompetencia;
                documentoProvisao.ValorRetencaoISS = documentoProvisao.ValorRetencaoISSCompetencia;
                documentoProvisao.Remetente = xmlNotaFiscal.Emitente;
                documentoProvisao.Destinatario = xmlNotaFiscal.Destinatario;
                documentoProvisao.Expedidor = cargaPedido.Expedidor;
                documentoProvisao.Recebedor = cargaPedido.Recebedor;
                documentoProvisao.Origem = pedidoXMLNotaFiscal.CargaPedido.Origem;
                documentoProvisao.Destino = pedidoXMLNotaFiscal.CargaPedido.Destino;
                documentoProvisao.PesoBruto = xmlNotaFiscal.Peso;
            }

            documentoProvisao.OutrasAliquotas = pedidoXMLNotaFiscal.OutrasAliquotas;
            documentoProvisao.CSTIBSCBS = pedidoXMLNotaFiscal.CSTIBSCBS;
            documentoProvisao.ClassificacaoTributariaIBSCBS = pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS;
            documentoProvisao.BaseCalculoIBSCBS = pedidoXMLNotaFiscal.BaseCalculoIBSCBS;
            documentoProvisao.AliquotaIBSEstadual = pedidoXMLNotaFiscal.AliquotaIBSEstadual;
            documentoProvisao.PercentualReducaoIBSEstadual = pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual;
            documentoProvisao.ValorIBSEstadual = pedidoXMLNotaFiscal.ValorIBSEstadual;
            documentoProvisao.AliquotaIBSMunicipal = pedidoXMLNotaFiscal.AliquotaIBSMunicipal;
            documentoProvisao.PercentualReducaoIBSMunicipal = pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal;
            documentoProvisao.ValorIBSMunicipal = pedidoXMLNotaFiscal.ValorIBSMunicipal;
            documentoProvisao.AliquotaCBS = pedidoXMLNotaFiscal.AliquotaCBS;
            documentoProvisao.PercentualReducaoCBS = pedidoXMLNotaFiscal.PercentualReducaoCBS;
            documentoProvisao.ValorCBS = pedidoXMLNotaFiscal.ValorCBS;

            if (inserir)
                repositorioDocumentoProvisao.Inserir(documentoProvisao);
            else
                repositorioDocumentoProvisao.Atualizar(documentoProvisao);

            Servicos.Log.GravarInfo($"1 - Finalizou a provisão da nota {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero} da carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}", "GeracaoDocumentosProvisao");
        }

        public static void AdicionarDocumentoParaProvisaoPorCargaPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool xmlNotaFiscalFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisaoCarga = null)
        {
            Servicos.Log.GravarInfo($"2 - Iniciou a provisão da nota {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero} da carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}", "GeracaoDocumentosProvisao");

            if (xmlNotaFiscalFilialEmissora || (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !(pedidoXMLNotaFiscal.CargaPedido.CargaOrigem.Empresa?.ProvisionarDocumentos ?? false)))
                return;

            bool provisionar = false;
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = pedidoXMLNotaFiscal.CargaPedido;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.CargaOrigem;
            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            if (tomador.GrupoPessoas != null)
                provisionar = tomador.GrupoPessoas.ProvisionarDocumentos;

            if (!provisionar)
                provisionar = tomador.ProvisionarDocumentos;

            if (!provisionar)
                provisionar = carga.Empresa?.ProvisionarDocumentos ?? false;

            if (provisionar && tomador.DataViradaProvisao.HasValue && tomador.DataViradaProvisao.Value >= carga.DataCriacaoCarga)
                provisionar = false;

            if (!provisionar)
                return;

            if (carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                return;

            if ((cargaPedido.StageRelevanteCusto == null) && (carga.TipoDocumentoTransporte != null))
                return;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repositorioPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            List<(decimal Valor, TipoComponenteFrete TipoComponente, int CodigoComponenteFrete)> valoresPorTipoComponente = repositorioPedidoXMLNotaFiscalComponenteFrete.BuscarValoresPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);
            int codigoComponenteFreteValorContrato = carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = null;
            bool inserir = false;

            new Servicos.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork).DefinirImpostoValorAgregado(cargaPedido);

            if (documentosProvisaoCarga != null)
                documentoProvisao = (from obj in documentosProvisaoCarga
                                     where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo
                                        && obj.Carga.Codigo == carga.Codigo
                                        && obj.CargaOcorrencia == null
                                     select obj).FirstOrDefault();
            else
                documentoProvisao = repositorioDocumentoProvisao.BuscarPorXMLNotaFiscalECarga(xmlNotaFiscal.Codigo, carga.Codigo, considerarProvisaoComOcorrencia: false);

            if (documentoProvisao == null)
            {
                documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao()
                {
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                };

                inserir = true;
                Servicos.Embarcador.Documentos.ModeloDocumentoFiscal modelo = Servicos.Embarcador.Documentos.ModeloDocumentoFiscal.GetInstance(unitOfWork);

                if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                    documentoProvisao.ModeloDocumentoFiscal = modelo.ModeloNFe;
                else
                    documentoProvisao.ModeloDocumentoFiscal = modelo.ModeloOutras;
            }

            documentoProvisao.ValorDesconto = repositorioPedidoXMLNotaFiscalComponenteFrete.BuscarValorTotalDescontoPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);
            documentoProvisao.ValorProvisaoCompetencia = cargaPedido.ValorFrete;
            documentoProvisao.BaseCalculoICMSCompetencia = cargaPedido.ValorFrete;
            documentoProvisao.PercentualAliquotaCompetencia = cargaPedido.PercentualAliquota;
            documentoProvisao.ValorICMSCompetencia = cargaPedido.ValorICMS;
            documentoProvisao.CSTCompetencia = cargaPedido.CST;
            documentoProvisao.ValorISSCompetencia = cargaPedido.ValorISS;
            documentoProvisao.BaseCalculoISSCompetencia = cargaPedido.BaseCalculoISS;
            documentoProvisao.ICMSInclusoBCCompetencia = false;
            documentoProvisao.ISSInclusoBCCompetencia = cargaPedido.IncluirISSBaseCalculo;
            documentoProvisao.PercentualAliquotaISSCompetencia = cargaPedido.PercentualAliquotaISS;
            documentoProvisao.ValorRetencaoISSCompetencia = cargaPedido.ValorRetencaoISS;
            documentoProvisao.ValorAdValorem = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.ADVALOREM).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorDescarga = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.DESCARGA).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorPedagio = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.PEDAGIO).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorGris = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.GRIS).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorEntrega = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.ENTREGA).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorPernoite = valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.TipoComponente == TipoComponenteFrete.PERNOITE).Sum(valorPorTipo => valorPorTipo.Valor);
            documentoProvisao.ValorContratoFrete = (codigoComponenteFreteValorContrato > 0) ? valoresPorTipoComponente.Where(valorPorTipo => valorPorTipo.CodigoComponenteFrete == codigoComponenteFreteValorContrato).Sum(valorPorTipo => valorPorTipo.Valor) : 0;

            GravarLogImpostosProvisao(documentoProvisao, $"pedidoXMLNotaFiscal = {pedidoXMLNotaFiscal?.Codigo ?? 0}, xmlNotaFiscal = {xmlNotaFiscal?.Codigo ?? 0}");

            SetarValoresFrete(documentoProvisao, carga, cargaPedido.ValorFrete, unitOfWork);

            if (documentoProvisao.Situacao == SituacaoProvisaoDocumento.AgProvisao)
            {
                int.TryParse(xmlNotaFiscal.Serie, out int serie);

                documentoProvisao.CentroResultado = cargaPedido.CentroResultado;
                documentoProvisao.Filial = carga.Filial;
                documentoProvisao.XMLNotaFiscal = xmlNotaFiscal;
                documentoProvisao.Empresa = carga.Empresa;
                documentoProvisao.TipoOperacao = carga.TipoOperacao;
                documentoProvisao.Tomador = tomador;
                documentoProvisao.NumeroDocumento = xmlNotaFiscal.Numero;
                documentoProvisao.SerieDocumento = serie;
                documentoProvisao.DataEmissao = xmlNotaFiscal.DataEmissao;
                documentoProvisao.Carga = carga;
                documentoProvisao.ValorProvisao = cargaPedido.ValorFreteAPagar;
                documentoProvisao.BaseCalculoICMS = documentoProvisao.BaseCalculoICMSCompetencia;
                documentoProvisao.PercentualAliquota = documentoProvisao.PercentualAliquotaCompetencia;
                documentoProvisao.ValorICMS = documentoProvisao.ValorICMSCompetencia;
                documentoProvisao.CST = documentoProvisao.CSTCompetencia;
                documentoProvisao.ValorISS = documentoProvisao.ValorISSCompetencia;
                documentoProvisao.BaseCalculoISS = documentoProvisao.BaseCalculoISSCompetencia;
                documentoProvisao.ICMSInclusoBC = documentoProvisao.ICMSInclusoBCCompetencia;
                documentoProvisao.ISSInclusoBC = documentoProvisao.ISSInclusoBCCompetencia;
                documentoProvisao.PercentualAliquotaISS = documentoProvisao.PercentualAliquotaISSCompetencia;
                documentoProvisao.ValorRetencaoISS = documentoProvisao.ValorRetencaoISSCompetencia;
                documentoProvisao.Remetente = xmlNotaFiscal.Emitente;
                documentoProvisao.Destinatario = xmlNotaFiscal.Destinatario;
                documentoProvisao.Expedidor = cargaPedido.Expedidor;
                documentoProvisao.Recebedor = cargaPedido.Recebedor;
                documentoProvisao.Origem = cargaPedido.Origem;
                documentoProvisao.Destino = cargaPedido.Destino;
                documentoProvisao.PesoBruto = xmlNotaFiscal.Peso;
            }

            documentoProvisao.OutrasAliquotas = cargaPedido.OutrasAliquotas;
            documentoProvisao.CSTIBSCBS = cargaPedido.CSTIBSCBS;
            documentoProvisao.ClassificacaoTributariaIBSCBS = cargaPedido.ClassificacaoTributariaIBSCBS;
            documentoProvisao.BaseCalculoIBSCBS = cargaPedido.BaseCalculoIBSCBS;
            documentoProvisao.AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadual;
            documentoProvisao.PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadual;
            documentoProvisao.ValorIBSEstadual = cargaPedido.ValorIBSEstadual;
            documentoProvisao.AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipal;
            documentoProvisao.PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipal;
            documentoProvisao.ValorIBSMunicipal = cargaPedido.ValorIBSMunicipal;
            documentoProvisao.AliquotaCBS = cargaPedido.AliquotaCBS;
            documentoProvisao.PercentualReducaoCBS = cargaPedido.PercentualReducaoCBS;
            documentoProvisao.ValorCBS = cargaPedido.ValorCBS;

            if (inserir)
                repositorioDocumentoProvisao.Inserir(documentoProvisao);
            else
                repositorioDocumentoProvisao.Atualizar(documentoProvisao);

            Servicos.Log.GravarInfo($"2 - Finalizou a provisão da nota {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero} da carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}", "GeracaoDocumentosProvisao");
        }

        public static void AdicionarDocumentoParaProvisao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && configuracaoTMS.ProvisionarDocumentosEmitidos)
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repConfiguracaoIntegracaoGrupoPessoas = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repConfiguracaoIntegracaoGrupoPessoas.BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour);
                if (integracaoGrupoPessoas.Count > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTe.BuscarCTesTransportadorPorCarga(carga.Codigo);
                    for (int i = 0; i < ctes.Count(); i++)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];
                        if (integracaoGrupoPessoas.Any(obj => obj.GrupoPessoas?.Codigo == cte.TomadorPagador.Cliente.GrupoPessoas?.Codigo))
                        {
                            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao
                            {
                                CentroResultado = cte.CentroResultado,
                                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao,
                                Filial = carga.Filial,
                                CTe = cte,
                                Empresa = cte.Empresa,
                                TipoOperacao = carga.TipoOperacao,
                                ModeloDocumentoFiscal = cte.ModeloDocumentoFiscal,
                                Tomador = cte.TomadorPagador.Cliente,
                                NumeroDocumento = cte.Numero,
                                SerieDocumento = cte.Serie.Numero,
                                DataEmissao = cte.DataEmissao.Value,
                                Remetente = cte.Remetente.Cliente,
                                Destinatario = cte.Destinatario.Cliente,
                                Expedidor = cte.Expedidor?.Cliente,
                                Recebedor = cte.Recebedor?.Cliente,
                                Origem = cte.LocalidadeInicioPrestacao,
                                Destino = cte.LocalidadeTerminoPrestacao,
                                PesoBruto = cte.Peso,
                                Carga = carga,
                                ValorProvisao = cte.ValorAReceber,
                                BaseCalculoICMS = cte.BaseCalculoICMS,
                                PercentualAliquota = cte.AliquotaICMS,
                                ValorICMS = cte.ValorICMS,
                                CST = cte.CST,
                                ValorISS = cte.ValorISS,
                                BaseCalculoISS = cte.BaseCalculoISS,
                                ICMSInclusoBC = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                                ISSInclusoBC = cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                                PercentualAliquotaISS = cte.PercentualISSRetido,
                                ValorRetencaoISS = cte.ValorISSRetido,

                                ValorProvisaoCompetencia = cte.ValorAReceber,
                                BaseCalculoICMSCompetencia = cte.BaseCalculoICMS,
                                PercentualAliquotaCompetencia = cte.AliquotaICMS,
                                ValorICMSCompetencia = cte.ValorICMS,
                                CSTCompetencia = cte.CST,
                                ValorISSCompetencia = cte.ValorISS,
                                BaseCalculoISSCompetencia = cte.BaseCalculoISS,
                                ICMSInclusoBCCompetencia = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                                ISSInclusoBCCompetencia = cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                                PercentualAliquotaISSCompetencia = cte.PercentualISSRetido,
                                ValorRetencaoISSCompetencia = cte.ValorISSRetido,

                                OutrasAliquotas = cte.OutrasAliquotas,
                                CSTIBSCBS = cte.CSTIBSCBS,
                                ClassificacaoTributariaIBSCBS = cte.ClassificacaoTributariaIBSCBS,
                                BaseCalculoIBSCBS = cte.BaseCalculoIBSCBS,
                                AliquotaIBSEstadual = cte.AliquotaIBSEstadual,
                                PercentualReducaoIBSEstadual = cte.PercentualReducaoIBSEstadual,
                                ValorIBSEstadual = cte.ValorIBSEstadual,
                                AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal,
                                PercentualReducaoIBSMunicipal = cte.PercentualReducaoIBSMunicipal,
                                ValorIBSMunicipal = cte.ValorIBSMunicipal,
                                AliquotaCBS = cte.AliquotaCBS,
                                PercentualReducaoCBS = cte.PercentualReducaoCBS,
                                ValorCBS = cte.ValorCBS
                            };

                            documentoProvisao.RegraEscrituracao = ObterRegraEscrituracao(documentoProvisao, unitOfWork);

                            repDocumentoProvisao.Inserir(documentoProvisao);
                        }
                    }
                }
            }
        }

        public static void AdicionarDocumentoParaProvisao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && configuracaoTMS.ProvisionarDocumentosEmitidos)
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repConfiguracaoIntegracaoGrupoPessoas = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repConfiguracaoIntegracaoGrupoPessoas.BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour);
                if (integracaoGrupoPessoas.Count > 0)
                {
                    Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = lancamentoNFSManual.CTe;
                    if (integracaoGrupoPessoas.Any(obj => obj.GrupoPessoas?.Codigo == cte.TomadorPagador.Cliente.GrupoPessoas?.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao
                        {
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao,
                            Filial = lancamentoNFSManual.Filial,
                            CTe = cte,
                            Empresa = cte.Empresa,
                            TipoOperacao = lancamentoNFSManual.TipoOperacao,
                            ModeloDocumentoFiscal = cte.ModeloDocumentoFiscal,
                            Tomador = cte.TomadorPagador.Cliente,
                            NumeroDocumento = cte.Numero,
                            SerieDocumento = cte.Serie.Numero,
                            DataEmissao = cte.DataEmissao.Value,
                            Remetente = cte.Remetente.Cliente,
                            Destinatario = cte.Destinatario.Cliente,
                            Expedidor = cte.Expedidor?.Cliente,
                            Recebedor = cte.Recebedor?.Cliente,
                            Origem = cte.LocalidadeInicioPrestacao,
                            Destino = cte.LocalidadeTerminoPrestacao,
                            PesoBruto = cte.Peso,
                            ValorProvisao = cte.ValorAReceber,
                            BaseCalculoICMS = cte.BaseCalculoICMS,
                            PercentualAliquota = cte.AliquotaICMS,
                            ValorICMS = cte.ValorICMS,
                            CST = cte.CST,
                            ValorISS = cte.ValorISS,
                            BaseCalculoISS = cte.BaseCalculoISS,
                            ICMSInclusoBC = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                            ISSInclusoBC = cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                            PercentualAliquotaISS = cte.PercentualISSRetido,
                            ValorRetencaoISS = cte.ValorISSRetido,
                            ValorProvisaoCompetencia = cte.ValorAReceber,
                            BaseCalculoICMSCompetencia = cte.BaseCalculoICMS,
                            PercentualAliquotaCompetencia = cte.AliquotaICMS,
                            ValorICMSCompetencia = cte.ValorICMS,
                            CSTCompetencia = cte.CST,
                            ValorISSCompetencia = cte.ValorISS,
                            BaseCalculoISSCompetencia = cte.BaseCalculoISS,
                            ICMSInclusoBCCompetencia = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                            ISSInclusoBCCompetencia = cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                            PercentualAliquotaISSCompetencia = cte.PercentualISSRetido,
                            ValorRetencaoISSCompetencia = cte.ValorISSRetido,

                            OutrasAliquotas = cte.OutrasAliquotas,
                            CSTIBSCBS = cte.CSTIBSCBS,
                            ClassificacaoTributariaIBSCBS = cte.ClassificacaoTributariaIBSCBS,
                            BaseCalculoIBSCBS = cte.BaseCalculoIBSCBS,
                            AliquotaIBSEstadual = cte.AliquotaIBSEstadual,
                            PercentualReducaoIBSEstadual = cte.PercentualReducaoIBSEstadual,
                            ValorIBSEstadual = cte.ValorIBSEstadual,
                            AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal,
                            PercentualReducaoIBSMunicipal = cte.PercentualReducaoIBSMunicipal,
                            ValorIBSMunicipal = cte.ValorIBSMunicipal,
                            AliquotaCBS = cte.AliquotaCBS,
                            PercentualReducaoCBS = cte.PercentualReducaoCBS,
                            ValorCBS = cte.ValorCBS
                        };
                        repDocumentoProvisao.Inserir(documentoProvisao);
                    }
                }
            }
        }

        public static void AdicionarDocumentoParaProvisao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && configuracaoTMS.ProvisionarDocumentosEmitidos)
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repConfiguracaoIntegracaoGrupoPessoas = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repConfiguracaoIntegracaoGrupoPessoas.BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour);
                if (integracaoGrupoPessoas.Count > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplentoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                    Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTeComplentoInfo.BuscarCTesPorOcorrencia(cargaOcorrencia.Codigo);
                    for (int i = 0; i < ctes.Count(); i++)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];
                        if (integracaoGrupoPessoas.Any(obj => obj.GrupoPessoas?.Codigo == cte.TomadorPagador.Cliente.GrupoPessoas?.Codigo))
                        {
                            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao
                            {
                                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao,
                                Filial = cargaOcorrencia.Carga?.Filial,
                                CTe = cte,
                                Empresa = cte.Empresa,
                                TipoOperacao = cargaOcorrencia.Carga?.TipoOperacao,
                                ModeloDocumentoFiscal = cte.ModeloDocumentoFiscal,
                                Tomador = cte.TomadorPagador.Cliente,
                                NumeroDocumento = cte.Numero,
                                SerieDocumento = cte.Serie.Numero,
                                DataEmissao = cte.DataEmissao.Value,
                                CargaOcorrencia = cargaOcorrencia,
                                Remetente = cte.Remetente.Cliente,
                                Destinatario = cte.Destinatario.Cliente,
                                Expedidor = cte.Expedidor?.Cliente,
                                Recebedor = cte.Recebedor?.Cliente,
                                Origem = cte.LocalidadeInicioPrestacao,
                                Destino = cte.LocalidadeTerminoPrestacao,
                                PesoBruto = cte.Peso,
                                ValorProvisao = cte.ValorAReceber,
                                BaseCalculoICMS = cte.BaseCalculoICMS,
                                PercentualAliquota = cte.AliquotaICMS,
                                ValorICMS = cte.ValorICMS,
                                CST = cte.CST,
                                ValorISS = cte.ValorISS,
                                BaseCalculoISS = cte.BaseCalculoISS,
                                ICMSInclusoBC = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                                ISSInclusoBC = cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                                PercentualAliquotaISS = cte.PercentualISSRetido,
                                ValorRetencaoISS = cte.ValorISSRetido,
                                ValorProvisaoCompetencia = cte.ValorAReceber,
                                BaseCalculoICMSCompetencia = cte.BaseCalculoICMS,
                                PercentualAliquotaCompetencia = cte.AliquotaICMS,
                                ValorICMSCompetencia = cte.ValorICMS,
                                CSTCompetencia = cte.CST,
                                ValorISSCompetencia = cte.ValorISS,
                                BaseCalculoISSCompetencia = cte.BaseCalculoISS,
                                ICMSInclusoBCCompetencia = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                                ISSInclusoBCCompetencia = cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                                PercentualAliquotaISSCompetencia = cte.PercentualISSRetido,
                                ValorRetencaoISSCompetencia = cte.ValorISSRetido,

                                OutrasAliquotas = cte.OutrasAliquotas,
                                CSTIBSCBS = cte.CSTIBSCBS,
                                ClassificacaoTributariaIBSCBS = cte.ClassificacaoTributariaIBSCBS,
                                BaseCalculoIBSCBS = cte.BaseCalculoIBSCBS,
                                AliquotaIBSEstadual = cte.AliquotaIBSEstadual,
                                PercentualReducaoIBSEstadual = cte.PercentualReducaoIBSEstadual,
                                ValorIBSEstadual = cte.ValorIBSEstadual,
                                AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal,
                                PercentualReducaoIBSMunicipal = cte.PercentualReducaoIBSMunicipal,
                                ValorIBSMunicipal = cte.ValorIBSMunicipal,
                                AliquotaCBS = cte.AliquotaCBS,
                                PercentualReducaoCBS = cte.PercentualReducaoCBS,
                                ValorCBS = cte.ValorCBS
                            };

                            repDocumentoProvisao.Inserir(documentoProvisao);
                        }
                    }
                }
            }
        }

        private static Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao ObterRegraEscrituracao(Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao, Repositorio.UnitOfWork unitOfWork)
        {
            if (documentoProvisao.CTe == null)
                return null;

            Repositorio.Embarcador.Escrituracao.RegraEscrituracao repRegraEscrituracao = new Repositorio.Embarcador.Escrituracao.RegraEscrituracao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            double remetente = documentoProvisao.CTe.Remetente?.Cliente?.CPF_CNPJ ?? 0;
            double destinatario = documentoProvisao.CTe.Destinatario?.Cliente?.CPF_CNPJ ?? 0;
            bool origemFilial = remetente > 0 ? repFilial.BuscarPorCNPJ(String.Format(@"{0:00000000000}", remetente)) != null : false;
            bool destinoFilial = destinatario > 0 ? repFilial.BuscarPorCNPJ(String.Format(@"{0:00000000000}", destinatario)) != null : false;

            List<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao> regras = repRegraEscrituracao.BuscarRegrasValidas(remetente, destinatario, origemFilial, destinoFilial);

            return regras.FirstOrDefault();
        }

        private static void SetarValoresFrete(Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao, Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioValores = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unitOfWork);
            TipoParametroBaseTabelaFrete? tipoParametroBaseTabelaFrete = repositorioValores.BuscarTipoParametroBaseComValorInformadoPorParametrosTabelaFreteDaCarga(carga.Codigo);
            TipoValorFreteDocumentoProvisao tipoValorFrete = TipoValorFreteDocumentoProvisao.NaoDefinido;

            if (tipoParametroBaseTabelaFrete == TipoParametroBaseTabelaFrete.TipoCarga)
                tipoValorFrete = TipoValorFreteDocumentoProvisao.TipoCarga;
            else if (tipoParametroBaseTabelaFrete == TipoParametroBaseTabelaFrete.Distancia)
                tipoValorFrete = TipoValorFreteDocumentoProvisao.Distancia;
            else if (tipoParametroBaseTabelaFrete == TipoParametroBaseTabelaFrete.Peso)
                tipoValorFrete = TipoValorFreteDocumentoProvisao.Peso;
            else if (tipoParametroBaseTabelaFrete == TipoParametroBaseTabelaFrete.TipoEmbalagem)
                tipoValorFrete = TipoValorFreteDocumentoProvisao.TipoEmbalagem;

            if (tipoValorFrete == TipoValorFreteDocumentoProvisao.NaoDefinido)
                return;

            documentoProvisao.TipoValorFrete = tipoValorFrete;
            documentoProvisao.ValorFrete = valorFrete;
        }

        private static void SetarValoresDocumentoProvisaoOcorrecia(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, ref Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao, ref decimal valorProvisao, ref decimal baseCalculoICMS, ref decimal valorICMS, ref decimal valorISS, ref decimal baseCalculoISS, ref decimal valorRetencaoISS, int quantidade, bool ultimoRegistro)
        {
            decimal valorProvisaoRateado = Math.Round((cargaCTeComplementoInfo.ValorComplemento / quantidade), 2, MidpointRounding.AwayFromZero);
            decimal baseCalculoICMSRateado = Math.Round((cargaCTeComplementoInfo.BaseCalculoICMS / quantidade), 2, MidpointRounding.AwayFromZero);
            decimal valorICMSRateado = Math.Round((cargaCTeComplementoInfo.ValorICMS / quantidade), 2, MidpointRounding.AwayFromZero);
            decimal valorISSRateado = Math.Round((cargaCTeComplementoInfo.ValorISS / quantidade), 2, MidpointRounding.AwayFromZero);
            decimal baseCalculoISSRateado = Math.Round((cargaCTeComplementoInfo.BaseCalculoISS / quantidade), 2, MidpointRounding.AwayFromZero);
            decimal valorRetencaoISSRateado = Math.Round((cargaCTeComplementoInfo.ValorRetencaoISS / quantidade), 2, MidpointRounding.AwayFromZero);

            valorProvisao += valorProvisaoRateado;
            baseCalculoICMS += baseCalculoICMSRateado;
            valorICMS += valorICMSRateado;
            valorISS += valorISSRateado;
            baseCalculoISS += baseCalculoISSRateado;
            valorRetencaoISS += valorRetencaoISSRateado;

            if (ultimoRegistro)
            {
                decimal diferencaProvisao = cargaCTeComplementoInfo.ValorComplemento - valorProvisao;
                valorProvisaoRateado += diferencaProvisao;

                decimal diferencaBCICMS = cargaCTeComplementoInfo.BaseCalculoICMS - baseCalculoICMS;
                baseCalculoICMSRateado += diferencaBCICMS;

                decimal diferencaICMS = cargaCTeComplementoInfo.ValorICMS - valorICMS;
                valorICMSRateado += diferencaICMS;

                decimal diferencaISS = cargaCTeComplementoInfo.ValorISS - valorISS;
                valorISSRateado += diferencaISS;

                decimal diferencaBCISS = cargaCTeComplementoInfo.BaseCalculoISS - baseCalculoISS;
                baseCalculoISSRateado += diferencaBCISS;

                decimal diferencaRetencaoISS = cargaCTeComplementoInfo.ValorRetencaoISS - valorRetencaoISS;
                valorRetencaoISSRateado += diferencaRetencaoISS;
            }

            documentoProvisao.ValorProvisaoCompetencia = valorProvisaoRateado;
            documentoProvisao.BaseCalculoICMSCompetencia = baseCalculoICMSRateado;
            documentoProvisao.PercentualAliquotaCompetencia = cargaCTeComplementoInfo.PercentualAliquota;
            documentoProvisao.ValorICMSCompetencia = valorICMSRateado;
            documentoProvisao.CSTCompetencia = cargaCTeComplementoInfo.CST;
            documentoProvisao.ValorISSCompetencia = valorISSRateado;
            documentoProvisao.BaseCalculoISSCompetencia = baseCalculoISSRateado;
            documentoProvisao.PercentualAliquotaISSCompetencia = cargaCTeComplementoInfo.PercentualAliquotaISS;
            documentoProvisao.ValorRetencaoISSCompetencia = valorRetencaoISSRateado;
            documentoProvisao.ICMSInclusoBCCompetencia = cargaCTeComplementoInfo.IncluirICMSFrete;
            documentoProvisao.ISSInclusoBCCompetencia = cargaCTeComplementoInfo.IncluirISSBaseCalculo;

            if (documentoProvisao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao)
            {
                documentoProvisao.ValorProvisao = documentoProvisao.ValorProvisaoCompetencia;
                documentoProvisao.BaseCalculoICMS = documentoProvisao.BaseCalculoICMSCompetencia;
                documentoProvisao.PercentualAliquota = documentoProvisao.PercentualAliquotaCompetencia;
                documentoProvisao.ValorICMS = documentoProvisao.ValorICMSCompetencia;
                documentoProvisao.CST = documentoProvisao.CSTCompetencia;
                documentoProvisao.ValorISS = documentoProvisao.ValorISSCompetencia;
                documentoProvisao.BaseCalculoISS = documentoProvisao.BaseCalculoISSCompetencia;
                documentoProvisao.ICMSInclusoBC = documentoProvisao.ICMSInclusoBCCompetencia;
                documentoProvisao.ISSInclusoBC = documentoProvisao.ISSInclusoBCCompetencia;
                documentoProvisao.PercentualAliquotaISS = documentoProvisao.PercentualAliquotaISSCompetencia;
                documentoProvisao.ValorRetencaoISS = documentoProvisao.ValorRetencaoISSCompetencia;
            }

            documentoProvisao.OutrasAliquotas = cargaCTeComplementoInfo.OutrasAliquotas;
            documentoProvisao.CSTIBSCBS = cargaCTeComplementoInfo.CSTIBSCBS;
            documentoProvisao.ClassificacaoTributariaIBSCBS = cargaCTeComplementoInfo.ClassificacaoTributariaIBSCBS;
            documentoProvisao.BaseCalculoIBSCBS = cargaCTeComplementoInfo.BaseCalculoIBSCBS;
            documentoProvisao.AliquotaIBSEstadual = cargaCTeComplementoInfo.AliquotaIBSEstadual;
            documentoProvisao.PercentualReducaoIBSEstadual = cargaCTeComplementoInfo.PercentualReducaoIBSEstadual;
            documentoProvisao.ValorIBSEstadual = cargaCTeComplementoInfo.ValorIBSEstadual;
            documentoProvisao.AliquotaIBSMunicipal = cargaCTeComplementoInfo.AliquotaIBSMunicipal;
            documentoProvisao.PercentualReducaoIBSMunicipal = cargaCTeComplementoInfo.PercentualReducaoIBSMunicipal;
            documentoProvisao.ValorIBSMunicipal = cargaCTeComplementoInfo.ValorIBSMunicipal;
            documentoProvisao.AliquotaCBS = cargaCTeComplementoInfo.AliquotaCBS;
            documentoProvisao.PercentualReducaoCBS = cargaCTeComplementoInfo.PercentualReducaoCBS;
            documentoProvisao.ValorCBS = cargaCTeComplementoInfo.ValorCBS;

            Servicos.Log.GravarInfo($"Valor de provisão calculado: {documentoProvisao.ValorProvisao.ToString()}", "LogDocumentoProvisaoOcorrencia");
        }

        public static void GravarLogImpostosProvisao(Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao, string chave, [System.Runtime.CompilerServices.CallerMemberName] string metodoChamou = "")
        {
            Servicos.Log.GravarInfo($"{metodoChamou} - {chave}, issInclusoBC = {documentoProvisao.ISSInclusoBCCompetencia}, valorISS = {documentoProvisao.ValorISSCompetencia}, situacao = {(int)documentoProvisao.Situacao}", "AdicionarDocumentoProvisao");
        }

        private static void ZerarValoresISSProvisaoParaNFSEManual(Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao)
        {
            documentoProvisao.ValorISSCompetencia = 0;
            documentoProvisao.BaseCalculoISSCompetencia = 0;
            documentoProvisao.PercentualAliquotaISSCompetencia = 0;
            documentoProvisao.ValorRetencaoISSCompetencia = 0;
            documentoProvisao.ISSInclusoBCCompetencia = false;
        }
    }
}
