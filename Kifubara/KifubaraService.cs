using Kifubara.Models;

namespace Kifubara;

public class KifubaraService(IKifubaraClient kifubaraClient)
{
    public async Task<string> SendSgf(string nextGameSgf)
    {
        var res = await kifubaraClient.AnalyzeAsync(
            new AnalyzeGameRequest()
            {
                Sgf = nextGameSgf,
            });

        return res.ShareUrl;
    }
}