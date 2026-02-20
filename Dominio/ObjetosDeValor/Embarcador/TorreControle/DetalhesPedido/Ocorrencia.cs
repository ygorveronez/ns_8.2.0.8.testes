namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido
{
    public class Ocorrencia
    {
        public int Codigo { get; set; }

        public int CodigoOcorrencia { get; set; }

        public int CodigoPedido { get; set; }

        public string Descricao { get; set; }

        public string DataOcorrencia { get; set; }

        public string DataPosicao { get; set; }

        public string DataReprogramada { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string TempoPercurso { get; set; }

        public string Distancia { get; set; }

        public string Origem { get; set; }

        public string Natureza { get; set; }

        public string GrupoOcorrencia { get; set; }

        public string Razao { get; set; }

        public int NotaFiscalDevolucao { get; set; }

        public string SolicitacaoCliente { get; set; }
    }
}