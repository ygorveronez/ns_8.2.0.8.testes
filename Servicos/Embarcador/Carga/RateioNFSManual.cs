using Dominio.Entidades.Embarcador.Logistica;
using Dominio.Entidades.Embarcador.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class RateioNFSManual
    {
        public void AplicarRateioNaEstruturaDasCargasNoLancamentoManual(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);


            Servicos.Embarcador.Carga.ISS serISS = new ISS(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.CTePorCTeParaSubcontratacao serCTePorCTeParaSubcontratacao = new CTePorCTeParaSubcontratacao(unitOfWork);

            decimal aliquotaISS = lancamentoNFSManual.DadosNFS.AliquotaISS;
            decimal valorTotalISS = lancamentoNFSManual.DadosNFS.ValorISS;
            decimal baseTotalIBSCBS = lancamentoNFSManual.DadosNFS.BaseCalculoIBSCBS;
            decimal valorTotalCBS = lancamentoNFSManual.DadosNFS.ValorCBS;
            decimal valorTotalIBSEstadual = lancamentoNFSManual.DadosNFS.ValorIBSEstadual;
            decimal valorTotalIBSMunicipal = lancamentoNFSManual.DadosNFS.ValorIBSMunicipal;
            decimal aliquotaCBS = lancamentoNFSManual.DadosNFS.AliquotaCBS;
            decimal aliquotaIBSMunicipal = lancamentoNFSManual.DadosNFS.AliquotaIBSMunicipal;
            decimal aliquotaIBSEstadual = lancamentoNFSManual.DadosNFS.AliquotaIBSEstadual;
            decimal valorTotalPrestacao = lancamentoNFSManual.DadosNFS.ValorFrete;
            decimal valorTotalRetencaoISS = lancamentoNFSManual.DadosNFS.ValorRetido;
            decimal totalBaseCalculoISS = lancamentoNFSManual.DadosNFS.ValorBaseCalculo;
            decimal valorFreteTotalBruto = lancamentoNFSManual.Documentos.Sum(o => o.ValorFrete);
            decimal valorFreteTotal = valorFreteTotalBruto - lancamentoNFSManual.DadosNFS.ValorDescontos;
            decimal valorPrestacaoRateado = valorTotalPrestacao / valorFreteTotal;
            decimal valorISSRateado = valorTotalISS / valorFreteTotal;
            decimal valorRetencaoISSRateado = valorTotalRetencaoISS / valorFreteTotal;
            decimal baseCalculoISSRateado = totalBaseCalculoISS / valorFreteTotal;
            decimal baseCalculoIBSCBSRateado = baseTotalIBSCBS / valorFreteTotal;
            decimal valorIBSEstadualRateado = valorTotalIBSEstadual / valorFreteTotal;
            decimal valorIBSMunicipalRateado = valorTotalIBSMunicipal / valorFreteTotal;
            decimal valorCBSRateado = valorTotalCBS / valorFreteTotal;
            decimal valorPrestacaoTotalAplicado = repCargaDocumentoParaEmissaoNFSManual.BuscarValorPrestacaoServicoJaRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            decimal valorISSTotalAplicado = repCargaDocumentoParaEmissaoNFSManual.BuscarValorISSServicoJaRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            decimal valorRetencaoISSTotalAplicado = repCargaDocumentoParaEmissaoNFSManual.BuscarValorRetencaoISSJaRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            decimal baseCalculoISSTotalAplicado = repCargaDocumentoParaEmissaoNFSManual.BuscarBaseCalculoISSJaRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            decimal baseCalculoIBSCBSTotalAplicado = repCargaDocumentoParaEmissaoNFSManual.BuscarBaseCalculoIBSCBSJaRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            decimal valorIBSEstadualTotalAplicado = repCargaDocumentoParaEmissaoNFSManual.BuscarValorIBSEstadualJaRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            decimal valorIBSMunicipalTotalAplicado = repCargaDocumentoParaEmissaoNFSManual.BuscarValorIBSMunicipalJaRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            decimal valorCBSTotalAplicado = repCargaDocumentoParaEmissaoNFSManual.BuscarValorCBSJaRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);

            List<int> documentos = repCargaDocumentoParaEmissaoNFSManual.BuscarNaoRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            int countDocumentos = documentos.Count;

            for (var i = 0; i < countDocumentos; i++)
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCodigo(documentos[i]); //lancamentoNFSManual.Documentos[i];

                if ((i + 1) == countDocumentos)
                {
                    documento.ValorPrestacaoServico = valorTotalPrestacao - valorPrestacaoTotalAplicado;
                    documento.ValorISS = valorTotalISS - valorISSTotalAplicado;
                    documento.ValorRetencaoISS = valorTotalRetencaoISS - valorRetencaoISSTotalAplicado;
                    documento.BaseCalculoISS = totalBaseCalculoISS - baseCalculoISSTotalAplicado;
                    documento.PercentualAliquotaISS = aliquotaISS;
                    documento.BaseCalculoIBSCBS = baseTotalIBSCBS - baseCalculoIBSCBSTotalAplicado;
                    documento.ValorCBS = valorTotalCBS - valorCBSTotalAplicado;
                    documento.ValorIBSMunicipal = valorTotalIBSMunicipal - valorIBSMunicipalTotalAplicado;
                    documento.ValorIBSEstadual = valorTotalIBSEstadual - valorIBSEstadualTotalAplicado;
                }
                else
                {
                    documento.ValorPrestacaoServico = Math.Floor((documento.ValorFrete * valorPrestacaoRateado) * 100) / 100;
                    documento.ValorISS = Math.Floor((documento.ValorFrete * valorISSRateado) * 100) / 100;
                    documento.ValorRetencaoISS = Math.Floor((documento.ValorFrete * valorRetencaoISSRateado) * 100) / 100;
                    documento.BaseCalculoISS = Math.Floor((documento.ValorFrete * baseCalculoISSRateado) * 100) / 100;
                    documento.PercentualAliquotaISS = aliquotaISS;
                    documento.BaseCalculoIBSCBS = Math.Floor((documento.ValorFrete * baseCalculoIBSCBSRateado) * 100) / 100;
                    documento.ValorCBS = Math.Floor((documento.ValorFrete * valorCBSRateado) * 100) / 100;
                    documento.ValorIBSMunicipal = Math.Floor((documento.ValorFrete * valorIBSMunicipalRateado) * 100) / 100;
                    documento.ValorIBSEstadual = Math.Floor((documento.ValorFrete * valorIBSEstadualRateado) * 100) / 100;

                    valorPrestacaoTotalAplicado += documento.ValorPrestacaoServico;
                    valorISSTotalAplicado += documento.ValorISS;
                    valorRetencaoISSTotalAplicado += documento.ValorRetencaoISS;
                    baseCalculoISSTotalAplicado += documento.BaseCalculoISS;
                    baseCalculoIBSCBSTotalAplicado += documento.BaseCalculoIBSCBS;
                    valorCBSTotalAplicado += documento.ValorCBS;
                    valorIBSEstadualTotalAplicado += documento.ValorIBSEstadual;
                    valorIBSMunicipalTotalAplicado += documento.ValorIBSMunicipal;
                }

                documento.AliquotaCBS = aliquotaCBS;
                documento.AliquotaIBSEstadual = aliquotaIBSEstadual;
                documento.AliquotaIBSMunicipal = aliquotaIBSMunicipal;
                documento.PercentualReducaoIBSEstadual = lancamentoNFSManual.DadosNFS.PercentualReducaoIBSEstadual;
                documento.PercentualReducaoIBSMunicipal = lancamentoNFSManual.DadosNFS.PercentualReducaoIBSMunicipal;
                documento.PercentualReducaoCBS = lancamentoNFSManual.DadosNFS.PercentualReducaoCBS;
                documento.OutrasAliquotas = lancamentoNFSManual.DadosNFS.OutrasAliquotas;
                documento.NBS = lancamentoNFSManual.DadosNFS.NBS;
                documento.CodigoIndicadorOperacao = lancamentoNFSManual.DadosNFS.IndicadorOperacao;

                documento.RateouValorFrete = true;
                repCargaDocumentoParaEmissaoNFSManual.Atualizar(documento);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

                if (documento.PedidoXMLNotaFiscal != null && documento.CargaDocumentoParaEmissaoNFSManualOcorrenciaOrigem == null)
                {
                    AplicarRateioNFe(lancamentoNFSManual, documento.PedidoXMLNotaFiscal, documento.ValorISS, documento.ValorRetencaoISS, documento.BaseCalculoISS, documento.BaseCalculoIBSCBS, documento.ValorCBS, documento.ValorIBSEstadual, documento.ValorIBSMunicipal, unitOfWork);

                    cargaPedido = documento.PedidoXMLNotaFiscal.CargaPedido;
                }
                else if (documento.PedidoCTeParaSubContratacao != null && documento.CargaDocumentoParaEmissaoNFSManualOcorrenciaOrigem == null)
                {
                    AplicarRateioCTeSub(lancamentoNFSManual, documento.PedidoCTeParaSubContratacao, documento.ValorISS, documento.ValorRetencaoISS, documento.BaseCalculoISS, documento.BaseCalculoIBSCBS, documento.ValorCBS, documento.ValorIBSEstadual, documento.ValorIBSMunicipal, unitOfWork);

                    cargaPedido = documento.PedidoCTeParaSubContratacao.CargaPedido;
                }
                else if (documento.CargaCTeComplementoInfo != null)
                {
                    AplicarRateioComplementoInfo(lancamentoNFSManual, documento.CargaCTeComplementoInfo, documento.ValorISS, documento.ValorRetencaoISS, documento.BaseCalculoISS, documento.BaseCalculoIBSCBS, documento.ValorCBS, documento.ValorIBSEstadual, documento.ValorIBSMunicipal, unitOfWork);
                }
                else if (documento.CargaCTe != null && documento.CargaCTe.CargaCTeComplementoInfo == null && documento.CargaDocumentoParaEmissaoNFSManualOcorrenciaOrigem == null) //se é CT-e complementar (ocorrência) não rateia os valores nas notas e nos pedidos
                {
                    AplicarRateioCargaCTe(lancamentoNFSManual, documento.CargaCTe, documento.ValorISS, documento.ValorRetencaoISS, documento.BaseCalculoISS, documento.BaseCalculoIBSCBS, documento.ValorCBS, documento.ValorIBSEstadual, documento.ValorIBSMunicipal, unitOfWork);

                    cargaPedido = documento.CargaCTe.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.CargaPedido).FirstOrDefault();
                }

                if (cargaPedido != null)
                {
                    if (cargaPedido.ModeloDocumentoFiscal == null)
                        cargaPedido.ModeloDocumentoFiscal = lancamentoNFSManual.DadosNFS.ModeloDocumentoFiscal;

                    cargaPedido.PercentualAliquotaISS = lancamentoNFSManual.DadosNFS.AliquotaISS;
                    cargaPedido.IncluirISSBaseCalculo = lancamentoNFSManual.DadosNFS.IncluirISSBC;
                    cargaPedido.PercentualRetencaoISS = lancamentoNFSManual.DadosNFS.PercentualRetencao;
                    cargaPedido.ValorISS += documento.ValorISS;
                    cargaPedido.ValorRetencaoISS += documento.ValorRetencaoISS;
                    cargaPedido.BaseCalculoISS += documento.BaseCalculoISS;
                    
                    cargaPedido.AliquotaCBS = aliquotaCBS;
                    cargaPedido.AliquotaIBSEstadual = aliquotaIBSEstadual;
                    cargaPedido.AliquotaIBSMunicipal = aliquotaIBSMunicipal;
                    cargaPedido.ValorCBS += documento.ValorCBS;
                    cargaPedido.BaseCalculoIBSCBS += documento.BaseCalculoIBSCBS;
                    cargaPedido.ValorIBSMunicipal += documento.ValorIBSMunicipal;
                    cargaPedido.ValorIBSEstadual += documento.ValorIBSEstadual;
                    cargaPedido.PercentualReducaoIBSEstadual = lancamentoNFSManual.DadosNFS.PercentualReducaoIBSEstadual;
                    cargaPedido.PercentualReducaoIBSMunicipal = lancamentoNFSManual.DadosNFS.PercentualReducaoIBSMunicipal;
                    cargaPedido.PercentualReducaoCBS = lancamentoNFSManual.DadosNFS.PercentualReducaoCBS;
                    cargaPedido.OutrasAliquotas = lancamentoNFSManual.DadosNFS.OutrasAliquotas;
                    cargaPedido.NBS = lancamentoNFSManual.DadosNFS.NBS;
                    cargaPedido.CodigoIndicadorOperacao = lancamentoNFSManual.DadosNFS.IndicadorOperacao;
                    cargaPedido.CSTIBSCBS = lancamentoNFSManual.DadosNFS.CSTIBSCBS;
                    cargaPedido.ClassificacaoTributariaIBSCBS = lancamentoNFSManual.DadosNFS.ClassificacaoTributariaIBSCBS;

                    repCargaPedido.Atualizar(cargaPedido);

                    serRateioFrete.GerarComponenteISS(cargaPedido, false, unitOfWork);

                    cargaPedido.Carga.ValorISS += documento.ValorISS;
                    cargaPedido.Carga.ValorRetencaoISS += documento.ValorRetencaoISS;

                    repCarga.Atualizar(cargaPedido.Carga);

                    serRateioFrete.GerarComponenteISS(cargaPedido.Carga, cargaPedido.Carga.Pedidos.ToList(), unitOfWork);
                }
                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
        }

        private void AplicarRateioNFe(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, decimal valorISS, decimal valorRetencaoISS, decimal baseCalculoISS, decimal baseCalculoIBSCBS, decimal valorCBS, decimal valorIBSEstadual, decimal valorIBSMunicipal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = lancamentoNFSManual.DadosNFS;

            pedidoXMLNotaFiscal.PercentualAliquotaISS = dadosNFS.AliquotaISS;
            pedidoXMLNotaFiscal.PercentualRetencaoISS = dadosNFS.PercentualRetencao;
            pedidoXMLNotaFiscal.IncluirISSBaseCalculo = dadosNFS.IncluirISSBC;
            pedidoXMLNotaFiscal.ModeloDocumentoFiscal = dadosNFS.ModeloDocumentoFiscal;
            pedidoXMLNotaFiscal.ValorISS = valorISS;
            pedidoXMLNotaFiscal.ValorRetencaoISS = valorRetencaoISS;
            pedidoXMLNotaFiscal.BaseCalculoISS = baseCalculoISS;
            pedidoXMLNotaFiscal.BaseCalculoIBSCBS = baseCalculoIBSCBS;
            pedidoXMLNotaFiscal.ValorCBS = valorCBS;
            pedidoXMLNotaFiscal.ValorIBSEstadual = valorIBSEstadual;
            pedidoXMLNotaFiscal.ValorIBSMunicipal = valorIBSMunicipal;
            pedidoXMLNotaFiscal.AliquotaCBS = dadosNFS.AliquotaCBS;
            pedidoXMLNotaFiscal.AliquotaIBSEstadual = dadosNFS.AliquotaIBSEstadual;
            pedidoXMLNotaFiscal.AliquotaIBSMunicipal = dadosNFS.AliquotaIBSMunicipal;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual = dadosNFS.PercentualReducaoIBSEstadual;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal = dadosNFS.PercentualReducaoIBSMunicipal;
            pedidoXMLNotaFiscal.PercentualReducaoCBS = dadosNFS.PercentualReducaoCBS;
            pedidoXMLNotaFiscal.OutrasAliquotas = dadosNFS.OutrasAliquotas;
            pedidoXMLNotaFiscal.NBS = dadosNFS.NBS;
            pedidoXMLNotaFiscal.CodigoIndicadorOperacao = dadosNFS.IndicadorOperacao;
            pedidoXMLNotaFiscal.CSTIBSCBS = dadosNFS.CSTIBSCBS;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS = dadosNFS.ClassificacaoTributariaIBSCBS;

            Servicos.Log.TratarErro($"Atualizando AplicarRateioNFe lancamentoNFSManual = {lancamentoNFSManual?.Codigo ?? 0}, pedidoXmlNotaFiscal = {pedidoXMLNotaFiscal.Codigo} com valorISS = {pedidoXMLNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXMLNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXMLNotaFiscal.ValorRetencaoISS}, CSTIBSCBS = {pedidoXMLNotaFiscal.CSTIBSCBS}, ClassificacaoTributariaIBSCBS = {pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS}, CodigoIndicadorOperacao = {pedidoXMLNotaFiscal.CodigoIndicadorOperacao}", "AtualizarPedidoXMLNotaFiscal");

			serRateioNotaFiscal.GerarComponenteISS(pedidoXMLNotaFiscal, unitOfWork);

            repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
        }

        private void AplicarRateioComplementoInfo(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, decimal valorISS, decimal valorRetencaoISS, decimal baseCalculoISS, decimal baseCalculoIBSCBS, decimal valorCBS, decimal valorIBSEstadual, decimal valorIBSMunicipal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = lancamentoNFSManual.DadosNFS;

            cargaCTeComplementoInfo.PercentualAliquotaISS = lancamentoNFSManual.DadosNFS.AliquotaISS;
            cargaCTeComplementoInfo.PercentualRetencaoISS = lancamentoNFSManual.DadosNFS.PercentualRetencao;
            cargaCTeComplementoInfo.IncluirISSBaseCalculo = lancamentoNFSManual.DadosNFS.IncluirISSBC;
            cargaCTeComplementoInfo.ValorISS = valorISS;
            cargaCTeComplementoInfo.ValorRetencaoISS = valorRetencaoISS;
            cargaCTeComplementoInfo.BaseCalculoISS = baseCalculoISS;
            cargaCTeComplementoInfo.BaseCalculoIBSCBS = baseCalculoIBSCBS;
            cargaCTeComplementoInfo.ValorCBS = valorCBS;
            cargaCTeComplementoInfo.ValorIBSEstadual = valorIBSEstadual;
            cargaCTeComplementoInfo.ValorIBSMunicipal = valorIBSMunicipal;
            cargaCTeComplementoInfo.AliquotaCBS = dadosNFS.AliquotaCBS;
            cargaCTeComplementoInfo.AliquotaIBSEstadual = dadosNFS.AliquotaIBSEstadual;
            cargaCTeComplementoInfo.AliquotaIBSMunicipal = dadosNFS.AliquotaIBSMunicipal;
            cargaCTeComplementoInfo.PercentualReducaoIBSEstadual = dadosNFS.PercentualReducaoIBSEstadual;
            cargaCTeComplementoInfo.PercentualReducaoIBSMunicipal = dadosNFS.PercentualReducaoIBSMunicipal;
            cargaCTeComplementoInfo.PercentualReducaoCBS = dadosNFS.PercentualReducaoCBS;
            cargaCTeComplementoInfo.OutrasAliquotas = dadosNFS.OutrasAliquotas;
            cargaCTeComplementoInfo.NBS = dadosNFS.NBS;
            cargaCTeComplementoInfo.CodigoIndicadorOperacao = dadosNFS.IndicadorOperacao;
            cargaCTeComplementoInfo.CSTIBSCBS = dadosNFS.CSTIBSCBS;
            cargaCTeComplementoInfo.ClassificacaoTributariaIBSCBS = dadosNFS.ClassificacaoTributariaIBSCBS;

            repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);
        }

        private void AplicarRateioCTeSub(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, decimal valorISS, decimal valorRetencaoISS, decimal baseCalculoISS, decimal baseCalculoIBSCBS, decimal valorCBS, decimal valorIBSEstadual, decimal valorIBSMunicipal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = lancamentoNFSManual.DadosNFS;

            Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);

            pedidoCTeParaSubContratacao.PercentualAliquotaISS = lancamentoNFSManual.DadosNFS.AliquotaISS;
            pedidoCTeParaSubContratacao.PercentualRetencaoISS = lancamentoNFSManual.DadosNFS.PercentualRetencao;
            pedidoCTeParaSubContratacao.IncluirISSBaseCalculo = lancamentoNFSManual.DadosNFS.IncluirISSBC;
            pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = lancamentoNFSManual.DadosNFS.ModeloDocumentoFiscal;
            pedidoCTeParaSubContratacao.ValorISS = valorISS;
            pedidoCTeParaSubContratacao.ValorRetencaoISS = valorRetencaoISS;
            pedidoCTeParaSubContratacao.BaseCalculoISS = baseCalculoISS;
            pedidoCTeParaSubContratacao.BaseCalculoIBSCBS = baseCalculoIBSCBS;
            pedidoCTeParaSubContratacao.ValorCBS = valorCBS;
            pedidoCTeParaSubContratacao.ValorIBSEstadual = valorIBSEstadual;
            pedidoCTeParaSubContratacao.ValorIBSMunicipal = valorIBSMunicipal;
            pedidoCTeParaSubContratacao.AliquotaCBS = dadosNFS.AliquotaCBS;
            pedidoCTeParaSubContratacao.AliquotaIBSEstadual = dadosNFS.AliquotaIBSEstadual;
            pedidoCTeParaSubContratacao.AliquotaIBSMunicipal = dadosNFS.AliquotaIBSMunicipal;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual = dadosNFS.PercentualReducaoIBSEstadual;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal = dadosNFS.PercentualReducaoIBSMunicipal;
            pedidoCTeParaSubContratacao.PercentualReducaoCBS = dadosNFS.PercentualReducaoCBS;
            pedidoCTeParaSubContratacao.OutrasAliquotas = dadosNFS.OutrasAliquotas;
            pedidoCTeParaSubContratacao.NBS = dadosNFS.NBS;
            pedidoCTeParaSubContratacao.CodigoIndicadorOperacao = dadosNFS.IndicadorOperacao;
            pedidoCTeParaSubContratacao.CSTIBSCBS = dadosNFS.CSTIBSCBS;
            pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS = dadosNFS.ClassificacaoTributariaIBSCBS;

            serRateioNotaFiscal.GerarComponenteISS(pedidoCTeParaSubContratacao, unitOfWork);

            repPedidoCTeParaSubContratacao.Atualizar(pedidoCTeParaSubContratacao);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);

            decimal valorTotalISS = pedidoCTeParaSubContratacao.ValorISS;
            decimal valorTotalRetencaoISS = pedidoCTeParaSubContratacao.ValorRetencaoISS;
            decimal totalBaseCalculoISS = pedidoCTeParaSubContratacao.BaseCalculoISS;
            decimal valorFreteTotal = pedidoCTeParaSubContratacaoNotasFiscais.Count;
            decimal valorISSRateado = valorTotalISS / valorFreteTotal;
            decimal valorRetencaoISSRateado = valorTotalRetencaoISS / valorFreteTotal;
            decimal baseCalculoISSRateado = totalBaseCalculoISS / valorFreteTotal;
            decimal baseCalculoIBSCBSRateado = baseCalculoIBSCBS / valorFreteTotal;
            decimal valorCBSRateado = valorCBS / valorFreteTotal;
            decimal valorIBSEstadualRateado = valorIBSEstadual / valorFreteTotal;
            decimal valorIBSMunicipalRateado = valorIBSMunicipal / valorFreteTotal;
            decimal baseCalculoIBSCBSAplicado = 0m;
            decimal valorCBSAplicado = 0m;
            decimal valorIBSEstadualAplicado = 0m;
            decimal valorIBSMunicipalAplicado = 0m;
            decimal valorISSTotalAplicado = 0m;
            decimal valorRetencaoISSTotalAplicado = 0m;
            decimal baseCalculoISSTotalAplicado = 0m;

            int countDocumentos = pedidoCTeParaSubContratacaoNotasFiscais.Count;

            for (var i = 0; i < countDocumentos; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotasFiscais[i].PedidoXMLNotaFiscal;

                decimal valorISSAplicar = 0m, valorRetencaoISSAplicar = 0m, baseCalculoISSAplicar = 0m,
                    baseCalculoIBSCBSAplicar = 0m, valorCBSAplicar = 0m, valorIBSEstadualAplicar = 0m, valorIBSMunicipalAplicar = 0m;

                if ((i + 1) == countDocumentos)
                {
                    valorISSAplicar = valorTotalISS - valorISSTotalAplicado;
                    valorRetencaoISSAplicar = valorTotalRetencaoISS - valorRetencaoISSTotalAplicado;
                    baseCalculoISSAplicar = totalBaseCalculoISS - baseCalculoISSTotalAplicado;
                    baseCalculoIBSCBSAplicar = baseCalculoIBSCBS - baseCalculoIBSCBSAplicado;
                    valorCBSAplicar = valorCBS - valorCBSAplicado;
                    valorIBSEstadualAplicar = valorIBSEstadual - valorIBSEstadualAplicado;
                    valorIBSMunicipalAplicar = valorIBSMunicipal - valorIBSMunicipalAplicado;
                }
                else
                {
                    valorISSAplicar = Math.Floor(valorISSRateado);
                    valorRetencaoISSAplicar = Math.Floor(valorRetencaoISSRateado);
                    baseCalculoISSAplicar = Math.Floor(baseCalculoISSRateado);

                    baseCalculoIBSCBSAplicar = Math.Floor(baseCalculoIBSCBSRateado);
                    valorCBSAplicar = Math.Floor(valorCBSRateado);
                    valorIBSEstadualAplicar = Math.Floor(valorIBSEstadualRateado);
                    valorIBSMunicipalAplicar = Math.Floor(valorIBSMunicipalRateado);

                    valorISSTotalAplicado += valorISSAplicar;
                    valorRetencaoISSTotalAplicado += valorRetencaoISSAplicar;
                    baseCalculoISSTotalAplicado += baseCalculoISSAplicar;

                    baseCalculoIBSCBSAplicado += baseCalculoIBSCBSAplicar;
                    valorCBSAplicado += valorCBSAplicar;
                    valorIBSMunicipalAplicado += valorIBSMunicipalAplicar;
                    valorIBSEstadualAplicado += valorIBSEstadualAplicar;
                }

                AplicarRateioNFe(lancamentoNFSManual, pedidoXMLNotaFiscal, valorISSAplicar, valorRetencaoISSAplicar, baseCalculoISSAplicar, baseCalculoIBSCBSAplicar, valorCBSAplicar, valorIBSEstadualAplicar, valorIBSMunicipalAplicar, unitOfWork);
            }
        }

        private void AplicarRateioCargaCTe(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, decimal valorISS, decimal valorRetencaoISS, decimal baseCalculoISS, decimal baseCalculoIBSCBS, decimal valorCBS, decimal valorIBSEstadual, decimal valorIBSMunicipal, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = (from obj in cargaCTe.NotasFiscais select obj.PedidoXMLNotaFiscal).ToList();
            Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = lancamentoNFSManual.DadosNFS;

            if (pedidoXMLNotasFiscais.Any(o => o.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao))
            {
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCodigoPedidoXMLNotaFiscal(pedidoXMLNotasFiscais.Select(o => o.Codigo).ToArray());

                decimal valorTotalISS = valorISS;
                decimal valorTotalRetencaoISS = valorRetencaoISS;
                decimal totalBaseCalculoISS = baseCalculoISS;
                decimal valorFreteTotal = pedidoCTesParaSubcontratacao.Sum(o => o.ValorFrete);
                decimal valorISSRateado = valorFreteTotal > 0m ? valorTotalISS / valorFreteTotal : 0m;
                decimal valorRetencaoISSRateado = valorFreteTotal > 0m ? valorTotalRetencaoISS / valorFreteTotal : 0m;
                decimal baseCalculoISSRateado = valorFreteTotal > 0m ? totalBaseCalculoISS / valorFreteTotal : 0m;
                decimal baseCalculoIBSCBSRateado = baseCalculoIBSCBS / valorFreteTotal;
                decimal valorCBSRateado = valorCBS / valorFreteTotal;
                decimal valorIBSEstadualRateado = valorIBSEstadual / valorFreteTotal;
                decimal valorIBSMunicipalRateado = valorIBSMunicipal / valorFreteTotal;

                decimal valorISSTotalAplicado = 0m;
                decimal valorRetencaoISSTotalAplicado = 0m;
                decimal baseCalculoISSTotalAplicado = 0m;
                decimal baseCalculoIBSCBSAplicado = 0m;
                decimal valorCBSAplicado = 0m;
                decimal valorIBSEstadualAplicado = 0m;
                decimal valorIBSMunicipalAplicado = 0m;

                int countDocumentos = pedidoCTesParaSubcontratacao.Count;

                for (var i = 0; i < countDocumentos; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = pedidoCTesParaSubcontratacao[i];

                    decimal valorISSAplicar = 0m, valorRetencaoISSAplicar = 0m, baseCalculoISSAplicar = 0m, baseCalculoIBSCBSAplicar = 0m, valorCBSAplicar = 0m, valorIBSEstadualAplicar = 0m, valorIBSMunicipalAplicar = 0m;

                    if ((i + 1) == countDocumentos)
                    {
                        valorISSAplicar = valorTotalISS - valorISSTotalAplicado;
                        valorRetencaoISSAplicar = valorTotalRetencaoISS - valorRetencaoISSTotalAplicado;
                        baseCalculoISSAplicar = totalBaseCalculoISS - baseCalculoISSTotalAplicado;
                        baseCalculoIBSCBSAplicar = baseCalculoIBSCBS - baseCalculoIBSCBSAplicado;
                        valorCBSAplicar = valorCBS - valorCBSAplicado;
                        valorIBSEstadualAplicar = valorIBSEstadual - valorIBSEstadualAplicado;
                        valorIBSMunicipalAplicar = valorIBSMunicipal - valorIBSMunicipalAplicado;
                    }
                    else
                    {
                        valorISSAplicar = Math.Floor((pedidoCTeParaSubContratacao.ValorFrete * valorISSRateado) * 100) / 100;
                        valorRetencaoISSAplicar = Math.Floor((pedidoCTeParaSubContratacao.ValorFrete * valorRetencaoISSRateado) * 100) / 100;
                        baseCalculoISSAplicar = Math.Floor((pedidoCTeParaSubContratacao.ValorFrete * baseCalculoISSRateado) * 100) / 100;
                        baseCalculoIBSCBSAplicar = Math.Floor((pedidoCTeParaSubContratacao.ValorFrete * baseCalculoIBSCBSRateado) * 100) / 100;
                        valorCBSAplicar = Math.Floor((pedidoCTeParaSubContratacao.ValorFrete * valorCBSRateado) * 100) / 100;
                        valorIBSEstadualAplicar = Math.Floor((pedidoCTeParaSubContratacao.ValorFrete * valorIBSEstadualRateado) * 100) / 100;
                        valorIBSMunicipalAplicar = Math.Floor((pedidoCTeParaSubContratacao.ValorFrete * valorIBSMunicipalRateado) * 100) / 100;

                        baseCalculoIBSCBSAplicado += baseCalculoIBSCBSAplicar;
                        valorCBSAplicado += valorCBSAplicar;
                        valorIBSMunicipalAplicado += valorIBSMunicipalAplicar;
                        valorIBSEstadualAplicado += valorIBSEstadualAplicar;
                        
                        valorISSTotalAplicado += valorISSAplicar;
                        valorRetencaoISSTotalAplicado += valorRetencaoISSAplicar;
                        baseCalculoISSTotalAplicado += baseCalculoISSAplicar;
                    }

                    AplicarRateioCTeSub(lancamentoNFSManual, pedidoCTeParaSubContratacao, valorISSAplicar, valorRetencaoISSAplicar, baseCalculoISSAplicar, baseCalculoIBSCBSAplicar, valorCBSAplicar, valorIBSEstadualAplicar, valorIBSMunicipalAplicar, unitOfWork);
                }
            }
            else
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

                decimal valorFreteTotal = pedidoXMLNotasFiscais.Sum(o => o.ValorFrete) +
                                          repPedidoXMLNotaFiscalComponenteFrete.ObterValorComponentesPorPedidoXMLNotaFiscal(pedidoXMLNotasFiscais.Select(o => o.Codigo));

                decimal valorTotalISS = valorISS;
                decimal valorTotalRetencaoISS = valorRetencaoISS;
                decimal totalBaseCalculoISS = baseCalculoISS;
                //decimal valorFreteTotal = pedidoXMLNotasFiscais.Sum(o => o.ValorFrete);
                decimal valorISSRateado = valorFreteTotal > 0m ? valorTotalISS / valorFreteTotal : 0m;
                decimal valorRetencaoISSRateado = valorFreteTotal > 0m ? valorTotalRetencaoISS / valorFreteTotal : 0m;
                decimal baseCalculoISSRateado = valorFreteTotal > 0m ? totalBaseCalculoISS / valorFreteTotal : 0m;
                decimal baseCalculoIBSCBSRateado = baseCalculoIBSCBS / valorFreteTotal;
                decimal valorCBSRateado = valorCBS / valorFreteTotal;
                decimal valorIBSEstadualRateado = valorIBSEstadual / valorFreteTotal;
                decimal valorIBSMunicipalRateado = valorIBSMunicipal / valorFreteTotal;

                decimal valorISSTotalAplicado = 0m;
                decimal valorRetencaoISSTotalAplicado = 0m;
                decimal baseCalculoISSTotalAplicado = 0m;
                decimal baseCalculoIBSCBSAplicado = 0m;
                decimal valorCBSAplicado = 0m;
                decimal valorIBSEstadualAplicado = 0m;
                decimal valorIBSMunicipalAplicado = 0m;

                int countDocumentos = pedidoXMLNotasFiscais.Count;

                for (var i = 0; i < countDocumentos; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoXMLNotasFiscais[i];

                    decimal valorFreteTotalNotaFiscal = pedidoXMLNotaFiscal.ValorFrete + repPedidoXMLNotaFiscalComponenteFrete.ObterValorComponentesPorPedidoXMLNotaFiscal(new int[] { pedidoXMLNotaFiscal.Codigo });

                    decimal valorISSAplicar = 0m, valorRetencaoISSAplicar = 0m, baseCalculoISSAplicar = 0m, baseCalculoIBSCBSAplicar = 0m, valorCBSAplicar = 0m, valorIBSEstadualAplicar = 0m, valorIBSMunicipalAplicar = 0m;

                    if ((i + 1) == countDocumentos)
                    {
                        valorISSAplicar = valorTotalISS - valorISSTotalAplicado;
                        valorRetencaoISSAplicar = valorTotalRetencaoISS - valorRetencaoISSTotalAplicado;
                        baseCalculoISSAplicar = totalBaseCalculoISS - baseCalculoISSTotalAplicado;
                        baseCalculoIBSCBSAplicar = baseCalculoIBSCBS - baseCalculoIBSCBSAplicado;
                        valorCBSAplicar = valorCBS - valorCBSAplicado;
                        valorIBSEstadualAplicar = valorIBSEstadual - valorIBSEstadualAplicado;
                        valorIBSMunicipalAplicar = valorIBSMunicipal - valorIBSMunicipalAplicado;
                    }
                    else
                    {
                        valorISSAplicar = Math.Floor((valorFreteTotalNotaFiscal * valorISSRateado) * 100) / 100;
                        valorRetencaoISSAplicar = Math.Floor((valorFreteTotalNotaFiscal * valorRetencaoISSRateado) * 100) / 100;
                        baseCalculoISSAplicar = Math.Floor((valorFreteTotalNotaFiscal * baseCalculoISSRateado) * 100) / 100;

                        baseCalculoIBSCBSAplicar = Math.Floor((valorFreteTotalNotaFiscal * baseCalculoIBSCBSRateado) * 100) / 100; 
                        valorCBSAplicar = Math.Floor((valorFreteTotalNotaFiscal * valorCBSRateado) * 100) / 100;
                        valorIBSEstadualAplicar = Math.Floor((valorFreteTotalNotaFiscal * valorIBSEstadualRateado) * 100) / 100;
                        valorIBSMunicipalAplicar = Math.Floor((valorFreteTotalNotaFiscal * valorIBSMunicipalRateado) * 100) / 100;

                        baseCalculoIBSCBSAplicado += baseCalculoIBSCBSAplicar;
                        valorCBSAplicado += valorCBSAplicar;
                        valorIBSMunicipalAplicado += valorIBSMunicipalAplicar;
                        valorIBSEstadualAplicado += valorIBSEstadualAplicar;
                        
                        valorISSTotalAplicado += valorISSAplicar;
                        valorRetencaoISSTotalAplicado += valorRetencaoISSAplicar;
                        baseCalculoISSTotalAplicado += baseCalculoISSAplicar;
                    }

                    AplicarRateioNFe(lancamentoNFSManual, pedidoXMLNotaFiscal, valorISSAplicar, valorRetencaoISSAplicar, baseCalculoISSAplicar, baseCalculoIBSCBSAplicar, valorCBSAplicar, valorIBSEstadualAplicar, valorIBSMunicipalAplicar, unitOfWork);
                }
            }
        }

        public void ReverterRateioNaEstruturaDasCargasNoLancamentoManual(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

            Servicos.Embarcador.Carga.ISS serISS = new ISS(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.CTePorCTeParaSubcontratacao serCTePorCTeParaSubcontratacao = new CTePorCTeParaSubcontratacao(unitOfWork);

            List<int> documentos = repCargaDocumentoParaEmissaoNFSManual.BuscarRateadasPorLancamentoNFsManual(lancamentoNFSManual.Codigo);
            int countDocumentos = documentos.Count;

            for (var i = 0; i < countDocumentos; i++)
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCodigo(documentos[i]); //lancamentoNFSManual.Documentos[i];

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

                if (documento.PedidoXMLNotaFiscal != null)
                {
                    ReverterRateioNFe(lancamentoNFSManual, documento.PedidoXMLNotaFiscal, unitOfWork);

                    cargaPedido = documento.PedidoXMLNotaFiscal.CargaPedido;
                }
                else if (documento.PedidoCTeParaSubContratacao != null)
                {
                    ReverterRateioCTeSub(lancamentoNFSManual, documento.PedidoCTeParaSubContratacao, unitOfWork);

                    cargaPedido = documento.PedidoCTeParaSubContratacao.CargaPedido;
                }
                else if (documento.CargaCTe != null && documento.CargaCTe.CargaCTeComplementoInfo == null) //se é CT-e complementar (ocorrência) não rateia os valores nas notas e nos pedidos
                {
                    ReverterRateioCargaCTe(lancamentoNFSManual, documento.CargaCTe, unitOfWork);

                    cargaPedido = documento.CargaCTe.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.CargaPedido).FirstOrDefault();
                }

                if (cargaPedido != null)
                {
                    cargaPedido.ValorISS -= documento.ValorISS;
                    cargaPedido.ValorRetencaoISS -= documento.ValorRetencaoISS;
                    cargaPedido.BaseCalculoISS -= documento.BaseCalculoISS;
                    cargaPedido.BaseCalculoIBSCBS -= documento.BaseCalculoIBSCBS;
                    cargaPedido.ValorIBSEstadual -= documento.ValorIBSEstadual;
                    cargaPedido.ValorIBSMunicipal -= documento.ValorIBSMunicipal;
                    cargaPedido.ValorCBS -= documento.ValorCBS;
            
                    repCargaPedido.Atualizar(cargaPedido);

                    serRateioFrete.GerarComponenteISS(cargaPedido, false, unitOfWork);

                    cargaPedido.Carga.ValorISS -= documento.ValorISS;
                    cargaPedido.Carga.ValorRetencaoISS -= documento.ValorRetencaoISS;
                    cargaPedido.Carga.ValorIBSEstadual -= documento.ValorIBSEstadual;
                    cargaPedido.Carga.ValorIBSMunicipal -= documento.ValorIBSMunicipal;
                    cargaPedido.Carga.ValorCBS -= documento.ValorCBS;

                    repCarga.Atualizar(cargaPedido.Carga);

                    serRateioFrete.GerarComponenteISS(cargaPedido.Carga, cargaPedido.Carga.Pedidos.ToList(), unitOfWork);
                }

                documento.ValorPrestacaoServico = 0m;
                documento.ValorISS = 0m;
                documento.ValorRetencaoISS = 0m;
                documento.BaseCalculoISS = 0m;
                documento.PercentualAliquotaISS = 0m;
                documento.BaseCalculoIBSCBS = 0m;
                documento.ValorCBS = 0m;
                documento.ValorIBSEstadual = 0m;
                documento.ValorIBSMunicipal = 0m;
                documento.AliquotaCBS = 0m;
                documento.AliquotaIBSEstadual = 0m;
                documento.AliquotaIBSMunicipal = 0m;
                documento.PercentualReducaoIBSEstadual = 0m;
                documento.PercentualReducaoIBSMunicipal = 0m;
                documento.PercentualReducaoCBS = 0m;
                documento.OutrasAliquotas = null;
                documento.NBS = "";
                documento.CodigoIndicadorOperacao = "";
                documento.CSTIBSCBS = "";
                documento.ClassificacaoTributariaIBSCBS = "";

                documento.RateouValorFrete = false;

                repCargaDocumentoParaEmissaoNFSManual.Atualizar(documento);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
        }

        private void ReverterRateioNFe(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);

            pedidoXMLNotaFiscal.PercentualAliquotaISS = 0m;
            pedidoXMLNotaFiscal.PercentualRetencaoISS = 0m;
            pedidoXMLNotaFiscal.IncluirISSBaseCalculo = false;
            pedidoXMLNotaFiscal.ModeloDocumentoFiscal = null;
            pedidoXMLNotaFiscal.ValorISS = 0m;
            pedidoXMLNotaFiscal.ValorRetencaoISS = 0m;
            pedidoXMLNotaFiscal.BaseCalculoISS = 0m;
            pedidoXMLNotaFiscal.BaseCalculoIBSCBS = 0m;
            pedidoXMLNotaFiscal.ValorCBS = 0m;
            pedidoXMLNotaFiscal.ValorIBSEstadual = 0m;
            pedidoXMLNotaFiscal.ValorIBSMunicipal = 0m;
            pedidoXMLNotaFiscal.AliquotaCBS = 0m;
            pedidoXMLNotaFiscal.AliquotaIBSEstadual = 0m;
            pedidoXMLNotaFiscal.AliquotaIBSMunicipal = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoCBS = 0m;
            pedidoXMLNotaFiscal.OutrasAliquotas = null;
            pedidoXMLNotaFiscal.NBS = "";
            pedidoXMLNotaFiscal.CodigoIndicadorOperacao = "";
            pedidoXMLNotaFiscal.CSTIBSCBS = "";
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS = "";

            Servicos.Log.TratarErro($"Atualizando ReverterRateioNFe lancamentoNFSManual = {lancamentoNFSManual?.Codigo ?? 0}, pedidoXmlNotaFiscal = {pedidoXMLNotaFiscal.Codigo} com valorISS = {pedidoXMLNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXMLNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXMLNotaFiscal.ValorRetencaoISS} , CSTIBSCBS = {pedidoXMLNotaFiscal.CSTIBSCBS}, ClassificacaoTributariaIBSCBS = {pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS}, CodigoIndicadorOperacao = {pedidoXMLNotaFiscal.CodigoIndicadorOperacao}", "AtualizarPedidoXMLNotaFiscal");

			serRateioNotaFiscal.GerarComponenteISS(pedidoXMLNotaFiscal, unitOfWork);

            repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
        }

        private void ReverterRateioCTeSub(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);

            pedidoCTeParaSubContratacao.PercentualAliquotaISS = 0m;
            pedidoCTeParaSubContratacao.PercentualRetencaoISS = 0m;
            pedidoCTeParaSubContratacao.IncluirISSBaseCalculo = false;
            pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = null;
            pedidoCTeParaSubContratacao.ValorISS = 0m;
            pedidoCTeParaSubContratacao.ValorRetencaoISS = 0m;
            pedidoCTeParaSubContratacao.BaseCalculoISS = 0m;
            pedidoCTeParaSubContratacao.BaseCalculoIBSCBS = 0m;
            pedidoCTeParaSubContratacao.ValorCBS = 0m;
            pedidoCTeParaSubContratacao.ValorIBSEstadual = 0m;
            pedidoCTeParaSubContratacao.ValorIBSMunicipal = 0m;
            pedidoCTeParaSubContratacao.AliquotaCBS = 0m;
            pedidoCTeParaSubContratacao.AliquotaIBSEstadual = 0m;
            pedidoCTeParaSubContratacao.AliquotaIBSMunicipal = 0m;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual = 0m;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal = 0m;
            pedidoCTeParaSubContratacao.PercentualReducaoCBS = 0m;
            pedidoCTeParaSubContratacao.OutrasAliquotas = null;
            pedidoCTeParaSubContratacao.NBS = "";
            pedidoCTeParaSubContratacao.CodigoIndicadorOperacao = "";
            pedidoCTeParaSubContratacao.CSTIBSCBS = "";
            pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS = "";

            serRateioNotaFiscal.GerarComponenteISS(pedidoCTeParaSubContratacao, unitOfWork);

            repPedidoCTeParaSubContratacao.Atualizar(pedidoCTeParaSubContratacao);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);

            int countDocumentos = pedidoCTeParaSubContratacaoNotasFiscais.Count;

            for (var i = 0; i < countDocumentos; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotasFiscais[i].PedidoXMLNotaFiscal;

                ReverterRateioNFe(lancamentoNFSManual, pedidoXMLNotaFiscal, unitOfWork);
            }
        }

        private void ReverterRateioCargaCTe(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = (from obj in cargaCTe.NotasFiscais select obj.PedidoXMLNotaFiscal).ToList();

            if (pedidoXMLNotasFiscais.Any(o => o.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao))
            {
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCodigoPedidoXMLNotaFiscal(pedidoXMLNotasFiscais.Select(o => o.Codigo).ToArray());

                int countDocumentos = pedidoCTesParaSubcontratacao.Count;

                for (var i = 0; i < countDocumentos; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = pedidoCTesParaSubcontratacao[i];

                    ReverterRateioCTeSub(lancamentoNFSManual, pedidoCTeParaSubContratacao, unitOfWork);
                }
            }
            else
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

                int countDocumentos = pedidoXMLNotasFiscais.Count;

                for (var i = 0; i < countDocumentos; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoXMLNotasFiscais[i];

                    ReverterRateioNFe(lancamentoNFSManual, pedidoXMLNotaFiscal, unitOfWork);
                }
            }
        }
    }
}
