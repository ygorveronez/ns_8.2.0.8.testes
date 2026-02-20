using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega
{
    public class ComprovanteColeta
    {
        public string Cliente { get; set; }
        public string Transportador { get; set; }
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
        public string DataChegada { get; set; }
        public string DataInicioCarregamento { get; set; }
        public string DataFimCarregamento { get; set; }
        public string DataConfirmacaoColeta { get; set; }
        public string QuantidadePlanejada { get; set; }
        public string QuantidadeColetada { get; set; }
        public string Filial { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string ModeloVeicular { get; set; }
        public string CpfMotorista { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public string TipoCarga { get; set; }
        public string TipoOpercao { get; set; }
        public string Rota { get; set; }
        public DateTime DataCarregamento { get; set; }
        public string DadosGta { get; set; }
        public string NotasFiscais { get; set; }
        public string DataCarregamentoFormatada { get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy") : string.Empty; } }
        public string QuantidadeAnimais { get; set; }
        public bool PossuiPerguntasColeta { get; set; }
        public bool PossuiPerguntasDesembarque { get; set; }
        public bool PossuiFotoNotasFiscais { get; set; }
        public bool PossuiFotosEmbarque { get; set; }
    }
}
