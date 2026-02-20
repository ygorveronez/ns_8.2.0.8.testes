namespace Dominio.Entidades.Embarcador.Frota.Programacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROGRAMACAO_SITUACAO", EntityName = "ProgramacaoSituacao", Name = "Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao", NameType = typeof(ProgramacaoSituacao))]
    public class ProgramacaoSituacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSI_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEntidadeProgramacao", Column = "PSI_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao TipoEntidadeProgramacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cores", Column = "PSI_COR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Cores), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Cores Cores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSI_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string DescricaoCor
        {
            get
            {
                switch (this.Cores)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Amarelo:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Warning(ObjetosDeValor.Embarcador.Enumeradores.IntensidadeCor._100);
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Azul:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Info(ObjetosDeValor.Embarcador.Enumeradores.IntensidadeCor._100);
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Branco:
                        return string.Empty;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Cinza:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Secondary(ObjetosDeValor.Embarcador.Enumeradores.IntensidadeCor._100); ;
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Laranja:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Warning(ObjetosDeValor.Embarcador.Enumeradores.IntensidadeCor._900);
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Verde:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Sucess(ObjetosDeValor.Embarcador.Enumeradores.IntensidadeCor._100);
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.VerdeEscuro:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Sucess(ObjetosDeValor.Embarcador.Enumeradores.IntensidadeCor._900);
                    case ObjetosDeValor.Embarcador.Enumeradores.Cores.Vermelho:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Danger(ObjetosDeValor.Embarcador.Enumeradores.IntensidadeCor._100);
                    default:
                        return string.Empty;
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
