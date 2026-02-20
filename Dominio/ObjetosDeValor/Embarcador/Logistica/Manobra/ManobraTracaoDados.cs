namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class ManobraTracaoDados
    {
        public int CentroCarregamento { get; set; }

        public int Codigo { get; set; }

        public string AcaoAtual { get; set; }

        public string ClasseCor { get; set; }

        public string DescricaoSituacao { get; set; }

        public string Motorista { get; set; }

        public string Placa { get; set; }

        public Enumeradores.SituacaoManobraTracao Situacao { get; set; }

        public int Tracao { get; set; }

        public string Transportador { get; set; }
    }
}
