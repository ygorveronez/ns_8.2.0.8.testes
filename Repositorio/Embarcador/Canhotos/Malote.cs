using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Canhotos
{
    public class Malote : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.Malote>
    {

        public Malote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Canhotos.Malote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Malote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoProtocolo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Malote>();

            int? retorno = query.Max(o => (int?)o.Protocolo);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotos(int malote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.MaloteCanhoto>();

            var result = from o in query where o.Malote.Codigo == malote select o.Canhoto;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Malote BuscarMalotePorTransportadorFilialOuTipoOperacao(int transportador, int filial, int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Malote>();
            var result = from obj in query where obj.Empresa.Codigo == transportador && obj.Filial.Codigo == filial && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto.Gerado select obj;

            if (tipoOperacao > 0)
                result = result.Where(o => o.TipoOperacao == null || o.TipoOperacao.Codigo == tipoOperacao);

            return result.FirstOrDefault();
        }

        public void SetarCanhotosRecebidos(int malote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto situacaoCanhoto, int localArmazenamentoCanhoto, DateTime dataEnvioCanhoto)
        {
            //string hql = "UPDATE MaloteCanhoto maloteCanhoto SET maloteCanhoto.Canhoto.SituacaoCanhoto = :Situacao, maloteCanhoto.Canhoto.LocalArmazenamentoCanhoto = :LocalArmazenamentoCanhoto WHERE maloteCanhoto.Malote = :Malote";
            string hql = "UPDATE Canhoto canhoto SET canhoto.SituacaoCanhoto = :Situacao, DataEnvioCanhoto = :DataEnvioCanhoto, canhoto.LocalArmazenamentoCanhoto = :LocalArmazenamentoCanhoto WHERE canhoto.Codigo IN (SELECT maloteCanhoto.Canhoto FROM MaloteCanhoto maloteCanhoto WHERE maloteCanhoto.Malote = :Malote)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Malote", malote);
            query.SetEnum("Situacao", situacaoCanhoto);
            query.SetDateTime("DataEnvioCanhoto", dataEnvioCanhoto);
            query.SetInt32("LocalArmazenamentoCanhoto", localArmazenamentoCanhoto);
            query.ExecuteUpdate();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.Malote> _Consultar(int empresa, int protocolo, int numeroCanhoto, double emitente, int destino, int operador, string nomeOperador, int filial, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Malote>();

            var result = from obj in query select obj;

            // Filtros
            if (empresa > 0)
                result = result.Where(o => o.Empresa.Codigo == empresa);

            if (protocolo > 0)
                result = result.Where(o => o.Protocolo == protocolo);

            if (numeroCanhoto > 0)
                result = result.Where(o => o.Canhotos.Any(c => c.Canhoto.Numero == numeroCanhoto));

            if (emitente > 0)
                result = result.Where(o => o.Canhotos.Any(c => c.Canhoto.Emitente.CPF_CNPJ == emitente));

            if (destino > 0)
                result = result.Where(o => o.Destino.Codigo == destino);

            if (filial > 0)
                result = result.Where(o => o.Filial.Codigo == filial);

            if (operador > 0)
                result = result.Where(o => o.Operador.Codigo == operador || o.Operador.Nome == nomeOperador);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio.Date >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio.Date <= dataFinal.Date);

            if (situacao.HasValue)
                result = result.Where(o => o.Situacao == situacao.Value);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Malote> Consultar(int empresa, int protocolo, int numeroCanhoto, double emitente, int destino, int operador, string nomeOperador, int filial, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(empresa, protocolo, numeroCanhoto, emitente, destino, operador, nomeOperador, filial, dataInicial, dataFinal, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int empresa, int protocolo, int numeroCanhoto, double emitente, int destino, int operador, string nomeOperador, int filial, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMaloteCanhoto? situacao)
        {
            var result = _Consultar(empresa, protocolo, numeroCanhoto, emitente, destino, operador, nomeOperador, filial, dataInicial, dataFinal, situacao);

            return result.Count();
        }
    }
}
