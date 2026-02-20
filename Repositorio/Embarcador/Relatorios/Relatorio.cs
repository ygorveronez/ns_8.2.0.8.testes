using NHibernate.Linq;
using System.Collections.Generic;
//using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Relatorios
{
    public class Relatorio : RepositorioBase<Dominio.Entidades.Embarcador.Relatorios.Relatorio>
    {
        #region Construtores

        public Relatorio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Relatorio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Relatorios.Relatorio> Consultar(string descricao, Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, int codigoUsuario)
        {
            var consultaRelatorio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.Relatorio>()
                .Where(o =>
                    o.Ativo && o.CodigoControleRelatorios == codigoControleRelatorio && (o.TipoServicoMultisoftware == tipoServicoMultisoftware || o.TipoServicoMultisoftware == null)
                );

            if (codigoEmpresa > 0)
                consultaRelatorio = consultaRelatorio.Where(o => o.Empresa.Codigo == codigoEmpresa || o.PadraoMultisoftware);
            else
                consultaRelatorio = consultaRelatorio.Where(o => o.Empresa == null);

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaRelatorio = consultaRelatorio.Where(o => o.Descricao.Contains(descricao));

            if (codigoUsuario > 0)
                consultaRelatorio = consultaRelatorio.Where(o => o.RelatorioParaTodosUsuarios || o.Usuario.Codigo == codigoUsuario);

            return consultaRelatorio;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Relatorios.Relatorio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        //TODO: (ct default)
        public async Task<Dominio.Entidades.Embarcador.Relatorios.Relatorio> BuscarPorCodigoAsync(int codigo, CancellationToken cancellationToken = default)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.Relatorio>()
               .Where(x => x.Codigo == codigo).FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Relatorios.Relatorio BuscarPorTitulo(string titulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();
            var result = from obj in query where obj.Titulo.Equals(titulo) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Relatorios.Relatorio BuscarPadraoMultisoftware(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();
            var result = from obj in query where obj.CodigoControleRelatorios == codigoControleRelatorio && obj.PadraoMultisoftware && (obj.TipoServicoMultisoftware == tipoServico || obj.TipoServicoMultisoftware == null) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Relatorios.Relatorio BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();
            var result = from obj in query where obj.Ativo && obj.CodigoControleRelatorios == codigoControleRelatorio && obj.Padrao && (obj.TipoServicoMultisoftware == tipoServico || obj.TipoServicoMultisoftware == null) select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Relatorios.Relatorio> BuscarPadraoPorCodigoControleRelatorioAsync(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();
            var result = from obj in query where obj.Ativo && obj.CodigoControleRelatorios == codigoControleRelatorio && obj.Padrao && (obj.TipoServicoMultisoftware == tipoServico || obj.TipoServicoMultisoftware == null) select obj;
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Relatorios.Relatorio BuscarCodigoControleRelatorioEDescricao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, string descricao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool padraoParaTodos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();
            var result = from obj in query where obj.Ativo && obj.CodigoControleRelatorios == codigoControleRelatorio && obj.Descricao.ToLower().Equals(descricao) && obj.RelatorioParaTodosUsuarios == padraoParaTodos && (obj.TipoServicoMultisoftware == tipoServico || obj.TipoServicoMultisoftware == null) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Relatorios.Relatorio> Consultar(string descricao, Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRelatorio = Consultar(descricao, codigoControleRelatorio, tipoServicoMultisoftware, codigoEmpresa, codigoUsuario);

            return ObterLista(consultaRelatorio, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, int codigoUsuario)
        {
            var consultaRelatorio = Consultar(descricao, codigoControleRelatorio, tipoServicoMultisoftware, codigoEmpresa, codigoUsuario);

            return consultaRelatorio.Count();
        }

        #endregion
    }
}
