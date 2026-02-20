using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.CTeAgrupado
{
    public class FiltroSelecaoCargas
    {
        public int CodigoCTeAgrupado { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public List<int> CodigoTipoOperacao { get; set; } = new List<int>();
        public List<int> CodigoTipoCarga { get; set; } = new List<int>();
        public List<int> CodigoVeiculo { get; set; } = new List<int>();
        public List<int> CodigoMotorista { get; set; } = new List<int>();
        public bool? SemCTeAgrupado { get; set; }
        public List<int> CargasSelecionadas { get; set; } = new List<int>();
        public bool SelecionarTodos { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> Situacao { get; set; } = new List<Enumeradores.SituacaoCarga>();

    }
}
