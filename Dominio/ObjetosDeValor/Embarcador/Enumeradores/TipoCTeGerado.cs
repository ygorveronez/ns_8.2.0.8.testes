namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCTeGerado
    {
        Anulacao = 1,
        Complemento = 2,
        Substituicao = 3,
        Duplicado = 4,
        CopiaDesvinculadoCarga = 5
    }

    public static class TipoCTeGeradoHelper
    {
        public static string ObterDescricao(this TipoCTeGerado tipoCTeGerado)
        {
            switch (tipoCTeGerado)
            {
                case TipoCTeGerado.Anulacao: return "Anulação";
                case TipoCTeGerado.Complemento: return "Complemento";
                case TipoCTeGerado.Substituicao: return "Substitução";
                case TipoCTeGerado.Duplicado: return "Duplicado";
                case TipoCTeGerado.CopiaDesvinculadoCarga: return "Cópia Desvinculado Carga";
                default: return string.Empty;
            }
        }

    }
}