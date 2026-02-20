using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete
{
    public class TransportadorGrupoTransportadoresHUBOfertas : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>
    {
        #region Construtores

        public TransportadorGrupoTransportadoresHUBOfertas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas BuscarPorCodigo(int codigo)
        {
            var consultaTransportadorGrupoTransportadoresHUB = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>()
                .Where(o => o.Codigo == codigo);

            return consultaTransportadorGrupoTransportadoresHUB.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>> BuscarPorGruposTransportadores(List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas> gruposTransportadores)
        {

            var codigosGruposTransportadores = gruposTransportadores.Select(g => g.Codigo).ToList();

            var consultaTransportadorGrupoTransportadoresHUB = SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>()
                .Where(o => codigosGruposTransportadores.Contains(o.GrupoTransportador.Codigo));

            return consultaTransportadorGrupoTransportadoresHUB.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>> BuscarPorGrupoTransportador(int codigoGrupoTransportador)
        {
            var consultaTransportadorGrupoTransportadoresHUB = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>()
                .Where(o => o.GrupoTransportador.Codigo == codigoGrupoTransportador);

            return consultaTransportadorGrupoTransportadoresHUB.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>> BuscarPorGrupoTransportadorAsync(int codigoGrupoTransportador)
        {
            var consultaTransportadorGrupoTransportadoresHUB = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>()
                .Where(o => o.GrupoTransportador.Codigo == codigoGrupoTransportador);

            return consultaTransportadorGrupoTransportadoresHUB.ToListAsync(CancellationToken);
        }

        public bool ExisteEmpresaEmOutroCadastro(int emp, int gp)
        {
            var consultaTransportadorGrupoTransportadoresHUB = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>()
                .Where(o => o.Empresa.Codigo == emp && o.GrupoTransportador.Codigo != gp);

            return consultaTransportadorGrupoTransportadoresHUB.Any();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas> BuscarPorTransportador(int empresa)
        {
            var consultaTransportadorGrupoTransportadoresHUB = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>()
                .Where(o => o.Empresa.Codigo == empresa);

            return consultaTransportadorGrupoTransportadoresHUB.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarTransportadoresPorGrupoTransportador(int codigoGrupoTransportador)
        {
            var consultaTransportadorGrupoTransportadoresHUB = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas>()
                .Where(o => o.GrupoTransportador.Codigo == codigoGrupoTransportador);

            return consultaTransportadorGrupoTransportadoresHUB
                .Select(o => o.Empresa)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
