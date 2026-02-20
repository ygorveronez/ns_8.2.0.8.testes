using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public sealed class FiltroPesquisaCliente
    {
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public GeoLocalizacaoTipo GeoLocalizacaoTipo { get; set; }
        public Entidades.Embarcador.Financeiro.TituloBaixa BaixaPagar { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public bool ConsultarCodigoIntegracao { get; set; }
        public bool FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos { get; set; }
        public double CpfCnpj { get; set; }
        public string Estado { get; set; }
        public int CodigoPais { get; set; }
        public List<double> ListaCnpj { get; set; }
        public List<string> ListaRaizCnpj { get; set; }
        public Entidades.Localidade Localidade { get; set; }
        public List<TipoModalidade> Modalidades { get; set; }
        public string Nome { get; set; }
        public string RaizCNPJ { get; set; }
        public bool SomenteSemValorDescarga { get; set; }
        public bool SomenteSemGeolocalizacao { get; set; }
        public string Telefone { get; set; }
        public string Tipo { get; set; }
        public bool ComGeolocalizacao { get; set; }
        public bool AlvoEstrategico { get; set; }
        public string NomeFantasia { get; set; }
        public bool SomenteFilial { get; set; }
        public bool SomenteFronteira { get; set; }
        public int CodigoCategoria { get; set; }
        public int CodigoEmpresa { get; set; }
        public bool SomenteAreaRedex { get; set; }
        public bool SomenteArmador { get; set; }
        public bool SomenteSupridores { get; set; }
        public bool ApenasVinculadosACentroDescarregamento { get; set; }
        public bool PossuiExcecaoCheckinFilaH { get; set; }
        public List<double> ListaCodigosRecebedores { get; set; }
        public List<double> ListaCodigosExpedidores { get; set; }
        public string CodigoIntegracao { get; set; }
    }
}
