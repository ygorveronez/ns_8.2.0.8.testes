using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>
    {
        public OcorrenciaCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento BuscarPorOcorrencia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>();
            var result = from obj in query where obj.Ocorrencia.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ExistePorOcorrencia(int codigoCargaOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>();

            query = query.Where(obj => obj.Ocorrencia.Codigo == codigoCargaOcorrencia);

            return query.Select(o => o.Codigo).Any();
        }

        public List<int> BuscarCancelamentosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia situacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>();

            var result = from obj in query where obj.Situacao == situacao select obj;

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Select(o => o.Codigo).ToList();

        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> BuscarCancelamentosPorSituacaoETipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>();

            var result = from obj in query where obj.Situacao == situacao && obj.Tipo == tipo select obj;

            return result.ToList();

        }

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> _Consultar(int numeroOcorrencia, int numeroCTe, int codigoOperador, string carga, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia? tipo, string numeroOS, string numeroBooking, string numeroControle, List<int> codigosFiliais, List<double> codigosRecebedores)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>();

            var result = from obj in query select obj;

            // Filtros
            if (numeroOcorrencia > 0)
                result = result.Where(o => o.Ocorrencia.NumeroOcorrencia == numeroOcorrencia);

            if (!String.IsNullOrWhiteSpace(numeroOS))
            {
                var queryComplementos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                var resultComplementos = from obj in queryComplementos where obj.CargaCTeComplementado.CTe.NumeroOS == numeroOS select obj;
                result = result.Where(o => resultComplementos.Any(cte => cte.CargaOcorrencia.Codigo == o.Ocorrencia.Codigo));
            }

            if (!String.IsNullOrWhiteSpace(numeroBooking))
            {
                var queryComplementos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                var resultComplementos = from obj in queryComplementos where obj.CargaCTeComplementado.CTe.NumeroBooking == numeroBooking select obj;
                result = result.Where(o => resultComplementos.Any(cte => cte.CargaOcorrencia.Codigo == o.Ocorrencia.Codigo));
            }

            if (!String.IsNullOrWhiteSpace(numeroControle))
            {
                var queryComplementos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                var resultComplementos = from obj in queryComplementos where obj.CargaCTeComplementado.CTe.NumeroControle == numeroControle select obj;
                result = result.Where(o => resultComplementos.Any(cte => cte.CargaOcorrencia.Codigo == o.Ocorrencia.Codigo));
            }

            if (numeroCTe > 0)
            {
                var queryComplementos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                var resultComplementos = from obj in queryComplementos where obj.CTe.Numero == numeroCTe select obj;
                result = result.Where(o => resultComplementos.Any(cte => cte.CargaOcorrencia.Codigo == o.Ocorrencia.Codigo));
            }

            if (codigoOperador > 0)
                result = result.Where(o => o.Usuario.Codigo == codigoOperador);

            if (!string.IsNullOrWhiteSpace(carga))
                result = result.Where(o => o.Ocorrencia.Carga.CodigoCargaEmbarcador.Contains(carga));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataCancelamento.Value.Date >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataCancelamento.Value.Date <= dataFinal.Date);

            if (situacao.HasValue)
                result = result.Where(o => o.Situacao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.Tipo == tipo);

            if (codigosFiliais.Any(codigo => codigo == -1))
                result = result.Where(o => codigosFiliais.Contains(o.Ocorrencia.Carga.Filial.Codigo) || o.Ocorrencia.Carga.Pedidos.Any(ped => ped.Recebedor != null && codigosRecebedores.Contains(ped.Recebedor.CPF_CNPJ)));
            else if (codigosFiliais.Count > 0)
                result = result.Where(o => codigosFiliais.Contains(o.Ocorrencia.Carga.Filial.Codigo));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> Consultar(int numeroOcorrencia, int numeroCTe, int codigoOperador, string carga, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia? tipo, string numeroOS, string numeroBooking, string numeroControle, List<int> codigosFiliais, List<double> codigosRecebedores, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(numeroOcorrencia, numeroCTe, codigoOperador, carga, dataInicial, dataFinal, situacao, tipo, numeroOS, numeroBooking, numeroControle, codigosFiliais, codigosRecebedores);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(int numeroOcorrencia, int numeroCTe, int codigoOperador, string carga, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia? tipo, string numeroOS, string numeroBooking, string numeroControle, List<int> codigosFiliais, List<double> codigosRecebedores)
        {
            var result = _Consultar(numeroOcorrencia, numeroCTe, codigoOperador, carga, dataInicial, dataFinal, situacao, tipo, numeroOS, numeroBooking, numeroControle, codigosFiliais, codigosRecebedores);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> BuscarNaoIntegradasComTransportador(List<int> codigosTransportadores)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>();

            query = query.Where(o => !o.IntegrouTransportador && o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.Cancelada && codigosTransportadores.Contains(o.Ocorrencia.Carga.Empresa.Codigo));

            return query.ToList();
        }
    }
}
