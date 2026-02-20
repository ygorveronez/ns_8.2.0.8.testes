using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento>
    {
        public CargaMDFeManualCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> BuscarPorSituacao(SituacaoMDFeManualCancelamento situacao, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento>();
            query = query.Where(o => o.SituacaoMDFeManualCancelamento == situacao);
            return query.Skip(inicio).Take(limite).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento BuscarPorCargaMDFeManual(int codigoMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoMDFeManual);

            return query.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> Consultar(int codigoVeiculo, int codigoMotorista, int codigoOrigem, int codigoDestino, int numeroCTe, int numeroMDFe, int codigoCarga, int empresa, SituacaoMDFeManualCancelamento? situacao, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento>();

            if (codigoVeiculo > 0)
                query = query.Where(o => o.CargaMDFeManual.Veiculo.Codigo == codigoVeiculo || o.CargaMDFeManual.Reboques.Any(r => r.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.CargaMDFeManual.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (codigoOrigem > 0)
                query = query.Where(o => o.CargaMDFeManual.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.CargaMDFeManual.Destino.Codigo == codigoDestino || o.CargaMDFeManual.Destinos.Any(det => det.Localidade.Codigo == codigoDestino));

            if (numeroCTe > 0)
                query = query.Where(o => o.CargaMDFeManual.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (numeroMDFe > 0)
                query = query.Where(o => o.CargaMDFeManual.MDFeManualMDFes.Any(c => c.MDFe.Numero == numeroMDFe));

            if (codigoCarga > 0)
                query = query.Where(o => o.CargaMDFeManual.Cargas.Any(c => c.Codigo == codigoCarga));

            if (empresa > 0)
                query = query.Where(o => o.CargaMDFeManual.Empresa.Codigo == empresa);

            if (situacao.HasValue && situacao != SituacaoMDFeManualCancelamento.todos)
                query = query.Where(o => o.SituacaoMDFeManualCancelamento == situacao);

            return query.Fetch(o => o.CargaMDFeManual)
                        .ThenFetch(o => o.Origem)
                        .Fetch(o => o.CargaMDFeManual)
                        .ThenFetch(o => o.Destino)
                        .Fetch(o => o.CargaMDFeManual)
                        .ThenFetch(o => o.Veiculo)
                        .Fetch(o => o.CargaMDFeManual)
                        .OrderBy(propOrdenar + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsulta(int codigoVeiculo, int codigoMotorista, int codigoOrigem, int codigoDestino, int numeroCTe, int numeroMDFe, int codigoCarga, int empresa, SituacaoMDFeManualCancelamento? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento>();

            if (codigoVeiculo > 0)
                query = query.Where(o => o.CargaMDFeManual.Veiculo.Codigo == codigoVeiculo || o.CargaMDFeManual.Reboques.Any(r => r.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.CargaMDFeManual.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (codigoOrigem > 0)
                query = query.Where(o => o.CargaMDFeManual.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.CargaMDFeManual.Destino.Codigo == codigoDestino || o.CargaMDFeManual.Destinos.Any(det => det.Localidade.Codigo == codigoDestino));

            if (numeroCTe > 0)
                query = query.Where(o => o.CargaMDFeManual.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (numeroMDFe > 0)
                query = query.Where(o => o.CargaMDFeManual.MDFeManualMDFes.Any(c => c.MDFe.Numero == numeroMDFe));

            if (codigoCarga > 0)
                query = query.Where(o => o.CargaMDFeManual.Cargas.Any(c => c.Codigo == codigoCarga));

            if (empresa > 0)
                query = query.Where(o => o.CargaMDFeManual.Empresa.Codigo == empresa);

            if (situacao.HasValue && situacao != SituacaoMDFeManualCancelamento.todos)
                query = query.Where(o => o.SituacaoMDFeManualCancelamento == situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntergacao(int codigo, int inicio, int limite)
        {
            var queryCargaCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>();
            var resultCargaCTeIntegracao = from obj in queryCargaCTeIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultCargaCTeIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntergacao(int codigo)
        {
            var queryCargaCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>();
            var resultCargaCTeIntegracao = from obj in queryCargaCTeIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultCargaCTeIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarIntergacaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> BuscarCargaMDFeManualCancelamentoAgIntegracao(bool gerandoIntegracoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento>();
            var result = from obj in query where obj.SituacaoMDFeManualCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.AgIntegracao && obj.GerandoIntegracoes == gerandoIntegracoes select obj;

            return result.ToList();
        }
    }
}
