namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_DISTANCIA", EntityName = "DistanciaTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete", NameType = typeof(DistanciaTabelaFrete))]
    public class DistanciaTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometragemInicial", Column = "TFD_QUILOMETRAGEM_INICIAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? QuilometragemInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometragemFinal", Column = "TFD_QUILOMETRAGEM_FINAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? QuilometragemFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quilometros", Column = "TFD_QUILOMETROS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Quilometros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFD_MULTIPLICAR_VALOR_FAIXA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarValorDaFaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFP_MULTIPLICAR_PELO_RESULTADO_DA_DISTANCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarPeloResultadoDaDistancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TFD_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDistanciaTabelaFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDistanciaTabelaFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFP_MULTIPLICAR_VALOR_FIXO_FAIXA_DISTANCIA_PELO_PESO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? MultiplicarValorFixoFaixaDistanciaPeloPesoCarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                string sigla = " km";
                if(TabelaFrete.UsarCubagemComoParametroDeDistancia)
                    sigla = " m²";

                if (Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoDistanciaTabelaFrete.PorFaixaDistanciaPercorrida)
                {
                    if (QuilometragemInicial.HasValue && QuilometragemFinal.HasValue && QuilometragemInicial.Value > 0 && QuilometragemFinal.Value > 0)
                        return "De " + QuilometragemInicial.Value.ToString("n2") + " até " + QuilometragemFinal.Value.ToString("n2") + sigla;
                    else if (QuilometragemFinal.HasValue && QuilometragemFinal.Value <= 0)
                        return "À partir de " + QuilometragemInicial.Value.ToString("n2") + sigla;
                    else
                        return "Até " + QuilometragemFinal.Value.ToString("n2") + sigla;
                }
                else
                {
                    return "A cada " + Quilometros.Value.ToString("n2") + sigla;
                }
            }
        }
    }
}
