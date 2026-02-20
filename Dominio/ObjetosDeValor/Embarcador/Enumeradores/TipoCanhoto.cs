namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCanhoto
    {
        Todos = 0,
        NFe = 1,
        Avulso = 2,
        CTeSubcontratacao = 3,
        CTe = 4
    }


    public static class TipoCanhotoHelper
    {
        public static string ObterDescricao(this TipoCanhoto tipoCanhoto)
        {
            switch (tipoCanhoto)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe:
                    return "NF-e";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso:
                    return "Avulso";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao:
                    return "CT-e Subcontratação";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe:
                    return "CT-e";
                default:
                    return "";
            }
        }
    }
}
