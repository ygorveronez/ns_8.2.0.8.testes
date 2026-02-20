using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class ObjetoOrdemServico
    {
        public ObjetoOrdemServico()
        {
            DataProgramada = DateTime.Now.Date;
        }

        public bool CadastrandoVeiculoEquipamento { get; set; }
        public DateTime DataProgramada { get; set; }
        public Dominio.Entidades.Cliente LocalManutencao { get; set; }
        public Dominio.Entidades.Usuario Motorista { get; set; }
        public string Observacao { get; set; }
        public string CondicaoPagamento { get; set; }
        public Dominio.Entidades.Usuario Operador { get; set; }
        public Dominio.Entidades.Veiculo Veiculo { get; set; }
        public Dominio.Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }
        public int Horimetro { get; set; }
        public int QuilometragemVeiculo { get; set; }
        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo TipoOrdemServico { get; set; }
        public Dominio.Entidades.Empresa Empresa { get; set; }
        public bool LancarServicosManualmente { get; set; }
        public Dominio.Entidades.Embarcador.Frota.GrupoServico GrupoServico { get; set; }
        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }
        public Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota ServicoVeiculo { get; set; }
        public List<ObjetoOrdemServicoServicos> Servicos { get; set; }
        public DateTime DataManutencao { get; set; }
        public decimal Custo { get; set; }
        public Dominio.Entidades.Usuario Responsavel { get; set; }
        public Dominio.Entidades.Embarcador.Frota.Pneu Pneu { get; set; }
        public Dominio.Entidades.Embarcador.Frota.PneuEnvioReforma PneuEnvioReforma { get; set; }
        public DateTime? DataLimiteExecucao { get; set; }
        public PrioridadeOrdemServico? Prioridade { get; set; }
        public decimal ValorOrcado { get; set; }
        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao TipoLocalManutencao { get; set; }

    }
}
