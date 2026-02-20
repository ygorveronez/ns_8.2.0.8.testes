namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumAceiteMotorista
    {
        Enviado = 0,
        Aceite = 1,
        Recusou = 2,
        Expirou = 3
    }

    public static class EnumAceiteMotoristaHelper
    {
        public static string ObterDescricao(this EnumAceiteMotorista aceite)
        {
            switch (aceite)
            {
                case EnumAceiteMotorista.Enviado: return "Escala enviada";
                case EnumAceiteMotorista.Aceite: return "Motorista confirmou escala";
                case EnumAceiteMotorista.Recusou: return "Motorista recusou escala";
                case EnumAceiteMotorista.Expirou: return "Expirou tempo confirmação escala";
                default: return string.Empty;
            }
        }
    }
}
