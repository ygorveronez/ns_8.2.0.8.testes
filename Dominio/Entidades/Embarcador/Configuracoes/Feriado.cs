namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FERIADO", EntityName = "Feriado", Name = "Dominio.Entidades.Embarcador.Configuracoes.Feriado", NameType = typeof(Feriado))]
    public class Feriado : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FER_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FER_DIA", TypeType = typeof(int), NotNull = true)]
        public virtual int Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FER_MES", TypeType = typeof(int), NotNull = true)]
        public virtual int Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FER_ANO", TypeType = typeof(int), NotNull = false)]
        public virtual int? Ano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FER_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FER_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FER_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual string DescricaoData
        {
            get
            {
                return $"{Dia.ToString("D2")}/{Mes.ToString("D2")}{(Ano.HasValue ? $"/{Ano.Value.ToString("D4")}" : "")}";
            }
        }

        #endregion Propriedades com Regras
    }
}
