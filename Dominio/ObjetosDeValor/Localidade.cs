using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor
{
    public class Localidade
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public int IBGE { get; set; }
        public string CodigoIntegracao { get; set; }
        public string SiglaUF { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Pais Pais { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Regiao Regiao { get; set; }
        public string CodigoDocumento { get; set; }
        public bool Atualizar { get; set; }
        public string RKST { get; set; }
        public TipoEmissaoIntramunicipal TipoEmissaoIntramunicipal { get; set; }

    }
}
