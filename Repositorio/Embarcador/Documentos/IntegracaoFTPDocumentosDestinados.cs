using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class IntegracaoFTPDocumentosDestinados : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados>
    {
        public IntegracaoFTPDocumentosDestinados(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados>();
            var result = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados> BuscarTodos()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados>()
                .Where(integracaoFTP => integracaoFTP.Usuario != null && integracaoFTP.Usuario != "" &&
                    integracaoFTP.Senha != null && integracaoFTP.Senha != "" &&
                    integracaoFTP.EnderecoFTP != null && integracaoFTP.EnderecoFTP != "" &&
                    integracaoFTP.Porta != null && integracaoFTP.Porta != ""
            );

            return query.ToList();
        }

    }
}
