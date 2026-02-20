using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{


    public class Carga
    {
        public int Codigo { get; set; }
        public bool ViagemIniciada { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public DateTime? DataCarregamentoCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Filial { get; set; }
        public string Origens { get; set; }
        public string Destinos { get; set; }
        public DateTime DataSaida { get; set; }
        public DateTime DataCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacao TipoOperacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pedido.TipoCarga TipoCarga { get; set; }
        public decimal Peso { get; set; }
        public decimal Pallets { get; set; }
        public int QuantidadeNotas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaColetaEntega Situacao { get; set; }
        public decimal Tempertura { get; set; }
        public string Polilinha { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada> Paradas { get; set; }
        //public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Entrega> Entregas { get; set; }
        //public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coleta> Coletas { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota> Notas { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> Produtos { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoCarga Configuracoes { get; set; }

    }
}
