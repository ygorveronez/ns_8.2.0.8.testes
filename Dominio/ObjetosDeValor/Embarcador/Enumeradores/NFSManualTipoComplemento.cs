namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum NFSManualTipoComplemento
    {
        Todos = 0,
        ApenasDocumentosOriginais = 1,
        ApenasComplementosOcorrencia = 2
    }

    public static class NFSManualTipoComplementoHelper
    {
        public static string ObterDescricao(this NFSManualTipoComplemento modo)
        {
            switch (modo)
            {
                case NFSManualTipoComplemento.ApenasDocumentosOriginais: return "Apenas documentos originais";
                case NFSManualTipoComplemento.ApenasComplementosOcorrencia: return "Apenas complementos de ocorrência";
                default: return "";
            }
        }
    }
}
