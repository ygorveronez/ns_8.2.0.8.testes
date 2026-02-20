using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Atendimento
{
    public class AtendimentoChamadoRedmine
    {
        public Issue issue { get; set; }
    }

    public class Issue
    {
        public int project_id { get; set; }
        public string subject { get; set; }
        public int tracker_id { get; set; }
        public int status_id { get; set; }
        public int priority_id { get; set; }
        public int assigned_to_id { get; set; }
        public int estimated_hours { get; set; }
        public string description { get; set; }
        public List<CustomField> custom_fields { get; set; }
    }


    public class CustomField
    {
        public string value { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }
}
