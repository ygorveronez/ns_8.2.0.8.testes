using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Operacional.Canhoto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_CANHOTO", EntityName = "OperadorCanhoto", Name = "Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto", NameType = typeof(OperadorCanhoto))]
    public class OperadorCanhoto : EntidadeBase, IEquatable<OperadorCanhoto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "OPC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizaCargasSemTipoOperacao", Column = "OPC_VISUALIZA_CARGA_SEM_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizaCargasSemTipoOperacao { get; set; } 

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiFiltroTipoOperacao", Column = "OPC_POSSUI_FILTRO_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiFiltroTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_CANHOTO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_CANHOTO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Filiais.Filial> Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_CANHOTO_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OperadorCanhotoTipoCarga", Column = "CTC_CODIGO")]
        public virtual IList<OperadorCanhotoTipoCarga> TiposCarga { get; set; }

        public virtual string Descricao
        {
            get { return this.Usuario?.Descricao ?? string.Empty; }
        }

        public virtual bool Equals(OperadorCanhoto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

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
