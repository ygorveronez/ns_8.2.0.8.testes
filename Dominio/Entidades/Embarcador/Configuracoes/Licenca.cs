using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LICENCA", EntityName = "Licenca", Name = "Dominio.Entidades.Embarcador.Configuracoes.Licenca", NameType = typeof(Licenca))]
    public class Licenca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIC_TIPO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoLicenca), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.TipoLicenca Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIC_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIC_BLOQUEAR_CHECKLIST_COM_LICENCA_INVALIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCheckListComLicencaInvalida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIC_REQUISICAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarRequisicao { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                return this.Tipo.ObterDescricao();
            }

        }
    }
}
