namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOpcaoCheckListRFI
    {
        SimNao = 1,
        Opcoes = 2,        
    }

    public static class TipoOpcaoCheckListHelperRFI
    {
        public static bool IsPossuiAlternativas(this TipoOpcaoCheckListRFI tipo)
        {
            return tipo == TipoOpcaoCheckListRFI.Opcoes;
        }

        public static string ObterDescricao(this TipoOpcaoCheckListRFI tipoOpcaoCheckList)
        {
            switch (tipoOpcaoCheckList)
            {
                case TipoOpcaoCheckListRFI.SimNao: return "Sim e Não";
                case TipoOpcaoCheckListRFI.Opcoes: return "Opções (Múltiplo)";               
                default: return string.Empty;
            }
        }

        public static int ObterCodigo(this TipoOpcaoCheckListRFI tipoOpcaoCheckList)
        {
            switch (tipoOpcaoCheckList)
            {
                case TipoOpcaoCheckListRFI.SimNao: return 1;
                case TipoOpcaoCheckListRFI.Opcoes: return 2;
                    
                default: return 0;
            }
        }
    }
}
