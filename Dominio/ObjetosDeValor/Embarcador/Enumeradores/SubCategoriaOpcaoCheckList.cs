namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SubCategoriaOpcaoCheckList
    {
        NaoDefinido = 0,
        Reboque = 1,
        SegundoReboque = 2,
    }

    public static class SubCategoriaOpcaoCheckListHelper
    {
        public static string ObterDescricao(this SubCategoriaOpcaoCheckList categoria)
        {
            switch (categoria)
            {
                case SubCategoriaOpcaoCheckList.NaoDefinido: return "Não definido";
                case SubCategoriaOpcaoCheckList.Reboque: return "Reboque";
                case SubCategoriaOpcaoCheckList.SegundoReboque: return "Segundo Reboque";
        
                default: return string.Empty;
            }
        }
    }
}
