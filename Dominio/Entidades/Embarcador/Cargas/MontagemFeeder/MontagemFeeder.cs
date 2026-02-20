using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemFeeder
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONTAGEM_FEEDER", EntityName = "MontagemFeeder", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder", NameType = typeof(MontagemFeeder))]
    public class MontagemFeeder : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Planilha", Column = "MOF_PLANILHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Planilha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoPlanilha", Column = "MOF_CAMINHO_PLANILHA", Type = "StringClob", NotNull = false)]
        public virtual string CaminhoPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLinhas", Column = "MOF_QTDE_LINHAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeLinhas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "MOF_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "MOF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "MOF_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioProcessamento", Column = "MOF_DATA_INICIO_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimProcessamento", Column = "MOF_DATA_FIM_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalSegundosProcessamento", Column = "MOF_TOTAL_SEGUNDOS_PROCESSAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? TotalSegundosProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPlanilhaFeeder", Column = "MOF_TIPO_PLANILHA_FEEDER", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPlanilhaFeeder), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPlanilhaFeeder TipoPlanilhaFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportarMesmoSemCTeAbsorvidoAnteriormente", Column = "MOF_IMPORTAR_MESMO_SEM_CTE_ABSORVIDO_ANTERIORMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarMesmoSemCTeAbsorvidoAnteriormente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportarMesmoComDocumentacaoDuplicada", Column = "MOF_IMPORTAR_MESMO_COM_DOCUMENTACAO_DUPLICADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarMesmoComDocumentacaoDuplicada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmbarqueAfretamento", Column = "MOF_EMBARQUE_AFRETADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmbarqueAfretamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJCPFDestinatario", Column = "MOF_CNPJ_CPF_DESTINATARIO", TypeType = typeof(double), NotNull = false)]  
        public virtual double CNPJCPFDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJCPFExpedidor", Column = "MOF_CNPJ_CPF_EXPEDIDOR", TypeType = typeof(double), NotNull = false)]
        public virtual double CNPJCPFExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJCPFTomador", Column = "MOF_CNPJ_CPF_TOMADOR", TypeType = typeof(double), NotNull = false)]
        public virtual double CNPJCPFTomador  { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino{ get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Bookings", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONTAGEM_FEEDER_BOOKING")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MFB_BOOKING", TypeType = typeof(string), NotNull = true, Length = 80)]
        public virtual ICollection<string> Bookings { get; set; }

        public virtual TimeSpan? Tempo()
        {
            if (TotalSegundosProcessamento != null)
                return TimeSpan.FromSeconds(TotalSegundosProcessamento.Value);
            else
                return null;
        }

        public virtual string Descricao
        {
            get
            {
                return Planilha;
            }
        }

    }
}
