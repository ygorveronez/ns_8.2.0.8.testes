namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AplicacaoOpcaoCheckList
    {
        Sempre = 1,
        Carregamento = 2,
        Descarregamento = 3
    }

    public static class AplicacaoOpcaoCheckListHelper
    {
        public static string ObterDescricao(this AplicacaoOpcaoCheckList aplicacao)
        {
            switch (aplicacao)
            {
                case AplicacaoOpcaoCheckList.Carregamento: return "Carregamento";
                case AplicacaoOpcaoCheckList.Descarregamento: return "Descarregamento";
                case AplicacaoOpcaoCheckList.Sempre: return "Sempre";
                default: return string.Empty;
            }
        }
    }
}
