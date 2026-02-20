using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaPneuHistorico
    {
        public int CodigoBandaRodagem { get; set; }

        public int CodigoDimensao { get; set; }

        public int CodigoEmpresa { get; set; }

        public int CodigoMarca { get; set; }

        public int CodigoModelo { get; set; }

        public int CodigoPneu { get; set; }

        public int CodigoServico { get; set; }

        public System.DateTime? DataInicio { get; set; }

        public System.DateTime? DataLimite { get; set; }

        public Enumeradores.VidaPneu? Vida { get; set; }

        public Enumeradores.SituacaoPneu? SituacaoPneu { get; set; }

        public bool SomenteSucata { get; set; }

        public int CodigoVeiculo { get; set; }

        public string DTO { get; set; }

        public int CodigoUsuarioOperador { get; set; }

        public List<int> CodigoMotivoSucata { get; set; }

        public List<int> CodigoAlmoxarifado { get; set; }

        public List<Enumeradores.TipoAquisicaoPneu> TiposAquisicao { get; set; }
    }
}
