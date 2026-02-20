using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class BiddingRotaFiltro
    {
        public List<LocalidesBidding> ListaRotas { get; set; }
        public List<LocalidesBidding> ListaFilial { get; set; }
        public List<LocalidesBidding> ListaOrigem { get; set; }
        public List<LocalidesBidding> ListaDestino { get; set; }
        public List<LocalidesBidding> ListaMesorregiaoDestino { get; set; }
        public List<LocalidesBidding> ListaMesorregiaoOrigem { get; set; }
        public List<int> ListaQuantidadeEntregas { get; set; }
        public List<int> ListaQuantidadeAjudantes { get; set; }
        public List<int> ListaQuantidadeViagensAno { get; set; }
        public List<LocalidesBidding> ListaRegiaoDestino { get; set; }
        public List<LocalidesBidding> ListaRegiaoOrigem { get; set; }
        public List<LocalidesBidding> ListaClienteDestino { get; set; }
        public List<LocalidesBidding> ListaClienteOrigem { get; set; }
        public List<LocalidesBidding> ListaRotaDestino { get; set; }
        public List<LocalidesBidding> ListaRotaOrigem { get; set; }
        public List<LocalidesBidding> ListaEstadoDestino { get; set; }
        public List<LocalidesBidding> ListaEstadoOrigem { get; set; }
        public List<LocalidesBidding> ListaPaisDestino { get; set; }
        public List<LocalidesBidding> ListaPaisOrigem { get; set; }
        public bool PossuiCEPDestino { get; set; }
        public bool PossuiCEPOrigem { get; set; }
        public List<LocalidesBidding> ListaModelosVeiculares { get; set; }
        public string FiltrosString { get; set; }
    }
}
