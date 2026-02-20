namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoArea
    {
        Raio = 1,
        Poligono = 2,
        Ponto = 3
    }

    public static class TipoAreaHelper
    {
        public static string ObterDescricao(this TipoArea tipoArea)
        {
            switch (tipoArea)
            {
                case TipoArea.Raio: return "Raio";
                case TipoArea.Poligono: return "Polígono";
                case TipoArea.Ponto: return "Ponto";
                default: return string.Empty;
            }
        }
    }
}