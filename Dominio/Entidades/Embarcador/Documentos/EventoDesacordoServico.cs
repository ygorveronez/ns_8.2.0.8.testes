using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EVENTO_DESACORDO_SERVICO", EntityName = "EventoDesacordoServico", Name = "Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico", NameType = typeof(EventoDesacordoServico))]
    public class EventoDesacordoServico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "EDS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "EDS_STATUS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ambiente", Column = "EDS_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAmbiente), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoAmbiente Ambiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdLote", Column = "EDS_ID_LOTE", TypeType = typeof(long), NotNull = true)]
        public virtual long IdLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencialEvento", Column = "EDS_NUMERO_SEQUENCIAL_EVENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroSequencialEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTe", Column = "EDS_CHAVE_CTE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "EDS_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAutorizacao", Column = "EDS_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "EDS_PROTOCOLO", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoStatusResposta", Column = "EDS_CODIGO_STATUS_RESPOSTA", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string CodigoStatusResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoStatusResposta", Column = "EDS_DESCRICAO_STATUS_RESPOSTA", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string DescricaoStatusResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoAplicacao", Column = "EDS_VERSAO_APLICACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string VersaoAplicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "EDS_JUSTIFICATIVA", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "EDS_XML", Type = "StringClob", NotNull = false)]
        public virtual string XML { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Status)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico.Autorizado:
                        return "Autorizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico.Rejeitado:
                        return "Rejeitado";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
