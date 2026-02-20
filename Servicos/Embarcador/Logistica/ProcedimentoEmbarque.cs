using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica
{
    public class ProcedimentoEmbarque
    {
        public static void SetarProcedimentoEmbarqueCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque procedimentoEmbarque = ObterProcedimentoEmbarqueCarga(carga.TipoOperacao, carga.Filial, unitOfWork);
            if (procedimentoEmbarque != null)
                carga.ProcedimentoEmbarque = procedimentoEmbarque;
            else
                carga.ProcedimentoEmbarque = null;

            if (carga.ProcedimentoEmbarque != null && !string.IsNullOrWhiteSpace(carga.IntegracaoTemperatura) && (carga.FaixaTemperatura == null || carga.FaixaTemperatura.ProcedimentoEmbarque?.Codigo != procedimentoEmbarque.Codigo))
            {
                Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                carga.FaixaTemperatura = repFaixaTemperatura.BuscarPorDescricaoEProcedimento(carga.IntegracaoTemperatura, procedimentoEmbarque.Codigo);
            }
        }
        
        public async static Task SetarProcedimentoEmbarqueCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque procedimentoEmbarque = ObterProcedimentoEmbarqueCarga(carga.TipoOperacao, carga.Filial, unitOfWork);
            if (procedimentoEmbarque != null)
                carga.ProcedimentoEmbarque = procedimentoEmbarque;
            else
                carga.ProcedimentoEmbarque = null;

            if (carga.ProcedimentoEmbarque != null && !string.IsNullOrWhiteSpace(carga.IntegracaoTemperatura) && (carga.FaixaTemperatura == null || carga.FaixaTemperatura.ProcedimentoEmbarque?.Codigo != procedimentoEmbarque.Codigo))
            {
                Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                carga.FaixaTemperatura = repFaixaTemperatura.BuscarPorDescricaoEProcedimento(carga.IntegracaoTemperatura, procedimentoEmbarque.Codigo);
            }
        }

        public static Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque ObterProcedimentoEmbarqueCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque> procedimentoEmbarques = repProcedimentoEmbarque.BuscarTodosAtivos();

            Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque procedimentoEmbarque = null;
            if (procedimentoEmbarques.Count > 0)
            {
                procedimentoEmbarque = (from obj in procedimentoEmbarques where obj.TipoOperacao?.Codigo == tipoOperacao?.Codigo && obj.Filial?.Codigo == filial?.Codigo select obj).FirstOrDefault();
                if (procedimentoEmbarque == null)
                {
                    if (filial != null)
                        procedimentoEmbarque = (from obj in procedimentoEmbarques where obj.Filial?.Codigo == filial.Codigo select obj).FirstOrDefault();

                    if (procedimentoEmbarque == null)
                    {
                        if (tipoOperacao != null)
                            procedimentoEmbarque = (from obj in procedimentoEmbarques where obj.TipoOperacao?.Codigo == tipoOperacao.Codigo select obj).FirstOrDefault();
                    }
                }
            }
            return procedimentoEmbarque;
        }
    }
}
