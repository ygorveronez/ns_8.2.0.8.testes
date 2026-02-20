using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frotas
{
    public class ConfiguracaoAbastecimentoPlanilha : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha>
    {
        public ConfiguracaoAbastecimentoPlanilha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha> BuscarPorCodigoConfiguracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha>();
            var result = from obj in query where obj.ConfiguracaoAbastecimento.Codigo == codigo select obj;
            return result.ToList();
        }

        public int BuscarPosicaoColuna(int codigoConfiguracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha coluna)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha>();
            var result = from obj in query where obj.ConfiguracaoAbastecimento.Codigo == codigoConfiguracao && obj.ColunaPlanilha == coluna select obj;
            if (result.Count() > 0)
                return result.FirstOrDefault().Posicao;
            else
                return 0;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo BuscarTipoCampoColuna(int codigoConfiguracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha coluna)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha>();
            var result = from obj in query where obj.ConfiguracaoAbastecimento.Codigo == codigoConfiguracao && obj.ColunaPlanilha == coluna select obj;
            return result.FirstOrDefault().TipoCampo;
        }
    }
}
