namespace Dominio.Entidades.Embarcador.Financeiro
{
    /// <summary>
    /// Tabela pivot entre ConciliacaoTransportador e Empresa
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONCILIACAO_TRANSPORTADOR_EMPRESA", EntityName = "ConciliacaoTransportadorEmpresa", Name = "Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa", NameType = typeof(ConciliacaoTransportadorEmpresa))]
    public class ConciliacaoTransportadorEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConciliacaoTransportador", Column = "COT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConciliacaoTransportador ConciliacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
       
    }
}
