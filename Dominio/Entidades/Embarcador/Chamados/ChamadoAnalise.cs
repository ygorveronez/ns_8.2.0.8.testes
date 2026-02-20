using System;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_ANALISES", EntityName = "ChamadoAnalise", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise", NameType = typeof(ChamadoAnalise))]
    public class ChamadoAnalise : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ANC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_OBSERVACAO", Type = "StringClob", NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_NAO_REGISTRAR_OBSERVACAO_TRANSPORTADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRegistrarObservacaoTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Autor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_DATA_ANALISE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaOcorrencia", Column = "JTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Ocorrencias.JustificativaOcorrencia JustificativaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_DATA_REENTREGA_MESMA_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReentregaMesmaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_APROVADO_VALOR_CARGA_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? LiberadoValorCargaDescarga { get; set; }

        [Obsolete("O campo foi adicionado ao chamado")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "ANC_ESTADIA", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao Estadia { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Autor?.Descricao ?? string.Empty) + " - " + (this.Chamado?.Descricao ?? string.Empty);
            }
        }
    }
}
