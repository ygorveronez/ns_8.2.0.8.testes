using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class OcorrenciaColetaEntregaAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo>
    {
        public OcorrenciaColetaEntregaAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo> BuscarPorOcorrencia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo>();
            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigo select obj;
            return result.ToList();
        }

        public bool PossuiAnexo(int codigoChamado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo>();
            var result = query.Where(obj => obj.EntidadeAnexo.Codigo == codigoChamado);
            return result.Any();
        }

        public int BuscarProximoNumero(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo>();
            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigoOcorrencia select obj;
            int ultimoNumero = result.Count() > 0 ? result.Select(o => o.Numero).Max() : 0;
            return ++ultimoNumero;
        }

        public int ContarConsultaImagensOcorrenciaPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo>();

            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo> ConsultarImagensOcorrenciaPendentesIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo>();

            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(o => o.EntidadeAnexo)
                .ThenFetch(o => o.CargaEntrega)
                .ToList();
        }

        public IList<int> BuscarCodigoAnexosCarga(int carga)
        {
            string sqlQuery = $@"select ANX_CODIGO from T_OCORRENCIA_COLETA_ENTREGA_ANEXO where OCE_CODIGO 
                                in( select OCE_CODIGO from  T_OCORRENCIA_COLETA_ENTREGA where CEN_CODIGO 
	                                in (select CEN_CODIGO from  T_CARGA_ENTREGA where CAR_CODIGO = :COD_CARGA ) )";

            var queryCodCarga = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            queryCodCarga.SetParameter("COD_CARGA", carga);            
            IList<int> codigosCarga = queryCodCarga.List<int>();
            return codigosCarga;
        }

        #endregion
    }
}
