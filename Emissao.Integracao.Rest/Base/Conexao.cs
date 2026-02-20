namespace Emissao.Integracao.Rest.Base
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
    }
}