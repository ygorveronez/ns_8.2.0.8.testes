using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.RFI
{
    public class FiltroPesquisaRFI
    {
        public string descricao { get; set; }
        public DateTime dataInicio { get; set; }
        public DateTime dataLimite { get; set; }
        public Entidades.Empresa empresa { get; set; }
        public List<StatusRFIConvite> situacao { get; set; }
    }
}
