using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class IntegracaoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete>
    {
        #region Construtores

        public IntegracaoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> BuscarPorCodigos(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracaoFreteRetornar = new List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete>();
            int limiteRegistros = 1000;
            int inicio = 0;

            while (inicio < codigos.Count)
            {
                List<int> codigosFiltrar = codigos.Skip(inicio).Take(limiteRegistros).ToList();

                var consultaIntegracaoFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete>()
                    .Where(o => codigosFiltrar.Contains(o.Codigo));

                integracaoFreteRetornar.AddRange(consultaIntegracaoFrete.ToList());

                inicio += limiteRegistros;
            }

            return integracaoFreteRetornar;
        }

        public Dominio.Entidades.Embarcador.Frete.IntegracaoFrete BuscarPorCodigoIntegracaoETipo(int codigoIntegracao, TipoIntegracaoFrete tipo)
        {
            var consultaIntegracaoFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao && o.Tipo == tipo);

            return consultaIntegracaoFrete.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> BuscarPorCodigosIntegracaoETipo(List<int> codigosIntegracao, TipoIntegracaoFrete tipo)
        {
            List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> resultadoIntegracaoETipo = new List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete>();
            int limiteRegistros = 1000;
            int inicio = 0;

            while (inicio < codigosIntegracao.Count)
            {
                var lote = codigosIntegracao.Skip(inicio).Take(limiteRegistros).ToList();

                var consultaLote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete>()
                    .Where(o => lote.Contains(o.CodigoIntegracao) && o.Tipo == tipo)
                    .ToList();

                resultadoIntegracaoETipo.AddRange(consultaLote);

                inicio += limiteRegistros;
            }

            return resultadoIntegracaoETipo;
        }

        #endregion Métodos Públicos
    }
}
