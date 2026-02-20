namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica
{
    public sealed class ControleCarregamento
    {
        public int Codigo { get; set; }

        public int CodigoCentroCarregamento { get; set; }

        public int CodigoVeiculo { get; set; }

        public string DataCriacao { get; set; }

        public string DataInicioCarregamento { get; set; }

        public string Local { get; set; }

        public string Placa { get; set; }

        public string Transportador { get; set; }
    }
}
