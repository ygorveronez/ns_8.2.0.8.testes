using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;


namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class FiltroPesquisaRelatorioTacografo
    {
        public List<int> CodigosMotoristas { get; set; }
        public List<int> CodigosVeiculos { get; set; }
        public List<int> Situacoes { get; set; }
        public DateTime DataInicialRepasse { get; set; }
        public DateTime DataFinalRepasse { get; set; }
        public DateTime DataInicialRetorno { get; set; }
        public DateTime DataFinalRetorno { get; set; }
        public SituacaoAtivoPesquisa ExcessoVelocidade { get; set; }
    }
}
