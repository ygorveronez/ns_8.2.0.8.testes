namespace Dominio.ObjetosDeValor.WebService.Frota
{
    public class Multa
    {
        public string event_type { get; set; }

        public int? id { get; set; }

        public int? company_id { get; set; }

        public Car car { get; set; }

        public Frame frame { get; set; }

        public Organization organization { get; set; }

        public Driver driver { get; set; }

        public Payment payment { get; set; }

        public string link { get; set; }

        public string created_at { get; set; }

        public string updated_at { get; set; }
    }
}
