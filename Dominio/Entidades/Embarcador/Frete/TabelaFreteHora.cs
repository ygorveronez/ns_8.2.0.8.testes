namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_HORA", EntityName = "TabelaFreteHora", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteHora", NameType = typeof(TabelaFreteHora))]
    public class TabelaFreteHora : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaHoraTabelaFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaHoraTabelaFrete Tipo { get; set; }

        /// <summary>
        /// Armazena em minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_MINUTO_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int? MinutoInicial { get; set; }

        /// <summary>
        /// Armazena em minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_MINUTO_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int? MinutoFinal { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaHoraTabelaFrete.PorFaixaHora)
                {
                    if (MinutoInicial.HasValue && MinutoFinal.HasValue && MinutoInicial.Value > 0 && MinutoFinal.Value > 0)
                        return "De " + (MinutoInicial.Value / 60).ToString() + " à " + (MinutoFinal.Value / 60).ToString() + " horas";
                    else if (!MinutoFinal.HasValue || MinutoFinal.Value <= 0)
                        return "À partir de " + (MinutoInicial.Value / 60).ToString() + " horas";
                    else
                        return "Até " + (MinutoFinal.Value / 60).ToString() + " horas";

                }
                else
                {
                    return "Valor fixo por hora";
                }
            }
        }
    }
}
