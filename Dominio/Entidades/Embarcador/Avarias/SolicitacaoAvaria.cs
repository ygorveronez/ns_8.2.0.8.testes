using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_AVARIA", EntityName = "SolicitacaoAvaria", Name = "Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria", NameType = typeof(SolicitacaoAvaria))]
    public class SolicitacaoAvaria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_NUMERO_AVARIA", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_SITUACAO_FLUXO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria SituacaoFluxo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_DATA_AVARIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_DATA_SOLICITACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAvaria", Column = "MAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.MotivoAvaria MotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoDescontoAvaria", Column = "MDA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.MotivoDescontoAvaria MotivoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_MOTORISTA_MODIFICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MotoristaModificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_RG_MOTORISTA_MODIFICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RGMotoristaModificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_MOTORISTA_ORIGINAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MotoristaOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_RG_MOTORISTA_ORIGINAL", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string RGMotoristaOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_MOTORISTA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_RG_MOTORISTA", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string RGMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_CPF_MOTORISTA", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CPFMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_JUSTIFICATIVA", TypeType = typeof(string), Length = 1500, NotNull = false)]
        public virtual string Justificativa { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_DATA_SOLICITACAO_AVARIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ProdutosAvariados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTOS_AVARIADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutosAvariados", Column = "PAV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> ProdutosAvariados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "SolicitacaoAvariaAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SOLICITACAO_AVARIA_AUTORIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SolicitacaoAvariaAutorizacao", Column = "SAA_CODIGO")]
        public virtual ICollection<SolicitacaoAvariaAutorizacao> SolicitacaoAvariaAutorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Lote", Column = "LAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.Lote Lote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixaAgrupadoDocumento", Column = "TBD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento TituloBaixaAgrupadoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }


        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsaveis", Formula = @"SUBSTRING((SELECT ', ' + funcionario.FUN_NOME
                                                                                    FROM T_REPONSAVEL_AVARIA responsavel
                                                                                    inner join T_FUNCIONARIO funcionario ON funcionario.FUN_CODIGO = responsavel.FUN_CODIGO
                                                                                    WHERE responsavel.SAV_CODIGO = SAV_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string Responsaveis { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroAvaria.ToString();
            }
        }


        /*[NHibernate.Mapping.Attributes.Property(0, Name = "DataAprovacao", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + funcionario.FUN_NOME
                                                                                    FROM T_REPONSAVEL_AVARIA responsavel
                                                                                    inner join T_FUNCIONARIO funcionario ON funcionario.FUN_CODIGO = responsavel.FUN_CODIGO
                                                                                    WHERE responsavel.SAV_CODIGO = SAV_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime DataAprovacao { get; set; }*/

        public virtual decimal ValorAvaria
        {
            get
            {
                return (ProdutosAvariados.Count > 0) ? (from obj in ProdutosAvariados where obj.RemovidoLote == false select obj).Sum(o => o.ValorAvaria) : 0;
            }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual string DescricaoSituacaoFluxo
        {
            get { return SituacaoFluxo.ObterDescricao(); }
        }

        #endregion
    }
}
