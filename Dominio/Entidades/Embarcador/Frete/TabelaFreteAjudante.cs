namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_AJUDANTE", EntityName = "TabelaFreteAjudante", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante", NameType = typeof(TabelaFreteAjudante))]
    public class TabelaFreteAjudante : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaAjudanteTabelaFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaAjudanteTabelaFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0,  Column = "TFA_NUMERO_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0,  Column = "TFA_NUMERO_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroFinal { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes)
                {
                    if (NumeroInicial.HasValue && NumeroFinal.HasValue && NumeroInicial.Value > 0 && NumeroFinal.Value > 0)
                        return "De " + NumeroInicial.Value.ToString() + " à " + NumeroFinal.Value.ToString() + " ajudantes";
                    else if (!NumeroFinal.HasValue || NumeroFinal.Value <= 0)
                        return "À partir de " + NumeroInicial.Value.ToString() + " ajudantes";
                    else
                        return "Até " + NumeroFinal.Value.ToString() + " ajudantes";

                }
                else
                {
                    return "Valor fixo por ajudante";
                }
            }
        }
    }
}
