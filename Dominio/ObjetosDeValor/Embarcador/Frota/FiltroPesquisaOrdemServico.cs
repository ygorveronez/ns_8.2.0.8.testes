using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroPesquisaOrdemServico
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoEquipamento { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoOperador { get; set; }
        public int CodigoServico { get; set; }
        public int CodigoEmpresa { get; set; }
        public double CpfCnpjLocalManutencao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<SituacaoOrdemServicoFrota> Situacao { get; set; }
        public TipoManutencaoOrdemServicoFrota? TipoManutencao { get; set; }
        public TipoOficina? TipoOrdemServico { get; set; }
        public int CodigoGrupoServico { get; set; }
        public int CodigoCentroResultado { get; set; }
        public string Placa { get; set; }
        public List<int> CodigosEmpresa { get; set; }
        public PrioridadeOrdemServico? Prioridade { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public int Inicio { get; set; }
        public int Limite { get; set; }
        public string NumeroFogoPneu { get; set; }
    }
}
