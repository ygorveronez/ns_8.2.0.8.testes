using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Integracao.Redmine
{
    public class User
    {
        public int id { get; set; }
        public string login { get; set; }
        public bool admin { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mail { get; set; }
        public string created_on { get; set; }
        public string last_login_on { get; set; }
        public string api_key { get; set; }

        /// <summary>
        /// STATUS_ANONYMOUS  = 0
        /// STATUS_ACTIVE     = 1
        /// STATUS_REGISTERED = 2
        /// STATUS_LOCKED     = 3
        /// </summary>
        public int status { get; set; }
        public List<Group> groups { get; set; }
    }
}
