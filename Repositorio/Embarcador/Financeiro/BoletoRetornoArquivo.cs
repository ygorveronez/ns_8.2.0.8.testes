using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BoletoRetornoArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo>
    {
        public BoletoRetornoArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo> BuscarRetornoArquivoConcluido()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo>();
            var result = from obj in query where obj.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Gerado select obj;
            return result.ToList();
        }
    }
}
