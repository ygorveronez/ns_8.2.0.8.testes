namespace Dominio.Entidades.Embarcador.Frota.Programacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROGRAMACAO_SITUACAO_TMS", EntityName = "ProgramacaoSituacaoTMS", Name = "Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacaoTMS", NameType = typeof(ProgramacaoSituacaoTMS))]
    public class ProgramacaoSituacaoTMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PST_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PST_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEntidadeProgramacao", Column = "PST_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao TipoEntidadeProgramacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cores", Column = "PST_COR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Cores), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Cores Cores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PST_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "PST_FINALIZADORA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Finalizadora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

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

        public virtual string DescricaoTipoEntidadeProgramacao
        {
            get
            {
                switch (this.TipoEntidadeProgramacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Veiculo:
                        return "Veículo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Motorista:
                        return "Motorista";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
