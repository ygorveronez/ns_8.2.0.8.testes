namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoParentesco
    {
        Nenhum = 0,
        Outro = 1,
        Pai = 2,
        Mae = 3,
        Filhos = 4,
        Irmao = 5,
        Avo = 6,
        Neto = 7,
        Tio = 8,
        Sobrinho = 9,
        Bisavo = 10,
        Bisneto = 11,
        Primo = 12,
        Trisavo = 13,
        Trineto = 14,
        TioAvo = 15,
        SobrinhoNeto = 16,
        Esposo = 17
    }

    public static class TipoParentescoHelper
    {
        public static string ObterDescricao(this TipoParentesco tipo)
        {
            switch (tipo)
            {
                case TipoParentesco.Nenhum: return "Nenhum";
                case TipoParentesco.Outro: return "Outro";
                case TipoParentesco.Pai: return "Pai";
                case TipoParentesco.Mae: return "Mãe";
                case TipoParentesco.Filhos: return "Filho";
                case TipoParentesco.Irmao: return "Irmão";
                case TipoParentesco.Avo: return "Avó";
                case TipoParentesco.Neto: return "Neto";
                case TipoParentesco.Tio: return "Tio";
                case TipoParentesco.Sobrinho: return "Sobrinho";
                case TipoParentesco.Bisavo: return "Bisavo";
                case TipoParentesco.Bisneto: return "Bisneto";
                case TipoParentesco.Primo: return "Primo";
                case TipoParentesco.Trisavo: return "Trisavo";
                case TipoParentesco.Trineto: return "Trineto";
                case TipoParentesco.TioAvo: return "Tio-avó";
                case TipoParentesco.SobrinhoNeto: return "Sobrino-Neto";
                case TipoParentesco.Esposo: return "Esposo";
                default: return string.Empty;
            }
        }
    }
}
