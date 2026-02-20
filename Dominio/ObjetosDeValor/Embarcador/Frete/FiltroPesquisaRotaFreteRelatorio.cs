using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FiltroPesquisaRotaFreteRelatorio
    {
        public string Descricao { get; set; }
        public List<int> CodigosOrigem { get; set; }
        public List<int> CodigosDestino { get; set; }
        public bool? Situacao { get; set; }
        public double Destinatario { get; set; }
        public double Remetente { get; set; }
        public List<string> CodigosUFDestino { get; set; }
        public int GrupoPessoas { get; set; }
        public int TipoOperacao { get; set; }
        public bool? RotaExclusivaCompraValePedagio { get; set; }

        public string DescricaoSituacao
        {
            get
            {
                if (Situacao.HasValue)
                    return Situacao == true ? "Ativo" : "Inativo";

                return string.Empty;
            }
        }
    }
}
