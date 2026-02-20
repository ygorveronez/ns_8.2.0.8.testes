namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemFotoDadosNFEGestaoDadosColeta
    {
        GestaoDadosColeta = 0,
        ControleDeEntrega = 1
    }

    public static class TipoFotoDadosNFEGestaoDadosColetaHelper
    {
        public static string ObterDescricao(this OrigemFotoDadosNFEGestaoDadosColeta origem)
        {
            switch (origem)
            {
                case OrigemFotoDadosNFEGestaoDadosColeta.GestaoDadosColeta: return "Gestão de Dados de Coleta";
                case OrigemFotoDadosNFEGestaoDadosColeta.ControleDeEntrega: return "Controle de Entrega";
                default: return string.Empty;
            }
        }
    }
}
