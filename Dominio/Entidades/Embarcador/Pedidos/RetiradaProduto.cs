using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RETIRADA_PRODUTO", EntityName = "RetiradaProduto", Name = "Dominio.Entidades.Embarcador.Produtos.RetiradaProduto", NameType = typeof(RetiradaProduto))]
    public class RetiradaProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculo", Column = "RP_PLACA_VEICULO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string PlacaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeTransportadora", Column = "RP_NOME_TRANSPORTADORA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMotorista", Column = "RP_NOME_MOTORISTA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CpfMotorista", Column = "RP_CPF_MOTORISTA", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string CpfMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeVeiculo", Column = "RP_CAPACIDADE_VEICULO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal CapacidadeVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotal", Column = "RP_PESO_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotal { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EspacoDisponivel", Column = "RP_ESPACO_DISPONIVEL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal EspacoDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ocupacao", Column = "RP_OCUPACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Ocupacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetirada", Column = "RP_DATA_RETIRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRetirada { get; set; }







        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RETIRADA_PRODUTO_PEDIDOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pedido", Column = "PED_CODIGO")]
        public virtual ICollection<Pedido> Pedidos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Retirada de Produto - {Filial.Descricao}";
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}

