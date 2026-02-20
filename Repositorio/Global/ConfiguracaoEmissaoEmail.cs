using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ConfiguracaoEmissaoEmail : RepositorioBase<Dominio.Entidades.ConfiguracaoEmissaoEmail>, Dominio.Interfaces.Repositorios.ConfiguracaoEmissaoEmail
    {
        public ConfiguracaoEmissaoEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoEmissaoEmail(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Task<Dominio.Entidades.ConfiguracaoEmissaoEmail> BuscarPorEmailAsync(int email)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmissaoEmail>();
            var result = from obj in query where obj.Email.Codigo == email select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.ConfiguracaoEmissaoEmail BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmissaoEmail>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> BuscarListaEmailAtivo()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmissaoEmail>();
            var result = from obj in query select obj.Email;

            result = result.Where(o => o.EmailAtivo);

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.ConfiguracaoEmissaoEmail Buscar(int codigoEmail, int codigoEmpresa, double cnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmissaoEmail>();
            var result = from obj in query select obj;

            if (codigoEmail > 0)
                result = result.Where(o => o.Email.Codigo == codigoEmail);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (cnpjCliente > 0)
                result = result.Where(o => o.ClienteRemetente.CPF_CNPJ == cnpjCliente);

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.ConfiguracaoEmissaoEmail> Consultar(string nome, string cnpj, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmissaoEmail>();
            var result = from obj in query where obj.Status.Equals(status) select obj;

            if (!string.IsNullOrEmpty(nome))
                result = result.Where(o => o.Empresa.RazaoSocial.Contains(nome));

            if (!string.IsNullOrEmpty(cnpj))
                result = result.Where(o => o.Empresa.CNPJ.Equals(cnpj));

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string nome, string cnpj, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmissaoEmail>();
            var result = from obj in query where obj.Status.Equals(status) select obj;

            if (!string.IsNullOrEmpty(nome))
                result = result.Where(o => o.Empresa.RazaoSocial.Contains(nome));

            if (!string.IsNullOrEmpty(cnpj))
                result = result.Where(o => o.Empresa.CNPJ.Equals(cnpj));

            return result.Count();
        }

        public Dominio.Entidades.ConfiguracaoEmissaoEmail BuscarPorEmpresaTipoDocumento(int codigoEmail, int codigoEmpresa, Dominio.Enumeradores.TipoDocumento tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmissaoEmail>();
            query = query.Where(o => o.Email.Codigo == codigoEmail && o.Empresa.Codigo == codigoEmpresa && o.TipoDocumento == tipoDocumento);

            return query.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }
    }
}
