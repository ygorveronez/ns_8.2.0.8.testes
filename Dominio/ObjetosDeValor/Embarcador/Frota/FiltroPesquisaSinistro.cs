using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaSinistro
    {
        public DateTime DataSinistroInicial { get; set; }
        public DateTime DataSinistroFinal { get; set; }
        public int Numero { get; set; }
        public string NumeroBoletimOcorrencia { get; set; }
        public int CodigoCidade { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoVeiculoReboque { get; set; }
        public int CodigoMotorista { get; set; }
        public SituacaoEtapaFluxoSinistro? Situacao { get; set; }
        public EtapaSinistro? Etapa { get; set; }
        public int CodigoTipoSinistro { get; set; }
        public int CodigoGravidadeSinistro { get; set; }
    }
}
