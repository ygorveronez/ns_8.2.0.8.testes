namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class GradeCarregamentoExclusividade
    {
        public GradeCarregamentoExclusividade(int codigoTransportador, double codigoCliente, int codigoModeloVeicular, int quantidadeExclusivas)
        {
            Transportador = codigoTransportador;
            Cliente = codigoCliente;
            ModeloVeicular = codigoModeloVeicular;
            Quantidade = quantidadeExclusivas;
        }


        public int Transportador { get; set; }

        public double Cliente { get; set; }

        public int ModeloVeicular { get; set; }

        public int Quantidade { get; set; }
    }
}
