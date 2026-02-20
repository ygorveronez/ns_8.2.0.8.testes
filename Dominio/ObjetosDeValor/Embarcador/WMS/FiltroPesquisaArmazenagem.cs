using System;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.WMS
{
    public class FiltroPesquisaArmazenagem
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public TipoEntradaSaida? Tipo { get; set; }
        public StatusNFe? StatusNF { get; set; }
    }
}