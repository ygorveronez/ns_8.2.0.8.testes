using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Administrativo
{
    public class FiltroPesquisaRelatorioLicencaVeiculo
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Descricao { get; set; }
        public string NumeroLicenca { get; set; }
        public string Renavam { get; set; }
        public int CodigoLicenca { get; set; }
        public int CodigoFuncionario { get; set; }
        public int CodigoCentroResultado { get; set; }
        public int CodigoMarca { get; set; }
        public int CodigoModelo { get; set; }
        public StatusLicenca StatusLicenca { get; set; }
        public SituacaoAtivoPesquisa StatusVeiculo { get; set; }
    }
}
