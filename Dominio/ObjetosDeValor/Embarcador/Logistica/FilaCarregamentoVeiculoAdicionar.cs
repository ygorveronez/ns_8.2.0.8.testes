using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoAdicionar
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoMotorista { get; set; }
        
        public int CodigoTipoRetornoCarga { get; set; }

        public int CodigoVeiculo { get; set; }

        public int CodigoEquipamento { get; set; }

        public List<int> CodigosDestino { get; set; }

        public List<int> CodigosRegiaoDestino { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo ConjuntoVeiculo { get; set; }

        public Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }

        public DateTime? DataProgramada { get; set; }

        public bool EmTransicao { get; set; }

        public List<string> SiglasEstadoDestino { get; set; }

        public int CodigoAreaVeiculo { get; set; }
    }
}
