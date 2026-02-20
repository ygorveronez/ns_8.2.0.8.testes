namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum Mes
    {
        Janeiro = 1,
        Fevereiro = 2,
        Marco = 3,
        Abril = 4,
        Maio = 5,
        Junho = 6,
        Julho = 7,
        Agosto = 8,
        Setembro = 9,
        Outubro = 10,
        Novembro = 11,
        Dezembro = 12
    }

    public static class MesHelper
    {
        public static string ObterDescricao(this Mes mes)
        {
            switch (mes)
            {
                case Mes.Janeiro: return "Janeiro";
                case Mes.Fevereiro: return "Fevereiro";
                case Mes.Marco: return "Março";
                case Mes.Abril: return "Abril";
                case Mes.Maio: return "Maio";
                case Mes.Junho: return "Junho";
                case Mes.Julho: return "Julho";
                case Mes.Agosto: return "Agosto";
                case Mes.Setembro: return "Setembro";
                case Mes.Outubro: return "Outubro";
                case Mes.Novembro: return "Novembro";
                case Mes.Dezembro: return "Dezembro";
                default: return string.Empty;
            }
        }
    }
}
