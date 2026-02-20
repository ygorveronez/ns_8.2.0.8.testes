using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ContatosTrizy
    {
        public List<Contact> contacts { get; set; }

    }

    public class ContactItem
    {
        public string type { get; set; }

        public string value { get; set; }

        public string label { get; set; }
    }

    public class Contact
    {
        public List<ContactItem> items { get; set; }

        public string description { get; set; }

        [JsonProperty("label")]
        public string label { get; set; }
    }

}
