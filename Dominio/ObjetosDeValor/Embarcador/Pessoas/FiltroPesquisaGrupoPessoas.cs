using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class FiltroPesquisaGrupoPessoas
    {
        public TipoGrupoPessoas TipoGrupoPessoas { get; set; }
        public string Descricao { get; set; }
        public string RaizCNPJ { get; set; }
        public double CnpjCpfCliente { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public List<int> ListaCodigoGrupoPessoasPermitidos { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
