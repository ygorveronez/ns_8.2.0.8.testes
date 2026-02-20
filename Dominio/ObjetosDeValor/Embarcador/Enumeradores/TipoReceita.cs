namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoReceita
    {
        Feeder = 1,
        NoShow = 2,
        Cabotage = 3,
        CaptainPeter = 4,
        ExtraCost = 5,
        Detention = 6,
        ContainerDevolution = 7,
        TaxChargeCTeSubstitution = 8,
        OwnFleet = 9,
        Infraction = 10,
        TakeOrPay = 11,
        ContainerCleaning = 12,
        FeederDebitNote = 13,
        OperationalAndOthers = 14,
        ContainerRevenue = 15,
        ContainerRevenueRepair = 16,
        Reimbursement = 17,
        VAS = 18,
        Demurrage = 19,
        ResidualValue = 20
    }


    public static class TipoReceitaHelper
    {
        public static string ObterDescricao(this TipoReceita? tipo)
        {
            switch (tipo)
            {
                case TipoReceita.Feeder: return "Feeder";
                case TipoReceita.NoShow: return "No show";
                case TipoReceita.Cabotage: return "Cabotage";
                case TipoReceita.CaptainPeter: return "Captain Peter";
                case TipoReceita.ExtraCost: return "Extra Cost";
                case TipoReceita.Detention: return "Detention";
                case TipoReceita.ContainerDevolution: return "Container Devolution";
                case TipoReceita.TaxChargeCTeSubstitution: return "Tax Charge for CT-e Substitution";
                case TipoReceita.OwnFleet: return "Own Fleet";
                case TipoReceita.Infraction: return "Infraction";
                case TipoReceita.TakeOrPay: return "Take or Pay";
                case TipoReceita.ContainerCleaning: return "Container Cleaning";
                case TipoReceita.FeederDebitNote: return "Feeder Debit Note";
                case TipoReceita.OperationalAndOthers: return "Operational and Others";
                case TipoReceita.ContainerRevenue: return "Container Revenue";
                case TipoReceita.ContainerRevenueRepair: return "Container Revenue Repair";
                case TipoReceita.Reimbursement: return "Reimbursement";
                case TipoReceita.VAS: return "VAS";
                case TipoReceita.Demurrage: return "Demurrage";
                case TipoReceita.ResidualValue: return "Residual Value";
                default: return string.Empty;
            }
        }
    }

}
