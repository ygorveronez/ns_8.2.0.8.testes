namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class FiltroPesquisaContratoFreteCliente
    {
        public string NumeroContrato { get; set; }

        public string Descricao { get; set; }

        public double Cliente { get; set; }

        public int TipoOperacao { get; set; }

        public bool? ContratoFechado { get; set; }
    }
}
