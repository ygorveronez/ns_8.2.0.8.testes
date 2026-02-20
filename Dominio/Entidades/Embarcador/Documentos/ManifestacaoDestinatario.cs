using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MANIFESTACAO_DESTINATARIO", EntityName = "ManifestacaoDestinatario", Name = "Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario", NameType = typeof(ManifestacaoDestinatario))]
    public class ManifestacaoDestinatario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "MDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "MDE_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MDE_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ambiente", Column = "MDE_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAmbiente), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoAmbiente Ambiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdLote", Column = "MDE_ID_LOTE", TypeType = typeof(long), NotNull = true)]
        public virtual long IdLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencialEvento", Column = "MDE_NUMERO_SEQUENCIAL_EVENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroSequencialEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFe", Column = "MDE_CHAVE_NFE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "MDE_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAutorizacao", Column = "MDE_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "MDE_PROTOCOLO", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoStatusResposta", Column = "MDE_CODIGO_STATUS_RESPOSTA", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string CodigoStatusResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoStatusResposta", Column = "MDE_DESCRICAO_STATUS_RESPOSTA", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string DescricaoStatusResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoAplicacao", Column = "MDE_VERSAO_APLICACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string VersaoAplicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "MDE_JUSTIFICATIVA", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Justificativa { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.CienciaOperacao:
                        return "Ciência da Operação";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.ConfirmadaOperacao:
                        return "Confirmada a Operação";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.Desconhecida:
                        return "Operação Desconhecida";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.OperacaoNaoRealizada:
                        return "Operação não realizada";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario.Autorizado:
                        return "Autorizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario.Rejeitado:
                        return "Rejeitado";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
