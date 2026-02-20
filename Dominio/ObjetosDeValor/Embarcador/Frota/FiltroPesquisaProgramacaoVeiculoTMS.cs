using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroPesquisaProgramacaoVeiculoTMS
    {
        public string NumeroFrota { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoReboque { get; set; }
        public int ModeloVeicular { get; set; }
        public List<int> Situacoes { get; set; }
        public int Motorista { get; set; }
        public List<string> Estados { get; set; }
        public int CodigoFuncionarioResponsavelCavalo { get; set; }
        public int CodigoMarcaCavalo { get; set; }
        public DateTime? DataCadastroPlanejamentoInicial { get; set; }
        public DateTime? DataCadastroPlanejamentoFinal { get; set; }
        public DateTime? DataDisponibilidadeInicial { get; set; }
        public DateTime? DataDisponibilidadeFinal { get; set; }
    }
}
