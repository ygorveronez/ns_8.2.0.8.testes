using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CotacaoFreteCarregamento
    {
        public bool CarregamentoRedespacho { get; set; }

        public int ModeloVeicularCarga { get; set; }

        public int TipoDeCarga { get; set; }

        public int Transportador { get; set; }

        public int TipoOperacao { get; set; }

        public int Veiculo { get; set; }

        public int Distancia { get; set; }

        public List<int> Pedidos { get; set; }

        // Utilizados no Fretes/SimulacaoFrete
        public decimal PesoBruto { get; set; }
        public int Filial { get; set; }
        public int Origem { get; set; }
        public List<int> Destinos { get; set; }
    }
}
