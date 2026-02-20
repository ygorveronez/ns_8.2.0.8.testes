namespace Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.TipoOcorrencia
{
    public class TipoOcorrencia
    {
        public int Codigo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public string CodigoProceda { get; set; }
        public string Tipo { get; set; }
        public string Situacao { get; set; }
        public string GrupoPessoas { get; set; }
        public string NomePessoa { get; set; }
        public double CPFCNPJPessoa { get; set; }
        public string TipoPessoa { get; set; }
        public string CPFCNPJPessoaFormatado
        {
            get
            {
                if (CPFCNPJPessoa > 0d)
                {
                    if (TipoPessoa == "J")
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJPessoa);
                    else if (TipoPessoa == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CPFCNPJPessoa);
                }

                return "";
            }
        }
        public string ComponenteFrete { get; set; }
        public string GrupoOcorrencia { get; set; }
        public string DescricaoAuxiliar { get; set; }
        public string CodigoIntegracaoAuxiliar { get; set; }

    }
}
