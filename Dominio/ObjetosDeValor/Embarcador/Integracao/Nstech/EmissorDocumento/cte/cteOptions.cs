using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteOptions
    {
        public bool removeSpecialsChars { get; set; }
        public cteOptionsDacte dacte { get; set; }
    }

    public class cteOptionsDacte
    {
        public bool enabled { get; set; }
        public cteOptionsDacteNotifications notifications { get; set; }
    }

    public class cteOptionsDacteNotifications
    {
        public bool enabled { get; set; }
        public List<cteOptionsDacteNotificationsContacts> contacts { get; set; }
    }

    public class cteOptionsDacteNotificationsContacts
    {
        public string type { get; set; }
        public string email { get; set; }
    }
}