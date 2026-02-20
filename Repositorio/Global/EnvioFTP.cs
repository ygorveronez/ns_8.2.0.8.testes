using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class EnvioFTP : RepositorioBase<Dominio.Entidades.EnvioFTP>, Dominio.Interfaces.Repositorios.EnvioFTP
    {
        public EnvioFTP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.EnvioFTP> BuscarPorStatus(Dominio.Enumeradores.StatusEnvioFTP status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EnvioFTP>();

            var result = from obj in query where obj.Status == status select obj;

            return result.ToList();
        }

        public DateTime BuscarUltimoEnvio(int codigoEmpresa, double cnpjCliente, Dominio.Enumeradores.TipoArquivoFTP tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EnvioFTP>();

            var result = from obj in query
                         where
                                obj.Status == Dominio.Enumeradores.StatusEnvioFTP.Sucesso &&
                                obj.Tipo == tipo
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (cnpjCliente > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == cnpjCliente);

            return result.OrderByDescending(o => o.Codigo).Select(o => o.DataFiltro).FirstOrDefault();
        }

    }
}
