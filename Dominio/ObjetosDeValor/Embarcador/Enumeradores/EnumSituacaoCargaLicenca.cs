namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumSituacaoCargaLicenca
    {
        Nenhuma = 0,
        Valida = 1,
        Vencida = 2,
        Reprovada = 3,
        SemLicenca = 4
    }

    public static class EnumSituacaoCargaLicencaHelper
    {
        public static bool Isvalida(this EnumSituacaoCargaLicenca situacao)
        {
            return (situacao == EnumSituacaoCargaLicenca.Nenhuma) || (situacao == EnumSituacaoCargaLicenca.Valida);
        }

        public static string ObterDescricao(this EnumSituacaoCargaLicenca situacao)
        {
            switch (situacao)
            {
                case EnumSituacaoCargaLicenca.Valida: return "Válida";
                case EnumSituacaoCargaLicenca.Vencida: return "Vencida";
                case EnumSituacaoCargaLicenca.Reprovada: return "Reprovada";
                case EnumSituacaoCargaLicenca.SemLicenca: return "Sem Licença";
                default: return string.Empty;
            }
        }
    }
}
