using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class FiltroPesquisaRelatorioVeiculo
    {
        public string Placa { get; set; }
        public string Chassi { get; set; }
        public string TipoVeiculo { get; set; }
        public string Tipo { get; set; }
        public double CpfcnpjProprietario { get; set; }
        public int CodigoEmpresa { get; set; }
        public List<int> CodigosEmpresa { get; set; }
        public int CodigoMotorista { get; set; }
        public Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }
        public List<int> CodigosSegmento { get; set; }
        public List<int> CodigosFuncionarioResponsavel { get; set; }
        public int CodigoCentroCarregamento { get; set; }
        public int CodigoCentroResultado { get; set; }
        public bool VeiculoPossuiTagValePedagio { get; set; }
        public List<int> CodigosMarcaVeiculo { get; set; }
        public List<int> CodigosModeloVeiculo { get; set; }
        public DateTime? DataCadastroInicial { get; set; }
        public DateTime? DataCadastroFinal { get; set; }
        public DateTime? DataCriacaoInicial { get; set; }
        public DateTime? DataCriacaoFinal { get; set; }
        public bool? Bloqueado { get; set; }
        public List<int> ContratosFrete { get; set; }
        public Enumeradores.SimNao PossuiVinculo { get; set; }
        public List<int> CodigosModeloVeicularCarga { get; set; }
        public string TagSemParar { get; set; }
        public double Locador { get; set; }
    }
}
