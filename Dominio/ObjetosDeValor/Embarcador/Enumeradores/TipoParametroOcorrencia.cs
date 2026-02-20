namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoParametroOcorrencia
    {
        Periodo = 1,
        Booleano = 2,
        Inteiro = 3,
        Texto = 4,
        Data = 5
    }

    public static class TipoParametroOcorrenciaHelper
    {
        public static string ObterDescricao(this TipoParametroOcorrencia tipo)
        {
            switch (tipo)
            {
                case TipoParametroOcorrencia.Booleano: return "Booleano";
                case TipoParametroOcorrencia.Data: return "Data";
                case TipoParametroOcorrencia.Inteiro: return "Inteiro";
                case TipoParametroOcorrencia.Periodo: return "Período";
                case TipoParametroOcorrencia.Texto: return "Texto";
                default: return "Não Definido";
            }
        }
    }
}
