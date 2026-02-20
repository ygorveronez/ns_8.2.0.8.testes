using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_CONJUNTO_VEICULO", EntityName = "FilaCarregamentoConjuntoVeiculo", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo", NameType = typeof(FilaCarregamentoConjuntoVeiculo))]
    public class FilaCarregamentoConjuntoVeiculo : EntidadeBase, IEquatable<FilaCarregamentoConjuntoVeiculo>
    {
        #region Propriedades Públicas

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "FCV_CODIGO_TRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Tracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Reboques", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FCV_CODIGO_REBOQUE")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Reboques { get; set; }

        #endregion

        #region Construtores

        public static FilaCarregamentoConjuntoVeiculo Criar(Veiculo veiculo)
        {
            return Criar(veiculo, utilizarModeloVeicularCargaPorReboque: true);
        }

        public static FilaCarregamentoConjuntoVeiculo Criar(Veiculo veiculo, bool utilizarModeloVeicularCargaPorReboque)
        {
            if (veiculo.IsTipoVeiculoTracao())
            {
                return new FilaCarregamentoConjuntoVeiculo()
                {
                    ModeloVeicularCarga = utilizarModeloVeicularCargaPorReboque ? veiculo.VeiculosVinculados?.FirstOrDefault()?.ModeloVeicularCarga ?? veiculo.ModeloVeicularCarga : veiculo.ModeloVeicularCarga,
                    Reboques = new List<Veiculo>(veiculo.VeiculosVinculados),
                    Tracao = veiculo
                };
            }

            return new FilaCarregamentoConjuntoVeiculo()
            {
                ModeloVeicularCarga = utilizarModeloVeicularCargaPorReboque ? veiculo.ModeloVeicularCarga : veiculo.VeiculosTracao?.FirstOrDefault()?.ModeloVeicularCarga ?? veiculo.ModeloVeicularCarga,
                Reboques = new List<Veiculo>() { veiculo },
                Tracao = veiculo.VeiculosTracao?.FirstOrDefault() ?? null
            };
        }

        public static FilaCarregamentoConjuntoVeiculo Criar(FilaCarregamentoConjuntoVeiculo conjuntoVeiculo)
        {
            return new FilaCarregamentoConjuntoVeiculo()
            {
                ModeloVeicularCarga = conjuntoVeiculo.ModeloVeicularCarga,
                Reboques = conjuntoVeiculo.Reboques.ToList(),
                Tracao = conjuntoVeiculo.Tracao
            };
        }

        #endregion

        #region Métodos Públicos

        public virtual bool IsCompleto()
        {
            if (Tracao == null)
                return false;

            if (ModeloVeicularCarga.NumeroReboques == 0)
                return true;

            return (Reboques?.Count > 0);
        }

        public virtual bool IsPermiteEntrarFila()
        {
            if ((Tracao != null) && (ModeloVeicularCarga?.NumeroReboques == 0))
                return true;

            return Reboques?.Count > 0;
        }

        public virtual Empresa ObterEmpresa()
        {
            if (Tracao?.Empresa != null)
                return Tracao.Empresa;

            if (Reboques?.Count > 0)
                return Reboques.Where(reboque => reboque.Empresa != null).Select(reboque => reboque.Empresa).FirstOrDefault();

            return null;
        }

        public virtual List<int> ObterCodigos()
        {
            List<int> codigos = new List<int>();

            if (Tracao != null)
                codigos.Add(Tracao.Codigo);

            if (Reboques?.Count > 0)
            {
                foreach (Veiculo reboque in Reboques)
                    codigos.Add(reboque.Codigo);
            }

            return codigos;
        }

        public virtual AreaVeiculoPosicao ObterLocalAtual()
        {
            if (Reboques?.Count > 0)
                return Reboques.FirstOrDefault().LocalAtual;

            return Tracao.LocalAtual;
        }

        public virtual string ObterPlacas()
        {
            List<string> placas = new List<string>();

            if (Tracao != null)
                placas.Add(Tracao.Placa_Formatada);

            if (Reboques?.Count > 0)
            {
                foreach (Veiculo reboque in Reboques)
                    placas.Add(reboque.Placa_Formatada);
            }

            return string.Join(", ", placas);
        }

        public virtual string ObterPlacasReboques()
        {
            if (Reboques?.Count > 0)
                return string.Join(", ", (from reboque in Reboques select reboque.Placa_Formatada));

            return "";
        }

        public virtual int ObterTotalPlacas()
        {
            int totalPlacas = ((Tracao != null) ? 1 : 0) + (Reboques?.Count ?? 0);

            return totalPlacas;
        }

        public virtual bool Equals(FilaCarregamentoConjuntoVeiculo other)
        {
            if (other == null)
                return false;

            if (Tracao?.Codigo != other.Tracao?.Codigo)
                return false;

            if (Reboques?.Count != other.Reboques?.Count)
                return false;

            if (Reboques != null)
            {
                foreach (Veiculo reboque in Reboques)
                {
                    if (!other.Reboques.Any(r => r.Codigo == reboque.Codigo))
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}
