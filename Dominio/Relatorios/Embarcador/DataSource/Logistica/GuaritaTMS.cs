namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class GuaritaTMS
    {
        public string Veiculo { get; set; }
        public string Carga { get; set; }
        public int OrdemServico { get; set; }
        public string Motorista { get; set; }
        public string Operador { get; set; }
        public int KMLancamento { get; set; }
        public string DataPassagem { get; set; }
        public string EntradaSaida { get; set; }
        public bool FinalizouViagem { get; set; }
        public bool RetornouComReboque { get; set; }
        public bool VeiculoVazio { get; set; }
        public string Observacao { get; set; }
        public string NumeroFrota { get; set; }
        public string StatusVeiculoOS { get; set; }
        public string Reboques { get; set; }
        public string NumeroFrotaReboques { get; set; }
        public string Empresa { get; set; }
        public string SegmentoTracao { get; set; }
        public string SegmentoReboque { get; set; }
        public string FinalizouViagemFormatado
        {
            get
            {
                switch (this.FinalizouViagem)
                {
                    case true:
                        return "Sim";
                    case false:
                        return "Não";
                    default:
                        return "Não Informado";
                }
            }
        }

        public string RetornouComReboqueFormatado
        {
            get
            {
                switch (this.RetornouComReboque)
                {
                    case true:
                        return "Sim";
                    case false:
                        return "Não";
                    default:
                        return "Não Informado";
                }
            }
        }

        public string VeiculoVazioFormatado
        {
            get
            {
                switch (this.VeiculoVazio)
                {
                    case true:
                        return "Sim";
                    case false:
                        return "Não";
                    default:
                        return "Não Informado";
                }
            }
        }
    }
}
