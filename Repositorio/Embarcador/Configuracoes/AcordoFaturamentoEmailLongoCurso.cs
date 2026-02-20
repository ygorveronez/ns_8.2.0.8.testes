using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class AcordoFaturamentoEmailLongoCurso : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso>
    {
        public AcordoFaturamentoEmailLongoCurso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> BuscarPorAcordoFaturamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso>();
            query = query.Where(o => o.AcordoFaturamentoCliente.Codigo == codigo);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> BuscarPorPessoa(double cnnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso>();
            query = query.Where(o => o.Pessoa.CPF_CNPJ == cnnpjPessoa);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> BuscarPorGrupoPessoa(int grupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso>();
            query = query.Where(o => o.AcordoFaturamentoCliente.GrupoPessoas.Codigo == grupoPessoa);
            return query.ToList();
        }
    }
}
