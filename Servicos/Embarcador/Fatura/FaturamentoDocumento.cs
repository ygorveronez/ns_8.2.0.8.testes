using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Fatura
{
    public static class FaturamentoDocumento
    {
        public static void GerarControleFaturamentoPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = new Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento();

            documentoFaturamento.Carga = carga;
            documentoFaturamento.TipoDocumento = TipoDocumentoFaturamento.Carga;
            documentoFaturamento.ValorLiquido = repCargaCTe.BuscarValorFreteLiquidoPorCarga(carga.Codigo, "A");
            documentoFaturamento.TipoOperacao = carga.TipoOperacao;
            documentoFaturamento.TipoCarga = carga.TipoDeCarga;
            documentoFaturamento.ValorICMS = repCargaCTe.BuscarValorICMSPorCarga(carga.Codigo, "A");
            documentoFaturamento.ValorISS = repCargaCTe.BuscarValorISSPorCarga(carga.Codigo, "A");
            documentoFaturamento.AliquotaICMS = repCargaCTe.BuscarAliquotaICMSPorCarga(carga.Codigo, "A");
            documentoFaturamento.AliquotaISS = repCargaCTe.BuscarAliquotaISSPorCarga(carga.Codigo, "A");
            documentoFaturamento.PercentualRetencaoISS = repCargaCTe.BuscarPercentualRetencaoISSPorCarga(carga.Codigo, "A");
            documentoFaturamento.ValorDocumento = repCargaCTe.BuscarValorTotalReceberPorCarga(carga.Codigo, "A");
            documentoFaturamento.Situacao = SituacaoDocumentoFaturamento.Autorizado;
            documentoFaturamento.ValorEmFatura = repTitulo.ObterValorPorCarga(carga.Codigo);
            documentoFaturamento.ValorAFaturar = documentoFaturamento.ValorDocumento - documentoFaturamento.ValorEmFatura;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

            documentoFaturamento.Tomador = cargaPedido.ObterTomador();

            if (documentoFaturamento.Tomador != null)
                documentoFaturamento.GrupoPessoas = documentoFaturamento.Tomador.GrupoPessoas;

            if (documentoFaturamento.GrupoPessoas == null)
                documentoFaturamento.GrupoPessoas = carga.GrupoPessoaPrincipal ?? cargaPedido.Pedido.GrupoPessoas;

            documentoFaturamento.Filial = carga.Filial;
            documentoFaturamento.Numero = carga.CodigoCargaEmbarcador;
            documentoFaturamento.NumeroCarga = carga.CodigoCargaEmbarcador;
            documentoFaturamento.Remetente = cargaPedido.Pedido.Remetente;
            documentoFaturamento.Expedidor = cargaPedido.Expedidor;
            documentoFaturamento.Recebedor = cargaPedido.Recebedor;
            documentoFaturamento.Destinatario = cargaPedido.Pedido.Destinatario;
            //documentoFaturamento.Remetente = cargaPedido.Expedidor != null ? cargaPedido.Expedidor : cargaPedido.Pedido.Remetente;
            //documentoFaturamento.Destinatario = cargaPedido.Recebedor != null ? cargaPedido.Recebedor : cargaPedido.Pedido.Destinatario;
            documentoFaturamento.DataEmissao = repCargaCTe.BuscarUltimaDataEmissaoPorCarga(carga.Codigo, "A");
            documentoFaturamento.DataLiberacaoPagamento = documentoFaturamento.DataEmissao;
            documentoFaturamento.DataAutorizacao = repCargaCTe.BuscarUltimaDataAutorizacaoPorCarga(carga.Codigo, "A");
            documentoFaturamento.DataVinculoCarga = repCargaCTe.BuscarUltimaDataVinculoCargaPorCarga(carga.Codigo, "A");
            documentoFaturamento.SistemaEmissor = repCargaCTe.BuscarUltimoSistemaEmissorPorCarga(carga.Codigo, "A");

            documentoFaturamento.Moeda = repCargaCTe.BuscarMoedaPorCarga(carga.Codigo, "A");
            documentoFaturamento.ValorCotacaoMoeda = repCargaCTe.BuscarCotacaoMoedaPorCarga(carga.Codigo, "A");
            documentoFaturamento.ValorTotalMoeda = repCargaCTe.BuscarValorTotalMoedaPorCarga(carga.Codigo, "A");

            documentoFaturamento.CST = cargaPedido.CST;
            documentoFaturamento.CargaPagamento = carga;

            if ((documentoFaturamento.Tomador.ProvisionarDocumentos || (documentoFaturamento.GrupoPessoas != null && documentoFaturamento.GrupoPessoas.ProvisionarDocumentos)) && (!documentoFaturamento.Tomador.DataViradaProvisao.HasValue || documentoFaturamento.Tomador.DataViradaProvisao.Value < carga.DataCriacaoCarga))
                documentoFaturamento.TipoLiquidacao = TipoLiquidacao.PagamentoTransportador;
            else
                documentoFaturamento.TipoLiquidacao = TipoLiquidacao.Fatura;

            PreencherVeiculosMotoristas(ref documentoFaturamento, carga);

            documentoFaturamento.Origem = cargaPedido.Origem;
            documentoFaturamento.Destino = cargaPedido.Destino;
            documentoFaturamento.Empresa = carga.Empresa;

            documentoFaturamento.FaturamentoPermissaoExclusiva = false;
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordo = null;
            if (documentoFaturamento.GrupoPessoas != null)
                acordo = repAcordoFaturamento.BuscarAcordoCliente(0, documentoFaturamento.GrupoPessoas.Codigo);
            if (acordo == null && documentoFaturamento.Tomador != null)
                acordo = repAcordoFaturamento.BuscarAcordoCliente(documentoFaturamento.Tomador.CPF_CNPJ, 0);
            if (acordo != null)
            {
                if (acordo.FaturamentoPermissaoExclusiva || acordo.FaturamentoPermissaoExclusivaCabotagem || acordo.FaturamentoPermissaoExclusivaCustoExtra || acordo.FaturamentoPermissaoExclusivaLongoCurso)
                    documentoFaturamento.FaturamentoPermissaoExclusiva = true;

                if (carga.TipoOperacao != null && carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado)
                {
                    if (carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                        documentoFaturamento.FaturamentoPermissaoExclusiva = acordo.FaturamentoPermissaoExclusivaLongoCurso;
                    if (carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                        documentoFaturamento.FaturamentoPermissaoExclusiva = acordo.FaturamentoPermissaoExclusivaCustoExtra;
                }
            }

            PreencherNumerosPedidos(ref documentoFaturamento, carga, null, unidadeTrabalho);

            repDocumentoFaturamento.Inserir(documentoFaturamento);
        }

        public static void GerarControleFaturamentoPorDocumento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, bool provisaoPorNotaFiscal, bool cteFilialEmissora, bool pagamentoLiberado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unidadeTrabalho, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Log.GravarInfo($"GerarControleFaturamentoPorDocumento inserindo documento faturamento - Carga {carga?.Codigo ?? 0} -  CTe {cte?.Codigo ?? 0}", "ControleDocumentoFaturamento");

            GerarControleFaturamentoPorDocumento(carga, cte, cargaOcorrencia, lancamentoNFSManual, fechamentoFrete, cargaCTeAgrupado, provisaoPorNotaFiscal, cteFilialEmissora, SituacaoLiberacaoEscrituracaoPagamentoCarga.NaoInformada, pagamentoLiberado, configuracao, unidadeTrabalho, tipoServicoMultisoftware);
        }

        public static Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento GerarControleFaturamentoPorDocumento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, bool provisaoPorNotaFiscal, bool cteFilialEmissora, SituacaoLiberacaoEscrituracaoPagamentoCarga situacaoLiberacaoEscrituracaoPagamentoCarga, bool pagamentoLiberado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unidadeTrabalho, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Log.GravarInfo($"Inserindo documento faturamento - Carga {carga?.Codigo ?? 0} - CTe {cte?.Codigo ?? 0}", "ControleDocumentoFaturamento");

            if (cte.ModeloDocumentoFiscal.NaoGerarFaturamento)
            {
                if (!(cte.TomadorPagador?.Cliente.DisponibilizarDocumentosParaPagamento ?? false) && !(cte.TomadorPagador?.Cliente.GrupoPessoas?.DisponibilizarDocumentosParaPagamento ?? false) && !(carga?.TipoOperacao?.DisponibilizarDocumentosParaPagamento ?? false))
                    return null;
            }

            if (carga?.CargaSVM ?? false)
                return null;

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.NaoGerarFaturamento)
                return null;

            if (cte != null && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao)
                return null;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = repDocumentoOriginario.BuscarPrimeiroPorCTe(cte.Codigo);

            //todo: regra temporaria para recriar arquivos antigos das ocorrencias Danone, acho que é uma regra que podemos manter para tudo para evitar duplicidade.
            if (cargaOcorrencia != null)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoExiste = repDocumentoFaturamento.BuscarPorCTe(cte.Codigo);
                if (documentoFaturamentoExiste != null)
                    return null;
            }
            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            {
                if (cargaOcorrencia == null && carga != null)
                {
                    documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(carga.Codigo);
                    if (documentoFaturamento != null)
                        return documentoFaturamento;
                }

                documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(cte.Codigo);
                if (documentoFaturamento != null)
                    return documentoFaturamento;
            }

            documentoFaturamento = new Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento();
            documentoFaturamento.NumeroCarga = fechamentoFrete == null ? carga?.CodigoCargaEmbarcador : "";
            documentoFaturamento.NumeroOcorrencia = cargaOcorrencia?.NumeroOcorrencia;
            documentoFaturamento.NumeroFechamento = fechamentoFrete?.Numero;

            documentoFaturamento.AguardandoAutorizacao = !situacaoLiberacaoEscrituracaoPagamentoCarga.IsLiberada();
            documentoFaturamento.CTe = cte;
            documentoFaturamento.TipoDocumento = TipoDocumentoFaturamento.CTe;
            documentoFaturamento.ValorDocumento = cte.ValorAReceber;
            documentoFaturamento.ValorLiquido = cte.ValorFrete;
            documentoFaturamento.ValorICMS = cte.ValorICMS;
            documentoFaturamento.ValorISS = cte.ValorISS;
            documentoFaturamento.AliquotaICMS = cte.AliquotaICMS;
            documentoFaturamento.AliquotaISS = cte.AliquotaISS;
            documentoFaturamento.PercentualRetencaoISS = cte.PercentualISSRetido;
            documentoFaturamento.TipoCarga = carga?.TipoDeCarga ?? cargaOcorrencia?.Carga?.TipoDeCarga;
            documentoFaturamento.TipoOperacao = carga?.TipoOperacao ?? (cargaOcorrencia?.Carga?.TipoOperacao ?? lancamentoNFSManual?.TipoOperacao);
            documentoFaturamento.Situacao = SituacaoDocumentoFaturamento.Autorizado;
            documentoFaturamento.ValorEmFatura = repTitulo.ObterValorPorCTe(cte.Codigo);
            documentoFaturamento.ValorAFaturar = documentoFaturamento.ValorDocumento - documentoFaturamento.ValorEmFatura;
            documentoFaturamento.Tomador = cte.TomadorPagador?.Cliente;
            documentoFaturamento.GrupoPessoas = cte.TomadorPagador?.Cliente?.GrupoPessoas;
            documentoFaturamento.ProvisaoPorNotaFiscal = provisaoPorNotaFiscal;

            documentoFaturamento.Empresa = cte.Empresa;
            documentoFaturamento.Filial = carga?.Filial ?? lancamentoNFSManual?.Filial;
            documentoFaturamento.Numero = cte.Numero.ToString();
            documentoFaturamento.EmpresaSerie = cte.Serie;
            documentoFaturamento.DataEmissao = cte.DataEmissao.Value;
            documentoFaturamento.DataLiberacaoPagamento = documentoFaturamento.DataEmissao;
            documentoFaturamento.DataAutorizacao = cte.DataAutorizacao.HasValue ? cte.DataAutorizacao.Value : cte.DataEmissao.Value;
            documentoFaturamento.ModeloDocumentoFiscal = cte.ModeloDocumentoFiscal;
            documentoFaturamento.Remetente = cte.Remetente?.Cliente;
            documentoFaturamento.Expedidor = cte.Expedidor?.Cliente;
            documentoFaturamento.Recebedor = cte.Recebedor?.Cliente;
            documentoFaturamento.Destinatario = cte.Destinatario?.Cliente;
            //documentoFaturamento.Remetente = cte.Expedidor != null ? cte.Expedidor.Cliente : cte.Remetente.Cliente;
            //documentoFaturamento.Destinatario = cte.Recebedor != null ? cte.Recebedor.Cliente : cte.Destinatario.Cliente;
            documentoFaturamento.NumeroDocumentoOriginario = documentoOriginario?.Numero;
            documentoFaturamento.DataEmissaoDocumentoOriginario = documentoOriginario?.DataEmissao;
            documentoFaturamento.DataVinculoCarga = repCargaCTe.BuscarUltimaDataVinculoCargaPorCTe(cte.Codigo);
            documentoFaturamento.SistemaEmissor = repCargaCTe.BuscarUltimoSistemaEmissorPorCTe(cte.Codigo);
            documentoFaturamento.TipoCTE = cte.TipoCTE;
            documentoFaturamento.NumeroControle = cte.NumeroControle;
            documentoFaturamento.NumeroFiscal = cte.NumeroNotas;

            documentoFaturamento.CST = cte.CST;
            documentoFaturamento.CargaPagamento = fechamentoFrete == null ? carga : null;
            documentoFaturamento.CargaOcorrenciaPagamento = cargaOcorrencia;
            documentoFaturamento.FechamentoFrete = fechamentoFrete;
            documentoFaturamento.LancamentoNFSManual = lancamentoNFSManual;

            documentoFaturamento.PagamentoDocumentoBloqueado = configuracao.GerarPagamentoBloqueado;
            if (documentoFaturamento.PagamentoDocumentoBloqueado)
            {
                if (cargaOcorrencia != null)
                    documentoFaturamento.LiberarPagamentoAutomaticamente = true;
                else if (carga?.TipoOperacao?.LiberarAutomaticamentePagamento ?? false)
                    documentoFaturamento.LiberarPagamentoAutomaticamente = true;
                else if (carga?.Filial?.LiberarAutomaticamentePagamento ?? false)
                    documentoFaturamento.LiberarPagamentoAutomaticamente = true;
                else if (fechamentoFrete != null)
                    documentoFaturamento.LiberarPagamentoAutomaticamente = true;
            }

            if ((documentoFaturamento.LiberarPagamentoAutomaticamente && configuracao.GerarSomenteDocumentosDesbloqueados) || pagamentoLiberado)
                documentoFaturamento.PagamentoDocumentoBloqueado = false;

            documentoFaturamento.Moeda = cte.Moeda;
            documentoFaturamento.ValorTotalMoeda = cte.ValorTotalMoeda;
            documentoFaturamento.ValorCotacaoMoeda = cte.ValorCotacaoMoeda;

            documentoFaturamento.FaturamentoPermissaoExclusiva = false;
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordo = null;
            if (documentoFaturamento.GrupoPessoas != null)
                acordo = repAcordoFaturamento.BuscarAcordoCliente(0, documentoFaturamento.GrupoPessoas.Codigo);
            if (acordo == null && documentoFaturamento.Tomador != null)
                acordo = repAcordoFaturamento.BuscarAcordoCliente(documentoFaturamento.Tomador.CPF_CNPJ, 0);
            if (acordo != null)
            {
                if (acordo.FaturamentoPermissaoExclusiva || acordo.FaturamentoPermissaoExclusivaCabotagem || acordo.FaturamentoPermissaoExclusivaCustoExtra || acordo.FaturamentoPermissaoExclusivaLongoCurso)
                    documentoFaturamento.FaturamentoPermissaoExclusiva = true;

                if (carga?.TipoOperacao != null && carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado)
                {
                    if (carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                        documentoFaturamento.FaturamentoPermissaoExclusiva = acordo.FaturamentoPermissaoExclusivaLongoCurso;
                    if (carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                        documentoFaturamento.FaturamentoPermissaoExclusiva = acordo.FaturamentoPermissaoExclusivaCustoExtra;
                }
            }

            DateTime data = fechamentoFrete == null ? (carga?.DataCriacaoCarga ?? DateTime.Now) : fechamentoFrete.DataFechamento;
            if (cargaOcorrencia != null)
                data = cargaOcorrencia.DataOcorrencia;

            if ((!cteFilialEmissora && (((documentoFaturamento.Tomador?.ProvisionarDocumentos ?? false) || (documentoFaturamento.GrupoPessoas != null && documentoFaturamento.GrupoPessoas.ProvisionarDocumentos)) && ((!documentoFaturamento.Tomador?.DataViradaProvisao.HasValue ?? false) || documentoFaturamento.Tomador?.DataViradaProvisao.Value < data)))) //|| provisaoPorNotaFiscal)
                documentoFaturamento.TipoLiquidacao = TipoLiquidacao.PagamentoTransportador;
            else
                documentoFaturamento.TipoLiquidacao = TipoLiquidacao.Fatura;
            if (fechamentoFrete == null)
                PreencherVeiculosMotoristas(ref documentoFaturamento, carga);

            documentoFaturamento.Origem = cte.LocalidadeInicioPrestacao;
            documentoFaturamento.Destino = cte.LocalidadeTerminoPrestacao;
            documentoFaturamento.Titulo = repTituloDocumento.BuscarTituloPorCTe(cte.Codigo);

            if (fechamentoFrete == null)
                PreencherNumerosPedidos(ref documentoFaturamento, carga, cargaOcorrencia, unidadeTrabalho);

            try
            {
                repDocumentoFaturamento.Inserir(documentoFaturamento);
                Servicos.Log.TratarErro($"Inseriu documento faturamento - CTe {cte?.Codigo ?? 0}", "ControleDocumentoFaturamento");
            }
            catch (Exception excecao)
            {
                if (excecao.InnerException != null && (excecao.InnerException is System.Data.SqlClient.SqlException sqlException) && sqlException.Number == 2627 && excecao.InnerException.Message.Contains("UK_DOCUMENTO_FATURAMENTO_CTE"))
                {
                    Servicos.Log.TratarErro($"Cancelou insert do documento faturamento - CTe {cte?.Codigo ?? 0} - UK_DOCUMENTO_FATURAMENTO_CTE", "ControleDocumentoFaturamento");
                    return null;
                }
                else
                    throw;
            }
            return documentoFaturamento;
        }

        public static void GerarDocumentoFaturamentoPorControleDocumento(Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento, decimal valorDeGeracao, Repositorio.UnitOfWork unitOfWork, bool validarExistencia = false)
        {
            if (documento.Carga == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = documento.CargaCTe.Carga;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = documento.CTe;

            if (cte.ModeloDocumentoFiscal.NaoGerarFaturamento)
            {
                if (!(cte.TomadorPagador?.Cliente.DisponibilizarDocumentosParaPagamento ?? false) && !(cte.TomadorPagador?.Cliente.GrupoPessoas?.DisponibilizarDocumentosParaPagamento ?? false) && !(carga?.TipoOperacao?.DisponibilizarDocumentosParaPagamento ?? false))
                    return;
            }

            if ((carga != null && carga.TipoOperacao != null && carga.TipoOperacao.NaoGerarFaturamento) || (carga?.CargaSVM ?? false) || cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao)
                return;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = repDocumentoOriginario.BuscarPrimeiroPorCTe(cte.Codigo);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = validarExistencia ? repDocumentoFaturamento.BuscarPorCTe(cte.Codigo) : null;

            if (documentoFaturamento != null)
                return;

            documentoFaturamento = new Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento();
            documentoFaturamento.NumeroCarga = carga?.CodigoCargaEmbarcador ?? "";
            documentoFaturamento.NumeroOcorrencia = 0;
            documentoFaturamento.NumeroFechamento = 0;

            documentoFaturamento.AguardandoAutorizacao = !SituacaoLiberacaoEscrituracaoPagamentoCarga.NaoInformada.IsLiberada();
            documentoFaturamento.CTe = cte;
            documentoFaturamento.TipoDocumento = TipoDocumentoFaturamento.CTe;
            documentoFaturamento.ValorDocumento = valorDeGeracao;
            documentoFaturamento.ValorLiquido = cte.ValorFrete;
            documentoFaturamento.ValorICMS = cte.ValorICMS;
            documentoFaturamento.ValorISS = cte.ValorISS;
            documentoFaturamento.AliquotaICMS = cte.AliquotaICMS;
            documentoFaturamento.AliquotaISS = cte.AliquotaISS;
            documentoFaturamento.PercentualRetencaoISS = cte.PercentualISSRetido;
            documentoFaturamento.TipoCarga = carga?.TipoDeCarga ?? null;
            documentoFaturamento.TipoOperacao = carga?.TipoOperacao ?? null;
            documentoFaturamento.Situacao = SituacaoDocumentoFaturamento.Autorizado;
            documentoFaturamento.ValorEmFatura = repTitulo.ObterValorPorCTe(cte.Codigo);
            documentoFaturamento.ValorAFaturar = documentoFaturamento.ValorDocumento - documentoFaturamento.ValorEmFatura;
            documentoFaturamento.Tomador = cte.TomadorPagador?.Cliente;
            documentoFaturamento.GrupoPessoas = cte.TomadorPagador?.Cliente?.GrupoPessoas;
            documentoFaturamento.ProvisaoPorNotaFiscal = false; //Preciso ver sobre 

            documentoFaturamento.Empresa = cte.Empresa;
            documentoFaturamento.Filial = carga?.Filial ?? null;
            documentoFaturamento.Numero = cte.Numero.ToString();
            documentoFaturamento.EmpresaSerie = cte.Serie;
            documentoFaturamento.DataEmissao = cte.DataEmissao.Value;
            documentoFaturamento.DataLiberacaoPagamento = documentoFaturamento.DataEmissao;
            documentoFaturamento.DataAutorizacao = cte.DataAutorizacao.HasValue ? cte.DataAutorizacao.Value : cte.DataEmissao.Value;
            documentoFaturamento.ModeloDocumentoFiscal = cte.ModeloDocumentoFiscal;
            documentoFaturamento.Remetente = cte.Remetente?.Cliente;
            documentoFaturamento.Expedidor = cte.Expedidor?.Cliente;
            documentoFaturamento.Recebedor = cte.Recebedor?.Cliente;
            documentoFaturamento.Destinatario = cte.Destinatario?.Cliente;
            documentoFaturamento.NumeroDocumentoOriginario = documentoOriginario?.Numero;
            documentoFaturamento.DataEmissaoDocumentoOriginario = documentoOriginario?.DataEmissao;
            documentoFaturamento.DataVinculoCarga = repCargaCTe.BuscarUltimaDataVinculoCargaPorCTe(cte.Codigo);
            documentoFaturamento.SistemaEmissor = repCargaCTe.BuscarUltimoSistemaEmissorPorCTe(cte.Codigo);
            documentoFaturamento.TipoCTE = cte.TipoCTE;
            documentoFaturamento.NumeroControle = cte.NumeroControle;
            documentoFaturamento.NumeroFiscal = cte.NumeroNotas;

            documentoFaturamento.CST = cte.CST;
            documentoFaturamento.CargaPagamento = carga;
            documentoFaturamento.CargaOcorrenciaPagamento = null;
            documentoFaturamento.FechamentoFrete = null;
            documentoFaturamento.LancamentoNFSManual = null;

            documentoFaturamento.PagamentoDocumentoBloqueado = configuracaoFinanceiro.GerarPagamentoBloqueado;
            if (documentoFaturamento.PagamentoDocumentoBloqueado)
            {
                if (carga?.TipoOperacao?.LiberarAutomaticamentePagamento ?? false)
                    documentoFaturamento.LiberarPagamentoAutomaticamente = true;
                else if (carga?.Filial?.LiberarAutomaticamentePagamento ?? false)
                    documentoFaturamento.LiberarPagamentoAutomaticamente = true;
            }

            if ((documentoFaturamento.LiberarPagamentoAutomaticamente && configuracaoFinanceiro.GerarSomenteDocumentosDesbloqueados) || false)
                documentoFaturamento.PagamentoDocumentoBloqueado = false;

            documentoFaturamento.Moeda = cte.Moeda;
            documentoFaturamento.ValorTotalMoeda = cte.ValorTotalMoeda;
            documentoFaturamento.ValorCotacaoMoeda = cte.ValorCotacaoMoeda;

            documentoFaturamento.FaturamentoPermissaoExclusiva = false;
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordo = null;
            if (documentoFaturamento.GrupoPessoas != null)
                acordo = repAcordoFaturamento.BuscarAcordoCliente(0, documentoFaturamento.GrupoPessoas.Codigo);
            if (acordo == null && documentoFaturamento.Tomador != null)
                acordo = repAcordoFaturamento.BuscarAcordoCliente(documentoFaturamento.Tomador.CPF_CNPJ, 0);
            if (acordo != null)
            {
                if (acordo.FaturamentoPermissaoExclusiva || acordo.FaturamentoPermissaoExclusivaCabotagem || acordo.FaturamentoPermissaoExclusivaCustoExtra || acordo.FaturamentoPermissaoExclusivaLongoCurso)
                    documentoFaturamento.FaturamentoPermissaoExclusiva = true;

                if (carga.TipoOperacao != null && carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado)
                {
                    if (carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                        documentoFaturamento.FaturamentoPermissaoExclusiva = acordo.FaturamentoPermissaoExclusivaLongoCurso;
                    if (carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                        documentoFaturamento.FaturamentoPermissaoExclusiva = acordo.FaturamentoPermissaoExclusivaCustoExtra;
                }
            }

            DateTime data = (carga?.DataCriacaoCarga ?? DateTime.Now);

            if ((((documentoFaturamento.Tomador?.ProvisionarDocumentos ?? false) || (documentoFaturamento.GrupoPessoas != null && documentoFaturamento.GrupoPessoas.ProvisionarDocumentos)) && ((!documentoFaturamento.Tomador?.DataViradaProvisao.HasValue ?? false) || documentoFaturamento.Tomador?.DataViradaProvisao.Value < data))) //|| provisaoPorNotaFiscal)
                documentoFaturamento.TipoLiquidacao = TipoLiquidacao.PagamentoTransportador;
            else
                documentoFaturamento.TipoLiquidacao = TipoLiquidacao.Fatura;

            PreencherVeiculosMotoristas(ref documentoFaturamento, carga);

            documentoFaturamento.Origem = cte.LocalidadeInicioPrestacao;
            documentoFaturamento.Destino = cte.LocalidadeTerminoPrestacao;
            documentoFaturamento.Titulo = repTituloDocumento.BuscarTituloPorCTe(cte.Codigo);

            repDocumentoFaturamento.Inserir(documentoFaturamento);
            return;
        }

        private static void PreencherVeiculosMotoristas(ref Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return;

            documentoFaturamento.Veiculos = new List<Dominio.Entidades.Veiculo>();
            documentoFaturamento.Motoristas = new List<Dominio.Entidades.Usuario>();

            if (carga.Veiculo != null)
                documentoFaturamento.Veiculos.Add(carga.Veiculo);

            foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                documentoFaturamento.Veiculos.Add(reboque);

            foreach (Dominio.Entidades.Usuario motorista in carga.Motoristas)
                documentoFaturamento.Motoristas.Add(motorista);
        }

        private static void PreencherNumerosPedidos(ref Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            List<string> numerosPedidos = new List<string>();
            if (documentoFaturamento.CTe != null)
                numerosPedidos = repCargaPedidoXMLNotaFiscalCTe.BuscarNumeroPedidosPorCTe(documentoFaturamento.CTe.Codigo);
            else
                numerosPedidos = repCargaPedido.BuscarNumeroPedidosPorCarga(carga.Codigo);

            documentoFaturamento.NumeroPedidoCliente = numerosPedidos.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
            documentoFaturamento.NumeroPedidoOcorrenciaCliente = new List<string>();

            if (ocorrencia != null && !string.IsNullOrWhiteSpace(ocorrencia.NumeroOcorrenciaCliente))
                documentoFaturamento.NumeroPedidoOcorrenciaCliente.Add(ocorrencia.NumeroOcorrenciaCliente);
        }
    }
}
