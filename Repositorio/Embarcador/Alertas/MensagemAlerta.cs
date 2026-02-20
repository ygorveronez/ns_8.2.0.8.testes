using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Alertas
{
    public sealed class MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> : RepositorioBase<TMensagemAlerta>
        where TMensagemAlerta : Dominio.Entidades.Embarcador.Alertas.MensagemAlerta<TEntidadeMensagemAlerta>
        where TEntidadeMensagemAlerta : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Construtores

        public MensagemAlerta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<TMensagemAlerta> BuscarConfirmadasPorEntidadeETipo(int codigoEntidade, TipoMensagemAlerta tipo)
        {
            var consultaMensagemAlerta = this.SessionNHiBernate.Query<TMensagemAlerta>()
                .Where(o => o.Entidade.Codigo == codigoEntidade && o.Confirmada == true && o.Tipo == tipo);

            return consultaMensagemAlerta.ToList();
        }

        public List<TMensagemAlerta> BuscarNaoConfirmadasPorEntidadeETipo(int codigoEntidade, TipoMensagemAlerta tipo)
        {
            var consultaMensagemAlerta = this.SessionNHiBernate.Query<TMensagemAlerta>()
                .Where(o => o.Entidade.Codigo == codigoEntidade && o.Confirmada == false && o.Tipo == tipo);

            return consultaMensagemAlerta.ToList();
        }

        public List<TMensagemAlerta> BuscarNaoConfirmadasPorEntidades(List<int> codigosEntidades)
        {
            var consultaMensagemAlerta = this.SessionNHiBernate.Query<TMensagemAlerta>()
                .Where(o => codigosEntidades.Contains(o.Entidade.Codigo) && o.Confirmada == false);

            return consultaMensagemAlerta.ToList();
        }

        public TMensagemAlerta BuscarPrimeiraNaoConfirmadaPorEntidadeETipo(int codigoEntidade, TipoMensagemAlerta tipo)
        {
            var consultaMensagemAlerta = this.SessionNHiBernate.Query<TMensagemAlerta>()
                .Where(o => o.Entidade.Codigo == codigoEntidade && o.Confirmada == false && o.Tipo == tipo);

            return consultaMensagemAlerta.FirstOrDefault();
        }

        public bool ExisteNaoConfirmadaComBloqueio(int codigoEntidade)
        {
            var consultaMensagemAlerta = this.SessionNHiBernate.Query<TMensagemAlerta>()
                .Where(o => o.Entidade.Codigo == codigoEntidade && o.Confirmada == false && ((bool?)o.Bloquear ?? false) == true);

            return consultaMensagemAlerta.Count() > 0;
        }

        public bool ExisteNaoConfirmadaPorEntidadeETipo(int codigoEntidade, TipoMensagemAlerta tipo)
        {
            var consultaMensagemAlerta = this.SessionNHiBernate.Query<TMensagemAlerta>()
                .Where(o => o.Entidade.Codigo == codigoEntidade && o.Confirmada == false && o.Tipo == tipo);

            return consultaMensagemAlerta.Count() > 0;
        }

        public bool ExisteNaoConfirmadaPorEntidadeETipos(int codigoEntidade, List<TipoMensagemAlerta> tipos)
        {
            var consultaMensagemAlerta = this.SessionNHiBernate.Query<TMensagemAlerta>()
                .Where(o => o.Entidade.Codigo == codigoEntidade && o.Confirmada == false && tipos.Contains(o.Tipo));

            return consultaMensagemAlerta.Count() > 0;
        }

        #endregion Métodos Públicos
    }
}
