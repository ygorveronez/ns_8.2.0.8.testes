namespace Dominio.ObjetosDeValor.WebService.Frota
{
    public class Notificacao
    {
        public string event_type { get; set; }

        public int id { get; set; }

        public int company_id { get; set; }

        public Car car { get; set; }

        public Notification notification { get; set; }

        public Organization organization { get; set; }

        public Driver driver { get; set; }
         
        public string created_at { get; set; }

        public string updated_at { get; set; }
    }
}
