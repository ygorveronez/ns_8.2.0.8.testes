using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public sealed class PrevisaoControleEntrega
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega _configuracaoControleEntrega;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao _configuracaoCalculoPrevisao;
        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega _cargaEntregaAtual;
        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ParametrosConfiguracaoCalculoPrevisaoEntrega _parametrosConfiguracaoCalculo;

        #endregion Atributos

        #region Construtores

        public PrevisaoControleEntrega(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, parametrosConfiguracaoCalculo: null) { }

        public PrevisaoControleEntrega(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, parametrosConfiguracaoCalculo: null) { }

        public PrevisaoControleEntrega(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ParametrosConfiguracaoCalculoPrevisaoEntrega parametrosConfiguracaoCalculo)
        {
            _parametrosConfiguracaoCalculo = parametrosConfiguracaoCalculo ?? new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ParametrosConfiguracaoCalculoPrevisaoEntrega();

            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos públicos

        public DateTime CalcularDataChegadaPrevista(DateTime dataInicio, Double distanciaMetros, Double velocidadeKmH)
        {
            return ObterDataInicialCalculada(dataInicio, distanciaMetros, velocidadeKmH, carga: null, tempoExtraEntregaMinutos: 0);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> CalcularPrevisoesEntregas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repositorioCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            if (cargaRotaFrete == null)
                return new List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega>();

            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagens = repositorioCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = ConverterCargaPontosPassagemParaObjetoDeValor(cargaRotaFretePontosPassagens, carga.TipoOperacao);

            CalcularPrevisoesEntregas(carga, previsoesCargaEntrega, cargaRotaFrete, centrosDescarregamento, cargaJanelasDescarregamento: null, dataPosicaoAtual: null, distanciaPosicaoAtualAteProximaEntrega: 0, velocidadeMediaCalculada: 0);

            return previsoesCargaEntrega;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> CalcularPrevisoesEntregas(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos)
        {
            if (carga.DataFimViagem != null)
                return null;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento = repositorioCargaJanelaDescarregamento.BuscarTodasPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatarios((from obj in cargasEntrega where obj.Cliente != null select obj.Cliente.CPF_CNPJ).Distinct().ToList());
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = ConverterCargaEntregasParaObjetoDeValor(cargasEntrega, cargaEntregaPedidos);

            CalcularPrevisoesEntregas(carga, previsoesCargaEntrega, cargaRotaFrete, centrosDescarregamento, cargaJanelasDescarregamento, dataPosicaoAtual: null, distanciaPosicaoAtualAteProximaEntrega: 0, velocidadeMediaCalculada: 0);

            return previsoesCargaEntrega;
        }

        //metodo usado pelo SGT.Monitoramento (ao processar monitoramentos)
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> CalcularPrevisoesEntregasComPosicao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega)
        {
            if (carga.DataFimViagem != null)
                return null;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento = repositorioCargaJanelaDescarregamento.BuscarTodasPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatarios((from obj in cargasEntrega where obj.Cliente != null select obj.Cliente.CPF_CNPJ).Distinct().ToList());
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = ConverterCargaEntregasParaObjetoDeValor(cargasEntrega, cargaEntregaPedidos);
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega proximaEntrega = ObterPrimeiraEntregaPendente(previsoesCargaEntrega);

            if (proximaEntrega?.Cliente == null)
                return previsoesCargaEntrega;

            int velocidadeMediaUltimasNPosicoes = 0;
            if (configuracaoControleEntrega?.ConsiderarMediaDeVelocidadeDasUltimasCincoPosicoes ?? false)
                try { velocidadeMediaUltimasNPosicoes = repPosicao.ObterVelocidadeMediaUltimasNPosicoes(carga.Veiculo.Codigo, 5, carga.DataCriacaoCarga); } 
                catch (Exception ex) 
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter velocidade média das últimas posições: {ex.ToString()}", "CatchNoAction");
                }

            Servicos.Embarcador.Logistica.Roteirizacao servicoRoteirizacao = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointPosicaoAtual = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(posicao.Latitude, posicao.Longitude);
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointProximaEntrega = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(proximaEntrega.Cliente.Latitude, proximaEntrega.Cliente.Longitude);

            servicoRoteirizacao.Add(wayPointPosicaoAtual);
            servicoRoteirizacao.Add(wayPointProximaEntrega);

            int distanciaPosicaoAtualAteProximaEntrega = (int)(servicoRoteirizacao.BuscarDistancia() * 1000);

            CalcularPrevisoesEntregas(carga, previsoesCargaEntrega, cargaRotaFrete, centrosDescarregamento, cargaJanelasDescarregamento, posicao.DataVeiculo, distanciaPosicaoAtualAteProximaEntrega, velocidadeMediaUltimasNPosicoes);

            return previsoesCargaEntrega;
        }

        public DateTime ObterDataConsiderandoFinalSemanaFeriados(DateTime dataBase, bool aplicarHorarioJornada, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade localidade = null)
        {
            Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao configuracaoCalculoPrevisao = ObterConfiguracaoCalculoPrevisao();

            bool periodoUtilConfigurado = ((configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial > TimeSpan.Zero) && (configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal > TimeSpan.Zero));

            while (
                (((configuracaoCalculoPrevisao?.DesconsiderarSabadosCalculoPrevisao ?? false) || (carga?.TipoOperacao?.ConfiguracaoControleEntrega?.DesconsiderarSabadosCalculoPrevisao ?? false)) && dataBase.DayOfWeek == DayOfWeek.Saturday) ||
                (((configuracaoCalculoPrevisao?.DesconsiderarDomingosCalculoPrevisao ?? false) || (carga?.TipoOperacao?.ConfiguracaoControleEntrega?.DesconsiderarDomingosCalculoPrevisao ?? false)) && dataBase.DayOfWeek == DayOfWeek.Sunday) ||
                (((configuracaoCalculoPrevisao?.DesconsiderarFeriadosCalculoPrevisao ?? false) || (carga?.TipoOperacao?.ConfiguracaoControleEntrega?.DesconsiderarFeriadosCalculoPrevisao ?? false)) && servicoFeriado.VerificarSePossuiFeriado(dataBase, localidade)))
            {
                dataBase = dataBase.AddDays(1);

                if (periodoUtilConfigurado && aplicarHorarioJornada)
                    dataBase = dataBase.Date.Add(configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial);
            }

            return dataBase;
        }

        public void CalcularDataAgendamentoColetaEntregaAutomatico(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listCargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);
            if (listCargaEntregas == null || listCargaEntregas.Count() == 0)
                return;

            listCargaEntregas = listCargaEntregas.Where(x => x.DataAgendamento == null && !x.Coleta).ToList();

            DateTime dataFaturamentoBase;
            foreach (var entrega in listCargaEntregas)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasEntrega = repXmlNotaFiscal.BuscarPorCargaEntrega(entrega.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> pedidosEntrega = repCargaEntregaPedido.BuscarPorCargaEntrega(entrega.Codigo);

                if (!notasEntrega.Any())
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = pedidosEntrega.Select(x => x.CargaPedido.Pedido).ToList();
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotas = repPedidoXmlNotaFiscal.BuscarPorPedidos(pedidos.Select(x => x.Codigo).ToList());
                    dataFaturamentoBase = pedidoXMLNotas.FirstOrDefault().XMLNotaFiscal.DataEmissao;
                }
                else
                    dataFaturamentoBase = notasEntrega.FirstOrDefault().DataEmissao;

                int leadTimePedido = pedidosEntrega.FirstOrDefault()?.CargaPedido?.Pedido.DiasUteisPrazoTransportador ?? 0;
                DateTime dataAgendamentoEntrega = DateTime.MinValue;

                for (int i = 1; i <= leadTimePedido; i++)
                {
                    dataFaturamentoBase = ObterDataConsiderandoFinalSemana(dataFaturamentoBase.AddDays(1), false);
                }

                dataAgendamentoEntrega = ObterDataConsiderandoFinalSemana(dataFaturamentoBase.AddHours(5), true);//pegar o centro de carregamento da filial...

                entrega.DataAgendamento = dataAgendamentoEntrega;

                foreach (var pedido in pedidosEntrega)
                {
                    if (pedido.CargaPedido?.Pedido != null && !pedido.CargaPedido.Pedido.DataAgendamento.HasValue)
                    {
                        pedido.CargaPedido.Pedido.DataAgendamento = dataAgendamentoEntrega;
                        repPedido.Atualizar(pedido.CargaPedido.Pedido);
                    }
                }

                repCargaEntrega.Atualizar(entrega);
            }

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega coleta = repCargaEntrega.BuscarPrimeiraCargaEntregaColetaPorCarga(carga.Codigo);

            if (coleta == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraEntrega = repCargaEntrega.BuscarPrimeiraCargaEntregaEntregaPorCarga(carga.Codigo);

            if (primeiraEntrega == null || !primeiraEntrega.DataAgendamento.HasValue)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> pedidosColeta = repCargaEntregaPedido.BuscarPorCargaEntrega(coleta.Codigo);

            coleta.DataAgendamento = CalcularDataAgendamentoColeta(coleta, primeiraEntrega);

            foreach (var pedido in pedidosColeta)
            {
                if (pedido.CargaPedido?.Pedido != null && !pedido.CargaPedido.Pedido.DataCarregamentoPedido.HasValue)
                {
                    pedido.CargaPedido.Pedido.DataCarregamentoPedido = coleta.DataAgendamento;
                    repPedido.Atualizar(pedido.CargaPedido.Pedido);
                }
            }

            repCargaEntrega.Atualizar(coleta);

        }

        public void CalcularDataAgendamentoColetaAoFecharCargaAutomatico(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega coleta = repCargaEntrega.BuscarPrimeiraCargaEntregaColetaPorCarga(carga.Codigo);

            if (coleta == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraEntrega = repCargaEntrega.BuscarPrimeiraCargaEntregaEntregaPorCarga(carga.Codigo);

            if ((primeiraEntrega == null) || !primeiraEntrega.DataAgendamento.HasValue)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> coletasDaCarga = repCargaEntrega.BuscarPorCarga(carga.Codigo);
            coletasDaCarga = coletasDaCarga.Where(x => x.Coleta).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> pedidosColeta = repCargaEntregaPedido.BuscarPorCargaEntregas(coletasDaCarga.Select(x => x.Codigo).ToList());

            foreach (var coletacarga in coletasDaCarga)
            {
                coletacarga.DataAgendamento = CalcularDataAgendamentoColeta(coletacarga, primeiraEntrega);

                foreach (var pedido in pedidosColeta)
                {
                    if (pedido.CargaPedido?.Pedido != null)
                    {
                        pedido.CargaPedido.Pedido.DataCarregamentoPedido = coletacarga.DataAgendamento;
                        repPedido.Atualizar(pedido.CargaPedido.Pedido);
                    }
                }
                repCargaEntrega.Atualizar(coletacarga);
            }
        }

        #endregion

        #region Métodos privados

        private void AdicionarComposicaoDataFimEntregaPrevistaAtual(string detalhe)
        {
            if (!_parametrosConfiguracaoCalculo.ArmazenarComposicoesPrevisoes || (_cargaEntregaAtual == null))
                return;

            if (_cargaEntregaAtual.ComposicaoPrevisao == null)
                _cargaEntregaAtual.ComposicaoPrevisao = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaComposicaoPrevisao();

            _cargaEntregaAtual.ComposicaoPrevisao.DetalhesDataFimEntregaPrevista.Add(detalhe);
        }

        private void AdicionarComposicaoDataInicioEntregaPrevistaAtual(string detalhe)
        {
            if (!_parametrosConfiguracaoCalculo.ArmazenarComposicoesPrevisoes || (_cargaEntregaAtual == null))
                return;

            if (_cargaEntregaAtual.ComposicaoPrevisao == null)
                _cargaEntregaAtual.ComposicaoPrevisao = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaComposicaoPrevisao();

            _cargaEntregaAtual.ComposicaoPrevisao.DetalhesDataInicioEntregaPrevista.Add(detalhe);
        }

        private void CalcularPrevisoesEntregas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento, DateTime? dataPosicaoAtual, int distanciaPosicaoAtualAteProximaEntrega, int velocidadeMediaCalculada = 0)
        {
            if (previsoesCargaEntrega.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoDeCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso repositorioTipoCargaTempoDescargaFaixaPeso = new Repositorio.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular = repositorioTipoDeCargaModeloVeicular.ConsultarPorModeloVeicular(carga.TipoDeCarga?.Codigo ?? 0, carga?.ModeloVeicularCarga?.Codigo ?? 0);
            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso> tiposCargaTempoDescargaFaixaPeso = repositorioTipoCargaTempoDescargaFaixaPeso.BuscarPorTipoCarga(carga.TipoDeCarga?.Codigo ?? 0);
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entregaPendenteAtual = ObterPrimeiraEntregaPendente(previsoesCargaEntrega);
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entregaAnterior = ObterUltimaEntregaRealizada(previsoesCargaEntrega);

            int velocidadeMediaDeslocamento = ObterVelocidadeVeiculoCarregado(carga, cargaRotaFrete);
            if (velocidadeMediaCalculada > 0)
                velocidadeMediaDeslocamento = velocidadeMediaCalculada;
            DateTime dataBaseCalcularPrevisao = DateTime.MinValue;
            bool primeiraEntregaPendente = true;

            while (entregaPendenteAtual != null)
            {
                _cargaEntregaAtual = entregaPendenteAtual;

                if (primeiraEntregaPendente)
                {
                    if (dataPosicaoAtual.HasValue)
                    {
                        dataBaseCalcularPrevisao = dataPosicaoAtual.Value;
                        AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataBaseCalcularPrevisao.ToDateTimeString()} [Data da posição atual]");
                    }
                    else
                    {
                        dataBaseCalcularPrevisao = ObterDataBasePrevisaoEntrega(carga, entregaPendenteAtual.Localidade);
                        DateTime dataFimUltimaEntregaRealizada = ObterDataFimUltimaEntregaRealizada(dataBaseCalcularPrevisao, previsoesCargaEntrega);

                        if (dataFimUltimaEntregaRealizada != dataBaseCalcularPrevisao)
                        {
                            dataBaseCalcularPrevisao = dataFimUltimaEntregaRealizada;
                            AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataBaseCalcularPrevisao.ToDateTimeString()} [Data final da última entrega realiza]");
                        }
                    }
                }
                else
                    AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataBaseCalcularPrevisao.ToDateTimeString()} [Data final da entrega anterior]");

                int distanciaAteEntregaAtual = (dataPosicaoAtual.HasValue && primeiraEntregaPendente) ? distanciaPosicaoAtualAteProximaEntrega : ObterDistanciaEntreEntregas(entregaAnterior, entregaPendenteAtual, previsoesCargaEntrega);
                int tempoExtraAteEntregaAtual = (dataPosicaoAtual.HasValue && primeiraEntregaPendente) ? 0 : ObterTempoExtraEntreEntregas(entregaAnterior, entregaPendenteAtual, previsoesCargaEntrega);

                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Velocidade média: {velocidadeMediaDeslocamento} km/h");
                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Distância até a entrega: {Math.Round(distanciaAteEntregaAtual / 1000d, digits: 3)} km");

                DateTime dataInicioPrevista = ObterDataInicialCalculada(dataBaseCalcularPrevisao, distanciaAteEntregaAtual, velocidadeMediaDeslocamento, carga, tempoExtraAteEntregaAtual, entregaPendenteAtual.Localidade);
                DateTime dataFimPrevista = ObterDataFinalCalculada(dataInicioPrevista, entregaPendenteAtual, carga, centrosDescarregamento, cargaJanelasDescarregamento, tipoCargaModeloVeicular, tiposCargaTempoDescargaFaixaPeso);

                entregaPendenteAtual.Calculada = true;
                entregaPendenteAtual.DataInicioEntregaPrevista = dataInicioPrevista;
                entregaPendenteAtual.DataFimEntregaPrevista = dataFimPrevista;
                entregaPendenteAtual.DataPrevisaoEntregaTransportador = ObterDataEntregaTransportador(carga);

                dataBaseCalcularPrevisao = dataFimPrevista;
                entregaAnterior = entregaPendenteAtual;
                entregaPendenteAtual = ObterProximaEntregaPendente(previsoesCargaEntrega);
                primeiraEntregaPendente = false;
            }

            _cargaEntregaAtual = null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> ConverterCargaEntregasParaObjetoDeValor(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega>();
            int total = cargaEntregas.Count;

            for (int i = 0; i < total; i++)
            {
                previsoesCargaEntrega.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega
                {
                    Calculada = false,
                    Cliente = cargaEntregas[i].Cliente,
                    Distancia = cargaEntregas[i].Distancia,
                    TempoExtraEntrega = cargaEntregas[i].TempoExtraEntrega,
                    OrdemPrevista = cargaEntregas[i].Ordem,
                    OrdemRealizada = cargaEntregas[i].OrdemRealizada,
                    DataInicioEntregaPrevista = cargaEntregas[i].DataPrevista,
                    DataInicioEntregaRealizada = cargaEntregas[i].DataInicio,
                    DataFimEntregaPrevista = cargaEntregas[i].DataFimPrevista,
                    DataFimEntregaRealizada = cargaEntregas[i].DataFim,
                    Coleta = cargaEntregas[i].Coleta,
                    Fronteira = cargaEntregas[i].Fronteira,
                    Localidade = cargaEntregas[i].Cliente?.Localidade ?? cargaEntregas[i].Localidade,
                    Peso = (from o in cargaEntregaPedidos where o.CargaEntrega.Codigo == cargaEntregas[i].Codigo select o.CargaPedido?.Pedido?.PesoTotal ?? 0).Sum(),
                });
            }

            return previsoesCargaEntrega;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> ConverterCargaPontosPassagemParaObjetoDeValor(List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega>();
            bool retornarColetas = tipoOperacao?.GerarControleColeta ?? false;

            if (retornarColetas)
                cargaRotaFretePontosPassagem = cargaRotaFretePontosPassagem.Where(x => x.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta || x.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega).ToList();
            else
                cargaRotaFretePontosPassagem = cargaRotaFretePontosPassagem.Where(x => x.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega).ToList();

            int total = cargaRotaFretePontosPassagem.Count;

            for (int i = 0; i < total; i++)
            {
                previsoesCargaEntrega.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega
                {
                    Calculada = false,
                    Cliente = cargaRotaFretePontosPassagem[i].Cliente,
                    Distancia = cargaRotaFretePontosPassagem[i].Distancia,
                    OrdemPrevista = cargaRotaFretePontosPassagem[i].Ordem,
                    Coleta = cargaRotaFretePontosPassagem[i].TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta,
                    Fronteira = cargaRotaFretePontosPassagem[i].Cliente?.FronteiraAlfandega ?? false
                });
            }

            return previsoesCargaEntrega;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao ObterConfiguracaoCalculoPrevisao()
        {
            if (_configuracaoCalculoPrevisao == null)
                _configuracaoCalculoPrevisao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoCalculoPrevisao;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega ObterConfiguracaoControleEntrega()
        {
            if (_configuracaoControleEntrega == null)
                _configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoControleEntrega;
        }

        private DateTime ObterDataBasePrevisaoEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade localidade = null)
        {
            if (_parametrosConfiguracaoCalculo.DataBase.HasValue)
                return _parametrosConfiguracaoCalculo.DataBase.Value;

            if (carga.DataInicioViagem.HasValue)
            {

                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {carga.DataInicioViagem.Value.ToDateTimeString()} [Data de início da viagem]");

                DateTime dataInicioViagemAjustadoFinalSemana = ObterDataConsiderandoFinalSemanaFeriados(carga.DataInicioViagem.Value, aplicarHorarioJornada: true, carga);
                if (carga.DataInicioViagem.Value != dataInicioViagemAjustadoFinalSemana)
                {
                    //carga.DataInicioViagem = dataInicioViagemAjustadoFinalSemana;
                    AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {carga.DataInicioViagem.Value.ToDateTimeString()} [Ajustado feriados e final de semana]");
                }

                return dataInicioViagemAjustadoFinalSemana;
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Logistica.CargaJanelaCarregamentoConsulta(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);
            DateTime? dataBase = null;
            string campoDataUtilizado = string.Empty;

            switch (configuracao.DataBaseCalculoPrevisaoControleEntrega)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                    dataBase = carga.DataCriacaoCarga;
                    campoDataUtilizado = "Data de criação da carga";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                    dataBase = carga.DataPrevisaoTerminoCarga;
                    campoDataUtilizado = "Data de previsão de término da carga";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                    dataBase = (carga.DataInicioViagemPrevista != null) ? carga.DataInicioViagemPrevista : carga.DataPrevisaoTerminoCarga ?? carga.DataCriacaoCarga;
                    campoDataUtilizado = "Data de início da viagem prevista";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                    dataBase = carga.DataCarregamentoCarga;
                    campoDataUtilizado = "Data de carregamento da carga";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                    dataBase = (cargaJanelaCarregamento != null) ? cargaJanelaCarregamento?.InicioCarregamento : null;
                    campoDataUtilizado = "Data de início do carregamento";
                    break;
            }

            if (dataBase == null)
            {
                if (carga.DataInicioViagemPrevista.HasValue)
                {
                    dataBase = carga.DataInicioViagemPrevista.Value;
                    campoDataUtilizado = "Data de início da viagem prevista";

                }
                else
                {
                    dataBase = DateTime.Now;
                    campoDataUtilizado = "Data do cálculo da previsão";
                }
            }

            int tempoPadraoTerminoCarregamento = ObterTempoPadraoTerminoCarregamento(carga, cargaJanelaCarregamento);
            DateTime dataBaseRetornar = dataBase.Value.AddHours(tempoPadraoTerminoCarregamento);
            DateTime dataConsiderandoFinalSemanaEFeriados = ObterDataConsiderandoFinalSemanaFeriados(dataBaseRetornar, aplicarHorarioJornada: true, carga, localidade);

            AdicionarComposicaoDataInicioEntregaPrevistaAtual($"{campoDataUtilizado}: {dataBase.Value.ToDateTimeString()}");
            AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Tempo padrão de carregamento: {tempoPadraoTerminoCarregamento * 60} minutos ({tempoPadraoTerminoCarregamento} horas)");
            AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataBaseRetornar.ToDateTimeString()} [{campoDataUtilizado} + Tempo padrão de carregamento]");

            if (dataBaseRetornar != dataConsiderandoFinalSemanaEFeriados)
            {
                dataBaseRetornar = dataConsiderandoFinalSemanaEFeriados;
                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataBaseRetornar.ToDateTimeString()} [Ajustado feriados e final de semana]");
            }

            return dataBaseRetornar;
        }

        public DateTime ObterDataInicialAjustadaParaHorarioUtil(DateTime data)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            bool periodoUtilConfigurado = ((configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial > TimeSpan.Zero) && (configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal > TimeSpan.Zero));

            if (!periodoUtilConfigurado)
                return data;

            return ObterDataInicialAjustadaParaHorarioUtil(data, configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial, configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal);
        }

        private DateTime ObterDataInicialAjustadaParaHorarioUtil(DateTime data, TimeSpan horarioInicial, TimeSpan horarioFinal)
        {
            TimeSpan horaDaData = data.TimeOfDay;
            bool iniciandoAntesHorarioInicialUtil = (horaDaData < horarioInicial);

            if (iniciandoAntesHorarioInicialUtil)
            {
                DateTime dataInicioHorarioUtilMesmoDia = data.Date.Add(horarioInicial);

                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito ajustada: {dataInicioHorarioUtilMesmoDia.ToDateTimeString()}");

                return dataInicioHorarioUtilMesmoDia;
            }

            bool iniciandoDepoisHorarioFinalUtil = (horaDaData > horarioFinal);

            if (iniciandoDepoisHorarioFinalUtil)
            {
                DateTime dataInicioHorarioUtilDiaSeguinte = data.Date.AddDays(1).Add(horarioInicial);

                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito ajustada: {dataInicioHorarioUtilDiaSeguinte.ToDateTimeString()}");

                return dataInicioHorarioUtilDiaSeguinte;
            }

            return data;
        }

        private DateTime ObterDataInicialCalculada(DateTime dataInicio, Double distanciaMetros, Double velocidadeKmH, Dominio.Entidades.Embarcador.Cargas.Carga carga, int tempoExtraEntregaMinutos, Dominio.Entidades.Localidade localidade = null)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = ObterConfiguracaoControleEntrega();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            DateTime dataPrevista = ObterDataInicialAjustadaParaHorarioUtil(dataInicio);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao configuracaoCalculoPrevisao = ObterConfiguracaoCalculoPrevisao();

            //#5413
            if (carga != null && (configuracaoControleEntrega?.UtilizarLeadTimeDaTabelaDeFreteParaCalculoDaPrevisaoDeEntrega ?? false))
            {
                DateTime? dataInicialCliente = ObterDataEntregaCliente(carga, dataPrevista);
                if (dataInicialCliente.HasValue) return dataInicialCliente.Value;
            }

            double tempoDeslocamentoEmMinutos = ((distanciaMetros > 0) && (velocidadeKmH > 0)) ? (((distanciaMetros / 1000) / velocidadeKmH) * 60).RoundUp(0) : 0d;

            AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Tempo de deslocamento: {tempoDeslocamentoEmMinutos} minutos ({Math.Round(tempoDeslocamentoEmMinutos / 60, digits: 2)} horas) [(Distância até a entrega / Velocidade média) * 60]");
            if (tempoExtraEntregaMinutos > 0)
                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Tempo extra deslocamento: {tempoExtraEntregaMinutos} minutos ({Math.Round(tempoExtraEntregaMinutos / 60, digits: 2)} horas) [Trecho de Balsa]");

            if (tempoDeslocamentoEmMinutos > 0)
            {
                TimeSpan horarioUtilInicial = configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial;
                TimeSpan horarioUtilFinal = (configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal > TimeSpan.Zero) ? configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal : DateTime.MaxValue.TimeOfDay;
                double periodoUtilEmMinutos = (horarioUtilFinal - horarioUtilInicial).TotalMinutes;
                double periodoUtilDiarioEmMinutos = Math.Min(periodoUtilEmMinutos, (double)configuracao.PrevisaoEntregaTempoUtilDiarioMinutos).RoundUp(0);

                if (periodoUtilDiarioEmMinutos > 0)
                {
                    double minutosDeslocamentoDistribuir = tempoDeslocamentoEmMinutos;
                    TimeSpan horarioInicioAlmoco = configuracaoCalculoPrevisao.HorarioInicialAlmoco;
                    int minutosAlmoco = configuracaoCalculoPrevisao.MinutosIntervalo;
                    bool horarioAlmocoConfigurado = configuracaoCalculoPrevisao.ConsiderarJornadaMotorita && (horarioInicioAlmoco > TimeSpan.Zero) && (minutosAlmoco > 0);

                    if (carga?.TipoOperacao?.ConfiguracaoControleEntrega?.ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao ?? false)
                    {
                        horarioInicioAlmoco = carga.TipoOperacao.ConfiguracaoControleEntrega.HorarioInicialAlmoco;
                        minutosAlmoco = carga.TipoOperacao.ConfiguracaoControleEntrega.MinutosIntervalo;
                        horarioAlmocoConfigurado = carga.TipoOperacao.ConfiguracaoControleEntrega.ConsiderarJornadaMotorista && (horarioInicioAlmoco > TimeSpan.Zero) && (minutosAlmoco > 0);
                    }

                    while (true)
                    {
                        DateTime dataPrevistaInicialDia = dataPrevista;
                        DateTime dataFinalHorarioUtilDia = dataPrevista.Date.Add(horarioUtilFinal);
                        double minutosDisponiveisAteFinalHorarioUtilDia = (dataFinalHorarioUtilDia - dataPrevista).TotalMinutes;
                        bool minutosAlmocoUtilizado = false;

                        if (horarioAlmocoConfigurado && (dataPrevistaInicialDia.TimeOfDay <= horarioInicioAlmoco))
                            minutosDisponiveisAteFinalHorarioUtilDia -= minutosAlmoco;

                        double minutosDisponiveisNoDia = Math.Min(minutosDisponiveisAteFinalHorarioUtilDia, periodoUtilDiarioEmMinutos).RoundUp(0);
                        double minutosUtilizadosdoDia = (minutosDisponiveisNoDia > minutosDeslocamentoDistribuir) ? minutosDeslocamentoDistribuir : minutosDisponiveisNoDia;

                        dataPrevista = dataPrevista.AddMinutes(minutosUtilizadosdoDia);

                        if (horarioAlmocoConfigurado && (dataPrevistaInicialDia.TimeOfDay <= horarioInicioAlmoco) && (dataPrevista.TimeOfDay > horarioInicioAlmoco))
                        {
                            dataPrevista = dataPrevista.AddMinutes(minutosAlmoco);
                            minutosAlmocoUtilizado = true;
                        }

                        minutosDeslocamentoDistribuir -= minutosUtilizadosdoDia;

                        AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Tempo disponível no dia {dataPrevista.ToDateString()}: {minutosDisponiveisNoDia} minutos ({Math.Round(minutosDisponiveisNoDia / 60, digits: 2)} horas)");
                        AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Tempo utilizado do dia {dataPrevista.ToDateString()}: {minutosUtilizadosdoDia} minutos ({Math.Round(minutosUtilizadosdoDia / 60, digits: 2)} horas)");
                        AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataPrevista.ToDateTimeString()} [Data de início de trânsito + Tempo utilizado do dia {dataPrevista.ToDateString()}{(minutosAlmocoUtilizado ? $" + {minutosAlmoco} minutos de almoço" : "")}]");

                        if (minutosDeslocamentoDistribuir <= 0)
                            break;

                        dataPrevista = dataPrevista.Date.AddDays(1).Add(horarioUtilInicial);
                        DateTime dataPrevistaDiaAjustadaFinalSemanaEFeriados = ObterDataConsiderandoFinalSemanaFeriados(dataPrevista, aplicarHorarioJornada: false, carga, localidade);

                        AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Tempo de deslocamento restante: {minutosDeslocamentoDistribuir} minutos ({Math.Round(minutosDeslocamentoDistribuir / 60, digits: 2)} horas)");
                        AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataPrevista.ToDateTimeString()} [Data inicial do próximo dia útil]");

                        if (dataPrevista != dataPrevistaDiaAjustadaFinalSemanaEFeriados)
                        {
                            dataPrevista = dataPrevistaDiaAjustadaFinalSemanaEFeriados;
                            AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataPrevista.ToDateTimeString()} [Ajustado feriados e final de semana]");
                        }
                    }
                }
                else
                {
                    dataPrevista = dataPrevista.AddMinutes(tempoDeslocamentoEmMinutos);
                    AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataPrevista.ToDateTimeString()} [Data de início de trânsito + Tempo de deslocamento]");
                }
            }
            else
                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataPrevista.ToDateTimeString()} [Data de início de trânsito + Tempo de deslocamento]");

            dataPrevista = dataPrevista.AddMinutes(tempoExtraEntregaMinutos);

            dataPrevista = ObterDataFinalAjustadaParaHorarioUtil(dataPrevista);

            if ((carga?.DataCarregamentoCarga != null) && (dataPrevista < carga.DataCarregamentoCarga.Value) && !configuracao.IncluirCargaCanceladaProcessarDT)
            {
                int tempoPadraoTerminoCarregamento = ObterTempoPadraoTerminoCarregamento(carga, cargaJanelaCarregamento: null);
                dataPrevista = carga.DataCarregamentoCarga.Value.AddHours(tempoPadraoTerminoCarregamento);

                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Tempo padrão de carregamento: {tempoPadraoTerminoCarregamento * 60} minutos ({tempoPadraoTerminoCarregamento} horas)");
                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de carregamento da carga: {carga.DataCarregamentoCarga.Value.ToDateTimeString()}");
                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataPrevista.ToDateTimeString()} [Data de carregamento da carga + Tempo padrão de carregamento]");
            }

            DateTime dataPrevistaAjustadaFinalSemanaEFeriados = ObterDataConsiderandoFinalSemanaFeriados(dataPrevista, aplicarHorarioJornada: false, carga, localidade);

            if (dataPrevista != dataPrevistaAjustadaFinalSemanaEFeriados)
            {
                dataPrevista = dataPrevistaAjustadaFinalSemanaEFeriados;
                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito: {dataPrevista.ToDateTimeString()} [Ajustado feriados e final de semana]");
            }

            return dataPrevista;
        }

        private DateTime? ObterDataInicioDescarregamento(Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            if (cliente != null && cargaJanelasDescarregamento != null)
            {
                int total = cargaJanelasDescarregamento.Count;

                for (int i = 0; i < total; i++)
                {
                    if (cargaJanelasDescarregamento[i].CentroDescarregamento?.Destinatario != null && cargaJanelasDescarregamento[i].CentroDescarregamento.Destinatario.CPF_CNPJ == cliente.CPF_CNPJ)
                        return cargaJanelasDescarregamento[i].InicioDescarregamento;
                }
            }

            return null;
        }

        private DateTime ObterDataFimUltimaEntregaRealizada(DateTime dataBase, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega)
        {
            int total = previsoesCargaEntrega.Count;
            DateTime dataFimUltimaEntregaRealizada = dataBase;

            for (int i = 0; i < total; i++)
            {
                if (previsoesCargaEntrega[i].DataFimEntregaRealizada != null && previsoesCargaEntrega[i].DataFimEntregaRealizada > dataFimUltimaEntregaRealizada)
                    dataFimUltimaEntregaRealizada = previsoesCargaEntrega[i].DataFimEntregaRealizada.Value;
            }

            return dataFimUltimaEntregaRealizada;
        }

        private DateTime ObterDataFinalAjustadaParaHorarioUtil(DateTime data)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            bool periodoUtilConfigurado = ((configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial > TimeSpan.Zero) && (configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal > TimeSpan.Zero));

            if (!periodoUtilConfigurado)
                return data;

            return ObterDataFinalAjustadaParaHorarioUtil(data, configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial, configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal);
        }

        private DateTime ObterDataFinalAjustadaParaHorarioUtil(DateTime data, TimeSpan horarioInicial, TimeSpan horarioFinal)
        {
            TimeSpan horaDaData = data.TimeOfDay;
            DateTime dataAjustada = data;
            DateTime dataFinalHorarioUtilBase;

            // Enquanto iniciar antes do horário inicial ou depois do horário final útil
            while (horaDaData < horarioInicial || horaDaData > horarioFinal)
            {
                // Remove o horário da data para os cálculos
                dataAjustada = dataAjustada.Date;

                bool iniciandoAntesHorarioInicialUtil = (horaDaData < horarioInicial);

                if (iniciandoAntesHorarioInicialUtil)
                {
                    DateTime dataFinalHorarioUtilDiaAnterior = dataAjustada.AddDays(-1).Add(horarioFinal);

                    dataFinalHorarioUtilBase = dataFinalHorarioUtilDiaAnterior;
                }
                else
                {
                    DateTime dataFinalHorarioUtilMesmoDia = dataAjustada.Add(horarioFinal);

                    dataFinalHorarioUtilBase = dataFinalHorarioUtilMesmoDia;
                    dataAjustada = dataAjustada.AddDays(1);
                }

                // Calcula a diferença
                TimeSpan diferenca = dataAjustada - dataFinalHorarioUtilBase;

                // Corrige o horário da data
                dataAjustada = dataAjustada.Add(horarioInicial).Add(diferenca);

                horaDaData = dataAjustada.TimeOfDay;
            }

            if (data != dataAjustada)
                AdicionarComposicaoDataInicioEntregaPrevistaAtual($"Data de início de trânsito ajustada: {dataAjustada.ToDateTimeString()}");

            return dataAjustada;
        }

        private DateTime ObterDataFinalCalculada(DateTime dataInicioPrevista, Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entrega, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento, Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular, List<Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso> tiposCargaTempoDescargaFaixaPeso)
        {
            DateTime? dataInicioDescarregamento = ObterDataInicioDescarregamento(entrega.Cliente, cargaJanelasDescarregamento);
            int tempoDescarregamento = ObterTempoDescarga(carga, entrega, centrosDescarregamento, tipoCargaModeloVeicular, tiposCargaTempoDescargaFaixaPeso);
            bool dataPrevistaAnteriorDataJanelaDescarregamento = ((dataInicioDescarregamento != null) && (dataInicioDescarregamento > dataInicioPrevista));
            DateTime dataFimPrevista;

            if (dataPrevistaAnteriorDataJanelaDescarregamento)
            {
                dataFimPrevista = dataInicioDescarregamento.Value.AddMinutes(tempoDescarregamento);
                AdicionarComposicaoDataFimEntregaPrevistaAtual($"Data inicial do descarregamento: {dataInicioDescarregamento.Value.ToDateTimeString()}");
                AdicionarComposicaoDataFimEntregaPrevistaAtual($"Data final de entrega prevista: {dataFimPrevista.ToDateTimeString()} [Data inicial do descarregamento + Tempo de descarga]");
            }
            else
            {
                dataFimPrevista = dataInicioPrevista.AddMinutes(tempoDescarregamento);
                AdicionarComposicaoDataFimEntregaPrevistaAtual($"Data de início de trânsito: {dataInicioPrevista.ToDateTimeString()}");
                AdicionarComposicaoDataFimEntregaPrevistaAtual($"Data final de entrega prevista: {dataFimPrevista.ToDateTimeString()} [Data de início de trânsito + Tempo de descarga]");
            }

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao configuracaoCalculoPrevisao = ObterConfiguracaoCalculoPrevisao();

            TimeSpan horarioInicioAlmoco = (carga.TipoOperacao?.ConfiguracaoControleEntrega?.ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao ?? false) ? carga.TipoOperacao.ConfiguracaoControleEntrega.HorarioInicialAlmoco : configuracaoCalculoPrevisao.HorarioInicialAlmoco;
            int minutosAlmoco = (carga.TipoOperacao?.ConfiguracaoControleEntrega?.ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao ?? false) ? carga.TipoOperacao.ConfiguracaoControleEntrega.MinutosIntervalo : configuracaoCalculoPrevisao.MinutosIntervalo;
            bool horarioAlmocoConfigurado = (carga.TipoOperacao?.ConfiguracaoControleEntrega?.ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao ?? false) ? carga.TipoOperacao.ConfiguracaoControleEntrega.ConsiderarJornadaMotorista && (horarioInicioAlmoco > TimeSpan.Zero) && (minutosAlmoco > 0) : configuracaoCalculoPrevisao.ConsiderarJornadaMotorita && (horarioInicioAlmoco > TimeSpan.Zero) && (minutosAlmoco > 0);

            if (horarioAlmocoConfigurado && (dataFimPrevista.TimeOfDay > horarioInicioAlmoco) && (dataFimPrevista.TimeOfDay <= horarioInicioAlmoco.Add(TimeSpan.FromMinutes(minutosAlmoco))))
            {
                dataFimPrevista = dataFimPrevista.AddMinutes(minutosAlmoco);
                AdicionarComposicaoDataFimEntregaPrevistaAtual($"Data final de entrega prevista: {dataFimPrevista.ToDateTimeString()} [Data final de entrega prevista + {minutosAlmoco} minutos de almoço]");
            }

            DateTime dataFimPrevistaAjustadaFinalSemanaEFeriados = ObterDataConsiderandoFinalSemanaFeriados(dataFimPrevista, aplicarHorarioJornada: false, carga);

            if (dataFimPrevista != dataFimPrevistaAjustadaFinalSemanaEFeriados)
            {
                dataFimPrevista = dataFimPrevistaAjustadaFinalSemanaEFeriados;
                AdicionarComposicaoDataFimEntregaPrevistaAtual($"Data final de entrega prevista: {dataFimPrevista.ToDateTimeString()} [Ajustado feriados e final de semana]");
            }

            return dataFimPrevista;
        }

        private int ObterDistanciaEntreEntregas(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entregaOrigem, Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entregaDestino, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega)
        {
            int distancia = 0;

            if (entregaDestino != null)
            {
                // Não há uma origem, a distância será da própria entrega de origem
                if (entregaOrigem == null)
                    distancia = entregaDestino.Distancia;
                else
                {
                    // Indiferente do sentido do deslocamento, a distância entre os destinos será considerada a mesma
                    int inicio = (entregaOrigem.OrdemPrevista < entregaDestino.OrdemPrevista) ? entregaOrigem.OrdemPrevista : entregaDestino.OrdemPrevista;
                    int fim = (entregaDestino.OrdemPrevista > entregaOrigem.OrdemPrevista) ? entregaDestino.OrdemPrevista : entregaOrigem.OrdemPrevista;

                    for (int i = inicio + 1; i <= fim; i++)
                    {
                        if (previsoesCargaEntrega.ElementAtOrDefault(i) != null)
                            distancia += previsoesCargaEntrega[i].Distancia;
                    }
                }
            }

            return distancia;
        }

        private int ObterTempoExtraEntreEntregas(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entregaOrigem, Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entregaDestino, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega)
        {
            int tempo = 0;

            if (entregaDestino != null)
            {
                // Não há uma origem, a distância será da própria entrega de origem
                if (entregaOrigem == null)
                    tempo = entregaDestino.TempoExtraEntrega;
                else
                {
                    // Indiferente do sentido do deslocamento, a distância entre os destinos será considerada a mesma
                    int inicio = (entregaOrigem.OrdemPrevista < entregaDestino.OrdemPrevista) ? entregaOrigem.OrdemPrevista : entregaDestino.OrdemPrevista;
                    int fim = (entregaDestino.OrdemPrevista > entregaOrigem.OrdemPrevista) ? entregaDestino.OrdemPrevista : entregaOrigem.OrdemPrevista;

                    for (int i = inicio + 1; i <= fim; i++)
                    {
                        if (previsoesCargaEntrega.ElementAtOrDefault(i) != null)
                            tempo += previsoesCargaEntrega[i].TempoExtraEntrega;
                    }
                }
            }

            return tempo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega ObterPrimeiraEntregaPendente(List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega)
        {
            int total = previsoesCargaEntrega.Count;

            for (int i = 0; i < total; i++)
            {
                if (previsoesCargaEntrega[i].DataFimEntregaRealizada == null)
                    return previsoesCargaEntrega[i];
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega ObterProximaEntregaPendente(List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega)
        {
            int total = previsoesCargaEntrega.Count;

            for (int i = 0; i < total; i++)
            {
                if (!previsoesCargaEntrega[i].Calculada && previsoesCargaEntrega[i].DataFimEntregaRealizada == null)
                    return previsoesCargaEntrega[i];
            }

            return null;
        }

        private int ObterTempoDescarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entrega, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento, Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular, List<Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso> tiposCargaTempoDescargaFaixaPeso)
        {
            if (entrega.Fronteira)
            {
                int tempoMedioPermanenciaFronteira = entrega.Cliente?.TempoMedioPermanenciaFronteira ?? 0;

                AdicionarComposicaoDataFimEntregaPrevistaAtual($"Tempo médio de permanência na fronteira: {tempoMedioPermanenciaFronteira} minutos");
                AdicionarComposicaoDataFimEntregaPrevistaAtual($"Tempo de descarga: {tempoMedioPermanenciaFronteira} minutos ({Math.Round(tempoMedioPermanenciaFronteira / 60d, digits: 2)} horas) [Tempo médio de permanência na fronteira]");

                return tempoMedioPermanenciaFronteira;
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            double cpfCpnjCliente = entrega.Cliente?.CPF_CNPJ ?? 0;
            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = (from o in centrosDescarregamento where o.Destinatario.CPF_CNPJ == cpfCpnjCliente select o).FirstOrDefault();
            int tempoPadraoDeColetaOuEntrega = (entrega.Coleta) ? configuracao?.TempoPadraoDeColetaParaCalcularPrevisao ?? 0 : configuracao?.TempoPadraoDeEntregaParaCalcularPrevisao ?? 0;
            int tempoDescarga = tempoPadraoDeColetaOuEntrega;
            List<string> detalhesTempoDescarga = new List<string>();

            detalhesTempoDescarga.Add($"Tempo padrão de {(entrega.Coleta ? "coleta" : "entrega")}");
            AdicionarComposicaoDataFimEntregaPrevistaAtual($"Tempo padrão de {(entrega.Coleta ? "coleta" : "entrega")}: {tempoPadraoDeColetaOuEntrega} minutos");

            if (centroDescarregamento?.TemposDescarregamento != null)
            {
                Dominio.Entidades.Embarcador.Logistica.TempoDescarregamento tempoDescarregamento = (
                    from o in centroDescarregamento.TemposDescarregamento
                    where o.ModeloVeicular.Codigo == carga.ModeloVeicularCarga?.Codigo && o.TipoCarga.Codigo == carga.TipoDeCarga?.Codigo && o.Tempo > 0
                    select o
                ).FirstOrDefault();

                if (tempoDescarregamento != null)
                {
                    tempoDescarga += tempoDescarregamento.Tempo;
                    detalhesTempoDescarga.Add("Tempo de descarga do centro de descarregamento");
                    AdicionarComposicaoDataFimEntregaPrevistaAtual($"Tempo de descarga do centro de descarregamento: {tempoDescarregamento.Tempo} minutos");
                }
            }

            if (carga.TipoDeCarga?.TipoTempoDescarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoDescargaTipoCarga.PorModeloVeicular)
            {
                int tempoPadraoDescargaPorModeloVeicular = tipoCargaModeloVeicular?.TempoDescarga ?? 0;

                if (tempoPadraoDescargaPorModeloVeicular > 0)
                {
                    tempoDescarga += tempoPadraoDescargaPorModeloVeicular;
                    detalhesTempoDescarga.Add("Tempo de descarga por modelo veicular");
                    AdicionarComposicaoDataFimEntregaPrevistaAtual($"Tempo de descarga por modelo veicular: {tempoPadraoDescargaPorModeloVeicular} minutos");
                }
            }
            else if (carga.TipoDeCarga?.TipoTempoDescarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoDescargaTipoCarga.PorPeso && entrega.Peso != null && tiposCargaTempoDescargaFaixaPeso != null)
            {
                int tempoDescargaPorFaixaPeso = ObterTempoDescarcaPorFaixaPeso(tiposCargaTempoDescargaFaixaPeso, entrega.Peso.Value);

                if (tempoDescargaPorFaixaPeso > 0)
                {
                    tempoDescarga += tempoDescargaPorFaixaPeso;
                    detalhesTempoDescarga.Add("Tempo de descarga por faixa de peso");
                    AdicionarComposicaoDataFimEntregaPrevistaAtual($"Tempo de descarga por faixa de peso: {tempoDescargaPorFaixaPeso} minutos");
                }
            }

            AdicionarComposicaoDataFimEntregaPrevistaAtual($"Tempo de descarga: {tempoDescarga} minutos ({Math.Round(tempoDescarga / 60d, digits: 2)} horas) [{string.Join(" + ", detalhesTempoDescarga)}]");

            return tempoDescarga;
        }

        private int ObterTempoDescarcaPorFaixaPeso(List<Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso> tiposCargaTempoDescargaFaixaPeso, decimal peso)
        {
            foreach (var tipoCargaTempoDescargaFaixaPeso in tiposCargaTempoDescargaFaixaPeso)
            {
                if (peso >= tipoCargaTempoDescargaFaixaPeso.Inicio && peso <= tipoCargaTempoDescargaFaixaPeso.Fim)
                {
                    return tipoCargaTempoDescargaFaixaPeso.TempoDescarga;
                }
            }

            return 0;
        }

        private int ObterTempoPadraoTerminoCarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            int tempoPadraoTerminoCarregamento = configuracao.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento;

            if (tempoPadraoTerminoCarregamento > 0)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = cargaJanelaCarregamento?.CentroCarregamento;

                if (centroCarregamento == null)
                {
                    Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(carga.TipoDeCarga?.Codigo ?? 0, carga.Filial?.Codigo ?? 0, ativo: true, carga);
                }

                if (centroCarregamento?.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento > 0)
                    tempoPadraoTerminoCarregamento = centroCarregamento.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento;
            }

            return tempoPadraoTerminoCarregamento;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega ObterUltimaEntregaRealizada(List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega ultimaEntregaRealizada = null;
            int total = previsoesCargaEntrega.Count;

            for (int i = 0; i < total; i++)
            {
                if (previsoesCargaEntrega[i].DataFimEntregaRealizada != null && (ultimaEntregaRealizada == null || previsoesCargaEntrega[i].DataFimEntregaRealizada > ultimaEntregaRealizada.DataFimEntregaRealizada))
                    ultimaEntregaRealizada = previsoesCargaEntrega[i];
            }

            return ultimaEntregaRealizada;
        }

        private int ObterVelocidadeVeiculoCarregado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();

            if (configuracao.PrevisaoEntregaVelocidadeMediaCarregado > 0)
            {
                return configuracao.PrevisaoEntregaVelocidadeMediaCarregado;
            }

            if (carga != null && carga.Rota != null && carga.Rota.VelocidadeMediaCarregado > 0)
            {
                return carga.Rota.VelocidadeMediaCarregado;
            }

            decimal velocidaRota = 0;

            if (cargaRotaFrete?.TempoDeViagemEmMinutos > 0)
            {
                decimal tempo = cargaRotaFrete.TempoDeViagemEmMinutos / Convert.ToDecimal(60);

                if (tempo > 0 && carga != null && carga.Rota != null && carga.Rota.Quilometros > 0)
                    velocidaRota = carga.Rota.Quilometros / tempo * Convert.ToDecimal(0.7); //70% da velocidade do veiculo

                if (velocidaRota <= 0 || velocidaRota > 100)
                    velocidaRota = 45;
            }

            return Decimal.ToInt32(velocidaRota);
        }

        private DateTime CalcularDataAgendamentoColeta(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega coleta, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraEntrega)
        {
            // Data base
            DateTime dataColeta = primeiraEntrega.DataAgendamento.Value;
            //DateTime dataColeta = new DateTime(2024, 7, 12, 0, 0, 0); testes

            // Distância total em quilômetros
            int distanciaTotal = primeiraEntrega.Distancia > 0 ? primeiraEntrega.Distancia / 1000 : 0; //distância total em quilômetros
            //distanciaTotal = 1450; testes

            // Calcular data de chegada retroativamente considerando dias úteis
            DateTime dataChegada = CalcularTempoDeslocamento(dataColeta, distanciaTotal, 50, 5);

            // Aplicar regras adicionais ao agendamento de coleta
            return AplicarRegrasAdicionais(dataChegada);

        }

        private DateTime? ObterDataEntregaCliente(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataPrevista)
        {
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repositorioCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repositorioCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente = repositorioCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, false);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = null;

            if (cargaTabelaFreteCliente != null && cargaTabelaFreteCliente.TabelaFreteCliente != null)
                tabelaFreteCliente = cargaTabelaFreteCliente.TabelaFreteCliente;

            if (cargaTabelaFreteCliente == null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaFreteCliente = repositorioCargaPedidoTabelaFreteCliente.BuscarPorCarga(carga.Codigo, false)?.FirstOrDefault();
                if (cargaPedidoTabelaFreteCliente != null && cargaPedidoTabelaFreteCliente.TabelaFreteCliente != null)
                    tabelaFreteCliente = cargaPedidoTabelaFreteCliente.TabelaFreteCliente;
            }

            if (!carga.DataInicioViagem.HasValue) return null;

            if (tabelaFreteCliente == null) return null;

            int tempoLeadTime = tabelaFreteCliente.LeadTimeMinutos;
            if (tempoLeadTime == 0) return null;

            DateTime dataEntregaCliente = dataPrevista.AddMinutes(tempoLeadTime);

            if (tabelaFreteCliente.TipoLeadTime == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoTempoDiasMinutos.Dias)
            {
                dataEntregaCliente = dataPrevista;

                for (int i = 0; i < tempoLeadTime; i++)
                    dataEntregaCliente = ObterDataConsiderandoFinalSemanaFeriados(dataEntregaCliente.AddDays(1), false, carga);
            }

            return dataEntregaCliente;
        }


        private DateTime? ObterDataEntregaTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repositorioCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repositorioCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente = repositorioCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, false);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = null;

            if (cargaTabelaFreteCliente != null && cargaTabelaFreteCliente.TabelaFreteCliente != null)
                tabelaFreteCliente = cargaTabelaFreteCliente.TabelaFreteCliente;

            if (cargaTabelaFreteCliente == null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaFreteCliente = repositorioCargaPedidoTabelaFreteCliente.BuscarPorCarga(carga.Codigo, false)?.FirstOrDefault();
                if (cargaPedidoTabelaFreteCliente != null && cargaPedidoTabelaFreteCliente.TabelaFreteCliente != null)
                    tabelaFreteCliente = cargaPedidoTabelaFreteCliente.TabelaFreteCliente;
            }

            if (!carga.DataInicioViagem.HasValue) return null;

            if (tabelaFreteCliente == null) return null;

            int tempoLeadTime = tabelaFreteCliente.LeadTimeTransportador;
            if (tempoLeadTime == 0) return null;

            DateTime dataEntregaTransportador = carga.DataInicioViagem.Value.AddMinutes(tempoLeadTime);

            if (cargaTabelaFreteCliente?.TabelaFreteCliente?.TipoLeadTime == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoTempoDiasMinutos.Dias)
            {
                dataEntregaTransportador = carga.DataInicioViagem.Value;
                for (int i = 0; i < tempoLeadTime; i++)
                    dataEntregaTransportador = ObterDataConsiderandoFinalSemanaFeriados(dataEntregaTransportador.AddDays(1), false, carga);
            }

            return dataEntregaTransportador;
        }

        private static DateTime ObterDataConsiderandoFinalSemana(DateTime dataBase, bool aplicarTimeSpan, bool retroativo = false)
        {
            while (
                (dataBase.DayOfWeek == DayOfWeek.Saturday) ||
                (dataBase.DayOfWeek == DayOfWeek.Sunday))
            {
                if (!retroativo)
                    dataBase = dataBase.AddDays(1);
                else
                    dataBase = dataBase.AddDays(-1);
            }

            if (aplicarTimeSpan)
            {
                TimeSpan horaentrega = new TimeSpan(0, 13, 0, 0);
                dataBase = dataBase.Date.Add(horaentrega);
            }

            return dataBase;
        }

        private DateTime CalcularTempoDeslocamento(DateTime dataInicio, int distanciaKm, Double velocidadeKmH, int tempoExtraEntregaMinutos)
        {
            int distanciaMaximaPorDiaKm = 500;
            DateTime dataChegada = dataInicio;
            int distanciaRestante = distanciaKm;

            while (distanciaRestante > 0)
            {
                //distância que pode ser percorrida no dia
                int distanciaPercorridaNoDia = Math.Min(distanciaRestante, distanciaMaximaPorDiaKm);

                //tempo para percorrer essa distância
                double horasParaPercorrer = distanciaPercorridaNoDia / (double)velocidadeKmH;
                if (horasParaPercorrer == 10)
                    horasParaPercorrer = 24;

                TimeSpan tempoPercorrido = TimeSpan.FromHours(horasParaPercorrer);

                dataChegada = dataChegada.Add(-tempoPercorrido);//;

                // Atualiza a distância restante
                distanciaRestante -= distanciaPercorridaNoDia;
            }

            return dataChegada.AddHours(-tempoExtraEntregaMinutos);
        }

        static DateTime AplicarRegrasAdicionais(DateTime dataAgendamentoColeta)
        {
            // Se a data de agendamento de coleta for sábado após as 18:00 horas, ajustar para 17:00
            if (dataAgendamentoColeta.DayOfWeek == DayOfWeek.Saturday && dataAgendamentoColeta.Hour >= 18)
            {
                dataAgendamentoColeta = new DateTime(dataAgendamentoColeta.Year, dataAgendamentoColeta.Month, dataAgendamentoColeta.Day, 17, 0, 0);
            }
            // Se a data de agendamento de coleta for domingo, ajustar para sábado às 17:00
            else if (dataAgendamentoColeta.DayOfWeek == DayOfWeek.Sunday)
            {
                dataAgendamentoColeta = dataAgendamentoColeta.AddDays(-1); // Antecipa para sábado
                dataAgendamentoColeta = new DateTime(dataAgendamentoColeta.Year, dataAgendamentoColeta.Month, dataAgendamentoColeta.Day, 17, 0, 0);
            }

            return dataAgendamentoColeta;
        }


    }

    #endregion
}


