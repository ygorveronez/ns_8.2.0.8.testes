using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class RegraAutomatizacaoEmissoesEmailRemetente : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente>
    {
        public RegraAutomatizacaoEmissoesEmailRemetente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente>();

            query = from obj in query where obj.Codigo == codigo select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente> BuscarPorRegraAutomatizacaoEmissoesEmail(int codigoRegraAutomatizacaoEmissoesEmail)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente>();

            query = from obj in query where obj.RegraAutomatizacaoEmissoesEmail.Codigo == codigoRegraAutomatizacaoEmissoesEmail select obj;

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente BuscarPorRegraAutomatizacaoEmissoesEmailERemetente(int codigoRegraAutomatizacaoEmissoesEmail, double cnpjcpfRemetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente>();

            query = from obj in query where obj.RegraAutomatizacaoEmissoesEmail.Codigo == codigoRegraAutomatizacaoEmissoesEmail && obj.Remetente.CPF_CNPJ == cnpjcpfRemetente select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente> BuscarPorCliente(double cnpjcpfRemetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente>();

            query = from obj in query where obj.Remetente.CPF_CNPJ == cnpjcpfRemetente select obj;

            return query.ToList();
        }
    }
}
