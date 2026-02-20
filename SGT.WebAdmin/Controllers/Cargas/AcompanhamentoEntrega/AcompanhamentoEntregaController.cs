using Dominio.Entidades.Embarcador.Cargas.ControleEntrega;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.AcompanhamentoEntrega
{
    [CustomAuthorize("Cargas/AcompanhamentoEntrega")]
    public class AcompanhamentoEntregaController : BaseController
    {
		#region Construtores

		public AcompanhamentoEntregaController(Conexao conexao) : base(conexao) { }

		#endregion


        private int _controleProxima = 0;

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigocargaEmbarcador"),
                CodigosVeiculo = Request.GetListParam<int>("Veiculos"),
                CpfCnpjEmitentes = Request.GetListParam<double>("Emitente"),
                ExibirEntregaAntesEtapaTransporte = ConfiguracaoEmbarcador.ExibirEntregaAntesEtapaTransporte,
                ExibirSomenteCargasComVeiculo = Request.GetBoolParam("ExibirSomenteCargasComVeiculo"),
                NumeroPedido = Request.GetIntParam("Pedido"),
                ExibirSomenteCargasComChamadoAberto = Request.GetBoolParam("ExibirSomenteCargasComChamadoAberto"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                ExibirSomenteCargasComChatNaoLido = Request.GetBoolParam("ExibirSomenteCargasComChatNaoLido"),
                ExibirSomenteCargasComReentrega = Request.GetBoolParam("ExibirSomenteCargasComReentrega"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoCargaEmbarcadorMulti = Request.GetListParam<string>("CodigoCargaEmbarcadorMulti"),
                ExibirSomenteCargasComRecebedor = Request.GetBoolParam("ExibirSomenteCargasComRecebedor"),
                NumeroNotasFiscais = Request.GetListParam<int>("NumeroNotaFiscal"),
                CpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                DescSituacaoEntrega = Request.GetStringParam("DescSituacaoEntrega"),
                StatusViagem = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega>("StatusViagemControleEntrega")
            };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                filtrosPesquisa.NumeroPedidosEmbarcador = new List<string>();
            else
                filtrosPesquisa.NumeroPedido = 0;

            return filtrosPesquisa;
        }

        private dynamic ObterDetalhesControleEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, ConfiguracaoWidgetUsuario configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            string Placas = carga?.DadosSumarizados?.Veiculos ?? ""; //carga != null ? ObterPlacas(carga.Veiculo, carga.VeiculosVinculados) : string.Empty;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasDaCarga = cargaEntregas.Where(o => o.Carga.Codigo == carga.Codigo).OrderBy(o => o.Ordem).ToList();
            _controleProxima = 0;
            var retorno = new
            {
                CargaCancelada = carga != null && (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada),
                Carga = carga?.CodigoCargaEmbarcador ?? "",
                NumeroCarregamento = servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador),
                DataInicioViagem = carga.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioViagemPrevista = carga.DataInicioViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioViagemReprogramada = carga.DataInicioViagemReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                carga.DiferencaInicioViagem,
                DataFimViagem = carga.DataFimViagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagemPrevista = carga.DataFimViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagemReprogramada = carga.DataFimViagemReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                carga.DiferencaFimViagem,
                Placas = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? (carga?.CodigoCargaEmbarcador ?? (carga?.Veiculo?.Placa ?? "")) : carga?.Veiculo?.Placa != null ? carga?.Veiculo?.Placa : ("Carga: " + carga?.CodigoCargaEmbarcador ?? string.Empty),
                Datacarga = carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Remetente = carga?.DadosSumarizados?.CodigoIntegracaoRemetentes ?? string.Empty,
                Transportador = carga != null ? carga.Empresa?.NomeFantasia : string.Empty,
                TipoOperacao = carga != null ? carga.TipoOperacao?.Descricao : string.Empty,
                Destinatario = carga?.DadosSumarizados?.CodigoIntegracaoDestinatarios, //carga != null ? ConcatenarDestinatarios(cargasPedidoDaCarga.ToList()) : string.Empty,
                CodigoVeiculo = carga.Veiculo?.Codigo ?? 0,
                Quantidade = carga?.DadosSumarizados?.PesoTotal ?? 0,
                Tooltip = ObterTooltipCarga(carga, Placas, entregasDaCarga),
                PermiteAdicionarColeta = carga.TipoOperacao?.PermiteAdicionarColeta ?? false,
                PermiteAdicionarReentrega = !carga.DataInicioViagem.HasValue,
                PermiteDownloadBoletimViagem = carga.TipoOperacao?.ConfiguracaoControleEntrega?.EnviarBoletimViagemAoFinalizarViagem ?? false,
                UtimaEntrega = entregasDaCarga.Select(o => o.Cliente?.CodigoIntegracao).LastOrDefault(),
                EstadoCidadeUtimaEntrega = entregasDaCarga.Select(o => o.Cliente?.Localidade.DescricaoCidadeEstado).LastOrDefault(),
                Monitoramento = ObterInformacoesMonitoramento(carga, unitOfWork),
                InformacoesCarga = ObterInformacoesCarga(carga, entregasDaCarga, unitOfWork),
                Entregas = (from entrega in entregasDaCarga
                            select new
                            {
                                Pessoa = entrega?.Cliente?.Descricao ?? string.Empty,
                                entrega.Codigo,
                                DataPrevista = entrega?.DataPrevista?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                DataReprogramada = entrega?.DataReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                DataRealizada = ObterDataRealizada(entrega),
                                DataEntrega = entrega.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? "",
                                DataChegada = entrega?.DataInicio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                NumeroCTe = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && entrega.NotasFiscais != null && entrega.NotasFiscais.Count > 0 ? RetornarNumerosCTes(entrega.NotasFiscais.Where(p => p.PedidoXMLNotaFiscal != null)?.Select(p => p.PedidoXMLNotaFiscal.Codigo).ToList() ?? null, unitOfWork) : "",
                                CodigoCliente = !string.IsNullOrEmpty(entrega?.Cliente?.CodigoIntegracao) ? entrega?.Cliente?.CodigoIntegracao : entrega?.Cliente?.Codigo.ToString(),
                                Cliente = entrega?.Cliente?.Nome,
                                EstadoCidadeCliente = entrega?.Cliente?.Localidade.DescricaoCidadeEstado,
                                Descricao = entrega?.Cliente?.CPF_CNPJ,
                                EntergaNaJanela = entrega.StatusPrazoEntrega,
                                entrega.ChamadoEmAberto,
                                Tooltip = ObterTooltipEntrega(carga, entrega),
                                Distancia = (entrega.Distancia > 0 ? (entrega.Distancia / 1000).ToString("n3") : "0") + " KM",
                                PrevisaoEntergaNaJanela = string.Empty,//Verificar
                                entrega?.Situacao,
                                entrega?.DiferencaEntrega,
                                entrega?.Ordem,
                                entrega.Coleta,
                                SituacaoEntrega = IdentificarSituacaoDaEntrega(entrega),
                                DestacarFiltrosConsultados = IdentificarPedidoOuNotaEntrega(entrega, filtrosPesquisa)
                            }).OrderBy(o => o.Ordem).ThenBy(o => o.Coleta).ToList(),
            };


            return retorno;
        }

        private string ObterInformacoesCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<CargaEntrega> entregasDaCarga, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = string.Empty;

            string Transportador = carga.Empresa != null ? carga.Empresa.RazaoSocial : string.Empty;
            retorno += Transportador + " - ";

            string nomeMotorista = carga.DadosSumarizados?.Motoristas ?? string.Empty;
            string primeiroNome = nomeMotorista.Split(' ').FirstOrDefault();
            string nomeMotorita = primeiroNome ?? "-";
            retorno += nomeMotorita;

            string PlacaVeiculo = carga.PlacasVeiculos ?? string.Empty;
            retorno += " - " + PlacaVeiculo;

            return retorno;
        }

        private string ObterInformacoesMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentocarga = repMonitoramento.BuscarUltimoPorCarga(carga.Codigo);
            List<(string Descricao, string Valor)> informacoes = new List<(string Descricao, string Valor)>();

            if (monitoramentocarga != null)
            {
                decimal percentualPercorrido = 0;
                decimal distanciaPrevista = (decimal)monitoramentocarga.DistanciaPrevista;
                decimal distanciaPercorrida = monitoramentocarga.DistanciaRealizada;
                if (distanciaPrevista > 0)
                {
                    percentualPercorrido = decimal.Round((distanciaPercorrida * 100) / distanciaPrevista, 2, MidpointRounding.AwayFromZero);
                    return string.Concat(percentualPercorrido.ToString(), "% (", decimal.Round(distanciaPercorrida, 2, MidpointRounding.AwayFromZero).ToString("n2"), " de ", decimal.Round(distanciaPrevista, 2, MidpointRounding.AwayFromZero).ToString("n2"), "km)");
                }
                else
                    return "Sem informações de monitoramento";
            }

            return "Sem informações de monitoramento";

        }

        private string ObterTooltipCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, string placas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasDaCarga)
        {
            int total = entregasDaCarga.Count();
            int jaRealizadas = entregasDaCarga.Where(obj => obj.Situacao != SituacaoEntrega.EmCliente && obj.Situacao != SituacaoEntrega.AgAtendimento && obj.Situacao != SituacaoEntrega.NaoEntregue).Count();

            int percentual = (int)(jaRealizadas * 100 / total);

            decimal? pesoCarga = carga?.DadosSumarizados?.PesoTotal;
            decimal? pesoReentrega = entregasDaCarga.Where(obj => obj.Reentrega).Sum(obj => obj.PesoPedidosReentrega);
            decimal? pesoTotal = pesoCarga + pesoReentrega;

            string placaOuEmpurrador = (!string.IsNullOrWhiteSpace(carga.NomeEmpurrador)) ? $"Empurrador: {carga.NomeEmpurrador}<BR>" : $"Placa: {placas}<BR>";

            return
                $"Carga: {carga.CodigoCargaEmbarcador}<BR>" +
                placaOuEmpurrador +
                $"Transportador: { carga?.Empresa?.Descricao ?? string.Empty}<BR>" +
                $"Peso: {pesoCarga?.ToString("n3") ?? string.Empty}<BR>" +
                (pesoReentrega > 0 ? $"Peso Reentrega: {pesoReentrega?.ToString("n3") ?? string.Empty}<BR>" : string.Empty) +
                (pesoReentrega > 0 ? $"Peso Total: {pesoTotal?.ToString("n3") ?? string.Empty}<BR>" : string.Empty) +
                $"% Conclusão: { percentual}% ({ jaRealizadas}/{ total})" +
                (carga.Rota != null ? $"<BR>Rota: {carga.Rota.Descricao}" : "");

        }

        private string ObterTooltipEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega)
        {
            if (entrega.Coleta)
            {
                return
                   $"Remetente: {entrega.Cliente?.Nome}<BR>" +
                   $"Saida Prevista:  { entrega.DataFimPrevista?.ToString("dd/MM/yyyy HH:mm") ?? ""}<BR>" +
                   $"Saida Realizada:  { entrega.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm") ?? ""}<BR>";
            }
            else
            {

                return
                    $"Destinatario: {entrega.Cliente?.Nome}<BR>" +
                    $"Chegada Prevista: { entrega.DataPrevista?.ToString("dd/MM/yyyy HH:mm") ?? ""}<BR>" +
                    $"Saida Prevista:  { entrega.DataFimPrevista?.ToString("dd/MM/yyyy HH:mm") ?? ""}<BR>" +
                    $"Chegada Realizada:  { entrega.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? ""}<BR>" +
                    $"Saida Realizada:  { entrega.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm") ?? ""}<BR>";
            }
        }

        private string RetornarNumerosCTes(List<int> codigosPedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (codigosPedidoXMLNotaFiscal == null || codigosPedidoXMLNotaFiscal.Count == 0)
                return "";
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<int> numerosCTes = repCargaPedidoXMLNotaFiscalCTe.NumerosCTesPorNotasFiscais(codigosPedidoXMLNotaFiscal);
            if (numerosCTes != null && numerosCTes.Count > 0)
                return string.Join(", ", numerosCTes);
            else
                return "";
        }

        //private dynamic ObterInformacoesComplementares(Dominio.Entidades.Embarcador.Cargas.Carga carga, ConfiguracaoWidgetUsuario configuracao, List<CargaEntrega> entregasDaCarga, Repositorio.UnitOfWork unitOfWork)
        //{
        //    List<(string Descricao, string Valor, bool Icone)> informacoes = new List<(string Descricao, string Valor, bool Icone)>();

        //    if (configuracao?.ExibirNomeMotorista ?? false)
        //    {
        //        string nomeMotorista = carga.DadosSumarizados?.Motoristas ?? string.Empty;
        //        string primeiroNome = nomeMotorista.Split(' ').FirstOrDefault();
        //        string nomeMotorita = primeiroNome ?? "-";
        //        informacoes.Add(ValueTuple.Create("nome-motorista", nomeMotorita, false));
        //    }

        //    if (configuracao?.ExibirNumeroCarga ?? false)
        //    {
        //        informacoes.Add(ValueTuple.Create("numero-carga", carga.CodigoCargaEmbarcador, false));
        //    }

        //    if (configuracao?.ExibirValorTotalProdutos ?? false)
        //    {
        //        decimal? valorTotal = carga.DadosSumarizados.ValorTotalProdutos;
        //        string valor = valorTotal?.ToString("n2") ?? "-";
        //        informacoes.Add(ValueTuple.Create("valor-total-produtos", valor, false));
        //    }
        //    if (configuracao?.ExibirNumeroPedidoCliente ?? false)
        //    {
        //        string pedidoCliente = carga.DadosSumarizados?.CodigoPedidoCliente ?? string.Empty;
        //        informacoes.Add(ValueTuple.Create("numero-pedido-cliente", pedidoCliente, false));
        //    }

        //    if (configuracao?.ExibirPrevisaoProximaParada ?? false)
        //    {
        //        CargaEntrega entrega = (from obj in entregasDaCarga where obj.Situacao == SituacaoEntrega.NaoEntregue select obj).FirstOrDefault();
        //        string previsao = entrega?.DataEntregaPrevista?.ToString("dd/MM HH:mm") ?? "-";
        //        informacoes.Add(ValueTuple.Create("previsao-proxima-parada", previsao, false));
        //    }

        //    if (configuracao?.ExibirDistanciaRota ?? false)
        //    {
        //        decimal? quilometros = carga.Rota?.Quilometros;
        //        string distancia = quilometros.HasValue ? $"{quilometros?.ToString("n2")}km" : "-";
        //        informacoes.Add(ValueTuple.Create("distancia-rota", distancia, false));
        //    }

        //    if (configuracao?.ExibirTempoRota ?? false)
        //    {
        //        int? horas = carga.Rota?.TempoDeViagemEmHoras;
        //        string tempo = horas.HasValue ? $"{horas}h" : "-";
        //        informacoes.Add(ValueTuple.Create("tempo-rota", tempo, false));
        //    }

        //    if (configuracao?.ExibirEntregaColetasRealizadas ?? false)
        //    {
        //        int quantidadeEntregues = (from obj in entregasDaCarga where obj.Situacao == SituacaoEntrega.Entregue select obj).Count();
        //        int quantidadeTotal = entregasDaCarga.Count();
        //        string realizadoETotal = $"{quantidadeEntregues}/{quantidadeTotal}";
        //        informacoes.Add(ValueTuple.Create("entrega-coletas-realizadas", realizadoETotal, false));
        //    }

        //    return (from info in informacoes
        //            select new
        //            {
        //                info.Descricao,
        //                info.Valor,
        //                info.Icone,
        //            }).ToList();
        //}

        private string ObterDataRealizada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega)
        {
            string retorno = string.Empty;
            if (entrega?.DataInicio != null)
                retorno += $" {entrega.DataInicio?.ToString("dd/MM/yyyy HH:mm") }";

            if (entrega?.DataFim != null)
                retorno += $" / {entrega.DataFim?.ToString("dd/MM/yyyy HH:mm") }";

            return retorno;

        }

        private bool IdentificarPedidoOuNotaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa)
        {
            if (entrega != null && !entrega.Coleta && filtrosPesquisa != null)
            {
                if (filtrosPesquisa.NumeroPedido > 0)
                {
                    bool pedidoNestaEntrega = entrega.Pedidos.Where(o => o.CargaPedido.Pedido.Numero == filtrosPesquisa.NumeroPedido).FirstOrDefault() != null;
                    if (pedidoNestaEntrega) return true;
                }

                if (filtrosPesquisa.NumeroNotasFiscais.Count > 0)
                {
                    bool notaFiscalNestaEntrega = entrega.NotasFiscais.Where(o => filtrosPesquisa.NumeroNotasFiscais.Contains(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero)).FirstOrDefault() != null;
                    if (notaFiscalNestaEntrega) return true;
                }
            }
            return false;
        }

        private SituacaoEntregaControleEntrega IdentificarSituacaoDaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega)
        {
            //Para saber se a entrega ou coleta estão em atraso deve-se comparar a data de previsão de entrega 
            //e se não for concluída a data atual (se foi concluída a data de fim de entrega)
            //primeira entrega q achar como nao entregue, é a proxima;

            if (_controleProxima == 0 && entrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue || entrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente)
            {
                if (entrega.Situacao == SituacaoEntrega.NaoEntregue)
                {
                    _controleProxima = 1;
                    //proxima entrega em transito, ainda nao entregue
                    if ((entrega.DataPrevista != null && entrega.DataPrevista != DateTime.MinValue) && (entrega.DataReprogramada != null && entrega.DataReprogramada != DateTime.MinValue))
                    {
                        //esta em transito.. tem q verificar pela data entrega reprogramada
                        var DataAtual = entrega.DataReprogramada;
                        if ((DataAtual - entrega.DataPrevista <= new TimeSpan(0, 30, 0)) && (DataAtual - entrega.DataPrevista > new TimeSpan(0, 0, 1)))
                            return SituacaoEntregaControleEntrega.atraso1;

                        else if ((DataAtual - entrega.DataPrevista <= new TimeSpan(1, 00, 0)) && (DataAtual - entrega.DataPrevista > new TimeSpan(0, 30, 1)))
                            return SituacaoEntregaControleEntrega.atraso2;

                        else if (DataAtual - entrega.DataPrevista > new TimeSpan(1, 00, 0))
                            return SituacaoEntregaControleEntrega.atraso3; // codigoo preetooo

                        else
                            return SituacaoEntregaControleEntrega.emtempo;
                    }
                    else
                        return SituacaoEntregaControleEntrega.naoEntregue;
                }
                else
                {
                    //em cliente CEN_DATA_SAIDA_RAIO com data atual
                    _controleProxima = 1;
                    if (entrega.DataSaidaRaio != null && entrega.DataSaidaRaio != DateTime.MinValue)
                    {
                        //esta em transito.. tem q verificar pela data entrega reprogramada
                        var DataAtual = DateTime.Now;
                        if ((DataAtual - entrega.DataSaidaRaio <= new TimeSpan(0, 30, 0)) && (DataAtual - entrega.DataSaidaRaio > new TimeSpan(0, 0, 1)))
                            return SituacaoEntregaControleEntrega.atraso1;

                        else if ((DataAtual - entrega.DataSaidaRaio <= new TimeSpan(1, 00, 0)) && (DataAtual - entrega.DataSaidaRaio > new TimeSpan(0, 30, 1)))
                            return SituacaoEntregaControleEntrega.atraso2;

                        else if (DataAtual - entrega.DataSaidaRaio > new TimeSpan(1, 00, 0))
                            return SituacaoEntregaControleEntrega.atraso3;// codigoo preetooo

                        else
                            return SituacaoEntregaControleEntrega.emtempo;
                    }
                    else
                        return SituacaoEntregaControleEntrega.naoEntregue;

                }
            }
            else if (entrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
            {
                //verificar como foi a entrega..
                if ((entrega.DataFimPrevista != null && entrega.DataFimPrevista != DateTime.MinValue) && (entrega.DataPrevista != null && entrega.DataPrevista != DateTime.MinValue))
                {
                    var DataAtual = entrega.DataPrevista;
                    if ((DataAtual - entrega.DataFimPrevista <= new TimeSpan(0, 30, 0)) && (DataAtual - entrega.DataFimPrevista > new TimeSpan(0, 0, 1)))
                        return SituacaoEntregaControleEntrega.atraso1;

                    else if ((DataAtual - entrega.DataFimPrevista <= new TimeSpan(1, 00, 0)) && (DataAtual - entrega.DataFimPrevista > new TimeSpan(0, 30, 1)))
                        return SituacaoEntregaControleEntrega.atraso2;

                    else if (DataAtual - entrega.DataFimPrevista > new TimeSpan(1, 00, 0))
                        return SituacaoEntregaControleEntrega.atraso3;// codigoo preetooo
                    else
                        return SituacaoEntregaControleEntrega.emtempo;

                }
                else
                    return SituacaoEntregaControleEntrega.emtempo;

            }
            else
                return SituacaoEntregaControleEntrega.naoEntregue;
        }

        private List<string> ObterParametroListaString(string parametro)
        {
            List<string> itens = parametro.Split(',').ToList();

            return itens.Select(i => i.Trim()).Where(i => i.Length > 0).ToList();
        }

        #endregion

        #region Metodos Publicos

        public async Task<IActionResult> ObterAcompanhamentoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                //para testar thread atualizar tendencias.
                //Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repAcompanhamentoEntregaConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);
                //Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas = new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();
                //configuracaoTempoTendendicas = repAcompanhamentoEntregaConfiguracao.BuscarConfiguracao();

                //Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                //Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                //bool possuiMonitoramento = configuracaoEmbarcador?.PossuiMonitoramento ?? false;

                //if (configuracaoTempoTendendicas != null && possuiMonitoramento)
                //{
                //    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarTendenciasEntrega(configuracaoTempoTendendicas, unitOfWork);
                //}


                SalvarFiltroPesquisa(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa.AcompanhamentoEntrega, Request.GetStringParam("FiltroPesquisa"), unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repacompanhamentoEntregaConfiguraca = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario repConfiguracaoWidgetUsuario = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao acompanhamentoEntregaConfig = repacompanhamentoEntregaConfiguraca.BuscarConfiguracao();

                if (acompanhamentoEntregaConfig == null)
                {
                    return new JsonpResult(false, "Porfavor é necessário configurar os tempos para parametros de acompanhamento das entregas em: Cargas\\AcompanhamentoEntregaConfiguracao");
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    filtrosPesquisa.CodigosTransportador = new List<int>();
                    filtrosPesquisa.CodigosTransportador.Add(this.Empresa.Codigo);
                }

                int totalRegistros = repositorioCarga.ContarConsultarCargaAcompanhamentoEntrega(filtrosPesquisa, acompanhamentoEntregaConfig, ConfiguracaoEmbarcador);

                if (totalRegistros == 0)
                    return new JsonpResult(new List<dynamic>(), totalRegistros);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = Request.GetStringParam("Ordenacao"),
                    InicioRegistros = Request.GetIntParam("inicio"),
                    LimiteRegistros = Request.GetIntParam("limite"),
                    PropriedadeOrdenar = "DataCriacaoCarga"
                };

                IList<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargaEntrega = repositorioCarga.ConsultarCargasAcompanhamentoEntrega(filtrosPesquisa, parametrosConsulta, acompanhamentoEntregaConfig, ConfiguracaoEmbarcador);
                List<int> codigosCarga = (from carga in listaCargaEntrega select carga.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarPorCargas(codigosCarga);
                ConfiguracaoWidgetUsuario configuracao = repConfiguracaoWidgetUsuario.BuscarPorUsuario(this.Usuario.Codigo);

                List<dynamic> listaCargaEntregaRetornar = (
                    from carga in listaCargaEntrega
                    select ObterDetalhesControleEntrega(
                        carga, cargasEntrega,
                        filtrosPesquisa,
                        ConfiguracaoEmbarcador, configuracao,
                        unitOfWork
                    )
                ).ToList();

                return new JsonpResult(listaCargaEntregaRetornar, totalRegistros);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ObterResumos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                SalvarFiltroPesquisa(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa.AcompanhamentoEntrega, Request.GetStringParam("FiltroPesquisa"), unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repacompanhamentoEntregaConfiguraca = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao acompanhamentoEntregaConfig = repacompanhamentoEntregaConfiguraca.BuscarConfiguracao();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    filtrosPesquisa.CodigosTransportador = new List<int>();
                    filtrosPesquisa.CodigosTransportador.Add(this.Empresa.Codigo);
                }
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = Request.GetStringParam("Ordenacao"),
                    InicioRegistros = Request.GetIntParam("inicio"),
                    LimiteRegistros = Request.GetIntParam("limite"),
                    PropriedadeOrdenar = "DataCriacaoCarga"
                };

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AcompanhamentoCarga.AcompanhamentoCargaSumarizadoRetorno> acompanhamentoSumarizadoRetornoColetas = repositorioCarga.BuscarCargasAcompanhamentoEntregaSumarizado(filtrosPesquisa, DateTime.Now, 0, acompanhamentoEntregaConfig, ConfiguracaoEmbarcador);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AcompanhamentoCarga.AcompanhamentoCargaSumarizadoRetorno> acompanhamentoSumarizadoRetornoDestino = repositorioCarga.BuscarCargasAcompanhamentoEntregaSumarizado(filtrosPesquisa, DateTime.Now, 1, acompanhamentoEntregaConfig, ConfiguracaoEmbarcador);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AcompanhamentoCarga.AcompanhamentoCargaSumarizadoRetorno> acompanhamentoSumarizadoRetornoEmTransito = repositorioCarga.BuscarCargasAcompanhamentoEntregaSumarizado(filtrosPesquisa, DateTime.Now, 2, acompanhamentoEntregaConfig, ConfiguracaoEmbarcador);

                Dominio.ObjetosDeValor.Embarcador.Carga.AcompanhamentoCarga.AcompanhamentoCargaSumarizado result = new Dominio.ObjetosDeValor.Embarcador.Carga.AcompanhamentoCarga.AcompanhamentoCargaSumarizado
                {
                    ColetasEmTempo = acompanhamentoSumarizadoRetornoColetas.Where(obj => obj.tempo <= acompanhamentoEntregaConfig.SaidaEmTempo.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    ColetasAtraso1 = acompanhamentoSumarizadoRetornoColetas.Where(obj => obj.tempo <= acompanhamentoEntregaConfig.SaidaAtraso1.TotalMinutes && obj.tempo > (acompanhamentoEntregaConfig.SaidaEmTempo.TotalMinutes + 1)).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    ColetasAtraso2 = acompanhamentoSumarizadoRetornoColetas.Where(obj => obj.tempo > acompanhamentoEntregaConfig.SaidaAtraso1.TotalMinutes && obj.tempo < acompanhamentoEntregaConfig.SaidaAtraso2.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    ColetasAtraso3 = acompanhamentoSumarizadoRetornoColetas.Where(obj => obj.tempo > acompanhamentoEntregaConfig.SaidaAtraso3.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    DestinoEmTempo = acompanhamentoSumarizadoRetornoDestino.Where(obj => obj.tempo <= acompanhamentoEntregaConfig.DestinoEmTempo.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    DestinoAtraso1 = acompanhamentoSumarizadoRetornoDestino.Where(obj => obj.tempo <= acompanhamentoEntregaConfig.DestinoAtraso1.TotalMinutes && obj.tempo > acompanhamentoEntregaConfig.DestinoEmTempo.TotalMinutes + 1).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    DestinoAtraso2 = acompanhamentoSumarizadoRetornoDestino.Where(obj => obj.tempo > acompanhamentoEntregaConfig.DestinoAtraso1.TotalMinutes && obj.tempo < acompanhamentoEntregaConfig.DestinoAtraso2.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    DestinoAtraso3 = acompanhamentoSumarizadoRetornoDestino.Where(obj => obj.tempo > acompanhamentoEntregaConfig.DestinoAtraso3.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    EmtransitoOK = acompanhamentoSumarizadoRetornoEmTransito.Where(obj => obj.tempo <= acompanhamentoEntregaConfig.EmtransitoEmTempo.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    EmtransitoAtraso1 = acompanhamentoSumarizadoRetornoEmTransito.Where(obj => obj.tempo <= acompanhamentoEntregaConfig.EmTrasitoAtraso1.TotalMinutes && obj.tempo > acompanhamentoEntregaConfig.EmtransitoEmTempo.TotalMinutes + 1).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    EmtransitoAtraso2 = acompanhamentoSumarizadoRetornoEmTransito.Where(obj => obj.tempo > acompanhamentoEntregaConfig.EmTrasitoAtraso1.TotalMinutes && obj.tempo < acompanhamentoEntregaConfig.EmTrasitoAtraso2.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                    EmtransitoAtraso3 = acompanhamentoSumarizadoRetornoEmTransito.Where(obj => obj.tempo > acompanhamentoEntregaConfig.EmTrasitoAtraso3.TotalMinutes).GroupBy(obj => obj.codigocarga).Select(obj => new { obj.FirstOrDefault().num }).Count(),
                };

                return new JsonpResult(result, 0);

            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion


        /// <summary>
        /// atraso 1 seria para amarelo ate 30min
        /// atraso 2 vermelho, até 1 hora
        /// atraso 3 pretoooo acima de 1 hora. 
        /// </summary>
        private enum SituacaoEntregaControleEntrega
        {
            naoEntregue = 0,
            atraso1 = 1,
            atraso2 = 2,
            atraso3 = 3,
            emtempo = 4
        }


    }
}
