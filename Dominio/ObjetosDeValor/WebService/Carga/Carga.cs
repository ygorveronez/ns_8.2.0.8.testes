using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class Carga
    {
        public int Protocolo { get; set; }

        public string NumeroCarga { get; set; }

        public bool CargaDePreCarga { get; set; }

        public string Origem { get; set; }

        public string FilialOrigem { get; set; }

        public string Destino { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Transportador { get; set; }

        public string DataPrevisaoEntrega { get; set; }

        public string DataCriacaoCarga { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador TipoCarga { get; set; }

        public List<Pedido> Pedidos { get; set; }

        public List<Dominio.ObjetosDeValor.WebService.Carga.CargaValePedagio> CargaValePedagio { get; set; }

        public List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoValePedagio> CargaIntegracaoValePedagio { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFrete RotaFrete { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCargaEmbarcador { get; set; }

    }
}
