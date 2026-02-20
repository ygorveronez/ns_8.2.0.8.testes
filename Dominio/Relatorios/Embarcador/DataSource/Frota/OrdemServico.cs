namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class OrdemServico
    {
        public int NumeroOS { get; set; }
        public string DataProgramada { get; set; }
        public string LocalManutencao { get; set; }
        public string Observacao { get; set; }
        public string NumeroFrota { get; set; }
        public string Motorista { get; set; }
        public string TelefoneMotorista { get; set; }
        public string Veiculo { get; set; }
        public string ModeloVeiculo { get; set; }
        public string MarcaVeiculo { get; set; }
        public string Quilometragem { get; set; }
        public string Equipamento { get; set; }
        public string ModeloEquipamento { get; set; }
        public string MarcaEquipamento { get; set; }
        public string Horimetro { get; set; }
        public string DataFechamento { get; set; }
        public string Tipo { get; set; }
        public string Operador { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJ { get; set; }
        public string Endereco { get; set; }
        public string Localidade { get; set; }
        public string CNPJMatriz { get; set; }
        public byte[] CodigoBarrasImagem { get; set; }

        //Dados Serviço
        public int CodigoServico { get; set; }
        public string Servico { get; set; }
        public string TipoServico { get; set; }
        public string ObservacaoServico { get; set; }
        public string UltimaExecucaoServico { get; set; }
        public string TempoEstimado { get; set; }
        public decimal CustoMedio { get; set; }
        public decimal CustoEstimado { get; set; }

    }
}
