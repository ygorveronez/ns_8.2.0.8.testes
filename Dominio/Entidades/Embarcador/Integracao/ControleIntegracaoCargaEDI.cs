using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_INTEGRACAO_CARGA_EDI", EntityName = "ControleIntegracaoCargaEDI", Name = "Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI", NameType = typeof(ControleIntegracaoCargaEDI))]
    public class ControleIntegracaoCargaEDI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMeioPagamento", Column = "CIE_CODIGO_MEIO_PAGAMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoMeioPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CIE_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacaoSituacaoCarga", Column = "CIE_DATA_ATUALIZACAO_SITUACAO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacaoSituacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "CIE_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDOC", Column = "CIE_IDOC", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IDOC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "CIE_MENSAGEM_RETORNO", Type = "StringClob", NotNull = true)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "CIE_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDT", Column = "CIE_NUMERO_DT", Type = "StringClob", NotNull = true)]
        public virtual string NumeroDT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "CIE_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "CIE_PLACA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Placa { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoCliente", Column = "CIE_CODIGO_INTEGRACAO_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_ARQUIVO_IMPORTACAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArquivoImportacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCarga", Column = "CIE_SITUACAO_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI? SituacaoCarga { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_TIPO_ARQUIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoCargaIntegracaoEDI), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoCargaIntegracaoEDI? TipoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracaoCargaEDI", Column = "CIE_SITUACAO_CARGA_EDI", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI SituacaoIntegracaoCargaEDI { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Cargas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTROLE_INTEGRACAO_CARGA_EDI_CARGAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual ICollection<Cargas.Carga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "NumerosDTs", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTROLE_INTEGRACAO_CARGA_EDI_NUMEROS_DTS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CIN_NUMERO_DT", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual ICollection<string> NumerosDTs { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioritario", Column = "CIE_PRIORITARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Prioritario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual WebService.Integradora Integradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsumoArquivo", Column = "CIE_DATA_CONSUMO_ARQUIVO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConsumoArquivo { get; set; }
    }
}
