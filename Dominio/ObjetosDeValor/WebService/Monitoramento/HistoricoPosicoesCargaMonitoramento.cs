namespace Dominio.ObjetosDeValor.WebService.Monitoramento
{
    public sealed class HistoricoPosicoesCargaMonitoramento
    {
        public string Transporte { get; set; }

        public string Status { get; set; }

        public string Placa { get; set; }

        public string Origem { get; set; }

        public Cliente[] Cliente { get; set; }

        public Percursos[] Percursos { get; set; }

        public Fornecimentos[] Fornecimentos { get; set; }
    }
}