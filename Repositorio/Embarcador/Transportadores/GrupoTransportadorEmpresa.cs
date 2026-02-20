using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class GrupoTransportadorEmpresa : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa>
    {
        #region Construtores

        public GrupoTransportadorEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa BuscarPorCodigo(int codigo)
        {
            var consultaGrupoTransportadorEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa>()
                .Where(o => o.Codigo == codigo);

            return consultaGrupoTransportadorEmpresa.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa> BuscarPorGrupoTransportador(int codigoGrupoTransportador)
        {
            var consultaGrupoTransportadorEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa>()
                .Where(o => o.GrupoTransportador.Codigo == codigoGrupoTransportador);

            return consultaGrupoTransportadorEmpresa.ToList();
        }

        public bool ExisteEmpresaEmOutroCadastro(int emp, int gp)
        {
            var consultaGrupoTransportadorEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa>()
                .Where(o => o.Empresa.Codigo == emp && o.GrupoTransportador.Codigo != gp);

            return consultaGrupoTransportadorEmpresa.Any();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa> BuscarPorTransportador(int empresa)
        {
            var consultaGrupoTransportadorEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa>()
                .Where(o => o.Empresa.Codigo == empresa);

            return consultaGrupoTransportadorEmpresa.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarTransportadoresPorGrupoTransportador(int codigoGrupoTransportador)
        {
            var consultaGrupoTransportadorEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa>()
                .Where(o => o.GrupoTransportador.Codigo == codigoGrupoTransportador);

            return consultaGrupoTransportadorEmpresa
                .Select(o => o.Empresa)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
