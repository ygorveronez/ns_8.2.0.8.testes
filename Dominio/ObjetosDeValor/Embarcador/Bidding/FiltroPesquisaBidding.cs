using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class FiltroPesquisaBidding
    {
        public string Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
        public Entidades.Empresa Empresa { get; set; }
        public int CodigoTipoBidding { get; set; }
        public List<StatusBiddingConvite> Situacao { get; set; }
        public int CodigoSolicitante { get; set; }
        public List<int> CodigosComprador { get; set; }
        public int NumeroBidding { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<int> FiliaisParticipantes { get; set; }
    }
}
