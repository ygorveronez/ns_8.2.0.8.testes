namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaIntegracao
    {
        Todos = 0,
        Manual = 1,
        OKColeta = 2,
        ClienteFTPOKColeta = 3,
        ClienteFTP = 4,
        ConflitoClienteFTPOKColeta = 5,
        NaoRecebido = 6,
        OKColetaManual = 7,
        ClienteFTPManual = 8
    }

    public static class FormaIntegracaoHelper
    {
        public static string ObterDescricao(this FormaIntegracao forma)
        {
            switch (forma)
            {
                case FormaIntegracao.Todos: return "Todos";
                case FormaIntegracao.Manual: return "Manual";
                case FormaIntegracao.OKColeta: return "OK Coleta";
                case FormaIntegracao.ClienteFTPOKColeta: return "Cliente/FTP + OK Coleta";
                case FormaIntegracao.ClienteFTP: return "Cliente/FTP";
                case FormaIntegracao.ConflitoClienteFTPOKColeta: return "Conflito Cliente/FTP + OK Coleta";
                case FormaIntegracao.NaoRecebido: return "Não Recebido";
                case FormaIntegracao.OKColetaManual: return "OK Coleta + Manual";
                case FormaIntegracao.ClienteFTPManual: return "Cliente/FTP + Manual";
                default: return string.Empty;
            }
        }
    }
}
