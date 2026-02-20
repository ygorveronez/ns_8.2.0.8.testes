using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SUBCONTRATACAO", EntityName = "Subcontratacao", Name = "Dominio.Entidades.Subcontratacao", NameType = typeof(Subcontratacao))]
    public class Subcontratacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SUB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "SUB_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataViagem", Column = "SUB_DATA_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJSubcontratado", Column = "SUB_CNPJ_SUBCONTRATADO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJSubcontratado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_SUBCONTRATADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaSubcontratada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "SUB_SITUACAO", TypeType = typeof(Dominio.Enumeradores.SituacaoSubcontratacao), NotNull = false)]
        public virtual Dominio.Enumeradores.SituacaoSubcontratacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoFalha", Column = "SUB_DESCRICAO_FALHA", Type = "StringClob", NotNull = false)]
        public virtual string DescricaoFalha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgruparCTes", Column = "SUB_AGRUPAR_CTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparCTes { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "SUB_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "SUB_TIPO_SERVICO", TypeType = typeof(Dominio.Enumeradores.TipoServico), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJExpedidor", Column = "SUB_CNPJ_EXPEDIDOR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJRecebedor", Column = "SUB_CNPJ_RECEBEDOR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO_SUBCONTRATACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico DocumentoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProcessoTransporte", Column = "SUB_CODIGO_PROCESSO_TRANSPORTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoProcessoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesCTeSubcontratacao", Column = "SUB_OBSERVACOES_SUBCONTRATACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacoesCTeSubcontratacao { get; set; }

        //[NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SUBCONTRATACAO_DOCUMENTOS")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "SUB_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SubcontratacaoDocumentos", Column = "SUD_CODIGO")]
        //public virtual IList<Dominio.Entidades.SubcontratacaoDocumentos> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoContrato", Column = "SUB_DATA_EMISSAO_CONTRATO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DataEmissaoContrato {get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoPercurso", Column = "SUB_DESCRICAO_PERCURSO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string DescricaoPercurso { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case Enumeradores.SituacaoSubcontratacao.AgProcessamento:
                        return "Ag. Processamento";
                    case Enumeradores.SituacaoSubcontratacao.DocumentosCancelados:
                        return "Documentos Cancelados";
                    case Enumeradores.SituacaoSubcontratacao.EmitindoCTes:
                        return "Emitindo CTes";
                    case Enumeradores.SituacaoSubcontratacao.FalhaProcessamento:
                        return "Falha processamento";
                    case Enumeradores.SituacaoSubcontratacao.Finalizado:
                        return "Finalizado";
                    case Enumeradores.SituacaoSubcontratacao.Pendente:
                        return "Pendente";
                    case Enumeradores.SituacaoSubcontratacao.RejeicaoCTe:
                        return "Rejeição CTe";
                    default:
                        return "";
                }
            }
        }
    }
}
