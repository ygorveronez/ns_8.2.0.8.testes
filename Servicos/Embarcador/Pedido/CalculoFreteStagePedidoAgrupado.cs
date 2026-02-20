using Dominio.Entidades;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Pedido
{
    public class CalculoFreteStagePedidoAgrupado
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido _configuracaoPedido;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos

        #region Construtores

        public CalculoFreteStagePedidoAgrupado(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void ProcessarFreteEGerarStagesAgrupadas(Dominio.Entidades.Embarcador.Cargas.Carga cargaDT, bool apenasVisualizando)
        {
            Repositorio.Embarcador.Pedidos.PedidoStage repositorioPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaPedidoStageRecalcular = repositorioPedidoStage.BuscarPorCargaDT(cargaDT.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDT = repositorioCargaPedido.BuscarPorCarga(cargaDT.Codigo);

            if (listaPedidoStageRecalcular.Count > 0)
                ProcessarFreteEGerarStagesAgrupadas(cargaDT, listaPedidoStageRecalcular.DistinctBy(s => s.Stage.Codigo).ToList(), cargaPedidosDT, false, false, apenasVisualizando);
        }

        public void ProcessarFreteEGerarStagesAgrupadas(Dominio.Entidades.Embarcador.Cargas.Carga cargaDT, List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDT, bool origemProcessarDT = false, bool tornarAssincrono = false, bool apenasVisualizando = false)
        {
            //PONTO DE LENTIDAO

            if (cargaDT?.TipoOperacao == null)
                return;

            if (apenasVisualizando)
                return;

            if (cargaDT.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                listaStagesPedido = listaStagesPedido.DistinctBy(x => x.Stage.Codigo).ToList();
            else
                listaStagesPedido.DistinctBy(x => x.Pedido.NumeroPedidoEmbarcador).Select(x => (x.Pedido.NumeroPedidoEmbarcador, x.Pedido.Remetente.CPF_CNPJ)).ToList();

            List<EnumTipoConsolidacao> consolidacoesPermitidas = new List<EnumTipoConsolidacao>() { EnumTipoConsolidacao.AutorizacaoEmissao, EnumTipoConsolidacao.PreCheckIn };
            int quantidadeStages = listaStagesPedido.Select(o => o.Stage).Distinct().Count();

            if (!consolidacoesPermitidas.Contains(cargaDT.TipoOperacao.TipoConsolidacao) || (quantidadeStages <= 1))
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever servicoIntegracaoUnilever = new Integracao.Unilever.IntegracaoUnilever(_unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Embarcador.Carga.RateioFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete repCargaTabelaFreteComponenteFrete = new Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!apenasVisualizando && cargaDT.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
            {
                Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new Servicos.Embarcador.Carga.ComplementoFrete(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponentesFrete.BuscarComplementosPorCarga(cargaDT.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFrete)
                    serComplementoFrete.ExtornarComplementoDeFrete(componente.CargaComplementoFrete, _tipoServicoMultisoftware, _unitOfWork);

                repCargaComponentesFrete.DeletarPorCarga(cargaDT.Codigo);

                repCargaTabelaFreteComponenteFrete.DeletarPorCarga(cargaDT.Codigo);
            }

            int ordemEntrega = listaStagesPedido.FirstOrDefault().Stage.OrdemEntrega;
            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupamentos = new List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            List<int> stagesComNumerosVeiculosDistintos = new List<int>();
            List<string> stagesSemVeiculosDistintos = new List<string>();

            if (cargaDT.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                stagesComNumerosVeiculosDistintos = listaStagesPedido.Select(obj => obj.Stage.NumeroVeiculo).Distinct().ToList();
            else
            {
                stagesComNumerosVeiculosDistintos = listaStagesPedido.Where(x => x.Stage.NumeroVeiculo > 0).Select(obj => obj.Stage.NumeroVeiculo).Distinct().ToList();
                stagesSemVeiculosDistintos = listaStagesPedido.Where(x => x.Stage.NumeroVeiculo == 0).Select(obj => obj.Stage.NumeroStage).Distinct().ToList();
            }

            foreach (int numeroVeiculo in stagesComNumerosVeiculosDistintos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedidoPorNumeroVeiculo = listaStagesPedido.Where(o => o.Stage.NumeroVeiculo == numeroVeiculo).ToList();
                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento = AdicionarOuAtualizarStageAgrupamento(cargaDT, listaStagesPedidoPorNumeroVeiculo, ordemEntrega, numeroVeiculo);

                CalcularFrete(stageAgrupamento, listaStagesPedidoPorNumeroVeiculo, cargaPedidosDT);

                stagesAgrupamentos.Add(stageAgrupamento);
            }

            foreach (string numeroStage in stagesSemVeiculosDistintos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedidoSemNumeroVeiculo = listaStagesPedido.Where(o => o.Stage.NumeroStage == numeroStage).ToList();
                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento = AdicionarOuAtualizarStageAgrupamento(cargaDT, listaStagesPedidoSemNumeroVeiculo, ordemEntrega, numeroVeiculo: 0);

                CalcularFrete(stageAgrupamento, listaStagesPedidoSemNumeroVeiculo, cargaPedidosDT);

                stagesAgrupamentos.Add(stageAgrupamento);
            }

            if (!cargaDT.CargaEmitidaParcialmente)
            {
                if (cargaDT.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
                {
                    cargaDT.PossuiPendencia = false;
                    cargaDT.MotivoPendencia = "";
                    cargaDT.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                    cargaDT.ValorFrete = 0;
                    cargaDT.ValorFreteAPagar = 0;
                    cargaDT.ValorICMS = 0;
                    cargaDT.ValorISS = 0;
                    cargaDT.ValorFreteLiquido = 0;
                    cargaDT.ValorPis = 0;
                }
                else
                {
                    cargaDT.PossuiPendencia = stagesAgrupamentos.Any(o => !string.IsNullOrWhiteSpace(o.MensagemRetornoDadosFrete));
                    cargaDT.MotivoPendencia = cargaDT.PossuiPendencia ? "Problema no calculo frete das Stages" : "";
                    cargaDT.MotivoPendenciaFrete = cargaDT.PossuiPendencia ? MotivoPendenciaFrete.ProblemaCalculoFrete : MotivoPendenciaFrete.NenhumPendencia;
                    cargaDT.ValorFrete = stagesAgrupamentos.Sum(o => o.ValorFreteTotal);
                    cargaDT.CalculandoFrete = false;
                    cargaDT.TipoFreteEscolhido = TipoFreteEscolhido.Embarcador;

                    Servicos.Embarcador.Carga.Frete servFrete = new Carga.Frete(_unitOfWork);

                    if (!apenasVisualizando && cargaDT.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                        servFrete.SumarizarComponentesFreteCargaTP(stagesAgrupamentos);

                    //O VALOR DO CUSTO FIXO (CONTRATO FRETE) PRECISA SER CALCULADO EM CADA AGRUPAMENTO E SUMARIZADO NA MAE.
                    //if (cargaDT.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao && !apenasVisualizando)
                    //{
                    //    Servicos.Embarcador.Carga.Frete servFrete = new Carga.Frete(_unitOfWork);
                    //    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = servFrete.AjustarValorFreteContratoFreteCargaTP(cargaDT, cargaPedidosDT, listaStagesPedido, _tipoServicoMultisoftware, _configuracaoEmbarcador);
                    //    if (dadosCalculoFrete.FreteCalculadoComProblemas)
                    //        cargaDT.MotivoPendencia = "Problema no calculo frete das Stages" + dadosCalculoFrete.MensagemRetorno;
                    //}

                    //PONTO DE LENTIDAO
                    servicoRateioFrete.RatearValorDoFrenteEntrePedidos(cargaDT, cargaPedidosDT, configuracaoEmbarcador, false, _unitOfWork, _tipoServicoMultisoftware);
                }
            }

            if (cargaDT.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                cargaDT.TipoFreteEscolhido = TipoFreteEscolhido.Embarcador;

                if (cargaDT.SituacaoCarga == SituacaoCarga.Nova)
                    cargaDT.SituacaoCarga = SituacaoCarga.CalculoFrete;
            }

            repositorioCarga.Atualizar(cargaDT);

            //Não deve integrar o precalculo até gerar transporte na etapa de transportador.
            if (cargaDT?.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn && !origemProcessarDT)
                servicoIntegracaoUnilever.IntegrarRetornoCargaPreCalculoStages(cargaDT, sucesso: !cargaDT.PossuiPendencia, retorno: string.Join(" ", stagesAgrupamentos.Select(o => o.MensagemRetornoDadosFrete)), tornarAssincrono);
        }

        public void ReprocessarFreteAgrupamento(Dominio.Entidades.Embarcador.Cargas.Carga cargaDT, Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage reppedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDT = repositorioCargaPedido.BuscarPorCarga(cargaDT.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.Stage> listaStagesdoAgrupamento = repStage.BuscarporAgrupamento(stageAgrupamento.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedidoPorNumeroVeiculo = reppedidoStage.BuscarPedidoStages(listaStagesdoAgrupamento.Select(x => x.Codigo).ToList());

            CalcularFrete(stageAgrupamento, listaStagesPedidoPorNumeroVeiculo, cargaPedidosDT);

        }

        public void RemoverStagesAgrupadasEcargasGeradas(Dominio.Entidades.Embarcador.Cargas.Carga cargaDT)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamentoComponenteFrete repStageAgrupamentoComponente = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete repStageAgrupamentoComposicao = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupamentos = repStageAgrupamento.BuscarPorCargaDt(cargaDT.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento in stagesAgrupamentos)
            {
                if (stageAgrupamento.CargaGerada != null)
                {
                    if (repositorioCargaCte.EstaCargaPossuiCte(stageAgrupamento.CargaGerada.Codigo))
                        throw new ServicoException($"Não foi possível finalizar essa ação a Carga possui subtrechos com CTes emitidos.");

                    stageAgrupamento.CargaGerada.SituacaoCarga = SituacaoCarga.Cancelada;
                    stageAgrupamento.CargaGerada.CargaFechada = false;

                    repositorioCarga.Atualizar(stageAgrupamento.CargaGerada);

                    repStageAgrupamentoComposicao.ExecuteDeletarPorAgrupamento(stageAgrupamento.Codigo);
                    repStageAgrupamentoComponente.ExecuteDeletarPorAgrupamento(stageAgrupamento.Codigo);
                    repStageAgrupamento.RemoverVinculoStagePorAgrupamento(stageAgrupamento.Codigo);
                    repStageAgrupamento.Deletar(stageAgrupamento);
                }
            }

            _unitOfWork.Flush();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void AdicionarComponenteFreteEmCargaPedidoDT(Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repCargaPedidoComponenteFrete.BuscarPorCargaPedido(cargaPedido.Codigo, false, cargaPedido.ModeloDocumentoFiscal);

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete componenteExiste = cargaPedidoComponentesFretes.Where(x => x.ComponenteFrete.TipoComponenteFrete == componente.TipoComponenteFrete).FirstOrDefault();

            if (componenteExiste == null)
                componenteExiste = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();

            componenteExiste.ValorComponente += (componente.DescontarValorTotalAReceber && componente.ValorComponente > 0) ? -componente.ValorComponente : componente.ValorComponente;
            componenteExiste.ComponenteFrete = componente.ComponenteFrete;
            componenteExiste.TipoComponenteFrete = componente.TipoComponenteFrete;
            componenteExiste.Percentual = componente.Percentual;
            componenteExiste.DescontarValorTotalAReceber = componente.DescontarValorTotalAReceber;
            componenteExiste.IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS;
            componenteExiste.NaoSomarValorTotalAReceber = componente.NaoSomarValorTotalAReceber;
            componenteExiste.DescontarDoValorAReceberValorComponente = componente.DescontarDoValorAReceberValorComponente;
            componenteExiste.DescontarDoValorAReceberOICMSDoComponente = componente.DescontarDoValorAReceberOICMSDoComponente;
            componenteExiste.ValorICMSComponenteDestacado = componente.ValorICMSComponenteDestacado;
            componenteExiste.NaoSomarValorTotalPrestacao = componente.NaoSomarValorTotalPrestacao;
            componenteExiste.TipoValor = componente.TipoValor;
            componenteExiste.CargaPedido = cargaPedido;
            componenteExiste.DescontarComponenteFreteLiquido = componente.DescontarComponenteFreteLiquido;
            componenteExiste.UtilizarFormulaRateioCarga = componente.UtilizarFormulaRateioCarga;

            if (componenteExiste.Codigo > 0)
                repCargaPedidoComponenteFrete.Atualizar(componenteExiste);
            else
                repCargaPedidoComponenteFrete.Inserir(componenteExiste);


        }

        private void AdicionarComponentesFrete(Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento, Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Repositorio.Embarcador.Pedidos.StageAgrupamentoComponenteFrete repositorioStageAgrupamentoComponente = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComponenteFrete(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete> listaComponentes = repositorioStageAgrupamentoComponente.BuscarPorAgrupamentoEComponente(stageAgrupamento.Codigo, componente.TipoComponenteFrete, componente.ComponenteFrete);
            Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete stageAgrupamentoComponenteFrete = null;

            if (listaComponentes.Count > 0)
                stageAgrupamentoComponenteFrete = listaComponentes.FirstOrDefault();

            if (stageAgrupamentoComponenteFrete == null)
                stageAgrupamentoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete();

            stageAgrupamentoComponenteFrete.StageAgrupamento = stageAgrupamento;
            stageAgrupamentoComponenteFrete.ComponenteFrete = componente.ComponenteFrete;
            stageAgrupamentoComponenteFrete.TipoComponenteFrete = componente.TipoComponenteFrete;
            stageAgrupamentoComponenteFrete.Percentual = componente.Percentual;
            stageAgrupamentoComponenteFrete.ValorComponente = (componente.DescontarValorTotalAReceber && componente.ValorComponente > 0) ? -componente.ValorComponente : componente.ValorComponente;
            stageAgrupamentoComponenteFrete.DescontarValorTotalAReceber = componente.DescontarValorTotalAReceber;
            stageAgrupamentoComponenteFrete.IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS;
            stageAgrupamentoComponenteFrete.NaoSomarValorTotalAReceber = componente.NaoSomarValorTotalAReceber;
            stageAgrupamentoComponenteFrete.NaoSomarValorTotalPrestacao = componente.NaoSomarValorTotalPrestacao;
            stageAgrupamentoComponenteFrete.TipoValor = componente.TipoValor;
            stageAgrupamentoComponenteFrete.DescontarComponenteFreteLiquido = componente.DescontarComponenteFreteLiquido;

            if (stageAgrupamentoComponenteFrete.Codigo > 0)
                repositorioStageAgrupamentoComponente.Atualizar(stageAgrupamentoComponenteFrete);
            else
                repositorioStageAgrupamentoComponente.Inserir(stageAgrupamentoComponenteFrete);

            AdicionarComponenteFreteEmCargaPedidoDT(componente, cargaPedidos.FirstOrDefault());
        }

        private void AdicionarComposicaoFrete(Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento, Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao)
        {
            Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete repositorioStageAgrupamentoComposicao = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete stageAgrupamentoComposicaoFrete = new Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete();

            stageAgrupamentoComposicaoFrete.StageAgrupamento = stageAgrupamento;
            stageAgrupamentoComposicaoFrete.Formula = composicao.Formula;
            stageAgrupamentoComposicaoFrete.ValoresFormula = composicao.ValoresFormula;
            stageAgrupamentoComposicaoFrete.DescricaoComponente = composicao.DescricaoComponente;
            stageAgrupamentoComposicaoFrete.Valor = composicao.Valor;
            stageAgrupamentoComposicaoFrete.ComponenteFrete = composicao.CodigoComponente > 0 ? new Dominio.Entidades.Embarcador.Frete.ComponenteFrete { Codigo = composicao.CodigoComponente } : null;
            stageAgrupamentoComposicaoFrete.ValorCalculado = composicao.ValorCalculado;
            stageAgrupamentoComposicaoFrete.TipoCampoValor = composicao.TipoValor;
            stageAgrupamentoComposicaoFrete.TipoParametro = composicao.TipoParametro;

            repositorioStageAgrupamentoComposicao.Inserir(stageAgrupamentoComposicaoFrete);
        }

        private Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento AdicionarOuAtualizarStageAgrupamento(Dominio.Entidades.Embarcador.Cargas.Carga cargaDT, List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagePedido, int ordemEntrega, int numeroVeiculo)
        {
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento = listaStagePedido.Where(o => o.Stage.StageAgrupamento != null).Select(o => o.Stage.StageAgrupamento).FirstOrDefault();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = ObterConfiguracaoPedido();

            if (stageAgrupamento != null)
                stageAgrupamento = repositorioStageAgrupamento.BuscarPorCodigo(stageAgrupamento.Codigo, false); //ver pra tirar isso

            if (stageAgrupamento == null)
                stageAgrupamento = new Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento();

            stageAgrupamento.CargaDT = cargaDT;
            stageAgrupamento.Expedidor = listaStagePedido.Select(o => o.Pedido?.Remetente ?? o.Stage.Expedidor)?.LastOrDefault();
            stageAgrupamento.NumeroVeiculo = numeroVeiculo;
            stageAgrupamento.ProcessadoPorPrechekin = false;

            if (numeroVeiculo == 0)
                stageAgrupamento.Processado = false;

            if (stageAgrupamento.CargaGerada != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
                Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga();
                Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                List<int> codigosPedidos = listaStagePedido.Select(o => o.Pedido.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = listaStagePedido.Select(o => o.Pedido).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> novosPedidosNaoAdicionadosCarga = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaGerada = repositorioCarga.BuscarPorCodigoFetch(stageAgrupamento.CargaGerada.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCargaGerada = repositorioCargaPedido.BuscarPorCargaEPedidos(cargaGerada.Codigo, codigosPedidos);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> todosCargaPedidoCargaGerada = repositorioCargaPedido.BuscarPorCarga(cargaGerada.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    if (!cargaPedidosCargaGerada.Any(o => o.Pedido.Codigo == pedido.Codigo))
                        novosPedidosNaoAdicionadosCarga.Add(pedido);
                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoNaoAdicionado in novosPedidosNaoAdicionadosCarga)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Stage stage = listaStagePedido.Where(o => o.Pedido.Codigo == pedidoNaoAdicionado.Codigo).Select(o => o.Stage).FirstOrDefault();
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidoProduto = repositorioPedidoProduto.BuscarPorPedido(pedidoNaoAdicionado.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido novoCargaPedido = servicoCargaWS.CriarCargaPedidoPorDocumentoTransporte(cargaGerada, pedidoNaoAdicionado, stage, null, configuracaoEmbarcador, _tipoServicoMultisoftware, null, _unitOfWork);

                    servicoCargaPedido.AdicionarProdutosCargaPedido(novoCargaPedido, listaPedidoProduto, configuracaoEmbarcador.UsarPesoProdutoSumarizacaoCarga, _unitOfWork);

                    if ((stage != null) && (novoCargaPedido.OrdemEntrega != stage.OrdemEntrega))
                    {
                        novoCargaPedido.OrdemEntrega = ordemEntrega;

                        if (novoCargaPedido.OrdemEntrega > 0)
                            cargaGerada.OrdemRoteirizacaoDefinida = true;
                    }

                    if ((cargaGerada.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.NaoConsolida) && (stage != null))
                    {
                        novoCargaPedido.Recebedor = stage.Recebedor;
                        novoCargaPedido.Expedidor = stage.Expedidor;
                    }

                    novoCargaPedido.CanalEntrega = stage?.CanalEntrega;
                    novoCargaPedido.CanalVenda = stage?.CanalVenda;

                    if (novoCargaPedido.Expedidor != null)
                        novoCargaPedido.Origem = novoCargaPedido.Expedidor.Localidade;

                    repositorioCargaPedido.Atualizar(novoCargaPedido);

                }

                if (novosPedidosNaoAdicionadosCarga.Count() > 0)
                {
                    //roteirizar a carga para quando inserido novo pedido atualizar no controle entrega e monitoramento;
                    if (cargaGerada.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                    {

                        Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(cargaGerada, todosCargaPedidoCargaGerada, _configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware);
                        servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(cargaGerada, todosCargaPedidoCargaGerada, _unitOfWork, _tipoServicoMultisoftware, configuracaoPedido);
                        servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaGerada, todosCargaPedidoCargaGerada, _configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware);
                        Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(cargaGerada, _unitOfWork, _configuracaoEmbarcador, _tipoServicoMultisoftware);

                        bool dadosTransporteInformados = (
                          (cargaGerada.TipoDeCarga != null) &&
                          (cargaGerada.ModeloVeicularCarga != null) &&
                          (cargaGerada.Veiculo != null) &&
                          (!(cargaGerada.TipoOperacao?.ExigePlacaTracao ?? false) || ((cargaGerada.VeiculosVinculados?.Count ?? 0) == cargaGerada.ModeloVeicularCarga.NumeroReboques)));

                        if (dadosTransporteInformados && todosCargaPedidoCargaGerada.FirstOrDefault()?.NotasFiscais?.Count > 0)
                        {
                            //carga filho de pre-chekin precisa calcular frete novamente
                            cargaGerada.SituacaoCarga = SituacaoCarga.CalculoFrete;
                            cargaGerada.PossuiPendencia = false;
                            cargaGerada.ProblemaIntegracaoValePedagio = false;
                            cargaGerada.MotivoPendencia = "";
                            cargaGerada.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                            cargaGerada.CalculandoFrete = true;
                            cargaGerada.DadosPagamentoInformadosManualmente = false;
                            cargaGerada.DataInicioCalculoFrete = DateTime.Now;
                        }

                        cargaGerada.PendenciaEmissaoAutomatica = false;
                        cargaGerada.CargaFechada = true;
                        repositorioCarga.Atualizar(cargaGerada);
                    }
                }
            }

            if (stageAgrupamento.Codigo > 0)
                repositorioStageAgrupamento.Atualizar(stageAgrupamento);
            else
                repositorioStageAgrupamento.Inserir(stageAgrupamento);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoStage stagePedido in listaStagePedido)
            {
                stagePedido.Stage.StageAgrupamento = stageAgrupamento;

                repositorioStage.Atualizar(stagePedido.Stage);
            }

            return stageAgrupamento;
        }

        private void CalcularFrete(Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento, List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagePedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDT)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete repositorioStageAgrupamentoComposicao = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = ObterDadosCalculoFrete(stageAgrupamento.CargaDT, listaStagePedido);
            bool problemaAoCalcularFrete = (!dadosCalculoFrete.FreteCalculado || dadosCalculoFrete.FreteCalculadoComProblemas);
            List<string> numerosStages = (stageAgrupamento.NumeroVeiculo > 0) ? listaStagePedido.Select(o => o.Stage.NumeroStage).ToList() : repositorioStage.ObterNumerosStagesPorAgrupamento(stageAgrupamento.Codigo);
            List<int> codigosPedidos = listaStagePedido.Select(o => o.Pedido.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaPedidosDT.Where(o => codigosPedidos.Contains(o.Pedido.Codigo)).ToList();

            stageAgrupamento.MensagemRetornoDadosFrete = problemaAoCalcularFrete ? $@"Stages: {string.Join(", ", numerosStages)} - {dadosCalculoFrete.MensagemRetorno}." + " Data: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") : "";
            stageAgrupamento.ValorComponentes = dadosCalculoFrete.Componentes.Sum(o => o.ValorComponente);
            stageAgrupamento.ValorFreteTotal = dadosCalculoFrete.ValorTotal;

            repositorioStageAgrupamentoComposicao.ExecuteDeletarPorAgrupamento(stageAgrupamento.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in dadosCalculoFrete.Componentes)
                AdicionarComponentesFrete(stageAgrupamento, componente, cargaPedidos);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao in dadosCalculoFrete.ComposicaoFrete)
                AdicionarComposicaoFrete(stageAgrupamento, composicao);

            if (stageAgrupamento.CargaGerada != null && dadosCalculoFrete.FreteCalculado)
            {
                stageAgrupamento.CargaGerada.ValorFrete = stageAgrupamento.ValorFreteTotal;
                stageAgrupamento.CargaGerada.ValorFreteAPagar = stageAgrupamento.ValorFreteTotal;
                repositorioCarga.Atualizar(stageAgrupamento.CargaGerada);
            }

            repositorioStageAgrupamento.Atualizar(stageAgrupamento);
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido ObterConfiguracaoPedido()
        {
            if (_configuracaoPedido == null)
                _configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoPedido;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete ObterDadosCalculoFrete(Dominio.Entidades.Embarcador.Cargas.Carga cargaDT, List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagePedido)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Servicos.Embarcador.Carga.Frete servFrete = new Carga.Frete(_tipoServicoMultisoftware);

            List<int> codigosPedidos = listaStagePedido.Select(o => o.Pedido.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = listaStagePedido.Select(o => o.Pedido).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = repositorioPedidoProduto.BuscarPorPedidos(codigosPedidos);
            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = Servicos.Embarcador.Carga.Frete.ObterParametrosCalculoFretePorAgrupamentoStages(listaStagePedido, pedidos, pedidoProdutos, cargaDT);
            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = Servicos.Embarcador.Carga.Frete.ObterDadosCalculoFreteMontagemCarga(parametrosCalculo, pedidos, pedidoProdutos, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, configuracaoEmbarcador);

            if (cargaDT.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao && (!dadosCalculoFrete.FreteCalculado || dadosCalculoFrete.FreteCalculadoComProblemas))
            {
                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelaFrete = Servicos.Embarcador.Carga.Frete.ObterTabelasFrete(parametrosCalculo, false, out StringBuilder mensagem, _unitOfWork, _tipoServicoMultisoftware, 0);
                if (tabelaFrete.Count == 0)
                    return dadosCalculoFrete;

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFreteUsar = tabelaFrete.FirstOrDefault();

                if (tabelaFreteUsar.TipoCalculo != TipoCalculoTabelaFrete.PorMaiorDistanciaPedidoAgrupados)
                    return dadosCalculoFrete;


                List<Localidade> destinos = parametrosCalculo.Destinos.ToList();
                decimal distanciaTotal = listaStagePedido.Select(x => x.Stage.Distancia).Sum();
                decimal maiorDistancia = listaStagePedido.Where(x => x.Stage.TipoPercurso != Vazio.PercursoRegreso).Select(x => x.Stage.Distancia).Max();
                listaStagePedido = listaStagePedido.Where(x => x.Stage.Distancia == maiorDistancia).ToList();

                if (listaStagePedido.Count > 1)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoStage pedidostageFirst = listaStagePedido.Where(x => x.Stage.Distancia == maiorDistancia).FirstOrDefault();
                    listaStagePedido.Clear();
                    listaStagePedido.Add(pedidostageFirst);
                }

                pedidos = listaStagePedido.Select(x => x.Pedido).ToList();
                pedidoProdutos = pedidoProdutos.Where(x => pedidos.Contains(x.Pedido)).ToList();

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete novosParamentros = Servicos.Embarcador.Carga.Frete.ObterParametrosCalculoFretePorAgrupamentoStages(listaStagePedido, pedidos, pedidoProdutos, cargaDT);
                novosParamentros.Distancia = distanciaTotal;

                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoAtual = Servicos.Embarcador.Carga.Frete.ObterDadosCalculoFreteMontagemCarga(novosParamentros, pedidos, pedidoProdutos, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, configuracaoEmbarcador);
                return dadosCalculoAtual;
            }

            return dadosCalculoFrete;
        }


        #endregion Métodos Privados
    }
}
