using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ConfiguracaoTabelaFrete
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string GrupoPessoas { get; set; }
        public string CodigoIntegracao { get; set; }
        public bool Situacao { get; set; }
        public string DescricaoDataInicial
        {
            get
            {
                if (DataInicial != DateTime.MinValue)
                    return DataInicial.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }
        public string DescricaoDataFinal
        {
            get
            {
                if (DataFinal != DateTime.MinValue)
                    return DataFinal.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }
        public string DescricaoSituacao
        {
            get
            {
                return Situacao ? "Ativo" : "Inativo";
            }
        }
    }
}
