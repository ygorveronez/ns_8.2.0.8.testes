using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_TRANSPORTADOR_FRETE", EntityName = "ContratoTransporteFrete", Name = "<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete", NameType = typeof(ContratoTransporteFrete))]
    public class ContratoTransporteFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContrato", Column = "CTF_NUMERO_CONTRATO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "ContratoExternoID", Column = "CTF_CONTRATO_EXTERNO_ID", TypeType = typeof(int), NotNull = false)]
        public virtual int ContratoExternoID { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "NomeContrato", Column = "CTF_NOME_CONTRATO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NomeContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0,Name = "AprovacaoAdicionalRequerida", Column = "CTF_APROVACAO_ADICIONAL_REQUERIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AprovacaoAdicionalRequerida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pais Pais { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConformidadeComRSP", Column = "CTF_CONFORMIDADE_COM_RSP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConformidadeComRSP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "CTF_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "CTF_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimaData", Column = "CTF_ULTIMA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? UltimaData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPrevistoContrato", Column = "CTF_VALOR_PREVISTO_CONTRATO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPrevistoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoValorPrevistoContrato", Column = "CTF_ULTIMO_VALOR_PREVISTO_CONTRATO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal UltimoValorPrevistoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClausulaPenal", Column = "CTF_CLAUSULA_PENAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ClausulaPenal { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Observacao", Column = "CTF_OBSERVACAO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CTF_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Moeda", Column = "CTF_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Network", Column = "CTF_NETWORK", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.NetworkContratoTransporte), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.NetworkContratoTransporte Network { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Equipe", Column = "CTF_EQUIPE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EquipeContratoTransporte), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EquipeContratoTransporte Equipe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Categoria", Column = "CTF_CATEGORIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaContratoTransporte), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaContratoTransporte Categoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubCategoria", Column = "CTF_SUB_CATEGORIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SubCategoriaContratoTransporte), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SubCategoriaContratoTransporte SubCategoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModoContrato", Column = "CTF_MODO_CONTRATO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoContratoTransporte), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoContratoTransporte ModoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PessoaJuridica", Column = "CTF_PESSOA_JURIDICA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PessoaJuridicaContratoTransporte), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PessoaJuridicaContratoTransporte PessoaJuridica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContrato", Column = "CTF_TIPO_CONTRATO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoTransporte), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoTransporte TipoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProcessoAprovacao", Column = "CTF_PROCESSO_APROVACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessoAprovacaoContratoTransporte), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessoAprovacaoContratoTransporte ProcessoAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cluster", Column = "CTF_CLUSTER", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.Cluster), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.Cluster Cluster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HubNonHub", Column = "CTF_HUB_NON_HUB", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.HubNonHub), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.HubNonHub HubNonHub { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DominioOTM", Column = "CTF_DOMINIO_OTM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DominioOTM), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DominioOTM DominioOTM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Padrao", Column = "CTF_PADRAO", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Padrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusAprovacaoTransportador", Column = "CTF_STATUS_APROVACAO_TRANSPORTADOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAprovacaoTransportador), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAprovacaoTransportador StatusAprovacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermosPagamento", Column = "TPG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TermosPagamento TermosPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "StatusAssinaturaContrato", Column = "STC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato StatusAssinaturaContrato { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_TRANSPORTE_FRETE_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoTransporteFreteAnexo", Column = "ANX_CODIGO")]
        public virtual IList<ContratoTransporteFreteAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoContratoClonado", Column = "CTF_CONTRATO_CLONADO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoContratoClonado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Clonado", Column = "CTF_CLONADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Clonado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContratoSequencial", Column = "CTF_NUMERO_CONTRATO_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroContratoSequencial { get; set; }

        [Obsolete("Campo transferido para ContratoFreteTransportador")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "EstruturaTabela", Column = "CTF_ESTRUTURA_TABELA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela EstruturaTabela { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NomeContrato;
            }
        }

        public virtual Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete)this.MemberwiseClone();
        }

        public virtual bool Equals(ContratoTransporteFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
