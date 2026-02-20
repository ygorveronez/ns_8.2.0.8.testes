namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAceiteContrato
    {
        Todos = 0,
        Aceito = 1,
        Pendente = 2
    }

    public static class SituacaoAceiteContratoHelper
    {
        public static string ObterDescricao(this SituacaoAceiteContrato situacao)
        {
            switch (situacao)
            {
                case SituacaoAceiteContrato.Aceito: return "Aceito";
                case SituacaoAceiteContrato.Pendente: return "Pendente";
                default: return string.Empty;
            }
        }
    }
}
