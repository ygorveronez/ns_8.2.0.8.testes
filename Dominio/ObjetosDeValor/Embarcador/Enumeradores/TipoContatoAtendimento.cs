namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoContatoAtendimento
    {
        Skype = 1,
        Telefone = 2,
        Email = 3,
        Celular = 4,
        ChatWeb = 6,
        Outros = 5
    }

    public static class TipoContatoAtendimentoHelper
    {
        public static string ObterDescricao(this TipoContatoAtendimento tipoContato)
        {
            switch (tipoContato)
            {
                case TipoContatoAtendimento.Celular: return "Celular";
                case TipoContatoAtendimento.ChatWeb: return "Chat Web";
                case TipoContatoAtendimento.Email: return "Email";
                case TipoContatoAtendimento.Outros: return "Outros";
                case TipoContatoAtendimento.Skype: return "Skype";
                case TipoContatoAtendimento.Telefone: return "Telefone";
                default: return string.Empty;
            }
        }
    }
}
