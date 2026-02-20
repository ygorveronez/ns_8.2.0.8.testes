namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusLicenca
    {
        Vigente = 1,
        Vencido = 2,
        Aprovado = 3,
        Reprovado = 4
    }

    public static class StatusLicencaHelper
    {
        public static string ObterDescricao(this StatusLicenca statusLicenca)
        {
            switch (statusLicenca)
            {
                case StatusLicenca.Vigente: return Localization.Resources.Enumeradores.StatusLicenca.Vigente;
                case StatusLicenca.Vencido: return Localization.Resources.Enumeradores.StatusLicenca.Vencido;
                case StatusLicenca.Aprovado: return Localization.Resources.Enumeradores.StatusLicenca.Aprovado;
                case StatusLicenca.Reprovado: return Localization.Resources.Enumeradores.StatusLicenca.Reprovado;
                default: return string.Empty;
            }
        }

        public static StatusLicenca ObterStatusLicenca(string statusLicenca, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (string.IsNullOrWhiteSpace(statusLicenca))
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return StatusLicenca.Aprovado;
                else
                    return StatusLicenca.Vigente;
            }

            switch (statusLicenca)
            {
                case "Vigente": return StatusLicenca.Vigente;
                case "Vencido": return StatusLicenca.Vencido;
                case "Aprovado": return StatusLicenca.Aprovado;
                case "Reprovado": return StatusLicenca.Reprovado;
                default: return tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? StatusLicenca.Aprovado : StatusLicenca.Vigente;
            }
        }
    }
}