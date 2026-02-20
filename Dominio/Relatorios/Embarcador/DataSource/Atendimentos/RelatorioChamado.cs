using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Atendimentos
{
    public class RelatorioChamado
    {
        public DateTime DataChamado { get; set; }
        public DateTime DataAtendimento { get; set; }
        public string DescricaoStatus { get; set; }
        public string DescricaoPrioridade { get; set; }
        public string Tela { get; set; }
        public string Modulo { get; set; }
        public string Sistema { get; set; }
        public string Tipo { get; set; }
        public string Empresa { get; set; }
        public string EmpresaFilho { get; set; }
        public string Titulo { get; set; }
        public string Motivo { get; set; }
        public string Observacao { get; set; }
        public string Funcionario { get; set; }
        public string Pessoa { get; set; }
        public string ObservacaoSuporte { get; set; }
        public string Justificativa { get; set; }
        public int Numero { get; set; }
        public string Solicitante { get; set; }
    }
}
