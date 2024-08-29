using MernisService;

namespace TurkiyeFinans.Models
{
    public class ServiceKPSPublic
    {
        public async Task<bool> OnGetService(MernisServiceParametters parametters)
        {
            bool resault=false;
            var client = new MernisService.KPSPublicSoapClient(KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);
            var response = await client.TCKimlikNoDogrulaAsync(parametters.TCKimlikNo,parametters.Ad,parametters.Soyad,parametters.DogumYili);
            return resault = response.Body.TCKimlikNoDogrulaResult;
        }
    }
}
