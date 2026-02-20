using System.Collections.Generic;

namespace Dominio.MSMQ
{
    public class Notification
    {

        #region Constructor

        public Notification()
        {

        }

        /// <summary>
        /// Construtor para notificar um usuário específico
        /// </summary>
        /// <param name="content"></param>
        /// <param name="userID"></param>
        /// <param name="msmqQueue"></param>
        /// <param name="hub"></param>
        /// <param name="service"></param>
        public Notification(dynamic content, int clientMultisoftwareID, int userID, Dominio.MSMQ.MSMQQueue msmqQueue, Dominio.SignalR.Hubs hub, string service)
        {
            this.Content = content;
            if (userID > 0)
            {
                this.UsersID = new List<int>();
                this.UsersID.Add(userID);
            }

            this.Hub = hub;
            this.Service = service;
            this.MSMQQueue = msmqQueue;
            this.ClientMultisoftwareID = clientMultisoftwareID;
        }
        /// <summary>
        /// Construtor para notificar um grupo de usuários
        /// </summary>
        /// <param name="content"></param>
        /// <param name="usersID"></param>
        /// <param name="msmqQueue"></param>
        /// <param name="hub"></param>
        /// <param name="service"></param>
        public Notification(dynamic content, int clientMultisoftwareID, List<int> usersID, Dominio.MSMQ.MSMQQueue msmqQueue, Dominio.SignalR.Hubs hub, string service)
        {
            this.Content = content;
            this.UsersID = usersID;
            this.Hub = hub;
            this.Service = service;
            this.MSMQQueue = msmqQueue;
            this.ClientMultisoftwareID = clientMultisoftwareID;
        }

        /// <summary>
        /// Contrutor para mandar um broadcast
        /// </summary>
        /// <param name="content"></param>
        /// <param name="msmqQueue"></param>
        /// <param name="hub"></param>
        /// <param name="service"></param>
        public Notification(dynamic content, int clientMultisoftwareID, Dominio.MSMQ.MSMQQueue msmqQueue, Dominio.SignalR.Hubs hub, string service)
        {
            this.Content = content;
            this.Hub = hub;
            this.Service = service;
            this.MSMQQueue = msmqQueue;
            this.ClientMultisoftwareID = clientMultisoftwareID;
        }

        #endregion


        public dynamic Content { get; set; }

        public List<int> UsersID { get; set; }

        public Dominio.SignalR.Hubs Hub { get; set; }

        public Dominio.MSMQ.MSMQQueue MSMQQueue { get; set; }

        public string Service { get; set; }

        /// <summary>
        /// Esse parametro é obrigatório para que a fila funcione direcionado para os endereços corretos.
        /// </summary>
        public int ClientMultisoftwareID { get; set; }

    }
}
