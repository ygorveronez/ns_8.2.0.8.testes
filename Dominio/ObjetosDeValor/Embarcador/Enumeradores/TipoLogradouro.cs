namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLogradouro
    {
        Rua = 1,
        Avenida = 2,
        Rodovia = 3,
        Estrada = 4,
        Praca = 5,
        Travessa = 6,
        Outros = 99
    }

    public static class TipoLogradouroHelper
    {
        public static string ObterDescricao(this TipoLogradouro tipoLogradouro)
        {
            switch (tipoLogradouro)
            {
                case TipoLogradouro.Rua: return Localization.Resources.Enumeradores.TipoLogradouro.Rua;
                case TipoLogradouro.Avenida: return Localization.Resources.Enumeradores.TipoLogradouro.Avenida;
                case TipoLogradouro.Rodovia: return Localization.Resources.Enumeradores.TipoLogradouro.Rodovia;
                case TipoLogradouro.Estrada: return Localization.Resources.Enumeradores.TipoLogradouro.Estrada;
                case TipoLogradouro.Praca: return Localization.Resources.Enumeradores.TipoLogradouro.Praca;
                case TipoLogradouro.Travessa: return Localization.Resources.Enumeradores.TipoLogradouro.Travessa;
                case TipoLogradouro.Outros: return Localization.Resources.Enumeradores.TipoLogradouro.Outros;
                default: return string.Empty;
            }
        }
    }
}
