namespace SGT.WebServiceMagalu.Base
{
    public class Conexao
    {
        public static string StringConexao
        {
            get
            {
                string stringConexao = System.Configuration.ConfigurationManager.ConnectionStrings["ControleCTe"].ToString();
                return stringConexao;
            }
        }
    }
}