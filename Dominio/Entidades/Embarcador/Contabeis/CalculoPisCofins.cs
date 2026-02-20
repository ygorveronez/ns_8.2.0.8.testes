namespace Dominio.Entidades.Embarcador.Contabeis
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CALCULO_PIS_COFINS", EntityName = "CalculoPisCofins", Name = "Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins", NameType = typeof(CalculoPisCofins))]
    public class CalculoPisCofins : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPISTributavel", Column = "CPC_CST_PIS_TRIBUTAVEL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS CSTPISTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPISNaoTributavel", Column = "CPC_CST_PIS_NAO_TRIBUTAVEL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS CSTPISNaoTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "CPC_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINSTributavel", Column = "CPC_CST_COFINS_TRIBUTAVEL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS CSTCOFINSTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINSNaoTributavel", Column = "CPC_CST_COFINS_NAO_TRIBUTAVEL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS CSTCOFINSNaoTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "CPC_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Imposto PIS/COFINS"; }
        }
    }
}
