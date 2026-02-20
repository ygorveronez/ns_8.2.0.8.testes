using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Filiais;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_PALLET", EntityName = "AgendamentoPallet", Name = "Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet", NameType = typeof(AgendamentoPallet))]
    public class AgendamentoPallet : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        public AgendamentoPallet()
        {
            DataCriacao = DateTime.Now;
        }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "REM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACP_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACP_QUANTIDADE_PALLETS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadePallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACP_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        /// <summary>
        /// Senha sequencial para compor a propriedade Senha.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "ACP_SENHA_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int? SenhaSequencial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "DES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_VEICULO_SELECIONADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo VeiculoSelecionado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario ResponsavelConfirmacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_SELECIONADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario MotoristaSelecionado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "ACP_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "ACP_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgendamento", Column = "ACP_DATA_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAgendamentoPallet", Column = "ACP_ETAPA", TypeType = typeof(EtapaAgendamentoPallet), NotNull = true)]
        public virtual EtapaAgendamentoPallet EtapaAgendamentoPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ACP_SITUACAO", TypeType = typeof(SituacaoAgendamentoPallet), NotNull = false)]
        public virtual SituacaoAgendamentoPallet? Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ResponsavelPallet", Column = "ACP_RESPONSAVEL_PALLET", TypeType = typeof(ResponsavelPallet), NotNull = false)]
        public virtual ResponsavelPallet ResponsavelPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "ACP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ACP_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 300)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_SOLICITANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Solicitante { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string Descricao => $"Agendamento Pallet {Codigo}";

        public virtual string DescricaoSituacao => Situacao?.ObterDescricao() ?? string.Empty;

        public virtual string DescricaoEtapa => EtapaAgendamentoPallet.ObterDescricao();

        #endregion Propriedades com Regras
    }
}
