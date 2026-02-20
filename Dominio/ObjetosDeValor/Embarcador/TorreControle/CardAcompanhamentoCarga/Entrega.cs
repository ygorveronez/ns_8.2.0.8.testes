using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga
{
    public class Entrega
    {
        #region propriedades privadas

        private string _formatDataHora = "dd-MM-yyyy HH:mm:ss";
        private static readonly string _caminhoImagem = "../../../../img/controle-entrega/";
        private static readonly string imagemForaRaio = _caminhoImagem + "fora-raio.png";
        private static readonly string imagemForaSequencia = _caminhoImagem + "fora-sequencia.png";
        private static readonly string imagemNotaCobertura = "../../../../Content/TorreControle/Icones/gerais/nota-cobertura.svg";
        private static readonly string imagemParcial = _caminhoImagem + "parcial.png";
        private static readonly string imagemSemCoordenada = _caminhoImagem + "sem-coordenada.png";
        private static readonly string imagemPedidoReentrega = "../../../../Content/TorreControle/Icones/alertas/pedido-reentrega.svg";
        private static readonly string imagemAtrasado = "../../../../Content/TorreControle/Icones/gerais/atrasado.svg";
        private static readonly string imagemEntregaFiltro = _caminhoImagem + "dentro_filtro.png";
        private static readonly string imagemPedidoEmMaisCargas = "../../../../Content/TorreControle/Icones/alertas/pedidosOutrasCargas.svg";
        private static readonly string imagemReentregarMesmaCarga = "../../../../Content/TorreControle/Icones/alertas/pedido-reentrega.svg";
        private static readonly string imagemEntregaFinalizadaViaMonitoramento = _caminhoImagem + $"entrega-finalizada-por-monitoramento.png";

        #endregion

        public int CodigoEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega Situacao { get; set; }
        public DateTime DataEntregaPrevista { get; set; }
        public virtual string DataEntregaPrevistaFormatada { get { return DataEntregaPrevista != DateTime.MinValue ? DataEntregaPrevista.ToString(_formatDataHora) : ""; } }
        public DateTime DataEntrega { get; set; }
        public virtual string DataEntregaFormatada { get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString(_formatDataHora) : ""; } }
        public DateTime DataEntregaReprogramada { get; set; }
        public virtual string DataEntregaReprogramadaFormatada { get { return DataEntregaReprogramada != DateTime.MinValue ? DataEntregaReprogramada.ToString(_formatDataHora) : ""; } }
        public DateTime DataInicioEntrega { get; set; }
        public virtual string DataInicioEntregaFormatada { get { return DataInicioEntrega != DateTime.MinValue ? DataInicioEntrega.ToString(_formatDataHora) : ""; } }
        public DateTime DataFIMEntrega { get; set; }
        public virtual string DataFIMEntregaFormatada { get { return DataFIMEntrega != DateTime.MinValue ? DataFIMEntrega.ToString(_formatDataHora) : ""; } }
        public bool Coleta { get; set; }
        public int OrdemPrevista { get; set; }
        public int OrdemRealizada { get; set; }
        public int EntregaNaJanela { get; set; }
        public bool MotoristaACaminho { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool ColetaEquipamento { get; set; }
        public bool ColetaAdicional { get; set; }
        public DateTime DataEntradaRaio { get; set; }
        public virtual string DataEntradaRaioFormatada { get { return DataEntradaRaio != DateTime.MinValue ? DataEntradaRaio.ToString(_formatDataHora) : ""; } }
        public DateTime DataSaidaRaio { get; set; }
        public virtual string DataSaidaRaioFormatada { get { return DataEntradaRaio != DateTime.MinValue ? DataEntradaRaio.ToString(_formatDataHora) : ""; } }
        public DateTime DataInicioDescarga { get; set; }
        public virtual string DataInicioDescargaFormatada { get { return DataInicioDescarga != DateTime.MinValue ? DataInicioDescarga.ToString(_formatDataHora) : ""; } }
        public DateTime DataTerminoDescarga { get; set; }
        public virtual string DataTerminoDescargaFormatada { get { return DataTerminoDescarga != DateTime.MinValue ? DataTerminoDescarga.ToString(_formatDataHora) : ""; } }
        public virtual bool DevolucaoParcial { get; set; }
        public bool EntregaNoRaio { get; set; }
        public bool Reentrega { get; set; }
        public bool Fronteira { get; set; }
        public bool PossuiNotaCobertura { get; set; }
        public bool ChamadoEmAberto { get; set; }
        public bool RealizadaNoPrazo { get; set; }
        public TendenciaEntrega TendendiaEntrega { get; set; }
        public virtual string Imagem
        {
            get
            {

                if (this.Fronteira)
                {
                    switch (this.Situacao)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue:
                            return $"Content/TorreControle/Icones/alertas/fronteira-realizada.svg";
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado:
                            return $"Content/TorreControle/Icones/alertas/fronteira-nao-realizada.svg";
                        default:
                            return "Content/TorreControle/Icones/alertas/fronteira-pendente.svg";
                    }
                }

                if (this.Coleta)
                {
                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                        return _caminhoImagem + $"coleta-realizada.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmFinalizacao)
                        return _caminhoImagem + $"coleta_finalizacao_assincrona.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado)
                        return _caminhoImagem + $"coleta-nao-realizada.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida)
                        return _caminhoImagem + $"coleta-revertida.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Reentergue)
                        return _caminhoImagem + $"coleta-reentregue.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento)
                        return _caminhoImagem + $"coleta-atendimento.png";

                    //if (Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(entrega.Cliente, posicaoAtual?.Latitude ?? 0, posicaoAtual?.Longitude ?? 0, raioPadrao))
                    //    return _caminhoImagem + $"coleta-veiculo-no-raio.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente)
                        return _caminhoImagem + $"coleta-veiculo-no-raio.png";

                    if (this.Reentrega)
                        return _caminhoImagem + $"coleta-reentregue.png";

                    //if (entrouESaiuDoRaioSemEntregar && configuracaoEmbarcador.HabilitarEstadoPassouRaioSemConfirmar)
                    //    return _caminhoImagem + $"coleta-entrou-e-saiu-sem-entregar.png";

                    if (this.MotoristaACaminho)
                        return _caminhoImagem + "coleta-motorista-chegou.png";

                    return _caminhoImagem + "coleta-pendente.png";
                }
                else
                {
                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                        return _caminhoImagem + $"entrega-realizada.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmFinalizacao)
                        return _caminhoImagem + $"entrega_finalizacao_assincrona.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado)
                        return _caminhoImagem + $"entrega-devolvida.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida)
                        return _caminhoImagem + $"entrega-revertida.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Reentergue)
                        return _caminhoImagem + $"entrega-reentregue.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento)
                        return _caminhoImagem + $"entrega-atendimento.png";

                    //if (noRaio && (dataLimitePermanenciaRaio.HasValue && DateTime.Now > dataLimitePermanenciaRaio.Value))
                    //    return _caminhoImagem + $"entrega-veiculo-no-raio-atrasado.png";

                    if (this.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente)
                        return _caminhoImagem + $"entrega-veiculo-no-raio.png";

                    if (this.Reentrega)
                        return _caminhoImagem + $"entrega-reentregue.png";

                    //if (entrouESaiuDoRaioSemEntregar && configuracaoEmbarcador.HabilitarEstadoPassouRaioSemConfirmar)
                    //    return _caminhoImagem + $"entrega-entrou-e-saiu-sem-entregar.png";

                    if (this.MotoristaACaminho)
                        return _caminhoImagem + "entrega-motorista-chegou.png";

                    return _caminhoImagem + "entrega-pendente.png";
                }
            }
        }
        public virtual string ImagemForaSequencia { get { return ObterSituacaoEntregaFinalizada(this.Situacao) && !(this.OrdemPrevista == this.OrdemRealizada) ? imagemForaSequencia : string.Empty; } }
        public virtual string ImagemNotaCobertura { get { return this.PossuiNotaCobertura ? imagemNotaCobertura : string.Empty; } }
        public virtual string ImagemForaRaio { get { return this.ObterSituacaoEntregaFinalizada(this.Situacao) && !this.EntregaNoRaio ? imagemForaRaio : string.Empty; } }
        public virtual string ImagemParcial { get { return this.DevolucaoParcial ? imagemParcial : string.Empty; } }
        public virtual string ImagemSemCoordenada { get { return string.IsNullOrWhiteSpace(this.Cliente[0]?.Latitude ?? "") ? imagemSemCoordenada : string.Empty; } }
        public virtual string ImagemPedidoReentrega { get { return this.Reentrega ? imagemPedidoReentrega : string.Empty; } }
        public virtual string ImagemAtrasado { get; set; }

        public virtual string ImagemPedidoEmMaisCargas { get { return this.Cliente.Any(obj => obj.PedidoEmMaisCargas > 0) ? imagemPedidoEmMaisCargas : string.Empty; } }
        public virtual string ImagemReentregarMesmaCarga { get { return this.Situacao == SituacaoEntrega.ReentregarMesmaCarga ? imagemReentregarMesmaCarga : string.Empty; } }

        public List<Cliente> Cliente { get; set; }

        public string ObterAtrasado(Entrega entrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador)
        {
            if (!entrega.Coleta && entrega.DataEntradaRaio != DateTime.MinValue && entrega.DataEntradaRaio < DateTime.Now && SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(entrega.Situacao) && ConfiguracaoEmbarcador.HabilitarIconeEntregaAtrasada)
            {
                return imagemAtrasado;
            }

            return "";
        }

        private bool ObterSituacaoEntregaFinalizada(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega Situacao)
        {
            return Situacao == Enumeradores.SituacaoEntrega.Entregue || Situacao == Enumeradores.SituacaoEntrega.Reentergue || Situacao == Enumeradores.SituacaoEntrega.Rejeitado;
        }

        public virtual bool DestacarFiltro { get; set; }

        public bool IdentificarFiltrosConsultados(Entrega entrega, Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaAcompanhamentoCarga filtrosPesquisa)
        {
            if (entrega != null && filtrosPesquisa != null)
            {
                if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroPedidoEmbarcador))
                    return entrega.Cliente.Any(y => y.Pedido != null && y.Pedido.Any(t => t.Numero == filtrosPesquisa.NumeroPedidoEmbarcador));

                if (filtrosPesquisa.NumeroNotasFiscais?.Count > 0)
                    return entrega.Cliente.Any(y => y.NotaFiscal != null && y.NotaFiscal.Any(t => filtrosPesquisa.NumeroNotasFiscais.Contains(t.Codigo)));

                //if (filtrosPesquisa.CodigosVendedor?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CodigosVendedor.Contains(p.CargaPedido.Pedido.FuncionarioVendedor?.Codigo ?? 0)))
                //    return true; NECESSARIO TRAZER O VENDEDOR NA LISTA PEDIDOS..

                //if (filtrosPesquisa.CodigosSupervisor?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CodigosSupervisor.Contains(p.CargaPedido.Pedido.FuncionarioSupervisor?.Codigo ?? 0)))
                //    return true; NECESSARIO TRAZER O SUPERVISOR NA LISTA PEDIDOS..

                //if (filtrosPesquisa.CodigosGerente?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CodigosGerente.Contains(p.CargaPedido.Pedido.FuncionarioGerente?.Codigo ?? 0)))
                //    return true; NECESSARIO TRAZER O GERENTE NA LISTA PEDIDOS..

            }

            return false;
        }

        public bool IdentificarPedidoEmOutraCarga(List<Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Pedido> pedidos)
        {



            return false;
        }

    }
}