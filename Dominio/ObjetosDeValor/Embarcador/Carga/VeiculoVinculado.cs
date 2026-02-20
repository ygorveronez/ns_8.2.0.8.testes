namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class VeiculoVinculado
    {
        public int CodigoCarga { get; set; }
        public int CodigoVeiculo { get; set;}
    }


    public class VeiculoVinculadoMaisVeiculaDaCarga
    {
        public int CodigoCarga { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Placa { get; set; }

    }

}
