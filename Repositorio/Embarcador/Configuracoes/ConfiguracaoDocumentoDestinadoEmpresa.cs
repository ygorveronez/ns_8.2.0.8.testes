using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoDocumentoDestinadoEmpresa : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa>
    {
        public ConfiguracaoDocumentoDestinadoEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa BuscarPorEmpresa(int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado modeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa>()
                                              .Where(o => o.Empresa.Codigo == codigoEmpresa && o.ModeloDocumento == modeloDocumento);

            return query.FirstOrDefault();
        }
    }
}
