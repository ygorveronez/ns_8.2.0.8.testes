using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.ObjetosDeValor
{
    public class Parametro
    {
        public Parametro() { }

        public Parametro(string nomeParametro, string valorParametro, bool visivel, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;
            this.ValorParametro = valorParametro;
            this.Visivel = visivel;
            this.DescricaoParametro = descricaoParametro;
        }

        public Parametro(string nomeParametro, string valor, string defValor = "", string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (!string.IsNullOrWhiteSpace(valor))
            {
                this.ValorParametro = valor;
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = defValor;
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, DateTime data, bool comHora = false, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (data != DateTime.MinValue)
            {
                this.ValorParametro = comHora ? data.ToString("dd/MM/yyyy HH:mm") : data.ToString("dd/MM/yyyy");
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, DateTime? data, bool comHora = false, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (data.HasValue)
            {
                this.ValorParametro = comHora ? data.Value.ToString("dd/MM/yyyy HH:mm") : data.Value.ToString("dd/MM/yyyy");
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, DateTime? dataInicial, DateTime? dataFinal, bool comHora = false, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            string descricao = string.Empty;
            string pattern = comHora ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy";

            if (dataInicial.HasValue && dataFinal.HasValue)
                descricao = $"De {dataInicial.Value.ToString(pattern)} até {dataFinal.Value.ToString(pattern)}";
            else if (dataInicial.HasValue)
                descricao = $"À partir de {dataInicial.Value.ToString(pattern)}";
            else if (dataFinal.HasValue)
                descricao = $"Até {dataFinal.Value.ToString(pattern)}";

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                this.ValorParametro = descricao;
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, DateTime dataInicial, DateTime dataFinal, bool comHora = false, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            string descricao = string.Empty;
            string pattern = comHora ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy";

            if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                descricao = $"De {dataInicial.ToString(pattern)} até {dataFinal.ToString(pattern)}";
            else if (dataInicial != DateTime.MinValue)
                descricao = $"À partir de {dataInicial.ToString(pattern)}";
            else if (dataFinal != DateTime.MinValue)
                descricao = $"Até {dataFinal.ToString(pattern)}";

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                this.ValorParametro = descricao;
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, int valor, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valor > 0)
            {
                this.ValorParametro = valor.ToString();
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, int? valor, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valor.HasValue)
            {
                this.ValorParametro = valor.ToString();
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, int? valorInicial, int? valorFinal, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valorInicial > 0 || valorFinal > 0)
            {
                if (valorInicial > 0 && valorFinal <= 0)
                    ValorParametro = $"À partir de {valorInicial}";
                else if (valorInicial <= 0 && valorFinal > 0)
                    ValorParametro = $"Até {valorFinal}";
                else
                    ValorParametro = $"De {valorInicial} até {valorFinal}";

                Visivel = true;
                DescricaoParametro = descricaoParametro;
            }
            else
            {
                ValorParametro = "";
                Visivel = false;
            }
        }

        public Parametro(string nomeParametro, decimal? valor, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valor > 0m)
            {
                this.ValorParametro = valor.Value.ToString("n2");
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, decimal? valorInicial, decimal? valorFinal, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valorInicial > 0m || valorFinal > 0m)
            {
                if (valorInicial > 0m && valorFinal <= 0m)
                    ValorParametro = $"À partir de {valorInicial.Value.ToString("n2")}";
                else if (valorInicial <= 0 && valorFinal > 0m)
                    ValorParametro = $"Até {valorFinal.Value.ToString("n2")}";
                else
                    ValorParametro = $"De {valorInicial.Value.ToString("n2")} até {valorFinal.Value.ToString("n2")}";

                Visivel = true;
                DescricaoParametro = descricaoParametro;
            }
            else
            {
                ValorParametro = "";
                Visivel = false;
            }
        }

        public Parametro(string nomeParametro, bool? valor, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valor.HasValue)
            {
                this.ValorParametro = valor.Value ? "Sim" : "Não";
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, bool visivel)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;
            this.ValorParametro = "";
            this.Visivel = visivel;
        }

        public Parametro(string nomeParametro, List<string> valor, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valor != null && valor.Count > 0)
            {
                this.ValorParametro = string.Join(", ", valor);
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, List<int> valor, string descricaoParametro = null)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valor != null && valor.Count > 0)
            {
                this.ValorParametro = string.Join(", ", valor);
                this.Visivel = true;
                this.DescricaoParametro = descricaoParametro;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        public Parametro(string nomeParametro, IEnumerable<string> valor)
        {
            this.IDTextoParametro = nomeParametro + "Text";
            this.IDParametro = nomeParametro + "Par";
            this.NomeParametro = nomeParametro;

            if (valor != null && valor.Count() > 0)
            {
                this.ValorParametro = string.Join(", ", valor);
                this.Visivel = true;
            }
            else
            {
                this.ValorParametro = "";
                this.Visivel = false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="idTextoParametro">ID do elemento do título do parâmetro no report.</param>
        /// <param name="idParametro">ID do elemento do valor do parâmetro no report.</param>
        /// <param name="nomeParametro">Nome do parâmetro.</param>
        /// <param name="valorParametro">Descrição do valor do parâmetro.</param>
        /// <param name="visivel">Indica se o parâmetro está visível ou não.</param>
        /// <param name="descricaoParametro">Descrição do título do parâmetro.</param>
        public Parametro(string idTextoParametro, string idParametro, string nomeParametro, string valorParametro, bool visivel, string descricaoParametro)
        {
            this.IDTextoParametro = idTextoParametro;
            this.IDParametro = idParametro;
            this.NomeParametro = nomeParametro;
            this.ValorParametro = valorParametro;
            this.Visivel = visivel;
        }

        public string IDTextoParametro { get; set; }
        public string IDParametro { get; set; }
        public string NomeParametro { get; set; }
        public string DescricaoParametro { get; set; }
        public string ValorParametro { get; set; }
        public bool Visivel { get; set; }
    }
}
