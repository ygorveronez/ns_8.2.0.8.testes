using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class Moeda
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Auditado _auditado;

        private const int _numeroCasasDecimais = 2;

        public Moeda(Repositorio.UnitOfWork unitOfWork, Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #region Métodos Publicos

        public bool AlterarMoedaCarga(out string erro, int codigoCarga, MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, Dominio.Entidades.Embarcador.Fatura.Fatura faturaAtual = null, List<int> codigosDocumentos = null, bool manterValorMoeda = false)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true);

            if (!ValidarAlteracaoMoedaCarga(carga, out erro, faturaAtual?.Codigo, codigosDocumentos))
                return false;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repositorioCargaComponenteFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCarga(carga.Codigo);

            _unitOfWork.Start();

            if (faturaAtual?.DataBaseCRT.HasValue ?? false)
                AtualizaDadosCargaPedido(cargaPedidos, carga, faturaAtual?.DataBaseCRT);

            if (moeda == MoedaCotacaoBancoCentral.Real)
                LimparValoresCotacaoCarga(carga, cargaComponentesFrete, cargaPedidos, cargaCTes);
            else
                AlterarValoresCotacaoCarga(carga, cargaComponentesFrete, cargaPedidos, cargaCTes, moeda, valorCotacaoMoeda, faturaAtual?.Codigo, manterValorMoeda);

            List<Dominio.Entidades.Auditoria.HistoricoPropriedade> cargaChanges = carga.GetChanges();

            repositorioCarga.Atualizar(carga);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, cargaChanges, "Alterou a moeda da carga.", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

            _unitOfWork.CommitChanges();

            erro = null;
            return true;
        }

        public bool AlterarMoedaOcorrencia(out string erro, int codigoCargaOcorrencia, MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, int? codigoFaturaAtual = null, bool manterValorMoeda = false)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repositorioCargaOcorrencia.BuscarPorCodigo(codigoCargaOcorrencia, true);

            if (!ValidarAlteracaoMoedaOcorrencia(cargaOcorrencia, out erro, codigoFaturaAtual))
                return false;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);

            _unitOfWork.Start();

            if (moeda == MoedaCotacaoBancoCentral.Real)
                LimparValoresCotacaoOcorrencia(cargaOcorrencia, cargaCTesComplementoInfo);
            else
                AlterarValoresCotacaoOcorrencia(cargaOcorrencia, cargaCTesComplementoInfo, moeda, valorCotacaoMoeda, manterValorMoeda);

            List<Dominio.Entidades.Auditoria.HistoricoPropriedade> cargaOcorrenciaChanges = cargaOcorrencia.GetChanges();

            repositorioCargaOcorrencia.Atualizar(cargaOcorrencia);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaOcorrencia, cargaOcorrenciaChanges, "Alterou a moeda da ocorrência.", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

            _unitOfWork.CommitChanges();

            erro = null;
            return true;
        }

        public bool AlterarMoedaCTeAgrupado(out string erro, int codigoCargaCTeAgrupado, MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(codigoCargaCTeAgrupado, true);

            if (cargaCTeAgrupado == null)
            {
                erro = "O CT-e agrupado não foi encontrado.";
                return false;
            }

            if (cargaCTeAgrupado.Situacao != SituacaoCargaCTeAgrupado.Finalizado)
            {
                erro = $"A situação do CT-e agrupado ({cargaCTeAgrupado.Situacao.ObterDescricao()}) não permite que a moeda seja alterada.";
                return false;
            }

            int[] codigosCTes = cargaCTeAgrupado.CTes.Select(o => o.CTe.Codigo).ToArray();

            List<int> numerosFaturasNova = repFaturaDocumento.BuscarNumeroFaturaPorCTe(codigosCTes);
            if (numerosFaturasNova.Count > 0)
            {
                erro = "Os documentos do CT-e agrupado estão vinculados à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar a alteração.";
                return false;
            }

            List<int> numerosTitulos = repTituloDocumento.BuscarNumeroTituloPorCTe(codigosCTes);
            if (numerosTitulos.Count > 0)
            {
                erro = "Os documentos do CT-e agrupado estão vinculados ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar a alteração.";
                return false;
            }

            List<int> nossoNumeroBoletoTitulos = repTituloDocumento.BuscarNumeroBoletoTituloPorCTe(codigosCTes);
            if (nossoNumeroBoletoTitulos.Count > 0)
            {
                erro = "Os documentos do CT-e agrupado estão vinculados a boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar a alteração.";
                return false;
            }

            _unitOfWork.Start();

            decimal valorTotalMoeda = 0m;

            foreach (Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe cargaCTeAgrupadoCTe in cargaCTeAgrupado.CTes)
            {
                if (moeda == MoedaCotacaoBancoCentral.Real)
                {
                    cargaCTeAgrupadoCTe.CTe.Moeda = null;
                    cargaCTeAgrupadoCTe.CTe.ValorCotacaoMoeda = null;
                    cargaCTeAgrupadoCTe.CTe.ValorTotalMoeda = null;
                }
                else
                {
                    cargaCTeAgrupadoCTe.CTe.Moeda = moeda;
                    cargaCTeAgrupadoCTe.CTe.ValorCotacaoMoeda = valorCotacaoMoeda;
                    cargaCTeAgrupadoCTe.CTe.ValorTotalMoeda = Math.Round(cargaCTeAgrupadoCTe.CTe.ValorAReceber / valorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);

                    valorTotalMoeda += cargaCTeAgrupadoCTe.CTe.ValorTotalMoeda.Value;
                }

                repCTe.Atualizar(cargaCTeAgrupadoCTe.CTe);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoCTe = repDocumentoFaturamento.BuscarPorCTe(cargaCTeAgrupadoCTe.CTe.Codigo);

                if (documentoFaturamentoCTe != null)
                {
                    if (moeda == MoedaCotacaoBancoCentral.Real)
                    {
                        documentoFaturamentoCTe.ValorTotalMoeda = null;
                        documentoFaturamentoCTe.Moeda = null;
                        documentoFaturamentoCTe.ValorCotacaoMoeda = null;
                    }
                    else
                    {
                        documentoFaturamentoCTe.ValorTotalMoeda = Math.Round(documentoFaturamentoCTe.ValorDocumento / valorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                        documentoFaturamentoCTe.Moeda = moeda;
                        documentoFaturamentoCTe.ValorCotacaoMoeda = valorCotacaoMoeda;
                    }

                    repDocumentoFaturamento.Atualizar(documentoFaturamentoCTe);
                }
            }

            if (moeda == MoedaCotacaoBancoCentral.Real)
            {
                cargaCTeAgrupado.ValorCotacaoMoeda = null;
                cargaCTeAgrupado.Moeda = null;
                cargaCTeAgrupado.ValorTotalMoeda = null;
            }
            else
            {
                cargaCTeAgrupado.ValorCotacaoMoeda = valorCotacaoMoeda;
                cargaCTeAgrupado.Moeda = moeda;
                cargaCTeAgrupado.ValorTotalMoeda = valorTotalMoeda;
            }

            List<Dominio.Entidades.Auditoria.HistoricoPropriedade> cargaCTeAgrupadoChanges = cargaCTeAgrupado.GetChanges();

            repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCTeAgrupado, cargaCTeAgrupadoChanges, "Alterou a moeda do CT-e agrupado.", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

            _unitOfWork.CommitChanges();

            erro = null;
            return true;
        }

        #endregion Métodos Publicos

        #region Métodos Privados

        #region Carga

        private bool ValidarAlteracaoMoedaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, out string erro, int? codigoFaturaAtual = null, List<int> codigosDocumentos = null)
        {
            erro = null;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repositorioFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repositorioTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);

            if (carga == null)
            {
                erro = "A carga não foi encontrada.";
                return false;
            }

            if (carga.SituacaoCarga != SituacaoCarga.Encerrada && carga.SituacaoCarga != SituacaoCarga.EmTransporte)
            {
                erro = $"A situação da carga ({carga.SituacaoCarga.ObterDescricao()}) não permite que a moeda seja alterada.";
                return false;
            }

            if (carga.CargaTransbordo)
            {
                erro = "Não é possível alterar a moeda de uma carga de transbordo.";
                return false;
            }

            bool documentoFinanceiroPorCarga = repositorioDocumentoFaturamento.ExistePorCarga(carga.Codigo);
            bool documentoFinanceiroPorCTe = repositorioDocumentoFaturamento.ExistePorCTe(carga.Codigo);

            if (documentoFinanceiroPorCarga)
            {
                List<int> numerosFaturasNova = repositorioFaturaDocumento.BuscarNumeroFaturaPorCarga(carga.Codigo, codigoFaturaAtual);
                if (numerosFaturasNova.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "A carga (" + carga.CodigoCargaEmbarcador + ") está vinculada à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar a alteração.";
                    return false;
                }

                List<int> numerosTitulos = repositorioTituloDocumento.BuscarNumeroTituloPorCarga(carga.Codigo);
                if (numerosTitulos.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "A carga (" + carga.CodigoCargaEmbarcador + ") está vinculada ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar a alteração.";
                    return false;
                }

                List<int> nossoNumeroBoletoTitulos = repositorioTituloDocumento.BuscarNumeroBoletoTituloPorCarga(carga.Codigo);
                if (nossoNumeroBoletoTitulos.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "A carga (" + carga.CodigoCargaEmbarcador + ") está vinculada a boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar a alteração.";
                    return false;
                }
            }

            if (documentoFinanceiroPorCTe)
            {
                if (codigosDocumentos == null)
                {
                    var faturaDocumento = repositorioDocumentoFaturamento.BuscarPorCTeComCodigoCarga(carga.Codigo);

                    if (faturaDocumento != null)
                        codigosDocumentos = faturaDocumento?.Fatura?.Documentos?.Select(o => o.Codigo).ToList();
                }

                List<int> numerosFaturasNova = codigosDocumentos != null ? repositorioFaturaDocumento.BuscarNumeroFaturaPorDocumento(codigosDocumentos) : new List<int>();
                if (numerosFaturasNova.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "Os documentos da carga estão vinculados à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar a alteração.";
                    return false;
                }

                List<int> numerosTitulos = repositorioTituloDocumento.BuscarNumeroTituloPorCargaCTe(carga.Codigo);
                if (numerosTitulos.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "Os documentos da carga estão vinculados ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar a alteração.";
                    return false;
                }

                List<int> nossoNumeroBoletoTitulos = repositorioTituloDocumento.BuscarNumeroBoletoTituloPorCargaCTe(carga.Codigo);
                if (nossoNumeroBoletoTitulos.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "Os documentos da carga estão vinculados a boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar a alteração.";
                    return false;
                }
            }

            return true;
        }

        private void AlterarValoresCotacaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, int? codigoFaturaAtual = null, bool manterValorMoeda = false)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento reposiotorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repositorioCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repositorioPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repositorioPedidoCTeParaSubcontratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete in cargaComponentesFrete)
            {
                if (cargaComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ISS || cargaComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS)
                    continue;

                cargaComponenteFrete.Moeda = moeda;
                cargaComponenteFrete.ValorCotacaoMoeda = valorCotacaoMoeda;

                if (manterValorMoeda)
                    cargaComponenteFrete.ValorComponente = Math.Round(cargaComponenteFrete.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                else
                    cargaComponenteFrete.ValorTotalMoeda = Math.Round(cargaComponenteFrete.ValorComponente / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);

                repositorioCargaComponenteFrete.Atualizar(cargaComponenteFrete);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                cargaPedido.Moeda = moeda;
                cargaPedido.ValorCotacaoMoeda = valorCotacaoMoeda;

                if (manterValorMoeda)
                {
                    cargaPedido.ValorFrete = Math.Round(cargaPedido.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                    cargaPedido.ValorFreteAPagar = Math.Round(cargaPedido.ValorTotalMoedaPagar.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                    cargaPedido.ValorFreteTabelaFrete = cargaPedido.ValorFreteAPagar;
                }
                else
                    cargaPedido.ValorTotalMoeda = Math.Round(cargaPedido.ValorFrete / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);

                repositorioCargaPedido.Atualizar(cargaPedido);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repositorioCargaPedidoComponenteFrete.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFretes)
                {
                    if (cargaPedidoComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS || cargaPedidoComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ISS)
                        continue;

                    cargaPedidoComponenteFrete.Moeda = moeda;
                    cargaPedidoComponenteFrete.ValorCotacaoMoeda = valorCotacaoMoeda;

                    if (manterValorMoeda)
                        cargaPedidoComponenteFrete.ValorComponente = Math.Round(cargaPedidoComponenteFrete.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                    else
                        cargaPedidoComponenteFrete.ValorTotalMoeda = Math.Round(cargaPedidoComponenteFrete.ValorComponente / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);

                    repositorioCargaPedidoComponenteFrete.Atualizar(cargaPedidoComponenteFrete);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscais)
                {
                    pedidoXMLNotaFiscal.Moeda = moeda;
                    pedidoXMLNotaFiscal.ValorCotacaoMoeda = valorCotacaoMoeda;

                    if (manterValorMoeda && pedidoXMLNotaFiscal.ValorTotalMoeda.HasValue)
                    {
                        pedidoXMLNotaFiscal.ValorFrete = Math.Round(pedidoXMLNotaFiscal.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                        pedidoXMLNotaFiscal.ValorTotalComponentes = Math.Round(pedidoXMLNotaFiscal.ValorTotalMoedaComponentes * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        pedidoXMLNotaFiscal.ValorTotalMoeda = Math.Round(pedidoXMLNotaFiscal.ValorFrete / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                        pedidoXMLNotaFiscal.ValorTotalMoedaComponentes = Math.Round(pedidoXMLNotaFiscal.ValorTotalComponentes / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                    }

                    repositorioPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalComponentesFrete = repositorioPedidoXMLNotaFiscalComponenteFrete.BuscarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete in pedidoXMLNotaFiscalComponentesFrete)
                    {
                        if (pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS || pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ISS)
                            continue;

                        pedidoXMLNotaFiscalComponenteFrete.Moeda = moeda;
                        pedidoXMLNotaFiscalComponenteFrete.ValorCotacaoMoeda = valorCotacaoMoeda;

                        if (manterValorMoeda)
                            pedidoXMLNotaFiscalComponenteFrete.ValorComponente = Math.Round(pedidoXMLNotaFiscalComponenteFrete.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                        else
                            pedidoXMLNotaFiscalComponenteFrete.ValorTotalMoeda = Math.Round(pedidoXMLNotaFiscalComponenteFrete.ValorComponente / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);

                        repositorioPedidoXMLNotaFiscalComponenteFrete.Atualizar(pedidoXMLNotaFiscalComponenteFrete);
                    }
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao = repositorioPedidoCTeParaSubcontratacao.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubcontratacao)
                {
                    pedidoCTeParaSubContratacao.Moeda = moeda;
                    pedidoCTeParaSubContratacao.ValorCotacaoMoeda = valorCotacaoMoeda;

                    if (manterValorMoeda)
                    {
                        pedidoCTeParaSubContratacao.ValorFrete = Math.Round(pedidoCTeParaSubContratacao.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                        pedidoCTeParaSubContratacao.ValorTotalComponentes = Math.Round(pedidoCTeParaSubContratacao.ValorTotalMoedaComponentes * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        pedidoCTeParaSubContratacao.ValorTotalMoeda = Math.Round(pedidoCTeParaSubContratacao.ValorFrete / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                        pedidoCTeParaSubContratacao.ValorTotalMoedaComponentes = Math.Round(pedidoCTeParaSubContratacao.ValorTotalComponentes / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                    }

                    repositorioPedidoCTeParaSubcontratacao.Atualizar(pedidoCTeParaSubContratacao);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCTeParaSubcontratacaoComponentesFrete = repositorioPedidoCTeParaSubcontratacaoComponenteFrete.BuscarPorPedidoCteParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo, false);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete in pedidoCTeParaSubcontratacaoComponentesFrete)
                    {
                        pedidoCteParaSubContratacaoComponenteFrete.Moeda = moeda;
                        pedidoCteParaSubContratacaoComponenteFrete.ValorCotacaoMoeda = valorCotacaoMoeda;

                        if (manterValorMoeda)
                            pedidoCteParaSubContratacaoComponenteFrete.ValorComponente = Math.Round(pedidoCteParaSubContratacaoComponenteFrete.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                        else
                            pedidoCteParaSubContratacaoComponenteFrete.ValorTotalMoeda = Math.Round(pedidoCteParaSubContratacaoComponenteFrete.ValorComponente / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);

                        repositorioPedidoCTeParaSubcontratacaoComponenteFrete.Atualizar(pedidoCteParaSubContratacaoComponenteFrete);
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                if (cargaCTe.CargaCTeComplementoInfo != null || cargaCTe.CargaCTeAgrupado != null)
                    continue;

                AtualizarValorCTe(cargaCTe.CTe, moeda, valorCotacaoMoeda, manterValorMoeda);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoCTe = reposiotorioDocumentoFaturamento.BuscarPorCTe(cargaCTe.CTe.Codigo);

                if (documentoFaturamentoCTe != null)
                    AtualizarValorDocumentoFaturamento(documentoFaturamentoCTe, moeda, valorCotacaoMoeda, codigoFaturaAtual, manterValorMoeda);
            }

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoCarga = reposiotorioDocumentoFaturamento.BuscarPorCarga(carga.Codigo);

            if (documentoFaturamentoCarga != null)
                AtualizarValorDocumentoFaturamento(documentoFaturamentoCarga, moeda, valorCotacaoMoeda, codigoFaturaAtual, manterValorMoeda);

            carga.ValorCotacaoMoeda = valorCotacaoMoeda;
            carga.Moeda = moeda;

            if (manterValorMoeda)
            {
                carga.ValorFrete = Math.Round(carga.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                carga.ValorFreteAPagar = Math.Round(carga.ValorTotalMoedaPagar.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
                carga.ValorFreteLiquido = carga.ValorFrete;
                carga.ValorFreteTabelaFrete = carga.ValorFreteAPagar;
            }
            else
                carga.ValorTotalMoeda = Math.Round(carga.ValorFrete / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
        }

        private void LimparValoresCotacaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repositorioCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repositorioPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repositorioPedidoCTeParaSubcontratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete in cargaComponentesFrete)
            {
                if (cargaComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ISS || cargaComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS)
                    continue;

                cargaComponenteFrete.Moeda = null;
                cargaComponenteFrete.ValorCotacaoMoeda = null;
                cargaComponenteFrete.ValorTotalMoeda = null;

                repositorioCargaComponenteFrete.Atualizar(cargaComponenteFrete);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                cargaPedido.Moeda = null;
                cargaPedido.ValorCotacaoMoeda = null;
                cargaPedido.ValorTotalMoeda = null;

                repositorioCargaPedido.Atualizar(cargaPedido);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repositorioCargaPedidoComponentesFrete.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFretes)
                {
                    if (cargaPedidoComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS || cargaPedidoComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ISS)
                        continue;

                    cargaPedidoComponenteFrete.Moeda = null;
                    cargaPedidoComponenteFrete.ValorCotacaoMoeda = null;
                    cargaPedidoComponenteFrete.ValorTotalMoeda = null;

                    repositorioCargaPedidoComponentesFrete.Atualizar(cargaPedidoComponenteFrete);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscais)
                {
                    pedidoXMLNotaFiscal.Moeda = null;
                    pedidoXMLNotaFiscal.ValorCotacaoMoeda = null;
                    pedidoXMLNotaFiscal.ValorTotalMoeda = null;
                    pedidoXMLNotaFiscal.ValorTotalMoedaComponentes = 0m;

                    repositorioPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalComponentesFrete = repositorioPedidoXMLNotaFiscalComponenteFrete.BuscarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete in pedidoXMLNotaFiscalComponentesFrete)
                    {
                        if (pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS || pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ISS)
                            continue;

                        pedidoXMLNotaFiscalComponenteFrete.Moeda = null;
                        pedidoXMLNotaFiscalComponenteFrete.ValorCotacaoMoeda = null;
                        pedidoXMLNotaFiscalComponenteFrete.ValorTotalMoeda = null;

                        repositorioPedidoXMLNotaFiscalComponenteFrete.Atualizar(pedidoXMLNotaFiscalComponenteFrete);
                    }
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao = repositorioPedidoCTeParaSubcontratacao.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubcontratacao)
                {
                    pedidoCTeParaSubContratacao.Moeda = null;
                    pedidoCTeParaSubContratacao.ValorCotacaoMoeda = null;
                    pedidoCTeParaSubContratacao.ValorTotalMoeda = null;
                    pedidoCTeParaSubContratacao.ValorTotalMoedaComponentes = 0m;

                    repositorioPedidoCTeParaSubcontratacao.Atualizar(pedidoCTeParaSubContratacao);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCTeParaSubcontratacaoComponentesFrete = repositorioPedidoCTeParaSubcontratacaoComponenteFrete.BuscarPorPedidoCteParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo, false);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete in pedidoCTeParaSubcontratacaoComponentesFrete)
                    {
                        pedidoCteParaSubContratacaoComponenteFrete.Moeda = null;
                        pedidoCteParaSubContratacaoComponenteFrete.ValorCotacaoMoeda = null;
                        pedidoCteParaSubContratacaoComponenteFrete.ValorTotalMoeda = null;

                        repositorioPedidoCTeParaSubcontratacaoComponenteFrete.Atualizar(pedidoCteParaSubContratacaoComponenteFrete);
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                if (cargaCTe.CargaCTeComplementoInfo != null || cargaCTe.CargaCTeAgrupado != null)
                    continue;

                cargaCTe.CTe.Moeda = null;
                cargaCTe.CTe.ValorCotacaoMoeda = null;
                cargaCTe.CTe.ValorTotalMoeda = null;

                repositorioCTe.Atualizar(cargaCTe.CTe);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoCTe = repositorioDocumentoFaturamento.BuscarPorCTe(cargaCTe.CTe.Codigo);

                if (documentoFaturamentoCTe != null)
                {
                    documentoFaturamentoCTe.ValorTotalMoeda = null;
                    documentoFaturamentoCTe.Moeda = null;
                    documentoFaturamentoCTe.ValorCotacaoMoeda = null;

                    repositorioDocumentoFaturamento.Atualizar(documentoFaturamentoCTe);
                }
            }

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoCarga = repositorioDocumentoFaturamento.BuscarPorCarga(carga.Codigo);

            if (documentoFaturamentoCarga != null)
            {
                documentoFaturamentoCarga.ValorTotalMoeda = null;
                documentoFaturamentoCarga.Moeda = null;
                documentoFaturamentoCarga.ValorCotacaoMoeda = null;

                repositorioDocumentoFaturamento.Atualizar(documentoFaturamentoCarga);
            }

            carga.ValorCotacaoMoeda = null;
            carga.Moeda = null;
            carga.ValorTotalMoeda = null;
        }

        private void AtualizaDadosCargaPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime? dataBaseCRT)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                cargaPedido.Pedido.Initialize();
                cargaPedido.Pedido.DataBaseCRT = dataBaseCRT;

                carga.SetExternalChanges(cargaPedido.Pedido.GetCurrentChanges());

                repositorioPedido.Atualizar(cargaPedido.Pedido);
            }
        }

        #endregion Carga

        #region Ocorrencia

        private bool ValidarAlteracaoMoedaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, out string erro, int? codigoFaturaAtual = null)
        {
            erro = null;

            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);

            if (cargaOcorrencia == null)
            {
                erro = "A ocorrência não foi encontrada.";
                return false;
            }

            if (cargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Finalizada)
            {
                erro = $"A situação da ocorrência ({cargaOcorrencia.SituacaoOcorrencia.ObterDescricao()}) não permite que a moeda seja alterada.";
                return false;
            }

            List<int> numerosFaturasNova = repFaturaDocumento.BuscarNumeroFaturaPorOcorrencia(cargaOcorrencia.Codigo, codigoFaturaAtual);
            if (numerosFaturasNova.Count > 0)
            {
                erro = "Os documentos da ocorrência estão vinculados à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar a alteração.";
                return false;
            }

            List<int> numerosTitulos = repTituloDocumento.BuscarNumeroTituloPorOcorrencia(cargaOcorrencia.Codigo);
            if (numerosTitulos.Count > 0)
            {
                erro = "Os documentos da ocorrência estão vinculados ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar a alteração.";
                return false;
            }

            List<int> nossoNumeroBoletoTitulos = repTituloDocumento.BuscarNumeroBoletoTituloPorOcorrencia(cargaOcorrencia.Codigo);
            if (nossoNumeroBoletoTitulos.Count > 0)
            {
                erro = "Os documentos da ocorrência estão vinculados a boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar a alteração.";
                return false;
            }

            return true;
        }

        private void AlterarValoresCotacaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo, MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, bool manterValorMoeda = false)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                cargaCTeComplementoInfo.Moeda = moeda;
                cargaCTeComplementoInfo.ValorCotacaoMoeda = valorCotacaoMoeda;

                AtualizarValorCTe(cargaCTeComplementoInfo.CTe, moeda, valorCotacaoMoeda, manterValorMoeda);

                if (manterValorMoeda)
                    cargaOcorrencia.ValorOcorrencia = cargaCTeComplementoInfo.CTe.ValorAReceber;
                else
                    cargaOcorrencia.ValorTotalMoeda = cargaCTeComplementoInfo.CTe.ValorTotalMoeda;

                repositorioCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoCTe = repositorioDocumentoFaturamento.BuscarPorCTe(cargaCTeComplementoInfo.CTe.Codigo);

                if (documentoFaturamentoCTe != null)
                    AtualizarValorDocumentoFaturamento(documentoFaturamentoCTe, moeda, valorCotacaoMoeda, null, manterValorMoeda);
            }

            cargaOcorrencia.ValorCotacaoMoeda = valorCotacaoMoeda;
            cargaOcorrencia.Moeda = moeda;

            if (manterValorMoeda)
                cargaOcorrencia.ValorOcorrencia = Math.Round(cargaOcorrencia.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
            else
                cargaOcorrencia.ValorTotalMoeda = Math.Round(cargaOcorrencia.ValorOcorrencia / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
        }

        private void LimparValoresCotacaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                cargaCTeComplementoInfo.Moeda = null;
                cargaCTeComplementoInfo.ValorCotacaoMoeda = null;
                cargaCTeComplementoInfo.ValorTotalMoeda = null;

                cargaCTeComplementoInfo.CTe.Moeda = null;
                cargaCTeComplementoInfo.CTe.ValorCotacaoMoeda = null;
                cargaCTeComplementoInfo.CTe.ValorTotalMoeda = null;

                repositorioCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);
                repositorioCTe.Atualizar(cargaCTeComplementoInfo.CTe);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoCTe = repositorioDocumentoFaturamento.BuscarPorCTe(cargaCTeComplementoInfo.CTe.Codigo);

                if (documentoFaturamentoCTe != null)
                {
                    documentoFaturamentoCTe.ValorTotalMoeda = null;
                    documentoFaturamentoCTe.Moeda = null;
                    documentoFaturamentoCTe.ValorCotacaoMoeda = null;

                    repositorioDocumentoFaturamento.Atualizar(documentoFaturamentoCTe);
                }
            }

            cargaOcorrencia.ValorCotacaoMoeda = null;
            cargaOcorrencia.Moeda = null;
            cargaOcorrencia.ValorTotalMoeda = null;
        }

        #endregion Ocorrencia

        private void AtualizarValorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, bool manterValorMoeda = false)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            cte.Moeda = moeda;
            cte.ValorCotacaoMoeda = valorCotacaoMoeda;

            if (manterValorMoeda)
                cte.ValorAReceber = Math.Round(cte.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);
            else
                cte.ValorTotalMoeda = Math.Round(cte.ValorAReceber / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);

            repositorioCTe.Atualizar(cte);
        }

        private void AtualizarValorDocumentoFaturamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, MoedaCotacaoBancoCentral moeda, decimal valorCotacaoMoeda, int? codigoFaturaAtual = null, bool manterValorMoeda = false)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento reposiotorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            decimal valorCotacaoAnterior = documentoFaturamento.ValorCotacaoMoeda.Value;

            documentoFaturamento.Moeda = moeda;
            documentoFaturamento.ValorCotacaoMoeda = valorCotacaoMoeda;

            if (manterValorMoeda)
            {
                documentoFaturamento.ValorDocumento = Math.Round(documentoFaturamento.ValorTotalMoeda.Value * valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);

                AtualizarValorFaturaDocumento(documentoFaturamento, valorCotacaoMoeda, valorCotacaoAnterior, codigoFaturaAtual);
            }
            else
                documentoFaturamento.ValorTotalMoeda = Math.Round(documentoFaturamento.ValorDocumento / valorCotacaoMoeda, _numeroCasasDecimais, MidpointRounding.AwayFromZero);

            reposiotorioDocumentoFaturamento.Atualizar(documentoFaturamento);
        }

        private void AtualizarValorFaturaDocumento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, decimal valoCotacaoAtual, decimal valorCotacaoAnterior, int? codigoFaturaAtual = null)
        {
            if (!codigoFaturaAtual.HasValue || (valorCotacaoAnterior <= 0))
                return;

            Repositorio.Embarcador.Fatura.FaturaDocumento repositorioFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);

            Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repositorioFaturaDocumento.BuscarPorDocumentoFaturamentoEFatura(documentoFaturamento.Codigo, codigoFaturaAtual.Value);

            if (faturaDocumento == null)
                return;

            documentoFaturamento.ValorEmFatura -= faturaDocumento.ValorACobrar;
            documentoFaturamento.ValorAFaturar += faturaDocumento.ValorACobrar;

            faturaDocumento.ValorACobrar = Math.Round(faturaDocumento.ValorACobrar * (valoCotacaoAtual / valorCotacaoAnterior), _numeroCasasDecimais, MidpointRounding.AwayFromZero);
            faturaDocumento.ValorTotalACobrar = faturaDocumento.ValorACobrar;

            repositorioFaturaDocumento.Atualizar(faturaDocumento);

            documentoFaturamento.ValorEmFatura += faturaDocumento.ValorACobrar;
            documentoFaturamento.ValorAFaturar -= faturaDocumento.ValorACobrar;
        }

        #endregion Métodos Privados
    }
}
