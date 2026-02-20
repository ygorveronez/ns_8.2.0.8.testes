using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.ModeloVeicularCarga
{
    public class ModeloVeicularCarga
    {
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public string CodigoIntegracaoGerenciadoraRisco { get; set; }
        public int NumeroEixos { get; set; }
        public PadraoEixosVeiculo PadraoEixos { get; set; }
        public int NumeroEixosSuspensos { get; set; }
        public TipoModeloVeicularCarga Tipo { get; set; }
        public int NumeroReboques { get; set; }
        public int DiasRealizarProximoChecklist { get; set; }
        public bool Ativo { get; set; }
        public string GrupoModeloVeicular { get; set; }
        public decimal FatorEmissaoCO2 { get; set; }
        public string CodigoTipoCargaANTT { get; set; }
        public int VelocidadeMedia { get; set; }
        public UnidadeCapacidade UnidadeCapacidade { get; set; }
        public decimal CapacidadePesoTransporte { get; set; }
        public decimal ToleranciaPesoMenor { get; set; }
        public decimal ToleranciaPesoExtra { get; set; }
        public int ToleranciaMinimaPaletes { get; set; }
        public decimal OcupacaoCubicaPaletes { get; set; }
        public string CodigosIntegracao { get; set; }
        
        public string AtivoDescricao
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public string PadraoEixosFormatado { get { return PadraoEixos.ObterDescricao() ?? ""; } }

        public string TipoFormatado { get { return Tipo.ObterDescricao() ?? ""; } }

        public string UnidadeCapacidadeFormatado { get { return UnidadeCapacidade.ToString(); } }
    }
}
