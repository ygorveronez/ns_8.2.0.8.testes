namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_INTELIPOST_INTEGRACAO", EntityName = "EmpresaIntelipostIntegracao", Name = "Dominio.Entidades.EmpresaIntelipostIntegracao", NameType = typeof(EmpresaIntelipostIntegracao))]
    public class EmpresaIntelipostIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EII_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "EII_TOKEN", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Token { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }




        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

       
    }
}
