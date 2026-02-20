using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class PlanejamentoFrotaDia
    {
        #region Propriedades

        public string Codigo { get; set; }

        public string Filial { get; set; }

        public string Placa { get; set; }

        public string ModeloVeicular { get; set; }

        public decimal Capacidade { get; set; }

        public int AnoModelo { get; set; }

        public string Transportador { get; set; }

        public string Motorista { get; set; }

        public string CPF { get; set; }

        public string Telefone { get; set; }

        public string BrasilRiskMotorista { get; set; }

        public string BrasilRiskVeiculo { get; set; }

        public string TesteFrio { get; set; }

        public string ProprioOuAgregado { get; set; }

        public DateTime DisponivelEm { get; set; }

        public string MotivoIndisponibilidade { get; set; }

        public string ObservacaoTransportador { get; set; }

        public string ObservacaoMarfrig { get; set; }

        public bool Paletizado { get; set; }

        public DateTime DataUltimoEmbarque { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string PlacaFormatada { get => Placa.ObterPlacaFormatada(); }

        public string CPFFormatado { get => CPF.ObterCpfFormatado(); }

        public string TelefoneFormatado { get => Telefone.ObterTelefoneFormatado(); }

        public string ProprioOuAgregadoDescricao { get => ProprioOuAgregado == "P" ? "Próprio" : "Agregado"; }

        public string DisponivelEmFormatada { get => DisponivelEm != DateTime.MinValue ? DisponivelEm.ToString("dd/MM/yyyy") : ""; } 
        
        public string DataUltimoEmbarqueFormatada { get => DataUltimoEmbarque != DateTime.MinValue ? DataUltimoEmbarque.ToString("dd/MM/yyyy") : ""; }

        public string PaletizadoDescricao { get => Paletizado.ObterDescricao(); }

        #endregion Propriedades com Regras
    }
}
