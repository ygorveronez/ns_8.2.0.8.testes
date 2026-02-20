using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class RecusaCargaCTe : ServicoBase
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;        
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado;
        #endregion

        #region Construtores

        public RecusaCargaCTe(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, tipoServicoMultisoftware)
        {            
            _configuracaoEmbarcador = configuracaoEmbarcador;
            Auditado = auditado;
        }        

        #endregion

        #region Métodos Públicos

        public void CriarCargaCtePedidoRecusa(int protocoloPedido, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoCargaGerada, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoCargaDT, Dominio.Entidades.Embarcador.Cargas.Carga cargaGerada, Dominio.Entidades.Embarcador.Cargas.Carga cargaDt)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE cargaPedidoRecusaCTE = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaPedidoRecusaCTE.BuscarPorCodigoPedido(protocoloPedido);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaCargaFiscaisCargaGerada = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedidoCargaGerada.Codigo);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteCargaGerada = repositorioCargaCTe.BuscarPorCargaECTe(cargaGerada.Codigo, cte.Codigo);
            if (cargaCteCargaGerada == null)
            {
                cargaCteCargaGerada = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();
                cargaCteCargaGerada.CargaCTeSubContratacaoFilialEmissora = null;
                cargaCteCargaGerada.Carga = cargaGerada;
                cargaCteCargaGerada.CargaOrigem = cargaGerada;
                cargaCteCargaGerada.CTe = cte;
                cargaCteCargaGerada.DataVinculoCarga = DateTime.Now;
                repositorioCargaCTe.Inserir(cargaCteCargaGerada);
            }

            ReplicarNotasCargaCte(pedidoXMLNotaCargaFiscaisCargaGerada, cargaCteCargaGerada);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaCargaFiscaisDT = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedidoCargaDT.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteCargaDt = repositorioCargaCTe.BuscarPorCargaECTe(cargaDt.Codigo, cte.Codigo);
            if (cargaCteCargaDt == null)
            {
                cargaCteCargaDt = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();
                cargaCteCargaDt.CargaCTeSubContratacaoFilialEmissora = null;
                cargaCteCargaDt.Carga = cargaDt;
                cargaCteCargaDt.CargaOrigem = cargaDt;
                cargaCteCargaDt.CTe = cte;
                cargaCteCargaDt.DataVinculoCarga = DateTime.Now;
                repositorioCargaCTe.Inserir(cargaCteCargaDt);
            }

            ReplicarNotasCargaCte(pedidoXMLNotaCargaFiscaisDT, cargaCteCargaDt);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoxmlNotaFiscalGerado in pedidoXMLNotaCargaFiscaisCargaGerada)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoxmlNotaFIscalMaeCorrespondente = pedidoXMLNotaCargaFiscaisDT.Where(x => x.XMLNotaFiscal.Codigo == pedidoxmlNotaFiscalGerado.XMLNotaFiscal.Codigo).FirstOrDefault();
                if (pedidoxmlNotaFIscalMaeCorrespondente != null)
                {
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorFrete = pedidoxmlNotaFiscalGerado.ValorFrete;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorICMS = pedidoxmlNotaFiscalGerado.ValorICMS;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorISS = pedidoxmlNotaFiscalGerado.ValorISS;
                    pedidoxmlNotaFIscalMaeCorrespondente.CST = pedidoxmlNotaFiscalGerado.CST;
                    pedidoxmlNotaFIscalMaeCorrespondente.CFOP = pedidoxmlNotaFiscalGerado.CFOP;
                    pedidoxmlNotaFIscalMaeCorrespondente.BaseCalculoICMS = pedidoxmlNotaFiscalGerado.BaseCalculoICMS;
                    pedidoxmlNotaFIscalMaeCorrespondente.BaseCalculoISS = pedidoxmlNotaFiscalGerado.BaseCalculoISS;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorCofins = pedidoxmlNotaFiscalGerado.ValorCofins;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorPis = pedidoxmlNotaFiscalGerado.ValorPis;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorTotalComponentes = pedidoxmlNotaFiscalGerado.ValorTotalComponentes;
                    pedidoxmlNotaFIscalMaeCorrespondente.PercentualAliquota = pedidoxmlNotaFiscalGerado.PercentualAliquota;
                    pedidoxmlNotaFIscalMaeCorrespondente.IncluirICMSBaseCalculo = pedidoxmlNotaFiscalGerado.IncluirICMSBaseCalculo;
                    pedidoxmlNotaFIscalMaeCorrespondente.IncluirISSBaseCalculo = pedidoxmlNotaFiscalGerado.IncluirISSBaseCalculo;
                    pedidoxmlNotaFIscalMaeCorrespondente.PercentualAliquotaISS = pedidoxmlNotaFiscalGerado.PercentualAliquotaISS;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorRetencaoISS = pedidoxmlNotaFiscalGerado.ValorRetencaoISS;
                    pedidoxmlNotaFIscalMaeCorrespondente.BaseCalculoIBSCBS = pedidoxmlNotaFiscalGerado.BaseCalculoIBSCBS;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorCBS = pedidoxmlNotaFiscalGerado.ValorCBS;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorIBSEstadual = pedidoxmlNotaFiscalGerado.ValorIBSEstadual;
                    pedidoxmlNotaFIscalMaeCorrespondente.ValorIBSMunicipal = pedidoxmlNotaFiscalGerado.ValorIBSMunicipal;
                    pedidoxmlNotaFIscalMaeCorrespondente.AliquotaCBS = pedidoxmlNotaFiscalGerado.AliquotaCBS;
                    pedidoxmlNotaFIscalMaeCorrespondente.AliquotaIBSEstadual = pedidoxmlNotaFiscalGerado.AliquotaIBSEstadual;
                    pedidoxmlNotaFIscalMaeCorrespondente.AliquotaIBSMunicipal = pedidoxmlNotaFiscalGerado.AliquotaIBSMunicipal;
                    pedidoxmlNotaFIscalMaeCorrespondente.PercentualReducaoIBSEstadual = pedidoxmlNotaFiscalGerado.PercentualReducaoIBSEstadual;
                    pedidoxmlNotaFIscalMaeCorrespondente.PercentualReducaoIBSMunicipal = pedidoxmlNotaFiscalGerado.PercentualReducaoIBSMunicipal;
                    pedidoxmlNotaFIscalMaeCorrespondente.PercentualReducaoCBS = pedidoxmlNotaFiscalGerado.PercentualReducaoCBS;
                    pedidoxmlNotaFIscalMaeCorrespondente.OutrasAliquotas = pedidoxmlNotaFiscalGerado.OutrasAliquotas;
                    pedidoxmlNotaFIscalMaeCorrespondente.NBS = pedidoxmlNotaFiscalGerado.NBS;
                    pedidoxmlNotaFIscalMaeCorrespondente.CodigoIndicadorOperacao = pedidoxmlNotaFiscalGerado.CodigoIndicadorOperacao;
                    pedidoxmlNotaFIscalMaeCorrespondente.CSTIBSCBS = pedidoxmlNotaFiscalGerado.CSTIBSCBS;
                    pedidoxmlNotaFIscalMaeCorrespondente.ClassificacaoTributariaIBSCBS = pedidoxmlNotaFiscalGerado.ClassificacaoTributariaIBSCBS;

                    repPedidoXMLNotaFiscal.Atualizar(pedidoxmlNotaFIscalMaeCorrespondente);

					Servicos.Log.TratarErro($"Atualizando CriarCargaCtePedidoRecusa pedidoXmlNotaFiscal = {pedidoxmlNotaFIscalMaeCorrespondente.Codigo} com valorISS = {pedidoxmlNotaFIscalMaeCorrespondente.ValorISS} e incluirISSBaseCalculo = {pedidoxmlNotaFIscalMaeCorrespondente.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoxmlNotaFIscalMaeCorrespondente.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");

					Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoxmlNotaFIscalMaeCorrespondente, false, _tipoServicoMultisoftware, _unitOfWork);
                }
            }
        }

        public void RemoverCteRecusaCargaPreChekin(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE repositorioCargaPedidoRecusaCte = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeRecusa = repositorioCargaCTe.BuscarPorCargaPedido(cargaPedido.Codigo);

            if (cargaCTeRecusa != null)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTeRecusa.CTe;
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE pedidoRecusaCte = repositorioCargaPedidoRecusaCte.BuscarRecusaPorPedido(cargaPedido.Pedido.Protocolo);

                repositorioCargaPedidoRecusaCte.DeletarCargaCTERecusado(cargaCTeRecusa.Codigo);

                //buscar os cargaCte do cteRecusado e deletar.
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repositorioCargaCTe.BuscarTodosPorCTe(cte.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteDeletar in listaCargaCTe)
                    repositorioCargaPedidoRecusaCte.DeletarCargaCTERecusado(cargaCteDeletar.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLnotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCarga(pedidoRecusaCte.CargaRecusaGerada.Codigo);

                List<int> codigosNotas = (from obj in pedidosXMLnotaFiscal select obj.XMLNotaFiscal.Codigo).Distinct().ToList();

                if (codigosNotas.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repDocumentoProvisao.BuscarPorCargaENotas(cargaPedido.Carga.Codigo, codigosNotas);
                    foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao in documentosProvisao)
                    {
                        documentoProvisao.Situacao = SituacaoProvisaoDocumento.Cancelado;
                        documentoProvisao.Carga = pedidoRecusaCte.CargaRecusaGerada;
                        repDocumentoProvisao.Atualizar(documentoProvisao);
                    }
                }

                pedidoRecusaCte.PedidoRemovido = true;
                repositorioCargaPedidoRecusaCte.Atualizar(pedidoRecusaCte);
            }

            //Resumariza frete da carga mae e calcula frete da carga de proximo trecho (transferencia).
            SumarizarValoresFreteAoRemoverRecusaCTePreChecking(cargaPedido.Carga, cargaPedido, configuracaoGeralCarga);

        }

        #endregion

        #region Metodos Privados

        private void ReplicarNotasCargaCte(List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaCargaFiscais, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaCargaFiscais)
            {
                if (!repCargaPedidoXMLNotaFiscalCTe.VerificarSeExisteCargaCTePorPedidoXMLNotaECargaCTe(pedidoXMLNotaFiscal.Codigo, cargaCTe.Codigo))
                {

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoCteXMLNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                    cargaPedidoCteXMLNotaFiscal.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                    cargaPedidoCteXMLNotaFiscal.CargaCTe = cargaCTe;
                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoCteXMLNotaFiscal);
                }
            }
        }


        private void SumarizarValoresFreteAoRemoverRecusaCTePreChecking(Dominio.Entidades.Embarcador.Cargas.Carga cargaMae, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE cargaPedidoRecusaCTE = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(_unitOfWork);

            if (cargaMae != null && cargaMae.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCargaPai = repCargaPedido.BuscarPorCarga(cargaMae.Codigo);

                cargaMae.ValorFrete = cargaPedidosCargaPai.Where(x => x.Codigo != cargaPedido.Codigo).Sum(x => x.ValorFrete);
                cargaMae.ValorBaseFrete = cargaPedidosCargaPai.Where(x => x.Codigo != cargaPedido.Codigo).Sum(x => x.ValorBaseFrete);
                cargaMae.ValorFreteAPagar = cargaPedidosCargaPai.Where(x => x.Codigo != cargaPedido.Codigo).Sum(x => x.ValorFreteAPagar);
                cargaMae.ValorCofins = cargaPedidosCargaPai.Where(x => x.Codigo != cargaPedido.Codigo).Sum(x => x.ValorCofins);
                cargaMae.ValorPis = cargaPedidosCargaPai.Where(x => x.Codigo != cargaPedido.Codigo).Sum(x => x.ValorPis);
                cargaMae.ValorISS = cargaPedidosCargaPai.Where(x => x.Codigo != cargaPedido.Codigo).Sum(x => x.ValorISS);
                cargaMae.ValorFreteLiquido = cargaPedidosCargaPai.Where(x => x.Codigo != cargaPedido.Codigo).Sum(x => x.ValorFrete);
                repCarga.Atualizar(cargaMae);

                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargaMae, cargaPedido, _configuracaoEmbarcador, _tipoServicoMultisoftware, _unitOfWork, configuracaoGeralCarga, null, true, true);

                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentosDemaisCargas = repStageAgrupamento.BuscarPorCargaDt(cargaMae.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in agrupamentosDemaisCargas)
                {
                    if (agrupamento.CargaGerada != null)
                    {
                        //deletar os carga pedido das cargas geradas.  
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidosCargaGerada = repCargaPedido.BuscarPorCargaEPedido(agrupamento.CargaGerada.Codigo, cargaPedido.Pedido.Codigo);

                        if (cargaPedidosCargaGerada?.CargaPedidoProximoTrecho != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRemover = cargaPedidosCargaGerada?.CargaPedidoProximoTrecho;
                            cargaPedidosCargaGerada.CargaPedidoProximoTrecho = null;
                            repCargaPedido.Atualizar(cargaPedidosCargaGerada);

                            Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargaPedidoRemover.Carga, cargaPedidoRemover, _configuracaoEmbarcador, _tipoServicoMultisoftware, _unitOfWork, configuracaoGeralCarga, null, true, true);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> todosCargaPedidoCargaGerada = repCargaPedido.BuscarPorCarga(agrupamento.CargaGerada.Codigo);
                            Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(agrupamento.CargaGerada, todosCargaPedidoCargaGerada, _configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware);
                            Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(agrupamento.CargaGerada, _unitOfWork, _configuracaoEmbarcador, _tipoServicoMultisoftware);

                            //Carga de consolidado recalcular o frete da carga gerada apenas se ja tem veiculo, e motoristas e nao esta avançada..
                            bool dadosTransporteInformados = (
                                    (agrupamento.CargaGerada.TipoDeCarga != null) &&
                                    (agrupamento.CargaGerada.ModeloVeicularCarga != null) &&
                                    (agrupamento.CargaGerada.Veiculo != null) &&
                                    (!(agrupamento.CargaGerada.TipoOperacao?.ExigePlacaTracao ?? false) || ((agrupamento.CargaGerada.VeiculosVinculados?.Count ?? 0) == agrupamento.CargaGerada.ModeloVeicularCarga.NumeroReboques)));

                            if (dadosTransporteInformados && todosCargaPedidoCargaGerada.FirstOrDefault()?.NotasFiscais?.Count > 0 && agrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.AgIntegracao && agrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.EmTransporte && agrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos)
                            {
                                agrupamento.CargaGerada.SituacaoCarga = SituacaoCarga.CalculoFrete;
                                agrupamento.CargaGerada.CalculandoFrete = true;
                                agrupamento.CargaGerada.PossuiPendencia = false;
                                agrupamento.CargaGerada.ProblemaIntegracaoValePedagio = false;
                                agrupamento.CargaGerada.MotivoPendencia = "";
                                agrupamento.CargaGerada.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                                agrupamento.CargaGerada.DadosPagamentoInformadosManualmente = false;
                                agrupamento.CargaGerada.DataInicioCalculoFrete = DateTime.Now;
                                agrupamento.CargaGerada.PendenciaEmissaoAutomatica = false;
                            }
                            else
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoCargaGeradaRestantes = repCargaPedido.BuscarPorCarga(agrupamento.CargaGerada.Codigo);

                                agrupamento.CargaGerada.ValorFrete = cargaPedidoCargaGeradaRestantes.Sum(x => x.ValorFrete);
                                agrupamento.CargaGerada.ValorBaseFrete = cargaPedidoCargaGeradaRestantes.Sum(x => x.ValorBaseFrete);
                                agrupamento.CargaGerada.ValorFreteAPagar = cargaPedidoCargaGeradaRestantes.Sum(x => x.ValorFreteAPagar);
                                agrupamento.CargaGerada.ValorCofins = cargaPedidoCargaGeradaRestantes.Sum(x => x.ValorCofins);
                                agrupamento.CargaGerada.ValorPis = cargaPedidoCargaGeradaRestantes.Sum(x => x.ValorPis);
                                agrupamento.CargaGerada.ValorISS = cargaPedidoCargaGeradaRestantes.Sum(x => x.ValorISS);
                                agrupamento.CargaGerada.ValorFreteLiquido = cargaPedidoCargaGeradaRestantes.Sum(x => x.ValorFrete);

                            }

                            repCarga.Atualizar(agrupamento.CargaGerada);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
