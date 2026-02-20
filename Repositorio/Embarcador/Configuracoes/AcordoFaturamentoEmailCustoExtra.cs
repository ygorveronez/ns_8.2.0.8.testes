using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class AcordoFaturamentoEmailCustoExtra : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra>
    {
        public AcordoFaturamentoEmailCustoExtra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> BuscarPorAcordoFaturamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra>();
            query = query.Where(o => o.AcordoFaturamentoCliente.Codigo == codigo);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> BuscarPorPessoa(double cnnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra>();
            query = query.Where(o => o.Pessoa.CPF_CNPJ == cnnpjPessoa);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> BuscarPorGrupoPessoa(int grupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra>();
            query = query.Where(o => o.AcordoFaturamentoCliente.GrupoPessoas.Codigo == grupoPessoa);
            return query.ToList();
        }
    }
}
