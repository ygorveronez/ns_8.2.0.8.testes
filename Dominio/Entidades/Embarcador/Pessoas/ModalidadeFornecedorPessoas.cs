using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES", EntityName = "ModalidadeFornecedorPessoas", Name = "Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas", NameType = typeof(ModalidadeFornecedorPessoas))]
    public class ModalidadeFornecedorPessoas : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>
    {
        public ModalidadeFornecedorPessoas() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalidadePessoas", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas ModalidadePessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_POSTO_CONVENIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PostoConveniado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_PAGO_POR_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PagoPorFatura { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "MOF_COMPARTILHAR_ACESSO_ENTRE_GRUPO_PESSOAS", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool CompartilharAcessoEntreGrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_PERMITE_DOWNLOAD_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteDownloadDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_ENVIAR_EMAIL_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailFornecedorDadosTransporte { get; set; }

        /// <summary>
        /// Existe uma opção para o app no TipoOperacao que obriga que seja informado NF-es durante a coleta.
        /// Essa opção evita que seja necessário que o motorista informe as NF-es para esse cliente em específico.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_NAO_E_OBRIGATORIO_INFORMAR_NFE_NA_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEObrigatorioInformarNfeNaColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_OFICINA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Oficina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_TEXTO_AVISO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string TextoAviso { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoOperacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual IList<Pedidos.TipoOperacao> TipoOperacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_EMPRESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual IList<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoCargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual IList<Cargas.TipoDeCarga> TipoCargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ModelosVeicular", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual IList<Cargas.ModeloVeicularCarga> ModelosVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Destinatarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual IList<Cliente> Destinatarios { get; set; }

        //[NHibernate.Mapping.Attributes.Bag(0, Name = "RestricaoModelosVeicular", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_RESTRICAO_MODELO_VEICULAR")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "MFR_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        //public virtual IList<Cargas.ModeloVeicularCarga> RestricaoModelosVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_TIPO_OFICINA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOficina), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOficina? TipoOficina { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_OFICINA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaOficina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_PERMITIR_MULTIPLOS_VENCIMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirMultiplosVencimentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TabelasValores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_TABELA_VALORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PostoCombustivelTabelaValores", Column = "MOT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores> TabelasValores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_FORNECEDOR_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PessoaFornecedorAnexo", Column = "ANX_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pessoas.PessoaFornecedorAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_OBRIGAR_LOCAL_ARMAZENAMENTO_NO_LANCAMENTO_DE_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOF_GERAR_AGENDAMENTO_SOMENTE_PEDIDOS_EXISTENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAgendamentoSomentePedidosExistentes { get; set; }

        public virtual string Descricao
        {
            get { return this.ModalidadePessoas?.DescricaoTipoModalidade ?? string.Empty; }
        }

        public virtual string TextoAvisoHTML
        {
            get { return (TextoAviso ?? string.Empty).Replace("\n", "<br />"); }
        }

        public virtual bool Equals(ModalidadeFornecedorPessoas other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}