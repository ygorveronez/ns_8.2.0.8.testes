namespace Dominio.ObjetosDeValor.Embarcador.Container
{
    public class RetornoConsultaContainer
    {
        public int Codigo { get; set; }
        public double? Armador { get; set; }
        public string Descricao { get; set; }
        public string ClienteArmador { get; set; }
        public string Numero { get; set; }
        public string CodigoIntegracao { get; set; }
        public bool Status { get; set; }
        public string DescricaoTipoContainer { get; set; }
        public decimal Tara { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer TipoPropriedade { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.TipoCarregamentoNavio TipoCarregamentoNavio { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

    }
}
