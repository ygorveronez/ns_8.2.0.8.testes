using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public sealed class Veiculo
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Chassi { get; set; }
        public string NumeroEquipamentoRastreador { get; set; }
        public string TecnologiaRastreador { get; set; }
        public string TipoComunicacaoRastreador { get; set; }
        private string CnpjTransportador { get; set; }
        private DateTime DataAtualizacao { get; set; }
        private DateTime DataValidadeGerenciadoraRisco { get; set; }
        private DateTime DataValidadeLiberacaoSeguradora { get; set; }
        public string Frota { get; set; }
        public string LocalAtual { get; set; }
        public string ModeloVeicular { get; set; }
        public string Motorista { get; set; }
        public string Placa { get; set; }
        public string Propriedade { get; set; }
        public string Proprietario { get; set; }
        public int CapacidadeTanque { get; set; }
        public string RENAVAM { get; set; }
        public string RNTRC { get; set; }
        public string Segmento { get; set; }
        public string TipoCarroceria { get; set; }
        public string TipoRodado { get; set; }
        public string TipoVeiculo { get; set; }
        public double CpfCnpjProprietario { get; set; }
        public string Transportador { get; set; }
        public string UF { get; set; }
        public string Reboques { get; set; }
        public string FuncionarioResponsavel { get; set; }
        public string Situacao { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Cor { get; set; }
        public string GrupoPessoas { get; set; }
        public int Tara { get; set; }
        public int CapacidadeM3 { get; set; }
        public double CapacidadeKG { get; set; }
        public int KMAtual { get; set; }
        private DateTime DataAquisicao { get; set; }
        public decimal ValorAquisicao { get; set; }
        public string CentroCarregamento { get; set; }
        public string CentroResultado { get; set; }
        public int QuantidadeEixos { get; set; }
        public bool VeiculoPossuiTagValePedagio { get; set; }
        public string CpfMotoristaFormatado { get; set; }
        public string Observacao { get; set; }
        public string ModeloReboques { get; set; }
        public string MarcaReboques { get; set; }
        public string AnoModeloReboques { get; set; }
        public string ModeloCarroceria { get; set; }
        public bool Bloqueado { get; set; }
        public string MotivoBloqueio { get; set; }
        public string RGMotorista { get; set; }
        public string DataEmissaoRGMotorista { get; set; }
        public string DataNascimentoMotorista { get; set; }
        public string TelefoneMotorista { get; set; }
        public string TaraReboque { get; set; }
        public bool NaogerarIntegracaoOpentechs { get; set; }
        public string NomeTransportadorCNPJTransportador { get; set; }
        public string NomeTransportador { get; set; }
        public string CNPJTransportador { get; set; }
        private DateTime DataValidadeAdicionalCarroceria { get; set; }
        public int QuantidadePaletes { get; set; }
        public bool VeiculoAlugado { get; set; }
        public string Tracao { get; set; }
        public string TagSemParar { get; set; }
        public string OperadoraValePedagio { get; set; }
        public double LocadorCNPJ { get; set; }
        public string Locador { get; set; }

        #endregion

        #region Propriedades com Regras

        public string BloqueadoDescricao
        {
            get
            {
                return Bloqueado ? "Sim" : "Não";
            }
        }

        public string VeiculoPossuiTagValePedagioFormatada
        {
            get
            {
                return VeiculoPossuiTagValePedagio ? "Sim" : "Não";
            }
        }

        public string DataAtualizacaoFormatada
        {
            get { return DataAtualizacao != DateTime.MinValue ? DataAtualizacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataValidadeGerenciadoraRiscoFormatada
        {
            get { return DataValidadeGerenciadoraRisco != DateTime.MinValue ? DataValidadeGerenciadoraRisco.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataValidadeLiberacaoSeguradoraFormatada
        {
            get { return DataValidadeLiberacaoSeguradora != DateTime.MinValue ? DataValidadeLiberacaoSeguradora.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataAquisicaoFormatada
        {
            get { return DataAquisicao != DateTime.MinValue ? DataAquisicao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string CnpjTransportadorFormatado
        {
            get { return CnpjTransportador.ObterCnpjFormatado(); }
        }

        public string CpfCnpjProprietarioFormatado
        {
            get { return this.CpfCnpjProprietario > 0 ? this.CpfCnpjProprietario.ToString().ObterCpfOuCnpjFormatado() : ""; }
        }

        public string DataValidadeAdicionalCarroceriaFormatada
        {
            get { return DataValidadeAdicionalCarroceria != DateTime.MinValue ? DataValidadeAdicionalCarroceria.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string NaogerarIntegracaoOpentechsFormatada
        {
            get { return NaogerarIntegracaoOpentechs.ObterDescricaoAtivo(); }
        }

        public string VeiculoAlugadoDescricao
        {
            get { return VeiculoAlugado ? "Sim" : "Não"; }
        }

        public string TipoCombustivel { get; set; }
        #endregion
    }
}
