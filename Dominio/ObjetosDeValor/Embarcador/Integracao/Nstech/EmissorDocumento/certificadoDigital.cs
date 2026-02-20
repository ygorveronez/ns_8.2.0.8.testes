using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class certificadoDigital
    {
        public string id { get; set; }
        public string externalId { get; set; }
        public string ownerDocument { get; set; }
        public string rootDocument { get; set; }
        public string notAfter { get; set; }
        public string notBefore { get; set; }
        public string serialNumber { get; set; }
        public certificateCertificate certificate { get; set; }
        public certificateNotifications notifications { get; set; }

        public class certificateNotifications
        {
            public bool sendNotification { get; set; }
            public int? daysNotification { get; set; }
            public List<certificateNotificationContacts> notificationContacts { get; set; }
        }

        public class certificateNotificationContacts
        {
            public string name { get; set; }
            public string type { get; set; }
            public string contact { get; set; }
        }

        public class certificateCertificate
        {
            public string type { get; set; }
            public string certificate { get; set; }
            public string password { get; set; }
        }
    }
}