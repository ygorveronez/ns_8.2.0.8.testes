using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_AVERBACAO_BRADESCO", EntityName = "AverbacaoMDFe", Name = "Dominio.Entidades.AverbacaoMDFe", NameType = typeof(AverbacaoMDFe))]
    public class AverbacaoMDFe : EntidadeBase
    {
        public AverbacaoMDFe() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "MAB_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "MAB_CODIGO_RETORNO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "MAB_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "MAB_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "MAB_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MAB_TIPO", TypeType = typeof(Enumeradores.TipoAverbacaoMDFe), NotNull = true)]
        public virtual Enumeradores.TipoAverbacaoMDFe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MAB_STATUS", TypeType = typeof(Enumeradores.StatusAverbacaoMDFe), NotNull = true)]
        public virtual Enumeradores.StatusAverbacaoMDFe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SeguradoraAverbacao", Column = "MAB_SEGURADORA", TypeType = typeof(Dominio.Enumeradores.IntegradoraAverbacao), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraAverbacao SeguradoraAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Averbacao", Column = "MAB_AVERBACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Averbacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguroAverbacao", Column = "CPA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Seguros.ApoliceSeguroAverbacao ApoliceSeguroAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosIntegracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_AVERBACAO_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MAB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosIntegracao { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoAverbacaoMDFe.Autorizacao:
                        return "Autorização";
                    case Enumeradores.TipoAverbacaoMDFe.Cancelamento:
                        return "Cancelamento";
                    case Enumeradores.TipoAverbacaoMDFe.Encerramento:
                        return "Encerramento";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusAverbacaoMDFe.Rejeicao:
                        return "Rejeição";
                    case Enumeradores.StatusAverbacaoMDFe.Sucesso:
                        return "Sucesso";
                    case Enumeradores.StatusAverbacaoMDFe.Pendente:
                        return "Pendente";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Averbacao;// + " - " + (this.CTe?.Descricao ?? string.Empty);
            }
        }
    }
}
