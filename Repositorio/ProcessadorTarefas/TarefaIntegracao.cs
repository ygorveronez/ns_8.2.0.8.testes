using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using MongoDB.Driver;
using Repositorio.Global.Banco;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.ProcessadorTarefas
{
    public class TarefaIntegracao : RepositorioBaseMongo<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>, ITarefaIntegracao
    {
        #region Construtores

        public TarefaIntegracao(IMongoDbContext context) : base(context)
        {
        }

        #endregion Construtores

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>> ObterTarefaIntegracoesPendentes(string codigoTarefa, CancellationToken cancellationToken)
        {
            var filtros = Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Filter.And(
                Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Filter.In(
                    x => x.SituacaoIntegracao,
                    new[]
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                    }),
                Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Filter.Eq(x => x.IdTarefa, codigoTarefa)
            );

            return Session != null
                ? Collection.Find(Session, filtros).ToListAsync(cancellationToken)
                : Collection.Find(filtros).ToListAsync(cancellationToken);
        }

        public async Task<Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao> ObterArquivoIntegracaoPorCodigo(string codigoArquivo, CancellationToken cancellationToken)
        {
            var filtro = Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Filter.ElemMatch(
                x => x.Arquivos,
                arquivo => arquivo.Identifcador == codigoArquivo
            );

            Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao tarefaIntegracao = Session != null
                ? await Collection.Find(Session, filtro).FirstOrDefaultAsync(cancellationToken)
                : await Collection.Find(filtro).FirstOrDefaultAsync(cancellationToken);

            if (tarefaIntegracao == null || tarefaIntegracao.Arquivos == null)
                return null;

            return tarefaIntegracao.Arquivos.FirstOrDefault(a => a.Identifcador == codigoArquivo);
        }

        public Task<List<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>> ObterIntegracaoIntegradora(string codigoTarefa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacaoIntegracao, CancellationToken cancellationToken)
        {
            var filtros = Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Filter.Eq(x => x.IdTarefa, codigoTarefa);

            if (situacaoIntegracao.HasValue)
                filtros = filtros & Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Filter.Eq(x => x.SituacaoIntegracao, situacaoIntegracao.Value);

            return Session != null
                    ? Collection.Find(Session, filtros).ToListAsync(cancellationToken)
                    : Collection.Find(filtros).ToListAsync(cancellationToken);
        }

        public async Task AdicionarArquivoAsync(string tarefaIntegracaoId, Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao arquivo, CancellationToken cancellationToken)
        {
            var filter = Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Filter.Eq(x => x.Id, tarefaIntegracaoId);
            var update = Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Update
                .Push(x => x.Arquivos, arquivo);

            if (Session != null)
                await Collection.UpdateOneAsync(Session, filter, update, cancellationToken: cancellationToken);
            else
                await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task AtualizarAsync(string tarefaIntegracaoId, UpdateDefinition<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao> update, CancellationToken cancellationToken)
        {
            var filter = Builders<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao>.Filter.Eq(x => x.Id, tarefaIntegracaoId);

            if (Session != null)
                await Collection.UpdateOneAsync(Session, filter, update, cancellationToken: cancellationToken);
            else
                await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        #endregion Métodos Públicos
    }
}
