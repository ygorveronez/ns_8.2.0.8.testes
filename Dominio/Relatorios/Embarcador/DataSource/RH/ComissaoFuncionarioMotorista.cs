namespace Dominio.Relatorios.Embarcador.DataSource.RH
{
    public class ComissaoFuncionarioMotorista
    {
        public string Funcionario { get; set; }
        public string CPF { get; set; }
        public string CodigoIntegracao { get; set; }
        public decimal PercentualBaseCalculo { get; set; }
        public decimal PercentualComissao { get; set; }
        public decimal ValorDiaria { get; set; }
        public int DiasEmViagem { get; set; }
        public string DataEmissao { get; set; }
        public string Modelo { get; set; }
        public string Numero { get; set; }
        public string Frota { get; set; }
        public string Remetente { get; set; }
        public string UFOrigem { get; set; }
        public string Destinatario { get; set; }
        public string UFDestino { get; set; }
        public decimal TotalFrete { get; set; }
        public decimal ICMS { get; set; }
        public decimal Pedagio { get; set; }
        public decimal Outros { get; set; }
        public decimal ValorFreteLiquido { get; set; }
        public decimal BaseCalculo { get; set; }
        public decimal ValorComissao { get; set; }
        public decimal ValorProdutividade { get; set; }
        public decimal ValorFreteLiquidoComMeta { get; set; }


        public string AtingiuMedia { get; set; }
        public string NaoHouveSinitro { get; set; }
        public string NaoHouveAdvertencia { get; set; }
        public string ContemDeslocamentoVazio { get; set; }
        public decimal FaturamentoMinimo { get; set; }
        public string CargoMotorista { get; set; }
        public decimal ValorBonificacao { get; set; }
        public decimal ValorBaseComissao { get; set; }
        public decimal PercentualMedia { get; set; }
        public decimal PercentualSinistro { get; set; }
        public decimal PercentualAdvertencia { get; set; }
        public decimal KMFinal { get; set; }
        public decimal KMInicial { get; set; }
        public decimal KMTotal { get; set; }
        public decimal LitrosTotalAbastecimento { get; set; }
        public decimal MediaFinal { get; set; }
        public decimal MediaIdeal { get; set; }
        public decimal PercentualComissaoDocumento { get; set; }
        public decimal ValorComissaoDocumento { get; set; }

        public int CodigoComissaoFuncionarioMotorista { get; set; }

        public decimal PesoCarga { get; set; }
        public decimal MediaIdealCarga { get; set; }

        public string CentroResultadoDescricao { get; set; }
    }
}
