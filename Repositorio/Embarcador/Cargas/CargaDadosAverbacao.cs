using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaDadosAverbacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao>
    {
        public CargaDadosAverbacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public bool ConterDadosAverbacaoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao BuscarPorCargaProtocolo(int codigoCarga, string protocolo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Protocolo == protocolo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao BuscarPorCargaAverbacaoApolice(int codigoCarga, string averbacao, string numeroApolice)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Averbacao == averbacao && obj.ApoliceSeguro.NumeroApolice == numeroApolice select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao BuscarPorCargaAverbacaoApolice(int codigoCarga, string averbacao, DateTime DataRetorno)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Averbacao == averbacao && obj.DataRetorno == DataRetorno select obj;
            return result.FirstOrDefault();
        }

        public void ExcluirTodosPorCTeECarga(int carga, string chaveCTe)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaDadosAverbacao docProvi WHERE docProvi.Carga =:carga and docProvi.ChaveCTe =:chaveCTe")
                             .SetString("chaveCTe", chaveCTe)
                             .SetInt32("carga", carga)
                             .ExecuteUpdate();
        }
    }
}
