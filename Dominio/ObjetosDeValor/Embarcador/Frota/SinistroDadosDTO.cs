using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class SinistroDadosDTO
    {
        public int Codigo { get; set; }

        public int Numero { get; set; }

        public string NumeroBoletimOcorrencia { get; set; }

        public CausadorSinistro CausadorSinistro { get; set; }

        public DateTime DataSinistro { get; set; }

        public DateTime? DataEmissao { get; set; }

        public string Local { get; set; }

        public int CodigoCidade { get; set; }

        public string Endereco { get; set; }

        public int CodigoVeiculo { get; set; }
        public int CodigoVeiculoReboque { get; set; }

        public int CodigoMotorista { get; set; }

        public string Observacao { get; set; }
        public int CodigoTipoSinistro { get; set; }
        public int CodigoGravidadeSinistro { get; set; }
    }
}
