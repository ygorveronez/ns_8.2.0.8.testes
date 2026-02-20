using System.Linq;


namespace Repositorio
{
    public class NaturaXML : RepositorioBase<Dominio.Entidades.NaturaXML>, Dominio.Interfaces.Repositorios.NaturaXML
    {
        public NaturaXML(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NaturaXML BuscaPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturaXML>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NaturaXML BuscaPorCodigoETipo(int codigo, Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturaXML>();

            var result = from obj in query where obj.Codigo == codigo && obj.Tipo == tipo select obj;

            return result.FirstOrDefault();
        }

    }
}
