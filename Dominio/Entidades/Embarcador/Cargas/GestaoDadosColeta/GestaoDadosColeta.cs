using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_GESTAO_DADOS_COLETA", EntityName = "GestaoDadosColeta", Name = "Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta", NameType = typeof(GestaoDadosColeta))]
    public class GestaoDadosColeta : EntidadeBase
    {
        #region Construtores

        public GestaoDadosColeta()
        {
            DataCriacao = DateTime.Now;
        }

        #endregion Construtores

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Motivo", Column = "MOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Motivo Motivo { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GDC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_USUARIO_CRIACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "GDC_TIPO", TypeType = typeof(TipoGestaoDadosColeta), NotNull = true)]
        public virtual TipoGestaoDadosColeta Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Origem", Column = "GDC_ORIGEM", TypeType = typeof(OrigemGestaoDadosColeta), NotNull = true)]
        public virtual OrigemGestaoDadosColeta Origem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "GDC_SITUACAO", TypeType = typeof(SituacaoGestaoDadosColeta), NotNull = true)]
        public virtual SituacaoGestaoDadosColeta Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "GDC_LATITUDE", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "GDC_LONGITUDE", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? Longitude { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.CargaEntrega.Carga.CodigoCargaEmbarcador;
            }
        }

        #endregion Propriedades

        #region Propriedades da Aprovação

        [NHibernate.Mapping.Attributes.Property(0, Column = "GDC_DATA_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_USUARIO_APROVACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioAprovacao { get; set; }

        #endregion Propriedades da Aprovação

        #region Propriedades do Retorno da Confirmação

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetornoConfirmacaoColeta", Column = "GDC_DATA_RETORNO_CONFIRMACAO_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetornoConfirmacaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ErroRetornoConfirmacaoColeta", Column = "GDC_ERRO_RETORNO_CONFIRMACAO_COLETA", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string ErroRetornoConfirmacaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdExternoRetornoConfirmacaoColeta", Column = "GDC_IDEXTERNO_RETORNO_CONFIRMACAO_COLETA", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string IdExternoRetornoConfirmacaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OperacaoRetornoConfirmacaoColeta", Column = "GDC_OPERACAO_RETORNO_CONFIRMACAO_COLETA", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string OperacaoRetornoConfirmacaoColeta { get; set; }

        #endregion Propriedades da Aprovação

        #region Métodos Públicos

        #endregion Métodos Públicos
    }
}
