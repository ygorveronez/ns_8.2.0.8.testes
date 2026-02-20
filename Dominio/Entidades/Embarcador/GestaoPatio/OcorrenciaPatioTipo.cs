using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_PATIO_TIPO", EntityName = "OcorrenciaPatioTipo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo", NameType = typeof(OcorrenciaPatioTipo))]
    public class OcorrenciaPatioTipo : EntidadeBase, IEntidade, IEquatable<OcorrenciaPatioTipo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "OPT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OPT_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "OPT_TIPO", TypeType = typeof(TipoOcorrenciaPatio), NotNull = false)]
        public virtual TipoOcorrenciaPatio Tipo { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(OcorrenciaPatioTipo other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
