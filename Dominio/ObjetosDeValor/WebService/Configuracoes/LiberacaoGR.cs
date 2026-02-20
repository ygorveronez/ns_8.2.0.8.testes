namespace Dominio.ObjetosDeValor.WebService.Configuracoes
{
    public class LiberacaoGR
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int QuantidadeCarga { get; set; }
        public int QuantidadePeriodo { get; set; }
        public Embarcador.Enumeradores.DiaSemanaMesAno TipoPeriodo { get; set; }
        public int CodigoInterno { get; set; }
    }
}
