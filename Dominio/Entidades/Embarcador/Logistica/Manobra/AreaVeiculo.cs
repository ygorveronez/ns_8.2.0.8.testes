using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Class(0, Table = "T_AREA_VEICULO", EntityName = "AreaVeiculo", Name = "Dominio.Entidades.Embarcador.Logistica.AreaVeiculo", NameType = typeof(AreaVeiculo))]
    public class AreaVeiculo : EntidadeBase, IEquatable<AreaVeiculo>
    {
        [Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARV_CODIGO")]
        [Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Property(0, Name = "Descricao", Column = "ARV_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [Property(0, Name = "Ativo", Column = "ARV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [Property(0, Name = "Observacao", Column = "ARV_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [Property(0, Name = "QRCode", Column = "ARV_QR_CODE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string QRCode { get; set; }

        [Property(0, Name = "Tipo", Column = "ARV_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAreaVeiculo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAreaVeiculo Tipo { get; set; }

        [ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [Bag(0, Name = "Posicoes", Cascade = "none", Lazy = CollectionLazy.True, Table = "T_AREA_VEICULO_POSICAO")]
        [Key(1, Column = "ARV_CODIGO")]
        [ManyToMany(2, Class = "AreaVeiculoPosicao", Column = "AVP_CODIGO")]
        public virtual ICollection<AreaVeiculoPosicao> Posicoes { get; set; }

        [Set(0, Name = "TiposRetornoCarga", Cascade = "all", Lazy = CollectionLazy.True, Table = "T_AREA_VEICULO_TIPO_RETORNO_CARGA")]
        [Key(1, Column = "ARV_CODIGO")]
        [ManyToMany(2, Class = "TipoRetornoCarga", Column = "TPR_CODIGO")]
        public virtual ICollection<Cargas.Retornos.TipoRetornoCarga> TiposRetornoCarga { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(AreaVeiculo other)
        {
            return other.Codigo == Codigo;
        }
    }
}
