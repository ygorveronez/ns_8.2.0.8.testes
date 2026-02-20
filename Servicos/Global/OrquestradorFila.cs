using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Global
{
    public sealed class OrquestradorFila
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly IdentificadorControlePosicaoThread _identificador;

        #endregion Atributos

        #region Construtores

        public OrquestradorFila(Repositorio.UnitOfWork unitOfWork, IdentificadorControlePosicaoThread identificador)
        {
            _unitOfWork = unitOfWork;
            _identificador = identificador;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<int> Ordenar(Func<int, List<int>> consultaRegistros)
        {
            Repositorio.ControleFalhaThread repositorioControleFalhaThread = new Repositorio.ControleFalhaThread(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = Servicos.Global.OrquestradorFilaConfiguracao.GetInstance().ObterConfiguracao(_unitOfWork, _identificador);

            int quantidadeRegistros = configuracaoOrquestradorFila.TratarRegistrosComFalha ? configuracaoOrquestradorFila.QuantidadeRegistrosConsulta : configuracaoOrquestradorFila.QuantidadeRegistrosRetorno;
            List<int> codigos = consultaRegistros(quantidadeRegistros);

            if (!configuracaoOrquestradorFila.TratarRegistrosComFalha || (codigos.Count <= 0))
                return codigos;

            List<(int CodigoEntidade, int NumeroTentativas)> registrosComFalha = repositorioControleFalhaThread.BuscarRegistrosComFalhaPorCodigosEntidades(codigos, _identificador);

            if (registrosComFalha.Count <= 0)
                return codigos;

            List<int> codigosNovaTentativa = new List<int>();

            foreach ((int CodigoEntidade, int NumeroTentativas) registroComFalha in registrosComFalha)
            {
                codigos.Remove(registroComFalha.CodigoEntidade);

                if ((configuracaoOrquestradorFila.LimiteTentativas <= 0) || (configuracaoOrquestradorFila.LimiteTentativas > registroComFalha.NumeroTentativas))
                    codigosNovaTentativa.Add(registroComFalha.CodigoEntidade);
            }

            codigos.AddRange(codigosNovaTentativa);

            return codigos.Take(configuracaoOrquestradorFila.QuantidadeRegistrosRetorno).ToList();
        }

        public List<(int Codigo, int EmpresaCodigo)> Ordenar(Func<int, List<(int Codigo, int EmpresaCodigo)>> consultaRegistros)
        {
            Repositorio.ControleFalhaThread repositorioControleFalhaThread = new Repositorio.ControleFalhaThread(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = Servicos.Global.OrquestradorFilaConfiguracao.GetInstance().ObterConfiguracao(_unitOfWork, _identificador);

            int quantidadeRegistros = configuracaoOrquestradorFila.TratarRegistrosComFalha ? configuracaoOrquestradorFila.QuantidadeRegistrosConsulta : configuracaoOrquestradorFila.QuantidadeRegistrosRetorno;
            List<(int Codigo, int EmpresaCodigo)> registros = consultaRegistros(quantidadeRegistros);

            if (!configuracaoOrquestradorFila.TratarRegistrosComFalha || (registros.Count <= 0))
                return registros;

            List<int> codigos = registros.Select(r => r.Codigo).ToList();
            List<(int CodigoEntidade, int NumeroTentativas)> registrosComFalha = repositorioControleFalhaThread.BuscarRegistrosComFalhaPorCodigosEntidades(codigos, _identificador);

            if (registrosComFalha.Count <= 0)
                return registros;

            List<(int Codigo, int EmpresaCodigo)> registrosNovaTentativa = new List<(int Codigo, int EmpresaCodigo)>();

            foreach ((int CodigoEntidade, int NumeroTentativas) registroComFalha in registrosComFalha)
            {
                var registroRemover = registros.FirstOrDefault(r => r.Codigo == registroComFalha.CodigoEntidade);
                registros.Remove(registroRemover);

                if ((configuracaoOrquestradorFila.LimiteTentativas <= 0) || (configuracaoOrquestradorFila.LimiteTentativas > registroComFalha.NumeroTentativas))
                    registrosNovaTentativa.Add(registroRemover);
            }

            registros.AddRange(registrosNovaTentativa);

            return registros.Take(configuracaoOrquestradorFila.QuantidadeRegistrosRetorno).ToList();
        }

        public void RegistroLiberadoComSucesso(int codigoEntidade)
        {
            Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = Servicos.Global.OrquestradorFilaConfiguracao.GetInstance().ObterConfiguracao(_unitOfWork, _identificador);

            if (!configuracaoOrquestradorFila.TratarRegistrosComFalha)
                return;

            Repositorio.ControleFalhaThread repositorioControleFalhaThread = new Repositorio.ControleFalhaThread(_unitOfWork);
            Dominio.Entidades.ControleFalhaThread controleFalhaThread = repositorioControleFalhaThread.BuscarComFalhaPorCodigoEntidadeComIdentificador(codigoEntidade, _identificador);

            if (controleFalhaThread == null)
                return;

            controleFalhaThread.DataUltimaTentativa = DateTime.Now;
            controleFalhaThread.Log = "Liberado com Sucesso!";
            controleFalhaThread.NumeroTentativas++;
            controleFalhaThread.NumeroTentativasTotais++;
            controleFalhaThread.RegistroComFalha = false;

            repositorioControleFalhaThread.Atualizar(controleFalhaThread);
        }

        public void RegistroComFalha(int codigoEntidade, string log)
        {
            Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = Servicos.Global.OrquestradorFilaConfiguracao.GetInstance().ObterConfiguracao(_unitOfWork, _identificador);

            if (!configuracaoOrquestradorFila.TratarRegistrosComFalha)
                return;

            Repositorio.ControleFalhaThread repositorioControleFalhaThread = new Repositorio.ControleFalhaThread(_unitOfWork);
            Dominio.Entidades.ControleFalhaThread controleFalhaThread = repositorioControleFalhaThread.BuscarComFalhaPorCodigoEntidadeComIdentificador(codigoEntidade, _identificador);

            if (controleFalhaThread == null)
                controleFalhaThread = new Dominio.Entidades.ControleFalhaThread()
                {
                    CodigoEntidade = codigoEntidade,
                    Identificador = _identificador,
                };

            controleFalhaThread.DataUltimaTentativa = DateTime.Now;
            controleFalhaThread.Log = log.Left(1999);
            controleFalhaThread.NumeroTentativas++;
            controleFalhaThread.NumeroTentativasTotais++;
            controleFalhaThread.RegistroComFalha = true;

            if (controleFalhaThread.Codigo > 0)
                repositorioControleFalhaThread.Atualizar(controleFalhaThread);
            else
                repositorioControleFalhaThread.Inserir(controleFalhaThread);
        }

        #endregion Métodos Públicos
    }
}
