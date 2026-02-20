namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOcorrenciaPatio
    {
        NaoInformado = 0,
        Checklist = 1,
        Higienizacao = 2
    }

    public static class TipoOcorrenciaPatioHelper
    {
        public static string ObterDescricao(this TipoOcorrenciaPatio tipo)
        {
            switch (tipo)
            {
                case TipoOcorrenciaPatio.Checklist: return "Checklist";
                case TipoOcorrenciaPatio.Higienizacao: return "Higienização";
                case TipoOcorrenciaPatio.NaoInformado: return "Não Informado";
                default: return string.Empty;
            }
        }
    }
}
