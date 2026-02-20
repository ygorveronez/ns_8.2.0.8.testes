namespace EmissaoCTe.WebAdmin
{
    public class Conexao
    {
        public static string StringConexao
        {
            get
            {
                string stringConexao = System.Configuration.ConfigurationManager.ConnectionStrings["ControleCTe"].ToString();
                Servicos.Log.SetStringConexao(stringConexao);
                return stringConexao;
            }
        }
        
        public static string AdminStringConexao
        {
            get
            {
                return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
            }
        }
    }
}