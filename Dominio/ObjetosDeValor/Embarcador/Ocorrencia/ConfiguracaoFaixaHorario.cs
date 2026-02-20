using System;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class ConfiguracaoFaixaHorario
    {
        public TimeSpan HoraInicial { get; set; }

        public TimeSpan HoraFinal { get; set; }

        public decimal Valor { get; set; }
    }
}
