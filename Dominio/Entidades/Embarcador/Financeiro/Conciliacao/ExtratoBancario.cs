using System;

namespace Dominio.Entidades.Embarcador.Financeiro.Conciliacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EXTRATO_BANCARIO", EntityName = "ExtratoBancario", Name = "Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario", NameType = typeof(ExtratoBancario))]
    public class ExtratoBancario : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EXB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMovimento", Column = "EXB_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracaoMovimento", Column = "EXB_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracaoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "EXB_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "EXB_DOCUMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoMovimento", Column = "EXB_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento TipoDocumentoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "EXB_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Colaborador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoGeracaoMovimento", Column = "EXB_TIPO_GERACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento TipoGeracaoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExtratoConcolidado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DebitoCredito", Column = "EXB_DEBITO_CREDITO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito DebitoCredito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExtratoBancarioTipoLancamento", Column = "ETP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ExtratoBancarioTipoLancamento ExtratoBancarioTipoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoLancamento", Column = "EXB_CODIGO_LANCAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoLancamento { get; set; }

        public virtual string DescricaoDebitoCredito
        {
            get
            {
                switch (this.DebitoCredito)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito:
                        return "Débito";
                    case ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito:
                        return "Crédito";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoGeracaoMovimento
        {
            get
            {
                switch (this.TipoGeracaoMovimento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Automatica:
                        return "Automática";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Manual:
                        return "Manual";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoDocumentoMovimento
        {
            get
            {
                switch (this.TipoDocumentoMovimento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe:
                        return "Documento Emitido";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual:
                        return "Manual";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada:
                        return "Nota de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida:
                        return "Nota de Saída";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros:
                        return "Outros";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento:
                        return "Pagamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento:
                        return "Recebimento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recibo:
                        return "Recibo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Faturamento:
                        return "Faturamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto:
                        return "Acerto de Viagem";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete:
                        return "Contrato de Frete";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.AdiantamentoMotorista:
                        return "Adiantamento Motorista";
                    default:
                        return "";
                }
            }
        }
        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(ExtratoBancario other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
