using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_PEDAGIO", EntityName = "FechamentoPedagio", Name = "Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio", NameType = typeof(FechamentoPedagio))]
    public class FechamentoPedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "FPE_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "FPE_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "FPE_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio Situacao { get; set; }

        public virtual bool Equals(FechamentoPedagio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.FalhaNaGeracao:
                        return "Falha Na Geração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.EmGeracao:
                        return "Em Geração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPedagio.AgGeracao:
                        return "Aguardado Geração";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                List<string> descricao = new List<string>();

                if (this.Veiculo != null)
                    descricao.Add(this.Veiculo.Placa);

                if (this.DataInicio.HasValue)
                    descricao.Add(this.DataInicio.Value.ToString("dd/MM/yyyy"));

                if (this.DataFim.HasValue)
                    descricao.Add(this.DataFim.Value.ToString("dd/MM/yyyy"));

                return String.Join(" - ", descricao);
            }
        }
    }
}