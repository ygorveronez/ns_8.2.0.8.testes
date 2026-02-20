using System.Collections.Generic;
using System.Linq;

namespace Google.OrTools.Api.Services.GoogleOrTools
{
    public class Resultado
    {
        public Resultado(Models.Veiculo veiculo)
        {
            this.veiculo = veiculo;
            this.itens = new List<ResultadoItens>();
        }

        /// <summary>
        /// Código do veiculo passado como parâmetro.
        /// </summary>
        //public int veiculo { get; set; }
        public Models.Veiculo veiculo { get; set; }

        public long distancia
        {
            get
            {
                if (this.itens == null)
                    return 0;
                else
                    return (from item in this.itens select item.distancia).Sum();
            }
        }

        public long tempo
        {
            get
            {
                if (this.itens == null)
                    return 0;
                else
                    return (from item in this.itens select item.tempo).Sum();
            }
        }

        public int qtde
        {
            get
            {
                return this.itens?.Count ?? 0;
            }
        }

        public double peso
        {
            get
            {
                if (this.itens == null)
                    return 0;
                else
                    return (from item in this.itens select item.item.PesoTotal).Sum();
            }
        }

        public List<ResultadoItens> itens { get; set; }
    }

    public class ResultadoItens
    {
        public Services.GoogleOrTools.Position item { get; set; }

        public long distancia { get; set; }

        public long tempo { get; set; }
    }
}