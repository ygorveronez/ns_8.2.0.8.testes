using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.Database;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.ProcessadorTarefas.Etapas
{
    public class GerarCarregamento : EtapaState
    {
        private readonly ITenantService _tenantService;
        private readonly IProcessamentoTarefaRepository _repositorioTarefa;

        public GerarCarregamento(ITenantService tenantService, IProcessamentoTarefaRepository repositorioTarefa)
        {
            _tenantService = tenantService;
            _repositorioTarefa = repositorioTarefa;
        }

        public override async Task ExecutarAsync(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            var carregamento = ObterRequest<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>(contexto);

            var resultadosDoc = contexto.Tarefa.Resultado ?? new BsonDocument();
            var cargasArray = resultadosDoc.Contains("cargas")
                ? resultadosDoc["cargas"].AsBsonArray
                : new BsonArray();

            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(
                    _tenantService.StringConexao(),
                    Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

                Dominio.ObjetosDeValor.WebService.Retorno<int> retorno = await new WebService.Carga.Carga(
                    unitOfWork,
                    _tenantService.ObterTipoServicoMultisoftware(),
                    _tenantService.ObterCliente(),
                    _tenantService.ObterAuditado(Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos),
                    _tenantService.AdminStringConexao()
                ).GerarCarregamentoNovoAsync(carregamento, cancellationToken, wsRest: true);

                bool sucesso = retorno.CodigoMensagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;

                var cargaDoc = new BsonDocument
                {
                    { "status", sucesso },
                    { "mensagem", retorno.Mensagem ?? "" },
                    { "protocolo", sucesso ? retorno.Objeto.ToString() : "" }
                };
                cargasArray.Add(cargaDoc);

                resultadosDoc["cargas"] = cargasArray;

                var update = Builders<ProcessamentoTarefa>.Update
                    .Set(t => t.Resultado, resultadosDoc);
                await _repositorioTarefa.AtualizarAsync(contexto.TarefaId, update, cancellationToken);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "GerarCarregamento");

                var cargaDoc = new BsonDocument
                {
                    { "status", false },
                    { "mensagem", "Ocorreu um erro ao gerar carregamento." },
                    { "erro", excecao.Message }
                };
                cargasArray.Add(cargaDoc);
                resultadosDoc["cargas"] = cargasArray;

                var update = Builders<ProcessamentoTarefa>.Update
                    .Set(t => t.Resultado, resultadosDoc);
                await _repositorioTarefa.AtualizarAsync(contexto.TarefaId, update, cancellationToken);

                throw;
            }
        }
    }
}
