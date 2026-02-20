using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public sealed class FiltroPesquisaPainelVeiculo
    {
        public string Placa { get; set; }
        public string NumeroFrota { get; set; }
        public string TipoVeiculo { get; set; }
        public string TipoPropriedade { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoFrota? TipoFrota { get; set; }
        public int CodigoMarcaVeiculo { get; set; }
        public int CodigoModeloVeiculo { get; set; }
        public int CodigoModeloVeicularCarga { get; set; }
        public int CodigoCentroCarregamento { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoLocalPrevisto { get; set; }
        public int CodigoTransportador { get; set; }
        public double CodigoProprietario { get; set; }
        public DateTime? DataInicioDisponivel { get; set; }
        public DateTime? DataFimDisponivel { get; set; }
        public DateTime? DataSituacao { get; set; }

        public SituacaoAtivoPesquisa Situacao { get; set; }
        public SituacaoVeiculo SituacaoVeiculo { get; set; }
        public bool HabilitarPainel { get; set; }
    }
}
