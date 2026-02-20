using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.Database;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.ProcessadorTarefas.Etapas
{
    public class FecharCarga : EtapaState
    {
        private readonly ITenantService _tenantService;
        private readonly IProcessamentoTarefaRepository _repositorioTarefa;

        public FecharCarga(ITenantService tenantService, IProcessamentoTarefaRepository repositorioTarefa)
        {
            _tenantService = tenantService;
            _repositorioTarefa = repositorioTarefa;
        }

        public override async Task ExecutarAsync(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            var resultadosDoc = contexto.Tarefa.Resultado ?? new BsonDocument();
            string protocoloCarga = null;
            if (resultadosDoc.Contains("cargas") && resultadosDoc["cargas"].IsBsonArray)
            {
                var cargas = resultadosDoc["cargas"].AsBsonArray;
                var primeiraComProtocolo = cargas.FirstOrDefault(c =>
                    c.IsBsonDocument &&
                    c.AsBsonDocument.Contains("protocolo") &&
                    !string.IsNullOrEmpty(c.AsBsonDocument["protocolo"].ToString()));

                if (primeiraComProtocolo != null)
                {
                    protocoloCarga = primeiraComProtocolo.AsBsonDocument["protocolo"].ToString();
                }
            }

            if (string.IsNullOrEmpty(protocoloCarga))
            {
                return;
            }

            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_tenantService.StringConexao(), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Embarcador.Carga.FecharCarga servicoFecharCarga = new Embarcador.Carga.FecharCarga(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorProtocoloAsync(protocoloCarga.ToInt());

                if (carga != null)
                {
                    await servicoFecharCarga.FecharCargaAsync(carga, _tenantService.ObterTipoServicoMultisoftware(), _tenantService.ObterCliente(), true, viaWSRest: true, cancellationToken: cancellationToken);

                    carga.CargaFechada = true;
                    await repositorioCarga.AtualizarAsync(carga);

                    resultadosDoc["carga_fechada"] = true;
                    resultadosDoc["protocolo_carga_fechada"] = protocoloCarga;

                    var update = Builders<ProcessamentoTarefa>.Update
                        .Set(t => t.Resultado, resultadosDoc);
                    await _repositorioTarefa.AtualizarAsync(contexto.TarefaId, update, cancellationToken);
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "FecharCarga");

                resultadosDoc["carga_fechada"] = false;
                resultadosDoc["erro_fechar_carga"] = excecao.Message;

                var update = Builders<ProcessamentoTarefa>.Update
                    .Set(t => t.Resultado, resultadosDoc);
                await _repositorioTarefa.AtualizarAsync(contexto.TarefaId, update, cancellationToken);

                throw;
            }
        }
    }
}