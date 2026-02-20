using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.PedidoVenda
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VENDA_DIRETA", EntityName = "VendaDireta", Name = "Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta", NameType = typeof(VendaDireta))]
    public class VendaDireta : EntidadeBase, IEquatable<VendaDireta>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "VED_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "VED_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacao", Column = "VED_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "VED_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "VED_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VED_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCobranca", Column = "VED_TIPO_COBRANCA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCobranca), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCobranca TipoCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCobrancaVendaDireta", Column = "VED_TIPO_COBRANCA_VENDA_DIRETA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaVendaDireta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaVendaDireta TipoCobrancaVendaDireta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_AGENDADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Agendador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgendamento", Column = "VED_DATA_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidacao", Column = "VED_DATA_VALIDACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoCertificado", Column = "VED_DATA_VENCIMENTO_CERTIFICADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoCobranca", Column = "VED_DATA_VENCIMENTO_COBRANCA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarNF", Column = "VED_GERAR_NF", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedido", Column = "VED_NUMERO_PEDIDO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAssinatura", Column = "VED_TIPO_ASSINATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAssinaturaVendaDireta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAssinaturaVendaDireta TipoAssinatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusPedido", Column = "VED_STATUS_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta StatusPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmissao1", Column = "VED_CODIGO_EMISSAO_1", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoEmissao1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmissao2", Column = "VED_CODIGO_EMISSAO_2", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoEmissao2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoServico", Column = "VED_PRODUTO_SERVICO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico ProdutoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTreinamento", Column = "VED_DATA_TREINAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTreinamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_TREINAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioTreinamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusCadastro", Column = "VED_STATUS_CADASTRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusCadastro), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusCadastro StatusCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoClienteVendaDireta", Column = "VED_TIPO_CLIENTE_VENDA_DIRETA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoClienteVendaDireta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoClienteVendaDireta TipoClienteVendaDireta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitidoDocumentos", Column = "VED_EMITIDO_DOCUMENTOS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SimNao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SimNao EmitidoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pendencia", Column = "VED_PENDENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SimNao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SimNao Pendencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Certificado", Column = "VED_CERTIFICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Certificado { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CONTESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioContestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataContestacao", Column = "VED_DATA_CONTESTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataContestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoContestacao", Column = "VED_OBSERVACAO_CONTESTACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoContestacao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgendadoFora", Column = "VED_DATA_AGENDADO_FORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendadoFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAprovado", Column = "VED_DATA_APROVADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaixado", Column = "VED_DATA_BAIXADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaixado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFaltaAgendar", Column = "VED_DATA_FALTA_AGENDAR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFaltaAgendar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgendado", Column = "VED_DATA_AGENDADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataContato1", Column = "VED_DATA_CONTATO_1", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataContato1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataContato2", Column = "VED_DATA_CONTATO_2", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataContato2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataContato3", Column = "VED_DATA_CONTATO_3", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataContato3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProblema", Column = "VED_DATA_PROBLEMA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProblema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataReagendar", Column = "VED_DATA_REAGENDAR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReagendar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataClienteBaixa", Column = "VED_DATA_CLIENTE_BAIXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataClienteBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAguardandoVerificacao", Column = "VED_DATA_AGUARDANDO_VERIFICACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAguardandoVerificacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_VALIDADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioValidador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Itens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VENDA_DIRETA_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VendaDiretaItem", Column = "VDI_CODIGO")]
        public virtual IList<VendaDiretaItem> Itens { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Parcelas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VENDA_DIRETA_PARCELA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VendaDiretaParcela", Column = "VDP_CODIGO")]
        public virtual IList<VendaDiretaParcela> Parcelas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VENDA_DIRETA_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VendaDiretaAnexo", Column = "VDA_CODIGO")]
        public virtual IList<VendaDiretaAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AnexosContestacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VENDA_DIRETA_CONTESTACAO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VendaDiretaContestacaoAnexo", Column = "VDX_CODIGO")]
        public virtual IList<VendaDiretaContestacaoAnexo> AnexosContestacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Cliente?.Descricao ?? string.Empty;
            }
        }

        public virtual decimal ValorTotal
        {
            get
            {
                decimal valorTotal = 0;

                if (this.Itens != null)
                    valorTotal = (from o in Itens.ToList() select o.ValorTotal).Sum();

                return valorTotal;
            }
        }

        public virtual bool Equals(VendaDireta other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
