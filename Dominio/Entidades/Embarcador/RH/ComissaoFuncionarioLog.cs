using System;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COMISSAO_FUNCIONARIO_LOG", EntityName = "ComissaoFuncionarioLog", Name = "Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioLog", NameType = typeof(ComissaoFuncionarioLog))]
    public class ComissaoFuncionarioLog : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioLog>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComissaoFuncionario", Column = "CMF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.RH.ComissaoFuncionario ComissaoFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "CFL_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcao", Column = "CFL_TIPO_ACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario TipoAcao { get; set; }


        public virtual bool Equals(ComissaoFuncionarioLog other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
