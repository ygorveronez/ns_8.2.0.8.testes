using System;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public sealed class FiltroPesquisaCadastroVeiculoAprovacao
    {
        public int Codigo { get; set; }

        public int CodigoUsuario { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoModeloVeicular { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public Enumeradores.SituacaoCadastroVeiculo Situacao { get; set; }
    }
}
