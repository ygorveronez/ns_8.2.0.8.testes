using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SPED_PIS_COFINS_GERACAO", EntityName = "SpedPISCOFINS", Name = "Dominio.Entidades.Embarcador.NotaFiscal.SpedPISCOFINS", NameType = typeof(SpedPISCOFINS))]
    public class SpedPISCOFINS : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.SpedPISCOFINS>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "SPS_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "SPS_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimento", Column = "SPS_TIPO_MOVIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "SPS_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusArquivo", Column = "SPS_STATUS_ARQUIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal StatusArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComRetorno", Column = "SPS_COM_RETORNO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ComRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.DataInicial.ToString("dd/MM/yyyy") + " - " + this.DataFinal.ToString("dd/MM/yyyy") + " - " + this.DescricaoTipo;
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.TipoMovimento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal.Todos:
                        return "Todos";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal.Entrada:
                        return "Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal.Saida:
                        return "Saída";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.StatusArquivo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.AguardandoGeracao:
                        return "Aguardando Geração";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.Gerado:
                        return "Gerado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.EmProcesso:
                        return "Em Processo";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.ErroValidacao:
                        return "Erro de Validação";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(SpedPISCOFINS other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}