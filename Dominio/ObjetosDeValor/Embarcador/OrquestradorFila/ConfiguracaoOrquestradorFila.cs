namespace Dominio.ObjetosDeValor.Embarcador.OrquestradorFila
{
    public class ConfiguracaoOrquestradorFila
    {
        public virtual int Codigo { get; set; }

        public virtual Enumeradores.IdentificadorControlePosicaoThread Identificador { get; set; }

        public virtual int QuantidadeRegistrosConsulta { get; set; }

        public virtual int QuantidadeRegistrosRetorno { get; set; }

        public virtual bool TratarRegistrosComFalha { get; set; }

        public virtual int LimiteTentativas { get; set; }
    }
}
