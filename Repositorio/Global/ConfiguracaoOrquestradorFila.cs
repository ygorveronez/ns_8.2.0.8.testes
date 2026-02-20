using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ConfiguracaoOrquestradorFila : RepositorioBase<Dominio.Entidades.ConfiguracaoOrquestradorFila>
    {
        #region Construtores

        public ConfiguracaoOrquestradorFila(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoOrquestradorFila(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.ConfiguracaoOrquestradorFila> Consultar(IdentificadorControlePosicaoThread? identificador, bool? tratarRegistrosComFalha)
        {
            var consultaConfiguracaoOrquestradorFila = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoOrquestradorFila>();

            if (identificador.HasValue)
                consultaConfiguracaoOrquestradorFila = consultaConfiguracaoOrquestradorFila.Where(configuracao => configuracao.Identificador == identificador.Value);

            if (tratarRegistrosComFalha.HasValue)
                consultaConfiguracaoOrquestradorFila = consultaConfiguracaoOrquestradorFila.Where(configuracao => configuracao.TratarRegistrosComFalha == tratarRegistrosComFalha.Value);

            return consultaConfiguracaoOrquestradorFila;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila> Buscar()
        {
            var consultaConfiguracaoOrquestradorFila = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoOrquestradorFila>();

            return consultaConfiguracaoOrquestradorFila.Select(configuracaoOrquestradorFila => new Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila()
            {
                Codigo = configuracaoOrquestradorFila.Codigo,
                Identificador = configuracaoOrquestradorFila.Identificador,
                QuantidadeRegistrosConsulta = configuracaoOrquestradorFila.QuantidadeRegistrosConsulta,
                QuantidadeRegistrosRetorno = configuracaoOrquestradorFila.QuantidadeRegistrosRetorno,
                TratarRegistrosComFalha = configuracaoOrquestradorFila.TratarRegistrosComFalha,
                LimiteTentativas = configuracaoOrquestradorFila.LimiteTentativas
            }).ToList();
        }

        public Task<List<Dominio.Entidades.ConfiguracaoOrquestradorFila>> ConsultarAsync(IdentificadorControlePosicaoThread? identificador, bool? tratarRegistrosComFalha, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaConfiguracaoOrquestradorFila = Consultar(identificador, tratarRegistrosComFalha);

            return ObterListaAsync(consultaConfiguracaoOrquestradorFila, parametroConsulta);
        }

        public Task<int> ContarConsultaAsync(IdentificadorControlePosicaoThread? identificador, bool? tratarRegistrosComFalha)
        {
            var consultaConfiguracaoOrquestradorFila = Consultar(identificador, tratarRegistrosComFalha);

            return consultaConfiguracaoOrquestradorFila.CountAsync(CancellationToken);
        }

        #endregion Métodos Públicos
    }
}
