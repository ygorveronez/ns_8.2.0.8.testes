namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Schedule
    {
        public TerminalPorto TerminalAtracacao { get; set; }
        public Porto PortoAtracacao { get; set; }
        public string DataPrevisaoChegadaNavio { get; set; }
        public string DataPrevisaoSaidaNavio { get; set; }
        public string DataDeadLine { get; set; }
        public bool ETAConfirmado { get; set; }
        public bool ETSConfirmado { get; set; }
        public bool InativarCadastro { get; set; }
        public bool Atualizar { get; set; }
    }
}
