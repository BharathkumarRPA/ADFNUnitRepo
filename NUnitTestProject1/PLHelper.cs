using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System.Threading;
using System.Threading.Tasks;

class PLHelper
    {
    public string PipelineOutcome { get; private set; }

    public async Task RunPipeline()
    {
        PipelineOutcome = "Unknown";

        // authenticate against Azure
        var context = new AuthenticationContext("https://login.windows.net/aef6baf5-6231-4690-9308-23d674d56b05");
        var cc = new ClientCredential("1d2b3d5c-0c3d-4d99-967e-307e7d714d13", "1V4xyp5TILsy821~TVe9wdE7P44.aR6r__");
        var authResult = await context.AcquireTokenAsync("https://management.azure.com/", cc);

        // prepare ADF client
        var cred = new TokenCredentials(authResult.AccessToken);
        using (var adfClient = new DataFactoryManagementClient(cred) { SubscriptionId = "506a4cb9-cf60-489a-963d-d80cce0f8444" })
        {
            var adfName = "worldpopADF5909";  // name of data factory
            var rgName = "Training";    // name of resource group that contains the data factory

            // run pipeline
            var response = await adfClient.Pipelines.CreateRunWithHttpMessagesAsync(rgName, adfName, "SQLtoBlob");
            string runId = response.Body.RunId;

            // wait for pipeline to finish
            var run = await adfClient.PipelineRuns.GetAsync(rgName, adfName, runId);
            while (run.Status == "Queued" || run.Status == "InProgress" || run.Status == "Canceling")
            {
                Thread.Sleep(2000);
                run = await adfClient.PipelineRuns.GetAsync(rgName, adfName, runId);
            }
            PipelineOutcome = run.Status;
        }
    }

    public PLHelper()
    {
        PipelineOutcome = "Unknown";
    }

}

