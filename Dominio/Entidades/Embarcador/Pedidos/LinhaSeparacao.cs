namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LINHA_SEPARACAO", DynamicUpdate = true, EntityName = "LinhaSeparacao", Name = "Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao", NameType = typeof(LinhaSeparacao))]
    public class LinhaSeparacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CLS_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CLS_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CLS_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CLS_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidadoAgrupamentos", Column = "CLS_VALIDADO_AGRUPA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidadoAgrupamentos { get; set; }

        /// <summary>
        /// Quando não roteiriza, os produtos desta linha não deve ser roteirizador nem nesmo disponibilizados para inclusão manual nos carregamentos.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Roteiriza", Column = "CLS_ROTEIRIZA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Roteiriza { get; set; }

        /// <summary>
        /// Atributo para definir a prioridade no processo de fechamento de cargas (MontagemCarga - ASSAI)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelPrioridade", Column = "CLS_NIVEL_PRIORIDADE", TypeType = typeof(System.Int32), NotNull = false)]
        public virtual int NivelPrioridade { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
