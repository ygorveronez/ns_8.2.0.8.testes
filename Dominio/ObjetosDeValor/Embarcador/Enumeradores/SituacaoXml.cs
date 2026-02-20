namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoXml
    {
        Todos = 0,
        Importado = 1,
        ProblemaImportacao = 2,
        Sincronizando = 3
    }

    public static class SituacaoXmlHelper
    {
        public static string ObterDescricao(this SituacaoXml situacao)
        {
            switch (situacao)
            {
                case SituacaoXml.Todos: return "Todos";
                case SituacaoXml.Importado: return "Importado";
                case SituacaoXml.ProblemaImportacao: return "Problema na Importação";
                case SituacaoXml.Sincronizando: return "Sincronizando";
                default: return string.Empty;
            }
        }
    }
}
