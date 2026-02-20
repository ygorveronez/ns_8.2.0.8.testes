namespace Dominio.ObjetosDeValor.CTe
{
    public class EntregaComponentePrestacao
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        public string Descricao { get; set; }

        public decimal Valor { get; set; }

        public bool IncluiBaseCalculoICMS { get; set; }

        public bool IncluiValorAReceber { get; set; }

        public int CodigoComponenteFrete { get; set; }
    }
}