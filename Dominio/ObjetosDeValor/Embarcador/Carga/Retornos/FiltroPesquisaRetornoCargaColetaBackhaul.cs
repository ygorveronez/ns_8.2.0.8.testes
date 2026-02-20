using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Retornos
{
    public sealed class FiltroPesquisaRetornoCargaColetaBackhaul
    {
        public int CodigoDestino { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoMotorista { get; set; }

        public int CodigoOrigem { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoVeiculo { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public double CpfCnpjRemetente { get; set; }

        public string NumeroCarga { get; set; }

        public SituacaoRetornoCargaColetaBackhaul? Situacao { get; set; }
    }
}
