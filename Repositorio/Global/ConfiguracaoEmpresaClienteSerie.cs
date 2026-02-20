using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class ConfiguracaoEmpresaClienteSerie : RepositorioBase<Dominio.Entidades.ConfiguracaoEmpresaClienteSerie>
    {

        public ConfiguracaoEmpresaClienteSerie(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoEmpresaClienteSerie BuscarPorCodigo(int empresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmpresaClienteSerie>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ConfiguracaoEmpresaClienteSerie> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmpresaClienteSerie>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }

        public Dominio.Entidades.ConfiguracaoEmpresaClienteSerie BuscarPorCliente(int codigoConfiguracao, string cnpjCliente, Dominio.Enumeradores.TipoTomador tipoCliente)
        {
            double cnpjNumerico = double.Parse(cnpjCliente);
            double cnpjRaiz1 = double.Parse(cnpjCliente.Substring(0, 8) + "000000");
            double cnpjRaiz2 = double.Parse(cnpjCliente.Substring(0, 8) + "999999");

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmpresaClienteSerie>();
            var result = from obj in query
                         where
                            obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao &&
                            (obj.Cliente.CPF_CNPJ == cnpjNumerico || (obj.RaizCNPJ && obj.Cliente.CPF_CNPJ >= cnpjRaiz1 && obj.Cliente.CPF_CNPJ <= cnpjRaiz2)) &&
                            obj.TipoCliente == tipoCliente
                         select obj;
            return result.FirstOrDefault();
        }
    }
}
