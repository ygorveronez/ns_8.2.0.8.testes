using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public class DisponibilidadeCarregamento
    {
        public PreCarga.PreCarga PreCarga { get; set; }
        public List<Pessoas.Pessoa> Destinatarios { get; set; }
        public string DataHoraEntrega { get; set; }
        public bool ReservarHorarioSeAtendeEntrega { get; set; }

    }
}
