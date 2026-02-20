namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CategoriaOpcaoCheckList
    {
        Tracao = 1,
        Reboque = 2,
        Motorista = 3,
        Manutencao = 4,
        Outro = 5
    }

    public static class CategoriaOpcaoCheckListHelper
    {
        public static string ObterDescricao(this CategoriaOpcaoCheckList categoria)
        {
            switch (categoria)
            {
                case CategoriaOpcaoCheckList.Manutencao: return "Manutenção";
                case CategoriaOpcaoCheckList.Motorista: return "Motorista";
                case CategoriaOpcaoCheckList.Reboque: return "Reboque";
                case CategoriaOpcaoCheckList.Tracao: return "Tração";
                case CategoriaOpcaoCheckList.Outro: return "Outro";
                default: return string.Empty;
            }
        }
    }
}
