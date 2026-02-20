namespace Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia
{
    public class RelacaoOcorrencia
    {
        public string Motivo { get; set; }

        public decimal Aberto { get; set; }

        public decimal Finalizado { get; set; }

        public decimal Total {
            get
            {
                return Aberto + Finalizado;
            }
        }
    }
}
