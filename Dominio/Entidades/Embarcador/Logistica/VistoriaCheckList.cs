using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VISTORIA_CHECKLIST", EntityName = "VistoriaCheckList", Name = "Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList", NameType = typeof(VistoriaCheckList))]
    public class VistoriaCheckList : EntidadeBase, IEquatable<VistoriaCheckList>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VCH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriocidadeVencimento", Column = "VCH_PERIOCIDADE_VENCIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int PeriocidadeVencimento{ get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VCH_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo Checklist { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeiculares", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VISTORIA_CHECKLIST_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VCH_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModelosVeiculares { get; set; }


        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case true: return "Ativo";
                    case false: return "Inativo";
                    default: return "";
                }
            }
        }

        public virtual bool Equals(VistoriaCheckList other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
