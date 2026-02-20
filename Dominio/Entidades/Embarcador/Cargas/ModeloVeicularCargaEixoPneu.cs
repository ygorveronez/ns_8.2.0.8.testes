using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODELO_VEICULAR_CARGA_EIXO_PNEU", EntityName = "ModeloVeicularCargaEixoPneu", Name = "Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu", NameType = typeof(ModeloVeicularCargaEixoPneu))]
    public class ModeloVeicularCargaEixoPneu : EntidadeBase, IEquatable<ModeloVeicularCargaEixoPneu>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "MEP_POSICAO", TypeType = typeof(PosicaoEixoPneu), NotNull = true)]
        public virtual PosicaoEixoPneu Posicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCargaEixo", Column = "MEX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCargaEixo Eixo { get; set; }

        public virtual string Descricao
        {
            get { return $"Pneu {Posicao.ObterDescricao()}"; }
        }

        public virtual bool Equals(ModeloVeicularCargaEixoPneu other)
        {
            return other.Codigo == this.Codigo;
        }

        public virtual int ObterPosicaoPneu()
        {
            switch (Posicao)
            {
                case PosicaoEixoPneu.DireitoExterno: return Eixo.QuantidadePneu == QuantidadePneuEixo.Duplo ? 8 : 7;
                case PosicaoEixoPneu.DireitoInterno: return 7;
                case PosicaoEixoPneu.EsquerdoExterno: return Eixo.QuantidadePneu == QuantidadePneuEixo.Duplo ? 1 : 2;
                case PosicaoEixoPneu.EsquerdoInterno: return 2;
                default: return 0;
            }
        }
    }
}
