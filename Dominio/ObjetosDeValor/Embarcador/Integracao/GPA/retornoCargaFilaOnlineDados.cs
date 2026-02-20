namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GPA
{
    public class RetornoCargaFilaOnlineDados
    {
        public string protocoloCarga { get; set; }

        public string cnpjOrigem { get; set; }

        public string numeroDaRota { get; set; }

        public string cnpjTransportador { get; set; }

        public string cpfMotorista { get; set; }

        public string placaCavalo { get; set; }

        public string placaCarreta { get; set; }
        
        public string dataHoraEntradaFila { get; set; }

        public string dataHoraSaidaFila { get; set; }

        public string posicaoEntradaNaFila { get; set; }
    }
}
