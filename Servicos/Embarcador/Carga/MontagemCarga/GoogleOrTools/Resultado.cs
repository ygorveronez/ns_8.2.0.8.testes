using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    public class Resultado
    {
        public Resultado()
        {
            this.itens = new List<ResultadoItens>();
        }

        public Resultado(Veiculo veiculo)
        {
            this.veiculo = veiculo;
            this.itens = new List<ResultadoItens>();
        }

        /// <summary>
        /// Código do veiculo passado como parâmetro.
        /// </summary>
        //public int veiculo { get; set; }
        public Veiculo veiculo { get; set; }

        //[JsonIgnore]
        //[JsonProperty(Required = Required.Default)]
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

        //[JsonIgnore]
        //[JsonProperty(Required = Required.Default)]
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

        //[JsonIgnore]
        //[JsonProperty(Required = Required.Default)]
        public int qtde
        {
            get
            {
                return this.itens?.Count ?? 0;
            }
        }

        //[JsonIgnore]
        //[JsonProperty(Required = Required.Default)]
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
        public Local item { get; set; }

        public long distancia { get; set; }

        public long tempo { get; set; }
    }
}
