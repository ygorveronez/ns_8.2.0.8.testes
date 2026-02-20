using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Configuracoes
{
    public class LiberacaoIntegracao
    {
        public static void ProcessarScriptsEspecificosPorIntegracao(TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            switch (tipoIntegracao)
            {
                case TipoIntegracao.APIGoogle:
                    ProcessarScriptsAPIGoogle(unitOfWork);
                    break;
                default:
                    break;
            }
        }

        public static void ProcessarScriptsAPIGoogle(Repositorio.UnitOfWork unitOfWork)
        {
            var repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            var integracao = repIntegracao.Buscar();

            if (integracao == null)
                integracao = new Dominio.Entidades.Embarcador.Configuracoes.Integracao();

            if (string.IsNullOrWhiteSpace(integracao.ServidorNominatim) || string.IsNullOrWhiteSpace(integracao.APIKeyGoogle))
            {
                integracao.GeoServiceGeocoding = GeoServiceGeocoding.Nominatim;
                integracao.ServidorNominatim = "http://20.206.78.118:8080/nominatim/{0}?";
                integracao.APIKeyGoogle = "AIzaSyBicwVH5sa9XGOlZzbTH4i_xmJX0FGHImo";

                repIntegracao.Atualizar(integracao);
            }

        }


    }
}
