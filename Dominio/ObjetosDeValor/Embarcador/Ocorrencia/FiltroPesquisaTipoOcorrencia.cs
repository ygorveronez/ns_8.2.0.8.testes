using System.Collections.Generic;


namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class FiltroPesquisaTipoOcorrencia
    {
        public int CodigoGrupoPessoas { get; set; }
        public double CpfCnpjPessoa { get; set; }
        public string Descricao { get; set; }
        public bool? Situacao { get; set; }
        public List<int> CodigoFilial { get; set; }
        public List<double> CodigoRecebedor { get; set; }
        public bool AcessoTerceiro { get; set; }
    }
}
