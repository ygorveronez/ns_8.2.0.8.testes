using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Transportadores
{
    public class ConfiguracaoIntegracaoLocalidade : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade>
    {
        public ConfiguracaoIntegracaoLocalidade(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade BuscarPorIBGE(int ibge, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade>();
            var result = from obj in query
                         where 
                         obj.Localidade.CodigoIBGE == ibge
                         && obj.TipoIntegracao == tipoIntegracao
                         select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade BuscarPorLocalidade(int localidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade>();
            var result = from obj in query where obj.Localidade.Codigo == localidade select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade BuscarPorCidadeExterior(string cidade, string siglaPais, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade>();
            var result = from obj in query
                         where
                         obj.Localidade.Descricao.Equals(cidade)
                         && obj.Localidade.Pais.Abreviacao.Equals(siglaPais)
                         && obj.TipoIntegracao == tipoIntegracao
                         select obj;
            return result.FirstOrDefault();
        }
    }
}
