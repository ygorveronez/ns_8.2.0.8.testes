using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento
{
    public class DataOccurrenceCreate : SuperAppData
    {
        public string _id { get; set; }
        public Source Source { get; set; }
        public Category Category { get; set; }
        public Owner Owner { get; set; }
        public List<Evidence>? Evidences { get; set; }
        public Response? Response { get; set; }
    }

    public class Source
    {
        public string _id { get; set; }
        public string Type { get; set; }
    }
    public class Category
    {
        public string _id { get; set; }
        public string ExternalId { get; set; }
    }


    public class Owner
    {
        public string _id { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public Document Document { get; set; }
    }
}
