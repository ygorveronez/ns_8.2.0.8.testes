namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum Cluster
    {
        Brasil = 1,
    }

    public static class ClusterHelper
    {
        public static string ObterDescricao(this Cluster cluster)
        {
            switch (cluster)
            {
                case Cluster.Brasil: return "Brasil";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoInternacional(this Cluster cluster)
        {
            switch (cluster)
            {
                case Cluster.Brasil: return "Brazil";
                default: return string.Empty;
            }
        }
    }
}