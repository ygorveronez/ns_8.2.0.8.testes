using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Administrativo
{
    public class FiltroPesquisaRelatorioLicenca
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoTipoLicenca { get; set; }
        public string Entidade { get; set; }
        public string Descricao { get; set; }
        public string NumeroLicenca { get; set; }
        public int CodigoFuncionario { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoVeiculo { get; set; }
        public double CodigoPessoa { get; set; }
        public StatusLicenca? StatusLicenca { get; set; }
        public TipoTelaLicenca? TipoLicenca { get; set; }
        public int CodigoEmpresa { get; set; }
        public SituacaoAtivoPesquisa StatusEntidade { get; set; }
    }
}
