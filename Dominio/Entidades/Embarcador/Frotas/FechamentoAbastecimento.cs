using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_ABASTECIMENTO", EntityName = "FechamentoAbastecimento", Name = "Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento", NameType = typeof(FechamentoAbastecimento))]
    public class FechamentoAbastecimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Posto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "FAB_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "FAB_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "FAB_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculos.Equipamento Equipamento { get; set; }

        public virtual bool Equals(FechamentoAbastecimento other)
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
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Cancelada:
                        return "Cancelada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.FalhaNaGeracao:
                        return "Falha Na Geração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.EmGeracao:
                        return "Em Geração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.AgGeracao:
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