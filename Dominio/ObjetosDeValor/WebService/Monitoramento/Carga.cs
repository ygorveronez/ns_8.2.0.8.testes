using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Monitoramento
{
    public sealed class Carga
    {
        public string DataChegada { get; set; }

        public string DataInicioCarregamento { get; set; }

        public Filial Filial { get; set; }

        public string NumeroCarga { get; set; }

        public string Observacao { get; set; }

        public List<Pedido> Pedidos { get; set; }

        public decimal PesoBruto { get; set; }

        public int Protocolo { get; set; }

        public Embarcador.Enumeradores.SituacaoCarga StatusCarga { get; set; }

        public string StatusMonitoramento { get; set; }

        public string StatusMonitoramentoDescricao { get; set; }

        public string TipoCarga { get; set; }

        public string TipoFrete { get; set; }

        public Empresa Transportador { get; set; }

        public Veiculo Veiculo { get; set; }

        public List<Veiculo> Reboques{ get; set; }
        public string StatusViagem { get; set; }

    }
}
