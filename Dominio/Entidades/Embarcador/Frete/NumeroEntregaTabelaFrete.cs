namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_NUMERO_ENTREGA", EntityName = "NumeroEntregaTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete", NameType = typeof(NumeroEntregaTabelaFrete))]
    public class NumeroEntregaTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TFN_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroEntregaTabelaFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroEntregaTabelaFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroInicialEntrega", Column = "TFN_NUMERO_INICIAL_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroInicialEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFinalEntrega", Column = "TFN_NUMERO_FINAL_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroFinalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFN_COM_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComAjudante { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroEntregaTabelaFrete.PorFaixaEntrega)
                {
                    string comAjudante = "";
                    if (ComAjudante)
                        comAjudante = " (Com Ajudante)";

                    if (NumeroInicialEntrega.HasValue && NumeroFinalEntrega.HasValue && NumeroInicialEntrega.Value > 0 && NumeroFinalEntrega.Value > 0)
                        return "De " + NumeroInicialEntrega.Value.ToString() + " à " + NumeroFinalEntrega.Value.ToString() + " entregas" + comAjudante;
                    else if (!NumeroFinalEntrega.HasValue || NumeroFinalEntrega.Value <= 0)
                        return "Acima de " + NumeroInicialEntrega.Value.ToString() + " entregas" + comAjudante;
                    else
                        return "Até " + NumeroFinalEntrega.Value.ToString() + " entregas" + comAjudante;
                }
                else
                {
                    return "Valor fixo por entrega";
                }
            }
        }
    }
}
