using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.IntegracaoAssincrona;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Bson;
using MongoDB.Driver;
using Repositorio.Global.Banco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.ProcessadorTarefas
{
    public class ProcessamentoTarefaRepository : RepositorioBaseMongo<ProcessamentoTarefa>, IProcessamentoTarefaRepository
    {
        public ProcessamentoTarefaRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<bool> AtualizarComFiltroAsync(
            FilterDefinition<ProcessamentoTarefa> filter,
            UpdateDefinition<ProcessamentoTarefa> update,
            CancellationToken cancellationToken)
        {
            var resultado = Session != null
                ? await Collection.UpdateOneAsync(Session, filter, update, new UpdateOptions(), cancellationToken)
                : await Collection.UpdateOneAsync(filter, update, new UpdateOptions(), cancellationToken);

            return resultado.ModifiedCount > 0;
        }

        public async Task AtualizarAsync(
            string id,
            UpdateDefinition<ProcessamentoTarefa> update,
            CancellationToken cancellationToken)
        {
            var filter = Builders<ProcessamentoTarefa>.Filter.Eq(t => t.Id, id);

            if (Session != null)
            {
                await Collection.UpdateOneAsync(Session, filter, update, new UpdateOptions(), cancellationToken);
            }
            else
            {
                await Collection.UpdateOneAsync(filter, update, new UpdateOptions(), cancellationToken);
            }
        }

        public async Task AtualizarStatusAsync(
            string id,
            StatusTarefa status,
            string mensagem,
            CancellationToken cancellationToken)
        {
            var update = Builders<ProcessamentoTarefa>.Update
                .Set(t => t.Status, status)
                .Set(t => t.AtualizadoEm, DateTime.UtcNow);

            if (!string.IsNullOrEmpty(mensagem))
            {
                update = update.Set("PRO_ETAPAS.0.ETA_MENSAGEM", mensagem);
            }

            await AtualizarAsync(id, update, cancellationToken);
        }

        public async Task<ProcessamentoTarefa> ObterPorIdAsync(string id, CancellationToken cancellationToken)
        {
            var filter = Builders<ProcessamentoTarefa>.Filter.Eq(t => t.Id, id);

            return Session != null
                ? await Collection.Find(Session, filter).FirstOrDefaultAsync(cancellationToken)
                : await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CriarIndicesAsync()
        {
            var idx1 = Builders<ProcessamentoTarefa>.IndexKeys
                .Ascending(t => t.Status)
                .Ascending(t => t.TipoRequest)
                .Descending(t => t.CriadoEm);

            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<ProcessamentoTarefa>(idx1,
                    new CreateIndexOptions
                    {
                        Name = "idx_status_tipo_data",
                        Background = true
                    })
            );

            var idx2 = Builders<ProcessamentoTarefa>.IndexKeys
                .Ascending(t => t.CodigoIntegradora)
                .Descending(t => t.CriadoEm);

            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<ProcessamentoTarefa>(idx2,
                    new CreateIndexOptions
                    {
                        Name = "idx_integradora_data",
                        Background = true
                    })
            );

            var idx3 = Builders<ProcessamentoTarefa>.IndexKeys
                .Ascending(t => t.AtualizadoEm);

            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<ProcessamentoTarefa>(idx3,
                    new CreateIndexOptions
                    {
                        Name = "idx_atualizado_em",
                        Background = true
                    })
            );

            var idx4 = Builders<ProcessamentoTarefa>.IndexKeys
                .Ascending(t => t.RequestId);

            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<ProcessamentoTarefa>(idx4,
                    new CreateIndexOptions
                    {
                        Name = "idx_request_id",
                        Background = true
                    })
            );
        }

        public async Task<List<ProcessamentoTarefa>> ObterPaginadoComFiltrosAsync(
            FiltroPesquisaIntegracaoAssincrona filtros,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros,
            CancellationToken cancellationToken)
        {
            bool precisaLookup = !string.IsNullOrWhiteSpace(filtros.NumeroPedido) ||
                                 !string.IsNullOrWhiteSpace(filtros.NumeroCarregamento) ||
                                 filtros.NumeroCarga.HasValue;

            if (precisaLookup)
            {
                return await ObterComLookupAsync(filtros, parametros, cancellationToken);
            }
            else
            {
                return await ObterSimplesAsync(filtros, parametros, cancellationToken);
            }
        }

        public async Task<long> ContarComFiltrosAsync(
            FiltroPesquisaIntegracaoAssincrona filtros,
            CancellationToken cancellationToken)
        {
            bool precisaLookup = !string.IsNullOrWhiteSpace(filtros.NumeroPedido) ||
                                 !string.IsNullOrWhiteSpace(filtros.NumeroCarregamento) ||
                                 filtros.NumeroCarga.HasValue;

            if (precisaLookup)
            {
                return await ContarComLookupAsync(filtros, cancellationToken);
            }
            else
            {
                var filter = ConstruirFiltroSimples(filtros);
                return Session != null
                    ? await Collection.CountDocumentsAsync(Session, filter, null, cancellationToken)
                    : await Collection.CountDocumentsAsync(filter, null, cancellationToken);
            }
        }

        private async Task<List<ProcessamentoTarefa>> ObterSimplesAsync(
            FiltroPesquisaIntegracaoAssincrona filtros,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros,
            CancellationToken cancellationToken)
        {
            var filter = ConstruirFiltroSimples(filtros);
            var query = Session != null
                ? Collection.Find(Session, filter)
                : Collection.Find(filter);

            if (parametros.InicioRegistros > 0)
                query = query.Skip(parametros.InicioRegistros);

            if (parametros.LimiteRegistros > 0)
                query = query.Limit(parametros.LimiteRegistros);

            query = query.Sort(Builders<ProcessamentoTarefa>.Sort.Descending(t => t.CriadoEm));

            return await query.ToListAsync(cancellationToken);
        }

        private async Task<List<ProcessamentoTarefa>> ObterComLookupAsync(
            FiltroPesquisaIntegracaoAssincrona filtros,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros,
            CancellationToken cancellationToken)
        {
            var pipeline = ConstruirPipelineComLookup(filtros, parametros);

            var resultado = Session != null
                ? await Collection.Aggregate<ProcessamentoTarefa>(Session, pipeline).ToListAsync(cancellationToken)
                : await Collection.Aggregate<ProcessamentoTarefa>(pipeline).ToListAsync(cancellationToken);

            return resultado;
        }

        private async Task<long> ContarComLookupAsync(
            FiltroPesquisaIntegracaoAssincrona filtros,
            CancellationToken cancellationToken)
        {
            var pipeline = ConstruirPipelineComLookup(filtros, null);
            pipeline.Add(new BsonDocument("$count", "total"));

            var resultado = Session != null
                ? await Collection.Aggregate<BsonDocument>(Session, pipeline).FirstOrDefaultAsync(cancellationToken)
                : await Collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync(cancellationToken);

            if (resultado == null || !resultado.Contains("total"))
                return 0;

            var totalValue = resultado["total"];
            if (totalValue.IsInt32)
                return totalValue.AsInt32;
            if (totalValue.IsInt64)
                return totalValue.AsInt64;
            
            return 0;
        }

        private FilterDefinition<ProcessamentoTarefa> ConstruirFiltroSimples(FiltroPesquisaIntegracaoAssincrona filtros)
        {
            var builder = Builders<ProcessamentoTarefa>.Filter;
            var filter = builder.Empty;

            if (filtros.StatusTarefa.HasValue)
                filter = filter & builder.Eq(t => t.Status, filtros.StatusTarefa.Value);

            if (filtros.TipoRequest.HasValue)
                filter = filter & builder.Eq(t => t.TipoRequest, filtros.TipoRequest.Value);

            if (filtros.DataInicialIntegracao.HasValue)
                filter = filter & builder.Gte(t => t.CriadoEm, filtros.DataInicialIntegracao.Value);

            if (filtros.DataFinalIntegracao.HasValue)
                filter = filter & builder.Lte(t => t.CriadoEm, filtros.DataFinalIntegracao.Value);

            if (filtros.TipoEtapaAtual.HasValue)
            {
                filter = filter & builder.Where(t => t.Etapas != null && 
                    t.Etapas.Count > t.EtapaAtual && 
                    t.Etapas[t.EtapaAtual].Tipo == filtros.TipoEtapaAtual.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtros.JobId))
                filter = filter & builder.Eq(t => t.JobId, filtros.JobId);

            return filter;
        }

        private List<BsonDocument> ConstruirPipelineComLookup(
            FiltroPesquisaIntegracaoAssincrona filtros,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros)
        {
            var pipeline = new List<BsonDocument>();

            var matchTarefa = new BsonDocument();

            if (filtros.StatusTarefa.HasValue)
                matchTarefa["PRO_STATUS"] = filtros.StatusTarefa.Value.ToString();

            if (filtros.TipoRequest.HasValue)
                matchTarefa["PRO_TIPO_REQUEST"] = filtros.TipoRequest.Value.ToString();

            if (filtros.DataInicialIntegracao.HasValue)
                matchTarefa["PRO_CRIADO_EM"] = new BsonDocument("$gte", filtros.DataInicialIntegracao.Value);

            if (filtros.DataFinalIntegracao.HasValue)
            {
                if (matchTarefa.Contains("PRO_CRIADO_EM"))
                {
                    matchTarefa["PRO_CRIADO_EM"].AsBsonDocument.Add("$lte", filtros.DataFinalIntegracao.Value);
                }
                else
                {
                    matchTarefa["PRO_CRIADO_EM"] = new BsonDocument("$lte", filtros.DataFinalIntegracao.Value);
                }
            }

            if (filtros.TipoEtapaAtual.HasValue)
            {
                var etapaAtualDoc = new BsonDocument("$arrayElemAt", new BsonArray { "$PRO_ETAPAS", "$PRO_ETAPA_ATUAL" });
                var tipoEtapaExpr = new BsonDocument("$getField", new BsonDocument
                {
                    { "field", "ETA_TIPO" },
                    { "input", etapaAtualDoc }
                });
                matchTarefa["$expr"] = new BsonDocument("$eq", new BsonArray
                {
                    tipoEtapaExpr,
                    BsonValue.Create(filtros.TipoEtapaAtual.Value.ToString())
                });
            }

            if (!string.IsNullOrWhiteSpace(filtros.JobId))
                matchTarefa["PRO_JOB_ID"] = filtros.JobId;

            if (matchTarefa.ElementCount > 0)
                pipeline.Add(new BsonDocument("$match", matchTarefa));

            pipeline.Add(new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "RequestDocumento" },
                { "localField", "PRO_REQUEST_ID" },
                { "foreignField", "_id" },
                { "as", "request_doc" }
            }));

            pipeline.Add(new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$request_doc" },
                { "preserveNullAndEmptyArrays", true }
            }));

            var matchConditions = new BsonDocument();
            var allExprConditions = new BsonArray();

            if (!string.IsNullOrWhiteSpace(filtros.NumeroPedido))
            {
                allExprConditions.Add(new BsonDocument("$regexMatch", new BsonDocument
                {
                    { "input", new BsonDocument("$ifNull", new BsonArray { "$request_doc.REQ_DADOS.NumeroPedido", "" }) },
                    { "regex", filtros.NumeroPedido },
                    { "options", "i" }
                }));

                allExprConditions.Add(new BsonDocument("$regexMatch", new BsonDocument
                {
                    { "input", new BsonDocument("$ifNull", new BsonArray { "$request_doc.REQ_DADOS.NumeroPedidoEmbarcador", "" }) },
                    { "regex", filtros.NumeroPedido },
                    { "options", "i" }
                }));

                allExprConditions.Add(new BsonDocument("$anyElementTrue", new BsonDocument("$map", new BsonDocument
                {
                    { "input", new BsonDocument("$ifNull", new BsonArray { "$request_doc.REQ_DADOS.items", new BsonArray() }) },
                    { "as", "item" },
                    { "in", new BsonDocument("$regexMatch", new BsonDocument
                        {
                            { "input", new BsonDocument("$ifNull", new BsonArray { "$$item.NumeroPedidoEmbarcador", "" }) },
                            { "regex", filtros.NumeroPedido },
                            { "options", "i" }
                        })
                    }
                })));

                allExprConditions.Add(new BsonDocument("$anyElementTrue", new BsonDocument("$map", new BsonDocument
                {
                    { "input", new BsonDocument("$ifNull", new BsonArray { "$request_doc.REQ_DADOS.Pedidos", new BsonArray() }) },
                    { "as", "pedido" },
                    { "in", new BsonDocument("$regexMatch", new BsonDocument
                        {
                            { "input", new BsonDocument("$ifNull", new BsonArray { "$$pedido.NumeroPedido", "" }) },
                            { "regex", filtros.NumeroPedido },
                            { "options", "i" }
                        })
                    }
                })));
            }

            if (!string.IsNullOrWhiteSpace(filtros.NumeroCarregamento) || filtros.NumeroCarga.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(filtros.NumeroCarregamento))
                {
                    allExprConditions.Add(new BsonDocument("$regexMatch", new BsonDocument
                    {
                        { "input", new BsonDocument("$ifNull", new BsonArray { "$request_doc.REQ_DADOS.NumeroCarregamento", "" }) },
                        { "regex", filtros.NumeroCarregamento },
                        { "options", "i" }
                    }));

                    allExprConditions.Add(new BsonDocument("$regexMatch", new BsonDocument
                    {
                        { "input", new BsonDocument("$ifNull", new BsonArray { "$request_doc.REQ_DADOS.NumeroCarga", "" }) },
                        { "regex", filtros.NumeroCarregamento },
                        { "options", "i" }
                    }));
                }

                if (filtros.NumeroCarga.HasValue)
                {
                    allExprConditions.Add(new BsonDocument("$eq", new BsonArray { "$request_doc.REQ_DADOS.NumeroCarga", filtros.NumeroCarga.Value }));
                    allExprConditions.Add(new BsonDocument("$eq", new BsonArray 
                    { 
                        new BsonDocument("$toString", new BsonDocument("$ifNull", new BsonArray { "$request_doc.REQ_DADOS.NumeroCarregamento", "" })), 
                        filtros.NumeroCarga.Value.ToString() 
                    }));
                }
            }

            if (allExprConditions.Count > 0)
            {
                matchConditions["$expr"] = new BsonDocument("$or", allExprConditions);
                pipeline.Add(new BsonDocument("$match", matchConditions));
            }

            pipeline.Add(new BsonDocument("$project", new BsonDocument
            {
                { "request_doc", 0 }
            }));

            pipeline.Add(new BsonDocument("$sort", new BsonDocument("PRO_CRIADO_EM", -1)));

            if (parametros != null)
            {
                if (parametros.InicioRegistros > 0)
                    pipeline.Add(new BsonDocument("$skip", parametros.InicioRegistros));

                if (parametros.LimiteRegistros > 0)
                    pipeline.Add(new BsonDocument("$limit", parametros.LimiteRegistros));
            }

            return pipeline;
        }
    }
}


