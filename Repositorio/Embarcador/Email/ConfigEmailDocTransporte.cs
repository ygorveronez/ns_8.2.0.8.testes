using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Email
{
    public class ConfigEmailDocTransporte : RepositorioBase<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>
    {
        public ConfigEmailDocTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfigEmailDocTransporte(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> BuscarListaEmailAtivo()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();
            var result = query.Where(obj => obj.EmailAtivo == true);
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>> BuscarEmailLerDocumentosAsync()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();
            var result = query.Where(obj => obj.EmailAtivo == true && obj.LerDocumentos);

            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> BuscarEmailLerDocumentos(int codigoEmpresa)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();
            var result = query.Where(obj => obj.EmailAtivo == true && obj.LerDocumentos && obj.Empresa.Codigo == codigoEmpresa);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte BuscarEmailEnviaDocumentoAtivo(int codigoEmpresa = 0)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();

            if (codigoEmpresa > 0) //MultiNFe
            {
                var resultEmpresa = query.Where(obj => obj.EmailAtivo == true && obj.EnviarDocumentos == true && obj.Empresa.Codigo == codigoEmpresa);
                if (resultEmpresa.Count() > 0)
                    return resultEmpresa.FirstOrDefault();
                else
                {
                    var result = query.Where(obj => obj.EmailAtivo == true && obj.EnviarDocumentos == true && obj.Empresa == null);
                    return result.FirstOrDefault();
                }
            }
            else
            {
                var result = query.Where(obj => obj.EmailAtivo == true && obj.EnviarDocumentos == true);
                return result.FirstOrDefault();
            }
        }

        public Task<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> BuscarEmailEnviaDocumentoAtivoAsync(int codigoEmpresa = 0)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();

            if (codigoEmpresa > 0)
            {
                var resultEmpresa = query.Where(obj => obj.EmailAtivo && obj.EnviarDocumentos && obj.Empresa.Codigo == codigoEmpresa);
                if (resultEmpresa.Count() > 0)
                    return resultEmpresa.FirstOrDefaultAsync();
                else
                {
                    var result = query.Where(obj => obj.EmailAtivo && obj.EnviarDocumentos && obj.Empresa == null);
                    return result.FirstOrDefaultAsync();
                }
            }
            else
            {
                var result = query.Where(obj => obj.EmailAtivo && obj.EnviarDocumentos);
                return result.FirstOrDefaultAsync();
            }
        }

        public List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> Consultar(string email, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool? ativo = null, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();

            var result = from obj in query select obj;

            if (ativo != null)
                result = result.Where(obj => obj.EmailAtivo == ativo);

            if (!string.IsNullOrWhiteSpace(email))
                result = result.Where(obj => obj.Email.Contains(email));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string email, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(email))
                result = result.Where(obj => obj.Email.Contains(email));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        public bool ValidaConfiguracaoEmailDuplicado(int codigoEmpresa, int codigoConfiguracao = 0)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>();
            var result = query.Where(obj => obj.EmailAtivo == true && obj.Empresa.Codigo == codigoEmpresa && obj.Codigo != codigoConfiguracao);
            return result.Count() > 0 ? true : false;
        }
    }
}
