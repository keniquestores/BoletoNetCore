namespace BoletoNetCore
{
	internal interface IImpressaoBoleto<T> where T : IBanco
	{
		void ConfigurarImpressaoBoleto(Boleto boleto);
	}
}
