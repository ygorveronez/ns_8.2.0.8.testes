namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ChamadoAosCuidadosDo
    {
        Embarcador = 1,
        Transporador = 2
    }

    public static class ChamadoAosCuidadosDoHelper
    {
        public static string ObterDescricao(this ChamadoAosCuidadosDo chamadoAosCuidadosDo)
        {
            switch (chamadoAosCuidadosDo)
            {
                case ChamadoAosCuidadosDo.Embarcador: return "Embarcador";
                case ChamadoAosCuidadosDo.Transporador: return "Transportador";
                default: return string.Empty;
            }
        }
    }
}
