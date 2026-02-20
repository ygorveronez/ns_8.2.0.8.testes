using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaRelatorioConsolidadoEntregas
    {
        public List<int> CodigoCarga { get; set; }
        public DateTime? DataInicioViagemPrevistaInicial { get; set; }
        public DateTime? DataInicioViagemPrevistaFinal{ get; set; }
        public DateTime? DataInicioViagemRealizadaInicial { get; set; }
        public DateTime? DataInicioViagemRealizadaFinal { get; set; }
        public DateTime? DataConfirmacaoInicial { get; set; }
        public DateTime? DataConfirmacaoFinal { get; set; }
        public int CodigoPedido { get; set; }
        public int CodigoNotaFiscal { get; set; }
        public List<int> CodigoTransportador { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoVeiculo { get; set; }
        public double CpfCnpjClienteOrigem { get; set; }
        public double CpfCnpjClienteDestino { get; set; }
        public int CodigoCidadeOrigem { get; set; }
        public int CodigoCidadeDestino { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public TipoInteracaoEntrega? TipoInteracaoInicioViagem { get; set; }
        public TipoInteracaoEntrega? TipoInteracaoChegadaViagem { get; set; }
        public StatusViagemControleEntrega? StatusViagem { get; set; }
    }
}
