namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemMercadoria
    {
        Origem0 = 0,
        Origem1 = 1,
        Origem2 = 2,
        Origem3 = 3,
        Origem4 = 4,
        Origem5 = 5,
        Origem6 = 6,
        Origem7 = 7,
        Origem8 = 8
    }

    public static class OrigemMercadoriaHelper
    {
        public static string ObterDescricao(this OrigemMercadoria origemMercadoria)
        {
            switch (origemMercadoria)
            {
                case OrigemMercadoria.Origem0: return "0 - Nacional, exceto as indicadas nos códigos 3, 4, 5 e 8";
                case OrigemMercadoria.Origem1: return "1 - Estrangeira - Importação direta, exceto a indicada no código 6";
                case OrigemMercadoria.Origem2: return "2 - Estrangeira - Adquirida no mercado interno, exceto a indicada no código 7";
                case OrigemMercadoria.Origem3: return "3 - Nacional, mercadoria ou bem com Conteúdo de Importação superior a 40% e inferior ou igual a 70%";
                case OrigemMercadoria.Origem4: return "4 - Nacional, cuja produção tenha sido feita em conformidade com os processos produtivos básicos de que tratam as legislações citadas nos Ajustes";
                case OrigemMercadoria.Origem5: return "5 - Nacional, mercadoria ou bem com Conteúdo de Importação inferior ou igual a 40%";
                case OrigemMercadoria.Origem6: return "6 - Estrangeira - Importação direta, sem similar nacional, constante em lista da CAMEX e gás natural";
                case OrigemMercadoria.Origem7: return "7 - Estrangeira - Adquirida no mercado interno, sem similar nacional, constante lista CAMEX e gás natural";
                case OrigemMercadoria.Origem8: return "8 - Nacional, mercadoria ou bem com Conteúdo de Importação superior a 70%";
                default: return string.Empty;
            }
        }
    }
}
