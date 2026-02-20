using System;
using System.Collections.Generic;


namespace Dominio.Entidades.Embarcador.ProdutorRural
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_COLETA_PRODUTOR", EntityName = "FechamentoColetaProdutor", Name = "Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor", NameType = typeof(FechamentoColetaProdutor))]
    public class FechamentoColetaProdutor : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "FCP_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "FCP_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCP_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "FCP_TIPO_TOMADOR", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCP_SITUACAO_NO_CANCELAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor SituacaoNoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCancelamento", Column = "FCP_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidosFechamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FECHAMENTO_COLETA_PEDIDO_COLETA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FCP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FechamentoColetaProdutorPedidos", Column = "FCC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos> PedidosFechamento { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoTipoTomador()
        {
            if (TipoTomador == Enumeradores.TipoTomador.Remetente)
                return "Remetente";
            else if (TipoTomador == Enumeradores.TipoTomador.Destinatario)
                return "Destinatario";
            else if (TipoTomador == Enumeradores.TipoTomador.Outros)
                return "Outro";
            else if (TipoTomador == Enumeradores.TipoTomador.Recebedor)
                return "Recebedor";
            else if (TipoTomador == Enumeradores.TipoTomador.Expedidor)
                return "Expedidor";
            else
                return "";
        }

    }
}
