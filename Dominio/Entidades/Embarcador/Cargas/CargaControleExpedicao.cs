using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CONTROLE_EXPEDICAO", EntityName = "CargaControleExpedicao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao", NameType = typeof(CargaControleExpedicao))]
    public class CargaControleExpedicao : EntidadeCargaBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoPatio.FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false,  Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramadaParaCarregamento", Column = "CCX_DATA_PROGRAMADA_PARA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime DataProgramadaParaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCargaControleExpedicao", Column = "CCX_SITUACAO_CARGA_EXPEDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaControleExpedicao), NotNull = true)]
        public virtual SituacaoCargaControleExpedicao SituacaoCargaControleExpedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "CCX_PLACA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Doca", Column = "CCX_DOCA", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Doca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaExpedicaoLiberada", Column = "CCX_ETAPA_EXPEDICAO_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaExpedicaoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioCarregamento", Column = "CJC_DATA_INICIO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioCarregamento { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalCarregamento", Column = "CJC_DATA_FINAL_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? DataFinalCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConfirmacao", Column = "CCX_DATA_CONFIRMACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataConfirmacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AutorizadoProdutosFaltantes", Column = "CCX_AUTORIZADO_PRODUTOS_FALTANTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AutorizadoProdutosFaltantes { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CCX_PREVISTO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? InicioPrevisto { get; set; }

        public virtual Filiais.Filial Filial
        {
            get { return FluxoGestaoPatio != null ? FluxoGestaoPatio.Filial : Carga?.Filial;  }
        }

        public virtual string DescricaoSituacao
        {
            get { return SituacaoCargaControleExpedicao.ObterDescricao(); }
        }

        public virtual string Descricao
        {
            get
            {
                List<string> descricao = new List<string>();
                if (this.Carga != null)
                    descricao.Add(this.Carga.CodigoCargaEmbarcador);
                if (!string.IsNullOrWhiteSpace(this.Doca))
                    descricao.Add(this.Doca);
                return String.Join(" - ", descricao);
            }
        }
    }
}
