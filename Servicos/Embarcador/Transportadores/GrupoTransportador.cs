using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Transportadores
{
    public class GrupoTransportador
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly System.Threading.CancellationToken _cancellationToken;

        #endregion Atributos

        #region Construtores

        public GrupoTransportador(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, default) { }
        public GrupoTransportador(Repositorio.UnitOfWork unitOfWork, System.Threading.CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancellationToken;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void DeletarGruposTransportadoresIntegracoes(Repositorio.Embarcador.Transportadores.GrupoTransportadorIntegracao repositorioGrupoTransportadorIntegracao, List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes, List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao> integracoesDeletar)
        {

            foreach (Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao IntegracaoDeletar in integracoesDeletar)
            {
                if (alteracoes != null)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Integração",
                        De = IntegracaoDeletar.Tipo.ObterDescricao(),
                        Para = ""
                    });
                }

                repositorioGrupoTransportadorIntegracao.Deletar(IntegracaoDeletar);
            }
        }

        public async Task SalvarIntegracoesGrupoTransportador(Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador grupoTransportador, dynamic integracoesGrid)
        {
            Repositorio.Embarcador.Transportadores.GrupoTransportadorIntegracao repositorioGrupoTransportadorIntegracao = new(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao> integracoes = await repositorioGrupoTransportadorIntegracao.BuscarIntegracoesPorGrupoTransportadorAsync(grupoTransportador.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (integracoes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic integracao in integracoesGrid)
                    if (integracao.Codigo != null)
                        codigos.Add(((string)integracao.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao> integracoesDeletar = (from gti in integracoes where !codigos.Contains(gti.Codigo) select gti).ToList();

                DeletarGruposTransportadoresIntegracoes(repositorioGrupoTransportadorIntegracao, alteracoes, integracoesDeletar);
            }

            foreach (dynamic integracao in integracoesGrid)
            {
                int codigoIntegracao = ((string)integracao.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao integracaoAdicionar = codigoIntegracao > 0 ? integracoes.Find(gti => gti.Codigo == codigoIntegracao) : null;

                if (integracaoAdicionar == null)
                {
                    integracaoAdicionar = new Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao()
                    {
                        GrupoTransportador = grupoTransportador,
                        Tipo = (TipoIntegracao)integracao.Tipo
                    };
                    await repositorioGrupoTransportadorIntegracao.InserirAsync(integracaoAdicionar);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Integração",
                        De = "",
                        Para = integracaoAdicionar.Tipo.ObterDescricao()
                    });
                }
            }

            grupoTransportador.SetExternalChanges(alteracoes);
        }

        #endregion Métodos Públicos
    }
}
