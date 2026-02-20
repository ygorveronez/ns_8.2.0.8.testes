using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CarregamentoRoteirizacaoDadosValidos
    {
        public Entidades.Embarcador.Filiais.Filial Filial { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }
        public Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }
        public Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }
        public Entidades.Veiculo Veiculo { get; set; }
        public Entidades.Usuario Motorista { get; set; }
        public Entidades.Empresa Transportador { get; set; }
        public Entidades.Embarcador.Cargas.TipoCarregamento TipoCarregamento { get; set; }
        public Dominio.Entidades.RotaFrete RotaFrete { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }
    }
}

