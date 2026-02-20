using Dominio.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Contabil
{
    public class FiltroPesquisaAlteracaoMercante
    {
        public int CodigoCTe { get; set; }
        public int CodigoViagem { get; set; }
        public int CodigoTerminalOrigem { get; set; }
        public int CodigoTerminalDestino { get; set; }
        public int CodigoContainer { get; set; }
        public OpcaoSimNaoPesquisa PossuiTransbordo { get; set; }
        public GeradoPendente Manifesto { get; set; }
        public GeradoPendente ManifestoTransbordo { get; set; }
        public GeradoPendente CE { get; set; }
        public string NumeroControle { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroCE { get; set; }
        public string NumeroManifesto { get; set; }
        public string NumeroManifestoTransbordo { get; set; }
        public int CodigoNavioTransbordo { get; set; }
        public List<string> StatusCTe { get; set; }
        public List<TipoCTE> TipoCTe { get; set; }
        public int CodigoPortoTransbordo { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoBalsa { get; set; }
    }
}
