namespace Dominio.ObjetosDeValor.CTe
{
    public class ComponentePrestacao
    {
        public ComponentePrestacao()
        {
        }

        public string Descricao { get; set; }

        public decimal Valor { get; set; }

        public bool IncluiBaseCalculoICMS { get; set; }
        
        public bool IncluiValorAReceber { get; set; }

        public int CodigoComponenteFrete { get; set; }
    }
}
