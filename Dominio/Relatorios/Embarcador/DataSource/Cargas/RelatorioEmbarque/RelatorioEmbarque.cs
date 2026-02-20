using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.RelatorioEmbarque
{
    public class RelatorioEmbarque
    {
        #region Propriedades

        public string NomeEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string ANTTEmpresa { get; set; }

        public string NomeRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string BairroRemetente { get; set; }
        public string CEPRemetente { get; set; }
        public double CNPJRemetente { get; set; }
        public string IERemetente { get; set; }
        public string CidadeRemetente { get; set; }
        public string EstadoRemetente { get; set; }

        public string Motorista { get; set; }
        public string RGMotorista { get; set; }
        public string CPFMotorista { get; set; }

        public string NumeroCarga { get; set; }
        public string OperadorCarga { get; set; }
        private DateTime DataCriacaoCarga { get; set; }
        private DateTime DataFinalizacaoEmissao { get; set; }
        public string ZonaTransporte { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string CodigoIntegracaoDestinatario { get; set; }
        public string CidadeDestinatario { get; set; }
        public string EstadoDestinatario { get; set; }
        public int NumeroNota { get; set; }
        public int NumeroCTe { get; set; }
        public decimal ValorNota { get; set; }
        public decimal Peso { get; set; }
        public decimal Cubagem { get; set; }
        public int Volumes { get; set; }
        public string Lacre { get; set; }
        public string ObservacaoRelatorioDeEmbarque { get; set; }
        public string Veiculo { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataCriacaoCargaFormatada
        {
            get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataFinalizacaoEmissaoFormatada
        {
            get { return DataFinalizacaoEmissao != DateTime.MinValue ? DataFinalizacaoEmissao.ToString("dd/MM/yyyy HH:mm:ss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        #endregion
    }
}
