using System;

namespace Dominio.Entidades.Embarcador.Usuarios.Colaborador
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLABORADOR_SITUACAO", EntityName = "ColaboradorSituacao", Name = "Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao", NameType = typeof(ColaboradorSituacao))]
    public class ColaboradorSituacao : EntidadeBase, IEquatable<ColaboradorSituacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSI_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CSI_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoColaborador", Column = "CSI_SITUACAO_COLABORADOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador SituacaoColaborador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSI_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cores", Column = "CSI_COR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Cores), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Cores Cores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSI_CODIGO_CONTABIL", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        public virtual string DescricaoCor
        {
            get
            {
                switch (this.Cores)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Amarelo:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Azul:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Branco:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Cinza:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Laranja:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Laranja;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Verde:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.VerdeEscuro:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.VerdeEscuro;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Vermelho:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
                    default:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco;
                }
            }
        }

        public virtual bool Equals(ColaboradorSituacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
