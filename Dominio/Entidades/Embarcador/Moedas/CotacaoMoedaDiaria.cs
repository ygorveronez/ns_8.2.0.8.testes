using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Moedas;

[NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_MOEDA_DIARIA", EntityName = "CotacaoMoedaDiaria", Name = "Dominio.Entidades.Embarcador.Moedas.CotacaoMoedaDiaria", NameType = typeof(CotacaoMoedaDiaria))]
public class CotacaoMoedaDiaria : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Moedas.CotacaoMoedaDiaria>
{
    [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMD_CODIGO")]
    [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
    public virtual int Codigo { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "CMD_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = true)]
    public virtual MoedaCotacaoBancoCentral MoedaCotacaoBancoCentral { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoeda", Column = "CMD_VALOR_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = true)]
    public virtual decimal ValorMoeda { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsulta", Column = "CMD_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = true)]
    public virtual DateTime DataConsulta { get; set; }

    public virtual string Descricao
    {
        get { return $"{MoedaCotacaoBancoCentral.ObterDescricao()} - {DataConsulta:d}"; }
    }

    public virtual bool Equals(CotacaoMoedaDiaria other)
    {
        if (other.Codigo == this.Codigo)
            return true;
        else
            return false;
    }
}
