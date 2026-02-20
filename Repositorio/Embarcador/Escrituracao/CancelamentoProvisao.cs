using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using Dominio.Excecoes.Embarcador;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Escrituracao;
using Repositorio.Embarcador.Consulta;
using System;

namespace Repositorio.Embarcador.Escrituracao
{
    public class CancelamentoProvisao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>
    {

        public CancelamentoProvisao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CancelamentoProvisao(UnitOfWork unitOfWork, CancellationToken cancelationToken) : base(unitOfWork, cancelationToken) { }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao BuscarSeExisteProvisaoEmCancelamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmCancelamento || obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.PendenciaCancelamento select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao> BuscarProvisaoEmCancelamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmCancelamento && obj.GerandoMovimentoFinanceiro select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao> BuscarCancelamentoProvisaoAguardandoAprovacao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgAprovacaoSolicitacao && obj.GerandoMovimentoFinanceiro select obj;

            return result.ToList();
        }


        public List<int> BuscarCancelamentoProvisaoAgIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao select obj;

            return result.Select(x => x.Codigo).ToList();
        }

        public int ObterProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>();

            var result = from obj in query select obj;

            int? retorno = result.Max(o => (int?)o.Numero);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        public List<int> BuscarCodigosFalhaIntegracao()
        {
            FiltroPesquisaCancelamentoProvisao filtro = new()
            {
                Situacao = SituacaoCancelamentoProvisao.FalhaIntegracao
            };

            return [.. _Consultar(filtro).Select(cancelamentoProvisao => cancelamentoProvisao.Codigo)];
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao> _Consultar(FiltroPesquisaCancelamentoProvisao filtroPesquisaCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao>();

            var result = from obj in query select obj;

            if (filtroPesquisaCancelamentoProvisao.DataInicio.HasValue)
                result = result.Where(obj => obj.DataInicial.Value.Date >= filtroPesquisaCancelamentoProvisao.DataInicio.Value.Date);

            if (filtroPesquisaCancelamentoProvisao.DataFinal.HasValue)
                result = result.Where(obj => obj.DataFinal.Value.Date <= filtroPesquisaCancelamentoProvisao.DataFinal.Value.Date);

            if (filtroPesquisaCancelamentoProvisao.Transportador > 0)
                result = result.Where(o => o.Empresa.Codigo == filtroPesquisaCancelamentoProvisao.Transportador);

            if (filtroPesquisaCancelamentoProvisao.Carga > 0)
                result = result.Where(o => o.DocumentosProvisao.Any(doc => doc.Carga.Codigo == filtroPesquisaCancelamentoProvisao.Carga));

            if (filtroPesquisaCancelamentoProvisao.NumeroDoc > 0)
                result = result.Where(o => o.DocumentosProvisao.Any(doc => doc.NumeroDocumento == filtroPesquisaCancelamentoProvisao.NumeroDoc));

            if (filtroPesquisaCancelamentoProvisao.Ocorrencia > 0)
                result = result.Where(o => o.DocumentosProvisao.Any(doc => doc.CargaOcorrencia.Codigo == filtroPesquisaCancelamentoProvisao.Ocorrencia));

            if (filtroPesquisaCancelamentoProvisao.Filial > 0)
                result = result.Where(o => o.Filial.Codigo == filtroPesquisaCancelamentoProvisao.Filial);

            if (filtroPesquisaCancelamentoProvisao.Numero > 0)
                result = result.Where(o => o.Numero == filtroPesquisaCancelamentoProvisao.Numero);


            if (filtroPesquisaCancelamentoProvisao.Tomador > 0)
                result = result.Where(o => o.Tomador.CPF_CNPJ == filtroPesquisaCancelamentoProvisao.Tomador);


            if (!string.IsNullOrEmpty(filtroPesquisaCancelamentoProvisao.NumeroFolha))
                result = result.Where(o => o.DocumentosProvisao.Any(x => x.Stage.NumeroFolha == filtroPesquisaCancelamentoProvisao.NumeroFolha) );

            if (filtroPesquisaCancelamentoProvisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.Todos)
                result = result.Where(o => o.Situacao == filtroPesquisaCancelamentoProvisao.Situacao);

            if(filtroPesquisaCancelamentoProvisao.CancelamentoProvisaoContraPartida.HasValue)
                result = result.Where(o => o.CancelamentoProvisaoContraPartida == filtroPesquisaCancelamentoProvisao.CancelamentoProvisaoContraPartida.Value);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao> Consultar(FiltroPesquisaCancelamentoProvisao filtroPesquisaCancelamentoProvisao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {

            var result = _Consultar(filtroPesquisaCancelamentoProvisao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(FiltroPesquisaCancelamentoProvisao filtroPesquisaCancelamentoProvisao)
        {
            var result = _Consultar(filtroPesquisaCancelamentoProvisao);

            return result.Count();
        }

        public async Task<int> AtualizarSituacaoAsync(
            IEnumerable<int> codigosCancelamentoProvisao,
            SituacaoCancelamentoProvisao situacao
        )
        {
            const int maxParameters = 2000;
            const int maxBatchSize = maxParameters - 1;

            var listaCodigos = codigosCancelamentoProvisao.ToList();
            int succeeded = 0;

            for (int i = 0; i < listaCodigos.Count; i += maxBatchSize)
            {
                var batch = listaCodigos.Skip(i).Take(maxBatchSize).ToArray();

                succeeded += await UnitOfWork.Sessao.CreateSQLQuery(@"
                    UPDATE 
                        T_PROVISAO_CANCELAMENTO
                    SET
                        CPV_SITUACAO = :situacao
                    WHERE
                        CPV_CODIGO IN (:codigosCancelamentoProvisao);
                ")
                .SetEnum("situacao", situacao)
                .SetParameterList("codigosCancelamentoProvisao", batch)
                .ExecuteUpdateAsync(CancellationToken);
            }

            return succeeded;
        }
    }
}
