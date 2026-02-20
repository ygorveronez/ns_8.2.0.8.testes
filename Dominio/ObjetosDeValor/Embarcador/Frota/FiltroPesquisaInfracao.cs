using Dominio.Entidades.Embarcador.Frota;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaInfracao
    {
        public int CodigoCidade { get; set; }

        public int CodigoTipoInfracao { get; set; }

        public int CodigoVeiculo { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public int Numero { get; set; }

        public string NumeroAtuacao { get; set; }
        public string Placa { get; set; }

        public Enumeradores.SituacaoInfracao? Situacao { get; set; }

        public Enumeradores.TipoOcorrenciaInfracao? TipoOcorrenciaInfracao { get; set; }

        public int Funcionario { get; set; }

        public bool InfracoesPendentes { get; set; }

        public int Motorista { get; set; }

        public double OrgaoEmissor { get; set; }

        public TipoInfracaoTransito? TipoInfracao { get; set; }

        public TipoHistoricoInfracao? TipoHistorico { get; set; }

        public DateTime? DataVencimento { get; set; }
    }
}
