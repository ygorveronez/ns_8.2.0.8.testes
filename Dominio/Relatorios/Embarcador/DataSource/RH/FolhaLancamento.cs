using System;

namespace Dominio.Relatorios.Embarcador.DataSource.RH
{
    public class FolhaLancamento
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        private DateTime DataInicial { get; set; }
        private DateTime DataFinal { get; set; }
        public int NumeroEvento { get; set; }
        public string NomeFuncionario { get; set; }
        private string CpfFuncionario { get; set; }
        public string CodigoIntegracaoFuncionario { get; set; }
        public string InformacaoFolha { get; set; }
        public decimal ValorBase { get; set; }
        public decimal Valor { get; set; }
        private DateTime DataCompetencia { get; set; }
        private string SituacaoFuncionario { get; set; }

        public string DataInicialFormatada
        {
            get { return DataInicial > DateTime.MinValue ? DataInicial.ToDateString() : string.Empty; }
        }

        public string DataFinalFormatada
        {
            get { return DataFinal > DateTime.MinValue ? DataFinal.ToDateString() : string.Empty; }
        }

        public string DataCompetenciaFormatada
        {
            get { return DataCompetencia > DateTime.MinValue ? DataCompetencia.ToDateString() : string.Empty; }
        }

        public string CpfFuncionarioFormatado
        {
            get { return CpfFuncionario.ObterCpfFormatado(); }
        }

        public string SituacaoFuncionarioDescricao
        {
            get { return SituacaoFuncionario == "A" ? "Ativo" : "Inativo"; }
        }
    }
}
