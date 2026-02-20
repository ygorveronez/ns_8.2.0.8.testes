namespace WSValidacaoCommerce
{
    public class Conexao
    {
        public static string StringConexao
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["ValidacaoCommerce"].ToString();
            }
        }
    }
}