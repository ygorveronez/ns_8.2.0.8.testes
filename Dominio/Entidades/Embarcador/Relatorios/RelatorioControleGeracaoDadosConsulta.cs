using System;

namespace Dominio.Entidades.Embarcador.Relatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RELATORIO_CONTROLE_GERACAO_DADOS_CONSULTA", EntityName = "RelatorioControleGeracaoDadosConsulta", Name = "Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracaoDadosConsulta", NameType = typeof(RelatorioControleGeracaoDadosConsulta))]

    public class RelatorioControleGeracaoDadosConsulta : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracaoDadosConsulta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCD_RELATORIO_TEMPORARIO", Type = "StringClob", NotNull = false)]
        public virtual string RelatorioTemporario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCD_PARAMETROS_CONSULTA", Type = "StringClob", NotNull = false)]
        public virtual string ParametrosConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCD_FILTROS_PESQUISA", Type = "StringClob", NotNull = false)]
        public virtual string FiltrosPesquisa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCD_PROPRIEDADES", Type = "StringClob", NotNull = false)]
        public virtual string Propriedades { get; set; }

        public virtual bool Equals(RelatorioControleGeracaoDadosConsulta other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
