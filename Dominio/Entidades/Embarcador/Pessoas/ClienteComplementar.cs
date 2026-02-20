using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_COMPLEMENTAR", EntityName = "ClienteComplementar", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar", NameType = typeof(ClienteComplementar))]
    public class ClienteComplementar : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMatriz", Column = "CLC_CODIGO_MATRIZ", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoMatriz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Matriz", Column = "CLC_MATRIZ", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Matriz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EquipeVendas", Column = "CLC_EQUIPE_VENDAS", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string EquipeVendas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EscritorioVendas", Column = "CLC_ESCRITORIO_VENDAS", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string EscritorioVendas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CanalDistribuicao", Column = "CLC_CANAL_DISTRIBUICAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CanalDistribuicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Abordagem", Column = "CLC_ABORDAGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Abordagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClienteCD", Column = "CLC_CLIENTE_CD", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ClienteCD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SegundaRemessa", Column = "CLC_SEGUNDA_REMESSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SegundaRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExclusividadeEntrega", Column = "CLC_EXCLUSIVIDADE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ExclusividadeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Paletizacao", Column = "CLC_PALETIZACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Paletizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClienteStrechado", Column = "CLC_CLIENTE_STRECHADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ClienteStrechado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agendamento", Column = "CLC_AGENDAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Agendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClienteComMulta", Column = "CLC_CLIENTE_COM_MULTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ClienteComMulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EquipeVendasFP", Column = "CLC_EQUIPE_VENDAS_FP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string EquipeVendasFP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EscritorioVendasFP", Column = "CLC_ESCRITORIO_VENDAS_FP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string EscritorioVendasFP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MatrizReferencia", Column = "CLC_MATRIZ_REFERENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string MatrizReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoTipoVeiculo", Column = "CLC_DESCRICAO_TIPO_VEICULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoTipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParticionamentoVeiculo", Column = "CLC_PARTICIONAMENTO_VEICULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ParticionamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoParticionamentoVeiculo", Column = "CLC_DESCRICAO_PARTICIONAMENTO_VEICULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoParticionamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentoDescarga", Column = "CLC_PAGAMENTO_DESCARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PagamentoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoPagamentoDescarga", Column = "CLC_DESCRICAO_PAGAMENTO_DESCARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoPagamentoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlturaRecebimento", Column = "CLC_ALTURA_RECEBIMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string AlturaRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoAlturaRecebimento", Column = "CLC_DESCRICAO_ALTURA_RECEBIMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoAlturaRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RestricaoCarregamento", Column = "CLC_RESTRICAO_CARREGAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string RestricaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoRestricaoCarregamento", Column = "CLC_DESCRICAO_RESTRICAO_CARREGAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoRestricaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComposicaoPalete", Column = "CLC_COMPOSICAO_PALETE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ComposicaoPalete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoComposicaoPalete", Column = "CLC_DESCRICAO_COMPOSICAO_PALETE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoComposicaoPalete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SegundaFeira", Column = "CLC_SEGUNDA_FEIRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SegundaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TercaFeira", Column = "CLC_TERCA_FEIRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TercaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuartaFeira", Column = "CLC_QUARTA_FEIRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string QuartaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuintaFeira", Column = "CLC_QUINTA_FEIRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string QuintaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SextaFeira", Column = "CLC_SEXTA_FEIRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SextaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sabado", Column = "CLC_SABADO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Sabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Domingo", Column = "CLC_DOMINGO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Domingo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeRecebimento", Column = "CLC_CAPACIDADE_RECEBIMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CapacidadeRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoDescarga", Column = "CLC_CUSTO_DESCARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal CustoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCusto", Column = "CLC_TIPO_CUSTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ajudantes", Column = "CLC_AJUDANTES", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Ajudantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraRecebimentoCliente", Column = "CLC_HORA_RECEBIMENTO_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string HoraRecebimentoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPallet", Column = "CLC_REGRA_PALLET", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraPallet), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraPallet RegraPallet { get; set; }

        public virtual bool Equals(ClienteComplementar other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get { return Cliente.Nome; }
        }
    }
}
