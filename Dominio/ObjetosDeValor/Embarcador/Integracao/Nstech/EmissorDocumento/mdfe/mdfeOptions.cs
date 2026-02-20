using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeOptions
    {
        public bool removeSpecialsChars { get; set; }
        public mdfeOptionsDamdfe damdfe { get; set; }
    }

    public class mdfeOptionsDamdfe
    {
        public bool enabled { get; set; }
        public mdfeOptionsDamdfeNotifications notifications { get; set; }
    }

    public class mdfeOptionsDamdfeNotifications
    {
        public bool enabled { get; set; }
        public List<mdfeOptionsDamdfeNotificationsContacts> contacts { get; set; }
    }

    public class mdfeOptionsDamdfeNotificationsContacts
    {
        public string type { get; set; }
        public string email { get; set; }
    }
}