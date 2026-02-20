namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoOfertaCarga
    {
        Ofertar = 0,
        Atualizar = 1,
        Inativar = 2,
        Cancelar = 3,
        Ativar = 4,
        Completar = 5,
        NaoDefinido = 6,
    }

    public static class TipoIntegracaoOfertaCargaHelper
    {
        public static string ObterDescricao(this TipoIntegracaoOfertaCarga tipo)
        {
            switch (tipo)
            {
                case TipoIntegracaoOfertaCarga.Ofertar: return "Ofertar";
                case TipoIntegracaoOfertaCarga.Atualizar: return "Atualizar";
                case TipoIntegracaoOfertaCarga.Inativar: return "Inativar";
                case TipoIntegracaoOfertaCarga.Cancelar: return "Cancelar";
                case TipoIntegracaoOfertaCarga.Ativar: return "Ativar";
                case TipoIntegracaoOfertaCarga.Completar: return "Completar";
                case TipoIntegracaoOfertaCarga.NaoDefinido: return "Não Definido";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoStatusTrizy(this TipoIntegracaoOfertaCarga tipo)
        {
            switch (tipo)
            {
                case TipoIntegracaoOfertaCarga.Ofertar: return null;
                case TipoIntegracaoOfertaCarga.Inativar: return "INACTIVE";
                case TipoIntegracaoOfertaCarga.Cancelar: return "COMPLETED";
                case TipoIntegracaoOfertaCarga.Completar: return "COMPLETED";
                case TipoIntegracaoOfertaCarga.Atualizar:
                case TipoIntegracaoOfertaCarga.Ativar:
                    return "ACTIVE";
                default: return string.Empty;
            }
        }
    }
}
