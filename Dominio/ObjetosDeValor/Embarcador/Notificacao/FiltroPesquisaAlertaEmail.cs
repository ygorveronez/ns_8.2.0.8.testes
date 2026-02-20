using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public sealed class FiltroPesquisaAlertaEmail
    {
        public string Descricao { get; set; }
        public List<int>  Setor { get; set; }
        public List<int>  Portfolio { get; set; }
        public List<int>  Irregularidade { get; set; }
        public DateTime? DataHoraInicio { get; set; }
        public DateTime? DataHoraFim { get; set; }
    }
}
