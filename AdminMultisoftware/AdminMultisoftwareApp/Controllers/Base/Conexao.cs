using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminMultisoftwareApp.Controllers
{
    public class Conexao
    {

        public static string StringConexao
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["AdminMultisoftware"].ToString();
            }
        }
    }
}