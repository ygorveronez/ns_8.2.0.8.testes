using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class User
    {
        public string _id { get; set; }
        public string name { get; set; }
    }

    public class ChatMobileMensagem
    {
        public string _id { get; set; }
        public string text { get; set; }
        public DateTime createdAt { get; set; }
        public int room { get; set; }
        public User user { get; set; }
    }
}
