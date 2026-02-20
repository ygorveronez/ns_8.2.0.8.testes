namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class CancelamentoProvisaoSumarizada
    {
        public int Codigo { get; set; }
        public int? Carga { get; set; }
        public int? CargaOcorrencia { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga? SituacaoCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia? SituacaoOcorrencia { get; set; }
    }
}
