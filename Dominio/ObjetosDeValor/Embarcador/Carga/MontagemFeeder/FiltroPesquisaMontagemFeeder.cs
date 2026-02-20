using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder
{
    public sealed class FiltroPesquisaMontagemFeeder
    {
        public int CodigoUsuario { get; set; }
        public string Planilha { get; set; }
        public DateTime? DataImportacaoInicial { get; set; }
        public DateTime? DataImportacaoFinal { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido Situacao { get; set; }
        public string Mensagem { get; set; }

        public int Viagem { get; set; }
        public int PortoOrigem { get; set; }
        public int PortoDestino { get; set; }
        public string NumeroBooking { get; set; }
    }
}
