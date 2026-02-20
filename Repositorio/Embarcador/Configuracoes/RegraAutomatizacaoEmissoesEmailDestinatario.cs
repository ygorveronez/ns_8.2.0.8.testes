using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class RegraAutomatizacaoEmissoesEmailDestinatario : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario>
    {
        public RegraAutomatizacaoEmissoesEmailDestinatario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario>();

            query = from obj in query where obj.Codigo == codigo select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario> BuscarPorRegraAutomatizacaoEmissoesEmail(int codigoRegraAutomatizacaoEmissoesEmail)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario>();

            query = from obj in query where obj.RegraAutomatizacaoEmissoesEmail.Codigo == codigoRegraAutomatizacaoEmissoesEmail select obj;

            return query.ToList();

        }

        public Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario BuscarPorRegraAutomatizacaoEmissoesEmailEDestinatario(int codigoRegraAutomatizacaoEmissoesEmail, double cnpjcpfDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario>();

            query = from obj in query where obj.RegraAutomatizacaoEmissoesEmail.Codigo == codigoRegraAutomatizacaoEmissoesEmail && obj.Destinatario.CPF_CNPJ == cnpjcpfDestinatario select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario> BuscarPorCliente(double cnpjcpfDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario>();

            query = from obj in query where obj.Destinatario.CPF_CNPJ == cnpjcpfDestinatario select obj;

            return query.ToList();
        }
    }
}
