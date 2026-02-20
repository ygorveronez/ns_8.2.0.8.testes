using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoTaxaDescargaAjudantes : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes>
    {
        #region Constructores
        public ConfiguracaoTaxaDescargaAjudantes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region  Merodos Publicos

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes> BuscarPorConfiguracaoTaxaDescarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes>();
            var result = from obj in query where obj.ConfiguracaoTaxaDescarga.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes BuscarPorQuantidadeVolumens(int quantiadeVolumens)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes>();
            query = from obj in query where obj.QuantidadeInicial >= quantiadeVolumens && quantiadeVolumens <= obj.QuantidadeFinal && obj.Tipo == ConfiguracaoTaxaDescargaTipo.AjudantesPorQuantidade select obj;

            return query.FirstOrDefault();
        }
        #endregion






    }
}
