using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class EntregaDatasPrevistas
    {
        public DateTime? SaidaRaio { get; set; }

        public DateTime? ChegadaPrevista { get; set; }

        public DateTime? ChegadaPrevistaReprogramada { get; set; }
    }
}
