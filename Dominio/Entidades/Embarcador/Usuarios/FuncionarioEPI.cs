using System;

namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_EPI", EntityName = "FuncionarioEPI", Name = "Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI", NameType = typeof(FuncionarioEPI))]
    public class FuncionarioEPI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "EPI", Column = "EPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.EPI EPI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "FEP_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieEPI", Column = "FEP_SERIE_EPI", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SerieEPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRepasse", Column = "FEP_DATA_REPASSE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRepasse { get; set; }

        public virtual string Descricao
        {
            get { return $"{this.Codigo} - {this.EPI.Descricao} - {this.Usuario.Nome}"; }
        }
    }
}
