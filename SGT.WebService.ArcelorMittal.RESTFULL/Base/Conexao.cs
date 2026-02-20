namespace Base
{
    public class Conexao
    {
        public static string StringConexao
        {
            get
            {
                string stringConexao = "";//System.Configuration.ConfigurationManager.ConnectionStrings["ControleCTe"].ToString();
                stringConexao = "Data Source=191.232.235.86;Initial Catalog=ControleCteMarfrigHomolog;User Id=sa;Password=Multi@2017;";
                return stringConexao;
            }
        }
    }
}