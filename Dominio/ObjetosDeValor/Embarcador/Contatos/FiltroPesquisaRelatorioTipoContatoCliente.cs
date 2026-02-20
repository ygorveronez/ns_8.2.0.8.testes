using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Contatos
{
    public class FiltroPesquisaRelatorioTipoContatoCliente
    {
        public int CodigoGrupoPessoas { get; set; }
        public double CpfCnpjPessoa { get; set; }
        public List<int> TiposContato { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}