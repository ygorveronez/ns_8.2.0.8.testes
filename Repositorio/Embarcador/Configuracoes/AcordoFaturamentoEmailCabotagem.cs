using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class AcordoFaturamentoEmailCabotagem : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem>
    {
        public AcordoFaturamentoEmailCabotagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem> BuscarPorAcordoFaturamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem>();
            query = query.Where(o => o.AcordoFaturamentoCliente.Codigo == codigo);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem> BuscarPorPessoa(double cnnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem>();
            query = query.Where(o => o.Pessoa.CPF_CNPJ == cnnpjPessoa);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem> BuscarPorGrupoPessoa(int grupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem>();
            query = query.Where(o => o.AcordoFaturamentoCliente.GrupoPessoas.Codigo == grupoPessoa);
            return query.ToList();
        }
    }
}
