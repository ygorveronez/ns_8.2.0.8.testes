namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum IndicadorPagadorSinistro
    {
        SeguroProprio = 1,
        Empresa = 2,
        SeguroTerceiro = 3,
        Terceiro = 4,
        EmpresaMotorista = 5,
        EmpresaNota = 6,
        MotoristaFolha = 7,
        SeguroTerceiroReembolso = 8
    }

    public static class IndicadorPagadorSinistroHelper
    {
        public static string ObterDescricao(this IndicadorPagadorSinistro indicadorPagadorSinistro)
        {
            switch (indicadorPagadorSinistro)
            {
                case IndicadorPagadorSinistro.SeguroProprio: return "Seguro Próprio";
                case IndicadorPagadorSinistro.Empresa: return "Empresa";
                case IndicadorPagadorSinistro.SeguroTerceiro: return "Seguro Terceiro";
                case IndicadorPagadorSinistro.Terceiro: return "Terceiro";
                case IndicadorPagadorSinistro.EmpresaMotorista: return "Empresa/Motorista";
                case IndicadorPagadorSinistro.EmpresaNota: return "Empresa/Nota";
                case IndicadorPagadorSinistro.MotoristaFolha: return "Motorista/Folha";
                case IndicadorPagadorSinistro.SeguroTerceiroReembolso: return "Seguro Terceiro/Reembolso";
                default: return string.Empty;
            }
        }
    }
}
