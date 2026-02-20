using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaFilaCarregamentoVeiculoHistorico
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoGrupoModeloVeicularCarga { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoMotivoRetivadaFilaCarregamento { get; set; }

        public int CodigoMotorista { get; set; }

        public int CodigoVeiculo { get; set; }

        public System.DateTime? DataInicio { get; set; }

        public System.DateTime? DataLimite { get; set; }

        public TipoFilaCarregamentoVeiculoHistorico? Tipo { get; set; }
    }
}
