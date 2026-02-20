namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class CheckListMinutaTransporte
    {
        #region Cabeçalho

        public string Empresa { get; set; }

        public string Endereco { get; set; }

        public string Telefone { get; set; }

        public string DataEmbarque { get; set; }

        public string DataPrevistaAbate { get; set; }

        public string NumeroMinutaEmbarque { get; set; }

        #endregion Cabeçalho

        #region Dados de Transporte

        public string Motorista { get; set; }

        public string CPFCNPJMotorista { get; set; }

        public string Placa { get; set; }

        public string Transportador { get; set; }

        public string TipoVeiculo { get; set; }

        public string AnimaisEmbarcados { get; set; }

        #endregion Dados de Transporte

        #region Dados do Proprietário

        public string Pecuarista { get; set; }

        public string Fazenda { get; set; }

        public string Cidade { get; set; }

        public string UF { get; set; }

        public string Roteiro { get; set; }

        public string CPFCNPJProprietario { get; set; }

        public string InscricaoEstadual { get; set; }

        #endregion Dados do Proprietário
    }
}
