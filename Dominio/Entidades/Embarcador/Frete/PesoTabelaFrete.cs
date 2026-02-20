namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_PESO", DynamicUpdate = true, EntityName = "PesoTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete", NameType = typeof(PesoTabelaFrete))]
    public class PesoTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeDeMedida UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TFP_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFP_CALCULO_PESO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorPesoTransportado), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorPesoTransportado CalculoPeso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoInicial", Column = "TFP_PESO_INICIAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? PesoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoFinal", Column = "TFP_PESO_FINAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? PesoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "TFP_PESO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFP_COM_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComAjudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFP_PARA_CALCULAR_VALOR_BASE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ParaCalcularValorBase { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ComponenteFrete ComponenteFrete { get; set; }


        public virtual string Descricao
        {
            get
            {
                string retorno = "";
                if (this.ModeloVeicularCarga != null)
                    retorno = this.ModeloVeicularCarga.Descricao + " - ";


                string comAjudante = "";
                if (ComAjudante)
                    comAjudante = " (Com Ajudante)";

                if (ParaCalcularValorBase)
                    comAjudante += " (Para Valor Base)";

                if (Tipo == ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete.PorFaixaPesoTransportado)
                {
                    if (PesoInicial.HasValue && PesoFinal.HasValue && PesoInicial.Value > 0 && PesoFinal.Value > 0)
                        retorno += "De " + PesoInicial.Value.ToString("n3") + " até " + PesoFinal.Value.ToString("n3") + " " + UnidadeMedida.Sigla;
                    else if (!PesoFinal.HasValue || PesoFinal.Value <= 0)
                        retorno += "À partir de " + PesoInicial.Value.ToString("n3") + " " + UnidadeMedida.Sigla;
                    else
                        retorno += "Até " + PesoFinal.Value.ToString("n3") + " " + UnidadeMedida.Sigla;
                }
                else
                {
                    retorno += "A cada " + Peso.Value.ToString("n3") + " " + UnidadeMedida.Sigla;
                }

                retorno += comAjudante;

                return retorno;
            }
        }
    }
}
