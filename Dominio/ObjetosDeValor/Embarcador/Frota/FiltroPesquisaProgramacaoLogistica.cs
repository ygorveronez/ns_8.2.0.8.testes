using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroPesquisaProgramacaoLogistica
    {
        public List<int> CodigosSituacaoColaborador { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoProgramacaoAlocacao { get; set; }
        public List<int> CodigosTipoVeiculo { get; set; }
        public int CodigoVeiculo { get; set; }
        public List<int> CodigosPlotagemVeiculo { get; set; }
        public List<int> CodigosTipoCarroceria { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa EmManutencao { get; set; }
        public TipoEntradaSaida TipoGuarita { get; set; }
        public SituacaoProgramacaoLogistica SituacaoProgramacaoLogistica { get; set; }
        public TipoMotorista TipoMotorista { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa EmViagem { get; set; }
        public bool SomenteMotoristaComOciosidade { get; set; }
    }
}
