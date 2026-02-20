using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoViagemNavioSchedule : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>
    {
        public PedidoViagemNavioSchedule(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> BuscarPorPedidoViagemNavio(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            var result = from obj in query where obj.PedidoViagemNavio.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> BuscarSchedulesExcluir(List<int> codigos, int codigoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            var result = from obj in query where !codigos.Contains(obj.Codigo) && obj.PedidoViagemNavio.Codigo == codigoViagem select obj;
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> BuscarPorETSConfirmadAguardandoEncerramento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            query = query.Where(obj => obj.ETSConfirmado == true && obj.PedidoViagemNavio != null && obj.PortoAtracacao != null && obj.PortoAtracacao.Localidade != null);

            var queryMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            query = query.Where(o => queryMDFe.Any(m => m.PortoOrigem.Codigo != o.PortoAtracacao.Codigo && m.PedidoViagemNavio.Codigo == o.PedidoViagemNavio.Codigo && m.Status == Dominio.Enumeradores.StatusMDFe.Autorizado));

            return query.ToList();
        }

        public List<int> BuscarNaoPesentesNaLista(int codigo, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            var result = from obj in query
                         where
                            obj.PedidoViagemNavio.Codigo == codigo
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule BuscarPorViagemPortoTerminal(int pedidoViagemNavio, int porto, int terminal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            var result = from obj in query
                         where
                            obj.PedidoViagemNavio.Codigo == pedidoViagemNavio
                            && obj.PortoAtracacao.Codigo == porto
                            && obj.TerminalAtracacao.Codigo == terminal
                         select obj;

            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule BuscarPorRequisicaoESchedule(int pedidoViagemNavio, int schedule)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            var result = from obj in query
                         where
                            obj.PedidoViagemNavio.Codigo == pedidoViagemNavio
                            && obj.Codigo == schedule
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> ConsultarPorNavioViagem(int navioViagem, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();

            var result = from obj in query where obj.PedidoViagemNavio.Codigo == navioViagem select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorNavioViagem(int navioViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();

            var result = from obj in query where obj.PedidoViagemNavio.Codigo == navioViagem select obj;

            return result.Count();
        }
    }
}
