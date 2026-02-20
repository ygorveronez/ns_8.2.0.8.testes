namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class AgrupamentoValoresTitulo
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorOriginalMoeda { get; set; }
    }
}
