namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe
{
    public class Cte
    {
        public string numero_frete { get; set; }
        public string numero_cte { get; set; }
        public string cnpj_unidade_empresa { get; set; }
        public bool? status { get; set; }
        public string erro { get; set; }
        public bool ShouldSerializestatus()
        {
            return status.HasValue;
        }
        public bool ShouldSerializeerro()
        {
            return !string.IsNullOrWhiteSpace(erro);
        }
    }
}
