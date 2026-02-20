using System.Globalization;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    public class MonitoramentoControllerBase : BaseController
    {
        #region Construtores

        public MonitoramentoControllerBase(Conexao conexao) : base(conexao) { }

        #endregion


        #region Atributos protegidos

        protected const string MASCARA_DATA_HM = "dd/MM/yyyy HH:mm";
        protected const string MASCARA_DATA_HMS = "dd/MM/yyyy HH:mm:ss";

        #endregion

        #region Métodos públicos

        public JsonpResult DadosMapa(Repositorio.UnitOfWork unitOfWork, int codigoMonitoramento, int codigoCarga, int codigoVeiculo)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);

            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = null;
            if (codigoMonitoramento > 0)
            {
                monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);

            }
            else if (codigoCarga > 0)
            {
                monitoramento = repMonitoramento.BuscarUltimoPorCarga(codigoCarga);
            }
            if (monitoramento == null) return new JsonpResult(false, true, "Monitoramento não encontrado.");

            if (codigoVeiculo == 0) codigoVeiculo = monitoramento.Veiculo?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repPosicaoAtual.BuscarPorVeiculo(codigoVeiculo);

            dynamic dadosVeiculo = ObterDadosVeiculoParaMapa(unitOfWork, monitoramento);
            dynamic dadosEntregas = ObterDadosEntregasParaMapa(unitOfWork, monitoramento);
            dynamic areas = ObterAreas(unitOfWork, monitoramento.Carga, configuracao);
            dynamic paradas = ObterParadas(unitOfWork, monitoramento, configuracao);
            dynamic monitoramentoVeiculos = ObterPolilinhaRealizadaVeiculos(unitOfWork, monitoramento);
            dynamic retorno = (
                new
                {
                    Veiculo = dadosVeiculo,
                    Entregas = dadosEntregas,
                    PolilinhaPrevista = monitoramento.PolilinhaPrevista,
                    DistanciaPrevista = String.Format("{0:n1} Km", monitoramento.DistanciaPrevista),
                    PolilinhaRealizada = monitoramento.PolilinhaRealizada,
                    DistanciaRealizada = String.Format("{0:n1} Km", monitoramento.DistanciaRealizada),
                    PolilinhaAteOrigem = monitoramento.PolilinhaAteOrigem,
                    DistanciaAteOrigem = String.Format("{0:n1} Km", monitoramento.DistanciaAteOrigem),
                    PolilinhaAteDestino = monitoramento.PolilinhaAteDestino,
                    DistanciaAteDestino = String.Format("{0:n1} Km", monitoramento.DistanciaAteDestino),
                    PontosPrevistos = monitoramento.PontosPrevistos,
                    Areas = areas,
                    Paradas = paradas,
                    Data = posicaoAtual != null ? posicaoAtual.DataVeiculo.ToString(MASCARA_DATA_HMS) : "",
                    UltimaPosicaoData = (posicaoAtual != null) ? posicaoAtual.DataVeiculo.ToString("HH:mm dd/MM/yyyy") : null,
                    UltimaPosicao = (posicaoAtual != null) ? $"({posicaoAtual.Latitude}, {posicaoAtual.Longitude})" : null,
                    Latitude = (posicaoAtual != null) ? posicaoAtual.Latitude : 0,
                    Longitude = (posicaoAtual != null) ? posicaoAtual.Longitude : 0,
                    MonitoramentoVeiculos = monitoramentoVeiculos,
                    online = (posicaoAtual != null && (DateTime.Now - posicaoAtual.DataVeiculo).TotalMinutes <= configuracao.TempoSemPosicaoParaVeiculoPerderSinal).ToString(),
                    Rastreador = (posicaoAtual != null && posicaoAtual.Posicao != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreadorHelper.ObterDescricao(posicaoAtual.Posicao.Rastreador) : "").ToString(),
                }
            );
            return new JsonpResult(retorno);

        }

        #endregion

        #region Métodos pretegidos

        protected dynamic ObterDadosVeiculoParaMapa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            int total;
            double latitude = 0, longitude = 0;
            string descricao = string.Empty, transportador = string.Empty, dataPosicao = string.Empty;
            List<dynamic> destinos = new List<dynamic>();

            string placaVeiculo = monitoramento.Veiculo?.Placa ?? string.Empty;
            string status = monitoramento.StatusViagem?.Descricao ?? string.Empty;
            string carga = monitoramento.Carga?.CodigoCargaEmbarcador ?? string.Empty;

            Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramento.Carga);
            string embarcador = (clienteOrigem != null) ? clienteOrigem.Descricao : string.Empty;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntregas = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = repCargaEntregas.BuscarPorCargaNaoRealizada(monitoramento?.Carga.Codigo ?? 0);
            total = entregas.Count;
            for (int i = 0; i < Math.Min(total, 3); i++)
            {
                destinos.Add(new
                {
                    Ordem = entregas[i].Ordem,
                    Descricao = entregas[i].Cliente?.Descricao ?? string.Empty
                });
            }

            Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoatual = null;
            if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado)
            {
                posicaoatual = repPosicaoAtual.BuscarPorVeiculo(monitoramento.Veiculo?.Codigo ?? 0);
            }

            if (posicaoatual != null)
            {
                latitude = Math.Round(posicaoatual.Latitude, 6);
                longitude = Math.Round(posicaoatual.Longitude, 6);
                transportador = monitoramento.Carga.Empresa?.Descricao ?? "";
                dataPosicao = posicaoatual.DataVeiculo.ToString(MASCARA_DATA_HMS);

                string descricaoAlvos = string.Empty;
                if (posicaoatual.EmAlvo ?? false)
                {
                    Repositorio.Embarcador.Logistica.PosicaoAlvo repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos = repPosicaoAlvo.BuscarPorPosicao(posicaoatual.Posicao.Codigo);
                    total = posicaoAlvos.Count;
                    if (total > 0)
                    {
                        for (int i = 0; i < total; i++)
                        {
                            descricaoAlvos += posicaoAlvos[i].Cliente.Descricao + "; ";
                        }
                        descricaoAlvos = descricaoAlvos.Substring(0, descricaoAlvos.Length - 2);
                    }
                }
                if (string.IsNullOrWhiteSpace(descricaoAlvos))
                {
                    descricao = posicaoatual.Descricao;
                }
                else
                {
                    descricao = descricaoAlvos + " (" + posicaoatual.Descricao + ")";
                }
            }

            return new
            {
                PlacaVeiculo = placaVeiculo,
                Transportador = transportador,
                Status = status,
                Carga = carga,
                Data = dataPosicao,
                Descricao = descricao,
                Latitude = latitude,
                Longitude = longitude,
                Embarcador = embarcador,
                Destinos = destinos
            };
        }

        protected dynamic ObterDadosEntregasParaMapa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PedidosCargaMonitoramento> pedidos = repMonitoramento.ConsultarPedidos(monitoramento.Carga.Codigo);
            int totalPedidos = pedidos.Count;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            double cnpjFornecedor = 0;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                cnpjFornecedor = this.Usuario.ClienteFornecedor.CPF_CNPJ;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(monitoramento.Carga.Codigo, cnpjFornecedor);
            List<dynamic> entregas = new List<dynamic>();


            int total = cargaEntregas.Count;
            if (total > 0)
            {
                int ordemEntregaDeslocamento = 0;

                // Se a primeira entrega não é coleta, adiciona a origem da carga como primeiro destino
                if (!cargaEntregas[0].Coleta)
                {
                    Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramento.Carga);
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiroPedidoPorCarga(monitoramento.Carga.Codigo);

                    if (clienteOrigem != null)
                    {
                        entregas.Add(new
                        {
                            Descricao = clienteOrigem.Nome,
                            Latitude = cargaPedido.Origem.Latitude,
                            Longitude = cargaPedido.Origem.Longitude,
                            OrdemPrevista = ordemEntregaDeslocamento
                        });
                    }
                    ordemEntregaDeslocamento++;
                }

                // Todas as entregas do controle de entrega
                for (int i = 0; i < total; i++)
                {
                    if (cargaEntregas[i].Cliente != null)
                    {

                        int quantidadeNF = 0;
                        decimal valorTotalNF = 0;
                        for (int j = 0; j < totalPedidos; j++)
                        {
                            if (cargaEntregas[i].Cliente.Codigo == pedidos[j].CodigoCliente)
                            {
                                quantidadeNF++;
                                valorTotalNF += pedidos[j].Valor;
                            }
                        }

                        string tempoDirigindo = string.Empty;
                        string distanciaPercorrida = string.Empty;
                        if (cargaEntregas[i].DataEntradaRaio != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAnterior = IdentificaEntregaAnterior(cargaEntregas[i], cargaEntregas);
                            if (cargaEntregaAnterior != null && cargaEntregaAnterior.DataSaidaRaio != null)
                            {
                                try
                                {
                                    tempoDirigindo = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(cargaEntregas[i].DataEntradaRaio.Value - cargaEntregaAnterior.DataSaidaRaio.Value);
                                    double totalDistanciaPercorrida = CalcularDistanciaPercorridaEntreEntregas(unitOfWork, monitoramento, cargaEntregaAnterior, cargaEntregas[i]);
                                    distanciaPercorrida = (totalDistanciaPercorrida < 1000) ? ((int)totalDistanciaPercorrida).ToString() + "m" : Math.Round(totalDistanciaPercorrida / 1000).ToString() + "Km";
                                }
                                catch (Exception) { /*enterrar o erro*/ }
                            }
                        }
                        string latitude = cargaEntregas[i].ClienteOutroEndereco?.Latitude ?? cargaEntregas[i].Cliente.Latitude;
                        string longitude = cargaEntregas[i].ClienteOutroEndereco?.Longitude ?? cargaEntregas[i].Cliente.Longitude;

                        entregas.Add(new
                        {
                            Descricao = cargaEntregas[i].Cliente.Nome,
                            Latitude = double.Parse((latitude).Replace(",", "."), CultureInfo.InvariantCulture),
                            Longitude = double.Parse((longitude).Replace(",", "."), CultureInfo.InvariantCulture),
                            QuantidadeNF = quantidadeNF,
                            ValorTotalNF = (valorTotalNF > 0) ? valorTotalNF.ToString("C") : string.Empty,
                            cargaEntregas[i].Situacao,
                            Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaHelper.ObterDescricao(cargaEntregas[i].Situacao),
                            cargaEntregas[i].Coleta,
                            OrdemPrevista = cargaEntregas[i].Ordem + ordemEntregaDeslocamento,
                            OrdemRealizada = (cargaEntregas[i].DataConfirmacao != null) ? cargaEntregas[i].OrdemRealizada + ordemEntregaDeslocamento : 0,
                            Chegada = (cargaEntregas[i].DataEntradaRaio != null) ? cargaEntregas[i].DataEntradaRaio.Value.ToString(MASCARA_DATA_HMS) : string.Empty,
                            Saida = (cargaEntregas[i].DataSaidaRaio != null) ? cargaEntregas[i].DataSaidaRaio.Value.ToString(MASCARA_DATA_HMS) : string.Empty,
                            TempoAtendimento = (cargaEntregas[i].DataEntradaRaio != null && cargaEntregas[i].DataSaidaRaio != null) ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(cargaEntregas[i].DataSaidaRaio.Value - cargaEntregas[i].DataEntradaRaio.Value) : string.Empty,
                            TempoDirigindo = tempoDirigindo,
                            DistanciaPercorrida = distanciaPercorrida
                        });
                    }
                }
            }
            return entregas;
        }

        protected dynamic ObterAreas(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (carga == null) return null;

            Repositorio.Embarcador.Logistica.SubareaCliente repSubareaCliente = new Repositorio.Embarcador.Logistica.SubareaCliente(unitOfWork);
            var remetente = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterRemetente(carga, unitOfWork);
            var lista = new List<dynamic>();

            if (remetente != null)
            {
                double remetenteLatitude = double.Parse((remetente.Latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                double remetenteLongitude = double.Parse((remetente.Longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                var remetenteRaio = remetente?.RaioEmMetros != null && remetente.RaioEmMetros > 0 ? remetente.RaioEmMetros : configuracao.RaioPadrao;
                List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> subareas = repSubareaCliente.BuscarPorCliente(remetente);
                lista.Add(new
                {
                    Raio = remetenteRaio,
                    Latitude = remetenteLatitude,
                    Longitude = remetenteLongitude,
                    remetente.TipoArea,
                    remetente.Area,
                    Subareas = (
                        from subarea in subareas
                        select new
                        {
                            subarea.Area,
                            subarea.Descricao,
                            Tipo = subarea.TipoSubarea.Descricao
                        }).ToList()
                });
            }

            foreach (var entrega in carga.Entregas)
            {
                if (entrega != null && entrega.Cliente != null)
                {
                    string clienteLatitude = entrega.ClienteOutroEndereco?.Latitude ?? entrega.Cliente.Latitude;
                    string clienteLongitude = entrega.ClienteOutroEndereco?.Longitude ?? entrega.Cliente.Longitude;
                    var raioEntrega = entrega.Cliente.RaioEmMetros != null && entrega.Cliente.RaioEmMetros > 0 ? entrega.Cliente.RaioEmMetros : configuracao.RaioPadrao;

                    List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> subareas = repSubareaCliente.BuscarPorCliente(entrega.Cliente);
                    lista.Add(new
                    {
                        Raio = raioEntrega,
                        Latitude = double.Parse((clienteLatitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture),
                        Longitude = double.Parse((clienteLongitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture),
                        TipoArea = entrega.Cliente.TipoArea ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio,
                        Area = entrega.Cliente.Area ?? string.Empty,
                        Subareas = (
                            from subarea in subareas
                            select new
                            {
                                subarea.Area,
                                subarea.Descricao,
                                Tipo = subarea.TipoSubarea.Descricao
                            }).ToList()
                    });
                }
            }

            return lista;
        }

        protected dynamic ObterParadas(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = Servicos.Embarcador.Monitoramento.MonitoramentoEventoParada.ObterParadasUnicasPriorizandoAlertas(unitOfWork, monitoramento, null, null, configuracao);
            var retorno = (
                from parada in paradas
                select new
                {
                    parada.Alerta,
                    parada.Latitude,
                    parada.Longitude,
                    parada.Tipo,
                    parada.Placa,
                    parada.TempoFormatado,
                    Tempo = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(parada.Tempo),
                    Data = parada.DataInicio,
                    parada.Descricao
                }).ToList();
            return retorno;
        }

        protected dynamic ObterPolilinhaRealizadaVeiculos(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> monitoramentoVeiculos = repMonitoramentoVeiculo.BuscarTodosPorMonitoramento(monitoramento.Codigo);
            var retorno = (
                from r in monitoramentoVeiculos
                select new
                {
                    r.Codigo,
                    r.Veiculo.Placa,
                    r.Polilinha,
                    Distancia = String.Format("{0:n1} Km", (r.Distancia != null) ? r.Distancia : 0),
                    DataInicio = r.DataInicio.ToString(MASCARA_DATA_HMS),
                    DataFim = (r.DataFim != null) ? r.DataFim.Value.ToString(MASCARA_DATA_HMS) : string.Empty
                }).ToList();
            return retorno;
        }

        protected Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega IdentificaEntregaAnterior(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas)
        {
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAnterior = null;
            if (cargaEntrega.DataInicio != null)
            {
                int total = cargaEntregas.Count;
                for (int i = 0; i < total; i++)
                {
                    if (cargaEntregas[i].DataInicio != null && cargaEntregas[i].DataInicio < cargaEntrega.DataInicio && (cargaEntregaAnterior == null || cargaEntregas[i].DataInicio > cargaEntregaAnterior.DataInicio))
                    {
                        cargaEntregaAnterior = cargaEntregas[i];
                    }
                }
            }
            return cargaEntregaAnterior;
        }

        protected double CalcularDistanciaPercorridaEntreEntregas(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAnterior, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAtual)
        {
            double distancia = 0;
            if (monitoramento != null && !string.IsNullOrWhiteSpace(monitoramento.PolilinhaRealizada) && cargaEntregaAnterior.DataSaidaRaio != null && cargaEntregaAtual.DataEntradaRaio != null)
            {
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarPorVeiculoDataInicialeFinal(monitoramento.Veiculo.Codigo, cargaEntregaAnterior.DataSaidaRaio.Value, cargaEntregaAtual.DataEntradaRaio.Value);
                if (posicoes.Count > 0)
                {

                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRotaRealizada = Servicos.Embarcador.Logistica.Polilinha.Decodificar(monitoramento.PolilinhaRealizada);

                    // Ponto de partida
                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(posicoes.First().Latitude, posicoes.First().Longitude);
                    wayPointOrigem = Servicos.Embarcador.Logistica.Polilinha.LocalizaPontoMaisProximoDaRota(wayPointsRotaRealizada, wayPointOrigem);

                    // Ponto de chegada
                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDestino = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(posicoes.Last().Latitude, posicoes.Last().Longitude);
                    wayPointDestino = Servicos.Embarcador.Logistica.Polilinha.LocalizaPontoMaisProximoDaRota(wayPointsRotaRealizada, wayPointDestino);

                    // Distância percorrida entre os dois pontos na rota 
                    distancia = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(wayPointsRotaRealizada, wayPointOrigem, wayPointDestino);

                }
            }
            return distancia;
        }

        #endregion
    }
}

