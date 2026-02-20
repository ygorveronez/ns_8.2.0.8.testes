namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAssinaturaCheckListCarga
    {
        Motorista = 1,
        Carregador = 2,
        ResponsavelAprovacao = 3
    }

    public static class TipoAssinaturaCheckListCargaHelper
    {
        public static string ObterDescricao(this TipoAssinaturaCheckListCarga tipoAssinatura)
        {
            switch (tipoAssinatura)
            {
                case TipoAssinaturaCheckListCarga.Motorista: return "Motorista";
                case TipoAssinaturaCheckListCarga.Carregador: return "Carregador";
                case TipoAssinaturaCheckListCarga.ResponsavelAprovacao: return "Responsável Aprovação";
                default: return string.Empty;
            }
        }

        public static string ObterNomeArquivo(this TipoAssinaturaCheckListCarga tipoAssinatura)
        {
            switch (tipoAssinatura)
            {
                case TipoAssinaturaCheckListCarga.Motorista: return "Assinatura Motorista.png";
                case TipoAssinaturaCheckListCarga.Carregador: return "Assinatura Carregador.png";
                case TipoAssinaturaCheckListCarga.ResponsavelAprovacao: return "Assinatura Responsável Aprovação.png";
                default: return string.Empty;
            }
        }
    }
}
