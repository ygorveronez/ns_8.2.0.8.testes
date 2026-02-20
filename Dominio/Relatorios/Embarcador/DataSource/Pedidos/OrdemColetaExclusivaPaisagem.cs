using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class OrdemColetaExclusivaPaisagem
    {
        public string Remetente { get; set; }
        public string CidadeRemetente { get; set; }
        public string BairroRemetente { get; set; }
        public string EnderecoRemetente { get; set; }

        public string Destinatario { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }
        private DateTime DataColeta { get; set; }
        public int Volume { get; set; }
        public decimal PesoTotal { get; set; }
        public string Fronteira { get; set; }

        public string Motorista { get; set; }
        public string Veiculo { get; set; }
        public string EstadoVeiculo { get; set; }
        public string Reboque { get; set; }

        public string ModeloVeicularCarga { get; set; }
        public string Observacao { get; set; }

        #region Propriedades com Regras

        public string DataColetaFormatada
        {
            get { return DataColeta != DateTime.MinValue ? DataColeta.ToDateTimeString() : string.Empty; }
        }

        #endregion
    }
}
