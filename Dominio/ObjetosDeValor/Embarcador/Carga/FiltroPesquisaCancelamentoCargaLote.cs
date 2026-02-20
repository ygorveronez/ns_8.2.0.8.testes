using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaCancelamentoCargaLote
    {
        public List<TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }
        public List<int> CodigoTipoOperacao { get; set; }
        public DateTime DataInicioEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public double CnpjTomador { get; set; }
        public double CnpjRemetente { get; set; }
        public double CnpjDestinatario { get; set; }
        public string NumeroBooking { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoPedidoViagemDirecao { get; set; }
        public int CodigoTerminalOrigem { get; set; }
        public int CodigoTerminalDestino { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoMotorista { get; set; }
        public List<TipoPropostaMultimodal> TipoPropostaMultimodal { get; set; }
        public List<SituacaoCarga> Situacoes { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public List<int> CodigosCarga { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
    }
}
