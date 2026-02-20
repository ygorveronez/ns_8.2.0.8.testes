namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusControleNotaDevolucao
    {
        Todos = 0,
        AgNotaFiscal = 1,
        ComNotaFiscal = 2,
        Conferido = 3,
        Rejeitado = 4,
        ComChaveNotaFiscal = 5
    }

    public static class StatusControleNotaDevolucaoHelper
    {
        public static string ObterDescricao(this StatusControleNotaDevolucao situacao)
        {
            switch (situacao)
            {
                case StatusControleNotaDevolucao.AgNotaFiscal: return "Ag. Nota Fiscal";
                case StatusControleNotaDevolucao.ComNotaFiscal: return "Com Nota Fiscal";
                case StatusControleNotaDevolucao.Conferido: return "Conferido";
                case StatusControleNotaDevolucao.Rejeitado: return "Rejeitado";
                case StatusControleNotaDevolucao.ComChaveNotaFiscal: return "Com Chave Nota Fiscal";
                default: return string.Empty;
            }
        }
    }
}
