using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador
{
    public class DirecionamentoOperador
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Operador { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroEntregas { get; set; }
        public int DiasAtrazo { get; set; }
        private DateTime DataCarregamento { get; set; }
        public string Observacao { get; set; }
        public string ModeloVeiculo { get; set; }
        public string TipoCarga { get; set; }
        private string CNPJTransportador { get; set; }
        public string Transportador { get; set; }
        public decimal ValorFrete { get; set; }
        public string Destino { get; set; }
        public string Rota { get; set; }
        public virtual string Destinatario { get; set; }
        public virtual string Situacao { get; set; }
        public virtual string Veiculos { get; set; }
        private DateTime DataCriacaoCarga { get; set; }
        private DateTime DataInteresse { get; set; }
        private DateTime DataCargaContratada { get; set; }
        public string NomeMotorista { get; set; }
        private string CPFMotorista { get; set; }
        public decimal NumeroPaletes { get; set; }
        public int QuantidadeVolumes { get; set; }
        public string MotivoRejeicaoCarga { get; set; }
        private DateTime DataPrimeiroSalvamentoDadosTransporte { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CNPJTransportadorFormatado
        {
            get { return CNPJTransportador.ObterCnpjFormatado(); }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataCriacaoCargaFormatada
        {
            get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataInteresseFormatada
        {
            get { return DataInteresse != DateTime.MinValue ? DataInteresse.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataCargaContratadaFormatada
        {
            get { return DataCargaContratada != DateTime.MinValue ? DataCargaContratada.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrimeiroSalvamentoDadosTransporteFormatada
        {
            get { return DataPrimeiroSalvamentoDadosTransporte != DateTime.MinValue ? DataPrimeiroSalvamentoDadosTransporte.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CPFMotoristaFormatado
        {
            get { return CPFMotorista.ObterCpfFormatado(); }
        }

        #endregion
    }
}
