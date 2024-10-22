namespace BoletoNetCore
{
	internal class ImpressaoBoletoCaixa : IImpressaoBoleto<BancoCaixa>
	{
		public void ConfigurarImpressaoBoleto(Boleto boleto)
		{
			boleto.ImprimirMensagemInstrucao = true;
			boleto.ImprimirValoresAuxiliares = false;
		}
	}
}
