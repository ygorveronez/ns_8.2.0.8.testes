namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class ModeloVeicularCarga
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public decimal CapacidadePesoTransporte { get; set; }

        public decimal ToleranciaPesoMenor { get; set; }

        public bool ModeloControlaCubagem { get; set; }

        public decimal Cubagem { get; set; }

        public decimal ToleranciaMinimaCubagem { get; set; }

        public bool VeiculoPaletizado { get; set; }

        public int? NumeroPaletes { get; set; }

        public int? NumeroReboques { get; set; }

        public int ToleranciaMinimaPaletes { get; set; }

        public decimal OcupacaoCubicaPaletes { get; set; }

        public bool ExigirDefinicaoReboquePedido { get; set; }
    }
}
