namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AcondicionamentoCarga
    {
        NaoDefinido = 0,
        Paletizado = 1,
        Estivado = 2,
        Pendurado = 3,
        Granel = 4,
        Outros = 5
    }

    public static class AcondicionamentoCargaHelper
    {
        public static string ObterDescricao(this AcondicionamentoCarga acondicionamentoCarga)
        {
            switch (acondicionamentoCarga)
            {
                case AcondicionamentoCarga.Estivado: return "Estivado";
                case AcondicionamentoCarga.Granel: return "Granel";
                case AcondicionamentoCarga.NaoDefinido: return "Não Definido";
                case AcondicionamentoCarga.Outros: return "Outros";
                case AcondicionamentoCarga.Paletizado: return "Paletizado";
                case AcondicionamentoCarga.Pendurado: return "Pendurado";
                default: return string.Empty;
            }
        }
    }
}
