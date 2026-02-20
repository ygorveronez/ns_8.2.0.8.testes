using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PARAMETROS_OCORRENCIA", EntityName = "ParametroOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia", NameType = typeof(ParametroOcorrencia))]
    public class ParametroOcorrencia : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoParametro", Column = "POC_TIPO_PARAMETRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia TipoParametro { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "POC_DESCRICAO", TypeType = typeof(string), NotNull = true, Length = 100)]
        public virtual string Descricao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_DESCRICAO_PARAMETRO", TypeType = typeof(string), NotNull = true, Length = 100)]
        public virtual string DescricaoParametro { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_DESCRICAO_PARAMETRO_FINAL", TypeType = typeof(string), NotNull = true, Length = 100)]
        public virtual string DescricaoParametroFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "POC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoTipoParametro
        {
            get { return this.TipoParametro.ObterDescricao(); }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo == true)
                    return "Ativo";
                if (this.Ativo == false)
                    return "Inativo";
                else
                    return "Não definido";
            }
        }

        public virtual bool Equals(ParametroOcorrencia other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
