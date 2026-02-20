using Dominio.Interfaces.Repositorios;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Global.Banco
{
    public class RepositorioBaseMongo<T> : IRepositorioGenerico<T> where T : class
    {
        #region Propriedades Privadas

        private readonly IMongoCollection<T> _collection;
        private readonly IClientSessionHandle _session;

        #endregion Propriedades Privadas

        #region Propriedades Protegidas

        protected IMongoCollection<T> Collection => _collection;

        protected IClientSessionHandle Session => _session;

        #endregion Propriedades Protegidas

        #region Construtores        

        public RepositorioBaseMongo(IMongoDbContext context, IMongoDatabase collection = null)
        {
            try
            {
                _collection = context.ObterCollection().GetCollection<T>(typeof(T).Name);

                if (collection != null)
                    _collection = collection.GetCollection<T>(typeof(T).Name);

                _session = null;
            }
            catch (Exception ex)
            {
                Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao inicializar repositório MongoDB: {ex}", "CatchNoAction");
            }
        }

        #endregion Construtores

        #region Leitura

        public async Task<List<T>> ObterAsync(CancellationToken cancellationToken)
        {
            return _session != null
                ? await _collection.Find(_session, _ => true).ToListAsync(cancellationToken)
                : await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task<List<string>> ObterDistintosAsync(FieldDefinition<T, string> campo, FilterDefinition<T> filtro, CancellationToken cancellationToken)
        {
            return _session != null
                ? await _collection.Distinct(_session, campo, filtro).ToListAsync(cancellationToken)
                : await _collection.Distinct(campo, filtro).ToListAsync(cancellationToken);
        }

        public async Task<List<T>> ObterPorExpressaoAsync(Expression<Func<T, bool>> condicao, CancellationToken cancellationToken, int? take = null)
        {
            var query = _session != null
                ? _collection.Find(_session, condicao)
                : _collection.Find(condicao);

            if (take.HasValue)
            {
                query = query.Limit(take.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T> ObterUmPorExpressaoAsync(Expression<Func<T, bool>> condicao, CancellationToken cancellationToken)
        {
            return _session != null
                ? await _collection.Find(_session, condicao).FirstOrDefaultAsync(cancellationToken)
                : await _collection.Find(condicao).FirstOrDefaultAsync(cancellationToken);
        }

        #endregion Leitura

        #region Inserção

        public async Task InserirUmAsync(T entidade, CancellationToken cancellationToken)
        {
            if (_session != null)
            {
                await _collection.InsertOneAsync(_session, entidade, null, cancellationToken);
            }
            else
            {
                await _collection.InsertOneAsync(entidade, null, cancellationToken);
            }
        }

        public async Task InserirMuitosAsync(List<T> entidades, CancellationToken cancellationToken)
        {
            if (_session != null)
            {
                await _collection.InsertManyAsync(_session, entidades, null, cancellationToken);
            }
            else
            {
                await _collection.InsertManyAsync(entidades, null, cancellationToken);
            }
        }

        public async Task<List<T>> InserirMuitosERetornarAsync(List<T> entidades, CancellationToken cancellationToken)
        {
            if (_session != null)
            {
                await _collection.InsertManyAsync(_session, entidades, null, cancellationToken);
            }
            else
            {
                await _collection.InsertManyAsync(entidades, null, cancellationToken);
            }
            return entidades;
        }

        public async Task<T> InserirOuAtualizarAsync(Expression<Func<T, bool>> condicao, T entidade, CancellationToken cancellationToken)
        {
            var opcoesDeAtualizacao = new FindOneAndUpdateOptions<T, T>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var atualizacao = Builders<T>.Update;
            var definicoesDeAtualizacao = new List<UpdateDefinition<T>>();

            var idProperty = entidade.GetType().GetProperty("Id");

            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(entidade);

                if (idValue == null || idValue.ToString() == string.Empty)
                {
                    idProperty.SetValue(entidade, ObjectId.GenerateNewId().ToString());
                }
            }

            var propriedades = entidade.GetType().GetProperties()
                .Where(prop => prop.Name != "Id");

            foreach (var prop in propriedades)
            {
                var valor = prop.GetValue(entidade);
                var campo = prop.Name;

                if (valor != null)
                {
                    definicoesDeAtualizacao.Add(atualizacao.Set(campo, valor));
                }
            }

            var definicaoCombinadaDeAtualizacao = atualizacao.Combine(definicoesDeAtualizacao);

            return _session != null
                ? await _collection.FindOneAndUpdateAsync(_session, condicao, definicaoCombinadaDeAtualizacao, opcoesDeAtualizacao, cancellationToken)
                : await _collection.FindOneAndUpdateAsync(condicao, definicaoCombinadaDeAtualizacao, opcoesDeAtualizacao, cancellationToken);
        }

        #endregion Inserção

        #region Atualização

        public async Task AtualizarUmAsync(FilterDefinition<T> filtro, UpdateDefinition<T> entidade, CancellationToken cancellationToken)
        {
            if (_session != null)
            {
                await _collection.FindOneAndUpdateAsync(_session, filtro, entidade, null, cancellationToken);
            }
            else
            {
                await _collection.FindOneAndUpdateAsync(filtro, entidade, null, cancellationToken);
            }
        }

        public async Task AtualizarAsync(T entidade, CancellationToken cancellationToken)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("A entidade não possui uma propriedade 'Id'.");
            }

            var idValue = idProperty.GetValue(entidade).ToString();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", idValue), entidade, new ReplaceOptions(), cancellationToken);
        }

        #endregion Atualização

        #region Exclusão

        public async Task DeletarAsync(FilterDefinition<T> filtro, CancellationToken cancellationToken)
        {
            if (_session != null)
            {
                await _collection.DeleteManyAsync(_session, filtro, null, cancellationToken);
            }
            else
            {
                await _collection.DeleteManyAsync(filtro, null, cancellationToken);
            }
        }

        public async Task DeletarUmAsync(Expression<Func<T, bool>> filtro, CancellationToken cancellationToken)
        {
            if (_session != null)
            {
                await _collection.DeleteOneAsync(_session, filtro, null, cancellationToken);
            }
            else
            {
                await _collection.DeleteOneAsync(filtro, null, cancellationToken);
            }
        }

        public async Task DeletarMuitosAsync(Expression<Func<T, bool>> filtro, CancellationToken cancellationToken)
        {
            if (_session != null)
            {
                await _collection.DeleteManyAsync(_session, filtro, null, cancellationToken);
            }
            else
            {
                await _collection.DeleteManyAsync(filtro, null, cancellationToken);
            }
        }

        #endregion Exclusão
    }
}

