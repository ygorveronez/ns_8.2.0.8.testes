namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EscalationList
    {
        SemNivel = 0,
        Nivel1 = 1,
        Nivel2 = 2,
        Nivel3 = 3,
        Nivel4 = 4,
        Nivel5 = 5,
        Nivel6 = 6,
        Nivel7 = 7,
        Nivel8 = 8,
        Nivel9 = 9,
        Nivel10 = 10
    }

    public static class EscalationListHelper
    {
        public static string ObterDescricao(this EscalationList situacao)
        {
            switch (situacao)
            {
                case EscalationList.Nivel1: return "Nível 1";
                case EscalationList.Nivel2: return "Nível 2";
                case EscalationList.Nivel3: return "Nível 3";
                case EscalationList.Nivel4: return "Nível 4";
                case EscalationList.Nivel5: return "Nível 5";
                case EscalationList.Nivel6: return "Nível 6";
                case EscalationList.Nivel7: return "Nível 7";
                case EscalationList.Nivel8: return "Nível 8";
                case EscalationList.Nivel9: return "Nível 9";
                case EscalationList.Nivel10: return "Nível 10";
                default: return string.Empty;
            }

        }
        public static EscalationList ObterProximoNivel(this EscalationList situacao)
        {
            switch (situacao)
            {
                case EscalationList.Nivel1: return EscalationList.Nivel2;
                case EscalationList.Nivel2: return EscalationList.Nivel3;
                case EscalationList.Nivel3: return EscalationList.Nivel4;
                case EscalationList.Nivel4: return EscalationList.Nivel5;
                case EscalationList.Nivel5: return EscalationList.Nivel6;
                case EscalationList.Nivel6: return EscalationList.Nivel7;
                case EscalationList.Nivel7: return EscalationList.Nivel8;
                case EscalationList.Nivel8: return EscalationList.Nivel9;
                case EscalationList.Nivel9: return EscalationList.Nivel10;
                case EscalationList.Nivel10: return EscalationList.SemNivel;
                default: return EscalationList.SemNivel;
            }

        }

    }
}
