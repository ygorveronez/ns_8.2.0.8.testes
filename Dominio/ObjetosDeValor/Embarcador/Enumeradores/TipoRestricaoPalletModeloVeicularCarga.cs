namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRestricaoPalletModeloVeicularCarga
    {
        BloquearSomenteNumeroSuperior = 0,
        BloquearSomenteNumeroInferior = 1,
        NaoBloquear = 2
    }

    public static class TipoRestricaoPalletModeloVeicularCargaHelper
    {
        public static string ObterDescricao(this TipoRestricaoPalletModeloVeicularCarga tipo)
        {
            switch (tipo)
            {
                case TipoRestricaoPalletModeloVeicularCarga.BloquearSomenteNumeroInferior: return "Bloquear Somente Número de Pallets Inferior";
                case TipoRestricaoPalletModeloVeicularCarga.BloquearSomenteNumeroSuperior: return "Bloquear Somente Número de Pallets Superior";
                case TipoRestricaoPalletModeloVeicularCarga.NaoBloquear: return "Não Bloquear";
                default: return string.Empty;
            }
        }
    }
}
