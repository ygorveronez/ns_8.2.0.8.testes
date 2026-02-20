using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODELO_VEICULAR_CARGA_GRUPO", EntityName = "GrupoModeloVeicular", Name = "Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular", NameType = typeof(GrupoModeloVeicular))]
    public class GrupoModeloVeicular : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MVG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MVG_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MVG_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }       

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
        public virtual bool Equals(GrupoModeloVeicular other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
