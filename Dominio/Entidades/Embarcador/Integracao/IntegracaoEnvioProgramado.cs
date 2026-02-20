using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Ocorrencias;
using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao;

[NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_ENVIO_PROGRAMADO", EntityName = "IntegracaoEnvioProgramado", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado", NameType = typeof(IntegracaoEnvioProgramado))]
public class IntegracaoEnvioProgramado : Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<IntegracaoEnvioProgramado>
{
    protected IntegracaoEnvioProgramado()
    {
    }

    public IntegracaoEnvioProgramado(DateTime dataEnvioProgramada)
    {
        this.DataEnvioProgramada = dataEnvioProgramada;
        this.DataIntegracao = dataEnvioProgramada;
    }

    [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IEP_CODIGO")]
    [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
    public virtual int Codigo { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioProgramada", Column = "IEP_DATA_ENVIO_PROGRAMADA", TypeType = typeof(DateTime), NotNull = true)]
    public virtual DateTime DataEnvioProgramada { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEntidadeIntegracao", Column = "IEP_TIPO_ENTIDADE_INTEGRACAO", TypeType = typeof(TipoEntidadeIntegracao), NotNull = true)]
    public virtual TipoEntidadeIntegracao TipoEntidadeIntegracao { get; set; }

    [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
    public virtual Carga Carga { get; set; }

    [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
    public virtual CargaOcorrencia CargaOcorrencia { get; set; }

    [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
    public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioAntecipado", Column = "IEP_ENVIO_ANTECIPADO", TypeType = typeof(bool), NotNull = false)]
    public virtual bool EnvioAntecipado { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioBloqueado", Column = "IEP_ENVIO_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
    public virtual bool EnvioBloqueado { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "StepIntegracao", Column = "IEP_STEP_INTEGRACAO", Type = "StringClob", NotNull = false)]
    public virtual string StepIntegracao { get; set; }

    [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "IEP_TIPO_DOCUMENTO", TypeType = typeof(Enumeradores.TipoDocumento), NotNull = false)]
    public virtual Enumeradores.TipoDocumento TipoDocumento { get; set; }

    [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_ENVIO_PROGRAMADO_ARQUIVO")]
    [NHibernate.Mapping.Attributes.Key(1, Column = "IEP_CODIGO")]
    [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
    public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

    public virtual string Descricao
    {
        get
        {
            return TipoEntidadeIntegracao.ObterDescricao();
        }
    }

    public virtual bool Equals(IntegracaoEnvioProgramado other)
    {
        if (other.Codigo == this.Codigo)
            return true;
        else
            return false;
    }
}
