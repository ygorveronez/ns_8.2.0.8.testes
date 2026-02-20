using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTO", EntityName = "MovimentoDoFinanceiro", Name = "Dominio.Entidades.MovimentoDoFinanceiro", NameType = typeof(MovimentoDoFinanceiro))]
    public class MovimentoDoFinanceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "PLA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoDeConta PlanoDeConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoDeViagem", Column = "ACE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AcertoDeViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "MOV_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "MOV_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaixa", Column = "MOV_DATA_BAIXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "MOV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "MOV_DOCUMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MOV_OBS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MOV_TIPO", TypeType = typeof(Enumeradores.TipoMovimento), NotNull = false)]
        public virtual Enumeradores.TipoMovimento? Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "MOV_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ParcelasCTe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_DUPLICATA_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ParcelaCobrancaCTe", Column = "CPA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ParcelaCobrancaCTe> ParcelasCTe { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Abastecimentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_ABASTECIMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Abastecimento", Column = "ABA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Abastecimento> Abastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Despesas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_DESPESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DespesaDoAcertoDeViagem", Column = "ACD_CODIGO")]
        public virtual ICollection<Dominio.Entidades.DespesaDoAcertoDeViagem> Despesas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "HistoricosVeiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_HIST_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "HistoricoVeiculo", Column = "HIS_CODIGO")]
        public virtual ICollection<Dominio.Entidades.HistoricoVeiculo> HistoricosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ParcelaDocumentoEntrada", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_DUPLICATA_DOCUMENTO_ENTRADA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ParcelaDocumentoEntrada", Column = "DED_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ParcelaDocumentoEntrada> ParcelaDocumentoEntrada { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "Duplicata", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_DUPLICATA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Duplicata", Column = "DUP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Duplicata> Duplicata { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DuplicataParcelas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_DUPLICATA_PARCELAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DuplicataParcelas", Column = "DPA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.DuplicataParcelas> DuplicataParcelas { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "CIOTs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_CIOT")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CIOTSigaFacil", Column = "SFC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.CIOTSigaFacil> CIOTs { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PagamentosMotoristasCTe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_PGTO_MOTORISTA_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PagamentoMotorista", Column = "PMO_CODIGO")]
        public virtual ICollection<Dominio.Entidades.PagamentoMotorista> PagamentosMotoristasCTe { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PagamentosMotoristasMDFe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_PGTO_MOTORISTA_MDFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PagamentoMotoristaMDFe", Column = "PMM_CODIGO")]
        public virtual ICollection<Dominio.Entidades.PagamentoMotoristaMDFe> PagamentosMotoristasMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DuplicataDesconto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_DUPLICATA_DESCONTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Duplicata", Column = "DUP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Duplicata> DuplicataDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DuplicataAcrescimo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_DUPLICATA_ACRESCIMO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Duplicata", Column = "DUP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Duplicata> DuplicataAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DuplicataBaixaCte", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_MOVIMENTO_DUPLICATA_BAIXA_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Duplicata", Column = "DUP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Duplicata> DuplicataBaixaCte { get; set; }

        public virtual int CodigoPlanoConta
        {
            get
            {
                return this.PlanoDeConta.Codigo;
            }
        }

        public virtual string DescricaoPlanoConta
        {
            get
            {
                return this.PlanoDeConta.Descricao;
            }
        }

        public virtual string ContaPlanoConta
        {
            get
            {
                return this.PlanoDeConta.Conta;
            }
        }

        public virtual string CPFCNPJPessoa
        {
            get
            {
                return this.Pessoa != null ? this.Pessoa.CPF_CNPJ_Formatado : string.Empty;
            }
        }

        public virtual string NomePessoa
        {
            get
            {
                return this.Pessoa != null ? this.Pessoa.Nome : string.Empty;
            }
        }
    }
}
