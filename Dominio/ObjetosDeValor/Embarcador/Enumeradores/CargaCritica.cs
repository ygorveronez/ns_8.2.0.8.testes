
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CargaCritica
    {
        Nao = 0,
        Sim = 1
    }

    public static class CaragaCriticaHelper
    {
        public static string ObterCargaCriticaFormatado(this CargaCritica status)
        {
            switch (status)
            {
                case CargaCritica.Nao: return "Não";
                case CargaCritica.Sim: return "Sim";

                default: return string.Empty;
            }
        }
    }
}
