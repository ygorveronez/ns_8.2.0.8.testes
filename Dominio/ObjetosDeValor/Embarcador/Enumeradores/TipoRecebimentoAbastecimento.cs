namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRecebimentoAbastecimento
    {
        Sistema = 0,
        CTF = 1,
        WSPosto = 2,
        ImportacaoXML = 3,
        Integracao = 4, 
        Interno = 5
    }

    public static class TipoRecebimentoAbastecimentoHelper
    {
        public static string ObterDescricao(this TipoRecebimentoAbastecimento tipoRecebimentoAbastecimento)
        {
            switch (tipoRecebimentoAbastecimento)
            {
                case TipoRecebimentoAbastecimento.Sistema: return "Sistema";
                case TipoRecebimentoAbastecimento.CTF: return "CTF";
                case TipoRecebimentoAbastecimento.WSPosto: return "WS - Posto";
                case TipoRecebimentoAbastecimento.ImportacaoXML: return "Importação XML";
                case TipoRecebimentoAbastecimento.Integracao: return "Integração";
                case TipoRecebimentoAbastecimento.Interno: return "Interno";
                default: return "Sistema";
            }
        }
    }
}
