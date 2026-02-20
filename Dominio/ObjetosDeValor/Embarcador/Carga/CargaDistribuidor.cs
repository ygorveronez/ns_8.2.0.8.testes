using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaDistribuidor
    {
        public Dominio.Entidades.Embarcador.Cargas.Carga CargaAntiga { get; set; }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public decimal Distancia { get; set; }

        public bool UsarTipoOperacao { get; set; }

        public Dominio.Entidades.Cliente Expedidor { get; set; }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> CargaPedidos { get; set; }

        public Dominio.Entidades.Empresa Empresa { get; set; }

        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoTMS { get; set; }

        public bool VincularTrechos { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.Redespacho Redespacho { get; set; }

        public Dominio.Entidades.Veiculo Veiculo { get; set; }

        public bool RedespachoContainer { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public Dominio.Entidades.Cliente Recebedor { get; set; }

        public Dominio.Entidades.Usuario Motorista { get; set; }

        public List<Dominio.Entidades.Veiculo> VeiculosVinculados { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

    }
}
