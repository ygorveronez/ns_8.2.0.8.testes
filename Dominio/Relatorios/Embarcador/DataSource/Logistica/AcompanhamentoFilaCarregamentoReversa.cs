using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class AcompanhamentoFilaCarregamentoReversa
    {
        public int Codigo { get; set; }

        public DateTime DataEntradaFila { get; set; }
        public string DataEntradaFilaFormatada
        {
            get { return DataEntradaFila != DateTime.MinValue ? DataEntradaFila.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DescricaoCentroCarregamento { get; set; }

        public string DescricaoGrupoModeloVeicularCarga { get; set; }

        public string DescricaoModeloVeicularCarga { get; set; }

        public bool LojaProximidade { get; set; }

        public string LojaProximidadeDescricao {
            get { return LojaProximidade ? "Sim" : "Não"; }
        }

        public decimal DistanciaCentroCarregamento { get; set; }

        public string DistanciaCentroCarregamentoFormatada {
            get { return DistanciaCentroCarregamento.ToString("n2"); }
        }

        public string NomeMotorista { get; set; }

        public string NomeTransportador { get; set; }

        public string Reboques { get; set; }

        public string TelefoneMotorista { get; set; }

        public string Tracao { get; set; }
    }
}
