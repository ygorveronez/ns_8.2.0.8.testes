namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GeoServiceGeocoding
    {
        Google = 0,
        Nominatim = 1
    }

    public enum GeoLocalizacaoStatus
    {
        NaoGerado = 0,
        Gerado = 1,
        Erro = 2,
        GeradoNominatim = 3
    }

    public enum GeoLocalizacaoTipo
    {
        Endereco = 0,
        Localidade = 1,
        Todos = -1
    }

    public enum GeoLocalizacaoRaioLocalidade
    {
        NaoValidado = 0,
        OK = 1,
        ForaRaio = 2
    }
}
