using System;

namespace Dominio.ObjetosDeValor.Embarcador.AutorizacaoAvaria
{
    public sealed class FiltroPesquisaAutorizacaoAvaria
    {
        public int NumeroAvaria { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoFilial { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria SituacaoAvaria { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria EtapaAutorizacaoAvaria { get; set; }
    }
}