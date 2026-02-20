namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOpcaoCheckList
    {
        Aprovacao = 0,
        SimNao = 1,
        Opcoes = 2,
        Informativo = 3,
        Selecoes = 4,
        Escala = 5
    }

    public static class TipoOpcaoCheckListHelper
    {
        public static bool IsPossuiAlternativas(this TipoOpcaoCheckList tipo)
        {
            return (tipo == TipoOpcaoCheckList.Opcoes) || (tipo == TipoOpcaoCheckList.Selecoes);
        }

        public static string ObterDescricao(this TipoOpcaoCheckList tipoOpcaoCheckList)
        {
            switch (tipoOpcaoCheckList)
            {
                case TipoOpcaoCheckList.Aprovacao: return "Aprovação";
                case TipoOpcaoCheckList.SimNao: return "Sim e Não";
                case TipoOpcaoCheckList.Opcoes: return "Opções (Múltiplo)";
                case TipoOpcaoCheckList.Informativo: return "Informativo";
                case TipoOpcaoCheckList.Selecoes: return "Seleções (Único)";
                case TipoOpcaoCheckList.Escala: return "Escala";
                default: return string.Empty;
            }
        }

        public static int ObterCodigo(this TipoOpcaoCheckList tipoOpcaoCheckList)
        {
            switch (tipoOpcaoCheckList)
            {
                case TipoOpcaoCheckList.Aprovacao: return 0;
                case TipoOpcaoCheckList.SimNao: return 1;
                case TipoOpcaoCheckList.Opcoes: return 2;
                case TipoOpcaoCheckList.Informativo: return 3;
                case TipoOpcaoCheckList.Selecoes: return 4;
                case TipoOpcaoCheckList.Escala: return 5;
                default: return -1;
            }
        }
    }
}
